using HalconDotNet;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace HY.Devices.Algorithm.Utils
{
    public class Ho_ImageHelper
    {
        public static HObject Bitmap2HObject(Bitmap bitmap)
        {
            try
            {
                return bitmap.PixelFormat == PixelFormat.Format8bppIndexed ? Bitmap2HObjectBpp8(bitmap) : Bitmap2HObjectBpp24(bitmap);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static HObject Bitmap2HObjectBpp24(Bitmap bmp)
        {
            try
            {
                HObject image;
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                HOperatorSet.GenImageInterleaved(out image, srcBmpData.Scan0, "bgr", bmp.Width, bmp.Height, 0, "byte", 0, 0, 0, 0, -1, 0);
                bmp.UnlockBits(srcBmpData);
                return image;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static HObject Bitmap2HObjectBpp8(Bitmap bmp)
        {
            try
            {
                HObject image;
                Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);

                BitmapData srcBmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format8bppIndexed);

                HOperatorSet.GenImage1(out image, "byte", bmp.Width, bmp.Height, srcBmpData.Scan0);
                bmp.UnlockBits(srcBmpData);
                return image;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static Bitmap HObject2Bitmap(HObject ho_image)
        {
            try
            {
                HOperatorSet.CountChannels(ho_image, out HTuple hv_Channel); 
                return hv_Channel.I == 1 ? HObject2Bpp8(ho_image) : HObject2Bpp24(ho_image);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static Bitmap HObject2Bpp24(HObject ho_image)
        {
            Bitmap res24;
            HTuple width0, height0, type, width, height;
            //获取图像尺寸
            HOperatorSet.GetImageSize(ho_image, out width0, out height0);
            //创建交错格式图像
            HOperatorSet.InterleaveChannels(ho_image, out HObject InterImage, "argb", "match", 255);  //"rgb", 4 * width0, 0     "argb", "match", 255

            //获取交错格式图像指针
            HOperatorSet.GetImagePointer1(InterImage, out HTuple Pointer, out type, out width, out height);
            IntPtr ptr = Pointer;
            //构建新Bitmap图像
            Bitmap res32 = new Bitmap(width / 4, height, width, PixelFormat.Format32bppArgb, ptr);  // Format32bppArgb     Format24bppRgb
            //32位Bitmap转24位
            res24 = new Bitmap(res32.Width, res32.Height, PixelFormat.Format24bppRgb);
            Graphics graphics = Graphics.FromImage(res24);
            graphics.DrawImage(res32, new Rectangle(0, 0, res32.Width, res32.Height));
            res32.Dispose();
            return res24;
        }
        public static Bitmap HObject2Bpp8(HObject ho_image)
        {
            Bitmap res;
            HTuple hpoint, type, width, height;

            const int Alpha = 255;

            HOperatorSet.GetImagePointer1(ho_image, out hpoint, out type, out width, out height);

            res = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            ColorPalette pal = res.Palette;
            for (int i = 0; i <= 255; i++)
            {
                pal.Entries[i] = Color.FromArgb(Alpha, i, i, i);
            }
            res.Palette = pal;
            Rectangle rect = new Rectangle(0, 0, width, height);
            BitmapData bitmapData = res.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            int PixelSize = Bitmap.GetPixelFormatSize(bitmapData.PixelFormat) / 8;

            IntPtr ptr1 = bitmapData.Scan0;
            IntPtr ptr2 = hpoint;
            int bytes = width * height;
            byte[] rgbvalues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr2, rgbvalues, 0, bytes);
            System.Runtime.InteropServices.Marshal.Copy(rgbvalues, 0, ptr1, bytes);
            res.UnlockBits(bitmapData);
            return res;
        }
        [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory", CharSet = CharSet.Ansi)]
        public extern static long CopyMemory(IntPtr dest, IntPtr source, int size);
        public static byte[] GetByteFromHobject(HObject ho)
        {
            HTuple hv_Pointer, hv_Type, hv_Width, hv_Height, hv_Channel;
            int channel, image_channel, image_height, image_width;
            byte[] buffer;

            HOperatorSet.CountChannels(ho, out hv_Channel);
            HOperatorSet.GetImageSize(ho, out hv_Width, out hv_Height);
            channel = image_channel = hv_Channel.I;
            if (image_channel == 1)
            {
                //单通道
                HOperatorSet.GetImagePointer1(ho, out hv_Pointer, out hv_Type, out hv_Width, out hv_Height);
                //IntPtr p=hv_Pointer[0].L;
                image_height = hv_Height;
                image_width = hv_Width;
                int size1 = image_height * image_width;
                buffer = new byte[size1];

                GCHandle hObject = GCHandle.Alloc(buffer, GCHandleType.Pinned);
                IntPtr pObject = hObject.AddrOfPinnedObject();
                uint count = (uint)(image_height * image_width);

                CopyMemory((IntPtr)pObject, (IntPtr)hv_Pointer[0].L, (int)count);
                if (hObject.IsAllocated)
                    hObject.Free();

            }
            else
            {

                //创建交错格式图像
                HOperatorSet.InterleaveChannels(ho, out HObject interleaved, "rgb", "match", 255);
                //获取交错格式图像指针
                HOperatorSet.GetImagePointer1(interleaved, out hv_Pointer, out HTuple type, out HTuple width, out HTuple height);
                image_height = hv_Height;
                image_width = hv_Width;
                //int size1 = image_height * image_width * channel;
                // buffer = new byte[size1];


                //构建新Bitmap图像
                // Bitmap bitmap = new Bitmap(width / 4, height, width, System.Drawing.Imaging.PixelFormat.Format24bppRgb, hv_Pointer);
                //pictureBox1.Image = bitmap;
                int stride;
                //buffer = GetBGRValues(bitmap, out stride);


                int size1 = width * height;
                buffer = new byte[size1];
                Marshal.Copy(hv_Pointer, buffer, 0, width * height);

            }
            return buffer;

        }
        public static HObject GetHoImageFromDynamic(dynamic data)
        {
            HObject ho_Image = null;
            HOperatorSet.GenEmptyObj(out ho_Image);
            ho_Image.Dispose();
            if (data is string)
            {
                HOperatorSet.ReadImage(out ho_Image, data);
            }
            else
            {
                ho_Image = Utils.Ho_ImageHelper.Bitmap2HObject(data);
            }
            return ho_Image;
        }
    }

}
