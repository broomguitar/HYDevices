using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;

namespace HY.Devices.Algorithm.Haier_JiaoNan
{
    /// <summary>
    /// Yolo缺陷检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class SurfaceInspectionByYolo : AbstractAlgorithm, IAlgorithm
    {
        #region Yolo 函数
        private const string YoloLibraryName = "Algorithms/YoloV4/yolo_cpp_dll.dll";
        private const int MaxObjects = 1000;

        [DllImport(YoloLibraryName, EntryPoint = "init")]
        private static extern int InitializeYolo(string configurationFilename, string weightsFilename, int gpu, int batch_size);

        [DllImport(YoloLibraryName, EntryPoint = "detect_image")]
        private static extern int DetectImage(string filename, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "detect_mat")]
        private static extern int DetectImage(IntPtr pArray, int nSize, ref BboxContainer container);

        [DllImport(YoloLibraryName, EntryPoint = "dispose")]
        private static extern int DisposeYolo();

        [StructLayout(LayoutKind.Sequential)]
        public struct bbox_t
        {
            public UInt32 x, y, w, h;    // (x,y) - top-left corner, (w, h) - width & height of bounded box
            public float prob;           // confidence - probability that the object was found correctly
            public UInt32 obj_id;        // class of object - from range [0, classes-1]
            public UInt32 track_id;      // tracking id for video (0 - untracked, 1 - inf - tracked object)
            public UInt32 frames_counter;
            public float x_3d, y_3d, z_3d;  // 3-D coordinates, if there is used 3D-stereo camera
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct BboxContainer
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxObjects)]
            public bbox_t[] candidates;
        }
        #endregion
        private static readonly object _lockObj = new object();
        private static SurfaceInspectionByYolo _instance;
        public static SurfaceInspectionByYolo Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new SurfaceInspectionByYolo();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectTarget;
        public override Dictionary<string, dynamic> InitParamNames { get; }= new Dictionary<string, dynamic> { { "cfg_Filename", "" }, { "weights_Filename", "" }, { "typeNames_Filename", "" }, { "gpu_Id", 0 }, { "batch_size", 1 } };

        public override Dictionary<string, dynamic> ActionParamNames { get; }= new Dictionary<string, dynamic> { { "Image", "" } };
        private string[] typeNames;
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                typeNames = System.IO.File.ReadAllLines(initParameters["typeNames_Filename"]);
                return IsInit = InitializeYolo(initParameters["cfg_Filename"].ToString(), initParameters["weights_Filename"], initParameters["gpu_Id"],initParameters["batch_size"]) == 1;
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
                BboxContainer bboxContainer = new BboxContainer();
                if (actionParams["Image"] is string)
                {
                     DetectImage(actionParams["Image"], ref bboxContainer);
                }
                else
                {

                    Utils.ImageHelper.BitmapToBytes(actionParams["Image"], out byte[] buffer);
                    IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                     DetectImage(ptr, buffer.Length, ref bboxContainer);
                }
                foreach (bbox_t item in bboxContainer.candidates)
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
