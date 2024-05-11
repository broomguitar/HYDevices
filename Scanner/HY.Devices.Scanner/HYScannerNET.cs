using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace HY.Devices.Scanner
{
    /// <summary>
    /// 网连扫码枪
    /// </summary>
    public class HYScannerNET : AbstractHYScanner
    {
        /// <summary>
        /// 网连扫码枪构造函数
        /// </summary>
        /// <param name="ip">IP</param>
        /// <param name="port">端口</param>
        public HYScannerNET(string ip, int port)
        {
            IP = ip;
            Port = port;
            _ping = new Ping();
        }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 扫码枪读取器
        /// </summary>
        public Socket ScannerReader { get; private set; } = null;
        private Ping _ping;
        /// <summary>
        /// 扫描枪状态
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                try
                {
                    if (ScannerReader?.Connected != true) return false;
                    if (_ping.Send(IP).Status == IPStatus.Success)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {

                    return false;
                }
            }
        }
        private byte[] m_buffRece = new byte[1024];
        /// <summary>
        /// 连接扫码枪
        /// </summary>
        public override void Open(bool isFilter)
        {
            try
            {
                if (IsConnected)
                {
                    Close();
                }
                _isFilter = isFilter;
                ScannerReader = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(IP);
                ScannerReader.Connect(new IPEndPoint(ip, Port));
                if (ScannerReader.Connected)
                {
                    m_buffRece = new byte[1024];
                    ScannerReader.BeginReceive(m_buffRece, 0, m_buffRece.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), ScannerReader);
                }
                else
                {
                    ScannerReader = null;
                }
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
                ScannerReader = null;
            }
            catch (Exception ex)
            {

                LastErrorMsg = ex.ToString();
            }
        }
        /// <summary>
        /// 一旦服务器发送信息，则会触发回调方法
        /// </summary>
        /// <param name="result"></param>
        private void ReceiveCallBack(IAsyncResult result)
        {
            try
            {
                Socket socket = result.AsyncState as Socket;
                if(socket?.Connected!=true)
                {
                    return;
                }
                //读取从服务器端传来的数据，EndReceive是关闭异步接收方法，同时读取数据
                int count = socket.EndReceive(result);
                if (count > 0)
                {
                    try
                    {
                        string msg = System.Text.Encoding.UTF8.GetString(m_buffRece, 0, count);
                        if (_isFilter)
                        {
                            msg = System.Text.RegularExpressions.Regex.Replace(msg, @"[^0-9a-zA-Z]+", "");
                        }
                        //接受完服务端的数据后的逻辑
                        SetNewData(msg);
                    }
                    catch (Exception ex)
                    {
                        LastErrorMsg = ex.ToString();
                    }
                }
                m_buffRece = new byte[1024];
                // 递归监听服务器端是否发来信息，一旦服务器再次发送信息，客户端仍然可以接收到
                socket.BeginReceive(m_buffRece, 0, m_buffRece.Length, SocketFlags.None, ReceiveCallBack, socket);
            }
            catch (Exception ex)
            {
                LastErrorMsg = ex.ToString();
            }
        }
    }
}
