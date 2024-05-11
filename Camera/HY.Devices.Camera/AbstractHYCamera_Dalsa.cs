using DALSA.SaperaLT.SapClassBasic;
using HY.Devices.Camera.Utils;
using System;
using System.Drawing;

namespace HY.Devices.Camera
{
    /// <summary>
    /// DALSA相机
    /// </summary>
    public abstract class AbstractHYCamera_Dalsa : IHYCamera
    {
        public abstract CameraTypes CameraType { get; }
        public virtual bool IsConnected { get; protected set; }
        public virtual bool IsGrabbing { get; protected set; }
        public uint CameraIndex { get; }
        public string CameraName { get; private set; }
        private SapLocation m_ServerLocation;   // 设备的连接地址
        private SapAcqDevice m_AcqDevice;       // 采集设备
        private SapBuffer m_Buffers;            // 缓存对象
        private SapAcqDeviceToBuf m_Xfer;       // 传输对象
        private string _serialNum = "";
        private string _configPath = "";
        private double _lineRate;
        private int _channel = 1;
        private BmpImageUtil m_bmpImage = new BmpImageUtil();
        public AbstractHYCamera_Dalsa(string serialNum, string configPath, int channel)
        {
            _serialNum = serialNum.Substring(1, serialNum.Length - 1);
            _configPath = configPath;
            _channel = channel;
        }


        /// <summary>
        /// 实例化相机
        /// </summary>
        /// <param name="serialNum"></param>
        /// <param name="channel"></param>
        public AbstractHYCamera_Dalsa(string serialNum, int channel) : this(serialNum, "", channel) { }
        private bool GetCameraInfo(string SerialNum, out string sCameraName)
        {
            sCameraName = "";
            bool isFind = false;
            string serverName = "";
            for (int serverIndex = 0; serverIndex < SapManager.GetServerCount(); serverIndex++)
            {
                if (SapManager.GetResourceCount(serverIndex, SapManager.ResourceType.AcqDevice) != 0)
                {

                    //string a = SapManager.GetSerialNumber(serverIndex);
                    if (SapManager.GetSerialNumber(serverIndex).Contains(SerialNum))
                    {
                        serverName = SapManager.GetServerName(serverIndex);
                        isFind = true;
                        break;
                    }
                }
            }
            sCameraName = serverName;
            return isFind;
        }
        private void DestroyObjects()
        {
            if (m_Xfer != null && m_Xfer.Initialized)
                m_Xfer.Destroy();
            if (m_Buffers != null && m_Buffers.Initialized)
                m_Buffers.Destroy();
            if (m_AcqDevice != null && m_AcqDevice.Initialized)
                m_AcqDevice.Destroy();
        }

        private void DisposeObjects()
        {
            if (m_Xfer != null)
            { m_Xfer.Dispose(); m_Xfer = null; }
            if (m_Buffers != null)
            { m_Buffers.Dispose(); m_Buffers = null; }
            if (m_AcqDevice != null)
            { m_AcqDevice.Dispose(); m_AcqDevice = null; }
        }
        private void m_Xfer_XferNotify(object sender, SapXferNotifyEventArgs argsNotify)
        {
            try
            {
                if (argsNotify.Trash) return;
                //获取m_Buffers的地址（指针），只要知道了图片内存的地址，其实就能有各种办法搞出图片了（例如转成Bitmap）
                IntPtr addr;
                m_Buffers.GetAddress(out addr);

                //观察buffer中的图片的一些属性值，语句后注释里面的值是可能的值
                int count = m_Buffers.Count;  //2
                SapFormat format = m_Buffers.Format;  //Uint8
                double rate = m_Buffers.FrameRate;  //30.0，连续采集时，这个值会动态变化
                int height = m_Buffers.Height;  //2800
                int width = m_Buffers.Width;  //4096
                int pixd = m_Buffers.PixelDepth;  //8
                                                  //int frameSize = width * height * pixd / 8;
                                                  //m_bmpImage.ReleaseImage();
                                                  //m_bmpImage.CreateImage(width, height, pixd, _channel);
                                                  //m_bmpImage.WriteImageData(addr,frameSize);

                int m_StartFrame = m_Buffers.Index;
                string m_Option = "-format bmp";
                string tempFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}temp.bmp";
                m_Buffers.Save(tempFilePath, m_Option, m_StartFrame, 0);
                Bitmap bitmap = new Bitmap(tempFilePath);
                DalsaNewImageEvent?.Invoke(this, bitmap);
            }
            catch
            {
            }
        }
        public virtual bool Close()
        {
            try
            {
                if (IsGrabbing)
                {
                    StopGrab();
                }
                DestroyObjects();
                DisposeObjects();
                IsConnected = false;
                return true;
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
                    return true;
                }
                IsGrabbing = m_Xfer.Grab();
                return IsGrabbing;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual void Dispose()
        {
            Close();
        }
        public virtual bool GetExposureTime(out double exposureTime)
        {
            try
            {
                return m_AcqDevice.GetFeatureValue("ExposureTime", out exposureTime);
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
                return m_AcqDevice.GetFeatureValue("FrameRate", out frameRate);
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
                return m_AcqDevice.GetFeatureValue("Gain", out gain);
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
                return m_AcqDevice.GetFeatureValue("Height", out height);
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual bool GetImageWidth(out uint width)
        {
            try
            {
                return m_AcqDevice.GetFeatureValue("Width", out width);
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
                model = TriggerMode.OFF;
                return false;
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
                triggerSource = TriggerSources.Line0;
                return false;
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
                bitmap = null;
                return m_Xfer.Snap();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual bool Open()
        {
            if (IsConnected)
            {
                Close();
            }
            string Name;

            if (!GetCameraInfo(_serialNum, out Name))
            {
                throw new Exception($"未识别到{_serialNum}的相机");
            }

            m_ServerLocation = new SapLocation(Name, 0);

            m_AcqDevice = _configPath != "" ?
                new SapAcqDevice(m_ServerLocation, _configPath) : new SapAcqDevice(m_ServerLocation, false);

            if (m_AcqDevice.Create() == false)
            {
                DestroyObjects();
                DisposeObjects();
                throw new Exception($"创建相机失败");
            }


            // 创建缓存对象
            if (SapBuffer.IsBufferTypeSupported(m_ServerLocation, SapBuffer.MemoryType.ScatterGather))
            {
                m_Buffers = new SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGather);
            }
            else
            {
                m_Buffers = new SapBufferWithTrash(2, m_AcqDevice, SapBuffer.MemoryType.ScatterGatherPhysical);
            }

            if (m_Buffers.Create() == false)
            {
                DestroyObjects();
                DisposeObjects();

                throw new Exception($"创建缓存对象失败");
            }

            //  m_AcqDevice.SetFeatureValue("AcquisitionLineRate", _lineRate); //注意：行频在相机工作时不能设置（曝光、增益可以），最好在初始化阶段设置

            // 创建传输对象
            m_Xfer = new SapAcqDeviceToBuf(m_AcqDevice, m_Buffers);
            m_Xfer.XferNotify += new SapXferNotifyHandler(m_Xfer_XferNotify);
            m_Xfer.XferNotifyContext = this;
            m_Xfer.Pairs[0].EventType = SapXferPair.XferEventType.EndOfFrame;
            m_Xfer.Pairs[0].Cycle = SapXferPair.CycleMode.NextWithTrash;
            if (m_Xfer.Pairs[0].Cycle != SapXferPair.CycleMode.NextWithTrash)
            {
                DestroyObjects();
                DisposeObjects();
                throw new Exception($"创建传输对象失败");
            }
            if (m_Xfer.Create() == false)
            {
                DestroyObjects();
                DisposeObjects();
                return false;
            }
            return IsConnected = true;
        }
        public virtual bool SaveImage(string savePath)
        {
            try
            {
                return false;
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
                return m_AcqDevice.SetFeatureValue("ExposureTime", exposureTime);
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
                switch (mode)
                {
                    case AutoMode.OFF:
                        break;
                    case AutoMode.Once:
                        break;
                    case AutoMode.Continous:
                        break;
                    default:
                        break;
                }
                return false;
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
                return m_AcqDevice.SetFeatureValue("FrameRate", frameRate);
            }
            catch (Exception)
            {

                throw;
            }
        }
        public virtual bool SetGain(double gain)
        {
            try
            {
                return m_AcqDevice.SetFeatureValue("Gain", (double)gain);
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
                switch (mode)
                {
                    case AutoMode.OFF:
                        break;
                    case AutoMode.Once:
                        break;
                    case AutoMode.Continous:
                        break;
                    default:
                        break;
                }
                return false;
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
                return m_AcqDevice.SetFeatureValue("SensorHeight", height);
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
                return m_AcqDevice.SetFeatureValue("SensorWidth", width);
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
                return false;
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
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual bool SoftWareTrigger()
        {
            try
            {
                return m_Xfer.Snap();
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
                if (IsGrabbing)
                {
                    bool b = m_Xfer.Freeze();
                    if (b)
                    {
                        IsGrabbing = false;
                    }
                    return b;
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        event EventHandler<Bitmap> DalsaNewImageEvent;
        object lockObj = new object();
        event EventHandler<Bitmap> IHYCamera.NewImageEvent
        {
            add
            {
                lock (lockObj)
                {
                    DalsaNewImageEvent += value;
                }
            }

            remove
            {
                lock (lockObj)
                {
                    DalsaNewImageEvent -= value;
                }
            }
        }
    }
}
