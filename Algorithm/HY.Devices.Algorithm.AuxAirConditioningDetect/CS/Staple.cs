using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using HY.Devices.Algorithm;


namespace HY.Devices.Algorithm.AuxAirConditioningDetect
{
    //封箱钉
    [Export(typeof(IAlgorithm))]
    public class Staple : AbstractAlgorithm, IAlgorithm
    {
        private static readonly object _lockObj = new object();
        private static Staple _instance;
        public static Staple Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObj)
                    {
                        if (_instance == null)
                        {
                            _instance = new Staple();
                        }
                    }
                }
                return _instance;
            }
        }
        public override AlgorithmTypes AlgorithmType => AlgorithmTypes.DetectTarget;
        public override Dictionary<string, dynamic> InitParamNames { get; } = new Dictionary<string, dynamic> { };

        public override Dictionary<string, dynamic> ActionParamNames { get; } = new Dictionary<string, dynamic> { };
        public override bool Init(Dictionary<string, dynamic> initParameters)
        {
            try
            {
                return IsInit = true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public override void UnInit()
        {
        }
        public override Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams)
        {
            if (!IsInit)
            {
                throw new Exception("未初始化模型");
            }
            Dictionary<string, dynamic> results = new Dictionary<string, dynamic>();
            return results;
        }
    }
}
