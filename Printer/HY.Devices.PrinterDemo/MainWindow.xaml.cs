using HY.Devices.Printer;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace HY.Devices.PrinterDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        IHYPrinter printer;
        private void btn_connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (printer != null)
                {
                    printer.Dispose();
                }
                if (cb_printerType.SelectedIndex < 0)
                {
                    MessageBox.Show("没选择打印机类型");
                    return;
                }
                if (string.IsNullOrEmpty(tb_printerName.Text))
                {
                    MessageBox.Show("打印机名称不能为空！");
                    return;
                }
                if (cb_printerType.SelectedIndex == 0)
                {
                    if (cb_connectType.SelectedIndex == 0)
                    {
                        printer = new HYPrinter_FanuuUSB(tb_printerName.Text);
                    }
                    else if (cb_connectType.SelectedIndex == 1)
                    {
                        printer = new HYPrinter_FanuuNET(tb_printerName.Text, tb_ip.Text, Convert.ToInt32(tb_port.Text));
                    }
                }
                else
                {
                    if (cb_connectType.SelectedIndex == 0)
                    {
                        printer = new HYPrinter_ZebraUSB(tb_printerName.Text);
                    }
                    else if (cb_connectType.SelectedIndex == 1)
                    {
                        printer = new HYPrinter_ZebraNET(tb_printerName.Text, tb_ip.Text, Convert.ToInt32(tb_port.Text));
                    }
                }
                elp_status.Fill = printer.IsConnected ? Brushes.Green : Brushes.Red;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        private void btn_selectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Btw文件|*.btw";
            if (ofd.ShowDialog() == true)
            {
                tb_btwFile.Text = ofd.FileName;
            }
        }
        private void btn_print_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (printer?.IsConnected != true)
                {
                    MessageBox.Show("打印机未连接");
                    return;
                }
                if (string.IsNullOrEmpty(tb_btwFile.Text))
                {
                    MessageBox.Show("没有模板文件");
                    return;
                }
                printer.IsPeel = check_isPeel.IsChecked == true;
                if (printer.IsHasPaper())
                {
                    MessageBox.Show("有纸未取");
                    return;
                }
                if (!int.TryParse(tb_printNum.Text, out int num))
                {
                    num = 1;
                }
                for (int i = 0; i < num; i++)
                {
                    while (printer.IsHasPaper())
                    {
                        System.Threading.Thread.Sleep(200);
                    }
                    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                    stopwatch.Start();
                    HYPrintResult ret = printer.Print(tb_btwFile.Text);
                    stopwatch.Stop();
                    tb_log.Clear();
                    switch (ret)
                    {
                        case HYPrintResult.Success:
                            tb_log.AppendText("打印成功，耗时" + stopwatch.ElapsedMilliseconds + "ms");
                            break;
                        case HYPrintResult.Timeout:
                            tb_log.AppendText("打印超时，耗时" + stopwatch.ElapsedMilliseconds + "ms");
                            break;
                        case HYPrintResult.Failure:
                            tb_log.AppendText("打印失败，耗时" + stopwatch.ElapsedMilliseconds + "ms");
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

        private void btn_checkPaper_Click(object sender, RoutedEventArgs e)
        {
            if (printer?.IsConnected != true)
            {
                MessageBox.Show("打印机未连接");
                return;
            }
            if (printer.IsHasPaper())
            {
                MessageBox.Show("有纸未取");
            }
            else
            {
                MessageBox.Show("空闲");
            }
        }

        private void btn_printStatus_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MessageBox.Show(printer.GetPrinterStatus().ToString());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_selectimFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "png文件|*.png";
            if (ofd.ShowDialog() == true)
            {
                tb_imgFile.Text = ofd.FileName;
            }
        }

        private void btn_testPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (printer?.IsConnected != true)
                {
                    MessageBox.Show("打印机未连接");
                    return;
                }
                if (string.IsNullOrEmpty(tb_btwFile.Text))
                {
                    MessageBox.Show("没有模板文件");
                    return;
                }
                if (printer.IsHasPaper())
                {
                    MessageBox.Show("有纸未取");
                    return;
                }
                printer.IsPeel = check_isPeel.IsChecked == true;
                printer.TemplatePath = tb_btwFile.Text;
                printer.TemplateInfos = new Dictionary<string, string>();
                printer.TemplateInfos["Image"] = tb_imgFile.Text;
                if (!string.IsNullOrEmpty(tb_barCode.Text))
                {
                    printer.TemplateInfos["barcode"] = tb_barCode.Text;
                }
                System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                stopwatch.Start();
                HYPrintResult ret = printer.Print(tb_btwFile.Text);
                stopwatch.Stop();
                tb_log.Clear();
                switch (ret)
                {
                    case HYPrintResult.Success:
                        tb_log.AppendText("打印成功，耗时" + stopwatch.ElapsedMilliseconds + "ms");
                        break;
                    case HYPrintResult.Timeout:
                        tb_log.AppendText("打印超时，耗时" + stopwatch.ElapsedMilliseconds + "ms");
                        break;
                    case HYPrintResult.Failure:
                        tb_log.AppendText("打印失败，耗时" + stopwatch.ElapsedMilliseconds + "ms");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btn_selectFolder_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowser.Description = "请选择图片所在的目录";
            //folderBrowser.ShowNewFolderButton = true;
            if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                this.tb_btwFolder.Text = folderBrowser.SelectedPath;
            }
        }

        private void btn_bactchPrint_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (printer?.IsConnected != true)
                {
                    MessageBox.Show("打印机未连接");
                    return;
                }
                printer.IsPeel = check_isPeel.IsChecked == true;
                var files = System.IO.Directory.GetFiles(this.tb_btwFolder.Text.Trim());
                foreach (var item in files)
                {
                    System.Threading.Thread.Sleep(50);
                    try
                    {

                            while (printer.IsHasPaper())
                            {
                                System.Threading.Thread.Sleep(200);
                            }
                            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
                            stopwatch.Start();
                            HYPrintResult ret = printer.Print(item);
                            stopwatch.Stop();
                            tb_log.Clear();
                            switch (ret)
                            {
                                case HYPrintResult.Success:
                                    tb_log.AppendText("打印成功，耗时" + stopwatch.ElapsedMilliseconds + "ms");
                                    break;
                                case HYPrintResult.Timeout:
                                    tb_log.AppendText("打印超时，耗时" + stopwatch.ElapsedMilliseconds + "ms");
                                    break;
                                case HYPrintResult.Failure:
                                    tb_log.AppendText("打印失败，耗时" + stopwatch.ElapsedMilliseconds + "ms");
                                    break;
                                default:
                                    break;
                            }
       
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
