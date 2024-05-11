using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Documents;

namespace HY.Devices.Camera
{
    /// <summary>
    /// 相机模块
    /// </summary>
    public interface IHYCamera : IDisposable
    {
        /// <summary>
        /// 相机类型
        /// </summary>
        CameraTypes CameraType { get; }
        /// <summary>
        /// 相机索引
        /// </summary>
        uint CameraIndex { get; }
        /// <summary>
        /// 相机名字
        /// </summary>
        string CameraName { get; }
        /// <summary>
        /// 是否在线
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 是否在采集
        /// </summary>
        bool IsGrabbing { get; }
        /// <summary>
        /// 获取当前宽度
        /// </summary>
        /// <returns></returns>
        bool GetImageWidth(out uint width);
        /// <summary>
        /// 设置宽度
        /// </summary>
        /// <returns></returns>
        bool SetImageWidth(uint width);
        /// <summary>
        /// 获取当前高度
        /// </summary>
        /// <returns></returns>
        bool GetImageHeight(out uint height);
        /// <summary>
        /// 设置高度
        /// </summary>
        /// <returns></returns>
        bool SetImageHeight(uint height);
        /// <summary>
        /// 获取当前曝光时间
        /// </summary>
        /// <returns></returns>
        bool GetExposureTime(out double exposureTime);
        /// <summary>
        /// 设置曝光时间
        /// </summary>
        /// <returns></returns>
        bool SetExposureTime(double exposureTime);
        /// <summary>
        /// 设置自动曝光
        /// </summary>
        /// <returns></returns>
        bool SetAutoExposure(AutoMode mode);
        /// <summary>
        /// 获取当前增益
        /// </summary>
        /// <returns></returns>
        bool GetGain(out double gain);
        /// <summary>
        /// 设置增益
        /// </summary>
        /// <returns></returns>
        bool SetGain(double gain);
        /// <summary>
        /// 设置自动增益
        /// </summary>
        /// <returns></returns>
        bool SetAutoGain(AutoMode mode);
        /// <summary>
        /// 获取当前帧率
        /// </summary>
        /// <returns></returns>
        bool GetFrameRate(out double frameRate);
        /// <summary>
        /// 设置帧率
        /// </summary>
        /// <returns></returns>
        bool SetFrameRate(double frameRate);
        /// <summary>
        /// 触发模式是否打开
        /// </summary>
        /// <returns>true 打开 false 关闭</returns>
        bool GetTriggerModel(out TriggerMode model);
        /// <summary>
        /// 设置触发模式打开或关闭
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        bool SetTriggerModel(TriggerMode model);
        /// <summary>
        /// 获取当前触发源，触发模式打开有效
        /// </summary>
        /// <returns></returns>
        bool GetTriggerSource(out TriggerSources triggerSource);
        /// <summary>
        /// 设置触发源,触发模式打开有效
        /// </summary>
        /// <param name="triggerSources"></param>
        /// <returns></returns>
        bool SetTriggerSource(TriggerSources triggerSources);
        /// <summary>
        /// 打开相机
        /// </summary>
        /// <returns></returns>
        bool Open();
        /// <summary>
        /// 软触发一次,只有触发模式打开并且设置为软触发，才生效
        /// </summary>
        /// <returns></returns>
        bool SoftWareTrigger();
        /// <summary>
        /// 抓拍一帧图片
        /// </summary>
        bool GrabOne(out Bitmap bitmap);
        /// <summary>
        /// 连续采集
        /// </summary>
        /// <returns></returns>
        bool ContinousGrab();
        /// <summary>
        /// 停止采集
        /// </summary>
        /// <returns></returns>
        bool StopGrab();
        /// <summary>
        /// 保存图片
        /// </summary>
        bool SaveImage(string savePath);
        /// <summary>
        /// 关闭相机`
        /// </summary>
        /// <returns></returns>
        bool Close();
        /// <summary>
        /// 新图片
        /// </summary>
        event EventHandler<Bitmap> NewImageEvent;

    }
    /// <summary>
    /// 相机列表信息
    /// </summary>
    public class CameraItemInfo
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 序列号
        /// </summary>
        public string SN { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 相机类型
        /// </summary>
        public CameraConnectTypes CameraType { get; set; }
    }
    /// <summary>
    /// 相机类型
    /// </summary>
    public enum CameraConnectTypes
    {
        /// <summary>
        /// GigE
        /// </summary>
        GigE,
        /// <summary>
        /// Usb
        /// </summary>
        Usb,
        /// <summary>
        /// 采集卡
        /// </summary>
        CameraLink,
        /// <summary>
        /// IEEE1394
        /// </summary>
        IEEE1394
    }
    /// <summary>
    /// 相机类型
    /// </summary>
    public enum CameraTypes
    {
        /// <summary>
        /// 未知
        /// </summary>
        UnKnown,
        /// <summary>
        /// 线扫
        /// </summary>
        Linear,
        /// <summary>
        /// 面阵
        /// </summary>
        Area,
    }
    /// <summary>
    /// 触发模式
    /// </summary>
    public enum TriggerMode
    {
        /// <summary>
        /// 关闭
        /// </summary>
        OFF = 0,
        /// <summary>
        /// 打开
        /// </summary>
        ON = 1
    }
    /// <summary>
    /// 自动模式
    /// </summary>
    public enum AutoMode
    {
        /// <summary>
        /// 关闭
        /// </summary>
        OFF,
        /// <summary>
        /// 一次
        /// </summary>
        Once,
        /// <summary>
        /// 连续
        /// </summary>
        Continous
    }
    /// <summary>
    /// 触发源
    /// </summary>
    public enum TriggerSources
    {
        /// <summary>
        /// 线0
        /// </summary>
        Line0 = 0,
        /// <summary>
        /// 线1
        /// </summary>
        Line1 = 1,
        /// <summary>
        /// 线2
        /// </summary>
        Line2 = 2,
        /// <summary>
        /// 线3
        /// </summary>
        Line3 = 3,
        /// <summary>
        /// 线4
        /// </summary>
        Counter = 4,
        /// <summary>
        /// 软触发
        /// </summary>
        Soft = 7
    }
    /// <summary>
    /// 采集卡类型
    /// </summary>
    public enum BoardInfoClasss
    {
        CameraLink,
        GigEVisionBoard
    }
}
