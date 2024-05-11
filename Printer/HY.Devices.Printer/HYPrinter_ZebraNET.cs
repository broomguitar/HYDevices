using System;

namespace HY.Devices.Printer
{
    /// <summary>
    /// 斑马打印机 网络连接
    /// </summary>
    public class HYPrinter_ZebraNET : AbstractHYPrinter
    {
        /// <summary>
        /// 打印机IP
        /// </summary>
        public string IpAddress { get; }
        /// <summary>
        /// 打印机端口
        /// </summary>
        public int Port { get; } = 9100;
        public override bool IsConnected
        {
            get
            {
                try
                {
                    int ret = TSCLIB_Util.openethernet(IpAddress, Port);
                    int r = TSCLIB_Util.closeport();
                    return ret > 0;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public override HYConnectTypes ConnectType => HYConnectTypes.ETHERNET;

        public override HYPrinterStatus GetPrinterStatus()
        {
            return HYPrinterStatus.待机;
        }

        public override bool IsHasPaper()
        {
            if (!IsPeel) return false;
            return false;
        }
        public HYPrinter_ZebraNET(string printerName, string ip, int port = 9100) : base(printerName)
        {
            IpAddress = ip;
            Port = port;
        }
    }
}
