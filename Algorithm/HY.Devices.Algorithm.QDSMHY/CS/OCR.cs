using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Security.AccessControl;
using System.Security.Cryptography;
using HY.Devices.Algorithm;
using PaddleOCR;
using System.IO;
using PaddleOCR.Onnx;


namespace HY.Devices.Algorithm.QDSMHY
{
    //字符识别
    [Export(typeof(IAlgorithm))]
    public class OCR : AbstractAlgorithm, IAlgorithm
    {
        private PaddleOCREngine engine;
        private static readonly object _lockObj = new object();
        private static OCR _instance;
        public static OCR Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new OCR();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectTarget;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic> { };

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "ImagePath", "" } };
        public override bool Init(Dictionary<string, dynamic> InitParam)
        {
            try
            {
                OCRModelConfig config;
                OCRParameter oCRParameter = new OCRParameter();

                string root = Environment.CurrentDirectory;
                config = new OCRModelConfig();
                string modelPathroot = root + @"\inferencev4";
                config.det_infer = modelPathroot + @"\ch_PP-OCRv4_det_infer.onnx";
                config.rec_infer = modelPathroot + @"\ch_PP-OCRv4_rec_infer.onnx";
                config.cls_infer = modelPathroot + @"\ch_ppocr_mobile_v2.0_cls_infer.onnx";
                config.keys = modelPathroot + @"\ppocr_keys.txt";


                engine = new PaddleOCREngine(config, oCRParameter);

                return IsInit = true;
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
                engine.Dispose();
            }
            catch (Exception DS)
            {
                Console.WriteLine(DS);
            }
        }
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> DoParam)
        {
            if (!IsInit)
            {
                throw new Exception("未初始化模型");
            }
            byte[] imagebyte = File.ReadAllBytes(DoParam["ImagePath"]);
            OCRResult ocrResult;
            ocrResult = engine.DetectText(imagebyte);
            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
            results.Add("result", ocrResult.Text);
            return results;
        }
    }
}
