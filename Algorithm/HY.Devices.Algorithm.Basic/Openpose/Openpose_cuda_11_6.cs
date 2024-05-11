using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Basic.Openpose
{
    [Export(typeof(IAlgorithm))]
    public class Openpose_cuda_11_6 : AbstractAlgorithm, IAlgorithm
    {
        #region Openpose
        private const string dllPath = @".\Algorithms\openpose_cuda11.6\03_keypoints_from_image.dll";
        //{0, “Nose”},{ 1, “Neck”},{ 2, “RShoulder”},{ 3, “RElbow”},{ 4, “RWrist”},{ 5, “LShoulder”},{ 6, “LElbow”},
        //{ 7, “LWrist”},{ 8, “MidHip”},{ 9, “RHip”},{ 10, “RKnee”},{ 11, “RAnkle”},{ 12, “LHip”},{ 13, “LKnee”},
        //{ 14, “LAnkle”},{ 15, “REye”},{ 16, “LEye”},{ 17, “REar”},{ 18, “LEar”},{ 19, “LBigToe”},{ 20, “LSmallToe”},
        //{ 21, “LHeel”},{ 22, “RBigToe”},{ 23, “RSmallToe”},{ 24, “RHeel”}
        [DllImport(dllPath, EntryPoint = "Init", CallingConvention = CallingConvention.Cdecl)]
        extern static bool init();
        [DllImport(dllPath, EntryPoint = "DetectImageFileAPI", CallingConvention = CallingConvention.Cdecl)]
        extern static int ImageFile(string imagefile, ref OpenposeModel.OpenposeResult result);
        [DllImport(dllPath, EntryPoint = "DetectImageVideoAPI", CallingConvention = CallingConvention.Cdecl)]
        extern static IntPtr ImageVideo(int deviceIndex, OpenposeResultCallback openposeResultCallback);
        [DllImport(dllPath, EntryPoint = "DetectImageBufferAPI", CallingConvention = CallingConvention.Cdecl)]
        extern static int ImageBuffer(IntPtr pArray, int nSize, ref OpenposeModel.OpenposeResult result);
        [DllImport(dllPath, EntryPoint = "UnInit", CallingConvention = CallingConvention.Cdecl)]
        extern static bool unInit();
        public delegate void OpenposeResultCallback(ref OpenposeModel.OpenposeResult result);
        #endregion
        private static readonly object _lockObj = new object();
        private static Openpose_cuda_11_6 _instance;
        public static Openpose_cuda_11_6 Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new Openpose_cuda_11_6();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.ActionRecognition;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();
        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                return IsInit = init();
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
                Dictionary<string, object> results = new Dictionary<string, object>();
                OpenposeModel.OpenposeResult result = new OpenposeModel.OpenposeResult();
                int data = 0;
                if (actionParams["Image"] is string)
                {
                    data = ImageFile(actionParams["Image"], ref result);
                }
                else
                {

                    Utils.ImageHelper.BitmapToBytes(actionParams["Image"], out byte[] buffer);
                    IntPtr ptr = Marshal.UnsafeAddrOfPinnedArrayElement(buffer, 0);
                    data = ImageBuffer(ptr, buffer.Length, ref result);
                }
                List<OpenposeModel.BodyPoint> bodyPoints = new List<OpenposeModel.BodyPoint>();
                if (data > 0)
                {
                    int count = data / 25;
                    for (int i = 0; i < count; i++)
                    {
                      bodyPoints.Add(new OpenposeModel.BodyPoint(result.points.Skip(i * 25).Take(25).ToArray()));
                    }
                }
                results.Add("result", bodyPoints);
                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
