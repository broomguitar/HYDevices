using HY.Devices.Camera;
using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace HY.Devices.CameraDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        IHYCamera _hyCamera;
        private void Window_Closed(object sender, EventArgs e)
        {
            _hyCamera?.StopGrab();
            _hyCamera?.Close();
            _hyCamera?.Dispose();
        }
        private void cmb_CameraType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cmb_CameraType.SelectedIndex < 0) return;
                switch (cmb_CameraType.SelectedIndex)
                {
                    case 0:
                        {
                            _hyCamera = new HYCamera_IKapLinear_BoardPCIE_New(0, 0, ConfigurationManager.AppSettings["ConfigName"]);
                        }
                        break;
                    case 1:
                        {
                            _hyCamera = new HYCamera_IKapLinear_Net(ConfigurationManager.AppSettings["IKSerialNum"]);
                        }
                        break;
                    case 2:
                        {
                            _hyCamera = new HYCamera_HIKArea(CameraConnectTypes.GigE, ConfigurationManager.AppSettings["HIKCameraSN"]);
                        }
                        break;
                    case 3:
                        {
                            _hyCamera = new HYCamera_DalsaLinear(ConfigurationManager.AppSettings["DalsaSerialNum"], ConfigurationManager.AppSettings["DalsaConfigFile"], 3);
                        }
                        break;
                    case 4:
                        {
                            _hyCamera = new HYCamera_BaslerArea(CameraConnectTypes.GigE, 0);
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void open_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_hyCamera?.Open() != true)
                {
                    MessageBox.Show("打开相机失败");
                    return;
                }
                MessageBox.Show("打开相机成功");
                _hyCamera.NewImageEvent += _hyCamera_NewImageEvent;
                cmb_CameraType.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("打开相机异常" + ex.ToString());
            }
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!_hyCamera.Close())
                {
                    MessageBox.Show("关闭相机失败");
                    return;
                }
                _hyCamera.NewImageEvent -= _hyCamera_NewImageEvent;
                cmb_CameraType.IsEnabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("关闭相机异常" + ex.ToString());
            }
        }

        private void grabContinous_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_hyCamera?.ContinousGrab() == true)
                {
                    MessageBox.Show("开始采集成功");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void stop_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_hyCamera?.IsGrabbing == true)
                {

                    if (_hyCamera.StopGrab())
                    {
                        MessageBox.Show("停止采集成功");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void grabOne_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_hyCamera.GrabOne(out Bitmap bitmap))
                {
                    img.Source = BitmapToBitmapSource(bitmap);
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void model_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsChecked == true)
            {
                _hyCamera?.SetTriggerModel(TriggerMode.ON);
                _hyCamera?.SetTriggerSource(TriggerSources.Soft);
            }
            else
            {
                _hyCamera?.SetTriggerModel(TriggerMode.OFF);
                _hyCamera?.SetTriggerSource(TriggerSources.Soft);
            }
        }

        private void trigger_Click(object sender, RoutedEventArgs e)
        {
            _hyCamera?.SoftWareTrigger();
        }
        WriteableBitmapUtils writeableBitmapUtils = null;
        int width = 0, height = 0;
        private void _hyCamera_NewImageEvent(object sender, Bitmap bitmap)
        {
            if (bitmap == null)
            {
                Addlog("数据无效");
                return;
            }
            try
            {
                if (writeableBitmapUtils == null || width != bitmap.Width || height != bitmap.Height)
                {
                    width = bitmap.Width;
                    height = bitmap.Height;
                    writeableBitmapUtils = new WriteableBitmapUtils();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        writeableBitmapUtils.InitialWriteableBitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
                        img.Source = writeableBitmapUtils.WriteableBitmap;
                    });
                }
                Addlog($"{DateTime.Now}---{width}×{height}");
                writeableBitmapUtils.GetImage(bitmap);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                bitmap.Dispose();
            }
        }
        private void Addlog(string msg)
        {
            Action _appendLogAction = new Action(() =>
            {
                this.tb_log.AppendText(msg + Environment.NewLine);
                this.tb_log.ScrollToEnd();
            });
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    _appendLogAction?.Invoke();
                });
            }
            else
            {
                _appendLogAction?.Invoke();
            }
        }
        /// <summary>
        /// Bitmap-->BitmapImage
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            try
            {
                if (bitmap == null)
                {
                    return null;
                }
                BitmapImage bitmapImage = new BitmapImage();
                using (MemoryStream ms = new MemoryStream())
                {
                    bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = ms;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                }
                return bitmapImage;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Bitmap-->BitmapFrame
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapFrame BitmapToBitmapFrame(Bitmap bitmap)
        {
            if (bitmap == null)
            {
                return null;
            }
            BitmapFrame bf = null;
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                bf = BitmapFrame.Create(ms, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            }
            return bf;
        }
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        public BitmapSource BitmapToBitmapSource(Bitmap bitmap)
        {

            IntPtr hBitmap = bitmap.GetHbitmap();

            try
            {
                var source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                return source;
            }
            finally
            {
                bitmap.Dispose();
                DeleteObject(hBitmap);
            }
        }
        private void btn_ExpSet_Click(object sender, RoutedEventArgs e)
        {
            _hyCamera?.SetExposureTime(float.Parse(tb_exposure.Text));
        }

        private void btn_ExpGet_Click(object sender, RoutedEventArgs e)
        {
            double exp = 0;
            if (_hyCamera?.GetExposureTime(out exp) == true)
            {
                tb_exposure.Text = exp.ToString();
            }
        }

        private void btn_GainSet_Click(object sender, RoutedEventArgs e)
        {
            _hyCamera?.SetGain(float.Parse(tb_gain.Text));
        }

        private void btn_GainGet_Click(object sender, RoutedEventArgs e)
        {
            double gain = 0;
            if (_hyCamera?.GetGain(out gain) == true)
            {
                tb_gain.Text = gain.ToString();
            }
        }

        private void autoExp_Click(object sender, RoutedEventArgs e)
        {
            if (autoExp.IsChecked == true)
            {
                _hyCamera?.SetAutoExposure(AutoMode.Continous);
            }
            else
            {
                _hyCamera?.SetAutoExposure(AutoMode.OFF);
            }
        }

        private void autoGain_Click(object sender, RoutedEventArgs e)
        {
            if (autoGain.IsChecked == true)
            {
                _hyCamera?.SetAutoGain(AutoMode.Continous);
            }
            else
            {
                _hyCamera?.SetAutoGain(AutoMode.OFF);
            }
        }
        uint imgHeight = 10000, count = 1;
        private void btn_Test_Click(object sender, RoutedEventArgs e)
        {
            count++;
            if (_hyCamera is AbstractHYCamera_IKap_Net iKap_Net)
            {
                if (iKap_Net.SetTriggerOutterModeFrameCount(count))
                {
                    MessageBox.Show("设置成功");
                }
            }
            if (_hyCamera is AbstractHYCamera_IKap_BoardPCIE_New boardPCIE_New)
            {
                if(boardPCIE_New.SetTriggerOutterModeFrameCount(count))
                    {
                    MessageBox.Show("设置成功");
                }
            }
            return;
            imgHeight += 1000;
            System.Threading.Tasks.Task.Run(() =>
            {

                if (!_hyCamera.StopGrab())
                {
                    MessageBox.Show("停止采集失败");
                }
                _hyCamera.SetImageHeight(imgHeight);
                if (!_hyCamera.ContinousGrab())
                {
                    MessageBox.Show("开始采集失败");
                }
            });
            return;
            HIKWindow hik = new HIKWindow();
            hik.ShowDialog();
        }

    }
    public class WriteableBitmapUtils
    {
        public string CameraName { get; set; }
        public WriteableBitmap WriteableBitmap { get; private set; }
        private void updateWritableBitmapData(byte[] byt, int Width, int Height, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            Action DoAction = delegate ()
            {
                try
                {
                    if (WriteableBitmap == null)
                    {
                        InitialWriteableBitmap(Width, Height, pixelFormat);
                    }
                    //锁住内存
                    WriteableBitmap.Lock();
                    if (byt == null)
                    {
                        return;
                    }
                    Marshal.Copy(byt, 0, WriteableBitmap.BackBuffer, byt.Length);
                    //指定更改位图的区域
                    WriteableBitmap.AddDirtyRect(new Int32Rect(0, 0, Width, Height));
                }
                finally
                {
                    WriteableBitmap.Unlock();
                }
            };
            UIThreadInvoke(DoAction);
        }
        private void UIThreadInvoke(Action Code)
        {
            try
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(Code);
                    return;
                }
                Code.Invoke();
            }
            catch
            {
                /*仅捕获、不处理！*/
            }
        }

        /// <summary>
        /// 异步执行 注：外层Try Catch语句不能捕获Code委托中的错误
        /// </summary>
        private void UIThreadBeginInvoke(Action Code)
        {
            try
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.BeginInvoke(Code);
                }
                else
                {
                    Code.BeginInvoke(null, null);
                }
            }
            catch { }
        }
        /// <summary>  
        /// Bitmap转换层RGB32  
        /// </summary>  
        /// <param name="Source">Bitmap图片</param>  
        /// <returns></returns>  
        public bool ConvertBitmap(Bitmap BpSource, out byte[] pRrgaByte)
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
        public bool BitmapToBytes(Bitmap bitmap, out byte[] bytes)
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
        private int GetChanel(System.Drawing.Imaging.PixelFormat pixelFormat)
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
        /// <summary>
        /// 初始化WriteableBitmap
        /// </summary>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <param name="pixelFormat"></param>
        public void InitialWriteableBitmap(int Width, int Height, System.Drawing.Imaging.PixelFormat pixelFormat)
        {
            WriteableBitmap = new WriteableBitmap(Width, Height,
                            96, 96, ConvertBmpPixelFormat(pixelFormat),
                          BitmapPalettes.Gray256);
        }
        private static System.Windows.Media.PixelFormat ConvertBmpPixelFormat(System.Drawing.Imaging.PixelFormat pixelformat)
        {
            System.Windows.Media.PixelFormat pixelFormats = System.Windows.Media.PixelFormats.Default;

            switch (pixelformat)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    pixelFormats = System.Windows.Media.PixelFormats.Bgra32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format32bppRgb:
                    pixelFormats = System.Windows.Media.PixelFormats.Bgr32;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format64bppArgb:
                    pixelFormats = System.Windows.Media.PixelFormats.Rgba64;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    pixelFormats = System.Windows.Media.PixelFormats.Bgr24;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format48bppRgb:
                    pixelFormats = System.Windows.Media.PixelFormats.Rgb48;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    pixelFormats = System.Windows.Media.PixelFormats.Gray8;
                    break;
                case System.Drawing.Imaging.PixelFormat.Format16bppGrayScale:
                    pixelFormats = System.Windows.Media.PixelFormats.Gray16;
                    break;
            }
            return pixelFormats;
        }
        /// <summary>
        /// 回调函数，获取图片
        /// </summary>
        /// <param name="bitmap"></param>
        public void GetImage(System.Drawing.Bitmap bp)
        {
            try
            {
                if (ConvertBitmap(bp, out byte[] bytes))
                {
                    updateWritableBitmapData(bytes, bp.Width, bp.Height, bp.PixelFormat);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
