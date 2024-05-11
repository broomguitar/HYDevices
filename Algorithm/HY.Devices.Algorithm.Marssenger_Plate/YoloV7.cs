using HalconDotNet;
using HY.Devices.Algorithm.Marssenger_Plate.CS;
using Microsoft.ML.OnnxRuntime;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using SixLabors.ImageSharp.Formats;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using SixLabors.ImageSharp.Processing;

namespace HY.Devices.Algorithm.Marssenger_Plate
{
    internal class YoloV7
    {
    }
    /// <summary>
    /// 新门齐算法
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class DoorAlign : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static DoorAlign _instance;
        public static DoorAlign Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new DoorAlign();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectDoorAlign;
        int inputWidth;
        int inputHeight;
        double minConfidence = 0.7;
        string inputName;
        InferenceSession session;
        string[] Labels;

        //    public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>() { { "ModelPath", "" } , { "Labels", new[] {
        //        "Foot_Margin_Gray",
        //        "Upper_Reinforcer1",
        //        "308_Without_Front_Board",
        //        "Break_Line1",
        //        "Break_Line2",
        //        "Positive_feature1",
        //        "Positive_feature2",
        //        "Positive_feature3",
        //        "Positive_feature4",
        //        "Positive_feature5",
        //        "Screw1",
        //        "Screw2",
        //        "Small_bowl1",
        //        "Small_bowl2",
        //        "Under_Reinforcer1",
        //        "Under_Reinforcer2"
        //                                                    }
        //} };
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>() { { "ModelPath", "" } };

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            session = new InferenceSession(initParameters["ModelPath"]);
            var inputDimensions = session.InputMetadata.ToList()[0].Value.Dimensions.ToList();
            //Labels = initParameters["Labels"];
            Labels = new[] {
            "Foot_Margin_Gray",
            "Upper_Reinforcer1",
            "308_Without_Front_Board",
            "Break_Line1",
            "Break_Line2",
            "Positive_feature1",
            "Positive_feature2",
            "Positive_feature3",
            "Positive_feature4",
            "Positive_feature5",
            "Screw1",
            "Screw2",
            "Small_bowl1",
            "Small_bowl2",
            "Under_Reinforcer1",
            "Under_Reinforcer2"
                                                        
    };
            ////yolov7 输入提取
            inputWidth = inputDimensions[2];
            inputHeight = inputDimensions[3];
            inputName = session.InputNames[0];
            IsInit = true;
            return true;
        }
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            Dictionary<string, dynamic> retunrnResults = new Dictionary<string, dynamic>();
            try
            {
                List<Prediction> deepResult = new List<Prediction>();
                HObject hoimage = new HObject();
                HTuple width = new HTuple();
                HTuple height = new HTuple();
                hoimage.Dispose();
                hoimage = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);


                HOperatorSet.GetImageSize(hoimage, out width, out height);
                DateTime t1 = DateTime.Now;
                DateTime t2;
                DateTime t3;
                using (SixLabors.ImageSharp.Image<Rgb24> image = SixLabors.ImageSharp.Image.Load<Rgb24>(actionParams["Image"], out IImageFormat format))
                {
                    using (Stream imageStream = new MemoryStream())
                    {
                        #region classfine
                        image.Mutate(x =>
                        {
                            x.Resize(new ResizeOptions
                            {
                                Size = new SixLabors.ImageSharp.Size(inputHeight, inputWidth),
                                Mode = SixLabors.ImageSharp.Processing.ResizeMode.Stretch
                            }); ;
                        });


                        //image.Save(imageStream, format);
                        Microsoft.ML.OnnxRuntime.Tensors.Tensor<float> input = new Microsoft.ML.OnnxRuntime.Tensors.DenseTensor<float>(new[] { 1, 3, inputWidth, inputHeight });
                        var mean = new[] { 0.485f, 0.456f, 0.406f };
                        var stddev = new[] { 0.229f, 0.224f, 0.225f };
                        image.ProcessPixelRows(accessor =>
                        {
                            for (int y = 0; y < accessor.Height - 1; y++)
                            {
                                Span<Rgb24> pixelSpan = accessor.GetRowSpan(y);
                                for (int x = 0; x < accessor.Width - 1; x++)
                                {
                                    input[0, 0, y, x] = (pixelSpan[x].R / 255f);
                                    input[0, 1, y, x] = (pixelSpan[x].G / 255f);
                                    input[0, 2, y, x] = (pixelSpan[x].B / 255f);
                                }
                            }
                        });


                        var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor(inputName, input) };

                        #endregion

                        t2 = DateTime.Now;
                        TimeSpan ts1 = t2 - t1;
                        Console.WriteLine("预处理耗时：" + ts1.TotalMilliseconds);
                        #region  Demo解析
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        using (IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs))
                        {
                            Console.WriteLine("推理耗时：" + stopwatch.ElapsedMilliseconds);
                            t3 = DateTime.Now;
                            TimeSpan ts2 = t3 - t2;
                            //Console.WriteLine("推理耗时：" + ts2.TotalMilliseconds);

                            #region rnnfast/yolov7解析
                            var resultsArray = results.ToArray();
                            float[] revalue = resultsArray[0].AsEnumerable<float>().ToArray();

                            
                            for (int i = 0; i < revalue.Length; i = i + 7)
                            {
                                if (revalue[i + 6] >= 0)
                                {
                                    deepResult.Add(new Prediction
                                    {
                                        Box = new Box(revalue[i + 1], revalue[i + 2], revalue[i + 3], revalue[i + 4]),
                                        Label = Labels[Convert.ToInt32(revalue[i + 5])],
                                        Confidence = revalue[i + 6]
                                    });
                                }
                            }
                            retunrnResults.Add("Result", deepResult);
                            
                            //var preNMS = ImageTool.Supress(deepResult, 0.1F);
                            //foreach (Prediction prediction in preNMS)
                            //{
                            //    HObject rec = new HObject();
                            //    //HOperatorSet.GenRectangle1(out rec, prediction.Box.Ymin / 640 * height, prediction.Box.Xmin / 640 * width, prediction.Box.Ymax / 640 * height, prediction.Box.Xmax / 640 * width);
                            //    HOperatorSet.GenRectangle1(out rec, prediction.Box.Ymin / inputHeight * height, prediction.Box.Xmin / inputWidth * width, prediction.Box.Ymax / inputHeight * height, prediction.Box.Xmax / inputWidth * width);
                            //    //HOperatorSet.GenRectangle1(out rec, prediction.Box.Ymin, prediction.Box.Xmin, prediction.Box.Ymax, prediction.Box.Xmax);
                            //    HOperatorSet.DispObj(rec, showImage.HalconWindow);

                            //    Console.WriteLine(prediction.Label + "   " + prediction.Confidence);
                            //}

                            #endregion

                        }
                        #endregion


                    }
                }
                return retunrnResults;
            }
            catch (Exception ex)
            {
                return retunrnResults;
                //throw ex;
            }
            finally
            {
            }
        }
        public override void UnInit()
        {
            session.Dispose();
            GC.Collect();
        }
    }
}
