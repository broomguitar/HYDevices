using HY.Devices.Camera.Utils;
using IKapC.NET;
using MvCamCtrl.NET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace HY.Devices.Camera
{
    #region Pre
    //  public abstract class AbstractHYCamera_IKap_Net : IHYCamera
    //  {
    //      #region members
    //      // 相机设备句柄。
    //      //
    //      // Camera device handle.
    //      private IntPtr m_hCamera = new IntPtr(-1);

    //      // 数据流句柄。
    //      //
    //      // Data stream handle.
    //      private IntPtr m_hStream = new IntPtr(-1);

    //      // 图像列表缓冲区句柄。
    //      //
    //      // Image buffer handle.
    //      List<IntPtr> m_hBufferList = new List<IntPtr>();
    //      // 图像缓冲区句柄。
    //      //
    //      // Image buffer handle.
    //      public IntPtr m_hBuffer = new IntPtr(-1);
    //      // 当前帧索引。
    //      //
    //      // Current frame index.
    //      public int m_nCurFrameIndex = 0;

    //      // 图像缓冲区申请的帧数。
    //      //
    //      // The number of frames requested by buffer.
    //      public int m_nBufferCountOfStream = 1;

    //      // 缓冲区数据。
    //      //
    //      // Buffer data.
    //      public IntPtr m_bufferData = new IntPtr(-1);

    //      BmpImageUtil m_bmpImage = new BmpImageUtil();

    //      // 数据流数量。
    //      //
    //      // The number of data stream.
    //      uint streamCount = 0;

    //      // 图像宽度。
    //      //
    //      // Image width.
    //      long nWidth = 0;

    //      // 图像高度。
    //      //
    //      // Image height.
    //      long nHeight = 0;

    //      // 像素格式。
    //      //
    //      // Pixel format.
    //      ItkBufferFormat nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO8;

    //      // 图像大小。
    //      //
    //      // Image size.
    //      IntPtr nImageSize = Marshal.AllocHGlobal(8);

    //      // 像素格式名称。
    //      //
    //      // Pixel format name.
    //      StringBuilder pixelFormat = new StringBuilder(128);

    //      // 像素格式名称长度。
    //      //
    //      // Pixel format name length.
    //      uint pixelFormatSize = 128;

    //      // 缓冲区大小。
    //      //
    //      // Buffer size.
    //      uint nBufferSize = 0;
    //      #endregion
    //      public uint CameraIndex { get; protected set; } = uint.MaxValue;
    //      public string CameraName { get; protected set; }
    //      public AbstractHYCamera_IKap_Net(uint cameraIndex)
    //      {
    //          CameraIndex = cameraIndex;
    //          try
    //          {
    //              InitEnvironment();
    //              ConfigureCamera(cameraIndex);
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }

    //      }
    //      public AbstractHYCamera_IKap_Net(string serialNum)
    //      {
    //          try
    //          {
    //              InitEnvironment();
    //              ConfigureCamera(serialNum);
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }

    //      }
    //      /// <summary>
    //      /// 相机类型
    //      /// </summary>
    //      public abstract CameraTypes CameraType { get; }

    //      public virtual bool IsConnected { get; protected set; }
    //      public virtual bool IsGrabbing { get; protected set; }
    //      public virtual bool GetImageWidth(out uint width)
    //      {
    //          try
    //          {
    //              long nWidth = 0;
    //              bool ret = IKapCLib.ItkDevGetInt64(m_hCamera, "Width", ref nWidth) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //              width = (uint)nWidth;
    //              return ret;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool SetImageWidth(uint width)
    //      {
    //          try
    //          {
    //              return IKapCLib.ItkDevSetInt64(m_hCamera, "Width", width) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool GetImageHeight(out uint height)
    //      {
    //          try
    //          {
    //              long nHeight = 0;
    //              bool ret = IKapCLib.ItkDevGetInt64(m_hCamera, "Height", ref nHeight) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //              height = (uint)nHeight;
    //              return ret;

    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool SetImageHeight(uint height)
    //      {
    //          try
    //          {
    //              return IKapCLib.ItkDevSetInt64(m_hCamera, "Height", height) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool GetExposureTime(out double exposureTime)
    //      {
    //          try
    //          {
    //              exposureTime = double.NaN;
    //              IntPtr hFeature = new IntPtr(-1);
    //              uint ret = IKapCLib.ItkDevAllocFeature(m_hCamera, "ExposureTime", ref hFeature);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              double val = 0;
    //              ret = IKapCLib.ItkFeatureGetDouble(hFeature, ref val);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              ret = IKapCLib.ItkDevFreeFeature(hFeature);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              exposureTime = val;
    //              return true;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool SetExposureTime(double exposureTime)
    //      {
    //          try
    //          {
    //              IntPtr hFeature = new IntPtr(-1);
    //              uint ret = IKapCLib.ItkDevAllocFeature(m_hCamera, "ExposureTime", ref hFeature);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              ret = IKapCLib.ItkFeatureSetDouble(hFeature, exposureTime);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              ret = IKapCLib.ItkDevFreeFeature(hFeature);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              return true;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool SetAutoExposure(AutoMode mode)
    //      {
    //          try
    //          {
    //              return false;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool GetGain(out double gain)
    //      {
    //          try
    //          {
    //              gain = double.NaN;
    //              IntPtr hFeature = new IntPtr(-1);
    //              uint ret = IKapCLib.ItkDevAllocFeature(m_hCamera, "DigitalGain", ref hFeature);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              double val = 0;
    //              ret = IKapCLib.ItkFeatureGetDouble(hFeature, ref val);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              ret = IKapCLib.ItkDevFreeFeature(hFeature);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              gain = val;
    //              return true;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool SetGain(double gain)
    //      {
    //          try
    //          {
    //              IntPtr hFeature = new IntPtr(-1);
    //              uint ret = IKapCLib.ItkDevAllocFeature(m_hCamera, "DigitalGain", ref hFeature);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              ret = IKapCLib.ItkFeatureSetDouble(hFeature, gain);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              ret = IKapCLib.ItkDevFreeFeature(hFeature);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              return true;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }

    //      }
    //      public virtual bool SetAutoGain(AutoMode mode)
    //      {
    //          try
    //          {
    //              return false;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool GetFrameRate(out double frameRate)
    //      {
    //          try
    //          {
    //              frameRate = float.NaN;
    //              ;
    //              return false;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool SetFrameRate(double frameRate)
    //      {
    //          try
    //          {
    //              return false;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }

    //      }
    //      /// <summary>
    //      /// 获取一个信号触发的帧数
    //      /// </summary>
    //      /// <param name="frameCount"></param>
    //      /// <returns></returns>
    //      public virtual bool GetTriggerOutterModeFrameCount(out uint frameCount)
    //      {
    //          try
    //          {
    //              long nFrameCount = 0;
    //              bool ret = IKapCLib.ItkDevGetInt64(m_hCamera, "TriggerFrameCount", ref nFrameCount) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //              frameCount = (uint)nFrameCount;
    //              return ret;

    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      /// <summary>
    //      ///设置一个信号触发的帧数
    //      /// </summary>
    //      /// <param name="frameCount"></param>
    //      /// <returns></returns>
    //      public virtual bool SetTriggerOutterModeFrameCount(uint frameCount)
    //      {
    //          try
    //          {
    //              return IKapCLib.ItkDevSetInt64(m_hCamera, "TriggerFrameCount", frameCount) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      /// <summary>
    //      /// 设置触发参数
    //      /// </summary>
    //      /// <param name="selector">线触发，帧触发</param>
    //      /// <param name="mode">触发开关</param>
    //      /// <param name="triggerSource">触发源  线 line1,帧line3</param>
    //      /// <returns></returns>
    //      public bool SetTriggerParameters(TriggerSelector selector, TriggerMode mode, string triggerSource = "")
    //      {
    //          try
    //          {
    //              uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //              ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerSelector", selector.ToString());
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //                  return false;
    //              switch (mode)
    //              {
    //                  case TriggerMode.OFF:
    //                      ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerMode", "Off");
    //                      break;
    //                  case TriggerMode.ON:
    //                      ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerMode", "On");
    //                      break;
    //                  default:
    //                      break;
    //              }
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //                  return false;
    //              if (string.IsNullOrEmpty(triggerSource))
    //              {
    //                  triggerSource = selector == TriggerSelector.LineStart ? "Line1" : "Line3";
    //              }
    //              ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerSource", selector.ToString());
    //              return ret == (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool GetTriggerModel(out TriggerMode mode)
    //      {
    //          try
    //          {
    //              mode = TriggerMode.OFF;
    //              uint size = 128;
    //              StringBuilder sb = new StringBuilder();
    //              uint ret = IKapCLib.ItkDevToString(m_hCamera, "TriggerMode", sb, ref size);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //                  return false;
    //              if (Enum.TryParse(sb.ToString(), out mode))
    //              {
    //                  return false;
    //              }
    //              return true;
    //          }
    //          catch (Exception ex)
    //          {
    //              throw ex;
    //          }
    //      }
    //      public virtual bool SetTriggerModel(TriggerMode mode)
    //      {
    //          try
    //          {
    //              uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;                                                     // Return value of IKapBoard methods

    //              // Set board frame trigger parameter
    //              switch (mode)
    //              {
    //                  case TriggerMode.OFF:
    //                      ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerMode", "Off");
    //                      break;
    //                  case TriggerMode.ON:
    //                      ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerMode", "On");
    //                      break;
    //                  default:
    //                      break;
    //              }
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              return true;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool GetTriggerSource(out TriggerSources triggerSource)
    //      {
    //          try
    //          {
    //              triggerSource = TriggerSources.Soft;
    //              return false;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool SetTriggerSource(TriggerSources triggerSources)
    //      {
    //          try
    //          {

    //              return false;
    //          }
    //          catch (Exception ex)
    //          {
    //              throw ex;
    //          }
    //      }
    //      public virtual bool Open()
    //      {
    //          try
    //          {
    //              uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //              IKapCLib.ITKGIGEDEV_INFO gv_board_info = new IKapCLib.ITKGIGEDEV_INFO();

    //              // 打开相机。
    //              //
    //              // Open camera.
    //              ret = IKapCLib.ItkDevOpen(CameraIndex, (int)ItkDeviceAccessMode.ITKDEV_VAL_ACCESS_MODE_CONTROL, ref m_hCamera);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }

    //              // 获取 GigECamera 相机设备信息。
    //              //
    //              // Get GigECamera camera device information.
    //              ret = IKapCLib.ItkManGetGigEDeviceInfo(CameraIndex, ref gv_board_info);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              /// ret= IKapCLib.ItkDevRegisterCallback(m_hCamera, "DeviceRemove", removalCallbackProc, m_hCamera);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //                  return false;
    //              return IsConnected = true;
    //          }
    //          catch (Exception ex)
    //          {
    //              throw ex;
    //          }
    //      }
    //      public abstract bool SoftWareTrigger();
    //      public virtual bool GrabOne(out Bitmap bitmap)
    //      {
    //          try
    //          {
    //              bitmap = null;
    //              if (IsGrabbing)
    //              {
    //                  if (!StopGrab())
    //                  {
    //                      return false;
    //                  }
    //              }
    //              uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //              CreateStreamAndBuffer();
    //              ret = IKapCLib.ItkStreamStart(m_hStream, 1);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              ret = IKapCLib.ItkStreamWait(m_hStream);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }

    //              IntPtr bufferStatus = Marshal.AllocHGlobal(4);
    //              IntPtr nImageSize = Marshal.AllocHGlobal(8);
    //              IntPtr hBuffer = m_hBuffer;
    //              uint nStatus = 0;
    //              uint nBufferSize = 0;

    //              ret = IKapCLib.ItkBufferGetPrm(hBuffer, (uint)ItkBufferPrm.ITKBUFFER_PRM_STATE, (IntPtr)(bufferStatus));
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              nStatus = (uint)Marshal.ReadInt32(bufferStatus);

    //              // 当图像缓冲区满或者图像缓冲区非满但是无法采集完整的一帧图像时。
    //              //
    //              // When buffer is full or buffer is not full but cannot grab a complete frame of image.
    //              if (nStatus == (uint)ItkBufferState.ITKBUFFER_VAL_STATE_FULL || nStatus == (uint)ItkBufferState.ITKBUFFER_VAL_STATE_UNCOMPLETED)
    //              {

    //                  // 读取缓冲区数据。
    //                  //
    //                  // Read buffer data.
    //                  ret = IKapCLib.ItkBufferGetPrm(hBuffer, (uint)ItkBufferPrm.ITKBUFFER_PRM_SIZE, nImageSize);
    //                  if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //                  {
    //                      return false;
    //                  }
    //                  nBufferSize = (uint)Marshal.ReadInt64(nImageSize);
    //                  ret = IKapCLib.ItkBufferRead(hBuffer, 0, m_bufferData, (uint)nBufferSize);
    //                  if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //                  {
    //                      return false;
    //                  }
    //                  m_bmpImage.ReleaseImage();
    //                  if (IsMonoData(nFormat))
    //                  {
    //                      m_bmpImage.CreateImage((int)nWidth, (int)nHeight, 8, 1);
    //                  }
    //                  else if (IsColorData(nFormat))
    //                  {
    //                      byte[] m_pBufForSaveImage = new byte[nBufferSize];
    //                      Marshal.Copy(m_bufferData, m_pBufForSaveImage, 0, m_pBufForSaveImage.Length);
    //                      for (int i = 0; i < (int)nHeight; i++)
    //                      {
    //                          for (int j = 0; j < (int)nWidth; j++)
    //                          {
    //                              byte chRed = m_pBufForSaveImage[(i * (int)nWidth + j) * 3];
    //                              m_pBufForSaveImage[(i * (int)nWidth + j) * 3] = m_pBufForSaveImage[(i * (int)nWidth + j) * 3 + 2];
    //                              m_pBufForSaveImage[(i * (int)nWidth + j) * 3 + 2] = chRed;
    //                          }
    //                      }
    //                      bitmap = new Bitmap((int)nWidth, (int)nHeight, (int)nWidth * 3, System.Drawing.Imaging.PixelFormat.Format24bppRgb, Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0));
    //                      return true;
    //                      m_bmpImage.CreateImage((int)nWidth, (int)nHeight, 8, 3);
    //                  }
    //                  if (m_bmpImage.WriteImageData(m_bufferData, (int)nBufferSize))
    //                  {
    //                      bitmap = m_bmpImage.m_bitmap;
    //                      return true;
    //                  }

    //              }
    //              return false;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //          finally
    //          {
    //              StopGrab();
    //          }
    //      }
    //      public virtual bool ContinousGrab()
    //      {
    //          try
    //          {
    //              if (IsGrabbing)
    //              {
    //                  if (!StopGrab())
    //                  {
    //                      return false;
    //                  }
    //              }
    //              uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //              m_nCurFrameIndex = 0;
    //              CreateStreamAndBuffer(true);
    //              ConfigureStream();
    //              RegisterCallBack();
    //              ret = IKapCLib.ItkStreamStart(m_hStream, (uint)IKapCLib.ITKSTREAM_CONTINUOUS);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              return IsGrabbing = true;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool StopGrab()
    //      {
    //          try
    //          {
    //              if (!IsGrabbing)
    //              {
    //                  return true;
    //              }
    //              uint ret = IKapCLib.ItkStreamStop(m_hStream);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  return false;
    //              }
    //              // 释放数据流和缓冲区。
    //              //
    //              // Free data stream and buffers.
    //              foreach (var it in m_hBufferList)
    //              {
    //                  IKapCLib.ItkStreamRemoveBuffer(m_hStream, it);
    //                  IKapCLib.ItkBufferFree(it);
    //              }
    //              m_hBufferList.Clear();
    //              if (!m_hBuffer.Equals(new IntPtr(-1)))
    //              {
    //                  IKapCLib.ItkStreamRemoveBuffer(m_hStream, m_hBuffer);
    //                  IKapCLib.ItkBufferFree(m_hBuffer);
    //              }
    //              IKapCLib.ItkDevFreeStream(m_hStream);
    //              if (!m_bufferData.Equals(new IntPtr(-1)))
    //              {
    //                  Marshal.FreeHGlobal(m_bufferData);
    //              }
    //              UnRegisterCallback();
    //              IsGrabbing = false;
    //              return true;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool SaveImage(string savePath)
    //      {
    //          try
    //          {
    //              if (GrabOne(out Bitmap bitmap))
    //              {
    //                  return m_bmpImage.SaveImage(savePath);
    //              }
    //              return false;
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      public virtual bool Close()
    //      {
    //          try
    //          {
    //              if (!IsConnected) return true;
    //              if (!m_hCamera.Equals((IntPtr)(-1)))
    //              {
    //                  if (IsGrabbing)
    //                  {
    //                      StopGrab();
    //                  }
    //                  IKapCLib.ItkDevClose(m_hCamera);
    //                  m_hCamera = new IntPtr(-1);
    //              }
    //              IsConnected = false;
    //              return true;
    //          }
    //          catch (Exception ex)
    //          {
    //              throw ex;
    //          }
    //      }
    //      event EventHandler<Bitmap> IKapNewImageEvent;
    //      public event EventHandler<Bitmap> NewImageEvent;

    //      object lockObj = new object();
    //      event EventHandler<Bitmap> IHYCamera.NewImageEvent
    //      {
    //          add
    //          {
    //              lock (lockObj)
    //              {
    //                  IKapNewImageEvent += value;
    //              }
    //          }
    //          remove
    //          {
    //              lock (lockObj)
    //              {
    //                  IKapNewImageEvent -= value;
    //              }
    //          }
    //      }
    //      public virtual void Dispose()
    //      {
    //          try
    //          {
    //              Close();
    //              IKapCLib.ItkManTerminate();
    //          }
    //          catch (Exception ex)
    //          {

    //              throw ex;
    //          }
    //      }
    //      #region Callback
    //      /* @brief：本函数被注册为一个回调函数。当数据流开始时，函数被调用。
    //       *
    //       * @brief：This function is registered as a callback function. When data stream starts, the function will be called. */
    //      public IKapCLib.PITKSTREAMCALLBACK cbOnStartOfStreamProc = null;

    //      /* @brief：本函数被注册为一个回调函数。当一帧图像采集结束时，函数被调用。
    //       *
    //       * @brief：This function is registered as a callback function. When grabbing a frame of image finished, the function will be called. */
    //      public IKapCLib.PITKSTREAMCALLBACK cbOnEndOfFrameProc = null;

    //      /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
    //       *
    //       * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called. */
    //      public IKapCLib.PITKSTREAMCALLBACK cbOnTimeOutProc = null;

    //      /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
    //       *
    //       * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called. */
    //      public IKapCLib.PITKSTREAMCALLBACK cbOnFrameLostProc = null;

    //      /* @brief：本函数被注册为一个回调函数。当数据流结束时，函数被调用。
    //       *
    //       * @brief：This function is registered as a callback function. When data stream ends, the function will be called. */
    //      public IKapCLib.PITKSTREAMCALLBACK cbOnEndOfStreamProc = null;
    //      #endregion

    //      #region CallbackMethods
    //      /// <summary>
    //      /// 掉线
    //      /// </summary>
    //      /// <param name="context"></param>
    //      /// <param name="eventInfo"></param>
    //      void removalCallbackProc(IntPtr context, IntPtr eventInfo)
    //      {
    //          /* Retrieve event type and countstamp */
    //          IsConnected = false;
    //          IntPtr type = Marshal.AllocHGlobal(4);
    //          IntPtr countstamp = Marshal.AllocHGlobal(8);
    //          IKapCLib.ItkEventInfoGetPrm(eventInfo, (uint)ItkEventInfoPrm.ITKEVENTINFO_PRM_TYPE, type);
    //      }
    //      /* @brief：本函数被注册为一个回调函数。当数据流开始时，函数被调用。
    //       * @param[in] eventType：事件类型。
    //       * @param[in] pContext：输入参数。
    //       *
    //       * @brief：This function is registered as a callback function. When data stream starts, the function will be called.
    //       * @param[in] eventType：Event type.
    //       * @param[in] pContext：Input parameter. */
    //      public void cbOnStartOfStreamFunc(uint eventType, IntPtr pContext)
    //      {
    //          Console.WriteLine("Start of stream");
    //      }

    //      /* @brief：本函数被注册为一个回调函数。当一帧图像采集结束时，函数被调用。
    //       * @param[in] eventType：事件类型。
    //       * @param[in] pContext：输入参数。
    //       *
    //       * @brief：This function is registered as a callback function. When grabbing a frame of image finished, the function will be called.
    //       * @param[in] eventType：Event type.
    //       * @param[in] pContext：Input parameter. */
    //      public void cbOnEndOfFrameFunc(uint eventType, IntPtr pContext)
    //      {
    //          Console.WriteLine("End of frame");
    //          uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //          IntPtr bufferStatus = Marshal.AllocHGlobal(4);
    //          IntPtr nImageSize = Marshal.AllocHGlobal(8);
    //          IntPtr hBuffer = m_hBufferList.ElementAt(m_nCurFrameIndex);
    //          uint nStatus = 0;
    //          uint nBufferSize = 0;

    //          ret = IKapCLib.ItkBufferGetPrm(hBuffer, (uint)ItkBufferPrm.ITKBUFFER_PRM_STATE, bufferStatus);
    //          nStatus = (uint)Marshal.ReadInt32(bufferStatus);

    //          // 当图像缓冲区满或者图像缓冲区非满但是无法采集完整的一帧图像时。
    //          //
    //          // When buffer is full or buffer is not full but cannot grab a complete frame of image.
    //          if (nStatus == (uint)ItkBufferState.ITKBUFFER_VAL_STATE_FULL || nStatus == (uint)ItkBufferState.ITKBUFFER_VAL_STATE_UNCOMPLETED)
    //          {
    //              // 保存图像。
    //              //
    //              // Save image. 

    //              //string tempFilePath = $"{AppDomain.CurrentDomain.BaseDirectory}temp.jpg";
    //              //ret = IKapCLib.ItkBufferSave(hBuffer, tempFilePath, (uint)ItkBufferSaveType.ITKBUFFER_VAL_TIFF);
    //              //Bitmap bitmap = new Bitmap(tempFilePath);
    //              //IKapNewImageEvent?.BeginInvoke(this, bitmap, null, null);
    //              //return;
    //              // 读取缓冲区数据。
    //              //
    //              // Read buffer data.
    //              ret = IKapCLib.ItkBufferGetPrm(hBuffer, (uint)ItkBufferPrm.ITKBUFFER_PRM_SIZE, nImageSize);
    //              nBufferSize = (uint)Marshal.ReadInt64(nImageSize);
    //              ret = IKapCLib.ItkBufferRead(hBuffer, 0, m_bufferData, nBufferSize);
    //              IntPtr pImg = m_bufferData;
    //              m_bmpImage.ReleaseImage();
    //              if (IsMonoData(nFormat))
    //              {
    //                  m_bmpImage.CreateImage((int)nWidth, (int)nHeight, 8, 1);
    //              }
    //              else if (IsColorData(nFormat))
    //              {
    //                  byte[] m_pBufForSaveImage = new byte[nBufferSize];
    //                  Marshal.Copy(m_bufferData, m_pBufForSaveImage, 0, m_pBufForSaveImage.Length);
    //                  for (int i = 0; i < (int)nHeight; i++)
    //                  {
    //                      for (int j = 0; j < (int)nWidth; j++)
    //                      {
    //                          byte chRed = m_pBufForSaveImage[(i * (int)nWidth + j) * 3];
    //                          m_pBufForSaveImage[(i * (int)nWidth + j) * 3] = m_pBufForSaveImage[(i * (int)nWidth + j) * 3 + 2];
    //                          m_pBufForSaveImage[(i * (int)nWidth + j) * 3 + 2] = chRed;
    //                      }
    //                  }
    //                  pImg = Marshal.UnsafeAddrOfPinnedArrayElement(m_pBufForSaveImage, 0);
    //                  m_bmpImage.CreateImage((int)nWidth, (int)nHeight, 8, 3);
    //              }
    //              if (m_bmpImage.WriteImageData(pImg, (int)nBufferSize))
    //              {
    //                  IKapNewImageEvent?.BeginInvoke(this, m_bmpImage.m_bitmap, null, null);
    //              }
    //              else
    //              {
    //                  Console.WriteLine("写入图片数据出错");
    //                  IKapNewImageEvent?.BeginInvoke(this, null, null, null);
    //              }
    //          }
    //          else
    //          {
    //              Console.WriteLine(nStatus);
    //              IKapNewImageEvent?.BeginInvoke(this, null, null, null);
    //          }
    //          m_nCurFrameIndex++;
    //          m_nCurFrameIndex = m_nCurFrameIndex % m_nBufferCountOfStream;
    //      }
    //      /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
    //       * @param[in] eventType：事件类型。
    //       * @param[in] pContext：输入参数。
    //       *
    //       * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called.
    //       * @param[in] eventType：Event type.
    //       * @param[in] pContext：Input parameter. */
    //      public void cbOnTimeOutFunc(uint eventType, IntPtr pContext)
    //      {
    //          Console.WriteLine("Grab timeout");
    //      }

    //      /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
    //       * @param[in] eventType：事件类型。
    //       * @param[in] pContext：输入参数。
    //       *
    //       * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called.
    //       * @param[in] eventType：Event type.
    //       * @param[in] pContext：Input parameter. */
    //      public void cbOnFrameLostFunc(uint eventType, IntPtr pContext)
    //      {
    //          Console.WriteLine("Grab Frame lost");
    //      }

    //      /* @brief：本函数被注册为一个回调函数。当数据流结束时，函数被调用。
    //       * @param[in] eventType：事件类型。
    //       * @param[in] pContext：输入参数。
    //       *
    //       * @brief：This function is registered as a callback function. When data stream ends, the function will be called.
    //       * @param[in] eventType：Event type.
    //       * @param[in] pContext：Input parameter. */
    //      public void cbOnEndOfStreamFunc(uint eventType, IntPtr pContext)
    //      {
    //          Console.WriteLine("End of stream");
    //      }
    //      #endregion

    //      #region member function
    //      private bool IsMonoData(ItkBufferFormat format)
    //      {
    //          switch (format)
    //          {
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO8:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO10:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO10PACKED:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO12:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO12PACKED:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO14:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO16:
    //                  return true;
    //              default:
    //                  return false;
    //          }
    //      }
    //      private bool IsColorData(ItkBufferFormat format)
    //      {
    //          switch (format)
    //          {
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB888:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB101010:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB121212:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB141414:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB161616:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR888:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR101010:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR121212:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR141414:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR161616:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR8:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG8:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB8:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG8:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR10:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG10:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB10:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG10:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR10PACKED:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG10PACKED:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB10PACKED:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG10PACKED:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR12:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG12:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB12:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG12:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_YUV422_8_YUYV:
    //              case ItkBufferFormat.ITKBUFFER_VAL_FORMAT_YUV422_8_UYUV:
    //                  return true;
    //              default:
    //                  return false;
    //          }
    //      }
    //      /// <summary>
    //      /// 初始化运行环境
    //      /// </summary>
    //      private void InitEnvironment()
    //      {
    //          // IKapC 函数返回值。
    //          //
    //          // Return value of IKapC functions.
    //          uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;

    //          ret = IKapCLib.ItkManInitialize();
    //          if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //          {
    //              throw new Exception("IKAP相机运行环境初始化异常");
    //          }
    //      }
    //      /// <summary>
    //      /// 加载相机
    //      /// </summary>
    //      /// <param name="cameraIndex">相机索引</param>
    //      private void ConfigureCamera(uint cameraIndex)
    //      {
    //          uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //          uint numCameras = 0;

    //          // 枚举可用相机的数量。在打开相机前，必须调用 ItkManGetDeviceCount() 函数。
    //          //
    //          // Enumerate the number of available cameras. Before opening the camera, ItkManGetDeviceCount() function must be called.
    //          ret = IKapCLib.ItkManGetDeviceCount(ref numCameras);
    //          if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK || numCameras <= cameraIndex)
    //          {
    //              throw new Exception("不存在该IKAP相机");
    //          }
    //          IKapCLib.ITKDEV_INFO di = new IKapCLib.ITKDEV_INFO();
    //          ret = IKapCLib.ItkManGetDeviceInfo(cameraIndex, ref di);
    //          if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //          {
    //              throw new Exception("获取该IKAP相机信息失败");
    //          }
    //          CameraName = di.FullName;
    //          if (di.DeviceClass != "GigEVision" || di.SerialNumber == "")
    //          {
    //              throw new Exception("该IKAP类型不对或者相机序列号号位空");
    //          }
    //      }
    //      /// <summary>
    //      /// 加载相机
    //      /// </summary>
    //      /// <param name="serialNum">相机序列号</param>
    //      private void ConfigureCamera(string serialNum)
    //      {
    //          uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //          uint numCameras = 0;

    //          // 枚举可用相机的数量。在打开相机前，必须调用 ItkManGetDeviceCount() 函数。
    //          //
    //          // Enumerate the number of available cameras. Before opening the camera, ItkManGetDeviceCount() function must be called.
    //          ret = IKapCLib.ItkManGetDeviceCount(ref numCameras);
    //          if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK || numCameras < 1)
    //          {
    //              throw new Exception("不存在该IKAP相机");
    //          }
    //          // Open GigECamera camera.
    //          for (uint i = 0; i < numCameras; i++)
    //          {
    //              IKapCLib.ITKDEV_INFO di = new IKapCLib.ITKDEV_INFO();
    //              ret = IKapCLib.ItkManGetDeviceInfo(i, ref di);
    //              if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
    //              {
    //                  throw new Exception("获取该IKAP相机信息失败");
    //              }

    //              // 当设备为 GigECamera 相机且序列号正确时。
    //              //
    //              // When the device is GigECamera camera and the serial number is proper.
    //              if (di.DeviceClass == "GigEVision" && di.SerialNumber == serialNum)
    //              {
    //                  CameraName = di.FullName;
    //                  CameraIndex = i;

    //                  break;
    //              }
    //          }
    //          if (CameraIndex == uint.MaxValue)
    //          {
    //              throw new Exception("不存在该IKAP相机");
    //          }
    //      }
    //      /* @brief：创建数据流和缓冲区。
    //       *
    //       * @brief：Create data stream and buffer. */
    //      private void CreateStreamAndBuffer(bool isConitnous = false)
    //      {
    //          uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;

    //          // 获取数据流数量。
    //          //
    //          // Get the number of data stream.
    //          ret = IKapCLib.ItkDevGetStreamCount(m_hCamera, ref streamCount);
    //          if (streamCount == 0)
    //          {
    //              IKapCLib.ItkManTerminate();
    //              throw new Exception("Camera does not have image stream channel.");
    //          }

    //          // 获取图像宽度。
    //          //
    //          // Get image width.
    //          ret = IKapCLib.ItkDevGetInt64(m_hCamera, "Width", ref nWidth);

    //          // 获取图像高度。
    //          //
    //          // Get image height.
    //          ret = IKapCLib.ItkDevGetInt64(m_hCamera, "Height", ref nHeight);
    //          // 获取像素格式。
    //          //
    //          // Get pixel format.
    //          ret = IKapCLib.ItkDevToString(m_hCamera, "PixelFormat", pixelFormat, ref pixelFormatSize);
    //          if (pixelFormat.ToString() == "Mono8")
    //              nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO8;
    //          else if (pixelFormat.ToString() == "Mono10")
    //              nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO10;
    //          else if (pixelFormat.ToString() == "Mono10Packed")
    //              nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO10PACKED;
    //          else if (pixelFormat.ToString() == "BayerGR8")
    //              nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR8;
    //          else if (pixelFormat.ToString() == "BayerRG8")
    //              nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG8;
    //          else if (pixelFormat.ToString() == "BayerGB8")
    //              nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB8;
    //          else if (pixelFormat.ToString() == "BayerBG8")
    //              nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG8;
    //          else if (pixelFormat.ToString() == "RGB8")
    //              nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB888;
    //          else
    //          {

    //              IKapCLib.ItkManTerminate();
    //              throw new Exception($"Camera does not support pixel format--- {pixelFormat}.");
    //          }

    //          // 创建图像缓冲区。
    //          //
    //          // Create image buffer.
    //          IntPtr hBuffer = new IntPtr(-1);
    //          ret = IKapCLib.ItkBufferNew(nWidth, nHeight, (uint)nFormat, ref hBuffer);
    //          m_hBuffer = hBuffer;

    //          // 获取缓冲区大小。
    //          //
    //          // Get buffer size.
    //          ret = IKapCLib.ItkBufferGetPrm(hBuffer, (uint)ItkBufferPrm.ITKBUFFER_PRM_SIZE, nImageSize);
    //          nBufferSize = (uint)Marshal.ReadInt64(nImageSize);

    //          // 创建缓冲区数据存储。
    //          //
    //          // Create buffer data saving.
    //          m_bufferData = Marshal.AllocHGlobal((int)nBufferSize);
    //          if (m_bufferData.Equals(new IntPtr(-1)))
    //          {
    //              IKapCLib.ItkManTerminate();
    //              throw new Exception($"Apply buffer data failure");
    //          }

    //          // 申请数据流资源。
    //          //
    //          // Allocate data stream source.
    //          ret = IKapCLib.ItkDevAllocStream(m_hCamera, 0, hBuffer, ref m_hStream);
    //          if (isConitnous)
    //          {
    //              m_hBufferList.Add(hBuffer);
    //              for (int i = 1; i < m_nBufferCountOfStream; i++)
    //              {
    //                  ret = IKapCLib.ItkBufferNew(nWidth, nHeight, (uint)nFormat, ref hBuffer);
    //                  ret = IKapCLib.ItkStreamAddBuffer(m_hStream, hBuffer);
    //                  m_hBufferList.Add(hBuffer);
    //              }
    //          }
    //          else
    //          {
    //              m_hBuffer = hBuffer;
    //          }
    //      }
    //      /* @brief：配置数据流。
    //   *
    //   * @brief：Configure data stream. */
    //      private void ConfigureStream()
    //      {
    //          uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;

    //          // 传输模式。
    //          //
    //          // Transfer mode.
    //          IntPtr xferMode = Marshal.AllocHGlobal(4);
    //          Marshal.WriteInt32(xferMode, 0, (int)ItkStreamTransferMode.ITKSTREAM_VAL_TRANSFER_MODE_SYNCHRONOUS_WITH_PROTECT);

    //          // 采集模式。
    //          //
    //          // Grab mode.
    //          IntPtr startMode = Marshal.AllocHGlobal(4);
    //          Marshal.WriteInt32(startMode, 0, (int)ItkStreamStartMode.ITKSTREAM_VAL_START_MODE_NON_BLOCK);

    //          // 超时时间。
    //          //
    //          // Time out time.
    //          IntPtr timeOut = Marshal.AllocHGlobal(4);
    //          Marshal.WriteInt32(timeOut, 0, (int)IKapCLib.ITKSTREAM_CONTINUOUS);

    //          // 设置采集模式。
    //          //
    //          // Set grab mode.
    //          ret = IKapCLib.ItkStreamSetPrm(m_hStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_START_MODE, startMode);

    //          // 设置传输模式。
    //          //
    //          // Set transfer mode.
    //          ret = IKapCLib.ItkStreamSetPrm(m_hStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_TRANSFER_MODE, xferMode);

    //          // 设置超时时间。
    //          //
    //          // Set time out time.
    //          ret = IKapCLib.ItkStreamSetPrm(m_hStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_TIME_OUT, timeOut);
    //      }
    //      /// <summary>
    //      /// 注册回调函数
    //      /// </summary>
    //      private void RegisterCallBack()
    //      {
    //          uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //          cbOnStartOfStreamProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnStartOfStreamFunc);
    //          ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM, cbOnStartOfStreamProc, m_hStream);
    //          cbOnEndOfStreamProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnEndOfStreamFunc);
    //          ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM, cbOnEndOfStreamProc, m_hStream);
    //          cbOnEndOfFrameProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnEndOfFrameFunc);
    //          ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME, cbOnEndOfFrameProc, m_hStream);
    //          cbOnTimeOutProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnTimeOutFunc);
    //          ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT, cbOnTimeOutProc, m_hStream);
    //          cbOnFrameLostProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnFrameLostFunc);
    //          ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST, cbOnFrameLostProc, m_hStream);
    //      }
    //      /* @brief：清除回调函数。
    //*
    //* @brief：Unregister callback functions. */
    //      private void UnRegisterCallback()
    //      {
    //          uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
    //          ret = IKapCLib.ItkDevUnregisterCallback(m_hCamera, "DeviceRemove");
    //          ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM);
    //          ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM);
    //          ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME);
    //          ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT);
    //          ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST);
    //      }
    //      #endregion
    //  }
    #endregion
    public abstract class AbstractHYCamera_IKap_Net : IHYCamera
    {
        #region members
        // 相机设备句柄 || Camera device handle.
         IntPtr m_hCamera = new IntPtr(-1);

        // 数据流句柄 || Data stream handle.
         IntPtr m_hStream = new IntPtr(-1);

        // 图像列表缓冲区句柄|| Image buffer handle.
        List<IntPtr> m_hBufferList = new List<IntPtr>();

        // 图像缓冲区句柄||Image buffer handle.
         IntPtr m_hBuffer = new IntPtr(-1);

        // 当前帧索引|| Current frame index.
         int m_nCurFrameIndex = 0;

        // 图像缓冲区申请的帧数||The number of frames requested by buffer.
         int m_nBufferCountOfStream = 3;

        // 缓冲区数据|| Buffer data.
         IntPtr m_bufferData = new IntPtr(-1);

        //图片保存类
        BmpImageUtil_New m_bmpImage = new BmpImageUtil_New();

        // 数据流数量|| The number of data stream.
        uint m_streamCount = 0;

        // 图像宽度||Image width.
        long m_nWidth = 0;

        // 图像高度||Image height.
        long m_nHeight = 0;

        // 像素位数
        int m_nDepth = 8;

        // 图像通道数
        int m_nChannels = 1;

        // 像素格式||Pixel format.
        ItkBufferFormat m_nFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB888;
        // 相机缓冲区大小
        int m_nBufferSize = 0;
        /// <summary>
        /// 图像缓冲区锁
        /// </summary>
        object m_lockBmp = new object();
        #endregion
        public uint CameraIndex { get; protected set; } = uint.MaxValue;
        public string CameraName { get; protected set; }
        public AbstractHYCamera_IKap_Net(uint cameraIndex)
        {
            CameraIndex = cameraIndex;
            try
            {
                InitEnvironment();
                ConfigureCamera(cameraIndex);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public AbstractHYCamera_IKap_Net(string serialNum)
        {
            try
            {
                InitEnvironment();
                ConfigureCamera(serialNum);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        /// <summary>
        /// 相机类型
        /// </summary>
        public abstract CameraTypes CameraType { get; }

        public virtual bool IsConnected { get; protected set; }
        public virtual bool IsGrabbing { get; protected set; }
        public virtual bool SetBufferCountOfStream(uint count=3)
        {
                m_nBufferCountOfStream = (int)count;
                return true;;
        }
        public virtual bool GetImageWidth(out uint width)
        {
            try
            {
                long nWidth = 0;
                bool ret = IKapCLib.ItkDevGetInt64(m_hCamera, "Width", ref nWidth) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
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
                return IKapCLib.ItkDevSetInt64(m_hCamera, "Width", width) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
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
                long nHeight = 0;
                bool ret = IKapCLib.ItkDevGetInt64(m_hCamera, "Height", ref nHeight) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
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
                return IKapCLib.ItkDevSetInt64(m_hCamera, "Height", height) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
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
                IntPtr hFeature = new IntPtr(-1);
                uint ret = IKapCLib.ItkDevAllocFeature(m_hCamera, "ExposureTime", ref hFeature);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                double val = 0;
                ret = IKapCLib.ItkFeatureGetDouble(hFeature, ref val);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                ret = IKapCLib.ItkDevFreeFeature(hFeature);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                exposureTime = val;
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
                IntPtr hFeature = new IntPtr(-1);
                uint ret = IKapCLib.ItkDevAllocFeature(m_hCamera, "ExposureTime", ref hFeature);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                ret = IKapCLib.ItkFeatureSetDouble(hFeature, exposureTime);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                ret = IKapCLib.ItkDevFreeFeature(hFeature);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
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
                IntPtr hFeature = new IntPtr(-1);
                uint ret = IKapCLib.ItkDevAllocFeature(m_hCamera, "DigitalGain", ref hFeature);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                double val = 0;
                ret = IKapCLib.ItkFeatureGetDouble(hFeature, ref val);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                ret = IKapCLib.ItkDevFreeFeature(hFeature);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                gain = val;
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
                IntPtr hFeature = new IntPtr(-1);
                uint ret = IKapCLib.ItkDevAllocFeature(m_hCamera, "DigitalGain", ref hFeature);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                ret = IKapCLib.ItkFeatureSetDouble(hFeature, gain);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                ret = IKapCLib.ItkDevFreeFeature(hFeature);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
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
                frameRate = float.NaN;
                ;
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
                return false;
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
                frameCount = 0;
                long nFrameCount = 0;
                uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
                ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerSelector", TriggerSelector.FrameStart.ToString());
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                    return false;
                ret = IKapCLib.ItkDevGetInt64(m_hCamera, "TriggerFrameCount", ref nFrameCount);
                frameCount = (uint)nFrameCount;
                return ret == (uint)ItkStatusErrorId.ITKSTATUS_OK;

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
                uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
                ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerSelector", TriggerSelector.FrameStart.ToString());
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                    return false;
                return IKapCLib.ItkDevSetInt64(m_hCamera, "TriggerFrameCount", (long)frameCount) == (uint)ItkStatusErrorId.ITKSTATUS_OK;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 设置触发参数
        /// </summary>
        /// <param name="selector">线触发，帧触发</param>
        /// <param name="mode">触发开关</param>
        /// <param name="triggerSource">触发源  线 line1,帧line3</param>
        /// <returns></returns>
        public bool SetTriggerParameters(TriggerSelector selector, TriggerMode mode, string triggerSource = "")
        {
            try
            {
                uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
                ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerSelector", selector.ToString());
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                    return false;
                switch (mode)
                {
                    case TriggerMode.OFF:
                        ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerMode", "Off");
                        break;
                    case TriggerMode.ON:
                        ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerMode", "On");
                        break;
                    default:
                        break;
                }
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                    return false;
                if (string.IsNullOrEmpty(triggerSource))
                {
                    triggerSource = selector == TriggerSelector.LineStart ? "Line1" : "Line3";
                }
                ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerSource", selector.ToString());
                return ret == (uint)ItkStatusErrorId.ITKSTATUS_OK;
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
                uint size = 128;
                StringBuilder sb = new StringBuilder();
                uint ret = IKapCLib.ItkDevToString(m_hCamera, "TriggerMode", sb, ref size);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                    return false;
                if (Enum.TryParse(sb.ToString(), out mode))
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
        public virtual bool SetTriggerModel(TriggerMode mode)
        {
            try
            {
                uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;                                                     // Return value of IKapBoard methods

                // Set board frame trigger parameter
                switch (mode)
                {
                    case TriggerMode.OFF:
                        ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerMode", "Off");
                        break;
                    case TriggerMode.ON:
                        ret = IKapCLib.ItkDevFromString(m_hCamera, "TriggerMode", "On");
                        break;
                    default:
                        break;
                }
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
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
        public virtual bool Open()
        {
            try
            {
                if (IsConnected)
                {
                    Close();
                }
                uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
                IKapCLib.ITKGIGEDEV_INFO gv_board_info = new IKapCLib.ITKGIGEDEV_INFO();

                // 打开相机。
                //
                // Open camera.
                ret = IKapCLib.ItkDevOpen(CameraIndex, (int)ItkDeviceAccessMode.ITKDEV_VAL_ACCESS_MODE_CONTROL, ref m_hCamera);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }

                // 获取 GigECamera 相机设备信息。
                //
                // Get GigECamera camera device information.
                ret = IKapCLib.ItkManGetGigEDeviceInfo(CameraIndex, ref gv_board_info);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                /// ret= IKapCLib.ItkDevRegisterCallback(m_hCamera, "DeviceRemove", removalCallbackProc, m_hCamera);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                    return false;
                return IsConnected = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public abstract bool SoftWareTrigger();
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
                uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
                CreateStreamAndBuffer();
                ret = IKapCLib.ItkStreamStart(m_hStream, 1);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                ret = IKapCLib.ItkStreamWait(m_hStream);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                IntPtr hBuffer = m_hBuffer;
                IntPtr bufferStatus = Marshal.AllocHGlobal(4);
                uint nStatus = 0;
                ret = IKapCLib.ItkBufferGetPrm(m_hBufferList[m_nCurFrameIndex], (uint)ItkBufferPrm.ITKBUFFER_PRM_STATE, bufferStatus);
                nStatus = (uint)Marshal.ReadInt32(bufferStatus);

                if (nStatus != (uint)ItkBufferState.ITKBUFFER_VAL_STATE_FULL)
                {
                    Console.WriteLine("写入图片数据出错");
                    return false;
                }
                Marshal.FreeHGlobal(bufferStatus);
                lock (m_lockBmp)
                {
                    IKapCLib.ItkBufferRead(m_hBufferList[m_nCurFrameIndex], 0, m_bufferData, (uint)m_nBufferSize);
                    if (m_bmpImage.CreateImage((int)m_nWidth, (int)m_nHeight, m_nBufferSize, m_nDepth, m_nChannels)&&m_bmpImage.WriteImageData(m_bufferData))
                    {
                        bitmap = m_bmpImage.m_bitmap;
                    }
                    else
                    {
                        Console.WriteLine("写入图片数据出错");
                        return false;
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
                uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
                m_nCurFrameIndex = 0;
                CreateStreamAndBuffer(true);
                ConfigureStream();
                RegisterCallBack();
                m_bmpImage.CreateImage((int)m_nWidth, (int)m_nHeight, m_nBufferSize, m_nDepth, m_nChannels);
                ret = IKapCLib.ItkStreamStart(m_hStream, (uint)IKapCLib.ITKSTREAM_CONTINUOUS);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
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
                uint ret = IKapCLib.ItkStreamStop(m_hStream);
                if (ret != (uint)ItkStatusErrorId.ITKSTATUS_OK)
                {
                    return false;
                }
                // 释放数据流和缓冲区。
                //
                // Free data stream and buffers.
                foreach (var it in m_hBufferList)
                {
                    IKapCLib.ItkStreamRemoveBuffer(m_hStream, it);
                    IKapCLib.ItkBufferFree(it);
                }
                m_hBufferList.Clear();
                if (!m_hBuffer.Equals(new IntPtr(-1)))
                {
                    IKapCLib.ItkStreamRemoveBuffer(m_hStream, m_hBuffer);
                    IKapCLib.ItkBufferFree(m_hBuffer);
                    m_hBuffer = new IntPtr(-1);
                }
                IKapCLib.ItkDevFreeStream(m_hStream);
                if (!m_bufferData.Equals(new IntPtr(-1)))
                {
                    Marshal.FreeHGlobal(m_bufferData);
                    m_bufferData = new IntPtr(-1);
                }
                if (m_bmpImage != null)
                {
                    m_bmpImage.ReleaseImage();
                }
                UnRegisterCallback();
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
                if (!m_hCamera.Equals((IntPtr)(-1)))
                {
                    if (IsGrabbing)
                    {
                        StopGrab();
                    }
                    IKapCLib.ItkDevClose(m_hCamera);
                    m_hCamera = (IntPtr)(-1);
                }
                IsConnected = false;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        event EventHandler<Bitmap> IKapNewImageEvent;
        public event EventHandler<Bitmap> NewImageEvent;

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
        /* @brief：本函数被注册为一个回调函数。当数据流开始时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When data stream starts, the function will be called. */
        public IKapCLib.PITKSTREAMCALLBACK cbOnStartOfStreamProc = null;

        /* @brief：本函数被注册为一个回调函数。当一帧图像采集结束时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When grabbing a frame of image finished, the function will be called. */
        public IKapCLib.PITKSTREAMCALLBACK cbOnEndOfFrameProc = null;

        /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called. */
        public IKapCLib.PITKSTREAMCALLBACK cbOnTimeOutProc = null;

        /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called. */
        public IKapCLib.PITKSTREAMCALLBACK cbOnFrameLostProc = null;

        /* @brief：本函数被注册为一个回调函数。当数据流结束时，函数被调用。
         *
         * @brief：This function is registered as a callback function. When data stream ends, the function will be called. */
        public IKapCLib.PITKSTREAMCALLBACK cbOnEndOfStreamProc = null;
        #endregion
        #region CallbackMethods
        /// <summary>
        /// 掉线
        /// </summary>
        /// <param name="context"></param>
        /// <param name="eventInfo"></param>
        void removalCallbackProc(IntPtr context, IntPtr eventInfo)
        {
            /* Retrieve event type and countstamp */
            IsConnected = false;
            IntPtr type = Marshal.AllocHGlobal(4);
            IntPtr countstamp = Marshal.AllocHGlobal(8);
            IKapCLib.ItkEventInfoGetPrm(eventInfo, (uint)ItkEventInfoPrm.ITKEVENTINFO_PRM_TYPE, type);
        }
        /* @brief：本函数被注册为一个回调函数。当数据流开始时，函数被调用。
         * @param[in] eventType：事件类型。
         * @param[in] pContext：输入参数。
         *
         * @brief：This function is registered as a callback function. When data stream starts, the function will be called.
         * @param[in] eventType：Event type.
         * @param[in] pContext：Input parameter. */
        public void cbOnStartOfStreamFunc(uint eventType, IntPtr pContext)
        {
            Console.WriteLine("Start of stream");
        }
        IntPtr tt = IntPtr.Zero;
        /* @brief：本函数被注册为一个回调函数。当一帧图像采集结束时，函数被调用。
         * @param[in] eventType：事件类型。
         * @param[in] pContext：输入参数。
         *
         * @brief：This function is registered as a callback function. When grabbing a frame of image finished, the function will be called.
         * @param[in] eventType：Event type.
         * @param[in] pContext：Input parameter. */
        public void cbOnEndOfFrameFunc(uint eventType, IntPtr pContext)
        {
            Console.WriteLine("End of frame");
            try
            {
                uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
                IntPtr bufferStatus = Marshal.AllocHGlobal(4);
                uint nStatus = 0;
                ret = IKapCLib.ItkBufferGetPrm(m_hBufferList[m_nCurFrameIndex], (uint)ItkBufferPrm.ITKBUFFER_PRM_STATE, bufferStatus);
                nStatus = (uint)Marshal.ReadInt32(bufferStatus);
                if (nStatus != (uint)ItkBufferState.ITKBUFFER_VAL_STATE_FULL)
                {
                    Console.WriteLine("写入图片数据出错");
                    IKapNewImageEvent?.Invoke(this, null);
                    //IKapNewImageEvent?.BeginInvoke(this, null, null, null);
                }
                else
                {
                    lock (m_lockBmp)
                    {
                        IKapCLib.ItkBufferRead(m_hBufferList[m_nCurFrameIndex], 0, m_bufferData, (uint)m_nBufferSize);
                        if (m_bmpImage.WriteImageData(m_bufferData))
                        {
                            IKapNewImageEvent?.Invoke(this, (Bitmap)m_bmpImage.m_bitmap.Clone());
                        }
                        else
                        {
                            Console.WriteLine("写入图片数据出错");
                            IKapNewImageEvent?.Invoke(this, null);
                        }
                    }
                }
                Marshal.FreeHGlobal(bufferStatus);
                m_nCurFrameIndex++;
                m_nCurFrameIndex = m_nCurFrameIndex % m_nBufferCountOfStream;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /* @brief：本函数被注册为一个回调函数。当图像采集超时时，函数被调用。
         * @param[in] eventType：事件类型。
         * @param[in] pContext：输入参数。
         *
         * @brief：This function is registered as a callback function. When grabbing images time out, the function will be called.
         * @param[in] eventType：Event type.
         * @param[in] pContext：Input parameter. */
        public void cbOnTimeOutFunc(uint eventType, IntPtr pContext)
        {
            Console.WriteLine("Grab timeout");
        }

        /* @brief：本函数被注册为一个回调函数。当采集丢帧时，函数被调用。
         * @param[in] eventType：事件类型。
         * @param[in] pContext：输入参数。
         *
         * @brief：This function is registered as a callback function. When grabbing frame lost, the function will be called.
         * @param[in] eventType：Event type.
         * @param[in] pContext：Input parameter. */
        public void cbOnFrameLostFunc(uint eventType, IntPtr pContext)
        {
            Console.WriteLine("Grab Frame lost");
        }

        /* @brief：本函数被注册为一个回调函数。当数据流结束时，函数被调用。
         * @param[in] eventType：事件类型。
         * @param[in] pContext：输入参数。
         *
         * @brief：This function is registered as a callback function. When data stream ends, the function will be called.
         * @param[in] eventType：Event type.
         * @param[in] pContext：Input parameter. */
        public void cbOnEndOfStreamFunc(uint eventType, IntPtr pContext)
        {
            Console.WriteLine("End of stream");
        }
        #endregion

        #region member function
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
        /// 加载相机
        /// </summary>
        /// <param name="cameraIndex">相机索引</param>
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
            if (di.DeviceClass != "GigEVision" || di.SerialNumber == "")
            {
                throw new Exception("该IKAP类型不对或者相机序列号号位空");
            }
        }
        /// <summary>
        /// 加载相机
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
                // When the device is GigECamera camera and the serial number is proper.
                if (di.DeviceClass == "GigEVision" && di.SerialNumber == serialNum)
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
                retList.Add(new CameraItemInfo { CameraType = connectType, Index = (int)i, Name = di.ModelName, SN = di.SerialNumber});
            }
            return retList;
        }
        /*
         *@brief:获取相机图片格式
         *@param [in]:
         *@return:相机图片格式
         */
        private bool getPixelFormat(StringBuilder sPixelFormat, out ItkBufferFormat nPixelFormat)
        {
            nPixelFormat = m_nFormat;
            if (sPixelFormat.ToString() == "Mono8")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO8;
                m_nDepth = 8;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "Mono10")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "Mono12")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_MONO12;
                m_nDepth = 12;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGR8")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR8;
                m_nDepth = 8;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerRG8")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG8;
                m_nDepth = 8;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGB8")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB8;
                m_nDepth = 8;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerBG8")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG8;
                m_nDepth = 8;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGR10")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerRG10")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGB10")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerBG10")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG10;
                m_nDepth = 10;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGR12")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GR12;
                m_nDepth = 12;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerRG12")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_RG12;
                m_nDepth = 12;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerGB12")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_GB12;
                m_nDepth = 12;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "BayerBG12")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BAYER_BG12;
                m_nDepth = 12;
                m_nChannels = 1;
            }
            else if (sPixelFormat.ToString() == "RGB8")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB888;
                m_nDepth = 8;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "RGB10")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB101010;
                m_nDepth = 10;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "RGB12")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_RGB121212;
                m_nDepth = 12;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "BGR8")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR888;
                m_nDepth = 8;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "BGR10")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR101010;
                m_nDepth = 10;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "BGR12")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_BGR121212;
                m_nDepth = 12;
                m_nChannels = 3;
            }
            else if (sPixelFormat.ToString() == "YUV422_8")
            {
                nPixelFormat = ItkBufferFormat.ITKBUFFER_VAL_FORMAT_YUV422_8_UYUV;
                m_nDepth = 8;
                m_nChannels = 3;
            }
            else
            {
                return false;
            }
            return true;
        }
        /* @brief：创建数据流和缓冲区。
         *
         * @brief：Create data stream and buffer. */
        private void CreateStreamAndBuffer(bool isConitnous = false)
        {
            uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;

            // 获取数据流数量。
            //
            // Get the number of data stream.
            ret = IKapCLib.ItkDevGetStreamCount(m_hCamera, ref m_streamCount);
            if (m_streamCount == 0)
            {
                IKapCLib.ItkManTerminate();
                throw new Exception("Camera does not have image stream channel.");
            }

            // 获取图像宽度。
            //
            // Get image width.
            ret = IKapCLib.ItkDevGetInt64(m_hCamera, "Width", ref m_nWidth);

            // 获取图像高度。
            //
            // Get image height.
            ret = IKapCLib.ItkDevGetInt64(m_hCamera, "Height", ref m_nHeight);
            StringBuilder sPixelFormat = new StringBuilder(64);
            uint pixelFormatSize = 64;
            // 获取像素格式。
            //
            // Get pixel format.
            ret = IKapCLib.ItkDevToString(m_hCamera, "PixelFormat", sPixelFormat, ref pixelFormatSize);
            if (!getPixelFormat(sPixelFormat, out m_nFormat))
            {

                IKapCLib.ItkManTerminate();
                throw new Exception($"Camera does not support pixel format--- {sPixelFormat}.");
            }
            // 创建图像缓冲区。
            //
            // Create image buffer.
            IntPtr hBuffer = new IntPtr(-1);
            ret = IKapCLib.ItkBufferNew(m_nWidth, m_nHeight, (uint)m_nFormat, ref hBuffer);
            if (isConitnous)
            {
                m_hBufferList.Add(hBuffer);
                // 申请数据流资源。
                //
                // Allocate data stream source.
                ret = IKapCLib.ItkDevAllocStream(m_hCamera, 0, hBuffer, ref m_hStream);
                for (int i = 1; i < m_nBufferCountOfStream; i++)
                {
                    ret = IKapCLib.ItkBufferNew(m_nWidth, m_nHeight, (uint)m_nFormat, ref hBuffer);
                    ret = IKapCLib.ItkStreamAddBuffer(m_hStream, hBuffer);
                    m_hBufferList.Add(hBuffer);
                }
            }
            else
            {
                m_hBuffer = hBuffer;
                // 申请数据流资源。
                //
                // Allocate data stream source.
                ret = IKapCLib.ItkDevAllocStream(m_hCamera, 0, hBuffer, ref m_hStream);
            }
            // 缓冲区大小|| Buffer size.
            uint nBufferSize = 0;
            // 图像大小|| Image size.
            IntPtr nImageSize = Marshal.AllocHGlobal(8);
            // 获取缓冲区大小。
            //
            // Get buffer size.
            ret = IKapCLib.ItkBufferGetPrm(hBuffer, (uint)ItkBufferPrm.ITKBUFFER_PRM_SIZE, nImageSize);
            nBufferSize = (uint)Marshal.ReadInt64(nImageSize);
            Marshal.FreeHGlobal(nImageSize);
            // 创建缓冲区数据存储。
            //
            // Create buffer data saving.
            m_bufferData = Marshal.AllocHGlobal((int)nBufferSize);
            if (m_bufferData.Equals(new IntPtr(-1)))
            {
                IKapCLib.ItkManTerminate();
                throw new Exception($"Apply buffer data failure");
            }
            m_nBufferSize = (int)nBufferSize;
        }
        /* @brief：配置数据流。
     *
     * @brief：Configure data stream. */
        private void ConfigureStream()
        {
            uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;

            // 传输模式。
            //
            // Transfer mode.
            IntPtr xferMode = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(xferMode, 0, (int)ItkStreamTransferMode.ITKSTREAM_VAL_TRANSFER_MODE_SYNCHRONOUS_WITH_PROTECT);

            // 采集模式。
            //
            // Grab mode.
            IntPtr startMode = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(startMode, 0, (int)ItkStreamStartMode.ITKSTREAM_VAL_START_MODE_NON_BLOCK);

            // 超时时间。
            //
            // Time out time.
            IntPtr timeOut = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(timeOut, 0, (int)IKapCLib.ITKSTREAM_CONTINUOUS);
            // 行超时时间。
            //
            // Time out time.
            IntPtr lineTimeOut = Marshal.AllocHGlobal(4);
            Marshal.WriteInt32(lineTimeOut, 0, 60*60*1000);

            // 设置采集模式。
            //
            // Set grab mode.
            ret = IKapCLib.ItkStreamSetPrm(m_hStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_START_MODE, startMode);

            // 设置传输模式。
            //
            // Set transfer mode.
            ret = IKapCLib.ItkStreamSetPrm(m_hStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_TRANSFER_MODE, xferMode);

            // 设置超时时间。
            //
            // Set time out time.
            ret = IKapCLib.ItkStreamSetPrm(m_hStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_TIME_OUT, timeOut);
            // 设置行超时时间。
            //
            // Set time out time.
            ret = IKapCLib.ItkStreamSetPrm(m_hStream, (uint)ItkStreamPrm.ITKSTREAM_PRM_GV_PACKET_INTER_TIMEOUT, lineTimeOut);
            Marshal.FreeHGlobal(xferMode);
            Marshal.FreeHGlobal(startMode);
            Marshal.FreeHGlobal(timeOut);
            Marshal.FreeHGlobal(lineTimeOut);
        }
        /// <summary>
        /// 注册回调函数
        /// </summary>
        private void RegisterCallBack()
        {
            uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            cbOnStartOfStreamProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnStartOfStreamFunc);
            ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM, cbOnStartOfStreamProc, m_hStream);
            cbOnEndOfStreamProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnEndOfStreamFunc);
            ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM, cbOnEndOfStreamProc, m_hStream);
            cbOnEndOfFrameProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnEndOfFrameFunc);
            ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME, cbOnEndOfFrameProc, m_hStream);
            cbOnTimeOutProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnTimeOutFunc);
            ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT, cbOnTimeOutProc, m_hStream);
            cbOnFrameLostProc = new IKapCLib.PITKSTREAMCALLBACK(cbOnFrameLostFunc);
            ret = IKapCLib.ItkStreamRegisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST, cbOnFrameLostProc, m_hStream);
        }
        /* @brief：清除回调函数。
  *
  * @brief：Unregister callback functions. */
        private void UnRegisterCallback()
        {
            uint ret = (uint)ItkStatusErrorId.ITKSTATUS_OK;
            ret = IKapCLib.ItkDevUnregisterCallback(m_hCamera, "DeviceRemove");
            ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_START_OF_STREAM);
            ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_STREAM);
            ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_END_OF_FRAME);
            ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_TIME_OUT);
            ret = IKapCLib.ItkStreamUnregisterCallback(m_hStream, (uint)ItkStreamEventType.ITKSTREAM_VAL_EVENT_TYPE_FRAME_LOST);
        }
        #endregion
    }
    public enum TriggerSelector
    {
        LineStart,
        FrameStart
    }
}
