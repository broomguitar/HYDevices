using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm
{
   public class OpenposeModel
    {
        private const int MaxObjects = 250;
        [StructLayout(LayoutKind.Sequential)]
        public struct OpenposePoint
        {
            public float x, y, prob;
        };
        [StructLayout(LayoutKind.Sequential)]
        public struct OpenposeResult
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxObjects)]
            public OpenposePoint[] points;
        };
        public class PointValue
        {
            public float X { set; get; }
            public float Y { set; get; }
            public float Score { set; get; }
        }
        public class BodyPoint
        {
            public BodyPoint(OpenposePoint[] datas)
            {
                Nose = new PointValue { X = datas[0].x, Y = datas[0].y, Score = datas[0].prob };
                Neck = new PointValue { X = datas[1].x, Y = datas[1].y, Score = datas[1].prob };
                RShoulder = new PointValue { X = datas[2].x, Y = datas[2].y, Score = datas[2].prob };
                RElbow = new PointValue { X = datas[3].x, Y = datas[3].y, Score = datas[3].prob };
                RWrist = new PointValue { X = datas[4].x, Y = datas[4].y, Score = datas[4].prob };
                LShoulder = new PointValue { X = datas[5].x, Y = datas[5].y, Score = datas[5].prob };
                LElbow = new PointValue { X = datas[6].x, Y = datas[6].y, Score = datas[6].prob };
                LWrist = new PointValue { X = datas[7].x, Y = datas[7].y, Score = datas[7].prob };
                MidHip = new PointValue { X = datas[8].x, Y = datas[8].y, Score = datas[8].prob };
                RHip = new PointValue { X = datas[9].x, Y = datas[9].y, Score = datas[9].prob };
                RKnee = new PointValue { X = datas[10].x, Y = datas[10].y, Score = datas[10].prob };
                RAnkle = new PointValue { X = datas[11].x, Y = datas[11].y, Score = datas[11].prob };
                LHip = new PointValue { X = datas[12].x, Y = datas[12].y, Score = datas[12].prob };
                LKnee = new PointValue { X = datas[13].x, Y = datas[13].y, Score = datas[13].prob };
                LAnkle = new PointValue { X = datas[14].x, Y = datas[14].y, Score = datas[14].prob };
                REye = new PointValue { X = datas[15].x, Y = datas[15].y, Score = datas[15].prob };
                LEye = new PointValue { X = datas[16].x, Y = datas[16].y, Score = datas[16].prob };
                REar = new PointValue { X = datas[17].x, Y = datas[17].y, Score = datas[17].prob };
                LEar = new PointValue { X = datas[18].x, Y = datas[18].y, Score = datas[18].prob };
                LBigToe = new PointValue { X = datas[19].x, Y = datas[19].y, Score = datas[19].prob };
                LSmallToe = new PointValue { X = datas[20].x, Y = datas[20].y, Score = datas[20].prob };
                LHeel = new PointValue { X = datas[21].x, Y = datas[21].y, Score = datas[21].prob };
                RBigToe = new PointValue { X = datas[22].x, Y = datas[22].y, Score = datas[22].prob };
                RSmallToe = new PointValue { X = datas[23].x, Y = datas[23].y, Score = datas[23].prob };
                RHeel = new PointValue { X = datas[24].x, Y = datas[24].y, Score = datas[24].prob };
            }
            /// <summary>
            /// 鼻子
            /// </summary>
            public PointValue Nose { set; get; } = new PointValue();
            /// <summary>
            /// 脖子
            /// </summary>
            public PointValue Neck { set; get; } = new PointValue();
            /// <summary>
            /// 右侧肩膀
            /// </summary>
            public PointValue RShoulder { set; get; } = new PointValue();
            /// <summary>
            /// 右肘
            /// </summary>
            public PointValue RElbow { set; get; } = new PointValue();
            /// <summary>
            /// 右拳
            /// </summary>
            public PointValue RWrist { set; get; } = new PointValue();
            /// <summary>
            /// 左侧肩膀
            /// </summary>
            public PointValue LShoulder { set; get; } = new PointValue();
            /// <summary>
            /// 左肘
            /// </summary>
            public PointValue LElbow { set; get; } = new PointValue();
            /// <summary>
            /// 左拳
            /// </summary>
            public PointValue LWrist { set; get; } = new PointValue();
            /// <summary>
            /// 中臀部
            /// </summary>
            public PointValue MidHip { set; get; } = new PointValue();
            /// <summary>
            /// 右臀部
            /// </summary>
            public PointValue RHip { set; get; } = new PointValue();
            /// <summary>
            /// 右膝
            /// </summary>
            public PointValue RKnee { set; get; } = new PointValue();
            /// <summary>
            /// 右脚踝
            /// </summary>
            public PointValue RAnkle { set; get; } = new PointValue();
            /// <summary>
            /// 左臀部
            /// </summary>
            public PointValue LHip { set; get; } = new PointValue();
            /// <summary>
            /// 左膝
            /// </summary>
            public PointValue LKnee { set; get; } = new PointValue();
            /// <summary>
            /// 左脚踝
            /// </summary>
            public PointValue LAnkle { set; get; } = new PointValue();
            /// <summary>
            /// 右眼
            /// </summary>
            public PointValue REye { set; get; } = new PointValue();
            /// <summary>
            /// 左眼
            /// </summary>
            public PointValue LEye { set; get; } = new PointValue();
            /// <summary>
            /// 右耳
            /// </summary>
            public PointValue REar { set; get; } = new PointValue();
            /// <summary>
            /// 左耳
            /// </summary>
            public PointValue LEar { set; get; } = new PointValue();
            /// <summary>
            /// 左大脚趾
            /// </summary>
            public PointValue LBigToe { set; get; } = new PointValue();
            /// <summary>
            /// 左小脚趾
            /// </summary>
            public PointValue LSmallToe { set; get; } = new PointValue();
            /// <summary>
            /// 左后脚跟
            /// </summary>
            public PointValue LHeel { set; get; } = new PointValue();
            /// <summary>
            /// 右大脚趾
            /// </summary>
            public PointValue RBigToe { set; get; } = new PointValue();
            /// <summary>
            /// 右小脚趾
            /// </summary>
            public PointValue RSmallToe { set; get; } = new PointValue();
            /// <summary>
            /// 右后脚跟
            /// </summary>
            public PointValue RHeel { set; get; } = new PointValue();

        }
    }
}
