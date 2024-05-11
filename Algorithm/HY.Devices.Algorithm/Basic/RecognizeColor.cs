using HalconDotNet;
using System;
using System.Drawing;
using System.IO;

namespace HY.Devices.Algorithm
{
    /// <summary>
    /// 色差检测
    /// </summary>
    public class RecognizeColor
    {
        private static readonly object _lockObj = new object();
        private static RecognizeColor _instance;
        public static RecognizeColor Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new RecognizeColor();
                        }
                    }
                }
                return _instance;
            }
        }
        private void gen_features(HObject ho_Image, out HTuple hv_FeatureVector)
        {
            hv_FeatureVector = new HTuple();
            gen_sobel_features(ho_Image, hv_FeatureVector, out hv_FeatureVector);
            hv_FeatureVector = hv_FeatureVector.TupleReal();
        }


        private void gen_sobel_features(HObject ho_Image, HTuple hv_Features, out HTuple hv_FeaturesExtended)
        {

            HObject ho_EdgeAmplitude, ho_R, ho_G, ho_B;
            HObject ho_ImageResult1, ho_ImageResult2, ho_ImageResult3;

            // Local control variables 

            HTuple hv_Energy = null, hv_Correlation = null;
            HTuple hv_Homogeneity = null, hv_Contrast = null, hv_AbsoluteHistoEdgeAmplitude2 = null;
            HTuple hv_Mean = null, hv_Deviation = null, hv_AbsoluteHistoEdgeAmplitude = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_EdgeAmplitude);
            HOperatorSet.GenEmptyObj(out ho_R);
            HOperatorSet.GenEmptyObj(out ho_G);
            HOperatorSet.GenEmptyObj(out ho_B);
            HOperatorSet.GenEmptyObj(out ho_ImageResult1);
            HOperatorSet.GenEmptyObj(out ho_ImageResult2);
            HOperatorSet.GenEmptyObj(out ho_ImageResult3);
            //Coocurrence matrix for 0 deg and 90 deg:
            HOperatorSet.CoocFeatureImage(ho_Image, ho_Image, 8, "mean", out hv_Energy, out hv_Correlation,
                out hv_Homogeneity, out hv_Contrast);
            //cooc_feature_image (Image, Image, 6, 90, Energy, Correlation, Homogeneity, Contrast)
            //Absolute histogram of edge amplitudes:
            ho_EdgeAmplitude.Dispose();
            HOperatorSet.SobelAmp(ho_Image, out ho_EdgeAmplitude, "thin_max_abs", 3);
            ho_EdgeAmplitude.Dispose();
            HOperatorSet.RobinsonAmp(ho_Image, out ho_EdgeAmplitude);
            HOperatorSet.GrayHistoAbs(ho_EdgeAmplitude, ho_EdgeAmplitude, 8, out hv_AbsoluteHistoEdgeAmplitude2);

            ho_R.Dispose(); ho_G.Dispose(); ho_B.Dispose();
            HOperatorSet.Decompose3(ho_Image, out ho_R, out ho_G, out ho_B);
            ho_ImageResult1.Dispose(); ho_ImageResult2.Dispose(); ho_ImageResult3.Dispose();
            HOperatorSet.TransFromRgb(ho_R, ho_G, ho_B, out ho_ImageResult1, out ho_ImageResult2,
                out ho_ImageResult3, "hsv");
            HOperatorSet.Intensity(ho_ImageResult1, ho_ImageResult1, out hv_Mean, out hv_Deviation);
            HOperatorSet.GrayHistoAbs(ho_ImageResult1, ho_ImageResult1, 8, out hv_AbsoluteHistoEdgeAmplitude);


            //Entropy and anisotropy:
            //entropy_gray (Image, Image, Entropy, Anisotropy)
            //Absolute histogram of gray values:
            //gray_histo_abs (Image, Image, 8, AbsoluteHistoImage)
            //Add features to feature vector:

            hv_FeaturesExtended = new HTuple();
            hv_FeaturesExtended = hv_FeaturesExtended.TupleConcat(hv_Mean);
            hv_FeaturesExtended = hv_FeaturesExtended.TupleConcat(hv_Deviation);
            hv_FeaturesExtended = hv_FeaturesExtended.TupleConcat(hv_Energy);
            hv_FeaturesExtended = hv_FeaturesExtended.TupleConcat(hv_Correlation);
            hv_FeaturesExtended = hv_FeaturesExtended.TupleConcat(hv_Homogeneity);
            hv_FeaturesExtended = hv_FeaturesExtended.TupleConcat(hv_Contrast);
            hv_FeaturesExtended = hv_FeaturesExtended.TupleConcat(hv_AbsoluteHistoEdgeAmplitude);
            hv_FeaturesExtended = hv_FeaturesExtended.TupleConcat(hv_AbsoluteHistoEdgeAmplitude2);


            //FeaturesExtended := [FeaturesExtended,Entropy,Anisotropy]
            //FeaturesExtended := [FeaturesExtended,AbsoluteHistoImage]
            ho_EdgeAmplitude.Dispose();
            ho_R.Dispose();
            ho_G.Dispose();
            ho_B.Dispose();
            ho_ImageResult1.Dispose();
            ho_ImageResult2.Dispose();
            ho_ImageResult3.Dispose();

            return;
        }

        private string Find(HObject ho_Image, string GmcPath, string NamesPath)
        {
            HTuple hv_Result = null, hv_ImageFiles = null;
            HTuple hv_Index = null, hv_MLPHandle = new HTuple(), hv_Classes = new HTuple();
            HTuple hv_FeatureVector = new HTuple(), hv_FoundClassIDs1 = new HTuple();
            HTuple hv_Confidence1 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            try
            {
                HOperatorSet.ReadClassMlp(GmcPath, out hv_MLPHandle);
                string[] classNames = File.ReadAllLines(NamesPath);
                for (int i = 0; i < classNames.Length; i++)
                {
                    hv_Classes[i] = classNames[i];
                }
                gen_features(ho_Image, out hv_FeatureVector);
                HOperatorSet.ClassifyClassMlp(hv_MLPHandle, hv_FeatureVector, 1, out hv_FoundClassIDs1,
                    out hv_Confidence1);
                hv_Result = hv_Classes.TupleSelect(hv_FoundClassIDs1);
                HOperatorSet.ClearClassMlp(hv_MLPHandle);
                return hv_Result.S;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ho_Image.Dispose();
            }
        }
        public string Find(string ImagePath, string GmcPath, string NamesPath)
        {
            try
            {
                HOperatorSet.ReadImage(out HObject ho_Image, ImagePath);
                return Find(ho_Image, GmcPath, NamesPath);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string Find(Bitmap bitmap, string GmcPath, string NamesPath)
        {
            try
            {
                return Find(Utils.Ho_ImageHelper.Bitmap2HObject(bitmap), GmcPath, NamesPath);

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
