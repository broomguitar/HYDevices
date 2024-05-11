using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Basic.OCR
{
    [Export(typeof(IAlgorithm))]
    public class OCR_cpu : AbstractAlgorithm, IAlgorithm
    {
        #region OCR
        private const string dllPath = @".\ocr_cpu\ppocr.dll";
        [DllImport(dllPath, EntryPoint = "Init")]
        extern static int init();
        [DllImport(dllPath, EntryPoint = "GetOcr_StrByImageFile")]
        extern static int GetOcr_StrByImageFile(IntPtr imagefile, ref byte pStr, int channel = 3);
        [DllImport(dllPath, EntryPoint = "GetOcrByImageFile")]
        extern static int GetOcrByImageFile(IntPtr imagefile, ref OCRModel.OCRResultContainer container, int channel = 3);
        [DllImport(dllPath, EntryPoint = "GetOcr_StrByImageBuffer")]
        extern static int GetOcr_StrByImageBuffer(IntPtr pArray, int nSize, ref byte pStr, int channel = 3);
        [DllImport(dllPath, EntryPoint = "GetOcrByImageBuffer")]
        extern static int GetOcrByImageBuffer(IntPtr pArray, int nSize, ref OCRModel.OCRResultContainer container, int channel = 3);
        [DllImport(dllPath, EntryPoint = "UnInit")]
        extern static int unInit();
        #endregion
        private static readonly object _lockObj = new object();
        private static OCR_cpu _instance;
        public static OCR_cpu Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new OCR_cpu();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.OCR;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();
        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                return IsInit = init()==0;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            try
            {

                if (!IsInit)
                {
                    throw new Exception("未初始化模型");
                }
                Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
                int ret = -1;
                byte[] s = new byte[1024];
                if (actionParams["Image"] is string)
                {
                    ret = GetOcr_StrByImageFile(Marshal.StringToHGlobalAnsi(actionParams["Image"]), ref s[0]);
                }
                else
                {

                    Utils.ImageHelper.BitmapToBytes(actionParams["Image"], out byte[] buffer);
                    IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    ret = GetOcr_StrByImageBuffer(ptr, buffer.Length, ref s[0]);
                }
                string dec = Encoding.Default.GetString(s, 0, s.Length).Replace("\u0000", string.Empty);
                results.Add("result", dec);
                return results;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public override void UnInit()
        {
            try
            {
                if (IsInit)
                {
                    unInit();
                    IsInit = false;
                }
            }
            catch { }

        }
    }
}
