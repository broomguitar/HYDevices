using HY.Devices.Camera;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;

namespace HY.Devices.CameraDemo
{
    /// <summary>
    /// HIKWindow.xaml 的交互逻辑
    /// </summary>
    public partial class HIKWindow : Window
    {
        public HIKWindow()
        {
            InitializeComponent();
            stopwatch = new System.Diagnostics.Stopwatch();
            double frametime = 1 / RENDERFPS;
            frameDurationTicks = (int)(System.Diagnostics.Stopwatch.Frequency * frametime);
        }
        List<IHYCamera> camera_HIKs = new List<IHYCamera>();
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            for (int i = 1; i < 7; i++)
            {
                if (ConfigurationManager.AppSettings.AllKeys.Contains($"HKCamera{i}"))
                {
                    var camera = LoadHikCmaera(ConfigurationManager.AppSettings[$"HKCamera{i}"]);
                    if (camera != null)
                    {
                        camera_HIKs.Add(camera);
                    }
                }
            }
            MessageBox.Show($"{camera_HIKs.Count}个相机初始化完成");
        }
        private AbstractHYCamera_HIK LoadHikCmaera(string paras)
        {
            try
            {
                string[] strs = paras.Split('-');
                uint index = uint.Parse(strs[0]);
                bool isLinear = int.Parse(strs[1]) == 1;
                AbstractHYCamera_HIK hYCamera_HIK = null;
                if (isLinear)
                {
                    hYCamera_HIK = new HYCamera_HIKLinear(CameraConnectTypes.GigE, index);
                }
                else
                {
                    hYCamera_HIK = new HYCamera_HIKArea(CameraConnectTypes.GigE, index);
                }
                return hYCamera_HIK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return null;
            }
        }
        private void open_Click(object sender, RoutedEventArgs e)
        {
            if (camera_HIKs.Count > 0)
            {
                bool b = true;
                foreach (var item in camera_HIKs)
                {
                    try
                    {
                        if (item?.Open() != true)
                        {
                            MessageBox.Show($"打开相机{item.CameraIndex}失败");
                            b = false;
                            continue;
                        }
                        item.NewImageEvent += _hyCamera_NewImageEvent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"打开相机{item.CameraIndex}异常" + ex.ToString());
                    }
                }
                if (b)
                {
                    MessageBox.Show("所有相机打开成功");
                }
            }
        }
        private void model_Click(object sender, RoutedEventArgs e)
        {
            if (model.IsChecked == true)
            {
                if (camera_HIKs.Count > 0)
                {
                    bool b = true;
                    foreach (var item in camera_HIKs)
                    {
                        try
                        {
                            item.SetTriggerModel(TriggerMode.ON);
                            item.SetTriggerSource(TriggerSources.Line0);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"打开相机{item.CameraIndex}硬触发异常" + ex.ToString());
                        }
                    }
                    if (b)
                    {
                        MessageBox.Show("所有相机打开硬触发成功");
                    }
                }
            }
            else
            {
                if (camera_HIKs.Count > 0)
                {
                    bool b = true;
                    foreach (var item in camera_HIKs)
                    {
                        try
                        {
                            item.SetTriggerModel(TriggerMode.OFF);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"关闭相机{item.CameraIndex}硬触发异常" + ex.ToString());
                        }
                    }
                    if (b)
                    {
                        MessageBox.Show("所有相机关闭硬触发成功");
                    }
                }
            }
        }
        private void close_Click(object sender, RoutedEventArgs e)
        {
            if (camera_HIKs.Count > 0)
            {
                foreach (var item in camera_HIKs)
                {
                    try
                    {
                        if (item?.Close() != true)
                        {
                            MessageBox.Show($"关闭相机{item.CameraIndex}失败");
                        }
                        item.NewImageEvent -= _hyCamera_NewImageEvent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"关闭相机{item.CameraIndex}失败");
                    }
                }
            }
        }

        private void grabContinous_Click(object sender, RoutedEventArgs e)
        {
            if (camera_HIKs.Count > 0)
            {
                foreach (var item in camera_HIKs)
                {
                    try
                    {
                        if (item?.ContinousGrab() != true)
                        {
                            MessageBox.Show($"相机{item.CameraIndex}实时抓拍失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"相机{item.CameraIndex}实时抓拍异常{ex.ToString()}");
                    }
                }
                MessageBox.Show("所有相机已开始采集");
            }
        }
        private void stop_Click(object sender, RoutedEventArgs e)
        {
            if (camera_HIKs.Count > 0)
            {
                foreach (var item in camera_HIKs)
                {
                    try
                    {
                        if (item?.IsGrabbing == true)
                        {
                            item.StopGrab();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"StopGrab相机{item.CameraIndex}异常");
                    }
                }
                return;
            }
        }
        private System.Diagnostics.Stopwatch stopwatch;
        private int frameDurationTicks;
        private readonly double RENDERFPS = 5;
        private void _hyCamera_NewImageEvent(object sender, Bitmap e)
        {
            string filePath = string.Empty;
            try
            {
                if (sender is IHYCamera camera)
                {
                    filePath = AppDomain.CurrentDomain.BaseDirectory + $"{camera.CameraIndex}_{DateTime.Now.ToString("yyyyMMddHHmmsss")}.jpg";

                    e.Save(filePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                    e.Dispose();
                    switch (camera.CameraIndex)
                    {
                        case 0: ShowImgByFile(img1, filePath); break;
                        case 1: ShowImgByFile(img2, filePath); break;
                        case 2: ShowImgByFile(img3, filePath); break;
                        case 3: ShowImgByFile(img4, filePath); break;
                        case 4: ShowImgByFile(img5, filePath); break;
                        case 5: ShowImgByFile(img6, filePath); break;
                        default:
                            break;
                    }
                    return;
                    switch (camera.CameraIndex)
                    {
                        case 0: ShowImg(img1, e); break;
                        case 1: ShowImg(img2, e); break;
                        case 2: ShowImg(img3, e); break;
                        case 3: ShowImg(img4, e); break;
                        case 4: ShowImg(img5, e); break;
                        case 5: ShowImg(img6, e); break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(filePath + ex.ToString());
            }
        }
        private void ShowImg(System.Windows.Controls.Image image, Bitmap e)
        {
            this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate
            {
                image.Source = null;
                if (e != null)
                {
                    image.Source = BitmapToBitmapSource(e);
                    e.Dispose();
                }
            });
        }
        private void ShowImgByFile(System.Windows.Controls.Image image, string filePath)
        {
            this.Dispatcher.Invoke((System.Threading.ThreadStart)delegate
            {
                image.Source = null;
                if (File.Exists(filePath))
                {
                    image.Source = GetBitmapImage(filePath);
                }
            });
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
        public BitmapImage GetBitmapImage(string imagePath, bool isZip = false)
        {
            if (File.Exists(imagePath))
            {
                try
                {
                    byte[] buf = null;
                    using (BinaryReader loader = new BinaryReader(File.Open(imagePath, FileMode.Open)))
                    {
                        FileInfo fd = new FileInfo(imagePath);
                        int Length = (int)fd.Length;
                        buf = new byte[Length];
                        buf = loader.ReadBytes((int)fd.Length);
                    }

                    //开始加载图像
                    BitmapImage bim = new BitmapImage();
                    bim.BeginInit();
                    bim.CacheOption = BitmapCacheOption.OnLoad;
                    if (isZip)
                    {
                        bim.DecodePixelHeight = 100;
                    }
                    bim.StreamSource = new MemoryStream(buf);
                    bim.EndInit();
                    bim.Freeze();
                    return bim;
                }
                catch (Exception)
                {
                    return null;
                }
            }
            return null;
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (camera_HIKs.Count > 0)
            {
                foreach (var item in camera_HIKs)
                {
                    try
                    {
                        if (item.IsConnected == true && item.Close())
                        {
                            MessageBox.Show($"关闭相机{item.CameraIndex}失败");
                        }
                        item.NewImageEvent -= _hyCamera_NewImageEvent;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"关闭相机{item.CameraIndex}异常");
                    }
                };
            }
            if (stopwatch.IsRunning)
            {
                stopwatch.Stop();
            }
        }
    }
}
