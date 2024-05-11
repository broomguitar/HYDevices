using HalconDotNet;
using HY.Devices.Algorithm;
using HYCommonUtils.SerializationUtils;
using HYWindowUtils.WPF.VMBaseUtil;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace HY.Devices.AlgorithmDemo
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window, INotifyPropertyChanged
    {
        
        public TestWindow()
        {
            //Task.Run(init);
            init();
            this.DataContext = this;
            this.Closed += TestWindow_Closed;
        }
        private void TestWindow_Closed(object sender, EventArgs e)
        {
            if (_currentAolgrithm != null)
            {
                _currentAolgrithm.UnInit();
            }
            if (hYCamera != null)
            {
                btn_Camera_Click(null, null);
            }
            hoimage0.Dispose();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void init()
        {
            try
            {
                var catalog = new DirectoryCatalog("Algorithms");
                var container = new CompositionContainer(catalog);
                container.ComposeParts(this);
                ProjectAssembly = AlgorithmItems.FirstOrDefault()?.GetType().Assembly;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        public Assembly ProjectAssembly { get; set; }
        [ImportMany(typeof(IAlgorithm))]
        public IEnumerable<IAlgorithm> AlgorithmItems { get; set; }
        public string ProjectName
        {
            get
            {
                if(ProjectAssembly!=null)
                {
                     return ProjectAssembly.GetName().Name;
                }
                return null;
            }
        }
        public IEnumerable<string> AlgorithmItemNames => AlgorithmItems.Select(a => a.GetType().Name);
        private IEnumerable<ParamModel> _initParams;

        public IEnumerable<ParamModel> InitParams
        {
            get { return _initParams; }
            set { _initParams = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(InitParams))); }
        }
        private IEnumerable<ParamModel> _actionParams;

        public IEnumerable<ParamModel> ActionParams
        {
            get { return _actionParams; }
            set
            {
                _actionParams = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ActionParams)));
            }
        }
        private void cb_AlgorithmMethods_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_Algorithms.SelectedIndex < 0) return;
            if (_currentAolgrithm != null)
            {
                _currentAolgrithm.UnInit();
            }

        }
        private IAlgorithm _currentAolgrithm = null;
        private void btn_load_Click(object sender, RoutedEventArgs e)
        {
            if (cb_Algorithms.SelectedIndex < 0)
            {
                MessageBox.Show("未选择算法");
                return;
            }
            _currentAolgrithm = AlgorithmItems.ElementAt(cb_Algorithms.SelectedIndex);
            List<ParamModel> ls = new List<ParamModel>();
            foreach (var item in _currentAolgrithm.InitParamNames)
            {
                ls.Add(new ParamModel { ParamName = item.Key, ParamValue = item.Value });
            }
            InitParams = ls;
            List<ParamModel> ls1 = new List<ParamModel>();
            foreach (var item in _currentAolgrithm.ActionParamNames)
            {
                if (item.Key == "HWindow")
                {
                    ls1.Add(new ParamModel { ParamName = item.Key, ParamValue = this.HalControl.HalconWindow });
                }
                else
                {
                    ls1.Add(new ParamModel { ParamName = item.Key, ParamValue = item.Value });
                }
            }
            ActionParams = ls1;

        }

        private void btn_Init_Click(object sender, RoutedEventArgs e)
        {
            Action action = new Action(() =>
            {
                try
                {
                    Dictionary<string, dynamic> initParams = new Dictionary<string, dynamic>();
                    if (InitParams != null)
                    {
                        foreach (var item in InitParams)
                        {
                            initParams.Add(item.ParamName, item.ParamValue);
                        }
                    }
                    stopwatch.Restart();
                    if (_currentAolgrithm.IsInit)
                    {
                        _currentAolgrithm.UnInit();
                    }
                    if (_currentAolgrithm.Init(initParams))
                    {
                        Addlog($"耗时:{stopwatch.ElapsedMilliseconds},初始化成功");
                    }
                    else
                    {
                        Addlog($"耗时:{stopwatch.ElapsedMilliseconds},初始化失败");

                    }
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.ToString());
                }
            });
            //action();
           Task.Run(() => { Application.Current.Dispatcher.Invoke(action); });
        }
        private Stopwatch stopwatch = new Stopwatch();
        private void btn_Action_Click(object sender, RoutedEventArgs e)
        {
            Action action = new Action(() =>
            {
                try
                {

                    if (!_currentAolgrithm.IsInit)
                    {
                        MessageBox.Show("未初始化");
                        return;
                    }
                    Dictionary<string, dynamic> actionParams = new Dictionary<string, dynamic>();
                    if (ActionParams != null)
                    {
                        foreach (var item in ActionParams)
                        {
                            try
                            {
                                TypeCode ty = Convert.GetTypeCode(_currentAolgrithm.ActionParamNames[item.ParamName]);
                                actionParams.Add(item.ParamName, Convert.ChangeType(item.ParamValue, ty));
                            }
                            catch (Exception ed)
                            {
                                MessageBox.Show("传入参数的类型不正确！");
                            }
                        }
                    }
                    string imgPath = ActionParams.First(a => a.ParamName == "Image").ParamValue;
                    stopwatch.Restart();
                    SetFileName(actionParams["Image"]);
                    var ret = _currentAolgrithm.DoAction(actionParams);
                    Addlog($"耗时:{stopwatch.ElapsedMilliseconds},结果" + ret["resultStr"]);

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            });
            //action();
            Task.Run(()=> { Application.Current.Dispatcher.Invoke(action); });
        }
        HObject hoimage0 = new HObject();
        private void showImage(string imgPath)
        {
            hoimage0.Dispose();
            HTuple width0 = new HTuple();
            HTuple height0 = new HTuple();
            HOperatorSet.ReadImage(out hoimage0, imgPath);
            HOperatorSet.GetImageSize(hoimage0, out width0, out height0);
            HOperatorSet.SetPart(HalControl.HalconWindow, 0, 0, height0 - 1, width0 - 1);
            HOperatorSet.SetDraw(HalControl.HalconWindow, "margin");
            HOperatorSet.SetColor(HalControl.HalconWindow, "red");
            hoimage0.DispObj(HalControl.HalconWindow);
        }
        private void Addlog(string msg)
        {
            msg = System.Threading.Thread.CurrentThread.ManagedThreadId + msg;
            Action _appendLogAction = new Action(() =>
            {
                this.tb_log.Clear();
                this.tb_log.AppendText(msg + Environment.NewLine);
                //this.tb_log.ScrollToEnd();
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
        private void SetFileName(string filePath)
        {
            Action _setFileNameAction = new Action(() =>
            {
                this.tb_fileName.Text = filePath;
                showImage(filePath);
            });
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(delegate
                {
                    _setFileNameAction?.Invoke();
                });
            }
            else
            {
                _setFileNameAction?.Invoke();
            }
        }

        private void btn_selectDir_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.Description = "请选择图片所在的目录";
            //folderBrowser.ShowNewFolderButton = true;
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.tb_imgDir.Text = folderBrowser.SelectedPath;
            }
        }
        private bool isRuning = false;
        private string[] files;
        /// <summary>
        /// 批量测试
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Batch_Click(object sender, RoutedEventArgs e)
        {
            if (!_currentAolgrithm.IsInit)
            {
                MessageBox.Show("未初始化");
                return;
            }
            if (!System.IO.Directory.Exists(this.tb_imgDir.Text.Trim()))
            {
                MessageBox.Show("未选择图片文件夹");
                return;
            }
            Dictionary<string, dynamic> actionParams = new Dictionary<string, dynamic>();
            if (ActionParams != null)
            {
                foreach (var item in ActionParams)
                {
                    //if (item.ParamName != "HWindow")
                    //{
                    //    //if (item.ParamValue as string == null)
                    //    //{
                    //    //    item.ParamValue = item.ParamValue.ToString();
                    //    //}
                    //}
                    actionParams.Add(item.ParamName, item.ParamValue);
                }
            }
            if (files == null)
            {
                files = System.IO.Directory.GetFiles(this.tb_imgDir.Text.Trim());
            }
            Task.Run(() =>
            {
                isRuning = true;
                foreach (var item in files)
                {
                    while (isPause)
                    {
                        System.Threading.Thread.Sleep(1000);
                    }
                    actionParams["Image"] = item;
                    SetFileName(actionParams["Image"]);
                    stopwatch.Restart();
                    var ret = _currentAolgrithm.DoAction(actionParams);
                    Addlog($"耗时:{stopwatch.ElapsedMilliseconds},--文件;{item}" + "--结果:" +ret["resultStr"]);
                    System.Threading.Thread.Sleep(10);
                }
                isRuning = false;
            });

        }
        private bool isPause = false;
        private void btn_BatchSwitch_Click(object sender, RoutedEventArgs e)
        {
            if (isRuning)
            {
                isPause = btn_BatchSwitch.IsChecked.Value;
                btn_BatchSwitch.Content = btn_BatchSwitch.IsChecked == true ? "继续" : "暂停";
            }
            else
            {
                btn_BatchSwitch.IsChecked = false;
                btn_BatchSwitch.Content = "暂停";
            }
        }

        private void btn_BatchLast_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_BatchNext_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_hdict2txt_Click(object sender, RoutedEventArgs e)
        {
            HY.Devices.Algorithm.Utils.Hdict2TxtHelper.Hdict2Txt("test.hdict", "");
        }

        private void btn_txt2hdict_Click(object sender, RoutedEventArgs e)
        {
            HY.Devices.Algorithm.Utils.Hdict2TxtHelper.Txt2Hdict("目标检测1224.hdict", "test");
        }
        private void btn_Bitmap2HImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "所有文件(*.*)|*.*|图片文件|*.jpg;*.png;*.jpeg;*.bmp";
                if (openFileDialog.ShowDialog() == true)
                {
                    Bitmap bitmap = new Bitmap(openFileDialog.FileName);

                    stopwatch.Restart();
                    HalconDotNet.HObject ho_Image = HY.Devices.Algorithm.Utils.Ho_ImageHelper.Bitmap2HObject(bitmap);
                    Addlog($"Bitmap2HImag耗时:{stopwatch.ElapsedMilliseconds}");
                    HalconDotNet.HOperatorSet.DispObj(ho_Image, HalControl.HalconWindow);
                    bitmap.Dispose();
                }
            }
            catch (Exception ex)
            {
                Addlog(ex.ToString());
            }
        }

        private void btn_File2HImage_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Multiselect = false;
                openFileDialog.Filter = "所有文件(*.*)|*.*|图片文件|*.jpg;*.png;*.jpeg;*.bmp";
                if (openFileDialog.ShowDialog() == true)
                {
                    stopwatch.Restart();
                    HalconDotNet.HObject ho_Image;
                    HalconDotNet.HOperatorSet.ReadImage(out ho_Image, openFileDialog.FileName);
                    Addlog($"Bitmap2HImag耗时:{stopwatch.ElapsedMilliseconds}");
                    HalconDotNet.HOperatorSet.DispObj(ho_Image, HalControl.HalconWindow);
                }
            }
            catch (Exception ex)
            {
                Addlog(ex.ToString());
            }
        }
        private void btn_ZBoom_Click(object sender, RoutedEventArgs e)
        {
            try
            {
               
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        private IAlgorithm openposeAolgrithm = null, yoloAolgrithm = null, ocrAolgrithm = null;
        private void btn_Camera_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (hYCamera == null)
                {
                    if (cb_OpenPose.IsChecked == true)
                    {
                        if (openposeAolgrithm == null)
                        {
                            openposeAolgrithm = AlgorithmItems.FirstOrDefault(a => a.AlgorithmType == AlgorithmTypes.ActionRecognition);
                            if (openposeAolgrithm != null)
                            {
                                if (!openposeAolgrithm.Init(openposeAolgrithm.InitParamNames))
                                {
                                    Addlog("Openpose算法加载失败");
                                }
                            }
                        }
                    }
                    if (cb_Yolo.IsChecked == true)
                    {
                        if (yoloAolgrithm == null)
                        {
                            yoloAolgrithm = AlgorithmItems.FirstOrDefault(a => a.AlgorithmType==AlgorithmTypes.DetectTarget);
                            if (yoloAolgrithm != null)
                            {
                                yoloAolgrithm.InitParamNames["batch_size"] = 1;
                                //yoloAolgrithm.InitParamNames["cfg_Filename"] = @"TestModel\custom.cfg";
                                //yoloAolgrithm.InitParamNames["weights_Filename"] = @"TestModel\custom_last.weights";
                                //yoloAolgrithm.InitParamNames["typeNames_Filename"] = @"TestModel\custom.names";
                                if (!yoloAolgrithm.Init(yoloAolgrithm.InitParamNames))
                                {
                                    Addlog("Yolo算法加载失败");
                                }
                            }
                        }
                    }
                    if (cb_Ocr.IsChecked == true)
                    {
                        if (ocrAolgrithm == null)
                        {
                            ocrAolgrithm = AlgorithmItems.FirstOrDefault(a => a.AlgorithmType==AlgorithmTypes.OCR);
                            if (ocrAolgrithm != null)
                            {
                                if (!ocrAolgrithm.Init(null))
                                {
                                    Addlog("Yolo算法加载失败");
                                }
                            }
                        }
                    }
                    hYCamera = new Camera.HYCamera_HIKArea(Camera.CameraConnectTypes.GigE, ConfigurationManager.AppSettings["HKSN"]);
                    if (hYCamera.Open())
                    {
                        Addlog("相机打开成功");
                        hYCamera.NewImageEvent -= HYCamera_NewImageEvent;
                        hYCamera.NewImageEvent += HYCamera_NewImageEvent;
                        hYCamera.ContinousGrab();
                    }
                }
                else
                {
                    hYCamera.Close();
                    hYCamera.Dispose();
                    hYCamera = null;
                    try
                    {
                        if (yoloAolgrithm != null)
                        {
                            if (yoloAolgrithm.IsInit)
                            {
                                yoloAolgrithm.UnInit();
                            }
                            yoloAolgrithm = null;
                        }
                        if (openposeAolgrithm != null)
                        {
                            if (openposeAolgrithm.IsInit)
                            {
                                openposeAolgrithm.UnInit();
                            }
                            openposeAolgrithm = null;
                        }
                        if (ocrAolgrithm != null)
                        {
                            if (ocrAolgrithm.IsInit)
                            {
                                ocrAolgrithm.UnInit();
                            }
                            ocrAolgrithm = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        Addlog(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Addlog(ex.ToString());
            }
        }
        HY.Devices.Camera.IHYCamera hYCamera;
        WriteableBitmapUtils writeableBitmapUtils = null;
        private static readonly object lockyolo = new object(), lockOpenpose = new object(), lockOcr = new object();
        bool yolo = false, openpose = false, ocr = false;


        private Stopwatch stopwatch_Yolo = new Stopwatch();
        private Stopwatch stopwatch_Openpose = new Stopwatch();
        private Stopwatch stopwatch_OCR = new Stopwatch();
        private void HYCamera_NewImageEvent(object sender, Bitmap bitmap)
        {
            try
            {
                if (hYCamera?.IsGrabbing != true)
                {
                    return;
                }
                if (bitmap == null)
                {
                    return;
                }
                if (writeableBitmapUtils == null)
                {
                    writeableBitmapUtils = new WriteableBitmapUtils();
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        writeableBitmapUtils.InitialWriteableBitmap(bitmap.Width, bitmap.Height, bitmap.PixelFormat);
                        img.Source = writeableBitmapUtils.WriteableBitmap;
                    });
                }
                if (writeableBitmapUtils.BitmapToBytes(bitmap, out byte[] buffer))
                {
                    Dictionary<string, dynamic> actionParams = new Dictionary<string, dynamic>();
                    IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    actionParams["size"] = buffer.Length;
                    actionParams["Intptr"] = ptr;
                    if (yoloAolgrithm?.IsInit == true)
                    {
                        if (!yolo)
                        {
                            lock (lockyolo)
                            {
                                yolo = true;
                                stopwatch_Yolo.Restart();
                                var yoloRet = yoloAolgrithm.DoAction(actionParams);
                                Addlog($"YOLO结果" + stopwatch_Yolo.ElapsedMilliseconds + JsonUtils.JsonSerialize(yoloRet["result"]));
                                yolo = false;
                            }
                        }
                    }
                    if (openposeAolgrithm?.IsInit == true)
                    {
                        if (!openpose)
                        {
                            lock (lockOpenpose)
                            {
                                openpose = true;
                                stopwatch_Openpose.Restart();
                                var openposeRet = openposeAolgrithm.DoAction(actionParams);
                                Addlog($"Openpose结果" + stopwatch_Openpose.ElapsedMilliseconds);
                                DrawKeyPoints(bitmap, openposeRet["result"] as List<OpenposeModel.BodyPoint>);
                                openpose = false;
                            }
                        }
                    }
                    if (ocrAolgrithm?.IsInit == true)
                    {
                        if (!ocr)
                        {
                            lock (lockOcr)
                            {
                                ocr = true;
                                stopwatch_OCR.Restart();
                                var ocrRet = ocrAolgrithm.DoAction(actionParams);
                                Addlog($"OCR结果" + stopwatch_OCR.ElapsedMilliseconds + ocrRet["result"]);
                                ocr = false;
                            }
                        }
                    }
                }
                writeableBitmapUtils.GetImage(bitmap);
            }
            catch (Exception ex)
            {

                Addlog(ex.ToString());
            }
            finally
            {
                bitmap?.Dispose();
            }
        }
        private void DrawKeyPoints(System.Drawing.Bitmap bitmap, List<OpenposeModel.BodyPoint> bodyPoints)
        {
            if (bodyPoints != null)
            {
                Graphics graphics = Graphics.FromImage(bitmap);
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.SmoothingMode = SmoothingMode.HighQuality;

                graphics.CompositingQuality = CompositingQuality.HighQuality;

                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                int boldLine = 15;
                int boldPoint = 20;
                Brush brushPoint = Brushes.Green;
                float minProb = 0.0f;
                foreach (var bodyPoint in bodyPoints)
                {
                    List<PointF> eye_r = new List<PointF>();
                    if (bodyPoint.Nose.Score > minProb)
                    {
                        eye_r.Add(new PointF(bodyPoint.Nose.X, bodyPoint.Nose.Y));
                    }
                    if (bodyPoint.REye.Score > minProb)
                    {
                        eye_r.Add(new PointF(bodyPoint.REye.X, bodyPoint.REye.Y));
                    }
                    if (bodyPoint.REar.Score > minProb)
                    {
                        eye_r.Add(new PointF(bodyPoint.REar.X, bodyPoint.REar.Y));
                    }
                    if (eye_r.Count > 1)
                    {
                        graphics.DrawLines(new Pen(Brushes.YellowGreen, boldLine), eye_r.ToArray());
                        foreach (var item in eye_r)
                        {
                            graphics.DrawEllipse(new Pen(brushPoint, boldPoint), item.X - 2 / boldPoint, item.Y, boldPoint, boldPoint);
                        }
                    }
                    List<PointF> eye_l = new List<PointF>();
                    if (bodyPoint.Nose.Score > minProb)
                    {
                        eye_l.Add(new PointF(bodyPoint.Nose.X, bodyPoint.Nose.Y));
                    }
                    if (bodyPoint.LEye.Score > minProb)
                    {
                        eye_l.Add(new PointF(bodyPoint.LEye.X, bodyPoint.LEye.Y));
                    }
                    if (bodyPoint.LEar.Score > minProb)
                    {
                        eye_l.Add(new PointF(bodyPoint.LEar.X, bodyPoint.LEar.Y));
                    }
                    if (eye_l.Count > 1)
                    {
                        graphics.DrawLines(new Pen(Brushes.YellowGreen, boldLine), eye_l.ToArray());
                        foreach (var item in eye_l)
                        {
                            graphics.DrawEllipse(new Pen(brushPoint, boldPoint), item.X - 2 / boldPoint, item.Y, boldPoint, boldPoint);
                        }
                    }
                    List<PointF> hip = new List<PointF>();
                    if (bodyPoint.Nose.Score > minProb)
                    {
                        hip.Add(new PointF(bodyPoint.Nose.X, bodyPoint.Nose.Y));
                    }
                    if (bodyPoint.Neck.Score > minProb)
                    {
                        hip.Add(new PointF(bodyPoint.Neck.X, bodyPoint.Neck.Y));
                    }
                    if (bodyPoint.MidHip.Score > minProb)
                    {
                        hip.Add(new PointF(bodyPoint.MidHip.X, bodyPoint.MidHip.Y));
                    }
                    if (hip.Count > 1)
                    {
                        graphics.DrawLines(new Pen(Brushes.Red, boldLine), hip.ToArray());
                        foreach (var item in hip)
                        {
                            graphics.DrawEllipse(new Pen(brushPoint, boldPoint), item.X - 2 / boldPoint, item.Y, boldPoint, boldPoint);
                        }
                    }
                    List<PointF> shoudlerR = new List<PointF>();
                    if (bodyPoint.Neck.Score > minProb)
                    {
                        shoudlerR.Add(new PointF(bodyPoint.Neck.X, bodyPoint.Neck.Y));
                    }
                    if (bodyPoint.RShoulder.Score > minProb)
                    {
                        shoudlerR.Add(new PointF(bodyPoint.RShoulder.X, bodyPoint.RShoulder.Y));
                    }
                    if (bodyPoint.RElbow.Score > minProb)
                    {
                        shoudlerR.Add(new PointF(bodyPoint.RElbow.X, bodyPoint.RElbow.Y));
                    }
                    if (bodyPoint.RWrist.Score > minProb)
                    {
                        shoudlerR.Add(new PointF(bodyPoint.RWrist.X, bodyPoint.RWrist.Y));
                    }
                    if (shoudlerR.Count > 1)
                    {
                        graphics.DrawLines(new Pen(Brushes.Orange, boldLine), shoudlerR.ToArray());
                        foreach (var item in shoudlerR)
                        {
                            graphics.DrawEllipse(new Pen(brushPoint, boldPoint), item.X - 2 / boldPoint, item.Y, boldPoint, boldPoint);
                        }
                    }
                    List<PointF> shoudlerL = new List<PointF>();
                    if (bodyPoint.Neck.Score > minProb)
                    {
                        shoudlerL.Add(new PointF(bodyPoint.Neck.X, bodyPoint.Neck.Y));
                    }
                    if (bodyPoint.LShoulder.Score > minProb)
                    {
                        shoudlerL.Add(new PointF(bodyPoint.LShoulder.X, bodyPoint.LShoulder.Y));
                    }
                    if (bodyPoint.LElbow.Score > minProb)
                    {
                        shoudlerL.Add(new PointF(bodyPoint.LElbow.X, bodyPoint.LElbow.Y));
                    }
                    if (bodyPoint.LWrist.Score > minProb)
                    {
                        shoudlerL.Add(new PointF(bodyPoint.LWrist.X, bodyPoint.LWrist.Y));
                    }
                    if (shoudlerL.Count > 1)
                    {
                        graphics.DrawLines(new Pen(Brushes.Orange, boldLine), shoudlerL.ToArray());
                        foreach (var item in shoudlerL)
                        {
                            graphics.DrawEllipse(new Pen(brushPoint, boldPoint), item.X - 2 / boldPoint, item.Y, boldPoint, boldPoint);
                        }
                    }
                    List<PointF> HipR = new List<PointF>();
                    if (bodyPoint.MidHip.Score > minProb)
                    {
                        HipR.Add(new PointF(bodyPoint.MidHip.X, bodyPoint.MidHip.Y));
                    }
                    if (bodyPoint.RHip.Score > minProb)
                    {
                        HipR.Add(new PointF(bodyPoint.RHip.X, bodyPoint.RHip.Y));
                    }
                    if (bodyPoint.RKnee.Score > minProb)
                    {
                        HipR.Add(new PointF(bodyPoint.RKnee.X, bodyPoint.RKnee.Y));
                    }
                    if (bodyPoint.RAnkle.Score > minProb)
                    {
                        HipR.Add(new PointF(bodyPoint.RAnkle.X, bodyPoint.RAnkle.Y));
                    }
                    if (HipR.Count > 1)
                    {
                        graphics.DrawLines(new Pen(Brushes.Green, boldLine), HipR.ToArray());
                        foreach (var item in HipR)
                        {
                            graphics.DrawEllipse(new Pen(brushPoint, boldPoint), item.X - 2 / boldPoint, item.Y, boldPoint, boldPoint);
                        }
                    }
                    List<PointF> HipL = new List<PointF>();
                    if (bodyPoint.MidHip.Score > minProb)
                    {
                        HipL.Add(new PointF(bodyPoint.MidHip.X, bodyPoint.MidHip.Y));
                    }
                    if (bodyPoint.LHip.Score > minProb)
                    {
                        HipL.Add(new PointF(bodyPoint.LHip.X, bodyPoint.LHip.Y));
                    }
                    if (bodyPoint.LKnee.Score > minProb)
                    {
                        HipL.Add(new PointF(bodyPoint.LKnee.X, bodyPoint.LKnee.Y));
                    }
                    if (bodyPoint.LAnkle.Score > minProb)
                    {
                        HipL.Add(new PointF(bodyPoint.LAnkle.X, bodyPoint.LAnkle.Y));
                    }
                    if (HipL.Count > 1)
                    {
                        graphics.DrawLines(new Pen(Brushes.Green, boldLine), HipL.ToArray());
                        foreach (var item in HipL)
                        {
                            graphics.DrawEllipse(new Pen(brushPoint, boldPoint), item.X - 2 / boldPoint, item.Y, boldPoint, boldPoint);
                        }
                    }

                }
                graphics.Dispose();
            }

        }
    }
    public class ParamModel : NotifyPropertyObject
    {
        public string ParamName { get; set; }
        private dynamic _paramValue;

        public dynamic ParamValue
        {
            get { return _paramValue; }
            set { _paramValue = value; RaisePropertyChanged(); }
        }
        private ICommand _selectFileCmd;

        public ICommand SelectFileCmd => _selectFileCmd = _selectFileCmd ?? new DelegateCommand(() =>
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Filter = "所有文件(*.*)|*.*|图片文件|*.jpg;*.png;*.jpeg;*.bmp";
            if (openFileDialog.ShowDialog() == true)
            {
                ParamValue = openFileDialog.FileName;
            }
        });

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
