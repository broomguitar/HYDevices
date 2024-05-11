using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Yolov7.YoloV7
{
    public class Prediction
    {
        public Box Box { get; set; }
        public string Label { get; set; }
        public float Confidence { get; set; }
    }
    internal class DetectProperty
    {
        public string Name { get; set; }
        public float Value { get; set; }
    }

    //public class Prediction
    //{
    //    public Box Box { set; get; }
    //    public string Label { set; get; }
    //    public float Confidence { set; get; }
    //}
    public class Box
    {
        public Box(float xMin, float yMin, float xMax, float yMax)
        {
            Xmin = xMin;
            Ymin = yMin;
            Xmax = xMax;
            Ymax = yMax;
        }
        public float Xmin { set; get; }
        public float Xmax { set; get; }
        public float Ymin { set; get; }
        public float Ymax { set; get; }
    }
}
