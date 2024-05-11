using System;
namespace HY.Devices.Printer
{
    /// <summary>
    /// 泛越打印机USB连接
    /// </summary>
    public class HYPrinter_FanuuUSB : AbstractHYPrinter
    {
        public override HYConnectTypes ConnectType => HYConnectTypes.USB;
        public override HYPrinterStatus GetPrinterStatus()
        {
            try
            {
                var state = TSCLIB_Util.usbportqueryprinter();
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
                int ret = TSCLIB_Util.openport("USB");
                string states = TSCLIB_Util.sendcommand_getstring("OUT GETSENSOR(\"PEEL\")", 300).ToString().Replace("\r\n", "");
                TSCLIB_Util.sendcommand("SET PEEL ON");//不行就删
                TSCLIB_Util.closeport();
                return states == "1";
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public HYPrinter_FanuuUSB(string printerName) : base(printerName) { }
    }
}
