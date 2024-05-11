using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace HY.Devices.Camera
{
    /// <summary>
    /// 海康相机
    /// </summary>
    public abstract class AbstractHYCamera_HIK : IHYCamera
    {
        #region private members
        /// <summary>
        /// 设备列表
        /// </summary>
        MyCamera.MV_CC_DEVICE_INFO_LIST m_pDeviceList;
        /// <summary>
        /// 当前设备
        /// </summary>
        IntPtr m_pDevice = IntPtr.Zero;
        /// <summary>
        /// 相机
        /// </summary>
        private MyCamera m_pMyCamera;
        /// <summary>
        /// 图片地址
        /// </summary>
        // ch:用于从驱动获取图像的缓存 | en:Buffer for getting image from driver
        uint m_nBufSizeForDriver = 0;//3072 * 2048 * 3;
        IntPtr m_pBufForDriver;
        byte[] m_BufForDriver;

        // ch:用于保存图像的缓存 | en:Buffer for saving image
        uint m_nBufSizeForSaveImage = 0;// 3072 * 2048 * 3 * 3 + 2048;
        IntPtr m_pBufForSaveImage;
        byte[] m_BufForSaveImage;
        MyCamera.cbOutputExdelegate ImageCallback;
        #endregion
        /// <summary>
        /// 相机类型
        /// </summary>
        public abstract CameraTypes CameraType { get; }
        public uint CameraIndex { get; private set; }
        public string SerialNumber { get; private set; }
        public string CameraName { get; private set; }
        public CameraConnectTypes HIKCameraConnectType { get; set; } = CameraConnectTypes.GigE;
        public virtual bool IsConnected { get; protected set; }
        public virtual bool IsGrabbing { get; protected set; }
        public virtual bool GetImageWidth(out uint width)
        {
            try
            {
                int nRet = MyCamera.MV_OK;
                width = uint.MaxValue;
                // Get value of Integer nodes. Such as, 'width' etc.
                MyCamera.MVCC_INTVALUE stIntVal = new MyCamera.MVCC_INTVALUE();
                nRet = m_pMyCamera.MV_CC_GetWidth_NET(ref stIntVal);
                if (MyCamera.MV_OK == nRet)
                {
                    width = stIntVal.nCurValue;
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
                return m_pMyCamera.MV_CC_SetWidth_NET(width) == MyCamera.MV_OK;
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
                int nRet = MyCamera.MV_OK;
                height = uint.MaxValue;
                // Get value of Integer nodes. Such as, 'width' etc.
                MyCamera.MVCC_INTVALUE stIntVal = new MyCamera.MVCC_INTVALUE();
                nRet = m_pMyCamera.MV_CC_GetHeight_NET(ref stIntVal);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
                height = stIntVal.nCurValue;
                return true;
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
                return m_pMyCamera.MV_CC_SetHeight_NET(height) == MyCamera.MV_OK;
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
                MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
                int nRet = m_pMyCamera.MV_CC_GetExposureTime_NET(ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
                exposureTime = stParam.fCurValue;
                return true;
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
                return m_pMyCamera.MV_CC_SetExposureTime_NET((float)exposureTime) == MyCamera.MV_OK;
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
                uint value = (uint)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF;
                switch (mode)
                {
                    case AutoMode.OFF:
                        value = (uint)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_OFF;
                        break;
                    case AutoMode.Once:
                        value = (uint)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_ONCE;
                        break;
                    case AutoMode.Continous:
                        value = (uint)MyCamera.MV_CAM_EXPOSURE_AUTO_MODE.MV_EXPOSURE_AUTO_MODE_CONTINUOUS;
                        break;
                    default:
                        break;
                }
                return m_pMyCamera.MV_CC_SetExposureAutoMode_NET(value) == MyCamera.MV_OK;
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
                MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
                int nRet = m_pMyCamera.MV_CC_GetGain_NET(ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
                gain = stParam.fCurValue;
                return true;
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
                return m_pMyCamera.MV_CC_SetGain_NET((float)gain) == MyCamera.MV_OK;
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
                uint value = (uint)MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF;
                switch (mode)
                {
                    case AutoMode.OFF:
                        value = (uint)MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_OFF;
                        break;
                    case AutoMode.Once:
                        value = (uint)MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_ONCE;
                        break;
                    case AutoMode.Continous:
                        value = (uint)MyCamera.MV_CAM_GAIN_MODE.MV_GAIN_MODE_CONTINUOUS;
                        break;
                    default:
                        break;
                }
                return m_pMyCamera.MV_CC_SetGainMode_NET(value) == MyCamera.MV_OK;
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
                MyCamera.MVCC_FLOATVALUE stParam = new MyCamera.MVCC_FLOATVALUE();
                int nRet = m_pMyCamera.MV_CC_GetFrameRate_NET(ref stParam);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
                frameRate = stParam.fCurValue;
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
                return m_pMyCamera.MV_CC_SetFrameRate_NET((float)frameRate) == MyCamera.MV_OK;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool GetTriggerModel(out TriggerMode mode)
        {
            try
            {
                mode = default(TriggerMode);
                MyCamera.MVCC_ENUMVALUE stEnumVal = new MyCamera.MVCC_ENUMVALUE();
                int nRet = m_pMyCamera.MV_CC_GetEnumValue_NET("TriggerMode", ref stEnumVal);
                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
                mode = (TriggerMode)stEnumVal.nCurValue;
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool SetTriggerModel(TriggerMode mode)
        {
            try
            {
                return m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerMode", (uint)mode) == MyCamera.MV_OK;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool GetTriggerSource(out TriggerSources triggerSources)
        {
            try
            {
                triggerSources = default(TriggerSources);
                MyCamera.MVCC_ENUMVALUE stEnumVal = new MyCamera.MVCC_ENUMVALUE();
                int ret = m_pMyCamera.MV_CC_GetEnumValue_NET("TriggerSource", ref stEnumVal);
                if (ret != MyCamera.MV_OK)
                {
                    return false;
                }
                triggerSources = (TriggerSources)stEnumVal.nCurValue;
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public bool SetTriggerSource(TriggerSources triggerSources)
        {
            try
            {
                return m_pMyCamera.MV_CC_SetEnumValue_NET("TriggerSource", (uint)triggerSources) == MyCamera.MV_OK;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        event EventHandler<Bitmap> HIKNewImageEvent;
        object lockObj = new object();
        event EventHandler<Bitmap> IHYCamera.NewImageEvent
        {
            add
            {
                lock (lockObj)
                {
                    HIKNewImageEvent += value;
                }
            }

            remove
            {
                lock (lockObj)
                {
                    HIKNewImageEvent -= value;
                }
            }
        }
        void ImageCallbackFunc(IntPtr pData, ref MyCamera.MV_FRAME_OUT_INFO_EX pFrameInfo, IntPtr pUser)
        {
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            int nRet = m_pMyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return;
            }
            uint nPayloadSize = stParam.nCurValue;
            if (nPayloadSize > m_nBufSizeForDriver)
            {
                m_nBufSizeForDriver = nPayloadSize;
                m_BufForDriver = new byte[m_nBufSizeForDriver];

                // ch:同时对保存图像的缓存做大小判断处理 | en:Determine the buffer size to save image
                // ch:BMP图片大小：width * height * 3 + 2048(预留BMP头大小) | en:BMP image size: width * height * 3 + 2048 (Reserved for BMP header)
                m_nBufSizeForSaveImage = m_nBufSizeForDriver * 3 + 2048;
                m_BufForSaveImage = new byte[m_nBufSizeForSaveImage];
                if (m_pBufForSaveImage != IntPtr.Zero)
                {
                    Marshal.Release(m_pBufForSaveImage);
                }
                m_pBufForSaveImage = Marshal.AllocHGlobal((Int32)m_nBufSizeForSaveImage);
            }

            HIKNewImageEvent?.Invoke(this, TransferToBitmap(pData, ref pFrameInfo));
        }
        private static object BufForDriverLock = new object();
        private Bitmap TransferToBitmap(IntPtr ptr, ref MyCamera.MV_FRAME_OUT_INFO_EX m_stFrameInfo)
        {
            try
            {
                if (RemoveCustomPixelFormats(m_stFrameInfo.enPixelType))
                {
                    return null;
                }

                IntPtr pTemp = IntPtr.Zero;
                MyCamera.MvGvspPixelType enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Undefined;
                if (m_stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8 || m_stFrameInfo.enPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed)
                {
                    pTemp = ptr;
                    enDstPixelType = m_stFrameInfo.enPixelType;
                }
                else
                {
                    MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();

                    lock (BufForDriverLock)
                    {
                        if (m_stFrameInfo.nFrameLen == 0)
                        {
                            return null;
                        }

                        if (IsMonoData(m_stFrameInfo.enPixelType))
                        {
                            enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                        }
                        else if (IsColorData(m_stFrameInfo.enPixelType))
                        {
                            enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_BGR8_Packed;
                        }
                        else
                        {
                            return null;
                        }
                        stConverPixelParam.nWidth = m_stFrameInfo.nWidth;
                        stConverPixelParam.nHeight = m_stFrameInfo.nHeight;
                        stConverPixelParam.pSrcData = ptr;
                        stConverPixelParam.nSrcDataLen = m_stFrameInfo.nFrameLen;
                        stConverPixelParam.enSrcPixelType = m_stFrameInfo.enPixelType;
                        stConverPixelParam.enDstPixelType = enDstPixelType;
                        stConverPixelParam.pDstBuffer = m_pBufForSaveImage;
                        stConverPixelParam.nDstBufferSize = m_nBufSizeForSaveImage;
                        int nRet = m_pMyCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
                        if (MyCamera.MV_OK != nRet)
                        {
                            return null;
                        }
                        pTemp = m_pBufForSaveImage;
                    }
                }

                lock (BufForDriverLock)
                {
                    if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                    {
                        //************************Mono8 转 Bitmap*******************************
                        Bitmap bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, pTemp);

                        System.Drawing.Imaging.ColorPalette cp = bmp.Palette;
                        // init palette
                        for (int i = 0; i < 256; i++)
                        {
                            cp.Entries[i] = Color.FromArgb(i, i, i);
                        }
                        // set palette back
                        bmp.Palette = cp;
                        return bmp;
                    }
                    else
                    {
                        //*********************BGR8 转 Bitmap**************************
                        for (int i = 0; i < m_stFrameInfo.nHeight; i++)
                        {
                            for (int j = 0; j < m_stFrameInfo.nWidth; j++)
                            {
                                byte chRed = m_BufForSaveImage[i * m_stFrameInfo.nWidth * 3 + j * 3];
                                m_BufForSaveImage[i * m_stFrameInfo.nWidth * 3 + j * 3] = m_BufForSaveImage[i * m_stFrameInfo.nWidth * 3 + j * 3 + 2];
                                m_BufForSaveImage[i * m_stFrameInfo.nWidth * 3 + j * 3 + 2] = chRed;
                            }
                        }
                        try
                        {
                            Bitmap bmp = new Bitmap(m_stFrameInfo.nWidth, m_stFrameInfo.nHeight, m_stFrameInfo.nWidth * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, pTemp);
                            return bmp;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }// ch:去除自定义的像素格式 | en:Remove custom pixel formats
        private bool RemoveCustomPixelFormats(MyCamera.MvGvspPixelType enPixelFormat)
        {
            Int32 nResult = ((int)enPixelFormat) & (unchecked((Int32)0x80000000));
            if (0x80000000 == nResult)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public AbstractHYCamera_HIK(CameraConnectTypes connectType, uint cameraIndex)
        {
            HIKCameraConnectType = connectType;
            CameraIndex = cameraIndex;
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();

            try
            {
                DeviceListAcq(HIKCameraConnectType, (int)cameraIndex);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public AbstractHYCamera_HIK(CameraConnectTypes connectType, string serialNum)
        {
            HIKCameraConnectType = connectType;
            SerialNumber = serialNum;
            m_pDeviceList = new MyCamera.MV_CC_DEVICE_INFO_LIST();

            try
            {
                DeviceListAcq(HIKCameraConnectType, serialNum);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void DeviceListAcq(CameraConnectTypes connectType, int cameraIndex)
        {
            int nRet;
            // ch:创建设备列表 en:Create Device List
            System.GC.Collect();
            uint connType = MyCamera.MV_GIGE_DEVICE;
            switch (connectType)
            {
                case CameraConnectTypes.GigE:
                    connType = MyCamera.MV_GIGE_DEVICE;
                    break;
                case CameraConnectTypes.Usb:
                    connType = MyCamera.MV_USB_DEVICE;
                    break;
                case CameraConnectTypes.CameraLink:
                    connType = MyCamera.MV_CAMERALINK_DEVICE;
                    break;
                case CameraConnectTypes.IEEE1394:
                    connType = MyCamera.MV_1394_DEVICE;
                    break;
                default:
                    break;
            }
            nRet = MyCamera.MV_CC_EnumDevices_NET(connType, ref m_pDeviceList);
            if (0 != nRet)
            {
                return;
            }
            if (m_pDeviceList.nDeviceNum > cameraIndex)
            {
                m_pDevice = m_pDeviceList.pDeviceInfo[cameraIndex];
            }
            else
            {
                throw new Exception("不存在该HK相机");
            }
            MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDevice, typeof(MyCamera.MV_CC_DEVICE_INFO));
            if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
            {
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                if (gigeInfo.chUserDefinedName != "")
                {
                    CameraName = "GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")";
                }
                else
                {
                    CameraName = "GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")";
                }
                SerialNumber = gigeInfo.chSerialNumber;
            }
            else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
            {
                IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                if (usbInfo.chUserDefinedName != "")
                {
                    CameraName = "USB: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")";
                }
                else
                {
                    CameraName = "USB: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")";
                }
                SerialNumber = usbInfo.chSerialNumber;
            }
        }
        private void DeviceListAcq(CameraConnectTypes connectType, string serialNum)
        {
            int nRet;
            // ch:创建设备列表 en:Create Device List
            GC.Collect();
            uint connType = MyCamera.MV_GIGE_DEVICE;
            switch (connectType)
            {
                case CameraConnectTypes.GigE:
                    connType = MyCamera.MV_GIGE_DEVICE;
                    break;
                case CameraConnectTypes.Usb:
                    connType = MyCamera.MV_USB_DEVICE;
                    break;
                case CameraConnectTypes.CameraLink:
                    connType = MyCamera.MV_CAMERALINK_DEVICE;
                    break;
                case CameraConnectTypes.IEEE1394:
                    connType = MyCamera.MV_1394_DEVICE;
                    break;
                default:
                    break;
            }
            nRet = MyCamera.MV_CC_EnumDevices_NET(connType, ref m_pDeviceList);
            if (0 != nRet)
            {
                return;
            }
            if (m_pDeviceList.nDeviceNum < 1)
            {
                throw new Exception("不存在该HK相机");
            }
            for (int i = 0; i < m_pDeviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDeviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    if (gigeInfo.chSerialNumber.Equals(serialNum))
                    {
                        CameraIndex = (uint)i;
                        m_pDevice = m_pDeviceList.pDeviceInfo[i];
                        if (gigeInfo.chUserDefinedName != "")
                        {
                            CameraName = "GigE: " + gigeInfo.chUserDefinedName + " (" + gigeInfo.chSerialNumber + ")";
                        }
                        else
                        {
                            CameraName = "GigE: " + gigeInfo.chManufacturerName + " " + gigeInfo.chModelName + " (" + gigeInfo.chSerialNumber + ")";
                        }
                        break;
                    }
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    if (usbInfo.chSerialNumber.Equals(serialNum))
                    {
                        CameraIndex = (uint)i;
                        m_pDevice = m_pDeviceList.pDeviceInfo[i];
                        if (usbInfo.chUserDefinedName != "")
                        {
                            CameraName = "USB: " + usbInfo.chUserDefinedName + " (" + usbInfo.chSerialNumber + ")";
                        }
                        else
                        {
                            CameraName = "USB: " + usbInfo.chManufacturerName + " " + usbInfo.chModelName + " (" + usbInfo.chSerialNumber + ")";
                        }
                        break;
                    }
                }
            }
            if (m_pDevice == IntPtr.Zero)
            {
                throw new Exception("不存在该HK相机");
            }

        }
        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public static List<CameraItemInfo> GetDeviceList(CameraConnectTypes connectType)
        {
            int nRet;
            List<CameraItemInfo> retList=new List<CameraItemInfo>();
            // ch:创建设备列表 en:Create Device List
            GC.Collect();
            uint connType = MyCamera.MV_GIGE_DEVICE;
            switch (connectType)
            {
                case CameraConnectTypes.GigE:
                    connType = MyCamera.MV_GIGE_DEVICE;
                    break;
                case CameraConnectTypes.Usb:
                    connType = MyCamera.MV_USB_DEVICE;
                    break;
                case CameraConnectTypes.CameraLink:
                    connType = MyCamera.MV_CAMERALINK_DEVICE;
                    break;
                case CameraConnectTypes.IEEE1394:
                    connType = MyCamera.MV_1394_DEVICE;
                    break;
                default:
                    break;
            }
            MyCamera.MV_CC_DEVICE_INFO_LIST deviceList= new MyCamera.MV_CC_DEVICE_INFO_LIST();
            nRet = MyCamera.MV_CC_EnumDevices_NET(connType, ref deviceList);
            if (0 != nRet)
            {
                return retList;
            }
            for (int i = 0; i < deviceList.nDeviceNum; i++)
            {
                MyCamera.MV_CC_DEVICE_INFO device = (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(deviceList.pDeviceInfo[i], typeof(MyCamera.MV_CC_DEVICE_INFO));
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stGigEInfo, 0);
                    MyCamera.MV_GIGE_DEVICE_INFO gigeInfo = (MyCamera.MV_GIGE_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_GIGE_DEVICE_INFO));
                    retList.Add(new CameraItemInfo { CameraType=connectType, Index=i,Name=gigeInfo.chModelName,SN=gigeInfo.chSerialNumber,IP=gigeInfo.nCurrentIp.ToString()});
                }
                else if (device.nTLayerType == MyCamera.MV_USB_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stUsb3VInfo, 0);
                    MyCamera.MV_USB3_DEVICE_INFO usbInfo = (MyCamera.MV_USB3_DEVICE_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_USB3_DEVICE_INFO));
                    retList.Add(new CameraItemInfo { CameraType = connectType, Index = i, Name = usbInfo.chModelName, SN = usbInfo.chSerialNumber });
                }
                else if (device.nTLayerType == MyCamera.MV_CAMERALINK_DEVICE)
                {
                    IntPtr buffer = Marshal.UnsafeAddrOfPinnedArrayElement(device.SpecialInfo.stCamLInfo, 0);
                    MyCamera.MV_CamL_DEV_INFO camLInfo = (MyCamera.MV_CamL_DEV_INFO)Marshal.PtrToStructure(buffer, typeof(MyCamera.MV_CamL_DEV_INFO));
                    retList.Add(new CameraItemInfo { CameraType = connectType, Index = i, Name = camLInfo.chModelName, SN = camLInfo.chSerialNumber });
                }
            }
            return retList;
        }
        public virtual bool Open()
        {
            try
            {
                if (IsConnected)
                {
                    Close();
                }
                if (m_pDevice == IntPtr.Zero)
                {
                    return false;
                }
                int nRet = -1;
                MyCamera.MV_CC_DEVICE_INFO device =
                    (MyCamera.MV_CC_DEVICE_INFO)Marshal.PtrToStructure(m_pDevice,
                                                                  typeof(MyCamera.MV_CC_DEVICE_INFO));
                // ch:打开设备 | en:Open device
                if (null == m_pMyCamera)
                {
                    m_pMyCamera = new MyCamera();
                    if (null == m_pMyCamera)
                    {
                        return false;
                    }
                }
                ///创建设备
                nRet = m_pMyCamera.MV_CC_CreateDevice_NET(ref device);

                if (MyCamera.MV_OK != nRet)
                {
                    return false;
                }
                //打开设备
                nRet = m_pMyCamera.MV_CC_OpenDevice_NET();
                if (MyCamera.MV_OK != nRet)
                {
                    m_pMyCamera.MV_CC_DestroyDevice_NET();
                    return false;
                }
                // ch:探测网络最佳包大小(只对GigE相机有效) | en:Detection network optimal package size(It only works for the GigE camera)
                if (device.nTLayerType == MyCamera.MV_GIGE_DEVICE)
                {
                    int nPacketSize = m_pMyCamera.MV_CC_GetOptimalPacketSize_NET();
                    if (nPacketSize > 0)
                    {
                        nRet = m_pMyCamera.MV_CC_SetIntValue_NET("GevSCPSPacketSize", (uint)nPacketSize);
                        if (nRet != MyCamera.MV_OK)
                        {
                            Console.WriteLine("Warning: Set Packet Size failed {0:x8}", nRet);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Warning: Get Packet Size failed {0:x8}", nPacketSize);
                    }
                }
                //SetTriggerModel(TriggerMode.OFF);
                ImageCallback = new MyCamera.cbOutputExdelegate(ImageCallbackFunc);
                nRet = m_pMyCamera.MV_CC_RegisterImageCallBackEx_NET(ImageCallback, IntPtr.Zero);
                // ch:设置采集连续模式 | en:Set Continues Aquisition Mode
                //m_pMyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", 2);// ch:工作在连续模式 | en:Acquisition On Continuous Mode
                //SetTriggerModel(TriggerModel.OFF);//触发模式关闭
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
                if (!GetTriggerSource(out TriggerSources triggerSources))
                {
                    return false;
                }
                if (triggerSources == TriggerSources.Soft)
                {
                    if (!IsGrabbing)
                    {
                        return false;
                    }
                    int nRet;

                    // ch:触发命令 | en:Trigger command
                    nRet = m_pMyCamera.MV_CC_SetCommandValue_NET("TriggerSoftware");
                    return MyCamera.MV_OK == nRet;
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
        public virtual bool GrabOne(out Bitmap bitmap)
        {
            try
            {
                bitmap = null;
                if (IsGrabbing)
                {
                    if (!StopGrab())
                    {
                        return false;
                    }
                }
                m_pMyCamera.MV_CC_RegisterImageCallBackEx_NET(null, IntPtr.Zero);
                try
                {
                    if (!ContinousGrab())
                    {
                        return false;
                    }
                    int nRet;
                    uint nPayloadSize = 0;
                    MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
                    nRet = m_pMyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
                    if (MyCamera.MV_OK != nRet)
                    {
                        return false;
                    }
                    nPayloadSize = stParam.nCurValue;
                    if (nPayloadSize > m_nBufSizeForDriver)
                    {
                        m_nBufSizeForDriver = nPayloadSize;
                        m_BufForDriver = new byte[m_nBufSizeForDriver];

                        // ch:同时对保存图像的缓存做大小判断处理 | en:Determine the buffer size to save image
                        // ch:BMP图片大小：width * height * 3 + 2048(预留BMP头大小) | en:BMP image size: width * height * 3 + 2048 (Reserved for BMP header)
                        m_nBufSizeForSaveImage = m_nBufSizeForDriver * 3 + 2048;
                        m_BufForSaveImage = new byte[m_nBufSizeForSaveImage];
                    }

                    IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(m_BufForDriver, 0);
                    MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
                    // ch:超时获取一帧，超时时间为1秒 | en:Get one frame timeout, timeout is 1 sec
                    nRet = m_pMyCamera.MV_CC_GetOneFrameTimeout_NET(pData, m_nBufSizeForDriver, ref stFrameInfo, 1000);
                    if (MyCamera.MV_OK != nRet)
                    {
                        return false;
                    }

                    MyCamera.MvGvspPixelType enDstPixelType;
                    if (IsMonoData(stFrameInfo.enPixelType))
                    {
                        enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
                    }
                    else if (IsColorData(stFrameInfo.enPixelType))
                    {
                        enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
                    }
                    else
                    {
                        return false;
                    }

                    IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_BufForSaveImage, 0);
                    //MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
                    MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();
                    stConverPixelParam.nWidth = stFrameInfo.nWidth;
                    stConverPixelParam.nHeight = stFrameInfo.nHeight;
                    stConverPixelParam.pSrcData = pData;
                    stConverPixelParam.nSrcDataLen = stFrameInfo.nFrameLen;
                    stConverPixelParam.enSrcPixelType = stFrameInfo.enPixelType;
                    stConverPixelParam.enDstPixelType = enDstPixelType;
                    stConverPixelParam.pDstBuffer = pImage;
                    stConverPixelParam.nDstBufferSize = m_nBufSizeForSaveImage;
                    nRet = m_pMyCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
                    if (MyCamera.MV_OK != nRet)
                    {
                        return false;
                    }

                    if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                    {
                        //************************Mono8 转 Bitmap*******************************
                        bitmap = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, pImage);

                        System.Drawing.Imaging.ColorPalette cp = bitmap.Palette;
                        // init palette
                        for (int i = 0; i < 256; i++)
                        {
                            cp.Entries[i] = Color.FromArgb(i, i, i);
                        }
                        // set palette back
                        bitmap.Palette = cp;
                        return true;
                    }
                    else
                    {
                        //*********************RGB8 转 Bitmap**************************
                        for (int i = 0; i < stFrameInfo.nHeight; i++)
                        {
                            for (int j = 0; j < stFrameInfo.nWidth; j++)
                            {
                                byte chRed = m_BufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3];
                                m_BufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3] = m_BufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2];
                                m_BufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2] = chRed;
                            }
                        }
                        try
                        {
                            bitmap = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, pImage);
                            return true;
                        }
                        catch
                        {
                            return false;
                        }

                    }
                }
                finally
                {
                    if (IsGrabbing)
                    {
                        StopGrab();
                    }
                    m_pMyCamera.MV_CC_RegisterImageCallBackEx_NET(ImageCallback, IntPtr.Zero);
                }
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
                if (IsGrabbing) return true;
                int ret = -1;
                m_pMyCamera.MV_CC_SetEnumValue_NET("AcquisitionMode", (uint)MyCamera.MV_CAM_ACQUISITION_MODE.MV_ACQ_MODE_CONTINUOUS);// ch:工作在连续模式 | en:Acquisition On Continuous Mode
                ret = m_pMyCamera.MV_CC_StartGrabbing_NET();
                if (ret == MyCamera.MV_OK)
                {
                    IsGrabbing = true;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool IsRecording { get; set; }
        public virtual bool StartRecord()
        {
            try
            {
                if (IsRecording) return true;
                IsRecording = true;
                if (IsGrabbing) return false;
                int ret = -1;
                MyCamera.MV_CC_RECORD_PARAM mV_CC_RECORD_PARAM = new MyCamera.MV_CC_RECORD_PARAM { enPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Jpeg, enRecordFmtType = MyCamera.MV_RECORD_FORMAT_TYPE.MV_FormatType_AVI };
                ret = m_pMyCamera.MV_CC_StartRecord_NET(ref mV_CC_RECORD_PARAM);
                if (ret == MyCamera.MV_OK)
                {
                    IsGrabbing = true;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual bool StopRecord()
        {
            try
            {
                if (!IsRecording) return true;
                if (IsGrabbing) return false;
                int ret = -1;
                ret = m_pMyCamera.MV_CC_StopRecord_NET();
                if (ret == MyCamera.MV_OK)
                {
                    IsGrabbing = true;
                    return true;
                }
                return false;
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
                if (!IsGrabbing)
                {
                    return true;
                }
                if (m_pMyCamera == null)
                {
                    return false;
                }
                int ret = -1;
                ret = m_pMyCamera.MV_CC_StopGrabbing_NET();
                if (ret == MyCamera.MV_OK)
                {
                    IsGrabbing = false;
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 暂时只支持jpg和bmp格式
        /// </summary>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public virtual bool SaveImage(string savePath)
        {
            if (!SetTriggerModel(TriggerMode.OFF))
            {
                return false;
            }
            if (!IsGrabbing)
            {
                if (!ContinousGrab())
                    return false;
            }
            int nRet;
            UInt32 nPayloadSize = 0;
            MyCamera.MVCC_INTVALUE stParam = new MyCamera.MVCC_INTVALUE();
            nRet = m_pMyCamera.MV_CC_GetIntValue_NET("PayloadSize", ref stParam);
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }
            if (nPayloadSize > m_nBufSizeForDriver)
            {
                if (m_pBufForDriver != IntPtr.Zero)
                {
                    Marshal.Release(m_pBufForDriver);
                }
                m_nBufSizeForDriver = nPayloadSize;
                m_pBufForDriver = Marshal.AllocHGlobal((Int32)m_nBufSizeForDriver);
            }

            if (m_pBufForDriver == IntPtr.Zero)
            {
                return false;
            }

            IntPtr pData = Marshal.UnsafeAddrOfPinnedArrayElement(m_BufForDriver, 0);
            MyCamera.MV_FRAME_OUT_INFO_EX stFrameInfo = new MyCamera.MV_FRAME_OUT_INFO_EX();
            // ch:超时获取一帧，超时时间为1秒 | en:Get one frame timeout, timeout is 1 sec
            nRet = m_pMyCamera.MV_CC_GetOneFrameTimeout_NET(pData, m_nBufSizeForDriver, ref stFrameInfo, 1000);
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }

            MyCamera.MvGvspPixelType enDstPixelType;
            if (IsMonoData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8;
            }
            else if (IsColorData(stFrameInfo.enPixelType))
            {
                enDstPixelType = MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed;
            }
            else
            {
                return false;
            }

            IntPtr pImage = Marshal.UnsafeAddrOfPinnedArrayElement(m_BufForSaveImage, 0);
            switch (System.IO.Path.GetExtension(savePath).ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    {
                        MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
                        stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Jpeg;
                        stSaveParam.enPixelType = stFrameInfo.enPixelType;
                        stSaveParam.pData = pData;
                        stSaveParam.nDataLen = stFrameInfo.nFrameLen;
                        stSaveParam.nHeight = stFrameInfo.nHeight;
                        stSaveParam.nWidth = stFrameInfo.nWidth;
                        stSaveParam.pImageBuffer = pImage;
                        stSaveParam.nBufferSize = m_nBufSizeForSaveImage;
                        stSaveParam.nJpgQuality = 80;
                        nRet = m_pMyCamera.MV_CC_SaveImageEx_NET(ref stSaveParam);
                        if (MyCamera.MV_OK != nRet)
                        {
                            return false;
                        }

                        try
                        {
                            using (FileStream file = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                            {
                                file.Write(m_BufForSaveImage, 0, (int)stSaveParam.nImageLen);
                            }
                            return true;
                        }
                        catch
                        {
                            return false;
                        }

                    }
                case ".png":
                    {
                        return false;
                        MyCamera.MV_SAVE_IMAGE_PARAM_EX stSaveParam = new MyCamera.MV_SAVE_IMAGE_PARAM_EX();
                        stSaveParam.enImageType = MyCamera.MV_SAVE_IAMGE_TYPE.MV_Image_Png;
                        stSaveParam.enPixelType = stFrameInfo.enPixelType;
                        stSaveParam.pData = pData;
                        stSaveParam.nDataLen = stFrameInfo.nFrameLen;
                        stSaveParam.nHeight = stFrameInfo.nHeight;
                        stSaveParam.nWidth = stFrameInfo.nWidth;
                        stSaveParam.pImageBuffer = pImage;
                        stSaveParam.nBufferSize = m_nBufSizeForSaveImage;
                        stSaveParam.nJpgQuality = 80;
                        nRet = m_pMyCamera.MV_CC_SaveImageEx_NET(ref stSaveParam);
                        if (MyCamera.MV_OK != nRet)
                        {
                            return false;
                        }

                        try
                        {
                            using (FileStream file = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                            {
                                file.Write(m_BufForSaveImage, 0, (int)stSaveParam.nImageLen);
                            }
                            return true;
                        }
                        catch
                        {
                            return false;
                        }

                    }
                case ".bmp":
                    {
                        MyCamera.MV_PIXEL_CONVERT_PARAM stConverPixelParam = new MyCamera.MV_PIXEL_CONVERT_PARAM();
                        stConverPixelParam.nWidth = stFrameInfo.nWidth;
                        stConverPixelParam.nHeight = stFrameInfo.nHeight;
                        stConverPixelParam.pSrcData = pData;
                        stConverPixelParam.nSrcDataLen = stFrameInfo.nFrameLen;
                        stConverPixelParam.enSrcPixelType = stFrameInfo.enPixelType;
                        stConverPixelParam.enDstPixelType = enDstPixelType;
                        stConverPixelParam.pDstBuffer = pImage;
                        stConverPixelParam.nDstBufferSize = m_nBufSizeForSaveImage;
                        nRet = m_pMyCamera.MV_CC_ConvertPixelType_NET(ref stConverPixelParam);
                        if (MyCamera.MV_OK != nRet)
                        {
                            return false;
                        }

                        if (enDstPixelType == MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8)
                        {
                            //************************Mono8 转 Bitmap*******************************
                            Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 1, System.Drawing.Imaging.PixelFormat.Format8bppIndexed, pImage);

                            System.Drawing.Imaging.ColorPalette cp = bmp.Palette;
                            // init palette
                            for (int i = 0; i < 256; i++)
                            {
                                cp.Entries[i] = Color.FromArgb(i, i, i);
                            }
                            // set palette back
                            bmp.Palette = cp;

                            bmp.Save(savePath, System.Drawing.Imaging.ImageFormat.Bmp);
                            return true;
                        }
                        else
                        {
                            //*********************RGB8 转 Bitmap**************************
                            for (int i = 0; i < stFrameInfo.nHeight; i++)
                            {
                                for (int j = 0; j < stFrameInfo.nWidth; j++)
                                {
                                    byte chRed = m_BufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3];
                                    m_BufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3] = m_BufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2];
                                    m_BufForSaveImage[i * stFrameInfo.nWidth * 3 + j * 3 + 2] = chRed;
                                }
                            }
                            try
                            {
                                Bitmap bmp = new Bitmap(stFrameInfo.nWidth, stFrameInfo.nHeight, stFrameInfo.nWidth * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, pImage);
                                bmp.Save(savePath, System.Drawing.Imaging.ImageFormat.Bmp);
                                return true;
                            }
                            catch
                            {
                                return false;
                            }

                        }
                    }
                default: throw new Exception("文件格式不正确,暂时只支持jpg和bmp图片保存！");
            }
        }
        public virtual bool Close()
        {
            if (m_pMyCamera == null)
            {
                return false;
            }
            if (IsGrabbing)
            {
                StopGrab();
            }
            // ch:关闭设备 | en:Close Device
            int nRet;

            nRet = m_pMyCamera.MV_CC_CloseDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }

            nRet = m_pMyCamera.MV_CC_DestroyDevice_NET();
            if (MyCamera.MV_OK != nRet)
            {
                return false;
            }
            m_pMyCamera = null;
            IsConnected = false;
            return MyCamera.MV_OK == nRet;
        }
        public virtual void Dispose()
        {
            Close();
            GC.Collect();

        }
        private bool IsMonoData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_Mono12_Packed:
                    return true;

                default:
                    return false;
            }
        }
        /************************************************************************
                 *  @fn     IsColorData()
                 *  @brief  判断是否是彩色数据
                 *  @param  enGvspPixelType         [IN]           像素格式
                 *  @return 成功，返回0；错误，返回-1 
                 ************************************************************************/
        private bool IsColorData(MyCamera.MvGvspPixelType enGvspPixelType)
        {
            switch (enGvspPixelType)
            {
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG8:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG10_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGR12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerRG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerGB12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_BayerBG12_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_RGB8_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YUV422_YUYV_Packed:
                case MyCamera.MvGvspPixelType.PixelType_Gvsp_YCBCR411_8_CBYYCRYY:
                    return true;

                default:
                    return false;
            }
        }
    }
}
