using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Utils
{
    public class ImageHelper
    {
        /// <summary>  
        /// Bitmap转换层RGB32  
        /// </summary>  
        /// <param name="Source">Bitmap图片</param>  
        /// <returns></returns>  
        public static bool ConvertBitmap(Bitmap BpSource, out byte[] pRrgaByte)
        {
            pRrgaByte = null;
            try
            {
                int PicWidth = BpSource.Width;
                int PicHeight = BpSource.Height;
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, PicWidth, PicHeight);
                System.Drawing.Imaging.BitmapData bmp_Data = BpSource.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite, BpSource.PixelFormat);
                IntPtr iPtr = bmp_Data.Scan0;
                int picSize = PicWidth * PicHeight * GetChanel(BpSource.PixelFormat);
                pRrgaByte = new byte[picSize];
                Marshal.Copy(iPtr, pRrgaByte, 0, picSize);
                BpSource.UnlockBits(bmp_Data);
                return true;
            }
            catch (Exception ex)
            {
                pRrgaByte = null;
            }
            return false;
        }
        public static bool BitmapToBytes(Bitmap bitmap, out byte[] bytes)
        {
            bytes = null;
            System.IO.MemoryStream ms = null;
            try
            {
                ms = new System.IO.MemoryStream();
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                bytes = ms.GetBuffer();
                return true;
            }
            catch (ArgumentNullException ex)
            {
                return false;
            }
            finally
            {
                ms.Close();
            }
        }
        private static int GetChanel(System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            int channel = 1;
            switch (pixelFormat)
            {
                case System.Drawing.Imaging.PixelFormat.Indexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Gdi:
                    break;
                case System.Drawing.Imaging.PixelFormat.Alpha:
                    break;
                case System.Drawing.Imaging.PixelFormat.PAlpha:
                    break;
                case System.Drawing.Imaging.PixelFormat.Extended:
                    break;
                case System.Drawing.Imaging.PixelFormat.Canonical:
                    break;
                case System.Drawing.Imaging.PixelFormat.Undefined:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format1bppIndexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format4bppIndexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb555:
                case System.Drawing.Imaging.PixelFormat.Format16bppRgb565:
                case System.Drawing.Imaging.PixelFormat.Format16bppArgb1555:
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    channel = 3;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format32bppPArgb:
                    channel = 4;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    channel = 6;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                case System.Drawing.Imaging.PixelFormat.Format64bppPArgb:
                    channel = 8;
                    break;
                case System.Drawing.Imaging.PixelFormat.Max:
                    break;
                default:
                    break;
            }
            return channel;
        }
    }
}
