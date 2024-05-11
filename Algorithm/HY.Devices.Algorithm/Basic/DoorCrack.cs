using HalconDotNet;
using System;
using System.Drawing;

namespace HY.Devices.Algorithm
{
    /// <summary>
    /// 新门缝算法
    /// </summary>
    public class DoorCrack
    {
        public HTuple hv_ExpDefaultWinHandle;

        // Main procedure 
        private DoorCrackResult DoorCrackAction(HObject hb_image, double Yscale, double Zscale, int y0, int x0, int y1, int x1, double threolddata1, int bshumenfeng)
        {


            // Local iconic variables 
            DoorCrackResult doorCrackResult = default(DoorCrackResult);
            HObject ho_ImageFeng = null, ho_ROI_0 = null, ho_ImageReduced = null;
            HObject ho_ImageMean = null, ho_Region1 = null, ho_RegionOpening = null;
            HObject ho_ConnectedRegions1 = null, ho_SelectedRegions1 = null;
            HObject ho_RegionFillUp = null, ho_SortedRegions = null, ho_ObjectSelectedOne = null;
            HObject ho_Contours = null, ho_ContoursSplit = null, ho_SelectedXLD = null;
            HObject ho_SelectedXLD1 = null, ho_SortedContours = null, ho_ObjectSelected = null;
            HObject ho_SmoothedContours = null, ho_Cross = null, ho_ObjectSelectedtwo = null;
            HObject ho_Contours2 = null, ho_ContoursSplit2 = null, ho_SelectedXLD2 = null;
            HObject ho_SelectedXLD3 = null, ho_SortedContours3 = null, ho_ObjectSelected2 = null;
            HObject ho_SmoothedContours2 = null, ho_RegionLines = null;

            // Local control variables 

            HTuple hv_bshumenfeng = new HTuple(), hv_y0 = new HTuple();
            HTuple hv_x0 = new HTuple(), hv_y1 = new HTuple(), hv_x1 = new HTuple();
            HTuple hv_Yscale = new HTuple(), hv_Zscale = new HTuple();
            HTuple hv_threolddata1 = new HTuple(), hv_H = new HTuple();
            HTuple hv_Number1 = new HTuple(), hv_SelectedRegionsNumber1 = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column3 = new HTuple(), hv_Area2 = new HTuple();
            HTuple hv_Row23 = new HTuple(), hv_Column23 = new HTuple();
            HTuple hv_RowBegin = new HTuple(), hv_ColBegin = new HTuple();
            HTuple hv_RowEnd = new HTuple(), hv_ColEnd = new HTuple();
            HTuple hv_Nr = new HTuple(), hv_Nc = new HTuple(), hv_Dist = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_resultDistance = new HTuple();
            HTuple hv_W = new HTuple();
            try
            {
                // Initialize local and output iconic variables 
                HOperatorSet.GenEmptyObj(out ho_ImageFeng);
                HOperatorSet.GenEmptyObj(out ho_ROI_0);
                HOperatorSet.GenEmptyObj(out ho_ImageReduced);
                HOperatorSet.GenEmptyObj(out ho_ImageMean);
                HOperatorSet.GenEmptyObj(out ho_Region1);
                HOperatorSet.GenEmptyObj(out ho_RegionOpening);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
                HOperatorSet.GenEmptyObj(out ho_SortedRegions);
                HOperatorSet.GenEmptyObj(out ho_ObjectSelectedOne);
                HOperatorSet.GenEmptyObj(out ho_Contours);
                HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
                HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
                HOperatorSet.GenEmptyObj(out ho_SelectedXLD1);
                HOperatorSet.GenEmptyObj(out ho_SortedContours);
                HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
                HOperatorSet.GenEmptyObj(out ho_SmoothedContours);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_ObjectSelectedtwo);
                HOperatorSet.GenEmptyObj(out ho_Contours2);
                HOperatorSet.GenEmptyObj(out ho_ContoursSplit2);
                HOperatorSet.GenEmptyObj(out ho_SelectedXLD2);
                HOperatorSet.GenEmptyObj(out ho_SelectedXLD3);
                HOperatorSet.GenEmptyObj(out ho_SortedContours3);
                HOperatorSet.GenEmptyObj(out ho_ObjectSelected2);
                HOperatorSet.GenEmptyObj(out ho_SmoothedContours2);
                HOperatorSet.GenEmptyObj(out ho_RegionLines);

                //输入：
                //深度输入Y轴的比例系数，Yscale
                //深度输入Z轴的比例系数，Zscale
                //深度灰度图片，ImageFeng

                //门缝处理区域，y0,x0,y1,x1
                //深度门缝阈值 threolddata1 一般默认

                //是否为竖门锋 bshumenfeng,0是横门缝，1是竖的
                hv_bshumenfeng.Dispose();
                hv_bshumenfeng = bshumenfeng;
                //y0 := 18.7562
                //x0 := 663.723
                //y1 := 38.9933
                //x1 := 1079.61

                //横门缝

                //y0 := 5137
                //x0 := 1920
                //y1 := 5289
                //x1 := 2130
                hv_y0.Dispose();
                hv_y0 = y0;
                hv_x0.Dispose();
                hv_x0 = x0;
                hv_y1.Dispose();
                hv_y1 = y1;
                hv_x1.Dispose();
                hv_x1 = x1;


                //竖门缝
                //y0 := 5540
                //x0 := 1151
                //y1 := 5665
                //x1 := 1497

                hv_Yscale.Dispose();
                hv_Yscale = Yscale;
                hv_Zscale.Dispose();
                hv_Zscale = Zscale;
                hv_threolddata1.Dispose();
                hv_threolddata1 = threolddata1;


                //输出：
                //门缝宽度resultDistance  最终有用
                //门缝右边直线   RowBegin, ColBegin, RowEnd, ColEnd
                //门缝左边中心点   Row3, Column3

                ho_ImageFeng.Dispose();
                HOperatorSet.CopyImage(hb_image, out ho_ImageFeng);
                ho_ROI_0.Dispose();
                HOperatorSet.GenRectangle1(out ho_ROI_0, hv_y0, hv_x0, hv_y1, hv_x1);
                if ((int)(new HTuple(hv_bshumenfeng.TupleEqual(1))) != 0)
                {

                    hv_H.Dispose();
                    HOperatorSet.RegionFeatures(ho_ROI_0, "height", out hv_H);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageFeng, ho_ROI_0, out ho_ImageReduced);
                    ho_ImageMean.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean, 3, 3);
                    ho_Region1.Dispose();
                    HOperatorSet.Threshold(ho_ImageMean, out ho_Region1, hv_threolddata1, 2147483647);

                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningRectangle1(ho_Region1, out ho_RegionOpening, hv_H * 0.5,
                        (3 * hv_H) / 4);

                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions1);

                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions1, "height",
                        "and", hv_H * 0.8, hv_H);

                    hv_Number1.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number1);

                    if ((int)(new HTuple(hv_Number1.TupleGreaterEqual(2))) != 0)
                    {
                        ho_RegionFillUp.Dispose();
                        HOperatorSet.FillUp(ho_SelectedRegions1, out ho_RegionFillUp);
                        ho_SortedRegions.Dispose();
                        HOperatorSet.SortRegion(ho_RegionFillUp, out ho_SortedRegions, "first_point",
                            "true", "column");
                        ho_ObjectSelectedOne.Dispose();
                        HOperatorSet.SelectObj(ho_SortedRegions, out ho_ObjectSelectedOne, 1);
                        ho_Contours.Dispose();
                        HOperatorSet.GenContourRegionXld(ho_ObjectSelectedOne, out ho_Contours, "border");
                        ho_ContoursSplit.Dispose();
                        HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines_circles",
                            1, 5, 5);

                        ho_SelectedXLD.Dispose();
                        HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_SelectedXLD, "contlength",
                            "and", (3 * hv_H) / 4, hv_H + 20);

                        hv_SelectedRegionsNumber1.Dispose();
                        HOperatorSet.CountObj(ho_SelectedXLD, out hv_SelectedRegionsNumber1);
                        if ((int)(new HTuple(hv_SelectedRegionsNumber1.TupleGreater(0))) != 0)
                        {
                            ho_SelectedXLD1.Dispose();
                            HOperatorSet.SelectShapeXld(ho_SelectedXLD, out ho_SelectedXLD1, (new HTuple("phi_points")).TupleConcat(
                                "phi_points"), "or", (new HTuple(-2)).TupleConcat(1), (new HTuple(-1)).TupleConcat(
                                2));
                            ho_SortedContours.Dispose();
                            HOperatorSet.SortContoursXld(ho_SelectedXLD1, out ho_SortedContours, "upper_left",
                                "false", "column");
                            ho_ObjectSelected.Dispose();
                            HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected, 1);
                            ho_SmoothedContours.Dispose();
                            HOperatorSet.SmoothContoursXld(ho_ObjectSelected, out ho_SmoothedContours,
                                9);
                            hv_Area.Dispose(); hv_Row3.Dispose(); hv_Column3.Dispose();
                            HOperatorSet.AreaCenterPointsXld(ho_SmoothedContours, out hv_Area, out hv_Row3,
                                out hv_Column3);
                        }
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row3, hv_Column3, 15, 0);
                        ho_ObjectSelectedtwo.Dispose();
                        HOperatorSet.SelectObj(ho_RegionFillUp, out ho_ObjectSelectedtwo, 2);
                        ho_Contours2.Dispose();
                        HOperatorSet.GenContourRegionXld(ho_ObjectSelectedtwo, out ho_Contours2,
                            "border");
                        ho_ContoursSplit2.Dispose();
                        HOperatorSet.SegmentContoursXld(ho_Contours2, out ho_ContoursSplit2, "lines_circles",
                            1, 5, 5);

                        ho_SelectedXLD2.Dispose();
                        HOperatorSet.SelectShapeXld(ho_ContoursSplit2, out ho_SelectedXLD2, "contlength",
                            "and", (3 * hv_H) / 4, hv_H + 20);

                        hv_SelectedRegionsNumber1.Dispose();
                        HOperatorSet.CountObj(ho_SelectedXLD2, out hv_SelectedRegionsNumber1);
                        if ((int)(new HTuple(hv_SelectedRegionsNumber1.TupleGreater(0))) != 0)
                        {
                            ho_SelectedXLD3.Dispose();
                            HOperatorSet.SelectShapeXld(ho_SelectedXLD2, out ho_SelectedXLD3, (new HTuple("phi_points")).TupleConcat(
                                "phi_points"), "or", (new HTuple(-2)).TupleConcat(1), (new HTuple(-1)).TupleConcat(
                                2));
                            ho_SortedContours3.Dispose();
                            HOperatorSet.SortContoursXld(ho_SelectedXLD3, out ho_SortedContours3, "upper_left",
                                "true", "column");
                            ho_ObjectSelected2.Dispose();
                            HOperatorSet.SelectObj(ho_SortedContours3, out ho_ObjectSelected2, 1);
                            ho_SmoothedContours2.Dispose();
                            HOperatorSet.SmoothContoursXld(ho_ObjectSelected2, out ho_SmoothedContours2,
                                9);
                            hv_Area2.Dispose(); hv_Row23.Dispose(); hv_Column23.Dispose();
                            HOperatorSet.AreaCenterPointsXld(ho_SmoothedContours2, out hv_Area2, out hv_Row23,
                                out hv_Column23);
                            hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                            HOperatorSet.FitLineContourXld(ho_SmoothedContours2, "tukey", -1, 0, 11,
                                2, out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd,
                                out hv_Nr, out hv_Nc, out hv_Dist);
                            hv_Distance.Dispose();
                            HOperatorSet.DistancePl(hv_Row3, hv_Column3, hv_RowBegin, hv_ColBegin,
                                hv_RowEnd, hv_ColEnd, out hv_Distance);
                            hv_resultDistance.Dispose();
                            hv_resultDistance = hv_Yscale * hv_Distance;
                            doorCrackResult.Distance = hv_resultDistance.D;

                        }

                    }

                }


                else if ((int)(new HTuple(hv_bshumenfeng.TupleEqual(0))) != 0)
                {

                    hv_W.Dispose();
                    HOperatorSet.RegionFeatures(ho_ROI_0, "width", out hv_W);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageFeng, ho_ROI_0, out ho_ImageReduced);
                    ho_ImageMean.Dispose();
                    HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean, 3, 3);
                    ho_Region1.Dispose();
                    HOperatorSet.Threshold(ho_ImageMean, out ho_Region1, hv_threolddata1, 2147483647);

                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningRectangle1(ho_Region1, out ho_RegionOpening, hv_W * 0.5,
                        (1 * hv_W) / 6);

                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions1);


                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions1, "width",
                        "and", hv_W * 0.8, hv_W);


                    hv_Number1.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number1);

                    if ((int)(new HTuple(hv_Number1.TupleGreaterEqual(2))) != 0)
                    {
                        ho_RegionFillUp.Dispose();
                        HOperatorSet.FillUp(ho_SelectedRegions1, out ho_RegionFillUp);
                        ho_SortedRegions.Dispose();
                        HOperatorSet.SortRegion(ho_RegionFillUp, out ho_SortedRegions, "first_point",
                            "true", "column");
                        ho_ObjectSelectedOne.Dispose();
                        HOperatorSet.SelectObj(ho_SortedRegions, out ho_ObjectSelectedOne, 1);
                        ho_Contours.Dispose();
                        HOperatorSet.GenContourRegionXld(ho_ObjectSelectedOne, out ho_Contours, "border");
                        ho_ContoursSplit.Dispose();
                        HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines_circles",
                            1, 5, 5);

                        ho_SelectedXLD.Dispose();
                        HOperatorSet.SelectShapeXld(ho_ContoursSplit, out ho_SelectedXLD, "contlength",
                            "and", (3 * hv_W) / 4, hv_W + 20);

                        hv_SelectedRegionsNumber1.Dispose();
                        HOperatorSet.CountObj(ho_SelectedXLD, out hv_SelectedRegionsNumber1);
                        if ((int)(new HTuple(hv_SelectedRegionsNumber1.TupleGreater(0))) != 0)
                        {
                            ho_SelectedXLD1.Dispose();
                            HOperatorSet.SelectShapeXld(ho_SelectedXLD, out ho_SelectedXLD1, "phi_points",
                                "and", -0.5, 0.5);
                            ho_SortedContours.Dispose();
                            HOperatorSet.SortContoursXld(ho_SelectedXLD1, out ho_SortedContours, "upper_left",
                                "false", "row");
                            ho_ObjectSelected.Dispose();
                            HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected, 1);
                            ho_SmoothedContours.Dispose();
                            HOperatorSet.SmoothContoursXld(ho_ObjectSelected, out ho_SmoothedContours,
                                9);
                            hv_Area.Dispose(); hv_Row3.Dispose(); hv_Column3.Dispose();
                            HOperatorSet.AreaCenterPointsXld(ho_SmoothedContours, out hv_Area, out hv_Row3,
                                out hv_Column3);
                        }
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row3, hv_Column3, 15, 0);
                        ho_ObjectSelectedtwo.Dispose();
                        HOperatorSet.SelectObj(ho_RegionFillUp, out ho_ObjectSelectedtwo, 2);
                        ho_Contours2.Dispose();
                        HOperatorSet.GenContourRegionXld(ho_ObjectSelectedtwo, out ho_Contours2,
                            "border");
                        ho_ContoursSplit2.Dispose();
                        HOperatorSet.SegmentContoursXld(ho_Contours2, out ho_ContoursSplit2, "lines_circles",
                            1, 5, 5);

                        ho_SelectedXLD2.Dispose();
                        HOperatorSet.SelectShapeXld(ho_ContoursSplit2, out ho_SelectedXLD2, "contlength",
                            "and", (3 * hv_W) / 4, hv_W + 20);


                        hv_SelectedRegionsNumber1.Dispose();
                        HOperatorSet.CountObj(ho_SelectedXLD2, out hv_SelectedRegionsNumber1);
                        if ((int)(new HTuple(hv_SelectedRegionsNumber1.TupleGreater(0))) != 0)
                        {

                            ho_SelectedXLD3.Dispose();
                            HOperatorSet.SelectShapeXld(ho_SelectedXLD2, out ho_SelectedXLD3, "phi_points",
                                "and", -0.5, 0.5);
                            ho_SortedContours3.Dispose();
                            HOperatorSet.SortContoursXld(ho_SelectedXLD3, out ho_SortedContours3, "upper_left",
                                "true", "row");
                            ho_ObjectSelected2.Dispose();
                            HOperatorSet.SelectObj(ho_SortedContours3, out ho_ObjectSelected2, 1);

                            ho_SmoothedContours2.Dispose();
                            HOperatorSet.SmoothContoursXld(ho_ObjectSelected2, out ho_SmoothedContours2,
                                9);
                            hv_Area2.Dispose(); hv_Row23.Dispose(); hv_Column23.Dispose();
                            HOperatorSet.AreaCenterPointsXld(ho_SmoothedContours2, out hv_Area2, out hv_Row23,
                                out hv_Column23);
                            hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                            HOperatorSet.FitLineContourXld(ho_SmoothedContours2, "tukey", -1, 0, 11,
                                2, out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd,
                                out hv_Nr, out hv_Nc, out hv_Dist);
                            hv_Distance.Dispose();
                            HOperatorSet.DistancePl(hv_Row3, hv_Column3, hv_RowBegin, hv_ColBegin,
                                hv_RowEnd, hv_ColEnd, out hv_Distance);
                            hv_resultDistance.Dispose();
                            hv_resultDistance = hv_Yscale * hv_Distance;
                            doorCrackResult.Distance = hv_resultDistance.D;

                        }


                    }
                }
                return doorCrackResult;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                hb_image.Dispose();
                ho_ImageFeng.Dispose();
                ho_ROI_0.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageMean.Dispose();
                ho_Region1.Dispose();
                ho_RegionOpening.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_RegionFillUp.Dispose();
                ho_SortedRegions.Dispose();
                ho_ObjectSelectedOne.Dispose();
                ho_Contours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_SelectedXLD.Dispose();
                ho_SelectedXLD1.Dispose();
                ho_SortedContours.Dispose();
                ho_ObjectSelected.Dispose();
                ho_SmoothedContours.Dispose();
                ho_Cross.Dispose();
                ho_ObjectSelectedtwo.Dispose();
                ho_Contours2.Dispose();
                ho_ContoursSplit2.Dispose();
                ho_SelectedXLD2.Dispose();
                ho_SelectedXLD3.Dispose();
                ho_SortedContours3.Dispose();
                ho_ObjectSelected2.Dispose();
                ho_SmoothedContours2.Dispose();
                ho_RegionLines.Dispose();

                hv_bshumenfeng.Dispose();
                hv_y0.Dispose();
                hv_x0.Dispose();
                hv_y1.Dispose();
                hv_x1.Dispose();
                hv_Yscale.Dispose();
                hv_Zscale.Dispose();
                hv_threolddata1.Dispose();
                hv_H.Dispose();
                hv_Number1.Dispose();
                hv_SelectedRegionsNumber1.Dispose();
                hv_Area.Dispose();
                hv_Row3.Dispose();
                hv_Column3.Dispose();
                hv_Area2.Dispose();
                hv_Row23.Dispose();
                hv_Column23.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_Distance.Dispose();
                hv_resultDistance.Dispose();
                hv_W.Dispose();
            }
        }
        public struct DoorCrackResult
        {
            public double Distance { get; set; }
            public Point StartPoint_L { get; set; }
            public Point EndPoint_L { get; set; }
            public Point StartPoint_R { get; set; }
            public Point EndPoint_R { get; set; }
        }
    }
}
