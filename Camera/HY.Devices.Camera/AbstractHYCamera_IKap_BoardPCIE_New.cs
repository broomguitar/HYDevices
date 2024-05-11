using HY.Devices.Camera.Utils;
using IKapBoardClassLibrary;
using IKapC.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace HY.Devices.Camera
{
    class MyFrameGrabber
    {
        public IntPtr m_hDev = IntPtr.Zero;
        public int m_nGrabTotalFrameCount = 0;
        public int m_nSetBufferCount =3;
        public int m_nDevIndex = -1;

        /// <summary>
        ///  获取采集卡个数
        /// </summary>
        /// <param name="nType">采集卡类型</param>
        /// <returns>返回采集卡个数</returns>
        public static uint GetDeviceCount(uint nType)
        {
            int res = (int)ErrorCode.IK_RTN_OK;
            uint nCount = 0;
            res = IKapBoard.IKapGetBoardCount(nType, ref nCount);
            if (res == (int)ErrorCode.IK_RTN_OK)
                return nCount;
            return 0;
        }

        /// <summary>
        /// 获取采集卡名称
        /// </summary>
        /// <param name="nType">采集卡类型</param>
        /// <param name="nIndex">当前采集卡索引</param>
        /// <returns>采集卡名</returns>
        public static string GetDeviceName(uint nType, uint nIndex)
        {
            uint nDevSize = 128;
            StringBuilder szDevName = new StringBuilder(128);
            int res = IKapBoard.IKapGetBoardName(nType, nIndex, szDevName, ref nDevSize);
            if (res != (int)ErrorCode.IK_RTN_OK)
                return "";

            string strDevName = szDevName.ToString();
            szDevName = null;

            return strDevName;
        }

        /// <summary>
        /// 打开采集卡
        /// </summary>
        /// <param name="nType">采集卡类型</param>
        /// <param name="nIndex">采集卡索引</param>
        /// <returns>是否打开</returns>
        public bool OpenDevice(uint nType, uint nIndex)
        {
            m_hDev = IKapBoard.IKapOpen(nType, nIndex);
            if (m_hDev == new IntPtr(-1))
                return false;
            m_nDevIndex = (int)nIndex;
            return true;
        }

        /// <summary>
        /// 打开CXP采集卡
        /// </summary>
        /// <param name="nType">采集卡类型</param>
        /// <param name="nIndex">采集卡索引</param>
        /// <param name="info">CXP采集卡信息</param>
        /// <returns>是否打开CXP采集卡</returns>
        public bool OpenDeviceCxp(uint nType, uint nIndex, IKapBoard.IKAP_CXP_BOARD_INFO info)
        {
            m_hDev = IKapBoard.IKapOpenCXP(nType, nIndex, info);
            if (m_hDev == new IntPtr(-1))
                return false;
            m_nDevIndex = (int)nIndex;
            return true;
        }
        /// <summary>
        /// 打开万兆网卡
        /// </summary>
        /// <param name="nType">采集卡类型</param>
        /// <param name="nIndex">采集卡索引</param>
        /// <param name="info">GVB采集卡信息</param>
        /// <returns>是否打开GVB采集卡</returns>
        public bool OpenDeviceGVB(uint nType, uint nIndex, IKapBoard.IKAP_GVB_BOARD_INFO info)
        {
            m_hDev = IKapBoard.IKapOpenGVB(nType, nIndex, info);
            if (m_hDev == new IntPtr(-1))
                return false;
            m_nDevIndex = (int)nIndex;
            return true;
        }
        /// <summary>
        /// 是否打开采集卡
        /// </summary>
        /// <returns>是否打开</returns>
        public bool IsOpenDevice()
        {
            if (m_hDev == new IntPtr(-1) || m_hDev == IntPtr.Zero)
                return false;
            return true;
        }

        /// <summary>
        /// 关闭采集卡
        /// </summary>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int CloseDevice()
        {
            int res = (int)ErrorCode.IK_RTN_OK;
            if (IsOpenDevice())
                res = IKapBoard.IKapClose(m_hDev);
            m_hDev = IntPtr.Zero;
            m_nDevIndex = -1;
            return res;
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="strFileName">配置文件名</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int LoadConfigurationFromFile(string strFileName)
        {
            return IKapBoard.IKapLoadConfigurationFromFile(m_hDev, strFileName);
        }

        /// <summary>
        /// 保存配置文件
        /// </summary>
        /// <param name="strFileName">配置文件名</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int SaveConfigurationToFile(string strFileName)
        {
            return IKapBoard.IKapSaveConfigurationToFile(m_hDev, strFileName);
        }

        /// <summary>
        /// 获取采集卡参数值
        /// </summary>
        /// <param name="nType">参数类型，参考INFO_ID</param>
        /// <param name="pValue">获取的参数值</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int GetInfo(uint nType, ref int pValue)
        {
            return IKapBoard.IKapGetInfo(m_hDev, nType, ref pValue);
        }

        /// <summary>
        /// 设置采集卡参数值
        /// </summary>
        /// <param name="nType">参数类型，参考INFO_ID</param>
        /// <param name="nValue">设置的参数值</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int SetInfo(uint nType, int nValue)
        {
            return IKapBoard.IKapSetInfo(m_hDev, nType, nValue);
        }

        /// <summary>
        /// 注册回调函数
        /// </summary>
        /// <param name="nEventType">回调类型，参照CallBackEvents</param>
        /// <param name="fEventFunc">回调函数</param>
        /// <param name="pContext">回调参数</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int RegisterCallback(uint nEventType, IntPtr fEventFunc, IntPtr pContext)
        {
            return IKapBoard.IKapRegisterCallback(m_hDev, nEventType, fEventFunc, pContext);
        }

        /// <summary>
        /// 注销回调函数
        /// </summary>
        /// <param name="nEventType">回调类型，参照CallBackEvents</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int UnRegisterCallback(uint nEventType)
        {
            return IKapBoard.IKapUnRegisterCallback(m_hDev, nEventType);
        }

        /// <summary>
        ///  开始采集，注意非连续采集在采集开始后要调用WaitGrab函数才能获取图像
        /// </summary>
        /// <param name="nFrameCount">采集帧数，0表示连续采集</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int StartGrab(int nFrameCount)
        {
            // 每次重新开始采集，要确保当前帧索引设置为0
            m_nGrabTotalFrameCount = 0;

            // 设置帧超时时间
            int timeout = -1;
            int ret = IKapBoard.IKapSetInfo(m_hDev, (uint)INFO_ID.IKP_TIME_OUT, timeout);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return ret;
            IKapBoard.IKapSetInfo(m_hDev, (uint)INFO_ID.IKP_FPGA_EXTERNAL_TRIGGER_TIMEOUT, 60 * 60 * 1000);
            // 设置抓取模式，IKP_GRAB_NON_BLOCK为非阻塞模式
            int grab_mode = (int)GrabMode.IKP_GRAB_NON_BLOCK;
            ret = IKapBoard.IKapSetInfo(m_hDev, (uint)INFO_ID.IKP_GRAB_MODE, grab_mode);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return ret;

            // 设置帧传输模式，IKP_FRAME_TRANSFER_SYNCHRONOUS_NEXT_EMPTY_WITH_PROTECT为同步保存模式
            int transfer_mode = (int)FrameTransferMode.IKP_FRAME_TRANSFER_SYNCHRONOUS_NEXT_EMPTY_WITH_PROTECT;
            ret = IKapBoard.IKapSetInfo(m_hDev, (uint)INFO_ID.IKP_FRAME_TRANSFER_MODE, transfer_mode);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return ret;

            // 设置缓冲区格式
            ret = IKapBoard.IKapSetInfo(m_hDev, (uint)INFO_ID.IKP_FRAME_COUNT, m_nSetBufferCount);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return ret;

            return IKapBoard.IKapStartGrab(m_hDev, nFrameCount);
        }

        /// <summary>
        /// 停止抓取图像
        /// </summary>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int StopGrab()
        {
            return IKapBoard.IKapStopGrab(m_hDev);
        }

        /// <summary>
        /// 等待图像抓取结束，只用于单帧或多帧采集
        /// </summary>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int WaitGrab()
        {
            return IKapBoard.IKapWaitGrab(m_hDev);
        }

        /// <summary>
        /// 获取帧状态
        /// </summary>
        /// <param name="nFrameNum">帧索引</param>
        /// <param name="pStatus">帧状态</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int GetBufferStatus(int nFrameNum, ref IKapBoard.IKAPBUFFERSTATUS pStatus)
        {
            return IKapBoard.IKapGetBufferStatus(m_hDev, nFrameNum, ref pStatus);
        }

        /// <summary>
        /// 获取帧数据
        /// </summary>
        /// <param name="nFrameNum">帧索引</param>
        /// <param name="pAddress">帧地址</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int GetBufferAddress(int nFrameNum, ref IntPtr pAddress)
        {
            return IKapBoard.IKapGetBufferAddress(m_hDev, nFrameNum, ref pAddress);
        }

        /// <summary>
        /// 获取采集频率
        /// </summary>
        /// <param name="fFrameRate">采集频率，面阵相机是表示帧频率，线阵相机表示行频率</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int GetFrameRate(ref double fFrameRate)
        {
            return IKapBoard.IKapGetFrameRate(m_hDev, ref fFrameRate);
        }

        /// <summary>
        /// 保存帧数据到图像文件
        /// </summary>
        /// <param name="nFrameNum">帧索引</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="nFlag">保存类型，参考ImageCompressionFalg值</param>
        /// <returns>返回错误码，参考ErrorCode值</returns>
        public int SaveBuffer(int nFrameNum, string fileName, int nFlag)
        {
            return IKapBoard.IKapSaveBuffer(m_hDev, nFrameNum, fileName, nFlag);
        }

    }
    class MyBufferManager
    {
        [DllImport("kernel32.dll")]
        public static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);

        // 存放获取帧的原始数据和长度
        IntPtr m_pBufferData = new IntPtr(-1);
        int m_nBufferSize = 0;
        object m_lockBuffer = new object();
        // 创建BITMAP图像
        public Bitmap m_bmp = null;
        public object m_lockBmp = new object();
        // 图像信息
        public bool m_bUpdateImage = false;
        public int m_nWidth = 0;
        public int m_nHeight = 0;
        public int m_nDataFormat = 8;
        public int m_nImageType = 8;

        /// <summary>
        ///  根据采集卡配置信息创建数据转换缓冲区和BITMAP图像
        /// </summary>
        /// <param name="hDev">采集卡句柄</param>
        /// <returns>是否创建成功</returns>
        public bool CreateBuffer(MyFrameGrabber hDev)
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            int nFrameSize = 0;
            ret = hDev.GetInfo((uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize);
            if (ret != (int)ErrorCode.IK_RTN_OK)
                return false;

            lock (m_lockBuffer)
            {
                m_pBufferData = Marshal.AllocHGlobal(nFrameSize);
                if (m_pBufferData == null)
                    return false;
                m_nBufferSize = nFrameSize;
            }


            // 创建Bitmap图像，16bit图像无法显示，故只创建8bit图像
            hDev.GetInfo((uint)INFO_ID.IKP_IMAGE_WIDTH, ref m_nWidth);
            hDev.GetInfo((uint)INFO_ID.IKP_IMAGE_HEIGHT, ref m_nHeight);
            hDev.GetInfo((uint)INFO_ID.IKP_DATA_FORMAT, ref m_nDataFormat);
            hDev.GetInfo((uint)INFO_ID.IKP_IMAGE_TYPE, ref m_nImageType);

            // 创建BITMAP图像
            PixelFormat nPixelFormat = PixelFormat.Undefined;
            switch (m_nImageType)
            {
                case 0:
                    nPixelFormat = PixelFormat.Format8bppIndexed;
                    break;
                case 1:
                case 3:
                case 2:
                case 4:
                    nPixelFormat = PixelFormat.Format24bppRgb;
                    break;
                default:
                    break;
            }

            lock (m_lockBmp)
            {
                m_bmp = new Bitmap(m_nWidth, m_nHeight, nPixelFormat);
                // 灰度图需要进行调色板初始化
                if (nPixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                {
                    ColorPalette cp = m_bmp.Palette;
                    for (int i = 0; i < 256; i++)
                        cp.Entries[i] = System.Drawing.Color.FromArgb(i, i, i);
                    m_bmp.Palette = cp;
                }
            }


            return false;
        }

        /// <summary>
        ///  释放申请的缓冲区和BITMAP图像资源
        /// </summary>
        /// <param name></param>
        /// <returns>是否释放</returns>
        public bool FreeBuffer()
        {
            lock (m_lockBuffer)
            {
                if (m_pBufferData != null)
                    Marshal.FreeHGlobal(m_pBufferData);
                m_nBufferSize = 0;
            }

            lock (m_lockBmp)
            {
                if (m_bmp != null)
                    m_bmp.Dispose();
                m_bmp = null;
            }
            return false;
        }

        /// <summary>
        ///  向申请的缓冲区中写入原始数据
        /// </summary>
        /// <param name="pData"，"nDataLen">原始数据流指针，数据长度</param>
        /// <returns>是否写入成功</returns>
        public bool WriteImage(IntPtr pData, int nDataLen)
        {
            lock (m_lockBuffer)
            {
                if (m_pBufferData == null)
                    return false;

                // 计算拷贝长度
                int nCopyLen = nDataLen;
                if (nCopyLen > m_nBufferSize)
                    nCopyLen = m_nBufferSize;
                CopyMemory(m_pBufferData, pData, nCopyLen);
                m_bUpdateImage = true;
            }
            return false;
        }

        /// <summary>
        ///  将缓冲区中数据进行转换后写入BITMAP图像
        /// </summary>
        /// <param name></param>
        /// <returns>是否完成数据转换</returns>
        public bool ReadImage()
        {
            lock (m_lockBmp)
            {
                lock (m_lockBuffer)
                {
                    if (m_nImageType == 2 || m_nImageType == 4)
                    {
                        ReadRGBC();
                        return true;
                    }
                    if (m_pBufferData == null || m_bmp == null)
                        return false;
                    m_bUpdateImage = false;
                    Rectangle rect = new Rectangle(0, 0, m_bmp.Width, m_bmp.Height);
                    BitmapData bitmapData = m_bmp.LockBits(rect, ImageLockMode.ReadWrite, m_bmp.PixelFormat);

                    int nShift = m_nDataFormat - 8;
                    int nStride = m_nBufferSize / m_bmp.Height;
                    //  获取采集卡数据位
                    if (m_nDataFormat == 8)
                    {
                        for (int i = 0; i < m_bmp.Height; i++)
                        {
                            IntPtr iptrDst = bitmapData.Scan0 + bitmapData.Stride * i;
                            IntPtr iptrSrc = m_pBufferData + nStride * i;
                            CopyMemory(iptrDst, iptrSrc, nStride);
                        }
                        m_bmp.UnlockBits(bitmapData);
                        return true;
                    }

                    // 高位截取转换为Bitmap图像
                    short[] pData = new short[m_nBufferSize / 2];
                    byte[] pDstData = new byte[m_nBufferSize];
                    nStride = bitmapData.Stride;
                    Marshal.Copy(m_pBufferData, pData, 0, m_nBufferSize / 2);
                    for (int i = 0; i < bitmapData.Height; i++)
                    {
                        for (int j = 0; j < nStride; j++)
                        {
                            pDstData[i * nStride + j] = (byte)(pData[i * nStride + j] >> nShift);
                        }
                    }
                    Marshal.Copy(pDstData, 0, bitmapData.Scan0, (m_nBufferSize / 2));
                    m_bmp.UnlockBits(bitmapData);
                }
            }
            return true;
        }

        /// <summary>
        ///  将缓冲区中RGBC数据进行转换后写入BITMAP图像
        /// </summary>
        /// <param name></param>
        /// <returns>是否完成数据转换</returns>
        private bool ReadRGBC()
        {
            if (m_pBufferData == null || m_bmp == null)
                return false;
            m_bUpdateImage = false;
            Rectangle rect = new Rectangle(0, 0, m_bmp.Width, m_bmp.Height);
            BitmapData bitmapData = m_bmp.LockBits(rect, ImageLockMode.ReadWrite, m_bmp.PixelFormat);

            int nShift = m_nDataFormat - 8;
            int nStride = bitmapData.Stride;
            int nCount = 0;
            byte[] pByteData = new byte[m_nBufferSize];
            byte[] pDstData = new byte[(m_nBufferSize * 3) / 4];
            Marshal.Copy(m_pBufferData, pByteData, 0, m_nBufferSize);
            if (m_nDataFormat == 8)
            {
                for (int i = 0; i < m_bmp.Height; i++)
                {
                    for (int j = 0; j < nStride; j = j + 3)
                    {
                        pDstData[i * nStride + j] = (byte)(pByteData[nCount]);
                        pDstData[i * nStride + j + 1] = (byte)(pByteData[nCount + 1]);
                        pDstData[i * nStride + j + 2] = (byte)(pByteData[nCount + 2]);
                        nCount += 4;
                    }
                }
                Marshal.Copy(pDstData, 0, bitmapData.Scan0, (m_nBufferSize * 3 / 4));
                m_bmp.UnlockBits(bitmapData);
                return true;
            }
            short[] pShortData = new short[m_nBufferSize / 2];
            Marshal.Copy(m_pBufferData, pShortData, 0, m_nBufferSize / 2);
            for (int i = 0; i < bitmapData.Height; i++)
            {
                for (int j = 0; j < nStride; j = j + 3)
                {
                    pDstData[i * nStride + j] = (byte)(pShortData[nCount] >> nShift);
                    pDstData[i * nStride + j + 1] = (byte)(pShortData[nCount + 1] >> nShift);
                    pDstData[i * nStride + j + 2] = (byte)(pShortData[nCount + 2] >> nShift);
                    nCount += 4;
                }
            }
            Marshal.Copy(pDstData, 0, bitmapData.Scan0, (m_nBufferSize * 3 / 8));
            m_bmp.UnlockBits(bitmapData);
            return true;
        }
    }
    /// <summary>
    /// 埃科相机
    /// </summary>
    public abstract class AbstractHYCamera_IKap_BoardPCIE_New : IHYCamera
    {
        #region members
        //相机
         IntPtr m_hCamera = new IntPtr(-1);
        // 设备类
        MyFrameGrabber m_hDevice = new MyFrameGrabber();
        // 数据处理类
        MyBufferManager m_buffer = new MyBufferManager();
        private void CheckIKapC(uint res)
        {
            if (res != (uint)ItkStatusErrorId.ITKSTATUS_OK)
            {
                Console.Error.WriteLine("Error Code: {0}.\n", res.ToString("x8"));
                IKapCLib.ItkManTerminate();
                Console.ReadLine();
                Environment.Exit(1);
            }
        }
        /* @brief：判断 IKapBoard 函数是否成功调用。
         * @param[in] ret：函数返回值。
         *
         * @brief：Determine whether the IKapBoard function is called successfully.
         * @param[in] ret：Function return value. */
        private void CheckIKapBoard(int ret)
        {
            if (ret != (int)ErrorCode.IK_RTN_OK)
            {
                string sErrMsg = "";
                IKapBoard.IKAPERRORINFO tIKei = new IKapBoardClassLibrary.IKapBoard.IKAPERRORINFO();

                // 获取错误码信息。
                //
                // Get error code message.
                IKapBoard.IKapGetLastError(ref tIKei, true);

                // 打印错误信息。
                //
                // Print error message.
                sErrMsg = string.Concat("Error",
                                        sErrMsg,
                                        "Board Type\t = 0x", tIKei.uBoardType.ToString("X4"), "\n",
                                        "Board Index\t = 0x", tIKei.uBoardIndex.ToString("X4"), "\n",
                                        "Error Code\t = 0x", tIKei.uErrorCode.ToString("X4"), "\n"
                                        );
                throw new Exception(sErrMsg);
            }
        }
        #endregion
        /// <summary>
        /// 相机索引
        /// </summary>
        public uint CameraIndex { get; protected set; } = uint.MaxValue;
        /// <summary>
        /// 相机序列号
        /// </summary>
        public string SerialNumber { get; protected set; }
        public string CameraName { get; protected set; }
        /// <summary>
        /// 采集卡索引
        /// </summary>
        public uint BoardIndex { get; protected set; } = 0;
        public string BoardName { get; protected set; }
        public BoardInfoClasss DeviceClass { get; protected set; }
        public string ConfigFile { get; }
        public AbstractHYCamera_IKap_BoardPCIE_New(uint cameraIndex, uint boardIndex, string configFilename, BoardInfoClasss boardInfoClass)
        {
            CameraIndex = cameraIndex;
            BoardIndex = boardIndex;
            ConfigFile = configFilename;
            DeviceClass = boardInfoClass;
            try
            {
                DeviceListAcq(IKapConnectTypes.IKBoardPCIE, BoardIndex);
                InitEnvironment();
                ConfigureCamera(CameraIndex);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public AbstractHYCamera_IKap_BoardPCIE_New(string serialNumber, uint boardIndex, string configFilename, BoardInfoClasss boardInfoClass)
        {
            SerialNumber = serialNumber;
            BoardIndex = boardIndex;
            ConfigFile = configFilename;
            DeviceClass = boardInfoClass;
            try
            {
                DeviceListAcq(IKapConnectTypes.IKBoardPCIE, BoardIndex);
                InitEnvironment();
                ConfigureCamera(SerialNumber);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private void DeviceListAcq(IKapConnectTypes connectType, uint index)
        {
            int ret = (int)ErrorCode.IK_RTN_OK;// Return value of IKapBoard methods
            uint nPCIeDevCount = 0;
            IKapBoard.IKAPERRORINFO tIKei = new IKapBoard.IKAPERRORINFO();

            // Get the count of PCIe interface boards
            nPCIeDevCount = MyFrameGrabber.GetDeviceCount((uint)BoardType.IKBoardPCIE);
            if (ret != (int)ErrorCode.IK_RTN_OK || nPCIeDevCount <= index)
            {
                throw new Exception("不存在该采集卡");
            }
            BoardName = MyFrameGrabber.GetDeviceName((uint)BoardType.IKBoardPCIE, index);
        }
        /// <summary>
        /// 初始化运行环境
        /// </summary>
        private void InitEnvironment()
        {
            // IKapC 函数返回值。
            //
            // Return value of IKapC functions.
            uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;

            ret = IKapCLib.ItkManInitialize();
            if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
            {
                throw new Exception("IKAP相机运行环境初始化异常");
            }
        }
        /// <summary>
        /// 配置相机
        /// </summary>
        /// <param name="cameraIndex">相机索引号</param>
        private void ConfigureCamera(uint cameraIndex)
        {
            uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            uint numCameras = 0;

            // 枚举可用相机的数量。在打开相机前，必须调用 ItkManGetDeviceCount() 函数。
            //
            // Enumerate the number of available cameras. Before opening the camera, ItkManGetDeviceCount() function must be called.
            ret = IKapCLib.ItkManGetDeviceCount(ref numCameras);
            if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK || numCameras <= cameraIndex)
            {
                throw new Exception("不存在该IKAP相机");
            }
            IKapCLib.ITKDEV_INFO di = new IKapCLib.ITKDEV_INFO();
            ret = IKapCLib.ItkManGetDeviceInfo(cameraIndex, ref di);
            if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
            {
                throw new Exception("获取该IKAP相机信息失败");
            }
            CameraName = di.FullName;
            if (di.DeviceClass !=Enum.GetName(typeof(BoardInfoClasss),DeviceClass)|| di.SerialNumber == "")
            {
                throw new Exception("该IKAP类型不对或者相机序列号号位空");
            }
        }
        /// <summary>
        /// 配置相机
        /// </summary>
        /// <param name="serialNum">相机序列号</param>
        private void ConfigureCamera(string serialNum)
        {
            uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            uint numCameras = 0;

            // 枚举可用相机的数量。在打开相机前，必须调用 ItkManGetDeviceCount() 函数。
            //
            // Enumerate the number of available cameras. Before opening the camera, ItkManGetDeviceCount() function must be called.
            ret = IKapCLib.ItkManGetDeviceCount(ref numCameras);
            if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK || numCameras < 1)
            {
                throw new Exception("不存在该IKAP相机");
            }
            // Open GigECamera camera.
            for (uint i = 0; i < numCameras; i++)
            {
                IKapCLib.ITKDEV_INFO di = new IKapCLib.ITKDEV_INFO();
                ret = IKapCLib.ItkManGetDeviceInfo(i, ref di);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    throw new Exception("获取该IKAP相机信息失败");
                }

                // 当设备为 GigECamera 相机且序列号正确时。
                //
                // When the device is CameraLink camera and the serial number is proper.
                if (di.DeviceClass == Enum.GetName(typeof(BoardInfoClasss), DeviceClass) && di.SerialNumber == serialNum)
                {
                    CameraName = di.FullName;
                    CameraIndex = i;

                    break;
                }
            }
            if (CameraIndex == uint.MaxValue)
            {
                throw new Exception("不存在该IKAP相机");
            }
        }
        /// <summary>
        /// 获取设备列表
        /// </summary>
        /// <returns></returns>
        public static List<CameraItemInfo> GetDeviceList(CameraConnectTypes connectType)
        {
            List<CameraItemInfo> retList = new List<CameraItemInfo>();
            uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            uint numCameras = 0;
            ret = IKapCLib.ItkManGetDeviceCount(ref numCameras);
            if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK || numCameras < 1)
            {
                return retList;
            }
            // Open GigECamera camera.
            for (uint i = 0; i < numCameras; i++)
            {
                IKapCLib.ITKDEV_INFO di = new IKapCLib.ITKDEV_INFO();
                ret = IKapCLib.ItkManGetDeviceInfo(i, ref di);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    continue;
                }
                retList.Add(new CameraItemInfo { CameraType = connectType, Index = (int)i, Name = di.ModelName, SN = di.SerialNumber });
            }
            return retList;
        }
        public abstract CameraTypes CameraType { get; }
        public virtual bool IsConnected { get; protected set; }
        public virtual bool IsGrabbing { get; protected set; }
        public virtual bool GetImageWidth(out uint width)
        {
            try
            {
                int nWidth = 0;
                bool ret = m_hDevice.GetInfo((uint)INFO_ID.IKP_IMAGE_WIDTH, ref nWidth) == (int)ErrorCode.IK_RTN_OK;
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
                bool bRet = true;
                if (DeviceClass == BoardInfoClasss.GigEVisionBoard)
                {
                    bRet = IKapCLib.ItkDevSetInt64(m_hCamera, "Width", width) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
                }
                return m_hDevice.SetInfo((uint)INFO_ID.IKP_IMAGE_WIDTH, (int)width) == (int)ErrorCode.IK_RTN_OK;
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
                bool ret = m_hDevice.GetInfo((uint)INFO_ID.IKP_IMAGE_HEIGHT, ref nHeight) == (int)ErrorCode.IK_RTN_OK;
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
                bool bRet = true;
                if(DeviceClass==BoardInfoClasss.GigEVisionBoard)
                {
                     bRet=IKapCLib.ItkDevSetInt64(m_hCamera, "Height", height) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
                }
                return m_hDevice.SetInfo((uint)INFO_ID.IKP_IMAGE_HEIGHT, (int)height) == (int)ErrorCode.IK_RTN_OK&&bRet;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 获取一个信号触发的帧数
        /// </summary>
        /// <param name="frameCount"></param>
        /// <returns></returns>
        public virtual bool GetTriggerOutterModeFrameCount(out uint frameCount)
        {
            try
            {
                int nFrameCount = 0;
                bool ret = false;
                if (DeviceClass == BoardInfoClasss.GigEVisionBoard)
                {
                    long lFrameCount = 0;
                    ret = IKapCLib.ItkDevGetInt64(m_hCamera, "FrameTriggerCount", ref lFrameCount) == (int)ErrorCode.IK_RTN_OK;
                    frameCount = (uint)lFrameCount;
                }
                else
                {
                    ret = m_hDevice.GetInfo((uint)INFO_ID.IKP_BOARD_TRIGGER_OUTTER_MODE_FRAME_COUNT, ref nFrameCount) == (int)ErrorCode.IK_RTN_OK;
                    frameCount = (uint)nFrameCount;
                }

                return ret;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        ///设置一个信号触发的帧数
        /// </summary>
        /// <param name="frameCount"></param>
        /// <returns></returns>
        public virtual bool SetTriggerOutterModeFrameCount(uint frameCount)
        {
            try
            {
                if (DeviceClass == BoardInfoClasss.GigEVisionBoard)
                {
                    return IKapCLib.ItkDevSetInt64(m_hCamera, "FrameTriggerCount", (long)frameCount) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
                }
                else
                {
                    return m_hDevice.SetInfo((uint)INFO_ID.IKP_BOARD_TRIGGER_OUTTER_MODE_FRAME_COUNT, (int)frameCount) == (int)ErrorCode.IK_RTN_OK;
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
                double ExposureValue = 0;
                bool ret = IKapCLib.ItkDevGetDouble(m_hCamera, "ExposureTime", ref ExposureValue) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
                exposureTime = ExposureValue;
                return ret;
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
                return IKapCLib.ItkDevSetDouble(m_hCamera, "ExposureTime", exposureTime) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
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
                double Gain = double.NaN;
                bool ret = IKapCLib.ItkDevGetDouble(m_hCamera, "DigitalGain", ref Gain) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
                gain = Gain;
                return ret;
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
                return IKapCLib.ItkDevSetDouble(m_hCamera, "DigitalGain", gain) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
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
                frameRate = 0;
                return m_hDevice.GetFrameRate(ref frameRate) == (int)ErrorCode.IK_RTN_OK;
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
                if (m_hDevice.GetInfo((uint)INFO_ID.IKP_BOARD_TRIGGER_MODE, ref value) == (int)ErrorCode.IK_RTN_OK)
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
                        ret = m_hDevice.SetInfo((uint)INFO_ID.IKP_BOARD_TRIGGER_MODE, (int)BoardTriggerMode.IKP_BOARD_TRIGGER_MODE_VAL_INNER);
                        break;
                    case TriggerMode.ON:
                        ret = m_hDevice.SetInfo((uint)INFO_ID.IKP_BOARD_TRIGGER_MODE, (int)BoardTriggerMode.IKP_BOARD_TRIGGER_MODE_VAL_OUTTER);
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
                if (m_hDevice.GetInfo((uint)INFO_ID.IKP_BOARD_TRIGGER_SOURCE, ref value) == (int)ErrorCode.IK_RTN_OK)
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
                        ret = m_hDevice.SetInfo((uint)INFO_ID.IKP_BOARD_TRIGGER_SOURCE, (int)BoardTriggerSource.IKP_BOARD_TRIGGER_SOURCE_VAL_GENERAL_INPUT1);
                        break;
                    case TriggerSources.Soft:
                        ret = m_hDevice.SetInfo((uint)INFO_ID.IKP_BOARD_TRIGGER_SOURCE, (int)BoardTriggerSource.IKP_BOARD_TRIGGER_SOURCE_VAL_SOFTWARE);
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
        public virtual bool SetBufferCountOfStream(uint count=3)
        {
            if(m_hDevice!=null)
            {
                m_hDevice.m_nSetBufferCount = (int)count;
                return true;
            }
            return false;
        }
        public virtual bool Open()
        {
            try
            {
                if (IsConnected)
                {
                    Close();
                }
                bool ret = true;
                uint r = (uint)ItkStatusErrorId.ITKSTATUS_OK;
                // 打开相机。
                r = IKapCLib.ItkDevOpen(CameraIndex, (int)ItkDeviceAccessMode.ITKDEV_VAL_ACCESS_MODE_EXCLUSIVE, ref m_hCamera);
                if (r != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                //打开采集卡
                if (DeviceClass == BoardInfoClasss.CameraLink)
                {
                    ret = m_hDevice.OpenDevice((uint)BoardType.IKBoardPCIE, BoardIndex);
                }
                else if(DeviceClass== BoardInfoClasss.GigEVisionBoard)
                {
                    IKapCLib.ITK_GVB_DEV_INFO gvb_cam_info = new IKapCLib.ITK_GVB_DEV_INFO();
                    IKapBoard.IKAP_GVB_BOARD_INFO gvb_board_info = new IKapBoard.IKAP_GVB_BOARD_INFO();
                    r = IKapCLib.ItkManGetGVBDeviceInfo(CameraIndex, ref gvb_cam_info);
                    CheckIKapC(r);
                    gvb_board_info.BoardIndex = gvb_cam_info.BoardIndex;
                    gvb_board_info.MasterPort = gvb_cam_info.MasterPort;
                    gvb_board_info.MAC = gvb_cam_info.MAC;
                    gvb_board_info.Ip = gvb_cam_info.Ip;
                    gvb_board_info.SubNetMask = gvb_cam_info.SubNetMask;
                    gvb_board_info.GateWay = gvb_cam_info.GateWay;
                    gvb_board_info.Reserved2 = gvb_cam_info.Reserved2;
                    ret = m_hDevice.OpenDeviceGVB((uint)BoardType.IKBoardALL, gvb_board_info.BoardIndex, gvb_board_info);
                }
                if (!ret)
                {
                    Close();
                    return false;
                }
                if (!string.IsNullOrEmpty(ConfigFile))
                {
                    if (m_hDevice.LoadConfigurationFromFile(ConfigFile) != (int)ErrorCode.IK_RTN_OK)
                    {
                        Close();
                        return false;
                    }
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
                ret = IKapBoard.IKapClearGrab(m_hDevice.m_hDev);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                ret = IKapBoard.IKapStartGrab(m_hDevice.m_hDev, 1);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                ret = IKapBoard.IKapWaitGrab(m_hDevice.m_hDev);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                IntPtr pUserBuffer = IntPtr.Zero;
                ret = IKapBoard.IKapGetBufferAddress(m_hDevice.m_hDev, 0, ref pUserBuffer);
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                else
                {
                    if (!m_buffer.WriteImage(pUserBuffer, 0))
                    {
                        return false;
                    }
                    else
                    {
                        bitmap = m_buffer.m_bmp;
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
                registerCallBack();
                // 创建缓冲区管理
                m_buffer.CreateBuffer(m_hDevice);

                int ret = (int)ErrorCode.IK_RTN_OK;

                if (DeviceClass == BoardInfoClasss.GigEVisionBoard)
                {
                    uint res = IKapCLib.ItkDevExecuteCommand(m_hCamera, "AcquisitionStop");
                    CheckIKapC(res);
                }
                ret = m_hDevice.StartGrab(0);
                if (DeviceClass == BoardInfoClasss.GigEVisionBoard)
                {
                    uint res = IKapCLib.ItkDevExecuteCommand(m_hCamera, "AcquisitionStart");
                    CheckIKapC(res);
                }
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
                if (!IsGrabbing)
                {
                    return true;
                }
                if (DeviceClass == BoardInfoClasss.GigEVisionBoard)
                {
                    uint res = IKapCLib.ItkDevExecuteCommand(m_hCamera, "AcquisitionStop");
                    CheckIKapC(res);
                }
                int ret = m_hDevice.StopGrab();
                if (ret != (int)ErrorCode.IK_RTN_OK)
                {
                    return false;
                }
                unRegisterCallback();
                m_buffer.FreeBuffer();
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
                    return new BmpImageUtil().SaveImage(savePath);
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
                if (!m_hCamera.Equals(new IntPtr(-1)))
                {
                    if (IsGrabbing)
                    {
                        StopGrab();
                    }
                    IKapCLib.ItkDevClose(m_hCamera);
                    m_hCamera = new IntPtr(-1);
                }
                IsConnected=false;
                return m_hDevice.CloseDevice() == (int)ErrorCode.IK_RTN_OK;
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
            try
            {
                Close();
                IKapCLib.ItkManTerminate();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #region Callback
        delegate void IKapCallBackProc(IntPtr pParam);

        /* @brief：本函数被注册为一个回调函数。当图像采集开始时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When starting grabbing images, the function will be called. */
        private IKapCallBackProc OnGrabStartProc;

        /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called. */
        private IKapCallBackProc OnFrameLostProc;

        /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called. */
        private IKapCallBackProc OnTimeoutProc;

        /* @brief：本函数被注册为一个回调函数。当一帧图像采集完成时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When a frame of image grabbing ready, the function will be called. */
        private IKapCallBackProc OnFrameReadyProc;

        /* @brief：本函数被注册为一个回调函数。当图像采集停止时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When stopping grabbing images, the function will be called. */
        private IKapCallBackProc OnGrabStopProc;
        #endregion

        #region CallbackFunc
        /* @brief：本函数被注册为一个回调函数。当图像采集开始时，函数被调用。
        * @param[in] pParam：输入参数。
        *
        * @brief：This function is registered as a callback function. When starting grabbing images, the function will be called.
        * @param[in] pParam：Input parameter. */
        private void OnGrabStartFunc(IntPtr pParam)
        {
            Console.WriteLine("Start grabbing image");
        }

        /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called.
         * @param[in] pParam：Input parameter. */
        private void OnFrameLostFunc(IntPtr pParam)
        {
            Console.WriteLine("Grab frame lost");
        }

        /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called.
         * @param[in] pParam：Input parameter. */
        private void OnTimeoutFunc(IntPtr pParam)
        {
            Console.WriteLine("Grab image timeout");
        }

        /* @brief：本函数被注册为一个回调函数。当一帧图像采集完成时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When a frame of image grabbing ready, the function will be called.
         * @param[in] pParam：Input parameter. */
        private void OnFrameReadyFunc(IntPtr pParam)
        {
            Console.WriteLine("Grab frame ready");
            try
            {
                IntPtr pUserBuffer = IntPtr.Zero;
                int nFrameSize = 0;
                int nFrameCount = 0;
                int nCurIndex = 0;
                IKapBoard.IKAPBUFFERSTATUS status = new IKapBoard.IKAPBUFFERSTATUS();
                m_hDevice.GetInfo((uint)INFO_ID.IKP_FRAME_COUNT, ref nFrameCount);

                // 获取当前索引
                nCurIndex = m_hDevice.m_nGrabTotalFrameCount % nFrameCount;

                // 获取当前帧状态
                m_hDevice.GetBufferStatus(nCurIndex, ref status);
                if (status.uFull == 1)
                {
                    // 获取当前帧长度和帧数据地址
                    m_hDevice.GetInfo((uint)INFO_ID.IKP_FRAME_SIZE, ref nFrameSize);
                    m_hDevice.GetBufferAddress(nCurIndex, ref pUserBuffer);
                    m_buffer.WriteImage(pUserBuffer, nFrameSize);
                    m_buffer.ReadImage();
                    IKapNewImageEvent?.Invoke(this, (Bitmap)m_buffer.m_bmp.Clone());
                    //IKapNewImageEvent?.BeginInvoke(this, (Bitmap)m_buffer.m_bmp.Clone(), null, null);
                }
                m_hDevice.m_nGrabTotalFrameCount++;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                IKapNewImageEvent?.Invoke(this,null);
                //IKapNewImageEvent?.BeginInvoke(this, null, null, ex.ToString());
            }
        }

        /* @brief：本函数被注册为一个回调函数。当图像采集停止时，函数被调用。
         * @param[in] pParam：输入参数。
         *
         * @brief：This function is registered as a callback function. When stopping grabbing images, the function will be called.
         * @param[in] pParam：Input parameter. */
        private void OnGrabStopFunc(IntPtr pParam)
        {
            Console.WriteLine("Stop grabbing image");
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
        private void registerCallBack()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            IntPtr hPtr = new IntPtr(-1);
            OnGrabStartProc = new IKapCallBackProc(OnGrabStartFunc);
            ret = m_hDevice.RegisterCallback((uint)CallBackEvents.IKEvent_GrabStart, Marshal.GetFunctionPointerForDelegate(OnGrabStartProc), hPtr);

            OnFrameReadyProc = new IKapCallBackProc(OnFrameReadyFunc);
            ret = m_hDevice.RegisterCallback((uint)CallBackEvents.IKEvent_FrameReady, Marshal.GetFunctionPointerForDelegate(OnFrameReadyProc), hPtr);

            OnFrameLostProc = new IKapCallBackProc(OnFrameLostFunc);
            ret = m_hDevice.RegisterCallback((uint)CallBackEvents.IKEvent_FrameLost, Marshal.GetFunctionPointerForDelegate(OnFrameLostProc), hPtr);

            OnTimeoutProc = new IKapCallBackProc(OnTimeoutFunc);
            ret = m_hDevice.RegisterCallback((uint)CallBackEvents.IKEvent_TimeOut, Marshal.GetFunctionPointerForDelegate(OnTimeoutProc), hPtr);

            OnGrabStopProc = new IKapCallBackProc(OnGrabStopFunc);
            ret = m_hDevice.RegisterCallback((uint)CallBackEvents.IKEvent_GrabStop, Marshal.GetFunctionPointerForDelegate(OnGrabStopProc), hPtr);
        }
        // This function will be unregister callback
        private void unRegisterCallback()
        {
            int ret = (int)ErrorCode.IK_RTN_OK;
            ret = m_hDevice.UnRegisterCallback((uint)CallBackEvents.IKEvent_GrabStart);
            ret = m_hDevice.UnRegisterCallback((uint)CallBackEvents.IKEvent_FrameReady);
            ret = m_hDevice.UnRegisterCallback((uint)CallBackEvents.IKEvent_GrabStop);
            ret = m_hDevice.UnRegisterCallback((uint)CallBackEvents.IKEvent_FrameLost);
            ret = m_hDevice.UnRegisterCallback((uint)CallBackEvents.IKEvent_TimeOut);
        }
        #endregion
    }
}
