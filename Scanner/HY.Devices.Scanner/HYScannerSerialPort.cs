using System;
using System.IO.Ports;

namespace HY.Devices.Scanner
{
    /// <summary>
    /// 串口连扫码枪
    /// </summary>
    public class HYScannerSerialPort : AbstractHYScanner
    {
        /// <summary>
        /// 串口扫码枪构造函数
        /// </summary>
        /// <param name="portName">串口名字</param>
        public HYScannerSerialPort(string portName)
        {
            SerialPortName = portName;
            ScannerReader = new SerialPort();
        }
        /// <summary>
        /// 串口扫码枪构造函数
        /// </summary>
        /// <param name="portName">串口名字</param>
        /// <param name="baudRate">波特率</param>
        public HYScannerSerialPort(string portName, int baudRate) : this(portName)
        {
            SerailPortBaudRate = baudRate;
        }
        /// <summary>
        /// 串口名字
        /// </summary>
        public string SerialPortName { get; set; }
        /// <summary>
        /// 波特率
        /// </summary>
        public int SerailPortBaudRate { get; set; } = 9600;
        /// <summary>
        /// 扫码枪阅读器
        /// </summary>
        public SerialPort ScannerReader { get; private set; } = null;
        /// <summary>
        /// 扫描枪状态
        /// </summary>
        public override bool IsConnected => ScannerReader?.IsOpen == true;
        /// <summary>
        /// 连接扫码枪
        /// </summary>
        public override void Open( bool isFilter)
        {
            try
            {
                if (IsConnected)
                {
                    ScannerReader.Close();
                    ScannerReader.Dispose();
                }
                _isFilter = isFilter;
                ScannerReader.PortName = SerialPortName;
                ScannerReader.BaudRate = SerailPortBaudRate;
                ScannerReader.DataReceived += ScannerReader_DataReceived;
                ScannerReader.Open();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.ToString();
            }
        }
        /// <summary>
        /// 关闭扫码枪
        /// </summary>
        public override void Close()
        {
            try
            {
                ScannerReader?.Close();
                ScannerReader?.Dispose();
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.ToString();
            }
        }
        /// <summary>
        /// 收到数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ScannerReader_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                System.Threading.Thread.Sleep(100);
                int _byteToEnd = ScannerReader.BytesToRead;
                byte[] receiveData = new byte[_byteToEnd];
                int ret = ScannerReader.Read(receiveData, 0, _byteToEnd);
                string str_Buff = System.Text.Encoding.ASCII.GetString(receiveData);
                // string str_Buff = ScannerReader.ReadLine();
                if (_isFilter)
                {
                    str_Buff = System.Text.RegularExpressions.Regex.Replace(str_Buff, @"[^0-9a-zA-Z]+", "");
                }
                SetNewData(str_Buff);
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.ToString();
            }

        }
    }
}
