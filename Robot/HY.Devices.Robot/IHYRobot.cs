using System;

namespace HY.Devices.Robot
{
    /// <summary>
    /// 机械臂接口
    /// </summary>
    public interface IHYRobot : IDisposable
    {
        bool IsConnected { get; }
        void Connect();
        void Close();
        void PostDatas();
        void isFreeze();
    }
}
