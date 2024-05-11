﻿using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;

namespace HY.Devices.Algorithm.Haier_ZhongDe
{
    /// <summary>
    /// 二维码检测
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class RecognizeQRcode : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static RecognizeQRcode _instance;
        public static RecognizeQRcode Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new RecognizeQRcode();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.QRcode;

        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "Image", "" }, { "IsFind", 0} };

        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            if (!IsInit)
            {
                throw new Exception("未初始化模型");
            }
            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();

            string ResultString = "";
            // Local iconic variables 

            HObject ho_Image, ho_GrayImage, ho_ImageZoomed = null;
            HObject ho_SymbolXLDs = null;

            // Local control variables 

            HTuple hv_DataCodeHandle = new HTuple(), hv_index = new HTuple();
            HTuple hv_ResultHandles = new HTuple(), hv_DecodedDataStrings = new HTuple();
            HTuple hv_resultString = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_GrayImage);
            HOperatorSet.GenEmptyObj(out ho_ImageZoomed);
            HOperatorSet.GenEmptyObj(out ho_SymbolXLDs);

            try
            {
                ho_Image.Dispose();
                ho_Image = Utils.Ho_ImageHelper.GetHoImageFromDynamic(actionParams["Image"]);
                ho_GrayImage.Dispose();
                HOperatorSet.Rgb1ToGray(ho_Image, out ho_GrayImage);
                hv_DataCodeHandle.Dispose();
                HOperatorSet.CreateDataCode2dModel("QR Code", ((new HTuple("polarity")).TupleConcat(
                    "module_gap_max")).TupleConcat("position_pattern_min"), ((new HTuple("dark_on_light")).TupleConcat(
                    "big")).TupleConcat(2), out hv_DataCodeHandle);
                if (actionParams["IsFind"] == 1)
                {
                    ho_SymbolXLDs.Dispose(); hv_ResultHandles.Dispose(); hv_DecodedDataStrings.Dispose();
                    HOperatorSet.FindDataCode2d(ho_ImageZoomed, out ho_SymbolXLDs, hv_DataCodeHandle,
                        new HTuple(), new HTuple(), out hv_ResultHandles, out hv_DecodedDataStrings);
                }
                else
                {
                    for (hv_index = 0.2; (double)hv_index <= 4; hv_index = (double)hv_index + 0.2)
                    {
                        ho_ImageZoomed.Dispose();
                        try
                        {
                            HOperatorSet.ZoomImageFactor(ho_GrayImage, out ho_ImageZoomed, hv_index, hv_index,
                                "bicubic");
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"{hv_index}+{ex.ToString()}");
                        }
                        ho_SymbolXLDs.Dispose(); hv_ResultHandles.Dispose(); hv_DecodedDataStrings.Dispose();
                        HOperatorSet.FindDataCode2d(ho_ImageZoomed, out ho_SymbolXLDs, hv_DataCodeHandle,
                            new HTuple(), new HTuple(), out hv_ResultHandles, out hv_DecodedDataStrings);
                        if ((int)(new HTuple((new HTuple(hv_DecodedDataStrings.TupleLength())).TupleGreater(
                            0))) != 0)
                        {
                            break;
                        }
                    }
                }
                hv_resultString.Dispose();
                hv_resultString = new HTuple(hv_DecodedDataStrings);
                ResultString = hv_resultString.S;
                results.Add("result", ResultString);
                return results;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ho_Image.Dispose();
                ho_GrayImage.Dispose();
                ho_ImageZoomed.Dispose();
                ho_SymbolXLDs.Dispose();

                hv_DataCodeHandle.Dispose();
                hv_index.Dispose();
                hv_ResultHandles.Dispose();
                hv_DecodedDataStrings.Dispose();
                hv_resultString.Dispose();
            }
        }

        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            IsInit = true;
            return true;
        }

        public override void UnInit()
        {
            IsInit = false;
        }
    }
}
