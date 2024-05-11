
using HalconDotNet;
using HY.Devices.Algorithm;
using HY.Devices.Algorithm.Yolov7.CS;
using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Windows.Media;
using Brush = System.Drawing.Brush;
using Brushes = System.Drawing.Brushes;
using Pen = System.Drawing.Pen;

namespace HY.Devices.Algorithm.Yolov7.YoloV7
{
    public enum PreProcessFunc
    {
        Stretch,
        FillAndStretch
    }
    [Export(typeof(IAlgorithm))]
    public class YoloV7 : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static YoloV7 _instance;
        public static YoloV7 Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new YoloV7();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectTarget;
        int inputWidth;
        int inputHeight;
        double minConfidence = 0.7;
        string inputName;
        InferenceSession session;
        string[] Labels;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>() { { "ModelPath", "" }, { "LabelPath", "" } };

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            int gpuDeviceId = 0;
            session = new InferenceSession(initParameters["ModelPath"], SessionOptions.MakeSessionOptionWithCudaProvider(gpuDeviceId));
            var inputDimensions = session.InputMetadata.ToList()[0].Value.Dimensions.ToList();
            Labels = File.ReadAllLines(initParameters["LabelPath"]);
            ////yolov7 输入提取
            inputWidth = inputDimensions[2];
            inputHeight = inputDimensions[3];
            inputName = session.InputNames[0];
            Dictionary<string, dynamic> actionParams = new Dictionary<string, dynamic> { { "Image", Path.Combine(SaveDir, "logo.jpg") } };
            DoAction(actionParams);
            IsInit = true;
            return true;
        }
        private  object obj = new object();

        protected  List<Prediction> Detect(string filePath,Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc> inputtuple)
        {
            List<Prediction> listp = new List<Prediction>();
            int width = inputtuple.Item2;
            int height = inputtuple.Item3;
            PreProcessFunc prep = inputtuple.Item4;
            Bitmap bitmap = new Bitmap(filePath);
            Graphics g = Graphics.FromImage(bitmap);
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


                        listp.Add(new Prediction
                        {
                            Box = new Box((float)column1, (float)row1, (float)column2, (float)row2),
                            Label = Labels[Convert.ToInt32(revalue[i + 5])],
                            Confidence = revalue[i + 6]
                        });
         
                    }
                }
             
                DrawLinesOnImage(filePath, listp, prep, width, height);
            }
            return listp;
        }
        public Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc> ProcessByHObject(HObject image, PreProcessFunc pre)
        {
            HObject hoimageZoomed = new HObject();
            HTuple imageWith, imageHeight;
            HOperatorSet.GetImageSize(image, out imageWith, out imageHeight);
            if (pre == PreProcessFunc.Stretch)
            {
                HOperatorSet.ZoomImageSize(image, out hoimageZoomed, inputWidth, inputHeight, "constant");
            }
            else if (pre == PreProcessFunc.FillAndStretch)
            {
                hoimageZoomed = PreProcess.ZoomAndFillImage(image, inputWidth, inputHeight, 114);
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

            return new Tuple<List<NamedOnnxValue>, int, int, PreProcessFunc>(inputs, imageWith, imageHeight, pre);
        }

        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            Dictionary<string, dynamic> retunrnResults = new Dictionary<string, dynamic>();
            HObject hoimage = new HObject();
            try
            {
                List<Prediction> deepResult = new List<Prediction>();
                HOperatorSet.ReadImage(out hoimage, actionParams["Image"]);
                deepResult = Detect(actionParams["Image"],ProcessByHObject(hoimage, PreProcessFunc.FillAndStretch));
                retunrnResults.Add("result", deepResult);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                hoimage.Dispose();
            }
            return retunrnResults;
        }

        private static Brush[] brushes = new Brush[] { Brushes.Red, Brushes.Green, Brushes.Blue, Brushes.Yellow, Brushes.Gray, Brushes.AliceBlue };
        private void DrawLinesOnImage(string imgPath, List<Prediction> results, PreProcessFunc prep, float width , float height)
        {
            try
            {
                Bitmap bitmap = new Bitmap(imgPath);
                Graphics g = Graphics.FromImage(bitmap);
                int index = 0;
                var preNMS = ImageTool.Supress(results, 0.1F);
                foreach (var item in results.GroupBy(a => a.Label))
                {
                    var brush = brushes[index++ % 6];
                    Pen pen = new System.Drawing.Pen(brush, 3);
                    foreach (var prediction in item)
                    {
                        g.DrawRectangle(pen, new Rectangle((int)prediction.Box.Xmin, (int)prediction.Box.Ymin, (int)(prediction.Box.Xmax - prediction.Box.Xmin), (int)(prediction.Box.Ymax - prediction.Box.Ymin)));
                        //g.DrawRectangle(new Pen(brush, 3), column1, row1, column2 - column1, row2 - row1);
                        g.DrawString($"{prediction.Label}--{prediction.Confidence}", new Font("微软雅黑", 16, System.Drawing.FontStyle.Bold), brush, (int)prediction.Box.Xmin, (int)prediction.Box.Ymin);
                    }
                }
                string path = Path.Combine(Path.GetDirectoryName(imgPath) + $"\\{Path.GetFileNameWithoutExtension(imgPath)}_Ret.jpg");
                ImageHelper.SaveImage(bitmap, path, 75);
                g.Dispose();
                bitmap.Dispose();
            }
            catch (Exception ex)
            {
            }
        }

       
        public override void UnInit()
        {
            if (session != null)
            {
                session.Dispose();
            }
            GC.Collect();
        }
    }
}
