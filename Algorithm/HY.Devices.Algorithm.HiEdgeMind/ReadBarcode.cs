using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.HiEdgeMind
{
    /// <summary>
    /// 二维码识别
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class ReadBarcode : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static ReadBarcode _instance;
        public static ReadBarcode Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new ReadBarcode();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType =>AlgorithmTypes.Barcode;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();
        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" } };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                return IsInit = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            try
            {

                if (!IsInit)
                {
                    throw new Exception("未初始化模型");
                }
                Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
                string result = string.Empty;
                HObject ho_Image = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
                result = ReadBarCode(ho_Image);
                results.Add("result", result);
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public override void UnInit()
        {
            try
            {
                IsInit = false;
            }
            catch { }

        }
        private void scale_image_range(HObject ho_Image, out HObject ho_ImageScaled, HTuple hv_Min, HTuple hv_Max)
        {

            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_ImageSelected = null, ho_SelectedChannel = null;
            HObject ho_LowerRegion = null, ho_UpperRegion = null, ho_ImageSelectedScaled = null;

            // Local copy input parameter variables 
            HObject ho_Image_COPY_INP_TMP;
            ho_Image_COPY_INP_TMP = ho_Image.CopyObj(1, -1);



            // Local control variables 

            HTuple hv_LowerLimit = new HTuple(), hv_UpperLimit = new HTuple();
            HTuple hv_Mult = null, hv_Add = null, hv_NumImages = null;
            HTuple hv_ImageIndex = null, hv_Channels = new HTuple();
            HTuple hv_ChannelIndex = new HTuple(), hv_MinGray = new HTuple();
            HTuple hv_MaxGray = new HTuple(), hv_Range = new HTuple();
            HTuple hv_Max_COPY_INP_TMP = hv_Max.Clone();
            HTuple hv_Min_COPY_INP_TMP = hv_Min.Clone();

            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageSelected);
            HOperatorSet.GenEmptyObj(out ho_SelectedChannel);
            HOperatorSet.GenEmptyObj(out ho_LowerRegion);
            HOperatorSet.GenEmptyObj(out ho_UpperRegion);
            HOperatorSet.GenEmptyObj(out ho_ImageSelectedScaled);
            //Convenience procedure to scale the gray values of the
            //input image Image from the interval [Min,Max]
            //to the interval [0,255] (default).
            //Gray values < 0 or > 255 (after scaling) are clipped.
            //
            //If the image shall be scaled to an interval different from [0,255],
            //this can be achieved by passing tuples with 2 values [From, To]
            //as Min and Max.
            //Example:
            //scale_image_range(Image:ImageScaled:[100,50],[200,250])
            //maps the gray values of Image from the interval [100,200] to [50,250].
            //All other gray values will be clipped.
            //
            //input parameters:
            //Image: the input image
            //Min: the minimum gray value which will be mapped to 0
            //     If a tuple with two values is given, the first value will
            //     be mapped to the second value.
            //Max: The maximum gray value which will be mapped to 255
            //     If a tuple with two values is given, the first value will
            //     be mapped to the second value.
            //
            //Output parameter:
            //ImageScale: the resulting scaled image.
            //
            if ((int)(new HTuple((new HTuple(hv_Min_COPY_INP_TMP.TupleLength())).TupleEqual(
                2))) != 0)
            {
                hv_LowerLimit = hv_Min_COPY_INP_TMP.TupleSelect(1);
                hv_Min_COPY_INP_TMP = hv_Min_COPY_INP_TMP.TupleSelect(0);
            }
            else
            {
                hv_LowerLimit = 0.0;
            }
            if ((int)(new HTuple((new HTuple(hv_Max_COPY_INP_TMP.TupleLength())).TupleEqual(
                2))) != 0)
            {
                hv_UpperLimit = hv_Max_COPY_INP_TMP.TupleSelect(1);
                hv_Max_COPY_INP_TMP = hv_Max_COPY_INP_TMP.TupleSelect(0);
            }
            else
            {
                hv_UpperLimit = 255.0;
            }
            //
            //Calculate scaling parameters.
            hv_Mult = (((hv_UpperLimit - hv_LowerLimit)).TupleReal()) / (hv_Max_COPY_INP_TMP - hv_Min_COPY_INP_TMP);
            hv_Add = ((-hv_Mult) * hv_Min_COPY_INP_TMP) + hv_LowerLimit;
            //
            //Scale image.
            {
                HObject ExpTmpOutVar_0;
                HOperatorSet.ScaleImage(ho_Image_COPY_INP_TMP, out ExpTmpOutVar_0, hv_Mult, hv_Add);
                ho_Image_COPY_INP_TMP.Dispose();
                ho_Image_COPY_INP_TMP = ExpTmpOutVar_0;
            }
            //
            //Clip gray values if necessary.
            //This must be done for each image and channel separately.
            ho_ImageScaled.Dispose();
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.CountObj(ho_Image_COPY_INP_TMP, out hv_NumImages);
            HTuple end_val49 = hv_NumImages;
            HTuple step_val49 = 1;
            for (hv_ImageIndex = 1; hv_ImageIndex.Continue(end_val49, step_val49); hv_ImageIndex = hv_ImageIndex.TupleAdd(step_val49))
            {
                ho_ImageSelected.Dispose();
                HOperatorSet.SelectObj(ho_Image_COPY_INP_TMP, out ho_ImageSelected, hv_ImageIndex);
                HOperatorSet.CountChannels(ho_ImageSelected, out hv_Channels);
                HTuple end_val52 = hv_Channels;
                HTuple step_val52 = 1;
                for (hv_ChannelIndex = 1; hv_ChannelIndex.Continue(end_val52, step_val52); hv_ChannelIndex = hv_ChannelIndex.TupleAdd(step_val52))
                {
                    ho_SelectedChannel.Dispose();
                    HOperatorSet.AccessChannel(ho_ImageSelected, out ho_SelectedChannel, hv_ChannelIndex);
                    HOperatorSet.MinMaxGray(ho_SelectedChannel, ho_SelectedChannel, 0, out hv_MinGray,
                        out hv_MaxGray, out hv_Range);
                    ho_LowerRegion.Dispose();
                    HOperatorSet.Threshold(ho_SelectedChannel, out ho_LowerRegion, ((hv_MinGray.TupleConcat(
                        hv_LowerLimit))).TupleMin(), hv_LowerLimit);
                    ho_UpperRegion.Dispose();
                    HOperatorSet.Threshold(ho_SelectedChannel, out ho_UpperRegion, hv_UpperLimit,
                        ((hv_UpperLimit.TupleConcat(hv_MaxGray))).TupleMax());
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.PaintRegion(ho_LowerRegion, ho_SelectedChannel, out ExpTmpOutVar_0,
                            hv_LowerLimit, "fill");
                        ho_SelectedChannel.Dispose();
                        ho_SelectedChannel = ExpTmpOutVar_0;
                    }
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.PaintRegion(ho_UpperRegion, ho_SelectedChannel, out ExpTmpOutVar_0,
                            hv_UpperLimit, "fill");
                        ho_SelectedChannel.Dispose();
                        ho_SelectedChannel = ExpTmpOutVar_0;
                    }
                    if ((int)(new HTuple(hv_ChannelIndex.TupleEqual(1))) != 0)
                    {
                        ho_ImageSelectedScaled.Dispose();
                        HOperatorSet.CopyObj(ho_SelectedChannel, out ho_ImageSelectedScaled, 1,
                            1);
                    }
                    else
                    {
                        {
                            HObject ExpTmpOutVar_0;
                            HOperatorSet.AppendChannel(ho_ImageSelectedScaled, ho_SelectedChannel,
                                out ExpTmpOutVar_0);
                            ho_ImageSelectedScaled.Dispose();
                            ho_ImageSelectedScaled = ExpTmpOutVar_0;
                        }
                    }
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_ImageScaled, ho_ImageSelectedScaled, out ExpTmpOutVar_0
                        );
                    ho_ImageScaled.Dispose();
                    ho_ImageScaled = ExpTmpOutVar_0;
                }
            }
            ho_Image_COPY_INP_TMP.Dispose();
            ho_ImageSelected.Dispose();
            ho_SelectedChannel.Dispose();
            ho_LowerRegion.Dispose();
            ho_UpperRegion.Dispose();
            ho_ImageSelectedScaled.Dispose();

            return;
        }

        private string ReadBarCode(HObject hb_Image)
        {
            // Local iconic variables 

            HObject ho_Image1, ho_GrayImage, ho_SymbolRegions;
            HObject ho_BarCodeObjects, ho_ImageScaled = null, ho_SymbolRegions1 = null;
            HObject ho_ImageEmphasize = null, ho_SymbolRegions2 = null;

            // Local control variables 

            HTuple hv_Result = null, hv_BarCodeHandle = null;
            HTuple hv_DecodedDataStrings = null, hv_DecodedDataStrings1 = new HTuple();
            HTuple hv_Index = new HTuple(), hv_Index1 = new HTuple();
            HTuple hv_DecodedDataStrings2 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_SymbolRegions);
            HOperatorSet.GenEmptyObj(out ho_BarCodeObjects);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_SymbolRegions1);
            HOperatorSet.GenEmptyObj(out ho_ImageEmphasize);
            HOperatorSet.GenEmptyObj(out ho_SymbolRegions2);
            try
            {
                ho_Image1.Dispose();
                HOperatorSet.CopyImage(hb_Image, out ho_Image1);
                // HOperatorSet.ReadImage(out ho_Image1, "C:/Users/86176/Documents/WeChat Files/ooxx743157/FileStorage/File/2021-04/BCD-223WDPT/BB0B5109H00B7M3CA6KL/BC_BB0B5109H00B7M3CA6KL.jpg");
                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(ho_Image1, out ho_GrayImage);

                hv_Result = "";

                HOperatorSet.CreateBarCodeModel("persistence", 1, out hv_BarCodeHandle);
                HOperatorSet.SetBarCodeParam(hv_BarCodeHandle, "num_scanlines", 300);
                ho_SymbolRegions.Dispose();
                HOperatorSet.FindBarCode(ho_GrayImage, out ho_SymbolRegions, hv_BarCodeHandle,
                    "Code 128", out hv_DecodedDataStrings);
                ho_BarCodeObjects.Dispose();
                HOperatorSet.GetBarCodeObject(out ho_BarCodeObjects, hv_BarCodeHandle, "all",
                    "scanlines_all");
                hv_Result = hv_DecodedDataStrings.Clone();
                if ((int)(new HTuple(hv_Result.TupleEqual(new HTuple()))) != 0)
                {
                    ho_ImageScaled.Dispose();
                    scale_image_range(ho_GrayImage, out ho_ImageScaled, 100, 200);
                    ho_SymbolRegions1.Dispose();
                    HOperatorSet.FindBarCode(ho_ImageScaled, out ho_SymbolRegions1, hv_BarCodeHandle,
                        "Code 128", out hv_DecodedDataStrings1);
                    hv_Result = hv_DecodedDataStrings1.Clone();
                    if ((int)(new HTuple(hv_Result.TupleEqual(new HTuple()))) != 0)
                    {
                        for (hv_Index = 4; (int)hv_Index <= 8; hv_Index = (int)hv_Index + 1)
                        {
                            for (hv_Index1 = 1; (int)hv_Index1 <= 7; hv_Index1 = (int)hv_Index1 + 1)
                            {
                                ho_ImageEmphasize.Dispose();
                                HOperatorSet.Emphasize(ho_GrayImage, out ho_ImageEmphasize, hv_Index,
                                    hv_Index, hv_Index1);
                                ho_SymbolRegions2.Dispose();
                                HOperatorSet.FindBarCode(ho_ImageEmphasize, out ho_SymbolRegions2, hv_BarCodeHandle,
                                    "Code 128", out hv_DecodedDataStrings2);

                                hv_Result = hv_DecodedDataStrings2.Clone();
                                if ((int)(new HTuple(hv_Result.TupleNotEqual(new HTuple()))) != 0)
                                {
                                    break;
                                }
                            }
                        }
                    }
                }

                return hv_Result.S;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                HOperatorSet.ClearBarCodeModel(hv_BarCodeHandle);
                ho_Image1.Dispose();
                ho_GrayImage.Dispose();
                ho_SymbolRegions.Dispose();
                ho_BarCodeObjects.Dispose();
                ho_ImageScaled.Dispose();
                ho_SymbolRegions1.Dispose();
                ho_ImageEmphasize.Dispose();
                ho_SymbolRegions2.Dispose();
            }

        }
    }
}
