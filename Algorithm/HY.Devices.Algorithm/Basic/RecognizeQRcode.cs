using HalconDotNet;
using System;
using System.Drawing;

namespace HY.Devices.Algorithm
{

    /// <summary>
    /// 二维码检测
    /// </summary>
    public class RecognizeQRcode
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
        private string ReadQRCode(HObject hb_Image)
        {
            // Local iconic variables 

            HObject ho_Image, ho_SymbolXLDs;

            // Local control variables 

            HTuple hv_Result = null, hv_DataCodeHandle = null;
            HTuple hv_ResultHandles = null, hv_DecodedDataStrings = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_SymbolXLDs);
            try
            {
                ho_Image.Dispose();
                HOperatorSet.CopyImage(hb_Image, out ho_Image);
                HOperatorSet.ZoomImageFactor(ho_Image, out ho_Image, 1, 1.2, "constant");
                //HOperatorSet.Rgb1ToGray(ho_Image, out ho_Image);
                // HOperatorSet.ScaleImage(ho_Image, out ho_Image, 2, 0);
                //结果
                hv_Result = "";

                //图像处理
                HOperatorSet.CreateDataCode2dModel("QR Code", new HTuple(), new HTuple(), out hv_DataCodeHandle);
                //HOperatorSet.SetDataCode2dParam(hv_DataCodeHandle,,)
                ho_SymbolXLDs.Dispose();
                //HOperatorSet.FindDataCode2d(ho_Image, out ho_SymbolXLDs, hv_DataCodeHandle, "stop_after_result_num",
                //    2, out hv_ResultHandles, out hv_DecodedDataStrings);

                HOperatorSet.FindDataCode2d(ho_Image, out ho_SymbolXLDs, hv_DataCodeHandle, new HTuple(),
                 new HTuple(), out hv_ResultHandles, out hv_DecodedDataStrings);


                return hv_DecodedDataStrings.Clone();
            }
            catch (Exception ex)
            {
                throw ex;

            }
            finally
            {
                ho_Image.Dispose();
                ho_SymbolXLDs.Dispose();
                if (hv_DataCodeHandle != null) HOperatorSet.ClearDataCode2dModel(hv_DataCodeHandle);
            }

        }
        /// <summary>
        /// 输入图像读取二维码
        /// </summary>
        /// <param name="hb_Image"></param>
        /// <returns></returns>
        public string ReadQRCode(string hb_ImagePath)
        {
            try
            {
                HOperatorSet.ReadImage(out HObject hb_Image, hb_ImagePath);
                return ReadQRCode(hb_Image);
            }
            catch (Exception ex) { throw ex; }
        }
        public string ReadQRCode(Bitmap bitmap)
        {
            try
            {
                return ReadQRCode(Utils.Ho_ImageHelper.Bitmap2HObject(bitmap));
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
