using Emgu.CV;
using HalconDotNet;
using PaddleOCR.Onnx;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;


namespace HY.Devices.Algorithm.Haier_ZhongDe
{
    [Export(typeof(IAlgorithm))]
    public class OCR_onnx_cpu : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static OCR_onnx_cpu _instance;
        public static OCR_onnx_cpu Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new OCR_onnx_cpu();
                        }
                    }
                }
                return _instance;
            }
        }
        private PaddleOCREngine engine;
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.OCR;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();
        /// <summary>
        /// Logo:默认；NFC Scale 0.2；能耗贴：ZoomW 1.3，Scale0.45
        /// </summary>
        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" }, { "ZoomW", 1.0d }, { "ZoomH", 1.0d }, { "Scale", 1.0d } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                //自带轻量版中英文模型V3
                OCRModelConfig config = null;
                //OCR参数
                OCRParameter oCRParameter = new OCRParameter();
                //oCRParameter.MaxSideLen = 960;
                //oCRParameter.numThread = 6;//预测并发线程数
                //oCRParameter.Enable_mkldnn = true;//web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.

                oCRParameter.Enable_mkldnn = true;//web部署该值建议设置为0,否则出错，内存如果使用很大，建议该值也设置为0.
                oCRParameter.numThread = 10;
                oCRParameter.use_angle_cls = false;//是否开启方向检测，用于检测识别180旋转
                oCRParameter.use_polygon_score = false;//是否使用多段线，即文字区域是用多段线还是用矩形，
                oCRParameter.use_gpu = false;
                oCRParameter.gpu_id = 0;
                oCRParameter.gpu_mem = 12000;
                //初始化OCR引擎
                engine = new PaddleOCREngine(config, oCRParameter);
                return IsInit = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private readonly object lockFile = new object();
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            try
            {

                if (!IsInit)
                {
                    throw new Exception("未初始化模型");
                }
                Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
                OCRResult ocrResult = new OCRResult();
                double zoomW = actionParams["ZoomW"];
                double zoomH = actionParams["ZoomH"];
                double scale = actionParams["Scale"];
                if (actionParams["Image"] is string)
                {
                    string filePath = actionParams["Image"];
                    if (zoomW != 1.0d || zoomH != 1.0d && scale != 1.0d)
                    {
                        HObject image = new HObject();
                        HOperatorSet.ReadImage(out image, filePath);
                        HOperatorSet.ZoomImageFactor(image, out image, actionParams["ZoomW"], actionParams["ZoomH"], "constant");
                        HOperatorSet.ScaleImage(image, out image, actionParams["Scale"], 0);
                        lock (lockFile)
                        {
                            filePath = Path.Combine(Path.GetDirectoryName(filePath) + $"\\Pre_{Path.GetFileNameWithoutExtension(filePath)}.jpg");
                            HOperatorSet.WriteImage(image, "jpeg", 0, filePath);
                            //img = Utils.Bitmap2HImageHelper.HObject2Bitmap(image);
                            image.Dispose();
                        }
                    }
                    ocrResult = engine.DetectText(filePath);
                }
                else if (actionParams["Image"] is Bitmap b)
                {
                    ocrResult = engine.DetectText(b);
                }
                results["result"] = ocrResult.TextBlocks;
                results["resultStr"] = ocrResult.Text;
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
                    engine.Dispose();
                    IsInit = false;
                }
            }
            catch { }

        }
    }
}
