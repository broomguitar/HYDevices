using System.Collections.Generic;

namespace HY.Devices.Algorithm
{
    public interface IAlgorithm
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        bool IsAvailable { get; }
        /// <summary>
        /// 算法类型
        /// </summary>
        AlgorithmTypes AlgorithmType { get; }
        /// <summary>
        /// 是否初始化
        /// </summary>
        bool IsInit { get; }
        /// <summary>
        /// 是否保存运算过程中生成的图片
        /// </summary>
        bool IsSaveImg { get; set; }
        /// <summary>
        /// 保存图片的文件夹
        /// </summary>
        string SaveDir { set; get; }
        /// <summary>
        /// 初始化参数集合
        /// </summary>
        Dictionary<string, dynamic> InitParamNames { get; }
        /// <summary>
        /// 执行参数集合
        /// </summary>
        Dictionary<string, dynamic> ActionParamNames { get; }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="initParameters"></param>
        /// <returns></returns>
        bool Init(Dictionary<string, dynamic> initParameters);
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="actionParams"></param>
        /// <returns></returns>
        Dictionary<string, dynamic> DoAction(Dictionary<string, dynamic> actionParams);
        /// <summary>
        /// 卸载
        /// </summary>
        void UnInit();
    }
}
