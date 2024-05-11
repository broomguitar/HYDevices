using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
namespace HY.Devices.Algorithm.QDSMHY
{
    /// <summary>
    /// 二维码检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class QRCodeDetect : AbstractAlgorithm,IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static QRCodeDetect _instance;
        public static QRCodeDetect Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new QRCodeDetect();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.QRcode;

        public override Dictionary<string, dynamic> InitParamNames { get; }= new Dictionary<string, dynamic>();

        public override Dictionary<string, dynamic> ActionParamNames { get; }= new Dictionary<string, dynamic> { { "ImagePath", "" }};

        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParameters)
        {
            if (!IsInit)
            {
                throw new Exception("未初始化模型");
            }
            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();

            // Local iconic variables 

            HObject ho_Image;
            HObject ho_SymbolXLDs = null;
            HObject ho_ImageGauss = null;
            HObject ho_ImageScaled = null, ho_ImageZoomed = null;
            // Local control variables 

            HTuple hv_DataCodeHandle = new HTuple(), hv_index = new HTuple();
            HTuple hv_ResultHandles = new HTuple(), hv_DecodedDataStrings = new HTuple();
            HTuple hv_Index1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_SymbolXLDs);
            HOperatorSet.GenEmptyObj(out ho_ImageGauss);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageZoomed);

            try
            {
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, actionParameters["ImagePath"].ToString());



                ho_ImageGauss.Dispose();
                HOperatorSet.GaussFilter(ho_Image, out ho_ImageGauss, 3);
                ho_ImageScaled.Dispose();
                HOperatorSet.ScaleImage(ho_ImageGauss, out ho_ImageScaled, 1.5, 0);
                hv_DataCodeHandle.Dispose();
                HOperatorSet.CreateDataCode2dModel("QR Code", ((new HTuple("polarity")).TupleConcat(
                    "module_gap_max")).TupleConcat("position_pattern_min"), ((new HTuple("dark_on_light")).TupleConcat(
                    "big")).TupleConcat(2), out hv_DataCodeHandle);

                for (hv_index = 0.5; (double)hv_index <= 2; hv_index = (double)hv_index + 0.5)
                {
                    ho_ImageZoomed.Dispose();
                    HOperatorSet.ZoomImageFactor(ho_ImageScaled, out ho_ImageZoomed, hv_index,
                        hv_index, "bicubic");
                    ho_SymbolXLDs.Dispose(); hv_ResultHandles.Dispose(); hv_DecodedDataStrings.Dispose();
                    HOperatorSet.FindDataCode2d(ho_ImageZoomed, out ho_SymbolXLDs, hv_DataCodeHandle,
                        "stop_after_result_num", 1, out hv_ResultHandles, out hv_DecodedDataStrings);
                    if ((int)(new HTuple((new HTuple(hv_DecodedDataStrings.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        break;
                    }
                }
                for (hv_Index1 = 0; (int)hv_Index1 <= (int)((new HTuple(hv_DecodedDataStrings.TupleLength()
          )) - 1); hv_Index1 = (int)hv_Index1 + 1)
                {
                    results.Add("Result", hv_DecodedDataStrings.TupleSelect(hv_Index1).S);
                    //Console.WriteLine( "读取到二维码："+hv_DecodedDataStrings.TupleSelect(hv_Index1).S);
                }

            }
            catch (Exception ee)
            {
                Console.WriteLine(ee);
            }
            finally
            {
                ho_Image.Dispose();
                ho_ImageGauss.Dispose();
                ho_ImageScaled.Dispose();
                ho_ImageZoomed.Dispose();
                ho_SymbolXLDs.Dispose();

                hv_DataCodeHandle.Dispose();
                hv_index.Dispose();
                hv_ResultHandles.Dispose();
                hv_DecodedDataStrings.Dispose();
                hv_Index1.Dispose();
            }
            
            return results;
        }

        public override bool Init(Dictionary<string, object> initParameters)
        {

            return IsInit = true;
        }

        public override void UnInit()
        {
            IsInit = false;
        }
    }
}

