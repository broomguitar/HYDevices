using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
namespace HY.Devices.Algorithm.QDSMHY
{
    /// <summary>
    /// 二维码检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class LogoModel : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static LogoModel _instance;
        public static LogoModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new LogoModel();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.TemplateMatching;

        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "ImagePath", "" }, { "ModelPath", "" }, { "Row1", "0" }, { "Col1", "0" }, { "Row2", "2048" }, { "Col2", "2448" } ,{ "MinScore" ,"0.75"} };

        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> DoParam)
        {
            if (!IsInit)
            {
                throw new Exception("未初始化模型");
            }

            string Result = "NG";

            HObject ho_Image, ho_Rectangle, ho_ImageReduced;
            HObject ho_ModelContours = null, ho_TransContours = null, ho_Rectangle1 = null;
            HObject ho_ImageReduced1 = null, ho_Edges = null, ho_EmptyRegion = null;
            HObject ho_ObjectSelected = null, ho_Region = null, ho_EmptyRegionTrans = null;
            HObject ho_RegionTrans = null, ho_RegionDilationTrans = null;
            HObject ho_RegionDilation = null, ho_unionRegionsTrans = null;
            HObject ho_unionRegions = null, ho_RegionIntersection = null;
            HObject ho_RegionDifference = null, ho_RegionOpening = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;

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
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_TransContours);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Edges);
            HOperatorSet.GenEmptyObj(out ho_EmptyRegion);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_EmptyRegionTrans);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_RegionDilationTrans);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_unionRegionsTrans);
            HOperatorSet.GenEmptyObj(out ho_unionRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);

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
                HOperatorSet.GenRectangle1(out ho_Rectangle, Convert.ToInt32(DoParam["Row1"]), Convert.ToInt32(DoParam["Col1"]), Convert.ToInt32(DoParam["Row2"]), Convert.ToInt32(DoParam["Col2"]));


                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Angle.Dispose(); hv_Scale.Dispose(); hv_Score.Dispose();
                    HOperatorSet.FindScaledShapeModel(ho_ImageReduced, hv_ModelID, (new HTuple(-15)).TupleRad()
                        , (new HTuple(30)).TupleRad(), 0.8, 1.2, 0.3, 1, 0.1, "least_squares", (new HTuple(3)).TupleConcat(
                        1), 0.1, out hv_Row, out hv_Column, out hv_Angle, out hv_Scale, out hv_Score);
                }

                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Score.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    hv_HomMat2D.Dispose();
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2D);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dScale(hv_HomMat2D, hv_Scale.TupleSelect(hv_I), hv_Scale.TupleSelect(
                            hv_I), 0, 0, out ExpTmpOutVar_0);
                        hv_HomMat2D.Dispose();
                        hv_HomMat2D = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dRotate(hv_HomMat2D, hv_Angle.TupleSelect(hv_I), 0, 0,
                            out ExpTmpOutVar_0);
                        hv_HomMat2D.Dispose();
                        hv_HomMat2D = ExpTmpOutVar_0;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.HomMat2dTranslate(hv_HomMat2D, hv_Row.TupleSelect(hv_I), hv_Column.TupleSelect(
                            hv_I), out ExpTmpOutVar_0);
                        hv_HomMat2D.Dispose();
                        hv_HomMat2D = ExpTmpOutVar_0;
                    }
                    ho_ModelContours.Dispose();
                    HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);
                    ho_TransContours.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_TransContours,
                        hv_HomMat2D);
                    hv_Row12.Dispose(); hv_Column12.Dispose(); hv_Row22.Dispose(); hv_Column22.Dispose();
                    HOperatorSet.SmallestRectangle1Xld(ho_TransContours, out hv_Row12, out hv_Column12,
                        out hv_Row22, out hv_Column22);

                    hv_row1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_row1 = hv_Row12.TupleMin()
                            ;
                    }
                    hv_col1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_col1 = hv_Column12.TupleMin()
                            ;
                    }
                    hv_row2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_row2 = hv_Row22.TupleMax()
                            ;
                    }
                    hv_col2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_col2 = hv_Column22.TupleMax()
                            ;
                    }

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle1, hv_row1 - 10, hv_col1 - 10, hv_row2 + 10,
                            hv_col2 + 10);
                    }
                    ho_ImageReduced1.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle1, out ho_ImageReduced1);
                    ho_Edges.Dispose();
                    HOperatorSet.EdgesColorSubPix(ho_ImageReduced1, out ho_Edges, "canny", 2, 15,
                        20);


                    ho_EmptyRegion.Dispose();
                    HOperatorSet.GenEmptyRegion(out ho_EmptyRegion);
                    hv_Number.Dispose();
                    HOperatorSet.CountObj(ho_Edges, out hv_Number);
                    hv_Rownum.Dispose();
                    hv_Rownum = new HTuple();
                    hv_Colnum.Dispose();
                    hv_Colnum = new HTuple();
                    HTuple end_val35 = hv_Number;
                    HTuple step_val35 = 1;
                    for (hv_Index1 = 1; hv_Index1.Continue(end_val35, step_val35); hv_Index1 = hv_Index1.TupleAdd(step_val35))
                    {
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Edges, out ho_ObjectSelected, hv_Index1);
                        hv_RowAll.Dispose(); hv_ColAll.Dispose();
                        HOperatorSet.GetContourXld(ho_ObjectSelected, out hv_RowAll, out hv_ColAll);
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleConcat(hv_Rownum, hv_RowAll, out ExpTmpOutVar_0);
                            hv_Rownum.Dispose();
                            hv_Rownum = ExpTmpOutVar_0;
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleConcat(hv_Colnum, hv_ColAll, out ExpTmpOutVar_0);
                            hv_Colnum.Dispose();
                            hv_Colnum = ExpTmpOutVar_0;
                        }
                    }
                    ho_Region.Dispose();
                    HOperatorSet.GenRegionPoints(out ho_Region, hv_Rownum, hv_Colnum);


                    ho_EmptyRegionTrans.Dispose();
                    HOperatorSet.GenEmptyRegion(out ho_EmptyRegionTrans);
                    hv_Number.Dispose();
                    HOperatorSet.CountObj(ho_TransContours, out hv_Number);
                    hv_RownumTrans.Dispose();
                    hv_RownumTrans = new HTuple();
                    hv_ColnumTrans.Dispose();
                    hv_ColnumTrans = new HTuple();
                    HTuple end_val48 = hv_Number;
                    HTuple step_val48 = 1;
                    for (hv_Index1 = 1; hv_Index1.Continue(end_val48, step_val48); hv_Index1 = hv_Index1.TupleAdd(step_val48))
                    {
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_TransContours, out ho_ObjectSelected, hv_Index1);
                        hv_RowAllTrans.Dispose(); hv_ColAllTrans.Dispose();
                        HOperatorSet.GetContourXld(ho_ObjectSelected, out hv_RowAllTrans, out hv_ColAllTrans);
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleConcat(hv_RownumTrans, hv_RowAllTrans, out ExpTmpOutVar_0);
                            hv_RownumTrans.Dispose();
                            hv_RownumTrans = ExpTmpOutVar_0;
                        }
                        {
                            HTuple ExpTmpOutVar_0;
                            HOperatorSet.TupleConcat(hv_ColnumTrans, hv_ColAllTrans, out ExpTmpOutVar_0);
                            hv_ColnumTrans.Dispose();
                            hv_ColnumTrans = ExpTmpOutVar_0;
                        }
                    }
                    ho_RegionTrans.Dispose();
                    HOperatorSet.GenRegionPoints(out ho_RegionTrans, hv_RownumTrans, hv_ColnumTrans);


                    hv_factor.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_factor = hv_row2 - hv_row1;
                    }
                    //可变参数 膨胀范围
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_RegionDilationTrans.Dispose();
                        HOperatorSet.DilationRectangle1(ho_RegionTrans, out ho_RegionDilationTrans,
                            hv_factor / 12.0, hv_factor / 12.0);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_RegionDilation.Dispose();
                        HOperatorSet.DilationRectangle1(ho_Region, out ho_RegionDilation, hv_factor / 12.0,
                            hv_factor / 12.0);
                    }
                    ho_unionRegionsTrans.Dispose();
                    HOperatorSet.Union1(ho_RegionDilationTrans, out ho_unionRegionsTrans);
                    ho_unionRegions.Dispose();
                    HOperatorSet.Union1(ho_RegionDilation, out ho_unionRegions);
                    ho_RegionIntersection.Dispose();
                    HOperatorSet.Intersection(ho_unionRegionsTrans, ho_unionRegions, out ho_RegionIntersection
                        );
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_unionRegionsTrans, ho_RegionIntersection, out ho_RegionDifference
                        );

                    //可变参数 滤除杂波
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_RegionOpening.Dispose();
                        HOperatorSet.OpeningRectangle1(ho_RegionDifference, out ho_RegionOpening, hv_factor / 16.0,
                            hv_factor / 16.0);
                    }
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);

                    hv_area.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_area = (hv_factor * hv_factor) / 60.0;
                    }
                    if ((int)(new HTuple(hv_area.TupleLess(40))) != 0)
                    {
                        hv_area.Dispose();
                        hv_area = 40;
                    }
                    //可变参数，筛选面积
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", hv_area, 9999999);

                    hv_Number1.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number1);
                    if ((int)(new HTuple(hv_Number1.TupleGreater(0))) != 0)
                    {
                        hv_State.Dispose();
                        hv_State = "canque";
                    }
                    else
                    {
                        hv_State.Dispose();
                        hv_State = "OK";
                    }
                }
                if ((int)(new HTuple(hv_Score.TupleLess(hv_MinScore))) != 0)
                {
                    hv_State.Dispose();
                    hv_State = "NG";
                }
                HObject wrImage;
                HOperatorSet.GenEmptyObj(out wrImage);
                wrImage.Dispose();
                try
                {
                    try
                    {
                        HOperatorSet.PaintRegion(ho_RegionTrans, ho_Image, out wrImage, ((new HTuple(0)).TupleConcat(255)).TupleConcat(0), "fill");

                    }
                    catch(Exception ex) 
                    { 

                    }
                    try
                    {
                        HTuple Number = new HTuple();
                        HOperatorSet.CountObj(ho_SelectedRegions,out Number);
                        if(Number>0)
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.PaintRegion(ho_SelectedRegions, wrImage, out ExpTmpOutVar_0,
                                ((new HTuple(255)).TupleConcat(0)).TupleConcat(0), "fill");
                            wrImage.Dispose();
                            wrImage = ExpTmpOutVar_0;
                        }
                    }
                    catch(Exception esd)
                    {

                    }
                    FileInfo fileInfo = new FileInfo(DoParam["ImagePath"]);
                    DirectoryInfo dr = fileInfo.Directory;
                    HOperatorSet.WriteImage(wrImage, "jpeg", 0, dr.FullName  + "\\" + fileInfo.Name.Substring(0, dr.FullName.Length - 4) + "_rec.jpg");

                }
                catch(Exception eed)
                {

                }
                
                //检测
                Result = hv_State.S;
            }
            catch(Exception ex)
            {

            }


            ho_Image.Dispose();
            ho_Rectangle.Dispose();
            ho_ImageReduced.Dispose();
            ho_ModelContours.Dispose();
            ho_TransContours.Dispose();
            ho_Rectangle1.Dispose();
            ho_ImageReduced1.Dispose();
            ho_Edges.Dispose();
            ho_EmptyRegion.Dispose();
            ho_ObjectSelected.Dispose();
            ho_Region.Dispose();
            ho_EmptyRegionTrans.Dispose();
            ho_RegionTrans.Dispose();
            ho_RegionDilationTrans.Dispose();
            ho_RegionDilation.Dispose();
            ho_unionRegionsTrans.Dispose();
            ho_unionRegions.Dispose();
            ho_RegionIntersection.Dispose();
            ho_RegionDifference.Dispose();
            ho_RegionOpening.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();

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

