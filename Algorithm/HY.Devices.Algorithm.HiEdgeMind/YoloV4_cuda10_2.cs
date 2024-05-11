using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.HiEdgeMind
{
    /// <summary>
    /// Yolo缺陷检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class YoloV4_cuda10_2 : AbstractAlgorithm, IAlgorithm
    {
        #region Yolo_GPU 函数
        private const string dllPath = @".\yolov4_cuda10.2\yolo_cpp_dll.dll";

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
        private static YoloV4_cuda10_2 _instance;
        public static YoloV4_cuda10_2 Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new YoloV4_cuda10_2();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectTarget;
        //cfg/coco.data cfg/yolov4.cfg yolov4.weights
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic> { { "cfg_Filename", @"TestModel\yolov4.cfg" }, { "weights_Filename", @"TestModel\yolov4.weights" }, { "typeNames_Filename", @"TestModel\coco.names" }, { "gpu_Id", 0 }, { "batch_size", 1 } };

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" } };
        private string[] typeNames;
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                typeNames = System.IO.File.ReadAllLines(initParameters["typeNames_Filename"]);
                return IsInit = InitializeYolo(initParameters["cfg_Filename"], initParameters["weights_Filename"], initParameters["gpu_Id"], initParameters["batch_size"]) == 1;
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
                    TargetResult deepResult = new TargetResult();
                    deepResult.Row1 = item.y;
                    deepResult.Column1 = item.x;
                    deepResult.Row2 = item.y + item.h;
                    deepResult.Column2 = item.x + item.w;
                    deepResult.Score = item.prob;
                    deepResult.TypeName = typeNames[item.obj_id];
                    quexianResultInfos.Add(deepResult);
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
