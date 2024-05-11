using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Profinet.Siemens;
using System;

namespace HY.Devices.PLC
{
    /// <summary>
    /// 西门子系列PLC
    /// </summary>
    public class HYPLCComm_SiemensNet : AbstractHYPLCComm_Net
    {
        public override IReadWriteNet PLCNetworkReaderWriter => _plc_SiemensS7Net;

        private SiemensS7Net _plc_SiemensS7Net;
        /// <summary>
        /// 西门子PLC 构造函数
        /// </summary>
        /// <param name="type"> 西门子PLC类型</param>
        /// <param name="ipAddress">IP地址</param>
        /// <param name="connectTimeOut">连接超时时间 默认值5000 单位毫秒</param>
        public HYPLCComm_SiemensNet(SiemensPLCTypes type, string ipAddress, int connectTimeOut = 5000) : this(type)
        {
            IP = ipAddress;
            ConnectTimeOut = connectTimeOut;
        }
        /// <summary>
        /// 西门子PLC 构造函数
        /// </summary>
        /// <param name="type"> 西门子PLC类型</param>
        public HYPLCComm_SiemensNet(SiemensPLCTypes type = SiemensPLCTypes.S1200)
        {
            _plc_SiemensS7Net = new SiemensS7Net((SiemensPLCS)type);
        }
        public enum SiemensPLCTypes
        {
            //
            // 摘要:
            //     1200系列
            S1200 = 1,
            //
            // 摘要:
            //     300系列
            S300,
            //
            // 摘要:
            //     400系列
            S400,
            //
            // 摘要:
            //     1500系列PLC
            S1500,
            //
            // 摘要:
            //     200的smart系列
            S200Smart,
            //
            // 摘要:
            //     200系统，需要额外配置以太网模块
            S200
        }
        public override bool Open()
        {
            try
            {
                _plc_SiemensS7Net.IpAddress = IP;
                _plc_SiemensS7Net.ConnectTimeOut = ConnectTimeOut;
                OperateResult Ret = _plc_SiemensS7Net.ConnectServer
                    ();
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
                OperateResult Ret = _plc_SiemensS7Net.ConnectClose();
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
            _plc_SiemensS7Net.Dispose();
        }
    }
}
