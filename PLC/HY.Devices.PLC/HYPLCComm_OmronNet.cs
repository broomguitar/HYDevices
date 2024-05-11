using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Profinet.Omron;
using System;

namespace HY.Devices.PLC
{
    /// <summary>
    /// 欧姆龙PLC
    /// </summary>
    public class HYPLCComm_OmronNet : AbstractHYPLCComm_Net
    {
        public override IReadWriteNet PLCNetworkReaderWriter => _plc_OmronFinsNet;
        private OmronFinsNet _plc_OmronFinsNet;
        /// <summary>
        /// 欧姆龙PLC构造函数
        /// </summary>
        /// <param name="ipAddress">ip地址</param>
        /// <param name="port">端口</param>
        /// <param name="connectTimeOut">设置连接超时时长，默认5000， 单位毫秒</param>
        public HYPLCComm_OmronNet(string ipAddress, int port, int connectTimeOut = 5000) : this()
        {
            IP = ipAddress;
            Port = port;
            ConnectTimeOut = connectTimeOut;
        }
        public HYPLCComm_OmronNet()
        {
            _plc_OmronFinsNet = new OmronFinsNet();
        }
        public override bool Open()
        {
            try
            {
                _plc_OmronFinsNet.IpAddress = IP;
                _plc_OmronFinsNet.Port = Port;
                _plc_OmronFinsNet.ConnectTimeOut = ConnectTimeOut;
                OperateResult Ret = _plc_OmronFinsNet.ConnectServer();
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
                OperateResult Ret = _plc_OmronFinsNet.ConnectClose();
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
            _plc_OmronFinsNet.Dispose();
        }
    }
}
