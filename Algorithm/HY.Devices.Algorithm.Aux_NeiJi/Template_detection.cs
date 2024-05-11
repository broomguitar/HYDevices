using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HY.Devices.Algorithm.Aux_NeiJi
{
    /// <summary>
    /// 模板匹配
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class Template_detection : AbstractAlgorithm, IAlgorithm
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
        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" }, { "ModelPath", "" } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                IsInit = true;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        //--------------------------------顶部左右------------------------------------------------------
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParameters)
        {

            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            
            // Local iconic variables 

            HObject ho_Image, ho_ImageMean, ho_ImageScaled;
            HObject ho_GrayImage, ho_Rectangle, ho_Region, ho_ConnectedRegions;
            HObject ho_SelectedRegions, ho_RegionOpening, ho_RegionFillUp;
            HObject ho_ImageReduced, ho_Contours, ho_ContoursSplit;
            HObject ho_RegressContours, ho_UnionContours, ho_SortedContours;
            HObject ho_SelectedContours, ho_SelectedXLD, ho_RegionLines;
            HObject ho_ModelContours, ho_ModelTrans = null, ho_Region_label = null;
            HObject ho_RegionUnion = null, ho_Rectangle1 = null, ho_ObjectSelected = null;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Mean = new HTuple(), hv_Deviation = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_RowBegin = new HTuple(), hv_ColBegin = new HTuple();
            HTuple hv_RowEnd = new HTuple(), hv_ColEnd = new HTuple();
            HTuple hv_Nr = new HTuple(), hv_Nc = new HTuple(), hv_Dist = new HTuple();
            HTuple hv_RowBeginOut2 = new HTuple(), hv_ColBeginOut2 = new HTuple();
            HTuple hv_RowEndOut2 = new HTuple(), hv_ColEndOut2 = new HTuple();
            HTuple hv_rowa = new HTuple(), hv_cola = new HTuple();
            HTuple hv_rowb = new HTuple(), hv_colb = new HTuple();
            HTuple hv_ModelID1 = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Score = new HTuple();
            HTuple hv_result_concat = new HTuple(), hv_message = new HTuple();
            HTuple hv_I = new HTuple(), hv_HomMat2DIdentity = new HTuple();
            HTuple hv_HomMat2DTranslate = new HTuple(), hv_HomMat2DRotate = new HTuple();
            HTuple hv_HomMat2DScale = new HTuple(), hv_Row5 = new HTuple();
            HTuple hv_Column5 = new HTuple(), hv_Phi3 = new HTuple();
            HTuple hv_Length12 = new HTuple(), hv_Length22 = new HTuple();
            HTuple hv_Angle1 = new HTuple(), hv_relative_deg = new HTuple();
            HTuple hv_relative_deg_abs = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageMean);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
            HOperatorSet.GenEmptyObj(out ho_RegressContours);
            HOperatorSet.GenEmptyObj(out ho_UnionContours);
            HOperatorSet.GenEmptyObj(out ho_SortedContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_ModelTrans);
            HOperatorSet.GenEmptyObj(out ho_Region_label);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            //20231231 基准线选择V2,主要选额最好在同一个面
            //20240101 创建模板和检测模板分开

            //list_files ('E:/a_maweiwei_project/nb_aux_jiayong_data/2023_12_20_20231215/20231226/顶部右相机', 'files', Files)
            //* for f :=0 to |Files|-1 by 1
            try
            {
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, actionParameters["Image"]);
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                ho_ImageMean.Dispose();
                HOperatorSet.MeanImage(ho_Image, out ho_ImageMean, 7, 7);
                ho_ImageScaled.Dispose();
                HOperatorSet.ScaleImage(ho_ImageMean, out ho_ImageScaled, 1, 0);

                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(ho_ImageScaled, out ho_GrayImage);

                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_Height, hv_Width);
                hv_Mean.Dispose(); hv_Deviation.Dispose();
                HOperatorSet.Intensity(ho_Rectangle, ho_Image, out hv_Mean, out hv_Deviation);
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_GrayImage, out ho_Region, hv_Mean, 255);


                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                    70);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningRectangle1(ho_SelectedRegions, out ho_RegionOpening, hv_Width / 200,
                        hv_Height / 200);
                }
                //closing_rectangle1 (SelectedRegions, RegionClosing, Width/500, Height/500)

                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionOpening, out ho_RegionFillUp);
                hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                HOperatorSet.SmallestRectangle1(ho_RegionFillUp, out hv_Row1, out hv_Column1,
                    out hv_Row2, out hv_Column2);
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionOpening, out ho_ImageReduced
                    );


                ho_Contours.Dispose();
                HOperatorSet.GenContourRegionXld(ho_SelectedRegions, out ho_Contours, "border");
                ho_ContoursSplit.Dispose();
                HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines", 5,
                    4, 2);
                ho_RegressContours.Dispose();
                HOperatorSet.RegressContoursXld(ho_ContoursSplit, out ho_RegressContours, "no",
                    1);
                ho_UnionContours.Dispose();
                HOperatorSet.UnionCollinearContoursXld(ho_RegressContours, out ho_UnionContours,
                    10, 1, 2, 0.1, "attr_keep");
                ho_SortedContours.Dispose();
                HOperatorSet.SortContoursXld(ho_UnionContours, out ho_SortedContours, "upper_left",
                    "true", "column");
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_SelectedContours.Dispose();
                    HOperatorSet.SelectContoursXld(ho_SortedContours, out ho_SelectedContours, "direction",
                        (new HTuple(-30)).TupleRad(), (new HTuple(20)).TupleRad(), -0.5, 0.5);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_SelectedXLD.Dispose();
                    HOperatorSet.SelectShapeXld(ho_SelectedContours, out ho_SelectedXLD, "row", "and",
                        hv_Height / 10, hv_Height);
                }

                hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                HOperatorSet.FitLineContourXld(ho_SelectedXLD, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc, out hv_Dist);
                hv_RowBeginOut2.Dispose(); hv_ColBeginOut2.Dispose(); hv_RowEndOut2.Dispose(); hv_ColEndOut2.Dispose();
                HOperatorSet.SelectLinesLongest(hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd,
                    1, out hv_RowBeginOut2, out hv_ColBeginOut2, out hv_RowEndOut2, out hv_ColEndOut2);
                ho_RegionLines.Dispose();
                HOperatorSet.GenRegionLine(out ho_RegionLines, hv_RowBeginOut2, hv_ColBeginOut2,
                    hv_RowEndOut2, hv_ColEndOut2);


                //20231230 基准线优化，选择水平的
                hv_rowa.Dispose();
                hv_rowa = new HTuple(hv_RowBeginOut2);
                hv_cola.Dispose();
                hv_cola = new HTuple(hv_ColBeginOut2);
                hv_rowb.Dispose();
                hv_rowb = new HTuple(hv_RowEndOut2);
                hv_colb.Dispose();
                hv_colb = new HTuple(hv_ColEndOut2);


                //开始检测
                hv_ModelID1.Dispose();
                HOperatorSet.ReadShapeModel(actionParameters["ModelPath"],
                    out hv_ModelID1);
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID1, 1);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Angle.Dispose(); hv_Scale.Dispose(); hv_Score.Dispose();
                    HOperatorSet.FindScaledShapeModel(ho_GrayImage, hv_ModelID1, -((new HTuple(180)).TupleRad()
                        ), (new HTuple(360)).TupleRad(), 0.7, 1.3, 0.2, 1, 0.3, "none", (new HTuple(5)).TupleConcat(
                        1), 0.7, out hv_Row, out hv_Column, out hv_Angle, out hv_Scale, out hv_Score);
                }
                //dev_get_window (WindowHandle)
                //如果没有匹配的模板不需要计算角度

                //返回的结果汇总
                hv_result_concat.Dispose();
                hv_result_concat = new HTuple();
                if ((int)(new HTuple((new HTuple(hv_Score.TupleLength())).TupleEqual(0))) != 0)
                {
                    hv_message.Dispose();
                    hv_message = "No Lable";
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleConcat(hv_result_concat, hv_Score, out ExpTmpOutVar_0);
                        hv_result_concat.Dispose();
                        hv_result_concat = ExpTmpOutVar_0;
                    }
                    //dev_display (Image)
                    //disp_message (WindowHandle, message, 'window', 50, 50, 'red', 'true')
                    //stop ()
                }
                else
                {
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Score.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                    {
                        hv_HomMat2DIdentity.Dispose();
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_HomMat2DTranslate.Dispose();
                            HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_Row.TupleSelect(hv_I),
                                hv_Column.TupleSelect(hv_I), out hv_HomMat2DTranslate);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_HomMat2DRotate.Dispose();
                            HOperatorSet.HomMat2dRotate(hv_HomMat2DTranslate, hv_Angle.TupleSelect(hv_I),
                                hv_Row.TupleSelect(hv_I), hv_Column.TupleSelect(hv_I), out hv_HomMat2DRotate);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_HomMat2DScale.Dispose();
                            HOperatorSet.HomMat2dScale(hv_HomMat2DRotate, hv_Scale.TupleSelect(hv_I),
                                hv_Scale.TupleSelect(hv_I), hv_Row.TupleSelect(hv_I), hv_Column.TupleSelect(
                                hv_I), out hv_HomMat2DScale);
                        }
                        ho_ModelTrans.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ModelTrans, hv_HomMat2DScale);

                        ho_Region_label.Dispose();
                        HOperatorSet.GenRegionContourXld(ho_ModelTrans, out ho_Region_label, "margin");
                        ho_RegionUnion.Dispose();
                        HOperatorSet.Union1(ho_Region_label, out ho_RegionUnion);
                        hv_Row5.Dispose(); hv_Column5.Dispose(); hv_Phi3.Dispose(); hv_Length12.Dispose(); hv_Length22.Dispose();
                        HOperatorSet.SmallestRectangle2(ho_RegionUnion, out hv_Row5, out hv_Column5,
                            out hv_Phi3, out hv_Length12, out hv_Length22);

                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row5, hv_Column5, hv_Phi3,
                            hv_Length12, hv_Length22);
                        ho_ImageReduced.Dispose();
                        HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle1, out ho_ImageReduced);

                        //确保选择水平基线
                        ho_Contours.Dispose();
                        HOperatorSet.GenContourRegionXld(ho_ImageReduced, out ho_Contours, "border");
                        ho_ContoursSplit.Dispose();
                        HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines",
                            5, 4, 2);
                        ho_SelectedContours.Dispose();
                        HOperatorSet.SelectContoursXld(ho_ContoursSplit, out ho_SelectedContours,
                            "contour_length", 50, 90000, -0.5, 0.5);
                        ho_SortedContours.Dispose();
                        HOperatorSet.SortContoursXld(ho_SelectedContours, out ho_SortedContours,
                            "lower_left", "true", "row");
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected, 1);
                        hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                        HOperatorSet.FitLineContourXld(ho_ObjectSelected, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                            out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                            out hv_Dist);

                        //计算相对偏移角度
                        hv_Angle1.Dispose();
                        HOperatorSet.AngleLl(hv_rowa, hv_cola, hv_rowb, hv_colb, hv_RowBegin, hv_ColBegin,
                            hv_RowEnd, hv_ColEnd, out hv_Angle1);
                        hv_relative_deg.Dispose();
                        HOperatorSet.TupleDeg(hv_Angle1, out hv_relative_deg);
                        hv_relative_deg_abs.Dispose();
                        HOperatorSet.TupleAbs(hv_relative_deg, out hv_relative_deg_abs);
                        if ((int)(new HTuple(hv_relative_deg_abs.TupleGreater(90))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_relative_deg_abs = 180 - hv_relative_deg_abs;
                                    hv_relative_deg_abs.Dispose();
                                    hv_relative_deg_abs = ExpTmpLocalVar_relative_deg_abs;
                                }
                            }
                        }
                        hv_result_concat.Dispose();
                        HOperatorSet.TupleConcat(hv_Score, hv_relative_deg_abs, out hv_result_concat);
                        hv_message.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_message = new HTuple("OK Label, Relative_deg = ") + hv_relative_deg_abs;
                        }
                        if ((int)(new HTuple(hv_relative_deg_abs.TupleGreaterEqual(10))) != 0)
                        {
                            hv_message.Dispose();
                            hv_message = new HTuple("OK Label , but relative_deg more than 10 ");
                        }
                    }
                    //dev_set_draw ('margin')
                    //dev_display (Image)
                    //dev_display (RegionLines)
                    //dev_display (Rectangle1)
                    //*          disp_message (WindowHandle, message, 'window', 50, 50, 'red', 'true')
                    //stop ()
                }
                //* endfor
                HOperatorSet.ClearShapeModel(hv_ModelID1);
                if (hv_result_concat.Length != 0)
                {
                    result.Add("Score", hv_result_concat.DArr[0]);
                    result.Add("Relative_deg", hv_result_concat.DArr[1]);
                }
                else
                {
                    result.Add("Score", 0.0);
                    result.Add("Relative_deg", 0.0);
                }

                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ho_Image.Dispose();
                ho_ImageMean.Dispose();
                ho_ImageScaled.Dispose();
                ho_GrayImage.Dispose();
                ho_Rectangle.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ImageReduced.Dispose();
                ho_Contours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_RegressContours.Dispose();
                ho_UnionContours.Dispose();
                ho_SortedContours.Dispose();
                ho_SelectedContours.Dispose();
                ho_SelectedXLD.Dispose();
                ho_RegionLines.Dispose();
                ho_ModelContours.Dispose();
                ho_ModelTrans.Dispose();
                ho_Region_label.Dispose();
                ho_RegionUnion.Dispose();
                ho_Rectangle1.Dispose();
                ho_ObjectSelected.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_RowBeginOut2.Dispose();
                hv_ColBeginOut2.Dispose();
                hv_RowEndOut2.Dispose();
                hv_ColEndOut2.Dispose();
                hv_rowa.Dispose();
                hv_cola.Dispose();
                hv_rowb.Dispose();
                hv_colb.Dispose();
                hv_ModelID1.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Angle.Dispose();
                hv_Scale.Dispose();
                hv_Score.Dispose();
                hv_result_concat.Dispose();
                hv_message.Dispose();
                hv_I.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DTranslate.Dispose();
                hv_HomMat2DRotate.Dispose();
                hv_HomMat2DScale.Dispose();
                hv_Row5.Dispose();
                hv_Column5.Dispose();
                hv_Phi3.Dispose();
                hv_Length12.Dispose();
                hv_Length22.Dispose();
                hv_Angle1.Dispose();
                hv_relative_deg.Dispose();
                hv_relative_deg_abs.Dispose();
            }
            


        }

        //--------------------------------左前左后------------------------------------------------------
        public Dictionary<string, dynamic> DoAction_qianzuoqianyou(Dictionary<string, dynamic> actionParameters)
        {

            Dictionary<string, dynamic> result_qianzuoqianyou = new Dictionary<string, dynamic>();

            // Local iconic variables 

            HObject ho_Image, ho_ImageScaled, ho_GrayImage;
            HObject ho_Rectangle, ho_Region, ho_ConnectedRegions, ho_SelectedRegions;
            HObject ho_RegionOpening, ho_RegionFillUp, ho_ImageReduced;
            HObject ho_Contours, ho_ContoursSplit, ho_RegressContours;
            HObject ho_UnionContours, ho_SortedContours, ho_SelectedContours;
            HObject ho_SelectedXLD, ho_RegionLines, ho_ModelContours;
            HObject ho_ModelTrans = null, ho_Region2 = null, ho_RegionUnion = null;
            HObject ho_Rectangle1 = null, ho_ObjectSelected = null;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Mean = new HTuple(), hv_Deviation = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_RowBegin = new HTuple(), hv_ColBegin = new HTuple();
            HTuple hv_RowEnd = new HTuple(), hv_ColEnd = new HTuple();
            HTuple hv_Nr = new HTuple(), hv_Nc = new HTuple(), hv_Dist = new HTuple();
            HTuple hv_RowBeginOut2 = new HTuple(), hv_ColBeginOut2 = new HTuple();
            HTuple hv_RowEndOut2 = new HTuple(), hv_ColEndOut2 = new HTuple();
            HTuple hv_rowa = new HTuple(), hv_cola = new HTuple();
            HTuple hv_rowb = new HTuple(), hv_colb = new HTuple();
            HTuple hv_ModelID1 = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Score = new HTuple();
            HTuple hv_result_concat = new HTuple(), hv_message = new HTuple();
            HTuple hv_I = new HTuple(), hv_HomMat2DIdentity = new HTuple();
            HTuple hv_HomMat2DTranslate = new HTuple(), hv_HomMat2DRotate = new HTuple();
            HTuple hv_HomMat2DScale = new HTuple(), hv_Row5 = new HTuple();
            HTuple hv_Column5 = new HTuple(), hv_Phi3 = new HTuple();
            HTuple hv_Length12 = new HTuple(), hv_Length22 = new HTuple();
            HTuple hv_Angle1 = new HTuple(), hv_relative_deg = new HTuple();
            HTuple hv_relative_deg_abs = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
            HOperatorSet.GenEmptyObj(out ho_RegressContours);
            HOperatorSet.GenEmptyObj(out ho_UnionContours);
            HOperatorSet.GenEmptyObj(out ho_SortedContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_ModelTrans);
            HOperatorSet.GenEmptyObj(out ho_Region2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);


            try
            {
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, actionParameters["Image"]);
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                ho_ImageScaled.Dispose();
                HOperatorSet.ScaleImage(ho_Image, out ho_ImageScaled, 6, 0);
                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(ho_ImageScaled, out ho_GrayImage);

                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_Height, hv_Width);
                hv_Mean.Dispose(); hv_Deviation.Dispose();
                HOperatorSet.Intensity(ho_Rectangle, ho_Image, out hv_Mean, out hv_Deviation);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_GrayImage, out ho_Region, hv_Mean + hv_Deviation, 255);
                }


                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                    70);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningRectangle1(ho_SelectedRegions, out ho_RegionOpening, hv_Width / 20,
                        hv_Height / 20);
                }
                //closing_rectangle1 (SelectedRegions, RegionClosing, Width/100, Height/100)

                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionOpening, out ho_RegionFillUp);
                hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                HOperatorSet.SmallestRectangle1(ho_RegionFillUp, out hv_Row1, out hv_Column1,
                    out hv_Row2, out hv_Column2);
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionOpening, out ho_ImageReduced
                    );


                ho_Contours.Dispose();
                HOperatorSet.GenContourRegionXld(ho_SelectedRegions, out ho_Contours, "border");
                ho_ContoursSplit.Dispose();
                HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines", 5,
                    4, 2);
                ho_RegressContours.Dispose();
                HOperatorSet.RegressContoursXld(ho_ContoursSplit, out ho_RegressContours, "no",
                    1);
                ho_UnionContours.Dispose();
                HOperatorSet.UnionCollinearContoursXld(ho_RegressContours, out ho_UnionContours,
                    10, 1, 2, 0.1, "attr_keep");
                ho_SortedContours.Dispose();
                HOperatorSet.SortContoursXld(ho_UnionContours, out ho_SortedContours, "upper_left",
                    "true", "column");
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_SelectedContours.Dispose();
                    HOperatorSet.SelectContoursXld(ho_SortedContours, out ho_SelectedContours, "direction",
                        (new HTuple(-10)).TupleRad(), (new HTuple(45)).TupleRad(), -0.5, 0.5);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_SelectedXLD.Dispose();
                    HOperatorSet.SelectShapeXld(ho_SelectedContours, out ho_SelectedXLD, "row", "and",
                        hv_Width / 2, hv_Width);
                }

                hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                HOperatorSet.FitLineContourXld(ho_SelectedXLD, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc, out hv_Dist);
                hv_RowBeginOut2.Dispose(); hv_ColBeginOut2.Dispose(); hv_RowEndOut2.Dispose(); hv_ColEndOut2.Dispose();
                HOperatorSet.SelectLinesLongest(hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd,
                    1, out hv_RowBeginOut2, out hv_ColBeginOut2, out hv_RowEndOut2, out hv_ColEndOut2);
                ho_RegionLines.Dispose();
                HOperatorSet.GenRegionLine(out ho_RegionLines, hv_RowBeginOut2, hv_ColBeginOut2,
                    hv_RowEndOut2, hv_ColEndOut2);

                //20231230 基准线优化，选择水平的
                hv_rowa.Dispose();
                hv_rowa = new HTuple(hv_RowBeginOut2);
                hv_cola.Dispose();
                hv_cola = new HTuple(hv_ColBeginOut2);
                hv_rowb.Dispose();
                hv_rowb = new HTuple(hv_RowEndOut2);
                hv_colb.Dispose();
                hv_colb = new HTuple(hv_ColEndOut2);


                //开始检测
                hv_ModelID1.Dispose();
                HOperatorSet.ReadShapeModel(actionParameters["ModelPath"],
                    out hv_ModelID1);
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID1, 1);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Angle.Dispose(); hv_Scale.Dispose(); hv_Score.Dispose();
                    HOperatorSet.FindScaledShapeModel(ho_GrayImage, hv_ModelID1, (new HTuple(-180)).TupleRad()
                        , (new HTuple(360)).TupleRad(), 0.7, 1.3, 0.2, 1, 0.3, "none", (new HTuple(5)).TupleConcat(
                        1), 0.7, out hv_Row, out hv_Column, out hv_Angle, out hv_Scale, out hv_Score);
                }
                //dev_get_window (WindowHandle)
                hv_result_concat.Dispose();
                hv_result_concat = new HTuple();
                //如果没有匹配的模板不需要计算角度
                if ((int)(new HTuple((new HTuple(hv_Score.TupleLength())).TupleEqual(0))) != 0)
                {
                    hv_message.Dispose();
                    hv_message = "No Lable";
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleConcat(hv_result_concat, hv_Score, out ExpTmpOutVar_0);
                        hv_result_concat.Dispose();
                        hv_result_concat = ExpTmpOutVar_0;
                    }
                    //dev_display (Image)
                    //*         disp_message (WindowHandle, message, 'window', 50, 50, 'red', 'true')
                    //stop ()

                    //如果没有匹配的模板不需要计算角度
                }
                else
                {
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Score.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                    {
                        hv_HomMat2DIdentity.Dispose();
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_HomMat2DTranslate.Dispose();
                            HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_Row.TupleSelect(hv_I),
                                hv_Column.TupleSelect(hv_I), out hv_HomMat2DTranslate);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_HomMat2DRotate.Dispose();
                            HOperatorSet.HomMat2dRotate(hv_HomMat2DTranslate, hv_Angle.TupleSelect(hv_I),
                                hv_Row.TupleSelect(hv_I), hv_Column.TupleSelect(hv_I), out hv_HomMat2DRotate);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_HomMat2DScale.Dispose();
                            HOperatorSet.HomMat2dScale(hv_HomMat2DRotate, hv_Scale.TupleSelect(hv_I),
                                hv_Scale.TupleSelect(hv_I), hv_Row.TupleSelect(hv_I), hv_Column.TupleSelect(
                                hv_I), out hv_HomMat2DScale);
                        }
                        ho_ModelTrans.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ModelTrans, hv_HomMat2DScale);

                        ho_Region2.Dispose();
                        HOperatorSet.GenRegionContourXld(ho_ModelTrans, out ho_Region2, "margin");
                        ho_RegionUnion.Dispose();
                        HOperatorSet.Union1(ho_Region2, out ho_RegionUnion);
                        hv_Row5.Dispose(); hv_Column5.Dispose(); hv_Phi3.Dispose(); hv_Length12.Dispose(); hv_Length22.Dispose();
                        HOperatorSet.SmallestRectangle2(ho_RegionUnion, out hv_Row5, out hv_Column5,
                            out hv_Phi3, out hv_Length12, out hv_Length22);

                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row5, hv_Column5, hv_Phi3,
                            hv_Length12, hv_Length22);
                        ho_ImageReduced.Dispose();
                        HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle1, out ho_ImageReduced);

                        //确保选择水平基线
                        ho_Contours.Dispose();
                        HOperatorSet.GenContourRegionXld(ho_ImageReduced, out ho_Contours, "border");
                        ho_ContoursSplit.Dispose();
                        HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines",
                            5, 4, 2);
                        ho_SelectedContours.Dispose();
                        HOperatorSet.SelectContoursXld(ho_ContoursSplit, out ho_SelectedContours,
                            "contour_length", 50, 90000, -0.5, 0.5);
                        ho_SortedContours.Dispose();
                        HOperatorSet.SortContoursXld(ho_SelectedContours, out ho_SortedContours,
                            "lower_left", "true", "row");
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected, 1);
                        hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                        HOperatorSet.FitLineContourXld(ho_ObjectSelected, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                            out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                            out hv_Dist);

                        //计算相对偏移角度
                        hv_Angle1.Dispose();
                        HOperatorSet.AngleLl(hv_rowa, hv_cola, hv_rowb, hv_colb, hv_RowBegin, hv_ColBegin,
                            hv_RowEnd, hv_ColEnd, out hv_Angle1);
                        hv_relative_deg.Dispose();
                        HOperatorSet.TupleDeg(hv_Angle1, out hv_relative_deg);
                        hv_relative_deg_abs.Dispose();
                        HOperatorSet.TupleAbs(hv_relative_deg, out hv_relative_deg_abs);
                        if ((int)(new HTuple(hv_relative_deg_abs.TupleGreater(90))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_relative_deg_abs = 180 - hv_relative_deg_abs;
                                    hv_relative_deg_abs.Dispose();
                                    hv_relative_deg_abs = ExpTmpLocalVar_relative_deg_abs;
                                }
                            }
                        }
                        hv_result_concat.Dispose();
                        HOperatorSet.TupleConcat(hv_Score, hv_relative_deg_abs, out hv_result_concat);
                        hv_message.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_message = new HTuple("OK Label, relative_deg = ") + hv_relative_deg_abs;
                        }
                        if ((int)(new HTuple(hv_relative_deg_abs.TupleGreaterEqual(10))) != 0)
                        {
                            hv_message.Dispose();
                            hv_message = new HTuple("OK Label , but relative_deg more than 10 ");
                        }

                    }
                    //dev_set_draw ('margin')
                    //dev_display (Image)
                    //dev_display (RegionLines)
                    //dev_display (Rectangle1)
                    //*          disp_message (WindowHandle, message, 'window', 50, 50, 'red', 'true')
                    //stop ()

                }
                HOperatorSet.ClearShapeModel(hv_ModelID1);
                if (hv_result_concat.Length != 0)
                {
                    result_qianzuoqianyou.Add("Score", hv_result_concat.DArr[0]);
                    result_qianzuoqianyou.Add("Relative_deg", hv_result_concat.DArr[1]);
                }
                else
                {
                    result_qianzuoqianyou.Add("Score", 999);
                    result_qianzuoqianyou.Add("Relative_deg", 999);
                }

                return result_qianzuoqianyou;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ho_Image.Dispose();
                ho_ImageScaled.Dispose();
                ho_GrayImage.Dispose();
                ho_Rectangle.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ImageReduced.Dispose();
                ho_Contours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_RegressContours.Dispose();
                ho_UnionContours.Dispose();
                ho_SortedContours.Dispose();
                ho_SelectedContours.Dispose();
                ho_SelectedXLD.Dispose();
                ho_RegionLines.Dispose();
                ho_ModelContours.Dispose();
                ho_ModelTrans.Dispose();
                ho_Region2.Dispose();
                ho_RegionUnion.Dispose();
                ho_Rectangle1.Dispose();
                ho_ObjectSelected.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_RowBeginOut2.Dispose();
                hv_ColBeginOut2.Dispose();
                hv_RowEndOut2.Dispose();
                hv_ColEndOut2.Dispose();
                hv_rowa.Dispose();
                hv_cola.Dispose();
                hv_rowb.Dispose();
                hv_colb.Dispose();
                hv_ModelID1.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Angle.Dispose();
                hv_Scale.Dispose();
                hv_Score.Dispose();
                hv_result_concat.Dispose();
                hv_message.Dispose();
                hv_I.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DTranslate.Dispose();
                hv_HomMat2DRotate.Dispose();
                hv_HomMat2DScale.Dispose();
                hv_Row5.Dispose();
                hv_Column5.Dispose();
                hv_Phi3.Dispose();
                hv_Length12.Dispose();
                hv_Length22.Dispose();
                hv_Angle1.Dispose();
                hv_relative_deg.Dispose();
                hv_relative_deg_abs.Dispose();
            }



        }

        //--------------------------------左右相机--------------------------------------------------------- 
        public Dictionary<string, dynamic> DoAction_zuoyou(Dictionary<string, dynamic> actionParameters)
        {

            Dictionary<string, dynamic> result_zuoyou = new Dictionary<string, dynamic>();

            // Local iconic variables 

            HObject ho_Image, ho_ImageScaled, ho_GrayImage;
            HObject ho_Rectangle, ho_Region, ho_ConnectedRegions, ho_SelectedRegions;
            HObject ho_RegionOpening, ho_RegionFillUp, ho_ImageReduced;
            HObject ho_Contours, ho_ContoursSplit, ho_RegressContours;
            HObject ho_UnionContours, ho_SortedContours, ho_SelectedContours1;
            HObject ho_SelectedContours, ho_SelectedXLD, ho_RegionLines;
            HObject ho_ModelContours, ho_ModelTrans = null, ho_Region2 = null;
            HObject ho_RegionUnion = null, ho_Rectangle1 = null, ho_ObjectSelected = null;
            HObject ho_RegionLines1 = null;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Mean = new HTuple(), hv_Deviation = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_RowBegin = new HTuple(), hv_ColBegin = new HTuple();
            HTuple hv_RowEnd = new HTuple(), hv_ColEnd = new HTuple();
            HTuple hv_Nr = new HTuple(), hv_Nc = new HTuple(), hv_Dist = new HTuple();
            HTuple hv_RowBeginOut2 = new HTuple(), hv_ColBeginOut2 = new HTuple();
            HTuple hv_RowEndOut2 = new HTuple(), hv_ColEndOut2 = new HTuple();
            HTuple hv_ModelID = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Angle = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Score = new HTuple();
            HTuple hv_result_concat = new HTuple(), hv_message = new HTuple();
            HTuple hv_I = new HTuple(), hv_HomMat2DIdentity = new HTuple();
            HTuple hv_HomMat2DTranslate = new HTuple(), hv_HomMat2DRotate = new HTuple();
            HTuple hv_HomMat2DScale = new HTuple(), hv_Row5 = new HTuple();
            HTuple hv_Column5 = new HTuple(), hv_Phi3 = new HTuple();
            HTuple hv_Length12 = new HTuple(), hv_Length22 = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_Phi_deg = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_rowa = new HTuple();
            HTuple hv_cola = new HTuple(), hv_rowb = new HTuple();
            HTuple hv_colb = new HTuple(), hv_Angle1 = new HTuple();
            HTuple hv_relative_deg = new HTuple(), hv_relative_deg_abs = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
            HOperatorSet.GenEmptyObj(out ho_RegressContours);
            HOperatorSet.GenEmptyObj(out ho_UnionContours);
            HOperatorSet.GenEmptyObj(out ho_SortedContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedContours1);
            HOperatorSet.GenEmptyObj(out ho_SelectedContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_ModelContours);
            HOperatorSet.GenEmptyObj(out ho_ModelTrans);
            HOperatorSet.GenEmptyObj(out ho_Region2);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_RegionLines1);


            try
            {
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, actionParameters["Image"]);
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                ho_ImageScaled.Dispose();
                HOperatorSet.ScaleImage(ho_Image, out ho_ImageScaled, 6.5, 0);
                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(ho_ImageScaled, out ho_GrayImage);

                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_Height, hv_Width);
                hv_Mean.Dispose(); hv_Deviation.Dispose();
                HOperatorSet.Intensity(ho_Rectangle, ho_Image, out hv_Mean, out hv_Deviation);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_GrayImage, out ho_Region, hv_Mean + hv_Deviation, 255);
                }


                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                    70);

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningRectangle1(ho_SelectedRegions, out ho_RegionOpening, hv_Width / 100,
                        hv_Height / 100);
                }

                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionOpening, out ho_RegionFillUp);
                hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                HOperatorSet.SmallestRectangle1(ho_RegionFillUp, out hv_Row1, out hv_Column1,
                    out hv_Row2, out hv_Column2);
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_GrayImage, ho_RegionFillUp, out ho_ImageReduced);
                ho_Contours.Dispose();
                HOperatorSet.GenContourRegionXld(ho_ImageReduced, out ho_Contours, "border");
                ho_ContoursSplit.Dispose();
                HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines", 5,
                    4, 2);
                ho_RegressContours.Dispose();
                HOperatorSet.RegressContoursXld(ho_ContoursSplit, out ho_RegressContours, "no",
                    1);
                ho_UnionContours.Dispose();
                HOperatorSet.UnionCollinearContoursXld(ho_RegressContours, out ho_UnionContours,
                    10, 1, 2, 0.1, "attr_keep");
                ho_SortedContours.Dispose();
                HOperatorSet.SortContoursXld(ho_UnionContours, out ho_SortedContours, "upper_left",
                    "true", "column");
                ho_SelectedContours1.Dispose();
                HOperatorSet.SelectContoursXld(ho_SortedContours, out ho_SelectedContours1, "contour_length",
                    100, 3000, -0.5, 0.5);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_SelectedContours.Dispose();
                    HOperatorSet.SelectContoursXld(ho_SelectedContours1, out ho_SelectedContours,
                        "direction", (new HTuple(20)).TupleRad(), (new HTuple(89)).TupleRad(), -0.5,
                        0.5);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_SelectedXLD.Dispose();
                    HOperatorSet.SelectShapeXld(ho_SelectedContours, out ho_SelectedXLD, "column",
                        "and", hv_Width / 2, hv_Width);
                }
                hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                HOperatorSet.FitLineContourXld(ho_SelectedXLD, "gauss", -1, 0, 5, 4, out hv_RowBegin,
                    out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc, out hv_Dist);
                hv_RowBeginOut2.Dispose(); hv_ColBeginOut2.Dispose(); hv_RowEndOut2.Dispose(); hv_ColEndOut2.Dispose();
                HOperatorSet.SelectLinesLongest(hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd,
                    3, out hv_RowBeginOut2, out hv_ColBeginOut2, out hv_RowEndOut2, out hv_ColEndOut2);
                ho_RegionLines.Dispose();
                HOperatorSet.GenRegionLine(out ho_RegionLines, hv_RowBeginOut2, hv_ColBeginOut2,
                    hv_RowEndOut2, hv_ColEndOut2);
                //说明负值表示顺时针右想下，正值表示逆时针右向上



                //需要选择竖直线做基线参考
                //20231230二次优化，选择基线，用角度方向进行限制。


                //开始检测
                hv_ModelID.Dispose();
                HOperatorSet.ReadShapeModel(actionParameters["ModelPath"],
                    out hv_ModelID);
                ho_ModelContours.Dispose();
                HOperatorSet.GetShapeModelContours(out ho_ModelContours, hv_ModelID, 1);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Angle.Dispose(); hv_Scale.Dispose(); hv_Score.Dispose();
                    HOperatorSet.FindScaledShapeModel(ho_GrayImage, hv_ModelID, (new HTuple(-180)).TupleRad()
                        , (new HTuple(360)).TupleRad(), 0.7, 1.3, 0.2, 1, 0.2, "none", (new HTuple(5)).TupleConcat(
                        1), 0.7, out hv_Row, out hv_Column, out hv_Angle, out hv_Scale, out hv_Score);
                }
                //dev_get_window (WindowHandle)
                hv_result_concat.Dispose();
                hv_result_concat = new HTuple();
                //如果没有匹配的模板不需要计算角度
                if ((int)(new HTuple((new HTuple(hv_Score.TupleLength())).TupleEqual(0))) != 0)
                {
                    hv_message.Dispose();
                    hv_message = "No Lable";
                    {
                        HTuple ExpTmpOutVar_0;
                        HOperatorSet.TupleConcat(hv_result_concat, hv_Score, out ExpTmpOutVar_0);
                        hv_result_concat.Dispose();
                        hv_result_concat = ExpTmpOutVar_0;
                    }
                    //dev_display (Image)
                    //disp_message (WindowHandle, message, 'window', 50, 50, 'red', 'true')
                    //stop ()
                }
                else
                {
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_Score.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                    {
                        hv_HomMat2DIdentity.Dispose();
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_HomMat2DTranslate.Dispose();
                            HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_Row.TupleSelect(hv_I),
                                hv_Column.TupleSelect(hv_I), out hv_HomMat2DTranslate);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_HomMat2DRotate.Dispose();
                            HOperatorSet.HomMat2dRotate(hv_HomMat2DTranslate, hv_Angle.TupleSelect(hv_I),
                                hv_Row.TupleSelect(hv_I), hv_Column.TupleSelect(hv_I), out hv_HomMat2DRotate);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_HomMat2DScale.Dispose();
                            HOperatorSet.HomMat2dScale(hv_HomMat2DRotate, hv_Scale.TupleSelect(hv_I),
                                hv_Scale.TupleSelect(hv_I), hv_Row.TupleSelect(hv_I), hv_Column.TupleSelect(
                                hv_I), out hv_HomMat2DScale);
                        }
                        ho_ModelTrans.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_ModelContours, out ho_ModelTrans, hv_HomMat2DScale);

                        ho_Region2.Dispose();
                        HOperatorSet.GenRegionContourXld(ho_ModelTrans, out ho_Region2, "margin");
                        ho_RegionUnion.Dispose();
                        HOperatorSet.Union1(ho_Region2, out ho_RegionUnion);
                        hv_Row5.Dispose(); hv_Column5.Dispose(); hv_Phi3.Dispose(); hv_Length12.Dispose(); hv_Length22.Dispose();
                        HOperatorSet.SmallestRectangle2(ho_RegionUnion, out hv_Row5, out hv_Column5,
                            out hv_Phi3, out hv_Length12, out hv_Length22);

                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row5, hv_Column5, hv_Phi3,
                            hv_Length12, hv_Length22);
                        ho_ImageReduced.Dispose();
                        HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle1, out ho_ImageReduced);

                        //确保选择水平基线
                        ho_Contours.Dispose();
                        HOperatorSet.GenContourRegionXld(ho_ImageReduced, out ho_Contours, "border");
                        ho_ContoursSplit.Dispose();
                        HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines",
                            5, 4, 2);
                        ho_SelectedContours.Dispose();
                        HOperatorSet.SelectContoursXld(ho_ContoursSplit, out ho_SelectedContours,
                            "contour_length", 50, 90000, -0.5, 0.5);
                        ho_SortedContours.Dispose();
                        HOperatorSet.SortContoursXld(ho_SelectedContours, out ho_SortedContours,
                            "upper_right", "true", "column");
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected, 1);
                        hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                        HOperatorSet.FitLineContourXld(ho_ObjectSelected, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                            out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc,
                            out hv_Dist);
                        hv_Phi.Dispose();
                        HOperatorSet.LineOrientation(hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd,
                            out hv_Phi);
                        hv_Phi_deg.Dispose();
                        HOperatorSet.TupleDeg(hv_Phi, out hv_Phi_deg);

                        //排除选择标签上基准线
                        hv_Indices.Dispose();
                        HOperatorSet.TupleSortIndex(hv_ColBeginOut2, out hv_Indices);
                        hv_rowa.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_rowa = hv_RowBeginOut2.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1));
                        }
                        hv_cola.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_cola = hv_ColBeginOut2.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1));
                        }
                        hv_rowb.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_rowb = hv_RowEndOut2.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1));
                        }
                        hv_colb.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_colb = hv_ColEndOut2.TupleSelect(
                                hv_Indices.TupleSelect((new HTuple(hv_Indices.TupleLength())) - 1));
                        }
                        ho_RegionLines1.Dispose();
                        HOperatorSet.GenRegionLine(out ho_RegionLines1, hv_rowa, hv_cola, hv_rowb,
                            hv_colb);


                        hv_Angle1.Dispose();
                        HOperatorSet.AngleLl(hv_rowa, hv_cola, hv_rowb, hv_colb, hv_RowBegin, hv_ColBegin,
                            hv_RowEnd, hv_ColEnd, out hv_Angle1);
                        hv_relative_deg.Dispose();
                        HOperatorSet.TupleDeg(hv_Angle1, out hv_relative_deg);
                        hv_relative_deg_abs.Dispose();
                        HOperatorSet.TupleAbs(hv_relative_deg, out hv_relative_deg_abs);

                        if ((int)(new HTuple(hv_relative_deg_abs.TupleGreater(90))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_relative_deg_abs = 180 - hv_relative_deg_abs;
                                    hv_relative_deg_abs.Dispose();
                                    hv_relative_deg_abs = ExpTmpLocalVar_relative_deg_abs;
                                }
                            }
                        }
                        hv_result_concat.Dispose();
                        HOperatorSet.TupleConcat(hv_Score, hv_relative_deg_abs, out hv_result_concat);
                        hv_message.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_message = new HTuple("OK Label, relative_deg = ") + hv_relative_deg_abs;
                        }
                        if ((int)(new HTuple(hv_relative_deg_abs.TupleGreaterEqual(10))) != 0)
                        {
                            hv_message.Dispose();
                            hv_message = new HTuple("OK Label , but relative_deg more than 10 ");
                        }

                    }
                }
                HOperatorSet.ClearShapeModel(hv_ModelID);
                if (hv_result_concat.Length != 0)
                {
                    result_zuoyou.Add("Score", hv_result_concat.DArr[0]);
                    result_zuoyou.Add("Relative_deg", hv_result_concat.DArr[1]);
                }
                else
                {
                    result_zuoyou.Add("Score", 999);
                    result_zuoyou.Add("Relative_deg", 999);
                }

                return result_zuoyou;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ho_Image.Dispose();
                ho_ImageScaled.Dispose();
                ho_GrayImage.Dispose();
                ho_Rectangle.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ImageReduced.Dispose();
                ho_Contours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_RegressContours.Dispose();
                ho_UnionContours.Dispose();
                ho_SortedContours.Dispose();
                ho_SelectedContours1.Dispose();
                ho_SelectedContours.Dispose();
                ho_SelectedXLD.Dispose();
                ho_RegionLines.Dispose();
                ho_ModelContours.Dispose();
                ho_ModelTrans.Dispose();
                ho_Region2.Dispose();
                ho_RegionUnion.Dispose();
                ho_Rectangle1.Dispose();
                ho_ObjectSelected.Dispose();
                ho_RegionLines1.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_RowBeginOut2.Dispose();
                hv_ColBeginOut2.Dispose();
                hv_RowEndOut2.Dispose();
                hv_ColEndOut2.Dispose();
                hv_ModelID.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Angle.Dispose();
                hv_Scale.Dispose();
                hv_Score.Dispose();
                hv_result_concat.Dispose();
                hv_message.Dispose();
                hv_I.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DTranslate.Dispose();
                hv_HomMat2DRotate.Dispose();
                hv_HomMat2DScale.Dispose();
                hv_Row5.Dispose();
                hv_Column5.Dispose();
                hv_Phi3.Dispose();
                hv_Length12.Dispose();
                hv_Length22.Dispose();
                hv_Phi.Dispose();
                hv_Phi_deg.Dispose();
                hv_Indices.Dispose();
                hv_rowa.Dispose();
                hv_cola.Dispose();
                hv_rowb.Dispose();
                hv_colb.Dispose();
                hv_Angle1.Dispose();
                hv_relative_deg.Dispose();
                hv_relative_deg_abs.Dispose();

            }


        }


        // Main procedure 
        public override void UnInit()
        {
            try
            {
                IsInit = false;
            }
            catch { }

        }
    }
}


