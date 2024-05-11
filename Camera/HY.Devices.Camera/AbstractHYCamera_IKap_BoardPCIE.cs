using HY.Devices.Camera.Utils;
using IKapBoardClassLibrary;
using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace HY.Devices.Camera
{
    /// <summary>
    /// 埃科相机
    /// </summary>
    public abstract class AbstractHYCamera_IKap_BoardPCIE : IHYCamera
    {
        #region members
        // Frame Grabber parameters
        IntPtr m_hBoard = new IntPtr(-1);                             // Frame grabber device handle 
        int nFrameSize = 0;
        int nImageWidth = 0;
        int nImageHeight = 0;
        int nDataFormat = 0;
        int nBoardBit = 0;
        int nImageType = 0;
        int nChannels = 3;
        int nDepth = 8;
        int m_nCurFrameIndex = 0;                                         // Current grab frame index
        int m_nTotalFrameCount = 5;                                     // Total frame count
        BmpImageUtil m_bmpImage = new BmpImageUtil();
        private string m_strFileName = string.Empty;
        #endregion
        public SerialPortHelper SerialPort_CaptureBoard { get; set; } = new SerialPortHelper();
        public string SerialPortCom { get; set; } = "COM13";
        public uint CameraIndex { get; }
        public string CameraName { get; private set; }
        public AbstractHYCamera_IKap_BoardPCIE(uint cameraIndex, string configFilename)
        {
            CameraIndex = cameraIndex;
            ConfigFile = configFilename;
            try
            {
                DeviceListAcq(IKapCameraConnectType, cameraIndex);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public AbstractHYCamera_IKap_BoardPCIE(string serialNumber, string configFilename)
        {
            CameraIndex = 0;
            ConfigFile = configFilename;
            try
            {
                DeviceListAcq(IKapCameraConnectType, CameraIndex);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void DeviceListAcq(IKapConnectTypes connectType, uint cameraIndex)
        {
            int ret = (int)ErrorCode.IK_RTN_OK;                                                     // Return value of IKapBoard methods
            uint nPCIeDevCount = 0;
            StringBuilder resourceName;
            uint resourceNameSize = 0;
            IKapBoard.IKAPERRORINFO tIKei = new IKapBoard.IKAPERRORINFO();

            // Get the count of PCIe interface boards
            ret = IKapBoard.IKapGetBoardCount((uint)connectType, ref nPCIeDevCount);
            if (ret != (int)ErrorCode.IK_RTN_OK || nPCIeDevCount <= cameraIndex)
            {
                throw new Exception("不存在该IKAP相机");
            }
            resourceNameSize = 0;
            resourceName = new StringBuilder(0);
            IKapBoard.IKapGetBoardName((uint)connectType, cameraIndex, resourceName, ref resourceNameSize);
            IKapBoard.IKapGetLastError(ref tIKei, true);
            if (tIKei.uErrorCode == (uint)ErrorCode.IKStatus_BufferTooSmall)
            {
                resourceName = new StringBuilder((int)resourceNameSize);
                IKapBoard.IKapGetBoardName((uint)connectType, cameraIndex, resourceName, ref resourceNameSize);
            }
            IKapBoard.IKapGetLastError(ref tIKei, true);
            if (tIKei.uErrorCode != (uint)ErrorCode.IKStatus_Success)
                CameraName = null;
            else
            {
                string title = connectType == IKapConnectTypes.IKBoardPCIE ? "PCIE Device-" : "USB Device-";
                CameraName = string.Concat(title, cameraIndex.ToString("d"), "\nName: ", resourceName);
            }
        }
        public abstract CameraTypes CameraType { get; }
        public IKapConnectTypes IKapCameraConnectType { get; set; } = IKapConnectTypes.IKBoardPCIE;
        public string ConfigFile { get; }
        public virtual bool IsConnected { get; protected set; }
        public virtual bool IsGrabbing { get; protected set; }
        public virtual bool GetImageWidth(out uint width)
        {
            try
            {
                int nWidth = 0;
                bool ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_WIDTH, ref nWidth) == (int)ErrorCode.IK_RTN_OK;
                width = (uint)nWidth;
                return ret;
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
                return IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_WIDTH, (int)width) == (int)ErrorCode.IK_RTN_OK;
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
                int nHeight = 0;
                bool ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_HEIGHT, ref nHeight) == (int)ErrorCode.IK_RTN_OK;
                height = (uint)nHeight;
                return ret;

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
                return IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_HEIGHT, (int)height) == (int)ErrorCode.IK_RTN_OK;
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
                if (!SerialPort_CaptureBoard.IsOpen())
                {
                    SerialPort_CaptureBoard.OpenPort(SerialPortCom, 9600);
                }
                if (SerialPort_CaptureBoard.IsOpen())
                {
                    string str = $"TEXP";
                    string ret = SerialPort_CaptureBoard.ReadData(str, 2000);
                    if (!double.TryParse(ret, out exposureTime))
                    {
                        return false;
                    }
                    return true;
                }
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
                if (!SerialPort_CaptureBoard.IsOpen())
                {
                    SerialPort_CaptureBoard.OpenPort(SerialPortCom, 9600);
                }
                if (SerialPort_CaptureBoard.IsOpen())
                {
                    string str = $"TEXP={exposureTime}";
                    return SerialPort_CaptureBoard.WriteDataToPort(str, 2000);
                }
                return false;
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
                return false;
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
                if (!SerialPort_CaptureBoard.IsOpen())
                {
                    SerialPort_CaptureBoard.OpenPort(SerialPortCom, 9600);
                }
                if (SerialPort_CaptureBoard.IsOpen())
                {
                    string str = $"DIGN";
                    string ret = SerialPort_CaptureBoard.ReadData(str, 2000);
                    if (!double.TryParse(ret, out gain))
                    {
                        return false;
                    }
                    return true;
                }
                return false;
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
                if (!SerialPort_CaptureBoard.IsOpen())
                {
                    SerialPort_CaptureBoard.OpenPort(SerialPortCom, 9600);
                }
                if (SerialPort_CaptureBoard.IsOpen())
                {
                    string str = $"DIGN={gain}";
                    return SerialPort_CaptureBoard.WriteDataToPort(str, 2000);
                }
                return false;
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
                return false;
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
                double fr = float.NaN;
                int ret = IKapBoard.IKapGetFrameRate(m_hBoard, ref fr);
                frameRate = (float)fr;
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                return true;
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
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public virtual bool GetTriggerModel(out TriggerMode mode)
        {
            try
            {
                mode = TriggerMode.OFF;
                int value = (int)BoardTriggerMode.IKP_BOARD_TRIGGER_MODE_VAL_INNER;
                if (IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_BOARD_TRIGGER_MODE, ref value) == (int)ErrorCode.IK_RTN_OK)
                {
                    mode = value == (int)BoardTriggerMode.IKP_BOARD_TRIGGER_MODE_VAL_INNER ? TriggerMode.OFF : TriggerMode.ON;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual bool SetTriggerModel(TriggerMode mode)
        {
            try
            {
                int ret = (int)ErrorCode.IK_RTN_OK;                                                     // Return value of IKapBoard methods

                // Set board frame trigger parameter
                switch (mode)
                {
                    case TriggerMode.OFF:
                        ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_BOARD_TRIGGER_MODE, (int)BoardTriggerMode.IKP_BOARD_TRIGGER_MODE_VAL_INNER);
                        break;
                    case TriggerMode.ON:
                        ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_BOARD_TRIGGER_MODE, (int)BoardTriggerMode.IKP_BOARD_TRIGGER_MODE_VAL_OUTTER);
                        break;
                    default:
                        break;
                }
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                return true;
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
                triggerSource = TriggerSources.Soft;
                int value = (int)BoardTriggerSource.IKP_BOARD_TRIGGER_SOURCE_VAL_GENERAL_INPUT1;
                if (IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_BOARD_TRIGGER_SOURCE, ref value) == (int)ErrorCode.IK_RTN_OK)
                {
                    triggerSource = value == (int)BoardTriggerSource.IKP_BOARD_TRIGGER_SOURCE_VAL_GENERAL_INPUT1 ? TriggerSources.Line0 : TriggerSources.Soft;
                    return true;
                }
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
                int ret = (int)ErrorCode.IK_RTN_OK;
                switch (triggerSources)
                {
                    case TriggerSources.Line0:
                        ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_BOARD_TRIGGER_SOURCE, (int)BoardTriggerSource.IKP_BOARD_TRIGGER_SOURCE_VAL_GENERAL_INPUT1);
                        break;
                    case TriggerSources.Soft:
                        ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_BOARD_TRIGGER_SOURCE, (int)BoardTriggerSource.IKP_BOARD_TRIGGER_SOURCE_VAL_SOFTWARE);
                        break;
                    default:
                        break;
                }
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                return true;
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
                    Close();
                }
                int ret = (int)ErrorCode.IK_RTN_OK;
                m_hBoard = IKapBoard.IKapOpen((uint)IKapCameraConnectType, CameraIndex);
                if (m_hBoard.Equals(new IntPtr(-1)))
                {
                    Close();
                    return false;
                }
                if (!string.IsNullOrEmpty(ConfigFile))
                {
                    ret = IKapBoard.IKapLoadConfigurationFromFile(m_hBoard, ConfigFile);
                    if (ret != (int)ErrorCode.IK_RTN_OK)
                    {
                        Close();
                        return false;
                    }
                }
                if (!setBoardParams())
                {
                    Close();
                    return false;
                }
                if (!getBoardParams())
                {
                    Close();
                    return false;
                }
                return IsConnected = true;
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
                int ret = (int)ErrorCode.IK_RTN_OK;
                bitmap = null;
                if (IsGrabbing)
                {
                    if (!StopGrab())
                    {
                        return false;
                    }
                }
                unRegisterCallback();
                m_bmpImage.ReleaseImage();
                if (!m_bmpImage.CreateImage(nImageWidth, nImageHeight, nDepth, nChannels))
                {
                    return false;
                }
                ret = IKapBoard.IKapClearGrab(m_hBoard);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                ret = IKapBoard.IKapStartGrab(m_hBoard, 1);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                ret = IKapBoard.IKapWaitGrab(m_hBoard);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                IntPtr pUserBuffer = IntPtr.Zero;
                ret = IKapBoard.IKapGetBufferAddress(m_hBoard, 0, ref pUserBuffer);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                else
                {
                    if (!m_bmpImage.WriteImageData(pUserBuffer, nFrameSize))
                    {
                        return false;
                    }
                    else
                    {
                        bitmap = m_bmpImage.m_bitmap;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                StopGrab();
            }
        }
        public virtual bool ContinousGrab()
        {
            try
            {
                if (IsGrabbing)
                {
                    if (!StopGrab())
                    {
                        return false;
                    }
                }
                unRegisterCallback();
                registerCallBack();
                int ret = IKapBoard.IKapStartGrab(m_hBoard, 0);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                return IsGrabbing = true;
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
                int ret = IKapBoard.IKapStopGrab(m_hBoard);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                IsGrabbing = false;
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
                    return m_bmpImage.SaveImage(savePath);
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
                if (!IsConnected) return true;
                // Close frame grabber
                if (!m_hBoard.Equals(-1))
                {
                    if (IsGrabbing)
                    {
                        StopGrab();
                    }
                    unRegisterCallback();
                    int ret = IKapBoard.IKapClose(m_hBoard);
                    m_hBoard = (IntPtr)(-1);
                    IsConnected = false;
                }
                SerialPort_CaptureBoard?.ClosePort();

                // free buffer
                m_bmpImage.ReleaseImage();
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        event EventHandler<Bitmap> IKapNewImageEvent;
        object lockObj = new object();
        event EventHandler<Bitmap> IHYCamera.NewImageEvent
        {
            add
            {
                lock (lockObj)
                {
                    IKapNewImageEvent += value;
                }
            }
            remove
            {
                lock (lockObj)
                {
                    IKapNewImageEvent -= value;
                }
            }
        }
        public virtual void Dispose()
        {
            Close();
        }
        #region Callback
        delegate void IKapCallBackProc(IntPtr pParam);
        private IKapCallBackProc OnGrabStartProc;
        private IKapCallBackProc OnFrameLostProc;
        private IKapCallBackProc OnTimeoutProc;
        private IKapCallBackProc OnFrameReadyProc;
        private IKapCallBackProc OnGrabStopProc;
        private IKapCallBackProc OnGrabLineEndProc;
        #endregion

        #region CallbackFunc
        // This callback function will be called on grab start
        private void OnGrabStartFunc(IntPtr pParam)
        {
            Console.WriteLine("Start grabbing image");
        }
        // This callback function will be called on frame lost
        private void OnFrameLostFunc(IntPtr pParam)
        {
            Console.WriteLine("Grab frame lost");
        }
        // This callback function will be called on grabbing timeout
        private void OnTimeoutFunc(IntPtr pParam)
        {
            Console.WriteLine("Grab image timeout");
        }
        // This callback function will be called at the frame ready
        private void OnFrameReadyFunc(IntPtr pParam)
        {
            try
            {
                Console.WriteLine("Grab frame ready");
                IntPtr hDev = (IntPtr)pParam;
                IntPtr pUserBuffer = IntPtr.Zero;
                int nFrameSize = 0;
                IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();
                IKapBoard.IKapGetInfo(hDev, (uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize);
                IKapBoard.IKapGetBufferStatus(hDev, 0, ref status);
                if (status.uFull == 1)
                {
                    m_bmpImage.ReleaseImage();
                    m_bmpImage.CreateImage(nImageWidth, nImageHeight, nDepth, nChannels);
                    // get frame grabber buffer
                    IKapBoard.IKapGetBufferAddress(hDev, 0, ref pUserBuffer);
                    // save image

                    m_bmpImage.WriteImageData(pUserBuffer, nFrameSize);
                    IKapNewImageEvent?.Invoke(this, m_bmpImage.m_bitmap);
                }
                else
                {
                    IKapNewImageEvent?.Invoke(this, null);
                }
            }
            catch (Exception ex)
            {
                IKapNewImageEvent?.Invoke(this, null);
            }
        }
        // This callback function will be called on grab stop
        private void OnGrabStopFunc(IntPtr pParam)
        {
            Console.WriteLine("Stop grabbing image");
            IntPtr hDev = (IntPtr)pParam;
            //IKAPERRORINFO pIKErrInfo;
            //memset(&pIKErrInfo, 0, sizeof(IKAPERRORINFO));
            //IKapGetLastError(&pIKErrInfo, true);
            //if (pIKErrInfo.uErrorCode != IKStatus_Success)
            //{
            //    /**************************************************************/
            //    /*The termination of the image acquisition process, Check /*“IKapGetLastError” 
            //   to get detailed error information.
            //   /*e.g. DoErrorHandle( uErrorCode )*/
            //    /**************************************************************/
            //}
        }
        // This callback function will be called on grab line end
        private void OnGrabLineEndFunc(IntPtr pParam)
        {
            IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();
            IKapBoard.IKapGetBufferStatus(pParam, 0, ref status);
            string sMsg = string.Concat("IKapBoard Callback<OnGrabLineEnd> Valid line number:", status.uLineNum.ToString("d"), "\n");
            Console.WriteLine(sMsg);
        }
        #endregion

        #region member function

        private void configureBoard()
        {
            int timeout = -1;
            ImageType imageType = ImageType.IKP_IMAGE_TYPE_VAL_MONOCHROME;
            DataFormat dataFormat = DataFormat.IKP_DATA_FORMAT_VAL_8Bit;
            int tapNum = 8;
            CCSource cCSource = CCSource.IKP_CC_SOURCE_VAL_INTEGRATION_SIGNAL1;
            IntegrationTriggerSource integrationTriggerSource = IntegrationTriggerSource.IKP_INTEGRATION_TRIGGER_SOURCE_VAL_GENERAL_INPUT1;
            IntegrationMethod integrationMethod = IntegrationMethod.IKP_INTEGRATION_METHOD_VAL_4;
            int imageWidth = 2048;
            int imageHeight = 2048;
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_SCAN_TYPE, (int)ScanType.IKP_SCAN_TYPE_VAL_AREA) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_TIME_OUT, timeout) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_TYPE, (int)imageType) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_DATA_FORMAT, (int)dataFormat) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_WIDTH, imageWidth) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_HEIGHT, imageHeight) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_TAP_NUMBER, tapNum) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_CC1_SOURCE, (int)cCSource) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_INTEGRATION_TRIGGER_SOURCE, (int)integrationTriggerSource) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_INTEGRATION_METHOD, (int)integrationMethod) != (int)ErrorCode.IK_RTN_OK)
            {
                Close();
            }
        }
        private bool demonstrateGrabMultiSync()
        {
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_COUNT, m_nTotalFrameCount) != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_TRANSFER_MODE, (int)FrameTransferMode.IKP_FRAME_TRANSFER_SYNCHRONOUS) != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            if (IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_GRAB_MODE, (int)GrabMode.IKP_GRAB_NON_BLOCK) != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            IKapBoard.IKapClearGrab(m_hBoard);
            if (IKapBoard.IKapStartGrab(m_hBoard, m_nTotalFrameCount) != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            return true;
        }
        //设置参数
        private bool setBoardParams()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            // Configure timeout
            int timeout = -1;
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_TIME_OUT, timeout);
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            // Configure grab start mode
            int grab_mode = (int)GrabMode.IKP_GRAB_NON_BLOCK;
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_GRAB_MODE, grab_mode);
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            // Configure transfer mode
            int transfer_mode = (int)FrameTransferMode.IKP_FRAME_TRANSFER_SYNCHRONOUS_NEXT_EMPTY_WITH_PROTECT;
            ret = IKapBoard.IKapSetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_TRANSFER_MODE, transfer_mode);
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            return true;
        }
        private bool getBoardParams()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            // Set image height
            ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize);
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            // Configure image buffers
            ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_WIDTH, ref nImageWidth);
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_HEIGHT, ref nImageHeight);
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_DATA_FORMAT, ref nDataFormat);
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            // Get image type
            ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_IMAGE_TYPE, ref nImageType);
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            ret = IKapBoard.IKapGetInfo(m_hBoard, (uint)INFO_ID.IKP_BOARD_BIT, ref nBoardBit);
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                return false;
            }
            if (nDataFormat == 8)
            {
                nDepth = 8;
            }
            else
            {
                nDepth = 16;
            }
            // Get format value
            switch (nImageType)
            {
                case 0:
                    nChannels = 1;
                    break;
                case 1:
                case 3:
                    nChannels = 3;
                    break;
                case 2:
                case 4:
                    nChannels = 4;
                    break;
                default:
                    break;
            }
            return true;
        }
        private void registerCallBack()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            OnGrabStartProc = new IKapCallBackProc(OnGrabStartFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStart, Marshal.GetFunctionPointerForDelegate(OnGrabStartProc), m_hBoard);

            OnFrameReadyProc = new IKapCallBackProc(OnFrameReadyFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameReady, Marshal.GetFunctionPointerForDelegate(OnFrameReadyProc), m_hBoard);

            OnFrameLostProc = new IKapCallBackProc(OnFrameLostFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameLost, Marshal.GetFunctionPointerForDelegate(OnFrameLostProc), m_hBoard);

            OnTimeoutProc = new IKapCallBackProc(OnTimeoutFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_TimeOut, Marshal.GetFunctionPointerForDelegate(OnTimeoutProc), m_hBoard);

            OnGrabStopProc = new IKapCallBackProc(OnGrabStopFunc);
            ret = IKapBoard.IKapRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStop, Marshal.GetFunctionPointerForDelegate(OnGrabStopProc), m_hBoard);
        }
        // This function will be unregister callback
        private void unRegisterCallback()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStart);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameReady);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_FrameLost);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_TimeOut);
            ret = IKapBoard.IKapUnRegisterCallback(m_hBoard, (uint)CallBackEvents.IKEvent_GrabStop);
        }
        #endregion
    }
    public enum IKapConnectTypes : uint
    {
        IKBoardALL = 0,
        IKBoardUSB30 = 1,
        IKBoardPCIE = 2,
        IKBoardUnknown = uint.MaxValue
    }
}
