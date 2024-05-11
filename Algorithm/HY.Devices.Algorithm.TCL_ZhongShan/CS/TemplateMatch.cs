using HalconDotNet;
using System;
using System.Drawing;

namespace HY.Devices.Algorithm.TCL_ZhongShan
{
    /// <summary>
    /// 模板匹配
    /// </summary>
    public class TemplateMatch
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
        private bool TemplateMatch_shm(HObject hb_Image, string shmFilePath, double MinScore, out double Score)
        {
            Score = 0;
            bool Result = false;
            HObject ho_Image, ho_GrayImage, ho_ImageScaleMax1;
            //HObject ho_Image, ho_GrayImage, ho_ImageScaleMax1; ;
            HTuple hv_Result = null;// hv_ModelID = null;
            //HTuple hv_Row = null, hv_Column = null, hv_Angle = null;
            //HTuple hv_ScaleRow = null, hv_ScaleColumn = null, hv_Score = null, hv_Scale = null;
            HTuple hv_ModelID = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Score = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_ImageScaleMax1);

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
                ho_ImageScaleMax1.Dispose();
                HOperatorSet.ScaleImageMax(ho_GrayImage, out ho_ImageScaleMax1);
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Angle.Dispose(); hv_Scale.Dispose(); hv_Score.Dispose();
                HOperatorSet.FindScaledShapeModel(ho_ImageScaleMax1, hv_ModelID, -0.79, 0.78,
                    0.5, 1.5, 0.3, 1, 0, "least_squares", 6, 0.9, out hv_Row, out hv_Column,
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

                ho_ImageScaleMax1.Dispose();

                hv_ModelID.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Angle.Dispose();
                hv_Scale.Dispose();
            }
            return Result;
        }
        public bool TemplateMatch_shm(string ImagePath, string shmFilePath, double MinScore, out double Score)
        {
            try
            {
                HOperatorSet.ReadImage(out HObject ho_Image, ImagePath);
                return TemplateMatch_shm(ho_Image, shmFilePath, MinScore, out Score);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public bool TemplateMatch_shm(Bitmap bitmap, string shmFilePath, double MinScore, out double Score)
        {
            try
            {
                HObject ho_Image = Utils.Ho_ImageHelper.Bitmap2HObject(bitmap);
                return TemplateMatch_shm(ho_Image, shmFilePath, MinScore, out Score);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

    }
}