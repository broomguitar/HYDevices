using HalconDotNet;
using System;
using System.Collections.Generic;

namespace HY.Devices.Algorithm
{
    public abstract class AbstractAlgorithm : IAlgorithm
    {
        public bool IsAvailable { get; protected set; } = true;
        public abstract AlgorithmTypes AlgorithmType { get; }
        public bool IsInit { get; protected set; }
        public bool IsSaveImg { get; set; }
        public string SaveDir { set; get; } = AppDomain.CurrentDomain.BaseDirectory;
        /// <summary>
        /// 标注图路径
        /// </summary>
        public virtual string MarkerImagePath => System.IO.Path.Combine(SaveDir, "标注图");
        /// <summary>
        /// 保存小图的目录
        /// </summary>
        public virtual string PartImagePath => System.IO.Path.Combine(SaveDir, "PartImage");

        public abstract Dictionary<string, dynamic> InitParamNames { get; }

        public abstract Dictionary<string, dynamic> ActionParamNames { get; }

        public abstract bool Init(Dictionary<string, dynamic> initParameters);
        public abstract Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams);
        public abstract void UnInit();
    }
}
