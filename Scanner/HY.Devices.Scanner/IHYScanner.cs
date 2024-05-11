using System;

namespace HY.Devices.Scanner
{
    /// <summary>
    /// 扫码枪
    /// </summary>
    public interface IHYScanner : IDisposable
    {
        /// <summary>
        /// 扫码枪状态
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 连接扫码枪,是否过滤(只接受字母与数字)
        /// </summary>
        void Open(bool isFilter=false);
        /// <summary>
        /// 关闭扫码枪
        /// </summary>
        void Close();
        /// <summary>
        /// 获取错误信息
        /// </summary>
        string GetLastError();
        /// <summary>
        /// 接受到新数据事件
        /// </summary>
        event EventHandler<string> HasNewDataReceived;
        /// <summary>
        /// 错误事件
        /// </summary>
        event EventHandler<string> Error;
    }
}
