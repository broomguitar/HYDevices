﻿using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.ZbomHome
{
    /// <summary>
    /// 缺陷检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class WoodBoardNew_2023 : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static WoodBoardNew_2023 _instance;
        public static WoodBoardNew_2023 Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new WoodBoardNew_2023();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.Classification;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic> { { "HdlPath", @"TestModel\Over6.5_PartialRefined_model_best_map_0.482_RTX3070_TRT_float16_BatchSize8.hdl" }, { "HdictPath", @"TestModel\dl_preprocess_param.hdict" }, { "GPU", 0 } };

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Direction", "Over" }, { "Image", "" }, { "RowOffsetInWholeImage", 0 } };

        public HTuple hv_ExpDefaultWinHandle;
        HTuple hv_DLModelHandle = new HTuple(), hv_BatchSize = new HTuple();
        HTuple hv_DLPreprocessParam = new HTuple();
        HObject ho_ContoursTuple = null;
        HObject ho_BGImageLowerA = null, ho_BGImageLowerB = null;
        HObject ho_BGImageOver = null;
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {

            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Image, ho_ConveyerImage, ho_ImageSub;
            HObject ho_Regions, ho_TmpRegions, ho_Regiondilation, ho_ConnectedRegions;
            HObject ho_SelectedRegions, ho_RegionRectangle1, ho_ImageReduced;
            HObject ho_Rectangles, ho_SliceTuple, ho_ImageSlice = null;
            HObject ho_ImageSliceScaled = null, ho_SliceBatch = null, ho_DLResultAllRectangles;
            HObject ho_RectangleSelected = null, ho_Rectangle = null;
            HObject ho_CurrentClassRectangles = null, ho_CurrentClassRectanglesUnion = null;
            HObject ho_CurrentClassRectanglesUnionsComponents = null;
            HObject ho_CurrentClassRectanglesReduced = null, ho_RectanglesReduced = null;

            HObject ho_Regionclosing = null, ho_SelectedRegion = null, ho_Contours = null;
            HObject ho_ContoursSplit = null, ho_SingleSegment = null;


            // Local control variables 
            HTuple hv_cameraPosition = new HTuple();
            HTuple hv_NumSegments = new HTuple(), hv_NumLines = new HTuple();
            HTuple hv_RowsBegin = new HTuple(), hv_ColsBegin = new HTuple();
            HTuple hv_RowsEnd = new HTuple(), hv_ColsEnd = new HTuple();
            HTuple hv_Attrib = new HTuple(), hv_RowBegin = new HTuple();
            HTuple hv_ColBegin = new HTuple(), hv_RowEnd = new HTuple();
            HTuple hv_ColEnd = new HTuple(), hv_Nr = new HTuple();
            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_Length = new HTuple();
            HTuple hv_RowIntersect1 = new HTuple(), hv_ColumnIntersect1 = new HTuple();
            HTuple hv_IsOverlapping1 = new HTuple(), hv_RowIntersect2 = new HTuple();
            HTuple hv_ColumnIntersect2 = new HTuple(), hv_IsOverlapping2 = new HTuple();
            HTuple hv_RowIntersect3 = new HTuple(), hv_ColumnIntersect3 = new HTuple();
            HTuple hv_IsOverlapping3 = new HTuple(), hv_RowIntersect4 = new HTuple();
            HTuple hv_ColumnIntersect4 = new HTuple(), hv_IsOverlapping4 = new HTuple();

            HTuple hv_StartAll = new HTuple(), hv_bTopOrSide = new HTuple();
            HTuple hv_ImageFileName = new HTuple(), hv_ConveyerImageFileName = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_StartPreprocess = new HTuple(), hv_UsedThreshold = new HTuple();
            HTuple hv_Threshold = new HTuple(), hv_Number = new HTuple();
            HTuple hv_EndPreprocess = new HTuple(), hv_SecondsPreprocess = new HTuple();

            HTuple hv_StartSlice = new HTuple(), hv_SliceWidth = new HTuple();
            HTuple hv_OverlapWidth = new HTuple(), hv_RowLarge1 = new HTuple();
            HTuple hv_ColumnLarge1 = new HTuple(), hv_RowLarge2 = new HTuple();
            HTuple hv_ColumnLarge2 = new HTuple(), hv_TupleRow1 = new HTuple();
            HTuple hv_TupleRow2 = new HTuple(), hv_TupleColumn1 = new HTuple();
            HTuple hv_TupleColumn2 = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_EndSlice = new HTuple();
            HTuple hv_SecondsSlice = new HTuple(), hv_StartSliceTuple = new HTuple();
            HTuple hv_Num = new HTuple(), hv_Index2 = new HTuple();
            HTuple hv_EndSliceTuple = new HTuple(), hv_SecondsSliceTuple = new HTuple();
            HTuple hv_StartDL = new HTuple(), hv_DLResultAll = new HTuple();
            HTuple hv_Tuple_bbox_row1 = new HTuple(), hv_Tuple_bbox_col1 = new HTuple();
            HTuple hv_Tuple_bbox_row2 = new HTuple(), hv_Tuple_bbox_col2 = new HTuple();
            HTuple hv_Tuple_bbox_class_id = new HTuple(), hv_Tuple_bbox_class_name = new HTuple();
            HTuple hv_Tuple_bbox_confidence = new HTuple(), hv_startIndex = new HTuple();
            HTuple hv_endIndex = new HTuple(), hv_DLSampleInferenceBatch = new HTuple();
            HTuple hv_StartBatch = new HTuple(), hv_DLResultBatch = new HTuple();
            HTuple hv_EndBatch = new HTuple(), hv_SecondsBatch = new HTuple();
            HTuple hv_BatchNum = new HTuple(), hv_i = new HTuple();
            HTuple hv_Row1s = new HTuple(), hv_ResultNum = new HTuple();
            HTuple hv_Col1s = new HTuple(), hv_Row2s = new HTuple();
            HTuple hv_Col2s = new HTuple(), hv_IDs = new HTuple();
            HTuple hv_Names = new HTuple(), hv_Confidences = new HTuple();
            HTuple hv_Index3 = new HTuple(), hv_EndDL = new HTuple();
            HTuple hv_SecondsDL = new HTuple(), hv_EndAll = new HTuple();

            HTuple hv_Tuple_bboxReduced_row1 = new HTuple();
            HTuple hv_Tuple_bboxReduced_col1 = new HTuple(), hv_Tuple_bboxReduced_row2 = new HTuple();
            HTuple hv_Tuple_bboxReduced_col2 = new HTuple(), hv_Tuple_bboxReduced_class_id = new HTuple();
            HTuple hv_Tuple_bboxReduced_class_name = new HTuple();
            HTuple hv_ClassIDs = new HTuple(), hv_ClassNames = new HTuple();
            HTuple hv_IndexClass = new HTuple(), hv_Tuple_CurrentClass_bbox_row1 = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_col1 = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_row2 = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_col2 = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_class_id = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_class_name = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_confidence = new HTuple();
            HTuple hv_Index4 = new HTuple(), hv_RectNum = new HTuple();
            HTuple hv_Index5 = new HTuple();

            HTuple hv_Anisometry = new HTuple(), hv_Bulkiness = new HTuple();
            HTuple hv_StructureFactor = new HTuple();


            HTuple hv_SecondsAll = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ConveyerImage);
            HOperatorSet.GenEmptyObj(out ho_ImageSub);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_TmpRegions);
            HOperatorSet.GenEmptyObj(out ho_Regiondilation);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionRectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);

            HOperatorSet.GenEmptyObj(out ho_Regionclosing);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegion);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
            HOperatorSet.GenEmptyObj(out ho_SingleSegment);

            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rectangles);
            HOperatorSet.GenEmptyObj(out ho_SliceTuple);
            HOperatorSet.GenEmptyObj(out ho_ImageSlice);
            HOperatorSet.GenEmptyObj(out ho_ImageSliceScaled);
            HOperatorSet.GenEmptyObj(out ho_SliceBatch);
            HOperatorSet.GenEmptyObj(out ho_DLResultAllRectangles);
            HOperatorSet.GenEmptyObj(out ho_CurrentClassRectangles);
            HOperatorSet.GenEmptyObj(out ho_CurrentClassRectanglesUnion);
            HOperatorSet.GenEmptyObj(out ho_CurrentClassRectanglesUnionsComponents);
            HOperatorSet.GenEmptyObj(out ho_CurrentClassRectanglesReduced);
            HOperatorSet.GenEmptyObj(out ho_RectanglesReduced);
            HOperatorSet.GenEmptyObj(out ho_RectangleSelected);
            List<TargetResult> ListDetectResult = new List<TargetResult>();
            try
            {
                //用于导出到C#.
                //在V1的基础上优化运行速度。
                //切割出图像中的木板区域后，将木板区域全分辨率图像滑窗切割为1024*1024的小图像,重叠区域宽64,然后将切割出的小图送入模型进行目标检测，
                //最后把结果合并回大图。

                //处理部分
                hv_StartAll.Dispose();
                HOperatorSet.CountSeconds(out hv_StartAll);

                //相机位置， 1:over，2:lower，3:side free, 4:side fixed, 5:front, 6:back.
                hv_cameraPosition.Dispose();
                if (actionParams["Direction"] == "Over")
                {
                    hv_cameraPosition = 1;
                }
                else if (actionParams["Direction"] == "Lower")
                {
                    hv_cameraPosition = 2;
                }
                else if (actionParams["Direction"] == "SideFree")
                {
                    hv_cameraPosition = 3;
                }
                else if (actionParams["Direction"] == "SideFixed")
                {
                    hv_cameraPosition = 4;
                }
                else if (actionParams["Direction"] == "Front")
                {
                    hv_cameraPosition = 5;
                }
                else if (actionParams["Direction"] == "Back")
                {
                    hv_cameraPosition = 6;
                }
                    ho_Image.Dispose();
                    ho_Image = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
                hv_StartPreprocess.Dispose();
                HOperatorSet.CountSeconds(out hv_StartPreprocess);
                if ((int)((new HTuple(hv_cameraPosition.TupleEqual(1))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    2)))) != 0)
                {
                    //**********************************************************************************************************************
                    //ImageFileName := 'D:/Halcon/志邦/志邦/LinearCamera_Over_20230402081617.jpg'
                    //ConveyerImageFileName := 'D:/Halcon/志邦/志邦/Bg_300_上.bmp'


                    //read_image (Image, ImageFileName)
                    //get_image_size (Image, Width, Height)

                    //read_image (ConveyerImage, ConveyerImageFileName)

                    //原始图像减去传送带图像，得出感兴趣区域

                    //sub_image (Image, ConveyerImage, ImageSub, 1, 0)


                    //threshold (ImageSub, Regions, 15, 255)

                    //binary_threshold (ImageSub, TmpRegions, 'max_separability', 'light', UsedThreshold)

                    //if (UsedThreshold * 0.5 > 15)
                    //Threshold := UsedThreshold * 0.5
                    //else
                    //Threshold := 15
                    //endif
                    //threshold (ImageSub, Regions, Threshold, 255)
                    //dilation_circle (Regions, Regiondilation, 30)

                    //connection (Regiondilation, ConnectedRegions)
                    //select_shape_std (ConnectedRegions, SelectedRegions, 'max_area', 70)
                    //count_obj (SelectedRegions, Number)

                    //shape_trans (SelectedRegions, RegionRectangle1, 'rectangle1')

                    //reduce_domain (Image, RegionRectangle1, ImageReduced)
                    //smallest_rectangle1 (RegionRectangle1, RowLarge1, ColumnLarge1, RowLarge2, ColumnLarge2)
                    //********************************************************************************************************************
                    //小图像长和宽
                    hv_SliceWidth.Dispose();
                    hv_SliceWidth = 1024;
                    //重叠区域宽
                    hv_OverlapWidth.Dispose();
                    hv_OverlapWidth = 64;
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                    ho_Regions.Dispose();
                    HOperatorSet.Threshold(ho_Image, out ho_Regions, 35, 255);
                    ho_Regionclosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Regions, out ho_Regionclosing, 30);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Regionclosing, out ho_ConnectedRegions);
                    ho_SelectedRegion.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegion, "max_area",
                        70);

                    hv_Anisometry.Dispose(); hv_Bulkiness.Dispose(); hv_StructureFactor.Dispose();
                    HOperatorSet.Eccentricity(ho_SelectedRegion, out hv_Anisometry, out hv_Bulkiness,
                        out hv_StructureFactor);

                    //滤除条纹干扰。
                    if (new HTuple(hv_Anisometry.TupleGreater(30)) != 0)
                    {
                        return results;
                    }

                    ho_RegionRectangle1.Dispose();
                    HOperatorSet.ShapeTrans(ho_SelectedRegion, out ho_RegionRectangle1, "rectangle1");

                    hv_RowLarge1.Dispose(); hv_ColumnLarge1.Dispose(); hv_RowLarge2.Dispose(); hv_ColumnLarge2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_RegionRectangle1, out hv_RowLarge1, out hv_ColumnLarge1,
                        out hv_RowLarge2, out hv_ColumnLarge2);

                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_SelectedRegion, out ho_Contours, "center");
                    ho_ContoursSplit.Dispose();
                    HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines",
                        101, 80, 80);
                    //
                    //step: fit circles and lines into contours
                    //

                    hv_NumSegments.Dispose();
                    HOperatorSet.CountObj(ho_ContoursSplit, out hv_NumSegments);

                    hv_NumLines.Dispose();
                    hv_NumLines = 0;
                    hv_RowsBegin.Dispose();
                    hv_RowsBegin = new HTuple();
                    hv_ColsBegin.Dispose();
                    hv_ColsBegin = new HTuple();
                    hv_RowsEnd.Dispose();
                    hv_RowsEnd = new HTuple();
                    hv_ColsEnd.Dispose();
                    hv_ColsEnd = new HTuple();
                    HTuple end_val216 = hv_NumSegments;
                    HTuple step_val216 = 1;
                    for (hv_i = 1; hv_i.Continue(end_val216, step_val216); hv_i = hv_i.TupleAdd(step_val216))
                    {
                        ho_SingleSegment.Dispose();
                        HOperatorSet.SelectObj(ho_ContoursSplit, out ho_SingleSegment, hv_i);
                        hv_Attrib.Dispose();
                        HOperatorSet.GetContourGlobalAttribXld(ho_SingleSegment, "cont_approx",
                            out hv_Attrib);
                        if ((int)(new HTuple(hv_Attrib.TupleEqual(-1))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_NumLines = hv_NumLines + 1;
                                    hv_NumLines.Dispose();
                                    hv_NumLines = ExpTmpLocalVar_NumLines;
                                }
                            }
                            hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                            HOperatorSet.FitLineContourXld(ho_SingleSegment, "tukey", -1, 0, 5, 2,
                                out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr,
                                out hv_Nc, out hv_Dist);
                            hv_Length.Dispose();
                            HOperatorSet.DistancePp(hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd,
                                out hv_Length);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_RowsBegin = hv_RowsBegin.TupleConcat(
                                        hv_RowBegin);
                                    hv_RowsBegin.Dispose();
                                    hv_RowsBegin = ExpTmpLocalVar_RowsBegin;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_ColsBegin = hv_ColsBegin.TupleConcat(
                                        hv_ColBegin);
                                    hv_ColsBegin.Dispose();
                                    hv_ColsBegin = ExpTmpLocalVar_ColsBegin;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_RowsEnd = hv_RowsEnd.TupleConcat(
                                        hv_RowEnd);
                                    hv_RowsEnd.Dispose();
                                    hv_RowsEnd = ExpTmpLocalVar_RowsEnd;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_ColsEnd = hv_ColsEnd.TupleConcat(
                                        hv_ColEnd);
                                    hv_ColsEnd.Dispose();
                                    hv_ColsEnd = ExpTmpLocalVar_ColsEnd;
                                }
                            }
                        }
                    }
                    //
                    //intersection points and angles calculated
                    //by the lines obtained by line-fitting
                    //
                    if ((int)(new HTuple(hv_NumLines.TupleEqual(4))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RowIntersect1.Dispose(); hv_ColumnIntersect1.Dispose(); hv_IsOverlapping1.Dispose();
                            HOperatorSet.IntersectionLines(hv_RowsBegin.TupleSelect(0), hv_ColsBegin.TupleSelect(
                                0), hv_RowsEnd.TupleSelect(0), hv_ColsEnd.TupleSelect(0), hv_RowsBegin.TupleSelect(
                                1), hv_ColsBegin.TupleSelect(1), hv_RowsEnd.TupleSelect(1), hv_ColsEnd.TupleSelect(
                                1), out hv_RowIntersect1, out hv_ColumnIntersect1, out hv_IsOverlapping1);
                        }
                        //ho_CrossIntersection1.Dispose();
                        //HOperatorSet.GenCrossContourXld(out ho_CrossIntersection1, hv_RowIntersect1,
                        //hv_ColumnIntersect1, 200, 0.785398);

                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RowIntersect2.Dispose(); hv_ColumnIntersect2.Dispose(); hv_IsOverlapping2.Dispose();
                            HOperatorSet.IntersectionLines(hv_RowsBegin.TupleSelect(1), hv_ColsBegin.TupleSelect(
                                1), hv_RowsEnd.TupleSelect(1), hv_ColsEnd.TupleSelect(1), hv_RowsBegin.TupleSelect(
                                2), hv_ColsBegin.TupleSelect(2), hv_RowsEnd.TupleSelect(2), hv_ColsEnd.TupleSelect(
                                2), out hv_RowIntersect2, out hv_ColumnIntersect2, out hv_IsOverlapping2);
                        }
                        //distance_pp (RowJunctions[0], ColJunctions[0], RowIntersect2, ColumnIntersect2, Distance2)
                        //ho_CrossIntersection2.Dispose();
                        //HOperatorSet.GenCrossContourXld(out ho_CrossIntersection2, hv_RowIntersect2,
                        //hv_ColumnIntersect2, 200, 0.785398);

                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RowIntersect3.Dispose(); hv_ColumnIntersect3.Dispose(); hv_IsOverlapping3.Dispose();
                            HOperatorSet.IntersectionLines(hv_RowsBegin.TupleSelect(2), hv_ColsBegin.TupleSelect(
                                2), hv_RowsEnd.TupleSelect(2), hv_ColsEnd.TupleSelect(2), hv_RowsBegin.TupleSelect(
                                3), hv_ColsBegin.TupleSelect(3), hv_RowsEnd.TupleSelect(3), hv_ColsEnd.TupleSelect(
                                3), out hv_RowIntersect3, out hv_ColumnIntersect3, out hv_IsOverlapping3);
                        }
                        //ho_CrossIntersection3.Dispose();
                        //HOperatorSet.GenCrossContourXld(out ho_CrossIntersection3, hv_RowIntersect3,
                        //hv_ColumnIntersect3, 200, 0.785398);

                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RowIntersect4.Dispose(); hv_ColumnIntersect4.Dispose(); hv_IsOverlapping4.Dispose();
                            HOperatorSet.IntersectionLines(hv_RowsBegin.TupleSelect(3), hv_ColsBegin.TupleSelect(
                                3), hv_RowsEnd.TupleSelect(3), hv_ColsEnd.TupleSelect(3), hv_RowsBegin.TupleSelect(
                                0), hv_ColsBegin.TupleSelect(0), hv_RowsEnd.TupleSelect(0), hv_ColsEnd.TupleSelect(
                                0), out hv_RowIntersect4, out hv_ColumnIntersect4, out hv_IsOverlapping4);
                        }
                        //ho_CrossIntersection4.Dispose();
                        //HOperatorSet.GenCrossContourXld(out ho_CrossIntersection4, hv_RowIntersect4,
                        //hv_ColumnIntersect4, 200, 0.785398);
                        //结果。
                        results.Add("Row1", hv_RowIntersect1.D);
                        results.Add("Col1", hv_ColumnIntersect1.D);
                        results.Add("Row2", hv_RowIntersect2.D);
                        results.Add("Col2", hv_ColumnIntersect2.D);
                        results.Add("Row3", hv_RowIntersect3.D);
                        results.Add("Col3", hv_ColumnIntersect3.D);
                        results.Add("Row4", hv_RowIntersect4.D);
                        results.Add("Col4", hv_ColumnIntersect4.D);
                    }
                    else
                    {

                    }
                    //stop ()
                    //dev_close_window ()

                    //********************************************************************************************************************

                }
                //3,4:side
                if ((int)((new HTuple(hv_cameraPosition.TupleEqual(3))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    4)))) != 0)
                {
                    //小图像长和宽
                    hv_SliceWidth.Dispose();
                    hv_SliceWidth = 1024;
                    //重叠区域宽
                    hv_OverlapWidth.Dispose();
                    hv_OverlapWidth = 64;
                    //旋转图像，以便标注时人眼查看。
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.RotateImage(ho_Image, out ExpTmpOutVar_0, 270, "constant");
                        ho_Image.Dispose();
                        ho_Image = ExpTmpOutVar_0;
                    }
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                    hv_RowLarge1.Dispose();
                    hv_RowLarge1 = 0;
                    hv_ColumnLarge1.Dispose();
                    hv_ColumnLarge1 = 0;
                    hv_RowLarge2.Dispose();
                    hv_RowLarge2 = 1000;
                    hv_ColumnLarge2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColumnLarge2 = hv_Width - 1;
                    }
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, hv_RowLarge1, hv_ColumnLarge1,
                        hv_RowLarge2, hv_ColumnLarge2);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);

                }

                //相机位置，5:front.6:back.
                if ((int)((new HTuple(hv_cameraPosition.TupleEqual(5))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    6)))) != 0)
                {
                    //由于前后相机分辨率不高，因此，直接切割512*512大小的小图像，而不是切割1024*1024然后缩小为512*512。
                    //小图像长和宽
                    hv_SliceWidth.Dispose();
                    hv_SliceWidth = 512;
                    //重叠区域宽
                    hv_OverlapWidth.Dispose();
                    hv_OverlapWidth = 32;
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                    hv_RowLarge1.Dispose();
                    hv_RowLarge1 = 1800;
                    hv_ColumnLarge1.Dispose();
                    hv_ColumnLarge1 = 0;
                    hv_RowLarge2.Dispose();
                    hv_RowLarge2 = 2300;
                    hv_ColumnLarge2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColumnLarge2 = hv_Width - 1;
                    }
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, hv_RowLarge1, hv_ColumnLarge1,
                        hv_RowLarge2, hv_ColumnLarge2);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced);

                }

                hv_EndPreprocess.Dispose();
                HOperatorSet.CountSeconds(out hv_EndPreprocess);
                hv_SecondsPreprocess.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsPreprocess = hv_EndPreprocess - hv_StartPreprocess;
                }
                //*****************************************************************************************************

                hv_StartSlice.Dispose();
                HOperatorSet.CountSeconds(out hv_StartSlice);
                //滑窗，将木板区域全分辨率图像切割为SliceWidth*SliceWidth的小图像。
                //首先，生成矩形框。

                //矩形框坐标
                hv_TupleRow1.Dispose();
                hv_TupleRow1 = new HTuple();
                hv_TupleRow2.Dispose();
                hv_TupleRow2 = new HTuple();
                hv_TupleColumn1.Dispose();
                hv_TupleColumn1 = new HTuple();
                hv_TupleColumn2.Dispose();
                hv_TupleColumn2 = new HTuple();

                hv_Row1.Dispose();
                hv_Row1 = new HTuple(hv_RowLarge1);
                hv_Row2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row2 = (hv_RowLarge1 + hv_SliceWidth) - 1;
                }

                while (new HTuple(hv_Row2.TupleLessEqual(hv_RowLarge2)) != 0)
                {
                    hv_Column1.Dispose();
                    hv_Column1 = new HTuple(hv_ColumnLarge1);
                    hv_Column2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column2 = (hv_ColumnLarge1 + hv_SliceWidth) - 1;
                    }
                    while (new HTuple(hv_Column2.TupleLessEqual(hv_ColumnLarge2)) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleRow1 = hv_TupleRow1.TupleConcat(
                                    hv_Row1);
                                hv_TupleRow1.Dispose();
                                hv_TupleRow1 = ExpTmpLocalVar_TupleRow1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleRow2 = hv_TupleRow2.TupleConcat(
                                    hv_Row2);
                                hv_TupleRow2.Dispose();
                                hv_TupleRow2 = ExpTmpLocalVar_TupleRow2;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleColumn1 = hv_TupleColumn1.TupleConcat(
                                    hv_Column1);
                                hv_TupleColumn1.Dispose();
                                hv_TupleColumn1 = ExpTmpLocalVar_TupleColumn1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleColumn2 = hv_TupleColumn2.TupleConcat(
                                    hv_Column2);
                                hv_TupleColumn2.Dispose();
                                hv_TupleColumn2 = ExpTmpLocalVar_TupleColumn2;
                            }
                        }

                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Column1 = hv_Column1 + (hv_SliceWidth - hv_OverlapWidth);
                                hv_Column1.Dispose();
                                hv_Column1 = ExpTmpLocalVar_Column1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Column2 = hv_Column2 + (hv_SliceWidth - hv_OverlapWidth);
                                hv_Column2.Dispose();
                                hv_Column2 = ExpTmpLocalVar_Column2;
                            }
                        }
                    }
                    if (new HTuple(hv_Column2.TupleGreater(hv_ColumnLarge2)) != 0)
                    {
                        if (new HTuple(hv_Column2.TupleGreaterEqual(hv_Width - 1)) != 0)
                        {
                            hv_Column2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Column2 = hv_Width - 1;
                            }
                        }
                        hv_Column1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Column1 = (hv_Column2 - hv_SliceWidth) + 1;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleRow1 = hv_TupleRow1.TupleConcat(
                                    hv_Row1);
                                hv_TupleRow1.Dispose();
                                hv_TupleRow1 = ExpTmpLocalVar_TupleRow1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleRow2 = hv_TupleRow2.TupleConcat(
                                    hv_Row2);
                                hv_TupleRow2.Dispose();
                                hv_TupleRow2 = ExpTmpLocalVar_TupleRow2;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleColumn1 = hv_TupleColumn1.TupleConcat(
                                    hv_Column1);
                                hv_TupleColumn1.Dispose();
                                hv_TupleColumn1 = ExpTmpLocalVar_TupleColumn1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleColumn2 = hv_TupleColumn2.TupleConcat(
                                    hv_Column2);
                                hv_TupleColumn2.Dispose();
                                hv_TupleColumn2 = ExpTmpLocalVar_TupleColumn2;
                            }
                        }
                    }

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Row1 = hv_Row1 + (hv_SliceWidth - hv_OverlapWidth);
                            hv_Row1.Dispose();
                            hv_Row1 = ExpTmpLocalVar_Row1;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Row2 = hv_Row2 + (hv_SliceWidth - hv_OverlapWidth);
                            hv_Row2.Dispose();
                            hv_Row2 = ExpTmpLocalVar_Row2;
                        }
                    }
                }
                if (new HTuple(hv_Row2.TupleGreater(hv_RowLarge2)) != 0)
                {
                    if (new HTuple(hv_Row2.TupleGreaterEqual(hv_Height - 1)) != 0)
                    {
                        hv_Row2.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Row2 = hv_Height - 1;
                        }
                    }
                    hv_Row1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row1 = (hv_Row2 - hv_SliceWidth) + 1;
                    }

                    hv_Column1.Dispose();
                    hv_Column1 = new HTuple(hv_ColumnLarge1);
                    hv_Column2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column2 = (hv_ColumnLarge1 + hv_SliceWidth) - 1;
                    }
                    while (new HTuple(hv_Column2.TupleLessEqual(hv_ColumnLarge2)) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleRow1 = hv_TupleRow1.TupleConcat(
                                    hv_Row1);
                                hv_TupleRow1.Dispose();
                                hv_TupleRow1 = ExpTmpLocalVar_TupleRow1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleRow2 = hv_TupleRow2.TupleConcat(
                                    hv_Row2);
                                hv_TupleRow2.Dispose();
                                hv_TupleRow2 = ExpTmpLocalVar_TupleRow2;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleColumn1 = hv_TupleColumn1.TupleConcat(
                                    hv_Column1);
                                hv_TupleColumn1.Dispose();
                                hv_TupleColumn1 = ExpTmpLocalVar_TupleColumn1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleColumn2 = hv_TupleColumn2.TupleConcat(
                                    hv_Column2);
                                hv_TupleColumn2.Dispose();
                                hv_TupleColumn2 = ExpTmpLocalVar_TupleColumn2;
                            }
                        }

                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Column1 = hv_Column1 + (hv_SliceWidth - hv_OverlapWidth);
                                hv_Column1.Dispose();
                                hv_Column1 = ExpTmpLocalVar_Column1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Column2 = hv_Column2 + (hv_SliceWidth - hv_OverlapWidth);
                                hv_Column2.Dispose();
                                hv_Column2 = ExpTmpLocalVar_Column2;
                            }
                        }
                    }
                    if (new HTuple(hv_Column2.TupleGreater(hv_ColumnLarge2)) != 0)
                    {
                        if (new HTuple(hv_Column2.TupleGreaterEqual(hv_Width - 1)) != 0)
                        {
                            hv_Column2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Column2 = hv_Width - 1;
                            }
                        }
                        hv_Column1.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Column1 = (hv_Column2 - hv_SliceWidth) + 1;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleRow1 = hv_TupleRow1.TupleConcat(
                                    hv_Row1);
                                hv_TupleRow1.Dispose();
                                hv_TupleRow1 = ExpTmpLocalVar_TupleRow1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleRow2 = hv_TupleRow2.TupleConcat(
                                    hv_Row2);
                                hv_TupleRow2.Dispose();
                                hv_TupleRow2 = ExpTmpLocalVar_TupleRow2;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleColumn1 = hv_TupleColumn1.TupleConcat(
                                    hv_Column1);
                                hv_TupleColumn1.Dispose();
                                hv_TupleColumn1 = ExpTmpLocalVar_TupleColumn1;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_TupleColumn2 = hv_TupleColumn2.TupleConcat(
                                    hv_Column2);
                                hv_TupleColumn2.Dispose();
                                hv_TupleColumn2 = ExpTmpLocalVar_TupleColumn2;
                            }
                        }
                    }
                }
                ho_Rectangles.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangles, hv_TupleRow1, hv_TupleColumn1,
                    hv_TupleRow2, hv_TupleColumn2);
                hv_EndSlice.Dispose();
                HOperatorSet.CountSeconds(out hv_EndSlice);
                hv_SecondsSlice.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsSlice = hv_EndSlice - hv_StartSlice;
                }
                //********************************************************************************************************

                //生成小图像tuple.
                hv_StartSliceTuple.Dispose();
                HOperatorSet.CountSeconds(out hv_StartSliceTuple);
                ho_SliceTuple.Dispose();
                HOperatorSet.GenEmptyObj(out ho_SliceTuple);
                hv_Num.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Num = new HTuple(hv_TupleRow1.TupleLength()
                        );
                }
                HTuple end_val377 = hv_Num - 1;
                HTuple step_val377 = 1;
                for (hv_Index2 = 0; hv_Index2.Continue(end_val377, step_val377); hv_Index2 = hv_Index2.TupleAdd(step_val377))
                {

                    //select_obj (Rectangles, RectangleSelected, Index2)
                    //reduce_domain (Image, RectangleSelected, ImageSliceReduced)
                    //crop_domain (ImageSliceReduced, ImageSlice)

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ImageSlice.Dispose();
                        HOperatorSet.CropRectangle1(ho_Image, out ho_ImageSlice, hv_TupleRow1.TupleSelect(
                            hv_Index2), hv_TupleColumn1.TupleSelect(hv_Index2), hv_TupleRow2.TupleSelect(
                            hv_Index2), hv_TupleColumn2.TupleSelect(hv_Index2));
                    }

                    ho_ImageSliceScaled.Dispose();
                    HOperatorSet.ZoomImageFactor(ho_ImageSlice, out ho_ImageSliceScaled, 0.5,
                        0.5, "bicubic");
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_SliceTuple, ho_ImageSliceScaled, out ExpTmpOutVar_0
                            );
                        ho_SliceTuple.Dispose();
                        ho_SliceTuple = ExpTmpOutVar_0;
                    }
                }

                hv_EndSliceTuple.Dispose();
                HOperatorSet.CountSeconds(out hv_EndSliceTuple);
                hv_SecondsSliceTuple.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsSliceTuple = hv_EndSliceTuple - hv_StartSliceTuple;
                }

                //**************************************************************************************************************

                //将切割出的小图送入模型进行目标检测
                //所有小图中结果转换坐标后存入同一个字典
                hv_StartDL.Dispose();
                HOperatorSet.CountSeconds(out hv_StartDL);
                hv_DLResultAll.Dispose();
                HOperatorSet.CreateDict(out hv_DLResultAll);
                hv_Tuple_bbox_row1.Dispose();
                hv_Tuple_bbox_row1 = new HTuple();
                hv_Tuple_bbox_col1.Dispose();
                hv_Tuple_bbox_col1 = new HTuple();
                hv_Tuple_bbox_row2.Dispose();
                hv_Tuple_bbox_row2 = new HTuple();
                hv_Tuple_bbox_col2.Dispose();
                hv_Tuple_bbox_col2 = new HTuple();
                hv_Tuple_bbox_class_id.Dispose();
                hv_Tuple_bbox_class_id = new HTuple();
                hv_Tuple_bbox_class_name.Dispose();
                hv_Tuple_bbox_class_name = new HTuple();
                hv_Tuple_bbox_confidence.Dispose();
                hv_Tuple_bbox_confidence = new HTuple();

                hv_Num.Dispose();
                HOperatorSet.CountObj(ho_SliceTuple, out hv_Num);

                //*while(true)   

                HTuple end_val410 = hv_Num;
                HTuple step_val410 = hv_BatchSize;
                for (hv_Index2 = 1; hv_Index2.Continue(end_val410, step_val410); hv_Index2 = hv_Index2.TupleAdd(step_val410))
                {
                    hv_startIndex.Dispose();
                    hv_startIndex = new HTuple(hv_Index2);
                    hv_endIndex.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_endIndex = (((hv_Index2 + hv_BatchSize) - 1)).TupleMin2(
                            hv_Num);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_SliceBatch.Dispose();
                        HOperatorSet.SelectObj(ho_SliceTuple, out ho_SliceBatch, HTuple.TupleGenSequence(
                            hv_startIndex, hv_endIndex, 1));
                    }
                    hv_DLSampleInferenceBatch.Dispose();
                    gen_dl_samples_from_images(ho_SliceBatch, out hv_DLSampleInferenceBatch);

                    hv_StartBatch.Dispose();
                    HOperatorSet.CountSeconds(out hv_StartBatch);

                    if ((int)((new HTuple(hv_cameraPosition.TupleEqual(1))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                        2)))) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_num_channels", 3);
                        preprocess_dl_samples(hv_DLSampleInferenceBatch, hv_DLPreprocessParam);
                        hv_DLResultBatch.Dispose();
                        HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleInferenceBatch,
                            new HTuple(), out hv_DLResultBatch);
                    }
                    if ((int)((new HTuple(hv_cameraPosition.TupleEqual(3))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                        4)))) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_num_channels", 3);
                        preprocess_dl_samples(hv_DLSampleInferenceBatch, hv_DLPreprocessParam);
                        hv_DLResultBatch.Dispose();
                        HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleInferenceBatch,
                            new HTuple(), out hv_DLResultBatch);
                    }
                    if ((int)((new HTuple(hv_cameraPosition.TupleEqual(5))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                        6)))) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_num_channels", 3);
                        preprocess_dl_samples(hv_DLSampleInferenceBatch, hv_DLPreprocessParam);
                        hv_DLResultBatch.Dispose();
                        HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleInferenceBatch,
                            new HTuple(), out hv_DLResultBatch);
                    }

                    hv_EndBatch.Dispose();
                    HOperatorSet.CountSeconds(out hv_EndBatch);
                    hv_SecondsBatch.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SecondsBatch = hv_EndBatch - hv_StartBatch;
                    }
                    //stop ()
                    //*********************************************************************************************************

                    hv_BatchNum.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BatchNum = new HTuple(hv_DLResultBatch.TupleLength()
                            );
                    }

                    HTuple end_val441 = hv_BatchNum - 1;
                    HTuple step_val441 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val441, step_val441); hv_i = hv_i.TupleAdd(step_val441))
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Row1s.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_row1",
                                out hv_Row1s);
                        }
                        hv_ResultNum.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ResultNum = new HTuple(hv_Row1s.TupleLength()
                                );
                        }
                        if ((int)(new HTuple(hv_ResultNum.TupleNotEqual(0))) != 0)
                        {
                            //get_dict_tuple (DLResult, 'bbox_row1', Row1s)
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Col1s.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_col1",
                                    out hv_Col1s);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Row2s.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_row2",
                                    out hv_Row2s);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Col2s.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_col2",
                                    out hv_Col2s);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_IDs.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_class_id",
                                    out hv_IDs);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Names.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_class_name",
                                    out hv_Names);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Confidences.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_confidence",
                                    out hv_Confidences);
                            }
                        }
                        HTuple end_val453 = hv_ResultNum - 1;
                        HTuple step_val453 = 1;
                        for (hv_Index3 = 0; hv_Index3.Continue(end_val453, step_val453); hv_Index3 = hv_Index3.TupleAdd(step_val453))
                        {
                            //对于上下和两侧相机，因为小图缩小了一半，坐标需要乘以二
                            if ((int)((new HTuple(hv_cameraPosition.TupleNotEqual(5))).TupleAnd(new HTuple(hv_cameraPosition.TupleNotEqual(
                                6)))) != 0)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_row1 = hv_Tuple_bbox_row1.TupleConcat(
                                            ((hv_Row1s.TupleSelect(hv_Index3)) * 2) + (hv_TupleRow1.TupleSelect(
                                            (hv_Index2 + hv_i) - 1)));
                                        hv_Tuple_bbox_row1.Dispose();
                                        hv_Tuple_bbox_row1 = ExpTmpLocalVar_Tuple_bbox_row1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_col1 = hv_Tuple_bbox_col1.TupleConcat(
                                            ((hv_Col1s.TupleSelect(hv_Index3)) * 2) + (hv_TupleColumn1.TupleSelect(
                                            (hv_Index2 + hv_i) - 1)));
                                        hv_Tuple_bbox_col1.Dispose();
                                        hv_Tuple_bbox_col1 = ExpTmpLocalVar_Tuple_bbox_col1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_row2 = hv_Tuple_bbox_row2.TupleConcat(
                                            ((hv_Row2s.TupleSelect(hv_Index3)) * 2) + (hv_TupleRow1.TupleSelect(
                                            (hv_Index2 + hv_i) - 1)));
                                        hv_Tuple_bbox_row2.Dispose();
                                        hv_Tuple_bbox_row2 = ExpTmpLocalVar_Tuple_bbox_row2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_col2 = hv_Tuple_bbox_col2.TupleConcat(
                                            ((hv_Col2s.TupleSelect(hv_Index3)) * 2) + (hv_TupleColumn1.TupleSelect(
                                            (hv_Index2 + hv_i) - 1)));
                                        hv_Tuple_bbox_col2.Dispose();
                                        hv_Tuple_bbox_col2 = ExpTmpLocalVar_Tuple_bbox_col2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_class_id = hv_Tuple_bbox_class_id.TupleConcat(
                                            hv_IDs.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_class_id.Dispose();
                                        hv_Tuple_bbox_class_id = ExpTmpLocalVar_Tuple_bbox_class_id;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_class_name = hv_Tuple_bbox_class_name.TupleConcat(
                                            hv_Names.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_class_name.Dispose();
                                        hv_Tuple_bbox_class_name = ExpTmpLocalVar_Tuple_bbox_class_name;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_confidence = hv_Tuple_bbox_confidence.TupleConcat(
                                            hv_Confidences.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_confidence.Dispose();
                                        hv_Tuple_bbox_confidence = ExpTmpLocalVar_Tuple_bbox_confidence;
                                    }
                                }
                            }
                            else
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_row1 = hv_Tuple_bbox_row1.TupleConcat(
                                            (hv_Row1s.TupleSelect(hv_Index3)) + (hv_TupleRow1.TupleSelect((hv_Index2 + hv_i) - 1)));
                                        hv_Tuple_bbox_row1.Dispose();
                                        hv_Tuple_bbox_row1 = ExpTmpLocalVar_Tuple_bbox_row1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_col1 = hv_Tuple_bbox_col1.TupleConcat(
                                            (hv_Col1s.TupleSelect(hv_Index3)) + (hv_TupleColumn1.TupleSelect(
                                            (hv_Index2 + hv_i) - 1)));
                                        hv_Tuple_bbox_col1.Dispose();
                                        hv_Tuple_bbox_col1 = ExpTmpLocalVar_Tuple_bbox_col1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_row2 = hv_Tuple_bbox_row2.TupleConcat(
                                            (hv_Row2s.TupleSelect(hv_Index3)) + (hv_TupleRow1.TupleSelect((hv_Index2 + hv_i) - 1)));
                                        hv_Tuple_bbox_row2.Dispose();
                                        hv_Tuple_bbox_row2 = ExpTmpLocalVar_Tuple_bbox_row2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_col2 = hv_Tuple_bbox_col2.TupleConcat(
                                            (hv_Col2s.TupleSelect(hv_Index3)) + (hv_TupleColumn1.TupleSelect(
                                            (hv_Index2 + hv_i) - 1)));
                                        hv_Tuple_bbox_col2.Dispose();
                                        hv_Tuple_bbox_col2 = ExpTmpLocalVar_Tuple_bbox_col2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_class_id = hv_Tuple_bbox_class_id.TupleConcat(
                                            hv_IDs.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_class_id.Dispose();
                                        hv_Tuple_bbox_class_id = ExpTmpLocalVar_Tuple_bbox_class_id;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_class_name = hv_Tuple_bbox_class_name.TupleConcat(
                                            hv_Names.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_class_name.Dispose();
                                        hv_Tuple_bbox_class_name = ExpTmpLocalVar_Tuple_bbox_class_name;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_confidence = hv_Tuple_bbox_confidence.TupleConcat(
                                            hv_Confidences.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_confidence.Dispose();
                                        hv_Tuple_bbox_confidence = ExpTmpLocalVar_Tuple_bbox_confidence;
                                    }
                                }
                            }

                        }

                    }

                }

                if ((int)(new HTuple((new HTuple(hv_Tuple_bbox_row1.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    ho_DLResultAllRectangles.Dispose();
                    HOperatorSet.GenRectangle1(out ho_DLResultAllRectangles, hv_Tuple_bbox_row1,
                        hv_Tuple_bbox_col1, hv_Tuple_bbox_row2, hv_Tuple_bbox_col2);
                }

                hv_EndDL.Dispose();
                HOperatorSet.CountSeconds(out hv_EndDL);
                hv_SecondsDL.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsDL = hv_EndDL - hv_StartDL;
                }
                //*********************************************************************************************************

                //后处理
                //Chip,Groove,Hole,Pit,Scratch
                //**********************************Chip********************************************************************
                hv_Tuple_bboxReduced_row1.Dispose();
                hv_Tuple_bboxReduced_row1 = new HTuple();
                hv_Tuple_bboxReduced_col1.Dispose();
                hv_Tuple_bboxReduced_col1 = new HTuple();
                hv_Tuple_bboxReduced_row2.Dispose();
                hv_Tuple_bboxReduced_row2 = new HTuple();
                hv_Tuple_bboxReduced_col2.Dispose();
                hv_Tuple_bboxReduced_col2 = new HTuple();
                hv_Tuple_bboxReduced_class_id.Dispose();
                hv_Tuple_bboxReduced_class_id = new HTuple();
                hv_Tuple_bboxReduced_class_name.Dispose();
                hv_Tuple_bboxReduced_class_name = new HTuple();
                //Tuple_bboxReduced_confidence := []



                if ((int)((new HTuple(hv_cameraPosition.TupleEqual(1))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    2)))) != 0)
                {
                    hv_ClassIDs.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);
                    hv_ClassNames.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
                }
                if ((int)((new HTuple(hv_cameraPosition.TupleEqual(3))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    4)))) != 0)
                {
                    hv_ClassIDs.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);
                    hv_ClassNames.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
                }
                if ((int)((new HTuple(hv_cameraPosition.TupleEqual(5))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    6)))) != 0)
                {
                    hv_ClassIDs.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);
                    hv_ClassNames.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
                }

                for (hv_IndexClass = 0; (int)hv_IndexClass <= (int)((new HTuple(hv_ClassIDs.TupleLength()
                    )) - 1); hv_IndexClass = (int)hv_IndexClass + 1)
                {
                    hv_Tuple_CurrentClass_bbox_row1.Dispose();
                    hv_Tuple_CurrentClass_bbox_row1 = new HTuple();
                    hv_Tuple_CurrentClass_bbox_col1.Dispose();
                    hv_Tuple_CurrentClass_bbox_col1 = new HTuple();
                    hv_Tuple_CurrentClass_bbox_row2.Dispose();
                    hv_Tuple_CurrentClass_bbox_row2 = new HTuple();
                    hv_Tuple_CurrentClass_bbox_col2.Dispose();
                    hv_Tuple_CurrentClass_bbox_col2 = new HTuple();
                    hv_Tuple_CurrentClass_bbox_class_id.Dispose();
                    hv_Tuple_CurrentClass_bbox_class_id = new HTuple();
                    hv_Tuple_CurrentClass_bbox_class_name.Dispose();
                    hv_Tuple_CurrentClass_bbox_class_name = new HTuple();
                    hv_Tuple_CurrentClass_bbox_confidence.Dispose();
                    hv_Tuple_CurrentClass_bbox_confidence = new HTuple();
                    for (hv_Index4 = 0; (int)hv_Index4 <= (int)((new HTuple(hv_Tuple_bbox_row1.TupleLength()
                        )) - 1); hv_Index4 = (int)hv_Index4 + 1)
                    {
                        if ((int)(new HTuple(((hv_Tuple_bbox_class_id.TupleSelect(hv_Index4))).TupleEqual(
                            hv_IndexClass))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_row1 = hv_Tuple_CurrentClass_bbox_row1.TupleConcat(
                                        hv_Tuple_bbox_row1.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_row1.Dispose();
                                    hv_Tuple_CurrentClass_bbox_row1 = ExpTmpLocalVar_Tuple_CurrentClass_bbox_row1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_col1 = hv_Tuple_CurrentClass_bbox_col1.TupleConcat(
                                        hv_Tuple_bbox_col1.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_col1.Dispose();
                                    hv_Tuple_CurrentClass_bbox_col1 = ExpTmpLocalVar_Tuple_CurrentClass_bbox_col1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_row2 = hv_Tuple_CurrentClass_bbox_row2.TupleConcat(
                                        hv_Tuple_bbox_row2.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_row2.Dispose();
                                    hv_Tuple_CurrentClass_bbox_row2 = ExpTmpLocalVar_Tuple_CurrentClass_bbox_row2;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_col2 = hv_Tuple_CurrentClass_bbox_col2.TupleConcat(
                                        hv_Tuple_bbox_col2.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_col2.Dispose();
                                    hv_Tuple_CurrentClass_bbox_col2 = ExpTmpLocalVar_Tuple_CurrentClass_bbox_col2;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_class_id = hv_Tuple_CurrentClass_bbox_class_id.TupleConcat(
                                        hv_Tuple_bbox_class_id.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_class_id.Dispose();
                                    hv_Tuple_CurrentClass_bbox_class_id = ExpTmpLocalVar_Tuple_CurrentClass_bbox_class_id;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_class_name = hv_Tuple_CurrentClass_bbox_class_name.TupleConcat(
                                        hv_Tuple_bbox_class_name.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_class_name.Dispose();
                                    hv_Tuple_CurrentClass_bbox_class_name = ExpTmpLocalVar_Tuple_CurrentClass_bbox_class_name;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_confidence = hv_Tuple_CurrentClass_bbox_confidence.TupleConcat(
                                        hv_Tuple_bbox_confidence.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_confidence.Dispose();
                                    hv_Tuple_CurrentClass_bbox_confidence = ExpTmpLocalVar_Tuple_CurrentClass_bbox_confidence;
                                }
                            }
                        }
                    }
                    if ((int)(new HTuple((new HTuple(hv_Tuple_CurrentClass_bbox_row1.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        ho_CurrentClassRectangles.Dispose();
                        HOperatorSet.GenRectangle1(out ho_CurrentClassRectangles, hv_Tuple_CurrentClass_bbox_row1,
                            hv_Tuple_CurrentClass_bbox_col1, hv_Tuple_CurrentClass_bbox_row2, hv_Tuple_CurrentClass_bbox_col2);
                        ho_CurrentClassRectanglesUnion.Dispose();
                        HOperatorSet.Union1(ho_CurrentClassRectangles, out ho_CurrentClassRectanglesUnion);
                        ho_CurrentClassRectanglesUnionsComponents.Dispose();
                        HOperatorSet.Connection(ho_CurrentClassRectanglesUnion, out ho_CurrentClassRectanglesUnionsComponents
                            );
                        ho_CurrentClassRectanglesReduced.Dispose();
                        HOperatorSet.ShapeTrans(ho_CurrentClassRectanglesUnionsComponents, out ho_CurrentClassRectanglesReduced,
                            "rectangle2");

                        hv_RectNum.Dispose();
                        HOperatorSet.CountObj(ho_CurrentClassRectanglesReduced, out hv_RectNum);

                        HTuple end_val540 = hv_RectNum;
                        HTuple step_val540 = 1;
                        for (hv_Index5 = 1; hv_Index5.Continue(end_val540, step_val540); hv_Index5 = hv_Index5.TupleAdd(step_val540))
                        {
                            ho_RectangleSelected.Dispose();
                            HOperatorSet.SelectObj(ho_CurrentClassRectanglesReduced, out ho_RectangleSelected,
                                hv_Index5);
                            hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                            HOperatorSet.SmallestRectangle1(ho_RectangleSelected, out hv_Row1, out hv_Column1,
                                out hv_Row2, out hv_Column2);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_row1 = hv_Tuple_bboxReduced_row1.TupleConcat(
                                        hv_Row1);
                                    hv_Tuple_bboxReduced_row1.Dispose();
                                    hv_Tuple_bboxReduced_row1 = ExpTmpLocalVar_Tuple_bboxReduced_row1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_col1 = hv_Tuple_bboxReduced_col1.TupleConcat(
                                        hv_Column1);
                                    hv_Tuple_bboxReduced_col1.Dispose();
                                    hv_Tuple_bboxReduced_col1 = ExpTmpLocalVar_Tuple_bboxReduced_col1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_row2 = hv_Tuple_bboxReduced_row2.TupleConcat(
                                        hv_Row2);
                                    hv_Tuple_bboxReduced_row2.Dispose();
                                    hv_Tuple_bboxReduced_row2 = ExpTmpLocalVar_Tuple_bboxReduced_row2;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_col2 = hv_Tuple_bboxReduced_col2.TupleConcat(
                                        hv_Column2);
                                    hv_Tuple_bboxReduced_col2.Dispose();
                                    hv_Tuple_bboxReduced_col2 = ExpTmpLocalVar_Tuple_bboxReduced_col2;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_class_id = hv_Tuple_bboxReduced_class_id.TupleConcat(
                                        hv_IndexClass);
                                    hv_Tuple_bboxReduced_class_id.Dispose();
                                    hv_Tuple_bboxReduced_class_id = ExpTmpLocalVar_Tuple_bboxReduced_class_id;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_class_name = hv_Tuple_bboxReduced_class_name.TupleConcat(
                                        hv_ClassNames.TupleSelect(hv_IndexClass));
                                    hv_Tuple_bboxReduced_class_name.Dispose();
                                    hv_Tuple_bboxReduced_class_name = ExpTmpLocalVar_Tuple_bboxReduced_class_name;
                                }
                            }
                            //Tuple_bboxReduced_confidence := [Tuple_bboxReduced_confidence, Confidences[Index3]]
                            TargetResult aStructureOrDefect = new TargetResult();
                            aStructureOrDefect.ID = hv_IndexClass.I;
                            aStructureOrDefect.TypeName = hv_ClassNames.TupleSelect(hv_IndexClass).S;
                            aStructureOrDefect.Row1 = hv_Row1.D;
                            aStructureOrDefect.Row2 = hv_Row2.D;
                            aStructureOrDefect.Column1 = hv_Column1.D;
                            aStructureOrDefect.Column2 = hv_Column2.D;
                            //detectResult.Confidence = hv_Confidences.TupleSelect(hv_Index3).D;
                            ListDetectResult.Add(aStructureOrDefect);
                        }
                    }
                }

                //if ((int)(new HTuple((new HTuple(hv_Tuple_bboxReduced_row1.TupleLength())).TupleGreater(
                //    0))) != 0)
                //{
                //    ho_RectanglesReduced.Dispose();
                //    HOperatorSet.GenRectangle1(out ho_RectanglesReduced, hv_Tuple_bboxReduced_row1,
                //        hv_Tuple_bboxReduced_col1, hv_Tuple_bboxReduced_row2, hv_Tuple_bboxReduced_col2);
                //}


                hv_EndAll.Dispose();
                HOperatorSet.CountSeconds(out hv_EndAll);
                hv_SecondsAll.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsAll = hv_EndAll - hv_StartAll;
                }
                results.Add("result", ListDetectResult);
                return results;
                //*********************************************************************************************************
            }
            catch (HalconException HDevExpDefaultException)
            {
                throw HDevExpDefaultException;
            }
            finally
            {
                ho_Image.Dispose();
                ho_ConveyerImage.Dispose();
                ho_ImageSub.Dispose();
                ho_Regions.Dispose();
                ho_TmpRegions.Dispose();
                ho_Regiondilation.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionRectangle1.Dispose();
                ho_ImageReduced.Dispose();

                ho_Regionclosing.Dispose();
                ho_SelectedRegion.Dispose();
                ho_Contours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_SingleSegment.Dispose();

                ho_Rectangle.Dispose();
                ho_Rectangles.Dispose();
                ho_SliceTuple.Dispose();
                ho_ImageSlice.Dispose();
                ho_ImageSliceScaled.Dispose();
                ho_SliceBatch.Dispose();
                ho_DLResultAllRectangles.Dispose();
                ho_CurrentClassRectangles.Dispose();
                ho_CurrentClassRectanglesUnion.Dispose();
                ho_CurrentClassRectanglesUnionsComponents.Dispose();
                ho_CurrentClassRectanglesReduced.Dispose();
                ho_RectanglesReduced.Dispose();
                ho_RectangleSelected.Dispose();

                hv_cameraPosition.Dispose();
                hv_NumSegments.Dispose();
                hv_NumLines.Dispose();
                hv_RowsBegin.Dispose();
                hv_ColsBegin.Dispose();
                hv_RowsEnd.Dispose();
                hv_ColsEnd.Dispose();
                hv_Attrib.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_Length.Dispose();
                hv_RowIntersect1.Dispose();
                hv_ColumnIntersect1.Dispose();
                hv_IsOverlapping1.Dispose();
                hv_RowIntersect2.Dispose();
                hv_ColumnIntersect2.Dispose();
                hv_IsOverlapping2.Dispose();
                hv_RowIntersect3.Dispose();
                hv_ColumnIntersect3.Dispose();
                hv_IsOverlapping3.Dispose();
                hv_RowIntersect4.Dispose();
                hv_ColumnIntersect4.Dispose();
                hv_IsOverlapping4.Dispose();

                hv_Anisometry.Dispose();
                hv_Bulkiness.Dispose();
                hv_StructureFactor.Dispose();


                hv_StartAll.Dispose();
                hv_ImageFileName.Dispose();
                hv_ConveyerImageFileName.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_StartPreprocess.Dispose();
                hv_UsedThreshold.Dispose();
                hv_Threshold.Dispose();
                hv_Number.Dispose();
                hv_EndPreprocess.Dispose();
                hv_SecondsPreprocess.Dispose();
                hv_StartSlice.Dispose();
                hv_SliceWidth.Dispose();
                hv_OverlapWidth.Dispose();
                hv_RowLarge1.Dispose();
                hv_ColumnLarge1.Dispose();
                hv_RowLarge2.Dispose();
                hv_ColumnLarge2.Dispose();
                hv_TupleRow1.Dispose();
                hv_TupleRow2.Dispose();
                hv_TupleColumn1.Dispose();
                hv_TupleColumn2.Dispose();
                hv_Row1.Dispose();
                hv_Row2.Dispose();
                hv_Column1.Dispose();
                hv_Column2.Dispose();
                hv_EndSlice.Dispose();
                hv_SecondsSlice.Dispose();
                hv_StartSliceTuple.Dispose();
                hv_Num.Dispose();
                hv_Index2.Dispose();
                hv_EndSliceTuple.Dispose();
                hv_SecondsSliceTuple.Dispose();
                hv_StartDL.Dispose();
                hv_DLResultAll.Dispose();
                hv_Tuple_bbox_row1.Dispose();
                hv_Tuple_bbox_col1.Dispose();
                hv_Tuple_bbox_row2.Dispose();
                hv_Tuple_bbox_col2.Dispose();
                hv_Tuple_bbox_class_id.Dispose();
                hv_Tuple_bbox_class_name.Dispose();
                hv_Tuple_bbox_confidence.Dispose();
                hv_startIndex.Dispose();
                hv_endIndex.Dispose();
                hv_DLSampleInferenceBatch.Dispose();
                hv_StartBatch.Dispose();
                hv_DLResultBatch.Dispose();
                hv_EndBatch.Dispose();
                hv_SecondsBatch.Dispose();
                hv_BatchNum.Dispose();
                hv_i.Dispose();
                hv_Row1s.Dispose();
                hv_ResultNum.Dispose();
                hv_Col1s.Dispose();
                hv_Row2s.Dispose();
                hv_Col2s.Dispose();
                hv_IDs.Dispose();
                hv_Names.Dispose();
                hv_Confidences.Dispose();
                hv_Index3.Dispose();
                hv_EndDL.Dispose();
                hv_SecondsDL.Dispose();

                hv_Tuple_bboxReduced_row1.Dispose();
                hv_Tuple_bboxReduced_col1.Dispose();
                hv_Tuple_bboxReduced_row2.Dispose();
                hv_Tuple_bboxReduced_col2.Dispose();
                hv_Tuple_bboxReduced_class_id.Dispose();
                hv_Tuple_bboxReduced_class_name.Dispose();
                hv_ClassIDs.Dispose();
                hv_ClassNames.Dispose();
                hv_IndexClass.Dispose();
                hv_Tuple_CurrentClass_bbox_row1.Dispose();
                hv_Tuple_CurrentClass_bbox_col1.Dispose();
                hv_Tuple_CurrentClass_bbox_row2.Dispose();
                hv_Tuple_CurrentClass_bbox_col2.Dispose();
                hv_Tuple_CurrentClass_bbox_class_id.Dispose();
                hv_Tuple_CurrentClass_bbox_class_name.Dispose();
                hv_Tuple_CurrentClass_bbox_confidence.Dispose();
                hv_Index4.Dispose();
                hv_RectNum.Dispose();
                hv_Index5.Dispose();

                hv_EndAll.Dispose();
                hv_SecondsAll.Dispose();
            }
        }
        private string BgImgOver = "D:\\Halcon\\BackGroundImage\\Over.jpg", BgImgLowerA = "D:\\Halcon\\BackGroundImage\\LowerA.jpg", BgImgLowerB = "D:\\Halcon\\BackGroundImage\\LowerB.jpg";
        public override bool Init(Dictionary<string, dynamic> initParam)
        {
            //初始化部分
            //*********************************************************************************************************

            HTuple hv_StartDLPreProcess = new HTuple();
            HTuple hv_EndDLPreProcess = new HTuple(), hv_SecondsDLPreProcess = new HTuple();

            HTuple hv_MinConfidence = new HTuple(), hv_MaxOverlap = new HTuple();
            HTuple hv_MaxOverlapClassAgnostic = new HTuple(), hv_DLDeviceHandles = new HTuple();
            HTuple hv_DLDeviceTensorRT = new HTuple();

            HTuple hv_ImageFileName = new HTuple();
            HObject ho_Image = null;


            //初始化模型。
            try
            {
                HOperatorSet.SetSystem("clip_region", "false");

                hv_StartDLPreProcess.Dispose();
                HOperatorSet.CountSeconds(out hv_StartDLPreProcess);
                hv_DLModelHandle.Dispose();
                HOperatorSet.ReadDlModel(initParam["HdlPath"], out hv_DLModelHandle);

                hv_BatchSize.Dispose();
                hv_BatchSize = 8;

                //HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", hv_BatchSize);

                //HOperatorSet.SetDlModelParam(hv_DLModelHandle, "optimize_for_inference", "true");

                //For optimized execution of the model, choose a TensorRT device.
                hv_DLDeviceHandles.Dispose();
                HOperatorSet.QueryAvailableDlDevices("ai_accelerator_interface", "tensorrt",
                    out hv_DLDeviceHandles);
                if (new HTuple(new HTuple(hv_DLDeviceHandles.TupleLength()).TupleGreater(
                    initParam["GPU"] - 1)) != 0)
                {
                    hv_DLDeviceTensorRT.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DLDeviceTensorRT = hv_DLDeviceHandles.TupleSelect(
                            initParam["GPU"]);
                    }
                }
                else
                {
                    throw new HalconException("No supported device found to continue this example. Ensure that the TensorRT Plugin is installed!");
                }

                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "device", hv_DLDeviceTensorRT);



                //HOperatorSet.SetDlModelParam(hv_DLModelHandle, "runtime_init", "immediately");

                //Get the parameters used for preprocessing.
                hv_DLPreprocessParam.Dispose();
                HOperatorSet.ReadDict(initParam["HdictPath"], new HTuple(), new HTuple(),
                    out hv_DLPreprocessParam);
                hv_EndDLPreProcess.Dispose();
                HOperatorSet.CountSeconds(out hv_EndDLPreProcess);
                hv_SecondsDLPreProcess.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsDLPreProcess = hv_EndDLPreProcess - hv_StartDLPreProcess;
                    Console.WriteLine(string.Format("SecondsDLPreProcess:{0:F2}", hv_SecondsDLPreProcess.D));
                }
                ResetCacheData();

                HOperatorSet.GenEmptyObj(out ho_BGImageLowerA);
                HOperatorSet.GenEmptyObj(out ho_BGImageLowerB);
                HOperatorSet.GenEmptyObj(out ho_BGImageOver);
                HOperatorSet.GenEmptyObj(out ho_Image);

                hv_ImageFileName.Dispose();
                hv_ImageFileName = BgImgLowerA;
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, hv_ImageFileName);
                ho_BGImageLowerA.Dispose();
                ProcessBackGroundImage(ho_Image, out ho_BGImageLowerA);

                hv_ImageFileName.Dispose();
                hv_ImageFileName = BgImgLowerB;
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, hv_ImageFileName);
                ho_BGImageLowerB.Dispose();
                ProcessBackGroundImage(ho_Image, out ho_BGImageLowerB);

                hv_ImageFileName.Dispose();
                hv_ImageFileName = BgImgOver;
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, hv_ImageFileName);
                ho_BGImageOver.Dispose();
                ProcessBackGroundImage(ho_Image, out ho_BGImageOver);
                return IsInit = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                hv_StartDLPreProcess.Dispose();
                hv_EndDLPreProcess.Dispose();
                hv_SecondsDLPreProcess.Dispose();
                hv_DLDeviceHandles.Dispose();
                hv_DLDeviceTensorRT.Dispose();
                hv_ImageFileName.Dispose();
                ho_Image.Dispose();

            }
        }

        public void ProcessBackGroundImage(HObject ho_BackGroundImageRaw, out HObject ho_BackGroundImageProcessed)
        {



            // Local iconic variables 

            HObject ho_ImageSmoothed;

            // Local control variables 

            HTuple hv_SliceWidth = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_RowBegin = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_BackGroundImageProcessed);
            HOperatorSet.GenEmptyObj(out ho_ImageSmoothed);
            try
            {
                hv_SliceWidth.Dispose();
                hv_SliceWidth = 1024;
                //平滑
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_BackGroundImageRaw, out hv_Width, out hv_Height);
                if ((int)(new HTuple(hv_Height.TupleLess(hv_SliceWidth))) != 0)
                {
                    throw new HalconException(new HTuple("Wrong image height, should be >=1024."));
                }
                ho_ImageSmoothed.Dispose();
                HOperatorSet.MedianImage(ho_BackGroundImageRaw, out ho_ImageSmoothed, "circle",
                    3, "mirrored");
                hv_RowBegin.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_RowBegin = hv_Height - hv_SliceWidth;
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_BackGroundImageProcessed.Dispose();
                    HOperatorSet.CropRectangle1(ho_ImageSmoothed, out ho_BackGroundImageProcessed,
                        hv_RowBegin, 0, hv_RowBegin + 1023, hv_Width - 1);
                }
                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                throw HDevExpDefaultException;
            }
            finally
            {
                ho_ImageSmoothed.Dispose();
                hv_SliceWidth.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_RowBegin.Dispose();
            }
        }
        public void DetectImageStrip(Dictionary<string, dynamic> actionParams)
        {
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 
            HObject ho_ImageSmoothed = null, ho_RegionToKeep = null;
            HObject ho_Regions = null, ho_BoardRegion = null, ho_Regionclosing = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegion = null;
            HObject ho_RegionRectangle1 = null, ho_Rectangle = null, ho_ImageReduced = null;
            HObject ho_ImageStrip = null, ho_Rectangles, ho_SliceTuple;
            HObject ho_ImageSlice = null, ho_ImageSliceScaled = null, ho_SliceBatch = null;
            HObject ho_RectangleCrop = null, ho_RegionIntersection = null;
            HObject ho_RegionHoleOrGroove = null, ho_RegionDifference = null;
            HObject ho_RegionSide = null, ho_RegionBoardFace = null, ho_RegionBoardFaceclosing = null;
            HObject ho_Contours = null;
            HObject ho_ContoursAffineTrans = null, ho_BoardContour = null;


            HObject ho_RegionTmp = null, ho_Seg = null;
            HObject ho_BGImage = null, ho_ImageSub = null;
            HTuple hv_SegNumber = new HTuple(), hv_Area = new HTuple();
            HTuple hv_TotalArea = new HTuple(), hv_AreaThreshold = new HTuple();
            HTuple hv_index = new HTuple(), hv_OrininalRegionHeight = new HTuple();


            HTuple hv_cameraPosition = new HTuple();
            HTuple hv_RowOffsetInWholeImage = new HTuple();
            // Local copy input parameter variables 
            HObject ho_Image_COPY_INP_TMP;
            // Local control variables 
            HTuple hv_StartPreprocess = new HTuple(), hv_SliceWidth = new HTuple();
            HTuple hv_OverlapWidth = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_Anisometry = new HTuple();
            HTuple hv_Bulkiness = new HTuple(), hv_StructureFactor = new HTuple();
            HTuple hv_SelectedRegionHeight = new HTuple(), hv_SelectedRegionWidth = new HTuple();
            HTuple hv_SelectedRegionRatio = new HTuple(), hv_RowLarge1 = new HTuple();
            HTuple hv_ColumnLarge1 = new HTuple(), hv_RowLarge2 = new HTuple();
            HTuple hv_ColumnLarge2 = new HTuple(), hv_StripRow1 = new HTuple();
            HTuple hv_StripColumn1 = new HTuple(), hv_StripRow2 = new HTuple();
            HTuple hv_StripColumn2 = new HTuple(), hv_EndPreprocess = new HTuple();
            HTuple hv_SecondsPreprocess = new HTuple(), hv_StartSlice = new HTuple();
            HTuple hv_TupleRow1 = new HTuple(), hv_TupleRow2 = new HTuple();
            HTuple hv_TupleColumn1 = new HTuple(), hv_TupleColumn2 = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Column2 = new HTuple();
            HTuple hv_EndSlice = new HTuple(), hv_SecondsSlice = new HTuple();
            HTuple hv_StartSliceTuple = new HTuple(), hv_Num = new HTuple();
            HTuple hv_Index2 = new HTuple(), hv_EndSliceTuple = new HTuple();
            HTuple hv_SecondsSliceTuple = new HTuple(), hv_StartDL = new HTuple();
            HTuple hv_DLResultAll = new HTuple(), hv_HoleOrGrooveInFirstColumn_row1 = new HTuple();
            HTuple hv_HoleOrGrooveInFirstColumn_col1 = new HTuple();
            HTuple hv_HoleOrGrooveInFirstColumn_row2 = new HTuple();
            HTuple hv_HoleOrGrooveInFirstColumn_col2 = new HTuple();
            HTuple hv_startIndex = new HTuple(), hv_endIndex = new HTuple();
            HTuple hv_DLSampleInferenceBatch = new HTuple(), hv_StartBatch = new HTuple();
            HTuple hv_DLResultBatch = new HTuple(), hv_EndBatch = new HTuple();
            HTuple hv_SecondsBatch = new HTuple(), hv_BatchNum = new HTuple();
            HTuple hv_i = new HTuple(), hv_Row1s = new HTuple(), hv_ResultNum = new HTuple();
            HTuple hv_Col1s = new HTuple(), hv_Row2s = new HTuple();
            HTuple hv_Col2s = new HTuple(), hv_IDs = new HTuple();
            HTuple hv_Names = new HTuple(), hv_Confidences = new HTuple();
            HTuple hv_Index3 = new HTuple(), hv_DistA = new HTuple();
            HTuple hv_PixelWidth = new HTuple(), hv_OffSet = new HTuple();
            HTuple hv_DistB = new HTuple(), hv_alpha = new HTuple();
            HTuple hv_DistC = new HTuple(), hv_CosAlpha = new HTuple();
            HTuple hv_DistD = new HTuple(), hv_DistD_pixel = new HTuple();
            HTuple hv_UsedThreshold1 = new HTuple(), hv_UsedThreshold2 = new HTuple();
            HTuple hv_LowerThreshold = new HTuple(), hv_EndDL = new HTuple();
            HTuple hv_BaseThreshold = new HTuple(), hv_Mean = new HTuple();
            HTuple hv_SecondsDL = new HTuple();

            HTuple hv_HomMat2DIdentity = new HTuple(), hv_HomMat2DTranslate = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_AreaContour = new HTuple(), hv_PointOrder = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_ContoursAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_BoardContour);

            HOperatorSet.GenEmptyObj(out ho_Seg);
            HOperatorSet.GenEmptyObj(out ho_RegionTmp);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image_COPY_INP_TMP);
            HOperatorSet.GenEmptyObj(out ho_BoardRegion);
            HOperatorSet.GenEmptyObj(out ho_Regions);
            HOperatorSet.GenEmptyObj(out ho_Regionclosing);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionRectangle1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImageStrip);
            HOperatorSet.GenEmptyObj(out ho_Rectangles);
            HOperatorSet.GenEmptyObj(out ho_SliceTuple);
            HOperatorSet.GenEmptyObj(out ho_ImageSlice);
            HOperatorSet.GenEmptyObj(out ho_ImageSliceScaled);
            HOperatorSet.GenEmptyObj(out ho_SliceBatch);
            HOperatorSet.GenEmptyObj(out ho_RectangleCrop);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionHoleOrGroove);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionSide);
            HOperatorSet.GenEmptyObj(out ho_RegionBoardFace);
            HOperatorSet.GenEmptyObj(out ho_RegionBoardFaceclosing);

            HOperatorSet.GenEmptyObj(out ho_ImageSmoothed);
            HOperatorSet.GenEmptyObj(out ho_RegionToKeep);
            HOperatorSet.GenEmptyObj(out ho_BGImage);
            HOperatorSet.GenEmptyObj(out ho_ImageSub);


            try
            {
                hv_StartPreprocess.Dispose();
                HOperatorSet.CountSeconds(out hv_StartPreprocess);
                //相机位置， 1:over，2:lower，3:side free, 4:side fixed, 5:front, 6:back.
                hv_cameraPosition.Dispose();
                if (actionParams["Direction"] == "Over")
                {
                    hv_cameraPosition = 1;
                }
                else if (actionParams["Direction"] == "LowerA")
                {
                    hv_cameraPosition = 2;
                }
                else if (actionParams["Direction"] == "SideFree")
                {
                    hv_cameraPosition = 3;
                }
                else if (actionParams["Direction"] == "SideFixed")
                {
                    hv_cameraPosition = 4;
                }
                else if (actionParams["Direction"] == "Front")
                {
                    hv_cameraPosition = 5;
                }
                else if (actionParams["Direction"] == "Back")
                {
                    hv_cameraPosition = 6;
                }
                else if (actionParams["Direction"] == "LowerB")
                {
                    hv_cameraPosition = 7;
                }
                ho_Image_COPY_INP_TMP.Dispose();
                ho_Image_COPY_INP_TMP = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
                hv_RowOffsetInWholeImage = actionParams["RowOffsetInWholeImage"];
                if ((int)(new HTuple(hv_cameraPosition.TupleEqual(1))) != 0)
                {
                    ho_BGImage.Dispose();
                    ho_BGImage = new HObject(ho_BGImageOver);
                }
                if ((int)(new HTuple(hv_cameraPosition.TupleEqual(2))) != 0)
                {
                    ho_BGImage.Dispose();
                    ho_BGImage = new HObject(ho_BGImageLowerA);
                }
                if ((int)(new HTuple(hv_cameraPosition.TupleEqual(7))) != 0)
                {
                    ho_BGImage.Dispose();
                    ho_BGImage = new HObject(ho_BGImageLowerB);
                }
                if ((int)((new HTuple((new HTuple(hv_cameraPosition.TupleEqual(1))).TupleOr(
                     new HTuple(hv_cameraPosition.TupleEqual(2))))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                     7)))) != 0)
                {
                    //小图像长和宽
                    hv_SliceWidth.Dispose();
                    hv_SliceWidth = 1024;
                    //重叠区域宽
                    hv_OverlapWidth.Dispose();
                    hv_OverlapWidth = 64;

                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_Image_COPY_INP_TMP, out hv_Width, out hv_Height);
                    //HOperatorSet.WriteImage(ho_Image_COPY_INP_TMP, "png", 0, "D:/1.png");

                    //排除右边亮区域
                    //if ((int)(new HTuple(hv_cameraPosition.TupleEqual(1))) != 0)
                    //{
                    //    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //    {
                    //        ho_RegionToKeep.Dispose();
                    //        HOperatorSet.GenRectangle1(out ho_RegionToKeep, 0, 0, hv_Height - 1, 16020);
                    //    }
                    //    ho_ImageReduced.Dispose();
                    //    HOperatorSet.ReduceDomain(ho_Image_COPY_INP_TMP, ho_RegionToKeep, out ho_ImageReduced
                    //        );
                    //    ho_Image_COPY_INP_TMP.Dispose();
                    //    ho_Image_COPY_INP_TMP = new HObject(ho_ImageReduced);
                    //}

                    if (new HTuple(hv_Height.TupleNotEqual(hv_SliceWidth)) != 0)
                    {
                        throw new HalconException(new HTuple("Wrong image height, should be 1024."));
                    }

                    //平滑一下用于分割的图像，以去除干扰。
                    ho_ImageSmoothed.Dispose();
                    HOperatorSet.MedianImage(ho_Image_COPY_INP_TMP, out ho_ImageSmoothed, "circle",
                        3, "mirrored");
                    //read_image (BGImage, 'D:/Halcon/志邦/BackGroundImage/LowerA_7.13_BG.png')
                    ho_ImageSub.Dispose();
                    HOperatorSet.SubImage(ho_ImageSmoothed, ho_BGImage, out ho_ImageSub, 1, 0);



                    //下相机背景更亮，用高一点的阈值
                    //if (cameraPosition==1)
                    //15->10
                    //threshold (ImageSub, Regions, 10, 255)
                    //elseif (cameraPosition==2)
                    //threshold (ImageSub, Regions, 15, 255)
                    //endif

                    //用动态阈值分割
                    //平滑，求背景
                    //mean_image (ImageSmoothed, ImageMean, 50, 50)
                    //动态阈值
                    //dyn_threshold (ImageSmoothed, ImageMean, Regions, 20, 'light')

                    //下相机背景更亮，用高一点的阈值
                    if ((int)(new HTuple(hv_cameraPosition.TupleEqual(1))) != 0)
                    {
                        //10->4
                        hv_BaseThreshold.Dispose();
                        hv_BaseThreshold = 4;
                    }
                    else if ((int)((new HTuple(hv_cameraPosition.TupleEqual(2))).TupleOr(
                        new HTuple(hv_cameraPosition.TupleEqual(7)))) != 0)
                    {
                        hv_BaseThreshold.Dispose();
                        hv_BaseThreshold = 6;
                    }



                    //binary_threshold (ImageSub, Regions, 'max_separability', 'light', UsedThreshold)
                    hv_Mean.Dispose();
                    HOperatorSet.GrayFeatures(ho_ImageSub, ho_ImageSub, "mean", out hv_Mean);
                    //min_max_gray (ImageSub, ImageSub, 50, Min, Max, Range)
                    hv_LowerThreshold.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_LowerThreshold = hv_BaseThreshold + (hv_Mean / 10);
                    }
                    //LowerThreshold := UsedThreshold/2
                    ho_Regions.Dispose();
                    HOperatorSet.Threshold(ho_ImageSub, out ho_Regions, hv_LowerThreshold, 255);

                    ho_Regionclosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Regions, out ho_Regionclosing, 5);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Regionclosing, out ho_ConnectedRegions);
                    ho_SelectedRegion.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegion, "max_area",
                        70);

                    hv_Anisometry.Dispose(); hv_Bulkiness.Dispose(); hv_StructureFactor.Dispose();
                    HOperatorSet.Eccentricity(ho_SelectedRegion, out hv_Anisometry, out hv_Bulkiness,
                        out hv_StructureFactor);

                    //考虑图像条中只包括很窄一部分板材区域的情况。
                    hv_SelectedRegionHeight.Dispose(); hv_SelectedRegionWidth.Dispose(); hv_SelectedRegionRatio.Dispose();
                    HOperatorSet.HeightWidthRatio(ho_SelectedRegion, out hv_SelectedRegionHeight,
                        out hv_SelectedRegionWidth, out hv_SelectedRegionRatio);
                    if (new HTuple(hv_SelectedRegionRatio.TupleGreater(1)) != 0)
                    {
                        //HObject ExpTmpOutVar_0;
                        //HOperatorSet.GenEmptyRegion(out ho_BoardRegion);
                        //HOperatorSet.ConcatObj(ho_RegionTuple, ho_BoardRegion, out ExpTmpOutVar_0);
                        //ho_RegionTuple.Dispose();
                        //ho_RegionTuple = ExpTmpOutVar_0;

                        //HTuple ExpTmpLocalVar_Tuple_bbox_row1 = hv_Tuple_bbox_row1.TupleConcat(new HTuple());
                        //hv_Tuple_bbox_row1.Dispose();
                        //hv_Tuple_bbox_row1 = ExpTmpLocalVar_Tuple_bbox_row1;

                        //HTuple ExpTmpLocalVar_Tuple_bbox_col1 = hv_Tuple_bbox_col1.TupleConcat(new HTuple());
                        //hv_Tuple_bbox_col1.Dispose();
                        //hv_Tuple_bbox_col1 = ExpTmpLocalVar_Tuple_bbox_col1;

                        //HTuple ExpTmpLocalVar_Tuple_bbox_row2 = hv_Tuple_bbox_row2.TupleConcat(new HTuple());
                        //hv_Tuple_bbox_row2.Dispose();
                        //hv_Tuple_bbox_row2 = ExpTmpLocalVar_Tuple_bbox_row2;

                        //HTuple ExpTmpLocalVar_Tuple_bbox_col2 = hv_Tuple_bbox_col2.TupleConcat(new HTuple());
                        //hv_Tuple_bbox_col2.Dispose();
                        //hv_Tuple_bbox_col2 = ExpTmpLocalVar_Tuple_bbox_col2;

                        //HTuple ExpTmpLocalVar_Tuple_bbox_class_id = hv_Tuple_bbox_class_id.TupleConcat(new HTuple());
                        //hv_Tuple_bbox_class_id.Dispose();
                        //hv_Tuple_bbox_class_id = ExpTmpLocalVar_Tuple_bbox_class_id;

                        //HTuple ExpTmpLocalVar_Tuple_bbox_class_name = hv_Tuple_bbox_class_name.TupleConcat(new HTuple());
                        //hv_Tuple_bbox_class_name.Dispose();
                        //hv_Tuple_bbox_class_name = ExpTmpLocalVar_Tuple_bbox_class_name;

                        //HTuple ExpTmpLocalVar_Tuple_bbox_confidence = hv_Tuple_bbox_confidence.TupleConcat(new HTuple());
                        //hv_Tuple_bbox_confidence.Dispose();
                        //hv_Tuple_bbox_confidence = ExpTmpLocalVar_Tuple_bbox_confidence;
                        return;
                    }

                    ho_RegionRectangle1.Dispose();
                    HOperatorSet.ShapeTrans(ho_SelectedRegion, out ho_RegionRectangle1, "rectangle1");

                    hv_RowLarge1.Dispose(); hv_ColumnLarge1.Dispose(); hv_RowLarge2.Dispose(); hv_ColumnLarge2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_RegionRectangle1, out hv_RowLarge1, out hv_ColumnLarge1,
                        out hv_RowLarge2, out hv_ColumnLarge2);
                    hv_OrininalRegionHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_OrininalRegionHeight = hv_RowLarge2 - hv_RowLarge1;
                    }

                }
                //3:side
                if (new HTuple(hv_cameraPosition.TupleEqual(3)).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    4))) != 0)
                {
                    //小图像长和宽
                    hv_SliceWidth.Dispose();
                    hv_SliceWidth = 1024;
                    //重叠区域宽
                    hv_OverlapWidth.Dispose();
                    hv_OverlapWidth = 64;

                    //旋转图像，以便标注时人眼查看。
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.RotateImage(ho_Image_COPY_INP_TMP, out ExpTmpOutVar_0, 270,
                            "constant");
                        ho_Image_COPY_INP_TMP.Dispose();
                        ho_Image_COPY_INP_TMP = ExpTmpOutVar_0;
                    }

                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_Image_COPY_INP_TMP, out hv_Width, out hv_Height);
                    if (new HTuple(hv_Height.TupleNotEqual(hv_SliceWidth)) != 0)
                    {
                        throw new HalconException(new HTuple("Wrong image height, should be 1024."));
                    }
                    hv_RowLarge1.Dispose();
                    hv_RowLarge1 = 0;
                    hv_ColumnLarge1.Dispose();
                    hv_ColumnLarge1 = 0;
                    hv_RowLarge2.Dispose();
                    hv_RowLarge2 = 1023;
                    hv_ColumnLarge2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColumnLarge2 = hv_Width - 1;
                    }
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, hv_RowLarge1, hv_ColumnLarge1,
                        hv_RowLarge2, hv_ColumnLarge2);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image_COPY_INP_TMP, ho_Rectangle, out ho_ImageReduced
                        );

                }

                //相机位置，5:front.6:back.
                if (new HTuple(hv_cameraPosition.TupleEqual(5)).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    6))) != 0)
                {
                    //由于前后相机分辨率不高，因此，直接切割512*512大小的小图像，而不是切割1024*1024然后缩小为512*512。
                    //小图像长和宽
                    hv_SliceWidth.Dispose();
                    hv_SliceWidth = 512;
                    //重叠区域宽
                    hv_OverlapWidth.Dispose();
                    hv_OverlapWidth = 32;

                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_Image_COPY_INP_TMP, out hv_Width, out hv_Height);

                    hv_StripRow1.Dispose();
                    hv_StripRow1 = new HTuple(hv_RowOffsetInWholeImage);
                    hv_StripColumn1.Dispose();
                    hv_StripColumn1 = 0;
                    hv_StripRow2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_StripRow2 = hv_RowOffsetInWholeImage + hv_SliceWidth - 1;
                    }
                    hv_StripColumn2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_StripColumn2 = hv_Width - 1;
                    }

                    ho_ImageStrip.Dispose();
                    HOperatorSet.CropRectangle1(ho_Image_COPY_INP_TMP, out ho_ImageStrip, hv_StripRow1,
                        hv_StripColumn1, hv_StripRow2, hv_StripColumn2);

                    ho_Image_COPY_INP_TMP.Dispose();
                    ho_Image_COPY_INP_TMP = new HObject(ho_ImageStrip);
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_Image_COPY_INP_TMP, out hv_Width, out hv_Height);
                    if (new HTuple(hv_Height.TupleNotEqual(hv_SliceWidth)) != 0)
                    {
                        throw new HalconException(new HTuple("Wrong image height, should be 512."));
                    }
                    hv_RowLarge1.Dispose();
                    hv_RowLarge1 = 0;
                    hv_ColumnLarge1.Dispose();
                    hv_ColumnLarge1 = 0;
                    hv_RowLarge2.Dispose();
                    hv_RowLarge2 = 511;
                    hv_ColumnLarge2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColumnLarge2 = hv_Width - 1;
                    }
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, hv_RowLarge1, hv_ColumnLarge1,
                        hv_RowLarge2, hv_ColumnLarge2);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image_COPY_INP_TMP, ho_Rectangle, out ho_ImageReduced
                        );

                }

                hv_EndPreprocess.Dispose();
                HOperatorSet.CountSeconds(out hv_EndPreprocess);
                hv_SecondsPreprocess.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsPreprocess = hv_EndPreprocess - hv_StartPreprocess;
                }
                //*****************************************************************************************************
                //对于一条图像，所有行进行后续处理。
                hv_RowLarge1.Dispose();
                hv_RowLarge1 = 0;
                hv_RowLarge2.Dispose();
                hv_RowLarge2 = new HTuple(hv_SliceWidth);

                hv_StartSlice.Dispose();
                HOperatorSet.CountSeconds(out hv_StartSlice);
                //滑窗，将木板区域全分辨率图像切割为SliceWidth*SliceWidth的小图像。
                //首先，生成矩形框。

                //矩形框坐标
                hv_TupleRow1.Dispose();
                hv_TupleRow1 = new HTuple();
                hv_TupleRow2.Dispose();
                hv_TupleRow2 = new HTuple();
                hv_TupleColumn1.Dispose();
                hv_TupleColumn1 = new HTuple();
                hv_TupleColumn2.Dispose();
                hv_TupleColumn2 = new HTuple();

                hv_Row1.Dispose();
                hv_Row1 = new HTuple(hv_RowLarge1);
                hv_Row2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row2 = hv_RowLarge1 + hv_SliceWidth - 1;
                }


                hv_Column1.Dispose();
                hv_Column1 = new HTuple(hv_ColumnLarge1);
                hv_Column2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Column2 = hv_ColumnLarge1 + hv_SliceWidth - 1;
                }
                while (new HTuple(hv_Column2.TupleLessEqual(hv_ColumnLarge2)) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TupleRow1 = hv_TupleRow1.TupleConcat(
                                hv_Row1);
                            hv_TupleRow1.Dispose();
                            hv_TupleRow1 = ExpTmpLocalVar_TupleRow1;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TupleRow2 = hv_TupleRow2.TupleConcat(
                                hv_Row2);
                            hv_TupleRow2.Dispose();
                            hv_TupleRow2 = ExpTmpLocalVar_TupleRow2;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TupleColumn1 = hv_TupleColumn1.TupleConcat(
                                hv_Column1);
                            hv_TupleColumn1.Dispose();
                            hv_TupleColumn1 = ExpTmpLocalVar_TupleColumn1;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TupleColumn2 = hv_TupleColumn2.TupleConcat(
                                hv_Column2);
                            hv_TupleColumn2.Dispose();
                            hv_TupleColumn2 = ExpTmpLocalVar_TupleColumn2;
                        }
                    }

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Column1 = hv_Column1 + (hv_SliceWidth - hv_OverlapWidth);
                            hv_Column1.Dispose();
                            hv_Column1 = ExpTmpLocalVar_Column1;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Column2 = hv_Column2 + (hv_SliceWidth - hv_OverlapWidth);
                            hv_Column2.Dispose();
                            hv_Column2 = ExpTmpLocalVar_Column2;
                        }
                    }
                }
                if (new HTuple(hv_Column2.TupleGreater(hv_ColumnLarge2)) != 0)
                {
                    if (new HTuple(hv_Column2.TupleGreaterEqual(hv_Width - 1)) != 0)
                    {
                        hv_Column2.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Column2 = hv_Width - 1;
                        }
                    }
                    hv_Column1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Column1 = hv_Column2 - hv_SliceWidth + 1;
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TupleRow1 = hv_TupleRow1.TupleConcat(
                                hv_Row1);
                            hv_TupleRow1.Dispose();
                            hv_TupleRow1 = ExpTmpLocalVar_TupleRow1;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TupleRow2 = hv_TupleRow2.TupleConcat(
                                hv_Row2);
                            hv_TupleRow2.Dispose();
                            hv_TupleRow2 = ExpTmpLocalVar_TupleRow2;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TupleColumn1 = hv_TupleColumn1.TupleConcat(
                                hv_Column1);
                            hv_TupleColumn1.Dispose();
                            hv_TupleColumn1 = ExpTmpLocalVar_TupleColumn1;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_TupleColumn2 = hv_TupleColumn2.TupleConcat(
                                hv_Column2);
                            hv_TupleColumn2.Dispose();
                            hv_TupleColumn2 = ExpTmpLocalVar_TupleColumn2;
                        }
                    }
                }

                ho_Rectangles.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangles, hv_TupleRow1, hv_TupleColumn1,
                    hv_TupleRow2, hv_TupleColumn2);
                hv_EndSlice.Dispose();
                HOperatorSet.CountSeconds(out hv_EndSlice);
                hv_SecondsSlice.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsSlice = hv_EndSlice - hv_StartSlice;
                }
                //********************************************************************************************************

                //生成小图像tuple.
                hv_StartSliceTuple.Dispose();
                HOperatorSet.CountSeconds(out hv_StartSliceTuple);
                ho_SliceTuple.Dispose();
                HOperatorSet.GenEmptyObj(out ho_SliceTuple);
                hv_Num.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Num = new HTuple(hv_TupleRow1.TupleLength()
                        );
                }
                HTuple end_val149 = hv_Num - 1;
                HTuple step_val149 = 1;
                for (hv_Index2 = 0; hv_Index2.Continue(end_val149, step_val149); hv_Index2 = hv_Index2.TupleAdd(step_val149))
                {

                    //select_obj (Rectangles, RectangleSelected, Index2)
                    //reduce_domain (Image, RectangleSelected, ImageSliceReduced)
                    //crop_domain (ImageSliceReduced, ImageSlice)

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ImageSlice.Dispose();
                        HOperatorSet.CropRectangle1(ho_Image_COPY_INP_TMP, out ho_ImageSlice, hv_TupleRow1.TupleSelect(
                            hv_Index2), hv_TupleColumn1.TupleSelect(hv_Index2), hv_TupleRow2.TupleSelect(
                            hv_Index2), hv_TupleColumn2.TupleSelect(hv_Index2));
                    }

                    //由于前后相机直接切割为512*512大小的小图像，因此不用缩小。
                    if (new HTuple(hv_cameraPosition.TupleEqual(5)).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                        6))) != 0)
                    {
                        ho_ImageSliceScaled.Dispose();
                        ho_ImageSliceScaled = new HObject(ho_ImageSlice);
                    }
                    else
                    {
                        ho_ImageSliceScaled.Dispose();
                        HOperatorSet.ZoomImageFactor(ho_ImageSlice, out ho_ImageSliceScaled, 0.5,
                            0.5, "bicubic");
                    }

                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConcatObj(ho_SliceTuple, ho_ImageSliceScaled, out ExpTmpOutVar_0
                            );
                        ho_SliceTuple.Dispose();
                        ho_SliceTuple = ExpTmpOutVar_0;
                    }
                }

                hv_EndSliceTuple.Dispose();
                HOperatorSet.CountSeconds(out hv_EndSliceTuple);
                hv_SecondsSliceTuple.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsSliceTuple = hv_EndSliceTuple - hv_StartSliceTuple;
                }

                //**************************************************************************************************************

                //将切割出的小图送入模型进行目标检测
                //所有小图中结果转换坐标后存入同一个字典

                hv_StartDL.Dispose();
                HOperatorSet.CountSeconds(out hv_StartDL);
                hv_DLResultAll.Dispose();
                HOperatorSet.CreateDict(out hv_DLResultAll);
                //hv_Tuple_bbox_row1.Dispose();
                //hv_Tuple_bbox_row1 = new HTuple();
                //hv_Tuple_bbox_col1.Dispose();
                //hv_Tuple_bbox_col1 = new HTuple();
                //hv_Tuple_bbox_row2.Dispose();
                //hv_Tuple_bbox_row2 = new HTuple();
                //hv_Tuple_bbox_col2.Dispose();
                //hv_Tuple_bbox_col2 = new HTuple();
                //hv_Tuple_bbox_class_id.Dispose();
                //hv_Tuple_bbox_class_id = new HTuple();
                //hv_Tuple_bbox_class_name.Dispose();
                //hv_Tuple_bbox_class_name = new HTuple();
                //hv_Tuple_bbox_confidence.Dispose();
                //hv_Tuple_bbox_confidence = new HTuple();
                hv_Num.Dispose();
                HOperatorSet.CountObj(ho_SliceTuple, out hv_Num);

                //左侧第一个小图中的孔或槽。
                hv_HoleOrGrooveInFirstColumn_row1.Dispose();
                hv_HoleOrGrooveInFirstColumn_row1 = new HTuple();
                hv_HoleOrGrooveInFirstColumn_col1.Dispose();
                hv_HoleOrGrooveInFirstColumn_col1 = new HTuple();
                hv_HoleOrGrooveInFirstColumn_row2.Dispose();
                hv_HoleOrGrooveInFirstColumn_row2 = new HTuple();
                hv_HoleOrGrooveInFirstColumn_col2.Dispose();
                hv_HoleOrGrooveInFirstColumn_col2 = new HTuple();

                HTuple end_val193 = hv_Num;
                HTuple step_val193 = hv_BatchSize;
                for (hv_Index2 = 1; hv_Index2.Continue(end_val193, step_val193); hv_Index2 = hv_Index2.TupleAdd(step_val193))
                {
                    hv_startIndex.Dispose();
                    hv_startIndex = new HTuple(hv_Index2);
                    hv_endIndex.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_endIndex = (hv_Index2 + hv_BatchSize - 1).TupleMin2(
                            hv_Num);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_SliceBatch.Dispose();
                        HOperatorSet.SelectObj(ho_SliceTuple, out ho_SliceBatch, HTuple.TupleGenSequence(
                            hv_startIndex, hv_endIndex, 1));
                    }
                    hv_DLSampleInferenceBatch.Dispose();
                    gen_dl_samples_from_images(ho_SliceBatch, out hv_DLSampleInferenceBatch);

                    hv_StartBatch.Dispose();
                    HOperatorSet.CountSeconds(out hv_StartBatch);


                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_num_channels", 3);
                    preprocess_dl_samples(hv_DLSampleInferenceBatch, hv_DLPreprocessParam);
                    hv_DLResultBatch.Dispose();
                    HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleInferenceBatch,
                        new HTuple(), out hv_DLResultBatch);


                    hv_EndBatch.Dispose();
                    HOperatorSet.CountSeconds(out hv_EndBatch);
                    hv_SecondsBatch.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_SecondsBatch = hv_EndBatch - hv_StartBatch;
                    }
                    //stop ()
                    //*********************************************************************************************************

                    hv_BatchNum.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BatchNum = new HTuple(hv_DLResultBatch.TupleLength()
                            );
                    }

                    HTuple end_val226 = hv_BatchNum - 1;
                    HTuple step_val226 = 1;
                    for (hv_i = 0; hv_i.Continue(end_val226, step_val226); hv_i = hv_i.TupleAdd(step_val226))
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Row1s.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_row1",
                                out hv_Row1s);
                        }
                        hv_ResultNum.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ResultNum = new HTuple(hv_Row1s.TupleLength()
                                );
                        }
                        if (new HTuple(hv_ResultNum.TupleNotEqual(0)) != 0)
                        {
                            //get_dict_tuple (DLResult, 'bbox_row1', Row1s)
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Col1s.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_col1",
                                    out hv_Col1s);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Row2s.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_row2",
                                    out hv_Row2s);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Col2s.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_col2",
                                    out hv_Col2s);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_IDs.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_class_id",
                                    out hv_IDs);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Names.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_class_name",
                                    out hv_Names);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Confidences.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLResultBatch.TupleSelect(hv_i), "bbox_confidence",
                                    out hv_Confidences);
                            }
                        }
                        HTuple end_val238 = hv_ResultNum - 1;
                        HTuple step_val238 = 1;
                        for (hv_Index3 = 0; hv_Index3.Continue(end_val238, step_val238); hv_Index3 = hv_Index3.TupleAdd(step_val238))
                        {
                            //对于上下和两侧相机，因为小图缩小了一半，坐标需要乘以二
                            if (new HTuple(hv_cameraPosition.TupleNotEqual(5)).TupleAnd(new HTuple(hv_cameraPosition.TupleNotEqual(
                                6))) != 0)
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_row1 = hv_Tuple_bbox_row1.TupleConcat(
                                            (hv_Row1s.TupleSelect(hv_Index3) * 2) + hv_RowOffsetInWholeImage);
                                        hv_Tuple_bbox_row1.Dispose();
                                        hv_Tuple_bbox_row1 = ExpTmpLocalVar_Tuple_bbox_row1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_col1 = hv_Tuple_bbox_col1.TupleConcat(
                                            (hv_Col1s.TupleSelect(hv_Index3) * 2) + hv_TupleColumn1.TupleSelect(
                                            hv_Index2 + hv_i - 1));
                                        hv_Tuple_bbox_col1.Dispose();
                                        hv_Tuple_bbox_col1 = ExpTmpLocalVar_Tuple_bbox_col1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_row2 = hv_Tuple_bbox_row2.TupleConcat(
                                            (hv_Row2s.TupleSelect(hv_Index3) * 2) + hv_RowOffsetInWholeImage);
                                        hv_Tuple_bbox_row2.Dispose();
                                        hv_Tuple_bbox_row2 = ExpTmpLocalVar_Tuple_bbox_row2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_col2 = hv_Tuple_bbox_col2.TupleConcat(
                                            (hv_Col2s.TupleSelect(hv_Index3) * 2) + hv_TupleColumn1.TupleSelect(
                                            hv_Index2 + hv_i - 1));
                                        hv_Tuple_bbox_col2.Dispose();
                                        hv_Tuple_bbox_col2 = ExpTmpLocalVar_Tuple_bbox_col2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_class_id = hv_Tuple_bbox_class_id.TupleConcat(
                                            hv_IDs.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_class_id.Dispose();
                                        hv_Tuple_bbox_class_id = ExpTmpLocalVar_Tuple_bbox_class_id;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_class_name = hv_Tuple_bbox_class_name.TupleConcat(
                                            hv_Names.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_class_name.Dispose();
                                        hv_Tuple_bbox_class_name = ExpTmpLocalVar_Tuple_bbox_class_name;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_confidence = hv_Tuple_bbox_confidence.TupleConcat(
                                            hv_Confidences.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_confidence.Dispose();
                                        hv_Tuple_bbox_confidence = ExpTmpLocalVar_Tuple_bbox_confidence;
                                    }
                                }
                            }
                            else
                            {
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_row1 = hv_Tuple_bbox_row1.TupleConcat(
                                            hv_Row1s.TupleSelect(hv_Index3) + hv_RowOffsetInWholeImage);
                                        hv_Tuple_bbox_row1.Dispose();
                                        hv_Tuple_bbox_row1 = ExpTmpLocalVar_Tuple_bbox_row1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_col1 = hv_Tuple_bbox_col1.TupleConcat(
                                            hv_Col1s.TupleSelect(hv_Index3) + hv_TupleColumn1.TupleSelect(
                                            hv_Index2 + hv_i - 1));
                                        hv_Tuple_bbox_col1.Dispose();
                                        hv_Tuple_bbox_col1 = ExpTmpLocalVar_Tuple_bbox_col1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_row2 = hv_Tuple_bbox_row2.TupleConcat(
                                            hv_Row2s.TupleSelect(hv_Index3) + hv_RowOffsetInWholeImage);
                                        hv_Tuple_bbox_row2.Dispose();
                                        hv_Tuple_bbox_row2 = ExpTmpLocalVar_Tuple_bbox_row2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_col2 = hv_Tuple_bbox_col2.TupleConcat(
                                            hv_Col2s.TupleSelect(hv_Index3) + hv_TupleColumn1.TupleSelect(
                                            hv_Index2 + hv_i - 1));
                                        hv_Tuple_bbox_col2.Dispose();
                                        hv_Tuple_bbox_col2 = ExpTmpLocalVar_Tuple_bbox_col2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_class_id = hv_Tuple_bbox_class_id.TupleConcat(
                                            hv_IDs.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_class_id.Dispose();
                                        hv_Tuple_bbox_class_id = ExpTmpLocalVar_Tuple_bbox_class_id;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_class_name = hv_Tuple_bbox_class_name.TupleConcat(
                                            hv_Names.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_class_name.Dispose();
                                        hv_Tuple_bbox_class_name = ExpTmpLocalVar_Tuple_bbox_class_name;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_bbox_confidence = hv_Tuple_bbox_confidence.TupleConcat(
                                            hv_Confidences.TupleSelect(hv_Index3));
                                        hv_Tuple_bbox_confidence.Dispose();
                                        hv_Tuple_bbox_confidence = ExpTmpLocalVar_Tuple_bbox_confidence;
                                    }
                                }
                            }
                            if ((int)((new HTuple((new HTuple((new HTuple(hv_cameraPosition.TupleEqual(
                                1))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(2))))).TupleOr(
                                new HTuple(hv_cameraPosition.TupleEqual(7))))).TupleAnd(new HTuple(hv_Index2.TupleEqual(
                                1)))) != 0)
                            {
                                if (new HTuple(hv_Names.TupleSelect(hv_Index3).TupleEqual(
                                    "Hole")).TupleOr(new HTuple(hv_Names.TupleSelect(hv_Index3).TupleEqual(
                                    "Groove"))) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_HoleOrGrooveInFirstColumn_row1 = hv_HoleOrGrooveInFirstColumn_row1.TupleConcat(
                                                hv_Row1s.TupleSelect(hv_Index3) * 2);
                                            hv_HoleOrGrooveInFirstColumn_row1.Dispose();
                                            hv_HoleOrGrooveInFirstColumn_row1 = ExpTmpLocalVar_HoleOrGrooveInFirstColumn_row1;
                                        }
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_HoleOrGrooveInFirstColumn_col1 = hv_HoleOrGrooveInFirstColumn_col1.TupleConcat(
                                                (hv_Col1s.TupleSelect(hv_Index3) * 2) + hv_TupleColumn1.TupleSelect(
                                                hv_Index2 + hv_i - 1));
                                            hv_HoleOrGrooveInFirstColumn_col1.Dispose();
                                            hv_HoleOrGrooveInFirstColumn_col1 = ExpTmpLocalVar_HoleOrGrooveInFirstColumn_col1;
                                        }
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_HoleOrGrooveInFirstColumn_row2 = hv_HoleOrGrooveInFirstColumn_row2.TupleConcat(
                                                hv_Row2s.TupleSelect(hv_Index3) * 2);
                                            hv_HoleOrGrooveInFirstColumn_row2.Dispose();
                                            hv_HoleOrGrooveInFirstColumn_row2 = ExpTmpLocalVar_HoleOrGrooveInFirstColumn_row2;
                                        }
                                    }
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_HoleOrGrooveInFirstColumn_col2 = hv_HoleOrGrooveInFirstColumn_col2.TupleConcat(
                                                (hv_Col2s.TupleSelect(hv_Index3) * 2) + (hv_TupleColumn1.TupleSelect(
                                                hv_Index2 + hv_i - 1)));
                                            hv_HoleOrGrooveInFirstColumn_col2.Dispose();
                                            hv_HoleOrGrooveInFirstColumn_col2 = ExpTmpLocalVar_HoleOrGrooveInFirstColumn_col2;
                                        }
                                    }
                                }
                            }

                        }

                    }

                }
                if ((int)((new HTuple((new HTuple(hv_cameraPosition.TupleEqual(1))).TupleOr(
                      new HTuple(hv_cameraPosition.TupleEqual(2))))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                      7)))) != 0)
                {
                    //对于比较窄的板子，有可能拍到侧面，需要滤除侧面部分。
                    //板材首列大于图像一半宽度才有可能看到侧面。
                    //光轴不一定在图像中心，偏移量(向左)
                    if ((int)(new HTuple(hv_cameraPosition.TupleEqual(1))) != 0)
                    {
                        hv_OffSet.Dispose();
                        hv_OffSet = 500;
                    }
                    else if ((int)((new HTuple(hv_cameraPosition.TupleEqual(2))).TupleOr(
                        new HTuple(hv_cameraPosition.TupleEqual(7)))) != 0)
                    {
                        hv_OffSet.Dispose();
                        hv_OffSet = -200;
                    }
                    if ((int)(new HTuple(hv_ColumnLarge1.TupleGreater((hv_Width / 2) - hv_OffSet))) != 0)
                    {
                        //切割左边一小条分析。

                        //由于视线角度不一样，封边在图像中宽度不一样，因此需要计算封边在视线方向的投影宽度。

                        if (new HTuple(hv_cameraPosition.TupleEqual(1)) != 0)
                        {
                            //物距，mm
                            hv_DistA.Dispose();
                            hv_DistA = 952;
                            //每像素宽度 mm/pixel
                            hv_PixelWidth.Dispose();
                            hv_PixelWidth = 0.079725746;
                        }
                        else if ((int)((new HTuple(hv_cameraPosition.TupleEqual(2))).TupleOr(
                            new HTuple(hv_cameraPosition.TupleEqual(7)))) != 0)
                        {
                            hv_DistA.Dispose();
                            hv_DistA = 526;
                            hv_PixelWidth.Dispose();
                            hv_PixelWidth = 0.090454822;
                        }
                        //光轴到封边距离 mm
                        //光轴不一定在图像中心，偏移量
                        hv_OffSet.Dispose();
                        hv_OffSet = 500;
                        hv_DistB.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_DistB = (hv_ColumnLarge1 - ((hv_Width / 2) - hv_OffSet)) * hv_PixelWidth;
                        }
                        //投影角度
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_alpha.Dispose();
                            HOperatorSet.TupleAtan(hv_DistA / hv_DistB, out hv_alpha);
                        }
                        //最大封边宽度 mm
                        hv_DistC.Dispose();
                        hv_DistC = 35;
                        //投影宽度。mm
                        hv_CosAlpha.Dispose();
                        HOperatorSet.TupleCos(hv_alpha, out hv_CosAlpha);
                        hv_DistD.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_DistD = hv_DistC * hv_CosAlpha;
                        }
                        //投影宽度。pixel
                        hv_DistD_pixel.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_DistD_pixel = hv_DistD / hv_PixelWidth;
                        }

                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_RectangleCrop.Dispose();
                            HOperatorSet.GenRectangle1(out ho_RectangleCrop, hv_RowLarge1, hv_ColumnLarge1,
                                hv_RowLarge2, hv_ColumnLarge1 + (hv_DistD_pixel * 1.1));
                        }
                        ho_RegionIntersection.Dispose();
                        HOperatorSet.Intersection(ho_RectangleCrop, ho_SelectedRegion, out ho_RegionIntersection
                            );

                        //排除孔或槽区域，以免受其影响。
                        if (new HTuple(new HTuple(hv_HoleOrGrooveInFirstColumn_row1.TupleLength()
                            ).TupleGreater(0)) != 0)
                        {
                            ho_RegionHoleOrGroove.Dispose();
                            HOperatorSet.GenRectangle1(out ho_RegionHoleOrGroove, hv_HoleOrGrooveInFirstColumn_row1,
                                hv_HoleOrGrooveInFirstColumn_col1, hv_HoleOrGrooveInFirstColumn_row2,
                                hv_HoleOrGrooveInFirstColumn_col2);
                            ho_RegionDifference.Dispose();
                            HOperatorSet.Difference(ho_RegionIntersection, ho_RegionHoleOrGroove,
                                out ho_RegionDifference);
                            ho_ImageReduced.Dispose();
                            HOperatorSet.ReduceDomain(ho_Image_COPY_INP_TMP, ho_RegionDifference,
                                out ho_ImageReduced);
                        }
                        else
                        {
                            ho_ImageReduced.Dispose();
                            HOperatorSet.ReduceDomain(ho_Image_COPY_INP_TMP, ho_RegionIntersection,
                                out ho_ImageReduced);
                        }

                        //ho_RegionSide.Dispose(); hv_UsedThreshold1.Dispose();
                        //HOperatorSet.BinaryThreshold(ho_ImageReduced, out ho_RegionSide, "max_separability",
                        //    "dark", out hv_UsedThreshold1);
                        //ho_RegionSide.Dispose(); hv_UsedThreshold2.Dispose();
                        //HOperatorSet.BinaryThreshold(ho_ImageReduced, out ho_RegionSide, "smooth_histo",
                        //    "dark", out hv_UsedThreshold2);

                        //if (new HTuple(hv_UsedThreshold1.TupleLess(hv_UsedThreshold2)) != 0)
                        //{
                        //    hv_LowerThreshold.Dispose();
                        //    hv_LowerThreshold = new HTuple(hv_UsedThreshold1);
                        //}
                        //else
                        //{
                        //    hv_LowerThreshold.Dispose();
                        //    hv_LowerThreshold = new HTuple(hv_UsedThreshold2);
                        //}

                        //ho_RegionSide.Dispose();
                        //HOperatorSet.Threshold(ho_ImageReduced, out ho_RegionSide, 0, hv_LowerThreshold);

                        //用自动阈值法求取封边区域。
                        ho_Seg.Dispose();
                        HOperatorSet.AutoThreshold(ho_ImageReduced, out ho_Seg, 1.2);

                        //将灰度最低的区域合并起来，直到大于阈值。
                        hv_SegNumber.Dispose();
                        HOperatorSet.CountObj(ho_Seg, out hv_SegNumber);
                        ho_RegionSide.Dispose();
                        HOperatorSet.SelectObj(ho_Seg, out ho_RegionSide, 1);
                        hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                        HOperatorSet.AreaCenter(ho_RegionSide, out hv_Area, out hv_Row, out hv_Column);
                        hv_TotalArea.Dispose();
                        hv_TotalArea = new HTuple(hv_Area);

                        //hv_AreaThreshold.Dispose();
                        //hv_AreaThreshold = 10000;
                        //面积阈值根据投影宽度调整。
                        //AreaThreshold := 10000
                        hv_AreaThreshold.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AreaThreshold = (hv_DistD_pixel * hv_OrininalRegionHeight) * 0.2;
                        }



                        hv_index.Dispose();
                        hv_index = 2;
                        while ((int)((new HTuple(hv_TotalArea.TupleLess(hv_AreaThreshold))).TupleAnd(
                            new HTuple(hv_index.TupleLessEqual(hv_SegNumber)))) != 0)
                        {
                            ho_RegionTmp.Dispose();
                            HOperatorSet.SelectObj(ho_Seg, out ho_RegionTmp, hv_index);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_index = hv_index + 1;
                                    hv_index.Dispose();
                                    hv_index = ExpTmpLocalVar_index;
                                }
                            }
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.Union2(ho_RegionTmp, ho_RegionSide, out ExpTmpOutVar_0);
                                ho_RegionSide.Dispose();
                                ho_RegionSide = ExpTmpOutVar_0;
                            }
                            hv_TotalArea.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                            HOperatorSet.AreaCenter(ho_RegionSide, out hv_TotalArea, out hv_Row,
                                out hv_Column);
                        }

                        ho_RegionBoardFace.Dispose();
                        HOperatorSet.Difference(ho_SelectedRegion, ho_RegionSide, out ho_RegionBoardFace
                            );
                        ho_RegionBoardFaceclosing.Dispose();
                        HOperatorSet.ClosingCircle(ho_RegionBoardFace, out ho_RegionBoardFaceclosing,
                            11);
                        ho_ConnectedRegions.Dispose();
                        HOperatorSet.Connection(ho_RegionBoardFaceclosing, out ho_ConnectedRegions
                            );
                        ho_SelectedRegion.Dispose();
                        HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegion,
                            "max_area", 70);
                    }


                    //ho_BoardRegion.Dispose();
                    //HOperatorSet.MoveRegion(ho_SelectedRegion, out ho_BoardRegion, hv_RowOffsetInWholeImage,
                    //    0);

                    //HOperatorSet.AreaCenter(ho_BoardRegion, out hv_AreaContour, out hv_Row, out hv_Column);

                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_SelectedRegion, out ho_Contours, "center");
                    hv_HomMat2DIdentity.Dispose();
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                    hv_HomMat2DTranslate.Dispose();
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, hv_RowOffsetInWholeImage,
                        0, out hv_HomMat2DTranslate);
                    ho_ContoursAffineTrans.Dispose();
                    HOperatorSet.AffineTransContourXld(ho_Contours, out ho_ContoursAffineTrans,
                        hv_HomMat2DTranslate);
                    ho_BoardContour.Dispose();
                    ho_BoardContour = new HObject(ho_ContoursAffineTrans);

                    {
                        //area_center (BoardContour, Area, Row, Column)
                        hv_AreaContour.Dispose(); hv_Row.Dispose(); hv_Column.Dispose(); hv_PointOrder.Dispose();
                        HOperatorSet.AreaCenterXld(ho_BoardContour, out hv_AreaContour, out hv_Row,
                            out hv_Column, out hv_PointOrder);
                        //Empty region
                        if ((int)((new HTuple(hv_AreaContour.TupleEqual(0))).TupleOr(new HTuple((new HTuple(hv_AreaContour.TupleLength()
                            )).TupleEqual(0)))) != 0)
                        {
                            return;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ContoursTuple, ho_BoardContour, out ExpTmpOutVar_0
                                );
                            ho_ContoursTuple.Dispose();
                            ho_ContoursTuple = ExpTmpOutVar_0;
                        }
                    }


                    //ho_RegionRectangle1.Dispose();
                    //HOperatorSet.ShapeTrans(ho_SelectedRegion, out ho_RegionRectangle1, "rectangle1");

                    //hv_RowLarge1.Dispose(); hv_ColumnLarge1.Dispose(); hv_RowLarge2.Dispose(); hv_ColumnLarge2.Dispose();
                    //HOperatorSet.SmallestRectangle1(ho_RegionRectangle1, out hv_RowLarge1, out hv_ColumnLarge1,
                    //    out hv_RowLarge2, out hv_ColumnLarge2);

                    //stop ()
                    //dev_close_window ()

                    //********************************************************************************************************************

                }



                hv_EndDL.Dispose();
                HOperatorSet.CountSeconds(out hv_EndDL);
                hv_SecondsDL.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_SecondsDL = hv_EndDL - hv_StartDL;
                }
            }
            catch (HalconException HDevExpDefaultException)
            {
                throw HDevExpDefaultException;
            }
            finally
            {
                ho_Image_COPY_INP_TMP.Dispose();
                ho_BoardRegion.Dispose();
                ho_Regions.Dispose();
                ho_Regionclosing.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegion.Dispose();
                ho_RegionRectangle1.Dispose();
                ho_Rectangle.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageStrip.Dispose();
                ho_Rectangles.Dispose();
                ho_SliceTuple.Dispose();
                ho_ImageSlice.Dispose();
                ho_ImageSliceScaled.Dispose();
                ho_SliceBatch.Dispose();
                ho_RectangleCrop.Dispose();
                ho_RegionIntersection.Dispose();
                ho_RegionHoleOrGroove.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionSide.Dispose();
                ho_RegionBoardFace.Dispose();
                ho_RegionBoardFaceclosing.Dispose();

                ho_Contours.Dispose();
                ho_ContoursAffineTrans.Dispose();
                ho_BoardContour.Dispose();

                ho_Seg.Dispose();
                ho_RegionTmp.Dispose();
                hv_SegNumber.Dispose();
                hv_Area.Dispose();
                hv_TotalArea.Dispose();
                hv_AreaThreshold.Dispose();
                hv_index.Dispose();
                ho_ImageSmoothed.Dispose();
                ho_RegionToKeep.Dispose();
                ho_BGImage.Dispose();
                ho_ImageSub.Dispose();

                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DTranslate.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_AreaContour.Dispose();
                hv_PointOrder.Dispose();
                hv_OrininalRegionHeight.Dispose();
                hv_BaseThreshold.Dispose();
                hv_Mean.Dispose();

                hv_StartPreprocess.Dispose();
                hv_SliceWidth.Dispose();
                hv_OverlapWidth.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Anisometry.Dispose();
                hv_Bulkiness.Dispose();
                hv_StructureFactor.Dispose();
                hv_SelectedRegionHeight.Dispose();
                hv_SelectedRegionWidth.Dispose();
                hv_SelectedRegionRatio.Dispose();
                hv_RowLarge1.Dispose();
                hv_ColumnLarge1.Dispose();
                hv_RowLarge2.Dispose();
                hv_ColumnLarge2.Dispose();
                hv_StripRow1.Dispose();
                hv_StripColumn1.Dispose();
                hv_StripRow2.Dispose();
                hv_StripColumn2.Dispose();
                hv_EndPreprocess.Dispose();
                hv_SecondsPreprocess.Dispose();
                hv_StartSlice.Dispose();
                hv_TupleRow1.Dispose();
                hv_TupleRow2.Dispose();
                hv_TupleColumn1.Dispose();
                hv_TupleColumn2.Dispose();
                hv_Row1.Dispose();
                hv_Row2.Dispose();
                hv_Column1.Dispose();
                hv_Column2.Dispose();
                hv_EndSlice.Dispose();
                hv_SecondsSlice.Dispose();
                hv_StartSliceTuple.Dispose();
                hv_Num.Dispose();
                hv_Index2.Dispose();
                hv_EndSliceTuple.Dispose();
                hv_SecondsSliceTuple.Dispose();
                hv_StartDL.Dispose();
                hv_DLResultAll.Dispose();
                hv_HoleOrGrooveInFirstColumn_row1.Dispose();
                hv_HoleOrGrooveInFirstColumn_col1.Dispose();
                hv_HoleOrGrooveInFirstColumn_row2.Dispose();
                hv_HoleOrGrooveInFirstColumn_col2.Dispose();
                hv_startIndex.Dispose();
                hv_endIndex.Dispose();
                hv_DLSampleInferenceBatch.Dispose();
                hv_StartBatch.Dispose();
                hv_DLResultBatch.Dispose();
                hv_EndBatch.Dispose();
                hv_SecondsBatch.Dispose();
                hv_BatchNum.Dispose();
                hv_i.Dispose();
                hv_Row1s.Dispose();
                hv_ResultNum.Dispose();
                hv_Col1s.Dispose();
                hv_Row2s.Dispose();
                hv_Col2s.Dispose();
                hv_IDs.Dispose();
                hv_Names.Dispose();
                hv_Confidences.Dispose();
                hv_Index3.Dispose();
                hv_DistA.Dispose();
                hv_PixelWidth.Dispose();
                hv_OffSet.Dispose();
                hv_DistB.Dispose();
                hv_alpha.Dispose();
                hv_DistC.Dispose();
                hv_CosAlpha.Dispose();
                hv_DistD.Dispose();
                hv_DistD_pixel.Dispose();
                hv_UsedThreshold1.Dispose();
                hv_UsedThreshold2.Dispose();
                hv_LowerThreshold.Dispose();
                hv_EndDL.Dispose();
                hv_SecondsDL.Dispose();
            }
        }
        private void FindRegionCorner(HObject ho_Region, HTuple hv_cameraPosition, out HTuple hv_Tuple_corner_rows,
            out HTuple hv_Tuple_corner_cols)
        {

            // Local iconic variables 

            HObject ho_Regionclosing = null, ho_Regionopening = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegion = null;
            HObject ho_Contours = null, ho_ContoursSplit = null, ho_SingleSegment = null;
            HObject ho_CrossIntersection1 = null, ho_CrossIntersection2 = null;
            HObject ho_CrossIntersection3 = null, ho_CrossIntersection4 = null;

            // Local control variables 
            HObject ho_tmpContours = null, ho_SingleContour = null;
            HObject ho_SingleSegmentClip = null;



            HTuple hv_Tuple_RowIntersect = new HTuple(), hv_Tuple_ColumnIntersect = new HTuple();
            HTuple hv_tmp = new HTuple(), hv_RowIntersect = new HTuple();
            HTuple hv_ColumnIntersect = new HTuple(), hv_Length1 = new HTuple();
            HTuple hv_Length2 = new HTuple();
            HTuple hv_Tuple_ClipRowIntersect = new HTuple(), hv_Tuple_ClipColumnIntersect = new HTuple();
            HTuple hv_ClipRatio = new HTuple(), hv_LineSegmentIndex = new HTuple();
            HTuple hv_ClipRowBeginA = new HTuple(), hv_ClipColBeginA = new HTuple();
            HTuple hv_ClipRowEndA = new HTuple(), hv_ClipColEndA = new HTuple();
            HTuple hv_ClipRowBeginB = new HTuple(), hv_ClipColBeginB = new HTuple();
            HTuple hv_ClipRowEndB = new HTuple(), hv_ClipColEndB = new HTuple();
            HTuple hv_ClipRowIntersect = new HTuple(), hv_ClipColumnIntersect = new HTuple();
            HTuple hv_ClipIsOverlapping = new HTuple();

            HTuple hv_Anisometry = new HTuple(), hv_Bulkiness = new HTuple();
            HTuple hv_StructureFactor = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_row1 = new HTuple(), hv_column1 = new HTuple();
            HTuple hv_row2 = new HTuple(), hv_column2 = new HTuple();
            HTuple hv_rect2_len2 = new HTuple(), hv_EdgeWidth = new HTuple();
            HTuple hv_NumSegments = new HTuple(), hv_NumLines = new HTuple();
            HTuple hv_RowsBegin = new HTuple(), hv_ColsBegin = new HTuple();
            HTuple hv_RowsEnd = new HTuple(), hv_ColsEnd = new HTuple();
            HTuple hv_i = new HTuple(), hv_Attrib = new HTuple(), hv_RowBegin = new HTuple();
            HTuple hv_ColBegin = new HTuple(), hv_RowEnd = new HTuple();
            HTuple hv_ColEnd = new HTuple(), hv_Nr = new HTuple();
            HTuple hv_Nc = new HTuple(), hv_Dist = new HTuple(), hv_Length = new HTuple();
            HTuple hv_Phi = new HTuple(), hv_RowIntersect1 = new HTuple();
            HTuple hv_ColumnIntersect1 = new HTuple(), hv_IsOverlapping1 = new HTuple();
            HTuple hv_RowIntersect2 = new HTuple(), hv_ColumnIntersect2 = new HTuple();
            HTuple hv_IsOverlapping2 = new HTuple(), hv_RowIntersect3 = new HTuple();
            HTuple hv_ColumnIntersect3 = new HTuple(), hv_IsOverlapping3 = new HTuple();
            HTuple hv_RowIntersect4 = new HTuple(), hv_ColumnIntersect4 = new HTuple();
            HTuple hv_IsOverlapping4 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Regionclosing);
            HOperatorSet.GenEmptyObj(out ho_Regionopening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegion);
            HOperatorSet.GenEmptyObj(out ho_Contours);
            HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
            HOperatorSet.GenEmptyObj(out ho_SingleSegment);
            HOperatorSet.GenEmptyObj(out ho_CrossIntersection1);
            HOperatorSet.GenEmptyObj(out ho_CrossIntersection2);
            HOperatorSet.GenEmptyObj(out ho_CrossIntersection3);
            HOperatorSet.GenEmptyObj(out ho_CrossIntersection4);

            HOperatorSet.GenEmptyObj(out ho_tmpContours);
            HOperatorSet.GenEmptyObj(out ho_SingleContour);
            HOperatorSet.GenEmptyObj(out ho_SingleSegmentClip);

            hv_Tuple_corner_rows = new HTuple();
            hv_Tuple_corner_cols = new HTuple();
            try
            {
                if ((int)((new HTuple((new HTuple(hv_cameraPosition.TupleEqual(1))).TupleOr(
                      new HTuple(hv_cameraPosition.TupleEqual(2))))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                      7)))) != 0)
                {
                    ho_Regionclosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Region, out ho_Regionclosing, 50);
                    ho_Regionopening.Dispose();
                    HOperatorSet.OpeningCircle(ho_Regionclosing, out ho_Regionopening, 50);

                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Regionopening, out ho_ConnectedRegions);
                    ho_SelectedRegion.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegion, "max_area",
                        70);

                    hv_Anisometry.Dispose(); hv_Bulkiness.Dispose(); hv_StructureFactor.Dispose();
                    HOperatorSet.Eccentricity(ho_SelectedRegion, out hv_Anisometry, out hv_Bulkiness,
                        out hv_StructureFactor);

                    //滤除条纹干扰。
                    if ((int)(hv_cameraPosition.TupleAnd(new HTuple(hv_Anisometry.TupleGreater(
                        30)))) != 0)
                    {
                        //HDevelopStop();
                        ho_Regionclosing.Dispose();
                        ho_Regionopening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegion.Dispose();
                        ho_Contours.Dispose();
                        ho_ContoursSplit.Dispose();
                        ho_SingleSegment.Dispose();
                        ho_tmpContours.Dispose();
                        ho_SingleContour.Dispose();
                        ho_SingleSegmentClip.Dispose();

                        hv_Anisometry.Dispose();
                        hv_Bulkiness.Dispose();
                        hv_StructureFactor.Dispose();
                        hv_Area.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_row1.Dispose();
                        hv_column1.Dispose();
                        hv_row2.Dispose();
                        hv_column2.Dispose();
                        hv_rect2_len2.Dispose();
                        hv_EdgeWidth.Dispose();
                        hv_NumSegments.Dispose();
                        hv_NumLines.Dispose();
                        hv_RowsBegin.Dispose();
                        hv_ColsBegin.Dispose();
                        hv_RowsEnd.Dispose();
                        hv_ColsEnd.Dispose();
                        hv_i.Dispose();
                        hv_Attrib.Dispose();
                        hv_RowBegin.Dispose();
                        hv_ColBegin.Dispose();
                        hv_RowEnd.Dispose();
                        hv_ColEnd.Dispose();
                        hv_Nr.Dispose();
                        hv_Nc.Dispose();
                        hv_Dist.Dispose();
                        hv_Length.Dispose();
                        hv_Phi.Dispose();
                        hv_tmp.Dispose();
                        hv_Tuple_RowIntersect.Dispose();
                        hv_Tuple_ColumnIntersect.Dispose();
                        hv_RowIntersect.Dispose();
                        hv_ColumnIntersect.Dispose();
                        hv_IsOverlapping1.Dispose();
                        hv_IsOverlapping2.Dispose();
                        hv_IsOverlapping3.Dispose();
                        hv_IsOverlapping4.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Tuple_ClipRowIntersect.Dispose();
                        hv_Tuple_ClipColumnIntersect.Dispose();
                        hv_ClipRatio.Dispose();
                        hv_LineSegmentIndex.Dispose();
                        hv_ClipRowBeginA.Dispose();
                        hv_ClipColBeginA.Dispose();
                        hv_ClipRowEndA.Dispose();
                        hv_ClipColEndA.Dispose();
                        hv_ClipRowBeginB.Dispose();
                        hv_ClipColBeginB.Dispose();
                        hv_ClipRowEndB.Dispose();
                        hv_ClipColEndB.Dispose();
                        hv_ClipRowIntersect.Dispose();
                        hv_ClipColumnIntersect.Dispose();
                        hv_ClipIsOverlapping.Dispose();

                        return;
                    }

                    //滤除面积很小的干扰。
                    hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                    HOperatorSet.AreaCenter(ho_SelectedRegion, out hv_Area, out hv_Row, out hv_Column);
                    if ((int)(new HTuple(hv_Area.TupleLess(100000))) != 0)
                    {
                        //HDevelopStop();
                        ho_Regionclosing.Dispose();
                        ho_Regionopening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegion.Dispose();
                        ho_Contours.Dispose();
                        ho_ContoursSplit.Dispose();
                        ho_SingleSegment.Dispose();
                        ho_tmpContours.Dispose();
                        ho_SingleContour.Dispose();
                        ho_SingleSegmentClip.Dispose();

                        hv_Anisometry.Dispose();
                        hv_Bulkiness.Dispose();
                        hv_StructureFactor.Dispose();
                        hv_Area.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_row1.Dispose();
                        hv_column1.Dispose();
                        hv_row2.Dispose();
                        hv_column2.Dispose();
                        hv_rect2_len2.Dispose();
                        hv_EdgeWidth.Dispose();
                        hv_NumSegments.Dispose();
                        hv_NumLines.Dispose();
                        hv_RowsBegin.Dispose();
                        hv_ColsBegin.Dispose();
                        hv_RowsEnd.Dispose();
                        hv_ColsEnd.Dispose();
                        hv_i.Dispose();
                        hv_Attrib.Dispose();
                        hv_RowBegin.Dispose();
                        hv_ColBegin.Dispose();
                        hv_RowEnd.Dispose();
                        hv_ColEnd.Dispose();
                        hv_Nr.Dispose();
                        hv_Nc.Dispose();
                        hv_Dist.Dispose();
                        hv_Length.Dispose();
                        hv_Phi.Dispose();
                        hv_tmp.Dispose();
                        hv_Tuple_RowIntersect.Dispose();
                        hv_Tuple_ColumnIntersect.Dispose();
                        hv_RowIntersect.Dispose();
                        hv_ColumnIntersect.Dispose();
                        hv_IsOverlapping1.Dispose();
                        hv_IsOverlapping2.Dispose();
                        hv_IsOverlapping3.Dispose();
                        hv_IsOverlapping4.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Tuple_ClipRowIntersect.Dispose();
                        hv_Tuple_ClipColumnIntersect.Dispose();
                        hv_ClipRatio.Dispose();
                        hv_LineSegmentIndex.Dispose();
                        hv_ClipRowBeginA.Dispose();
                        hv_ClipColBeginA.Dispose();
                        hv_ClipRowEndA.Dispose();
                        hv_ClipColEndA.Dispose();
                        hv_ClipRowBeginB.Dispose();
                        hv_ClipColBeginB.Dispose();
                        hv_ClipRowEndB.Dispose();
                        hv_ClipColEndB.Dispose();
                        hv_ClipRowIntersect.Dispose();
                        hv_ClipColumnIntersect.Dispose();
                        hv_ClipIsOverlapping.Dispose();

                        return;
                    }

                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_SelectedRegion, out ho_Contours, "center");

                    ho_ContoursSplit.Dispose();
                    HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines",
                        201, 500, 250);
                }
                if ((int)((new HTuple(hv_cameraPosition.TupleEqual(3))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    4)))) != 0)
                {
                    //closing_circle (Region, Regionclosing, 50)

                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                    ho_SelectedRegion.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegion, "max_area",
                        70);

                    //滤除面积很小的干扰。
                    hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                    HOperatorSet.AreaCenter(ho_SelectedRegion, out hv_Area, out hv_Row, out hv_Column);
                    if ((int)(new HTuple(hv_Area.TupleLess(100000))) != 0)
                    {
                        //HDevelopStop();
                        ho_Regionclosing.Dispose();
                        ho_Regionopening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegion.Dispose();
                        ho_Contours.Dispose();
                        ho_ContoursSplit.Dispose();
                        ho_SingleSegment.Dispose();
                        ho_tmpContours.Dispose();
                        ho_SingleContour.Dispose();
                        ho_SingleSegmentClip.Dispose();

                        hv_Anisometry.Dispose();
                        hv_Bulkiness.Dispose();
                        hv_StructureFactor.Dispose();
                        hv_Area.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_row1.Dispose();
                        hv_column1.Dispose();
                        hv_row2.Dispose();
                        hv_column2.Dispose();
                        hv_rect2_len2.Dispose();
                        hv_EdgeWidth.Dispose();
                        hv_NumSegments.Dispose();
                        hv_NumLines.Dispose();
                        hv_RowsBegin.Dispose();
                        hv_ColsBegin.Dispose();
                        hv_RowsEnd.Dispose();
                        hv_ColsEnd.Dispose();
                        hv_i.Dispose();
                        hv_Attrib.Dispose();
                        hv_RowBegin.Dispose();
                        hv_ColBegin.Dispose();
                        hv_RowEnd.Dispose();
                        hv_ColEnd.Dispose();
                        hv_Nr.Dispose();
                        hv_Nc.Dispose();
                        hv_Dist.Dispose();
                        hv_Length.Dispose();
                        hv_Phi.Dispose();
                        hv_tmp.Dispose();
                        hv_Tuple_RowIntersect.Dispose();
                        hv_Tuple_ColumnIntersect.Dispose();
                        hv_RowIntersect.Dispose();
                        hv_ColumnIntersect.Dispose();
                        hv_IsOverlapping1.Dispose();
                        hv_IsOverlapping2.Dispose();
                        hv_IsOverlapping3.Dispose();
                        hv_IsOverlapping4.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Tuple_ClipRowIntersect.Dispose();
                        hv_Tuple_ClipColumnIntersect.Dispose();
                        hv_ClipRatio.Dispose();
                        hv_LineSegmentIndex.Dispose();
                        hv_ClipRowBeginA.Dispose();
                        hv_ClipColBeginA.Dispose();
                        hv_ClipRowEndA.Dispose();
                        hv_ClipColEndA.Dispose();
                        hv_ClipRowBeginB.Dispose();
                        hv_ClipColBeginB.Dispose();
                        hv_ClipRowEndB.Dispose();
                        hv_ClipColEndB.Dispose();
                        hv_ClipRowIntersect.Dispose();
                        hv_ClipColumnIntersect.Dispose();
                        hv_ClipIsOverlapping.Dispose();

                        return;
                    }

                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_SelectedRegion, out ho_Contours, "center");


                    ho_ContoursSplit.Dispose();
                    HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines",
                        151, 200, 100);

                }
                //由于前后相机封边位置倾斜角度较大，采用四边拟合直线的方式不适用，因此采用找到左上右下角，然后加上封边宽找右上左下角。
                if ((int)((new HTuple(hv_cameraPosition.TupleEqual(5))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                    6)))) != 0)
                {
                    //closing_circle (Region, Regionclosing, 50)

                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_Region, out ho_ConnectedRegions);
                    ho_SelectedRegion.Dispose();
                    HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegion, "max_area",
                        70);

                    //滤除面积很小的干扰。
                    hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                    HOperatorSet.AreaCenter(ho_SelectedRegion, out hv_Area, out hv_Row, out hv_Column);
                    if ((int)(new HTuple(hv_Area.TupleLess(10000))) != 0)
                    {
                        //HDevelopStop();
                        ho_Regionclosing.Dispose();
                        ho_Regionopening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegion.Dispose();
                        ho_Contours.Dispose();
                        ho_ContoursSplit.Dispose();
                        ho_SingleSegment.Dispose();
                        ho_tmpContours.Dispose();
                        ho_SingleContour.Dispose();
                        ho_SingleSegmentClip.Dispose();

                        hv_Anisometry.Dispose();
                        hv_Bulkiness.Dispose();
                        hv_StructureFactor.Dispose();
                        hv_Area.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_row1.Dispose();
                        hv_column1.Dispose();
                        hv_row2.Dispose();
                        hv_column2.Dispose();
                        hv_rect2_len2.Dispose();
                        hv_EdgeWidth.Dispose();
                        hv_NumSegments.Dispose();
                        hv_NumLines.Dispose();
                        hv_RowsBegin.Dispose();
                        hv_ColsBegin.Dispose();
                        hv_RowsEnd.Dispose();
                        hv_ColsEnd.Dispose();
                        hv_i.Dispose();
                        hv_Attrib.Dispose();
                        hv_RowBegin.Dispose();
                        hv_ColBegin.Dispose();
                        hv_RowEnd.Dispose();
                        hv_ColEnd.Dispose();
                        hv_Nr.Dispose();
                        hv_Nc.Dispose();
                        hv_Dist.Dispose();
                        hv_Length.Dispose();
                        hv_Phi.Dispose();
                        hv_tmp.Dispose();
                        hv_Tuple_RowIntersect.Dispose();
                        hv_Tuple_ColumnIntersect.Dispose();
                        hv_RowIntersect.Dispose();
                        hv_ColumnIntersect.Dispose();
                        hv_IsOverlapping1.Dispose();
                        hv_IsOverlapping2.Dispose();
                        hv_IsOverlapping3.Dispose();
                        hv_IsOverlapping4.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Tuple_ClipRowIntersect.Dispose();
                        hv_Tuple_ClipColumnIntersect.Dispose();
                        hv_ClipRatio.Dispose();
                        hv_LineSegmentIndex.Dispose();
                        hv_ClipRowBeginA.Dispose();
                        hv_ClipColBeginA.Dispose();
                        hv_ClipRowEndA.Dispose();
                        hv_ClipColEndA.Dispose();
                        hv_ClipRowBeginB.Dispose();
                        hv_ClipColBeginB.Dispose();
                        hv_ClipRowEndB.Dispose();
                        hv_ClipColEndB.Dispose();
                        hv_ClipRowIntersect.Dispose();
                        hv_ClipColumnIntersect.Dispose();
                        hv_ClipIsOverlapping.Dispose();

                        return;
                    }

                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_SelectedRegion, out ho_Contours, "center");

                    hv_row1.Dispose();
                    HOperatorSet.RegionFeatures(ho_SelectedRegion, "row1", out hv_row1);
                    hv_column1.Dispose();
                    HOperatorSet.RegionFeatures(ho_SelectedRegion, "column1", out hv_column1);
                    hv_row2.Dispose();
                    HOperatorSet.RegionFeatures(ho_SelectedRegion, "row2", out hv_row2);
                    hv_column2.Dispose();
                    HOperatorSet.RegionFeatures(ho_SelectedRegion, "column2", out hv_column2);

                    hv_rect2_len2.Dispose();
                    HOperatorSet.RegionFeatures(ho_SelectedRegion, "rect2_len2", out hv_rect2_len2);

                    //封边宽
                    hv_EdgeWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_EdgeWidth = hv_rect2_len2 * 2;
                    }
                    //结果。
                    if (hv_Tuple_corner_rows == null)
                        hv_Tuple_corner_rows = new HTuple();
                    hv_Tuple_corner_rows[0] = hv_row2 - hv_EdgeWidth;
                    if (hv_Tuple_corner_rows == null)
                        hv_Tuple_corner_rows = new HTuple();
                    hv_Tuple_corner_rows[1] = hv_row2;
                    if (hv_Tuple_corner_rows == null)
                        hv_Tuple_corner_rows = new HTuple();
                    hv_Tuple_corner_rows[2] = hv_row1 + hv_EdgeWidth;
                    if (hv_Tuple_corner_rows == null)
                        hv_Tuple_corner_rows = new HTuple();
                    hv_Tuple_corner_rows[3] = hv_row1;

                    if (hv_Tuple_corner_cols == null)
                        hv_Tuple_corner_cols = new HTuple();
                    hv_Tuple_corner_cols[0] = hv_column2;
                    if (hv_Tuple_corner_cols == null)
                        hv_Tuple_corner_cols = new HTuple();
                    hv_Tuple_corner_cols[1] = hv_column2;
                    if (hv_Tuple_corner_cols == null)
                        hv_Tuple_corner_cols = new HTuple();
                    hv_Tuple_corner_cols[2] = hv_column1;
                    if (hv_Tuple_corner_cols == null)
                        hv_Tuple_corner_cols = new HTuple();
                    hv_Tuple_corner_cols[3] = hv_column1;

                    ho_Regionclosing.Dispose();
                    ho_Regionopening.Dispose();
                    ho_ConnectedRegions.Dispose();
                    ho_SelectedRegion.Dispose();
                    ho_Contours.Dispose();
                    ho_ContoursSplit.Dispose();
                    ho_SingleSegment.Dispose();
                    ho_tmpContours.Dispose();
                    ho_SingleContour.Dispose();
                    ho_SingleSegmentClip.Dispose();

                    hv_Anisometry.Dispose();
                    hv_Bulkiness.Dispose();
                    hv_StructureFactor.Dispose();
                    hv_Area.Dispose();
                    hv_Row.Dispose();
                    hv_Column.Dispose();
                    hv_row1.Dispose();
                    hv_column1.Dispose();
                    hv_row2.Dispose();
                    hv_column2.Dispose();
                    hv_rect2_len2.Dispose();
                    hv_EdgeWidth.Dispose();
                    hv_NumSegments.Dispose();
                    hv_NumLines.Dispose();
                    hv_RowsBegin.Dispose();
                    hv_ColsBegin.Dispose();
                    hv_RowsEnd.Dispose();
                    hv_ColsEnd.Dispose();
                    hv_i.Dispose();
                    hv_Attrib.Dispose();
                    hv_RowBegin.Dispose();
                    hv_ColBegin.Dispose();
                    hv_RowEnd.Dispose();
                    hv_ColEnd.Dispose();
                    hv_Nr.Dispose();
                    hv_Nc.Dispose();
                    hv_Dist.Dispose();
                    hv_Length.Dispose();
                    hv_Phi.Dispose();
                    hv_tmp.Dispose();
                    hv_Tuple_RowIntersect.Dispose();
                    hv_Tuple_ColumnIntersect.Dispose();
                    hv_RowIntersect.Dispose();
                    hv_ColumnIntersect.Dispose();
                    hv_IsOverlapping1.Dispose();
                    hv_IsOverlapping2.Dispose();
                    hv_IsOverlapping3.Dispose();
                    hv_IsOverlapping4.Dispose();
                    hv_Length1.Dispose();
                    hv_Length2.Dispose();
                    hv_Tuple_ClipRowIntersect.Dispose();
                    hv_Tuple_ClipColumnIntersect.Dispose();
                    hv_ClipRatio.Dispose();
                    hv_LineSegmentIndex.Dispose();
                    hv_ClipRowBeginA.Dispose();
                    hv_ClipColBeginA.Dispose();
                    hv_ClipRowEndA.Dispose();
                    hv_ClipColEndA.Dispose();
                    hv_ClipRowBeginB.Dispose();
                    hv_ClipColBeginB.Dispose();
                    hv_ClipRowEndB.Dispose();
                    hv_ClipColEndB.Dispose();
                    hv_ClipRowIntersect.Dispose();
                    hv_ClipColumnIntersect.Dispose();
                    hv_ClipIsOverlapping.Dispose();

                    return;



                }

                //
                //step: fit circles and lines into contours
                //

                hv_NumSegments.Dispose();
                HOperatorSet.CountObj(ho_ContoursSplit, out hv_NumSegments);

                hv_NumLines.Dispose();
                hv_NumLines = 0;
                hv_RowsBegin.Dispose();
                hv_RowsBegin = new HTuple();
                hv_ColsBegin.Dispose();
                hv_ColsBegin = new HTuple();
                hv_RowsEnd.Dispose();
                hv_RowsEnd = new HTuple();
                hv_ColsEnd.Dispose();
                hv_ColsEnd = new HTuple();
                HTuple end_val98 = hv_NumSegments;
                HTuple step_val98 = 1;
                for (hv_i = 1; hv_i.Continue(end_val98, step_val98); hv_i = hv_i.TupleAdd(step_val98))
                {
                    ho_SingleSegment.Dispose();
                    HOperatorSet.SelectObj(ho_ContoursSplit, out ho_SingleSegment, hv_i);
                    hv_Attrib.Dispose();
                    HOperatorSet.GetContourGlobalAttribXld(ho_SingleSegment, "cont_approx", out hv_Attrib);
                    if ((int)(new HTuple(hv_Attrib.TupleEqual(-1))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_NumLines = hv_NumLines + 1;
                                hv_NumLines.Dispose();
                                hv_NumLines = ExpTmpLocalVar_NumLines;
                            }
                        }
                        hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                        HOperatorSet.FitLineContourXld(ho_SingleSegment, "tukey", -1, 0, 5, 2,
                            out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr,
                            out hv_Nc, out hv_Dist);
                        hv_Length.Dispose();
                        HOperatorSet.DistancePp(hv_RowBegin, hv_ColBegin, hv_RowEnd, hv_ColEnd,
                            out hv_Length);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_RowsBegin = hv_RowsBegin.TupleConcat(
                                    hv_RowBegin);
                                hv_RowsBegin.Dispose();
                                hv_RowsBegin = ExpTmpLocalVar_RowsBegin;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_ColsBegin = hv_ColsBegin.TupleConcat(
                                    hv_ColBegin);
                                hv_ColsBegin.Dispose();
                                hv_ColsBegin = ExpTmpLocalVar_ColsBegin;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_RowsEnd = hv_RowsEnd.TupleConcat(
                                    hv_RowEnd);
                                hv_RowsEnd.Dispose();
                                hv_RowsEnd = ExpTmpLocalVar_RowsEnd;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_ColsEnd = hv_ColsEnd.TupleConcat(
                                    hv_ColEnd);
                                hv_ColsEnd.Dispose();
                                hv_ColsEnd = ExpTmpLocalVar_ColsEnd;
                            }
                        }
                    }
                }
                //
                //intersection points and angles calculated
                //by the lines obtained by line-fitting
                //
                if ((int)(new HTuple(hv_NumLines.TupleEqual(4))) != 0)
                {
                    hv_Phi.Dispose();
                    HOperatorSet.LineOrientation(hv_RowsBegin, hv_ColsBegin, hv_RowsEnd, hv_ColsEnd,
                        out hv_Phi);
                    //第一条线是水平的
                    if ((int)((new HTuple(((hv_Phi.TupleSelect(0))).TupleGreater((new HTuple(-45)).TupleRad()
                        ))).TupleAnd(new HTuple(((hv_Phi.TupleSelect(0))).TupleLess((new HTuple(45)).TupleRad()
                        )))) != 0)
                    {
                        //第一条线是垂直的
                    }
                    else
                    {
                        //调换线段顺序。0123->3012
                        hv_tmp.Dispose();
                        hv_tmp = new HTuple();
                        if (hv_tmp == null)
                            hv_tmp = new HTuple();
                        hv_tmp[0] = hv_RowsBegin.TupleSelect(3);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_tmp = hv_tmp.TupleConcat(
                                    hv_RowsBegin.TupleSelectRange(0, 2));
                                hv_tmp.Dispose();
                                hv_tmp = ExpTmpLocalVar_tmp;
                            }
                        }
                        hv_RowsBegin.Dispose();
                        hv_RowsBegin = new HTuple(hv_tmp);

                        hv_tmp.Dispose();
                        hv_tmp = new HTuple();
                        if (hv_tmp == null)
                            hv_tmp = new HTuple();
                        hv_tmp[0] = hv_RowsEnd.TupleSelect(3);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_tmp = hv_tmp.TupleConcat(
                                    hv_RowsEnd.TupleSelectRange(0, 2));
                                hv_tmp.Dispose();
                                hv_tmp = ExpTmpLocalVar_tmp;
                            }
                        }
                        hv_RowsEnd.Dispose();
                        hv_RowsEnd = new HTuple(hv_tmp);

                        hv_tmp.Dispose();
                        hv_tmp = new HTuple();
                        if (hv_tmp == null)
                            hv_tmp = new HTuple();
                        hv_tmp[0] = hv_ColsBegin.TupleSelect(3);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_tmp = hv_tmp.TupleConcat(
                                    hv_ColsBegin.TupleSelectRange(0, 2));
                                hv_tmp.Dispose();
                                hv_tmp = ExpTmpLocalVar_tmp;
                            }
                        }
                        hv_ColsBegin.Dispose();
                        hv_ColsBegin = new HTuple(hv_tmp);

                        hv_tmp.Dispose();
                        hv_tmp = new HTuple();
                        if (hv_tmp == null)
                            hv_tmp = new HTuple();
                        hv_tmp[0] = hv_ColsEnd.TupleSelect(3);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_tmp = hv_tmp.TupleConcat(
                                    hv_ColsEnd.TupleSelectRange(0, 2));
                                hv_tmp.Dispose();
                                hv_tmp = ExpTmpLocalVar_tmp;
                            }
                        }
                        hv_ColsEnd.Dispose();
                        hv_ColsEnd = new HTuple(hv_tmp);

                        ho_tmpContours.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_tmpContours);
                        ho_SingleContour.Dispose();
                        HOperatorSet.SelectObj(ho_ContoursSplit, out ho_SingleContour, 4);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_tmpContours, ho_SingleContour, out ExpTmpOutVar_0
                                );
                            ho_tmpContours.Dispose();
                            ho_tmpContours = ExpTmpOutVar_0;
                        }
                        ho_SingleContour.Dispose();
                        HOperatorSet.SelectObj(ho_ContoursSplit, out ho_SingleContour, 1);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_tmpContours, ho_SingleContour, out ExpTmpOutVar_0
                                );
                            ho_tmpContours.Dispose();
                            ho_tmpContours = ExpTmpOutVar_0;
                        }
                        ho_SingleContour.Dispose();
                        HOperatorSet.SelectObj(ho_ContoursSplit, out ho_SingleContour, 2);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_tmpContours, ho_SingleContour, out ExpTmpOutVar_0
                                );
                            ho_tmpContours.Dispose();
                            ho_tmpContours = ExpTmpOutVar_0;
                        }
                        ho_SingleContour.Dispose();
                        HOperatorSet.SelectObj(ho_ContoursSplit, out ho_SingleContour, 3);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_tmpContours, ho_SingleContour, out ExpTmpOutVar_0
                                );
                            ho_tmpContours.Dispose();
                            ho_tmpContours = ExpTmpOutVar_0;
                        }

                        ho_ContoursSplit.Dispose();
                        ho_ContoursSplit = new HObject(ho_tmpContours);
                    }

                    hv_Tuple_RowIntersect.Dispose();
                    hv_Tuple_RowIntersect = new HTuple();
                    hv_Tuple_ColumnIntersect.Dispose();
                    hv_Tuple_ColumnIntersect = new HTuple();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RowIntersect.Dispose(); hv_ColumnIntersect.Dispose(); hv_IsOverlapping1.Dispose();
                        HOperatorSet.IntersectionLines(hv_RowsBegin.TupleSelect(0), hv_ColsBegin.TupleSelect(
                            0), hv_RowsEnd.TupleSelect(0), hv_ColsEnd.TupleSelect(0), hv_RowsBegin.TupleSelect(
                            1), hv_ColsBegin.TupleSelect(1), hv_RowsEnd.TupleSelect(1), hv_ColsEnd.TupleSelect(
                            1), out hv_RowIntersect, out hv_ColumnIntersect, out hv_IsOverlapping1);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Tuple_RowIntersect = hv_Tuple_RowIntersect.TupleConcat(
                                hv_RowIntersect);
                            hv_Tuple_RowIntersect.Dispose();
                            hv_Tuple_RowIntersect = ExpTmpLocalVar_Tuple_RowIntersect;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Tuple_ColumnIntersect = hv_Tuple_ColumnIntersect.TupleConcat(
                                hv_ColumnIntersect);
                            hv_Tuple_ColumnIntersect.Dispose();
                            hv_Tuple_ColumnIntersect = ExpTmpLocalVar_Tuple_ColumnIntersect;
                        }
                    }
                    //dev_set_color ('red')
                    //gen_cross_contour_xld (CrossIntersection1, RowIntersect1, ColumnIntersect1, 200, 0.785398)

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RowIntersect.Dispose(); hv_ColumnIntersect.Dispose(); hv_IsOverlapping2.Dispose();
                        HOperatorSet.IntersectionLines(hv_RowsBegin.TupleSelect(1), hv_ColsBegin.TupleSelect(
                            1), hv_RowsEnd.TupleSelect(1), hv_ColsEnd.TupleSelect(1), hv_RowsBegin.TupleSelect(
                            2), hv_ColsBegin.TupleSelect(2), hv_RowsEnd.TupleSelect(2), hv_ColsEnd.TupleSelect(
                            2), out hv_RowIntersect, out hv_ColumnIntersect, out hv_IsOverlapping2);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Tuple_RowIntersect = hv_Tuple_RowIntersect.TupleConcat(
                                hv_RowIntersect);
                            hv_Tuple_RowIntersect.Dispose();
                            hv_Tuple_RowIntersect = ExpTmpLocalVar_Tuple_RowIntersect;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Tuple_ColumnIntersect = hv_Tuple_ColumnIntersect.TupleConcat(
                                hv_ColumnIntersect);
                            hv_Tuple_ColumnIntersect.Dispose();
                            hv_Tuple_ColumnIntersect = ExpTmpLocalVar_Tuple_ColumnIntersect;
                        }
                    }
                    //distance_pp (RowJunctions[0], ColJunctions[0], RowIntersect2, ColumnIntersect2, Distance2)
                    //dev_set_color ('green')
                    //gen_cross_contour_xld (CrossIntersection2, RowIntersect2, ColumnIntersect2, 200, 0.785398)

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RowIntersect.Dispose(); hv_ColumnIntersect.Dispose(); hv_IsOverlapping3.Dispose();
                        HOperatorSet.IntersectionLines(hv_RowsBegin.TupleSelect(2), hv_ColsBegin.TupleSelect(
                            2), hv_RowsEnd.TupleSelect(2), hv_ColsEnd.TupleSelect(2), hv_RowsBegin.TupleSelect(
                            3), hv_ColsBegin.TupleSelect(3), hv_RowsEnd.TupleSelect(3), hv_ColsEnd.TupleSelect(
                            3), out hv_RowIntersect, out hv_ColumnIntersect, out hv_IsOverlapping3);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Tuple_RowIntersect = hv_Tuple_RowIntersect.TupleConcat(
                                hv_RowIntersect);
                            hv_Tuple_RowIntersect.Dispose();
                            hv_Tuple_RowIntersect = ExpTmpLocalVar_Tuple_RowIntersect;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Tuple_ColumnIntersect = hv_Tuple_ColumnIntersect.TupleConcat(
                                hv_ColumnIntersect);
                            hv_Tuple_ColumnIntersect.Dispose();
                            hv_Tuple_ColumnIntersect = ExpTmpLocalVar_Tuple_ColumnIntersect;
                        }
                    }
                    //dev_set_color ('blue')
                    //gen_cross_contour_xld (CrossIntersection3, RowIntersect3, ColumnIntersect3, 200, 0.785398)

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RowIntersect.Dispose(); hv_ColumnIntersect.Dispose(); hv_IsOverlapping4.Dispose();
                        HOperatorSet.IntersectionLines(hv_RowsBegin.TupleSelect(3), hv_ColsBegin.TupleSelect(
                            3), hv_RowsEnd.TupleSelect(3), hv_ColsEnd.TupleSelect(3), hv_RowsBegin.TupleSelect(
                            0), hv_ColsBegin.TupleSelect(0), hv_RowsEnd.TupleSelect(0), hv_ColsEnd.TupleSelect(
                            0), out hv_RowIntersect, out hv_ColumnIntersect, out hv_IsOverlapping4);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Tuple_RowIntersect = hv_Tuple_RowIntersect.TupleConcat(
                                hv_RowIntersect);
                            hv_Tuple_RowIntersect.Dispose();
                            hv_Tuple_RowIntersect = ExpTmpLocalVar_Tuple_RowIntersect;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_Tuple_ColumnIntersect = hv_Tuple_ColumnIntersect.TupleConcat(
                                hv_ColumnIntersect);
                            hv_Tuple_ColumnIntersect.Dispose();
                            hv_Tuple_ColumnIntersect = ExpTmpLocalVar_Tuple_ColumnIntersect;
                        }
                    }
                    //dev_set_color ('yellow')
                    //gen_cross_contour_xld (CrossIntersection4, RowIntersect4, ColumnIntersect4, 200, 0.785398)


                    //结果。
                    //Tuple_corner_rows[0] := RowIntersect1
                    //Tuple_corner_rows[1] := RowIntersect2
                    //Tuple_corner_rows[2] := RowIntersect3
                    //Tuple_corner_rows[3] := RowIntersect4

                    //Tuple_corner_cols[0] := ColumnIntersect1
                    //Tuple_corner_cols[1] := ColumnIntersect2
                    //Tuple_corner_cols[2] := ColumnIntersect3
                    //Tuple_corner_cols[3] := ColumnIntersect4
                    if ((int)((new HTuple((new HTuple(hv_cameraPosition.TupleNotEqual(1))).TupleAnd(
                        new HTuple(hv_cameraPosition.TupleNotEqual(2))))).TupleAnd(new HTuple(hv_cameraPosition.TupleNotEqual(
                        7)))) != 0)
                    {
                        hv_Tuple_corner_rows.Dispose();
                        hv_Tuple_corner_rows = new HTuple(hv_Tuple_RowIntersect);
                        hv_Tuple_corner_cols.Dispose();
                        hv_Tuple_corner_cols = new HTuple(hv_Tuple_ColumnIntersect);

                        ho_Regionclosing.Dispose();
                        ho_Regionopening.Dispose();
                        ho_ConnectedRegions.Dispose();
                        ho_SelectedRegion.Dispose();
                        ho_Contours.Dispose();
                        ho_ContoursSplit.Dispose();
                        ho_SingleSegment.Dispose();
                        ho_tmpContours.Dispose();
                        ho_SingleContour.Dispose();
                        ho_SingleSegmentClip.Dispose();

                        hv_Anisometry.Dispose();
                        hv_Bulkiness.Dispose();
                        hv_StructureFactor.Dispose();
                        hv_Area.Dispose();
                        hv_Row.Dispose();
                        hv_Column.Dispose();
                        hv_row1.Dispose();
                        hv_column1.Dispose();
                        hv_row2.Dispose();
                        hv_column2.Dispose();
                        hv_rect2_len2.Dispose();
                        hv_EdgeWidth.Dispose();
                        hv_NumSegments.Dispose();
                        hv_NumLines.Dispose();
                        hv_RowsBegin.Dispose();
                        hv_ColsBegin.Dispose();
                        hv_RowsEnd.Dispose();
                        hv_ColsEnd.Dispose();
                        hv_i.Dispose();
                        hv_Attrib.Dispose();
                        hv_RowBegin.Dispose();
                        hv_ColBegin.Dispose();
                        hv_RowEnd.Dispose();
                        hv_ColEnd.Dispose();
                        hv_Nr.Dispose();
                        hv_Nc.Dispose();
                        hv_Dist.Dispose();
                        hv_Length.Dispose();
                        hv_Phi.Dispose();
                        hv_tmp.Dispose();
                        hv_Tuple_RowIntersect.Dispose();
                        hv_Tuple_ColumnIntersect.Dispose();
                        hv_RowIntersect.Dispose();
                        hv_ColumnIntersect.Dispose();
                        hv_IsOverlapping1.Dispose();
                        hv_IsOverlapping2.Dispose();
                        hv_IsOverlapping3.Dispose();
                        hv_IsOverlapping4.Dispose();
                        hv_Length1.Dispose();
                        hv_Length2.Dispose();
                        hv_Tuple_ClipRowIntersect.Dispose();
                        hv_Tuple_ClipColumnIntersect.Dispose();
                        hv_ClipRatio.Dispose();
                        hv_LineSegmentIndex.Dispose();
                        hv_ClipRowBeginA.Dispose();
                        hv_ClipColBeginA.Dispose();
                        hv_ClipRowEndA.Dispose();
                        hv_ClipColEndA.Dispose();
                        hv_ClipRowBeginB.Dispose();
                        hv_ClipColBeginB.Dispose();
                        hv_ClipRowEndB.Dispose();
                        hv_ClipColEndB.Dispose();
                        hv_ClipRowIntersect.Dispose();
                        hv_ClipColumnIntersect.Dispose();
                        hv_ClipIsOverlapping.Dispose();

                        return;
                    }

                    //对于上下相机图像，细化四边位置。
                    //由于机械不稳，板材四边不一定是直线，用更短的一段线段拟合直线，以期四角定位更准一些。
                    //求短边长（近似）。
                    hv_Length1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Length1 = (hv_Tuple_RowIntersect.TupleSelect(
                            1)) - (hv_Tuple_RowIntersect.TupleSelect(0));
                    }
                    hv_Length2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Length2 = (hv_Tuple_ColumnIntersect.TupleSelect(
                            1)) - (hv_Tuple_ColumnIntersect.TupleSelect(2));
                    }
                    if ((int)(new HTuple(hv_Length1.TupleLess(hv_Length2))) != 0)
                    {
                        hv_Length.Dispose();
                        hv_Length = new HTuple(hv_Length1);
                    }
                    else
                    {
                        hv_Length.Dispose();
                        hv_Length = new HTuple(hv_Length2);
                    }

                    hv_Tuple_ClipRowIntersect.Dispose();
                    hv_Tuple_ClipRowIntersect = new HTuple();
                    hv_Tuple_ClipColumnIntersect.Dispose();
                    hv_Tuple_ClipColumnIntersect = new HTuple();

                    //利用线段之间距离求板材四边长度。
                    //LineSegmentRowsBegin := []
                    //LineSegmentColsBegin := []
                    //LineSegmentRowsEnd := []
                    //LineSegmentColsEnd := []

                    hv_ClipRatio.Dispose();
                    hv_ClipRatio = 1.5;
                    hv_LineSegmentIndex.Dispose();
                    hv_LineSegmentIndex = 0;
                    for (hv_i = 0; (int)hv_i <= (int)((new HTuple(hv_Tuple_RowIntersect.TupleLength()
                        )) - 1); hv_i = (int)hv_i + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_SingleSegment.Dispose();
                            HOperatorSet.SelectObj(ho_ContoursSplit, out ho_SingleSegment, hv_i + 1);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_SingleSegmentClip.Dispose();
                            HOperatorSet.ClipContoursXld(ho_SingleSegment, out ho_SingleSegmentClip,
                                (hv_Tuple_RowIntersect.TupleSelect(hv_i)) - (hv_Length / hv_ClipRatio),
                                (hv_Tuple_ColumnIntersect.TupleSelect(hv_i)) - (hv_Length / hv_ClipRatio),
                                (hv_Tuple_RowIntersect.TupleSelect(hv_i)) + (hv_Length / hv_ClipRatio),
                                (hv_Tuple_ColumnIntersect.TupleSelect(hv_i)) + (hv_Length / hv_ClipRatio));
                        }
                        hv_ClipRowBeginA.Dispose(); hv_ClipColBeginA.Dispose(); hv_ClipRowEndA.Dispose(); hv_ClipColEndA.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                        HOperatorSet.FitLineContourXld(ho_SingleSegmentClip, "tukey", -1, 0, 5,
                            2, out hv_ClipRowBeginA, out hv_ClipColBeginA, out hv_ClipRowEndA,
                            out hv_ClipColEndA, out hv_Nr, out hv_Nc, out hv_Dist);
                        //LineSegmentRowsBegin := [LineSegmentRowsBegin,ClipRowBeginA]
                        //LineSegmentColsBegin := [LineSegmentColsBegin,ClipColBeginA]
                        //LineSegmentRowsEnd := [LineSegmentRowsEnd,ClipRowEndA]
                        //LineSegmentColsEnd := [LineSegmentColsEnd,ClipColEndA]

                        if ((int)(new HTuple(hv_i.TupleEqual(3))) != 0)
                        {
                            ho_SingleSegment.Dispose();
                            HOperatorSet.SelectObj(ho_ContoursSplit, out ho_SingleSegment, 1);
                        }
                        else
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_SingleSegment.Dispose();
                                HOperatorSet.SelectObj(ho_ContoursSplit, out ho_SingleSegment, hv_i + 2);
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_SingleSegmentClip.Dispose();
                            HOperatorSet.ClipContoursXld(ho_SingleSegment, out ho_SingleSegmentClip,
                                (hv_Tuple_RowIntersect.TupleSelect(hv_i)) - (hv_Length / hv_ClipRatio),
                                (hv_Tuple_ColumnIntersect.TupleSelect(hv_i)) - (hv_Length / hv_ClipRatio),
                                (hv_Tuple_RowIntersect.TupleSelect(hv_i)) + (hv_Length / hv_ClipRatio),
                                (hv_Tuple_ColumnIntersect.TupleSelect(hv_i)) + (hv_Length / hv_ClipRatio));
                        }
                        hv_ClipRowBeginB.Dispose(); hv_ClipColBeginB.Dispose(); hv_ClipRowEndB.Dispose(); hv_ClipColEndB.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                        HOperatorSet.FitLineContourXld(ho_SingleSegmentClip, "tukey", -1, 0, 5,
                            2, out hv_ClipRowBeginB, out hv_ClipColBeginB, out hv_ClipRowEndB,
                            out hv_ClipColEndB, out hv_Nr, out hv_Nc, out hv_Dist);
                        //LineSegmentRowsBegin := [LineSegmentRowsBegin,ClipRowBeginB]
                        //LineSegmentColsBegin := [LineSegmentColsBegin,ClipColBeginB]
                        //LineSegmentRowsEnd := [LineSegmentRowsEnd,ClipRowEndB]
                        //LineSegmentColsEnd := [LineSegmentColsEnd,ClipColEndB]

                        hv_ClipRowIntersect.Dispose(); hv_ClipColumnIntersect.Dispose(); hv_ClipIsOverlapping.Dispose();
                        HOperatorSet.IntersectionLines(hv_ClipRowBeginA, hv_ClipColBeginA, hv_ClipRowEndA,
                            hv_ClipColEndA, hv_ClipRowBeginB, hv_ClipColBeginB, hv_ClipRowEndB,
                            hv_ClipColEndB, out hv_ClipRowIntersect, out hv_ClipColumnIntersect,
                            out hv_ClipIsOverlapping);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Tuple_ClipRowIntersect = hv_Tuple_ClipRowIntersect.TupleConcat(
                                    hv_ClipRowIntersect);
                                hv_Tuple_ClipRowIntersect.Dispose();
                                hv_Tuple_ClipRowIntersect = ExpTmpLocalVar_Tuple_ClipRowIntersect;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Tuple_ClipColumnIntersect = hv_Tuple_ClipColumnIntersect.TupleConcat(
                                    hv_ClipColumnIntersect);
                                hv_Tuple_ClipColumnIntersect.Dispose();
                                hv_Tuple_ClipColumnIntersect = ExpTmpLocalVar_Tuple_ClipColumnIntersect;
                            }
                        }
                    }


                    //结果。
                    hv_Tuple_corner_rows.Dispose();
                    hv_Tuple_corner_rows = new HTuple(hv_Tuple_ClipRowIntersect);
                    hv_Tuple_corner_cols.Dispose();
                    hv_Tuple_corner_cols = new HTuple(hv_Tuple_ClipColumnIntersect);

                    //转换为世界坐标，返回物理距离。
                    //ImagePointToWorldPlane (CameraParameters, CameraPose, LineSegmentRowsBegin, LineSegmentColsBegin, LineSegmentBegin_WorldY, LineSegmentBegin_WorldX)
                    //ImagePointToWorldPlane (CameraParameters, CameraPose, LineSegmentRowsEnd, LineSegmentColsEnd, LineSegmentEnd_WorldY, LineSegmentEnd_WorldX)

                    //distance_ss (LineSegmentBegin_WorldY[1], LineSegmentBegin_WorldX[1], LineSegmentEnd_WorldY[1], LineSegmentEnd_WorldX[1], LineSegmentBegin_WorldY[6], LineSegmentBegin_WorldX[6], LineSegmentEnd_WorldY[6], LineSegmentEnd_WorldX[6], DistanceMin, DistanceMax)
                    //TopWidth := DistanceMin
                    //distance_ss (LineSegmentBegin_WorldY[2], LineSegmentBegin_WorldX[2], LineSegmentEnd_WorldY[2], LineSegmentEnd_WorldX[2], LineSegmentBegin_WorldY[5], LineSegmentBegin_WorldX[5], LineSegmentEnd_WorldY[5], LineSegmentEnd_WorldX[5], DistanceMin, DistanceMax)
                    //BottomWidth := DistanceMin
                    //distance_ss (LineSegmentBegin_WorldY[4], LineSegmentBegin_WorldX[4], LineSegmentEnd_WorldY[4], LineSegmentEnd_WorldX[4], LineSegmentBegin_WorldY[7], LineSegmentBegin_WorldX[7], LineSegmentEnd_WorldY[7], LineSegmentEnd_WorldX[7], DistanceMin, DistanceMax)
                    //LeftHeight := DistanceMin
                    //distance_ss (LineSegmentBegin_WorldY[0], LineSegmentBegin_WorldX[0], LineSegmentEnd_WorldY[0], LineSegmentEnd_WorldX[0], LineSegmentBegin_WorldY[3], LineSegmentBegin_WorldX[3], LineSegmentEnd_WorldY[3], LineSegmentEnd_WorldX[3], DistanceMin, DistanceMax)
                    //RightHeight := DistanceMin
                }
                else
                {
                    return;

                }
                ho_Regionclosing.Dispose();
                ho_Regionopening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegion.Dispose();
                ho_Contours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_SingleSegment.Dispose();
                ho_tmpContours.Dispose();
                ho_SingleContour.Dispose();
                ho_SingleSegmentClip.Dispose();

                hv_Anisometry.Dispose();
                hv_Bulkiness.Dispose();
                hv_StructureFactor.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_row1.Dispose();
                hv_column1.Dispose();
                hv_row2.Dispose();
                hv_column2.Dispose();
                hv_rect2_len2.Dispose();
                hv_EdgeWidth.Dispose();
                hv_NumSegments.Dispose();
                hv_NumLines.Dispose();
                hv_RowsBegin.Dispose();
                hv_ColsBegin.Dispose();
                hv_RowsEnd.Dispose();
                hv_ColsEnd.Dispose();
                hv_i.Dispose();
                hv_Attrib.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_Length.Dispose();
                hv_Phi.Dispose();
                hv_tmp.Dispose();
                hv_Tuple_RowIntersect.Dispose();
                hv_Tuple_ColumnIntersect.Dispose();
                hv_RowIntersect.Dispose();
                hv_ColumnIntersect.Dispose();
                hv_IsOverlapping1.Dispose();
                hv_IsOverlapping2.Dispose();
                hv_IsOverlapping3.Dispose();
                hv_IsOverlapping4.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Tuple_ClipRowIntersect.Dispose();
                hv_Tuple_ClipColumnIntersect.Dispose();
                hv_ClipRatio.Dispose();
                hv_LineSegmentIndex.Dispose();
                hv_ClipRowBeginA.Dispose();
                hv_ClipColBeginA.Dispose();
                hv_ClipRowEndA.Dispose();
                hv_ClipColEndA.Dispose();
                hv_ClipRowBeginB.Dispose();
                hv_ClipColBeginB.Dispose();
                hv_ClipRowEndB.Dispose();
                hv_ClipColEndB.Dispose();
                hv_ClipRowIntersect.Dispose();
                hv_ClipColumnIntersect.Dispose();
                hv_ClipIsOverlapping.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Regionclosing.Dispose();
                ho_Regionopening.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegion.Dispose();
                ho_Contours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_SingleSegment.Dispose();
                ho_tmpContours.Dispose();
                ho_SingleContour.Dispose();
                ho_SingleSegmentClip.Dispose();

                hv_Anisometry.Dispose();
                hv_Bulkiness.Dispose();
                hv_StructureFactor.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_row1.Dispose();
                hv_column1.Dispose();
                hv_row2.Dispose();
                hv_column2.Dispose();
                hv_rect2_len2.Dispose();
                hv_EdgeWidth.Dispose();
                hv_NumSegments.Dispose();
                hv_NumLines.Dispose();
                hv_RowsBegin.Dispose();
                hv_ColsBegin.Dispose();
                hv_RowsEnd.Dispose();
                hv_ColsEnd.Dispose();
                hv_i.Dispose();
                hv_Attrib.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_Length.Dispose();
                hv_Phi.Dispose();
                hv_tmp.Dispose();
                hv_Tuple_RowIntersect.Dispose();
                hv_Tuple_ColumnIntersect.Dispose();
                hv_RowIntersect.Dispose();
                hv_ColumnIntersect.Dispose();
                hv_IsOverlapping1.Dispose();
                hv_IsOverlapping2.Dispose();
                hv_IsOverlapping3.Dispose();
                hv_IsOverlapping4.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Tuple_ClipRowIntersect.Dispose();
                hv_Tuple_ClipColumnIntersect.Dispose();
                hv_ClipRatio.Dispose();
                hv_LineSegmentIndex.Dispose();
                hv_ClipRowBeginA.Dispose();
                hv_ClipColBeginA.Dispose();
                hv_ClipRowEndA.Dispose();
                hv_ClipColEndA.Dispose();
                hv_ClipRowBeginB.Dispose();
                hv_ClipColBeginB.Dispose();
                hv_ClipRowEndB.Dispose();
                hv_ClipColEndB.Dispose();
                hv_ClipRowIntersect.Dispose();
                hv_ClipColumnIntersect.Dispose();
                hv_ClipIsOverlapping.Dispose();

                throw HDevExpDefaultException;
            }
        }
        public Dictionary<string, dynamic> PostProcessParamNames { get; } = new Dictionary<string, dynamic> { { "Direction", "Over" } };
        HTuple hv_Tuple_bbox_class_id = new HTuple(), hv_Tuple_bbox_row1 = new HTuple(), hv_Tuple_bbox_col1 = new HTuple(), hv_Tuple_bbox_row2 = new HTuple(),
         hv_Tuple_bbox_col2 = new HTuple(), hv_Tuple_bbox_class_name = new HTuple(), hv_Tuple_bbox_confidence = new HTuple();

        public Dictionary<string, dynamic> ImagePointToWorldPlane(double Row, double Col)
        {
            //HTuple hv_CameraParameters = new HTuple(), hv_CameraPose = new HTuple();

            HTuple hv_CameraParameters = new HTuple(), hv_CameraPose = new HTuple();
            //['line_scan_division', 0.0561791, -10.4051, 3.5e-06, 3.5e-06, 8188.07, 156.671, 16384, 13401, 7.53418e-07, 6.3077e-05, -4.42947e-05]
            //[0.00792464, 0.37401, 0.997163, 325.048, 359.658, 352.984, 0]
            //Calibration 相机参数。
            //0816相机升高10mm
            //['line_scan_division', 0.0662223, -8.20652, 3.5e-06, 3.5e-06, 8204.43, 168.68, 16384, 14336, -1.04567e-07, 6.26166e-05, 4.46629e-05]
            //[0.0309349, 0.353714, 1.77338, 35.6431, 359.504, 180.369, 0]
            hv_CameraParameters.Dispose();
            hv_CameraParameters = new HTuple();
            hv_CameraParameters[0] = "line_scan_division";
            hv_CameraParameters[1] = 0.0662223;
            hv_CameraParameters[2] = -8.20652;
            hv_CameraParameters[3] = 3.5e-06;
            hv_CameraParameters[4] = 3.5e-06;
            hv_CameraParameters[5] = 8204.43;
            hv_CameraParameters[6] = 168.68;
            hv_CameraParameters[7] = 16384;
            hv_CameraParameters[8] = 14336;
            hv_CameraParameters[9] = -1.04567e-07;
            hv_CameraParameters[10] = 6.26166e-05;
            hv_CameraParameters[11] = 4.46629e-05;
            hv_CameraPose.Dispose();
            hv_CameraPose = new HTuple();
            hv_CameraPose[0] = 0.0309349;
            hv_CameraPose[1] = 0.353714;
            hv_CameraPose[2] = 1.77338;
            hv_CameraPose[3] = 35.6431;
            hv_CameraPose[4] = 359.504;
            hv_CameraPose[5] = 180.369;
            hv_CameraPose[6] = 0;
            // Initialize local and output iconic variables 
            HTuple hv_Y1 = new HTuple();
            HTuple hv_X1 = new HTuple();
            hv_X1.Dispose(); hv_Y1.Dispose();

            HTuple hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple();
            hv_Row1.Dispose(); hv_Column1.Dispose();
            hv_Row1 = Row;
            hv_Column1 = Col;

            HOperatorSet.ImagePointsToWorldPlane(hv_CameraParameters, hv_CameraPose, hv_Row1,
                hv_Column1, "m", out hv_X1, out hv_Y1);

            hv_CameraParameters.Dispose();
            hv_CameraPose.Dispose();

            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();

            results.Add("X", hv_X1[0].D);
            results.Add("Y", hv_Y1[0].D);
            return results;

        }
        public Dictionary<string, dynamic> PostProcess(Dictionary<string, dynamic> postProcessParamNames)
        {
            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
            // Local iconic variables 
            HObject ho_RegionTuple = null;

            HObject ho_RegionUnion = null, ho_GrooveRectangles = null;
            HObject ho_ExtendedGrooveRectangles = null, ho_HoleRectangle = null;
            HObject ho_ExtendedGrooveSelected = null, ho_RegionIntersection = null;
            HObject ho_GrooveSelected = null, ho_CurrentClassRectangles = null;
            HObject ho_CurrentClassRectanglesUnion = null, ho_CurrentClassRectanglesclosing = null;
            HObject ho_CurrentClassRectanglesUnionsComponents = null;
            HObject ho_CurrentClassRectanglesReduced = null, ho_RectangleSelected = null;
            HTuple hv_cameraPosition = new HTuple();
            HTuple hv_CameraParameters = new HTuple(), hv_CameraPose = new HTuple();
            HTuple hv_Tuple_bboxReduced_class_id = new HTuple();
            HTuple hv_Tuple_bboxReduced_row1 = new HTuple();
            HTuple hv_Tuple_bboxReduced_col1 = new HTuple();
            HTuple hv_Tuple_bboxReduced_row2 = new HTuple();
            HTuple hv_Tuple_bboxReduced_col2 = new HTuple();
            HTuple hv_Tuple_bboxReduced_class_name = new HTuple();
            HTuple hv_Tuple_corner_rows = new HTuple();
            HTuple hv_Tuple_corner_cols = new HTuple();
            HTuple hv_BoardThickness = new HTuple();
            // Local control variables 

            HObject ho_Image = null;
            HTuple hv_RowUnion1 = new HTuple(), hv_ColumnUnion1 = new HTuple();
            HTuple hv_RowUnion2 = new HTuple(), hv_ColumnUnion2 = new HTuple();

            HTuple hv_ClassIDs = new HTuple(), hv_ClassNames = new HTuple();
            HTuple hv_Length = new HTuple(), hv_HoleIndex = new HTuple();
            HTuple hv_GrooveIndex = new HTuple(), hv_Tuple_Groove_bbox_row1 = new HTuple();
            HTuple hv_Tuple_Groove_bbox_col1 = new HTuple(), hv_Tuple_Groove_bbox_row2 = new HTuple();
            HTuple hv_Tuple_Groove_bbox_col2 = new HTuple(), hv_Tuple_ExtendedGroove_bbox_row1 = new HTuple();
            HTuple hv_Tuple_ExtendedGroove_bbox_col1 = new HTuple();
            HTuple hv_Tuple_ExtendedGroove_bbox_row2 = new HTuple();
            HTuple hv_Tuple_ExtendedGroove_bbox_col2 = new HTuple();
            HTuple hv_Index = new HTuple(), hv_GrooveHeight = new HTuple();
            HTuple hv_GrooveWidth = new HTuple(), hv_ExtendDistance = new HTuple();
            HTuple hv_GrooveNum = new HTuple(), hv_Index2 = new HTuple();
            HTuple hv_AreaIntersection = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_AreaHole = new HTuple();
            HTuple hv_X1 = new HTuple();
            HTuple hv_Y1 = new HTuple(), hv_X2 = new HTuple(), hv_Y2 = new HTuple();
            HTuple hv_Ra = new HTuple(), hv_Rb = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_HoleWidth = new HTuple(), hv_IndexClass = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_row1 = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_col1 = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_row2 = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_col2 = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_class_id = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_class_name = new HTuple();
            HTuple hv_Tuple_CurrentClass_bbox_confidence = new HTuple();
            HTuple hv_Index4 = new HTuple(), hv_RectNum = new HTuple();
            HTuple hv_Index5 = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Tuple_Thickness = new HTuple();
            HTuple hv_EndAll = new HTuple(), hv_Tuple_corner_WorldX = new HTuple();
            HTuple hv_Tuple_corner_WorldY = new HTuple(), hv_Tuple_bboxReduced_WorldX1 = new HTuple();
            HTuple hv_Tuple_bboxReduced_WorldY1 = new HTuple(), hv_Tuple_bboxReduced_WorldX2 = new HTuple();
            HTuple hv_Tuple_bboxReduced_WorldY2 = new HTuple();
            HTuple hv_Tuple_bbox_class_id_COPY_INP_TMP = new HTuple(hv_Tuple_bbox_class_id);
            HTuple hv_Tuple_bbox_class_name_COPY_INP_TMP = new HTuple(hv_Tuple_bbox_class_name);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_RegionTuple);
            HOperatorSet.GenEmptyObj(out ho_Image);

            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_GrooveRectangles);
            HOperatorSet.GenEmptyObj(out ho_ExtendedGrooveRectangles);
            HOperatorSet.GenEmptyObj(out ho_HoleRectangle);
            HOperatorSet.GenEmptyObj(out ho_ExtendedGrooveSelected);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_GrooveSelected);
            HOperatorSet.GenEmptyObj(out ho_CurrentClassRectangles);
            HOperatorSet.GenEmptyObj(out ho_CurrentClassRectanglesUnion);
            HOperatorSet.GenEmptyObj(out ho_CurrentClassRectanglesclosing);
            HOperatorSet.GenEmptyObj(out ho_CurrentClassRectanglesUnionsComponents);
            HOperatorSet.GenEmptyObj(out ho_CurrentClassRectanglesReduced);
            HOperatorSet.GenEmptyObj(out ho_RectangleSelected);
            List<TargetResult> ListDetectResult = new List<TargetResult>();
            try
            {
                //后处理
                //Chip,Groove,Hole,Pit,Scratch
                //**********************************Chip********************************************************************
                hv_cameraPosition.Dispose();
                if (postProcessParamNames["Direction"] == "Over")
                {
                    hv_cameraPosition = 1;
                }
                else if (postProcessParamNames["Direction"] == "LowerA")
                {
                    hv_cameraPosition = 2;
                }
                else if (postProcessParamNames["Direction"] == "SideFree")
                {
                    hv_cameraPosition = 3;
                }
                else if (postProcessParamNames["Direction"] == "SideFixed")
                {
                    hv_cameraPosition = 4;
                }
                else if (postProcessParamNames["Direction"] == "Front")
                {
                    hv_cameraPosition = 5;
                }
                else if (postProcessParamNames["Direction"] == "Back")
                {
                    hv_cameraPosition = 6;
                }
                else if (postProcessParamNames["Direction"] == "LowerB")
                {
                    hv_cameraPosition = 7;
                }
                //Calibration 相机参数。
                //['line_scan_division', 0.0561791, -10.4051, 3.5e-06, 3.5e-06, 8188.07, 156.671, 16384, 13401, 7.53418e-07, 6.3077e-05, -4.42947e-05]
                //[0.00792464, 0.37401, 0.997163, 325.048, 359.658, 352.984, 0]
                //['line_scan_division', 0.0662223, -8.20652, 3.5e-06, 3.5e-06, 8204.43, 168.68, 16384, 14336, -1.04567e-07, 6.26166e-05, 4.46629e-05]
                //[0.0309349, 0.353714, 1.77338, 35.6431, 359.504, 180.369, 0]
                //hv_CameraParameters.Dispose();
                //hv_CameraParameters = new HTuple();
                //hv_CameraParameters[0] = "line_scan_division";
                //hv_CameraParameters[1] = 0.0561791;
                //hv_CameraParameters[2] = -10.4051;
                //hv_CameraParameters[3] = 3.5e-06;
                //hv_CameraParameters[4] = 3.5e-06;
                //hv_CameraParameters[5] = 8188.07;
                //hv_CameraParameters[6] = 156.671;
                //hv_CameraParameters[7] = 16384;
                //hv_CameraParameters[8] = 13401;
                //hv_CameraParameters[9] = 7.53418e-07;
                //hv_CameraParameters[10] = 6.3077e-05;
                //hv_CameraParameters[11] = -4.42947e-05;
                //hv_CameraPose.Dispose();
                //hv_CameraPose = new HTuple();
                //hv_CameraPose[0] = 0.00792464;
                //hv_CameraPose[1] = 0.37401;
                //hv_CameraPose[2] = 0.997163;
                //hv_CameraPose[3] = 325.048;
                //hv_CameraPose[4] = 359.658;
                //hv_CameraPose[5] = 352.984;
                //hv_CameraPose[6] = 0;
                //Tuple_bboxReduced_confidence := []

                //HOperatorSet.ReadImage(out ho_Image, @"D:\ZBoom\20230804\TestAlgorithm-0\Y0310261512722\LinearCamera_Over_20230804112904.jpg");               

                if ((int)((new HTuple((new HTuple(hv_cameraPosition.TupleEqual(1))).TupleOr(
                     new HTuple(hv_cameraPosition.TupleEqual(2))))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                     7)))) != 0)
                {
                    hv_ClassIDs.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);
                    hv_ClassNames.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);

                    //检测板材四角。
                    ho_RegionTuple.Dispose();
                    HOperatorSet.GenRegionContourXld(ho_ContoursTuple, out ho_RegionTuple, "filled");

                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_RegionTuple, out ho_RegionUnion);

                    hv_RowUnion1.Dispose(); hv_ColumnUnion1.Dispose(); hv_RowUnion2.Dispose(); hv_ColumnUnion2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_RegionUnion, out hv_RowUnion1, out hv_ColumnUnion1,
                        out hv_RowUnion2, out hv_ColumnUnion2);

                    hv_Tuple_corner_rows.Dispose(); hv_Tuple_corner_cols.Dispose();
                    FindRegionCorner(ho_RegionUnion, hv_cameraPosition, out hv_Tuple_corner_rows,
                        out hv_Tuple_corner_cols);
                }
                else if (new HTuple(hv_cameraPosition.TupleEqual(3)).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(4))) != 0)
                {
                    hv_ClassIDs.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);
                    hv_ClassNames.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);

                }
                else if (new HTuple(hv_cameraPosition.TupleEqual(5)).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(6))) != 0)
                {
                    hv_ClassIDs.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);
                    hv_ClassNames.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
                }

                //由于切割小图进行检测，如果槽的两头没到板材边，并且被切割在单独的小图里，那这部分特征和孔没有区别，有可能被识别为孔，
                //需要把这类检测结果改为槽，并与相邻的槽合并。
                //具体方法：将所有检测到的槽沿长轴方向延长一定距离ExtendDistance，如果和某个孔有相交，
                //并且孔和扩展后的槽相交面积大于阈值，孔区域宽和槽宽差不多，则修改该孔标签为槽。
                //首先，确定检测结果中同时有槽和孔。
                hv_Length.Dispose();
                HOperatorSet.TupleLength(hv_Tuple_bbox_class_name_COPY_INP_TMP, out hv_Length);
                hv_HoleIndex.Dispose();
                HOperatorSet.TupleFindFirst(hv_Tuple_bbox_class_name_COPY_INP_TMP, "Hole",
                    out hv_HoleIndex);
                hv_GrooveIndex.Dispose();
                HOperatorSet.TupleFindFirst(hv_Tuple_bbox_class_name_COPY_INP_TMP, "Groove",
                    out hv_GrooveIndex);
                if (new HTuple(new HTuple(hv_Length.TupleNotEqual(0)).TupleAnd(new HTuple(hv_HoleIndex.TupleNotEqual(-1)))).TupleAnd(new HTuple(hv_GrooveIndex.TupleNotEqual(-1))) != 0)
                {
                    //槽区域
                    hv_Tuple_Groove_bbox_row1.Dispose();
                    hv_Tuple_Groove_bbox_row1 = new HTuple();
                    hv_Tuple_Groove_bbox_col1.Dispose();
                    hv_Tuple_Groove_bbox_col1 = new HTuple();
                    hv_Tuple_Groove_bbox_row2.Dispose();
                    hv_Tuple_Groove_bbox_row2 = new HTuple();
                    hv_Tuple_Groove_bbox_col2.Dispose();
                    hv_Tuple_Groove_bbox_col2 = new HTuple();
                    //延长后的槽区域
                    hv_Tuple_ExtendedGroove_bbox_row1.Dispose();
                    hv_Tuple_ExtendedGroove_bbox_row1 = new HTuple();
                    hv_Tuple_ExtendedGroove_bbox_col1.Dispose();
                    hv_Tuple_ExtendedGroove_bbox_col1 = new HTuple();
                    hv_Tuple_ExtendedGroove_bbox_row2.Dispose();
                    hv_Tuple_ExtendedGroove_bbox_row2 = new HTuple();
                    hv_Tuple_ExtendedGroove_bbox_col2.Dispose();
                    hv_Tuple_ExtendedGroove_bbox_col2 = new HTuple();
                    for (hv_Index = 0; (int)hv_Index <= (int)(new HTuple(hv_Tuple_bbox_row1.TupleLength()
                        ) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        if (new HTuple(hv_Tuple_bbox_class_name_COPY_INP_TMP.TupleSelect(
                            hv_Index).TupleEqual("Groove")) != 0)
                        {
                            hv_GrooveHeight.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_GrooveHeight = hv_Tuple_bbox_row2.TupleSelect(
                                    hv_Index) - hv_Tuple_bbox_row1.TupleSelect(hv_Index);
                            }
                            hv_GrooveWidth.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_GrooveWidth = hv_Tuple_bbox_col2.TupleSelect(
                                    hv_Index) - hv_Tuple_bbox_col1.TupleSelect(hv_Index);
                            }
                            if (new HTuple(hv_GrooveHeight.TupleGreater(hv_GrooveWidth)) != 0)
                            {
                                hv_ExtendDistance.Dispose();
                                hv_ExtendDistance = new HTuple(hv_GrooveWidth);
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_row1 = hv_Tuple_ExtendedGroove_bbox_row1.TupleConcat(
                                            hv_Tuple_bbox_row1.TupleSelect(hv_Index) - hv_ExtendDistance);
                                        hv_Tuple_ExtendedGroove_bbox_row1.Dispose();
                                        hv_Tuple_ExtendedGroove_bbox_row1 = ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_row1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_col1 = hv_Tuple_ExtendedGroove_bbox_col1.TupleConcat(
                                            hv_Tuple_bbox_col1.TupleSelect(hv_Index));
                                        hv_Tuple_ExtendedGroove_bbox_col1.Dispose();
                                        hv_Tuple_ExtendedGroove_bbox_col1 = ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_col1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_row2 = hv_Tuple_ExtendedGroove_bbox_row2.TupleConcat(
                                            hv_Tuple_bbox_row2.TupleSelect(hv_Index) + hv_ExtendDistance);
                                        hv_Tuple_ExtendedGroove_bbox_row2.Dispose();
                                        hv_Tuple_ExtendedGroove_bbox_row2 = ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_row2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_col2 = hv_Tuple_ExtendedGroove_bbox_col2.TupleConcat(
                                            hv_Tuple_bbox_col2.TupleSelect(hv_Index));
                                        hv_Tuple_ExtendedGroove_bbox_col2.Dispose();
                                        hv_Tuple_ExtendedGroove_bbox_col2 = ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_col2;
                                    }
                                }
                            }
                            else
                            {
                                hv_ExtendDistance.Dispose();
                                hv_ExtendDistance = new HTuple(hv_GrooveHeight);
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_row1 = hv_Tuple_ExtendedGroove_bbox_row1.TupleConcat(
                                            hv_Tuple_bbox_row1.TupleSelect(hv_Index));
                                        hv_Tuple_ExtendedGroove_bbox_row1.Dispose();
                                        hv_Tuple_ExtendedGroove_bbox_row1 = ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_row1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_col1 = hv_Tuple_ExtendedGroove_bbox_col1.TupleConcat(
                                            hv_Tuple_bbox_col1.TupleSelect(hv_Index) - hv_ExtendDistance);
                                        hv_Tuple_ExtendedGroove_bbox_col1.Dispose();
                                        hv_Tuple_ExtendedGroove_bbox_col1 = ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_col1;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_row2 = hv_Tuple_ExtendedGroove_bbox_row2.TupleConcat(
                                            hv_Tuple_bbox_row2.TupleSelect(hv_Index));
                                        hv_Tuple_ExtendedGroove_bbox_row2.Dispose();
                                        hv_Tuple_ExtendedGroove_bbox_row2 = ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_row2;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_col2 = hv_Tuple_ExtendedGroove_bbox_col2.TupleConcat(
                                            hv_Tuple_bbox_col2.TupleSelect(hv_Index) + hv_ExtendDistance);
                                        hv_Tuple_ExtendedGroove_bbox_col2.Dispose();
                                        hv_Tuple_ExtendedGroove_bbox_col2 = ExpTmpLocalVar_Tuple_ExtendedGroove_bbox_col2;
                                    }
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_Groove_bbox_row1 = hv_Tuple_Groove_bbox_row1.TupleConcat(
                                        hv_Tuple_bbox_row1.TupleSelect(hv_Index));
                                    hv_Tuple_Groove_bbox_row1.Dispose();
                                    hv_Tuple_Groove_bbox_row1 = ExpTmpLocalVar_Tuple_Groove_bbox_row1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_Groove_bbox_col1 = hv_Tuple_Groove_bbox_col1.TupleConcat(
                                        hv_Tuple_bbox_col1.TupleSelect(hv_Index));
                                    hv_Tuple_Groove_bbox_col1.Dispose();
                                    hv_Tuple_Groove_bbox_col1 = ExpTmpLocalVar_Tuple_Groove_bbox_col1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_Groove_bbox_row2 = hv_Tuple_Groove_bbox_row2.TupleConcat(
                                        hv_Tuple_bbox_row2.TupleSelect(hv_Index));
                                    hv_Tuple_Groove_bbox_row2.Dispose();
                                    hv_Tuple_Groove_bbox_row2 = ExpTmpLocalVar_Tuple_Groove_bbox_row2;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_Groove_bbox_col2 = hv_Tuple_Groove_bbox_col2.TupleConcat(
                                        hv_Tuple_bbox_col2.TupleSelect(hv_Index));
                                    hv_Tuple_Groove_bbox_col2.Dispose();
                                    hv_Tuple_Groove_bbox_col2 = ExpTmpLocalVar_Tuple_Groove_bbox_col2;
                                }
                            }
                        }
                    }
                    ho_GrooveRectangles.Dispose();
                    HOperatorSet.GenRectangle1(out ho_GrooveRectangles, hv_Tuple_Groove_bbox_row1,
                        hv_Tuple_Groove_bbox_col1, hv_Tuple_Groove_bbox_row2, hv_Tuple_Groove_bbox_col2);
                    ho_ExtendedGrooveRectangles.Dispose();
                    HOperatorSet.GenRectangle1(out ho_ExtendedGrooveRectangles, hv_Tuple_ExtendedGroove_bbox_row1,
                        hv_Tuple_ExtendedGroove_bbox_col1, hv_Tuple_ExtendedGroove_bbox_row2,
                        hv_Tuple_ExtendedGroove_bbox_col2);
                    for (hv_Index = 0; (int)hv_Index <= (int)(new HTuple(hv_Tuple_bbox_row1.TupleLength()
                        ) - 1); hv_Index = (int)hv_Index + 1)
                    {
                        if (new HTuple(hv_Tuple_bbox_class_name_COPY_INP_TMP.TupleSelect(
                            hv_Index).TupleEqual("Hole")) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_HoleRectangle.Dispose();
                                HOperatorSet.GenRectangle1(out ho_HoleRectangle, hv_Tuple_bbox_row1.TupleSelect(
                                    hv_Index), hv_Tuple_bbox_col1.TupleSelect(hv_Index), hv_Tuple_bbox_row2.TupleSelect(
                                    hv_Index), hv_Tuple_bbox_col2.TupleSelect(hv_Index));
                            }

                            hv_GrooveNum.Dispose();
                            HOperatorSet.CountObj(ho_ExtendedGrooveRectangles, out hv_GrooveNum);

                            HTuple end_val80 = hv_GrooveNum;
                            HTuple step_val80 = 1;
                            for (hv_Index2 = 1; hv_Index2.Continue(end_val80, step_val80); hv_Index2 = hv_Index2.TupleAdd(step_val80))
                            {
                                ho_ExtendedGrooveSelected.Dispose();
                                HOperatorSet.SelectObj(ho_ExtendedGrooveRectangles, out ho_ExtendedGrooveSelected,
                                    hv_Index2);
                                ho_RegionIntersection.Dispose();
                                HOperatorSet.Intersection(ho_HoleRectangle, ho_ExtendedGrooveSelected,
                                    out ho_RegionIntersection);
                                hv_AreaIntersection.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                                HOperatorSet.AreaCenter(ho_RegionIntersection, out hv_AreaIntersection,
                                    out hv_Row, out hv_Column);
                                hv_AreaHole.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                                HOperatorSet.AreaCenter(ho_HoleRectangle, out hv_AreaHole, out hv_Row,
                                    out hv_Column);
                                //孔和扩展槽相交
                                if (new HTuple(hv_AreaIntersection.TupleGreater(hv_AreaHole * 0.5)) != 0)
                                {
                                    ho_GrooveSelected.Dispose();
                                    HOperatorSet.SelectObj(ho_GrooveRectangles, out ho_GrooveSelected,
                                        hv_Index2);
                                    hv_Ra.Dispose(); hv_Rb.Dispose(); hv_Phi.Dispose();
                                    HOperatorSet.EllipticAxis(ho_GrooveSelected, out hv_Ra, out hv_Rb,
                                        out hv_Phi);
                                    if ((int)(new HTuple(hv_Phi.TupleEqual(0))) != 0)
                                    {
                                        hv_GrooveWidth.Dispose();
                                        HOperatorSet.RegionFeatures(ho_GrooveSelected, "height", out hv_GrooveWidth);
                                        hv_HoleWidth.Dispose();
                                        HOperatorSet.RegionFeatures(ho_HoleRectangle, "height", out hv_HoleWidth);
                                    }
                                    else
                                    {
                                        hv_GrooveWidth.Dispose();
                                        HOperatorSet.RegionFeatures(ho_GrooveSelected, "width", out hv_GrooveWidth);
                                        hv_HoleWidth.Dispose();
                                        HOperatorSet.RegionFeatures(ho_HoleRectangle, "width", out hv_HoleWidth);
                                    }
                                    //孔区域宽和槽宽差不多
                                    if ((int)((new HTuple(((hv_GrooveWidth / hv_HoleWidth)).TupleLess(1.5))).TupleAnd(
                                        new HTuple(((hv_GrooveWidth / hv_HoleWidth)).TupleGreater(0.67)))) != 0)
                                    {
                                        if (hv_Tuple_bbox_class_name_COPY_INP_TMP == null)
                                            hv_Tuple_bbox_class_name_COPY_INP_TMP = new HTuple();
                                        hv_Tuple_bbox_class_name_COPY_INP_TMP[hv_Index] = "Groove";
                                        hv_GrooveIndex.Dispose();
                                        HOperatorSet.TupleFindFirst(hv_ClassNames, "Groove", out hv_GrooveIndex);
                                        if (hv_Tuple_bbox_class_id_COPY_INP_TMP == null)
                                            hv_Tuple_bbox_class_id_COPY_INP_TMP = new HTuple();
                                        hv_Tuple_bbox_class_id_COPY_INP_TMP[hv_Index] = hv_GrooveIndex;
                                    }
                                }
                            }
                        }
                    }
                }



                for (hv_IndexClass = 0; (int)hv_IndexClass <= (int)((new HTuple(hv_ClassIDs.TupleLength()
                    )) - 1); hv_IndexClass = (int)hv_IndexClass + 1)
                {
                    hv_Tuple_CurrentClass_bbox_row1.Dispose();
                    hv_Tuple_CurrentClass_bbox_row1 = new HTuple();
                    hv_Tuple_CurrentClass_bbox_col1.Dispose();
                    hv_Tuple_CurrentClass_bbox_col1 = new HTuple();
                    hv_Tuple_CurrentClass_bbox_row2.Dispose();
                    hv_Tuple_CurrentClass_bbox_row2 = new HTuple();
                    hv_Tuple_CurrentClass_bbox_col2.Dispose();
                    hv_Tuple_CurrentClass_bbox_col2 = new HTuple();
                    hv_Tuple_CurrentClass_bbox_class_id.Dispose();
                    hv_Tuple_CurrentClass_bbox_class_id = new HTuple();
                    hv_Tuple_CurrentClass_bbox_class_name.Dispose();
                    hv_Tuple_CurrentClass_bbox_class_name = new HTuple();
                    hv_Tuple_CurrentClass_bbox_confidence.Dispose();
                    hv_Tuple_CurrentClass_bbox_confidence = new HTuple();
                    for (hv_Index4 = 0; (int)hv_Index4 <= (int)((new HTuple(hv_Tuple_bbox_row1.TupleLength()
                        )) - 1); hv_Index4 = (int)hv_Index4 + 1)
                    {
                        if ((int)new HTuple(hv_Tuple_bbox_class_id_COPY_INP_TMP.TupleSelect(
                            hv_Index4).TupleEqual(hv_IndexClass)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_row1 = hv_Tuple_CurrentClass_bbox_row1.TupleConcat(
                                        hv_Tuple_bbox_row1.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_row1.Dispose();
                                    hv_Tuple_CurrentClass_bbox_row1 = ExpTmpLocalVar_Tuple_CurrentClass_bbox_row1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_col1 = hv_Tuple_CurrentClass_bbox_col1.TupleConcat(
                                        hv_Tuple_bbox_col1.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_col1.Dispose();
                                    hv_Tuple_CurrentClass_bbox_col1 = ExpTmpLocalVar_Tuple_CurrentClass_bbox_col1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_row2 = hv_Tuple_CurrentClass_bbox_row2.TupleConcat(
                                        hv_Tuple_bbox_row2.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_row2.Dispose();
                                    hv_Tuple_CurrentClass_bbox_row2 = ExpTmpLocalVar_Tuple_CurrentClass_bbox_row2;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_col2 = hv_Tuple_CurrentClass_bbox_col2.TupleConcat(
                                        hv_Tuple_bbox_col2.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_col2.Dispose();
                                    hv_Tuple_CurrentClass_bbox_col2 = ExpTmpLocalVar_Tuple_CurrentClass_bbox_col2;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_class_id = hv_Tuple_CurrentClass_bbox_class_id.TupleConcat(
                                        hv_Tuple_bbox_class_id_COPY_INP_TMP.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_class_id.Dispose();
                                    hv_Tuple_CurrentClass_bbox_class_id = ExpTmpLocalVar_Tuple_CurrentClass_bbox_class_id;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_class_name = hv_Tuple_CurrentClass_bbox_class_name.TupleConcat(
                                        hv_Tuple_bbox_class_name_COPY_INP_TMP.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_class_name.Dispose();
                                    hv_Tuple_CurrentClass_bbox_class_name = ExpTmpLocalVar_Tuple_CurrentClass_bbox_class_name;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_CurrentClass_bbox_confidence = hv_Tuple_CurrentClass_bbox_confidence.TupleConcat(
                                        hv_Tuple_bbox_confidence.TupleSelect(hv_Index4));
                                    hv_Tuple_CurrentClass_bbox_confidence.Dispose();
                                    hv_Tuple_CurrentClass_bbox_confidence = ExpTmpLocalVar_Tuple_CurrentClass_bbox_confidence;
                                }
                            }
                        }
                    }
                    if ((int)(new HTuple((new HTuple(hv_Tuple_CurrentClass_bbox_row1.TupleLength()
                        )).TupleGreater(0))) != 0)
                    {
                        ho_CurrentClassRectangles.Dispose();
                        HOperatorSet.GenRectangle1(out ho_CurrentClassRectangles, hv_Tuple_CurrentClass_bbox_row1,
                            hv_Tuple_CurrentClass_bbox_col1, hv_Tuple_CurrentClass_bbox_row2, hv_Tuple_CurrentClass_bbox_col2);
                        ho_CurrentClassRectanglesUnion.Dispose();
                        HOperatorSet.Union1(ho_CurrentClassRectangles, out ho_CurrentClassRectanglesUnion
                            );

                        if (new HTuple(((hv_ClassNames.TupleSelect(hv_IndexClass))).TupleEqual(
                            "Hole")) != 0)
                        {
                            //对于孔，防止临近的小孔连在一起。
                            ho_CurrentClassRectanglesclosing.Dispose();
                            HOperatorSet.ClosingCircle(ho_CurrentClassRectanglesUnion, out ho_CurrentClassRectanglesclosing,
                                5);
                        }
                        else
                        {
                            //对于槽，封边，尽量将检测出来了，但是不完全连接的区域连接起来。
                            if ((int)((new HTuple(hv_cameraPosition.TupleEqual(5))).TupleOr(new HTuple(hv_cameraPosition.TupleEqual(
                                6)))) != 0)
                            {
                                ho_CurrentClassRectanglesclosing.Dispose();
                                HOperatorSet.ClosingCircle(ho_CurrentClassRectanglesUnion, out ho_CurrentClassRectanglesclosing,
                                    20);
                            }
                            else
                            {
                                ho_CurrentClassRectanglesclosing.Dispose();
                                HOperatorSet.ClosingCircle(ho_CurrentClassRectanglesUnion, out ho_CurrentClassRectanglesclosing,
                                    50);
                            }
                        }


                        ho_CurrentClassRectanglesUnionsComponents.Dispose();
                        HOperatorSet.Connection(ho_CurrentClassRectanglesclosing, out ho_CurrentClassRectanglesUnionsComponents
                            );
                        ho_CurrentClassRectanglesReduced.Dispose();
                        HOperatorSet.ShapeTrans(ho_CurrentClassRectanglesUnionsComponents, out ho_CurrentClassRectanglesReduced,
                            "rectangle2");

                        hv_RectNum.Dispose();
                        HOperatorSet.CountObj(ho_CurrentClassRectanglesReduced, out hv_RectNum);

                        HTuple end_val151 = hv_RectNum;
                        HTuple step_val151 = 1;
                        for (hv_Index5 = 1; hv_Index5.Continue(end_val151, step_val151); hv_Index5 = hv_Index5.TupleAdd(step_val151))
                        {
                            ho_RectangleSelected.Dispose();
                            HOperatorSet.SelectObj(ho_CurrentClassRectanglesReduced, out ho_RectangleSelected,
                                hv_Index5);
                            hv_Row1.Dispose(); hv_Column1.Dispose(); hv_Row2.Dispose(); hv_Column2.Dispose();
                            HOperatorSet.SmallestRectangle1(ho_RectangleSelected, out hv_Row1, out hv_Column1,
                                out hv_Row2, out hv_Column2);

                            //转换图像坐标为世界坐标
                            //if ((int)(new HTuple(hv_cameraPosition.TupleEqual(1))) != 0)
                            //{
                            //    hv_X1.Dispose(); hv_Y1.Dispose();
                            //    HOperatorSet.ImagePointsToWorldPlane(hv_CameraParameters, hv_CameraPose,
                            //        hv_Row1, hv_Column1, "m", out hv_X1, out hv_Y1);
                            //    hv_Row1.Dispose();
                            //    hv_Row1 = new HTuple(hv_Y1);
                            //    hv_Column1.Dispose();
                            //    hv_Column1 = new HTuple(hv_X1);
                            //    hv_X2.Dispose(); hv_Y2.Dispose();
                            //    HOperatorSet.ImagePointsToWorldPlane(hv_CameraParameters, hv_CameraPose,
                            //        hv_Row2, hv_Column2, "m", out hv_X2, out hv_Y2);
                            //    hv_Row2.Dispose();
                            //    hv_Row2 = new HTuple(hv_Y2);
                            //    hv_Column2.Dispose();
                            //    hv_Column2 = new HTuple(hv_X2);
                            //}
                            //以上注销，直接返回图像坐标**********************************************************************************************************

                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_row1 = hv_Tuple_bboxReduced_row1.TupleConcat(
                                        hv_Row1);
                                    hv_Tuple_bboxReduced_row1.Dispose();
                                    hv_Tuple_bboxReduced_row1 = ExpTmpLocalVar_Tuple_bboxReduced_row1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_col1 = hv_Tuple_bboxReduced_col1.TupleConcat(
                                        hv_Column1);
                                    hv_Tuple_bboxReduced_col1.Dispose();
                                    hv_Tuple_bboxReduced_col1 = ExpTmpLocalVar_Tuple_bboxReduced_col1;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_row2 = hv_Tuple_bboxReduced_row2.TupleConcat(
                                        hv_Row2);
                                    hv_Tuple_bboxReduced_row2.Dispose();
                                    hv_Tuple_bboxReduced_row2 = ExpTmpLocalVar_Tuple_bboxReduced_row2;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_col2 = hv_Tuple_bboxReduced_col2.TupleConcat(
                                        hv_Column2);
                                    hv_Tuple_bboxReduced_col2.Dispose();
                                    hv_Tuple_bboxReduced_col2 = ExpTmpLocalVar_Tuple_bboxReduced_col2;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_class_id = hv_Tuple_bboxReduced_class_id.TupleConcat(
                                        hv_IndexClass);
                                    hv_Tuple_bboxReduced_class_id.Dispose();
                                    hv_Tuple_bboxReduced_class_id = ExpTmpLocalVar_Tuple_bboxReduced_class_id;
                                }
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Tuple_bboxReduced_class_name = hv_Tuple_bboxReduced_class_name.TupleConcat(
                                        hv_ClassNames.TupleSelect(hv_IndexClass));
                                    hv_Tuple_bboxReduced_class_name.Dispose();
                                    hv_Tuple_bboxReduced_class_name = ExpTmpLocalVar_Tuple_bboxReduced_class_name;
                                }
                            }
                            //Tuple_bboxReduced_confidence := [Tuple_bboxReduced_confidence, Confidences[Index3]]
                            TargetResult aStructureOrDefect = new TargetResult();
                            aStructureOrDefect.ID = (int)hv_IndexClass.L;
                            aStructureOrDefect.TypeName = hv_ClassNames.TupleSelect(hv_IndexClass).S;
                            aStructureOrDefect.Row1 = hv_Row1.D;
                            aStructureOrDefect.Row2 = hv_Row2.D;
                            aStructureOrDefect.Column1 = hv_Column1.D;
                            aStructureOrDefect.Column2 = hv_Column2.D;
                            //detectResult.Confidence = hv_Confidences.TupleSelect(hv_Index3).D;
                            ListDetectResult.Add(aStructureOrDefect);
                        }

                        //检测封边四角。
                        if ((int)((new HTuple(hv_cameraPosition.TupleGreaterEqual(3))).TupleOr(
                            new HTuple(hv_cameraPosition.TupleLessEqual(6)))) != 0)
                        {
                            if ((int)((new HTuple(hv_RectNum.TupleGreaterEqual(1))).TupleAnd(new HTuple(((hv_ClassNames.TupleSelect(
                                hv_IndexClass))).TupleEqual("Edge")))) != 0)
                            {
                                hv_Tuple_corner_rows.Dispose(); hv_Tuple_corner_cols.Dispose();
                                FindRegionCorner(ho_CurrentClassRectanglesclosing, hv_cameraPosition,
                                    out hv_Tuple_corner_rows, out hv_Tuple_corner_cols);


                                //计算封边厚度
                                hv_Tuple_Thickness.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Tuple_Thickness = hv_Tuple_CurrentClass_bbox_row2 - hv_Tuple_CurrentClass_bbox_row1;
                                }
                                hv_BoardThickness.Dispose();
                                HOperatorSet.TupleMean(hv_Tuple_Thickness, out hv_BoardThickness);
                                results.Add("BoardThickness", hv_BoardThickness.D * 0.04964652 * 1.05409);

                            }
                        }

                    }
                }



                //if (|Tuple_bboxReduced_row1|>0)
                //gen_rectangle1 (RectanglesReduced, Tuple_bboxReduced_row1, Tuple_bboxReduced_col1, Tuple_bboxReduced_row2, Tuple_bboxReduced_col2)
                //endif


                hv_EndAll.Dispose();
                HOperatorSet.CountSeconds(out hv_EndAll);
                //SecondsAll := EndAll - StartAll
                //*********************************************************************************************************
                //转换图像坐标为世界坐标
                //if ((int)(new HTuple(hv_cameraPosition.TupleEqual(1))) != 0)
                //{
                //    hv_Tuple_corner_WorldX.Dispose(); hv_Tuple_corner_WorldY.Dispose();
                //    HOperatorSet.ImagePointsToWorldPlane(hv_CameraParameters, hv_CameraPose,
                //        hv_Tuple_corner_rows, hv_Tuple_corner_cols, "m", out hv_Tuple_corner_WorldX,
                //        out hv_Tuple_corner_WorldY);
                //    hv_Tuple_corner_rows.Dispose();
                //    hv_Tuple_corner_rows = new HTuple(hv_Tuple_corner_WorldY);
                //    hv_Tuple_corner_cols.Dispose();
                //    hv_Tuple_corner_cols = new HTuple(hv_Tuple_corner_WorldX);
                //}
                //以上注销，直接返回图像坐标**********************************************************************************************************
                if ((int)(new HTuple((new HTuple(hv_Tuple_corner_rows.TupleLength()
                       )).TupleGreater(0))) != 0)
                {
                    results.Add("Row1", hv_Tuple_corner_rows[0].D);
                    results.Add("Col1", hv_Tuple_corner_cols[0].D);
                    results.Add("Row2", hv_Tuple_corner_rows[1].D);
                    results.Add("Col2", hv_Tuple_corner_cols[1].D);
                    results.Add("Row3", hv_Tuple_corner_rows[2].D);
                    results.Add("Col3", hv_Tuple_corner_cols[2].D);
                    results.Add("Row4", hv_Tuple_corner_rows[3].D);
                    results.Add("Col4", hv_Tuple_corner_cols[3].D);
                    results.Add("result", ListDetectResult);
                }

                return results;
            }
            catch (HalconException HDevExpDefaultException)
            {

                throw HDevExpDefaultException;
            }
            finally
            {
                hv_Tuple_corner_rows.Dispose();
                hv_Tuple_corner_cols.Dispose();
                ho_RegionUnion.Dispose();
                ho_GrooveRectangles.Dispose();
                ho_ExtendedGrooveRectangles.Dispose();
                ho_HoleRectangle.Dispose();
                ho_ExtendedGrooveSelected.Dispose();
                ho_RegionIntersection.Dispose();
                ho_GrooveSelected.Dispose();
                ho_CurrentClassRectangles.Dispose();
                ho_CurrentClassRectanglesUnion.Dispose();
                ho_CurrentClassRectanglesclosing.Dispose();
                ho_CurrentClassRectanglesUnionsComponents.Dispose();
                ho_CurrentClassRectanglesReduced.Dispose();
                ho_RectangleSelected.Dispose();

                ho_RegionTuple.Dispose();
                ho_Image.Dispose();

                hv_RowUnion1.Dispose();
                hv_ColumnUnion1.Dispose();
                hv_RowUnion2.Dispose();
                hv_ColumnUnion2.Dispose();

                hv_Tuple_bbox_class_id_COPY_INP_TMP.Dispose();
                hv_Tuple_bbox_class_name_COPY_INP_TMP.Dispose();
                hv_ClassIDs.Dispose();
                hv_ClassNames.Dispose();
                hv_Length.Dispose();
                hv_HoleIndex.Dispose();
                hv_GrooveIndex.Dispose();
                hv_Tuple_Groove_bbox_row1.Dispose();
                hv_Tuple_Groove_bbox_col1.Dispose();
                hv_Tuple_Groove_bbox_row2.Dispose();
                hv_Tuple_Groove_bbox_col2.Dispose();
                hv_Tuple_ExtendedGroove_bbox_row1.Dispose();
                hv_Tuple_ExtendedGroove_bbox_col1.Dispose();
                hv_Tuple_ExtendedGroove_bbox_row2.Dispose();
                hv_Tuple_ExtendedGroove_bbox_col2.Dispose();
                hv_Index.Dispose();
                hv_GrooveHeight.Dispose();
                hv_GrooveWidth.Dispose();
                hv_ExtendDistance.Dispose();
                hv_GrooveNum.Dispose();
                hv_Index2.Dispose();
                hv_AreaIntersection.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_AreaHole.Dispose();
                hv_Ra.Dispose();
                hv_Rb.Dispose();
                hv_Phi.Dispose();
                hv_HoleWidth.Dispose();
                hv_IndexClass.Dispose();
                hv_Tuple_CurrentClass_bbox_row1.Dispose();
                hv_Tuple_CurrentClass_bbox_col1.Dispose();
                hv_Tuple_CurrentClass_bbox_row2.Dispose();
                hv_Tuple_CurrentClass_bbox_col2.Dispose();
                hv_Tuple_CurrentClass_bbox_class_id.Dispose();
                hv_Tuple_CurrentClass_bbox_class_name.Dispose();
                hv_Tuple_CurrentClass_bbox_confidence.Dispose();
                hv_Index4.Dispose();
                hv_RectNum.Dispose();
                hv_Index5.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_Tuple_Thickness.Dispose();
                hv_EndAll.Dispose();
                hv_Tuple_corner_WorldX.Dispose();
                hv_Tuple_corner_WorldY.Dispose();
                hv_Tuple_bboxReduced_WorldX1.Dispose();
                hv_Tuple_bboxReduced_WorldY1.Dispose();
                hv_Tuple_bboxReduced_WorldX2.Dispose();
                hv_Tuple_bboxReduced_WorldY2.Dispose();

                hv_X1.Dispose();
                hv_Y1.Dispose();
                hv_X2.Dispose();
                hv_Y2.Dispose();
                hv_cameraPosition.Dispose();
                hv_CameraParameters.Dispose();
                hv_CameraPose.Dispose();
                hv_Tuple_bboxReduced_class_id.Dispose();
                hv_Tuple_bboxReduced_row1.Dispose();
                hv_Tuple_bboxReduced_col1.Dispose();
                hv_Tuple_bboxReduced_row2.Dispose();
                hv_Tuple_bboxReduced_col2.Dispose();
                hv_Tuple_bboxReduced_class_name.Dispose();
                hv_Tuple_corner_rows.Dispose();
                hv_Tuple_corner_cols.Dispose();
                hv_BoardThickness.Dispose();
                ResetCacheData();
            }
        }
        public void ResetCacheData()
        {
            ho_ContoursTuple?.Dispose();
            hv_Tuple_bbox_class_id?.Dispose();
            hv_Tuple_bbox_row1?.Dispose();
            hv_Tuple_bbox_col1?.Dispose();
            hv_Tuple_bbox_row2?.Dispose();
            hv_Tuple_bbox_col2?.Dispose();
            hv_Tuple_bbox_class_name?.Dispose();
            hv_Tuple_bbox_confidence?.Dispose();
            hv_Tuple_bbox_class_id = new HTuple();
            hv_Tuple_bbox_row1 = new HTuple();
            hv_Tuple_bbox_col1 = new HTuple();
            hv_Tuple_bbox_row2 = new HTuple();
            hv_Tuple_bbox_col2 = new HTuple();
            hv_Tuple_bbox_class_name = new HTuple();
            hv_Tuple_bbox_confidence = new HTuple();
            HOperatorSet.GenEmptyObj(out ho_ContoursTuple);
        }
        public override void UnInit()
        {
            if (hv_DLModelHandle != null) hv_DLModelHandle.Dispose();
            if (hv_BatchSize != null) hv_BatchSize.Dispose();
            if (hv_DLPreprocessParam != null) hv_DLPreprocessParam.Dispose();
            IsInit = false;
        }
        #region Halcon External Procedure

        // Procedures 
        // External procedures 
        // Chapter: Deep Learning / Model
        // Short Description: Checks the content of the parameter dictionary DLPreprocessParam. 
        private void check_dl_preprocess_param(HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_CheckParams = new HTuple(), hv_KeyExists = new HTuple();
            HTuple hv_DLModelType = new HTuple(), hv_Exception = new HTuple();
            HTuple hv_SupportedModelTypes = new HTuple(), hv_Index = new HTuple();
            HTuple hv_ParamNamesGeneral = new HTuple(), hv_ParamNamesSegmentation = new HTuple();
            HTuple hv_ParamNamesDetectionOptional = new HTuple(), hv_ParamNamesPreprocessingOptional = new HTuple();
            HTuple hv_ParamNamesAll = new HTuple(), hv_ParamNames = new HTuple();
            HTuple hv_KeysExists = new HTuple(), hv_I = new HTuple();
            HTuple hv_Exists = new HTuple(), hv_InputKeys = new HTuple();
            HTuple hv_Key = new HTuple(), hv_Value = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_ValidValues = new HTuple();
            HTuple hv_ValidTypes = new HTuple(), hv_V = new HTuple();
            HTuple hv_T = new HTuple(), hv_IsInt = new HTuple(), hv_ValidTypesListing = new HTuple();
            HTuple hv_ValidValueListing = new HTuple(), hv_EmptyStrings = new HTuple();
            HTuple hv_ImageRangeMinExists = new HTuple(), hv_ImageRangeMaxExists = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_IndexParam = new HTuple(), hv_SetBackgroundID = new HTuple();
            HTuple hv_ClassIDsBackground = new HTuple(), hv_Intersection = new HTuple();
            HTuple hv_IgnoreClassIDs = new HTuple(), hv_KnownClasses = new HTuple();
            HTuple hv_IgnoreClassID = new HTuple(), hv_OptionalKeysExist = new HTuple();
            HTuple hv_InstanceType = new HTuple(), hv_IsInstanceSegmentation = new HTuple();
            HTuple hv_IgnoreDirection = new HTuple(), hv_ClassIDsNoOrientation = new HTuple();
            HTuple hv_SemTypes = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure checks a dictionary with parameters for DL preprocessing.
                //
                hv_CheckParams.Dispose();
                hv_CheckParams = 1;
                //If check_params is set to false, do not check anything.
                hv_KeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "check_params",
                    out hv_KeyExists);
                if ((int)(hv_KeyExists) != 0)
                {
                    hv_CheckParams.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "check_params", out hv_CheckParams);
                    if ((int)(hv_CheckParams.TupleNot()) != 0)
                    {

                        hv_CheckParams.Dispose();
                        hv_KeyExists.Dispose();
                        hv_DLModelType.Dispose();
                        hv_Exception.Dispose();
                        hv_SupportedModelTypes.Dispose();
                        hv_Index.Dispose();
                        hv_ParamNamesGeneral.Dispose();
                        hv_ParamNamesSegmentation.Dispose();
                        hv_ParamNamesDetectionOptional.Dispose();
                        hv_ParamNamesPreprocessingOptional.Dispose();
                        hv_ParamNamesAll.Dispose();
                        hv_ParamNames.Dispose();
                        hv_KeysExists.Dispose();
                        hv_I.Dispose();
                        hv_Exists.Dispose();
                        hv_InputKeys.Dispose();
                        hv_Key.Dispose();
                        hv_Value.Dispose();
                        hv_Indices.Dispose();
                        hv_ValidValues.Dispose();
                        hv_ValidTypes.Dispose();
                        hv_V.Dispose();
                        hv_T.Dispose();
                        hv_IsInt.Dispose();
                        hv_ValidTypesListing.Dispose();
                        hv_ValidValueListing.Dispose();
                        hv_EmptyStrings.Dispose();
                        hv_ImageRangeMinExists.Dispose();
                        hv_ImageRangeMaxExists.Dispose();
                        hv_ImageRangeMin.Dispose();
                        hv_ImageRangeMax.Dispose();
                        hv_IndexParam.Dispose();
                        hv_SetBackgroundID.Dispose();
                        hv_ClassIDsBackground.Dispose();
                        hv_Intersection.Dispose();
                        hv_IgnoreClassIDs.Dispose();
                        hv_KnownClasses.Dispose();
                        hv_IgnoreClassID.Dispose();
                        hv_OptionalKeysExist.Dispose();
                        hv_InstanceType.Dispose();
                        hv_IsInstanceSegmentation.Dispose();
                        hv_IgnoreDirection.Dispose();
                        hv_ClassIDsNoOrientation.Dispose();
                        hv_SemTypes.Dispose();

                        return;
                    }
                }
                //
                try
                {
                    hv_DLModelType.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_DLModelType);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    throw new HalconException(new HTuple(new HTuple("DLPreprocessParam needs the parameter: '") + "model_type") + "'");
                }
                //
                //Check for correct model type.
                hv_SupportedModelTypes.Dispose();
                hv_SupportedModelTypes = new HTuple();
                hv_SupportedModelTypes[0] = "anomaly_detection";
                hv_SupportedModelTypes[1] = "classification";
                hv_SupportedModelTypes[2] = "detection";
                hv_SupportedModelTypes[3] = "segmentation";
                hv_Index.Dispose();
                HOperatorSet.TupleFind(hv_SupportedModelTypes, hv_DLModelType, out hv_Index);
                if ((int)((new HTuple(hv_Index.TupleEqual(-1))).TupleOr(new HTuple(hv_Index.TupleEqual(
                    new HTuple())))) != 0)
                {
                    throw new HalconException(new HTuple("Only models of type 'anomaly_detection', 'classification', 'detection', or 'segmentation' are supported"));

                    hv_CheckParams.Dispose();
                    hv_KeyExists.Dispose();
                    hv_DLModelType.Dispose();
                    hv_Exception.Dispose();
                    hv_SupportedModelTypes.Dispose();
                    hv_Index.Dispose();
                    hv_ParamNamesGeneral.Dispose();
                    hv_ParamNamesSegmentation.Dispose();
                    hv_ParamNamesDetectionOptional.Dispose();
                    hv_ParamNamesPreprocessingOptional.Dispose();
                    hv_ParamNamesAll.Dispose();
                    hv_ParamNames.Dispose();
                    hv_KeysExists.Dispose();
                    hv_I.Dispose();
                    hv_Exists.Dispose();
                    hv_InputKeys.Dispose();
                    hv_Key.Dispose();
                    hv_Value.Dispose();
                    hv_Indices.Dispose();
                    hv_ValidValues.Dispose();
                    hv_ValidTypes.Dispose();
                    hv_V.Dispose();
                    hv_T.Dispose();
                    hv_IsInt.Dispose();
                    hv_ValidTypesListing.Dispose();
                    hv_ValidValueListing.Dispose();
                    hv_EmptyStrings.Dispose();
                    hv_ImageRangeMinExists.Dispose();
                    hv_ImageRangeMaxExists.Dispose();
                    hv_ImageRangeMin.Dispose();
                    hv_ImageRangeMax.Dispose();
                    hv_IndexParam.Dispose();
                    hv_SetBackgroundID.Dispose();
                    hv_ClassIDsBackground.Dispose();
                    hv_Intersection.Dispose();
                    hv_IgnoreClassIDs.Dispose();
                    hv_KnownClasses.Dispose();
                    hv_IgnoreClassID.Dispose();
                    hv_OptionalKeysExist.Dispose();
                    hv_InstanceType.Dispose();
                    hv_IsInstanceSegmentation.Dispose();
                    hv_IgnoreDirection.Dispose();
                    hv_ClassIDsNoOrientation.Dispose();
                    hv_SemTypes.Dispose();

                    return;
                }
                //
                //Parameter names that are required.
                //General parameters.
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesGeneral = new HTuple();
                hv_ParamNamesGeneral[0] = "model_type";
                hv_ParamNamesGeneral[1] = "image_width";
                hv_ParamNamesGeneral[2] = "image_height";
                hv_ParamNamesGeneral[3] = "image_num_channels";
                hv_ParamNamesGeneral[4] = "image_range_min";
                hv_ParamNamesGeneral[5] = "image_range_max";
                hv_ParamNamesGeneral[6] = "normalization_type";
                hv_ParamNamesGeneral[7] = "domain_handling";
                //Segmentation specific parameters.
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesSegmentation = new HTuple();
                hv_ParamNamesSegmentation[0] = "ignore_class_ids";
                hv_ParamNamesSegmentation[1] = "set_background_id";
                hv_ParamNamesSegmentation[2] = "class_ids_background";
                //Detection specific parameters.
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesDetectionOptional = new HTuple();
                hv_ParamNamesDetectionOptional[0] = "instance_type";
                hv_ParamNamesDetectionOptional[1] = "ignore_direction";
                hv_ParamNamesDetectionOptional[2] = "class_ids_no_orientation";
                hv_ParamNamesDetectionOptional[3] = "instance_segmentation";
                //Normalization specific parameters.
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNamesPreprocessingOptional = new HTuple();
                hv_ParamNamesPreprocessingOptional[0] = "mean_values_normalization";
                hv_ParamNamesPreprocessingOptional[1] = "deviation_values_normalization";
                hv_ParamNamesPreprocessingOptional[2] = "check_params";
                //All parameters
                hv_ParamNamesAll.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ParamNamesAll = new HTuple();
                    hv_ParamNamesAll = hv_ParamNamesAll.TupleConcat(hv_ParamNamesGeneral, hv_ParamNamesSegmentation, hv_ParamNamesDetectionOptional, hv_ParamNamesPreprocessingOptional);
                }
                hv_ParamNames.Dispose();
                hv_ParamNames = new HTuple(hv_ParamNamesGeneral);
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    //Extend ParamNames for models of type segmentation.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_ParamNames = hv_ParamNames.TupleConcat(
                                hv_ParamNamesSegmentation);
                            hv_ParamNames.Dispose();
                            hv_ParamNames = ExpTmpLocalVar_ParamNames;
                        }
                    }
                }
                //
                //Check if legacy parameter exist.
                //Otherwise map it to the legal parameter.
                replace_legacy_preprocessing_parameters(hv_DLPreprocessParam);
                //
                //Check that all necessary parameters are included.
                //
                hv_KeysExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNames,
                    out hv_KeysExists);
                if ((int)(new HTuple(((((hv_KeysExists.TupleEqualElem(0))).TupleSum())).TupleGreater(
                    0))) != 0)
                {
                    for (hv_I = 0; (int)hv_I <= (int)(new HTuple(hv_KeysExists.TupleLength())); hv_I = (int)hv_I + 1)
                    {
                        hv_Exists.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Exists = hv_KeysExists.TupleSelect(
                                hv_I);
                        }
                        if ((int)(hv_Exists.TupleNot()) != 0)
                        {
                            throw new HalconException(("DLPreprocessParam needs the parameter: '" + (hv_ParamNames.TupleSelect(
                                hv_I))) + "'");
                        }
                    }
                }
                //
                //Check the keys provided.
                hv_InputKeys.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "keys", new HTuple(), out hv_InputKeys);
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_InputKeys.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                {
                    hv_Key.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Key = hv_InputKeys.TupleSelect(
                            hv_I);
                    }
                    hv_Value.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_Key, out hv_Value);
                    //Check that the key is known.
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_ParamNamesAll, hv_Key, out hv_Indices);
                    if ((int)(new HTuple(hv_Indices.TupleEqual(-1))) != 0)
                    {
                        throw new HalconException(("Unknown key for DLPreprocessParam: '" + (hv_InputKeys.TupleSelect(
                            hv_I))) + "'");

                        hv_CheckParams.Dispose();
                        hv_KeyExists.Dispose();
                        hv_DLModelType.Dispose();
                        hv_Exception.Dispose();
                        hv_SupportedModelTypes.Dispose();
                        hv_Index.Dispose();
                        hv_ParamNamesGeneral.Dispose();
                        hv_ParamNamesSegmentation.Dispose();
                        hv_ParamNamesDetectionOptional.Dispose();
                        hv_ParamNamesPreprocessingOptional.Dispose();
                        hv_ParamNamesAll.Dispose();
                        hv_ParamNames.Dispose();
                        hv_KeysExists.Dispose();
                        hv_I.Dispose();
                        hv_Exists.Dispose();
                        hv_InputKeys.Dispose();
                        hv_Key.Dispose();
                        hv_Value.Dispose();
                        hv_Indices.Dispose();
                        hv_ValidValues.Dispose();
                        hv_ValidTypes.Dispose();
                        hv_V.Dispose();
                        hv_T.Dispose();
                        hv_IsInt.Dispose();
                        hv_ValidTypesListing.Dispose();
                        hv_ValidValueListing.Dispose();
                        hv_EmptyStrings.Dispose();
                        hv_ImageRangeMinExists.Dispose();
                        hv_ImageRangeMaxExists.Dispose();
                        hv_ImageRangeMin.Dispose();
                        hv_ImageRangeMax.Dispose();
                        hv_IndexParam.Dispose();
                        hv_SetBackgroundID.Dispose();
                        hv_ClassIDsBackground.Dispose();
                        hv_Intersection.Dispose();
                        hv_IgnoreClassIDs.Dispose();
                        hv_KnownClasses.Dispose();
                        hv_IgnoreClassID.Dispose();
                        hv_OptionalKeysExist.Dispose();
                        hv_InstanceType.Dispose();
                        hv_IsInstanceSegmentation.Dispose();
                        hv_IgnoreDirection.Dispose();
                        hv_ClassIDsNoOrientation.Dispose();
                        hv_SemTypes.Dispose();

                        return;
                    }
                    //Set expected values and types.
                    hv_ValidValues.Dispose();
                    hv_ValidValues = new HTuple();
                    hv_ValidTypes.Dispose();
                    hv_ValidTypes = new HTuple();
                    if ((int)(new HTuple(hv_Key.TupleEqual("normalization_type"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "all_channels";
                        hv_ValidValues[1] = "first_channel";
                        hv_ValidValues[2] = "constant_values";
                        hv_ValidValues[3] = "none";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("domain_handling"))) != 0)
                    {
                        if ((int)(new HTuple(hv_DLModelType.TupleEqual("anomaly_detection"))) != 0)
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                            hv_ValidValues[2] = "keep_domain";
                        }
                        else
                        {
                            hv_ValidValues.Dispose();
                            hv_ValidValues = new HTuple();
                            hv_ValidValues[0] = "full_domain";
                            hv_ValidValues[1] = "crop_domain";
                        }
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("model_type"))) != 0)
                    {
                        hv_ValidValues.Dispose();
                        hv_ValidValues = new HTuple();
                        hv_ValidValues[0] = "anomaly_detection";
                        hv_ValidValues[1] = "classification";
                        hv_ValidValues[2] = "detection";
                        hv_ValidValues[3] = "segmentation";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("set_background_id"))) != 0)
                    {
                        hv_ValidTypes.Dispose();
                        hv_ValidTypes = "int";
                    }
                    else if ((int)(new HTuple(hv_Key.TupleEqual("class_ids_background"))) != 0)
                    {
                        hv_ValidTypes.Dispose();
                        hv_ValidTypes = "int";
                    }
                    //Check that type is valid.
                    if ((int)(new HTuple((new HTuple(hv_ValidTypes.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        for (hv_V = 0; (int)hv_V <= (int)((new HTuple(hv_ValidTypes.TupleLength())) - 1); hv_V = (int)hv_V + 1)
                        {
                            hv_T.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_T = hv_ValidTypes.TupleSelect(
                                    hv_V);
                            }
                            if ((int)(new HTuple(hv_T.TupleEqual("int"))) != 0)
                            {
                                hv_IsInt.Dispose();
                                HOperatorSet.TupleIsInt(hv_Value, out hv_IsInt);
                                if ((int)(hv_IsInt.TupleNot()) != 0)
                                {
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        {
                                            HTuple
                                              ExpTmpLocalVar_ValidTypes = ("'" + hv_ValidTypes) + "'";
                                            hv_ValidTypes.Dispose();
                                            hv_ValidTypes = ExpTmpLocalVar_ValidTypes;
                                        }
                                    }
                                    if ((int)(new HTuple((new HTuple(hv_ValidTypes.TupleLength())).TupleLess(
                                        2))) != 0)
                                    {
                                        hv_ValidTypesListing.Dispose();
                                        hv_ValidTypesListing = new HTuple(hv_ValidTypes);
                                    }
                                    else
                                    {
                                        hv_ValidTypesListing.Dispose();
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            hv_ValidTypesListing = ((((hv_ValidTypes.TupleSelectRange(
                                                0, (new HTuple(0)).TupleMax2((new HTuple(hv_ValidTypes.TupleLength()
                                                )) - 2))) + new HTuple(", ")) + (hv_ValidTypes.TupleSelect((new HTuple(hv_ValidTypes.TupleLength()
                                                )) - 1)))).TupleSum();
                                        }
                                    }
                                    throw new HalconException(((((("The value given in the key '" + hv_Key) + "' of DLPreprocessParam is invalid. Valid types are: ") + hv_ValidTypesListing) + ". The given value was '") + hv_Value) + "'.");

                                    hv_CheckParams.Dispose();
                                    hv_KeyExists.Dispose();
                                    hv_DLModelType.Dispose();
                                    hv_Exception.Dispose();
                                    hv_SupportedModelTypes.Dispose();
                                    hv_Index.Dispose();
                                    hv_ParamNamesGeneral.Dispose();
                                    hv_ParamNamesSegmentation.Dispose();
                                    hv_ParamNamesDetectionOptional.Dispose();
                                    hv_ParamNamesPreprocessingOptional.Dispose();
                                    hv_ParamNamesAll.Dispose();
                                    hv_ParamNames.Dispose();
                                    hv_KeysExists.Dispose();
                                    hv_I.Dispose();
                                    hv_Exists.Dispose();
                                    hv_InputKeys.Dispose();
                                    hv_Key.Dispose();
                                    hv_Value.Dispose();
                                    hv_Indices.Dispose();
                                    hv_ValidValues.Dispose();
                                    hv_ValidTypes.Dispose();
                                    hv_V.Dispose();
                                    hv_T.Dispose();
                                    hv_IsInt.Dispose();
                                    hv_ValidTypesListing.Dispose();
                                    hv_ValidValueListing.Dispose();
                                    hv_EmptyStrings.Dispose();
                                    hv_ImageRangeMinExists.Dispose();
                                    hv_ImageRangeMaxExists.Dispose();
                                    hv_ImageRangeMin.Dispose();
                                    hv_ImageRangeMax.Dispose();
                                    hv_IndexParam.Dispose();
                                    hv_SetBackgroundID.Dispose();
                                    hv_ClassIDsBackground.Dispose();
                                    hv_Intersection.Dispose();
                                    hv_IgnoreClassIDs.Dispose();
                                    hv_KnownClasses.Dispose();
                                    hv_IgnoreClassID.Dispose();
                                    hv_OptionalKeysExist.Dispose();
                                    hv_InstanceType.Dispose();
                                    hv_IsInstanceSegmentation.Dispose();
                                    hv_IgnoreDirection.Dispose();
                                    hv_ClassIDsNoOrientation.Dispose();
                                    hv_SemTypes.Dispose();

                                    return;
                                }
                            }
                            else
                            {
                                throw new HalconException("Internal error. Unknown valid type.");
                            }
                        }
                    }
                    //Check that value is valid.
                    if ((int)(new HTuple((new HTuple(hv_ValidValues.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_Index.Dispose();
                        HOperatorSet.TupleFindFirst(hv_ValidValues, hv_Value, out hv_Index);
                        if ((int)(new HTuple(hv_Index.TupleEqual(-1))) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_ValidValues = ("'" + hv_ValidValues) + "'";
                                    hv_ValidValues.Dispose();
                                    hv_ValidValues = ExpTmpLocalVar_ValidValues;
                                }
                            }
                            if ((int)(new HTuple((new HTuple(hv_ValidValues.TupleLength())).TupleLess(
                                2))) != 0)
                            {
                                hv_ValidValueListing.Dispose();
                                hv_ValidValueListing = new HTuple(hv_ValidValues);
                            }
                            else
                            {
                                hv_EmptyStrings.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_EmptyStrings = HTuple.TupleGenConst(
                                        (new HTuple(hv_ValidValues.TupleLength())) - 2, "");
                                }
                                hv_ValidValueListing.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_ValidValueListing = ((((hv_ValidValues.TupleSelectRange(
                                        0, (new HTuple(0)).TupleMax2((new HTuple(hv_ValidValues.TupleLength()
                                        )) - 2))) + new HTuple(", ")) + (hv_EmptyStrings.TupleConcat(hv_ValidValues.TupleSelect(
                                        (new HTuple(hv_ValidValues.TupleLength())) - 1))))).TupleSum();
                                }
                            }
                            throw new HalconException(((((("The value given in the key '" + hv_Key) + "' of DLPreprocessParam is invalid. Valid values are: ") + hv_ValidValueListing) + ". The given value was '") + hv_Value) + "'.");
                        }
                    }
                }
                //
                //Check the correct setting of ImageRangeMin and ImageRangeMax.
                if ((int)((new HTuple(hv_DLModelType.TupleEqual("classification"))).TupleOr(
                    new HTuple(hv_DLModelType.TupleEqual("detection")))) != 0)
                {
                    //Check ImageRangeMin and ImageRangeMax.
                    hv_ImageRangeMinExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "image_range_min",
                        out hv_ImageRangeMinExists);
                    hv_ImageRangeMaxExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "image_range_max",
                        out hv_ImageRangeMaxExists);
                    //If they are present, check that they are set correctly.
                    if ((int)(hv_ImageRangeMinExists) != 0)
                    {
                        hv_ImageRangeMin.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                        if ((int)(new HTuple(hv_ImageRangeMin.TupleNotEqual(-127))) != 0)
                        {
                            throw new HalconException(("For model type " + hv_DLModelType) + " ImageRangeMin has to be -127.");
                        }
                    }
                    if ((int)(hv_ImageRangeMaxExists) != 0)
                    {
                        hv_ImageRangeMax.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                        if ((int)(new HTuple(hv_ImageRangeMax.TupleNotEqual(128))) != 0)
                        {
                            throw new HalconException(("For model type " + hv_DLModelType) + " ImageRangeMax has to be 128.");
                        }
                    }
                }
                //
                //Check segmentation specific parameters.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    //Check if detection specific parameters are set.
                    hv_KeysExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesDetectionOptional,
                        out hv_KeysExists);
                    //If they are present, check that they are [].
                    for (hv_IndexParam = 0; (int)hv_IndexParam <= (int)((new HTuple(hv_ParamNamesDetectionOptional.TupleLength()
                        )) - 1); hv_IndexParam = (int)hv_IndexParam + 1)
                    {
                        if ((int)(hv_KeysExists.TupleSelect(hv_IndexParam)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Value.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                    hv_IndexParam), out hv_Value);
                            }
                            if ((int)(new HTuple(hv_Value.TupleNotEqual(new HTuple()))) != 0)
                            {
                                throw new HalconException(((("The preprocessing parameter '" + (hv_ParamNamesDetectionOptional.TupleSelect(
                                    hv_IndexParam))) + "' was set to ") + hv_Value) + new HTuple(" but for segmentation it should be set to [], as it is not used for this method."));
                            }
                        }
                    }
                    //Check 'set_background_id'.
                    hv_SetBackgroundID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "set_background_id", out hv_SetBackgroundID);
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleGreater(
                        1))) != 0)
                    {
                        throw new HalconException("Only one class_id as 'set_background_id' allowed.");
                    }
                    //Check 'class_ids_background'.
                    hv_ClassIDsBackground.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_background", out hv_ClassIDsBackground);
                    if ((int)((new HTuple((new HTuple((new HTuple(hv_SetBackgroundID.TupleLength()
                        )).TupleGreater(0))).TupleAnd((new HTuple((new HTuple(hv_ClassIDsBackground.TupleLength()
                        )).TupleGreater(0))).TupleNot()))).TupleOr((new HTuple((new HTuple(hv_ClassIDsBackground.TupleLength()
                        )).TupleGreater(0))).TupleAnd((new HTuple((new HTuple(hv_SetBackgroundID.TupleLength()
                        )).TupleGreater(0))).TupleNot()))) != 0)
                    {
                        throw new HalconException("Both keys 'set_background_id' and 'class_ids_background' are required.");
                    }
                    //Check that 'class_ids_background' and 'set_background_id' are disjoint.
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleGreater(
                        0))) != 0)
                    {
                        hv_Intersection.Dispose();
                        HOperatorSet.TupleIntersection(hv_SetBackgroundID, hv_ClassIDsBackground,
                            out hv_Intersection);
                        if ((int)(new HTuple(hv_Intersection.TupleLength())) != 0)
                        {
                            throw new HalconException("Class IDs in 'set_background_id' and 'class_ids_background' need to be disjoint.");
                        }
                    }
                    //Check 'ignore_class_ids'.
                    hv_IgnoreClassIDs.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", out hv_IgnoreClassIDs);
                    hv_KnownClasses.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_KnownClasses = new HTuple();
                        hv_KnownClasses = hv_KnownClasses.TupleConcat(hv_SetBackgroundID, hv_ClassIDsBackground);
                    }
                    for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_IgnoreClassIDs.TupleLength()
                        )) - 1); hv_I = (int)hv_I + 1)
                    {
                        hv_IgnoreClassID.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IgnoreClassID = hv_IgnoreClassIDs.TupleSelect(
                                hv_I);
                        }
                        hv_Index.Dispose();
                        HOperatorSet.TupleFindFirst(hv_KnownClasses, hv_IgnoreClassID, out hv_Index);
                        if ((int)((new HTuple((new HTuple(hv_Index.TupleLength())).TupleGreater(
                            0))).TupleAnd(new HTuple(hv_Index.TupleNotEqual(-1)))) != 0)
                        {
                            throw new HalconException("The given 'ignore_class_ids' must not be included in the 'class_ids_background' or 'set_background_id'.");
                        }
                    }
                }
                else if ((int)(new HTuple(hv_DLModelType.TupleEqual("detection"))) != 0)
                {
                    //Check if segmentation specific parameters are set.
                    hv_KeysExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesSegmentation,
                        out hv_KeysExists);
                    //If they are present, check that they are [].
                    for (hv_IndexParam = 0; (int)hv_IndexParam <= (int)((new HTuple(hv_ParamNamesSegmentation.TupleLength()
                        )) - 1); hv_IndexParam = (int)hv_IndexParam + 1)
                    {
                        if ((int)(hv_KeysExists.TupleSelect(hv_IndexParam)) != 0)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Value.Dispose();
                                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesSegmentation.TupleSelect(
                                    hv_IndexParam), out hv_Value);
                            }
                            if ((int)(new HTuple(hv_Value.TupleNotEqual(new HTuple()))) != 0)
                            {
                                throw new HalconException(((("The preprocessing parameter '" + (hv_ParamNamesSegmentation.TupleSelect(
                                    hv_IndexParam))) + "' was set to ") + hv_Value) + new HTuple(" but for detection it should be set to [], as it is not used for this method."));
                            }
                        }
                    }
                    //Check optional parameters.
                    hv_OptionalKeysExist.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", hv_ParamNamesDetectionOptional,
                        out hv_OptionalKeysExist);
                    if ((int)(hv_OptionalKeysExist.TupleSelect(0)) != 0)
                    {
                        //Check 'instance_type'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_InstanceType.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                0), out hv_InstanceType);
                        }
                        if ((int)(new HTuple((new HTuple((((new HTuple("rectangle1")).TupleConcat(
                            "rectangle2")).TupleConcat("mask")).TupleFind(hv_InstanceType))).TupleEqual(
                            -1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_type': " + hv_InstanceType) + new HTuple(", only 'rectangle1' and 'rectangle2' are allowed"));
                        }
                    }
                    //If instance_segmentation is set we might overwrite the instance_type for the preprocessing.
                    if ((int)(hv_OptionalKeysExist.TupleSelect(3)) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IsInstanceSegmentation.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                3), out hv_IsInstanceSegmentation);
                        }
                        if ((int)(new HTuple((new HTuple(((((new HTuple(1)).TupleConcat(0)).TupleConcat(
                            "true")).TupleConcat("false")).TupleFind(hv_IsInstanceSegmentation))).TupleEqual(
                            -1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_segmentation': " + hv_IsInstanceSegmentation) + new HTuple(", only true, false, 'true' and 'false' are allowed"));
                        }
                    }
                    if ((int)(hv_OptionalKeysExist.TupleSelect(1)) != 0)
                    {
                        //Check 'ignore_direction'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IgnoreDirection.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                1), out hv_IgnoreDirection);
                        }
                        if ((int)(new HTuple((new HTuple(((new HTuple(1)).TupleConcat(0)).TupleFind(
                            hv_IgnoreDirection))).TupleEqual(-1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'ignore_direction': " + hv_IgnoreDirection) + new HTuple(", only true and false are allowed"));
                        }
                    }
                    if ((int)(hv_OptionalKeysExist.TupleSelect(2)) != 0)
                    {
                        //Check 'class_ids_no_orientation'.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ClassIDsNoOrientation.Dispose();
                            HOperatorSet.GetDictTuple(hv_DLPreprocessParam, hv_ParamNamesDetectionOptional.TupleSelect(
                                2), out hv_ClassIDsNoOrientation);
                        }
                        hv_SemTypes.Dispose();
                        HOperatorSet.TupleSemTypeElem(hv_ClassIDsNoOrientation, out hv_SemTypes);
                        if ((int)((new HTuple(hv_ClassIDsNoOrientation.TupleNotEqual(new HTuple()))).TupleAnd(
                            new HTuple(((((hv_SemTypes.TupleEqualElem("integer"))).TupleSum())).TupleNotEqual(
                            new HTuple(hv_ClassIDsNoOrientation.TupleLength()))))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'class_ids_no_orientation': " + hv_ClassIDsNoOrientation) + new HTuple(", only integers are allowed"));
                        }
                        else
                        {
                            if ((int)((new HTuple(hv_ClassIDsNoOrientation.TupleNotEqual(new HTuple()))).TupleAnd(
                                new HTuple(((((hv_ClassIDsNoOrientation.TupleGreaterEqualElem(0))).TupleSum()
                                )).TupleNotEqual(new HTuple(hv_ClassIDsNoOrientation.TupleLength()
                                ))))) != 0)
                            {
                                throw new HalconException(("Invalid generic parameter for 'class_ids_no_orientation': " + hv_ClassIDsNoOrientation) + new HTuple(", only non-negative integers are allowed"));
                            }
                        }
                    }
                }
                //

                hv_CheckParams.Dispose();
                hv_KeyExists.Dispose();
                hv_DLModelType.Dispose();
                hv_Exception.Dispose();
                hv_SupportedModelTypes.Dispose();
                hv_Index.Dispose();
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNamesAll.Dispose();
                hv_ParamNames.Dispose();
                hv_KeysExists.Dispose();
                hv_I.Dispose();
                hv_Exists.Dispose();
                hv_InputKeys.Dispose();
                hv_Key.Dispose();
                hv_Value.Dispose();
                hv_Indices.Dispose();
                hv_ValidValues.Dispose();
                hv_ValidTypes.Dispose();
                hv_V.Dispose();
                hv_T.Dispose();
                hv_IsInt.Dispose();
                hv_ValidTypesListing.Dispose();
                hv_ValidValueListing.Dispose();
                hv_EmptyStrings.Dispose();
                hv_ImageRangeMinExists.Dispose();
                hv_ImageRangeMaxExists.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IndexParam.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassIDsBackground.Dispose();
                hv_Intersection.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_KnownClasses.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_OptionalKeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_SemTypes.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_CheckParams.Dispose();
                hv_KeyExists.Dispose();
                hv_DLModelType.Dispose();
                hv_Exception.Dispose();
                hv_SupportedModelTypes.Dispose();
                hv_Index.Dispose();
                hv_ParamNamesGeneral.Dispose();
                hv_ParamNamesSegmentation.Dispose();
                hv_ParamNamesDetectionOptional.Dispose();
                hv_ParamNamesPreprocessingOptional.Dispose();
                hv_ParamNamesAll.Dispose();
                hv_ParamNames.Dispose();
                hv_KeysExists.Dispose();
                hv_I.Dispose();
                hv_Exists.Dispose();
                hv_InputKeys.Dispose();
                hv_Key.Dispose();
                hv_Value.Dispose();
                hv_Indices.Dispose();
                hv_ValidValues.Dispose();
                hv_ValidTypes.Dispose();
                hv_V.Dispose();
                hv_T.Dispose();
                hv_IsInt.Dispose();
                hv_ValidTypesListing.Dispose();
                hv_ValidValueListing.Dispose();
                hv_EmptyStrings.Dispose();
                hv_ImageRangeMinExists.Dispose();
                hv_ImageRangeMaxExists.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IndexParam.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassIDsBackground.Dispose();
                hv_Intersection.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_KnownClasses.Dispose();
                hv_IgnoreClassID.Dispose();
                hv_OptionalKeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_SemTypes.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Tools / Geometry
        // Short Description: Convert the parameters of rectangles with format rectangle2 to the coordinates of its 4 corner-points. 
        private void convert_rect2_5to8param(HTuple hv_Row, HTuple hv_Col, HTuple hv_Length1,
            HTuple hv_Length2, HTuple hv_Phi, out HTuple hv_Row1, out HTuple hv_Col1, out HTuple hv_Row2,
            out HTuple hv_Col2, out HTuple hv_Row3, out HTuple hv_Col3, out HTuple hv_Row4,
            out HTuple hv_Col4)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Co1 = new HTuple(), hv_Co2 = new HTuple();
            HTuple hv_Si1 = new HTuple(), hv_Si2 = new HTuple();
            // Initialize local and output iconic variables 
            hv_Row1 = new HTuple();
            hv_Col1 = new HTuple();
            hv_Row2 = new HTuple();
            hv_Col2 = new HTuple();
            hv_Row3 = new HTuple();
            hv_Col3 = new HTuple();
            hv_Row4 = new HTuple();
            hv_Col4 = new HTuple();
            try
            {
                //This procedure takes the parameters for a rectangle of type 'rectangle2'
                //and returns the coordinates of the four corners.
                //
                hv_Co1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Co1 = (hv_Phi.TupleCos()
                        ) * hv_Length1;
                }
                hv_Co2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Co2 = (hv_Phi.TupleCos()
                        ) * hv_Length2;
                }
                hv_Si1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Si1 = (hv_Phi.TupleSin()
                        ) * hv_Length1;
                }
                hv_Si2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Si2 = (hv_Phi.TupleSin()
                        ) * hv_Length2;
                }

                hv_Col1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col1 = (hv_Co1 - hv_Si2) + hv_Col;
                }
                hv_Row1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row1 = ((-hv_Si1) - hv_Co2) + hv_Row;
                }
                hv_Col2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col2 = ((-hv_Co1) - hv_Si2) + hv_Col;
                }
                hv_Row2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row2 = (hv_Si1 - hv_Co2) + hv_Row;
                }
                hv_Col3.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col3 = ((-hv_Co1) + hv_Si2) + hv_Col;
                }
                hv_Row3.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row3 = (hv_Si1 + hv_Co2) + hv_Row;
                }
                hv_Col4.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col4 = (hv_Co1 + hv_Si2) + hv_Col;
                }
                hv_Row4.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row4 = ((-hv_Si1) + hv_Co2) + hv_Row;
                }


                hv_Co1.Dispose();
                hv_Co2.Dispose();
                hv_Si1.Dispose();
                hv_Si2.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Co1.Dispose();
                hv_Co2.Dispose();
                hv_Si1.Dispose();
                hv_Si2.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Tools / Geometry
        // Short Description: Convert for four-sided figures the coordinates of the 4 corner-points to the parameters of format rectangle2. 
        private void convert_rect2_8to5param(HTuple hv_Row1, HTuple hv_Col1, HTuple hv_Row2,
            HTuple hv_Col2, HTuple hv_Row3, HTuple hv_Col3, HTuple hv_Row4, HTuple hv_Col4,
            HTuple hv_ForceL1LargerL2, out HTuple hv_Row, out HTuple hv_Col, out HTuple hv_Length1,
            out HTuple hv_Length2, out HTuple hv_Phi)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Hor = new HTuple(), hv_Vert = new HTuple();
            HTuple hv_IdxSwap = new HTuple(), hv_Tmp = new HTuple();
            // Initialize local and output iconic variables 
            hv_Row = new HTuple();
            hv_Col = new HTuple();
            hv_Length1 = new HTuple();
            hv_Length2 = new HTuple();
            hv_Phi = new HTuple();
            try
            {
                //This procedure takes the corners of four-sided figures
                //and returns the parameters of type 'rectangle2'.
                //
                //Calculate center row and column.
                hv_Row.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Row = (((hv_Row1 + hv_Row2) + hv_Row3) + hv_Row4) / 4.0;
                }
                hv_Col.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Col = (((hv_Col1 + hv_Col2) + hv_Col3) + hv_Col4) / 4.0;
                }
                //Length1 and Length2.
                hv_Length1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Length1 = (((((hv_Row1 - hv_Row2) * (hv_Row1 - hv_Row2)) + ((hv_Col1 - hv_Col2) * (hv_Col1 - hv_Col2)))).TupleSqrt()
                        ) / 2.0;
                }
                hv_Length2.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Length2 = (((((hv_Row2 - hv_Row3) * (hv_Row2 - hv_Row3)) + ((hv_Col2 - hv_Col3) * (hv_Col2 - hv_Col3)))).TupleSqrt()
                        ) / 2.0;
                }
                //Calculate the angle phi.
                hv_Hor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Hor = hv_Col1 - hv_Col2;
                }
                hv_Vert.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Vert = hv_Row2 - hv_Row1;
                }
                if ((int)(hv_ForceL1LargerL2) != 0)
                {
                    //Swap length1 and length2 if necessary.
                    hv_IdxSwap.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_IdxSwap = ((((hv_Length2 - hv_Length1)).TupleGreaterElem(
                            1e-9))).TupleFind(1);
                    }
                    if ((int)(new HTuple(hv_IdxSwap.TupleNotEqual(-1))) != 0)
                    {
                        hv_Tmp.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Tmp = hv_Length1.TupleSelect(
                                hv_IdxSwap);
                        }
                        if (hv_Length1 == null)
                            hv_Length1 = new HTuple();
                        hv_Length1[hv_IdxSwap] = hv_Length2.TupleSelect(hv_IdxSwap);
                        if (hv_Length2 == null)
                            hv_Length2 = new HTuple();
                        hv_Length2[hv_IdxSwap] = hv_Tmp;
                        if (hv_Hor == null)
                            hv_Hor = new HTuple();
                        hv_Hor[hv_IdxSwap] = (hv_Col2.TupleSelect(hv_IdxSwap)) - (hv_Col3.TupleSelect(
                            hv_IdxSwap));
                        if (hv_Vert == null)
                            hv_Vert = new HTuple();
                        hv_Vert[hv_IdxSwap] = (hv_Row3.TupleSelect(hv_IdxSwap)) - (hv_Row2.TupleSelect(
                            hv_IdxSwap));
                    }
                }
                hv_Phi.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Phi = hv_Vert.TupleAtan2(
                        hv_Hor);
                }
                //

                hv_Hor.Dispose();
                hv_Vert.Dispose();
                hv_IdxSwap.Dispose();
                hv_Tmp.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Hor.Dispose();
                hv_Vert.Dispose();
                hv_IdxSwap.Dispose();
                hv_Tmp.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection
        // Short Description: Filters the instance segmentation masks of a DL sample based on a given selection. 
        private void filter_dl_sample_instance_segmentation_masks(HTuple hv_DLSample,
            HTuple hv_BBoxSelectionMask)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_EmptyMasks = null, ho_Masks = null;

            // Local control variables 

            HTuple hv_MaskKeyExists = new HTuple(), hv_Indices = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_EmptyMasks);
            HOperatorSet.GenEmptyObj(out ho_Masks);
            try
            {
                hv_MaskKeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLSample, "key_exists", "mask", out hv_MaskKeyExists);
                if ((int)(hv_MaskKeyExists) != 0)
                {
                    //Only if masks exist (-> instance segmentation).
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_BBoxSelectionMask, 1, out hv_Indices);
                    if ((int)(new HTuple(hv_Indices.TupleEqual(-1))) != 0)
                    {
                        //We define here that this case will result in an empty object value
                        //for the mask key. Another option would be to remove the
                        //key 'mask'. However, this would be an unwanted big change in the dictionary.
                        ho_EmptyMasks.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_EmptyMasks);
                        HOperatorSet.SetDictObject(ho_EmptyMasks, hv_DLSample, "mask");
                    }
                    else
                    {
                        ho_Masks.Dispose();
                        HOperatorSet.GetDictObject(out ho_Masks, hv_DLSample, "mask");
                        //Remove all unused masks.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.SelectObj(ho_Masks, out ExpTmpOutVar_0, hv_Indices + 1);
                            ho_Masks.Dispose();
                            ho_Masks = ExpTmpOutVar_0;
                        }
                        HOperatorSet.SetDictObject(ho_Masks, hv_DLSample, "mask");
                    }
                }
                ho_EmptyMasks.Dispose();
                ho_Masks.Dispose();

                hv_MaskKeyExists.Dispose();
                hv_Indices.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_EmptyMasks.Dispose();
                ho_Masks.Dispose();

                hv_MaskKeyExists.Dispose();
                hv_Indices.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Store the given images in a tuple of dictionaries DLSamples. 
        private void gen_dl_samples_from_images(HObject ho_Images, out HTuple hv_DLSampleBatch)
        {



            // Local iconic variables 

            HObject ho_Image = null;

            // Local control variables 

            HTuple hv_NumImages = new HTuple(), hv_ImageIndex = new HTuple();
            HTuple hv_DLSample = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            hv_DLSampleBatch = new HTuple();
            try
            {
                //
                //This procedure creates DLSampleBatch, a tuple
                //containing a dictionary DLSample
                //for every image given in Images.
                //
                //Initialize output tuple.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images, out hv_NumImages);
                hv_DLSampleBatch.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DLSampleBatch = HTuple.TupleGenConst(
                        hv_NumImages, -1);
                }
                //
                //Loop through all given images.
                HTuple end_val10 = hv_NumImages - 1;
                HTuple step_val10 = 1;
                for (hv_ImageIndex = 0; hv_ImageIndex.Continue(end_val10, step_val10); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val10))
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Image.Dispose();
                        HOperatorSet.SelectObj(ho_Images, out ho_Image, hv_ImageIndex + 1);
                    }
                    //Create DLSample from image.
                    hv_DLSample.Dispose();
                    HOperatorSet.CreateDict(out hv_DLSample);
                    HOperatorSet.SetDictObject(ho_Image, hv_DLSample, "image");
                    //
                    //Collect the DLSamples.
                    if (hv_DLSampleBatch == null)
                        hv_DLSampleBatch = new HTuple();
                    hv_DLSampleBatch[hv_ImageIndex] = hv_DLSample;
                }
                ho_Image.Dispose();

                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_DLSample.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Image.Dispose();

                hv_NumImages.Dispose();
                hv_ImageIndex.Dispose();
                hv_DLSample.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess anomaly images for evaluation and visualization of the deep-learning-based anomaly detection. 
        private void preprocess_dl_model_anomaly(HObject ho_AnomalyImages, out HObject ho_AnomalyImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            // Local copy input parameter variables 
            HObject ho_AnomalyImages_COPY_INP_TMP;
            ho_AnomalyImages_COPY_INP_TMP = new HObject(ho_AnomalyImages);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_Min = new HTuple();
            HTuple hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_ImageWidthInput = new HTuple(), hv_ImageHeightInput = new HTuple();
            HTuple hv_EqualWidth = new HTuple(), hv_EqualHeight = new HTuple();
            HTuple hv_Type = new HTuple(), hv_NumMatches = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_EqualByte = new HTuple();
            HTuple hv_NumChannelsAllImages = new HTuple(), hv_ImageNumChannelsTuple = new HTuple();
            HTuple hv_IndicesWrongChannels = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_AnomalyImagesPreprocessed);
            try
            {
                //
                //This procedure preprocesses the anomaly images given by AnomalyImages
                //according to the parameters in the dictionary DLPreprocessParam.
                //Note that depending on the images,
                //additional preprocessing steps might be beneficial.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                hv_ImageNumChannels.Dispose();
                hv_ImageNumChannels = 1;
                //
                //Preprocess the images.
                //
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CropDomain(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)((new HTuple(hv_DomainHandling.TupleEqual("keep_domain"))).TupleAnd(
                    new HTuple(hv_ModelType.TupleEqual("anomaly_detection")))) != 0)
                {
                    //Anomaly detection models accept the additional option 'keep_domain'.
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                HOperatorSet.MinMaxGray(ho_AnomalyImages_COPY_INP_TMP, ho_AnomalyImages_COPY_INP_TMP,
                    0, out hv_Min, out hv_Max, out hv_Range);
                if ((int)(new HTuple(hv_Min.TupleLess(0.0))) != 0)
                {
                    throw new HalconException("Values of anomaly image must not be smaller than 0.0.");
                }
                //
                //Zoom images only if they have a different size than the specified size.
                hv_ImageWidthInput.Dispose(); hv_ImageHeightInput.Dispose();
                HOperatorSet.GetImageSize(ho_AnomalyImages_COPY_INP_TMP, out hv_ImageWidthInput,
                    out hv_ImageHeightInput);
                hv_EqualWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualWidth = hv_ImageWidth.TupleEqualElem(
                        hv_ImageWidthInput);
                }
                hv_EqualHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualHeight = hv_ImageHeight.TupleEqualElem(
                        hv_ImageHeightInput);
                }
                if ((int)((new HTuple(((hv_EqualWidth.TupleMin())).TupleEqual(0))).TupleOr(
                    new HTuple(((hv_EqualHeight.TupleMin())).TupleEqual(0)))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ImageWidth, hv_ImageHeight, "nearest_neighbor");
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the type of the input images.
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_AnomalyImages_COPY_INP_TMP, out hv_Type);
                hv_NumMatches.Dispose();
                HOperatorSet.TupleRegexpTest(hv_Type, "byte|real", out hv_NumMatches);
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_AnomalyImages_COPY_INP_TMP, out hv_NumImages);
                if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                {
                    throw new HalconException("Please provide only images of type 'byte' or 'real'.");
                }
                //
                //If the type is 'byte', convert it to 'real' and scale it.
                //The gray value scaling does not work on 'byte' images.
                //For 'real' images it is assumed that the range is already correct.
                hv_EqualByte.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualByte = hv_Type.TupleEqualElem(
                        "byte");
                }
                if ((int)(new HTuple(((hv_EqualByte.TupleMax())).TupleEqual(1))) != 0)
                {
                    if ((int)(new HTuple(((hv_EqualByte.TupleMin())).TupleEqual(0))) != 0)
                    {
                        throw new HalconException("Passing mixed type images is not supported.");
                    }
                    //Convert the image type from 'byte' to 'real',
                    //because the model expects 'real' images.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_AnomalyImages_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_AnomalyImages_COPY_INP_TMP.Dispose();
                        ho_AnomalyImages_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the number of channels.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_AnomalyImages_COPY_INP_TMP, out hv_NumImages);
                //Check all images for number of channels.
                hv_NumChannelsAllImages.Dispose();
                HOperatorSet.CountChannels(ho_AnomalyImages_COPY_INP_TMP, out hv_NumChannelsAllImages);
                hv_ImageNumChannelsTuple.Dispose();
                HOperatorSet.TupleGenConst(hv_NumImages, hv_ImageNumChannels, out hv_ImageNumChannelsTuple);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_IndicesWrongChannels.Dispose();
                    HOperatorSet.TupleFind(hv_NumChannelsAllImages.TupleNotEqualElem(hv_ImageNumChannelsTuple),
                        1, out hv_IndicesWrongChannels);
                }
                //
                //Check for anomaly image channels.
                //Only single channel images are accepted.
                if ((int)(new HTuple(hv_IndicesWrongChannels.TupleNotEqual(-1))) != 0)
                {
                    throw new HalconException("Number of channels in anomaly image is not supported. Please check for anomaly images with a number of channels different from 1.");
                }
                //
                //Write preprocessed image to output variable.
                ho_AnomalyImagesPreprocessed.Dispose();
                ho_AnomalyImagesPreprocessed = new HObject(ho_AnomalyImages_COPY_INP_TMP);
                //
                ho_AnomalyImages_COPY_INP_TMP.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_ModelType.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_EqualByte.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_AnomalyImages_COPY_INP_TMP.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_ModelType.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_ImageWidthInput.Dispose();
                hv_ImageHeightInput.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_NumImages.Dispose();
                hv_EqualByte.Dispose();
                hv_NumChannelsAllImages.Dispose();
                hv_ImageNumChannelsTuple.Dispose();
                hv_IndicesWrongChannels.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection
        // Short Description: This procedure preprocesses the bounding boxes of type 'rectangle1' for a given sample. 
        private void preprocess_dl_model_bbox_rect1(HObject ho_ImageRaw, HTuple hv_DLSample,
            HTuple hv_DLPreprocessParam)
        {




            // Local iconic variables 

            HObject ho_DomainRaw = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_BBoxCol1 = new HTuple();
            HTuple hv_BBoxCol2 = new HTuple(), hv_BBoxRow1 = new HTuple();
            HTuple hv_BBoxRow2 = new HTuple(), hv_BBoxLabel = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_ImageId = new HTuple();
            HTuple hv_ExceptionMessage = new HTuple(), hv_BoxesInvalid = new HTuple();
            HTuple hv_DomainRow1 = new HTuple(), hv_DomainColumn1 = new HTuple();
            HTuple hv_DomainRow2 = new HTuple(), hv_DomainColumn2 = new HTuple();
            HTuple hv_WidthRaw = new HTuple(), hv_HeightRaw = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Col1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Col2 = new HTuple();
            HTuple hv_MaskDelete = new HTuple(), hv_MaskNewBbox = new HTuple();
            HTuple hv_BBoxCol1New = new HTuple(), hv_BBoxCol2New = new HTuple();
            HTuple hv_BBoxRow1New = new HTuple(), hv_BBoxRow2New = new HTuple();
            HTuple hv_BBoxLabelNew = new HTuple(), hv_FactorResampleWidth = new HTuple();
            HTuple hv_FactorResampleHeight = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_DomainRaw);
            try
            {
                //
                //This procedure preprocesses the bounding boxes of type 'rectangle1' for a given sample.
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //
                //Get bounding box coordinates and labels.
                try
                {
                    hv_BBoxCol1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col1", out hv_BBoxCol1);
                    hv_BBoxCol2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col2", out hv_BBoxCol2);
                    hv_BBoxRow1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row1", out hv_BBoxRow1);
                    hv_BBoxRow2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row2", out hv_BBoxRow2);
                    hv_BBoxLabel.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_label_id", out hv_BBoxLabel);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_ImageId.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                    if ((int)(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1302))) != 0)
                    {
                        hv_ExceptionMessage.Dispose();
                        hv_ExceptionMessage = "A bounding box coordinate key is missing.";
                    }
                    else
                    {
                        hv_ExceptionMessage.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ExceptionMessage = hv_Exception.TupleSelect(
                                2);
                        }
                    }
                    throw new HalconException((("An error has occurred during preprocessing image_id " + hv_ImageId) + " when getting bounding box coordinates : ") + hv_ExceptionMessage);
                }
                //
                //Check that there are no invalid boxes.
                if ((int)(new HTuple((new HTuple(hv_BBoxRow1.TupleLength())).TupleGreater(0))) != 0)
                {
                    hv_BoxesInvalid.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BoxesInvalid = ((hv_BBoxRow1.TupleGreaterEqualElem(
                            hv_BBoxRow2))).TupleOr(hv_BBoxCol1.TupleGreaterEqualElem(hv_BBoxCol2));
                    }
                    if ((int)(new HTuple(((hv_BoxesInvalid.TupleSum())).TupleGreater(0))) != 0)
                    {
                        hv_ImageId.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                        throw new HalconException(("An error has occurred during preprocessing image_id " + hv_ImageId) + new HTuple(": Sample contains at least one box with zero-area, i.e. bbox_col1 >= bbox_col2 or bbox_row1 >= bbox_row2."));
                    }
                }
                else
                {
                    //There are no bounding boxes, hence nothing to do.
                    ho_DomainRaw.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_DomainHandling.Dispose();
                    hv_BBoxCol1.Dispose();
                    hv_BBoxCol2.Dispose();
                    hv_BBoxRow1.Dispose();
                    hv_BBoxRow2.Dispose();
                    hv_BBoxLabel.Dispose();
                    hv_Exception.Dispose();
                    hv_ImageId.Dispose();
                    hv_ExceptionMessage.Dispose();
                    hv_BoxesInvalid.Dispose();
                    hv_DomainRow1.Dispose();
                    hv_DomainColumn1.Dispose();
                    hv_DomainRow2.Dispose();
                    hv_DomainColumn2.Dispose();
                    hv_WidthRaw.Dispose();
                    hv_HeightRaw.Dispose();
                    hv_Row1.Dispose();
                    hv_Col1.Dispose();
                    hv_Row2.Dispose();
                    hv_Col2.Dispose();
                    hv_MaskDelete.Dispose();
                    hv_MaskNewBbox.Dispose();
                    hv_BBoxCol1New.Dispose();
                    hv_BBoxCol2New.Dispose();
                    hv_BBoxRow1New.Dispose();
                    hv_BBoxRow2New.Dispose();
                    hv_BBoxLabelNew.Dispose();
                    hv_FactorResampleWidth.Dispose();
                    hv_FactorResampleHeight.Dispose();

                    return;
                }
                //
                //If the domain is cropped, crop bounding boxes.
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //
                    //Get domain.
                    ho_DomainRaw.Dispose();
                    HOperatorSet.GetDomain(ho_ImageRaw, out ho_DomainRaw);
                    //
                    //Set the size of the raw image to the domain extensions.
                    hv_DomainRow1.Dispose(); hv_DomainColumn1.Dispose(); hv_DomainRow2.Dispose(); hv_DomainColumn2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_DomainRaw, out hv_DomainRow1, out hv_DomainColumn1,
                        out hv_DomainRow2, out hv_DomainColumn2);
                    //The domain is always given as a pixel-precise region.
                    hv_WidthRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WidthRaw = (hv_DomainColumn2 - hv_DomainColumn1) + 1.0;
                    }
                    hv_HeightRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HeightRaw = (hv_DomainRow2 - hv_DomainRow1) + 1.0;
                    }
                    //
                    //Crop the bounding boxes.
                    hv_Row1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row1 = hv_BBoxRow1.TupleMax2(
                            hv_DomainRow1 - .5);
                    }
                    hv_Col1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col1 = hv_BBoxCol1.TupleMax2(
                            hv_DomainColumn1 - .5);
                    }
                    hv_Row2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Row2 = hv_BBoxRow2.TupleMin2(
                            hv_DomainRow2 + .5);
                    }
                    hv_Col2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Col2 = hv_BBoxCol2.TupleMin2(
                            hv_DomainColumn2 + .5);
                    }
                    hv_MaskDelete.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskDelete = ((hv_Row1.TupleGreaterEqualElem(
                            hv_Row2))).TupleOr(hv_Col1.TupleGreaterEqualElem(hv_Col2));
                    }
                    hv_MaskNewBbox.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskNewBbox = 1 - hv_MaskDelete;
                    }
                    //Store the preprocessed bounding box entries.
                    hv_BBoxCol1New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxCol1New = (hv_Col1.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainColumn1;
                    }
                    hv_BBoxCol2New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxCol2New = (hv_Col2.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainColumn1;
                    }
                    hv_BBoxRow1New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxRow1New = (hv_Row1.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainRow1;
                    }
                    hv_BBoxRow2New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxRow2New = (hv_Row2.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainRow1;
                    }
                    hv_BBoxLabelNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLabelNew = hv_BBoxLabel.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    //
                    //If we remove/select bounding boxes we also need to filter the corresponding
                    //instance segmentation masks if they exist.
                    filter_dl_sample_instance_segmentation_masks(hv_DLSample, hv_MaskNewBbox);
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    //If the entire image is used, set the variables accordingly.
                    //Get the original size.
                    hv_WidthRaw.Dispose(); hv_HeightRaw.Dispose();
                    HOperatorSet.GetImageSize(ho_ImageRaw, out hv_WidthRaw, out hv_HeightRaw);
                    //Set new coordinates to input coordinates.
                    hv_BBoxCol1New.Dispose();
                    hv_BBoxCol1New = new HTuple(hv_BBoxCol1);
                    hv_BBoxCol2New.Dispose();
                    hv_BBoxCol2New = new HTuple(hv_BBoxCol2);
                    hv_BBoxRow1New.Dispose();
                    hv_BBoxRow1New = new HTuple(hv_BBoxRow1);
                    hv_BBoxRow2New.Dispose();
                    hv_BBoxRow2New = new HTuple(hv_BBoxRow2);
                    hv_BBoxLabelNew.Dispose();
                    hv_BBoxLabelNew = new HTuple(hv_BBoxLabel);
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Rescale the bounding boxes.
                //
                //Get required images width and height.
                //
                //Only rescale bounding boxes if the required image dimensions are not the raw dimensions.
                if ((int)((new HTuple(hv_ImageHeight.TupleNotEqual(hv_HeightRaw))).TupleOr(
                    new HTuple(hv_ImageWidth.TupleNotEqual(hv_WidthRaw)))) != 0)
                {
                    //Calculate rescaling factor.
                    hv_FactorResampleWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleWidth = (hv_ImageWidth.TupleReal()
                            ) / hv_WidthRaw;
                    }
                    hv_FactorResampleHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleHeight = (hv_ImageHeight.TupleReal()
                            ) / hv_HeightRaw;
                    }
                    //Rescale the bounding box coordinates.
                    //As we use XLD-coordinates we temporarily move the boxes by (.5,.5) for rescaling.
                    //Doing so, the center of the XLD-coordinate system (-0.5,-0.5) is used
                    //for scaling, hence the scaling is performed w.r.t. the pixel coordinate system.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol1New = ((hv_BBoxCol1New + .5) * hv_FactorResampleWidth) - .5;
                            hv_BBoxCol1New.Dispose();
                            hv_BBoxCol1New = ExpTmpLocalVar_BBoxCol1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol2New = ((hv_BBoxCol2New + .5) * hv_FactorResampleWidth) - .5;
                            hv_BBoxCol2New.Dispose();
                            hv_BBoxCol2New = ExpTmpLocalVar_BBoxCol2New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow1New = ((hv_BBoxRow1New + .5) * hv_FactorResampleHeight) - .5;
                            hv_BBoxRow1New.Dispose();
                            hv_BBoxRow1New = ExpTmpLocalVar_BBoxRow1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow2New = ((hv_BBoxRow2New + .5) * hv_FactorResampleHeight) - .5;
                            hv_BBoxRow2New.Dispose();
                            hv_BBoxRow2New = ExpTmpLocalVar_BBoxRow2New;
                        }
                    }
                    //
                }
                //
                //Make a final check and remove bounding boxes that have zero area.
                if ((int)(new HTuple((new HTuple(hv_BBoxRow1New.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    hv_MaskDelete.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskDelete = ((hv_BBoxRow1New.TupleGreaterEqualElem(
                            hv_BBoxRow2New))).TupleOr(hv_BBoxCol1New.TupleGreaterEqualElem(hv_BBoxCol2New));
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol1New = hv_BBoxCol1New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxCol1New.Dispose();
                            hv_BBoxCol1New = ExpTmpLocalVar_BBoxCol1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxCol2New = hv_BBoxCol2New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxCol2New.Dispose();
                            hv_BBoxCol2New = ExpTmpLocalVar_BBoxCol2New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow1New = hv_BBoxRow1New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxRow1New.Dispose();
                            hv_BBoxRow1New = ExpTmpLocalVar_BBoxRow1New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxRow2New = hv_BBoxRow2New.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxRow2New.Dispose();
                            hv_BBoxRow2New = ExpTmpLocalVar_BBoxRow2New;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_BBoxLabelNew = hv_BBoxLabelNew.TupleSelectMask(
                                1 - hv_MaskDelete);
                            hv_BBoxLabelNew.Dispose();
                            hv_BBoxLabelNew = ExpTmpLocalVar_BBoxLabelNew;
                        }
                    }
                    //
                    //If we remove/select bounding boxes we also need to filter the corresponding
                    //instance segmentation masks if they exist.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        filter_dl_sample_instance_segmentation_masks(hv_DLSample, 1 - hv_MaskDelete);
                    }
                }
                //
                //Set new bounding box coordinates in the dictionary.
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col1", hv_BBoxCol1New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col2", hv_BBoxCol2New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row1", hv_BBoxRow1New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row2", hv_BBoxRow2New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_label_id", hv_BBoxLabelNew);
                //
                ho_DomainRaw.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_DomainRaw.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_Row2.Dispose();
                hv_Col2.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection
        // Short Description: This procedure preprocesses the bounding boxes of type 'rectangle2' for a given sample. 
        private void preprocess_dl_model_bbox_rect2(HObject ho_ImageRaw, HTuple hv_DLSample,
            HTuple hv_DLPreprocessParam)
        {




            // Local iconic variables 

            HObject ho_DomainRaw = null, ho_Rectangle2XLD = null;
            HObject ho_Rectangle2XLDSheared = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_IgnoreDirection = new HTuple();
            HTuple hv_ClassIDsNoOrientation = new HTuple(), hv_KeyExists = new HTuple();
            HTuple hv_BBoxRow = new HTuple(), hv_BBoxCol = new HTuple();
            HTuple hv_BBoxLength1 = new HTuple(), hv_BBoxLength2 = new HTuple();
            HTuple hv_BBoxPhi = new HTuple(), hv_BBoxLabel = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_ImageId = new HTuple();
            HTuple hv_ExceptionMessage = new HTuple(), hv_BoxesInvalid = new HTuple();
            HTuple hv_DomainRow1 = new HTuple(), hv_DomainColumn1 = new HTuple();
            HTuple hv_DomainRow2 = new HTuple(), hv_DomainColumn2 = new HTuple();
            HTuple hv_WidthRaw = new HTuple(), hv_HeightRaw = new HTuple();
            HTuple hv_MaskDelete = new HTuple(), hv_MaskNewBbox = new HTuple();
            HTuple hv_BBoxRowNew = new HTuple(), hv_BBoxColNew = new HTuple();
            HTuple hv_BBoxLength1New = new HTuple(), hv_BBoxLength2New = new HTuple();
            HTuple hv_BBoxPhiNew = new HTuple(), hv_BBoxLabelNew = new HTuple();
            HTuple hv_ClassIDsNoOrientationIndices = new HTuple();
            HTuple hv_Index = new HTuple(), hv_ClassIDsNoOrientationIndicesTmp = new HTuple();
            HTuple hv_DirectionLength1Row = new HTuple(), hv_DirectionLength1Col = new HTuple();
            HTuple hv_DirectionLength2Row = new HTuple(), hv_DirectionLength2Col = new HTuple();
            HTuple hv_Corner1Row = new HTuple(), hv_Corner1Col = new HTuple();
            HTuple hv_Corner2Row = new HTuple(), hv_Corner2Col = new HTuple();
            HTuple hv_FactorResampleWidth = new HTuple(), hv_FactorResampleHeight = new HTuple();
            HTuple hv_BBoxRow1 = new HTuple(), hv_BBoxCol1 = new HTuple();
            HTuple hv_BBoxRow2 = new HTuple(), hv_BBoxCol2 = new HTuple();
            HTuple hv_BBoxRow3 = new HTuple(), hv_BBoxCol3 = new HTuple();
            HTuple hv_BBoxRow4 = new HTuple(), hv_BBoxCol4 = new HTuple();
            HTuple hv_BBoxCol1New = new HTuple(), hv_BBoxCol2New = new HTuple();
            HTuple hv_BBoxCol3New = new HTuple(), hv_BBoxCol4New = new HTuple();
            HTuple hv_BBoxRow1New = new HTuple(), hv_BBoxRow2New = new HTuple();
            HTuple hv_BBoxRow3New = new HTuple(), hv_BBoxRow4New = new HTuple();
            HTuple hv_HomMat2DIdentity = new HTuple(), hv_HomMat2DScale = new HTuple();
            HTuple hv__ = new HTuple(), hv_BBoxPhiTmp = new HTuple();
            HTuple hv_PhiDelta = new HTuple(), hv_PhiDeltaNegativeIndices = new HTuple();
            HTuple hv_IndicesRot90 = new HTuple(), hv_IndicesRot180 = new HTuple();
            HTuple hv_IndicesRot270 = new HTuple(), hv_SwapIndices = new HTuple();
            HTuple hv_Tmp = new HTuple(), hv_BBoxPhiNewIndices = new HTuple();
            HTuple hv_PhiThreshold = new HTuple(), hv_PhiToCorrect = new HTuple();
            HTuple hv_NumCorrections = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_DomainRaw);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2XLD);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2XLDSheared);
            try
            {
                //This procedure preprocesses the bounding boxes of type 'rectangle2' for a given sample.
                //
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get preprocess parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //The keys 'ignore_direction' and 'class_ids_no_orientation' are optional.
                hv_IgnoreDirection.Dispose();
                hv_IgnoreDirection = 0;
                hv_ClassIDsNoOrientation.Dispose();
                hv_ClassIDsNoOrientation = new HTuple();
                hv_KeyExists.Dispose();
                HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", (new HTuple("ignore_direction")).TupleConcat(
                    "class_ids_no_orientation"), out hv_KeyExists);
                if ((int)(hv_KeyExists.TupleSelect(0)) != 0)
                {
                    hv_IgnoreDirection.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_direction", out hv_IgnoreDirection);
                    if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("true"))) != 0)
                    {
                        hv_IgnoreDirection.Dispose();
                        hv_IgnoreDirection = 1;
                    }
                    else if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("false"))) != 0)
                    {
                        hv_IgnoreDirection.Dispose();
                        hv_IgnoreDirection = 0;
                    }
                }
                if ((int)(hv_KeyExists.TupleSelect(1)) != 0)
                {
                    hv_ClassIDsNoOrientation.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_no_orientation",
                        out hv_ClassIDsNoOrientation);
                }
                //
                //Get bounding box coordinates and labels.
                try
                {
                    hv_BBoxRow.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_row", out hv_BBoxRow);
                    hv_BBoxCol.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_col", out hv_BBoxCol);
                    hv_BBoxLength1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_length1", out hv_BBoxLength1);
                    hv_BBoxLength2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_length2", out hv_BBoxLength2);
                    hv_BBoxPhi.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_phi", out hv_BBoxPhi);
                    hv_BBoxLabel.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "bbox_label_id", out hv_BBoxLabel);
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                    hv_ImageId.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                    if ((int)(new HTuple(((hv_Exception.TupleSelect(0))).TupleEqual(1302))) != 0)
                    {
                        hv_ExceptionMessage.Dispose();
                        hv_ExceptionMessage = "A bounding box coordinate key is missing.";
                    }
                    else
                    {
                        hv_ExceptionMessage.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_ExceptionMessage = hv_Exception.TupleSelect(
                                2);
                        }
                    }
                    throw new HalconException((("An error has occurred during preprocessing image_id " + hv_ImageId) + " when getting bounding box coordinates : ") + hv_ExceptionMessage);
                }
                //
                //Check that there are no invalid boxes.
                if ((int)(new HTuple((new HTuple(hv_BBoxRow.TupleLength())).TupleGreater(0))) != 0)
                {
                    hv_BoxesInvalid.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BoxesInvalid = (((hv_BBoxLength1.TupleEqualElem(
                            0))).TupleSum()) + (((hv_BBoxLength2.TupleEqualElem(0))).TupleSum());
                    }
                    if ((int)(new HTuple(hv_BoxesInvalid.TupleGreater(0))) != 0)
                    {
                        hv_ImageId.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                        throw new HalconException(("An error has occurred during preprocessing image_id " + hv_ImageId) + new HTuple(": Sample contains at least one bounding box with zero-area, i.e. bbox_length1 == 0 or bbox_length2 == 0!"));
                    }
                }
                else
                {
                    //There are no bounding boxes, hence nothing to do.
                    ho_DomainRaw.Dispose();
                    ho_Rectangle2XLD.Dispose();
                    ho_Rectangle2XLDSheared.Dispose();

                    hv_ImageWidth.Dispose();
                    hv_ImageHeight.Dispose();
                    hv_DomainHandling.Dispose();
                    hv_IgnoreDirection.Dispose();
                    hv_ClassIDsNoOrientation.Dispose();
                    hv_KeyExists.Dispose();
                    hv_BBoxRow.Dispose();
                    hv_BBoxCol.Dispose();
                    hv_BBoxLength1.Dispose();
                    hv_BBoxLength2.Dispose();
                    hv_BBoxPhi.Dispose();
                    hv_BBoxLabel.Dispose();
                    hv_Exception.Dispose();
                    hv_ImageId.Dispose();
                    hv_ExceptionMessage.Dispose();
                    hv_BoxesInvalid.Dispose();
                    hv_DomainRow1.Dispose();
                    hv_DomainColumn1.Dispose();
                    hv_DomainRow2.Dispose();
                    hv_DomainColumn2.Dispose();
                    hv_WidthRaw.Dispose();
                    hv_HeightRaw.Dispose();
                    hv_MaskDelete.Dispose();
                    hv_MaskNewBbox.Dispose();
                    hv_BBoxRowNew.Dispose();
                    hv_BBoxColNew.Dispose();
                    hv_BBoxLength1New.Dispose();
                    hv_BBoxLength2New.Dispose();
                    hv_BBoxPhiNew.Dispose();
                    hv_BBoxLabelNew.Dispose();
                    hv_ClassIDsNoOrientationIndices.Dispose();
                    hv_Index.Dispose();
                    hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                    hv_DirectionLength1Row.Dispose();
                    hv_DirectionLength1Col.Dispose();
                    hv_DirectionLength2Row.Dispose();
                    hv_DirectionLength2Col.Dispose();
                    hv_Corner1Row.Dispose();
                    hv_Corner1Col.Dispose();
                    hv_Corner2Row.Dispose();
                    hv_Corner2Col.Dispose();
                    hv_FactorResampleWidth.Dispose();
                    hv_FactorResampleHeight.Dispose();
                    hv_BBoxRow1.Dispose();
                    hv_BBoxCol1.Dispose();
                    hv_BBoxRow2.Dispose();
                    hv_BBoxCol2.Dispose();
                    hv_BBoxRow3.Dispose();
                    hv_BBoxCol3.Dispose();
                    hv_BBoxRow4.Dispose();
                    hv_BBoxCol4.Dispose();
                    hv_BBoxCol1New.Dispose();
                    hv_BBoxCol2New.Dispose();
                    hv_BBoxCol3New.Dispose();
                    hv_BBoxCol4New.Dispose();
                    hv_BBoxRow1New.Dispose();
                    hv_BBoxRow2New.Dispose();
                    hv_BBoxRow3New.Dispose();
                    hv_BBoxRow4New.Dispose();
                    hv_HomMat2DIdentity.Dispose();
                    hv_HomMat2DScale.Dispose();
                    hv__.Dispose();
                    hv_BBoxPhiTmp.Dispose();
                    hv_PhiDelta.Dispose();
                    hv_PhiDeltaNegativeIndices.Dispose();
                    hv_IndicesRot90.Dispose();
                    hv_IndicesRot180.Dispose();
                    hv_IndicesRot270.Dispose();
                    hv_SwapIndices.Dispose();
                    hv_Tmp.Dispose();
                    hv_BBoxPhiNewIndices.Dispose();
                    hv_PhiThreshold.Dispose();
                    hv_PhiToCorrect.Dispose();
                    hv_NumCorrections.Dispose();

                    return;
                }
                //
                //If the domain is cropped, crop bounding boxes.
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //
                    //Get domain.
                    ho_DomainRaw.Dispose();
                    HOperatorSet.GetDomain(ho_ImageRaw, out ho_DomainRaw);
                    //
                    //Set the size of the raw image to the domain extensions.
                    hv_DomainRow1.Dispose(); hv_DomainColumn1.Dispose(); hv_DomainRow2.Dispose(); hv_DomainColumn2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_DomainRaw, out hv_DomainRow1, out hv_DomainColumn1,
                        out hv_DomainRow2, out hv_DomainColumn2);
                    hv_WidthRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WidthRaw = (hv_DomainColumn2 - hv_DomainColumn1) + 1;
                    }
                    hv_HeightRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HeightRaw = (hv_DomainRow2 - hv_DomainRow1) + 1;
                    }
                    //
                    //Crop the bounding boxes.
                    //Remove the boxes with center outside of the domain.
                    hv_MaskDelete.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskDelete = (new HTuple((new HTuple(((hv_BBoxRow.TupleLessElem(
                            hv_DomainRow1))).TupleOr(hv_BBoxCol.TupleLessElem(hv_DomainColumn1)))).TupleOr(
                            hv_BBoxRow.TupleGreaterElem(hv_DomainRow2)))).TupleOr(hv_BBoxCol.TupleGreaterElem(
                            hv_DomainColumn2));
                    }
                    hv_MaskNewBbox.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_MaskNewBbox = 1 - hv_MaskDelete;
                    }
                    //Store the preprocessed bounding box entries.
                    hv_BBoxRowNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxRowNew = (hv_BBoxRow.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainRow1;
                    }
                    hv_BBoxColNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxColNew = (hv_BBoxCol.TupleSelectMask(
                            hv_MaskNewBbox)) - hv_DomainColumn1;
                    }
                    hv_BBoxLength1New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLength1New = hv_BBoxLength1.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    hv_BBoxLength2New.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLength2New = hv_BBoxLength2.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    hv_BBoxPhiNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxPhiNew = hv_BBoxPhi.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    hv_BBoxLabelNew.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BBoxLabelNew = hv_BBoxLabel.TupleSelectMask(
                            hv_MaskNewBbox);
                    }
                    //
                    //If we remove/select bounding boxes we also need to filter the corresponding
                    //instance segmentation masks if they exist.
                    filter_dl_sample_instance_segmentation_masks(hv_DLSample, hv_MaskNewBbox);
                    //
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    //If the entire image is used, set the variables accordingly.
                    //Get the original size.
                    hv_WidthRaw.Dispose(); hv_HeightRaw.Dispose();
                    HOperatorSet.GetImageSize(ho_ImageRaw, out hv_WidthRaw, out hv_HeightRaw);
                    //Set new coordinates to input coordinates.
                    hv_BBoxRowNew.Dispose();
                    hv_BBoxRowNew = new HTuple(hv_BBoxRow);
                    hv_BBoxColNew.Dispose();
                    hv_BBoxColNew = new HTuple(hv_BBoxCol);
                    hv_BBoxLength1New.Dispose();
                    hv_BBoxLength1New = new HTuple(hv_BBoxLength1);
                    hv_BBoxLength2New.Dispose();
                    hv_BBoxLength2New = new HTuple(hv_BBoxLength2);
                    hv_BBoxPhiNew.Dispose();
                    hv_BBoxPhiNew = new HTuple(hv_BBoxPhi);
                    hv_BBoxLabelNew.Dispose();
                    hv_BBoxLabelNew = new HTuple(hv_BBoxLabel);
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Generate smallest enclosing axis-aligned bounding box for classes in ClassIDsNoOrientation.
                hv_ClassIDsNoOrientationIndices.Dispose();
                hv_ClassIDsNoOrientationIndices = new HTuple();
                for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_ClassIDsNoOrientation.TupleLength()
                    )) - 1); hv_Index = (int)hv_Index + 1)
                {
                    hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ClassIDsNoOrientationIndicesTmp = ((hv_BBoxLabelNew.TupleEqualElem(
                            hv_ClassIDsNoOrientation.TupleSelect(hv_Index)))).TupleFind(1);
                    }
                    if ((int)(new HTuple(hv_ClassIDsNoOrientationIndicesTmp.TupleNotEqual(-1))) != 0)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_ClassIDsNoOrientationIndices = hv_ClassIDsNoOrientationIndices.TupleConcat(
                                    hv_ClassIDsNoOrientationIndicesTmp);
                                hv_ClassIDsNoOrientationIndices.Dispose();
                                hv_ClassIDsNoOrientationIndices = ExpTmpLocalVar_ClassIDsNoOrientationIndices;
                            }
                        }
                    }
                }
                if ((int)(new HTuple((new HTuple(hv_ClassIDsNoOrientationIndices.TupleLength()
                    )).TupleGreater(0))) != 0)
                {
                    //Calculate length1 and length2 using position of corners.
                    hv_DirectionLength1Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DirectionLength1Row = -(((hv_BBoxPhiNew.TupleSelect(
                            hv_ClassIDsNoOrientationIndices))).TupleSin());
                    }
                    hv_DirectionLength1Col.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DirectionLength1Col = ((hv_BBoxPhiNew.TupleSelect(
                            hv_ClassIDsNoOrientationIndices))).TupleCos();
                    }
                    hv_DirectionLength2Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DirectionLength2Row = -hv_DirectionLength1Col;
                    }
                    hv_DirectionLength2Col.Dispose();
                    hv_DirectionLength2Col = new HTuple(hv_DirectionLength1Row);
                    hv_Corner1Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner1Row = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Row) + ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Row);
                    }
                    hv_Corner1Col.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner1Col = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Col) + ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Col);
                    }
                    hv_Corner2Row.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner2Row = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Row) - ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Row);
                    }
                    hv_Corner2Col.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Corner2Col = ((hv_BBoxLength1New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength1Col) - ((hv_BBoxLength2New.TupleSelect(
                            hv_ClassIDsNoOrientationIndices)) * hv_DirectionLength2Col);
                    }
                    //
                    if (hv_BBoxPhiNew == null)
                        hv_BBoxPhiNew = new HTuple();
                    hv_BBoxPhiNew[hv_ClassIDsNoOrientationIndices] = 0.0;
                    if (hv_BBoxLength1New == null)
                        hv_BBoxLength1New = new HTuple();
                    hv_BBoxLength1New[hv_ClassIDsNoOrientationIndices] = ((hv_Corner1Col.TupleAbs()
                        )).TupleMax2(hv_Corner2Col.TupleAbs());
                    if (hv_BBoxLength2New == null)
                        hv_BBoxLength2New = new HTuple();
                    hv_BBoxLength2New[hv_ClassIDsNoOrientationIndices] = ((hv_Corner1Row.TupleAbs()
                        )).TupleMax2(hv_Corner2Row.TupleAbs());
                }
                //
                //Rescale bounding boxes.
                //
                //Get required images width and height.
                //
                //Only rescale bounding boxes if the required image dimensions are not the raw dimensions.
                if ((int)((new HTuple(hv_ImageHeight.TupleNotEqual(hv_HeightRaw))).TupleOr(
                    new HTuple(hv_ImageWidth.TupleNotEqual(hv_WidthRaw)))) != 0)
                {
                    //Calculate rescaling factor.
                    hv_FactorResampleWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleWidth = (hv_ImageWidth.TupleReal()
                            ) / hv_WidthRaw;
                    }
                    hv_FactorResampleHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleHeight = (hv_ImageHeight.TupleReal()
                            ) / hv_HeightRaw;
                    }
                    if ((int)((new HTuple(hv_FactorResampleHeight.TupleNotEqual(hv_FactorResampleWidth))).TupleAnd(
                        new HTuple((new HTuple(hv_BBoxRowNew.TupleLength())).TupleGreater(0)))) != 0)
                    {
                        //In order to preserve the correct orientation we have to transform the points individually.
                        //Get the coordinates of the four corner points.
                        hv_BBoxRow1.Dispose(); hv_BBoxCol1.Dispose(); hv_BBoxRow2.Dispose(); hv_BBoxCol2.Dispose(); hv_BBoxRow3.Dispose(); hv_BBoxCol3.Dispose(); hv_BBoxRow4.Dispose(); hv_BBoxCol4.Dispose();
                        convert_rect2_5to8param(hv_BBoxRowNew, hv_BBoxColNew, hv_BBoxLength1New,
                            hv_BBoxLength2New, hv_BBoxPhiNew, out hv_BBoxRow1, out hv_BBoxCol1,
                            out hv_BBoxRow2, out hv_BBoxCol2, out hv_BBoxRow3, out hv_BBoxCol3,
                            out hv_BBoxRow4, out hv_BBoxCol4);
                        //
                        //Rescale the coordinates.
                        hv_BBoxCol1New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol1New = hv_BBoxCol1 * hv_FactorResampleWidth;
                        }
                        hv_BBoxCol2New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol2New = hv_BBoxCol2 * hv_FactorResampleWidth;
                        }
                        hv_BBoxCol3New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol3New = hv_BBoxCol3 * hv_FactorResampleWidth;
                        }
                        hv_BBoxCol4New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxCol4New = hv_BBoxCol4 * hv_FactorResampleWidth;
                        }
                        hv_BBoxRow1New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow1New = hv_BBoxRow1 * hv_FactorResampleHeight;
                        }
                        hv_BBoxRow2New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow2New = hv_BBoxRow2 * hv_FactorResampleHeight;
                        }
                        hv_BBoxRow3New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow3New = hv_BBoxRow3 * hv_FactorResampleHeight;
                        }
                        hv_BBoxRow4New.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxRow4New = hv_BBoxRow4 * hv_FactorResampleHeight;
                        }
                        //
                        //The rectangles will get sheared, that is why new rectangles have to be found.
                        //Generate homography to scale rectangles.
                        hv_HomMat2DIdentity.Dispose();
                        HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                        hv_HomMat2DScale.Dispose();
                        HOperatorSet.HomMat2dScale(hv_HomMat2DIdentity, hv_FactorResampleHeight,
                            hv_FactorResampleWidth, 0, 0, out hv_HomMat2DScale);
                        //Generate XLD contours for the rectangles.
                        ho_Rectangle2XLD.Dispose();
                        HOperatorSet.GenRectangle2ContourXld(out ho_Rectangle2XLD, hv_BBoxRowNew,
                            hv_BBoxColNew, hv_BBoxPhiNew, hv_BBoxLength1New, hv_BBoxLength2New);
                        //Scale the XLD contours --> results in sheared regions.
                        ho_Rectangle2XLDSheared.Dispose();
                        HOperatorSet.AffineTransContourXld(ho_Rectangle2XLD, out ho_Rectangle2XLDSheared,
                            hv_HomMat2DScale);
                        hv_BBoxRowNew.Dispose(); hv_BBoxColNew.Dispose(); hv_BBoxPhiNew.Dispose(); hv_BBoxLength1New.Dispose(); hv_BBoxLength2New.Dispose();
                        HOperatorSet.SmallestRectangle2Xld(ho_Rectangle2XLDSheared, out hv_BBoxRowNew,
                            out hv_BBoxColNew, out hv_BBoxPhiNew, out hv_BBoxLength1New, out hv_BBoxLength2New);
                        //
                        //smallest_rectangle2_xld might change the orientation of the bounding box.
                        //Hence, take the orientation that is closest to the one obtained out of the 4 corner points.
                        hv__.Dispose(); hv__.Dispose(); hv__.Dispose(); hv__.Dispose(); hv_BBoxPhiTmp.Dispose();
                        convert_rect2_8to5param(hv_BBoxRow1New, hv_BBoxCol1New, hv_BBoxRow2New,
                            hv_BBoxCol2New, hv_BBoxRow3New, hv_BBoxCol3New, hv_BBoxRow4New, hv_BBoxCol4New,
                            hv_IgnoreDirection, out hv__, out hv__, out hv__, out hv__, out hv_BBoxPhiTmp);
                        hv_PhiDelta.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_PhiDelta = ((hv_BBoxPhiTmp - hv_BBoxPhiNew)).TupleFmod(
                                (new HTuple(360)).TupleRad());
                        }
                        //Guarantee that angles are positive.
                        hv_PhiDeltaNegativeIndices.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_PhiDeltaNegativeIndices = ((hv_PhiDelta.TupleLessElem(
                                0.0))).TupleFind(1);
                        }
                        if ((int)(new HTuple(hv_PhiDeltaNegativeIndices.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_PhiDelta == null)
                                hv_PhiDelta = new HTuple();
                            hv_PhiDelta[hv_PhiDeltaNegativeIndices] = (hv_PhiDelta.TupleSelect(hv_PhiDeltaNegativeIndices)) + ((new HTuple(360)).TupleRad()
                                );
                        }
                        hv_IndicesRot90.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndicesRot90 = (new HTuple(((hv_PhiDelta.TupleGreaterElem(
                                (new HTuple(45)).TupleRad()))).TupleAnd(hv_PhiDelta.TupleLessEqualElem(
                                (new HTuple(135)).TupleRad())))).TupleFind(1);
                        }
                        hv_IndicesRot180.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndicesRot180 = (new HTuple(((hv_PhiDelta.TupleGreaterElem(
                                (new HTuple(135)).TupleRad()))).TupleAnd(hv_PhiDelta.TupleLessEqualElem(
                                (new HTuple(225)).TupleRad())))).TupleFind(1);
                        }
                        hv_IndicesRot270.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_IndicesRot270 = (new HTuple(((hv_PhiDelta.TupleGreaterElem(
                                (new HTuple(225)).TupleRad()))).TupleAnd(hv_PhiDelta.TupleLessEqualElem(
                                (new HTuple(315)).TupleRad())))).TupleFind(1);
                        }
                        hv_SwapIndices.Dispose();
                        hv_SwapIndices = new HTuple();
                        if ((int)(new HTuple(hv_IndicesRot90.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_IndicesRot90] = (hv_BBoxPhiNew.TupleSelect(hv_IndicesRot90)) + ((new HTuple(90)).TupleRad()
                                );
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_SwapIndices = hv_SwapIndices.TupleConcat(
                                        hv_IndicesRot90);
                                    hv_SwapIndices.Dispose();
                                    hv_SwapIndices = ExpTmpLocalVar_SwapIndices;
                                }
                            }
                        }
                        if ((int)(new HTuple(hv_IndicesRot180.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_IndicesRot180] = (hv_BBoxPhiNew.TupleSelect(hv_IndicesRot180)) + ((new HTuple(180)).TupleRad()
                                );
                        }
                        if ((int)(new HTuple(hv_IndicesRot270.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_IndicesRot270] = (hv_BBoxPhiNew.TupleSelect(hv_IndicesRot270)) + ((new HTuple(270)).TupleRad()
                                );
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_SwapIndices = hv_SwapIndices.TupleConcat(
                                        hv_IndicesRot270);
                                    hv_SwapIndices.Dispose();
                                    hv_SwapIndices = ExpTmpLocalVar_SwapIndices;
                                }
                            }
                        }
                        if ((int)(new HTuple(hv_SwapIndices.TupleNotEqual(new HTuple()))) != 0)
                        {
                            hv_Tmp.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Tmp = hv_BBoxLength1New.TupleSelect(
                                    hv_SwapIndices);
                            }
                            if (hv_BBoxLength1New == null)
                                hv_BBoxLength1New = new HTuple();
                            hv_BBoxLength1New[hv_SwapIndices] = hv_BBoxLength2New.TupleSelect(hv_SwapIndices);
                            if (hv_BBoxLength2New == null)
                                hv_BBoxLength2New = new HTuple();
                            hv_BBoxLength2New[hv_SwapIndices] = hv_Tmp;
                        }
                        //Change angles such that they lie in the range (-180°, 180°].
                        hv_BBoxPhiNewIndices.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BBoxPhiNewIndices = ((hv_BBoxPhiNew.TupleGreaterElem(
                                (new HTuple(180)).TupleRad()))).TupleFind(1);
                        }
                        if ((int)(new HTuple(hv_BBoxPhiNewIndices.TupleNotEqual(-1))) != 0)
                        {
                            if (hv_BBoxPhiNew == null)
                                hv_BBoxPhiNew = new HTuple();
                            hv_BBoxPhiNew[hv_BBoxPhiNewIndices] = (hv_BBoxPhiNew.TupleSelect(hv_BBoxPhiNewIndices)) - ((new HTuple(360)).TupleRad()
                                );
                        }
                        //
                    }
                    else
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxColNew = hv_BBoxColNew * hv_FactorResampleWidth;
                                hv_BBoxColNew.Dispose();
                                hv_BBoxColNew = ExpTmpLocalVar_BBoxColNew;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxRowNew = hv_BBoxRowNew * hv_FactorResampleWidth;
                                hv_BBoxRowNew.Dispose();
                                hv_BBoxRowNew = ExpTmpLocalVar_BBoxRowNew;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxLength1New = hv_BBoxLength1New * hv_FactorResampleWidth;
                                hv_BBoxLength1New.Dispose();
                                hv_BBoxLength1New = ExpTmpLocalVar_BBoxLength1New;
                            }
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_BBoxLength2New = hv_BBoxLength2New * hv_FactorResampleWidth;
                                hv_BBoxLength2New.Dispose();
                                hv_BBoxLength2New = ExpTmpLocalVar_BBoxLength2New;
                            }
                        }
                        //Phi stays the same.
                    }
                    //
                }
                //
                //Adapt the bounding box angles such that they are within the correct range,
                //which is (-180°,180°] for 'ignore_direction'==false and (-90°,90°] else.
                hv_PhiThreshold.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PhiThreshold = ((new HTuple(180)).TupleRad()
                        ) - (hv_IgnoreDirection * ((new HTuple(90)).TupleRad()));
                }
                hv_PhiDelta.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PhiDelta = 2 * hv_PhiThreshold;
                }
                //Correct angles that are too large.
                hv_PhiToCorrect.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PhiToCorrect = ((hv_BBoxPhiNew.TupleGreaterElem(
                        hv_PhiThreshold))).TupleFind(1);
                }
                if ((int)((new HTuple(hv_PhiToCorrect.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_PhiToCorrect.TupleNotEqual(
                    new HTuple())))) != 0)
                {
                    hv_NumCorrections.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumCorrections = (((((hv_BBoxPhiNew.TupleSelect(
                            hv_PhiToCorrect)) - hv_PhiThreshold) / hv_PhiDelta)).TupleInt()) + 1;
                    }
                    if (hv_BBoxPhiNew == null)
                        hv_BBoxPhiNew = new HTuple();
                    hv_BBoxPhiNew[hv_PhiToCorrect] = (hv_BBoxPhiNew.TupleSelect(hv_PhiToCorrect)) - (hv_NumCorrections * hv_PhiDelta);
                }
                //Correct angles that are too small.
                hv_PhiToCorrect.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_PhiToCorrect = ((hv_BBoxPhiNew.TupleLessEqualElem(
                        -hv_PhiThreshold))).TupleFind(1);
                }
                if ((int)((new HTuple(hv_PhiToCorrect.TupleNotEqual(-1))).TupleAnd(new HTuple(hv_PhiToCorrect.TupleNotEqual(
                    new HTuple())))) != 0)
                {
                    hv_NumCorrections.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumCorrections = (((((((hv_BBoxPhiNew.TupleSelect(
                            hv_PhiToCorrect)) + hv_PhiThreshold)).TupleAbs()) / hv_PhiDelta)).TupleInt()
                            ) + 1;
                    }
                    if (hv_BBoxPhiNew == null)
                        hv_BBoxPhiNew = new HTuple();
                    hv_BBoxPhiNew[hv_PhiToCorrect] = (hv_BBoxPhiNew.TupleSelect(hv_PhiToCorrect)) + (hv_NumCorrections * hv_PhiDelta);
                }
                //
                //Check that there are no invalid boxes.
                if ((int)(new HTuple((new HTuple(hv_BBoxRowNew.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    hv_BoxesInvalid.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_BoxesInvalid = (((hv_BBoxLength1New.TupleEqualElem(
                            0))).TupleSum()) + (((hv_BBoxLength2New.TupleEqualElem(0))).TupleSum());
                    }
                    if ((int)(new HTuple(hv_BoxesInvalid.TupleGreater(0))) != 0)
                    {
                        hv_ImageId.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLSample, "image_id", out hv_ImageId);
                        throw new HalconException(("An error has occurred during preprocessing image_id " + hv_ImageId) + new HTuple(": Sample contains at least one box with zero-area, i.e. bbox_length1 == 0 or bbox_length2 == 0!"));
                    }
                }
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_row", hv_BBoxRowNew);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_col", hv_BBoxColNew);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_length1", hv_BBoxLength1New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_length2", hv_BBoxLength2New);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_phi", hv_BBoxPhiNew);
                HOperatorSet.SetDictTuple(hv_DLSample, "bbox_label_id", hv_BBoxLabelNew);
                //
                ho_DomainRaw.Dispose();
                ho_Rectangle2XLD.Dispose();
                ho_Rectangle2XLDSheared.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_KeyExists.Dispose();
                hv_BBoxRow.Dispose();
                hv_BBoxCol.Dispose();
                hv_BBoxLength1.Dispose();
                hv_BBoxLength2.Dispose();
                hv_BBoxPhi.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxRowNew.Dispose();
                hv_BBoxColNew.Dispose();
                hv_BBoxLength1New.Dispose();
                hv_BBoxLength2New.Dispose();
                hv_BBoxPhiNew.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_ClassIDsNoOrientationIndices.Dispose();
                hv_Index.Dispose();
                hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                hv_DirectionLength1Row.Dispose();
                hv_DirectionLength1Col.Dispose();
                hv_DirectionLength2Row.Dispose();
                hv_DirectionLength2Col.Dispose();
                hv_Corner1Row.Dispose();
                hv_Corner1Col.Dispose();
                hv_Corner2Row.Dispose();
                hv_Corner2Col.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow3.Dispose();
                hv_BBoxCol3.Dispose();
                hv_BBoxRow4.Dispose();
                hv_BBoxCol4.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxCol3New.Dispose();
                hv_BBoxCol4New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxRow3New.Dispose();
                hv_BBoxRow4New.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DScale.Dispose();
                hv__.Dispose();
                hv_BBoxPhiTmp.Dispose();
                hv_PhiDelta.Dispose();
                hv_PhiDeltaNegativeIndices.Dispose();
                hv_IndicesRot90.Dispose();
                hv_IndicesRot180.Dispose();
                hv_IndicesRot270.Dispose();
                hv_SwapIndices.Dispose();
                hv_Tmp.Dispose();
                hv_BBoxPhiNewIndices.Dispose();
                hv_PhiThreshold.Dispose();
                hv_PhiToCorrect.Dispose();
                hv_NumCorrections.Dispose();

                return;

            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_DomainRaw.Dispose();
                ho_Rectangle2XLD.Dispose();
                ho_Rectangle2XLDSheared.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();
                hv_KeyExists.Dispose();
                hv_BBoxRow.Dispose();
                hv_BBoxCol.Dispose();
                hv_BBoxLength1.Dispose();
                hv_BBoxLength2.Dispose();
                hv_BBoxPhi.Dispose();
                hv_BBoxLabel.Dispose();
                hv_Exception.Dispose();
                hv_ImageId.Dispose();
                hv_ExceptionMessage.Dispose();
                hv_BoxesInvalid.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_MaskDelete.Dispose();
                hv_MaskNewBbox.Dispose();
                hv_BBoxRowNew.Dispose();
                hv_BBoxColNew.Dispose();
                hv_BBoxLength1New.Dispose();
                hv_BBoxLength2New.Dispose();
                hv_BBoxPhiNew.Dispose();
                hv_BBoxLabelNew.Dispose();
                hv_ClassIDsNoOrientationIndices.Dispose();
                hv_Index.Dispose();
                hv_ClassIDsNoOrientationIndicesTmp.Dispose();
                hv_DirectionLength1Row.Dispose();
                hv_DirectionLength1Col.Dispose();
                hv_DirectionLength2Row.Dispose();
                hv_DirectionLength2Col.Dispose();
                hv_Corner1Row.Dispose();
                hv_Corner1Col.Dispose();
                hv_Corner2Row.Dispose();
                hv_Corner2Col.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();
                hv_BBoxRow1.Dispose();
                hv_BBoxCol1.Dispose();
                hv_BBoxRow2.Dispose();
                hv_BBoxCol2.Dispose();
                hv_BBoxRow3.Dispose();
                hv_BBoxCol3.Dispose();
                hv_BBoxRow4.Dispose();
                hv_BBoxCol4.Dispose();
                hv_BBoxCol1New.Dispose();
                hv_BBoxCol2New.Dispose();
                hv_BBoxCol3New.Dispose();
                hv_BBoxCol4New.Dispose();
                hv_BBoxRow1New.Dispose();
                hv_BBoxRow2New.Dispose();
                hv_BBoxRow3New.Dispose();
                hv_BBoxRow4New.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DScale.Dispose();
                hv__.Dispose();
                hv_BBoxPhiTmp.Dispose();
                hv_PhiDelta.Dispose();
                hv_PhiDeltaNegativeIndices.Dispose();
                hv_IndicesRot90.Dispose();
                hv_IndicesRot180.Dispose();
                hv_IndicesRot270.Dispose();
                hv_SwapIndices.Dispose();
                hv_Tmp.Dispose();
                hv_BBoxPhiNewIndices.Dispose();
                hv_PhiThreshold.Dispose();
                hv_PhiToCorrect.Dispose();
                hv_NumCorrections.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess images for deep-learning-based training and inference. 
        private void preprocess_dl_model_images(HObject ho_Images, out HObject ho_ImagesPreprocessed,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImagesScaled = null, ho_ImageSelected = null;
            HObject ho_ImageScaled = null, ho_Channel = null, ho_ChannelScaled = null;
            HObject ho_ThreeChannelImage = null, ho_SingleChannelImage = null;

            // Local copy input parameter variables 
            HObject ho_Images_COPY_INP_TMP;
            ho_Images_COPY_INP_TMP = new HObject(ho_Images);



            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_ImageNumChannels = new HTuple(), hv_ImageRangeMin = new HTuple();
            HTuple hv_ImageRangeMax = new HTuple(), hv_DomainHandling = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_ModelType = new HTuple();
            HTuple hv_NumImages = new HTuple(), hv_Type = new HTuple();
            HTuple hv_NumMatches = new HTuple(), hv_InputNumChannels = new HTuple();
            HTuple hv_OutputNumChannels = new HTuple(), hv_NumChannels1 = new HTuple();
            HTuple hv_NumChannels3 = new HTuple(), hv_AreInputNumChannels1 = new HTuple();
            HTuple hv_AreInputNumChannels3 = new HTuple(), hv_AreInputNumChannels1Or3 = new HTuple();
            HTuple hv_ValidNumChannels = new HTuple(), hv_ValidNumChannelsText = new HTuple();
            HTuple hv_ImageIndex = new HTuple(), hv_NumChannels = new HTuple();
            HTuple hv_ChannelIndex = new HTuple(), hv_Min = new HTuple();
            HTuple hv_Max = new HTuple(), hv_Range = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Shift = new HTuple();
            HTuple hv_MeanValues = new HTuple(), hv_DeviationValues = new HTuple();
            HTuple hv_UseDefaultNormalizationValues = new HTuple();
            HTuple hv_Exception = new HTuple(), hv_Indices = new HTuple();
            HTuple hv_RescaleRange = new HTuple(), hv_CurrentNumChannels = new HTuple();
            HTuple hv_DiffNumChannelsIndices = new HTuple(), hv_Index = new HTuple();
            HTuple hv_DiffNumChannelsIndex = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImagesPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageSelected);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_Channel);
            HOperatorSet.GenEmptyObj(out ho_ChannelScaled);
            HOperatorSet.GenEmptyObj(out ho_ThreeChannelImage);
            HOperatorSet.GenEmptyObj(out ho_SingleChannelImage);
            try
            {
                //
                //This procedure preprocesses the provided Images according to the parameters in
                //the dictionary DLPreprocessParam. Note that depending on the images, additional
                //preprocessing steps might be beneficial.
                //
                //Validate the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the preprocessing parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                hv_NormalizationType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                hv_ModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_ModelType);
                //
                //Validate the type of the input images.
                hv_NumImages.Dispose();
                HOperatorSet.CountObj(ho_Images_COPY_INP_TMP, out hv_NumImages);
                if ((int)(new HTuple(hv_NumImages.TupleEqual(0))) != 0)
                {
                    throw new HalconException("Please provide some images to preprocess.");
                }
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_Images_COPY_INP_TMP, out hv_Type);
                hv_NumMatches.Dispose();
                HOperatorSet.TupleRegexpTest(hv_Type, "byte|int|real", out hv_NumMatches);
                if ((int)(new HTuple(hv_NumMatches.TupleNotEqual(hv_NumImages))) != 0)
                {
                    throw new HalconException(new HTuple("Please provide only images of type 'byte', 'int1', 'int2', 'uint2', 'int4', 'int8', or 'real'."));
                }
                //
                //Validate the number channels of the input images.
                hv_InputNumChannels.Dispose();
                HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_InputNumChannels);
                hv_OutputNumChannels.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_OutputNumChannels = HTuple.TupleGenConst(
                        hv_NumImages, hv_ImageNumChannels);
                }
                //Only for 'image_num_channels' 1 and 3 combinations of 1- and 3-channel images are allowed.
                if ((int)((new HTuple(hv_ImageNumChannels.TupleEqual(1))).TupleOr(new HTuple(hv_ImageNumChannels.TupleEqual(
                    3)))) != 0)
                {
                    hv_NumChannels1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumChannels1 = HTuple.TupleGenConst(
                            hv_NumImages, 1);
                    }
                    hv_NumChannels3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_NumChannels3 = HTuple.TupleGenConst(
                            hv_NumImages, 3);
                    }
                    hv_AreInputNumChannels1.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreInputNumChannels1 = hv_InputNumChannels.TupleEqualElem(
                            hv_NumChannels1);
                    }
                    hv_AreInputNumChannels3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreInputNumChannels3 = hv_InputNumChannels.TupleEqualElem(
                            hv_NumChannels3);
                    }
                    hv_AreInputNumChannels1Or3.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_AreInputNumChannels1Or3 = hv_AreInputNumChannels1 + hv_AreInputNumChannels3;
                    }
                    hv_ValidNumChannels.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ValidNumChannels = new HTuple(hv_AreInputNumChannels1Or3.TupleEqual(
                            hv_NumChannels1));
                    }
                    hv_ValidNumChannelsText.Dispose();
                    hv_ValidNumChannelsText = "Valid numbers of channels for the specified model are 1 or 3.";
                }
                else
                {
                    hv_ValidNumChannels.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ValidNumChannels = new HTuple(hv_InputNumChannels.TupleEqual(
                            hv_OutputNumChannels));
                    }
                    hv_ValidNumChannelsText.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ValidNumChannelsText = ("Valid number of channels for the specified model is " + hv_ImageNumChannels) + ".";
                    }
                }
                if ((int)(hv_ValidNumChannels.TupleNot()) != 0)
                {
                    throw new HalconException("Please provide images with a valid number of channels. " + hv_ValidNumChannelsText);
                }
                //Preprocess the images.
                //
                //Apply the domain to the images.
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CropDomain(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0);
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)((new HTuple(hv_DomainHandling.TupleEqual("keep_domain"))).TupleAnd(
                    new HTuple(hv_ModelType.TupleEqual("anomaly_detection")))) != 0)
                {
                    //Anomaly detection models accept the additional option 'keep_domain'.
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'.");
                }
                //
                //Convert the images to real and zoom the images.
                //Zoom first to speed up if all image types are supported by zoom_image_size.
                if ((int)(new HTuple((new HTuple(hv_Type.TupleRegexpTest("int1|int4|int8"))).TupleEqual(
                    0))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0, hv_ImageWidth,
                            hv_ImageHeight, "constant");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Images_COPY_INP_TMP, out ExpTmpOutVar_0, hv_ImageWidth,
                            hv_ImageHeight, "constant");
                        ho_Images_COPY_INP_TMP.Dispose();
                        ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                if ((int)(new HTuple(hv_NormalizationType.TupleEqual("all_channels"))) != 0)
                {
                    //Scale for each image the gray values of all channels to ImageRangeMin-ImageRangeMax.
                    ho_ImagesScaled.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                    HTuple end_val74 = hv_NumImages;
                    HTuple step_val74 = 1;
                    for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val74, step_val74); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val74))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                        hv_NumChannels.Dispose();
                        HOperatorSet.CountChannels(ho_ImageSelected, out hv_NumChannels);
                        ho_ImageScaled.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImageScaled);
                        HTuple end_val78 = hv_NumChannels;
                        HTuple step_val78 = 1;
                        for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val78, step_val78); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val78))
                        {
                            ho_Channel.Dispose();
                            HOperatorSet.AccessChannel(ho_ImageSelected, out ho_Channel, hv_ChannelIndex);
                            hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                            HOperatorSet.MinMaxGray(ho_Channel, ho_Channel, 0, out hv_Min, out hv_Max,
                                out hv_Range);
                            if ((int)(new HTuple(((hv_Max - hv_Min)).TupleEqual(0))) != 0)
                            {
                                hv_Scale.Dispose();
                                hv_Scale = 1;
                            }
                            else
                            {
                                hv_Scale.Dispose();
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    hv_Scale = (hv_ImageRangeMax - hv_ImageRangeMin) / (hv_Max - hv_Min);
                                }
                            }
                            hv_Shift.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Shift = ((-hv_Scale) * hv_Min) + hv_ImageRangeMin;
                            }
                            ho_ChannelScaled.Dispose();
                            HOperatorSet.ScaleImage(ho_Channel, out ho_ChannelScaled, hv_Scale, hv_Shift);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.AppendChannel(ho_ImageScaled, ho_ChannelScaled, out ExpTmpOutVar_0
                                    );
                                ho_ImageScaled.Dispose();
                                ho_ImageScaled = ExpTmpOutVar_0;
                            }
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ImagesScaled, ho_ImageScaled, out ExpTmpOutVar_0
                                );
                            ho_ImagesScaled.Dispose();
                            ho_ImagesScaled = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_ImagesScaled);
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("first_channel"))) != 0)
                {
                    //Scale for each image the gray values of first channel to ImageRangeMin-ImageRangeMax.
                    ho_ImagesScaled.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                    HTuple end_val96 = hv_NumImages;
                    HTuple step_val96 = 1;
                    for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val96, step_val96); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val96))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                        hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                        HOperatorSet.MinMaxGray(ho_ImageSelected, ho_ImageSelected, 0, out hv_Min,
                            out hv_Max, out hv_Range);
                        if ((int)(new HTuple(((hv_Max - hv_Min)).TupleEqual(0))) != 0)
                        {
                            hv_Scale.Dispose();
                            hv_Scale = 1;
                        }
                        else
                        {
                            hv_Scale.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Scale = (hv_ImageRangeMax - hv_ImageRangeMin) / (hv_Max - hv_Min);
                            }
                        }
                        hv_Shift.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Shift = ((-hv_Scale) * hv_Min) + hv_ImageRangeMin;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImage(ho_ImageSelected, out ExpTmpOutVar_0, hv_Scale,
                                hv_Shift);
                            ho_ImageSelected.Dispose();
                            ho_ImageSelected = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ImagesScaled, ho_ImageSelected, out ExpTmpOutVar_0
                                );
                            ho_ImagesScaled.Dispose();
                            ho_ImagesScaled = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_ImagesScaled);
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("constant_values"))) != 0)
                {
                    //Scale for each image the gray values of all channels to the corresponding channel DeviationValues[].
                    try
                    {
                        hv_MeanValues.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "mean_values_normalization",
                            out hv_MeanValues);
                        hv_DeviationValues.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "deviation_values_normalization",
                            out hv_DeviationValues);
                        hv_UseDefaultNormalizationValues.Dispose();
                        hv_UseDefaultNormalizationValues = 0;
                    }
                    // catch (Exception) 
                    catch (HalconException HDevExpDefaultException1)
                    {
                        HDevExpDefaultException1.ToHTuple(out hv_Exception);
                        hv_MeanValues.Dispose();
                        hv_MeanValues = new HTuple();
                        hv_MeanValues[0] = 123.675;
                        hv_MeanValues[1] = 116.28;
                        hv_MeanValues[2] = 103.53;
                        hv_DeviationValues.Dispose();
                        hv_DeviationValues = new HTuple();
                        hv_DeviationValues[0] = 58.395;
                        hv_DeviationValues[1] = 57.12;
                        hv_DeviationValues[2] = 57.375;
                        hv_UseDefaultNormalizationValues.Dispose();
                        hv_UseDefaultNormalizationValues = 1;
                    }
                    ho_ImagesScaled.Dispose();
                    HOperatorSet.GenEmptyObj(out ho_ImagesScaled);
                    HTuple end_val121 = hv_NumImages;
                    HTuple step_val121 = 1;
                    for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val121, step_val121); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val121))
                    {
                        ho_ImageSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                        hv_NumChannels.Dispose();
                        HOperatorSet.CountChannels(ho_ImageSelected, out hv_NumChannels);
                        //Ensure that the number of channels is equal |DeviationValues| and |MeanValues|
                        if ((int)(hv_UseDefaultNormalizationValues) != 0)
                        {
                            if ((int)(new HTuple(hv_NumChannels.TupleEqual(1))) != 0)
                            {
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.Compose3(ho_ImageSelected, ho_ImageSelected, ho_ImageSelected,
                                        out ExpTmpOutVar_0);
                                    ho_ImageSelected.Dispose();
                                    ho_ImageSelected = ExpTmpOutVar_0;
                                }
                                hv_NumChannels.Dispose();
                                HOperatorSet.CountChannels(ho_ImageSelected, out hv_NumChannels);
                            }
                            else if ((int)(new HTuple(hv_NumChannels.TupleNotEqual(
                                3))) != 0)
                            {
                                throw new HalconException("Using default values for normalization type 'constant_values' is allowed only for 1- and 3-channel images.");
                            }
                        }
                        if ((int)((new HTuple((new HTuple(hv_MeanValues.TupleLength())).TupleNotEqual(
                            hv_NumChannels))).TupleOr(new HTuple((new HTuple(hv_DeviationValues.TupleLength()
                            )).TupleNotEqual(hv_NumChannels)))) != 0)
                        {
                            throw new HalconException("The length of mean and deviation values for normalization type 'constant_values' have to be the same size as the number of channels of the image.");
                        }
                        ho_ImageScaled.Dispose();
                        HOperatorSet.GenEmptyObj(out ho_ImageScaled);
                        HTuple end_val137 = hv_NumChannels;
                        HTuple step_val137 = 1;
                        for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val137, step_val137); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val137))
                        {
                            ho_Channel.Dispose();
                            HOperatorSet.AccessChannel(ho_ImageSelected, out ho_Channel, hv_ChannelIndex);
                            hv_Scale.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Scale = 1.0 / (hv_DeviationValues.TupleSelect(
                                    hv_ChannelIndex - 1));
                            }
                            hv_Shift.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Shift = (-hv_Scale) * (hv_MeanValues.TupleSelect(
                                    hv_ChannelIndex - 1));
                            }
                            ho_ChannelScaled.Dispose();
                            HOperatorSet.ScaleImage(ho_Channel, out ho_ChannelScaled, hv_Scale, hv_Shift);
                            {
                                HObject ExpTmpOutVar_0;
                                HOperatorSet.AppendChannel(ho_ImageScaled, ho_ChannelScaled, out ExpTmpOutVar_0
                                    );
                                ho_ImageScaled.Dispose();
                                ho_ImageScaled = ExpTmpOutVar_0;
                            }
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ConcatObj(ho_ImagesScaled, ho_ImageScaled, out ExpTmpOutVar_0
                                );
                            ho_ImagesScaled.Dispose();
                            ho_ImagesScaled = ExpTmpOutVar_0;
                        }
                    }
                    ho_Images_COPY_INP_TMP.Dispose();
                    ho_Images_COPY_INP_TMP = new HObject(ho_ImagesScaled);
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("none"))) != 0)
                {
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_Type, "byte", out hv_Indices);
                    if ((int)(new HTuple(hv_Indices.TupleNotEqual(-1))) != 0)
                    {
                        //Shift the gray values from [0-255] to the expected range for byte images.
                        hv_RescaleRange.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_RescaleRange = (hv_ImageRangeMax - hv_ImageRangeMin) / 255.0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ImageSelected.Dispose();
                            HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected, hv_Indices + 1);
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ScaleImage(ho_ImageSelected, out ExpTmpOutVar_0, hv_RescaleRange,
                                hv_ImageRangeMin);
                            ho_ImageSelected.Dispose();
                            ho_ImageSelected = ExpTmpOutVar_0;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ImageSelected, out ExpTmpOutVar_0,
                                hv_Indices + 1);
                            ho_Images_COPY_INP_TMP.Dispose();
                            ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                }
                else if ((int)(new HTuple(hv_NormalizationType.TupleNotEqual("none"))) != 0)
                {
                    throw new HalconException("Unsupported parameter value for 'normalization_type'");
                }
                //
                //Ensure that the number of channels of the resulting images is consistent with the
                //number of channels of the given model. The only exceptions that are adapted below
                //are combinations of 1- and 3-channel images if ImageNumChannels is either 1 or 3.
                if ((int)((new HTuple(hv_ImageNumChannels.TupleEqual(1))).TupleOr(new HTuple(hv_ImageNumChannels.TupleEqual(
                    3)))) != 0)
                {
                    hv_CurrentNumChannels.Dispose();
                    HOperatorSet.CountChannels(ho_Images_COPY_INP_TMP, out hv_CurrentNumChannels);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_DiffNumChannelsIndices.Dispose();
                        HOperatorSet.TupleFind(hv_CurrentNumChannels.TupleNotEqualElem(hv_OutputNumChannels),
                            1, out hv_DiffNumChannelsIndices);
                    }
                    if ((int)(new HTuple(hv_DiffNumChannelsIndices.TupleNotEqual(-1))) != 0)
                    {
                        for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_DiffNumChannelsIndices.TupleLength()
                            )) - 1); hv_Index = (int)hv_Index + 1)
                        {
                            hv_DiffNumChannelsIndex.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_DiffNumChannelsIndex = hv_DiffNumChannelsIndices.TupleSelect(
                                    hv_Index);
                            }
                            hv_ImageIndex.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_ImageIndex = hv_DiffNumChannelsIndex + 1;
                            }
                            hv_NumChannels.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_NumChannels = hv_CurrentNumChannels.TupleSelect(
                                    hv_ImageIndex - 1);
                            }
                            ho_ImageSelected.Dispose();
                            HOperatorSet.SelectObj(ho_Images_COPY_INP_TMP, out ho_ImageSelected,
                                hv_ImageIndex);
                            if ((int)((new HTuple(hv_NumChannels.TupleEqual(1))).TupleAnd(new HTuple(hv_ImageNumChannels.TupleEqual(
                                3)))) != 0)
                            {
                                //Conversion from 1- to 3-channel image required
                                ho_ThreeChannelImage.Dispose();
                                HOperatorSet.Compose3(ho_ImageSelected, ho_ImageSelected, ho_ImageSelected,
                                    out ho_ThreeChannelImage);
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_ThreeChannelImage,
                                        out ExpTmpOutVar_0, hv_ImageIndex);
                                    ho_Images_COPY_INP_TMP.Dispose();
                                    ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                                }
                            }
                            else if ((int)((new HTuple(hv_NumChannels.TupleEqual(3))).TupleAnd(
                                new HTuple(hv_ImageNumChannels.TupleEqual(1)))) != 0)
                            {
                                //Conversion from 3- to 1-channel image required
                                ho_SingleChannelImage.Dispose();
                                HOperatorSet.Rgb1ToGray(ho_ImageSelected, out ho_SingleChannelImage
                                    );
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.ReplaceObj(ho_Images_COPY_INP_TMP, ho_SingleChannelImage,
                                        out ExpTmpOutVar_0, hv_ImageIndex);
                                    ho_Images_COPY_INP_TMP.Dispose();
                                    ho_Images_COPY_INP_TMP = ExpTmpOutVar_0;
                                }
                            }
                            else
                            {
                                throw new HalconException(((("Unexpected error adapting the number of channels. The number of channels of the resulting image is " + hv_NumChannels) + new HTuple(", but the number of channels of the model is ")) + hv_ImageNumChannels) + ".");
                            }
                        }
                    }
                }
                //
                //Write preprocessed images to output variable.
                ho_ImagesPreprocessed.Dispose();
                ho_ImagesPreprocessed = new HObject(ho_Images_COPY_INP_TMP);
                //
                ho_Images_COPY_INP_TMP.Dispose();
                ho_ImagesScaled.Dispose();
                ho_ImageSelected.Dispose();
                ho_ImageScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ThreeChannelImage.Dispose();
                ho_SingleChannelImage.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_InputNumChannels.Dispose();
                hv_OutputNumChannels.Dispose();
                hv_NumChannels1.Dispose();
                hv_NumChannels3.Dispose();
                hv_AreInputNumChannels1.Dispose();
                hv_AreInputNumChannels3.Dispose();
                hv_AreInputNumChannels1Or3.Dispose();
                hv_ValidNumChannels.Dispose();
                hv_ValidNumChannelsText.Dispose();
                hv_ImageIndex.Dispose();
                hv_NumChannels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();
                hv_MeanValues.Dispose();
                hv_DeviationValues.Dispose();
                hv_UseDefaultNormalizationValues.Dispose();
                hv_Exception.Dispose();
                hv_Indices.Dispose();
                hv_RescaleRange.Dispose();
                hv_CurrentNumChannels.Dispose();
                hv_DiffNumChannelsIndices.Dispose();
                hv_Index.Dispose();
                hv_DiffNumChannelsIndex.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Images_COPY_INP_TMP.Dispose();
                ho_ImagesScaled.Dispose();
                ho_ImageSelected.Dispose();
                ho_ImageScaled.Dispose();
                ho_Channel.Dispose();
                ho_ChannelScaled.Dispose();
                ho_ThreeChannelImage.Dispose();
                ho_SingleChannelImage.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_NormalizationType.Dispose();
                hv_ModelType.Dispose();
                hv_NumImages.Dispose();
                hv_Type.Dispose();
                hv_NumMatches.Dispose();
                hv_InputNumChannels.Dispose();
                hv_OutputNumChannels.Dispose();
                hv_NumChannels1.Dispose();
                hv_NumChannels3.Dispose();
                hv_AreInputNumChannels1.Dispose();
                hv_AreInputNumChannels3.Dispose();
                hv_AreInputNumChannels1Or3.Dispose();
                hv_ValidNumChannels.Dispose();
                hv_ValidNumChannelsText.Dispose();
                hv_ImageIndex.Dispose();
                hv_NumChannels.Dispose();
                hv_ChannelIndex.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();
                hv_Scale.Dispose();
                hv_Shift.Dispose();
                hv_MeanValues.Dispose();
                hv_DeviationValues.Dispose();
                hv_UseDefaultNormalizationValues.Dispose();
                hv_Exception.Dispose();
                hv_Indices.Dispose();
                hv_RescaleRange.Dispose();
                hv_CurrentNumChannels.Dispose();
                hv_DiffNumChannelsIndices.Dispose();
                hv_Index.Dispose();
                hv_DiffNumChannelsIndex.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Object Detection
        // Short Description: This procedure preprocesses the instance segmentation masks for a sample given by the dictionary DLSample. 
        private void preprocess_dl_model_instance_masks(HObject ho_ImageRaw, HTuple hv_DLSample,
            HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_InstanceMasks, ho_Domain = null;

            // Local control variables 

            HTuple hv_ImageWidth = new HTuple(), hv_ImageHeight = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_NumMasks = new HTuple();
            HTuple hv_WidthRaw = new HTuple(), hv_HeightRaw = new HTuple();
            HTuple hv_DomainRow1 = new HTuple(), hv_DomainColumn1 = new HTuple();
            HTuple hv_DomainRow2 = new HTuple(), hv_DomainColumn2 = new HTuple();
            HTuple hv_FactorResampleWidth = new HTuple(), hv_FactorResampleHeight = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_InstanceMasks);
            HOperatorSet.GenEmptyObj(out ho_Domain);
            try
            {
                //
                //This procedure preprocesses the instance masks of a DLSample.
                //
                //Check preprocess parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get relevant preprocess parameters.
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //
                //Get the preprocessed instance masks.
                ho_InstanceMasks.Dispose();
                HOperatorSet.GetDictObject(out ho_InstanceMasks, hv_DLSample, "mask");
                //
                //Get the number of instance masks.
                hv_NumMasks.Dispose();
                HOperatorSet.CountObj(ho_InstanceMasks, out hv_NumMasks);
                //
                //Domain handling of the image to be preprocessed.
                //
                hv_WidthRaw.Dispose(); hv_HeightRaw.Dispose();
                HOperatorSet.GetImageSize(ho_ImageRaw, out hv_WidthRaw, out hv_HeightRaw);
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //Clip and translate masks w.r.t. the image domain
                    ho_Domain.Dispose();
                    HOperatorSet.GetDomain(ho_ImageRaw, out ho_Domain);
                    hv_DomainRow1.Dispose(); hv_DomainColumn1.Dispose(); hv_DomainRow2.Dispose(); hv_DomainColumn2.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_Domain, out hv_DomainRow1, out hv_DomainColumn1,
                        out hv_DomainRow2, out hv_DomainColumn2);
                    //
                    //Clip the remaining regions to the domain.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ClipRegion(ho_InstanceMasks, out ExpTmpOutVar_0, hv_DomainRow1,
                            hv_DomainColumn1, hv_DomainRow2, hv_DomainColumn2);
                        ho_InstanceMasks.Dispose();
                        ho_InstanceMasks = ExpTmpOutVar_0;
                    }
                    hv_WidthRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_WidthRaw = (hv_DomainColumn2 - hv_DomainColumn1) + 1.0;
                    }
                    hv_HeightRaw.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HeightRaw = (hv_DomainRow2 - hv_DomainRow1) + 1.0;
                    }
                    //We need to move the remaining regions back to the origin,
                    //because crop_domain will be applied to the image
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.MoveRegion(ho_InstanceMasks, out ExpTmpOutVar_0, -hv_DomainRow1,
                            -hv_DomainColumn1);
                        ho_InstanceMasks.Dispose();
                        ho_InstanceMasks = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleNotEqual("full_domain"))) != 0)
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Zoom masks only if the image has a different size than the specified size.
                if ((int)(((hv_ImageWidth.TupleNotEqualElem(hv_WidthRaw))).TupleOr(hv_ImageHeight.TupleNotEqualElem(
                    hv_HeightRaw))) != 0)
                {
                    //Calculate rescaling factor.
                    hv_FactorResampleWidth.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleWidth = (hv_ImageWidth.TupleReal()
                            ) / hv_WidthRaw;
                    }
                    hv_FactorResampleHeight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_FactorResampleHeight = (hv_ImageHeight.TupleReal()
                            ) / hv_HeightRaw;
                    }

                    //Zoom the masks.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomRegion(ho_InstanceMasks, out ExpTmpOutVar_0, hv_FactorResampleWidth,
                            hv_FactorResampleHeight);
                        ho_InstanceMasks.Dispose();
                        ho_InstanceMasks = ExpTmpOutVar_0;
                    }
                }
                //
                //Set the preprocessed instance masks.
                HOperatorSet.SetDictObject(ho_InstanceMasks, hv_DLSample, "mask");
                //
                //
                ho_InstanceMasks.Dispose();
                ho_Domain.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_NumMasks.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_InstanceMasks.Dispose();
                ho_Domain.Dispose();

                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_DomainHandling.Dispose();
                hv_NumMasks.Dispose();
                hv_WidthRaw.Dispose();
                hv_HeightRaw.Dispose();
                hv_DomainRow1.Dispose();
                hv_DomainColumn1.Dispose();
                hv_DomainRow2.Dispose();
                hv_DomainColumn2.Dispose();
                hv_FactorResampleWidth.Dispose();
                hv_FactorResampleHeight.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Semantic Segmentation
        // Short Description: Preprocess segmentation and weight images for deep-learning-based segmentation training and inference. 
        private void preprocess_dl_model_segmentations(HObject ho_ImagesRaw, HObject ho_Segmentations,
            out HObject ho_SegmentationsPreprocessed, HTuple hv_DLPreprocessParam)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Domain = null, ho_SelectedSeg = null;
            HObject ho_SelectedDomain = null;

            // Local copy input parameter variables 
            HObject ho_Segmentations_COPY_INP_TMP;
            ho_Segmentations_COPY_INP_TMP = new HObject(ho_Segmentations);



            // Local control variables 

            HTuple hv_NumberImages = new HTuple(), hv_NumberSegmentations = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_WidthSeg = new HTuple(), hv_HeightSeg = new HTuple();
            HTuple hv_DLModelType = new HTuple(), hv_ImageWidth = new HTuple();
            HTuple hv_ImageHeight = new HTuple(), hv_ImageNumChannels = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_DomainHandling = new HTuple(), hv_SetBackgroundID = new HTuple();
            HTuple hv_ClassesToBackground = new HTuple(), hv_IgnoreClassIDs = new HTuple();
            HTuple hv_IsInt = new HTuple(), hv_IndexImage = new HTuple();
            HTuple hv_ImageWidthRaw = new HTuple(), hv_ImageHeightRaw = new HTuple();
            HTuple hv_EqualWidth = new HTuple(), hv_EqualHeight = new HTuple();
            HTuple hv_Type = new HTuple(), hv_EqualReal = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SegmentationsPreprocessed);
            HOperatorSet.GenEmptyObj(out ho_Domain);
            HOperatorSet.GenEmptyObj(out ho_SelectedSeg);
            HOperatorSet.GenEmptyObj(out ho_SelectedDomain);
            try
            {
                //
                //This procedure preprocesses the segmentation or weight images
                //given by Segmentations so that they can be handled by
                //train_dl_model_batch and apply_dl_model.
                //
                //Check input data.
                //Examine number of images.
                hv_NumberImages.Dispose();
                HOperatorSet.CountObj(ho_ImagesRaw, out hv_NumberImages);
                hv_NumberSegmentations.Dispose();
                HOperatorSet.CountObj(ho_Segmentations_COPY_INP_TMP, out hv_NumberSegmentations);
                if ((int)(new HTuple(hv_NumberImages.TupleNotEqual(hv_NumberSegmentations))) != 0)
                {
                    throw new HalconException("Equal number of images given in ImagesRaw and Segmentations required");
                }
                //Size of images.
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_ImagesRaw, out hv_Width, out hv_Height);
                hv_WidthSeg.Dispose(); hv_HeightSeg.Dispose();
                HOperatorSet.GetImageSize(ho_Segmentations_COPY_INP_TMP, out hv_WidthSeg, out hv_HeightSeg);
                if ((int)((new HTuple(hv_Width.TupleNotEqual(hv_WidthSeg))).TupleOr(new HTuple(hv_Height.TupleNotEqual(
                    hv_HeightSeg)))) != 0)
                {
                    throw new HalconException("Equal size of the images given in ImagesRaw and Segmentations required.");
                }
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //
                //Get the relevant preprocessing parameters.
                hv_DLModelType.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "model_type", out hv_DLModelType);
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "image_range_max", out hv_ImageRangeMax);
                hv_DomainHandling.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "domain_handling", out hv_DomainHandling);
                //Segmentation specific parameters.
                hv_SetBackgroundID.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "set_background_id", out hv_SetBackgroundID);
                hv_ClassesToBackground.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "class_ids_background", out hv_ClassesToBackground);
                hv_IgnoreClassIDs.Dispose();
                HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", out hv_IgnoreClassIDs);
                //
                //Check the input parameter for setting the background ID.
                if ((int)(new HTuple(hv_SetBackgroundID.TupleNotEqual(new HTuple()))) != 0)
                {
                    //Check that the model is a segmentation model.
                    if ((int)(new HTuple(hv_DLModelType.TupleNotEqual("segmentation"))) != 0)
                    {
                        throw new HalconException("Setting class IDs to background is only implemented for segmentation.");
                    }
                    //Check the background ID.
                    hv_IsInt.Dispose();
                    HOperatorSet.TupleIsIntElem(hv_SetBackgroundID, out hv_IsInt);
                    if ((int)(new HTuple((new HTuple(hv_SetBackgroundID.TupleLength())).TupleNotEqual(
                        1))) != 0)
                    {
                        throw new HalconException("Only one class_id as 'set_background_id' allowed.");
                    }
                    else if ((int)(hv_IsInt.TupleNot()) != 0)
                    {
                        //Given class_id has to be of type int.
                        throw new HalconException("The class_id given as 'set_background_id' has to be of type int.");
                    }
                    //Check the values of ClassesToBackground.
                    if ((int)(new HTuple((new HTuple(hv_ClassesToBackground.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        //Check that the given classes are of length > 0.
                        throw new HalconException(new HTuple("If 'set_background_id' is given, 'class_ids_background' must at least contain this class ID."));
                    }
                    else if ((int)(new HTuple(((hv_ClassesToBackground.TupleIntersection(
                        hv_IgnoreClassIDs))).TupleNotEqual(new HTuple()))) != 0)
                    {
                        //Check that class_ids_background is not included in the ignore_class_ids of the DLModel.
                        throw new HalconException("The given 'class_ids_background' must not be included in the 'ignore_class_ids' of the model.");
                    }
                }
                //
                //Domain handling of the image to be preprocessed.
                //
                if ((int)(new HTuple(hv_DomainHandling.TupleEqual("full_domain"))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.FullDomain(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else if ((int)(new HTuple(hv_DomainHandling.TupleEqual("crop_domain"))) != 0)
                {
                    //If the domain should be cropped the domain has to be transferred
                    //from the raw image to the segmentation image.
                    ho_Domain.Dispose();
                    HOperatorSet.GetDomain(ho_ImagesRaw, out ho_Domain);
                    HTuple end_val66 = hv_NumberImages;
                    HTuple step_val66 = 1;
                    for (hv_IndexImage = 1; hv_IndexImage.Continue(end_val66, step_val66); hv_IndexImage = hv_IndexImage.TupleAdd(step_val66))
                    {
                        ho_SelectedSeg.Dispose();
                        HOperatorSet.SelectObj(ho_Segmentations_COPY_INP_TMP, out ho_SelectedSeg,
                            hv_IndexImage);
                        ho_SelectedDomain.Dispose();
                        HOperatorSet.SelectObj(ho_Domain, out ho_SelectedDomain, hv_IndexImage);
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ChangeDomain(ho_SelectedSeg, ho_SelectedDomain, out ExpTmpOutVar_0
                                );
                            ho_SelectedSeg.Dispose();
                            ho_SelectedSeg = ExpTmpOutVar_0;
                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.ReplaceObj(ho_Segmentations_COPY_INP_TMP, ho_SelectedSeg,
                                out ExpTmpOutVar_0, hv_IndexImage);
                            ho_Segmentations_COPY_INP_TMP.Dispose();
                            ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                        }
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CropDomain(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0
                            );
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                else
                {
                    throw new HalconException("Unsupported parameter value for 'domain_handling'");
                }
                //
                //Preprocess the segmentation images.
                //
                //Set all background classes to the given background class ID.
                if ((int)(new HTuple(hv_SetBackgroundID.TupleNotEqual(new HTuple()))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        reassign_pixel_values(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ClassesToBackground, hv_SetBackgroundID);
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Zoom images only if they have a different size than the specified size.
                hv_ImageWidthRaw.Dispose(); hv_ImageHeightRaw.Dispose();
                HOperatorSet.GetImageSize(ho_Segmentations_COPY_INP_TMP, out hv_ImageWidthRaw,
                    out hv_ImageHeightRaw);
                hv_EqualWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualWidth = hv_ImageWidth.TupleEqualElem(
                        hv_ImageWidthRaw);
                }
                hv_EqualHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualHeight = hv_ImageHeight.TupleEqualElem(
                        hv_ImageHeightRaw);
                }
                if ((int)((new HTuple(((hv_EqualWidth.TupleMin())).TupleEqual(0))).TupleOr(
                    new HTuple(((hv_EqualHeight.TupleMin())).TupleEqual(0)))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ZoomImageSize(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0,
                            hv_ImageWidth, hv_ImageHeight, "nearest_neighbor");
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Check the type of the input images
                //and convert if necessary.
                hv_Type.Dispose();
                HOperatorSet.GetImageType(ho_Segmentations_COPY_INP_TMP, out hv_Type);
                hv_EqualReal.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_EqualReal = hv_Type.TupleEqualElem(
                        "real");
                }
                //
                if ((int)(new HTuple(((hv_EqualReal.TupleMin())).TupleEqual(0))) != 0)
                {
                    //Convert the image type to 'real',
                    //because the model expects 'real' images.
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.ConvertImageType(ho_Segmentations_COPY_INP_TMP, out ExpTmpOutVar_0,
                            "real");
                        ho_Segmentations_COPY_INP_TMP.Dispose();
                        ho_Segmentations_COPY_INP_TMP = ExpTmpOutVar_0;
                    }
                }
                //
                //Write preprocessed Segmentations to output variable.
                ho_SegmentationsPreprocessed.Dispose();
                ho_SegmentationsPreprocessed = new HObject(ho_Segmentations_COPY_INP_TMP);
                ho_Segmentations_COPY_INP_TMP.Dispose();
                ho_Domain.Dispose();
                ho_SelectedSeg.Dispose();
                ho_SelectedDomain.Dispose();

                hv_NumberImages.Dispose();
                hv_NumberSegmentations.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WidthSeg.Dispose();
                hv_HeightSeg.Dispose();
                hv_DLModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassesToBackground.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_IsInt.Dispose();
                hv_IndexImage.Dispose();
                hv_ImageWidthRaw.Dispose();
                hv_ImageHeightRaw.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_EqualReal.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Segmentations_COPY_INP_TMP.Dispose();
                ho_Domain.Dispose();
                ho_SelectedSeg.Dispose();
                ho_SelectedDomain.Dispose();

                hv_NumberImages.Dispose();
                hv_NumberSegmentations.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_WidthSeg.Dispose();
                hv_HeightSeg.Dispose();
                hv_DLModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_DomainHandling.Dispose();
                hv_SetBackgroundID.Dispose();
                hv_ClassesToBackground.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_IsInt.Dispose();
                hv_IndexImage.Dispose();
                hv_ImageWidthRaw.Dispose();
                hv_ImageHeightRaw.Dispose();
                hv_EqualWidth.Dispose();
                hv_EqualHeight.Dispose();
                hv_Type.Dispose();
                hv_EqualReal.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess given DLSamples according to the preprocessing parameters given in DLPreprocessParam. 
        private void preprocess_dl_samples(HTuple hv_DLSampleBatch, HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            HObject ho_ImageRaw = null, ho_ImagePreprocessed = null;
            HObject ho_AnomalyImageRaw = null, ho_AnomalyImagePreprocessed = null;
            HObject ho_SegmentationRaw = null, ho_SegmentationPreprocessed = null;

            // Local control variables 

            HTuple hv_SampleIndex = new HTuple(), hv_ImageExists = new HTuple();
            HTuple hv_KeysExists = new HTuple(), hv_AnomalyParamExist = new HTuple();
            HTuple hv_Rectangle1ParamExist = new HTuple(), hv_Rectangle2ParamExist = new HTuple();
            HTuple hv_InstanceMaskParamExist = new HTuple(), hv_SegmentationParamExist = new HTuple();
            HTuple hv_DLPreprocessParam_COPY_INP_TMP = new HTuple(hv_DLPreprocessParam);

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageRaw);
            HOperatorSet.GenEmptyObj(out ho_ImagePreprocessed);
            HOperatorSet.GenEmptyObj(out ho_AnomalyImageRaw);
            HOperatorSet.GenEmptyObj(out ho_AnomalyImagePreprocessed);
            HOperatorSet.GenEmptyObj(out ho_SegmentationRaw);
            HOperatorSet.GenEmptyObj(out ho_SegmentationPreprocessed);
            try
            {
                //
                //This procedure preprocesses all images of the sample dictionaries in the tuple DLSampleBatch.
                //The images are preprocessed according to the parameters provided in DLPreprocessParam.
                //
                //Check the validity of the preprocessing parameters.
                //The procedure check_dl_preprocess_param might change DLPreprocessParam. To avoid race
                //conditions when preprocess_dl_samples is used from multiple threads with the same
                //DLPreprocessParam dictionary, work on a copy.
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.CopyDict(hv_DLPreprocessParam_COPY_INP_TMP, new HTuple(), new HTuple(),
                        out ExpTmpOutVar_0);
                    hv_DLPreprocessParam_COPY_INP_TMP.Dispose();
                    hv_DLPreprocessParam_COPY_INP_TMP = ExpTmpOutVar_0;
                }
                check_dl_preprocess_param(hv_DLPreprocessParam_COPY_INP_TMP);
                //
                //
                //Preprocess the sample entries.
                //
                for (hv_SampleIndex = 0; (int)hv_SampleIndex <= (int)((new HTuple(hv_DLSampleBatch.TupleLength()
                    )) - 1); hv_SampleIndex = (int)hv_SampleIndex + 1)
                {
                    //
                    //Check the existence of the sample keys.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ImageExists.Dispose();
                        HOperatorSet.GetDictParam(hv_DLSampleBatch.TupleSelect(hv_SampleIndex), "key_exists",
                            "image", out hv_ImageExists);
                    }
                    //
                    //Preprocess the images.
                    if ((int)(hv_ImageExists) != 0)
                    {
                        //
                        //Get the image.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ImageRaw.Dispose();
                            HOperatorSet.GetDictObject(out ho_ImageRaw, hv_DLSampleBatch.TupleSelect(
                                hv_SampleIndex), "image");
                        }
                        //
                        //Preprocess the image.
                        ho_ImagePreprocessed.Dispose();
                        preprocess_dl_model_images(ho_ImageRaw, out ho_ImagePreprocessed, hv_DLPreprocessParam_COPY_INP_TMP);
                        //
                        //Replace the image in the dictionary.
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictObject(ho_ImagePreprocessed, hv_DLSampleBatch.TupleSelect(
                                hv_SampleIndex), "image");
                        }
                        //
                        //Check existence of model specific sample keys:
                        //- bbox_row1 for 'rectangle1'
                        //- bbox_phi for 'rectangle2'
                        //- segmentation_image for 'semantic segmentation'
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_KeysExists.Dispose();
                            HOperatorSet.GetDictParam(hv_DLSampleBatch.TupleSelect(hv_SampleIndex),
                                "key_exists", ((((new HTuple("anomaly_ground_truth")).TupleConcat("bbox_row1")).TupleConcat(
                                "bbox_phi")).TupleConcat("mask")).TupleConcat("segmentation_image"),
                                out hv_KeysExists);
                        }
                        hv_AnomalyParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_AnomalyParamExist = hv_KeysExists.TupleSelect(
                                0);
                        }
                        hv_Rectangle1ParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Rectangle1ParamExist = hv_KeysExists.TupleSelect(
                                1);
                        }
                        hv_Rectangle2ParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Rectangle2ParamExist = hv_KeysExists.TupleSelect(
                                2);
                        }
                        hv_InstanceMaskParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_InstanceMaskParamExist = hv_KeysExists.TupleSelect(
                                3);
                        }
                        hv_SegmentationParamExist.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_SegmentationParamExist = hv_KeysExists.TupleSelect(
                                4);
                        }
                        //
                        //Preprocess the anomaly ground truth if present.
                        if ((int)(hv_AnomalyParamExist) != 0)
                        {
                            //
                            //Get the anomaly image.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_AnomalyImageRaw.Dispose();
                                HOperatorSet.GetDictObject(out ho_AnomalyImageRaw, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), "anomaly_ground_truth");
                            }
                            //
                            //Preprocess the anomaly image.
                            ho_AnomalyImagePreprocessed.Dispose();
                            preprocess_dl_model_anomaly(ho_AnomalyImageRaw, out ho_AnomalyImagePreprocessed,
                                hv_DLPreprocessParam_COPY_INP_TMP);
                            //
                            //Set preprocessed anomaly image.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetDictObject(ho_AnomalyImagePreprocessed, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), "anomaly_ground_truth");
                            }
                        }
                        //
                        //Preprocess depending on the model type.
                        //If bounding boxes are given, rescale them as well.
                        if ((int)(hv_Rectangle1ParamExist) != 0)
                        {
                            //
                            //Preprocess the bounding boxes of type 'rectangle1'.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                preprocess_dl_model_bbox_rect1(ho_ImageRaw, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), hv_DLPreprocessParam_COPY_INP_TMP);
                            }
                        }
                        else if ((int)(hv_Rectangle2ParamExist) != 0)
                        {
                            //
                            //Preprocess the bounding boxes of type 'rectangle2'.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                preprocess_dl_model_bbox_rect2(ho_ImageRaw, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), hv_DLPreprocessParam_COPY_INP_TMP);
                            }
                        }
                        if ((int)(hv_InstanceMaskParamExist) != 0)
                        {
                            //
                            //Preprocess the instance masks.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                preprocess_dl_model_instance_masks(ho_ImageRaw, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), hv_DLPreprocessParam_COPY_INP_TMP);
                            }
                        }
                        //
                        //Preprocess the segmentation image if present.
                        if ((int)(hv_SegmentationParamExist) != 0)
                        {
                            //
                            //Get the segmentation image.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_SegmentationRaw.Dispose();
                                HOperatorSet.GetDictObject(out ho_SegmentationRaw, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), "segmentation_image");
                            }
                            //
                            //Preprocess the segmentation image.
                            ho_SegmentationPreprocessed.Dispose();
                            preprocess_dl_model_segmentations(ho_ImageRaw, ho_SegmentationRaw, out ho_SegmentationPreprocessed,
                                hv_DLPreprocessParam_COPY_INP_TMP);
                            //
                            //Set preprocessed segmentation image.
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                HOperatorSet.SetDictObject(ho_SegmentationPreprocessed, hv_DLSampleBatch.TupleSelect(
                                    hv_SampleIndex), "segmentation_image");
                            }
                        }
                    }
                    else
                    {
                        throw new HalconException((new HTuple("All samples processed need to include an image, but the sample with index ") + hv_SampleIndex) + " does not.");
                    }
                }
                //
                ho_ImageRaw.Dispose();
                ho_ImagePreprocessed.Dispose();
                ho_AnomalyImageRaw.Dispose();
                ho_AnomalyImagePreprocessed.Dispose();
                ho_SegmentationRaw.Dispose();
                ho_SegmentationPreprocessed.Dispose();

                hv_DLPreprocessParam_COPY_INP_TMP.Dispose();
                hv_SampleIndex.Dispose();
                hv_ImageExists.Dispose();
                hv_KeysExists.Dispose();
                hv_AnomalyParamExist.Dispose();
                hv_Rectangle1ParamExist.Dispose();
                hv_Rectangle2ParamExist.Dispose();
                hv_InstanceMaskParamExist.Dispose();
                hv_SegmentationParamExist.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_ImageRaw.Dispose();
                ho_ImagePreprocessed.Dispose();
                ho_AnomalyImageRaw.Dispose();
                ho_AnomalyImagePreprocessed.Dispose();
                ho_SegmentationRaw.Dispose();
                ho_SegmentationPreprocessed.Dispose();

                hv_DLPreprocessParam_COPY_INP_TMP.Dispose();
                hv_SampleIndex.Dispose();
                hv_ImageExists.Dispose();
                hv_KeysExists.Dispose();
                hv_AnomalyParamExist.Dispose();
                hv_Rectangle1ParamExist.Dispose();
                hv_Rectangle2ParamExist.Dispose();
                hv_InstanceMaskParamExist.Dispose();
                hv_SegmentationParamExist.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Image / Manipulation
        // Short Description: Changes a value of ValuesToChange in Image to NewValue. 
        private void reassign_pixel_values(HObject ho_Image, out HObject ho_ImageOut,
            HTuple hv_ValuesToChange, HTuple hv_NewValue)
        {




            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_RegionToChange, ho_RegionClass = null;

            // Local control variables 

            HTuple hv_IndexReset = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageOut);
            HOperatorSet.GenEmptyObj(out ho_RegionToChange);
            HOperatorSet.GenEmptyObj(out ho_RegionClass);
            try
            {
                //
                //This procedure sets all pixels of Image
                //with the values given in ValuesToChange to the given value NewValue.
                //
                ho_RegionToChange.Dispose();
                HOperatorSet.GenEmptyRegion(out ho_RegionToChange);
                for (hv_IndexReset = 0; (int)hv_IndexReset <= (int)((new HTuple(hv_ValuesToChange.TupleLength()
                    )) - 1); hv_IndexReset = (int)hv_IndexReset + 1)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_RegionClass.Dispose();
                        HOperatorSet.Threshold(ho_Image, out ho_RegionClass, hv_ValuesToChange.TupleSelect(
                            hv_IndexReset), hv_ValuesToChange.TupleSelect(hv_IndexReset));
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.Union2(ho_RegionToChange, ho_RegionClass, out ExpTmpOutVar_0
                            );
                        ho_RegionToChange.Dispose();
                        ho_RegionToChange = ExpTmpOutVar_0;
                    }
                }
                HOperatorSet.OverpaintRegion(ho_Image, ho_RegionToChange, hv_NewValue, "fill");
                ho_ImageOut.Dispose();
                ho_ImageOut = new HObject(ho_Image);
                ho_RegionToChange.Dispose();
                ho_RegionClass.Dispose();

                hv_IndexReset.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_RegionToChange.Dispose();
                ho_RegionClass.Dispose();

                hv_IndexReset.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: This procedure replaces legacy preprocessing parameters. 
        private void replace_legacy_preprocessing_parameters(HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_Exception = new HTuple(), hv_NormalizationTypeExists = new HTuple();
            HTuple hv_NormalizationType = new HTuple(), hv_LegacyNormalizationKeyExists = new HTuple();
            HTuple hv_ContrastNormalization = new HTuple();
            // Initialize local and output iconic variables 
            try
            {
                //
                //This procedure adapts the dictionary DLPreprocessParam
                //if a legacy preprocessing parameter is set.
                //
                //Map legacy value set to new parameter.
                hv_Exception.Dispose();
                hv_Exception = 0;
                try
                {
                    hv_NormalizationTypeExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "normalization_type",
                        out hv_NormalizationTypeExists);
                    //
                    if ((int)(hv_NormalizationTypeExists) != 0)
                    {
                        hv_NormalizationType.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "normalization_type", out hv_NormalizationType);
                        if ((int)(new HTuple(hv_NormalizationType.TupleEqual("true"))) != 0)
                        {
                            hv_NormalizationType.Dispose();
                            hv_NormalizationType = "first_channel";
                        }
                        else if ((int)(new HTuple(hv_NormalizationType.TupleEqual("false"))) != 0)
                        {
                            hv_NormalizationType.Dispose();
                            hv_NormalizationType = "none";
                        }
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type", hv_NormalizationType);
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }
                //
                //Map legacy parameter to new parameter and corresponding value.
                hv_Exception.Dispose();
                hv_Exception = 0;
                try
                {
                    hv_LegacyNormalizationKeyExists.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", "contrast_normalization",
                        out hv_LegacyNormalizationKeyExists);
                    if ((int)(hv_LegacyNormalizationKeyExists) != 0)
                    {
                        hv_ContrastNormalization.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "contrast_normalization",
                            out hv_ContrastNormalization);
                        //Replace 'contrast_normalization' by 'normalization_type'.
                        if ((int)(new HTuple(hv_ContrastNormalization.TupleEqual("false"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type",
                                "none");
                        }
                        else if ((int)(new HTuple(hv_ContrastNormalization.TupleEqual(
                            "true"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type",
                                "first_channel");
                        }
                        HOperatorSet.RemoveDictKey(hv_DLPreprocessParam, "contrast_normalization");
                    }
                }
                // catch (Exception) 
                catch (HalconException HDevExpDefaultException1)
                {
                    HDevExpDefaultException1.ToHTuple(out hv_Exception);
                }

                hv_Exception.Dispose();
                hv_NormalizationTypeExists.Dispose();
                hv_NormalizationType.Dispose();
                hv_LegacyNormalizationKeyExists.Dispose();
                hv_ContrastNormalization.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_Exception.Dispose();
                hv_NormalizationTypeExists.Dispose();
                hv_NormalizationType.Dispose();
                hv_LegacyNormalizationKeyExists.Dispose();
                hv_ContrastNormalization.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion
    }
}