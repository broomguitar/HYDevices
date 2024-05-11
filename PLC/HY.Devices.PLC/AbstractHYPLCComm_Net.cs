using HslCommunication;
using HslCommunication.Core;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace HY.Devices.PLC
{
    /// <summary>
    /// 网络连接PLC的基类
    /// </summary>
    public abstract class AbstractHYPLCComm_Net : IHYPLCComm
    {
        /// <summary>
        /// 网络PLC读写器
        /// </summary>
        public abstract IReadWriteNet PLCNetworkReaderWriter { get; }
        /// <summary>
        /// ip地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// 连接超时时长，默认5000， 单位毫秒
        /// </summary>
        public int ConnectTimeOut { get; set; } = 5000;
        protected bool _isConnected = false;
        /// <summary>
        /// 连接状态
        /// </summary>
        public virtual bool IsConnected => _isConnected;
        public abstract bool Open();
        public abstract bool Close();
        public virtual void Dispose()
        {
            Close();
        }
        public virtual T Read<T>(string address)
        {
            try
            {
                OperateResult result = null;
                T ret = default(T);
                TypeCode typeCode = Type.GetTypeCode(typeof(T));
                switch (typeCode)
                {
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Object:
                        break;
                    case TypeCode.DBNull:
                        break;
                    case TypeCode.Boolean:
                        {
                            OperateResult<bool> operateResult = PLCNetworkReaderWriter.ReadBool(address);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    case TypeCode.Char:
                        {
                            OperateResult<byte[]> operateResult = PLCNetworkReaderWriter.Read(address, 1);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(BitConverter.ToChar(operateResult.Content, 0), typeof(T));
                            }
                        }
                        break;
                    case TypeCode.SByte:
                        {
                            OperateResult<byte[]> operateResult = PLCNetworkReaderWriter.Read(address, 1);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content[0], typeof(T));
                            }
                        }
                        break;
                    case TypeCode.Byte:
                        {
                            OperateResult<byte[]> operateResult = PLCNetworkReaderWriter.Read(address, 1);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content[0], typeof(T));
                            }
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            OperateResult<short> operateResult = PLCNetworkReaderWriter.ReadInt16(address);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            OperateResult<ushort> operateResult = PLCNetworkReaderWriter.ReadUInt16(address);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            OperateResult<int> operateResult = PLCNetworkReaderWriter.ReadInt32(address);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    case TypeCode.UInt32:
                        {
                            OperateResult<uint> operateResult = PLCNetworkReaderWriter.ReadUInt32(address);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    case TypeCode.Int64:
                        {
                            OperateResult<long> operateResult = PLCNetworkReaderWriter.ReadInt64(address);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    case TypeCode.UInt64:
                        {
                            OperateResult<ulong> operateResult = PLCNetworkReaderWriter.ReadUInt64(address);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    case TypeCode.Single:
                        {
                            OperateResult<float> operateResult = PLCNetworkReaderWriter.ReadFloat(address);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    case TypeCode.Double:
                        {
                            OperateResult<double> operateResult = PLCNetworkReaderWriter.ReadDouble(address);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    case TypeCode.Decimal:
                        break;
                    case TypeCode.DateTime:
                        break;
                    case TypeCode.String:
                        {
                            OperateResult<string> operateResult = PLCNetworkReaderWriter.ReadString(address, 1);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T)Convert.ChangeType(operateResult.Content, typeof(T));
                            }
                        }
                        break;
                    default:
                        break;
                }
                if (result.IsSuccess)
                {
                    return ret;
                }
                else
                {
                    throw new Exception(result.Message);
                }
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
                OperateResult result = null;
                T[] ret = default(T[]);
                TypeCode typeCode = Type.GetTypeCode(typeof(T));
                switch (typeCode)
                {
                    case TypeCode.Empty:
                        break;
                    case TypeCode.Object:
                        break;
                    case TypeCode.DBNull:
                        break;
                    case TypeCode.Boolean:
                        {
                            OperateResult<bool[]> operateResult = PLCNetworkReaderWriter.ReadBool(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.Char:
                        {
                            OperateResult<string> operateResult = PLCNetworkReaderWriter.ReadString(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content.ToCharArray(), typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.SByte:
                        {
                            OperateResult<byte[]> operateResult = PLCNetworkReaderWriter.Read(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.Byte:
                        {
                            OperateResult<byte[]> operateResult = PLCNetworkReaderWriter.Read(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.Int16:
                        {
                            OperateResult<short[]> operateResult = PLCNetworkReaderWriter.ReadInt16(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.UInt16:
                        {
                            OperateResult<ushort[]> operateResult = PLCNetworkReaderWriter.ReadUInt16(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.Int32:
                        {
                            OperateResult<int[]> operateResult = PLCNetworkReaderWriter.ReadInt32(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.UInt32:
                        {
                            OperateResult<uint[]> operateResult = PLCNetworkReaderWriter.ReadUInt32(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.Int64:
                        {
                            OperateResult<long[]> operateResult = PLCNetworkReaderWriter.ReadInt64(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.UInt64:
                        {
                            OperateResult<ulong[]> operateResult = PLCNetworkReaderWriter.ReadUInt64(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.Single:
                        {
                            OperateResult<float[]> operateResult = PLCNetworkReaderWriter.ReadFloat(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.Double:
                        {
                            OperateResult<double[]> operateResult = PLCNetworkReaderWriter.ReadDouble(address, length);
                            result = operateResult;
                            if (result.IsSuccess)
                            {
                                ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            }
                        }
                        break;
                    case TypeCode.Decimal:
                        break;
                    case TypeCode.DateTime:
                        break;
                    case TypeCode.String:
                        {
                            //OperateResult<string> operateResult = PLCNetworkReaderWriter.ReadString(address, length);
                            //result = operateResult;
                            //if (result.IsSuccess)
                            //{
                            //    ret = (T[])Convert.ChangeType(operateResult.Content, typeof(T[]));
                            //}
                        }
                        break;
                    default:
                        break;
                }
                if (result.IsSuccess)
                {
                    return ret;
                }
                else
                {
                    throw new Exception(result.Message);
                }
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
                OperateResult Ret = null;
                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        {
                            bool value = (bool)Convert.ChangeType(data, typeof(bool));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.Char:
                        {
                            char value = (char)Convert.ChangeType(data, typeof(char));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.SByte:
                        {
                            sbyte value = (sbyte)Convert.ChangeType(data, typeof(sbyte));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.Byte:
                        {
                            byte value = (byte)Convert.ChangeType(data, typeof(byte));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.Int16:
                        {
                            short value = (short)Convert.ChangeType(data, typeof(short));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.UInt16:
                        {
                            ushort value = (ushort)Convert.ChangeType(data, typeof(ushort));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.Int32:
                        {
                            int value = (int)Convert.ChangeType(data, typeof(int));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.UInt32:
                        {
                            uint value = (uint)Convert.ChangeType(data, typeof(uint));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.Int64:
                        {
                            long value = (long)Convert.ChangeType(data, typeof(long));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.UInt64:
                        {
                            ulong value = (ulong)Convert.ChangeType(data, typeof(ulong));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.Single:
                        {
                            float value = (float)Convert.ChangeType(data, typeof(float));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.Double:
                        {
                            double value = (double)Convert.ChangeType(data, typeof(double));
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    case TypeCode.String:
                        {
                            string value = data.ToString();
                            Ret = PLCNetworkReaderWriter.Write(address, value);
                            break;
                        }
                    default:
                        break;
                }
                if (Ret.IsSuccess)
                {
                    return true;
                }
                else
                {
                    throw new Exception(Ret.Message);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        protected ConcurrentDictionary<string, bool> _risingEdgeAddressDic = new ConcurrentDictionary<string, bool>(), _fallingEdgeAddressDic = new ConcurrentDictionary<string, bool>();
        public virtual void WaitRisingEdge(string address, int milliseconds, Action<string> callback, bool isSimulation)
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
                    int preValue = PLCNetworkReaderWriter.ReadInt16(address).Content;
                    while (IsConnected && _risingEdgeAddressDic.ContainsKey(address))
                    {
                        System.Threading.Thread.Sleep(milliseconds);
                        int value = PLCNetworkReaderWriter.ReadInt16(address).Content;

                        if (preValue == 0 && value == 1)
                        {
                            callback?.BeginInvoke(address, null, null);
                            if (isSimulation)
                            {
                                Write<short>(address, 0);
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
        public virtual void WaitFallingEdge(string address, int milliseconds, Action<string> callback, bool isSimulation)
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
                    int preValue = PLCNetworkReaderWriter.ReadInt16(address).Content;
                    while (IsConnected && _fallingEdgeAddressDic.ContainsKey(address))
                    {
                        System.Threading.Thread.Sleep(milliseconds);
                        int value = PLCNetworkReaderWriter.ReadInt16(address).Content;
                        if (preValue == 1 && value == 0)
                        {
                            callback?.BeginInvoke(address, null, null);
                            if (isSimulation)
                            {
                                Write<short>(address, 1);
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
