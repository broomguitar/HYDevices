using System;
using System.Collections.Generic;

namespace HY.Devices.Printer
{
    /// <summary>
    /// 打印机模块
    /// </summary>
    public interface IHYPrinter : IDisposable
    {
        /// <summary>
        /// 连接状态
        /// </summary>
        bool IsConnected { get; }
        /// <summary>
        /// 连接方式
        /// </summary>
        HYConnectTypes ConnectType { get; }
        /// <summary>
        /// 打印机名称
        /// </summary>
        string PrinterName { get; }
        /// <summary>
        /// 模板文件路径
        /// </summary>
        string TemplatePath { get; set; }
        /// <summary>
        /// 模板填充信息
        /// </summary>
        Dictionary<string, string> TemplateInfos { get; set; }
        /// <summary>
        /// 是否剥离模式，默认true
        /// </summary>
        bool IsPeel { get; set; }
        /// <summary>
        /// 获取打印机状态
        /// </summary>
        HYPrinterStatus GetPrinterStatus();
        /// <summary>
        /// 剥离模式下，判断是否有纸未取
        /// </summary>
        bool IsHasPaper();
        /// <summary>
        /// 打印
        /// </summary>
        /// <param name="btwPath">打印文件</param>
        /// <returns></returns>
        HYPrintResult Print(string btwPath);
        /// <summary>
        /// 打印,打印前设置 TemplatePath
        /// </summary>
        /// <returns></returns>
        HYPrintResult Print();

    }
    /// <summary>
    /// 打印机连接方式
    /// </summary>
    public enum HYConnectTypes
    {
        USB = 0,
        ETHERNET = 1,
        COM = 2,
        LPT = 3
    }
    /// <summary>
    /// 打印机状态
    /// </summary>
    public enum HYPrinterStatus
    {
        待机 = 0x00,
        打印头开启 = 0x01,
        纸张卡纸 = 0x02,
        纸张卡纸并且打印头开启 = 0x03,
        缺纸 = 0x04,
        缺纸并且打印头开启 = 0x05,
        无碳带 = 0x08,
        无碳带并且打印头开启 = 0x09,
        无碳带并且卡纸 = 0x0A,
        无碳带卡纸并且打印头开启 = 0x0B,
        无碳带并且缺纸 = 0x0C,
        无碳带缺纸并且打印头开启 = 0x0D,
        暂停 = 0x10,
        打印中 = 0x20,
        其它错误 = 0x80,
    }
    /// <summary>
    /// 打印返回值
    /// </summary>
    public enum HYPrintResult
    {
        Success = 0,
        Timeout = 1,
        Failure = 2
    }
}
