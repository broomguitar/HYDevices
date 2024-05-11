using HalconDotNet;
using System;
namespace HY.Devices.Algorithm
{
    /// <summary>
    /// 门平算法
    /// </summary>
    public class DoorPlain
    {
        private static readonly object _lockObj = new object();
        private static DoorPlain _instance;
        public static DoorPlain Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new DoorPlain();
                        }
                    }
                }
                return _instance;
            }
        }
        public HTuple hv_ExpDefaultWinHandle;

        // Main procedure 
        private void action(HObject hb_image)
        {
            HObject ho_ImageZ = null, ho_ROI_0 = null, ho_ImageReduced = null;
            HObject ho_ImageMean = null, ho_Region1 = null, ho_RegionOpening1 = null, ho_ConnectedRegions1 = null;
            HObject ho_SelectedRegions1 = null, ho_ObjectSelectedOne = null;
            HObject ho_Rectangle1 = null, ho_ImageReduced1 = null;
            HObject ho_ObjectSelectedTwo = null, ho_Rectangle = null;
            HObject ho_Region = null;

            // Local control variables 

            HTuple hv_y0 = new HTuple(), hv_x0 = new HTuple();
            HTuple hv_y1 = new HTuple(), hv_x1 = new HTuple(), hv_Yscale = new HTuple();
            HTuple hv_Zscale = new HTuple(), hv_threolddata1 = new HTuple();
            HTuple hv_threolddatastart = new HTuple(), hv_threolddataUp = new HTuple();
            HTuple hv_H = new HTuple(), hv_Number1 = new HTuple();
            HTuple hv_leftRow13 = new HTuple(), hv_leftColumn13 = new HTuple();
            HTuple hv_leftRow23 = new HTuple(), hv_leftColumn23 = new HTuple();
            HTuple hv_leftY = new HTuple(), hv_leftX = new HTuple();
            HTuple hv_pxLeft = new HTuple(), hv_Deviation = new HTuple();
            HTuple hv_rightRow13 = new HTuple(), hv_rightColumn13 = new HTuple();
            HTuple hv_rightRow23 = new HTuple(), hv_rightColumn23 = new HTuple();
            HTuple hv_rightY = new HTuple(), hv_rightX = new HTuple();
            HTuple hv_pxRight = new HTuple(), hv_resultDepth = new HTuple();
            try
            {
                // Initialize local and output iconic variables 
                HOperatorSet.GenEmptyObj(out ho_ImageZ);
                HOperatorSet.GenEmptyObj(out ho_ROI_0);
                HOperatorSet.GenEmptyObj(out ho_ImageReduced);
                HOperatorSet.GenEmptyObj(out ho_ImageMean);
                HOperatorSet.GenEmptyObj(out ho_Region1);
                HOperatorSet.GenEmptyObj(out ho_RegionOpening1);
                HOperatorSet.GenEmptyObj(out ho_ConnectedRegions1);
                HOperatorSet.GenEmptyObj(out ho_SelectedRegions1);
                HOperatorSet.GenEmptyObj(out ho_ObjectSelectedOne);

                HOperatorSet.GenEmptyObj(out ho_Rectangle1);
                HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
                HOperatorSet.GenEmptyObj(out ho_ObjectSelectedTwo);

                HOperatorSet.GenEmptyObj(out ho_Rectangle);
                HOperatorSet.GenEmptyObj(out ho_Region);

                //输入：
                //深度输入Y轴的比例系数，Yscale
                //深度输入Z轴的比例系数，Zscale
                //深度灰度图片，ImageZ
                //门平处理区域，y0,x0,y1,x1
                //深度门平阈值 threolddata1 一般默认
                //提取有用门体深度阈值 threolddatastart   threolddataUp 一般默认
                hv_y0.Dispose();
                hv_y0 = 18.7562;
                hv_x0.Dispose();
                hv_x0 = 663.723;
                hv_y1.Dispose();
                hv_y1 = 38.9933;
                hv_x1.Dispose();
                hv_x1 = 1079.61;
                hv_Yscale.Dispose();
                hv_Yscale = 1;
                hv_Zscale.Dispose();
                hv_Zscale = 1;
                hv_threolddata1.Dispose();
                hv_threolddata1 = -150000000;
                hv_threolddatastart.Dispose();
                hv_threolddatastart = 5000;
                hv_threolddataUp.Dispose();
                hv_threolddataUp = 80000;

                //输出：
                //门平数resultDepth  最终有用
                //门平两个位置点  leftY, leftX； rightY, rightX； 可划叉点，供人工观看位置
                //门平两个位置点图像深度 pxRight，pxLeft    可作为参考，看是否正确，原因是什么
                ho_ImageZ.Dispose();
                HOperatorSet.CopyImage(hb_image, out ho_ImageZ);

                ho_ROI_0.Dispose();
                HOperatorSet.GenRectangle1(out ho_ROI_0, hv_y0, hv_x0, hv_y1, hv_x1);
                hv_H.Dispose();
                HOperatorSet.RegionFeatures(ho_ROI_0, "height", out hv_H);
                ho_ImageReduced.Dispose();
                HOperatorSet.ReduceDomain(ho_ImageZ, ho_ROI_0, out ho_ImageReduced);
                ho_ImageMean.Dispose();
                HOperatorSet.MeanImage(ho_ImageReduced, out ho_ImageMean, 9, 9);
                ho_Region1.Dispose();
                HOperatorSet.Threshold(ho_ImageMean, out ho_Region1, hv_threolddata1, 2147483647);
                ho_RegionOpening1.Dispose();
                HOperatorSet.ClosingRectangle1(ho_Region1, out ho_RegionOpening1, 5, 5);
                ho_ConnectedRegions1.Dispose();
                HOperatorSet.Connection(ho_RegionOpening1, out ho_ConnectedRegions1);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_SelectedRegions1.Dispose();
                    HOperatorSet.SelectShape(ho_ConnectedRegions1, out ho_SelectedRegions1, "height",
                        "and", hv_H * 0.8, hv_H);
                }
                hv_Number1.Dispose();
                HOperatorSet.CountObj(ho_SelectedRegions1, out hv_Number1);
                if ((int)(new HTuple(hv_Number1.TupleGreaterEqual(2))) != 0)
                {

                    ho_ObjectSelectedOne.Dispose();
                    HOperatorSet.SelectObj(ho_SelectedRegions1, out ho_ObjectSelectedOne, 1);
                    hv_leftRow13.Dispose(); hv_leftColumn13.Dispose(); hv_leftRow23.Dispose(); hv_leftColumn23.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_ObjectSelectedOne, out hv_leftRow13, out hv_leftColumn13,
                        out hv_leftRow23, out hv_leftColumn23);
                    hv_leftY.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_leftY = (hv_leftRow23 + hv_leftRow13) / 2;
                    }
                    hv_leftX.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_leftX = hv_leftColumn23 - 10;
                    }


                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Rectangle1.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle1, hv_leftY - 5, hv_leftX - 5, hv_leftY + 5,
                            hv_leftX + 5);
                    }
                    ho_ImageReduced1.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageMean, ho_Rectangle1, out ho_ImageReduced1
                        );
                    ho_Region1.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced1, out ho_Region1, hv_threolddatastart,
                        hv_threolddataUp);
                    hv_pxLeft.Dispose(); hv_Deviation.Dispose();
                    HOperatorSet.Intensity(ho_Region1, ho_ImageMean, out hv_pxLeft, out hv_Deviation);
                    ho_ObjectSelectedTwo.Dispose();
                    HOperatorSet.SelectObj(ho_SelectedRegions1, out ho_ObjectSelectedTwo, 2);
                    hv_rightRow13.Dispose(); hv_rightColumn13.Dispose(); hv_rightRow23.Dispose(); hv_rightColumn23.Dispose();
                    HOperatorSet.SmallestRectangle1(ho_ObjectSelectedTwo, out hv_rightRow13, out hv_rightColumn13,
                        out hv_rightRow23, out hv_rightColumn23);
                    hv_rightY.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_rightY = (hv_rightRow23 + hv_rightRow13) / 2;
                    }
                    hv_rightX.Dispose();
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_rightX = hv_rightColumn13 + 10;
                    }

                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        ho_Rectangle.Dispose();
                        HOperatorSet.GenRectangle1(out ho_Rectangle, hv_rightY - 5, hv_rightX - 5, hv_rightY + 5,
                            hv_rightX + 5);
                    }
                    ho_ImageReduced.Dispose();
                    HOperatorSet.ReduceDomain(ho_ImageMean, ho_Rectangle, out ho_ImageReduced);
                    ho_Region.Dispose();
                    HOperatorSet.Threshold(ho_ImageReduced, out ho_Region, hv_threolddatastart,
                        hv_threolddataUp);
                    hv_pxRight.Dispose(); hv_Deviation.Dispose();
                    HOperatorSet.Intensity(ho_Region, ho_ImageMean, out hv_pxRight, out hv_Deviation);
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        hv_resultDepth.Dispose();
                        HOperatorSet.TupleAbs(hv_pxRight - hv_pxLeft, out hv_resultDepth);
                    }
                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                    {
                        {
                            HTuple
                              ExpTmpLocalVar_resultDepth = hv_resultDepth * hv_Zscale;
                            hv_resultDepth.Dispose();
                            hv_resultDepth = ExpTmpLocalVar_resultDepth;
                        }
                    }
                    //***************resultDepth 深度差

                    //reduce_domain (ImageX, ROI_0, ImageReducedX)
                    //mean_image (ImageReducedX, ImageMeanX, 9, 9)

                    //gen_region_line (RegionLines, leftRow13, leftColumn23, leftRow23, leftColumn23)
                    //gen_region_line (RegionLines1, rightRow13, rightColumn13, rightRow23, rightColumn13)

                    //distance_ss (leftRow13, leftColumn23, leftRow23, leftColumn23, rightRow13, rightColumn13, rightRow23, rightColumn13, DistanceMin, DistanceMax)

                    //resultLength := (DistanceMin+DistanceMax)/2*Yscale
                    //***************resultLength 门缝宽度

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                hb_image.Dispose();
                ho_ImageZ.Dispose();
                ho_ROI_0.Dispose();
                ho_ImageReduced.Dispose();
                ho_ImageMean.Dispose();
                ho_Region1.Dispose();
                ho_RegionOpening1.Dispose();
                ho_ConnectedRegions1.Dispose();
                ho_SelectedRegions1.Dispose();
                ho_ObjectSelectedOne.Dispose();
                ho_Rectangle1.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ObjectSelectedTwo.Dispose();
                ho_Rectangle.Dispose();
                ho_Region.Dispose();

                hv_y0.Dispose();
                hv_x0.Dispose();
                hv_y1.Dispose();
                hv_x1.Dispose();
                hv_Yscale.Dispose();
                hv_Zscale.Dispose();
                hv_threolddata1.Dispose();
                hv_threolddatastart.Dispose();
                hv_threolddataUp.Dispose();
                hv_H.Dispose();
                hv_Number1.Dispose();
                hv_leftRow13.Dispose();
                hv_leftColumn13.Dispose();
                hv_leftRow23.Dispose();
                hv_leftColumn23.Dispose();
                hv_leftY.Dispose();
                hv_leftX.Dispose();
                hv_pxLeft.Dispose();
                hv_Deviation.Dispose();
                hv_rightRow13.Dispose();
                hv_rightColumn13.Dispose();
                hv_rightRow23.Dispose();
                hv_rightColumn23.Dispose();
                hv_rightY.Dispose();
                hv_rightX.Dispose();
                hv_pxRight.Dispose();
                hv_resultDepth.Dispose();
            }
        }

    }
}


