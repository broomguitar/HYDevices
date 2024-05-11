using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace HY.Devices.Algorithm.Yolov7.CS
{
    public class ImageHelper
    {

        public static void SaveImage(Bitmap img, string filePath, long quality = 75L)
        {
            Bitmap bitmap = (Bitmap)img.Clone();
            try
            {
                if (bitmap.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    SaveGrayImage_Old(bitmap, filePath, (int)quality);
                }
                else
                {
                    SaveRGBImage(bitmap, filePath, quality);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                bitmap.Dispose();
            }
        }
        public static void SaveRGBImage(Bitmap bitmap, string filePath, long quality = 75L)
        {
            try
            {
                bitmap = bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                var extName = System.IO.Path.GetExtension(filePath).ToLower();
                switch (extName)
                {
                    case ".png":
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            // 设置压缩参数
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
                            bitmap.Save(filePath, GetEncoder(ImageFormat.Png), encoderParams);
                        }
                        break;
                    case ".jpg":
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
                            bitmap.Save(filePath, GetEncoder(ImageFormat.Jpeg), encoderParams);
                        }; break;
                    case ".tiff":
                        {
                            EncoderParameters encoderParams = new EncoderParameters(1);
                            // 设置压缩参数
                            encoderParams.Param[0] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionLZW);
                            // 将图像保存为 TIFF 格式
                            bitmap.Save(filePath, GetEncoder(ImageFormat.Tiff), encoderParams);
                        }
                        break;
                    case ".bmp":
                        {
                            bitmap.Save(filePath, ImageFormat.Bmp);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex) { throw ex; }
        }
        private static System.Drawing.Imaging.ImageCodecInfo GetEncoder(System.Drawing.Imaging.ImageFormat format)
        {
            System.Drawing.Imaging.ImageCodecInfo[] codecs = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();

            foreach (System.Drawing.Imaging.ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }

            return null;
        }
        /// <summary>
        /// 保存单通道图片
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="filePath"></param>
        public static void SaveGrayImage_Old(Bitmap bitmap, string filePath, int qualityLevel = 75)
        {
            try
            {
                if (qualityLevel < 1)
                {
                    qualityLevel = 1;
                }
                else if (qualityLevel > 100)
                {
                    qualityLevel = 100;
                }
                Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
                BitmapData bitmapData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, bitmap.PixelFormat);
                System.Windows.Media.PixelFormat pixelFormats = ConvertBmpPixelFormat(bitmap.PixelFormat);
                BitmapSource source = BitmapSource.Create(bitmap.Width,
                                                          bitmap.Height,
                                                          bitmap.HorizontalResolution,
                                                          bitmap.VerticalResolution,
                                                          pixelFormats,
                                                          null,
                                                          bitmapData.Scan0,
                                                          bitmapData.Stride * bitmap.Height,
                                                          bitmapData.Stride);

                bitmap.UnlockBits(bitmapData);
                //new File Stream
                using (FileStream stream = new FileStream(filePath, FileMode.Create))

                {   //save Tiff Bitmap 
                    BitmapEncoder encoder = null;
                    var extName = System.IO.Path.GetExtension(filePath).ToLower();
                    switch (extName)
                    {
                        case ".png":
                            {
                                encoder = new PngBitmapEncoder();
                            }
                            break;
                        case ".tiff":
                            {
                                encoder = new TiffBitmapEncoder { Compression = TiffCompressOption.Lzw };
                            }
                            break;
                        case ".jpg":
                            {
                                encoder = new JpegBitmapEncoder { QualityLevel = qualityLevel };
                            }
                            break;
                        case ".bmp": encoder = new BmpBitmapEncoder(); break;
                        default:
                            break;
                    }
                    encoder.Frames.Add(BitmapFrame.Create(source));
                    encoder.Save(stream);
                    encoder = null;
                    stream.Close();
                    stream.Dispose();
                }
            }
            catch (Exception ex) { throw ex; }
        }

        public static System.Windows.Media.PixelFormat ConvertBmpPixelFormat(System.Drawing.Imaging.PixelFormat pixelformat)
        {
            System.Windows.Media.PixelFormat pixelFormats = PixelFormats.Default;

            switch (pixelformat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    pixelFormats = PixelFormats.Bgra32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    pixelFormats = PixelFormats.Bgr32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                    pixelFormats = PixelFormats.Rgba64;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    pixelFormats = PixelFormats.Bgr24;
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
