using System;
using System.Management;
using System.Runtime.InteropServices;

namespace HY.Devices.Printer
{
    class HYPrinterStatusUtil
    {
        public static bool CheckPrinter(string printName)
        {
            try
            {
                ManagementScope scope = new ManagementScope(@"\root\cimv2");
                scope.Connect();
                // Select Printers from WMI Object Collections
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
                string printerName = "";
                foreach (ManagementObject printer in searcher.Get())
                {
                    printerName = printer["Name"].ToString().ToLower();
                    if (printerName.ToLower() == printName.ToLower())
                    {
                        foreach (var property in printer.Properties)
                        {
                            System.Diagnostics.Debug.WriteLine(property.Name + ":" + property.Value);
                        }
                        if (printer["WorkOffline"].ToString().ToLower().Equals("true"))
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }
    }
    class PrinterHelper
    {


        [DllImport("winspool.drv", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool OpenPrinter(string pPrinterName, out IntPtr hPrinter, IntPtr pDefault);


        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool ClosePrinter(IntPtr hPrinter);


        [DllImport("winspool.drv", SetLastError = true)]
        private static extern bool GetPrinter(IntPtr hPrinter,
            int dwLevel, IntPtr pPrinter, int cbBuf, out int pcbNeeded);


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct PRINTER_INFO_2
        {
            public string pServerName;
            public string pPrinterName;
            public string pShareName;
            public string pPortName;
            public string pDriverName;
            public string pComment;
            public string pLocation;
            public IntPtr pDevMode;
            public string pSepFile;
            public string pPrintProcessor;
            public string pDatatype;
            public string pParameters;
            public IntPtr pSecurityDescriptor;
            public uint Attributes;
            public uint Priority;
            public uint DefaultPriority;
            public uint StartTime;
            public uint UntilTime;
            public uint Status;
            public uint cJobs;
            public uint AveragePPM;
        }




        public static string GetPrinterStatus(string PrinterName)
        {
            int intValue = GetPrinterStatusInt(PrinterName);
            string strRet = string.Empty;
            switch (intValue)
            {
                case 0:
                    strRet = "准备就绪（Ready）";
                    break;
                case 0x00000200:
                    strRet = "忙(Busy）";
                    break;
                case 0x00400000:
                    strRet = "被打开（Printer Door Open）";
                    break;
                case 0x00000002:
                    strRet = "错误(Printer Error）";
                    break;
                case 0x0008000:
                    strRet = "初始化(Initializing）";
                    break;
                case 0x00000100:
                    strRet = "正在输入,输出（I/O Active）";
                    break;
                case 0x00000020:
                    strRet = "手工送纸（Manual Feed）";
                    break;
                case 0x00040000:
                    strRet = "无墨粉（No Toner）";
                    break;
                case 0x00001000:
                    strRet = "不可用（Not Available）";
                    break;
                case 0x00000080:
                    strRet = "脱机（Off Line）";
                    break;
                case 0x00200000:
                    strRet = "内存溢出（Out of Memory）";
                    break;
                case 0x00000800:
                    strRet = "输出口已满（Output Bin Full）";
                    break;
                case 0x00080000:
                    strRet = "当前页无法打印（Page Punt）";
                    break;
                case 0x00000008:
                    strRet = "塞纸（Paper Jam）";
                    break;
                case 0x00000010:
                    strRet = "打印纸用完（Paper Out）";
                    break;
                case 0x00000040:
                    strRet = "纸张问题（Page Problem）";
                    break;
                case 0x00000001:
                    strRet = "暂停（Paused）";
                    break;
                case 0x00000004:
                    strRet = "正在删除（Pending Deletion）";
                    break;
                case 0x00000400:
                    strRet = "正在打印（Printing）";
                    break;
                case 0x00004000:
                    strRet = "正在处理（Processing）";
                    break;
                case 0x00020000:
                    strRet = "墨粉不足（Toner Low）";
                    break;
                case 0x00100000:
                    strRet = "需要用户干预（User Intervention）";
                    break;
                case 0x20000000:
                    strRet = "等待（Waiting）";
                    break;
                case 0x00010000:
                    strRet = "热机中（Warming Up）";
                    break;
                default:
                    strRet = "未知状态（Unknown Status）";
                    break;
            }
            return strRet;
        }


        internal static int GetPrinterStatusInt(string PrinterName)
        {
            int intRet = 0;
            IntPtr hPrinter;


            if (OpenPrinter(PrinterName, out hPrinter, IntPtr.Zero))
            {
                int cbNeeded = 0;
                bool bolRet = GetPrinter(hPrinter, 2, IntPtr.Zero, 0, out cbNeeded);
                if (cbNeeded > 0)
                {
                    IntPtr pAddr = Marshal.AllocHGlobal((int)cbNeeded);
                    bolRet = GetPrinter(hPrinter, 2, pAddr, cbNeeded, out cbNeeded);
                    if (bolRet)
                    {
                        PRINTER_INFO_2 Info2 = new PRINTER_INFO_2();


                        Info2 = (PRINTER_INFO_2)Marshal.PtrToStructure(pAddr, typeof(PRINTER_INFO_2));


                        intRet = System.Convert.ToInt32(Info2.Status);
                    }
                    Marshal.FreeHGlobal(pAddr);
                }
                ClosePrinter(hPrinter);
            }


            return intRet;
        }




    }

    /// <summary>
    /// 对接打印机DLL
    /// </summary>
    class TSCLIB_Util
    {
        private const string _TSCLIBPath = "TSCLIB.dll";
        [DllImport(_TSCLIBPath, EntryPoint = "about")]
        public static extern int about();

        [DllImport(_TSCLIBPath, EntryPoint = "openport")]
        public static extern int openport(string printername);
        [DllImport(_TSCLIBPath, EntryPoint = "openethernet")]
        public static extern int openethernet(string ip, int port);

        [DllImport(_TSCLIBPath, EntryPoint = "barcode")]
        public static extern int barcode(string x, string y, string type,
                    string height, string readable, string rotation,
                    string narrow, string wide, string code);

        [DllImport(_TSCLIBPath, EntryPoint = "clearbuffer")]
        public static extern int clearbuffer();

        [DllImport(_TSCLIBPath, EntryPoint = "closeport")]
        public static extern int closeport();

        [DllImport(_TSCLIBPath, EntryPoint = "downloadpcx")]
        public static extern int downloadpcx(string filename, string image_name);

        [DllImport(_TSCLIBPath, EntryPoint = "formfeed")]
        public static extern int formfeed();

        [DllImport(_TSCLIBPath, EntryPoint = "nobackfeed")]
        public static extern int nobackfeed();

        [DllImport(_TSCLIBPath, EntryPoint = "printerfont")]
        public static extern int printerfont(string x, string y, string fonttype,
                        string rotation, string xmul, string ymul,
                        string text);

        [DllImport(_TSCLIBPath, EntryPoint = "printlabel")]
        public static extern int printlabel(string set, string copy);

        [DllImport(_TSCLIBPath, EntryPoint = "sendcommand")]
        public static extern int sendcommand(string printercommand);
        [DllImport(_TSCLIBPath, EntryPoint = "sendBinaryData")]
        public static extern int sendBinaryData(byte[] printercommand, int CommandLength);

        [DllImport(_TSCLIBPath, EntryPoint = "setup")]
        public static extern int setup(string width, string height,
                  string speed, string density,
                  string sensor, string vertical,
                  string offset);

        [DllImport(_TSCLIBPath, EntryPoint = "windowsfont")]
        public static extern int windowsfont(int x, int y, int fontheight,
                        int rotation, int fontstyle, int fontunderline,
                        string szFaceName, string content);
        [DllImport(_TSCLIBPath, EntryPoint = "windowsfontUnicode")]
        public static extern int windowsfontUnicode(int x, int y, int fontheight,
                        int rotation, int fontstyle, int fontunderline,
                        string szFaceName, byte[] content);
        [DllImport(_TSCLIBPath, EntryPoint = "ethernetprinterstatus")]
        public static extern string ethernetprinterstatus();
        [DllImport(_TSCLIBPath, EntryPoint = "closeethernetport")]
        public static extern int closeethernetport();
        [DllImport(_TSCLIBPath, EntryPoint = "usbprinterquantity")]
        public static extern int usbprinterquantity();
        [DllImport(_TSCLIBPath, EntryPoint = "usbprintermileage")]
        public static extern string usbprintermileage();
        [DllImport(_TSCLIBPath, EntryPoint = "sendcommand_getstring")]
        public static extern string sendcommand_getstring(string Cmd, int Delay);
        [DllImport(_TSCLIBPath, EntryPoint = "usbprintercompletestatus")]
        public static extern string usbprintercompletestatus();
        [DllImport("TSCLIB.dll", EntryPoint = "usbportqueryprinter")]
        public static extern int usbportqueryprinter();
        [DllImport("TSCLIB.dll", EntryPoint = "ethernetportqueryprinter")]
        public static extern int ethernetportqueryprinter();
    }
}
