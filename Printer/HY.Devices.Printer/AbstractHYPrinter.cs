using Seagull.BarTender.Print;
using System;
using System.Collections.Generic;
using System.IO;
using BTPrinter = Seagull.BarTender.Print.Printer;

namespace HY.Devices.Printer
{
    /// <summary>
    /// 打印机抽象类
    /// </summary>
    public abstract class AbstractHYPrinter : IHYPrinter
    {
        public virtual bool IsConnected => HYPrinterStatusUtil.CheckPrinter(PrinterName);
        public abstract HYConnectTypes ConnectType { get; }
        public string PrinterName { get; }
        public string TemplatePath { get; set; }
        public Dictionary<string, string> TemplateInfos { get; set; } = new Dictionary<string, string>();
        public bool IsPeel { get; set; } = true;

        protected BTPrinter BTprinter;
        protected Engine engine;
        public AbstractHYPrinter(string printerName)
        {
            try
            {
                PrinterName = printerName;
                BTprinter = new BTPrinter(printerName);
                engine = new Engine();
                engine.Start();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// 获取打印机状态
        /// </summary>
        /// <returns></returns>
        public abstract HYPrinterStatus GetPrinterStatus();
        /// <summary>
        /// 是否有纸未取，剥离模式下生效
        /// </summary>
        /// <returns></returns>
        public abstract bool IsHasPaper();
        public virtual HYPrintResult Print(string btwPath)
        {
            try
            {
                if (!IsConnected)
                {
                    throw new Exception("打印机未连接");
                }
                if (!File.Exists(btwPath))
                {
                    throw new Exception("模板文件不存在");
                }
                HYPrinterStatus status = GetPrinterStatus();
                if (status != HYPrinterStatus.待机)
                {
                    throw new Exception(status.ToString());
                }
                if (IsHasPaper())
                {
                    throw new Exception("有纸未取");
                }
                //engine.Start();
                LabelFormatDocument labelFormatDocument = engine.Documents.Open(btwPath);
                foreach (var item in TemplateInfos)
                {
                    labelFormatDocument.SubStrings[item.Key].Value = item.Value;//打印字段
                }
                //打印机名称
                labelFormatDocument.PrintSetup.PrinterName = PrinterName;
                //改变标签打印数份连载 
                labelFormatDocument.PrintSetup.NumberOfSerializedLabels = 1;
                //打印份数                   
                labelFormatDocument.PrintSetup.IdenticalCopiesOfLabel = 1;
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                Result nResult1 = labelFormatDocument.Print("BarPrint" + DateTime.Now, 2000);
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine(stopwatch.ElapsedMilliseconds);
                labelFormatDocument.PrintSetup.Cache.FlushInterval = CacheFlushInterval.PerSession;
                //不保存对打开模板的修改 
                labelFormatDocument.Close(SaveOptions.DoNotSaveChanges);
                //结束打印引擎
                return (HYPrintResult)nResult1;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public virtual HYPrintResult Print()
        {
            return Print(TemplatePath);
        }
        public virtual void Dispose()
        {
            engine?.Stop();
            engine?.Dispose();
            BTprinter = null;
        }
    }
}
