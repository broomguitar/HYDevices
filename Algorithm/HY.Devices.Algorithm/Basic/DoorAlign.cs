using HalconDotNet;
using HY.Devices.Algorithm.Utils;
using System;
using System.Drawing;
namespace HY.Devices.Algorithm
{
    /// <summary>
    /// 新门齐算法
    /// </summary>
    public class DoorAlign
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
        public HTuple hv_ExpDefaultWinHandle;
        #region 辅助方法
        // Procedures 
        // External procedures 
        // Chapter: Develop
        // Short Description: Open a new graphics window that preserves the aspect ratio of the given image. 
        private void dev_open_window_fit_image(HObject ho_Image, HTuple hv_Row, HTuple hv_Column,
            HTuple hv_WidthLimit, HTuple hv_HeightLimit, out HTuple hv_WindowHandle)
        {




            // Local iconic variables 

            // Local control variables 

            HTuple hv_MinWidth = new HTuple(), hv_MaxWidth = new HTuple();
            HTuple hv_MinHeight = new HTuple(), hv_MaxHeight = new HTuple();
            HTuple hv_ResizeFactor = new HTuple(), hv_ImageWidth = new HTuple();
            HTuple hv_ImageHeight = new HTuple(), hv_TempWidth = new HTuple();
            HTuple hv_TempHeight = new HTuple(), hv_WindowWidth = new HTuple();
            HTuple hv_WindowHeight = new HTuple();
            // Initialize local and output iconic variables 
            hv_WindowHandle = new HTuple();
            //This procedure opens a new graphics window and adjusts the size
            //such that it fits into the limits specified by WidthLimit
            //and HeightLimit, but also maintains the correct image aspect ratio.
            //
            //If it is impossible to match the minimum and maximum extent requirements
            //at the same time (f.e. if the image is very long but narrow),
            //the maximum value gets a higher priority,
            //
            //Parse input tuple WidthLimit
            if ((int)((new HTuple((new HTuple(hv_WidthLimit.TupleLength())).TupleEqual(0))).TupleOr(
                new HTuple(hv_WidthLimit.TupleLess(0)))) != 0)
            {
                hv_MinWidth.Dispose();
                hv_MinWidth = 500;
                hv_MaxWidth.Dispose();
                hv_MaxWidth = 800;
            }
            else if ((int)(new HTuple((new HTuple(hv_WidthLimit.TupleLength())).TupleEqual(
                1))) != 0)
            {
                hv_MinWidth.Dispose();
                hv_MinWidth = 0;
                hv_MaxWidth.Dispose();
                hv_MaxWidth = new HTuple(hv_WidthLimit);
            }
            else
            {
                hv_MinWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MinWidth = hv_WidthLimit.TupleSelect(
                        0);
                }
                hv_MaxWidth.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MaxWidth = hv_WidthLimit.TupleSelect(
                        1);
                }
            }
            //Parse input tuple HeightLimit
            if ((int)((new HTuple((new HTuple(hv_HeightLimit.TupleLength())).TupleEqual(0))).TupleOr(
                new HTuple(hv_HeightLimit.TupleLess(0)))) != 0)
            {
                hv_MinHeight.Dispose();
                hv_MinHeight = 400;
                hv_MaxHeight.Dispose();
                hv_MaxHeight = 600;
            }
            else if ((int)(new HTuple((new HTuple(hv_HeightLimit.TupleLength())).TupleEqual(
                1))) != 0)
            {
                hv_MinHeight.Dispose();
                hv_MinHeight = 0;
                hv_MaxHeight.Dispose();
                hv_MaxHeight = new HTuple(hv_HeightLimit);
            }
            else
            {
                hv_MinHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MinHeight = hv_HeightLimit.TupleSelect(
                        0);
                }
                hv_MaxHeight.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_MaxHeight = hv_HeightLimit.TupleSelect(
                        1);
                }
            }
            //
            //Test, if window size has to be changed.
            hv_ResizeFactor.Dispose();
            hv_ResizeFactor = 1;
            hv_ImageWidth.Dispose(); hv_ImageHeight.Dispose();
            HOperatorSet.GetImageSize(ho_Image, out hv_ImageWidth, out hv_ImageHeight);
            //First, expand window to the minimum extents (if necessary).
            if ((int)((new HTuple(hv_MinWidth.TupleGreater(hv_ImageWidth))).TupleOr(new HTuple(hv_MinHeight.TupleGreater(
                hv_ImageHeight)))) != 0)
            {
                hv_ResizeFactor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_ResizeFactor = (((((hv_MinWidth.TupleReal()
                        ) / hv_ImageWidth)).TupleConcat((hv_MinHeight.TupleReal()) / hv_ImageHeight))).TupleMax()
                        ;
                }
            }
            hv_TempWidth.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TempWidth = hv_ImageWidth * hv_ResizeFactor;
            }
            hv_TempHeight.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_TempHeight = hv_ImageHeight * hv_ResizeFactor;
            }
            //Then, shrink window to maximum extents (if necessary).
            if ((int)((new HTuple(hv_MaxWidth.TupleLess(hv_TempWidth))).TupleOr(new HTuple(hv_MaxHeight.TupleLess(
                hv_TempHeight)))) != 0)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    {
                        HTuple
                          ExpTmpLocalVar_ResizeFactor = hv_ResizeFactor * ((((((hv_MaxWidth.TupleReal()
                            ) / hv_TempWidth)).TupleConcat((hv_MaxHeight.TupleReal()) / hv_TempHeight))).TupleMin()
                            );
                        hv_ResizeFactor.Dispose();
                        hv_ResizeFactor = ExpTmpLocalVar_ResizeFactor;
                    }
                }
            }
            hv_WindowWidth.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_WindowWidth = hv_ImageWidth * hv_ResizeFactor;
            }
            hv_WindowHeight.Dispose();
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_WindowHeight = hv_ImageHeight * hv_ResizeFactor;
            }
            //Resize window
            //dev_open_window(...);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                HOperatorSet.SetPart(hv_ExpDefaultWinHandle, 0, 0, hv_ImageHeight - 1, hv_ImageWidth - 1);
            }

            hv_MinWidth.Dispose();
            hv_MaxWidth.Dispose();
            hv_MinHeight.Dispose();
            hv_MaxHeight.Dispose();
            hv_ResizeFactor.Dispose();
            hv_ImageWidth.Dispose();
            hv_ImageHeight.Dispose();
            hv_TempWidth.Dispose();
            hv_TempHeight.Dispose();
            hv_WindowWidth.Dispose();
            hv_WindowHeight.Dispose();

            return;
        }

        // Chapter: Graphics / Text
        // Short Description: This procedure writes one or multiple text messages. 
        private void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
            HTuple hv_Row, HTuple hv_Column, HTuple hv_Color, HTuple hv_Box)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_GenParamName = new HTuple(), hv_GenParamValue = new HTuple();
            HTuple hv_Color_COPY_INP_TMP = new HTuple(hv_Color);
            HTuple hv_Column_COPY_INP_TMP = new HTuple(hv_Column);
            HTuple hv_CoordSystem_COPY_INP_TMP = new HTuple(hv_CoordSystem);
            HTuple hv_Row_COPY_INP_TMP = new HTuple(hv_Row);

            // Initialize local and output iconic variables 
            //This procedure displays text in a graphics window.
            //
            //Input parameters:
            //WindowHandle: The WindowHandle of the graphics window, where
            //   the message should be displayed.
            //String: A tuple of strings containing the text messages to be displayed.
            //CoordSystem: If set to 'window', the text position is given
            //   with respect to the window coordinate system.
            //   If set to 'image', image coordinates are used.
            //   (This may be useful in zoomed images.)
            //Row: The row coordinate of the desired text position.
            //   You can pass a single value or a tuple of values.
            //   See the explanation below.
            //   Default: 12.
            //Column: The column coordinate of the desired text position.
            //   You can pass a single value or a tuple of values.
            //   See the explanation below.
            //   Default: 12.
            //Color: defines the color of the text as string.
            //   If set to [] or '' the currently set color is used.
            //   If a tuple of strings is passed, the colors are used cyclically
            //   for every text position defined by Row and Column,
            //   or every new text line in case of |Row| == |Column| == 1.
            //Box: A tuple controlling a possible box surrounding the text.
            //   Its entries:
            //   - Box[0]: Controls the box and its color. Possible values:
            //     -- 'true' (Default): An orange box is displayed.
            //     -- 'false': No box is displayed.
            //     -- color string: A box is displayed in the given color, e.g., 'white', '#FF00CC'.
            //   - Box[1] (Optional): Controls the shadow of the box. Possible values:
            //     -- 'true' (Default): A shadow is displayed in
            //               darker orange if Box[0] is not a color and in 'white' otherwise.
            //     -- 'false': No shadow is displayed.
            //     -- color string: A shadow is displayed in the given color, e.g., 'white', '#FF00CC'.
            //
            //It is possible to display multiple text strings in a single call.
            //In this case, some restrictions apply on the
            //parameters String, Row, and Column:
            //They can only have either 1 entry or n entries.
            //Behavior in the different cases:
            //   - Multiple text positions are specified, i.e.,
            //       - |Row| == n, |Column| == n
            //       - |Row| == n, |Column| == 1
            //       - |Row| == 1, |Column| == n
            //     In this case we distinguish:
            //       - |String| == n: Each element of String is displayed
            //                        at the corresponding position.
            //       - |String| == 1: String is displayed n times
            //                        at the corresponding positions.
            //   - Exactly one text position is specified,
            //      i.e., |Row| == |Column| == 1:
            //      Each element of String is display in a new textline.
            //
            //
            //Convert the parameters for disp_text.
            if ((int)((new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(new HTuple()))).TupleOr(
                new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(new HTuple())))) != 0)
            {

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

                return;
            }
            if ((int)(new HTuple(hv_Row_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Row_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP = 12;
            }
            if ((int)(new HTuple(hv_Column_COPY_INP_TMP.TupleEqual(-1))) != 0)
            {
                hv_Column_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP = 12;
            }
            //
            //Convert the parameter Box to generic parameters.
            hv_GenParamName.Dispose();
            hv_GenParamName = new HTuple();
            hv_GenParamValue.Dispose();
            hv_GenParamValue = new HTuple();
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(0))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleEqual("false"))) != 0)
                {
                    //Display no box
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual("true"))) != 0)
                {
                    //Set a color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "box_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(0));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            if ((int)(new HTuple((new HTuple(hv_Box.TupleLength())).TupleGreater(1))) != 0)
            {
                if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleEqual("false"))) != 0)
                {
                    //Display no shadow.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                "false");
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
                else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual("true"))) != 0)
                {
                    //Set a shadow color other than the default.
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamName = hv_GenParamName.TupleConcat(
                                "shadow_color");
                            hv_GenParamName.Dispose();
                            hv_GenParamName = ExpTmpLocalVar_GenParamName;
                        }
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_GenParamValue = hv_GenParamValue.TupleConcat(
                                hv_Box.TupleSelect(1));
                            hv_GenParamValue.Dispose();
                            hv_GenParamValue = ExpTmpLocalVar_GenParamValue;
                        }
                    }
                }
            }
            //Restore default CoordSystem behavior.
            if ((int)(new HTuple(hv_CoordSystem_COPY_INP_TMP.TupleNotEqual("window"))) != 0)
            {
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP = "image";
            }
            //
            if ((int)(new HTuple(hv_Color_COPY_INP_TMP.TupleEqual(""))) != 0)
            {
                //disp_text does not accept an empty string for Color.
                hv_Color_COPY_INP_TMP.Dispose();
                hv_Color_COPY_INP_TMP = new HTuple();
            }
            //
            HOperatorSet.DispText(hv_ExpDefaultWinHandle, hv_String, hv_CoordSystem_COPY_INP_TMP,
                hv_Row_COPY_INP_TMP, hv_Column_COPY_INP_TMP, hv_Color_COPY_INP_TMP, hv_GenParamName,
                hv_GenParamValue);

            hv_Color_COPY_INP_TMP.Dispose();
            hv_Column_COPY_INP_TMP.Dispose();
            hv_CoordSystem_COPY_INP_TMP.Dispose();
            hv_Row_COPY_INP_TMP.Dispose();
            hv_GenParamName.Dispose();
            hv_GenParamValue.Dispose();

            return;
        }
        #endregion
        // Main procedure 
        private DoorAlignResult DoorAlignAction(HObject hb_Image, int x0, int y0, int x1, int y1, int x00, int y00, int x10, int y10, int threholddata = 225)
        {
            // Local iconic variables 
            DoorAlignResult doorAlignResult = default(DoorAlignResult);
            HObject ho_Image = null, ho_Rectangle1 = null, ho_ImageReduced1 = null;
            HObject ho_Image1 = null, ho_Image2 = null, ho_Image3 = null;
            HObject ho_ImageResult1 = null, ho_ImageResult2 = null, ho_ImageResult3 = null;
            HObject ho_ImageScaled1 = null, ho_Region = null, ho_RegionDilation = null;
            HObject ho_RegionFillUp = null, ho_RegionOpen = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions = null, ho_Contours = null, ho_ContoursSplit = null;
            HObject ho_SelectedContours = null, ho_SelectedXLD = null, ho_SortedContours = null;
            HObject ho_ObjectSelected = null, ho_SmoothedContours = null;
            HObject ho_Rectangle2 = null, ho_ImageReduced2 = null, ho_Image4 = null;
            HObject ho_Image5 = null, ho_Image6 = null, ho_ImageResult4 = null;
            HObject ho_ImageResult5 = null, ho_ImageResult6 = null, ho_ImageScaled2 = null;
            HObject ho_Region2 = null, ho_RegionDilation2 = null, ho_RegionFillUp2 = null;
            HObject ho_RegionOpen2 = null, ho_ConnectedRegions2 = null;
            HObject ho_SelectedRegions2 = null, ho_Contours2 = null, ho_ContoursSplit2 = null;
            HObject ho_SelectedContours2 = null, ho_SortedContours2 = null;
            HObject ho_ObjectSelected2 = null, ho_SmoothedContours2 = null;
            HObject ho_SelectedXLD2 = null, ho_RegionLines = null, ho_Cross = null;
            HObject ho_Cross1 = null;

            // Local control variables 

            HTuple hv_threholddata = new HTuple(), hv_DistanceResult = new HTuple();
            HTuple hv_WindowHandle = new HTuple(), hv_Row1 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Row12 = new HTuple();
            HTuple hv_Column12 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Column2 = new HTuple(), hv_Row22 = new HTuple();
            HTuple hv_Column22 = new HTuple(), hv_ImageFiles = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple();
            HTuple hv_Mean = new HTuple(), hv_Deviation = new HTuple();
            HTuple hv_Scale = new HTuple(), hv_Number = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column3 = new HTuple(), hv_Rowxld = new HTuple();
            HTuple hv_Colxld = new HTuple(), hv_Max = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_RowRight = new HTuple();
            HTuple hv_ColRight = new HTuple(), hv_Mean2 = new HTuple();
            HTuple hv_Deviation2 = new HTuple(), hv_Row21 = new HTuple();
            HTuple hv_Column21 = new HTuple(), hv_Phi2 = new HTuple();
            HTuple hv_Length3 = new HTuple(), hv_Length4 = new HTuple();
            HTuple hv_Scale2 = new HTuple(), hv_Number2 = new HTuple();
            HTuple hv_RowBegin = new HTuple(), hv_ColBegin = new HTuple();
            HTuple hv_RowEnd = new HTuple(), hv_ColEnd = new HTuple();
            HTuple hv_Nr = new HTuple(), hv_Nc = new HTuple(), hv_Dist = new HTuple();
            HTuple hv_Area1 = new HTuple(), hv_Row4 = new HTuple();
            HTuple hv_Column4 = new HTuple(), hv_PointOrder1 = new HTuple();
            HTuple hv_Distance = new HTuple(), hv_Distance2 = new HTuple();
            HTuple hv_RowCenter = new HTuple(), hv_ColCenter = new HTuple();
            HTuple hv_Length = new HTuple(), hv_Phi1 = new HTuple();
            HTuple hv_linelenth = new HTuple(), hv_RowExtendstart = new HTuple();
            HTuple hv_ColExtendstart = new HTuple(), hv_RowExtendend = new HTuple();
            HTuple hv_ColExtendend = new HTuple();
            try
            {
                // Initialize local and output iconic variables 
                HOperatorSet.GenEmptyObj(out ho_Image);
                HOperatorSet.GenEmptyObj(out ho_Rectangle1);
                HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
                HOperatorSet.GenEmptyObj(out ho_Image1);
                HOperatorSet.GenEmptyObj(out ho_Image2);
                HOperatorSet.GenEmptyObj(out ho_Image3);
                HOperatorSet.GenEmptyObj(out ho_ImageResult1);
                HOperatorSet.GenEmptyObj(out ho_ImageResult2);
                HOperatorSet.GenEmptyObj(out ho_ImageResult3);
                HOperatorSet.GenEmptyObj(out ho_ImageScaled1);
                HOperatorSet.GenEmptyObj(out ho_Region);
                HOperatorSet.GenEmptyObj(out ho_RegionDilation);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
                HOperatorSet.GenEmptyObj(out ho_RegionOpen);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
                HOperatorSet.GenEmptyObj(out ho_Contours);
                HOperatorSet.GenEmptyObj(out ho_ContoursSplit);
                HOperatorSet.GenEmptyObj(out ho_SelectedContours);
                HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
                HOperatorSet.GenEmptyObj(out ho_SortedContours);
                HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
                HOperatorSet.GenEmptyObj(out ho_SmoothedContours);
                HOperatorSet.GenEmptyObj(out ho_Rectangle2);
                HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
                HOperatorSet.GenEmptyObj(out ho_Image4);
                HOperatorSet.GenEmptyObj(out ho_Image5);
                HOperatorSet.GenEmptyObj(out ho_Image6);
                HOperatorSet.GenEmptyObj(out ho_ImageResult4);
                HOperatorSet.GenEmptyObj(out ho_ImageResult5);
                HOperatorSet.GenEmptyObj(out ho_ImageResult6);
                HOperatorSet.GenEmptyObj(out ho_ImageScaled2);
                HOperatorSet.GenEmptyObj(out ho_Region2);
                HOperatorSet.GenEmptyObj(out ho_RegionDilation2);
                HOperatorSet.GenEmptyObj(out ho_RegionFillUp2);
                HOperatorSet.GenEmptyObj(out ho_RegionOpen2);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions2);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions2);
                HOperatorSet.GenEmptyObj(out ho_Contours2);
                HOperatorSet.GenEmptyObj(out ho_ContoursSplit2);
                HOperatorSet.GenEmptyObj(out ho_SelectedContours2);
                HOperatorSet.GenEmptyObj(out ho_SortedContours2);
                HOperatorSet.GenEmptyObj(out ho_ObjectSelected2);
                HOperatorSet.GenEmptyObj(out ho_SmoothedContours2);
                HOperatorSet.GenEmptyObj(out ho_SelectedXLD2);
                HOperatorSet.GenEmptyObj(out ho_RegionLines);
                HOperatorSet.GenEmptyObj(out ho_Cross);
                HOperatorSet.GenEmptyObj(out ho_Cross1);
                //输入
                //门齐左ROI的Row1, Column1, Row12, Column12
                //门齐右ROI的Row1, Column1, Row12, Column12
                //threholddata,默认225
                //object图片
                hv_Row1 = y0;
                hv_Column1 = x0;
                hv_Row12 = y1;
                hv_Column12 = x1;

                hv_Row2 = y00;
                hv_Column2 = x00;
                hv_Row21 = y10;
                hv_Column21 = x10;
                //输出
                //右门坐标：RowExtendstart,ColExtendstart, RowExtendend, ColExtendend
                //左门中间坐标  Row3, Column3
                //左门右坐标   RowRight  ColRight
                //距离DistanceResult
                hv_threholddata.Dispose();
                hv_threholddata = threholddata;
                hv_DistanceResult.Dispose();
                hv_DistanceResult = 0;
                ho_Image.Dispose();
                HOperatorSet.CopyImage(hb_Image, out ho_Image);
                ho_Rectangle1.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle1, hv_Row1, hv_Column1, hv_Row12, hv_Column12);
                ho_ImageReduced1.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle1, out ho_ImageReduced1);
                ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                HOperatorSet.Decompose3(ho_ImageReduced1, out ho_Image1, out ho_Image2, out ho_Image3);
                ho_ImageResult1.Dispose(); ho_ImageResult2.Dispose(); ho_ImageResult3.Dispose();
                HOperatorSet.TransFromRgb(ho_Image1, ho_Image2, ho_Image3, out ho_ImageResult1, out ho_ImageResult2, out ho_ImageResult3, "hsv");
                hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                HOperatorSet.SmallestRectangle2(ho_Rectangle1, out hv_Row, out hv_Column, out hv_Phi, out hv_Length1, out hv_Length2);
                hv_Mean.Dispose(); hv_Deviation.Dispose();
                HOperatorSet.Intensity(ho_ImageReduced1, ho_ImageReduced1, out hv_Mean, out hv_Deviation);
                hv_Scale.Dispose();
                hv_Scale = 1;
                if ((int)(new HTuple(hv_Mean.TupleLess(230))) != 0)
                {
                    hv_Scale.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Scale = 250 / hv_Mean;
                    }
                }
                if ((int)(new HTuple(hv_Mean.TupleLessEqual(20))) != 0)
                {
                    hv_Scale.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Scale = 200 / hv_Mean;
                    }
                }
                ho_ImageScaled1.Dispose();
                HOperatorSet.ScaleImage(ho_Image1, out ho_ImageScaled1, hv_Scale, 0);
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_ImageScaled1, out ho_Region, hv_threholddata, 255);
                ho_RegionDilation.Dispose();
                HOperatorSet.DilationCircle(ho_Region, out ho_RegionDilation, 5);
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_RegionDilation, out ho_RegionFillUp);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_RegionOpen.Dispose();
                    HOperatorSet.OpeningRectangle1(ho_RegionFillUp, out ho_RegionOpen, hv_Length2,
                        (3 * hv_Length2) / 4);
                }
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionOpen, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                    70);
                hv_Number.Dispose();
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);

                if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                {
                    ho_Contours.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_SelectedRegions, out ho_Contours, "border");
                    ho_ContoursSplit.Dispose();
                    HOperatorSet.SegmentContoursXld(ho_Contours, out ho_ContoursSplit, "lines_circles",
                        3, 20, 20);

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_SelectedContours.Dispose();
                        HOperatorSet.SelectContoursXld(ho_ContoursSplit, out ho_SelectedContours,
                            "contour_length", hv_Length2, (hv_Length2 * 2) + 50, hv_Length2, (hv_Length2 * 2) + 50);
                    }
                    //select_shape_xld (SelectedContours, SelectedXLD1, 'outer_radius', 'and', 40, 2000)
                    ho_SelectedXLD.Dispose();
                    HOperatorSet.SelectShapeXld(ho_SelectedContours, out ho_SelectedXLD, "phi_points",
                        "and", -0.5, 0.5);
                    ho_SortedContours.Dispose();
                    HOperatorSet.SortContoursXld(ho_SelectedXLD, out ho_SortedContours, "upper_left",
                        "true", "row");
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_SortedContours, out ho_ObjectSelected, 1);
                    ho_SmoothedContours.Dispose();
                    HOperatorSet.SmoothContoursXld(ho_ObjectSelected, out ho_SmoothedContours,
                        9);

                    hv_Area.Dispose(); hv_Row3.Dispose(); hv_Column3.Dispose();
                    HOperatorSet.AreaCenterPointsXld(ho_SmoothedContours, out hv_Area, out hv_Row3,
                        out hv_Column3);
                    hv_Rowxld.Dispose(); hv_Colxld.Dispose();
                    HOperatorSet.GetContourXld(ho_SmoothedContours, out hv_Rowxld, out hv_Colxld);
                    hv_Max.Dispose();
                    HOperatorSet.TupleMax(hv_Colxld, out hv_Max);
                    hv_Indices.Dispose();
                    HOperatorSet.TupleFind(hv_Colxld, hv_Max, out hv_Indices);
                    hv_RowRight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_RowRight = hv_Rowxld.TupleSelect(
                            hv_Indices);
                    }
                    hv_ColRight.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_ColRight = hv_Colxld.TupleSelect(
                            hv_Indices);
                    }
                }




                ho_Rectangle2.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle2, hv_Row2, hv_Column2, hv_Row21, hv_Column21);
                ho_ImageReduced2.Dispose();
                HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle2, out ho_ImageReduced2);
                ho_Image4.Dispose(); ho_Image5.Dispose(); ho_Image6.Dispose();
                HOperatorSet.Decompose3(ho_ImageReduced2, out ho_Image4, out ho_Image5, out ho_Image6);
                ho_ImageResult4.Dispose(); ho_ImageResult5.Dispose(); ho_ImageResult6.Dispose();
                HOperatorSet.TransFromRgb(ho_Image4, ho_Image5, ho_Image6, out ho_ImageResult4, out ho_ImageResult5, out ho_ImageResult6, "hsv");
                hv_Mean2.Dispose(); hv_Deviation2.Dispose();
                HOperatorSet.Intensity(ho_ImageReduced2, ho_ImageReduced2, out hv_Mean2, out hv_Deviation2);
                hv_Row21.Dispose(); hv_Column21.Dispose(); hv_Phi2.Dispose(); hv_Length3.Dispose(); hv_Length4.Dispose();
                HOperatorSet.SmallestRectangle2(ho_Rectangle2, out hv_Row21, out hv_Column21, out hv_Phi2, out hv_Length3, out hv_Length4);
                hv_Scale.Dispose();
                hv_Scale = 1;
                if ((int)(new HTuple((new HTuple((new HTuple(20)).TupleLess(hv_Mean2))).TupleLess(
                    230))) != 0)
                {
                    hv_Scale2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Scale2 = 250 / hv_Mean2;
                    }
                }
                if ((int)(new HTuple(hv_Mean2.TupleLessEqual(20))) != 0)
                {
                    hv_Scale2.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Scale2 = 230 / hv_Mean2;
                    }
                }
                ho_ImageScaled2.Dispose();
                HOperatorSet.ScaleImage(ho_Image4, out ho_ImageScaled2, hv_Scale2, 0);
                ho_Region2.Dispose();
                HOperatorSet.Threshold(ho_ImageScaled2, out ho_Region2, hv_threholddata, 255);
                ho_RegionDilation2.Dispose();
                HOperatorSet.DilationCircle(ho_Region2, out ho_RegionDilation2, 5);
                ho_RegionFillUp2.Dispose();
                HOperatorSet.FillUp(ho_RegionDilation2, out ho_RegionFillUp2);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_RegionOpen2.Dispose();
                    HOperatorSet.OpeningRectangle1(ho_RegionFillUp2, out ho_RegionOpen2, hv_Length4,
                        (1 * hv_Length4) / 4);
                }
                ho_ConnectedRegions2.Dispose();
                HOperatorSet.Connection(ho_RegionOpen2, out ho_ConnectedRegions2);
                ho_SelectedRegions2.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions2, out ho_SelectedRegions2, "max_area", 70);
                hv_Number2.Dispose();
                HOperatorSet.CountObj(ho_SelectedRegions2, out hv_Number2);
                if ((int)(new HTuple(hv_Number2.TupleGreater(0))) != 0)
                {
                    ho_Contours2.Dispose();
                    HOperatorSet.GenContourRegionXld(ho_SelectedRegions2, out ho_Contours2, "border");
                    ho_ContoursSplit2.Dispose();
                    HOperatorSet.SegmentContoursXld(ho_Contours2, out ho_ContoursSplit2, "lines_circles", 5, 20, 20);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_SelectedContours2.Dispose();
                        HOperatorSet.SelectContoursXld(ho_ContoursSplit2, out ho_SelectedContours2,
                            "contour_length", hv_Length2, (hv_Length2 * 2) + 350, hv_Length2, (hv_Length2 * 2) + 350);
                    }
                    ho_SortedContours2.Dispose();
                    HOperatorSet.SortContoursXld(ho_SelectedContours2, out ho_SortedContours2,
                        "upper_left", "true", "row");
                    ho_ObjectSelected2.Dispose();
                    HOperatorSet.SelectObj(ho_SortedContours2, out ho_ObjectSelected2, 1);
                    ho_SmoothedContours2.Dispose();
                    HOperatorSet.SmoothContoursXld(ho_ObjectSelected2, out ho_SmoothedContours2,
                        15);
                    hv_RowBegin.Dispose(); hv_ColBegin.Dispose(); hv_RowEnd.Dispose(); hv_ColEnd.Dispose(); hv_Nr.Dispose(); hv_Nc.Dispose(); hv_Dist.Dispose();
                    HOperatorSet.FitLineContourXld(ho_SmoothedContours2, "tukey", -1, 0, 11,
                        2, out hv_RowBegin, out hv_ColBegin, out hv_RowEnd, out hv_ColEnd, out hv_Nr,
                        out hv_Nc, out hv_Dist);
                    ho_SelectedXLD2.Dispose();
                    HOperatorSet.SelectShapeXld(ho_SelectedContours2, out ho_SelectedXLD2, "phi_points",
                        "and", -0.5, 0.5);
                    hv_Area1.Dispose(); hv_Row4.Dispose(); hv_Column4.Dispose(); hv_PointOrder1.Dispose();
                    HOperatorSet.AreaCenterXld(ho_SelectedXLD2, out hv_Area1, out hv_Row4, out hv_Column4,
                        out hv_PointOrder1);
                    if ((int)new HTuple(hv_Number.TupleGreater(0)) != 0)
                    {
                        hv_Distance.Dispose();
                        HOperatorSet.DistancePl(hv_Row3, hv_Column3, hv_RowBegin, hv_ColBegin,
                            hv_RowEnd, hv_ColEnd, out hv_Distance);
                        hv_Distance2.Dispose();
                        HOperatorSet.DistancePl(hv_RowRight, hv_ColRight, hv_RowBegin, hv_ColBegin,
                            hv_RowEnd, hv_ColEnd, out hv_Distance2);
                        doorAlignResult.Distance = hv_DistanceResult.D;
                        doorAlignResult.StartPoint_L = new Point { X = hv_Column3, Y = hv_Row3 };
                        doorAlignResult.EndPoint_L = new Point { X = hv_ColRight, Y = hv_RowRight };
                        doorAlignResult.StartPoint_R = new Point { X = hv_ColExtendstart, Y = hv_RowExtendstart };
                        doorAlignResult.EndPoint_R = new Point { X = hv_ColExtendend, Y = hv_RowExtendend };
                        hv_RowCenter.Dispose();
                        hv_ColCenter.Dispose();
                        hv_Length.Dispose();
                        hv_Phi1.Dispose();
                    }
                }

                //右门坐标：RowExtendstart,ColExtendstart, RowExtendend, ColExtendend
                //左门中间坐标  Row3, Column3
                //左门右坐标   RowRight  ColRight
                //距离DistanceResult
                return doorAlignResult;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ho_Image.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_Image1.Dispose();
                ho_Image2.Dispose();
                ho_Image3.Dispose();
                ho_ImageResult1.Dispose();
                ho_ImageResult2.Dispose();
                ho_ImageResult3.Dispose();
                ho_ImageScaled1.Dispose();
                ho_Region.Dispose();
                ho_RegionDilation.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionOpen.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_Contours.Dispose();
                ho_ContoursSplit.Dispose();
                ho_SelectedContours.Dispose();
                ho_SelectedXLD.Dispose();
                ho_SortedContours.Dispose();
                ho_ObjectSelected.Dispose();
                ho_SmoothedContours.Dispose();
                ho_Rectangle2.Dispose();
                ho_ImageReduced2.Dispose();
                ho_Image4.Dispose();
                ho_Image5.Dispose();
                ho_Image6.Dispose();
                ho_ImageResult4.Dispose();
                ho_ImageResult5.Dispose();
                ho_ImageResult6.Dispose();
                ho_ImageScaled2.Dispose();
                ho_Region2.Dispose();
                ho_RegionDilation2.Dispose();
                ho_RegionFillUp2.Dispose();
                ho_RegionOpen2.Dispose();
                ho_ConnectedRegions2.Dispose();
                ho_SelectedRegions2.Dispose();
                ho_Contours2.Dispose();
                ho_ContoursSplit2.Dispose();
                ho_SelectedContours2.Dispose();
                ho_SortedContours2.Dispose();
                ho_ObjectSelected2.Dispose();
                ho_SmoothedContours2.Dispose();
                ho_SelectedXLD2.Dispose();
                ho_RegionLines.Dispose();
                ho_Cross.Dispose();
                ho_Cross1.Dispose();
                hv_threholddata.Dispose();
                hv_DistanceResult.Dispose();
                hv_WindowHandle.Dispose();
                hv_Row1.Dispose();
                hv_Column1.Dispose();
                hv_Row12.Dispose();
                hv_Column12.Dispose();
                hv_Row2.Dispose();
                hv_Column2.Dispose();
                hv_Row22.Dispose();
                hv_Column22.Dispose();
                hv_ImageFiles.Dispose();
                hv_Index.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_Scale.Dispose();
                hv_Number.Dispose();
                hv_Area.Dispose();
                hv_Row3.Dispose();
                hv_Column3.Dispose();
                hv_Rowxld.Dispose();
                hv_Colxld.Dispose();
                hv_Max.Dispose();
                hv_Indices.Dispose();
                hv_RowRight.Dispose();
                hv_ColRight.Dispose();
                hv_Mean2.Dispose();
                hv_Deviation2.Dispose();
                hv_Row21.Dispose();
                hv_Column21.Dispose();
                hv_Phi2.Dispose();
                hv_Length3.Dispose();
                hv_Length4.Dispose();
                hv_Scale2.Dispose();
                hv_Number2.Dispose();
                hv_RowBegin.Dispose();
                hv_ColBegin.Dispose();
                hv_RowEnd.Dispose();
                hv_ColEnd.Dispose();
                hv_Nr.Dispose();
                hv_Nc.Dispose();
                hv_Dist.Dispose();
                hv_Area1.Dispose();
                hv_Row4.Dispose();
                hv_Column4.Dispose();
                hv_PointOrder1.Dispose();
                hv_Distance.Dispose();
                hv_Distance2.Dispose();
                hv_RowCenter.Dispose();
                hv_ColCenter.Dispose();
                hv_Length.Dispose();
                hv_Phi1.Dispose();
                hv_linelenth.Dispose();
                hv_RowExtendstart.Dispose();
                hv_ColExtendstart.Dispose();
                hv_RowExtendend.Dispose();
                hv_ColExtendend.Dispose();
            }
        }
        public DoorAlignResult DoorAlignAction(string imgPath, Point point1_LT, Point point1_RB, Point point2_LT, Point point2_RB)
        {
            try
            {
                HOperatorSet.ReadImage(out HObject hb_Image, imgPath);
                return DoorAlignAction(hb_Image, point1_LT.Y, point1_LT.X, point1_RB.Y, point1_RB.X, point2_LT.Y, point2_LT.X, point2_RB.Y, point2_RB.X);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DoorAlignResult DoorAlignAction(Bitmap bitmap, Point point1_LT, Point point1_RB, Point point2_LT, Point point2_RB)
        {
            try
            {
                return DoorAlignAction(Ho_ImageHelper.Bitmap2HObject(bitmap), point1_LT.Y, point1_LT.X, point1_RB.Y, point1_RB.X, point2_LT.Y, point2_LT.X, point2_RB.Y, point2_RB.X);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public struct DoorAlignResult
        {
            public double Distance { get; set; }
            public Point StartPoint_L { get; set; }
            public Point EndPoint_L { get; set; }
            public Point StartPoint_R { get; set; }
            public Point EndPoint_R { get; set; }
        }
    }
}
