using HalconDotNet;
using System;
using System.IO;
using System.Text;

namespace HY.Devices.Algorithm.Utils
{
    public class Hdict2TxtHelper
    {
        public static bool Hdict2Txt(string hdcitFilename, string txtFilename)
        {
            try
            {
                if (!File.Exists(hdcitFilename))
                {
                    throw new Exception($"{hdcitFilename}文件不存在");
                }
                if (File.Exists(txtFilename))
                {
                    File.Delete(txtFilename);
                }
                if (string.IsNullOrEmpty(txtFilename))
                {
                    txtFilename = hdcitFilename + ".txt";
                }
                HTuple hdict = new HTuple();
                HOperatorSet.ReadDict(hdcitFilename, new HTuple(), new HTuple(), out hdict);
                DictDatas dictDatas = getDictInfo(hdict, txtFilename);
                getLabels(dictDatas, txtFilename);
                return true;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static DictDatas getDictInfo(HTuple hdict, string saveInfoPath)
        {
            HTuple AllKeys = new HTuple();
            DictDatas dictDatas = new DictDatas();
            HOperatorSet.GetDictParam(hdict, "keys", new HTuple(), out AllKeys);
            if (AllKeys.Length != 0)
            {
                try
                {
                    HOperatorSet.GetDictTuple(hdict, "class_ids", out dictDatas.class_ids);
                    HOperatorSet.GetDictTuple(hdict, "class_names", out dictDatas.class_names);
                    HOperatorSet.GetDictTuple(hdict, "samples", out dictDatas.samples);
                    HOperatorSet.GetDictTuple(hdict, "image_dir", out dictDatas.image_dir);

                    StringBuilder class_ids = new StringBuilder();
                    StringBuilder class_names = new StringBuilder();
                    class_ids.Append("class_ids:");
                    class_names.Append("class_names:");
                    for (int i = 0; i < dictDatas.class_ids.Length; i++)
                    {
                        class_ids.Append(dictDatas.class_ids.TupleSelect(i).ToString() + ";");
                        class_names.Append(dictDatas.class_names.TupleSelect(i).ToString() + ";");
                    }

                    string[] lines = new string[4];
                    lines[0] = class_ids.ToString();
                    lines[1] = class_names.ToString();
                    lines[2] = "image_dir:" + dictDatas.image_dir.ToString();
                    lines[3] = "samples:";

                    File.AppendAllLines(saveInfoPath, lines);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                throw new Exception("hdict文件为空！");
            }
            return dictDatas;
        }
        private static void getLabels(DictDatas dictDatas, string saveInfoPath)
        {
            try
            {
                HTuple selectsample = dictDatas.samples.TupleSelect(0);
                HTuple AllKeys = new HTuple();
                HOperatorSet.GetDictParam(selectsample, "keys", new HTuple(), out AllKeys);
                if (AllKeys.Length == 7)
                {
                    //image_id; image_file_name; bbox_label_id; bbox_row1; bbox_col1; bbox_row2; bbox_col2;
                    File.AppendAllText(saveInfoPath, "image_id;image_file_name;bbox_label_id;bbox_row1;bbox_col;bbox_row2;bbox_col2" + "\r\n");
                    for (int i = 0; i < dictDatas.samples.Length; i++)
                    {
                        selectsample = dictDatas.samples.TupleSelect(i);
                        HOperatorSet.GetDictParam(selectsample, "keys", new HTuple(), out AllKeys);

                        DictObjectItems doi = new DictObjectItems();
                        HOperatorSet.GetDictTuple(selectsample, "image_id", out doi.image_id);
                        HOperatorSet.GetDictTuple(selectsample, "image_file_name", out doi.image_file_name);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_label_id", out doi.bbox_label_id);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_row1", out doi.bbox_row);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_col1", out doi.bbox_col);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_row2", out doi.bbox_length1);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_col2", out doi.bbox_length2);

                        string[] lines = new string[doi.bbox_label_id.Length];
                        for (int k = 0; k < doi.bbox_label_id.Length; k++)
                        {
                            lines[k] = doi.image_id + ";" + doi.image_file_name + ";" + doi.bbox_label_id.TupleSelect(k) + ";" + doi.bbox_row.TupleSelect(k) + ";"
                                + doi.bbox_col.TupleSelect(k) + ";" + doi.bbox_length1.TupleSelect(k) + ";" + doi.bbox_length2.TupleSelect(k);
                        }
                        File.AppendAllLines(saveInfoPath, lines);
                    }
                }
                else if (AllKeys.Length == 8)
                {
                    //['image_file_name', 'image_id', 'bbox_row', 'bbox_col', 'bbox_length1', 'bbox_length2', 'bbox_phi', 'bbox_label_id']
                    File.AppendAllText(saveInfoPath, "image_id;image_file_name;bbox_label_id;bbox_row;bbox_col;bbox_length1;bbox_length2;bbox_phi" + "\r\n");
                    for (int i = 0; i < dictDatas.samples.Length; i++)
                    {
                        selectsample = dictDatas.samples.TupleSelect(i);
                        HOperatorSet.GetDictParam(selectsample, "keys", new HTuple(), out AllKeys);

                        DictObjectItems doi = new DictObjectItems();
                        HOperatorSet.GetDictTuple(selectsample, "image_id", out doi.image_id);
                        HOperatorSet.GetDictTuple(selectsample, "image_file_name", out doi.image_file_name);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_label_id", out doi.bbox_label_id);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_row", out doi.bbox_row);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_col", out doi.bbox_col);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_length1", out doi.bbox_length1);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_length2", out doi.bbox_length2);
                        HOperatorSet.GetDictTuple(selectsample, "bbox_phi", out doi.bbox_phi);

                        string[] lines = new string[doi.bbox_label_id.Length];
                        for (int k = 0; k < doi.bbox_label_id.Length; k++)
                        {
                            lines[k] = doi.image_id + ";" + doi.image_file_name + ";" + doi.bbox_label_id.TupleSelect(k) + ";" + doi.bbox_row.TupleSelect(k) + ";"
                                + doi.bbox_col.TupleSelect(k) + ";" + doi.bbox_length1.TupleSelect(k) + ";" + doi.bbox_length2.TupleSelect(k) + ";" + doi.bbox_phi.TupleSelect(k);
                        }
                        File.AppendAllLines(saveInfoPath, lines);
                    }
                }
                else
                {
                    throw new Exception("此文件格式不支持,无法正常读取！");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static bool Txt2Hdict(string txtFilename, string hdcitFilename)
        {
            try
            {
                if (!System.IO.File.Exists(txtFilename))
                {
                    throw new Exception($"{txtFilename}文件不存在");
                }
                if (File.Exists(hdcitFilename))
                {
                    File.Delete(hdcitFilename);
                }
                if (string.IsNullOrEmpty(hdcitFilename))
                {
                    hdcitFilename = txtFilename + ".hdcit";
                }

                HOperatorSet.WriteDict(getHdic(txtFilename), hdcitFilename, new HTuple(), new HTuple());
                return true;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private static HTuple getHdic(string txtFilename)
        {
            try
            {
                HTuple Hdic = new HTuple();
                #region MyRegion
                HTuple hTuple_ids = new HTuple();
                HTuple hTuple_names = new HTuple();
                for (int i = 0; i < 5; i++)
                {
                    hTuple_ids.Append(i);
                    hTuple_names.Append(i.ToString());
                }
                HOperatorSet.CreateDict(out Hdic);
                HOperatorSet.SetDictTuple(Hdic, "class_ids", hTuple_ids);
                HOperatorSet.SetDictTuple(Hdic, "class_names", hTuple_names);
                HOperatorSet.SetDictTuple(Hdic, "image_dir", "image_dir");
                HTuple hTuple_lables = new HTuple();
                for (int i = 0; i < 100; i++)
                {
                    HTuple hTuple_lable = new HTuple();
                    HOperatorSet.CreateDict(out hTuple_lable);
                    HOperatorSet.SetDictTuple(hTuple_lable, "image_id", i);
                    HOperatorSet.SetDictTuple(hTuple_lable, "image_file_name", $"image_file_{i}");
                    HOperatorSet.SetDictTuple(hTuple_lable, "bbox_label_id", i % 6);
                    HOperatorSet.SetDictTuple(hTuple_lable, "bbox_row", i * 11.2);
                    HOperatorSet.SetDictTuple(hTuple_lable, "bbox_col", i * 9.3);
                    HOperatorSet.SetDictTuple(hTuple_lable, "bbox_length1", i * 7.5);
                    HOperatorSet.SetDictTuple(hTuple_lable, "bbox_length2", i * 8.3);
                    HOperatorSet.SetDictTuple(hTuple_lable, "bbox_phi", 0.1);
                    hTuple_lables.Append(hTuple_lable);
                }
                HOperatorSet.SetDictTuple(Hdic, "samples", hTuple_lables);
                #endregion
                return Hdic;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
    class DictDatas
    {
        //分类名
        public HTuple class_names;
        //分类ID
        public HTuple class_ids;
        //图片文件夹路径
        public HTuple image_dir;
        //图片标注信息，包括名称、标签、目标矩形框坐标等
        public HTuple samples;
    }
    class DictObjectItems
    {
        public HTuple image_id;
        public HTuple image_file_name;
        public HTuple bbox_label_id;
        public HTuple bbox_row;
        public HTuple bbox_col;
        public HTuple bbox_length1;
        public HTuple bbox_length2;
        public HTuple bbox_phi;
    }

}
