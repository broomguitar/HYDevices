using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HY.Devices.Algorithm.Haier_TeBing
{
    /// <summary>
    /// 门平算法
    /// </summary>
    [Export(typeof(IAlgorithm))]
    public class DoorPlane :AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static DoorPlane _instance;
        public static DoorPlane Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new DoorPlane();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DoorPlane;

        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic>();

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { { "InputBackPointsList",new List<double>(4)}, { "FontPoints_Top", new List<double>(2) }, { "FontPoints_Middle", new List<double>(2) }, { "FontPoints_Bottom", new List<double>(2) }, { "FrontLaserSpace", 0 }, { "TopUtilData", 0 }, { "MiddleUtilData", 0 }, { "BottomUtilData", 0 }, { "BackLaserSpace_Hor", 0 }, { "BackLaserSpace_Ver", 0 }, { "TopAlpha", 0 }, { "MiddleAlpha", 0}, { "BottomAlpha", 0 } };
        public override bool Init(Dictionary<string, object> initParameters)
        {

            return IsInit = true;
        }        /*
         * 传入值参数 :  后4激光位置传入数据 Input1 ，Input2， Input3，Input4 ， 前2 激光位置传入数据  Input5， input6,input7,input8,input9,input10
         * 传入值参数 :  前激光相对水平位置数值  FrontHeight1,FrontHeight2,FrontHeight3 
         * 传入值参数 :  输出于实际数据补偿系数  alpha
         * 固定数据 : 后激光设备水平距离及垂直间距 BackWidth，BackHeight ， 前激光设备水平距离 FrontWidth
         */
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            try
            {
                if (!IsInit)
                {
                    throw new Exception("未初始化模型");
                }
                Dictionary<string, object> results = new Dictionary<string, object>();

                //{ 490.1, 490.8, 488.3, 485.675 }
                double Input1 = actionParams["InputBackPointsList"][0], Input2 = actionParams["InputBackPointsList"][1], Input3 = actionParams["InputBackPointsList"][2], Input4 = actionParams["InputBackPointsList"][3];
                double Input5 = actionParams["FontPoints_Top"][0], input6 = actionParams["FontPoints_Top"][1];
                double input7 = actionParams["FontPoints_Middle"][0], input8 = actionParams["FontPoints_Middle"][1];
                double input9 = actionParams["FontPoints_Bottom"][0], input10 = actionParams["FontPoints_Bottom"][1];

                // 全局变量
                double FrontWidth = actionParams["FrontLaserSpace"];
                double FrontHeight1 = actionParams["TopUtilData"], FrontHeight2 = actionParams["MiddleUtilData"], FrontHeight3 = actionParams["BottomUtilData"];
                double BackWidth = actionParams["BackLaserSpace_Hor"];
                double BackHeight = actionParams["BackLaserSpace_Ver"];
                // 调整系数
                List<double> AlphaParam = new List<double> { actionParams["TopAlpha"], actionParams["MiddleAlpha"], actionParams["BottomAlpha"] };

                // 组成后4个激光的 空间坐标 集合
                BackPoints BackPointsList = new BackPoints
                {
                    TopLeft = new Points { X = 0, Y = 0, Z = Input1 },
                    TopRight = new Points { X = BackWidth, Y = 0, Z = Input2 },
                    BottomLeft = new Points { X = 0, Y = BackHeight, Z = Input3 },
                    BottomRight = new Points { X = BackWidth, Y = BackHeight, Z = Input4 }
                };
                //  前6个点的空间坐标 集合
                List<ForwardPoints> ForwardPoints = new List<ForwardPoints>();

                //  前6个点的空间坐标 
                ForwardPoints X1 = new ForwardPoints { LeftPoint = new Points { X = (BackWidth / 2 - FrontWidth / 2), Y = FrontHeight1, Z = Input5 }, RightPoint = new Points { X = (BackWidth / 2 - FrontWidth / 2 + FrontWidth), Y = FrontHeight1, Z = input6 } };
                ForwardPoints X2 = new ForwardPoints { LeftPoint = new Points { X = (BackWidth / 2 - FrontWidth / 2), Y = FrontHeight2, Z = input7 }, RightPoint = new Points { X = (BackWidth / 2 - FrontWidth / 2 + FrontWidth), Y = FrontHeight2, Z = input8 } };
                ForwardPoints X3 = new ForwardPoints { LeftPoint = new Points { X = (BackWidth / 2 - FrontWidth / 2), Y = FrontHeight3, Z = input9 }, RightPoint = new Points { X = (BackWidth / 2 - FrontWidth / 2 + FrontWidth), Y = FrontHeight3, Z = input10 } };

                ForwardPoints.Add(X1);
                ForwardPoints.Add(X2);
                ForwardPoints.Add(X3);


                List<double> res = Calculation(BackPointsList, ForwardPoints, ref AlphaParam);
                results.Add("result", res);
                return results;
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
            }
        }
        public override void UnInit()
        {
        }
        // 背后4个点 每个点（x,y,z）
        public class Points
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }

        }
        public class BackPoints
        {
            public Points TopLeft { get; set; }

            public Points TopRight { get; set; }

            public Points BottomLeft { get; set; }

            public Points BottomRight { get; set; }

        }

        // 正面6个点 每个点（x,y,z）
        public class ForwardPoints
        {
            public Points LeftPoint { get; set; }

            public Points RightPoint { get; set; }


        }



        // 计算总体逻辑函数，传入参数（param1=背后4个点，param2=前面6个点,param3=）
        private static List<double> Calculation(BackPoints X1, List<ForwardPoints> Y1, ref List<double> alpha)
        {
            List<double> ResultList = new List<double>();

            for (int i = 0; i < Y1.Count; i++)
            {
                //left_disp = ((back_point3.z - back_point1.z) / (back_point3.y - back_point1.y)) * (Res_Height - 0) + back_point1.z
                //right_disp = ((back_point4.z - back_point2.z) / (back_point4.y - back_point2.y)) * (Res_Height - 0) + back_point2.z

                double left_disp = (X1.BottomLeft.Z - X1.TopLeft.Z) / (X1.BottomLeft.Y - X1.TopLeft.Y) * (Y1[i].LeftPoint.Y - 0) + X1.BottomLeft.Z;
                double right_disp = ((X1.BottomRight.Z - X1.TopRight.Z) / (X1.BottomRight.Y - X1.TopRight.Y)) * (Y1[i].RightPoint.Y - 0) + X1.BottomRight.Z;

                //Temporary_left_point = Point_info(0, Res_Height, left_disp)
                //Temporary_right_point = Point_info(back_width_distance, Res_Height, right_disp)

                Points Temporary_left_point = new Points() { X = X1.TopLeft.X, Y = Y1[i].LeftPoint.Y, Z = left_disp };
                Points Temporary_right_point = new Points() { X = X1.TopRight.X, Y = Y1[i].RightPoint.Y, Z = right_disp };

                double res_left = ((Temporary_right_point.Z - Temporary_left_point.Z) / (
                Temporary_right_point.X - Temporary_left_point.X)) * (
                        Y1[i].LeftPoint.X - Temporary_left_point.X) + Temporary_left_point.Z;
                double res_right = ((Temporary_right_point.Z - Temporary_left_point.Z) / (
                Temporary_right_point.X - Temporary_left_point.X)) * (
                        Y1[i].RightPoint.X - Temporary_left_point.X) + Temporary_left_point.Z;
                double result = Math.Abs((res_left + Y1[i].LeftPoint.Z) - (res_right + Y1[i].RightPoint.Z)) * alpha[i];
                ResultList.Add(result);
            }
            return ResultList;
        }
    }
}
