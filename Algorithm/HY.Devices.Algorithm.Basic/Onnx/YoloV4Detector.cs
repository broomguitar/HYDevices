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
using System.Drawing;
using System.ComponentModel.Composition;

namespace HY.Devices.Algorithm.Basic.Onnx
{
    [Export(typeof(IAlgorithm))]
    public class YoloV4Detector : OnnxDetector
    {
        #region 结果

        #endregion
        protected override bool InitModel(string modelPath)
        {
            return base.InitModel(modelPath);
        }
        #region rnnfast/YoloV4解析
        protected override List<Prediction> Detect(Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc> inputtuple)
        {
            List<Prediction> listp = new List<Prediction>();
            int width = inputtuple.Item2;
            int height = inputtuple.Item3;
            PreProcessFunc prep = inputtuple.Item4;

            int outputNum = session.OutputMetadata.ToList()[0].Value.Dimensions[1];
            int classNum = session.OutputMetadata.ToList()[1].Value.Dimensions[2];

            using (IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputtuple.Item1))
            {

                var resultsArray = results.ToArray();
                float[] boxvalue = resultsArray[0].AsEnumerable<float>().ToArray();
                float[] classvalue = resultsArray[1].AsEnumerable<float>().ToArray();
                for (int i = 0; i < outputNum; i++)
                {
                    int index = 0;
                    float conf = 0;
                    for (int I = 0; I < classNum; I++)
                    {
                        if (conf < classvalue[i * classNum + I])
                        {
                            conf = classvalue[i * classNum + I];
                            index = I;
                        }
                    }

                    //Box temp = new Box(boxvalue[i * 4] , boxvalue[i * 4 + 1] , boxvalue[i * 4 + 2] , boxvalue[i * 4 + 3] );
                    if (conf >= 0.5)
                    {
                        Box temp = new Box(boxvalue[i * 4] * inputWidth, boxvalue[i * 4 + 1] * inputHeight, boxvalue[i * 4 + 2] * inputWidth, boxvalue[i * 4 + 3] * inputHeight);
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


                        listp.Add(new YoloV4Prediction
                        {
                            Box = new Box((float)column1, (float)row1, (float)column2, (float)row2),
                            Label = Labels[index],
                            Confidence = conf
                        });
                    }
                }

            }
            List<Prediction> listpNMS = Supress(listp, 0.5f);
            return listpNMS;
        }

        protected override List<Prediction> Detect(Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc> inputtuple, HWindow hw)
        {

            List<Prediction> listp = new List<Prediction>();
            int width = inputtuple.Item2;
            int height = inputtuple.Item3;
            PreProcessFunc prep = inputtuple.Item4;

            int outputNum = session.OutputMetadata.ToList()[0].Value.Dimensions[1];
            int classNum = session.OutputMetadata.ToList()[1].Value.Dimensions[2];

            using (IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputtuple.Item1))
            {

                #region 原处理
                var resultsArray = results.ToArray();
                float[] boxvalue = resultsArray[0].AsEnumerable<float>().ToArray();
                float[] classvalue = resultsArray[1].AsEnumerable<float>().ToArray();
                for (int i = 0; i < outputNum; i++)
                {
                    int index = 0;
                    float conf = 0;
                    for (int I = 0;I < classNum; I++)
                    {
                       if(conf< classvalue[i * classNum + I])
                        {
                            conf = classvalue[i * classNum + I];
                            index = I;
                        }
                    }

                    //Box temp = new Box(boxvalue[i * 4] , boxvalue[i * 4 + 1] , boxvalue[i * 4 + 2] , boxvalue[i * 4 + 3] );
                    if (conf >= 0.5)
                    {
                        Box temp = new Box(boxvalue[i * 4] * inputWidth, boxvalue[i * 4 + 1] * inputHeight, boxvalue[i * 4 + 2] * inputWidth, boxvalue[i * 4 + 3] * inputHeight);
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


                        listp.Add(new YoloV4Prediction
                        {
                            Box = new Box((float)column1, (float)row1, (float)column2, (float)row2),
                            Label = Labels[index],
                            Confidence = conf
                        });
                    }
                }
                #endregion
            }
            List<Prediction> listpNMS = Supress(listp,0.5f);
            HOperatorSet.SetLineWidth(hw, 1);
            foreach (Prediction predictiontem in listpNMS)
            {
                YoloV4Prediction prediction = predictiontem as YoloV4Prediction;
                Console.WriteLine(prediction.Box.Xmin.ToString() + "  " + prediction.Box.Ymin.ToString() + "  " + prediction.Box.Xmax.ToString() + "  " + prediction.Box.Ymax.ToString() + "  " + prediction.Confidence.ToString());
                HObject rec = new HObject();
                if (prediction.Confidence > 0)
                {
                    HOperatorSet.GenRectangle1(out rec, prediction.Box.Ymin, prediction.Box.Xmin, prediction.Box.Ymax, prediction.Box.Xmax);
                    HOperatorSet.DispObj(rec, hw);
                }
                rec.Dispose();
                //Console.WriteLine(prediction.Label + "   " + prediction.Confidence);
            }

            return listpNMS;
        }
        private static List<Prediction> Supress(List<Prediction> items, float Standardverlop)
        {
            List<Prediction> result = new List<Prediction>(items);

            foreach (var item in items) // iterate every prediction
            {
                YoloV4Prediction tempItem = item as YoloV4Prediction;
                foreach (var current in result.ToList()) // make a copy for each iteration
                {
                    YoloV4Prediction currentItem = current as YoloV4Prediction;
                    if (current == item) continue;

                    //var (rect1, rect2) = (item.Box, current.Box);
                    var rect1 = RectangleF.FromLTRB(tempItem.Box.Xmin, tempItem.Box.Ymin, tempItem.Box.Xmax, tempItem.Box.Ymax);
                    var rect2 = RectangleF.FromLTRB(currentItem.Box.Xmin, currentItem.Box.Ymin, currentItem.Box.Xmax, currentItem.Box.Ymax);
                    RectangleF intersection = RectangleF.Intersect(rect1, rect2);

                    float intArea = intersection.Width * intersection.Height; // intersection area
                    float unionArea = rect1.Width * rect1.Height + rect2.Width * rect2.Height - intArea; // union area
                    float overlap = intArea / unionArea; // overlap ratio

                    if (overlap >= Standardverlop)
                    {
                        if (tempItem.Confidence >= currentItem.Confidence)
                        {
                            result.Remove(current);
                        }
                    }
                }
            }
            return result;
        }
        public void ShowResult(HObject im, List<Prediction> listp, HWindow hw, int Width, int Height, PreProcessFunc prep)
        {
            HOperatorSet.SetLineWidth(hw, 1);
            HOperatorSet.SetColor(hw, "yellow");
            foreach (YoloV4Prediction prediction in listp)
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
                HObject ho_Image=Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
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
    public class YoloV4Prediction : Prediction
    {
        public Box Box { get; set; }
        public string Label { get; set; }
        public float Confidence { get; set; }
    }

}
