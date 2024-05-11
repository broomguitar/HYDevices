
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HalconDotNet;
using Newtonsoft.Json;

namespace HY.Devices.Algorithm.YanFeng
{

    [Export(typeof(IAlgorithm))]

    public class YFDetect : AbstractAlgorithm, IAlgorithm
    {

        private static readonly object _lockObj = new object();
        private static YFDetect _instance;
        public static YFDetect Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new YFDetect();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectTarget;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic> { { "class_HdlPath", "D:/0_halcon/延锋/20240312目标检测/wlbest240307.hdl" }, { "class_HdictPath", "D:/0_halcon/延锋/20240312目标检测/model_best_info.hdict" } };

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" }, { "Params", "[{\"Name\":\"大毛毡\",\"X\":2438.0,\"Y\":1850.0,\"Width\":178.0,\"Height\":206.0,\"Guid\":\"b5c8c16b-d192-437e-9483-1a84c5b325eb\"},{\"Name\":\"大毛毡\",\"X\":2008.0,\"Y\":1800.0,\"Width\":199.0,\"Height\":217.0,\"Guid\":\"0afa9bd9-c966-4d5e-b44b-03cad445f089\"},{\"Name\":\"魔术扣\",\"X\":2239.0,\"Y\":1867.0,\"Width\":199.0,\"Height\":200.0,\"Guid\":\"a0a3eb7c-7866-434a-978e-809bc5cd2478\"},{\"Name\":\"银色卡扣\",\"X\":2532.0,\"Y\":724.0,\"Width\":199.0,\"Height\":105.0,\"Guid\":\"390e356a-c2a2-49fa-9900-e733543964fb\"},{\"Name\":\"银色卡扣\",\"X\":1966.0,\"Y\":763.0,\"Width\":199.0,\"Height\":139.0,\"Guid\":\"797c6906-cace-4bda-8535-b35b0f0e98ff\"},{\"Name\":\"银色卡扣\",\"X\":1578.0,\"Y\":769.0,\"Width\":230.0,\"Height\":156.0,\"Guid\":\"f4e56939-f799-46c0-a54c-d9db53f68973\"},{\"Name\":\"银色卡扣\",\"X\":1002.0,\"Y\":914.0,\"Width\":199.0,\"Height\":133.0,\"Guid\":\"21fa2a4c-0518-4be3-91ca-ec1fef76aaeb\"},{\"Name\":\"金色卡扣\",\"X\":803.0,\"Y\":1527.0,\"Width\":167.0,\"Height\":122.0,\"Guid\":\"d1f8f72e-94e9-4903-865c-d23c5bedb89b\"},{\"Name\":\"金色卡扣\",\"X\":677.0,\"Y\":1805.0,\"Width\":167.0,\"Height\":117.0,\"Guid\":\"681182ff-2d69-45ed-a0bc-d508acff03ae\"},{\"Name\":\"金色卡扣\",\"X\":908.0,\"Y\":1755.0,\"Width\":220.0,\"Height\":122.0,\"Guid\":\"54afebd9-7af6-4a11-b5cf-3d0130686d7e\"},{\"Name\":\"金色卡扣\",\"X\":3234.0,\"Y\":1794.0,\"Width\":188.0,\"Height\":144.0,\"Guid\":\"3db04001-7163-458e-a2c6-e6838e4d7823\"},{\"Name\":\"金色卡扣\",\"X\":3255.0,\"Y\":1270.0,\"Width\":188.0,\"Height\":133.0,\"Guid\":\"4e213b76-606c-4ab9-97ad-14be6231f20f\"},{\"Name\":\"安全带出口盖板\",\"X\":636.0,\"Y\":1978.0,\"Width\":649.0,\"Height\":412.0,\"Guid\":\"b2bcb36d-e79f-4228-b724-4a1b67e50c0e\"}]" }, { "HWindow", null }, { "hv_bimageshow", 1 }, { "hv_bregionshow", 1 } };

        public HTuple hv_ExpDefaultWinHandle;
        HTuple hv_DLDataset = new HTuple(), hv_DLModelHandle = new HTuple();
        HTuple hv_DLDictHandleModel = new HTuple(), hv_DLModelHandleModel = new HTuple();
        HTuple hv_DLPreprocessParam = new HTuple(), hv_DLDeviceHandles = new HTuple(), hv_DLDevice = new HTuple();




        public override bool Init(Dictionary<string, dynamic> InitParam)
        {

            try
            {
                //读取目标检测模型
                hv_DLDataset.Dispose();
                HOperatorSet.ReadDict("D:/20240125新增按钮/HYDetect/Algorithms/detect.hdict",
                //HOperatorSet.ReadDict("D:/HySourceCode/HYDevices/Algorithm/HY.Devices.AlgorithmDemo/bin/延锋1_dl_preprocess_params.hdict",

                new HTuple(), new HTuple(), out hv_DLDataset);
                hv_DLModelHandle.Dispose();
                HOperatorSet.ReadDlModel("D:/20240125新增按钮/HYDetect/Algorithms/detect.hdl",
                //HOperatorSet.ReadDlModel("D:/HySourceCode/HYDevices/Algorithm/HY.Devices.AlgorithmDemo/bin/延锋1.hdl",

                    out hv_DLModelHandle);

                //读取分类模型
                hv_DLDictHandleModel.Dispose();
                HOperatorSet.ReadDict("D:/20240125新增按钮/HYDetect/Algorithms/model.hdict",
                //HOperatorSet.ReadDict("D:/HySourceCode/HYDevices/Algorithm/HY.Devices.Algorithm.YanFeng/bin/Debug/model.hdict",
                    new HTuple(), new HTuple(), out hv_DLDictHandleModel);
                hv_DLModelHandleModel.Dispose();
                HOperatorSet.ReadDlModel("D:/20240125新增按钮/HYDetect/Algorithms/model.hdl",
                //HOperatorSet.ReadDlModel("D:/HySourceCode/HYDevices/Algorithm/HY.Devices.Algorithm.YanFeng/bin/Debug/model.hdl",
                    out hv_DLModelHandleModel);

                //设置目标检测参数
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "batch_size", 1);
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "max_overlap", 0.3);
                HOperatorSet.SetDlModelParam(hv_DLModelHandle, "max_overlap_class_agnostic",
                    0.3);
                hv_DLPreprocessParam.Dispose();
                create_dl_preprocess_param_from_model(hv_DLModelHandle, "false", "full_domain",
                    new HTuple(), new HTuple(), new HTuple(), out hv_DLPreprocessParam);
                hv_DLDeviceHandles.Dispose();
                HOperatorSet.QueryAvailableDlDevices((new HTuple("runtime")).TupleConcat("runtime"),
                    (new HTuple("gpu")).TupleConcat("cpu"), out hv_DLDeviceHandles);
                hv_DLDevice.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_DLDevice = hv_DLDeviceHandles.TupleSelect(
                        0);
                }
                HOperatorSet.SetDlModelParam(hv_DLModelHandleModel, "batch_size", 1);
                HOperatorSet.SetDlModelParam(hv_DLModelHandleModel, "device", hv_DLDevice);

                return IsInit = true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
            }
        }


        public override void UnInit()
        {
            hv_DLDataset.Dispose();
            hv_DLModelHandle.Dispose();
            hv_DLDictHandleModel.Dispose();
            hv_DLModelHandleModel.Dispose();
            hv_DLPreprocessParam.Dispose();
            hv_DLDeviceHandles.Dispose();
            hv_DLDevice.Dispose();

        }



        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {

            if (!IsInit)
            {
                throw new Exception("未初始化模型");
            }
            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
            List<int> CoverplateInfos = new List<int>();
            List<int> BoltInfos = new List<int>();
            List<int> FeltInfos = new List<int>();
            List<int> MetalInfos = new List<int>();


            HObject[] OTemp = new HObject[20];
            hv_ExpDefaultWinHandle = actionParams["HWindow"] as HWindow;
            // Local iconic variables 

            HObject ho_Image, ho_ImageAffineTransPaints;
            HObject ho_Rectangle = null, ho_ImageReduced = null, ho_ImagePart = null;
            HObject ho_Rectangle1 = null, ho_Rectangle2 = null, ho_RegionDifference = null;

            // Local control variables 


            HTuple hv_ImagePath = new HTuple();
            HTuple hv_BaseName = new HTuple(), hv_Extension = new HTuple();
            HTuple hv_Directory = new HTuple(), hv_I = new HTuple();
            //HTuple hv_domain = new HTuple(), hv_Name = new HTuple();
            //HTuple hv_x = new HTuple(), hv_y = new HTuple(), hv_h = new HTuple();
            //HTuple hv_w = new HTuple();
            HTuple hv_FileExists = new HTuple(), hv_path = new HTuple();
            HTuple hv_MSecond1 = new HTuple(), hv_Second1 = new HTuple();
            HTuple hv_Minute1 = new HTuple(), hv_Hour1 = new HTuple();
            HTuple hv_Day1 = new HTuple(), hv_YDay1 = new HTuple();
            HTuple hv_Month1 = new HTuple(), hv_Year1 = new HTuple();
            HTuple hv_Imagename = new HTuple();
            HTuple hv_savepath = new HTuple(), hv_imageSavePath = new HTuple();

            HTuple hv_DLSampleInference = new HTuple(), hv_DLResultDetect = new HTuple();
            HTuple hv_DLSample = new HTuple(), hv_DLResultModel = new HTuple();
            HTuple hv_Detectconfidence = new HTuple(), hv_DetectID = new HTuple();
            HTuple hv_DetectNames = new HTuple(), hv_ModelNames = new HTuple();
            HTuple hv_ModelConf = new HTuple(), hv_Length = new HTuple();




            HTuple hv_DetectRes = new HTuple(), hv_ModelRes = new HTuple();



            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTransPaints);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_ImagePart);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);





            // 显示//////////
            //显示图片/
            HTuple hv_bregionshow = new HTuple();
            //hv_bregionshow.Dispose();
            //hv_bregionshow = 0;
            //*分类及显示bimageshow为显示分类后的图像，bregionshow显示未分类前的原图和矩形框
            hv_bregionshow.Dispose();
            hv_bregionshow = Convert.ToInt32(actionParams["hv_bimageshow"]);
            /////////////////
            try
            {
                //输入参数
                List<dynamic> hv_params = JsonConvert.DeserializeObject<List<dynamic>>(actionParams["Params"]);
                hv_ImagePath.Dispose();
                hv_ImagePath = actionParams["Image"];

                ho_Image.Dispose();
                HOperatorSet.ReadImage(out ho_Image, hv_ImagePath);
                hv_BaseName.Dispose(); hv_Extension.Dispose(); hv_Directory.Dispose();
                parse_filename(hv_ImagePath, out hv_BaseName, out hv_Extension, out hv_Directory);
                //Image Acquisition 01: Do something
                ho_ImageAffineTransPaints.Dispose();
                HOperatorSet.CopyImage(ho_Image, out ho_ImageAffineTransPaints);
                Dictionary<string, bool> resultList = new Dictionary<string, bool>();
                for (hv_I = 0; (int)hv_I <= (int)((new HTuple(hv_params.Count())) - 1); hv_I = (int)hv_I + 1)
                {
                    //hv_domain.Dispose();
                    hv_DetectRes.Dispose();
                    hv_DetectRes = 0;
                    hv_ModelRes.Dispose();
                    hv_ModelRes = 0;


                    dynamic hv_domain = hv_params[hv_I];
                    //// hv_Name.Dispose();
                    //HOperatorSet.GetDictTuple(hv_domain, "name", out hv_Name);
                    //hv_x.Dispose();
                    //HOperatorSet.GetDictTuple(hv_domain, "x", out hv_x);
                    //hv_y.Dispose();
                    //HOperatorSet.GetDictTuple(hv_domain, "y", out hv_y);
                    //hv_h.Dispose();
                    //HOperatorSet.GetDictTuple(hv_domain, "h", out hv_h);
                    //hv_w.Dispose();
                    //HOperatorSet.GetDictTuple(hv_domain, "w", out hv_w);

                    string hv_Name = hv_domain["Name"];
                    string hv_Guid = hv_domain["Guid"];

                    double hv_w = hv_domain["Height"] / 2;
                    double hv_h = hv_domain["Width"] / 2;
                    double hv_x = hv_domain["Y"] + hv_w;
                    double hv_y = hv_domain["X"] + hv_h;
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle2(out ho_Rectangle, hv_x, hv_y, 0, hv_h, hv_w);
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_Image, ho_Rectangle, out ho_ImageReduced
                        );

                    ho_ImagePart.Dispose();
                    HOperatorSet.CropDomain(ho_ImageReduced, out ho_ImagePart);
                    hv_path.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_path = (hv_Directory + "/") + hv_Name;
                    }
                    hv_FileExists.Dispose();
                    HOperatorSet.FileExists(hv_path, out hv_FileExists);
                    hv_MSecond1.Dispose(); hv_Second1.Dispose(); hv_Minute1.Dispose(); hv_Hour1.Dispose(); hv_Day1.Dispose(); hv_YDay1.Dispose(); hv_Month1.Dispose(); hv_Year1.Dispose();
                    HOperatorSet.GetSystemTime(out hv_MSecond1, out hv_Second1, out hv_Minute1,
                        out hv_Hour1, out hv_Day1, out hv_YDay1, out hv_Month1, out hv_Year1);
                    hv_Imagename.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_Imagename = ((((((((((hv_path + "/") + hv_BaseName) + "-") + hv_Name) + "-") + "-") + hv_Minute1) + "-") + hv_Second1) + "-") + hv_MSecond1;
                    }
                    if ((int)(hv_FileExists) != 0)
                    {
                        HOperatorSet.WriteImage(ho_ImagePart, "jpg", 0, hv_Imagename);
                        //HOperatorSet.WriteImage(ho_ImagePart, "tiff", 0, hv_Imagename);

                    }
                    else
                    {
                        HOperatorSet.MakeDir(hv_path);
                        HOperatorSet.WriteImage(ho_ImagePart, "jpg", 0, hv_Imagename);
                        //HOperatorSet.WriteImage(ho_ImagePart, "tiff", 0, hv_Imagename);

                    }

                    // 获取结果

                    //目标检测模型结果
                    hv_DLSampleInference.Dispose();
                    gen_dl_samples_from_images(ho_ImagePart, out hv_DLSampleInference);
                    preprocess_dl_samples(hv_DLSampleInference, hv_DLPreprocessParam);
                    hv_DLResultDetect.Dispose();
                    HOperatorSet.ApplyDlModel(hv_DLModelHandle, hv_DLSampleInference, new HTuple(),
                        out hv_DLResultDetect);

                    //分类模型结果
                    hv_DLSample.Dispose();
                    gen_dl_samples_from_images(ho_ImagePart, out hv_DLSample);
                    preprocess_dl_samples(hv_DLSample, hv_DLDictHandleModel);
                    hv_DLResultModel.Dispose();
                    HOperatorSet.ApplyDlModel(hv_DLModelHandleModel, hv_DLSample, new HTuple(),
                        out hv_DLResultModel);

                    //获取目标检测结果
                    hv_Detectconfidence.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResultDetect, "bbox_confidence", out hv_Detectconfidence);
                    hv_DetectID.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResultDetect, "bbox_class_id", out hv_DetectID);
                    //hv_DetectNames.Dispose();
                    //HOperatorSet.GetDictTuple(hv_DLDataset, "class_names", out hv_DetectNames);

                    hv_DetectNames.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResultDetect, "bbox_class_name", out hv_DetectNames);
                    //获取分类结果
                    hv_ModelNames.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResultModel, "classification_class_names",
                        out hv_ModelNames);
                    hv_ModelConf.Dispose();
                    HOperatorSet.GetDictTuple(hv_DLResultModel, "classification_confidences",
                        out hv_ModelConf);


                    // 结果逻辑处理
                    hv_Length.Dispose();
                    HOperatorSet.TupleLength(hv_Detectconfidence, out hv_Length);
                    if ((int)(new HTuple(hv_Length.TupleGreater(0))) != 0)
                    {
                        if (hv_DetectNames.TupleSelect(0).S == "安全带出口盖板")
                        {
                            if ((int)((new HTuple(hv_DetectNames.TupleSelect(0).TupleEqual(hv_Name))).TupleAnd(
                           new HTuple(((hv_ModelConf.TupleSelect(0))).TupleGreater(0.2)))) != 0)
                            {
                                hv_ModelRes.Dispose();
                                hv_ModelRes = 1;
                            }
                        }
                        else
                        {
                            if ((int)(new HTuple(((hv_DetectNames.TupleSelect(0)).TupleEqual(hv_Name))).TupleAnd(new HTuple(((hv_Detectconfidence.TupleSelect(
                            0))).TupleGreater(0.70)))) != 0)
                            {
                                hv_DetectRes.Dispose();
                                hv_DetectRes = 1;

                            }
                        }
                    }
                    //分类模型
                    else
                    {
                        if (hv_ModelNames.TupleSelect(0).S == "安全带出口盖板")
                        {
                            if ((int)((new HTuple(((hv_ModelNames.TupleSelect(0))).TupleEqual(hv_Name))).TupleAnd(
                           new HTuple(((hv_ModelConf.TupleSelect(0))).TupleGreater(0.9)))) != 0)
                            {
                                hv_ModelRes.Dispose();
                                hv_ModelRes = 1;
                            }
                        }
                        //else
                        //{

                        //    if ((int)((new HTuple(((hv_ModelNames.TupleSelect(0))).TupleEqual(hv_Name))).TupleAnd(
                        //    new HTuple(((hv_ModelConf.TupleSelect(0))).TupleGreater(0.93)))) != 0)
                        //    {
                        //        hv_ModelRes.Dispose();
                        //        hv_ModelRes = 1;
                        //    }
                        //}
                    }



                    if ((int)(hv_ModelRes.TupleOr(hv_DetectRes)) != 0)
                    {

                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_x, hv_y, 0, hv_h, hv_w);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Rectangle2.Dispose();
                            HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_x, hv_y, 0, hv_h + 10, hv_w + 10);
                        }
                        ho_RegionDifference.Dispose();
                        HOperatorSet.Difference(ho_Rectangle2, ho_Rectangle1, out ho_RegionDifference
                            );
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.PaintRegion(ho_RegionDifference, ho_ImageAffineTransPaints,
                                out ExpTmpOutVar_0, ((new HTuple(0)).TupleConcat(255)).TupleConcat(
                                0), "fill");
                            ho_ImageAffineTransPaints.Dispose();
                            ho_ImageAffineTransPaints = ExpTmpOutVar_0;
                            resultList.Add(hv_Guid, true);

                        }

                    }
                    else
                    {
                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_x, hv_y, 0, hv_h, hv_w);
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_Rectangle2.Dispose();
                            HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_x, hv_y, 0, hv_h + 10, hv_w + 10);
                        }
                        ho_RegionDifference.Dispose();
                        HOperatorSet.Difference(ho_Rectangle2, ho_Rectangle1, out ho_RegionDifference
                            );
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.PaintRegion(ho_RegionDifference, ho_ImageAffineTransPaints,
                                out ExpTmpOutVar_0, ((new HTuple(255)).TupleConcat(0)).TupleConcat(
                                0), "fill");
                            ho_ImageAffineTransPaints.Dispose();
                            ho_ImageAffineTransPaints = ExpTmpOutVar_0;
                            // 测试代码 所有结果返回true
                            //resultList.Add(hv_Guid, true);
                            resultList.Add(hv_Guid, false);
                        }
                    }







                    //            if ((int)(new HTuple(hv_confidence.TupleGreater(0.80))) != 0)
                    //            {

                    //                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //                {
                    //                    hv_result = ((hv_Names.TupleSelect(
                    //                         hv_ID))).TupleSelect(0);
                    //                }




                    //                if (hv_result.S != hv_Name)
                    //                {

                    //                    ho_Rectangle1.Dispose();
                    //                    HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_x, hv_y, 0, hv_h, hv_w);
                    //                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //                    {
                    //                        ho_Rectangle2.Dispose();
                    //                        HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_x, hv_y, 0, hv_h + 10, hv_w + 10);
                    //                    }
                    //                    ho_RegionDifference.Dispose();
                    //                    HOperatorSet.Difference(ho_Rectangle2, ho_Rectangle1, out ho_RegionDifference
                    //                        );
                    //                    {
                    //                        HObject ExpTmpOutVar_0;
                    //                        HOperatorSet.PaintRegion(ho_RegionDifference, ho_ImageAffineTransPaints,
                    //                            out ExpTmpOutVar_0, ((new HTuple(255)).TupleConcat(0)).TupleConcat(
                    //                            0), "fill");
                    //                        ho_ImageAffineTransPaints.Dispose();
                    //                        ho_ImageAffineTransPaints = ExpTmpOutVar_0;
                    //                        // 测试代码 所有结果返回true
                    //                        //resultList.Add(hv_Guid, true);
                    //                        resultList.Add(hv_Guid, false);

                    //                    }
                    //                }
                    //                else
                    //                {
                    //                    if ((int)(new HTuple(((hv_confidence)).TupleGreaterEqual(
                    //0.80))) != 0)
                    //                    {
                    //                        ho_Rectangle1.Dispose();
                    //                        HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_x, hv_y, 0, hv_h, hv_w);
                    //                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //                        {
                    //                            ho_Rectangle2.Dispose();
                    //                            HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_x, hv_y, 0, hv_h + 10, hv_w + 10);
                    //                        }
                    //                        ho_RegionDifference.Dispose();
                    //                        HOperatorSet.Difference(ho_Rectangle2, ho_Rectangle1, out ho_RegionDifference
                    //                            );
                    //                        {
                    //                            HObject ExpTmpOutVar_0;
                    //                            HOperatorSet.PaintRegion(ho_RegionDifference, ho_ImageAffineTransPaints,
                    //                                out ExpTmpOutVar_0, ((new HTuple(0)).TupleConcat(255)).TupleConcat(
                    //                                0), "fill");
                    //                            ho_ImageAffineTransPaints.Dispose();
                    //                            ho_ImageAffineTransPaints = ExpTmpOutVar_0;
                    //                            resultList.Add(hv_Guid, true);

                    //                        }
                    //                    }
                    //                }
                    //            }

                    //            else
                    //            {
                    //                ho_Rectangle1.Dispose();
                    //                HOperatorSet.GenRectangle2(out ho_Rectangle1, hv_x, hv_y, 0, hv_h, hv_w);
                    //                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    //                {
                    //                    ho_Rectangle2.Dispose();
                    //                    HOperatorSet.GenRectangle2(out ho_Rectangle2, hv_x, hv_y, 0, hv_h + 10, hv_w + 10);
                    //                }
                    //                ho_RegionDifference.Dispose();
                    //                HOperatorSet.Difference(ho_Rectangle2, ho_Rectangle1, out ho_RegionDifference
                    //                    );
                    //                {
                    //                    HObject ExpTmpOutVar_0;
                    //                    HOperatorSet.PaintRegion(ho_RegionDifference, ho_ImageAffineTransPaints,
                    //                        out ExpTmpOutVar_0, ((new HTuple(255)).TupleConcat(0)).TupleConcat(
                    //                        0), "fill");
                    //                    ho_ImageAffineTransPaints.Dispose();
                    //                    ho_ImageAffineTransPaints = ExpTmpOutVar_0;
                    //                    // 测试代码 所有结果返回true
                    //                    //resultList.Add(hv_Guid, true);
                    //                    resultList.Add(hv_Guid, false);
                    //                }

                    //            }
                }

                hv_savepath.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_savepath = hv_Directory + "Res/";
                }
                hv_imageSavePath.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_imageSavePath = (hv_savepath + hv_BaseName) + "_res";
                }
                hv_FileExists.Dispose();
                HOperatorSet.FileExists(hv_savepath, out hv_FileExists);
                if ((int)(hv_FileExists) != 0)
                {
                    HOperatorSet.WriteImage(ho_ImageAffineTransPaints, "jpg", 0, hv_imageSavePath);
                }
                else
                {
                    HOperatorSet.MakeDir(hv_savepath);
                    HOperatorSet.WriteImage(ho_ImageAffineTransPaints, "jpg", 0, hv_imageSavePath);
                }


                //for (int i = 0; i < hv_NumsLength1; i++)
                //{
                //    CoverplateInfos.Add(hv_CoverplateNums[i].I);
                //}
                //for (int i = 0; i < hv_NumsLength2; i++)
                //{
                //    BoltInfos.Add(hv_BoltNums[i].I);
                //}
                //for (int i = 0; i < hv_NumsLength3; i++)
                //{
                //    FeltInfos.Add(hv_FeltNums[i].I);
                //}
                //for (int i = 0; i < hv_NumsLength4; i++)
                //{
                //    MetalInfos.Add(hv_MetalNums[i].I);
                //}


                //if ((int)(hv_bregionshow) != 0)
                //{
                //    //正常在C#里用下面的disp_obj
                //    if (hv_ExpDefaultWinHandle.H != IntPtr.Zero)
                //    {
                //        HOperatorSet.DispObj(ho_Image, hv_ExpDefaultWinHandle);
                //        //HOperatorSet.DispObj(ho_Rectangle1, hv_ExpDefaultWinHandle);
                //    }
                //    //disp_obj (MultiChannelImage, [H1A1F9104020])
                //    //disp_obj (Rectangle1, [H1A1F9104020])
                //}


                results.Add("Result", resultList);
                return results;
            }
            catch (HalconException HDevExpDefaultException)
            {
                throw HDevExpDefaultException;
            }
            finally
            {
                hv_DetectRes.Dispose();
                hv_ModelRes.Dispose();

                ho_Image.Dispose();
                ho_ImageAffineTransPaints.Dispose();
                ho_Rectangle.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImagePart.Dispose();
                ho_Rectangle1.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionDifference.Dispose();


                hv_ImagePath.Dispose();
                hv_BaseName.Dispose();
                hv_Extension.Dispose();
                hv_Directory.Dispose();
                hv_I.Dispose();

                hv_path.Dispose();
                hv_FileExists.Dispose();
                hv_MSecond1.Dispose();
                hv_Second1.Dispose();
                hv_Minute1.Dispose();
                hv_Hour1.Dispose();
                hv_Day1.Dispose();
                hv_YDay1.Dispose();
                hv_Month1.Dispose();
                hv_Year1.Dispose();
                hv_Imagename.Dispose();

                hv_savepath.Dispose();
                hv_imageSavePath.Dispose();

                hv_DLSampleInference.Dispose();
                hv_DLResultDetect.Dispose();
                hv_DLSample.Dispose();
                hv_DLResultModel.Dispose();
                hv_Detectconfidence.Dispose();
                hv_DetectID.Dispose();
                hv_DetectNames.Dispose();
                hv_ModelNames.Dispose();
                hv_ModelConf.Dispose();
                hv_Length.Dispose();
            }

        }
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

        #region Halcon model detect

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
