using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Yolov7.YoloV7
{
    public class PreProcess
    {
        public static HObject ZoomAndFillImage(HObject imaga, int width, int height, int gray)
        {


            // Local iconic variables 

            HObject ho_Image, ho_Rectangle = null, ho_Rectangle11 = null;
            HObject ho_ImageZoomed, ho_ImageEmpty, ho_ImageR, ho_ImageG;
            HObject ho_ImageB, ho_MultiImage, ho_DupImage, ho_ImageAffineTrans;
            HObject ho_ImageReduced, ho_Rectangle1, ho_ImageResult1;

            // Local control variables 

            HTuple hv_Height = new HTuple(), hv_Width = new HTuple();
            HTuple hv_targetGray = new HTuple(), hv_Width1 = new HTuple();
            HTuple hv_Height1 = new HTuple(), hv_factor = new HTuple();
            HTuple hv_HomMat2DIdentity = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Rectangle);
            HOperatorSet.GenEmptyObj(out ho_Rectangle11);
            HOperatorSet.GenEmptyObj(out ho_ImageZoomed);
            HOperatorSet.GenEmptyObj(out ho_ImageEmpty);
            HOperatorSet.GenEmptyObj(out ho_ImageR);
            HOperatorSet.GenEmptyObj(out ho_ImageG);
            HOperatorSet.GenEmptyObj(out ho_ImageB);
            HOperatorSet.GenEmptyObj(out ho_MultiImage);
            HOperatorSet.GenEmptyObj(out ho_DupImage);
            HOperatorSet.GenEmptyObj(out ho_ImageAffineTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_ImageResult1);


            hv_Height.Dispose();
            hv_Height = height;
            hv_Width.Dispose();
            hv_Width = width;
            hv_targetGray.Dispose();
            hv_targetGray = gray;

            ho_Image.Dispose();
            ho_Image = imaga.Clone();

            hv_Width1.Dispose(); hv_Height1.Dispose();
            HOperatorSet.GetImageSize(ho_Image, out hv_Width1, out hv_Height1);

            if ((int)(new HTuple((((float)hv_Width1 / hv_Width)).TupleGreater((float)hv_Height1 / hv_Height))) != 0)
            {
                hv_factor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_factor = (double)hv_Width / (double)hv_Width1;
                }
                hv_HomMat2DIdentity.Dispose();
                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, (hv_Height - (hv_Height1 * hv_factor)) / 2,
                        0, out ExpTmpOutVar_0);
                    hv_HomMat2DIdentity.Dispose();
                    hv_HomMat2DIdentity = ExpTmpOutVar_0;
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, (hv_Height - (hv_Height1 * hv_factor)) / 2,
                        hv_Width);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle11.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle11, (hv_Height - (hv_Height1 * hv_factor)) / 2,
                        0, (hv_Height + (hv_Height1 * hv_factor)) / 2, hv_Width);
                }
            }
            else
            {
                hv_factor.Dispose();
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    hv_factor = (double)hv_Height / (double)hv_Height1;
                }

                hv_HomMat2DIdentity.Dispose();
                HOperatorSet.HomMat2dIdentity(out hv_HomMat2DIdentity);
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HTuple ExpTmpOutVar_0;
                    HOperatorSet.HomMat2dTranslate(hv_HomMat2DIdentity, 0, (hv_Width - (hv_Width1 * hv_factor)) / 2,
                        out ExpTmpOutVar_0);
                    hv_HomMat2DIdentity.Dispose();
                    hv_HomMat2DIdentity = ExpTmpOutVar_0;
                }

                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle, 0, 0, hv_Height, (hv_Width - (hv_Width1 * hv_factor)) / 2);
                }
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_Rectangle11.Dispose();
                    HOperatorSet.GenRectangle1(out ho_Rectangle11, 0, (hv_Width - (hv_Width1 * hv_factor)) / 2,
                        hv_Height, (hv_Width + (hv_Width1 * hv_factor)) / 2);
                }
            }

            ho_ImageZoomed.Dispose();
            HOperatorSet.ZoomImageFactor(ho_Image, out ho_ImageZoomed, hv_factor, hv_factor,
                "constant");


            ho_ImageEmpty.Dispose();
            HOperatorSet.GenImageConst(out ho_ImageEmpty, "byte", hv_Width, hv_Height);

            ho_ImageR.Dispose();
            HOperatorSet.GenImageProto(ho_ImageEmpty, out ho_ImageR, 0);
            ho_ImageG.Dispose();
            HOperatorSet.GenImageProto(ho_ImageEmpty, out ho_ImageG, 0);
            ho_ImageB.Dispose();
            HOperatorSet.GenImageProto(ho_ImageEmpty, out ho_ImageB, 0);
            ho_MultiImage.Dispose();
            HOperatorSet.Compose3(ho_ImageR, ho_ImageG, ho_ImageB, out ho_MultiImage);

            ho_DupImage.Dispose();
            HOperatorSet.CopyImage(ho_MultiImage, out ho_DupImage);

            HOperatorSet.OverpaintGray(ho_MultiImage, ho_ImageZoomed);
            ho_ImageAffineTrans.Dispose();
            HOperatorSet.AffineTransImage(ho_MultiImage, out ho_ImageAffineTrans, hv_HomMat2DIdentity,
                "constant", "false");
            ho_ImageReduced.Dispose();
            HOperatorSet.ReduceDomain(ho_ImageAffineTrans, ho_Rectangle11, out ho_ImageReduced
                );



            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_Rectangle1.Dispose();
                HOperatorSet.GenRectangle1(out ho_Rectangle1, 0, 0, hv_Height - 1, hv_Width - 1);
            }
            using (HDevDisposeHelper dh = new HDevDisposeHelper())
            {
                ho_ImageResult1.Dispose();
                HOperatorSet.PaintRegion(ho_Rectangle1, ho_DupImage, out ho_ImageResult1, ((hv_targetGray.TupleConcat(
                    hv_targetGray))).TupleConcat(hv_targetGray), "fill");
            }

            HOperatorSet.OverpaintGray(ho_ImageResult1, ho_ImageReduced);


            //compose3 (ImageResult1, ImageResult2, ImageResult3, ImageResult)



            ho_Image.Dispose();
            ho_Rectangle.Dispose();
            ho_Rectangle11.Dispose();
            ho_ImageZoomed.Dispose();
            ho_ImageEmpty.Dispose();
            ho_ImageR.Dispose();
            ho_ImageG.Dispose();
            ho_ImageB.Dispose();
            ho_MultiImage.Dispose();
            ho_DupImage.Dispose();
            ho_ImageAffineTrans.Dispose();
            //ho_ImageReduced.Dispose();
            ho_Rectangle1.Dispose();
            ho_ImageResult1.Dispose();

            hv_Height.Dispose();
            hv_Width.Dispose();
            hv_targetGray.Dispose();
            hv_Width1.Dispose();
            hv_Height1.Dispose();
            hv_factor.Dispose();
            hv_HomMat2DIdentity.Dispose();

            return ho_ImageReduced;
        }
    }
}
