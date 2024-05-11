using Basler.Pylon;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;

namespace HY.Devices.Camera
{
    /// <summary>
    /// Basler相机
    /// </summary>
    public abstract class AbstractHYCamera_Basler : IHYCamera
    {
        private ICameraInfo _cameraInfo;
        private Basler.Pylon.Camera _camera;
        private int imageCount = 0;
        private int errorCount = 0;
        private Object monitor = new Object();
        private Bitmap latestFrame = null;
        private PixelDataConverter converter = new PixelDataConverter();
        private System.Diagnostics.Stopwatch stopwatch;
        private int frameDurationTicks;
        private readonly double RENDERFPS = 10;
        event EventHandler<Bitmap> BaslerNewImageEvent;
        object lockObj = new object();
        event EventHandler<Bitmap> IHYCamera.NewImageEvent
        {
            add
            {
                lock (lockObj)
                {
                    BaslerNewImageEvent += value;
                }
            }

            remove
            {
                lock (lockObj)
                {
                    BaslerNewImageEvent -= value;
                }
            }
        }

        public abstract CameraTypes CameraType { get; }

        public uint CameraIndex { get; }

        public string CameraName { get; protected set; }

        public CameraConnectTypes CameraConnectType { get; set; } = CameraConnectTypes.GigE;

        public virtual bool IsConnected => _camera?.IsConnected == true;

        public virtual bool IsGrabbing => _camera?.StreamGrabber.IsGrabbing == true;
        public virtual bool GetImageWidth(out uint width)
        {
            try
            {
                width = uint.MaxValue;
                if (_camera != null)
                {
                    width = (uint)_camera.Parameters[PLCamera.Width].GetValue();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool SetImageWidth(uint width)
        {
            try
            {
                if (_camera != null)
                {
                    return _camera.Parameters[PLCamera.Width].TrySetValue((long)width);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool GetImageHeight(out uint height)
        {
            try
            {
                height = uint.MaxValue;
                if (_camera != null)
                {
                    height = (uint)_camera.Parameters[PLCamera.Height].GetValue();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool SetImageHeight(uint height)
        {
            try
            {
                if (_camera != null)
                {
                    return _camera.Parameters[PLCamera.Height].TrySetValue((long)height);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool GetExposureTime(out double exposureTime)
        {
            try
            {
                exposureTime = double.NaN;
                if (_camera != null)
                {
                    exposureTime = _camera.Parameters[PLCamera.ExposureTime].GetValue();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool SetExposureTime(double exposureTime)
        {
            try
            {
                if (_camera != null)
                {
                    return _camera.Parameters[PLCamera.ExposureTime].TrySetValue(exposureTime);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool SetAutoExposure(AutoMode mode)
        {
            try
            {
                if (_camera != null)
                {
                    string value = PLCamera.ExposureAuto.Off;
                    switch (mode)
                    {
                        case AutoMode.OFF:
                            value = PLCamera.ExposureAuto.Off;
                            break;
                        case AutoMode.Once:
                            value = PLCamera.ExposureAuto.Once;
                            break;
                        case AutoMode.Continous:
                            value = PLCamera.ExposureAuto.Continuous;
                            break;
                        default:
                            break;
                    }
                    return _camera.Parameters[PLCamera.ExposureAuto].TrySetValue(value);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual bool GetGain(out double gain)
        {
            try
            {
                gain = double.NaN;
                if (_camera != null)
                {
                    gain = _camera.Parameters[PLCamera.Gain].GetValue();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool SetGain(double gain)
        {
            try
            {
                if (_camera != null)
                {
                    return _camera.Parameters[PLCamera.ExposureTime].TrySetValue(gain);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public virtual bool SetAutoGain(AutoMode mode)
        {
            try
            {
                if (_camera != null)
                {
                    string value = PLCamera.GainAuto.Off;
                    switch (mode)
                    {
                        case AutoMode.OFF:
                            value = PLCamera.GainAuto.Off;
                            break;
                        case AutoMode.Once:
                            value = PLCamera.GainAuto.Once;
                            break;
                        case AutoMode.Continous:
                            value = PLCamera.GainAuto.Continuous;
                            break;
                        default:
                            break;
                    }
                    return _camera.Parameters[PLCamera.GainAuto].TrySetValue(value);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual bool GetFrameRate(out double frameRate)
        {
            try
            {
                frameRate = float.NaN;
                if (_camera != null)
                {
                    frameRate = (float)_camera.Parameters[PLCamera.FrameDuration].GetValue();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool SetFrameRate(double frameRate)
        {
            try
            {
                if (_camera != null)
                {
                    return _camera.Parameters[PLCamera.FrameDuration].TrySetValue((long)frameRate);
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool GetTriggerModel(out TriggerMode model)
        {
            try
            {
                model = default(TriggerMode);
                if (_camera != null)
                {
                    string ret = _camera.Parameters[PLCamera.TriggerMode].GetValue();
                    model = ret.ToLower() == "on" ? TriggerMode.ON : TriggerMode.OFF;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool SetTriggerModel(TriggerMode model)
        {
            try
            {
                if (_camera != null)
                {
                    return _camera.Parameters[PLCamera.TriggerMode].TrySetValue(model == TriggerMode.ON ? "On" : "Off");
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual bool GetTriggerSource(out TriggerSources triggerSource)
        {
            try
            {
                triggerSource = default(TriggerSources);
                if (_camera != null)
                {
                    string ret = _camera.Parameters[PLCamera.TriggerSource].GetValue();
                    triggerSource = ret == "Software" ? TriggerSources.Soft : TriggerSources.Line0;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool SetTriggerSource(TriggerSources triggerSources)
        {
            try
            {
                if (_camera != null)
                {
                    return _camera.Parameters[PLCamera.TriggerSource].TrySetValue(triggerSources == TriggerSources.Soft ? "Software" : "Line1");
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public AbstractHYCamera_Basler(CameraConnectTypes connectType, uint index)
        {
            CameraConnectType = connectType;
            CameraIndex = index;
            try
            {
                InitialCamera(connectType, (int)CameraIndex);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public AbstractHYCamera_Basler(string serialNumber)
        {
            try
            {
                _camera = new Basler.Pylon.Camera(serialNumber);
                RegisterEvents();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual bool Open()
        {
            try
            {
                if (IsConnected)
                {
                    _camera.Close();
                }
                ICamera camera = _camera.Open();
                return _camera.IsOpen;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void InitialCamera(CameraConnectTypes connectType, int index)
        {
            try
            {
                string deviceType = string.Empty;
                switch (connectType)
                {
                    case CameraConnectTypes.GigE:
                        deviceType = DeviceType.GigE;
                        break;
                    case CameraConnectTypes.Usb:
                        deviceType = DeviceType.Usb;
                        break;
                    case CameraConnectTypes.CameraLink:
                        deviceType = DeviceType.CameraLink;
                        break;
                    case CameraConnectTypes.IEEE1394:
                        deviceType = DeviceType.IEEE1394;
                        break;
                    default:
                        break;
                }
                List<ICameraInfo> cameras = new List<ICameraInfo>();
                if (string.IsNullOrEmpty(deviceType))
                {
                    cameras = CameraFinder.Enumerate();
                }
                else
                {
                    cameras = CameraFinder.Enumerate(deviceType);
                }
                if (cameras.Count > CameraIndex)
                {
                    _cameraInfo = cameras[index];
                    CameraName = _cameraInfo[CameraInfoKey.FriendlyName];
                    _camera = new Basler.Pylon.Camera(_cameraInfo);
                    stopwatch = new System.Diagnostics.Stopwatch();
                    double frametime = 1 / RENDERFPS;
                    frameDurationTicks = (int)(System.Diagnostics.Stopwatch.Frequency * frametime);
                    RegisterEvents();
                }
                else
                {
                    throw new Exception("不存在该Basler相机");
                }

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public void DestroyCamera()
        {
            try
            {
                if (IsGrabbing)
                {
                    StopGrab();
                }
                if (_camera?.IsOpen == true)
                {
                    Close();
                }
                if (_camera?.IsOpen == true)
                {
                    UnRegisterEvents();
                    _camera.Dispose();
                    _camera = null;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        protected void ResetGrabStatistics()
        {
            Interlocked.Exchange(ref imageCount, 0);
            Interlocked.Exchange(ref errorCount, 0);
        }
        protected void ClearLatestFrame()
        {
            ResetGrabStatistics();
            lock (monitor)
            {
                if (latestFrame != null)
                {
                    latestFrame.Dispose();
                    latestFrame = null;
                }
            }
        }
        private void RegisterEvents()
        {
            _camera.CameraOpened += camera_CameraOpened;
            _camera.ConnectionLost += camera_ConnectionLost;
            _camera.CameraClosed += camera_CameraClosed;
            _camera.StreamGrabber.GrabStarted += StreamGrabber_GrabStarted;
            _camera.StreamGrabber.ImageGrabbed += StreamGrabber_ImageGrabbed;
            _camera.StreamGrabber.GrabStopped += StreamGrabber_GrabStopped;
        }
        private void UnRegisterEvents()
        {
            _camera.CameraOpened -= camera_CameraOpened;
            _camera.ConnectionLost -= camera_ConnectionLost;
            _camera.CameraClosed -= camera_CameraClosed;
            _camera.StreamGrabber.GrabStarted -= StreamGrabber_GrabStarted;
            _camera.StreamGrabber.ImageGrabbed -= StreamGrabber_ImageGrabbed;
            _camera.StreamGrabber.GrabStopped -= StreamGrabber_GrabStopped;
        }
        private void StreamGrabber_GrabStopped(object sender, GrabStopEventArgs e)
        {
        }
        private void StreamGrabber_ImageGrabbed(object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult grabResult = e?.GrabResult;
                if (grabResult == null)
                {
                    return;
                }
                using (grabResult)
                {
                    if (grabResult.GrabSucceeded && grabResult.IsValid)
                    {
                        if (!stopwatch.IsRunning || stopwatch.ElapsedTicks >= frameDurationTicks)
                        {
                            stopwatch.Restart();
                            if (IsMonoData(grabResult))
                            {
                                Bitmap bitmap = new Bitmap(grabResult.Width, grabResult.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                                System.Drawing.Imaging.ColorPalette cp = bitmap.Palette;
                                for (int i = 0; i < 256; i++)
                                {
                                    cp.Entries[i] = Color.FromArgb(i, i, i);
                                }
                                bitmap.Palette = cp;
                                BaslerNewImageEvent?.Invoke(this, bitmap);
                            }
                            else
                            {
                                Bitmap bitmap = new Bitmap(grabResult.Width, grabResult.Height, System.Drawing.Imaging.PixelFormat.Format32bppRgb);
                                System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
                                converter.OutputPixelFormat = PixelType.BGRA8packed;
                                IntPtr ptrBmp = bmpData.Scan0;
                                converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
                                bitmap.UnlockBits(bmpData);
                                BaslerNewImageEvent?.Invoke(this, bitmap);
                            }
                        }
                    }
                }
            }
            catch
            {


            }
        }

        private void StreamGrabber_GrabStarted(object sender, EventArgs e)
        {
        }

        private void camera_CameraClosed(object sender, EventArgs e)
        {
        }

        private void camera_ConnectionLost(object sender, EventArgs e)
        {
        }

        private void camera_CameraOpened(object sender, EventArgs e)
        {
        }

        public virtual bool SoftWareTrigger()
        {
            try
            {
                if (_camera.Parameters[PLCamera.TriggerMode].GetValueOrDefault(PLCamera.TriggerMode.Off) == PLCamera.TriggerMode.Off)
                {
                    return false;
                }
                if (_camera.CanWaitForFrameTriggerReady)
                {
                    _camera.WaitForFrameTriggerReady(1000, TimeoutHandling.ThrowException);
                }

                _camera.ExecuteSoftwareTrigger();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool GrabOne(out Bitmap bitmap)
        {
            try
            {
                if (IsGrabbing)
                {
                    StopGrab();
                }
                bitmap = null;
                IGrabResult grabResult = _camera.StreamGrabber.GrabOne(5000);
                if (grabResult != null)
                {
                    using (grabResult)
                    {
                        if (grabResult.GrabSucceeded && grabResult.IsValid)
                        {
                            if (IsMonoData(grabResult))
                            {
                                bitmap = new Bitmap(grabResult.Width, grabResult.Height, System.Drawing.Imaging.PixelFormat.Format8bppIndexed);

                                System.Drawing.Imaging.ColorPalette cp = bitmap.Palette;
                                for (int i = 0; i < 256; i++)
                                {
                                    cp.Entries[i] = Color.FromArgb(i, i, i);
                                }
                                bitmap.Palette = cp;
                            }
                            else
                            {
                                bitmap = new Bitmap(grabResult.Width, grabResult.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                                System.Drawing.Imaging.BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bitmap.PixelFormat);
                                converter.OutputPixelFormat = PixelType.BGRA8packed;
                                IntPtr ptrBmp = bmpData.Scan0;
                                converter.Convert(ptrBmp, bmpData.Stride * bitmap.Height, grabResult);
                                bitmap.UnlockBits(bmpData);
                            }
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool ContinousGrab()
        {
            try
            {
                if (IsGrabbing)
                {
                    StopGrab();
                }
                ResetGrabStatistics();
                _camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                _camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool StopGrab()
        {
            try
            {
                _camera.StreamGrabber.Stop();
                if (stopwatch.IsRunning)
                {
                    stopwatch.Stop();
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool SaveImage(string savePath)
        {
            try
            {
                if (GrabOne(out Bitmap bitmap))
                {
                    bitmap.Save(savePath);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual bool Close()
        {
            try
            {
                if (_camera.IsOpen)
                {
                    ResetGrabStatistics();
                    ClearLatestFrame();
                    _camera.Close();
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual void Dispose()
        {
            try
            {
                DestroyCamera();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private Boolean IsMonoData(IGrabResult iGrabResult)//判断图像是否为黑白格式
        {
            switch (iGrabResult.PixelTypeValue)
            {
                case PixelType.Mono1packed:
                case PixelType.Mono2packed:
                case PixelType.Mono4packed:
                case PixelType.Mono8:
                case PixelType.Mono8signed:
                case PixelType.Mono10:
                case PixelType.Mono10p:
                case PixelType.Mono10packed:
                case PixelType.Mono12:
                case PixelType.Mono12p:
                case PixelType.Mono12packed:
                case PixelType.Mono16:
                    return true;
                default:
                    return false;
            }
        }
    }
}
