using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using HalconDotNet;
using System.Runtime.InteropServices;
using System.Drawing;

namespace HY.Devices.Algorithm.Basic.Onnx
{
    public abstract class OnnxDetector: AbstractAlgorithm, IAlgorithm
    {

        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectTarget;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic> { { "ModelPath", @"TestModel\yolo.Onnx" }, { "TypePath", @"TestModel\type.txt" }};
        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image",""},{ "PreFunc", "" } };
        protected string[] Labels;

        public InferenceSession session;
        public int inputWidth;
        public int inputHeight;
        public string inputName;
        #region 初始化
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                Labels = (System.IO.File.ReadAllLines(initParameters["TypePath"]));
                return IsInit = InitModel(initParameters["ModelPath"]);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected virtual bool InitModel(string modelPath)
        {
            try
            {
                int gpuDeviceId = 0; // The GPU device ID to execute on
                session = new InferenceSession(modelPath, SessionOptions.MakeSessionOptionWithCudaProvider(gpuDeviceId));
                var inputDimensions = session.InputMetadata.ToList()[0].Value.Dimensions.ToList();
                inputWidth = inputDimensions[2];
                inputHeight = inputDimensions[3];
                inputName = session.InputNames[0];
                
                return true;
            }
            catch(Exception ep)
            {
                return false;
            }
            
        }
        #endregion
        #region 释放模型
        public override void UnInit()
        {
            try
            {
                if (IsInit)
                {
                    DisposeModel();
                    IsInit = false;
                }
            }
            catch { }
        }
        public bool DisposeModel()
        {
            try
            {
                session.Dispose();
                return true;
            }
            catch(Exception ex) 
            { 
                return false; 
            }
        }
        #endregion

        protected virtual List<Prediction> Detect(Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc> input)
        {
            return new List<Prediction>();
        }
        protected virtual List<Prediction> Detect(Tuple<List<NamedOnnxValue>, int, int,PreProcessFunc> input, HWindow hw)
        {
            return new List<Prediction>();
        }

        #region 归一化
        public delegate HObject PreProcess(HObject image, List<string> prePram);

        public enum PreProcessFunc
        {
            Stretch,
            FillAndStretch
        }
        public Tuple<List<NamedOnnxValue>, int,int, PreProcessFunc> ProcessByHObject(HObject image, PreProcessFunc pre)
        {
            HObject hoimageZoomed = new HObject();
            HTuple imageWith, imageHeight;
            HOperatorSet.GetImageSize(image, out imageWith,out imageHeight);
            if (pre == PreProcessFunc.Stretch)
            {
                HOperatorSet.ZoomImageSize(image, out hoimageZoomed, inputWidth, inputHeight, "constant");
            }
            else if(pre == PreProcessFunc.FillAndStretch)
            {
                hoimageZoomed = ZoomAndFillImage(image, inputWidth, inputHeight, 114);
            }
            HOperatorSet.GetImagePointer3(hoimageZoomed, out HTuple pointerRed, out HTuple pointerGreen, out HTuple pointerBlue, out HTuple type, out HTuple width, out HTuple height);

            byte[] by = new byte[width * height * 3];
            Marshal.Copy(pointerRed, by, 0, width * height);
            Marshal.Copy(pointerGreen, by, width * height, width * height);
            Marshal.Copy(pointerBlue, by, width * height * 2, width * height);
            float[] fimage = new float[width * height * 3];

            for (int y = 0; y < by.Length - 1; y++)
            {
                fimage[y] = (int)by[y] / (float)255.0;
            }
            Tensor<float> data = new DenseTensor<float>(fimage, new[] { 1, 3, inputWidth, inputHeight });
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, data) };

            return new Tuple<List<NamedOnnxValue>, int, int,PreProcessFunc>(inputs, imageWith, imageHeight, pre);
        }
        public Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc> ProcessByHObject(HObject image, PreProcess pre, List<string> prePram)
        {
            HObject hoimageZoomed = new HObject();
            HTuple imageWith, imageHeight;
            HOperatorSet.GetImageSize(image, out imageWith, out imageHeight);
            hoimageZoomed = pre(image, prePram);
            HOperatorSet.GetImagePointer3(hoimageZoomed, out HTuple pointerRed, out HTuple pointerGreen, out HTuple pointerBlue, out HTuple type, out HTuple width, out HTuple height);

            byte[] by = new byte[width * height * 3];
            Marshal.Copy(pointerRed, by, 0, width * height);
            Marshal.Copy(pointerGreen, by, width * height, width * height);
            Marshal.Copy(pointerBlue, by, width * height * 2, width * height);
            float[] fimage = new float[width * height * 3];

            for (int y = 0; y < by.Length - 1; y++)
            {
                fimage[y] = (int)by[y] / (float)255.0;
            }
            Tensor<float> data = new DenseTensor<float>(fimage, new[] { 1, 3, inputWidth, inputHeight });
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, data) };
            return new Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc>(inputs, imageWith, imageHeight,PreProcessFunc.Stretch);
        }
        #endregion
        #region 预处理方法
        private HObject ZoomAndFillImage(HObject imaga, int width, int height, int gray)
        {


            // Local iconic variables 

            HObject ho_Image, ho_Rectangle = null, ho_Rectangle11 = null;
            HObject ho_ImageZoomed, ho_ImageEmpty, ho_ImageR, ho_ImageG;
            HObject ho_ImageB, ho_MultiImage, ho_DupImage, ho_ImageAffineTrans;
            HObject ho_ImageReduced, ho_Rectangle1, ho_ImageResult1;

            // Local control variables 

            HTuple hv_Height = new HTuple(), hv_Width = new HTuple();
            HTuple hv_targetGray = new HTuple(), hv_Width1 = new HTuple();
            HTuple hv_Height1 = new HTuple(), hv_factor = new HTuple();
            HTuple hv_HomMat2DIdentity = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle11);
            HOperatorSet.GenEmptyObj(out ho_ImageZoomed);
            HOperatorSet.GenEmptyObj(out ho_ImageEmpty);
            HOperatorSet.GenEmptyObj(out ho_ImageR);
            HOperatorSet.GenEmptyObj(out ho_ImageG);
            HOperatorSet.GenEmptyObj(out ho_ImageB);
            HOperatorSet.GenEmptyObj(out ho_MultiImage);
            HOperatorSet.GenEmptyObj(out ho_DupImage);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageResult1);


            hv_Height.Dispose();
            hv_Height = height;
            hv_Width.Dispose();
            hv_Width = width;
            hv_targetGray.Dispose();
            hv_targetGray = gray;

            ho_Image.Dispose();
            ho_Image = imaga.Clone();

            hv_Width1.Dispose(); hv_Height1.Dispose();
            HOperatorSet.GetImageSize(ho_Image, out hv_Width1, out hv_Height1);

            if ((int)(new HTuple((((float)hv_Width1 / hv_Width)).TupleGreater((float)hv_Height1 / hv_Height))) != 0)
            {
                hv_factor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_factor = (double)hv_Width / (double)hv_Width1;
                }
                hv_HomMat2DIdentity.Dispose();
                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, (hv_Height - (hv_Height1 * hv_factor)) / 2,
                        0, out ExpTmpOutVar_0);
                    hv_HomMat2DIdentity.Dispose();
                    hv_HomMat2DIdentity = ExpTmpOutVar_0;
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, (hv_Height - (hv_Height1 * hv_factor)) / 2,
                        hv_Width);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle11.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle11, (hv_Height - (hv_Height1 * hv_factor)) / 2,
                        0, (hv_Height + (hv_Height1 * hv_factor)) / 2, hv_Width);
                }
            }
            else
            {
                hv_factor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_factor = (double)hv_Height / (double)hv_Height1;
                }

                hv_HomMat2DIdentity.Dispose();
                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, 0, (hv_Width - (hv_Width1 * hv_factor)) / 2,
                        out ExpTmpOutVar_0);
                    hv_HomMat2DIdentity.Dispose();
                    hv_HomMat2DIdentity = ExpTmpOutVar_0;
                }

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_Height, (hv_Width - (hv_Width1 * hv_factor)) / 2);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle11.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle11, 0, (hv_Width - (hv_Width1 * hv_factor)) / 2,
                        hv_Height, (hv_Width + (hv_Width1 * hv_factor)) / 2);
                }
            }

            ho_ImageZoomed.Dispose();
            HOperatorSet.ZoomImageFactor(ho_Image, out ho_ImageZoomed, hv_factor, hv_factor,
                "constant");


            ho_ImageEmpty.Dispose();
            HOperatorSet.GenImageConst(out ho_ImageEmpty, "byte", hv_Width, hv_Height);

            ho_ImageR.Dispose();
            HOperatorSet.GenImageProto(ho_ImageEmpty, out ho_ImageR, 0);
            ho_ImageG.Dispose();
            HOperatorSet.GenImageProto(ho_ImageEmpty, out ho_ImageG, 0);
            ho_ImageB.Dispose();
            HOperatorSet.GenImageProto(ho_ImageEmpty, out ho_ImageB, 0);
            ho_MultiImage.Dispose();
            HOperatorSet.Compose3(ho_ImageR, ho_ImageG, ho_ImageB, out ho_MultiImage);

            ho_DupImage.Dispose();
            HOperatorSet.CopyImage(ho_MultiImage, out ho_DupImage);

            HOperatorSet.OverpaintGray(ho_MultiImage, ho_ImageZoomed);
            ho_ImageAffineTrans.Dispose();
            HOperatorSet.AffineTransImage(ho_MultiImage, out ho_ImageAffineTrans, hv_HomMat2DIdentity,
                "constant", "false");
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageAffineTrans, ho_Rectangle11, out ho_ImageReduced
                );



            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_Rectangle1.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle1, 0, 0, hv_Height - 1, hv_Width - 1);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_ImageResult1.Dispose();
                HOperatorSet.PaintRegion(ho_Rectangle1, ho_DupImage, out ho_ImageResult1, ((hv_targetGray.TupleConcat(
                    hv_targetGray))).TupleConcat(hv_targetGray), "fill");
            }

            HOperatorSet.OverpaintGray(ho_ImageResult1, ho_ImageReduced);


            //compose3 (ImageResult1, ImageResult2, ImageResult3, ImageResult)



            ho_Image.Dispose();
            ho_Rectangle.Dispose();
            ho_Rectangle11.Dispose();
            ho_ImageZoomed.Dispose();
            ho_ImageEmpty.Dispose();
            ho_ImageR.Dispose();
            ho_ImageG.Dispose();
            ho_ImageB.Dispose();
            ho_MultiImage.Dispose();
            ho_DupImage.Dispose();
            ho_ImageAffineTrans.Dispose();
            //ho_ImageReduced.Dispose();
            ho_Rectangle1.Dispose();
            ho_ImageResult1.Dispose();

            hv_Height.Dispose();
            hv_Width.Dispose();
            hv_targetGray.Dispose();
            hv_Width1.Dispose();
            hv_Height1.Dispose();
            hv_factor.Dispose();
            hv_HomMat2DIdentity.Dispose();

            return ho_ImageReduced;
        }
        #endregion
        

    }
    public interface Prediction
    { 

    }
    internal class DetectProperty
    {
        public string Name { get; set; }
        public float Value { get; set; }
    }
    public class Box
    {
        public Box(float xMin, float yMin, float xMax, float yMax)
        {
            Xmin = xMin;
            Ymin = yMin;
            Xmax = xMax;
            Ymax = yMax;
        }
        public float Xmin { set; get; }
        public float Xmax { set; get; }
        public float Ymin { set; get; }
        public float Ymax { set; get; }
    }
    

}
