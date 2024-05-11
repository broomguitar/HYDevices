using System;

namespace HY.Devices.Scanner
{
    /// <summary>
    /// 扫码枪基类
    /// </summary>
    public abstract class AbstractHYScanner : IHYScanner
    {
        protected bool _isFilter = false;
        private string _lastErrorMsg;
        /// <summary>
        /// 错误信息
        /// </summary>
        protected string LastErrorMsg
        {
            get
            {
                return _lastErrorMsg;
            }
            set
            {
                _lastErrorMsg = value;
                Error?.Invoke(this, _lastErrorMsg);
            }
        }
        /// <summary>
        /// 扫码枪状态
        /// </summary>
        public abstract bool IsConnected { get; }
        /// <summary>
        /// 收到新数据事件
        /// </summary>
        public event EventHandler<string> HasNewDataReceived;
        /// <summary>
        /// 错误异常事件
        /// </summary>
        public event EventHandler<string> Error;

        /// <summary>
        /// 连接扫码枪
        /// </summary>
        public abstract void Open(bool isFilter);
        /// <summary>
        /// 关闭扫码枪
        /// </summary>
        public abstract void Close();
        /// <summary>
        /// 资源释放
        /// </summary>
        public virtual void Dispose()
        {
            Close();
        }
        /// <summary>
        /// 获取最后错误信息
        /// </summary>
        /// <returns></returns>
        public string GetLastError()
        {
            return _lastErrorMsg;
        }
        /// <summary>
        /// 赋值最新数据
        /// </summary>
        /// <param name="msg"></param>
        protected void SetNewData(string msg)
        {
            HasNewDataReceived?.Invoke(this, msg);
        }
    }
}
