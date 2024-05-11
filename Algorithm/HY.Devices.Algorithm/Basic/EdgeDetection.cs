using HalconDotNet;
using System;
using System.Drawing;

namespace HY.Devices.Algorithm
{
    /// <summary>
    /// 门齐门缝检测
    /// </summary>
    public class EdgeDetection
    {
        private static readonly object _lockObj = new object();
        private static EdgeDetection _instance;
        public static EdgeDetection Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new EdgeDetection();
                        }
                    }
                }
                return _instance;
            }
        }
        /// <summary>
        /// 门齐检测
        /// </summary>
        /// <param name="hb_Image"></param>
        /// <returns></returns>
        private double GetUnevenHeight(HObject hb_Image)
        {
            HObject ho_Image, ho_Image1, ho_Image2, ho_Image3;
            HObject ho_ImageResult1, ho_ImageResult2, ho_ImageResult3;
            HObject ho_ROI_0, ho_ImageScaled, ho_Region2, ho_RegionOpening2;
            HObject ho_ConnectedRegions5, ho_SelectedRegions5, ho_Rectangle1;
            HObject ho_Region1, ho_ImageReduced, ho_EdgeAmplitude, ho_ImageConverted;
            HObject ho_ImageMax, ho_SE, ho_ImageTopHat, ho_Region, ho_ConnectedRegions3;
            HObject ho_SelectedRegions3, ho_RegionUnion, ho_RegionClosing;
            HObject ho_ConnectedRegions4, ho_SelectedRegions4, ho_RegionUnion1;
            HObject ho_RegionDilation, ho_RegionOpening, ho_RegionFillUp;
            HObject ho_RectangleWhole, ho_RegionDifference, ho_ConnectedRegions;
            HObject ho_SelectedRegions, ho_RegionOpening1, ho_ConnectedRegions1;
            HObject ho_SelectedRegions1, ho_Rectangle, ho_RegionDifference1;
            HObject ho_ConnectedRegions2, ho_SelectedRegions2, ho_ObjectSelectedOne = null;
            HObject ho_ObjectSelectedTwo = null, ho_ObjectSelected0 = null;
            HObject ho_ObjectSelected1 = null, ho_ContoursLeft = null, ho_ContoursLeftPart = null;
            HObject ho_UnionContours = null, ho_ContoursRightPart = null;
            HObject ho_SelectedXLD = null, ho_Cross = null, ho_RegionLine = null;
            HObject ho_Contours = null;

            // Local control variables 

            HTuple hv_DistanceResult = null, hv_Width = null;
            HTuple hv_Height = null, hv_Row12 = null, hv_Column12 = null;
            HTuple hv_Row22 = null, hv_Column22 = null, hv_Y = null;
            HTuple hv_X = null, hv_W = null, hv_H = null, hv_Width1 = null;
            HTuple hv_Height1 = null, hv_Row1 = null, hv_Column1 = null;
            HTuple hv_Row2 = null, hv_Column2 = null, hv_Number = null;
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_X1 = new HTuple();
            HTuple hv_X2 = new HTuple(), hv_Row11 = new HTuple(), hv_Column11 = new HTuple();
            HTuple hv_Row21 = new HTuple(), hv_Column21 = new HTuple();
            HTuple hv_RowBegin = new HTuple(), hv_ColBegin = new HTuple();
            HTuple hv_RowEnd = new HTuple(), hv_ColEnd = new HTuple();
            HTuple hv_Nr = new HTuple(), hv_Nc = new HTuple(), hv_Dist = new HTuple();
            HTuple hv_pIndexLeft = new HTuple(), hv_maxLegth = new HTuple();
            HTuple hv_i = new HTuple(), hv_Distance = new HTuple();
            HTuple hv_RowBegin0 = new HTuple(), hv_ColBegin0 = new HTuple();
            HTuple hv_RowEnd0 = new HTuple(), hv_ColEnd0 = new HTuple();
            HTuple hv_pIndexRightTop = new HTuple(), hv_RowBegin1 = new HTuple();
            HTuple hv_ColBegin1 = new HTuple(), hv_RowEnd1 = new HTuple();
            HTuple hv_ColEnd1 = new HTuple(), hv_pIndexRightLeft = new HTuple();
            HTuple hv_Row3 = new HTuple(), hv_Column3 = new HTuple();
            HTuple hv_IsOverlapping = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_ImageResult1);
            HOperatorSet.GenEmptyObj(out ho_ImageResult2);
            HOperatorSet.GenEmptyObj(out ho_ImageResult3);
            HOperatorSet.GenEmptyObj(out ho_ROI_0);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_Region2);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening2);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions5);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions5);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_EdgeAmplitude);
            HOperatorSet.GenEmptyObj(out ho_ImageConverted);
            HOperatorSet.GenEmptyObj(out ho_ImageMax);
            HOperatorSet.GenEmptyObj(out ho_SE);
            HOperatorSet.GenEmptyObj(out ho_ImageTopHat);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions4);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions4);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion1);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RectangleWhole);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelectedOne);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelectedTwo);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected0);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected1);
            HOperatorSet.GenEmptyObj(out ho_ContoursLeft);
            HOperatorSet.GenEmptyObj(out ho_ContoursLeftPart);
            HOperatorSet.GenEmptyObj(out ho_UnionContours);
            HOperatorSet.GenEmptyObj(out ho_ContoursRightPart);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_RegionLine);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            try
            {


                //*********DistanceResult为门平结果
                hv_DistanceResult = 0;
                ho_Image.Dispose();
                HOperatorSet.CopyImage(hb_Image, out ho_Image);
                // HOperatorSet.ReadImage(out ho_Image, "D:/项目/107 TCL外观检测/补光门齐/LinearCameraA_DetectDoorAlign_20220221034058.jpg");
                //read_image (Image, ImageFiles[Index])


                ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                HOperatorSet.Decompose3(ho_Image, out ho_Image1, out ho_Image2, out ho_Image3
                    );
                ho_ImageResult1.Dispose(); ho_ImageResult2.Dispose(); ho_ImageResult3.Dispose();
                HOperatorSet.TransFromRgb(ho_Image1, ho_Image2, ho_Image3, out ho_ImageResult1,
                    out ho_ImageResult2, out ho_ImageResult3, "hsv");
                HOperatorSet.GetImageSize(ho_Image1, out hv_Width, out hv_Height);

                //gen_rectangle1 (ROI_0, 1627.92, 1974.48, 2614.45, 5261.45)
                //gen_rectangle1 (ROI_0, 700.797, 1844.75, 2276.68, 5355.83)
                //gen_rectangle1 (ROI_0, 242.676, 489.043, 652.559, 2861.62)

                ho_ROI_0.Dispose();
                HOperatorSet.GenRectangle1(out ho_ROI_0, 0, 200, hv_Height - 100, hv_Width - 200);


                ho_ImageScaled.Dispose();
                HOperatorSet.ScaleImage(ho_Image1, out ho_ImageScaled, 5, 0);
                ho_Region2.Dispose();
                HOperatorSet.Threshold(ho_ImageScaled, out ho_Region2, 0, 150);
                ho_RegionOpening2.Dispose();
                HOperatorSet.OpeningRectangle1(ho_Region2, out ho_RegionOpening2, 20, 100);
                ho_ConnectedRegions5.Dispose();
                HOperatorSet.Connection(ho_RegionOpening2, out ho_ConnectedRegions5);
                ho_SelectedRegions5.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions5, out ho_SelectedRegions5, "max_area",
                    70);
                HOperatorSet.SmallestRectangle1(ho_SelectedRegions5, out hv_Row12, out hv_Column12,
                    out hv_Row22, out hv_Column22);
                ho_Rectangle1.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle1, hv_Row12, hv_Column12, hv_Row22 - 20,
                    hv_Column22);

                ho_Region1.Dispose();
                HOperatorSet.Threshold(ho_ImageScaled, out ho_Region1, 0, 120);

                HOperatorSet.RegionFeatures(ho_ROI_0, "row1", out hv_Y);
                HOperatorSet.RegionFeatures(ho_ROI_0, "column1", out hv_X);
                HOperatorSet.RegionFeatures(ho_ROI_0, "width", out hv_W);
                HOperatorSet.RegionFeatures(ho_ROI_0, "height", out hv_H);
                ho_ImageReduced.Dispose();
                HOperatorSet.CropRectangle1(ho_ImageScaled, out ho_ImageReduced, hv_Row12, hv_Column12,
                    hv_Row22 - 20, hv_Column22);

                //crop_part (ImageResult3, ImageReduced, Row12, Column12, Row22-20, Column22)
                ho_EdgeAmplitude.Dispose();
                HOperatorSet.SobelAmp(ho_ImageReduced, out ho_EdgeAmplitude, "y", 9);
                ho_ImageConverted.Dispose();
                HOperatorSet.ConvertImageType(ho_EdgeAmplitude, out ho_ImageConverted, "byte");
                ho_ImageMax.Dispose();
                HOperatorSet.MaxImage(ho_ImageReduced, ho_ImageConverted, out ho_ImageMax);
                ho_SE.Dispose();
                HOperatorSet.GenDiscSe(out ho_SE, "byte", 1, 19, 0);
                ho_ImageTopHat.Dispose();
                HOperatorSet.GrayTophat(ho_ImageReduced, ho_SE, out ho_ImageTopHat);
                //log_image (ImageReduced, LogImage, 'e')
                //sobel_amp (LogImage, EdgeAmplitude, 'y', 7)
                //abs_image (EdgeAmplitude, ImageAbs)

                HOperatorSet.GetImageSize(ho_ImageReduced, out hv_Width1, out hv_Height1);
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, 160, 255);
                ho_ConnectedRegions3.Dispose();
                HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions3);
                ho_SelectedRegions3.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions3, out ho_SelectedRegions3, "area",
                    "and", 1000, 999999999);
                ho_RegionUnion.Dispose();
                HOperatorSet.Union1(ho_SelectedRegions3, out ho_RegionUnion);

                //opening_rectangle1 (RegionUnion, RegionOpening3, 10, 10)
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingRectangle1(ho_RegionUnion, out ho_RegionClosing, 10, 10);
                ho_ConnectedRegions4.Dispose();
                HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions4);
                ho_SelectedRegions4.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions4, out ho_SelectedRegions4, "area",
                    "and", 10000, 9999999);
                ho_RegionUnion1.Dispose();
                HOperatorSet.Union1(ho_SelectedRegions4, out ho_RegionUnion1);

                //dilation_circle (Region, RegionDilation, 3.5)
                ho_RegionDilation.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionUnion1, out ho_RegionDilation, 8);
                //erosion_circle (RegionDilation, RegionErosion, 3.5)
                //opening_circle (RegionDilation, RegionOpening, 3.5)
                ho_RegionOpening.Dispose();
                HOperatorSet.OpeningRectangle1(ho_RegionDilation, out ho_RegionOpening, 50, 5);

                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionOpening, out ho_RegionFillUp);
                ho_RectangleWhole.Dispose();
                HOperatorSet.GenRectangle1(out ho_RectangleWhole, 0, 0, hv_Height1 - 1, hv_Width1 - 1);
                ho_RegionDifference.Dispose();
                HOperatorSet.Difference(ho_RectangleWhole, ho_RegionFillUp, out ho_RegionDifference
                    );
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionDifference, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                    70);
                ho_RegionOpening1.Dispose();
                HOperatorSet.OpeningRectangle1(ho_SelectedRegions, out ho_RegionOpening1, 5,
                    5);
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_RegionOpening1, out ho_ConnectedRegions1);
                ho_SelectedRegions1.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions1, out ho_SelectedRegions1, "max_area",
                    70);
                HOperatorSet.SmallestRectangle1(ho_SelectedRegions1, out hv_Row1, out hv_Column1,
                    out hv_Row2, out hv_Column2);
                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row1 + 5, hv_Column1 + 5, hv_Row2 - 5,
                    hv_Column2 - 5);
                ho_RegionDifference1.Dispose();
                HOperatorSet.Difference(ho_Rectangle, ho_SelectedRegions, out ho_RegionDifference1
                    );
                ho_ConnectedRegions2.Dispose();
                HOperatorSet.Connection(ho_RegionDifference1, out ho_ConnectedRegions2);
                ho_SelectedRegions2.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions2, out ho_SelectedRegions2, "area",
                    "and", 500, 99999999);
                HOperatorSet.CountObj(ho_SelectedRegions2, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleGreaterEqual(2))) != 0)
                {
                    HOperatorSet.AreaCenter(ho_SelectedRegions2, out hv_Area, out hv_Row, out hv_Column);
                    HOperatorSet.TupleSortIndex(hv_Area, out hv_Indices);
                    ho_ObjectSelectedOne.Dispose();
                    HOperatorSet.SelectObj(ho_SelectedRegions2, out ho_ObjectSelectedOne, (hv_Indices.TupleSelect(
                        hv_Number - 1)) + 1);
                    ho_ObjectSelectedTwo.Dispose();
                    HOperatorSet.SelectObj(ho_SelectedRegions2, out ho_ObjectSelectedTwo, (hv_Indices.TupleSelect(
                        hv_Number - 2)) + 1);
                    HOperatorSet.RegionFeatures(ho_ObjectSelectedOne, "column1", out hv_X1);
                    HOperatorSet.RegionFeatures(ho_ObjectSelectedTwo, "column1", out hv_X2);
                    if ((int)(new HTuple(hv_X1.TupleLess(hv_X2))) != 0)
                    {
                        ho_ObjectSelected0.Dispose();
                        ho_ObjectSelected0 = ho_ObjectSelectedOne.CopyObj(1, -1);
                        ho_ObjectSelected1.Dispose();
                        ho_ObjectSelected1 = ho_ObjectSelectedTwo.CopyObj(1, -1);
                    }
                    else
                    {
                        ho_ObjectSelected0.Dispose();
                        ho_ObjectSelected0 = ho_ObjectSelectedTwo.CopyObj(1, -1);
                        ho_ObjectSelected1.Dispose();
                        ho_ObjectSelected1 = ho_ObjectSelectedOne.CopyObj(1, -1);
                    }
                    HOperatorSet.SmallestRectangle1(ho_ObjectSelected0, out hv_Row11, out hv_Column11,
                        out hv_Row21, out hv_Column21);
                    ho_ContoursLeft.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_ObjectSelected0, out ho_ContoursLeft, "border");
                    ho_ContoursLeftPart.Dispose();
                    HOperatorSet.ClipContoursXld(ho_ContoursLeft, out ho_ContoursLeftPart, hv_Row11,
                        hv_Column11 + 10, hv_Row11 + 20, hv_Column21 - 10);
                    ho_UnionContours.Dispose();
                    HOperatorSet.UnionAdjacentContoursXld(ho_ContoursLeftPart, out ho_UnionContours,
                        50, 1, "attr_keep");
                    HOperatorSet.FitLineContourXld(ho_UnionContours, "tukey", -1, 0, 5, 2, out hv_RowBegin,
                        out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr, out hv_Nc, out hv_Dist);
                    hv_pIndexLeft = 0;
                    hv_maxLegth = 0;
                    for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_RowBegin.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                    {
                        HOperatorSet.DistancePp(hv_RowBegin.TupleSelect(hv_i), hv_ColBegin.TupleSelect(
                            hv_i), hv_RowEnd.TupleSelect(hv_i), hv_ColEnd.TupleSelect(hv_i), out hv_Distance);
                        if ((int)(new HTuple(hv_Distance.TupleGreater(hv_maxLegth))) != 0)
                        {
                            hv_maxLegth = hv_Distance.Clone();
                            hv_pIndexLeft = hv_i.Clone();
                        }
                    }
                    HOperatorSet.SmallestRectangle1(ho_ObjectSelected1, out hv_Row11, out hv_Column11,
                        out hv_Row21, out hv_Column21);
                    ho_ContoursLeft.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_ObjectSelected1, out ho_ContoursLeft, "border");
                    ho_ContoursRightPart.Dispose();
                    HOperatorSet.ClipContoursXld(ho_ContoursLeft, out ho_ContoursRightPart, hv_Row11,
                        hv_Column11 + 10, hv_Row11 + 20, hv_Column21 - 10);
                    ho_UnionContours.Dispose();
                    HOperatorSet.UnionAdjacentContoursXld(ho_ContoursRightPart, out ho_UnionContours,
                        50, 1, "attr_keep");
                    ho_SelectedXLD.Dispose();
                    HOperatorSet.SelectShapeXld(ho_UnionContours, out ho_SelectedXLD, "contlength",
                        "and", 150, 99999);

                    HOperatorSet.FitLineContourXld(ho_SelectedXLD, "tukey", -1, 0, 5, 2, out hv_RowBegin0,
                        out hv_ColBegin0, out hv_RowEnd0, out hv_ColEnd0, out hv_Nr, out hv_Nc,
                        out hv_Dist);
                    hv_pIndexRightTop = 0;
                    hv_maxLegth = 0;
                    for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_RowBegin0.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                    {
                        HOperatorSet.DistancePp(hv_RowBegin0.TupleSelect(hv_i), hv_ColBegin0.TupleSelect(
                            hv_i), hv_RowEnd0.TupleSelect(hv_i), hv_ColEnd0.TupleSelect(hv_i), out hv_Distance);
                        if ((int)(new HTuple(hv_Distance.TupleGreater(hv_maxLegth))) != 0)
                        {
                            hv_maxLegth = hv_Distance.Clone();
                            hv_pIndexRightTop = hv_i.Clone();
                        }
                    }

                    ho_ContoursRightPart.Dispose();
                    HOperatorSet.ClipContoursXld(ho_ContoursLeft, out ho_ContoursRightPart, hv_Row11 + 10,
                        hv_Column11, hv_Row21 - 10, hv_Column11 + 60);
                    ho_UnionContours.Dispose();
                    HOperatorSet.UnionAdjacentContoursXld(ho_ContoursRightPart, out ho_UnionContours,
                        50, 1, "attr_keep");
                    HOperatorSet.FitLineContourXld(ho_UnionContours, "tukey", -1, 0, 5, 2, out hv_RowBegin1,
                        out hv_ColBegin1, out hv_RowEnd1, out hv_ColEnd1, out hv_Nr, out hv_Nc,
                        out hv_Dist);
                    hv_pIndexRightLeft = 0;
                    hv_maxLegth = 0;
                    for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_RowBegin1.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                    {
                        HOperatorSet.DistancePp(hv_RowBegin1.TupleSelect(hv_i), hv_ColBegin1.TupleSelect(
                            hv_i), hv_RowEnd1.TupleSelect(hv_i), hv_ColEnd1.TupleSelect(hv_i), out hv_Distance);
                        if ((int)(new HTuple(hv_Distance.TupleGreater(hv_maxLegth))) != 0)
                        {
                            hv_maxLegth = hv_Distance.Clone();
                            hv_pIndexRightLeft = hv_i.Clone();
                        }
                    }
                    // HOperatorSet.SetLineWidth(hv_ExpDefaultWinHandle, 5);
                    if ((int)(new HTuple(hv_RowBegin0.TupleNotEqual(new HTuple()))) != 0)
                    {
                        HOperatorSet.IntersectionLines(hv_RowBegin0.TupleSelect(hv_pIndexRightTop),
                            hv_ColBegin0.TupleSelect(hv_pIndexRightTop), hv_RowEnd0.TupleSelect(hv_pIndexRightTop),
                            hv_ColEnd0.TupleSelect(hv_pIndexRightTop), hv_RowBegin1.TupleSelect(hv_pIndexRightLeft),
                            hv_ColBegin1.TupleSelect(hv_pIndexRightLeft), hv_RowEnd1.TupleSelect(
                            hv_pIndexRightLeft), hv_ColEnd1.TupleSelect(hv_pIndexRightLeft), out hv_Row3,
                            out hv_Column3, out hv_IsOverlapping);
                        //HOperatorSet.ClearWindow(hv_ExpDefaultWinHandle);
                        //HOperatorSet.DispObj(ho_ImageReduced, hv_ExpDefaultWinHandle);
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row3, hv_Column3, 20, 0);

                        ho_RegionLine.Dispose();
                        HOperatorSet.GenRegionLine(out ho_RegionLine, hv_RowBegin.TupleSelect(hv_pIndexLeft),
                            hv_ColBegin.TupleSelect(hv_pIndexLeft), hv_RowEnd.TupleSelect(hv_pIndexLeft),
                            hv_ColEnd.TupleSelect(hv_pIndexLeft));
                        ho_Contours.Dispose();
                        HOperatorSet.GenContourRegionXld(ho_RegionLine, out ho_Contours, "center");

                        HOperatorSet.DistancePl(hv_Row3, hv_Column3, hv_RowBegin.TupleSelect(hv_pIndexLeft),
                            hv_ColBegin.TupleSelect(hv_pIndexLeft), hv_RowEnd.TupleSelect(hv_pIndexLeft),
                            hv_ColEnd.TupleSelect(hv_pIndexLeft), out hv_DistanceResult);
                        return hv_DistanceResult.D;
                    }
                    else
                    {
                        return 0;
                        //HOperatorSet.ClearWindow(hv_ExpDefaultWinHandle);
                        //HOperatorSet.DispObj(ho_ImageReduced, hv_ExpDefaultWinHandle);
                    }

                }
                else
                {
                    return 0;
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
                ho_Image2.Dispose();
                ho_Image3.Dispose();
                ho_ImageResult1.Dispose();
                ho_ImageResult2.Dispose();
                ho_ImageResult3.Dispose();
                ho_ROI_0.Dispose();
                ho_ImageScaled.Dispose();
                ho_Region2.Dispose();
                ho_RegionOpening2.Dispose();
                ho_ConnectedRegions5.Dispose();
                ho_SelectedRegions5.Dispose();
                ho_Rectangle1.Dispose();
                ho_Region1.Dispose();
                ho_ImageReduced.Dispose();
                ho_EdgeAmplitude.Dispose();
                ho_ImageConverted.Dispose();
                ho_ImageMax.Dispose();
                ho_SE.Dispose();
                ho_ImageTopHat.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions3.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionClosing.Dispose();
                ho_ConnectedRegions4.Dispose();
                ho_SelectedRegions4.Dispose();
                ho_RegionUnion1.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RectangleWhole.Dispose();
                ho_RegionDifference.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionOpening1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_Rectangle.Dispose();
                ho_RegionDifference1.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_ObjectSelectedOne.Dispose();
                ho_ObjectSelectedTwo.Dispose();
                ho_ObjectSelected0.Dispose();
                ho_ObjectSelected1.Dispose();
                ho_ContoursLeft.Dispose();
                ho_ContoursLeftPart.Dispose();
                ho_UnionContours.Dispose();
                ho_ContoursRightPart.Dispose();
                ho_SelectedXLD.Dispose();
                ho_Cross.Dispose();
                ho_RegionLine.Dispose();
                ho_Contours.Dispose();
            }
        }
        public double GetUnevenHeight(string ImagePath)
        {
            try
            {
                HOperatorSet.ReadImage(out HObject hb_Image, ImagePath);
                return GetUnevenHeight(hb_Image);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public double GetUnevenHeight(Bitmap bitmap)
        {
            try
            {
                return GetUnevenHeight(Utils.Ho_ImageHelper.Bitmap2HObject(bitmap));
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 门缝检测
        /// </summary>
        /// <param name="hb_Image"></param>
        /// <param name="DoorType"></param>
        /// <param name="Gray"></param>
        /// <returns></returns>
        private double GetDoorCrack(HObject hb_Image, int DoorType, int Gray)
        {
            // Local iconic variables 

            HObject ho_Image, ho_Image1, ho_Image2, ho_Image3;
            HObject ho_ImageReduced1, ho_Region1, ho_RegionOpening2;
            HObject ho_RegionFillUp, ho_ConnectedRegions, ho_SelectedRegions3;
            HObject ho_SelectedRegions4, ho_Contours = null, ho_CroppedContours = null;
            HObject ho_UnionContours = null, ho_SelectedXLD = null, ho_Regions = null;
            HObject ho_ObjectSelected = null, ho_sel0 = null, ho_sel1 = null;
            HObject ho_RegionLine0 = null, ho_RegionLine1 = null;

            // Local control variables 

            HTuple hv_Abs = null, hv_dLowThre = null, hv_dHeightThre = null;
            HTuple hv_Width = null, hv_Height = null, hv_Number1 = null;
            HTuple hv_Row12 = new HTuple(), hv_Column12 = new HTuple();
            HTuple hv_Row22 = new HTuple(), hv_Column22 = new HTuple();
            HTuple hv_RMin = new HTuple(), hv_RMax = new HTuple();
            HTuple hv_CMin = new HTuple(), hv_CMax = new HTuple();
            HTuple hv_NumberRegion = new HTuple(), hv_Row0 = new HTuple();
            HTuple hv_Column0 = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_nLength = new HTuple();
            HTuple hv_leftRow = new HTuple(), hv_leftCol = new HTuple();
            HTuple hv_Rows = new HTuple(), hv_Columns = new HTuple();
            HTuple hv_i = new HTuple(), hv_pp = new HTuple(), hv_rightRow = new HTuple();
            HTuple hv_rightCol = new HTuple(), hv_Value = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_RowBegin1 = new HTuple();
            HTuple hv_ColBegin1 = new HTuple(), hv_RowEnd1 = new HTuple();
            HTuple hv_ColEnd1 = new HTuple(), hv_Nr = new HTuple();
            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_RowBegin2 = new HTuple();
            HTuple hv_ColBegin2 = new HTuple(), hv_RowEnd2 = new HTuple();
            HTuple hv_ColEnd2 = new HTuple(), hv_x = new HTuple();
            HTuple hv_k = new HTuple(), hv_b = new HTuple(), hv_y0 = new HTuple();
            HTuple hv_x0 = new HTuple(), hv_y1 = new HTuple(), hv_x1 = new HTuple();
            HTuple hv_Distance1 = new HTuple(), hv_Distance2 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening2);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions3);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions4);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_CroppedContours);
            HOperatorSet.GenEmptyObj(out ho_UnionContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_sel0);
            HOperatorSet.GenEmptyObj(out ho_sel1);
            HOperatorSet.GenEmptyObj(out ho_RegionLine0);
            HOperatorSet.GenEmptyObj(out ho_RegionLine1);
            try
            {
                //*************************************************************************Abs门缝差值
                hv_Abs = 0;
                hv_dLowThre = 66;
                hv_dHeightThre = 150;
                //****0  板面比门缝亮   1 板面比门缝暗


                ho_Image.Dispose();
                // HOperatorSet.ReadImage(out ho_Image, "");
                HOperatorSet.CopyImage(hb_Image, out ho_Image);
                ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                HOperatorSet.Decompose3(ho_Image, out ho_Image1, out ho_Image2, out ho_Image3
                    );
                ho_ImageReduced1.Dispose();
                HOperatorSet.CopyImage(ho_Image1, out ho_ImageReduced1);
                HOperatorSet.GetImageSize(ho_ImageReduced1, out hv_Width, out hv_Height);
                //if (ptype==0)
                //invert_image (ImageReduced1, ImageInvert)
                //threshold (ImageInvert, Region1, 180, 255)
                //else
                //threshold (ImageReduced1, Region1, 30, 255)
                //endif
                ho_Region1.Dispose();
                HOperatorSet.Threshold(ho_ImageReduced1, out ho_Region1, hv_dLowThre, hv_dHeightThre);
                ho_RegionOpening2.Dispose();
                HOperatorSet.ClosingCircle(ho_Region1, out ho_RegionOpening2, 5);
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionOpening2, out ho_RegionFillUp);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);

                ho_SelectedRegions3.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions3, "height",
                    "and", hv_Height * 0.65, hv_Height * 1.05);
                ho_SelectedRegions4.Dispose();
                HOperatorSet.SelectShape(ho_SelectedRegions3, out ho_SelectedRegions4, "area",
                    "and", 1000, hv_Height * hv_Width);
                //select_shape (SelectedRegions4, SelectedRegions4, 'inner_width', 'and', 30, Width)
                HOperatorSet.CountObj(ho_SelectedRegions4, out hv_Number1);
                if ((int)(new HTuple(hv_Number1.TupleGreaterEqual(1))) != 0)
                {

                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_SelectedRegions4, out ho_Contours, "border");
                    HOperatorSet.SmallestRectangle1Xld(ho_Contours, out hv_Row12, out hv_Column12,
                        out hv_Row22, out hv_Column22);
                    HOperatorSet.TupleMax(hv_Row12, out hv_RMin);
                    HOperatorSet.TupleMin(hv_Row22, out hv_RMax);
                    HOperatorSet.TupleMin(hv_Column12, out hv_CMin);
                    HOperatorSet.TupleMax(hv_Column22, out hv_CMax);
                    ho_CroppedContours.Dispose();
                    HOperatorSet.CropContoursXld(ho_Contours, out ho_CroppedContours, hv_RMin + 10,
                        hv_CMin, hv_RMax - 10, hv_CMax, "false");
                    ho_UnionContours.Dispose();
                    HOperatorSet.UnionAdjacentContoursXld(ho_CroppedContours, out ho_UnionContours,
                        50, 10, "attr_keep");
                    ho_SelectedXLD.Dispose();
                    HOperatorSet.SelectShapeXld(ho_UnionContours, out ho_SelectedXLD, "height",
                        "and", hv_Height * 0.65, hv_Height * 1.05);
                    ho_Regions.Dispose();
                    HOperatorSet.GenRegionContourXld(ho_SelectedXLD, out ho_Regions, "margin");
                    HOperatorSet.CountObj(ho_Regions, out hv_NumberRegion);
                    if ((int)(new HTuple(hv_NumberRegion.TupleEqual(1))) != 0)
                    {
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Regions, out ho_ObjectSelected, 1);
                        HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out hv_Row0, out hv_Column0,
                            out hv_Row1, out hv_Column1);
                        hv_nLength = (hv_Row1 - hv_Row0) + 1;
                        HOperatorSet.TupleGenSequence(hv_Row0, hv_Row1, 1, out hv_leftRow);
                        //tuple_gen_const (nLength, 0, leftRow)
                        HOperatorSet.TupleGenConst(hv_nLength, hv_Width, out hv_leftCol);
                        HOperatorSet.GetRegionPoints(ho_ObjectSelected, out hv_Rows, out hv_Columns);
                        for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Rows.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                        {
                            hv_pp = (hv_Rows.TupleSelect(hv_i)) - hv_Row0;
                            if ((int)(new HTuple(((hv_leftCol.TupleSelect(hv_pp))).TupleGreater(hv_Columns.TupleSelect(
                                hv_i)))) != 0)
                            {
                                if (hv_leftCol == null)
                                    hv_leftCol = new HTuple();
                                hv_leftCol[hv_pp] = hv_Columns.TupleSelect(hv_i);
                            }
                        }
                        //for i := 0 to nLength-1 by 1
                        //leftRow[i] := Row0+i
                        //endfor
                        //***********right
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Regions, out ho_ObjectSelected, 1);
                        HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out hv_Row0, out hv_Column0,
                            out hv_Row1, out hv_Column1);
                        hv_nLength = (hv_Row1 - hv_Row0) + 1;
                        HOperatorSet.TupleGenSequence(hv_Row0, hv_Row1, 1, out hv_rightRow);
                        //tuple_gen_const (nLength, 0, rightRow)
                        HOperatorSet.TupleGenConst(hv_nLength, 0, out hv_rightCol);
                        HOperatorSet.GetRegionPoints(ho_ObjectSelected, out hv_Rows, out hv_Columns);
                        for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Rows.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                        {
                            hv_pp = (hv_Rows.TupleSelect(hv_i)) - hv_Row0;
                            if ((int)(new HTuple(((hv_rightCol.TupleSelect(hv_pp))).TupleLess(hv_Columns.TupleSelect(
                                hv_i)))) != 0)
                            {
                                if (hv_rightCol == null)
                                    hv_rightCol = new HTuple();
                                hv_rightCol[hv_pp] = hv_Columns.TupleSelect(hv_i);
                            }
                        }
                        //for i := 0 to nLength-1 by 1
                        //rightRow[i] := Row0+i
                        //endfor
                    }
                    else
                    {
                        HOperatorSet.RegionFeatures(ho_Regions, "column1", out hv_Value);
                        HOperatorSet.TupleSortIndex(hv_Value, out hv_Indices);
                        //***********left
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Regions, out ho_ObjectSelected, (hv_Indices.TupleSelect(
                            0)) + 1);
                        HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out hv_Row0, out hv_Column0,
                            out hv_Row1, out hv_Column1);
                        hv_nLength = (hv_Row1 - hv_Row0) + 1;
                        HOperatorSet.TupleGenSequence(hv_Row0, hv_Row1, 1, out hv_leftRow);
                        //tuple_gen_const (nLength, 0, leftRow)
                        HOperatorSet.TupleGenConst(hv_nLength, hv_Width, out hv_leftCol);
                        HOperatorSet.GetRegionPoints(ho_ObjectSelected, out hv_Rows, out hv_Columns);


                        for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Rows.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                        {
                            hv_pp = (hv_Rows.TupleSelect(hv_i)) - hv_Row0;
                            if ((int)(new HTuple(((hv_leftCol.TupleSelect(hv_pp))).TupleGreater(hv_Columns.TupleSelect(
                                hv_i)))) != 0)
                            {
                                if (hv_leftCol == null)
                                    hv_leftCol = new HTuple();
                                hv_leftCol[hv_pp] = hv_Columns.TupleSelect(hv_i);
                            }
                        }



                        //for i := 0 to nLength-1 by 1
                        //leftRow[i] := Row0+i
                        //endfor
                        //********************right
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Regions, out ho_ObjectSelected, (hv_Indices.TupleSelect(
                            hv_NumberRegion - 1)) + 1);
                        HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out hv_Row0, out hv_Column0,
                            out hv_Row1, out hv_Column1);
                        hv_nLength = (hv_Row1 - hv_Row0) + 1;
                        HOperatorSet.TupleGenSequence(hv_Row0, hv_Row1, 1, out hv_rightRow);
                        //tuple_gen_const (nLength, 0, rightRow)
                        HOperatorSet.TupleGenConst(hv_nLength, 0, out hv_rightCol);
                        HOperatorSet.GetRegionPoints(ho_ObjectSelected, out hv_Rows, out hv_Columns);
                        for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Rows.TupleLength())) - 1); hv_i = (int)hv_i + 1)
                        {
                            hv_pp = (hv_Rows.TupleSelect(hv_i)) - hv_Row0;
                            if ((int)(new HTuple(((hv_rightCol.TupleSelect(hv_pp))).TupleLess(hv_Columns.TupleSelect(
                                hv_i)))) != 0)
                            {
                                if (hv_rightCol == null)
                                    hv_rightCol = new HTuple();
                                hv_rightCol[hv_pp] = hv_Columns.TupleSelect(hv_i);
                            }
                        }
                        //for i := 0 to nLength-1 by 1
                        //rightRow[i] := Row0+i
                        //endfor
                    }

                    ho_sel0.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_sel0, hv_leftRow, hv_leftCol);
                    ho_sel1.Dispose();
                    HOperatorSet.GenContourPolygonXld(out ho_sel1, hv_rightRow, hv_rightCol);
                    HOperatorSet.FitLineContourXld(ho_sel0, "tukey", -1, 0, 5, 2, out hv_RowBegin1,
                        out hv_ColBegin1, out hv_RowEnd1, out hv_ColEnd1, out hv_Nr, out hv_Nc,
                        out hv_Dist);
                    HOperatorSet.FitLineContourXld(ho_sel1, "tukey", -1, 0, 5, 2, out hv_RowBegin2,
                        out hv_ColBegin2, out hv_RowEnd2, out hv_ColEnd2, out hv_Nr, out hv_Nc,
                        out hv_Dist);
                    //HOperatorSet.ClearWindow(hv_ExpDefaultWinHandle);
                    //HOperatorSet.DispObj(ho_ImageReduced1, hv_ExpDefaultWinHandle);
                    ho_RegionLine0.Dispose();
                    HOperatorSet.GenRegionLine(out ho_RegionLine0, hv_RowBegin1, hv_ColBegin1,
                        hv_RowEnd1, hv_ColEnd1);
                    ho_RegionLine1.Dispose();
                    HOperatorSet.GenRegionLine(out ho_RegionLine1, hv_RowBegin2, hv_ColBegin2,
                        hv_RowEnd2, hv_ColEnd2);
                    hv_x = hv_ColEnd1 - hv_ColBegin1;
                    if ((int)(new HTuple(hv_x.TupleEqual(0))) != 0)
                    {
                        hv_k = 0;
                        hv_b = hv_ColEnd1.Clone();
                        hv_y0 = 0;
                        hv_x0 = hv_ColBegin1.Clone();
                        hv_y1 = hv_Height - 1;
                        hv_x1 = hv_ColBegin1.Clone();
                    }
                    else
                    {
                        hv_k = (hv_RowEnd1 - hv_RowBegin1) / (hv_ColEnd1 - hv_ColBegin1);
                        hv_b = hv_RowEnd1 - (hv_k * hv_ColEnd1);
                        hv_y0 = 0;
                        hv_x0 = (hv_y0 - hv_b) / hv_k;
                        hv_y1 = hv_Height - 1;
                        hv_x1 = (hv_y1 - hv_b) / hv_k;
                    }

                    HOperatorSet.DistancePl(hv_y0, hv_x0, hv_RowBegin2, hv_ColBegin2, hv_RowEnd2,
                        hv_ColEnd2, out hv_Distance1);
                    HOperatorSet.DistancePl(hv_y1, hv_x1, hv_RowBegin2, hv_ColBegin2, hv_RowEnd2,
                        hv_ColEnd2, out hv_Distance2);

                    //distance_pl (RowBegin1, ColBegin1, RowBegin2, ColBegin2, RowEnd2, ColEnd2, Distance1)
                    //distance_pl (RowEnd1, ColEnd1, RowBegin2, ColBegin2, RowEnd2, ColEnd2, Distance2)
                    HOperatorSet.TupleAbs(hv_Distance1 - hv_Distance2, out hv_Abs);
                    return hv_Abs.D;

                }
                else
                {
                    return 0;
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
                ho_Image2.Dispose();
                ho_Image3.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Region1.Dispose();
                ho_RegionOpening2.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions3.Dispose();
                ho_SelectedRegions4.Dispose();
                ho_Contours.Dispose();
                ho_CroppedContours.Dispose();
                ho_UnionContours.Dispose();
                ho_SelectedXLD.Dispose();
                ho_Regions.Dispose();
                ho_ObjectSelected.Dispose();
                ho_sel0.Dispose();
                ho_sel1.Dispose();
                ho_RegionLine0.Dispose();
                ho_RegionLine1.Dispose();
            }

        }

        /// <summary>
        /// 门缝检测
        /// </summary>
        /// <param name="ImagePath">图片路径</param>
        /// <param name="DoorType">门体类型（0表示亮门体 1表示暗门体）</param>
        /// <param name="Gray">灰度值（0-255）</param>
        /// <returns></returns>
        public double GetDoorCrack(string ImagePath, int DoorType, int Gray)
        {
            try
            {
                HOperatorSet.ReadImage(out HObject hb_Image, ImagePath);
                return GetDoorCrack(hb_Image, DoorType, Gray);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public double GetDoorCrack(Bitmap bitmap, int DoorType, int Gray)
        {
            try
            {
                return GetDoorCrack(Utils.Ho_ImageHelper.Bitmap2HObject(bitmap), DoorType, Gray);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}

