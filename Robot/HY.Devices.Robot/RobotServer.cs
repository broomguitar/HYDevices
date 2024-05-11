using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Robot
{
    public class RobotServer
    {
        public IPAddress IP_Server { get; set; }
        public int Port_Server { get; set; }

        public event EventHandler<string> PushResultEvent;
        private TcpListener _tcpServer;
        private bool isRunning = false;
        public bool Connect()
        {
            try
            {
                if (_tcpServer == null)
                {
                    InitialModule();
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool DisConnect()
        {
            if (_tcpServer != null)
            {
                try
                {
                    _tcpServer.Stop();
                    foreach (var item in tcpClients)
                    {
                        item.Close();
                        //item.Dispose();
                    }
                    tcpClients.Clear();
                    isRunning = false;
                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
            return true;
        }
        public event EventHandler ClientOfflineEvent;
        private readonly List<TcpClient> tcpClients = new List<TcpClient>();
        private Queue<TcpClient> removeQueue = new Queue<TcpClient>();
        private Task _monitorTask;
        private void InitialModule()
        {
            try
            {
                _tcpServer = new TcpListener(IP_Server, Port_Server);
                if (!isRunning)
                {
                    if (_tcpServer != null)
                    {
                        isRunning = true;
                        if (_monitorTask == null || _monitorTask.IsCompleted)
                        {
                            _monitorTask = Task.Factory.StartNew(
                                () =>
                                {
                                    while (isRunning)
                                    {
                                        System.Threading.Thread.Sleep(2000);
                                        checkClientsStatus();
                                    }
                                }, TaskCreationOptions.LongRunning);
                        }
                        _tcpServer.Start();
                        _tcpServer.BeginAcceptTcpClient(new AsyncCallback(HasNewClient), _tcpServer);
                    }
                    else
                    {
                        isRunning = false;
                    }
                }
            }
            catch (Exception ex)
            {
                _tcpServer = null;
            }
        }
        private void HasNewClient(IAsyncResult asyncResult)
        {
            try
            {
                TcpListener listener = (TcpListener)asyncResult.AsyncState;
                TcpClient tcpClient = listener.EndAcceptTcpClient(asyncResult);
                PushResultEvent?.Invoke(this, $"新的客户端：{((IPEndPoint)tcpClient.Client.RemoteEndPoint).Address}:{((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port}");
                tcpClient.Client.IOControl(IOControlCode.KeepAliveValues, GetKeepAliveData(), null);
                tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                tcpClients.Add(tcpClient);
                NetworkStream networkStream = tcpClient.GetStream();
                byte[] buffRece = new byte[tcpClient.ReceiveBufferSize];
                networkStream.BeginRead(buffRece, 0, buffRece.Length, HasNewMsg, new KeyValuePair<byte[], NetworkStream>(buffRece, networkStream));
                _tcpServer.BeginAcceptTcpClient(new AsyncCallback(HasNewClient), _tcpServer);
            }
            catch (Exception ex)
            {
            }
        }
        private void HasNewMsg(IAsyncResult asyncResult)
        {
            try
            {
                KeyValuePair<byte[], NetworkStream> keyValue = (KeyValuePair<byte[], NetworkStream>)asyncResult.AsyncState;
                byte[] buffRece = keyValue.Key;
                NetworkStream networkStream = keyValue.Value;
                int recv = 0;
                try
                {
                    recv = networkStream.EndRead(asyncResult);
                }
                catch
                {
                }
                if (recv > 0)
                {
                    string message = Encoding.UTF8.GetString(buffRece, 0, recv);
                    PushResultEvent?.BeginInvoke(this, message, null, null);
                    networkStream.BeginRead(buffRece, 0, buffRece.Length, HasNewMsg, keyValue);
                }
                else
                {
                    PushResultEvent?.BeginInvoke(this, "客户端断开", null, null);
                }
            }
            catch (Exception ex)
            {
            }
            System.Threading.Thread.Sleep(10);
        }
        private void checkClientsStatus()
        {
            try
            {
                lock (tcpClients)
                {
                    foreach (var item in tcpClients)

                    {
                        //if (item.Client.Client.Poll(500, System.Net.Sockets.SelectMode.SelectRead) && (item.Client.Client.Available == 0))
                        if (!isClientConnected(item))
                        {
                            removeQueue.Enqueue(item);
                            ClientOfflineEvent?.Invoke(this, null);
                            continue;
                        }
                    }
                    while (removeQueue.Count > 0)
                    {
                        TcpClient item = removeQueue.Dequeue();
                        tcpClients.Remove(item);
                        try
                        {
                            item.Close();
                        }
                        catch (Exception ex)
                        {
                            PushResultEvent?.Invoke(this, "关闭客户端连接" + ex.ToString());
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
        }
        private bool isClientConnected(TcpClient ClientSocket)
        {
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();

            TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections();

            foreach (TcpConnectionInformation c in tcpConnections)
            {
                TcpState stateOfConnection = c.State;

                if (c.LocalEndPoint.Equals(ClientSocket.Client.LocalEndPoint) && c.RemoteEndPoint.Equals(ClientSocket.Client.RemoteEndPoint))
                {
                    if (stateOfConnection == TcpState.Established)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

            }

            return false;
        }

        private byte[] GetKeepAliveData()
        {
            uint dummy = 0;
            byte[] inOptionValues = new byte[Marshal.SizeOf(dummy) * 3];
            BitConverter.GetBytes((uint)1).CopyTo(inOptionValues, 0);
            BitConverter.GetBytes((uint)3000).CopyTo(inOptionValues, Marshal.SizeOf(dummy));
            BitConverter.GetBytes((uint)500).CopyTo(inOptionValues, Marshal.SizeOf(dummy) * 2);
            return inOptionValues;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool WriteData(string msg)
        {
            if (msg == null) return false;
            try
            {
                if (tcpClients.Count < 1) return false;
                foreach (var item in tcpClients)
                {
                    NetworkStream network = item.GetStream();
                    byte[] buffer = Encoding.UTF8.GetBytes(msg.ToString());
                    network.Write(buffer, 0, buffer.Length);
                    PushResultEvent?.Invoke(this, $"发送给--{((IPEndPoint)item.Client.RemoteEndPoint).Address}:{((IPEndPoint)item.Client.RemoteEndPoint).Port}指令【{msg}】");
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
