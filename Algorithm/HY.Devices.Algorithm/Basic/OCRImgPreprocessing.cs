using HalconDotNet;
using System;

namespace HY.Devices.Algorithm
{
    /// <summary>
    /// OCR预处理图片
    /// </summary>
    public class OCRImgPreprocessing
    {
        private static readonly object _lockObj = new object();
        private static OCRImgPreprocessing _instance;
        public static OCRImgPreprocessing Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new OCRImgPreprocessing();
                        }
                    }
                }
                return _instance;
            }
        }

        private void GetPowerImageTB(HObject ho_SourceImage, out HObject ho_PowerImage)
        {



            // Local iconic variables 

            HObject ho_ImageScaled1, ho_Edges, ho_UnionContours;
            HObject ho_SelectedXLD, ho_Region1, ho_RegionTrans, ho_ImageReduced1;
            HObject ho_ImagePart1, ho_ImageScaled, ho_ImageEmphasize;
            HObject ho_Region = null, ho_RegionOpening = null, ho_ConnectedRegions = null;
            HObject ho_SelectedRegions = null, ho_ImagePart = null;

            // Local control variables 

            HTuple hv_Number = null, hv_Width = null, hv_Height = null;
            HTuple hv_Row1 = new HTuple(), hv_Column1 = new HTuple();
            HTuple hv_Row2 = new HTuple(), hv_Column2 = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_PowerImage);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled1);
            HOperatorSet.GenEmptyObj(out ho_Edges);
            HOperatorSet.GenEmptyObj(out ho_UnionContours);
            HOperatorSet.GenEmptyObj(out ho_SelectedXLD);
            HOperatorSet.GenEmptyObj(out ho_Region1);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_ImagePart1);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageEmphasize);
            HOperatorSet.GenEmptyObj(out ho_Region);
            HOperatorSet.GenEmptyObj(out ho_RegionOpening);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_ImagePart);
            ho_ImageScaled1.Dispose();
            HOperatorSet.ScaleImage(ho_SourceImage, out ho_ImageScaled1, 1, 0);
            ho_Edges.Dispose();
            HOperatorSet.EdgesSubPix(ho_ImageScaled1, out ho_Edges, "canny", 3, 20, 40);
            ho_UnionContours.Dispose();
            HOperatorSet.UnionAdjacentContoursXld(ho_Edges, out ho_UnionContours, 50, 50,
                "attr_keep");

            //union_cotangential_contours_xld (Edges, UnionContours, 2, 'auto', 3.1415926, 40, 60, 2, 'attr_forget')
            ho_SelectedXLD.Dispose();
            HOperatorSet.SelectShapeXld(ho_UnionContours, out ho_SelectedXLD, "area", "and",
                350000, 3800000);
            HOperatorSet.CountObj(ho_SelectedXLD, out hv_Number);
            if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
            {
                ho_ImageScaled1.Dispose();
                HOperatorSet.ScaleImage(ho_SourceImage, out ho_ImageScaled1, 2, 0);
                ho_Edges.Dispose();
                HOperatorSet.EdgesSubPix(ho_ImageScaled1, out ho_Edges, "canny", 3, 20, 40);
                ho_UnionContours.Dispose();
                HOperatorSet.UnionCotangentialContoursXld(ho_Edges, out ho_UnionContours, 0,
                    30, 0.785398, 25, 10, 2, "attr_forget");
                ho_SelectedXLD.Dispose();
                HOperatorSet.SelectShapeXld(ho_UnionContours, out ho_SelectedXLD, "area", "and",
                    350000, 3800000);
            }
            ho_Region1.Dispose();
            HOperatorSet.GenRegionContourXld(ho_SelectedXLD, out ho_Region1, "filled");
            ho_RegionTrans.Dispose();
            HOperatorSet.ShapeTrans(ho_Region1, out ho_RegionTrans, "rectangle1");
            ho_ImageReduced1.Dispose();
            HOperatorSet.ReduceDomain(ho_SourceImage, ho_RegionTrans, out ho_ImageReduced1
                );
            ho_ImagePart1.Dispose();
            HOperatorSet.CropDomain(ho_ImageReduced1, out ho_ImagePart1);

            ho_ImageScaled.Dispose();
            HOperatorSet.ScaleImage(ho_ImagePart1, out ho_ImageScaled, 1, 0);
            ho_ImageEmphasize.Dispose();
            HOperatorSet.Emphasize(ho_ImageScaled, out ho_ImageEmphasize, 5, 5, 3);
            HOperatorSet.GetImageSize(ho_ImageEmphasize, out hv_Width, out hv_Height);
            if ((int)(new HTuple(hv_Height.TupleGreater(1400))) != 0)
            {
                ho_Region.Dispose();
                HOperatorSet.Threshold(ho_ImageEmphasize, out ho_Region, 0, 150);
                ho_RegionOpening.Dispose();
                HOperatorSet.OpeningRectangle1(ho_Region, out ho_RegionOpening, 5, 5);
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionOpening, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShapeStd(ho_ConnectedRegions, out ho_SelectedRegions, "max_area",
                    70);
                HOperatorSet.SmallestRectangle1(ho_SelectedRegions, out hv_Row1, out hv_Column1,
                    out hv_Row2, out hv_Column2);
                ho_ImagePart.Dispose();
                HOperatorSet.CropRectangle1(ho_ImageEmphasize, out ho_ImagePart, hv_Row1, hv_Column1,
                    hv_Row2, hv_Column2);
                ho_PowerImage.Dispose();
                HOperatorSet.CopyImage(ho_ImagePart, out ho_PowerImage);
            }
            else
            {
                ho_PowerImage.Dispose();
                HOperatorSet.CopyImage(ho_ImageEmphasize, out ho_PowerImage);
            }
            ho_ImageScaled1.Dispose();
            ho_Edges.Dispose();
            ho_UnionContours.Dispose();
            ho_SelectedXLD.Dispose();
            ho_Region1.Dispose();
            ho_RegionTrans.Dispose();
            ho_ImageReduced1.Dispose();
            ho_ImagePart1.Dispose();
            ho_ImageScaled.Dispose();
            ho_ImageEmphasize.Dispose();
            ho_Region.Dispose();
            ho_RegionOpening.Dispose();
            ho_ConnectedRegions.Dispose();
            ho_SelectedRegions.Dispose();
            ho_ImagePart.Dispose();

            return;
        }

        // Main procedure 
        private bool GetPowerPart_TB(HObject hb_Image, string NewImagePath, int Height)
        {
            bool Result = true;
            HObject ho_Image, ho_Image1, ho_Image2, ho_Image3;
            HObject ho_PowerImage;

            // Local control variables 

            HTuple hv_Number = null;
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_PowerImage);
            try
            {
                ho_Image.Dispose();
                HOperatorSet.CopyImage(hb_Image, out ho_Image);
                ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                HOperatorSet.Decompose3(ho_Image, out ho_Image1, out ho_Image2, out ho_Image3
                    );
                ho_PowerImage.Dispose();
                GetPowerImageTB(ho_Image1, out ho_PowerImage);
                HOperatorSet.CountObj(ho_PowerImage, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                {
                    ho_PowerImage.Dispose();
                    GetPowerImageTB(ho_Image2, out ho_PowerImage);
                    HOperatorSet.CountObj(ho_PowerImage, out hv_Number);
                    if ((int)(new HTuple(hv_Number.TupleEqual(0))) != 0)
                    {
                        ho_PowerImage.Dispose();
                        GetPowerImageTB(ho_Image3, out ho_PowerImage);
                    }
                }
                HOperatorSet.CountObj(ho_PowerImage, out hv_Number);
                if ((int)(new HTuple(hv_Number.TupleGreater(0))) != 0)
                {
                    HOperatorSet.WriteImage(ho_PowerImage, "jpeg", 0, NewImagePath);
                }
                else
                {
                    Result = false;
                }

            }
            catch (Exception ex)
            {
                Result = false;
                Console.WriteLine(ex);
            }
            finally
            {
                ho_Image.Dispose();
                ho_Image1.Dispose();
                ho_Image2.Dispose();
                ho_Image3.Dispose();
                ho_PowerImage.Dispose();

            }

            return Result;
        }

        private bool GetPowerPart(HObject hb_Image, string NewImagePath, int Height)
        {
            bool Result = true;


            // Local iconic variables 

            HObject ho_Image = null, ho_Image1 = null, ho_Image2 = null;
            HObject ho_Image3 = null, ho_Region2 = null, ho_Region3 = null;
            HObject ho_Region4 = null, ho_RegionIntersection = null, ho_RegionIntersection1 = null;
            HObject ho_ConnectedRegions = null, ho_SelectedRegions = null;
            HObject ho_RegionUnion = null, ho_RegionClosing = null, ho_RegionTrans = null;
            HObject ho_ImageReduced1 = null, ho_ImagePart1 = null, ho_ImageScaled = null;
            HObject ho_ImageEmphasize = null;

            // Local control variables 

            HTuple hv_ImageFiles = null, hv_Index = null;
            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_Image);
            HOperatorSet.GenEmptyObj(out ho_Image1);
            HOperatorSet.GenEmptyObj(out ho_Image2);
            HOperatorSet.GenEmptyObj(out ho_Image3);
            HOperatorSet.GenEmptyObj(out ho_Region2);
            HOperatorSet.GenEmptyObj(out ho_Region3);
            HOperatorSet.GenEmptyObj(out ho_Region4);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection);
            HOperatorSet.GenEmptyObj(out ho_RegionIntersection1);
            HOperatorSet.GenEmptyObj(out ho_ConnectedRegions);
            HOperatorSet.GenEmptyObj(out ho_SelectedRegions);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionClosing);
            HOperatorSet.GenEmptyObj(out ho_RegionTrans);
            HOperatorSet.GenEmptyObj(out ho_ImageReduced1);
            HOperatorSet.GenEmptyObj(out ho_ImagePart1);
            HOperatorSet.GenEmptyObj(out ho_ImageScaled);
            HOperatorSet.GenEmptyObj(out ho_ImageEmphasize);
            try
            {
                ho_Image.Dispose();
                HOperatorSet.CopyImage(hb_Image, out ho_Image);
                //HOperatorSet.ReadImage(out ho_Image, hv_ImageFiles.TupleSelect(hv_Index));
                ho_Image1.Dispose(); ho_Image2.Dispose(); ho_Image3.Dispose();
                HOperatorSet.Decompose3(ho_Image, out ho_Image1, out ho_Image2, out ho_Image3
                    );
                ho_Region2.Dispose();
                HOperatorSet.Threshold(ho_Image1, out ho_Region2, 10, 45);
                ho_Region3.Dispose();
                HOperatorSet.Threshold(ho_Image2, out ho_Region3, 30, 100);
                ho_Region4.Dispose();
                HOperatorSet.Threshold(ho_Image3, out ho_Region4, 55, 150);
                ho_RegionIntersection.Dispose();
                HOperatorSet.Intersection(ho_Region2, ho_Region3, out ho_RegionIntersection
                    );
                ho_RegionIntersection1.Dispose();
                HOperatorSet.Intersection(ho_RegionIntersection, ho_Region4, out ho_RegionIntersection1
                    );
                ho_ConnectedRegions.Dispose();
                HOperatorSet.Connection(ho_RegionIntersection1, out ho_ConnectedRegions);
                ho_SelectedRegions.Dispose();
                HOperatorSet.SelectShape(ho_ConnectedRegions, out ho_SelectedRegions, "area",
                    "and", 13000, 99999999);
                ho_RegionUnion.Dispose();
                HOperatorSet.Union1(ho_SelectedRegions, out ho_RegionUnion);
                ho_RegionClosing.Dispose();
                HOperatorSet.ClosingCircle(ho_RegionUnion, out ho_RegionClosing, 10);
                ho_RegionTrans.Dispose();
                HOperatorSet.ShapeTrans(ho_RegionClosing, out ho_RegionTrans, "convex");

                ho_ImageReduced1.Dispose();
                HOperatorSet.ReduceDomain(ho_Image2, ho_RegionTrans, out ho_ImageReduced1);
                ho_ImagePart1.Dispose();
                HOperatorSet.CropDomain(ho_ImageReduced1, out ho_ImagePart1);

                ho_ImageScaled.Dispose();
                HOperatorSet.ScaleImage(ho_ImagePart1, out ho_ImageScaled, 2, 0);
                ho_ImageEmphasize.Dispose();
                HOperatorSet.Emphasize(ho_ImageScaled, out ho_ImageEmphasize, 5, 5, 3);
                HOperatorSet.GetImageSize(ho_ImageEmphasize, out hv_Width, out hv_Height);
                //HOperatorSet.ClearWindow(hv_ExpDefaultWinHandle);
                //HOperatorSet.DispObj(ho_ImageEmphasize, hv_ExpDefaultWinHandle);
                HOperatorSet.WriteImage(ho_ImageEmphasize, "jpeg", 0, NewImagePath);
                Result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Result = false;
            }
            finally
            {
                ho_Image.Dispose();
                ho_Image1.Dispose();
                ho_Image2.Dispose();
                ho_Image3.Dispose();
                ho_Region2.Dispose();
                ho_Region3.Dispose();
                ho_Region4.Dispose();
                ho_RegionIntersection.Dispose();
                ho_RegionIntersection1.Dispose();
                ho_ConnectedRegions.Dispose();
                ho_SelectedRegions.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionClosing.Dispose();
                ho_RegionTrans.Dispose();
                ho_ImageReduced1.Dispose();
                ho_ImagePart1.Dispose();
                ho_ImageScaled.Dispose();
                ho_ImageEmphasize.Dispose();
            }

            return Result;
        }
        public bool GetPowerPart(string ImagePath, string NewImagePath)
        {
            try
            {
                HOperatorSet.ReadImage(out HObject hb_Image, ImagePath);
                return GetPowerPart(hb_Image, NewImagePath, 750);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool GetPowerPart_TB(string ImagePath, string NewImagePath)
        {
            try
            {
                HOperatorSet.ReadImage(out HObject hb_Image, ImagePath);
                return GetPowerPart_TB(hb_Image, NewImagePath, 1400);
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }
    }
}
