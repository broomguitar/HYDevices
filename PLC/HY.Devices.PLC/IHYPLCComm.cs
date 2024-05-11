using System;

namespace HY.Devices.PLC
{
    /// <summary>
    /// PLC通讯模块
    /// </summary>
    public interface IHYPLCComm : IDisposable
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 打开PLC
        /// </summary>
        /// <returns></returns>
        bool Open();
        /// <summary>
        /// 关闭PLC
        /// </summary>
        /// <returns></returns>
        bool Close();
        /// <summary>
        /// 读取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">地址</param>
        /// <returns></returns>
        T Read<T>(string address);
        /// <summary>
        /// 批量读取数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">初始地址</param>
        /// <param name="length">数据长度</param>
        /// <returns></returns>
        T[] Read<T>(string address, ushort length);
        /// <summary>
        /// 写入数据
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="address">地址</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        bool Write<T>(string address, T data);
        /// <summary>
        ///监测上升沿，有信号就触发回调
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="milliseconds">轮询间隔 毫秒</param>
        /// <param name="callback">回调方法 带着地址值</param>
        /// <param name="isSimulation">是否模拟，默认为false</param>
        /// <returns></returns>
        void WaitRisingEdge(string address, int milliseconds, Action<string> callback, bool isSimulation = false);
        /// <summary>
        /// 监测下降沿，有信号就触发回调
        /// </summary>
        /// <param name="address">地址</param>
        /// <param name="milliseconds">轮询间隔 毫秒</param>
        /// <param name="callback">回调方法 带着地址值</param>
        /// <param name="isSimulation">是否模拟，默认为false</param>
        /// <returns></returns>
        void WaitFallingEdge(string address, int milliseconds, Action<string> callback, bool isSimulation = false);
        /// <summary>
        /// 停止检测地址信号
        /// </summary>
        /// <param name="address">地址值</param>
        void StopWaitAddress(string address);

    }
}
