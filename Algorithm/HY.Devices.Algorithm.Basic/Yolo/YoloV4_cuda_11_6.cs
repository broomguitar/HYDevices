using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Basic.Yolo
{
    /// <summary>
    /// Yolo缺陷检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class YoloV4_cuda_11_6 : AbstractAlgorithm, IAlgorithm
    {
        #region Yolo_GPU 函数
        private const string dllPath = @".\Algorithms\yolov4_cuda11.6\yolo_cpp_dll.dll";

        [DllImport(dllPath, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(dllPath, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref YoloModel.BboxContainer container);

        [DllImport(dllPath, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int nSize, ref YoloModel.BboxContainer container);

        [DllImport(dllPath, EntryPoint = "dispose")]
        private static extern int DisposeYolo();
        #endregion
        private static readonly object _lockObj = new object();
        private static YoloV4_cuda_11_6 _instance;
        public static YoloV4_cuda_11_6 Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new YoloV4_cuda_11_6();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectTarget;
        //cfg/coco.data cfg/yolov4.cfg yolov4.weights
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic> { { "YoloCfgFile", @"TestModel\yolov4.cfg" }, { "YoloWeightsFile", @"TestModel\yolov4.weights" }, { "YoloDataFile", @"TestModel\coco.names" }, { "gpu_Id","0" }, { "batch_size", "1" } };

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" },{"Prob","0.8"} };
        private string[] typeNames;
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                typeNames = System.IO.File.ReadAllLines(initParameters["YoloDataFile"]);
                return IsInit = InitializeYolo(initParameters["YoloCfgFile"], initParameters["YoloWeightsFile"], initParameters["gpu_Id"], initParameters["batch_size"]) == 1;
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
                List<TargetResult> quexianResultInfos = new List<TargetResult>();
                YoloModel.BboxContainer bboxContainer = new YoloModel.BboxContainer();
                int count = 0;
                if (actionParams["Image"] is string)
                {
                    count = DetectImage(actionParams["Image"], ref bboxContainer);
                }
                else
                {

                    Utils.ImageHelper.BitmapToBytes(actionParams["Image"], out byte[] buffer);
                    IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    count = DetectImage(ptr, buffer.Length, ref bboxContainer);
                }
                for (int i = 0; i < count; i++)
                {
                    YoloModel.bbox_t item = bboxContainer.candidates[i];
                    if (item.prob >= actionParams["Prob"])
                    {
                        TargetResult deepResult = new TargetResult();
                        deepResult.Row1 = item.y;
                        deepResult.Column1 = item.x;
                        deepResult.Row2 = item.y + item.h;
                        deepResult.Column2 = item.x + item.w;
                        deepResult.Score = item.prob;
                        deepResult.TypeName = typeNames[item.obj_id];
                        quexianResultInfos.Add(deepResult);
                    }
                }
                results.Add("result", quexianResultInfos);
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
                    DisposeYolo();
                    IsInit = false;
                }
            }
            catch { }

        }
    }
}
