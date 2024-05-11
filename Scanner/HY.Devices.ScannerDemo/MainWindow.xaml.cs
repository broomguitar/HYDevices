using HY.Devices.Scanner;
using System;
using System.Windows;
using System.Windows.Media;

namespace HY.Devices.ScannerDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            cb_com.ItemsSource = System.IO.Ports.SerialPort.GetPortNames();
        }
        private IHYScanner scanner;
        private void btn_Connect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string p1 = null, p2 = null;
                switch (cb_type.SelectedIndex)
                {
                    case 0: p1 = cb_com.Text.Trim(); p2 = tb_rate.Text.Trim(); scanner = new HYScannerSerialPort(cb_com.Text.Trim(), int.Parse(tb_rate.Text.Trim())); break;
                    case 1: p1 = tb_ip.Text.Trim(); p2 = tb_port.Text.Trim(); scanner = new HYScannerNET(p1, int.Parse(p2)); break;
                }
                if (scanner != null)
                {
                    scanner.HasNewDataReceived += Scanner_HasNewDataReceived;
                    scanner.Error += Scanner_Error;
                    scanner.Open();
                    SetStatu(scanner.IsConnected);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void SetStatu(bool b)
        {
            switch (cb_type.SelectedIndex)
            {
                case 0: elp_serialPortStatus.Fill = b ? Brushes.Green : Brushes.Red; break;
                case 1: elp_netStatus.Fill = b ? Brushes.Green : Brushes.Red; break;
            }
        }
        private void Scanner_Error(object sender, string e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                tb_error.AppendText(DateTime.Now + "=====>>>>" + e + Environment.NewLine);
            }));

        }

        private void Scanner_HasNewDataReceived(object sender, string e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                tb_newData.AppendText(DateTime.Now + "=====>>>>" + e + Environment.NewLine);
            }));
        }

        private void btn_Close_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                scanner?.Close();
                SetStatu(scanner?.IsConnected == true);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }

        }

        private void Btn_newDataCls_Click(object sender, RoutedEventArgs e)
        {
            tb_newData.Clear();

        }

        private void Btn_errorCls_Click(object sender, RoutedEventArgs e)
        {
            tb_error.Clear();
        }

        private void statu_Click(object sender, RoutedEventArgs e)
        {
            if (!scanner.IsConnected)
            {
                scanner.Open();
            }
            SetStatu(scanner?.IsConnected == true);
        }
    }
}
