using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Paste.CS
{
    /// <summary>
    /// 模板匹配
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class TemplateMatch : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static TemplateMatch _instance;
        public static TemplateMatch Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new TemplateMatch();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.TemplateMatching;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();
        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" }, { "ModelFile", "" }, { "minScore", 0 } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                return IsInit = true;
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
                bool b = false;
                double Score = double.NaN;
                if (System.IO.Path.GetExtension(actionParams["ModelFile"]) == ".ncm")
                {
                    HObject ho_Image = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
                    b = TemplateMatch_ncm(ho_Image, actionParams["ModelFile"], actionParams["minScore"], 1, out Score);
                }
                else if (System.IO.Path.GetExtension(actionParams["ModelFile"]) == ".shm")
                {
                    HObject ho_Image = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
                    b = TemplateMatch_shm(ho_Image, actionParams["ModelFile"], actionParams["minScore"], out Score);
                }
                results.Add("result", b);
                results.Add("Score", Score);
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
                IsInit = false;
            }
            catch { }

        }
        private bool TemplateMatch_shm(HObject hb_Image, string shmFilePath, double MinScore, out double Score)
        {
            Score = 0;
            bool Result = false;
            HObject ho_Image, ho_Image1, ho_Image2, ho_Image3, ho_GrayImage;
            HTuple hv_Result = null, hv_ModelID = null;
            HTuple hv_Row = null, hv_Column = null, hv_Angle = null;
            HTuple hv_ScaleRow = null, hv_ScaleColumn = null, hv_Score = null, hv_Scale = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            try
            {
                ho_Image.Dispose();
                // HOperatorSet.ReadImage(out ho_Image, "D:/项目/87 老板电器/烟机新图/23/CXW-200-8329/pic16-27-24.jpeg");
                HOperatorSet.CopyImage(hb_Image, out ho_Image);
                hv_Result = "NG";
                HOperatorSet.ReadShapeModel(shmFilePath, out hv_ModelID);
                //ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                //HOperatorSet.Decompose3(ho_Image, out ho_Image1, out ho_Image2, out ho_Image3);
                //HOperatorSet.FindAnisoShapeModel(ho_Image2, hv_ModelID, (new HTuple(0)).TupleRad()
                //    , (new HTuple(360)).TupleRad(), 0.9, 1.1, 0.9, 1.1, 0.35, 1, 0.5, (new HTuple("least_squares")).TupleConcat(
                //    "max_deformation 10"), (new HTuple(8)).TupleConcat(3), 0.6, out hv_Row, out hv_Column,
                //    out hv_Angle, out hv_ScaleRow, out hv_ScaleColumn, out hv_Score);
                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                HOperatorSet.FindScaledShapeModel(ho_GrayImage, hv_ModelID, -0.39, 0.78, 0.9,
                1.1, 0.3, 1, 0, "least_squares", 4, 1, out hv_Row, out hv_Column,
                out hv_Angle, out hv_Scale, out hv_Score);


                HOperatorSet.ClearShapeModel(hv_ModelID);

                if ((int)(new HTuple(hv_Score.TupleNotEqual(new HTuple()))) != 0)
                {
                    Score = hv_Score.D;
                    Result = Score > MinScore;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ho_Image.Dispose();
                ho_Image1.Dispose();
                ho_GrayImage.Dispose();
                ho_Image2.Dispose();
                ho_Image3.Dispose();
            }
            return Result;
        }
        private bool TemplateMatch_ncm(HObject hb_Image, string _ModelPath, double MinScore, double Scale, out double Score)
        {
            bool Result = false;
            Score = 0;
            HObject ho_Image, ho_ModelContours, ho_GrayImage = null;

            // Local control variables 

            HTuple hv_ModelID = null, hv_Result = null;
            HTuple hv_NumLevels = null, hv_AngleStart = null, hv_AngleExtent = null;
            HTuple hv_AngleStep = null, hv_ScaleMin = null, hv_ScaleMax = null;
            HTuple hv_ScaleStep = null, hv_Metric = null, hv_MinContrast = null;
            HTuple hv_Channels = null, hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Angle = new HTuple(), hv_Scale = new HTuple();
            HTuple hv_Score = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            try
            {
                ho_Image.Dispose();
                // HOperatorSet.ReadImage(out ho_Image, "D:/项目/106 重庆滚筒四码合一/30/EG100B209S/CE0JP300100PKMASJ2Q8/显示板-17-09-37.jpeg");
                HOperatorSet.CopyImage(hb_Image, out ho_Image);
                HOperatorSet.ReadShapeModel(_ModelPath, out hv_ModelID);

                hv_Result = "NG";


                HOperatorSet.GetShapeModelParams(hv_ModelID, out hv_NumLevels, out hv_AngleStart,
                    out hv_AngleExtent, out hv_AngleStep, out hv_ScaleMin, out hv_ScaleMax, out hv_ScaleStep,
                    out hv_Metric, out hv_MinContrast);
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);

                HOperatorSet.CountChannels(ho_Image, out hv_Channels);
                if ((int)(new HTuple(hv_Channels.TupleNotEqual(1))) != 0)
                {
                    ho_GrayImage.Dispose();
                    HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                    HOperatorSet.ScaleImage(ho_GrayImage, out ho_GrayImage, Scale, 0);
                    HOperatorSet.FindScaledShapeModel(ho_GrayImage, hv_ModelID, hv_AngleStart,
                        hv_AngleExtent, hv_ScaleMin, hv_ScaleMax, 0.3, 1, 0, "least_squares",
                        hv_NumLevels, 0.9, out hv_Row, out hv_Column, out hv_Angle, out hv_Scale,
                        out hv_Score);
                }
                else
                {
                    HOperatorSet.FindScaledShapeModel(ho_Image, hv_ModelID, hv_AngleStart, hv_AngleExtent,
                        hv_ScaleMin, hv_ScaleMax, 0.3, 1, 0, "least_squares", hv_NumLevels, 0.9,
                        out hv_Row, out hv_Column, out hv_Angle, out hv_Scale, out hv_Score);
                }

                HOperatorSet.ClearShapeModel(hv_ModelID);

                if ((int)(new HTuple(hv_Score.TupleNotEqual(new HTuple()))) != 0)
                {
                    Score = hv_Score.D;
                    Result = Score >= MinScore;
                    hv_Result = Result ? "OK" : "NG";
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {

                ho_Image.Dispose();
                ho_ModelContours.Dispose();
                ho_GrayImage.Dispose();
            }

            return Result;
        }
    }
}
