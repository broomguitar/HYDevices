using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HY.Devices.Camera.Utils
{
    class BmpImageUtil
    {
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

        bool bCompletion = false;
        ColorPalette m_palette = null;
        public Bitmap m_bitmap = null;

        public bool CreateImage(int nWidth, int nHeight, int nDepth, int nChannels)
        {
            System.Drawing.Imaging.PixelFormat nPixelFromat = System.Drawing.Imaging.PixelFormat.Undefined;
            // Get pixel format
            switch (nChannels)
            {
                case 1:
                    {
                        if (nDepth == 8)
                            nPixelFromat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                        else
                            nPixelFromat = System.Drawing.Imaging.PixelFormat.Format16bppGrayScale;
                    }
                    break;
                case 3:
                    {
                        if (nDepth == 8)
                            nPixelFromat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                        else
                            nPixelFromat = System.Drawing.Imaging.PixelFormat.Format48bppRgb;
                    }
                    break;
                case 4:
                    {
                        if (nDepth == 8)
                            nPixelFromat = System.Drawing.Imaging.PixelFormat.Format32bppPArgb;
                        else
                            nPixelFromat = System.Drawing.Imaging.PixelFormat.Format64bppArgb;
                    }
                    break;
            }

            m_bitmap = new Bitmap(nWidth, nHeight, nPixelFromat);

            // Modify the index table that generates the bitmap
            if (nPixelFromat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                m_palette = m_bitmap.Palette;
                for (int i = 0; i < 256; i++)
                    m_palette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                m_bitmap.Palette = m_palette;
            }
            // 
            int nStep = nChannels * nWidth * nDepth / 8;
            if (nStep % 4 == 0)
                bCompletion = false;
            else
                bCompletion = true;

            return true;
        }
        public bool ReleaseImage()
        {
            if (m_bitmap != null)
                m_bitmap.Dispose();
            m_bitmap = null;
            return true;
        }

        // Write image data
        public bool WriteImageData(IntPtr imageData, int imageDataLen)
        {
            if (m_bitmap == null)
                return false;

            // bitmap data
            Rectangle rect = new Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height);
            BitmapData bitmapData = m_bitmap.LockBits(rect, ImageLockMode.ReadWrite, m_bitmap.PixelFormat);

            int nStribe = imageDataLen / m_bitmap.Height;
            if (!bCompletion)
            {
                IntPtr iptr = bitmapData.Scan0;
                CopyMemory(iptr, imageData, imageDataLen);
            }
            else
            {
                for (int i = 0; i < m_bitmap.Height; i++)
                {
                    IntPtr iptrDst = bitmapData.Scan0 + bitmapData.Stride * i;
                    IntPtr iptrSrc = imageData + nStribe * i;
                    CopyMemory(iptrDst, iptrSrc, nStribe);
                }
            }
            m_bitmap.UnlockBits(bitmapData);

            return true;
        }

        // Save image
        public bool SaveImage(string fileName)
        {
            Rectangle rect = new Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height);

            BitmapData bitmapData = m_bitmap.LockBits(rect, ImageLockMode.ReadOnly, m_bitmap.PixelFormat);
            System.Windows.Media.PixelFormat pixelFormats = ConvertBmpPixelFormat(m_bitmap.PixelFormat);

            //Create Bitmap Source
            BitmapSource source = BitmapSource.Create(m_bitmap.Width,
                                                      m_bitmap.Height,
                                                      m_bitmap.HorizontalResolution,
                                                      m_bitmap.VerticalResolution,
                                                      pixelFormats,
                                                      null,
                                                      bitmapData.Scan0,
                                                      bitmapData.Stride * m_bitmap.Height,
                                                      bitmapData.Stride);

            m_bitmap.UnlockBits(bitmapData);

            //new File Stream
            FileStream stream = new FileStream(fileName, FileMode.Create);

            //save Tiff Bitmap 
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Compression = TiffCompressOption.Zip;
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);
            stream.Close();
            return true;
        }

        //Convert Bmp Pixel Format to Media Pixel Format
        public System.Windows.Media.PixelFormat ConvertBmpPixelFormat(System.Drawing.Imaging.PixelFormat pixelformat)
        {
            System.Windows.Media.PixelFormat pixelFormats = System.Windows.Media.PixelFormats.Default;

            switch (pixelformat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    pixelFormats = PixelFormats.Bgra32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                    pixelFormats = PixelFormats.Rgba64;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    pixelFormats = PixelFormats.Rgb24;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    pixelFormats = PixelFormats.Rgb48;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormats = PixelFormats.Gray8;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    pixelFormats = PixelFormats.Gray16;
                    break;
            }
            return pixelFormats;
        }

    }
    class BmpImageUtil_New
    {
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        // 缓冲区句柄
        IntPtr m_pBuffer = new IntPtr(-1);
        // 缓冲区锁
        object m_mutexBuffer = new object();
        // Bitmap锁
        object m_mutexBitmap = new object();
        // 缓冲区大小
        int m_nBufferSize = -1;
        // 像素位数
        int m_nDepth = -1;
        // 图像通道数
        int m_nChannels = -1;
        // 图像宽度
        int m_nWidth = -1;
        // 图像高度
        int m_nHeight = -1;
        bool bCompletion = true;
        ColorPalette m_palette = null;
        public Bitmap m_bitmap = null;

        public bool CreateImage(int nWidth, int nHeight, int nSize, int nDepth, int nChannels)
        {
            m_pBuffer = Marshal.AllocHGlobal(nSize);
            m_nBufferSize = nSize;
            m_nDepth = nDepth;
            m_nChannels = nChannels;
            m_nWidth = nWidth;
            m_nHeight = nHeight;
            System.Drawing.Imaging.PixelFormat nPixelFormat = System.Drawing.Imaging.PixelFormat.Undefined;
            switch (nChannels)
            {
                case 1:
                    nPixelFormat = System.Drawing.Imaging.PixelFormat.Format8bppIndexed;
                    break;
                case 3:
                case 4:
                    nPixelFormat = System.Drawing.Imaging.PixelFormat.Format24bppRgb;
                    break;
            }
            m_bitmap = new Bitmap(nWidth, nHeight, nPixelFormat);

            // Modify the index table that generates the bitmap
            if (nPixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
            {
                m_palette = m_bitmap.Palette;
                for (int i = 0; i < 256; i++)
                    m_palette.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                m_bitmap.Palette = m_palette;
            }
            return true;
        }
        public bool ReleaseImage()
        {
            if (m_pBuffer != new IntPtr(-1))
                Marshal.FreeHGlobal(m_pBuffer);
            if (m_bitmap != null)
                m_bitmap.Dispose();
            m_bitmap = null;
            return true;
        }
        public bool WriteImageData(IntPtr pSrc)
        {
            if (m_bitmap == null)
                return false;
            lock (m_mutexBitmap)
            {
                lock (m_mutexBuffer)
                {
                    CopyMemory(m_pBuffer, pSrc, m_nBufferSize);
                    if (m_nChannels == 4)
                    {
                        readRGBC();
                        return true;
                    }
                    Rectangle rect = new Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height);
                    BitmapData bitmapData = m_bitmap.LockBits(rect, ImageLockMode.ReadWrite, m_bitmap.PixelFormat);
                    int nShift = m_nDepth - 8;
                    int nStride = m_nBufferSize / m_bitmap.Height;
                    // 8bit 像素直接拷贝，高于8位的去除低位
                    if (m_nDepth == 8)
                    {
                        if (m_nChannels == 3)
                        {
                            byte[] pBufForSaveImage = new byte[m_nBufferSize];
                            Marshal.Copy(pSrc, pBufForSaveImage, 0, pBufForSaveImage.Length);
                            for (int i = 0; i < (int)m_nHeight; i++)
                            {
                                for (int j = 0; j < (int)m_nWidth; j++)
                                {
                                    byte chRed = pBufForSaveImage[(i * (int)m_nWidth + j) * 3];
                                    pBufForSaveImage[(i * (int)m_nWidth + j) * 3] = pBufForSaveImage[(i * (int)m_nWidth + j) * 3 + 2];
                                    pBufForSaveImage[(i * (int)m_nWidth + j) * 3 + 2] = chRed;
                                }
                            }
                            Marshal.Copy(pBufForSaveImage, 0, bitmapData.Scan0, m_nBufferSize);
                        }
                        else
                        {
                            for (int i = 0; i < m_bitmap.Height; i++)
                            {
                                IntPtr iptrDst = bitmapData.Scan0 + bitmapData.Stride * i;
                                IntPtr iptrSrc = m_pBuffer + nStride * i;
                                CopyMemory(iptrDst, iptrSrc, nStride);
                            }
                        }
                        m_bitmap.UnlockBits(bitmapData);
                    }
                    else
                    {
                        short[] pData = new short[m_nBufferSize / 2];
                        byte[] pDstData = new byte[m_nBufferSize];
                        nStride = bitmapData.Stride;
                        Marshal.Copy(m_pBuffer, pData, 0, m_nBufferSize / 2);
                        for (int i = 0; i < bitmapData.Height; i++)
                        {
                            for (int j = 0; j < nStride; j++)
                            {
                                pDstData[i * nStride + j] = (byte)(pData[i * nStride + j] >> nShift);
                            }
                        }
                        Marshal.Copy(pDstData, 0, bitmapData.Scan0, m_nBufferSize / 2);
                        m_bitmap.UnlockBits(bitmapData);
                    }
                }
            }
            return true;
        }
        public bool SaveImage(string fileName)
        {
            Rectangle rect = new Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height);

            BitmapData bitmapData = m_bitmap.LockBits(rect, ImageLockMode.ReadOnly, m_bitmap.PixelFormat);
            System.Windows.Media.PixelFormat pixelFormats = ConvertBmpPixelFormat(m_bitmap.PixelFormat);

            //Create Bitmap Source
            BitmapSource source = BitmapSource.Create(m_bitmap.Width,
                                                      m_bitmap.Height,
                                                      m_bitmap.HorizontalResolution,
                                                      m_bitmap.VerticalResolution,
                                                      pixelFormats,
                                                      null,
                                                      bitmapData.Scan0,
                                                      bitmapData.Stride * m_bitmap.Height,
                                                      bitmapData.Stride);

            m_bitmap.UnlockBits(bitmapData);

            //new File Stream
            FileStream stream = new FileStream(fileName, FileMode.Create);

            //save Tiff Bitmap 
            TiffBitmapEncoder encoder = new TiffBitmapEncoder();
            encoder.Compression = TiffCompressOption.Zip;
            encoder.Frames.Add(BitmapFrame.Create(source));
            encoder.Save(stream);
            stream.Close();
            return true;
        }
        void readRGBC()
        {
            Rectangle rect = new Rectangle(0, 0, m_bitmap.Width, m_bitmap.Height);
            BitmapData bitmapData = m_bitmap.LockBits(rect, ImageLockMode.ReadWrite, m_bitmap.PixelFormat);

            int nShift = m_nDepth - 8;
            int nStride = bitmapData.Stride;
            int nCount = 0;
            byte[] pByteData = new byte[m_nBufferSize];
            byte[] pDstData = new byte[(m_nBufferSize * 3) / 4];
            Marshal.Copy(m_pBuffer, pByteData, 0, m_nBufferSize);
            if (m_nDepth == 8)
            {
                for (int i = 0; i < m_bitmap.Height; i++)
                {
                    for (int j = 0; j < nStride; j = j + 3)
                    {
                        pDstData[i * nStride + j] = (byte)(pByteData[nCount]);
                        pDstData[i * nStride + j + 1] = (byte)(pByteData[nCount + 1]);
                        pDstData[i * nStride + j + 2] = (byte)(pByteData[nCount + 2]);
                        nCount += 4;
                    }
                }
                Marshal.Copy(pDstData, 0, bitmapData.Scan0, (m_nBufferSize * 3 / 4));
                m_bitmap.UnlockBits(bitmapData);
                return;
            }
            short[] pShortData = new short[m_nBufferSize / 2];
            Marshal.Copy(m_pBuffer, pShortData, 0, m_nBufferSize / 2);
            for (int i = 0; i < bitmapData.Height; i++)
            {
                for (int j = 0; j < nStride; j = j + 3)
                {
                    pDstData[i * nStride + j] = (byte)(pShortData[nCount] >> nShift);
                    pDstData[i * nStride + j + 1] = (byte)(pShortData[nCount + 1] >> nShift);
                    pDstData[i * nStride + j + 2] = (byte)(pShortData[nCount + 2] >> nShift);
                    nCount += 4;
                }
            }
            Marshal.Copy(pDstData, 0, bitmapData.Scan0, (m_nBufferSize * 3 / 8));
            m_bitmap.UnlockBits(bitmapData);
        }
        //Convert Bmp Pixel Format to Media Pixel Format
        public System.Windows.Media.PixelFormat ConvertBmpPixelFormat(System.Drawing.Imaging.PixelFormat pixelformat)
        {
            System.Windows.Media.PixelFormat pixelFormats = System.Windows.Media.PixelFormats.Default;

            switch (pixelformat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    pixelFormats = PixelFormats.Bgra32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                    pixelFormats = PixelFormats.Rgba64;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    pixelFormats = PixelFormats.Rgb24;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    pixelFormats = PixelFormats.Rgb48;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormats = PixelFormats.Gray8;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    pixelFormats = PixelFormats.Gray16;
                    break;
            }
            return pixelFormats;
        }
    }
}
