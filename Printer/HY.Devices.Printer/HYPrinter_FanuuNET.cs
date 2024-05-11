using System;

namespace HY.Devices.Printer
{
    /// <summary>
    /// 泛越打印机网络连接
    /// </summary>
    public class HYPrinter_FanuuNET : AbstractHYPrinter
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
                    //int r = TSCLIB_Util.closeethernetport();
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
            try
            {
                var state = TSCLIB_Util.ethernetportqueryprinter();
                return (HYPrinterStatus)state;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override bool IsHasPaper()
        {
            if (!IsPeel) return false;
            try
            {
                int ret = TSCLIB_Util.openethernet(IpAddress, Port);
                string states = TSCLIB_Util.sendcommand_getstring("OUT GETSENSOR(\"PEEL\")", 300).ToString().Replace("\r\n", "");
                TSCLIB_Util.sendcommand("SET PEEL ON");//不行就删
                //int r = TSCLIB_Util.closeethernetport();
                return states == "1";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public HYPrinter_FanuuNET(string printerName, string ip, int port = 9100) : base(printerName)
        {
            IpAddress = ip;
            Port = port;
        }
    }
}
