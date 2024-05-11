using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Profinet.Panasonic;
using System;

namespace HY.Devices.PLC
{
    /// <summary>
    /// 松下PLC
    /// </summary>
    public class HYPLCComm_PanasonicNet : AbstractHYPLCComm_Net
    {
        public override IReadWriteNet PLCNetworkReaderWriter => _plc_PanasonicMcNet;
        private PanasonicMcNet _plc_PanasonicMcNet;
        /// <summary>
        /// 松下PLC构造函数
        /// </summary>
        /// <param name="ipAddress">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="connectTimeOut">连接超时时间 默认值5000 单位毫秒</param>
        public HYPLCComm_PanasonicNet(string ipAddress, int port, int connectTimeOut = 5000) : this()
        {
            IP = ipAddress;
            Port = port;
            ConnectTimeOut = connectTimeOut;
        }
        public HYPLCComm_PanasonicNet()
        {
            _plc_PanasonicMcNet = new PanasonicMcNet();
        }
        public override bool Open()
        {
            try
            {
                _plc_PanasonicMcNet.IpAddress = IP;
                _plc_PanasonicMcNet.Port = Port;
                _plc_PanasonicMcNet.ConnectTimeOut = ConnectTimeOut;
                OperateResult Ret = _plc_PanasonicMcNet.ConnectServer();
                _isConnected = Ret.IsSuccess;
                if (_isConnected)
                {
                    return true;
                }
                else
                {
                    throw new Exception(Ret.Message);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public override bool Close()
        {
            try
            {
                _risingEdgeAddressDic.Clear();
                _fallingEdgeAddressDic.Clear();
                OperateResult Ret = _plc_PanasonicMcNet.ConnectClose();
                _isConnected = !Ret.IsSuccess;
                if (!_isConnected)
                {
                    return true;
                }
                else
                {
                    throw new Exception(Ret.Message);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public override void Dispose()
        {
            Close();
            _plc_PanasonicMcNet.Dispose();
        }
    }
}
