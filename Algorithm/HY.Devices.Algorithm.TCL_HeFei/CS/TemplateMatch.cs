using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HY.Devices.Algorithm.TCL_HeFei
{
    /// <summary>
    /// 模板匹配
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class TemplateMatch : AbstractAlgorithm, IAlgorithm
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

        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)

        {
            Dictionary<string, dynamic> result = new Dictionary<string, dynamic>();
            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_Image, ho_Region, ho_RegionFillUp;
            HObject ho_ConnectedRegions, ho_SelectedRegions, ho_RegionAffineTrans;
            HObject ho_ImageAffineTrans, ho_ImageReduced, ho_TextLines;
            // Local control variables 
            HTuple hv_Range = new HTuple(), hv_Min = new HTuple(), hv_Max = new HTuple();


            HTuple hv_str = new HTuple(), hv_Row = new HTuple();
            HTuple hv_Column = new HTuple(), hv_Phi = new HTuple();
            HTuple hv_Length1 = new HTuple(), hv_Length2 = new HTuple(), hv_Number = new HTuple();
            HTuple hv_HomMat2DIdentity = new HTuple(), hv_HomMat2DRotate = new HTuple();
            HTuple hv_OCRHandle = new HTuple(), hv_TextModel = new HTuple();
            HTuple hv_TextResult = new HTuple(), hv_SingleCharacters = new HTuple();
            HTuple hv_TextLineCharacters = new HTuple(), hv_CharacterIndex = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionFillUp);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_TextLines);
            ho_Image.Dispose();
            ho_Image = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
            hv_str.Dispose();
            hv_str = "";
            try
            {
                hv_Min.Dispose(); hv_Max.Dispose(); hv_Range.Dispose();
                //HOperatorSet.MinMaxGray(ho_Image, ho_Image, 0, out hv_Min, out hv_Max, out hv_Range);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ScaleImage(ho_Image, out ExpTmpOutVar_0, 3, 0);
                    ho_Image.Dispose();
                    ho_Image = ExpTmpOutVar_0;
                }
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_Image, out ho_Region, 240, 255);
                ho_RegionFillUp.Dispose();
                HOperatorSet.FillUp(ho_Region, out ho_RegionFillUp);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionFillUp, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", 10000, 99999);
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.SelectShape(ho_SelectedRegions, out ExpTmpOutVar_0, "rectangularity",
                        "and", 0.8, 1);
                    ho_SelectedRegions.Dispose();
                    ho_SelectedRegions = ExpTmpOutVar_0;
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.SelectShape(ho_SelectedRegions, out ExpTmpOutVar_0, (new HTuple("width")).TupleConcat(
                        "height"), "and", (new HTuple(240)).TupleConcat(110), (new HTuple(520)).TupleConcat(
                        260));
                    ho_SelectedRegions.Dispose();
                    ho_SelectedRegions = ExpTmpOutVar_0;
                }
                hv_Number.Dispose();
                HOperatorSet.CountObj(ho_SelectedRegions, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleNotEqual(0))) != 0)
                {
                    hv_Row.Dispose(); hv_Column.Dispose(); hv_Phi.Dispose(); hv_Length1.Dispose(); hv_Length2.Dispose();
                    HOperatorSet.SmallestRectangle2(ho_SelectedRegions, out hv_Row, out hv_Column,
                        out hv_Phi, out hv_Length1, out hv_Length2);
                    hv_HomMat2DIdentity.Dispose();
                    HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_HomMat2DRotate.Dispose();
                        HOperatorSet.HomMat2dRotate(hv_HomMat2DIdentity, -hv_Phi, hv_Column, hv_Row,
                            out hv_HomMat2DRotate);
                    }
                    ho_RegionAffineTrans.Dispose();
                    HOperatorSet.AffineTransRegion(ho_SelectedRegions, out ho_RegionAffineTrans,
                        hv_HomMat2DRotate, "nearest_neighbor");
                    ho_ImageAffineTrans.Dispose();
                    HOperatorSet.AffineTransImage(ho_Image, out ho_ImageAffineTrans, hv_HomMat2DRotate,
                        "constant", "false");
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageAffineTrans, ho_RegionAffineTrans, out ho_ImageReduced
                        );


                    hv_OCRHandle.Dispose();
                    HOperatorSet.ReadOcrClassMlp(actionParams["ModelPath"], out hv_OCRHandle);

                    hv_TextModel.Dispose();
                    HOperatorSet.CreateTextModelReader("auto", hv_OCRHandle, out hv_TextModel);

                    hv_TextResult.Dispose();
                    HOperatorSet.FindText(ho_ImageReduced, hv_TextModel, out hv_TextResult);
                    ho_TextLines.Dispose();
                    HOperatorSet.GetTextObject(out ho_TextLines, hv_TextResult, "all_lines");
                    hv_SingleCharacters.Dispose();
                    HOperatorSet.GetTextResult(hv_TextResult, "class", out hv_SingleCharacters);
                    hv_TextLineCharacters.Dispose();
                    HOperatorSet.TupleSum(hv_SingleCharacters, out hv_TextLineCharacters);

                    for (hv_CharacterIndex = 0; (int)hv_CharacterIndex <= (int)((new HTuple(hv_SingleCharacters.TupleLength()
                        )) - 1); hv_CharacterIndex = (int)hv_CharacterIndex + 1)
                    {
                        if ((int)(new HTuple(((hv_SingleCharacters.TupleSelect(hv_CharacterIndex))).TupleEqual(
                            "O"))) != 0)
                        {
                            if (hv_SingleCharacters == null)
                                hv_SingleCharacters = new HTuple();
                            hv_SingleCharacters[hv_CharacterIndex] = 0;
                        }
                        else if ((int)(new HTuple(((hv_SingleCharacters.TupleSelect(hv_CharacterIndex))).TupleEqual(
                   "-"))) != 0)
                        {
                            continue;
                        }
                        else if ((int)(new HTuple(((hv_SingleCharacters.TupleSelect(hv_CharacterIndex))).TupleEqual(
                            "."))) != 0)
                        {
                            continue;
                        }
                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            {
                                HTuple
                                  ExpTmpLocalVar_str = hv_str + (hv_SingleCharacters.TupleSelect(
                                    hv_CharacterIndex));
                                hv_str.Dispose();
                                hv_str = ExpTmpLocalVar_str;
                            }
                        }
                    }
                }
                string resultStr = hv_str.S;
                if (!string.IsNullOrEmpty(resultStr))
                    //resultStr = resultStr.Substring(0, 5) + "0" + resultStr.Substring(resultStr.Length - 5);
                    resultStr = "3B102000" + resultStr.Substring(resultStr.Length - 3);
                result.Add("result", resultStr);
                return result;
            }
            catch (Exception ex)
            {
                result.Add("result", "-1");
                return result;
            }
            finally
            {

                ho_Image.Dispose();
                ho_Region.Dispose();
                ho_RegionFillUp.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionAffineTrans.Dispose();
                ho_ImageAffineTrans.Dispose();
                ho_ImageReduced.Dispose();
                ho_TextLines.Dispose();

                hv_Min.Dispose();
                hv_Max.Dispose();
                hv_Range.Dispose();

                hv_str.Dispose();
                hv_Row.Dispose();
                hv_Column.Dispose();
                hv_Phi.Dispose();
                hv_Length1.Dispose();
                hv_Length2.Dispose();
                hv_HomMat2DIdentity.Dispose();
                hv_HomMat2DRotate.Dispose();
                hv_OCRHandle.Dispose();
                hv_TextModel.Dispose();
                hv_TextResult.Dispose();
                hv_SingleCharacters.Dispose();
                hv_TextLineCharacters.Dispose();
                hv_CharacterIndex.Dispose();
                hv_Number.Dispose();
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
