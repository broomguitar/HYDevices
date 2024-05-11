using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace HY.Devices.Algorithm
{
    /// <summary>
    /// OCR
    /// </summary>
    public class OCR
    {
        private static readonly object _lockObj = new object();
        private static OCR _instance;
        public static OCR Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new OCR();
                        }
                    }
                }
                return _instance;
            }
        }
        [DllImport("ocr_system.dll", EntryPoint = "Init", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Init(byte[] str, int length);


        [DllImport("ocr_system.dll", EntryPoint = "Detect", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr Detect(byte[] input, int width, int height, int nchanel, ref int ret);

        [DllImport("ocr_system.dll", EntryPoint = "Release", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Release();

        public bool IsInit { get; private set; }
        /// <summary>
        /// 识别模型初始化
        /// </summary>
        /// <param name="ConfigPath">配置文件路径</param>
        /// <returns></returns>
        public int PaddleInit(string ConfigPath)
        {
            byte[] bstr = System.Text.Encoding.UTF8.GetBytes(ConfigPath);
            int ret = Init(bstr, bstr.Length);
            IsInit = ret == 0;
            return ret;
        }


        /// <summary>
        /// 处理图像
        /// </summary>
        /// <param name="ImagePath">图像路径</param>
        /// <returns></returns>
        public string PaddleDetect(string ImagePath)
        {
            int ret = 0;
            Bitmap bmp = new Bitmap(ImagePath);
            byte[] source = GetBGRValues(bmp, out int stride);
            IntPtr p = Detect(source, bmp.Width, bmp.Height, Image.GetPixelFormatSize(bmp.PixelFormat) / 8, ref ret);
            if (ret == 1)
            {
                bmp.Dispose();
                return Marshal.PtrToStringAnsi(p);
            }
            else
            {
                bmp.Dispose();
                return "";
            }

        }


        /// <summary>
        /// 卸载模型
        /// </summary>
        /// <returns></returns>
        public int UInit()
        {
            return Release();
        }

        private byte[] GetBGRValues(Bitmap bmp, out int stride)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);
            stride = bmpData.Stride;
            var rowBytes = bmpData.Width * Image.GetPixelFormatSize(bmp.PixelFormat) / 8;
            var imgBytes = bmp.Height * rowBytes;
            byte[] rgbValues = new byte[imgBytes];
            IntPtr ptr = bmpData.Scan0;
            for (var i = 0; i < bmp.Height; i++)
            {
                Marshal.Copy(ptr, rgbValues, i * rowBytes, rowBytes);
                ptr += bmpData.Stride;
            }
            bmp.UnlockBits(bmpData);
            return rgbValues;
        }
    }
}
