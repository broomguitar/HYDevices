using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Profinet.Melsec;
using System;

namespace HY.Devices.PLC
{
    /// <summary>
    /// 三菱的PLC
    /// </summary>
    public class HYPLCComm_MelsecNet : AbstractHYPLCComm_Net
    {
        public override IReadWriteNet PLCNetworkReaderWriter => _plc_Melsec;

        private MelsecMcNet _plc_Melsec;
        /// <summary>
        /// 三菱网络PLC构造函数
        /// </summary>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="port">端口</param>
        /// <param name="connectTimeOut">设置连接超时时长，默认5000， 单位毫秒</param>
        public HYPLCComm_MelsecNet(string ipAddress, int port, int connectTimeOut = 5000) : this()
        {
            IP = ipAddress;
            Port = port;
            ConnectTimeOut = connectTimeOut;
        }
        public HYPLCComm_MelsecNet()
        {
            _plc_Melsec = new MelsecMcNet();
        }
        public override bool Open()
        {
            try
            {
                _plc_Melsec.IpAddress = IP;
                _plc_Melsec.Port = Port;
                _plc_Melsec.ConnectTimeOut = ConnectTimeOut;
                OperateResult Ret = _plc_Melsec.ConnectServer();
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
                OperateResult Ret = _plc_Melsec.ConnectClose();
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
            _plc_Melsec.Dispose();
        }
    }
}