#if NET48_OR_GREATER
using HslCommunication.Serial;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace HY.Devices.PLC
{
    /// <summary>
    /// 串口连接PLC基类
    /// </summary>
    public abstract class AbstractHYPLCComm_Serial<TTransform> : IHYPLCComm where TTransform : HslCommunication.Core.IByteTransform, new()
    {
        /// <summary>
        /// 串口PLC读写器
        /// </summary>
        public abstract SerialDeviceBase<TTransform> PLCSerialReaderWriter { get; }
        public bool IsConnected => PLCSerialReaderWriter?.IsOpen() == true;
        public bool Open()
        {
            try
            {
                PLCSerialReaderWriter.Open();
                return PLCSerialReaderWriter.IsOpen();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool Close()
        {
            try
            {
                _risingEdgeAddressDic.Clear();
                _fallingEdgeAddressDic.Clear();
                PLCSerialReaderWriter.Close();
                return true;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public void Dispose()
        {
            Close();
            PLCSerialReaderWriter.Dispose();
        }

        public virtual T Read<T>(string address)
        {
            try
            {
                byte[] dd = PLCSerialReaderWriter.Read(address, 1).Content;
                return (T)Convert.ChangeType(dd[0], typeof(T));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public virtual T[] Read<T>(string address, ushort length)
        {
            try
            {
                byte[] dd = PLCSerialReaderWriter.Read(address, length).Content;
                T[] data = new T[length];
                for (int i = 0; i < length; i++)
                {
                    data[i] = (T)Convert.ChangeType(dd[i], typeof(T));
                }
                return data;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public virtual bool Write<T>(string address, T data)
        {
            try
            {
                TypeCode typeCode = Type.GetTypeCode(typeof(T));
                ;
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        {
                            bool value = (bool)Convert.ChangeType(data, typeof(bool));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.Char:
                        {
                            char value = (char)Convert.ChangeType(data, typeof(char));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.SByte:
                        {
                            sbyte value = (sbyte)Convert.ChangeType(data, typeof(sbyte));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.Byte:
                        {
                            byte value = (byte)Convert.ChangeType(data, typeof(byte));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.Int16:
                        {
                            short value = (short)Convert.ChangeType(data, typeof(short));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.UInt16:
                        {
                            ushort value = (ushort)Convert.ChangeType(data, typeof(ushort));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.Int32:
                        {
                            int value = (int)Convert.ChangeType(data, typeof(int));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.UInt32:
                        {
                            uint value = (uint)Convert.ChangeType(data, typeof(uint));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.Int64:
                        {
                            long value = (long)Convert.ChangeType(data, typeof(long));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.UInt64:
                        {
                            ulong value = (ulong)Convert.ChangeType(data, typeof(ulong));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.Single:
                        {
                            float value = (float)Convert.ChangeType(data, typeof(float));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.Double:
                        {
                            double value = (double)Convert.ChangeType(data, typeof(double));
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    case TypeCode.String:
                        {
                            string value = data.ToString();
                            return PLCSerialReaderWriter.Write(address, value).IsSuccess;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private ConcurrentDictionary<string, bool> _risingEdgeAddressDic = new ConcurrentDictionary<string, bool>(), _fallingEdgeAddressDic = new ConcurrentDictionary<string, bool>();
        public void WaitRisingEdge(string address, int milliseconds, Action<string> callback, bool isSimulation)
        {
            try
            {
                if (_risingEdgeAddressDic.ContainsKey(address))
                {
                    throw new Exception("该地址正在监测中,可以停止进行中的监测");
                }
                int times = 5;
                while (!_risingEdgeAddressDic.TryAdd(address, true) && times-- > 0)
                {
                    System.Threading.Thread.Sleep(30);
                }
                if (!_risingEdgeAddressDic.ContainsKey(address))
                {
                    throw new Exception("监测异常");
                }
                Task.Run(new Action(() =>
                {
                    int preValue = PLCSerialReaderWriter.ReadInt32(address).Content;
                    while (IsConnected && _risingEdgeAddressDic.ContainsKey(address))
                    {
                        System.Threading.Thread.Sleep(milliseconds);
                        int value = PLCSerialReaderWriter.ReadInt32(address).Content;
                        if (preValue == 0 && value == 1)
                        {
                            callback?.BeginInvoke(address, null, null);
                            if (isSimulation)
                            {
                                Write<int>(address, 0);
                            }
                        }
                        preValue = value;

                    }
                }));
            }
            catch (Exception ex)
            {
                int times = 5;
                while (!_risingEdgeAddressDic.TryRemove(address, out bool flag) && times-- > 0)
                {
                    System.Threading.Thread.Sleep(30);
                }
                throw ex;
            }
        }

        public void WaitFallingEdge(string address, int milliseconds, Action<string> callback, bool isSimulation)
        {
            try
            {
                if (_fallingEdgeAddressDic.ContainsKey(address))
                {
                    throw new Exception("该地址正在监测中,可以停止进行中的监测");
                }
                int times = 5;
                while (!_fallingEdgeAddressDic.TryAdd(address, true) && times-- > 0)
                {
                    System.Threading.Thread.Sleep(30);
                }
                if (!_fallingEdgeAddressDic.ContainsKey(address))
                {
                    throw new Exception("监测异常");
                }
                Task.Run(new Action(() =>
                {
                    int preValue = PLCSerialReaderWriter.ReadInt32(address).Content;
                    while (IsConnected && _fallingEdgeAddressDic.ContainsKey(address))
                    {
                        System.Threading.Thread.Sleep(milliseconds);
                        int value = PLCSerialReaderWriter.ReadInt32(address).Content;
                        if (preValue == 1 && value == 0)
                        {
                            callback?.BeginInvoke(address, null, null);
                            if (isSimulation)
                            {
                                Write<int>(address, 0);
                            }
                        }
                        preValue = value;

                    }
                }));
            }
            catch (Exception ex)
            {
                int times = 5;
                while (!_fallingEdgeAddressDic.TryRemove(address, out bool flag) && times-- > 0)
                {
                    System.Threading.Thread.Sleep(30);
                }

                throw ex;
            }
        }
        public virtual void StopWaitAddress(string address)
        {
            if (_risingEdgeAddressDic.ContainsKey(address))
            {
                _risingEdgeAddressDic[address] = false;
                _risingEdgeAddressDic.TryRemove(address, out bool l);
            }
            if (_fallingEdgeAddressDic.ContainsKey(address))
            {
                _fallingEdgeAddressDic[address] = false;
                _fallingEdgeAddressDic.TryRemove(address, out bool l);
            }
        }
    }
}
#endif
