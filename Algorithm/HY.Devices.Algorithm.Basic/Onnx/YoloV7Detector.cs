using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using HalconDotNet;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Diagnostics;
using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel.Composition;

namespace HY.Devices.Algorithm.Basic.Onnx
{
    [Export(typeof(IAlgorithm))]
    public class YoloV7Detector : OnnxDetector
    {
        #region 结果
        #endregion

        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            return base.Init(initParameters);
        }
        #region rnnfast/yolov7解析
        protected override List<Prediction> Detect(Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc> inputtuple)
        {
            List<Prediction> listp = new List<Prediction>();
            int width = inputtuple.Item2;
            int height = inputtuple.Item3;
            PreProcessFunc prep = inputtuple.Item4;

            using (IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputtuple.Item1))
            {

                var resultsArray = results.ToArray();
                float[] revalue = resultsArray[0].AsEnumerable<float>().ToArray();

                for (int i = 0; i < revalue.Length; i = i + 7)
                {
                    if (revalue[i + 6] >= 0)
                    {
                        Box temp = new Box(revalue[i + 1], revalue[i + 2], revalue[i + 3], revalue[i + 4]);
                        double row1 = 0, column1 = 0, row2 = 0, column2 = 0;
                        if (prep == PreProcessFunc.FillAndStretch)
                        {
                            double factor = 0;
                            if ((double)width / inputWidth > (double)height / inputHeight)
                            {
                                factor = (double)inputWidth / (double)width;
                                row1 = (temp.Ymin - (inputHeight - (height * factor)) / 2) / (height * factor) * height;
                                column1 = temp.Xmin / inputWidth * width;
                                row2 = (temp.Ymax - (inputHeight - (height * factor)) / 2) / (height * factor) * height;
                                column2 = temp.Xmax / inputWidth * width;
                            }
                            else
                            {
                                factor = (double)inputHeight / (double)height;
                                row2 = temp.Ymax / inputHeight * height;
                                column2 = ((temp.Xmax - (inputWidth - (width * factor)) / 2) / (width * factor) * width);
                                row1 = temp.Ymin / inputHeight * height;
                                column1 = (temp.Xmin - (inputWidth - (width * factor)) / 2) / (width * factor) * width;
                            }
                        }
                        else if (prep == PreProcessFunc.Stretch)
                        {
                            row1 = temp.Ymin / inputHeight * height;
                            column1 = temp.Xmin / inputWidth * width;
                            row2 = temp.Ymax / inputHeight * height;
                            column2 = temp.Xmax / inputWidth * width;
                        }


                        listp.Add(new YoloV7Prediction
                        {
                            Box = new Box((float)column1, (float)row1, (float)column2, (float)row2),
                            Label = Labels[Convert.ToInt32(revalue[i + 5])],
                            Confidence = revalue[i + 6]
                        });
                    }
                }

            }
            return listp;
        }

        protected override List<Prediction> Detect(Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc> inputtuple, HWindow hw)
        {

            List<Prediction> listp = new List<Prediction>();
            int width = inputtuple.Item2;
            int height = inputtuple.Item3;
            PreProcessFunc prep = inputtuple.Item4;
            using (IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputtuple.Item1))
            {

                var resultsArray = results.ToArray();
                float[] revalue = resultsArray[0].AsEnumerable<float>().ToArray();

                for (int i = 0; i < revalue.Length; i = i + 7)
                {
                    if (revalue[i + 6] >= 0)
                    {
                        Box temp = new Box(revalue[i + 1], revalue[i + 2], revalue[i + 3], revalue[i + 4]);
                        double row1 = 0, column1 = 0, row2 = 0, column2 = 0;
                        if (prep == PreProcessFunc.FillAndStretch)
                        {
                            double factor = 0;
                            if ((double)width / inputWidth > (double)height / inputHeight)
                            {
                                factor = (double)inputWidth / (double)width;
                                row1 = (temp.Ymin - (inputHeight - (height * factor)) / 2) / (height * factor) * height;
                                column1 = temp.Xmin / inputWidth * width;
                                row2 = (temp.Ymax - (inputHeight - (height * factor)) / 2) / (height * factor) * height;
                                column2 = temp.Xmax / inputWidth * width;
                            }
                            else
                            {
                                factor = (double)inputHeight / (double)height;
                                row2 = temp.Ymax / inputHeight * height;
                                column2 = ((temp.Xmax - (inputWidth - (width * factor)) / 2) / (width * factor) * width);
                                row1 = temp.Ymin / inputHeight * height;
                                column1 = (temp.Xmin - (inputWidth - (width * factor)) / 2) / (width * factor) * width;
                            }
                        }
                        else if (prep == PreProcessFunc.Stretch)
                        {
                            row1 = temp.Ymin / inputHeight * height;
                            column1 = temp.Xmin / inputWidth * width;
                            row2 = temp.Ymax / inputHeight * height;
                            column2 = temp.Xmax / inputWidth * width;
                        }


                        listp.Add(new YoloV7Prediction
                        {
                            Box = new Box((float)column1, (float)row1, (float)column2, (float)row2),
                            Label = Labels[Convert.ToInt32(revalue[i + 5])],
                            Confidence = revalue[i + 6]
                        });
                    }
                }
            }
            HOperatorSet.SetLineWidth(hw, 4);
            foreach (YoloV7Prediction prediction in listp)
            {

                HObject rec = new HObject();
                if (prediction.Confidence > 0.5)
                    HOperatorSet.GenRectangle1(out rec, prediction.Box.Ymin, prediction.Box.Xmin, prediction.Box.Ymax, prediction.Box.Xmax);
                HOperatorSet.DispObj(rec, hw);
                rec.Dispose();
                //Console.WriteLine(prediction.Label + "   " + prediction.Confidence);
            }

            return listp;
        }
        public void ShowResult(HObject im, List<Prediction> listp, HWindow hw, int Width, int Height, PreProcessFunc prep)
        {
            HOperatorSet.SetLineWidth(hw, 2);
            HOperatorSet.SetColor(hw, "yellow");
            foreach (YoloV7Prediction prediction in listp)
            {
                HObject rec = new HObject();
                if (prediction.Confidence > 0.5)
                {
                    HOperatorSet.GenRectangle1(out rec, prediction.Box.Ymin, prediction.Box.Xmin, prediction.Box.Ymax, prediction.Box.Xmax);
                    HOperatorSet.DispObj(rec, hw);

                    HOperatorSet.SetTposition(hw, prediction.Box.Ymin, prediction.Box.Xmin);
                    HOperatorSet.SetColor(hw, "red");
                    HOperatorSet.SetFont(hw, "default-Normal-35");
                    HOperatorSet.WriteString(hw, prediction.Label.ToString());
                }
                rec.Dispose();
                //Console.WriteLine(prediction.Label + "   " + prediction.Confidence);
            }

        }
        #endregion

        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            try
            {
                Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
                List<Prediction> pl = new List<Prediction>();
                if (!IsInit)
                {
                    throw new Exception("未初始化模型");
                }
                Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();

                int count = 0;
                HObject ho_Image = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
                if (actionParams["PreFunc"] == "Fill")
                {
                    OnnxDetector.PreProcessFunc pre = OnnxDetector.PreProcessFunc.Stretch;
                }
                else
                {
                    OnnxDetector.PreProcessFunc pre = OnnxDetector.PreProcessFunc.FillAndStretch;
                }

                pl = Detect(ProcessByHObject(ho_Image, OnnxDetector.PreProcessFunc.FillAndStretch));
                result.Add("result", pl);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }

    public class YoloV7Prediction : Prediction
    {
        public Box Box { get; set; }
        public string Label { get; set; }
        public float Confidence { get; set; }
    }

}
