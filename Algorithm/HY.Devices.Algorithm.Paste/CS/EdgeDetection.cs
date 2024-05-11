using HalconDotNet;
using HYCommonUtils.SerializationUtils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace HY.Devices.Algorithm.Paste.CS
{
    /// <summary>
    /// 边缘检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class EdgeDetection : AbstractAlgorithm, IAlgorithm
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
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.EdgeDetection;

        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Type", 0 }, { "Image", "" } };


        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            return IsInit = true;
        }

        public override void UnInit()
        {
            IsInit = false;
        }
        private HTuple hv_ExpDefaultWinHandle;

        // Main procedure 
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {

            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
            // Local iconic variables 

            HObject ho_Image, ho_ImageMedian = null, ho_Region = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;
            HObject ho_RegionUnion = null, ho_RegionFillUp = null, ho_RegionClosing = null;
            HObject ho_Contours = null, ho_SmoothedContours = null, ho_ContoursSplit = null;
            HObject ho_SortedContours = null, ho_ObjectSelected = null;
            HObject ho_Cross = null, ho_RegionLines = null;

            // Local control variables 

            HTuple hv_ResAngle = new HTuple(), hv_Col1 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Col2 = new HTuple();
            HTuple hv_Row2 = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Col = new HTuple(), hv_a = new HTuple();
            HTuple hv_Angle1 = new HTuple(), hv_Deg = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageMedian);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_SmoothedContours);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
            HOperatorSet.GenEmptyObj(out ho_SortedContours);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_Cross);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            try
            {
                ho_Image.Dispose();
                ho_Image = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);

                hv_ResAngle.Dispose();
                hv_ResAngle = 0;
                hv_Col1.Dispose();
                hv_Col1 = 0;
                hv_Row1.Dispose();
                hv_Row1 = 0;
                hv_Col2.Dispose();
                hv_Col2 = 0;
                hv_Row2.Dispose();
                hv_Row2 = 0;

                if (actionParams["Type"] == 0)
                {
                    ho_ImageMedian.Dispose();
                    HOperatorSet.MedianImage(ho_Image, out ho_ImageMedian, "circle", 1, "mirrored");
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, 120, 255);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 3000, 999999999);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_RegionUnion, out ho_RegionFillUp);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionFillUp, out ho_RegionClosing, 3.5);
                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_RegionClosing, out ho_Contours, "border");
                    ho_SmoothedContours.Dispose();
                    HOperatorSet.SmoothContoursXld(ho_Contours, out ho_SmoothedContours, 5);
                    ho_ContoursSplit.Dispose();
                    HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines",
                        5, 20, 2);
                    ho_SortedContours.Dispose();
                    HOperatorSet.SortContoursXld(ho_ContoursSplit, out ho_SortedContours, "character",
                        "true", "row");
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected, 4);
                    hv_Row.Dispose(); hv_Col.Dispose();
                    HOperatorSet.GetContourXld(ho_ObjectSelected, out hv_Row, out hv_Col);
                    hv_a.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_a = HTuple.TupleGenSequence(
                            400, new HTuple(hv_Row.TupleLength()), 400);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row.TupleSelect(hv_a), hv_Col.TupleSelect(
                            hv_a), 60, 0.785398);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_RegionLines.Dispose();
                        HOperatorSet.GenRegionLine(out ho_RegionLines, hv_Row.TupleSelect(hv_a.TupleSelect(
                            0)), hv_Col.TupleSelect(hv_a.TupleSelect(0)), hv_Row.TupleSelect(hv_a.TupleSelect(
                            (new HTuple(hv_a.TupleLength())) - 1)), hv_Col.TupleSelect(hv_a.TupleSelect(
                            (new HTuple(hv_a.TupleLength())) - 1)));
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Angle1.Dispose();
                        HOperatorSet.AngleLx(hv_Row.TupleSelect(hv_a.TupleSelect(0)), hv_Col.TupleSelect(
                            hv_a.TupleSelect(0)), hv_Row.TupleSelect(hv_a.TupleSelect((new HTuple(hv_a.TupleLength()
                            )) - 1)), hv_Col.TupleSelect(hv_a.TupleSelect((new HTuple(hv_a.TupleLength()
                            )) - 1)), out hv_Angle1);
                    }
                    hv_Deg.Dispose();
                    HOperatorSet.TupleDeg(hv_Angle1, out hv_Deg);
                    hv_ResAngle.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ResAngle = hv_Deg + 90;
                    }
                    hv_Col1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col1 = hv_Row.TupleSelect(
                            hv_a.TupleSelect(0));
                    }
                    hv_Row1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row1 = hv_Col.TupleSelect(
                            hv_a.TupleSelect(0));
                    }
                    hv_Col2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col2 = hv_Row.TupleSelect(
                            hv_a.TupleSelect((new HTuple(hv_a.TupleLength())) - 1));
                    }
                    hv_Row2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row2 = hv_Col.TupleSelect(
                            hv_a.TupleSelect((new HTuple(hv_a.TupleLength())) - 1));
                    }

                }
                else if (actionParams["Type"] == 1)
                {
                    ho_ImageMedian.Dispose();
                    HOperatorSet.MedianImage(ho_Image, out ho_ImageMedian, "circle", 4, "mirrored");
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageMedian, out ho_Region, 160, 255);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 3000, 999999999);
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_RegionUnion, out ho_RegionFillUp);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_RegionFillUp, out ho_RegionClosing, 3.5);
                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_RegionClosing, out ho_Contours, "border");
                    ho_SmoothedContours.Dispose();
                    HOperatorSet.SmoothContoursXld(ho_Contours, out ho_SmoothedContours, 5);
                    ho_ContoursSplit.Dispose();
                    HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines",
                        5, 20, 2);
                    ho_SortedContours.Dispose();
                    HOperatorSet.SortContoursXld(ho_ContoursSplit, out ho_SortedContours, "character",
                        "true", "row");
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected, 3);
                    hv_Row.Dispose(); hv_Col.Dispose();
                    HOperatorSet.GetContourXld(ho_ObjectSelected, out hv_Row, out hv_Col);
                    hv_a.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_a = HTuple.TupleGenSequence(
                            50, new HTuple(hv_Row.TupleLength()), 400);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Cross.Dispose();
                        HOperatorSet.GenCrossContourXld(out ho_Cross, hv_Row.TupleSelect(hv_a), hv_Col.TupleSelect(
                            hv_a), 60, 0.785398);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_RegionLines.Dispose();
                        HOperatorSet.GenRegionLine(out ho_RegionLines, hv_Row.TupleSelect(hv_a.TupleSelect(
                            0)), hv_Col.TupleSelect(hv_a.TupleSelect(0)), hv_Row.TupleSelect(hv_a.TupleSelect(
                            (new HTuple(hv_a.TupleLength())) - 1)), hv_Col.TupleSelect(hv_a.TupleSelect(
                            (new HTuple(hv_a.TupleLength())) - 1)));
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Angle1.Dispose();
                        HOperatorSet.AngleLx(hv_Row.TupleSelect(hv_a.TupleSelect(0)), hv_Col.TupleSelect(
                            hv_a.TupleSelect(0)), hv_Row.TupleSelect(hv_a.TupleSelect((new HTuple(hv_a.TupleLength()
                            )) - 1)), hv_Col.TupleSelect(hv_a.TupleSelect((new HTuple(hv_a.TupleLength()
                            )) - 1)), out hv_Angle1);
                    }
                    hv_Deg.Dispose();
                    HOperatorSet.TupleDeg(hv_Angle1, out hv_Deg);
                    hv_ResAngle.Dispose();
                    hv_ResAngle = new HTuple(hv_Deg);
                    hv_Col1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col1 = hv_Row.TupleSelect(
                            hv_a.TupleSelect(0));
                    }
                    hv_Row1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row1 = hv_Col.TupleSelect(
                            hv_a.TupleSelect(0));
                    }
                    hv_Col2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col2 = hv_Row.TupleSelect(
                            hv_a.TupleSelect((new HTuple(hv_a.TupleLength())) - 1));
                    }
                    hv_Row2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row2 = hv_Col.TupleSelect(
                            hv_a.TupleSelect((new HTuple(hv_a.TupleLength())) - 1));
                    }

                }
                LineResultDict lineResultDict = new LineResultDict { Angle = hv_ResAngle.D, Col1 = hv_Col1.I, Col2 = hv_Col2.I, Row1 = hv_Row1.I, Row2 = hv_Row2.I };
                results.Add("result", JsonUtils.JsonSerialize(lineResultDict));
                return results;

            }
            catch (Exception ex)
            {
                LineResultDict lineResultDict = new LineResultDict { Angle = hv_ResAngle.D, Col1 = hv_Col1.I, Col2 = hv_Col2.I, Row1 = hv_Row1.I, Row2 = hv_Row2.I };
                results.Add("result", JsonUtils.JsonSerialize(lineResultDict));
                return results;
            }
            finally
            {
                ho_Image.Dispose();
                ho_ImageMedian.Dispose();
                ho_Region.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionClosing.Dispose();
                ho_Contours.Dispose();
                ho_SmoothedContours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_SortedContours.Dispose();
                ho_ObjectSelected.Dispose();
                ho_Cross.Dispose();
                ho_RegionLines.Dispose();
                hv_ResAngle.Dispose();
                hv_Col1.Dispose();
                hv_Row1.Dispose();
                hv_Col2.Dispose();
                hv_Row2.Dispose();
                hv_Row.Dispose();
                hv_Col.Dispose();
                hv_a.Dispose();
                hv_Angle1.Dispose();
                hv_Deg.Dispose();
            }

        }
        public class LineResultDict
        {
            public double Angle { set; get; }
            public int Row1 { set; get; }
            public int Col1 { set; get; }
            public int Row2 { set; get; }
            public int Col2 { set; get; }
        }
    }
}
