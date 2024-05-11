namespace HY.Devices.Printer
{
    /// <summary>
    /// 斑马打印机 USB连接
    /// </summary>
    public class HYPrinter_ZebraUSB : AbstractHYPrinter
    {
        public override HYConnectTypes ConnectType => HYConnectTypes.USB;

        public override HYPrinterStatus GetPrinterStatus()
        {
            return HYPrinterStatus.待机;
        }

        public override bool IsHasPaper()
        {
            if (!IsPeel) return false;
            return false;
        }
        public HYPrinter_ZebraUSB(string printerName) : base(printerName) { }
    }
}
