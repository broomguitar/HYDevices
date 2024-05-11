using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
namespace HY.Devices.Algorithm.QDSMHY
{
    /// <summary>
    /// 二维码检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class ShapeModel : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static ShapeModel _instance;
        public static ShapeModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new ShapeModel();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.TemplateMatching;

        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "ImagePath", "" }, { "ModelPath", "" } ,{ "Row1", "0" }, { "Col1", "0" } ,{ "Row2", "2048" } ,{ "Col2", "2448" }, { "MinScore", "0.75" } };

        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> DoParam)
        {
            if (!IsInit)
            {
                throw new Exception("未初始化模型");
            }

            string Result = "NG";

            HObject ho_Image, ho_Rectangle, ho_ImageReduced;
            HObject ho_Rectangle1 = null;
            HObject ho_ImageReduced1 = null;

            // Local control variables 

            HTuple hv_MinScore = new HTuple(), hv_ModelID = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Angle = new HTuple(), hv_Scale = new HTuple();
            HTuple hv_Score = new HTuple(), hv_I = new HTuple(), hv_HomMat2D = new HTuple();
            HTuple hv_Row12 = new HTuple(), hv_Column12 = new HTuple();
            HTuple hv_Row22 = new HTuple(), hv_Column22 = new HTuple();
            HTuple hv_row1 = new HTuple(), hv_col1 = new HTuple();
            HTuple hv_row2 = new HTuple(), hv_col2 = new HTuple();
            HTuple hv_Number = new HTuple(), hv_Rownum = new HTuple();
            HTuple hv_Colnum = new HTuple(), hv_Index1 = new HTuple();
            HTuple hv_RowAll = new HTuple(), hv_ColAll = new HTuple();
            HTuple hv_RownumTrans = new HTuple(), hv_ColnumTrans = new HTuple();
            HTuple hv_RowAllTrans = new HTuple(), hv_ColAllTrans = new HTuple();
            HTuple hv_factor = new HTuple(), hv_area = new HTuple();
            HTuple hv_Number1 = new HTuple(), hv_State = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);


            try
            {
                //检测
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, DoParam["ImagePath"]);
                hv_ModelID.Dispose();
                HOperatorSet.ReadShapeModel(DoParam["ModelPath"], out hv_ModelID);
                hv_MinScore.Dispose();
                hv_MinScore = Convert.ToDouble(DoParam["MinScore"]);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle,Convert.ToInt32( DoParam["Row1"]), Convert.ToInt32(DoParam["Col1"]), Convert.ToInt32(DoParam["Row2"]), Convert.ToInt32(DoParam["Col2"]));



                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Angle.Dispose(); hv_Scale.Dispose(); hv_Score.Dispose();
                    HOperatorSet.FindScaledShapeModel(ho_ImageReduced, hv_ModelID, (new HTuple(-15)).TupleRad()
                        , (new HTuple(30)).TupleRad(), 0.8, 1.2, 0.3, 1, 0.1, "least_squares", (new HTuple(3)).TupleConcat(
                        1), 0.1, out hv_Row, out hv_Column, out hv_Angle, out hv_Scale, out hv_Score);
                }

                if ((int)(new HTuple(hv_Score.TupleLess(hv_MinScore))) != 0)
                {
                    hv_State.Dispose();
                    hv_State = "NG";
                }
                else
                {
                    hv_State.Dispose();
                    hv_State = "OK";
                }

                //HObject wrImage;
                //HOperatorSet.GenEmptyObj(out wrImage);
                //wrImage.Dispose();
                //HOperatorSet.PaintRegion(ho_RegionTrans, ho_Image, out wrImage, (
                //    (new HTuple(0)).TupleConcat(255)).TupleConcat(0), "fill");


                //检测
                Result = hv_State.S;
            }
            catch (Exception ex)
            {

            }


            ho_Image.Dispose();
            ho_Rectangle.Dispose();
            ho_ImageReduced.Dispose();
            ho_Rectangle1.Dispose();
            ho_ImageReduced1.Dispose();

            hv_MinScore.Dispose();
            hv_ModelID.Dispose();
            hv_Row.Dispose();
            hv_Column.Dispose();
            hv_Angle.Dispose();
            hv_Scale.Dispose();
            hv_Score.Dispose();
            hv_I.Dispose();
            hv_HomMat2D.Dispose();
            hv_Row12.Dispose();
            hv_Column12.Dispose();
            hv_Row22.Dispose();
            hv_Column22.Dispose();
            hv_row1.Dispose();
            hv_col1.Dispose();
            hv_row2.Dispose();
            hv_col2.Dispose();
            hv_Number.Dispose();
            hv_Rownum.Dispose();
            hv_Colnum.Dispose();
            hv_Index1.Dispose();
            hv_RowAll.Dispose();
            hv_ColAll.Dispose();
            hv_RownumTrans.Dispose();
            hv_ColnumTrans.Dispose();
            hv_RowAllTrans.Dispose();
            hv_ColAllTrans.Dispose();
            hv_factor.Dispose();
            hv_area.Dispose();
            hv_Number1.Dispose();
            hv_State.Dispose();

            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
            results.Add("Result", Result);
            return results;

        }

        public override bool Init(Dictionary<string, object> initParameters)
        {

            return IsInit = true;
        }

        public override void UnInit()
        {
            IsInit = false;
        }
    }
}

