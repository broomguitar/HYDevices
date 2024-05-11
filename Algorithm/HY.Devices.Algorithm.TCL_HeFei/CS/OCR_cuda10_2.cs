using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.TCL_HeFei
{
    /// <summary>
    /// 字符识别
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class OCR_cuda10_2 : AbstractAlgorithm, IAlgorithm
    {
        #region OCR
        private const string dllPath = @".\Algorithms\ocr_cuda10.2\ppocr.dll";
        [DllImport(dllPath, EntryPoint = "Init", CallingConvention = CallingConvention.Cdecl,SetLastError  = true)]
        extern static IntPtr init();
        [DllImport(dllPath, EntryPoint = "GetOcr_StrByImageFile", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        extern static int GetOcr_StrByImageFile(IntPtr ocr, ref byte byteStr, string imagefile, int channel = 3);
        [DllImport(dllPath, EntryPoint = "GetOcrByImageFile", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        extern static int GetOcrByImageFile(IntPtr ocr, string imagefile, ref OCRModel.OCRResultContainer container, int channel = 3);
        [DllImport(dllPath, EntryPoint = "GetOcr_StrByImageBuffer", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        extern static int GetOcr_StrByImageBuffer(IntPtr ocr, ref byte byteStr, IntPtr pArray, int nSize, int channel = 3);
        [DllImport(dllPath, EntryPoint = "GetOcrByImageBuffer", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        extern static int GetOcrByImageBuffer(IntPtr ocr, IntPtr pArray, int nSize, ref OCRModel.OCRResultContainer container, int channel = 3);
        [DllImport(dllPath, EntryPoint = "UnInit", CallingConvention = CallingConvention.Cdecl, SetLastError = true)]
        extern static bool unInit(IntPtr ocr);
        #endregion
        private static readonly object _lockObj = new object();
        private static OCR_cuda10_2 _instance;
        public static OCR_cuda10_2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new OCR_cuda10_2();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.OCR;
        private IntPtr ocr = IntPtr.Zero;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>() {  };
        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                ocr = init();
                IsInit = ocr != IntPtr.Zero;
                Dictionary<string, dynamic> actionParams = new Dictionary<string, dynamic> { { "Image", Path.Combine(SaveDir, "logo.jpg") } };
                DoAction(actionParams);
                return IsInit;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private  string Pretreatment(string imagePath)
        {
            HObject ho_Image, ho_GrayImage, ho_ImageScaleMax;
            HObject ho_ImageFFT, ho_ImageLowpass, ho_ImageConvol, ho_ImageFFT1;
            HObject ho_ImageEmphasize;
            HTuple hv_FileName = new HTuple();
            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_ImageScaleMax);
            HOperatorSet.GenEmptyObj(out ho_ImageFFT);
            HOperatorSet.GenEmptyObj(out ho_ImageLowpass);
            HOperatorSet.GenEmptyObj(out ho_ImageConvol);
            HOperatorSet.GenEmptyObj(out ho_ImageFFT1);
            HOperatorSet.GenEmptyObj(out ho_ImageEmphasize);

            try
            {
                string fileName = imagePath.Substring(0, imagePath.LastIndexOf(".") - 1);
                hv_FileName.Dispose();
                hv_FileName = fileName + "_OCR";
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, imagePath);
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                ho_ImageScaleMax.Dispose();
                HOperatorSet.ScaleImageMax(ho_GrayImage, out ho_ImageScaleMax);
                ho_ImageFFT.Dispose();
                HOperatorSet.FftGeneric(ho_ImageScaleMax, out ho_ImageFFT, "to_freq", -1, "sqrt",
                    "dc_center", "complex");

                ho_ImageLowpass.Dispose();
                HOperatorSet.GenLowpass(out ho_ImageLowpass, 0.3, "none", "dc_center", hv_Width,
                    hv_Height);
                ho_ImageConvol.Dispose();
                HOperatorSet.ConvolFft(ho_ImageFFT, ho_ImageLowpass, out ho_ImageConvol);
                ho_ImageFFT1.Dispose();
                HOperatorSet.FftGeneric(ho_ImageConvol, out ho_ImageFFT1, "from_freq", 1, "sqrt",
                    "dc_center", "byte");
                ho_ImageEmphasize.Dispose();
                HOperatorSet.Emphasize(ho_ImageFFT1, out ho_ImageEmphasize, 10, 10, 1);

   
                HOperatorSet.WriteImage(ho_ImageEmphasize, "jpg", 100, hv_FileName);
                return hv_FileName+".jpg";
            }
            catch
            {
                return imagePath;
            }
            finally
            {
                ho_Image.Dispose();
                ho_GrayImage.Dispose();
                ho_ImageScaleMax.Dispose();
                ho_ImageFFT.Dispose();
                ho_ImageLowpass.Dispose();
                ho_ImageConvol.Dispose();
                ho_ImageFFT1.Dispose();
                ho_ImageEmphasize.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
            }


        }
        private object _lock=new object();
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            try
            {

                //if (!IsInit)
                //{
                //    throw new Exception("未初始化模型");
                //}
                lock (_lock)
                {

                    Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
                    int ret = -1;
                    byte[] s = new byte[1024];
                    //IntPtr ret = IntPtr.Zero;
                    if (actionParams.ContainsKey("Image"))
                    {
                        string imagePath = Pretreatment(actionParams["Image"]);
                        ret = GetOcr_StrByImageFile(ocr, ref s[0], imagePath);
                        //ret = GetOcr_StrByImageFile(ocr, imagePath);
                    }
                    else
                    {
                        ret = GetOcr_StrByImageBuffer(ocr, ref s[0], actionParams["Intptr"], actionParams["size"]);
                        //ret = GetOcr_StrByImageBuffer(ocr, actionParams["Intptr"], actionParams["size"]);
                    }
                    //results.Add("result", Marshal.PtrToStringAnsi(ret));
                    string dec = System.Text.Encoding.Default.GetString(s, 0, s.Length).Replace("\u0000", string.Empty);
                    results.Add("result", dec);
                    return results;
                }
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
                    unInit(ocr);
                    IsInit = false;
                }
            }
            catch { }

        }

    }
}
