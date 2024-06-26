﻿using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace HY.Devices.Algorithm.Haier_JiaoNan
{
    /// <summary>
    /// 缺陷检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class SurfaceInspectionByDL : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static SurfaceInspectionByDL _instance;
        public static SurfaceInspectionByDL Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new SurfaceInspectionByDL();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectTarget;
        public override Dictionary<string, dynamic> InitParamNames { get; }= new Dictionary<string, dynamic> { { "class_HdlPath", "E:\\Projects\\JiaoNan\\Fenlei_12091.hdl" }, { "class_HdictPath", "E:\\Projects\\JiaoNan\\Fenlei_12091.hdict" }, { "objection_HdlPath", "E:\\Projects\\JiaoNan\\objection.hdl" }, { "objection_HdictPath", "E:\\Projects\\JiaoNan\\objection.hdict" } };

        public override Dictionary<string, dynamic> ActionParamNames { get; }= new Dictionary<string, dynamic> { { "ImagePath", "" }, { "minGray", 10 }, { "Area", 1200 }, { "Row1", 3593 }, { "Col1", 2365 }, { "length1", 200 }, { "length2", 200 }, { "Offset_Top", 0 }, { "Offset_Left", 0 }, { "Offset_Right", 0 }, { "Offset_Bottom", 0 }, { "HWindow", null }, { "hv_bimageshow", 1 }, { "hv_bregionshow", 1 } };

        public HTuple hv_ExpDefaultWinHandle;
        private HTuple hv_DLPreprocessParamFenlei, hv_DLModelHandleFenlei, hv_WindowhandledictFenlei;
        private HTuple hv_DLPreprocessParamMubiao, hv_DLDatasetMubiao, hv_DLModelHandleMubiao;
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                FenleiInt(initParameters["class_HdlPath"], initParameters["class_HdictPath"], out hv_WindowhandledictFenlei,
                out hv_DLPreprocessParamFenlei, out hv_DLModelHandleFenlei);
                MubiaoIni(initParameters["objection_HdlPath"], initParameters["objection_HdictPath"], out hv_DLPreprocessParamMubiao,
            out hv_DLDatasetMubiao, out hv_DLModelHandleMubiao);
                return IsInit = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        // Main procedure 
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            if (!IsInit)
            {
                throw new Exception("未初始化模型");
            }
            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
            List<TargetResult> quexianResultInfos = new List<TargetResult>();
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];
            hv_ExpDefaultWinHandle = actionParams["HWindow"] as HWindow;
            // Local iconic variables 

            HTuple hv_Exception = new HTuple();

            HObject ho_Rectangle = null, ho_ImagePart = null, ho_ImageReduced = null;
            HObject ho_Image = null, ho_ImageScaled = null;
            HObject ho_MubiaoROI = null, ho_ImagePart1 = null, ho_ConnectedRegions = null;
            HObject ho_ConnectedRegions1 = null, ho_EmptyRegion = null;
            HObject ho_EmptyRegion2 = null, ho_EmptyRegion3 = null, ho_GImage = null;
            HObject ho_BImage = null, ho_RImage = null, ho_Image1 = null;
            HObject ho_Rectangle1 = null, ho_ObjectSelected = null, ho_ImageReduced1 = null;
            HObject ho_ImagePartdomain = null, ho_RegionErosion = null;
            HObject ho_RegionDifference = null, ho_RegionErosion2 = null;
            HObject ho_RegionDifference2 = null, ho_RegionErosion3 = null;
            HObject ho_RegionDifference3 = null, ho_MultiChannelImage = null;

            // Local control variables 

            HTuple hv_QuexianRow1 = new HTuple(), hv_QuexianColumn1 = new HTuple();
            HTuple hv_QuexianRow2 = new HTuple(), hv_QuexianColumn2 = new HTuple();
            HTuple hv_QuexianType = new HTuple(), hv_QuexianArea = new HTuple();
            HTuple hv_Savepath = new HTuple(), hv_ImagePath = new HTuple();
            HTuple hv_threholddata = new HTuple(), hv_Area_huashang_zangwu = new HTuple();
            HTuple hv_Row1 = new HTuple(), hv_Col1 = new HTuple();
            HTuple hv_lenth1 = new HTuple(), hv_lenth2 = new HTuple();
            HTuple hv_bimageshow = new HTuple(), hv_bregionshow = new HTuple();
            HTuple hv_Sigma1 = new HTuple(), hv_Sigma2 = new HTuple();
            HTuple hv_WindowHandle1 = new HTuple(), hv_dictpathFenlei = new HTuple();
            HTuple hv_dllpathFeneli = new HTuple();
            HTuple hv_hdictpathMubiao = new HTuple(), hv_hdlpathMubiao = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Row11 = new HTuple();
            HTuple hv_Column11 = new HTuple(), hv_Row21 = new HTuple();
            HTuple hv_Column21 = new HTuple(), hv_Width = new HTuple();
            HTuple hv_Height = new HTuple(), hv_Area = new HTuple();
            HTuple hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Number1 = new HTuple(), hv_Quexiannum = new HTuple();
            HTuple hv_ResultScore = new HTuple(), hv_Num0 = new HTuple();
            HTuple hv_Num1 = new HTuple(), hv_Num2 = new HTuple();
            HTuple hv_Num3 = new HTuple(), hv_Index2 = new HTuple();
            HTuple hv_cropRow = new HTuple(), hv_cropColumn1 = new HTuple();
            HTuple hv_cropRow2 = new HTuple(), hv_cropColumn2 = new HTuple();
            HTuple hv_DLSample = new HTuple(), hv_DLResult = new HTuple();
            HTuple hv_MSecond1 = new HTuple(), hv_Second1 = new HTuple();
            HTuple hv_Minute1 = new HTuple(), hv_Hour1 = new HTuple();
            HTuple hv_Day1 = new HTuple(), hv_YDay1 = new HTuple();
            HTuple hv_Month1 = new HTuple(), hv_Year1 = new HTuple();
            HTuple hv_BaseName = new HTuple(), hv_Extension = new HTuple();
            HTuple hv_Directory = new HTuple(), hv_path = new HTuple();
            HTuple hv_imagename = new HTuple(), hv_FileExists = new HTuple();
            HTuple hv_QRow1 = new HTuple(), hv_QColumn1 = new HTuple();
            HTuple hv_QRow2 = new HTuple(), hv_QColumn2 = new HTuple();
            HTuple hv_SavepathImage = new HTuple(), hv_message = new HTuple();
            HTuple hv_Indexshow = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImagePart);



            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_MubiaoROI);
            HOperatorSet.GenEmptyObj(out ho_ImagePart1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_EmptyRegion);
            HOperatorSet.GenEmptyObj(out ho_EmptyRegion2);
            HOperatorSet.GenEmptyObj(out ho_EmptyRegion3);
            HOperatorSet.GenEmptyObj(out ho_GImage);
            HOperatorSet.GenEmptyObj(out ho_BImage);
            HOperatorSet.GenEmptyObj(out ho_RImage);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_ImagePartdomain);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference2);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion3);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference3);
            HOperatorSet.GenEmptyObj(out ho_MultiChannelImage);
            try
            {
                //*结果部分
                //缺陷坐标
                hv_QuexianRow1.Dispose();
                hv_QuexianRow1 = 0;
                hv_QuexianColumn1.Dispose();
                hv_QuexianColumn1 = 0;
                hv_QuexianRow2.Dispose();
                hv_QuexianRow2 = 0;
                hv_QuexianColumn2.Dispose();
                hv_QuexianColumn2 = 0;
                //缺陷类型
                hv_QuexianType.Dispose();
                hv_QuexianType = "";
                //面积
                hv_QuexianArea.Dispose();
                hv_QuexianArea = 0;
                //标注存图路径
                hv_Savepath.Dispose();
                hv_Savepath = MarkerImagePath;

                //*参数部分
                //图片路径，可用两个函数表示，一个是直接图片读取
                hv_ImagePath.Dispose();
                //if (actionParams.ContainsKey("Image"))
                //{

                //}
                hv_ImagePath = actionParams["ImagePath"];
                //：输入参数：初始阈值 threholddata
                //白色建议
                hv_threholddata.Dispose();
                hv_threholddata = actionParams["minGray"];
                hv_Area_huashang_zangwu.Dispose();
                hv_Area_huashang_zangwu = actionParams["Area"];

                //预处理参照roi
                hv_Row1.Dispose();
                hv_Row1 = actionParams["Row1"];
                hv_Col1.Dispose();
                hv_Col1 = actionParams["Col1"];
                hv_lenth1.Dispose();
                hv_lenth1 = actionParams["length1"];
                hv_lenth2.Dispose();
                hv_lenth2 = actionParams["length2"];
                //显示图片
                hv_bimageshow.Dispose();
                hv_bimageshow = 0;
                hv_bregionshow.Dispose();
                hv_bregionshow = 0;
                //傅里叶预处理变换参数，可按默认，不放在函数参数内


                hv_Sigma1.Dispose();
                hv_Sigma1 = 10.0;
                hv_Sigma2.Dispose();
                hv_Sigma2 = 3.0;

                //dev_open_window(...);


                //****模型初始化-最好单独放在一个函数，只有开始的时候调用

                //读取图片。
                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, hv_ImagePath);
                //****预处理部分
                //图像预处理，将 Row1, Col1, lenth1, lenth2大小的rOI作为参照，进行图像增强
                ho_ImageScaled.Dispose();
                ProsessImage(ho_Image, out ho_ImageScaled, hv_Row1, hv_Col1, hv_lenth1, hv_lenth2);
                //将图像增强后的图片，作为背景，用目标识别进行 ，640是指当时深度训练时的图片大小，作为转换用
                ho_MubiaoROI.Dispose(); hv_Row11.Dispose(); hv_Column11.Dispose(); hv_Row21.Dispose(); hv_Column21.Dispose();
                /*************王雷1.9 640*640***************/
                Mubiaocheck(ho_ImageScaled, out ho_MubiaoROI, 960, 640, hv_DLModelHandleMubiao,
                    hv_DLPreprocessParamMubiao, out hv_Row11, out hv_Column11, out hv_Row21,
                    out hv_Column21);
                // 20230808 替换
                //ho_ImagePart1.Dispose();
                //if (hv_Row11 != null && hv_Row11 < 1)
                //{
                //    hv_Row11 = 1;
                //}
                //if (hv_Column11 != null && hv_Column11 < 1)
                //{
                //    hv_Column11 = 1;
                //}
                //if (hv_Row21 != null && hv_Row21 < 1)
                //{
                //    hv_Row21 = 1;
                //}
                //if (hv_Column21 != null && hv_Column21 < 1)
                //{
                //    hv_Column21 = 1;
                //}
                //HOperatorSet.CropRectangle1(ho_ImageScaled, out ho_ImagePart1, hv_Row11 + Convert.ToInt32(actionParams["Offset_Top"]),
                //    hv_Column11 + Convert.ToInt32(actionParams["Offset_Left"]), hv_Row21 + Convert.ToInt32(actionParams["Offset_Bottom"]), hv_Column21 + Convert.ToInt32(actionParams["Offset_Right"]));
                //hv_Width.Dispose(); hv_Height.Dispose();
                //HOperatorSet.GetImageSize(ho_ImagePart1, out hv_Width, out hv_Height);
                ////threholddata := 30
                ////Area_huashang_zangwu := 300
                ////经过傅里叶变换的函数输出感兴趣的区域，重要参数是上面两个。15这个参数变了，这些参数也得变更。
                //ho_ConnectedRegions.Dispose(); ho_ConnectedRegions1.Dispose();
                //FftCheck(ho_ImagePart1, out ho_ConnectedRegions, out ho_ConnectedRegions1,
                //    hv_Sigma1, hv_Sigma2, 15, hv_threholddata, hv_Area_huashang_zangwu);
                //*将大条码值扣除，生成分类前区域,12.6号重点改这个,后来发现不扣条码也可以，因为本身条码也有判断
                //cut_code_page (ConnectedRegions, ConnectedRegions1, ImagePart1, CutRegions)
                // 20230808 替换
                //修改 裁剪条码纸
                if ((int)(new HTuple((new HTuple(hv_Row11.TupleLength())).TupleEqual(1))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        //  加入判断
                        if (hv_Row11[0] != null && hv_Row11[0] < 1)
                        {
                            hv_Row11[0] = 1;
                        }
                        if (hv_Column11[0] != null && hv_Column11[0] < 1)
                        {
                            hv_Column11[0] = 1;
                        }
                        if (hv_Row21[0] != null && hv_Row21[0] < 1)
                        {
                            hv_Row21[0] = 1;
                        }
                        if (hv_Column21[0] != null && hv_Column21[0] < 1)
                        {
                            hv_Column21[0] = 1;
                        }
                        HOperatorSet.CropRectangle1(ho_ImageScaled, out ho_ImagePart1, hv_Row11[0] + Convert.ToInt32(actionParams["Offset_Top"]),
                            hv_Column11[0] + Convert.ToInt32(actionParams["Offset_Left"]), hv_Row21[0] + Convert.ToInt32(actionParams["Offset_Bottom"]), hv_Column21[0] + Convert.ToInt32(actionParams["Offset_Right"]));
                        hv_Width.Dispose(); hv_Height.Dispose();
                        //ho_ImagePart1.Dispose();
                        //HOperatorSet.CropRectangle1(ho_ImageScaled, out ho_ImagePart1, hv_Row11.TupleSelect(
                        //    0), hv_Column11.TupleSelect(0), hv_Row21.TupleSelect(0), hv_Column21.TupleSelect(
                        //    0));
                    }
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_ImagePart1, out hv_Width, out hv_Height);
                    ho_ConnectedRegions.Dispose(); ho_ConnectedRegions1.Dispose();
                    FftCheck(ho_ImagePart1, out ho_ConnectedRegions, out ho_ConnectedRegions1,
                        hv_Sigma1, hv_Sigma2, 15, hv_threholddata, hv_Area_huashang_zangwu);

                }
                else if ((int)(new HTuple((new HTuple(hv_Row11.TupleLength())).TupleGreater(
                    1))) != 0)
                {
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ImagePart1.Dispose();
                        //  加入判断
                        if (hv_Row11[0] != null && hv_Row11[0] < 1)
                        {
                            hv_Row11[0] = 1;
                        }
                        if (hv_Column11[0] != null && hv_Column11[0] < 1)
                        {
                            hv_Column11[0] = 1;
                        }
                        if (hv_Row21[0] != null && hv_Row21[0] < 1)
                        {
                            hv_Row21[0] = 1;
                        }
                        if (hv_Column21[0] != null && hv_Column21[0] < 1)
                        {
                            hv_Column21[0] = 1;
                        }
                        HOperatorSet.CropRectangle1(ho_ImageScaled, out ho_ImagePart1, hv_Row11[0] + Convert.ToInt32(actionParams["Offset_Top"]),
                            hv_Column11[0] + Convert.ToInt32(actionParams["Offset_Left"]), hv_Row21[0] + Convert.ToInt32(actionParams["Offset_Bottom"]), hv_Column21[0] + Convert.ToInt32(actionParams["Offset_Right"]));
                        hv_Width.Dispose(); hv_Height.Dispose();

                        //
                        //HOperatorSet.CropRectangle1(ho_ImageScaled, out ho_ImagePart1, hv_Row11.TupleSelect(
                        //    0), hv_Column11.TupleSelect(0), hv_Row21.TupleSelect(0), hv_Column21.TupleSelect(
                        //    0));
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle, (hv_Row11.TupleSelect(1)) - (hv_Row11.TupleSelect(
                            0)), (hv_Column11.TupleSelect(1)) - (hv_Column11.TupleSelect(0)), (hv_Row21.TupleSelect(
                            1)) - (hv_Row11.TupleSelect(0)), (hv_Column21.TupleSelect(1)) - (hv_Column11.TupleSelect(
                            0)));
                    }
                    ho_RegionDifference.Dispose();
                    HOperatorSet.Difference(ho_ImageScaled, ho_Rectangle, out ho_RegionDifference
                        );
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImagePart1, ho_RegionDifference, out ho_ImageReduced
                        );
                    ho_ImagePart.Dispose();
                    HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);
                    //ImagePart1 扣完的图
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_ImagePart1, out hv_Width, out hv_Height);
                    //threholddata := 30
                    //Area_huashang_zangwu := 300
                    //经过傅里叶变换的函数输出感兴趣的区域，重要参数是上面两个。15这个参数变了，这些参数也得变更。
                    ho_ConnectedRegions.Dispose(); ho_ConnectedRegions1.Dispose();
                    FftCheck(ho_ImagePart, out ho_ConnectedRegions, out ho_ConnectedRegions1,
                        hv_Sigma1, hv_Sigma2, 15, hv_threholddata, hv_Area_huashang_zangwu);
                }
                //完成




                hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                HOperatorSet.AreaCenter(ho_ConnectedRegions1, out hv_Area, out hv_Row, out hv_Column);
                //*划伤和条码的生成区域
                ho_EmptyRegion.Dispose();
                HOperatorSet.GenEmptyRegion(out ho_EmptyRegion);
                //*擦伤的生成区域
                ho_EmptyRegion2.Dispose();
                HOperatorSet.GenEmptyRegion(out ho_EmptyRegion2);
                //*脏污的生成区域
                ho_EmptyRegion3.Dispose();
                HOperatorSet.GenEmptyRegion(out ho_EmptyRegion3);
                //*判断区域数量
                hv_Number1.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_Number1 = new HTuple(hv_Area.TupleLength());
                }
                if ((int)(new HTuple(hv_Number1.TupleGreater(0))) != 0)
                {
                    //生成彩色图像模版
                    ho_GImage.Dispose();
                    HOperatorSet.CopyImage(ho_ImagePart1, out ho_GImage);
                    ho_BImage.Dispose();
                    HOperatorSet.CopyImage(ho_ImagePart1, out ho_BImage);
                    ho_RImage.Dispose();
                    HOperatorSet.CopyImage(ho_ImagePart1, out ho_RImage);
                    ho_Image1.Dispose();
                    HOperatorSet.GenImageConst(out ho_Image1, "byte", hv_Width, hv_Height);
                    //划检测区域
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_Row, hv_Column, HTuple.TupleGenConst(
                            hv_Number1, 0), HTuple.TupleGenConst(hv_Number1, 150), HTuple.TupleGenConst(
                            hv_Number1, 150));
                    }

                    //dev_display (ImagePart1)
                    //dev_display (Rectangle1)
                    // 缺陷总数量
                    hv_Quexiannum.Dispose();
                    hv_Quexiannum = 0;
                    hv_QuexianType.Dispose();
                    hv_QuexianType = "";
                    hv_QuexianArea.Dispose();
                    hv_QuexianArea = 0;
                    hv_ResultScore.Dispose();
                    hv_ResultScore = 0;
                    hv_QuexianRow1.Dispose();
                    hv_QuexianRow1 = 0;
                    hv_QuexianColumn1.Dispose();
                    hv_QuexianColumn1 = 0;
                    hv_QuexianRow2.Dispose();
                    hv_QuexianRow2 = 0;
                    hv_QuexianColumn2.Dispose();
                    hv_QuexianColumn2 = 0;
                    // 划伤数量
                    hv_Num0.Dispose();
                    hv_Num0 = 0;
                    // 贴纸数量
                    hv_Num1.Dispose();
                    hv_Num1 = 0;
                    // 擦伤数量
                    hv_Num2.Dispose();
                    hv_Num2 = 0;
                    // 脏污数量
                    hv_Num3.Dispose();
                    hv_Num3 = 0;

                    HTuple end_val122 = hv_Number1;
                    HTuple step_val122 = 1;
                    for (hv_Index2 = 1; hv_Index2.Continue(end_val122, step_val122); hv_Index2 = hv_Index2.TupleAdd(step_val122))
                    {

                        //输出参数，缺陷数量，缺陷轮廓，缺陷类型，面积，截图。默认保存在，以型号+时间+默认顺序命名，数量顺序存放在图片文件,并带数量标识，划伤文件夹，凹凸文件夹，赃物文件夹，误判文件夹
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_Rectangle1, out ho_ObjectSelected, hv_Index2);
                        //12.6号修改，会超出裁剪区域，按照原图会好点。12.6号验证，不能按原图，坐标对应不起来
                        ho_ImageReduced1.Dispose();
                        HOperatorSet.ReduceDomain(ho_ImagePart1, ho_ObjectSelected, out ho_ImageReduced1
                            );
                        hv_cropRow.Dispose(); hv_cropColumn1.Dispose(); hv_cropRow2.Dispose(); hv_cropColumn2.Dispose();
                        HOperatorSet.SmallestRectangle1(ho_ObjectSelected, out hv_cropRow, out hv_cropColumn1,
                            out hv_cropRow2, out hv_cropColumn2);
                        //crop_domain (ImageReduced1, ImagePartdomain)
                        //修改了这个算子可以避免裁剪超出边界的问题。
                        ho_ImagePartdomain.Dispose();
                        HOperatorSet.CropPart(ho_ImagePart1, out ho_ImagePartdomain, hv_cropRow,
                            hv_cropColumn1, 300, 300);
                        hv_DLSample.Dispose();
                        gen_dl_samples_from_images(ho_ImagePartdomain, out hv_DLSample);
                        preprocess_dl_samples(hv_DLSample, hv_DLPreprocessParamFenlei);
                        hv_DLResult.Dispose();
                        HOperatorSet.ApplyDlModel(hv_DLModelHandleFenlei, hv_DLSample, new HTuple(),
                            out hv_DLResult);
                        //按类文件夹保存part小图片信息，图片产品的条码名称和类型保存，12.6号改为汉字保存
                        hv_MSecond1.Dispose(); hv_Second1.Dispose(); hv_Minute1.Dispose(); hv_Hour1.Dispose(); hv_Day1.Dispose(); hv_YDay1.Dispose(); hv_Month1.Dispose(); hv_Year1.Dispose();
                        HOperatorSet.GetSystemTime(out hv_MSecond1, out hv_Second1, out hv_Minute1,
                            out hv_Hour1, out hv_Day1, out hv_YDay1, out hv_Month1, out hv_Year1);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_BaseName.Dispose(); hv_Extension.Dispose(); hv_Directory.Dispose();
                            parse_filename(hv_ImagePath, out hv_BaseName,
                                out hv_Extension, out hv_Directory);
                        }
                        hv_path.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            string dir = $"{PartImagePath}/";
                            if (!System.IO.Directory.Exists(dir))
                            {
                                System.IO.Directory.CreateDirectory(dir);
                            }
                            hv_path = dir + (((hv_DLResult.TupleGetDictTuple(
                                "classification_class_names"))).TupleSelect(0));
                        }
                        hv_imagename.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_imagename = (((((((((hv_path + "/") + hv_BaseName) + "-") + hv_Index2) + "-") + hv_Minute1) + "-") + hv_Second1) + "-") + hv_MSecond1;
                        }
                        hv_FileExists.Dispose();
                        HOperatorSet.FileExists(hv_path, out hv_FileExists);
                        if ((int)(hv_FileExists) != 0)
                        {
                            HOperatorSet.WriteImage(ho_ImagePartdomain, "jpeg", 0, hv_imagename);
                        }
                        else
                        {
                            HOperatorSet.MakeDir(hv_path);
                            HOperatorSet.WriteImage(ho_ImagePartdomain, "jpeg", 0, hv_imagename);
                        }

                        //结果为划伤和条码时，用红色表示。12.6号改为用汉字来分配，这样减少训练的时候的顺序问题。
                        //         if ((int)(new HTuple(((((hv_DLResult.TupleGetDictTuple("classification_class_names"))).TupleSelect(
                        //0))).TupleEqual("划痕"))) != 0 || (int)(new HTuple(((((hv_DLResult.TupleGetDictTuple("classification_class_names"))).TupleSelect(
                        //0))).TupleEqual("小贴纸"))) != 0)
                        //         {
                        //             if ((int)(new HTuple(((((hv_DLResult.TupleGetDictTuple("classification_confidences"))).TupleSelect(
                        //                 0))).TupleGreater(0.5))) != 0)
                        if ((int)(new HTuple(((((hv_DLResult.TupleGetDictTuple("classification_class_names"))).TupleSelect(
                                 0))).TupleEqual("划痕"))) != 0)
                            {
                                if ((int)(new HTuple(((((hv_DLResult.TupleGetDictTuple("classification_confidences"))).TupleSelect(
                                                        0))).TupleGreater(0.80))) != 0)
                                {
                                ho_RegionErosion.Dispose();
                                HOperatorSet.ErosionRectangle1(ho_ObjectSelected, out ho_RegionErosion,
                                    40, 40);
                                ho_RegionDifference.Dispose();
                                HOperatorSet.Difference(ho_ObjectSelected, ho_RegionErosion, out ho_RegionDifference
                                    );
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.Union2(ho_EmptyRegion, ho_RegionDifference, out ExpTmpOutVar_0
                                        );
                                    ho_EmptyRegion.Dispose();
                                    ho_EmptyRegion = ExpTmpOutVar_0;
                                }
                                hv_QRow1.Dispose(); hv_QColumn1.Dispose(); hv_QRow2.Dispose(); hv_QColumn2.Dispose();
                                HOperatorSet.SmallestRectangle1(ho_RegionDifference, out hv_QRow1,
                                    out hv_QColumn1, out hv_QRow2, out hv_QColumn2);
                                if (hv_QuexianType == null)
                                    hv_QuexianType = new HTuple();
                                hv_QuexianType[hv_Quexiannum] = (((hv_DLResult.TupleGetDictTuple(
                                    "classification_class_ids"))).TupleSelect(0)) + (((hv_DLResult.TupleGetDictTuple(
                                    "classification_class_names"))).TupleSelect(0));
                                if (hv_QuexianArea == null)
                                    hv_QuexianArea = new HTuple();
                                hv_QuexianArea[hv_Quexiannum] = "area:" + (hv_Area.TupleSelect(hv_Index2 - 1));
                                if (hv_ResultScore == null)
                                    hv_ResultScore = new HTuple();
                                hv_ResultScore[hv_Quexiannum] = ((hv_DLResult.TupleGetDictTuple("classification_confidences"))).TupleSelect(
                                    0);
                                if (hv_QuexianRow1 == null)
                                    hv_QuexianRow1 = new HTuple();
                                hv_QuexianRow1[hv_Quexiannum] = hv_QRow1;
                                if (hv_QuexianColumn1 == null)
                                    hv_QuexianColumn1 = new HTuple();
                                hv_QuexianColumn1[hv_Quexiannum] = hv_QColumn1;
                                if (hv_QuexianRow2 == null)
                                    hv_QuexianRow2 = new HTuple();
                                hv_QuexianRow2[hv_Quexiannum] = hv_QRow2;
                                if (hv_QuexianColumn2 == null)
                                    hv_QuexianColumn2 = new HTuple();
                                hv_QuexianColumn2[hv_Quexiannum] = hv_QColumn2;
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Num0 = hv_Num0 + 1;
                                        hv_Num0.Dispose();
                                        hv_Num0 = ExpTmpLocalVar_Num0;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Quexiannum = hv_Quexiannum + 1;
                                        hv_Quexiannum.Dispose();
                                        hv_Quexiannum = ExpTmpLocalVar_Quexiannum;
                                    }
                                }
                            }
                        }
                        //      endif
                        //结果为擦伤时，用蓝色表示。 因为擦伤比较不明显，增加判断分数。

                        //if ((int)(new HTuple(((((hv_DLResult.TupleGetDictTuple("classification_class_names"))).TupleSelect(
                        //    0))).TupleEqual("擦伤"))) != 0)
                        //{
                        //    if ((int)(new HTuple(((((hv_DLResult.TupleGetDictTuple("classification_confidences"))).TupleSelect(
                        //        0))).TupleGreater(0.92))) != 0)
                        //    {
                        //        ho_RegionErosion2.Dispose();
                        //        HOperatorSet.ErosionRectangle1(ho_ObjectSelected, out ho_RegionErosion2,
                        //            40, 40);
                        //        ho_RegionDifference2.Dispose();
                        //        HOperatorSet.Difference(ho_ObjectSelected, ho_RegionErosion2, out ho_RegionDifference2
                        //            );
                        //        {
                        //            HObject ExpTmpOutVar_0;
                        //            HOperatorSet.Union2(ho_RegionDifference2, ho_EmptyRegion2, out ExpTmpOutVar_0
                        //                );
                        //            ho_EmptyRegion2.Dispose();
                        //            ho_EmptyRegion2 = ExpTmpOutVar_0;
                        //        }
                        //        hv_QRow1.Dispose(); hv_QColumn1.Dispose(); hv_QRow2.Dispose(); hv_QColumn2.Dispose();
                        //        HOperatorSet.SmallestRectangle1(ho_RegionDifference2, out hv_QRow1,
                        //            out hv_QColumn1, out hv_QRow2, out hv_QColumn2);

                        //        if (hv_QuexianType == null)
                        //            hv_QuexianType = new HTuple();
                        //        hv_QuexianType[hv_Quexiannum] = (((hv_DLResult.TupleGetDictTuple(
                        //            "classification_class_ids"))).TupleSelect(0)) + (((hv_DLResult.TupleGetDictTuple(
                        //            "classification_class_names"))).TupleSelect(0));
                        //        if (hv_QuexianArea == null)
                        //            hv_QuexianArea = new HTuple();
                        //        hv_QuexianArea[hv_Quexiannum] = "area:" + (hv_Area.TupleSelect(hv_Index2 - 1));
                        //        if (hv_ResultScore == null)
                        //            hv_ResultScore = new HTuple();
                        //        hv_ResultScore[hv_Quexiannum] = ((hv_DLResult.TupleGetDictTuple("classification_confidences"))).TupleSelect(
                        //            0);
                        //        if (hv_QuexianRow1 == null)
                        //            hv_QuexianRow1 = new HTuple();
                        //        hv_QuexianRow1[hv_Quexiannum] = hv_QRow1;
                        //        if (hv_QuexianColumn1 == null)
                        //            hv_QuexianColumn1 = new HTuple();
                        //        hv_QuexianColumn1[hv_Quexiannum] = hv_QColumn1;
                        //        if (hv_QuexianRow2 == null)
                        //            hv_QuexianRow2 = new HTuple();
                        //        hv_QuexianRow2[hv_Quexiannum] = hv_QRow2;
                        //        if (hv_QuexianColumn2 == null)
                        //            hv_QuexianColumn2 = new HTuple();
                        //        hv_QuexianColumn2[hv_Quexiannum] = hv_QColumn2;
                        //        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        //        {
                        //            {
                        //                HTuple
                        //                  ExpTmpLocalVar_Quexiannum = hv_Quexiannum + 1;
                        //                hv_Quexiannum.Dispose();
                        //                hv_Quexiannum = ExpTmpLocalVar_Quexiannum;
                        //            }
                        //        }

                        //        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        //        {
                        //            {
                        //                HTuple
                        //                  ExpTmpLocalVar_Num2 = hv_Num2 + 1;
                        //                hv_Num2.Dispose();
                        //                hv_Num2 = ExpTmpLocalVar_Num2;
                        //            }
                        //        }
                        //        //stop ()
                        //    }
                        //}


                        //结果为脏污时，用绿色表示。
                        //12.5号更改，当时是7，边缘，改为6脏污。
                        if ((int)(new HTuple(((((hv_DLResult.TupleGetDictTuple("classification_class_names"))).TupleSelect(
                            0))).TupleEqual("脏污"))) != 0)
                        {
                            if ((int)(new HTuple(((((hv_DLResult.TupleGetDictTuple("classification_confidences"))).TupleSelect(
                                 0))).TupleGreater(0.80))) != 0)
                            {

                                ho_RegionErosion3.Dispose();
                                HOperatorSet.ErosionRectangle1(ho_ObjectSelected, out ho_RegionErosion3,
                                    40, 40);
                                ho_RegionDifference3.Dispose();
                                HOperatorSet.Difference(ho_ObjectSelected, ho_RegionErosion3, out ho_RegionDifference3
                                    );
                                {
                                    HObject ExpTmpOutVar_0;
                                    HOperatorSet.Union2(ho_RegionDifference3, ho_EmptyRegion3, out ExpTmpOutVar_0
                                        );
                                    ho_EmptyRegion3.Dispose();
                                    ho_EmptyRegion3 = ExpTmpOutVar_0;
                                }
                                hv_QRow1.Dispose(); hv_QColumn1.Dispose(); hv_QRow2.Dispose(); hv_QColumn2.Dispose();
                                HOperatorSet.SmallestRectangle1(ho_RegionDifference3, out hv_QRow1,
                                    out hv_QColumn1, out hv_QRow2, out hv_QColumn2);
                                if (hv_QuexianType == null)
                                    hv_QuexianType = new HTuple();
                                hv_QuexianType[hv_Quexiannum] = (((hv_DLResult.TupleGetDictTuple("classification_class_ids"))).TupleSelect(
                                    0)) + (((hv_DLResult.TupleGetDictTuple("classification_class_names"))).TupleSelect(
                                    0));
                                if (hv_QuexianArea == null)
                                    hv_QuexianArea = new HTuple();
                                hv_QuexianArea[hv_Quexiannum] = "area:" + (hv_Area.TupleSelect(hv_Index2 - 1));
                                if (hv_ResultScore == null)
                                    hv_ResultScore = new HTuple();
                                hv_ResultScore[hv_Quexiannum] = ((hv_DLResult.TupleGetDictTuple("classification_confidences"))).TupleSelect(
                                    0);
                                if (hv_QuexianRow1 == null)
                                    hv_QuexianRow1 = new HTuple();
                                hv_QuexianRow1[hv_Quexiannum] = hv_QRow1;
                                if (hv_QuexianColumn1 == null)
                                    hv_QuexianColumn1 = new HTuple();
                                hv_QuexianColumn1[hv_Quexiannum] = hv_QColumn1;
                                if (hv_QuexianRow2 == null)
                                    hv_QuexianRow2 = new HTuple();
                                hv_QuexianRow2[hv_Quexiannum] = hv_QRow2;
                                if (hv_QuexianColumn2 == null)
                                    hv_QuexianColumn2 = new HTuple();
                                hv_QuexianColumn2[hv_Quexiannum] = hv_QColumn2;
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Quexiannum = hv_Quexiannum + 1;
                                        hv_Quexiannum.Dispose();
                                        hv_Quexiannum = ExpTmpLocalVar_Quexiannum;
                                    }
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    {
                                        HTuple
                                          ExpTmpLocalVar_Num3 = hv_Num3 + 1;
                                        hv_Num3.Dispose();
                                        hv_Num3 = ExpTmpLocalVar_Num3;
                                    }
                                }
                            }

                        }


                    }

                    if (hv_ExpDefaultWinHandle.H != IntPtr.Zero)
                    {
                        HOperatorSet.DispObj(ho_ImagePart1, hv_ExpDefaultWinHandle);
                    }
                    HOperatorSet.OverpaintRegion(ho_RImage, ho_EmptyRegion, ((new HTuple(255)).TupleConcat(
                        0)).TupleConcat(0), "fill");
                    HOperatorSet.OverpaintRegion(ho_GImage, ho_EmptyRegion2, ((new HTuple(255)).TupleConcat(
                        0)).TupleConcat(0), "fill");
                    HOperatorSet.OverpaintRegion(ho_BImage, ho_EmptyRegion3, ((new HTuple(255)).TupleConcat(
                        0)).TupleConcat(0), "fill");
                    ho_MultiChannelImage.Dispose();
                    HOperatorSet.Compose3(ho_RImage, ho_GImage, ho_BImage, out ho_MultiChannelImage
                        );

                    hv_SavepathImage.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        if (!System.IO.Directory.Exists(hv_Savepath))
                        {
                            System.IO.Directory.CreateDirectory(hv_Savepath);
                        }
                        hv_SavepathImage = hv_Savepath + "/biaozhun-" + hv_BaseName;
                    }
                    hv_FileExists.Dispose();
                    HOperatorSet.FileExists(hv_Savepath, out hv_FileExists);
                    if ((int)(hv_FileExists) != 0)
                    {
                        HOperatorSet.WriteImage(ho_MultiChannelImage, "jpeg", 0, hv_SavepathImage);
                    }
                    else
                    {
                        HOperatorSet.MakeDir(hv_Savepath);
                        HOperatorSet.WriteImage(ho_MultiChannelImage, "jpeg", 0, hv_SavepathImage);
                    }
                }
                //*分类及显示bimageshow为显示分类后的图像，bregionshow显示未分类前的原图和矩形框
                hv_bimageshow.Dispose();
                hv_bimageshow = Convert.ToInt32(actionParams["hv_bimageshow"]);
                hv_bregionshow.Dispose();
                hv_bregionshow = Convert.ToInt32(actionParams["hv_bregionshow"]);

                //if ((int)(hv_bimageshow) != 0)
                //{
                //    hv_bregionshow.Dispose();
                //    hv_bregionshow = 0;
                //}
                //if ((int)(hv_bregionshow) != 0)
                //{
                //    hv_bimageshow.Dispose();
                //    hv_bimageshow = 0;
                //}
                if ((int)(hv_bimageshow) != 0)
                {
                    if (hv_ExpDefaultWinHandle.H != IntPtr.Zero)
                    {
                        HOperatorSet.DispObj(hv_Quexiannum > 0 ? ho_MultiChannelImage : ho_ImagePart1, hv_ExpDefaultWinHandle);
                    }

                }
                if ((int)(hv_bregionshow) != 0)
                {
                    //正常在C#里用下面的disp_obj
                    if (hv_ExpDefaultWinHandle.H != IntPtr.Zero)
                    {
                        HOperatorSet.DispObj(ho_MultiChannelImage, hv_ExpDefaultWinHandle);
                        HOperatorSet.DispObj(ho_Rectangle1, hv_ExpDefaultWinHandle);
                    }
                    //disp_obj (MultiChannelImage, [H1A1F9104020])
                    //disp_obj (Rectangle1, [H1A1F9104020])
                }

                //显示在每个缺陷旁边，他的缺陷类别和面积，主要可视化使用，软件这块可以用自己的代码实现。
                hv_message.Dispose();
                hv_message = "";
                HTuple end_val254 = hv_Quexiannum - 1;
                HTuple step_val254 = 1;
                if (end_val254 > -1)
                {
                    for (hv_Indexshow = 0; hv_Indexshow.Continue(end_val254, step_val254); hv_Indexshow = hv_Indexshow.TupleAdd(step_val254))
                    {
                        if (hv_ExpDefaultWinHandle.H != IntPtr.Zero)
                        {
                            hv_message.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_message = (("类别" + (hv_QuexianType.TupleSelect(
                                    hv_Indexshow))) + "大小") + (hv_QuexianArea.TupleSelect(hv_Indexshow));
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                disp_message(hv_ExpDefaultWinHandle, hv_message, "image", (hv_QuexianRow2.TupleSelect(
                                    hv_Indexshow)) + 200, hv_QuexianColumn1.TupleSelect(hv_Indexshow), "blue",
                                    "false");
                            }
                        }
                        quexianResultInfos.Add(new TargetResult { Area = double.Parse(hv_QuexianArea.TupleSelect(hv_Indexshow).S.Split(':')[1]), TypeName = hv_QuexianType.TupleSelect(hv_Indexshow).S, Column1 = hv_QuexianColumn1.TupleSelect(hv_Indexshow).I, Column2 = hv_QuexianColumn2.TupleSelect(hv_Indexshow).I, Row1 = hv_QuexianRow1.TupleSelect(hv_Indexshow).I, Row2 = hv_QuexianRow2.TupleSelect(hv_Indexshow).I, Score = hv_ResultScore.TupleSelect(hv_Indexshow).D });
                    }
                }
                results.Add("result", quexianResultInfos);
                return results;




                //模型的释放，可参照之前的释放
                //clear_dl_classifier（DLModelHandleFenlei）
                //clear_dl_classifier（DLModelHandleMubiao）


            }
            catch (HalconException HDevExpDefaultException)
            {

                //throw HDevExpDefaultException;


                HDevExpDefaultException.ToHTuple(out hv_Exception);


                hv_Exception.Dispose();

                results.Add("result", "");
                return results;
            }
            finally
            {

                hv_Exception.Dispose();

                ho_Rectangle.Dispose();
                ho_ImageReduced.Dispose();


                hv_ExpDefaultWinHandle.Dispose();
                ho_Image.Dispose();
                ho_ImageScaled.Dispose();
                ho_MubiaoROI.Dispose();
                ho_ImagePart1.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_EmptyRegion.Dispose();
                ho_EmptyRegion2.Dispose();
                ho_EmptyRegion3.Dispose();
                ho_GImage.Dispose();
                ho_BImage.Dispose();
                ho_RImage.Dispose();
                ho_Image1.Dispose();
                ho_Rectangle1.Dispose();
                ho_ObjectSelected.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ImagePartdomain.Dispose();
                ho_RegionErosion.Dispose();
                ho_RegionDifference.Dispose();
                ho_RegionErosion2.Dispose();
                ho_RegionDifference2.Dispose();
                ho_RegionErosion3.Dispose();
                ho_RegionDifference3.Dispose();
                ho_MultiChannelImage.Dispose();

                hv_QuexianRow1.Dispose();
                hv_QuexianColumn1.Dispose();
                hv_QuexianRow2.Dispose();
                hv_QuexianColumn2.Dispose();
                hv_QuexianType.Dispose();
                hv_QuexianArea.Dispose();
                hv_Savepath.Dispose();
                hv_ImagePath.Dispose();
                hv_threholddata.Dispose();
                hv_Area_huashang_zangwu.Dispose();
                hv_Row1.Dispose();
                hv_Col1.Dispose();
                hv_lenth1.Dispose();
                hv_lenth2.Dispose();
                hv_bimageshow.Dispose();
                hv_bregionshow.Dispose();
                hv_Sigma1.Dispose();
                hv_Sigma2.Dispose();
                hv_WindowHandle1.Dispose();
                hv_dictpathFenlei.Dispose();
                hv_dllpathFeneli.Dispose();
                hv_hdictpathMubiao.Dispose();
                hv_hdlpathMubiao.Dispose();
                hv_Index.Dispose();
                hv_Row11.Dispose();
                hv_Column11.Dispose();
                hv_Row21.Dispose();
                hv_Column21.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Number1.Dispose();
                hv_Quexiannum.Dispose();
                hv_ResultScore.Dispose();
                hv_Num0.Dispose();
                hv_Num1.Dispose();
                hv_Num2.Dispose();
                hv_Num3.Dispose();
                hv_Index2.Dispose();
                hv_cropRow.Dispose();
                hv_cropColumn1.Dispose();
                hv_cropRow2.Dispose();
                hv_cropColumn2.Dispose();
                hv_DLSample.Dispose();
                hv_DLResult.Dispose();
                hv_MSecond1.Dispose();
                hv_Second1.Dispose();
                hv_Minute1.Dispose();
                hv_Hour1.Dispose();
                hv_Day1.Dispose();
                hv_YDay1.Dispose();
                hv_Month1.Dispose();
                hv_Year1.Dispose();
                hv_BaseName.Dispose();
                hv_Extension.Dispose();
                hv_Directory.Dispose();
                hv_path.Dispose();
                hv_imagename.Dispose();
                hv_FileExists.Dispose();
                hv_QRow1.Dispose();
                hv_QColumn1.Dispose();
                hv_QRow2.Dispose();
                hv_QColumn2.Dispose();
                hv_SavepathImage.Dispose();
                hv_message.Dispose();
                hv_Indexshow.Dispose();
                ho_ImagePart.Dispose();

            }
        }
        public override void UnInit()
        {
            hv_DLPreprocessParamFenlei.Dispose();
            hv_DLModelHandleFenlei.Dispose();
            hv_WindowhandledictFenlei.Dispose();
            hv_DLPreprocessParamMubiao.Dispose();
            hv_DLDatasetMubiao.Dispose();
            hv_DLModelHandleMubiao.Dispose();
        }
        #region Local Procedure
        private void cut_code_page(HObject ho_ConnectedRegions, HObject ho_SelectedRegions777,
    HObject ho_Image, out HObject ho_SelectedRegions7)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Rectangle = null, ho_ImageScaled = null;
            HObject ho_Region2 = null, ho_RegionFillUp = null, ho_RegionClosing1 = null;
            HObject ho_RegionOpening = null, ho_RegionTrans = null, ho_RegionErosion1 = null;
            HObject ho_ImageReduced2 = null, ho_Region = null, ho_RegionClosing = null;
            HObject ho_RegionOpening1 = null, ho_RegionTrans1 = null, ho_Rectangle2 = null;
            HObject ho_SelectedRegions77 = null, ho_RegionUnion = null;
            HObject ho_SelectedRegions17 = null, ho_Region1 = null, ho_ConnectedRegions1 = null;
            HObject ho_SelectedRegions = null, ho_SortedRegions = null;
            HObject ho_ObjectSelected = null, ho_RegionDilation = null;
            HObject ho_Lines = null, ho_SelectedRegions1 = null, ho_ObjectSelected1 = null;

            // Local control variables 

            HTuple hv_Mean = new HTuple(), hv_Deviation = new HTuple();
            HTuple hv_Area = new HTuple(), hv_Row = new HTuple(), hv_Column = new HTuple();
            HTuple hv_Mean1 = new HTuple(), hv_Deviation1 = new HTuple();
            HTuple hv_Area1 = new HTuple(), hv_Row3 = new HTuple();
            HTuple hv_Column1 = new HTuple(), hv_Number = new HTuple();
            HTuple hv_Value = new HTuple(), hv_Max1 = new HTuple();
            HTuple hv_Indices = new HTuple(), hv_Number1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions7);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_Region2);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing1);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_RegionErosion1);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced2);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening1);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions77);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions17);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);
            HOperatorSet.GenEmptyObj(out ho_Lines);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected1);
            try
            {
                hv_Mean.Dispose(); hv_Deviation.Dispose();
                HOperatorSet.Intensity(ho_Image, ho_Image, out hv_Mean, out hv_Deviation);
                if ((int)(new HTuple(hv_Mean.TupleGreater(100))) != 0)
                {
                    hv_Area.Dispose(); hv_Row.Dispose(); hv_Column.Dispose();
                    HOperatorSet.AreaCenter(ho_Image, out hv_Area, out hv_Row, out hv_Column);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row, hv_Column - 2000, 0, 200,
                            200);
                    }
                    hv_Mean1.Dispose(); hv_Deviation1.Dispose();
                    HOperatorSet.Intensity(ho_Rectangle, ho_Image, out hv_Mean1, out hv_Deviation1);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_ImageScaled.Dispose();
                        HOperatorSet.ScaleImage(ho_Image, out ho_ImageScaled, 240 / hv_Mean1, 0);
                    }

                    ho_Region2.Dispose();
                    HOperatorSet.Threshold(ho_ImageScaled, out ho_Region2, 150, 255);
                    ho_RegionFillUp.Dispose();
                    HOperatorSet.FillUp(ho_Region2, out ho_RegionFillUp);
                    ho_RegionClosing1.Dispose();
                    HOperatorSet.ClosingRectangle1(ho_RegionFillUp, out ho_RegionClosing1, 100,
                        100);

                    ho_RegionOpening.Dispose();
                    HOperatorSet.OpeningRectangle1(ho_RegionClosing1, out ho_RegionOpening, 4000,
                        1000);
                    ho_RegionTrans.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionOpening, out ho_RegionTrans, "rectangle1");
                    ho_RegionErosion1.Dispose();
                    HOperatorSet.ErosionRectangle1(ho_RegionTrans, out ho_RegionErosion1, 3000,
                        800);
                    ho_ImageReduced2.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image, ho_RegionErosion1, out ho_ImageReduced2
                        );
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced2, out ho_Region, 0, 90);
                    ho_RegionClosing.Dispose();
                    HOperatorSet.ClosingCircle(ho_Region, out ho_RegionClosing, 10);
                    ho_RegionOpening1.Dispose();
                    HOperatorSet.OpeningCircle(ho_RegionClosing, out ho_RegionOpening1, 20);

                    ho_RegionTrans1.Dispose();
                    HOperatorSet.ShapeTrans(ho_RegionOpening1, out ho_RegionTrans1, "rectangle1");
                    hv_Area1.Dispose(); hv_Row3.Dispose(); hv_Column1.Dispose();
                    HOperatorSet.AreaCenter(ho_RegionTrans1, out hv_Area1, out hv_Row3, out hv_Column1);
                    ho_Rectangle2.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_Row3, hv_Column1, 0, 800,
                        800);

                    ho_SelectedRegions77.Dispose();
                    HOperatorSet.Difference(ho_SelectedRegions777, ho_Rectangle2, out ho_SelectedRegions77
                        );
                    ho_RegionUnion.Dispose();
                    HOperatorSet.Union1(ho_SelectedRegions77, out ho_RegionUnion);
                    ho_SelectedRegions17.Dispose();
                    HOperatorSet.Intersection(ho_RegionUnion, ho_RegionOpening, out ho_SelectedRegions17
                        );
                    ho_SelectedRegions7.Dispose();
                    HOperatorSet.Connection(ho_SelectedRegions17, out ho_SelectedRegions7);
                }
                else
                {
                    ho_Region1.Dispose();
                    HOperatorSet.Threshold(ho_Image, out ho_Region1, 200, 255);
                    ho_ConnectedRegions1.Dispose();
                    HOperatorSet.Connection(ho_Region1, out ho_ConnectedRegions1);
                    ho_SelectedRegions.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions, "area",
                        "and", 500000, 999999999);
                    ho_SortedRegions.Dispose();
                    HOperatorSet.SortRegion(ho_SelectedRegions, out ho_SortedRegions, "upper_left",
                        "true", "row");
                    ho_ObjectSelected.Dispose();
                    HOperatorSet.SelectObj(ho_SortedRegions, out ho_ObjectSelected, 1);
                    ho_RegionDilation.Dispose();
                    HOperatorSet.DilationCircle(ho_ObjectSelected, out ho_RegionDilation, 200);
                    ho_SelectedRegions7.Dispose();
                    HOperatorSet.Difference(ho_SelectedRegions777, ho_RegionDilation, out ho_SelectedRegions7
                        );
                    ho_ImageScaled.Dispose();
                    HOperatorSet.ScaleImage(ho_Image, out ho_ImageScaled, 3, 0);
                    ho_Lines.Dispose();
                    HOperatorSet.LinesGauss(ho_ImageScaled, out ho_Lines, 2.5, 2, 5, "dark",
                        "true", "gaussian", "true");
                    ho_Region.Dispose();
                    HOperatorSet.GenRegionContourXld(ho_Lines, out ho_Region, "filled");

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_SelectedRegions1.Dispose();
                        HOperatorSet.SelectShape(ho_Region, out ho_SelectedRegions1, "row", "and",
                            0, 6000 / 2);
                    }

                    hv_Number.Dispose();
                    HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number);
                    if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                    {
                        hv_Value.Dispose();
                        HOperatorSet.RegionFeatures(ho_SelectedRegions1, "width", out hv_Value);
                        hv_Max1.Dispose();
                        HOperatorSet.TupleMax(hv_Value, out hv_Max1);
                        hv_Indices.Dispose();
                        HOperatorSet.TupleFind(hv_Value, hv_Max1, out hv_Indices);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ObjectSelected1.Dispose();
                            HOperatorSet.SelectObj(ho_Region, out ho_ObjectSelected1, hv_Indices + 1);
                        }

                        hv_Number1.Dispose();
                        HOperatorSet.CountObj(ho_ObjectSelected1, out hv_Number1);

                        if ((int)(hv_Number1) != 0)
                        {

                            hv_Area1.Dispose(); hv_Row3.Dispose(); hv_Column1.Dispose();
                            HOperatorSet.AreaCenter(ho_ObjectSelected1, out hv_Area1, out hv_Row3,
                                out hv_Column1);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_Rectangle.Dispose();
                                HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row3, HTuple.TupleGenConst(
                                    hv_Number1, 500), HTuple.TupleGenConst(hv_Number1, 6000), HTuple.TupleGenConst(
                                    hv_Number1, 7400));
                            }

                        }
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.Intersection(ho_SelectedRegions7, ho_Rectangle, out ExpTmpOutVar_0
                                );
                            ho_SelectedRegions7.Dispose();
                            ho_SelectedRegions7 = ExpTmpOutVar_0;
                        }

                    }

                }

                ho_Rectangle.Dispose();
                ho_ImageScaled.Dispose();
                ho_Region2.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionClosing1.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionTrans.Dispose();
                ho_RegionErosion1.Dispose();
                ho_ImageReduced2.Dispose();
                ho_Region.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionOpening1.Dispose();
                ho_RegionTrans1.Dispose();
                ho_Rectangle2.Dispose();
                ho_SelectedRegions77.Dispose();
                ho_RegionUnion.Dispose();
                ho_SelectedRegions17.Dispose();
                ho_Region1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions.Dispose();
                ho_SortedRegions.Dispose();
                ho_ObjectSelected.Dispose();
                ho_RegionDilation.Dispose();
                ho_Lines.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_ObjectSelected1.Dispose();

                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Mean1.Dispose();
                hv_Deviation1.Dispose();
                hv_Area1.Dispose();
                hv_Row3.Dispose();
                hv_Column1.Dispose();
                hv_Number.Dispose();
                hv_Value.Dispose();
                hv_Max1.Dispose();
                hv_Indices.Dispose();
                hv_Number1.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Rectangle.Dispose();
                ho_ImageScaled.Dispose();
                ho_Region2.Dispose();
                ho_RegionFillUp.Dispose();
                ho_RegionClosing1.Dispose();
                ho_RegionOpening.Dispose();
                ho_RegionTrans.Dispose();
                ho_RegionErosion1.Dispose();
                ho_ImageReduced2.Dispose();
                ho_Region.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionOpening1.Dispose();
                ho_RegionTrans1.Dispose();
                ho_Rectangle2.Dispose();
                ho_SelectedRegions77.Dispose();
                ho_RegionUnion.Dispose();
                ho_SelectedRegions17.Dispose();
                ho_Region1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions.Dispose();
                ho_SortedRegions.Dispose();
                ho_ObjectSelected.Dispose();
                ho_RegionDilation.Dispose();
                ho_Lines.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_ObjectSelected1.Dispose();

                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_Area.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Mean1.Dispose();
                hv_Deviation1.Dispose();
                hv_Area1.Dispose();
                hv_Row3.Dispose();
                hv_Column1.Dispose();
                hv_Number.Dispose();
                hv_Value.Dispose();
                hv_Max1.Dispose();
                hv_Indices.Dispose();
                hv_Number1.Dispose();

                throw HDevExpDefaultException;
            }
        }

        private void FenleiInt(HTuple hv_DllPath, HTuple hv_DictPath, out HTuple hv_windowhandledict,
            out HTuple hv_DLPreprocessParam, out HTuple hv_DLModelHandle)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_DLDeviceHandles = new HTuple(), hv_DLDevice = new HTuple();
            HTuple hv_BatchSizeInference = new HTuple(), hv_ClassNames = new HTuple();
            HTuple hv_ClassIDs = new HTuple(), hv_DLDataInfo = new HTuple();
            HTuple hv_NumSamples = new HTuple(), hv_WindowHandleDict = new HTuple();
            // Initialize local and output iconic variables 
            hv_windowhandledict = new HTuple();
            hv_DLPreprocessParam = new HTuple();
            hv_DLModelHandle = new HTuple();
            try
            {
                //读取深度模型并配置参数
                hv_DLPreprocessParam.Dispose();
                HOperatorSet.ReadDict(hv_DictPath, new HTuple(), new HTuple(), out hv_DLPreprocessParam);
                hv_DLModelHandle.Dispose();
                HOperatorSet.ReadDlModel(hv_DllPath, out hv_DLModelHandle);
                hv_DLDeviceHandles.Dispose();
                HOperatorSet.QueryAvailableDlDevices((new HTuple("runtime")).TupleConcat("runtime"),
                    (new HTuple("gpu")).TupleConcat("cpu"), out hv_DLDeviceHandles);
                hv_DLDevice.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DLDevice = hv_DLDeviceHandles.TupleSelect(
                        0);
                }
                hv_BatchSizeInference.Dispose();
                hv_BatchSizeInference = 1;
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", hv_BatchSizeInference);
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "device", hv_DLDevice);
                hv_ClassNames.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_names", out hv_ClassNames);
                hv_ClassIDs.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids", out hv_ClassIDs);
                hv_DLDataInfo.Dispose();
                HOperatorSet.CreateDict(out hv_DLDataInfo);
                HOperatorSet.SetDictTuple(hv_DLDataInfo, "class_names", hv_ClassNames);
                HOperatorSet.SetDictTuple(hv_DLDataInfo, "class_ids", hv_ClassIDs);
                hv_NumSamples.Dispose();
                hv_NumSamples = 8;
                hv_WindowHandleDict.Dispose();
                HOperatorSet.CreateDict(out hv_WindowHandleDict);

                hv_DLDeviceHandles.Dispose();
                hv_DLDevice.Dispose();
                hv_BatchSizeInference.Dispose();
                hv_ClassNames.Dispose();
                hv_ClassIDs.Dispose();
                hv_DLDataInfo.Dispose();
                hv_NumSamples.Dispose();
                hv_WindowHandleDict.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_DLDeviceHandles.Dispose();
                hv_DLDevice.Dispose();
                hv_BatchSizeInference.Dispose();
                hv_ClassNames.Dispose();
                hv_ClassIDs.Dispose();
                hv_DLDataInfo.Dispose();
                hv_NumSamples.Dispose();
                hv_WindowHandleDict.Dispose();

                throw HDevExpDefaultException;
            }
        }

        private void FftCheck(HObject ho_Image, out HObject ho_ConnectedRegions, out HObject ho_ConnectedRegions1,
            HTuple hv_Sigma1, HTuple hv_Sigma2, HTuple hv_Fanxiangzengqiang, HTuple hv_threholddata,
            HTuple hv_QueArea)
        {




            // Local iconic variables 

            HObject ho_GaussFilter1, ho_GaussFilter2, ho_Filter;
            HObject ho_ImageFFT, ho_ImageConvol, ho_ImageFiltered, ho_ImageResult;
            HObject ho_RegionDynThresh, ho_SelectedRegions, ho_RegionUnion;
            HObject ho_RegionClosing;

            // Local control variables 

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_Min = new HTuple(), hv_Max = new HTuple(), hv_Range = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
            HOperatorSet.GenEmptyObj(out ho_GaussFilter1);
            HOperatorSet.GenEmptyObj(out ho_GaussFilter2);
            HOperatorSet.GenEmptyObj(out ho_Filter);
            HOperatorSet.GenEmptyObj(out ho_ImageFFT);
            HOperatorSet.GenEmptyObj(out ho_ImageConvol);
            HOperatorSet.GenEmptyObj(out ho_ImageFiltered);
            HOperatorSet.GenEmptyObj(out ho_ImageResult);
            HOperatorSet.GenEmptyObj(out ho_RegionDynThresh);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            try
            {


                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);
                //创建高低带通滤波器
                ho_GaussFilter1.Dispose();
                HOperatorSet.GenGaussFilter(out ho_GaussFilter1, hv_Sigma1, hv_Sigma1, 0.0,
                    "none", "rft", hv_Width, hv_Height);
                ho_GaussFilter2.Dispose();
                HOperatorSet.GenGaussFilter(out ho_GaussFilter2, hv_Sigma2, hv_Sigma2, 0.0,
                    "none", "rft", hv_Width, hv_Height);
                ho_Filter.Dispose();
                HOperatorSet.SubImage(ho_GaussFilter2, ho_GaussFilter1, out ho_Filter, 1, 0);

                //将图像转到频域
                ho_ImageFFT.Dispose();
                HOperatorSet.RftGeneric(ho_Image, out ho_ImageFFT, "to_freq", "none", "complex",
                    hv_Width);
                //使用上面的带通滤波器进行过滤
                ho_ImageConvol.Dispose();
                HOperatorSet.ConvolFft(ho_ImageFFT, ho_Filter, out ho_ImageConvol);
                //将图像还原到时域
                ho_ImageFiltered.Dispose();
                HOperatorSet.RftGeneric(ho_ImageConvol, out ho_ImageFiltered, "from_freq",
                    "n", "real", hv_Width);
                //用方格内的最大灰度减去最小灰度以实现凸显缺陷的目的
                ho_ImageResult.Dispose();
                HOperatorSet.GrayRangeRect(ho_ImageFiltered, out ho_ImageResult, 15, 15);
                //获取图像内的最大灰度值和最小灰度值，这里可以考虑改成用平均灰度值进行测试，但是目前还未进行测试
                hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                HOperatorSet.MinMaxGray(ho_ImageResult, ho_ImageResult, 0, out hv_Min, out hv_Max,
                    out hv_Range);
                //对图像进行二值化
                ho_RegionDynThresh.Dispose();
                HOperatorSet.Threshold(ho_ImageResult, out ho_RegionDynThresh, hv_threholddata,
                    255);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionDynThresh, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", hv_QueArea, 92000);
                ho_RegionUnion.Dispose();
                HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingRectangle1(ho_RegionUnion, out ho_RegionClosing, 50, 50);
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_RegionClosing, out ho_ConnectedRegions1);

                ho_GaussFilter1.Dispose();
                ho_GaussFilter2.Dispose();
                ho_Filter.Dispose();
                ho_ImageFFT.Dispose();
                ho_ImageConvol.Dispose();
                ho_ImageFiltered.Dispose();
                ho_ImageResult.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionClosing.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_GaussFilter1.Dispose();
                ho_GaussFilter2.Dispose();
                ho_Filter.Dispose();
                ho_ImageFFT.Dispose();
                ho_ImageConvol.Dispose();
                ho_ImageFiltered.Dispose();
                ho_ImageResult.Dispose();
                ho_RegionDynThresh.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionClosing.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();

                throw HDevExpDefaultException;
            }
        }

        public void Mubiaocheck(HObject ho_Image, out HObject ho_MubiaoROI, HTuple hv_ImageWidthXunlian,
            HTuple hv_ImageHeightXunlian, HTuple hv_DLModelHandle, HTuple hv_DLPreprocessParam,
            out HTuple hv_Row11, out HTuple hv_Column11, out HTuple hv_Row21, out HTuple hv_Column21)
        {




            // Local iconic variables 

            HObject ho_Rectangle = null, ho_RegionZoom = null;
            HObject ho_ObjectSelected = null;

            // Local control variables 

            HTuple hv_DLSampleInference = new HTuple();
            HTuple hv_DLResult = new HTuple(), hv_confidence = new HTuple();
            HTuple hv_ID = new HTuple(), hv_Row1 = new HTuple(), hv_Row2 = new HTuple();
            HTuple hv_Col1 = new HTuple(), hv_Col2 = new HTuple();
            HTuple hv_tuple_total = new HTuple(), hv_I = new HTuple();
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HTuple hv_scalex = new HTuple(), hv_scaley = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_MubiaoROI);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_RegionZoom);
            HOperatorSet.GenEmptyObj(out ho_ObjectSelected);
            hv_Row11 = new HTuple();
            hv_Column11 = new HTuple();
            hv_Row21 = new HTuple();
            hv_Column21 = new HTuple();
            try
            {
                hv_DLSampleInference.Dispose();
                gen_dl_samples_from_images(ho_Image, out hv_DLSampleInference);
                preprocess_dl_samples(hv_DLSampleInference, hv_DLPreprocessParam);
                hv_DLResult.Dispose();
                HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleInference, new HTuple(),
                    out hv_DLResult);
                hv_confidence.Dispose();
                HOperatorSet.GetDictTuple(hv_DLResult, "bbox_confidence", out hv_confidence);
                //加入判断语句， DLResult为0时，使用默认
                if ((int)(new HTuple((new HTuple(hv_confidence.TupleLength())).TupleGreater(
                    0))) != 0)
                {
                    hv_ID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_class_id", out hv_ID);
                    hv_Row1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_row1", out hv_Row1);
                    hv_Row2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_row2", out hv_Row2);
                    hv_Col1.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_col1", out hv_Col1);
                    hv_Col2.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResult, "bbox_col2", out hv_Col2);
                    if ((int)(new HTuple((new HTuple(hv_ID.TupleLength())).TupleGreater(1))) != 0)
                    {
                        hv_tuple_total.Dispose();
                        hv_tuple_total = new HTuple();
                        for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_ID.TupleLength())) - 1); hv_I = (int)hv_I + 1)
                        {
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                {
                                    HTuple
                                      ExpTmpLocalVar_tuple_total = ((((((((hv_tuple_total.TupleConcat(
                                        hv_ID.TupleSelect(hv_I)))).TupleConcat(hv_Row1.TupleSelect(hv_I)))).TupleConcat(
                                        hv_Row2.TupleSelect(hv_I)))).TupleConcat(hv_Col1.TupleSelect(hv_I)))).TupleConcat(
                                        hv_Col2.TupleSelect(hv_I));
                                    hv_tuple_total.Dispose();
                                    hv_tuple_total = ExpTmpLocalVar_tuple_total;
                                }
                            }
                        }
                        if ((int)(new HTuple(((hv_tuple_total.TupleSelect(0))).TupleGreater(hv_tuple_total.TupleSelect(
                            5)))) != 0)
                        {
                            hv_Row1.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Row1 = new HTuple();
                                hv_Row1 = hv_Row1.TupleConcat(hv_tuple_total.TupleSelect(
                                    6));
                                hv_Row1 = hv_Row1.TupleConcat(hv_tuple_total.TupleSelect(
                                    1));
                            }
                            hv_Row2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Row2 = new HTuple();
                                hv_Row2 = hv_Row2.TupleConcat(hv_tuple_total.TupleSelect(
                                    7));
                                hv_Row2 = hv_Row2.TupleConcat(hv_tuple_total.TupleSelect(
                                    2));
                            }
                            hv_Col1.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Col1 = new HTuple();
                                hv_Col1 = hv_Col1.TupleConcat(hv_tuple_total.TupleSelect(
                                    8));
                                hv_Col1 = hv_Col1.TupleConcat(hv_tuple_total.TupleSelect(
                                    3));
                            }
                            hv_Col2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Col2 = new HTuple();
                                hv_Col2 = hv_Col2.TupleConcat(hv_tuple_total.TupleSelect(
                                    9));
                                hv_Col2 = hv_Col2.TupleConcat(hv_tuple_total.TupleSelect(
                                    4));
                            }
                        }
                        else
                        {
                            hv_Row1.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Row1 = new HTuple();
                                hv_Row1 = hv_Row1.TupleConcat(hv_tuple_total.TupleSelect(
                                    1));
                                hv_Row1 = hv_Row1.TupleConcat(hv_tuple_total.TupleSelect(
                                    6));
                            }
                            hv_Row2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Row2 = new HTuple();
                                hv_Row2 = hv_Row2.TupleConcat(hv_tuple_total.TupleSelect(
                                    2));
                                hv_Row2 = hv_Row2.TupleConcat(hv_tuple_total.TupleSelect(
                                    7));
                            }
                            hv_Col1.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Col1 = new HTuple();
                                hv_Col1 = hv_Col1.TupleConcat(hv_tuple_total.TupleSelect(
                                    3));
                                hv_Col1 = hv_Col1.TupleConcat(hv_tuple_total.TupleSelect(
                                    8));
                            }
                            hv_Col2.Dispose();
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                hv_Col2 = new HTuple();
                                hv_Col2 = hv_Col2.TupleConcat(hv_tuple_total.TupleSelect(
                                    4));
                                hv_Col2 = hv_Col2.TupleConcat(hv_tuple_total.TupleSelect(
                                    9));
                            }
                        }
                    }

                    //2023年2月9号修改
                    hv_Width.Dispose(); hv_Height.Dispose();
                    HOperatorSet.GetImageSize(ho_Image, out hv_Width, out hv_Height);

                    //置信度0.95有点太高，0.95—>0.70 2023年2月9号修改
                    if ((int)(new HTuple(hv_confidence.TupleGreater(0.70))) != 0)
                    {

                        //下段代码位置不合适，导致Width,Height参数后面无法读取，移动到if语句外
                        //get_image_size (Image, Width, Height)
                        hv_scalex.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_scalex = (hv_Width.TupleReal()
                                ) / (hv_ImageWidthXunlian.TupleReal());
                        }
                        hv_scaley.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_scaley = (hv_Height.TupleReal()
                                ) / (hv_ImageHeightXunlian.TupleReal());
                        }


                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle, hv_Row1, hv_Col1, hv_Row2,
                            hv_Col2);
                        //修改12.5，增加1，因为检测边框时会有-0.24，后面无法裁剪
                        hv_Row11.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Row11 = (hv_Row1 + 1) * hv_scaley;
                        }
                        hv_Column11.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Column11 = hv_Col1 * hv_scalex;
                        }
                        hv_Row21.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Row21 = hv_Row2 * hv_scaley;
                        }
                        hv_Column21.Dispose();
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_Column21 = hv_Col2 * hv_scalex;
                        }

                        ho_RegionZoom.Dispose();
                        HOperatorSet.ZoomRegion(ho_Rectangle, out ho_RegionZoom, hv_scalex, hv_scaley);
                        ho_ObjectSelected.Dispose();
                        HOperatorSet.SelectObj(ho_RegionZoom, out ho_ObjectSelected, 1);
                        ho_MubiaoROI.Dispose();
                        HOperatorSet.CopyObj(ho_ObjectSelected, out ho_MubiaoROI, 1, 1);

                    }
                    else
                    {
                        //从图片中查询 （0，1250）（6000，6800）
                        //    gen_rectangle1 (Rectangle1, 0, 0, Width, Height) 修改此段代码 有产品特性得知下值 2023年2月9号修改
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 1250, 6000, 6800);
                        ho_MubiaoROI.Dispose();
                        HOperatorSet.CopyObj(ho_Rectangle, out ho_MubiaoROI, 1, 1);
                        hv_Row11.Dispose();
                        hv_Row11 = 0;
                        hv_Column11.Dispose();
                        hv_Column11 = 1250;
                        hv_Row21.Dispose();
                        hv_Row21 = 6000;
                        hv_Column21.Dispose();
                        hv_Column21 = 8000;
                    }
                }
                else
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 1250, 6000, 6800);
                    ho_MubiaoROI.Dispose();
                    HOperatorSet.CopyObj(ho_Rectangle, out ho_MubiaoROI, 1, 1);
                    hv_Row11.Dispose();
                    hv_Row11 = 0;
                    hv_Column11.Dispose();
                    hv_Column11 = 1250;
                    hv_Row21.Dispose();
                    hv_Row21 = 6000;
                    hv_Column21.Dispose();
                    hv_Column21 = 8000;
                }

                ho_Rectangle.Dispose();
                ho_RegionZoom.Dispose();
                ho_ObjectSelected.Dispose();

                hv_DLSampleInference.Dispose();
                hv_DLResult.Dispose();
                hv_confidence.Dispose();
                hv_ID.Dispose();
                hv_Row1.Dispose();
                hv_Row2.Dispose();
                hv_Col1.Dispose();
                hv_Col2.Dispose();
                hv_tuple_total.Dispose();
                hv_I.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_scalex.Dispose();
                hv_scaley.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Rectangle.Dispose();
                ho_RegionZoom.Dispose();
                ho_ObjectSelected.Dispose();

                hv_DLSampleInference.Dispose();
                hv_DLResult.Dispose();
                hv_confidence.Dispose();
                hv_ID.Dispose();
                hv_Row1.Dispose();
                hv_Row2.Dispose();
                hv_Col1.Dispose();
                hv_Col2.Dispose();
                hv_tuple_total.Dispose();
                hv_I.Dispose();
                hv_Width.Dispose();
                hv_Height.Dispose();
                hv_scalex.Dispose();
                hv_scaley.Dispose();

                throw HDevExpDefaultException;
            }
        }

        private void MubiaoIni(HTuple hv_hdlpath1, HTuple hv_hdictpath1, out HTuple hv_DLPreprocessParam,
            out HTuple hv_DLDataset, out HTuple hv_DLModelHandle)
        {


            // Initialize local and output iconic variables 
            hv_DLPreprocessParam = new HTuple();
            hv_DLDataset = new HTuple();
            hv_DLModelHandle = new HTuple();

            hv_DLDataset.Dispose();
            HOperatorSet.ReadDict(hv_hdictpath1, new HTuple(), new HTuple(), out hv_DLDataset);
            hv_DLModelHandle.Dispose();
            HOperatorSet.ReadDlModel(hv_hdlpath1, out hv_DLModelHandle);
            hv_DLPreprocessParam.Dispose();
            create_dl_preprocess_param_from_model(hv_DLModelHandle, "false", "full_domain",
                new HTuple(), new HTuple(), new HTuple(), out hv_DLPreprocessParam);
            //每次读取一张
            HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", 1);




            return;
        }

        private void ProsessImage(HObject ho_Image, out HObject ho_ImageScaled, HTuple hv_Row1,
            HTuple hv_Col1, HTuple hv_lenth1, HTuple hv_lenth2)
        {




            // Local iconic variables 

            HObject ho_Rectangle;

            // Local control variables 

            HTuple hv_Mean = new HTuple(), hv_Deviation = new HTuple();
            HTuple hv_scale = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            try
            {


                ho_Rectangle.Dispose();
                HOperatorSet.GenRectangle2(out ho_Rectangle, hv_Row1, hv_Col1, 0, hv_lenth1,
                    hv_lenth2);
                hv_Mean.Dispose(); hv_Deviation.Dispose();
                HOperatorSet.Intensity(ho_Rectangle, ho_Image, out hv_Mean, out hv_Deviation);
                if ((int)(new HTuple(hv_Mean.TupleLess(200))) != 0)
                {
                    if ((int)(new HTuple(hv_Mean.TupleEqual(0))) != 0)
                    {
                        hv_Mean.Dispose();
                        hv_Mean = 50;
                    }
                    hv_scale.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_scale = 100 / hv_Mean;
                    }
                    ho_ImageScaled.Dispose();
                    HOperatorSet.ScaleImage(ho_Image, out ho_ImageScaled, hv_scale, 0);
                }
                else
                {
                    hv_scale.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_scale = 150 / hv_Mean;
                    }
                    ho_ImageScaled.Dispose();
                    HOperatorSet.ScaleImage(ho_Image, out ho_ImageScaled, hv_scale, 0);
                }


                ho_Rectangle.Dispose();

                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_scale.Dispose();

                return;

            }
            catch (HalconException HDevExpDefaultException)
            {
                ho_Rectangle.Dispose();

                hv_Mean.Dispose();
                hv_Deviation.Dispose();
                hv_scale.Dispose();

                throw HDevExpDefaultException;
            }
        }
        #endregion
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

                    throw new HalconException(new HTuple("Only models of type 'anomaly_detection', 'classification', 'detection', or 'segmentation' are supported"));
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
                                    throw new HalconException(((((("The value given in the key '" + hv_Key) + "' of DLPreprocessParam is invalid. Valid types are: ") + hv_ValidTypesListing) + ". The given value was '") + hv_Value) + "'.");
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

        // Chapter: Deep Learning / Model
        // Short Description: Creates a dictionary with preprocessing parameters. 
        public void create_dl_preprocess_param(HTuple hv_DLModelType, HTuple hv_ImageWidth,
            HTuple hv_ImageHeight, HTuple hv_ImageNumChannels, HTuple hv_ImageRangeMin,
            HTuple hv_ImageRangeMax, HTuple hv_NormalizationType, HTuple hv_DomainHandling,
            HTuple hv_IgnoreClassIDs, HTuple hv_SetBackgroundID, HTuple hv_ClassIDsBackground,
            HTuple hv_GenParam, out HTuple hv_DLPreprocessParam)
        {



            // Local control variables 

            HTuple hv_GenParamNames = new HTuple(), hv_GenParamIndex = new HTuple();
            HTuple hv_GenParamValue = new HTuple(), hv_KeysExist = new HTuple();
            HTuple hv_InstanceType = new HTuple(), hv_IsInstanceSegmentation = new HTuple();
            // Initialize local and output iconic variables 
            hv_DLPreprocessParam = new HTuple();
            try
            {
                //
                //This procedure creates a dictionary with all parameters needed for preprocessing.
                //
                hv_DLPreprocessParam.Dispose();
                HOperatorSet.CreateDict(out hv_DLPreprocessParam);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "model_type", hv_DLModelType);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_width", hv_ImageWidth);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_height", hv_ImageHeight);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_num_channels", hv_ImageNumChannels);
                if ((int)(new HTuple(hv_ImageRangeMin.TupleEqual(new HTuple()))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_min", -127);
                }
                else
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_min", hv_ImageRangeMin);
                }
                if ((int)(new HTuple(hv_ImageRangeMax.TupleEqual(new HTuple()))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_max", 128);
                }
                else
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "image_range_max", hv_ImageRangeMax);
                }
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "normalization_type", hv_NormalizationType);
                //Replace possible legacy parameters.
                replace_legacy_preprocessing_parameters(hv_DLPreprocessParam);
                HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "domain_handling", hv_DomainHandling);
                //
                //Set segmentation specific parameters.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("segmentation"))) != 0)
                {
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "ignore_class_ids", hv_IgnoreClassIDs);
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "set_background_id", hv_SetBackgroundID);
                    HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "class_ids_background", hv_ClassIDsBackground);
                }
                //
                //Set generic parameters.
                if ((int)(new HTuple(hv_GenParam.TupleNotEqual(new HTuple()))) != 0)
                {
                    hv_GenParamNames.Dispose();
                    HOperatorSet.GetDictParam(hv_GenParam, "keys", new HTuple(), out hv_GenParamNames);
                    for (hv_GenParamIndex = 0; (int)hv_GenParamIndex <= (int)((new HTuple(hv_GenParamNames.TupleLength()
                        )) - 1); hv_GenParamIndex = (int)hv_GenParamIndex + 1)
                    {
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            hv_GenParamValue.Dispose();
                            HOperatorSet.GetDictTuple(hv_GenParam, hv_GenParamNames.TupleSelect(hv_GenParamIndex),
                                out hv_GenParamValue);
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, hv_GenParamNames.TupleSelect(
                                hv_GenParamIndex), hv_GenParamValue);
                        }
                    }
                }
                //
                //Set necessary default values.
                if ((int)(new HTuple(hv_DLModelType.TupleEqual("detection"))) != 0)
                {
                    hv_KeysExist.Dispose();
                    HOperatorSet.GetDictParam(hv_DLPreprocessParam, "key_exists", ((new HTuple("instance_type")).TupleConcat(
                        "ignore_direction")).TupleConcat("instance_segmentation"), out hv_KeysExist);
                    if ((int)(((hv_KeysExist.TupleSelect(0))).TupleNot()) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "instance_type", "rectangle1");
                    }
                    //Set default for 'ignore_direction' only if instance_type is 'rectangle2'.
                    hv_InstanceType.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "instance_type", out hv_InstanceType);
                    if ((int)((new HTuple(hv_InstanceType.TupleEqual("rectangle2"))).TupleAnd(
                        ((hv_KeysExist.TupleSelect(1))).TupleNot())) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "ignore_direction", 0);
                    }
                    //In case of instance_segmentation we overwrite the instance_type to mask.
                    if ((int)(hv_KeysExist.TupleSelect(2)) != 0)
                    {
                        hv_IsInstanceSegmentation.Dispose();
                        HOperatorSet.GetDictTuple(hv_DLPreprocessParam, "instance_segmentation",
                            out hv_IsInstanceSegmentation);
                        if ((int)(new HTuple((new HTuple(((((new HTuple(1)).TupleConcat(0)).TupleConcat(
                            "true")).TupleConcat("false")).TupleFind(hv_IsInstanceSegmentation))).TupleEqual(
                            -1))) != 0)
                        {
                            throw new HalconException(("Invalid generic parameter for 'instance_segmentation': " + hv_IsInstanceSegmentation) + new HTuple(", only true and false are allowed"));
                        }
                        if ((int)((new HTuple(hv_IsInstanceSegmentation.TupleEqual("true"))).TupleOr(
                            new HTuple(hv_IsInstanceSegmentation.TupleEqual(1)))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_DLPreprocessParam, "instance_type", "mask");
                        }
                    }
                }
                //
                //Check the validity of the preprocessing parameters.
                check_dl_preprocess_param(hv_DLPreprocessParam);
                //

                hv_GenParamNames.Dispose();
                hv_GenParamIndex.Dispose();
                hv_GenParamValue.Dispose();
                hv_KeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_GenParamNames.Dispose();
                hv_GenParamIndex.Dispose();
                hv_GenParamValue.Dispose();
                hv_KeysExist.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Creates a dictionary with the preprocessing parameters based on a given DL model. 
        public void create_dl_preprocess_param_from_model(HTuple hv_DLModelHandle, HTuple hv_NormalizationType,
            HTuple hv_DomainHandling, HTuple hv_SetBackgroundID, HTuple hv_ClassIDsBackground,
            HTuple hv_GenParam, out HTuple hv_DLPreprocessParam)
        {



            // Local iconic variables 

            // Local control variables 

            HTuple hv_ModelType = new HTuple(), hv_ImageWidth = new HTuple();
            HTuple hv_ImageHeight = new HTuple(), hv_ImageNumChannels = new HTuple();
            HTuple hv_ImageRangeMin = new HTuple(), hv_ImageRangeMax = new HTuple();
            HTuple hv_IgnoreClassIDs = new HTuple(), hv_InstanceType = new HTuple();
            HTuple hv_IsInstanceSegmentation = new HTuple(), hv_IgnoreDirection = new HTuple();
            HTuple hv_ClassIDsNoOrientation = new HTuple();
            HTuple hv_GenParam_COPY_INP_TMP = new HTuple(hv_GenParam);

            // Initialize local and output iconic variables 
            hv_DLPreprocessParam = new HTuple();
            try
            {
                //
                //This procedure creates a dictionary with all parameters needed for preprocessing
                //according to a model provided through DLModelHandle.
                //
                //Get the relevant model parameters.
                hv_ModelType.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "type", out hv_ModelType);
                hv_ImageWidth.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_width", out hv_ImageWidth);
                hv_ImageHeight.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_height", out hv_ImageHeight);
                hv_ImageNumChannels.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_num_channels", out hv_ImageNumChannels);
                hv_ImageRangeMin.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_range_min", out hv_ImageRangeMin);
                hv_ImageRangeMax.Dispose();
                HOperatorSet.GetDlModelParam(hv_DLModelHandle, "image_range_max", out hv_ImageRangeMax);
                hv_IgnoreClassIDs.Dispose();
                hv_IgnoreClassIDs = new HTuple();
                //
                //Get model specific parameters.
                if ((int)(new HTuple(hv_ModelType.TupleEqual("anomaly_detection"))) != 0)
                {
                    //No anomaly detection specific parameters.
                }
                else if ((int)(new HTuple(hv_ModelType.TupleEqual("classification"))) != 0)
                {
                    //No classification specific parameters.
                }
                else if ((int)(new HTuple(hv_ModelType.TupleEqual("detection"))) != 0)
                {
                    //Get detection specific parameters.
                    //If GenParam has not been created yet, create it to add new generic parameters.
                    if ((int)(new HTuple((new HTuple(hv_GenParam_COPY_INP_TMP.TupleLength())).TupleEqual(
                        0))) != 0)
                    {
                        hv_GenParam_COPY_INP_TMP.Dispose();
                        HOperatorSet.CreateDict(out hv_GenParam_COPY_INP_TMP);
                    }
                    //Add instance_type.
                    hv_InstanceType.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "instance_type", out hv_InstanceType);
                    //If the model can do instance segmentation, the preprocessing instance_type
                    //needs to be 'mask'.
                    hv_IsInstanceSegmentation.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "instance_segmentation", out hv_IsInstanceSegmentation);
                    if ((int)(new HTuple(hv_IsInstanceSegmentation.TupleEqual("true"))) != 0)
                    {
                        HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "instance_type", "mask");
                    }
                    else
                    {
                        HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "instance_type", hv_InstanceType);
                    }
                    //For instance_type 'rectangle2', add the boolean ignore_direction and class IDs without orientation.
                    if ((int)(new HTuple(hv_InstanceType.TupleEqual("rectangle2"))) != 0)
                    {
                        hv_IgnoreDirection.Dispose();
                        HOperatorSet.GetDlModelParam(hv_DLModelHandle, "ignore_direction", out hv_IgnoreDirection);
                        if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("true"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "ignore_direction",
                                1);
                        }
                        else if ((int)(new HTuple(hv_IgnoreDirection.TupleEqual("false"))) != 0)
                        {
                            HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "ignore_direction",
                                0);
                        }
                        hv_ClassIDsNoOrientation.Dispose();
                        HOperatorSet.GetDlModelParam(hv_DLModelHandle, "class_ids_no_orientation",
                            out hv_ClassIDsNoOrientation);
                        HOperatorSet.SetDictTuple(hv_GenParam_COPY_INP_TMP, "class_ids_no_orientation",
                            hv_ClassIDsNoOrientation);
                    }
                }
                else if ((int)(new HTuple(hv_ModelType.TupleEqual("segmentation"))) != 0)
                {
                    //Get segmentation specific parameters.
                    hv_IgnoreClassIDs.Dispose();
                    HOperatorSet.GetDlModelParam(hv_DLModelHandle, "ignore_class_ids", out hv_IgnoreClassIDs);
                }
                //
                //Create the dictionary with the preprocessing parameters returned by this procedure.
                hv_DLPreprocessParam.Dispose();
                create_dl_preprocess_param(hv_ModelType, hv_ImageWidth, hv_ImageHeight, hv_ImageNumChannels,
                    hv_ImageRangeMin, hv_ImageRangeMax, hv_NormalizationType, hv_DomainHandling,
                    hv_IgnoreClassIDs, hv_SetBackgroundID, hv_ClassIDsBackground, hv_GenParam_COPY_INP_TMP,
                    out hv_DLPreprocessParam);
                //

                hv_GenParam_COPY_INP_TMP.Dispose();
                hv_ModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();

                return;
            }
            catch (HalconException HDevExpDefaultException)
            {

                hv_GenParam_COPY_INP_TMP.Dispose();
                hv_ModelType.Dispose();
                hv_ImageWidth.Dispose();
                hv_ImageHeight.Dispose();
                hv_ImageNumChannels.Dispose();
                hv_ImageRangeMin.Dispose();
                hv_ImageRangeMax.Dispose();
                hv_IgnoreClassIDs.Dispose();
                hv_InstanceType.Dispose();
                hv_IsInstanceSegmentation.Dispose();
                hv_IgnoreDirection.Dispose();
                hv_ClassIDsNoOrientation.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Graphics / Text
        // Short Description: This procedure writes one or multiple text messages. 
        public void disp_message(HTuple hv_WindowHandle, HTuple hv_String, HTuple hv_CoordSystem,
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
            try
            {
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
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(0))).TupleNotEqual(
                        "true"))) != 0)
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
                    else if ((int)(new HTuple(((hv_Box.TupleSelect(1))).TupleNotEqual(
                        "true"))) != 0)
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
            catch (HalconException HDevExpDefaultException)
            {

                hv_Color_COPY_INP_TMP.Dispose();
                hv_Column_COPY_INP_TMP.Dispose();
                hv_CoordSystem_COPY_INP_TMP.Dispose();
                hv_Row_COPY_INP_TMP.Dispose();
                hv_GenParamName.Dispose();
                hv_GenParamValue.Dispose();

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
        public void gen_dl_samples_from_images(HObject ho_Images, out HTuple hv_DLSampleBatch)
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

        // Chapter: File / Misc
        // Short Description: Parse a filename into directory, base filename, and extension 
        public void parse_filename(HTuple hv_FileName, out HTuple hv_BaseName, out HTuple hv_Extension,
            out HTuple hv_Directory)
        {



            // Local control variables 

            HTuple hv_DirectoryTmp = new HTuple(), hv_Substring = new HTuple();
            // Initialize local and output iconic variables 
            hv_BaseName = new HTuple();
            hv_Extension = new HTuple();
            hv_Directory = new HTuple();
            try
            {
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
            catch (HalconException HDevExpDefaultException)
            {

                hv_DirectoryTmp.Dispose();
                hv_Substring.Dispose();

                throw HDevExpDefaultException;
            }
        }

        // Chapter: Deep Learning / Model
        // Short Description: Preprocess anomaly images for evaluation and visualization of the deep-learning-based anomaly detection. 
        public void preprocess_dl_model_anomaly(HObject ho_AnomalyImages, out HObject ho_AnomalyImagesPreprocessed,
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
        public void preprocess_dl_model_images(HObject ho_Images, out HObject ho_ImagesPreprocessed,
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
        public void preprocess_dl_model_segmentations(HObject ho_ImagesRaw, HObject ho_Segmentations,
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
        public void preprocess_dl_samples(HTuple hv_DLSampleBatch, HTuple hv_DLPreprocessParam)
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
