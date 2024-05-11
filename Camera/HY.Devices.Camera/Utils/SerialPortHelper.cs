using System;
using System.Diagnostics;
using System.IO.Ports;

namespace HY.Devices.Camera.Utils
{
    public class SerialPortHelper
    {
        private SerialPort m_port = new SerialPort();
        private bool m_bOpenPort = false;
        private object m_locker = new object();                       //创建锁
        Stopwatch m_watch = new Stopwatch();                 // 计时器

        /* @brief 打开串口
        *  @param[in] portNo: 串口号
        *  @param[in] baud: 波特率
        *  @return: 是否打开
        */
        public bool OpenPort(string portName, int bautRate)
        {
            if (IsOpen())
                return true;

            m_bOpenPort = false;

            System.Threading.Monitor.Enter(m_locker);
            // 设置串口信息
            m_port.PortName = portName;
            m_port.BaudRate = bautRate;
            m_port.StopBits = StopBits.One;
            m_port.DataBits = 8;
            m_port.Parity = Parity.None;

            m_port.ReadTimeout = 1000;          //  设置超时读取时间
            m_port.WriteTimeout = 1000;        //  超时写入时间

            try
            {
                m_port.Open();
                m_bOpenPort = true;
            }
            catch (Exception)
            {
                m_bOpenPort = false;
            }
            System.Threading.Monitor.Exit(m_locker);

            return m_bOpenPort;
        }

        /* @brief 是否打开串口
        *  @return: 是否打开
        */
        public bool IsOpen()
        {
            return m_bOpenPort;
        }

        /* @brief 是否打开串口
        *  @return: 是否打开
        */
        public bool ClosePort()
        {
            if (!IsOpen())
                return true;

            System.Threading.Monitor.Enter(m_locker);
            try
            {
                m_port.Close();
                m_bOpenPort = false;
            }
            catch (Exception)
            {
                m_bOpenPort = true;
            }
            System.Threading.Monitor.Exit(m_locker);

            return m_bOpenPort;
        }

        /* @brief 写入数据到串口(写入后根据读取值判断是否设置成功)
        *  @param[in] strData: 写入数据
        *  @param[in] nTimeOut: 超时时间
        *  @return: 写入是否成功
        */
        public bool WriteDataToPort(string strData, int nTimeOut)
        {
            bool bReturn = false;
            bReturn = WriteDataToPort(strData);
            if (!bReturn)
                return bReturn;

            string strReceive = ReadDataFromPort(nTimeOut);
            if (strReceive.Contains(">Ok"))
                bReturn = true;
            else
                bReturn = false;

            return bReturn;
        }
        /// <summary>
        /// 获取串口数据
        /// </summary>
        /// <param name="strData"></param>
        /// <param name="nTimeOut"></param>
        /// <returns></returns>
        public string ReadData(string strData, int nTimeOut)
        {
            bool bReturn = false;
            bReturn = WriteDataToPort(strData);
            if (!bReturn)
                return string.Empty;
            return ReadDataFromPort(nTimeOut);
        }

        /* @brief 写入数据到串口
        *  @param[in] strData: 写入数据
        *  @return: 写入是否成功
        */
        public bool WriteDataToPort(string strData)
        {
            bool bReturn = false;
            strData += "\r";
            System.Threading.Monitor.Enter(m_locker);
            try
            {
                m_port.Write(strData);
                bReturn = true;
            }
            catch (Exception)
            {
                bReturn = false;
            }
            System.Threading.Monitor.Exit(m_locker);

            return bReturn;
        }


        /* @brief 读取数据从串口
        *  @param[in] nTimeOut: 超时时间
        *  @return: 写入是否成功
        */
        public string ReadDataFromPort(int nTimeout)
        {
            string strReadData = string.Empty;

            m_watch.Restart();
            TimeSpan ts = m_watch.Elapsed;

            System.Threading.Monitor.Enter(m_locker);
            while (ts.TotalMilliseconds <= nTimeout)
            {
                if (!IsOpen())
                    break;

                strReadData += m_port.ReadExisting();

                //只有当指令执行完成后返回
                if (strReadData.Contains(">Ok\r") ||
                    strReadData.Contains(">128\r") ||
                    strReadData.Contains(">130\r") ||
                    strReadData.Contains(">131\r") ||
                    strReadData.Contains(">132\r") ||
                    strReadData.Contains(">133\r"))
                {
                    break;
                }
            }
            System.Threading.Monitor.Exit(m_locker);

            m_watch.Stop();

            return strReadData;
        }
    }
}
