using HalconDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HY.Devices.Algorithm;
using System.Drawing.Imaging;



namespace HY.Devices.Algorithm.ZbomHome
{

    public class WoodBoradCombineImage
    {

        private static void Read_TypesRegion(out HObject ho_SortedRegions, HTuple hv_TypesPath)
        {



            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 

            HObject ho_EmptyObject, ho_R = null;

            // Local control variables 

            HTuple hv_RegionFiles = new HTuple(), hv_Index = new HTuple();
            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);
            HOperatorSet.GenEmptyObj(out ho_R);
            hv_RegionFiles.Dispose();
            HOperatorSet.ListFiles(hv_TypesPath, (new HTuple("files")).TupleConcat("follow_links"),
                out hv_RegionFiles);
            {
                HTuple ExpTmpOutVar_0;
                HOperatorSet.TupleRegexpSelect(hv_RegionFiles, (new HTuple("\\.(hobj)$")).TupleConcat(
                    "ignore_case"), out ExpTmpOutVar_0);
                hv_RegionFiles.Dispose();
                hv_RegionFiles = ExpTmpOutVar_0;
            }
            ho_EmptyObject.Dispose();
            HOperatorSet.GenEmptyObj(out ho_EmptyObject);
            for (hv_Index = 0; (int)hv_Index <= (int)((new HTuple(hv_RegionFiles.TupleLength()
                )) - 1); hv_Index = (int)hv_Index + 1)
            {
                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    ho_R.Dispose();
                    HOperatorSet.ReadRegion(out ho_R, hv_RegionFiles.TupleSelect(hv_Index));
                }
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_R, ho_EmptyObject, out ExpTmpOutVar_0);
                    ho_EmptyObject.Dispose();
                    ho_EmptyObject = ExpTmpOutVar_0;
                }
            }
            ho_SortedRegions.Dispose();
            HOperatorSet.SortRegion(ho_EmptyObject, out ho_SortedRegions, "character", "true",
                "row");
            ho_EmptyObject.Dispose();
            ho_R.Dispose();

            hv_RegionFiles.Dispose();
            hv_Index.Dispose();

            return;
        }

        [System.Runtime.ExceptionServices.HandleProcessCorruptedStateExceptions]
        public static bool CombineImage(string fileName, string fileNameRet, IEnumerable<Bitmap> bitmaps, List<HY.Devices.Algorithm.TargetResult> targetResults, BoardPosition boardPosition, string TypeIconFilePath)
        {


            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 
            List<HObject> ho_Images = new List<HObject>();
            Dictionary<string, int> TypesDict = new Dictionary<string, int>();
            TypesDict.Add("Base", 0);
            TypesDict.Add("Crack", 1);
            TypesDict.Add("Edge", 2);
            TypesDict.Add("Groove", 3);
            TypesDict.Add("Defect", 4);
            TypesDict.Add("Hole", 5);
            TypesDict.Add("Chip", 6);



            HObject ho_SortedRegions, ho_TotalImages;
            HObject ho_TiledImage, ho_TiledLabelImageR, ho_TiledLabelImageG;
            HObject ho_TiledLabelImageB, ho_MultiChannelLabelImage;
            HObject ho_Rectangle1 = null, ho_Rectangle2 = null, ho_RegionDifference = null;
            HObject ho_TypeOriginal = null, ho_TypeRegion = null, ho_RegionMoved = null;
            HObject ho_RegionUnion = null, ho_RegionLines, ho_RegionUnionLine;
            HObject ho_RegionDilation;


            // Local control variables 

            HTuple hv_targetResults = new HTuple(), hv_TypesPath = new HTuple();
            HTuple hv_Channels = new HTuple();

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();




            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_TotalImages);
            HOperatorSet.GenEmptyObj(out ho_TiledImage);
            HOperatorSet.GenEmptyObj(out ho_TiledLabelImageR);
            HOperatorSet.GenEmptyObj(out ho_TiledLabelImageG);
            HOperatorSet.GenEmptyObj(out ho_TiledLabelImageB);
            HOperatorSet.GenEmptyObj(out ho_MultiChannelLabelImage);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_TypeOriginal);
            HOperatorSet.GenEmptyObj(out ho_TypeRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionMoved);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_RegionUnionLine);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);

            ho_TotalImages.Dispose();
            HOperatorSet.GenEmptyObj(out ho_TotalImages);
            //数据

            hv_TypesPath.Dispose();
            hv_TypesPath = TypeIconFilePath;
            //读取标注信息代码(初始化)
            ho_SortedRegions.Dispose();
            Read_TypesRegion(out ho_SortedRegions, hv_TypesPath);

            try
            {

                for (int i = 0; i < bitmaps.Count(); i++)
                {
                    try
                    {


                        HObject ho_Image;
                        HOperatorSet.GenEmptyObj(out ho_Image);

                        ho_Image.Dispose();
                        ho_Image = Utils.Ho_ImageHelper.Bitmap2HObject(bitmaps.ElementAt(i));
                        if (ho_Image != null)
                        {
                            ho_Images.Add(ho_Image);
                            if (i == 0)
                            {
                                hv_Channels.Dispose();
                                HOperatorSet.CountChannels(ho_Image, out hv_Channels);
                            }
                        }
                    }
                    catch (Exception)
                    {

                    }

                }
                //拼图
                if (ho_Images.Count == 0)
                {
                    return false;
                }
                for (int i = 0; i < ho_Images.Count; i++)
                {
                    HObject ExpTmpOutVar_0;
                    HOperatorSet.ConcatObj(ho_TotalImages, ho_Images[i], out ExpTmpOutVar_0);
                    ho_TotalImages.Dispose();
                    ho_TotalImages = ExpTmpOutVar_0;
                }

                ho_TiledImage.Dispose();
                HOperatorSet.TileImages(ho_TotalImages, out ho_TiledImage, 1, "vertical");

                if (hv_Channels == 3)
                {
                    ho_MultiChannelLabelImage.Dispose();
                    HOperatorSet.CopyImage(ho_TiledImage, out ho_MultiChannelLabelImage);
                }
                else
                {
                    //生成彩色图片和绘制区域
                    ho_TiledLabelImageR.Dispose();
                    HOperatorSet.CopyImage(ho_TiledImage, out ho_TiledLabelImageR);
                    ho_TiledLabelImageG.Dispose();
                    HOperatorSet.CopyImage(ho_TiledImage, out ho_TiledLabelImageG);
                    ho_TiledLabelImageB.Dispose();
                    HOperatorSet.CopyImage(ho_TiledImage, out ho_TiledLabelImageB);
                    ho_MultiChannelLabelImage.Dispose();
                    HOperatorSet.Compose3(ho_TiledLabelImageR, ho_TiledLabelImageG, ho_TiledLabelImageB,
                        out ho_MultiChannelLabelImage);
                }


                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_MultiChannelLabelImage, out hv_Width, out hv_Height);
                int factor = 3;


                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                {
                    HOperatorSet.ZoomImageSize(ho_MultiChannelLabelImage, out ho_MultiChannelLabelImage, hv_Width / factor, hv_Height / factor,
                        "constant");
                }



                Task SaveLabelImg = Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(fileNameRet))
                    {
                        try
                        {
                            foreach (var item in targetResults)
                            {
                                int hv_I;
                                TypesDict.TryGetValue(item.TypeName, out hv_I);


                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    ho_Rectangle1.Dispose();
                                    HOperatorSet.GenRectangle1(out ho_Rectangle1, item.Row1 / factor,
                                        item.Column1 / factor, item.Row2 / factor, item.Column2 / factor);
                                }
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    ho_Rectangle2.Dispose();
                                    HOperatorSet.GenRectangle1(out ho_Rectangle2, item.Row1 / factor - 5,
                                        item.Column1 / factor - 5, item.Row2 / factor + 5,
                                        item.Column2 / factor + 5);
                                }
                                ho_RegionDifference.Dispose();
                                HOperatorSet.Difference(ho_Rectangle2, ho_Rectangle1, out ho_RegionDifference
                                    );
                                ho_TypeOriginal.Dispose();
                                HOperatorSet.SelectObj(ho_SortedRegions, out ho_TypeOriginal, 7 - hv_I);
                                ho_TypeRegion.Dispose();
                                HOperatorSet.CopyObj(ho_TypeOriginal, out ho_TypeRegion, 1, 1);
                                using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                {
                                    ho_RegionMoved.Dispose();
                                    HOperatorSet.MoveRegion(ho_TypeRegion, out ho_RegionMoved, item.Row1 / factor, item.Column1 / factor);
                                }
                                ho_RegionUnion.Dispose();
                                HOperatorSet.Union2(ho_RegionDifference, ho_RegionMoved, out ho_RegionUnion
                                    );
                                int val = hv_I;
                                switch (val)
                                {
                                    case 6:
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            HOperatorSet.OverpaintRegion(ho_MultiChannelLabelImage, ho_RegionUnion, new HTuple(246).TupleConcat(25).TupleConcat(76), "fill");
                                        }
                                        break;
                                    case 5:
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            HOperatorSet.OverpaintRegion(ho_MultiChannelLabelImage, ho_RegionUnion, new HTuple(61).TupleConcat(196).TupleConcat(76), "fill");
                                        }
                                        break;
                                    case 4:
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            HOperatorSet.OverpaintRegion(ho_MultiChannelLabelImage, ho_RegionUnion, new HTuple(272).TupleConcat(241).TupleConcat(25), "fill");
                                        }
                                        break;
                                    case 3:
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            HOperatorSet.OverpaintRegion(ho_MultiChannelLabelImage, ho_RegionUnion, new HTuple(70).TupleConcat(153).TupleConcat(144), "fill");
                                        }
                                        break;
                                    case 2:
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            HOperatorSet.OverpaintRegion(ho_MultiChannelLabelImage, ho_RegionUnion, new HTuple(66).TupleConcat(228).TupleConcat(260), "fill");
                                        }
                                        break;
                                    case 1:
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            HOperatorSet.OverpaintRegion(ho_MultiChannelLabelImage, ho_RegionUnion, new HTuple(128).TupleConcat(0).TupleConcat(0), "fill");
                                        }
                                        break;
                                    case 0:
                                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                        {
                                            HOperatorSet.OverpaintRegion(ho_MultiChannelLabelImage, ho_RegionUnion, new HTuple(255).TupleConcat(50).TupleConcat(246), "fill");
                                        }
                                        break;
                                }

                            }

                            ho_RegionLines.Dispose();
                            HOperatorSet.GenRegionLine(out ho_RegionLines,
                               new HTuple(boardPosition.Row1 / factor).TupleConcat(boardPosition.Row2 / factor).TupleConcat(boardPosition.Row3 / factor).TupleConcat(boardPosition.Row4 / factor),
                               new HTuple(boardPosition.Col1 / factor).TupleConcat(boardPosition.Col2 / factor).TupleConcat(boardPosition.Col3 / factor).TupleConcat(boardPosition.Col4 / factor),
                               new HTuple(boardPosition.Row2 / factor).TupleConcat(boardPosition.Row3 / factor).TupleConcat(boardPosition.Row4 / factor).TupleConcat(boardPosition.Row1 / factor),
                               new HTuple(boardPosition.Col2 / factor).TupleConcat(boardPosition.Col3 / factor).TupleConcat(boardPosition.Col4 / factor).TupleConcat(boardPosition.Col1 / factor));

                            ho_RegionUnionLine.Dispose();
                            HOperatorSet.Union1(ho_RegionLines, out ho_RegionUnionLine);
                            ho_RegionDilation.Dispose();
                            HOperatorSet.DilationCircle(ho_RegionUnionLine, out ho_RegionDilation, 6);
                            HOperatorSet.OverpaintRegion(ho_MultiChannelLabelImage, ho_RegionDilation, new HTuple(66).TupleConcat(228).TupleConcat(260), "fill");

                            HOperatorSet.WriteImage(ho_MultiChannelLabelImage, "jpeg 80", 0, fileNameRet);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                });


                Task SaveImg = Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        HOperatorSet.WriteImage(ho_TiledImage, "jpeg 80", 0, fileName);
                    }
                });


                Task.WaitAll(SaveImg, SaveLabelImg);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {
                hv_Width.Dispose();
                hv_Height.Dispose();

                hv_Channels.Dispose();

                ho_Images?.ForEach(a => a?.Dispose());
                ho_SortedRegions.Dispose();
                ho_TotalImages.Dispose();
                ho_TiledImage.Dispose();
                ho_TiledLabelImageR.Dispose();
                ho_TiledLabelImageG.Dispose();
                ho_TiledLabelImageB.Dispose();
                ho_MultiChannelLabelImage.Dispose();
                ho_Rectangle1.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionDifference.Dispose();
                ho_TypeOriginal.Dispose();
                ho_TypeRegion.Dispose();
                ho_RegionMoved.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionLines.Dispose();
                ho_RegionUnionLine.Dispose();
                ho_RegionDilation.Dispose();

                hv_targetResults.Dispose();
                hv_TypesPath.Dispose();
            }

        }

        public static bool ZoomImage(string fileName, string fileNameRet, Bitmap bitmap, int RotateValue, List<HY.Devices.Algorithm.TargetResult> targetResults, BoardPosition boardPosition, string TypeIconFilePath)
        {


            // Stack for temporary objects 
            HObject[] OTemp = new HObject[20];

            // Local iconic variables 
            Dictionary<string, int> TypesDict = new Dictionary<string, int>();
            TypesDict.Add("Base", 0);
            TypesDict.Add("Chip", 1);//
            TypesDict.Add("Crack", 2);
            TypesDict.Add("Defect", 3);//
            TypesDict.Add("Edge", 4);
            TypesDict.Add("Groove", 5);
            TypesDict.Add("Hole", 6);

            HObject ho_SortedRegions, ho_TotalImages;
            HObject ho_TiledImage, ho_TiledLabelImageR, ho_TiledLabelImageG;
            HObject ho_TiledLabelImageB, ho_MultiChannelLabelImage;
            HObject ho_Rectangle1 = null, ho_Rectangle2 = null, ho_RegionDifference = null;
            HObject ho_TypeOriginal = null, ho_TypeRegion = null, ho_RegionMoved = null;
            HObject ho_RegionUnion = null, ho_RegionLines, ho_RegionUnionLine;
            HObject ho_RegionDilation;


            // Local control variables 

            HTuple hv_targetResults = new HTuple(), hv_TypesPath = new HTuple();
            HTuple hv_Channels = new HTuple();

            HTuple hv_Width = new HTuple(), hv_Height = new HTuple();
            HObject ho_ImageZoom;



            // Initialize local and output iconic variables 
            HOperatorSet.GenEmptyObj(out ho_SortedRegions);
            HOperatorSet.GenEmptyObj(out ho_TotalImages);
            HOperatorSet.GenEmptyObj(out ho_TiledImage);
            HOperatorSet.GenEmptyObj(out ho_TiledLabelImageR);
            HOperatorSet.GenEmptyObj(out ho_TiledLabelImageG);
            HOperatorSet.GenEmptyObj(out ho_TiledLabelImageB);
            HOperatorSet.GenEmptyObj(out ho_MultiChannelLabelImage);
            HOperatorSet.GenEmptyObj(out ho_Rectangle1);
            HOperatorSet.GenEmptyObj(out ho_Rectangle2);
            HOperatorSet.GenEmptyObj(out ho_RegionDifference);
            HOperatorSet.GenEmptyObj(out ho_TypeOriginal);
            HOperatorSet.GenEmptyObj(out ho_TypeRegion);
            HOperatorSet.GenEmptyObj(out ho_RegionMoved);
            HOperatorSet.GenEmptyObj(out ho_RegionUnion);
            HOperatorSet.GenEmptyObj(out ho_RegionLines);
            HOperatorSet.GenEmptyObj(out ho_RegionUnionLine);
            HOperatorSet.GenEmptyObj(out ho_RegionDilation);


            HOperatorSet.GenEmptyObj(out ho_ImageZoom);

            ho_TotalImages.Dispose();
            HOperatorSet.GenEmptyObj(out ho_TotalImages);
            //数据

            hv_TypesPath.Dispose();
            hv_TypesPath = TypeIconFilePath;
            //读取标注信息代码(初始化)
            ho_SortedRegions.Dispose();

            HObject ho_Image;
            HOperatorSet.GenEmptyObj(out ho_Image);

            try
            {


                Read_TypesRegion(out ho_SortedRegions, hv_TypesPath);
                ho_Image.Dispose();
                ho_Image = Utils.Ho_ImageHelper.Bitmap2HObject(bitmap);
                HOperatorSet.RotateImage(ho_Image, out ho_Image, RotateValue, "constant");
                //ho_Image = ho_ImageRotated;
                hv_Channels.Dispose();
                HOperatorSet.CountChannels(ho_Image, out hv_Channels);

                if ((int)(new HTuple(hv_Channels.TupleEqual(1))) != 0)
                {
                    {
                        HObject ExpTmpOutVar_0;
                        HOperatorSet.CfaToRgb(ho_Image, out ExpTmpOutVar_0, "bayer_gb", "bilinear");
                        ho_Image.Dispose();
                        ho_Image = ExpTmpOutVar_0;
                    }
                    ho_MultiChannelLabelImage.Dispose();
                    HOperatorSet.CopyImage(ho_Image, out ho_MultiChannelLabelImage);
                }
                else
                {
                    ho_MultiChannelLabelImage.Dispose();
                    HOperatorSet.CopyImage(ho_Image, out ho_MultiChannelLabelImage);
                }



                hv_Width.Dispose(); hv_Height.Dispose();
                HOperatorSet.GetImageSize(ho_MultiChannelLabelImage, out hv_Width, out hv_Height);

                Task SaveLabelImg = Task.Run(() =>
                {

                    if (!string.IsNullOrEmpty(fileNameRet))
                    {



                        int factor = 1;

                        if (hv_Width * hv_Height > 60000000)
                        {
                            factor = 2;
                        }

                        using (HDevDisposeHelper dh = new HDevDisposeHelper())
                        {
                            ho_ImageZoom.Dispose();
                            HOperatorSet.ZoomImageSize(ho_MultiChannelLabelImage, out ho_ImageZoom, hv_Width / factor, hv_Height / factor,
                                "constant");
                        }
                        foreach (var item in targetResults)
                        {
                            int hv_I;
                            TypesDict.TryGetValue(item.TypeName, out hv_I);


                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_Rectangle1.Dispose();
                                HOperatorSet.GenRectangle1(out ho_Rectangle1, item.Row1 / factor,
                                        item.Column1 / factor, item.Row2 / factor, item.Column2 / factor);
                            }
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_Rectangle2.Dispose();
                                HOperatorSet.GenRectangle1(out ho_Rectangle2, item.Row1 / factor - 5,
                                        item.Column1 / factor - 5, item.Row2 / factor + 5,
                                        item.Column2 / factor + 5);
                            }
                            ho_RegionDifference.Dispose();
                            HOperatorSet.Difference(ho_Rectangle2, ho_Rectangle1, out ho_RegionDifference
                                    );
                            ho_TypeOriginal.Dispose();
                            HOperatorSet.SelectObj(ho_SortedRegions, out ho_TypeOriginal, 7 - hv_I);
                            ho_TypeRegion.Dispose();
                            HOperatorSet.CopyObj(ho_TypeOriginal, out ho_TypeRegion, 1, 1);
                            using (HDevDisposeHelper dh = new HDevDisposeHelper())
                            {
                                ho_RegionMoved.Dispose();
                                HOperatorSet.MoveRegion(ho_TypeRegion, out ho_RegionMoved, item.Row1 / factor, item.Column1 / factor);
                            }
                            ho_RegionUnion.Dispose();
                            HOperatorSet.Union2(ho_RegionDifference, ho_RegionMoved, out ho_RegionUnion
                                    );
                            int val = hv_I;
                            switch (val)
                            {
                                case 6:
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.OverpaintRegion(ho_ImageZoom, ho_RegionUnion, new HTuple(246).TupleConcat(25).TupleConcat(76), "fill");
                                    }
                                    break;
                                case 5:
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.OverpaintRegion(ho_ImageZoom, ho_RegionUnion, new HTuple(61).TupleConcat(196).TupleConcat(76), "fill");
                                    }
                                    break;
                                case 4:
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.OverpaintRegion(ho_ImageZoom, ho_RegionUnion, new HTuple(272).TupleConcat(241).TupleConcat(25), "fill");
                                    }
                                    break;
                                case 3:
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.OverpaintRegion(ho_ImageZoom, ho_RegionUnion, new HTuple(70).TupleConcat(153).TupleConcat(144), "fill");
                                    }
                                    break;
                                case 2:
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.OverpaintRegion(ho_ImageZoom, ho_RegionUnion, new HTuple(66).TupleConcat(228).TupleConcat(260), "fill");
                                    }
                                    break;
                                case 1:
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.OverpaintRegion(ho_ImageZoom, ho_RegionUnion, new HTuple(128).TupleConcat(0).TupleConcat(0), "fill");
                                    }
                                    break;
                                case 0:
                                    using (HDevDisposeHelper dh = new HDevDisposeHelper())
                                    {
                                        HOperatorSet.OverpaintRegion(ho_ImageZoom, ho_RegionUnion, new HTuple(255).TupleConcat(50).TupleConcat(246), "fill");
                                    }
                                    break;
                            }

                        }

                        //if (!(boardPosition.Row1 == 0 && boardPosition.Col1 == 0 && boardPosition.Row2 == 0 && boardPosition.Col2 == 0 && boardPosition.Row3 == 0 && boardPosition.Col3 == 0 && boardPosition.Row4 == 0 && boardPosition.Col4 == 0))
                        //{
                        //    ho_RegionLines.Dispose();
                        //    HOperatorSet.GenRegionLine(out ho_RegionLines,
                        //           new HTuple(boardPosition.Row1 / factor).TupleConcat(boardPosition.Row2 / factor).TupleConcat(boardPosition.Row3).TupleConcat(boardPosition.Row4),
                        //           new HTuple(boardPosition.Col1 / factor).TupleConcat(boardPosition.Col2 / factor).TupleConcat(boardPosition.Col3).TupleConcat(boardPosition.Col4),
                        //           new HTuple(boardPosition.Row2 / factor).TupleConcat(boardPosition.Row3 / factor).TupleConcat(boardPosition.Row4 / factor).TupleConcat(boardPosition.Row1 / factor),
                        //           new HTuple(boardPosition.Col2 / factor).TupleConcat(boardPosition.Col3 / factor).TupleConcat(boardPosition.Col4 / factor).TupleConcat(boardPosition.Col1 / factor));

                        //    ho_RegionUnionLine.Dispose();
                        //    HOperatorSet.Union1(ho_RegionLines, out ho_RegionUnionLine);
                        //    ho_RegionDilation.Dispose();
                        //    HOperatorSet.DilationCircle(ho_RegionUnionLine, out ho_RegionDilation, 6);
                        //    HOperatorSet.OverpaintRegion(ho_ImageZoom, ho_RegionDilation, new HTuple(66).TupleConcat(228).TupleConcat(260), "fill");
                        //}

                        HOperatorSet.WriteImage(ho_ImageZoom, "jpeg 80", 0, fileNameRet);
                    }
                });
                Task SaveImg = Task.Run(() =>
                {
                    if (!string.IsNullOrEmpty(fileName))
                    {
                        if (hv_Width * hv_Height > 60000000)
                        {
                            HOperatorSet.WriteImage(ho_Image, "tiff", 0, fileName);
                        }
                        else
                        {
                            HOperatorSet.WriteImage(ho_Image, "jpeg 80", 0, fileName);
                        }
                    }
                });
                Task.WaitAll(SaveImg, SaveLabelImg);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            finally
            {

                hv_Channels.Dispose();
                ho_Image.Dispose();
                ho_ImageZoom.Dispose();
                bitmap.Dispose();
                ho_SortedRegions.Dispose();
                ho_TotalImages.Dispose();
                ho_TiledImage.Dispose();
                ho_TiledLabelImageR.Dispose();
                ho_TiledLabelImageG.Dispose();
                ho_TiledLabelImageB.Dispose();
                ho_MultiChannelLabelImage.Dispose();
                ho_Rectangle1.Dispose();
                ho_Rectangle2.Dispose();
                ho_RegionDifference.Dispose();
                ho_TypeOriginal.Dispose();
                ho_TypeRegion.Dispose();
                ho_RegionMoved.Dispose();
                ho_RegionUnion.Dispose();
                ho_RegionLines.Dispose();
                ho_RegionUnionLine.Dispose();
                ho_RegionDilation.Dispose();

                hv_Width.Dispose();
                hv_Height.Dispose();

                hv_targetResults.Dispose();
                hv_TypesPath.Dispose();
            }

        }

    }

    public class BoardPosition
    {
        public double Row1 { set; get; }
        public double Row2 { set; get; }
        public double Row3 { set; get; }
        public double Row4 { set; get; }
        public double Col1 { set; get; }
        public double Col2 { set; get; }
        public double Col3 { set; get; }
        public double Col4 { set; get; }

    }

}

