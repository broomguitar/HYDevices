using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Basic.Halcon
{
    /// <summary>
    /// 二维码识别
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class ReadQRCode : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static ReadQRCode _instance;
        public static ReadQRCode Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new ReadQRCode();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.QRcode;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();
        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" }};
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                return IsInit = true;
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
                string result = string.Empty;
                HObject ho_Image = null;
                HOperatorSet.GenEmptyObj(out ho_Image);
                ho_Image.Dispose();
                ho_Image = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
                result = ReadQRCodeResult(ho_Image);
                results.Add("result", result);
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
                IsInit = false;
            }
            catch { }

        }
        private string ReadQRCodeResult(HObject hb_Image)
        {
            // Local iconic variables 

            HObject ho_Image, ho_SymbolXLDs;

            // Local control variables 

            HTuple hv_Result = null, hv_DataCodeHandle = null;
            HTuple hv_ResultHandles = null, hv_DecodedDataStrings = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_SymbolXLDs);
            try
            {
                ho_Image.Dispose();
                HOperatorSet.CopyImage(hb_Image, out ho_Image);
                HOperatorSet.ZoomImageFactor(ho_Image, out ho_Image, 1, 1.2, "constant");
                //HOperatorSet.Rgb1ToGray(ho_Image, out ho_Image);
                // HOperatorSet.ScaleImage(ho_Image, out ho_Image, 2, 0);
                //结果
                hv_Result = "";

                //图像处理
                HOperatorSet.CreateDataCode2dModel("QR Code", new HTuple(), new HTuple(), out hv_DataCodeHandle);
                //HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle,,)
                ho_SymbolXLDs.Dispose();
                //HOperatorSet.FindDataCode2d(ho_Image, out ho_SymbolXLDs, hv_DataCodeHandle, "stop_after_result_num",
                //    2, out hv_ResultHandles, out hv_DecodedDataStrings);

                HOperatorSet.FindDataCode2d(ho_Image, out ho_SymbolXLDs, hv_DataCodeHandle, new HTuple(),
                 new HTuple(), out hv_ResultHandles, out hv_DecodedDataStrings);
                return hv_DecodedDataStrings.Clone();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ho_Image.Dispose();
                ho_SymbolXLDs.Dispose();
                if (hv_DataCodeHandle != null) HOperatorSet.ClearDataCode2dModel(hv_DataCodeHandle);
            }

        }
    }
}
