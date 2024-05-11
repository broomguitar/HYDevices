using HslCommunication;
using HslCommunication.Core;
using HslCommunication.Profinet.Siemens;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.PLC
{
    public class HYPLCComm_InovanceNet : AbstractHYPLCComm_Net
    {

        public override IReadWriteNet PLCNetworkReaderWriter => _plc_InovanceNet;
        public InovanceNet _plc_InovanceNet;
        public int NetId = 1;

        public HYPLCComm_InovanceNet(InovancePLCS plctype, string ipAddress, int port = 502) : this(plctype)
        {
            IP = ipAddress;
            Port = port;
        }

        public HYPLCComm_InovanceNet(InovancePLCS plctype=InovancePLCS.H5U)
        {
            _plc_InovanceNet = new InovanceNet(plctype);
        }

        public override bool Close()
        {
            try
            {
                if (_plc_InovanceNet.DisConnect())
                {
                    _isConnected = false;
                }
                return IsConnected;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public override bool Open()
        {
            try
            {
                _plc_InovanceNet.IP = IP;
                _plc_InovanceNet.Port = Port;
                if (_plc_InovanceNet.Connect())
                {
                    _isConnected = true;
                }
                return IsConnected;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override void Dispose()
        {
            Close();
            _plc_InovanceNet = null;
        }
    }
    public class InovanceNet : IReadWriteNet
    {
        [DllImport("StandardModbusApi.dll", EntryPoint = "Init_ETH_String", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Init_ETH_String(string sIpAddr, int nNetId = 0, int IpPort = 502);

        [DllImport("StandardModbusApi.dll", EntryPoint = "Exit_ETH", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool Exit_ETH(int nNetId = 0);

        [DllImport("StandardModbusApi.dll", EntryPoint = "H5u_Write_Soft_Elem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int H5u_Write_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);

        [DllImport("StandardModbusApi.dll", EntryPoint = "H5u_Read_Soft_Elem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int H5u_Read_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);

        [DllImport("StandardModbusApi.dll", EntryPoint = "H5u_Read_Device_Block", CallingConvention = CallingConvention.Cdecl)]
        public static extern int H5u_Read_Device_Block(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);

        [DllImport("StandardModbusApi.dll", EntryPoint = "H5u_Write_Device_Block", CallingConvention = CallingConvention.Cdecl)]
        public static extern int H5u_Write_Device_Block(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);


        [DllImport("StandardModbusApi.dll", EntryPoint = "Read_Soft_Elem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int H3u_Read_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);

        [DllImport("StandardModbusApi.dll", EntryPoint = "H3u_Write_Soft_Elem", CallingConvention = CallingConvention.Cdecl)]
        public static extern int H3u_Write_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId = 0);

        public string IP;
        public int Port;
        public int NetId = 1;
        public InovancePLCS inovancePLCS;
        public InovanceNet(InovancePLCS inovance)
        {
            inovancePLCS = inovance;
        }
        public bool Connect()
        {
            try
            {
                return Init_ETH_String(IP, NetId, Port);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public bool DisConnect()
        {
            try
            {
                return Exit_ETH(NetId);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private Tuple<SoftElemType, int> GetSoftType(string address)
        {
            try
            {
                string SoftType = address.Substring(0, 1);
                int Address = Convert.ToInt16(address.Substring(1));
                SoftElemType ElemType;
                switch (inovancePLCS)
                {
                    case InovancePLCS.H3U:
                        switch (SoftType.ToUpper())
                        {
                            case "Y": ElemType = SoftElemType.REGI_H3U_Y; break;
                            case "X": ElemType = SoftElemType.REGI_H3U_X; break;
                            case "S": ElemType = SoftElemType.REGI_H3U_S; break;
                            case "B": ElemType = SoftElemType.REGI_H3U_TB; break;
                            case "D": ElemType = SoftElemType.REGI_H3U_D; break;
                            case "R": ElemType = SoftElemType.REGI_H3U_R; break;
                            default: ElemType = SoftElemType.REGI_H3U_M; break;
                        }
                        break;
                    default:
                        switch (SoftType.ToUpper())
                        {
                            case "Y": ElemType = SoftElemType.REGI_H5U_Y; break;
                            case "X": ElemType = SoftElemType.REGI_H5U_X; break;
                            case "S": ElemType = SoftElemType.REGI_H5U_S; break;
                            case "B": ElemType = SoftElemType.REGI_H5U_B; break;
                            case "D": ElemType = SoftElemType.REGI_H5U_D; break;
                            case "R": ElemType = SoftElemType.REGI_H5U_R; break;
                            default: ElemType = SoftElemType.REGI_H5U_M; break;
                        }
                        break;
                }
                switch (SoftType.ToUpper())
                {
                    case "Y": ElemType = SoftElemType.REGI_H5U_Y; break;
                    case "X": ElemType = SoftElemType.REGI_H5U_X; break;
                    case "S": ElemType = SoftElemType.REGI_H5U_S; break;
                    case "B": ElemType = SoftElemType.REGI_H5U_B; break;
                    case "D": ElemType = SoftElemType.REGI_H5U_D; break;
                    case "R": ElemType = SoftElemType.REGI_H5U_R; break;
                    default: ElemType = SoftElemType.REGI_H5U_M; break;
                }
                return new Tuple<SoftElemType, int>(ElemType, Address);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        private int Read_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId)
        {
            try
            {
                switch (inovancePLCS)
                {
                    case InovancePLCS.H3U:
                        return H3u_Read_Soft_Elem(eType, nStartAddr, nCount, pValue, nNetId);
                    default:
                        return H5u_Read_Soft_Elem(eType, nStartAddr, nCount, pValue, nNetId); ;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        private int Write_Soft_Elem(SoftElemType eType, int nStartAddr, int nCount, byte[] pValue, int nNetId)
        {
            try
            {
                switch (inovancePLCS)
                {
                    case InovancePLCS.H3U:
                        return H3u_Write_Soft_Elem(eType, nStartAddr, nCount, pValue, nNetId);
                    default:
                        return H5u_Write_Soft_Elem(eType, nStartAddr, nCount, pValue, nNetId);
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
        public OperateResult<byte> Read(string address)
        {
            try
            {
                OperateResult<byte> oR = new OperateResult<byte>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[2];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, 1, pBuf, NetId);
                byte iTemp = pBuf[0];
                oR.Content = iTemp;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public OperateResult<byte[]> Read(string address, ushort length)
        {
            try
            {
                OperateResult<byte[]> oR = new OperateResult<byte[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 2];
                int k = Read_Soft_Elem(tuple.Item1, tuple.Item2, length, pBuf, NetId);
                Array.Copy(pBuf, oR.Content, 5);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<short> ReadInt16(string address)
        {
            try
            {
                OperateResult<short> oR = new OperateResult<short>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[2];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, 1, pBuf, NetId);
                short iTemp = BitConverter.ToInt16(pBuf, 0);
                oR.Content = iTemp;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<short[]> ReadInt16(string address, ushort length)
        {
            try
            {
                OperateResult<short[]> oR = new OperateResult<short[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 2];
                int k = Read_Soft_Elem(tuple.Item1, tuple.Item2, length, pBuf, NetId);
                short[] shorts = new short[length];
                for (int i = 0; i < length; i++)
                {
                    byte[] databuf = new byte[2] { 0, 0 };
                    databuf[0] = pBuf[i * 2];
                    databuf[1] = pBuf[i * 2 + 1];
                    shorts[i] = BitConverter.ToInt16(databuf, 0);
                }
                oR.Content = shorts;
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<ushort> ReadUInt16(string address)
        {
            try
            {
                OperateResult<ushort> oR = new OperateResult<ushort>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[2];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, 1, pBuf, NetId);
                ushort iTemp = BitConverter.ToUInt16(pBuf, 0);
                oR.Content = iTemp;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<ushort[]> ReadUInt16(string address, ushort length)
        {
            try
            {
                OperateResult<ushort[]> oR = new OperateResult<ushort[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 2];
                int k = Read_Soft_Elem(tuple.Item1, tuple.Item2, length, pBuf, NetId);
                ushort[] shorts = new ushort[length];
                for (int i = 0; i < length; i++)
                {
                    byte[] databuf = new byte[2] { 0, 0 };
                    databuf[0] = pBuf[i * 2];
                    databuf[1] = pBuf[i * 2 + 1];
                    shorts[i] = BitConverter.ToUInt16(databuf, 0);
                }
                oR.Content = shorts;
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<int> ReadInt32(string address)
        {
            try
            {
                OperateResult<int> oR = new OperateResult<int>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[4];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, 2, pBuf, NetId);
                int iTemp = BitConverter.ToInt32(pBuf, 0);
                oR.Content = iTemp;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<int[]> ReadInt32(string address, ushort length)
        {
            try
            {
                OperateResult<int[]> oR = new OperateResult<int[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 4];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, length * 2, pBuf, NetId);
                int[] ints = new int[length];
                for (int k = 0; k < length; k++)
                {
                    byte[] databuf = new byte[4] { 0, 0, 0, 0 };
                    databuf[0] = pBuf[k * 4];
                    databuf[1] = pBuf[k * 4 + 1];
                    databuf[2] = pBuf[k * 4 + 2];
                    databuf[3] = pBuf[k * 4 + 3];
                    int iTemp = BitConverter.ToInt32(databuf, 0);
                    ints[k] = iTemp;
                }
                oR.Content = ints;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<uint> ReadUInt32(string address)
        {
            try
            {
                OperateResult<uint> oR = new OperateResult<uint>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[4];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, 2, pBuf, NetId);
                uint iTemp = BitConverter.ToUInt32(pBuf, 0);
                oR.Content = iTemp;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<uint[]> ReadUInt32(string address, ushort length)
        {
            try
            {
                OperateResult<uint[]> oR = new OperateResult<uint[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 4];
                int k = Read_Soft_Elem(tuple.Item1, tuple.Item2, length * 2, pBuf, NetId);
                uint[] ints = new uint[length];
                for (int i = 0; i < length; i++)
                {
                    byte[] databuf = new byte[4] { 0, 0, 0, 0 };
                    databuf[0] = pBuf[k * 4];
                    databuf[1] = pBuf[i * 4 + 1];
                    databuf[2] = pBuf[i * 4 + 2];
                    databuf[3] = pBuf[i * 4 + 3];
                    uint iTemp = BitConverter.ToUInt32(databuf, 0);
                    ints[i] = iTemp;
                }
                oR.Content = ints;
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<long> ReadInt64(string address)
        {
            try
            {
                OperateResult<long> oR = new OperateResult<long>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[8];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, 4, pBuf, NetId);
                long iTemp = BitConverter.ToInt64(pBuf, 0);
                oR.Content = iTemp;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<long[]> ReadInt64(string address, ushort length)
        {
            try
            {
                OperateResult<long[]> oR = new OperateResult<long[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 8];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, length * 4, pBuf, NetId);
                long[] longs = new long[length];
                for (int k = 0; k < length; k++)
                {
                    byte[] databuf = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    databuf[0] = pBuf[k * 8];
                    databuf[1] = pBuf[k * 8 + 1];
                    databuf[2] = pBuf[k * 8 + 2];
                    databuf[3] = pBuf[k * 8 + 3];
                    databuf[4] = pBuf[k * 8 + 4];
                    databuf[5] = pBuf[k * 8 + 5];
                    databuf[6] = pBuf[k * 8 + 6];
                    databuf[7] = pBuf[k * 8 + 7];
                    longs[k] = BitConverter.ToInt64(databuf, 0);
                }
                oR.Content = longs;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<ulong> ReadUInt64(string address)
        {
            try
            {
                OperateResult<ulong> oR = new OperateResult<ulong>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[8];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, 4, pBuf, NetId);
                ulong iTemp = BitConverter.ToUInt64(pBuf, 0);
                oR.Content = iTemp;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<ulong[]> ReadUInt64(string address, ushort length)
        {
            try
            {
                OperateResult<ulong[]> oR = new OperateResult<ulong[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 8];
                int k = Read_Soft_Elem(tuple.Item1, tuple.Item2, length * 4, pBuf, NetId);
                ulong[] longs = new ulong[length];
                for (int i = 0; i < length; i++)
                {
                    byte[] databuf = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    databuf[0] = pBuf[k * 8];
                    databuf[1] = pBuf[k * 8 + 1];
                    databuf[2] = pBuf[k * 8 + 2];
                    databuf[3] = pBuf[k * 8 + 3];
                    databuf[4] = pBuf[k * 8 + 4];
                    databuf[5] = pBuf[k * 8 + 5];
                    databuf[6] = pBuf[k * 8 + 6];
                    databuf[7] = pBuf[k * 8 + 7];
                    longs[i] = BitConverter.ToUInt64(databuf, 0);
                }
                oR.Content = longs;
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<float> ReadFloat(string address)
        {
            try
            {
                OperateResult<float> oR = new OperateResult<float>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[4];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, 2, pBuf, NetId);
                float iTemp = BitConverter.ToSingle(pBuf, 0);
                oR.Content = iTemp;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<float[]> ReadFloat(string address, ushort length)
        {
            try
            {
                OperateResult<float[]> oR = new OperateResult<float[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 4];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, length * 2, pBuf, NetId);
                float[] ints = new float[length];
                for (int k = 0; k < length; k++)
                {
                    byte[] databuf = new byte[4] { 0, 0, 0, 0 };
                    databuf[0] = pBuf[k * 4];
                    databuf[1] = pBuf[k * 4 + 1];
                    databuf[2] = pBuf[k * 4 + 2];
                    databuf[3] = pBuf[k * 4 + 3];
                    float iTemp = BitConverter.ToSingle(databuf, 0);
                    ints[k] = iTemp;
                }
                oR.Content = ints;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<double> ReadDouble(string address)
        {
            try
            {
                OperateResult<double> oR = new OperateResult<double>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[8];
                int i = Read_Soft_Elem(tuple.Item1, tuple.Item2, 4, pBuf, NetId);
                double iTemp = BitConverter.ToDouble(pBuf, 0);
                oR.Content = iTemp;
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<double[]> ReadDouble(string address, ushort length)
        {
            try
            {
                OperateResult<double[]> oR = new OperateResult<double[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 8];
                int k = Read_Soft_Elem(tuple.Item1, tuple.Item2, length * 4, pBuf, NetId);
                double[] ints = new double[length];
                for (int i = 0; i < length; i++)
                {
                    byte[] databuf = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    databuf[0] = pBuf[i * 4];
                    databuf[1] = pBuf[i * 4 + 1];
                    databuf[2] = pBuf[i * 4 + 2];
                    databuf[3] = pBuf[i * 4 + 3];
                    databuf[4] = pBuf[i * 4 + 4];
                    databuf[5] = pBuf[i * 4 + 5];
                    databuf[6] = pBuf[i * 4 + 6];
                    databuf[7] = pBuf[i * 4 + 7];
                    double iTemp = BitConverter.ToDouble(databuf, 0);
                    ints[i] = iTemp;
                }
                oR.Content = ints;
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<string> ReadString(string address, ushort length)
        {
            try
            {
                OperateResult<string> oR = new OperateResult<string>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 128];
                int k = Read_Soft_Elem(tuple.Item1, tuple.Item2, length * 64, pBuf, NetId);
                oR.Content = Encoding.ASCII.GetString(pBuf, 0, pBuf.Length);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }

                return OperateResult.CreateSuccessResult(Encoding.ASCII.GetString(pBuf, 0, pBuf.Length));
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<T> ReadCustomer<T>(string address) where T : IDataTransfer, new()
        {
            throw new NotImplementedException();
        }

        public OperateResult<T> Read<T>() where T : class, new()
        {
            throw new NotImplementedException();
        }

        public OperateResult Write(string address, byte[] value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, (value.Length + 1) / 2, value, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, short value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] bytes = BitConverter.GetBytes(value);
                int i = Write_Soft_Elem(tuple.Item1, tuple.Item2, 1, bytes, NetId);
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, short[] values)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[values.Length * 2];
                for (int i = 0; i < values.Length; i++)
                {
                    int idata = Convert.ToInt16(values[i]);
                    byte[] dataBuf = new byte[2] { 0, 0 };
                    dataBuf = BitConverter.GetBytes(idata);
                    pBuf[2 * i] = dataBuf[0];
                    pBuf[2 * i + 1] = dataBuf[1];
                }
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, 1, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, ushort value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] bytes = BitConverter.GetBytes(value);
                int i = Write_Soft_Elem(tuple.Item1, tuple.Item2, 1, bytes, NetId);
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, ushort[] values)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[values.Length * 2];
                for (int i = 0; i < values.Length; i++)
                {
                    ushort idata = Convert.ToUInt16(values[i]);
                    byte[] dataBuf = new byte[2] { 0, 0 };
                    dataBuf = BitConverter.GetBytes(idata);
                    pBuf[2 * i] = dataBuf[0];
                    pBuf[2 * i + 1] = dataBuf[1];
                }
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, 1, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, int value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] bytes = BitConverter.GetBytes(value);
                int i = Write_Soft_Elem(tuple.Item1, tuple.Item2, 2, bytes, NetId);
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, int[] values)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[values.Length * 4];
                for (int i = 0; i < values.Length; i++)
                {
                    int idata = Convert.ToInt16(values[i]);
                    byte[] dataBuf = new byte[4] { 0, 0, 0, 0 };
                    dataBuf = BitConverter.GetBytes(idata);
                    pBuf[2 * i] = dataBuf[0];
                    pBuf[2 * i + 1] = dataBuf[1];
                    pBuf[2 * i + 2] = dataBuf[2];
                    pBuf[2 * i + 3] = dataBuf[3];
                }
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, values.Length * 2, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, uint value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] bytes = BitConverter.GetBytes(value);
                int i = Write_Soft_Elem(tuple.Item1, tuple.Item2, 2, bytes, NetId);
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, uint[] values)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[values.Length * 4];
                for (int i = 0; i < values.Length; i++)
                {
                    uint idata = Convert.ToUInt32(values[i]);
                    byte[] dataBuf = new byte[4] { 0, 0, 0, 0 };
                    dataBuf = BitConverter.GetBytes(idata);
                    pBuf[2 * i] = dataBuf[0];
                    pBuf[2 * i + 1] = dataBuf[1];
                    pBuf[2 * i + 2] = dataBuf[2];
                    pBuf[2 * i + 3] = dataBuf[3];
                }
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, values.Length * 2, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, long value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] bytes = BitConverter.GetBytes(value);
                int i = Write_Soft_Elem(tuple.Item1, tuple.Item2, 4, bytes, NetId);
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, long[] values)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[values.Length * 8];
                for (int i = 0; i < values.Length; i++)
                {
                    long idata = Convert.ToInt64(values[i]);
                    byte[] dataBuf = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    dataBuf = BitConverter.GetBytes(idata);
                    pBuf[2 * i] = dataBuf[0];
                    pBuf[2 * i + 1] = dataBuf[1];
                    pBuf[2 * i + 2] = dataBuf[2];
                    pBuf[2 * i + 3] = dataBuf[3];
                    pBuf[2 * i + 4] = dataBuf[4];
                    pBuf[2 * i + 5] = dataBuf[5];
                    pBuf[2 * i + 6] = dataBuf[6];
                    pBuf[2 * i + 7] = dataBuf[7];
                }
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, values.Length * 4, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, ulong value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] bytes = BitConverter.GetBytes(value);
                int i = Write_Soft_Elem(tuple.Item1, tuple.Item2, 4, bytes, NetId);
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, ulong[] values)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[values.Length * 8];
                for (int i = 0; i < values.Length; i++)
                {
                    ulong idata = Convert.ToUInt64(values[i]);
                    byte[] dataBuf = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    dataBuf = BitConverter.GetBytes(idata);
                    pBuf[2 * i] = dataBuf[0];
                    pBuf[2 * i + 1] = dataBuf[1];
                    pBuf[2 * i + 2] = dataBuf[2];
                    pBuf[2 * i + 3] = dataBuf[3];
                    pBuf[2 * i + 4] = dataBuf[4];
                    pBuf[2 * i + 5] = dataBuf[5];
                    pBuf[2 * i + 6] = dataBuf[6];
                    pBuf[2 * i + 7] = dataBuf[7];
                }
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, values.Length * 4, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, float value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] bytes = BitConverter.GetBytes(value);
                int i = Write_Soft_Elem(tuple.Item1, tuple.Item2, 2, bytes, NetId);
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, float[] values)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[values.Length * 4];
                for (int i = 0; i < values.Length; i++)
                {
                    byte[] dataBuf = new byte[4] { 0, 0, 0, 0 };
                    dataBuf = BitConverter.GetBytes(values[i]);
                    pBuf[2 * i] = dataBuf[0];
                    pBuf[2 * i + 1] = dataBuf[1];
                    pBuf[2 * i + 2] = dataBuf[2];
                    pBuf[2 * i + 3] = dataBuf[3];
                }
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, values.Length * 2, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, double value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] bytes = BitConverter.GetBytes(value);
                int i = Write_Soft_Elem(tuple.Item1, tuple.Item2, 4, bytes, NetId);
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, double[] values)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[values.Length * 8];
                for (int i = 0; i < values.Length; i++)
                {
                    byte[] dataBuf = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                    dataBuf = BitConverter.GetBytes(values[i]);
                    pBuf[2 * i] = dataBuf[0];
                    pBuf[2 * i + 1] = dataBuf[1];
                    pBuf[2 * i + 2] = dataBuf[2];
                    pBuf[2 * i + 3] = dataBuf[3];
                    pBuf[2 * i + 4] = dataBuf[4];
                    pBuf[2 * i + 5] = dataBuf[5];
                    pBuf[2 * i + 6] = dataBuf[6];
                    pBuf[2 * i + 7] = dataBuf[7];
                }
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, values.Length * 4, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, string value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = Encoding.UTF8.GetBytes(value);
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, (value.Length + 1) / 2, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, string value, int length)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = Encoding.UTF8.GetBytes(value);
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, (length + 1) / 2, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult WriteCustomer<T>(string address, T value) where T : IDataTransfer, new()
        {
            throw new NotImplementedException();
        }

        public OperateResult Write<T>(T data) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public OperateResult<bool[]> ReadBool(string address, ushort length)
        {
            try
            {
                OperateResult<bool[]> oR = new OperateResult<bool[]>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[length * 2];
                int k = Read_Soft_Elem(tuple.Item1, tuple.Item2, length, pBuf, NetId);
                bool[] bools = new bool[length];
                for (int i = 0; i < length; i++)
                {
                    bools[i] = Convert.ToBoolean(pBuf[i]);
                }
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult<bool> ReadBool(string address)
        {
            try
            {
                OperateResult<bool> oR = new OperateResult<bool>();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[2];
                int k = Read_Soft_Elem(tuple.Item1, tuple.Item2, 1, pBuf, NetId);
                bool iTemp = BitConverter.ToBoolean(pBuf, 0);
                oR.Content = iTemp;
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, bool[] value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] pBuf = new byte[value.Length * 4];
                for (int i = 0; i < value.Length; i++)
                {
                    int idata = Convert.ToInt16(value[i]);
                    byte[] dataBuf = new byte[4] { 0, 0, 0, 0 };
                    dataBuf = BitConverter.GetBytes(idata);
                    pBuf[2 * i] = dataBuf[0];
                    pBuf[2 * i + 1] = dataBuf[1];
                    pBuf[2 * i + 2] = dataBuf[2];
                    pBuf[2 * i + 3] = dataBuf[3];
                }
                int k = Write_Soft_Elem(tuple.Item1, tuple.Item2, value.Length, pBuf, NetId);
                if (k == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = k;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public OperateResult Write(string address, bool value)
        {
            try
            {
                OperateResult oR = new OperateResult();
                Tuple<SoftElemType, int> tuple = GetSoftType(address);
                byte[] bytes = BitConverter.GetBytes(value);
                int i = Write_Soft_Elem(tuple.Item1, tuple.Item2, 1, bytes, NetId);
                if (i == 1)
                {
                    oR.IsSuccess = true;
                }
                else
                {
                    oR.IsSuccess = false;
                    oR.ErrorCode = i;
                }
                return oR;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
    public enum InovancePLCS
    {
        H3U,
        /// <summary>
        /// 支持汇川H5U及Easy500系列PLC
        /// </summary>
        H5U
    }
    public enum SoftElemType
    {
        //H3U
        REGI_H3U_Y = 0x20,       //Y元件的定义	
        REGI_H3U_X = 0x21,		//X元件的定义							
        REGI_H3U_S = 0x22,		//S元件的定义				
        REGI_H3U_M = 0x23,		//M元件的定义							
        REGI_H3U_TB = 0x24,		//T位元件的定义				
        REGI_H3U_TW = 0x25,		//T字元件的定义				
        REGI_H3U_CB = 0x26,		//C位元件的定义				
        REGI_H3U_CW = 0x27,		//C字元件的定义				
        REGI_H3U_D = 0x28,		//D字元件的定义				
        REGI_H3U_CW2 = 0x29,	//C双字元件的定义
        REGI_H3U_SM = 0x2a,		//SM
        REGI_H3U_SD = 0x2b,		//
        REGI_H3U_R = 0x2c,		//
        //H5u
        REGI_H5U_Y = 0x30,       //Y元件的定义	
        REGI_H5U_X = 0x31,		//X元件的定义							
        REGI_H5U_S = 0x32,		//S元件的定义				
        REGI_H5U_M = 0x33,		//M元件的定义	
        REGI_H5U_B = 0x34,       //B元件的定义
        REGI_H5U_D = 0x35,       //D字元件的定义
        REGI_H5U_R = 0x36,       //R字元件的定义

    }
}
