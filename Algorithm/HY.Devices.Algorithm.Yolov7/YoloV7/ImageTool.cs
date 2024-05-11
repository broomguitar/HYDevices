using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Yolov7.YoloV7
{
    public class ImageTool
    {
        public static List<Prediction> Supress(List<Prediction> items, float Standardverlop)
        {
            List<Prediction> result = new List<Prediction>(items);

            foreach (var item in items) // iterate every prediction
            {
                foreach (var current in result.ToList()) // make a copy for each iteration
                {
                    if (current == item) continue;

                    //var (rect1, rect2) = (item.Box, current.Box);
                    var rect1 = RectangleF.FromLTRB(item.Box.Xmin, item.Box.Ymin, item.Box.Xmax, item.Box.Ymax);
                    var rect2 = RectangleF.FromLTRB(current.Box.Xmin, current.Box.Ymin, current.Box.Xmax, current.Box.Ymax);
                    RectangleF intersection = RectangleF.Intersect(rect1, rect2);

                    float intArea = intersection.Width * intersection.Height; // intersection area
                    float unionArea = rect1.Width * rect1.Height + rect2.Width * rect2.Height - intArea; // union area
                    float overlap = intArea / unionArea; // overlap ratio

                    if (overlap >= Standardverlop)
                    {
                        if (item.Confidence >= current.Confidence)
                        {
                            result.Remove(current);
                        }
                    }
                }
            }
            return result;
        }
    }
}
