using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;


namespace HY.Devices.Algorithm.Haier_ZhongDe
{
    /// <summary>
    ///3D缝隙算法
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class DepthMap3D : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static DepthMap3D _instance;
        public static DepthMap3D Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new DepthMap3D();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectDoorCrack;

        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" }, { "Coefficient", "0.004" }, { "Param", "1" }, { "Template", "A" }, { "X1", "0" }, { "Y1", "0" }, { "X2", "4000" }, { "Y2", "4000" } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {

            return IsInit = true;
        }

        public override void UnInit()
        {
            IsInit = false;
        }
        public HTuple hv_ExpDefaultWinHandle;

        private void parse_filename(HTuple hv_FileName, out HTuple hv_BaseName, out HTuple hv_Extension,
            out HTuple hv_Directory)
        {



            // Local control variables 

            HTuple hv_DirectoryTmp = new HTuple(), hv_Substring = new HTuple();
            // Initialize local and output iconic variables 
            hv_BaseName = new HTuple();
            hv_Extension = new HTuple();
            hv_Directory = new HTuple();
            //This procedure gets a filename (with full path) as input
            //and returns the directory path, the base filename and the extension
            //in three different strings.
            //
            //In the output path the path separators will be replaced
            //by '/' in all cases.
            //
            //The procedure shows the possibilities of regular expressions in HALCON.
            //
            //Input parameters:
            //FileName: The input filename
            //
            //Output parameters:
            //BaseName: The filename without directory description and file extension
            //Extension: The file extension
            //Directory: The directory path
            //
            //Example:
            //basename('C:/images/part_01.png',...) returns
            //BaseName = 'part_01'
            //Extension = 'png'
            //Directory = 'C:\\images\\' (on Windows systems)
            //
            //Explanation of the regular expressions:
            //
            //'([^\\\\/]*?)(?:\\.[^.]*)?$':
            //To start at the end, the '$' matches the end of the string,
            //so it is best to read the expression from right to left.
            //The part in brackets (?:\\.[^.}*) denotes a non-capturing group.
            //That means, that this part is matched, but not captured
            //in contrast to the first bracketed group ([^\\\\/], see below.)
            //\\.[^.]* matches a dot '.' followed by as many non-dots as possible.
            //So (?:\\.[^.]*)? matches the file extension, if any.
            //The '?' at the end assures, that even if no extension exists,
            //a correct match is returned.
            //The first part in brackets ([^\\\\/]*?) is a capture group,
            //which means, that if a match is found, only the part in
            //brackets is returned as a result.
            //Because both HDevelop strings and regular expressions need a '\\'
            //to describe a backslash, inside regular expressions within HDevelop
            //a backslash has to be written as '\\\\'.
            //[^\\\\/] matches any character but a slash or backslash ('\\' in HDevelop)
            //[^\\\\/]*? matches a string od 0..n characters (except '/' or '\\')
            //where the '?' after the '*' switches the greediness off,
            //that means, that the shortest possible match is returned.
            //This option is necessary to cut off the extension
            //but only if (?:\\.[^.]*)? is able to match one.
            //To summarize, the regular expression matches that part of
            //the input string, that follows after the last '/' or '\\' and
            //cuts off the extension (if any) after the last '.'.
            //
            //'\\.([^.]*)$':
            //This matches everything after the last '.' of the input string.
            //Because ([^.]) is a capturing group,
            //only the part after the dot is returned.
            //
            //'.*[\\\\/]':
            //This matches the longest substring with a '/' or a '\\' at the end.
            //
            hv_DirectoryTmp.Dispose();
            HOperatorSet.TupleRegexpMatch(hv_FileName, ".*[\\\\/]", out hv_DirectoryTmp);
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                hv_Substring.Dispose();
                HOperatorSet.TupleSubstr(hv_FileName, hv_DirectoryTmp.TupleStrlen(), (hv_FileName.TupleStrlen()
                    ) - 1, out hv_Substring);
            }
            hv_BaseName.Dispose();
            HOperatorSet.TupleRegexpMatch(hv_Substring, "([^\\\\/]*?)(?:\\.[^.]*)?$", out hv_BaseName);
            hv_Extension.Dispose();
            HOperatorSet.TupleRegexpMatch(hv_Substring, "\\.([^.]*)$", out hv_Extension);
            //
            //
            //Finally all found backslashes ('\\') are converted
            //to a slash to get consistent paths
            hv_Directory.Dispose();
            HOperatorSet.TupleRegexpReplace(hv_DirectoryTmp, (new HTuple("\\\\")).TupleConcat(
                "replace_all"), "/", out hv_Directory);

            hv_DirectoryTmp.Dispose();
            hv_Substring.Dispose();

            return;
        }


        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {

            if (!IsInit)
            {
                throw new Exception("未初始化模型");
            }
            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
            double[] TotalRes = new double[5];

            // Local iconic variables 

            // Local iconic variables 

            HObject ho_Image, ho_ImagePart, ho_ImageMirror = null;
            HObject ho_Region = null, ho_RegionDifference = null, ho_RegionClosing = null;
            HObject ho_ConnectedRegions = null, ho_ObjectSelectedRegion = null;
            HObject ho_Rectangle = null, ho_RegionIntersection = null, ho_SelectedRegions = null;

            // Local control variables 

            HTuple hv_path = new HTuple(), hv_BaseName = new HTuple();
            HTuple hv_Extension = new HTuple(), hv_Directory = new HTuple();
            HTuple hv_Length = new HTuple(), hv_Name = new HTuple();
            HTuple hv_Coefficient = new HTuple(), hv_param = new HTuple();
            HTuple hv_PointTX = new HTuple(), hv_PointTY = new HTuple();
            HTuple hv_PointBX = new HTuple(), hv_PointBY = new HTuple();
            HTuple hv_Template = new HTuple(), hv_ResWidth = new HTuple();
            HTuple hv_Result = new HTuple(), hv_Result2 = new HTuple();
            HTuple hv_Mean = new HTuple(), hv_Mean2 = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_start = new HTuple(), hv_end = new HTuple();
            HTuple hv_step = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_Preselect_box = new HTuple();
            HTuple hv_Num = new HTuple(), hv_I = new HTuple(), hv_Rows = new HTuple();
            HTuple hv_Columns = new HTuple(), hv_Grayval1 = new HTuple();
            HTuple hv_GrayVal2 = new HTuple(), hv_Greater = new HTuple();
            HTuple hv_Less = new HTuple(), hv_Prod = new HTuple();
            HTuple hv_Selected = new HTuple(), hv_CrackRes = new HTuple();
            HTuple hv_Greater2 = new HTuple(), hv_Less2 = new HTuple();
            HTuple hv_Prod2 = new HTuple(), hv_Selected2 = new HTuple();
            HTuple hv_Res = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImagePart);
            HOperatorSet.GenEmptyObj(out ho_ImageMirror);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelectedRegion);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);



            try
            {

                hv_path.Dispose();
                hv_path = actionParams["Image"];


                hv_BaseName.Dispose(); hv_Extension.Dispose(); hv_Directory.Dispose();
                parse_filename(hv_path, out hv_BaseName, out hv_Extension, out hv_Directory);
                hv_Length.Dispose();
                HOperatorSet.TupleStrlen(hv_BaseName, out hv_Length);
                hv_Name.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Name = (hv_BaseName.TupleStrFirstN(
                        0)) + (hv_BaseName.TupleStrLastN(hv_Length - 1));
                }

                hv_Coefficient.Dispose();
                hv_Coefficient = Convert.ToDouble(actionParams["Coefficient"]);
                hv_param.Dispose();
                hv_param = Convert.ToDouble(actionParams["Param"]);

                //hv_PointTX.Dispose();
                //hv_PointTX = Convert.ToDouble(actionParams["X1"]);
                //hv_PointTY.Dispose();
                //hv_PointTY = Convert.ToDouble(actionParams["Y1"]);
                //hv_PointBX.Dispose();
                //hv_PointBX = Convert.ToDouble(actionParams["X2"]);
                //hv_PointBY.Dispose();
                //hv_PointBY = Convert.ToDouble(actionParams["Y2"]);
                //hv_Template.Dispose();

                hv_PointTX.Dispose();
                hv_PointTX = Convert.ToInt32(actionParams["Y1"]);
                hv_PointTY.Dispose();
                hv_PointTY = Convert.ToInt32(actionParams["X1"]);
                hv_PointBX.Dispose();
                hv_PointBX = Convert.ToInt32(actionParams["Y2"]);
                hv_PointBY.Dispose();
                hv_PointBY = Convert.ToInt32(actionParams["X2"]);

                hv_Template.Dispose();
                hv_Template = "B";


                hv_ResWidth.Dispose();
                hv_ResWidth = 50;
                hv_Result.Dispose();
                hv_Result = new HTuple();
                hv_Result2.Dispose();
                hv_Result2 = new HTuple();
                hv_Mean.Dispose();
                hv_Mean = 1;
                hv_Mean2.Dispose();
                hv_Mean2 = 1;

                //TotalRes[4] = hv_Coefficient;
                //TotalRes[3] = hv_PointTX;
                //TotalRes[2] = hv_PointTY;
                //TotalRes[1] = hv_PointBX;
                //TotalRes[0] = hv_PointBY;

                //TotalRes[0] = 500;
                //TotalRes[1] = 0.005;

                //results.Add("Distance", TotalRes);

                //return results;

                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, hv_path);

                ho_ImagePart.Dispose();
                HOperatorSet.CropRectangle1(ho_Image, out ho_ImagePart, hv_PointTX, hv_PointTY,
                    hv_PointBX, hv_PointBY);
                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_ImagePart, out hv_Width, out hv_Height);
                hv_start.Dispose();
                hv_start = 50;
                hv_end.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_end = hv_Height - 50;
                }
                hv_step.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_step = (hv_Height - 50) / 10;
                }

                if ((int)(new HTuple(hv_Name.TupleEqual("AA"))) != 0)
                {
                    //crop_domain_rel (Image, ImagePart, 2000, 0, 1600, 800)
                    ho_ImageMirror.Dispose();
                    HOperatorSet.MirrorImage(ho_ImagePart, out ho_ImageMirror, "column");
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageMirror, out ho_Region, -15, 255);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_ImageMirror, ho_Region, out ho_RegionDifference);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionDifference, out ho_RegionClosing, 30,
                        50);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_ObjectSelectedRegion.Dispose();
                    HOperatorSet.SelectObj(ho_ConnectedRegions, out ho_ObjectSelectedRegion, 1);
                    hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                    HOperatorSet.AreaCenter(ho_ConnectedRegions, out hv_Area, out hv_Row, out hv_Column);
                    hv_Indices.Dispose();
                    HOperatorSet.TupleSortIndex(hv_Area, out hv_Indices);
                    hv_Length.Dispose();
                    HOperatorSet.TupleLength(hv_Area, out hv_Length);
                    //select_obj (ConnectedRegions, ObjectSelected, Indices[Length-1]+1)
                    hv_Preselect_box.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Preselect_box = HTuple.TupleGenSequence(
                            hv_start, hv_end, hv_step);
                    }
                    hv_Num.Dispose();
                    HOperatorSet.TupleLength(hv_Preselect_box, out hv_Num);
                    HTuple end_val44 = hv_Num - 1;
                    HTuple step_val44 = 1;
                    for (hv_I = 0; hv_I.Continue(end_val44, step_val44); hv_I = hv_I.TupleAdd(step_val44))
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Rectangle.Dispose();
                            HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Preselect_box.TupleSelect(
                                hv_I), 0, hv_Preselect_box.TupleSelect(hv_I), 900);
                        }
                        ho_RegionIntersection.Dispose();
                        HOperatorSet.Intersection(ho_Rectangle, ho_ObjectSelectedRegion, out ho_RegionIntersection
                            );
                        hv_Rows.Dispose(); hv_Columns.Dispose();
                        HOperatorSet.GetRegionPoints(ho_RegionIntersection, out hv_Rows, out hv_Columns);
                        hv_Length.Dispose();
                        HOperatorSet.TupleLength(hv_Rows, out hv_Length);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Result = hv_Result.TupleConcat(
                                    hv_Length);
                                hv_Result.Dispose();
                                hv_Result = ExpTmpLocalVar_Result;
                            }
                        }
                        if ((int)(hv_Length) != 0)
                        {

                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Grayval1.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    0)) - hv_ResWidth, out hv_Grayval1);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_GrayVal2.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    hv_Length - 1)) + hv_ResWidth, out hv_GrayVal2);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Result2 = hv_Result2.TupleConcat(
                                        ((hv_GrayVal2 - hv_Grayval1)).TupleAbs());
                                    hv_Result2.Dispose();
                                    hv_Result2 = ExpTmpLocalVar_Result2;
                                }
                            }
                        }

                    }
                    //stop ()

                }
                else if ((int)(new HTuple(hv_Name.TupleEqual("AB"))) != 0)
                {
                    //crop_domain_rel (Image, ImagePart, 1000, 500, 2500, 0)
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImagePart, out ho_Region, -15, 255);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_ImagePart, ho_Region, out ho_RegionDifference);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionDifference, out ho_RegionClosing, 30,
                        50);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 500, 999999);
                    hv_Preselect_box.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Preselect_box = HTuple.TupleGenSequence(
                            hv_start, hv_end, hv_step);
                    }
                    hv_Num.Dispose();
                    HOperatorSet.TupleLength(hv_Preselect_box, out hv_Num);
                    HTuple end_val69 = hv_Num - 1;
                    HTuple step_val69 = 1;
                    for (hv_I = 0; hv_I.Continue(end_val69, step_val69); hv_I = hv_I.TupleAdd(step_val69))
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Rectangle.Dispose();
                            HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Preselect_box.TupleSelect(
                                hv_I), 0, hv_Preselect_box.TupleSelect(hv_I), 900);
                        }
                        ho_RegionIntersection.Dispose();
                        HOperatorSet.Intersection(ho_Rectangle, ho_SelectedRegions, out ho_RegionIntersection
                            );
                        hv_Rows.Dispose(); hv_Columns.Dispose();
                        HOperatorSet.GetRegionPoints(ho_RegionIntersection, out hv_Rows, out hv_Columns);
                        hv_Length.Dispose();
                        HOperatorSet.TupleLength(hv_Rows, out hv_Length);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Result = hv_Result.TupleConcat(
                                    hv_Length);
                                hv_Result.Dispose();
                                hv_Result = ExpTmpLocalVar_Result;
                            }
                        }
                        if ((int)(hv_Length) != 0)
                        {

                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Grayval1.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    0)) - hv_ResWidth, out hv_Grayval1);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_GrayVal2.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    hv_Length - 1)) + hv_ResWidth, out hv_GrayVal2);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Result2 = hv_Result2.TupleConcat(
                                        ((hv_GrayVal2 - hv_Grayval1)).TupleAbs());
                                    hv_Result2.Dispose();
                                    hv_Result2 = ExpTmpLocalVar_Result2;
                                }
                            }
                        }
                    }

                }
                else if ((int)(new HTuple(hv_Name.TupleEqual("AC"))) != 0)
                {
                    //crop_domain_rel (Image, ImagePart, 1600, 500, 1000, 0)
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImagePart, out ho_Region, -15, 255);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_ImagePart, ho_Region, out ho_RegionDifference);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionDifference, out ho_RegionClosing, 30,
                        50);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 500, 999999);
                    hv_Preselect_box.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Preselect_box = HTuple.TupleGenSequence(
                            hv_start, hv_end, hv_step);
                    }
                    hv_Num.Dispose();
                    HOperatorSet.TupleLength(hv_Preselect_box, out hv_Num);
                    HTuple end_val92 = hv_Num - 1;
                    HTuple step_val92 = 1;
                    for (hv_I = 0; hv_I.Continue(end_val92, step_val92); hv_I = hv_I.TupleAdd(step_val92))
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Rectangle.Dispose();
                            HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Preselect_box.TupleSelect(
                                hv_I), 0, hv_Preselect_box.TupleSelect(hv_I), 900);
                        }
                        ho_RegionIntersection.Dispose();
                        HOperatorSet.Intersection(ho_Rectangle, ho_SelectedRegions, out ho_RegionIntersection
                            );
                        hv_Rows.Dispose(); hv_Columns.Dispose();
                        HOperatorSet.GetRegionPoints(ho_RegionIntersection, out hv_Rows, out hv_Columns);
                        hv_Length.Dispose();
                        HOperatorSet.TupleLength(hv_Rows, out hv_Length);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Result = hv_Result.TupleConcat(
                                    hv_Length);
                                hv_Result.Dispose();
                                hv_Result = ExpTmpLocalVar_Result;
                            }
                        }
                        if ((int)(hv_Length) != 0)
                        {

                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Grayval1.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    0)) - hv_ResWidth, out hv_Grayval1);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_GrayVal2.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    hv_Length - 1)) + hv_ResWidth, out hv_GrayVal2);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Result2 = hv_Result2.TupleConcat(
                                        ((hv_GrayVal2 - hv_Grayval1)).TupleAbs());
                                    hv_Result2.Dispose();
                                    hv_Result2 = ExpTmpLocalVar_Result2;
                                }
                            }
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Name.TupleEqual("BA"))) != 0)
                {
                    //crop_domain_rel (Image, ImagePart, 200, 100, 500, 200)
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImagePart, out ho_Region, -15, 255);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_ImagePart, ho_Region, out ho_RegionDifference);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionDifference, out ho_RegionClosing, 30,
                        50);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 500, 999999);
                    hv_Preselect_box.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Preselect_box = HTuple.TupleGenSequence(
                            hv_start, hv_end, hv_step);
                    }
                    hv_Num.Dispose();
                    HOperatorSet.TupleLength(hv_Preselect_box, out hv_Num);
                    HTuple end_val114 = hv_Num - 1;
                    HTuple step_val114 = 1;
                    for (hv_I = 0; hv_I.Continue(end_val114, step_val114); hv_I = hv_I.TupleAdd(step_val114))
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Rectangle.Dispose();
                            HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Preselect_box.TupleSelect(
                                hv_I), 0, hv_Preselect_box.TupleSelect(hv_I), 900);
                        }
                        ho_RegionIntersection.Dispose();
                        HOperatorSet.Intersection(ho_Rectangle, ho_SelectedRegions, out ho_RegionIntersection
                            );
                        hv_Rows.Dispose(); hv_Columns.Dispose();
                        HOperatorSet.GetRegionPoints(ho_RegionIntersection, out hv_Rows, out hv_Columns);
                        hv_Length.Dispose();
                        HOperatorSet.TupleLength(hv_Rows, out hv_Length);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Result = hv_Result.TupleConcat(
                                    hv_Length);
                                hv_Result.Dispose();
                                hv_Result = ExpTmpLocalVar_Result;
                            }
                        }
                        if ((int)(hv_Length) != 0)
                        {

                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Grayval1.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    0)) - hv_ResWidth, out hv_Grayval1);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_GrayVal2.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    hv_Length - 1)) + hv_ResWidth, out hv_GrayVal2);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Result2 = hv_Result2.TupleConcat(
                                        ((hv_GrayVal2 - hv_Grayval1)).TupleAbs());
                                    hv_Result2.Dispose();
                                    hv_Result2 = ExpTmpLocalVar_Result2;
                                }
                            }
                        }
                    }
                }
                else if ((int)(new HTuple(hv_Name.TupleEqual("BC"))) != 0)
                {
                    //crop_domain_rel (Image, ImagePart, 800, 300, 2700, 100)
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImagePart, out ho_Region, -15, 255);
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_ImagePart, ho_Region, out ho_RegionDifference);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionDifference, out ho_RegionClosing, 30,
                        50);
                    ho_ConnectedRegions.Dispose();
                    HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                        "and", 500, 999999);
                    hv_Preselect_box.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Preselect_box = HTuple.TupleGenSequence(
                            hv_start, hv_end, hv_step);
                    }
                    hv_Num.Dispose();
                    HOperatorSet.TupleLength(hv_Preselect_box, out hv_Num);
                    HTuple end_val136 = hv_Num - 1;
                    HTuple step_val136 = 1;
                    for (hv_I = 0; hv_I.Continue(end_val136, step_val136); hv_I = hv_I.TupleAdd(step_val136))
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Rectangle.Dispose();
                            HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Preselect_box.TupleSelect(
                                hv_I), 0, hv_Preselect_box.TupleSelect(hv_I), 1200);
                        }
                        ho_RegionIntersection.Dispose();
                        HOperatorSet.Intersection(ho_Rectangle, ho_SelectedRegions, out ho_RegionIntersection
                            );
                        hv_Rows.Dispose(); hv_Columns.Dispose();
                        HOperatorSet.GetRegionPoints(ho_RegionIntersection, out hv_Rows, out hv_Columns);
                        hv_Length.Dispose();
                        HOperatorSet.TupleLength(hv_Rows, out hv_Length);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_Result = hv_Result.TupleConcat(
                                    hv_Length);
                                hv_Result.Dispose();
                                hv_Result = ExpTmpLocalVar_Result;
                            }
                        }
                        if ((int)(hv_Length) != 0)
                        {

                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Grayval1.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    0)) - hv_ResWidth, out hv_Grayval1);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_GrayVal2.Dispose();
                                HOperatorSet.GetGrayval(ho_ImagePart, hv_Rows.TupleSelect(0), (hv_Columns.TupleSelect(
                                    hv_Length - 1)) + hv_ResWidth, out hv_GrayVal2);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_Result2 = hv_Result2.TupleConcat(
                                        ((hv_GrayVal2 - hv_Grayval1)).TupleAbs());
                                    hv_Result2.Dispose();
                                    hv_Result2 = ExpTmpLocalVar_Result2;
                                }
                            }
                        }
                    }
                }

                hv_Greater.Dispose();
                HOperatorSet.TupleGreaterElem(hv_Result, 0, out hv_Greater);
                hv_Less.Dispose();
                HOperatorSet.TupleLessElem(hv_Result, 300, out hv_Less);
                hv_Prod.Dispose();
                HOperatorSet.TupleMult(hv_Greater, hv_Less, out hv_Prod);
                hv_Selected.Dispose();
                HOperatorSet.TupleSelectMask(hv_Result, hv_Prod, out hv_Selected);

                if ((int)(new HTuple(hv_Selected.TupleLength())) != 0)
                {
                    hv_Mean.Dispose();
                    HOperatorSet.TupleMean(hv_Selected, out hv_Mean);
                    hv_CrackRes.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_CrackRes = hv_Mean * hv_Coefficient;
                    }
                }
                else
                {
                    hv_CrackRes.Dispose();
                    hv_CrackRes = 0;
                }

                hv_Greater2.Dispose();
                HOperatorSet.TupleGreaterElem(hv_Result2, 0, out hv_Greater2);
                hv_Less2.Dispose();
                HOperatorSet.TupleLessElem(hv_Result2, 2, out hv_Less2);
                hv_Prod2.Dispose();
                HOperatorSet.TupleMult(hv_Greater2, hv_Less2, out hv_Prod2);
                hv_Selected2.Dispose();
                HOperatorSet.TupleSelectMask(hv_Result2, hv_Prod2, out hv_Selected2);
                if ((int)(new HTuple(hv_Selected2.TupleLength())) != 0)
                {
                    hv_Mean2.Dispose();
                    HOperatorSet.TupleMean(hv_Selected2, out hv_Mean2);
                    hv_Res.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Res = hv_Mean2 * hv_param;
                    }
                }
                else
                {
                    hv_Res.Dispose();
                    hv_Res = 0;
                }

                TotalRes[0] = hv_CrackRes.D;
                TotalRes[1] = hv_Res.D;
                results.Add("Distance", TotalRes);

                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {

                ho_Image.Dispose();
                ho_ImagePart.Dispose();
                ho_ImageMirror.Dispose();
                ho_Region.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionClosing.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_ObjectSelectedRegion.Dispose();
                ho_Rectangle.Dispose();
                ho_RegionIntersection.Dispose();
                ho_SelectedRegions.Dispose();

                hv_path.Dispose();
                hv_BaseName.Dispose();
                hv_Extension.Dispose();
                hv_Directory.Dispose();
                hv_Length.Dispose();
                hv_Name.Dispose();
                hv_Coefficient.Dispose();
                hv_param.Dispose();
                hv_PointTX.Dispose();
                hv_PointTY.Dispose();
                hv_PointBX.Dispose();
                hv_PointBY.Dispose();
                hv_Template.Dispose();
                hv_ResWidth.Dispose();
                hv_Result.Dispose();
                hv_Result2.Dispose();
                hv_Mean.Dispose();
                hv_Mean2.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_start.Dispose();
                hv_end.Dispose();
                hv_step.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Indices.Dispose();
                hv_Preselect_box.Dispose();
                hv_Num.Dispose();
                hv_I.Dispose();
                hv_Rows.Dispose();
                hv_Columns.Dispose();
                hv_Grayval1.Dispose();
                hv_GrayVal2.Dispose();
                hv_Greater.Dispose();
                hv_Less.Dispose();
                hv_Prod.Dispose();
                hv_Selected.Dispose();
                hv_CrackRes.Dispose();
                hv_Greater2.Dispose();
                hv_Less2.Dispose();
                hv_Prod2.Dispose();
                hv_Selected2.Dispose();
                hv_Res.Dispose();
            }



        }

    }
}
