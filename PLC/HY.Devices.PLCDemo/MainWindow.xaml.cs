using HY.Devices.PLC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HY.Devices.PLCDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            InitialData();
            this.Closing += MainWindow_Closing;

        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _plc?.Close();
        }

        //(IPLCComm)Activator.CreateInstance(t)

        private IEnumerable<Type> _plcTypes;
        private void InitialData()
        {
            _plcTypes = System.Reflection.Assembly.GetAssembly(typeof(IHYPLCComm)).GetTypes().Where(t => typeof(IHYPLCComm).IsAssignableFrom(t))
                   .Where(t => !t.IsAbstract && t.IsClass);
            cb_plcTypes.ItemsSource = _plcTypes.Select(t => t.Name);
        }
        private IHYPLCComm _plc;
        #region 连接

        private void conn_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (_plc == null)
                {
                    MessageBox.Show("未选择PLC类型");
                    return;
                }
                if (string.IsNullOrEmpty(tb_ip.Text.Trim()) || string.IsNullOrEmpty(tb_port.Text.Trim()))
                {
                    MessageBox.Show("IP或者端口不能为空");
                    return;
                }
                if (_plc is AbstractHYPLCComm_Net pLCComm_Net)
                {
                    pLCComm_Net.IP = tb_ip.Text.Trim();
                    pLCComm_Net.Port = int.Parse(tb_port.Text.Trim());
                }
                Stopwatch _stopwatch = new Stopwatch();
                _stopwatch.Start();
                _plc.Open();
                elp_status.Fill = _plc.IsConnected ? Brushes.Green : Brushes.Red;
                time.Content = _stopwatch.Elapsed.TotalMilliseconds + "ms";
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void disConn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Stopwatch _stopwatch = new Stopwatch();
                _stopwatch.Start();
                _plc?.Close();
                elp_status.Fill = _plc?.IsConnected == true ? Brushes.Green : Brushes.Gray;
                time.Content = _stopwatch.Elapsed.TotalMilliseconds + "ms";
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
        #region 读数据
        private void cb_plcTypes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (cb_plcTypes.SelectedIndex > -1)
                {
                    if (_plc != null)
                    {
                        _plc.Close();
                        while (_plc.IsConnected == true)
                        {
                            System.Threading.Thread.Sleep(10);
                        }
                        elp_status.Fill = _plc.IsConnected ? Brushes.Green : Brushes.Gray;
                    }
                    if (_plcTypes.ToList()[cb_plcTypes.SelectedIndex].Name == "HYPLCComm_SiemensNet")
                    {

                        _plc = new HYPLCComm_SiemensNet(HYPLCComm_SiemensNet.SiemensPLCTypes.S1200);
                    }
                    else
                    {
                        _plc = (IHYPLCComm)Activator.CreateInstance(_plcTypes.ElementAt(cb_plcTypes.SelectedIndex));
                    }
                }

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private void btn_readData_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button)
            {
                if (_plc?.IsConnected != true)
                {
                    MessageBox.Show("PLC未连接");
                    return;
                }
                ushort num = 1;
                bool b = ushort.TryParse(tb_num.Text.Trim(), out num);
                ReadData(tb_address.Text.Trim(), num);
            }
        }
        Task t = null;
        public bool Flag { get; set; }

        private void btn_pollingReadData_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Primitives.ToggleButton tgbtn)
            {
                if (Flag && (t == null || t.IsCompleted))
                {
                    if (_plc?.IsConnected != true)
                    {
                        MessageBox.Show("PLC未连接");
                        tgbtn.IsChecked = false;
                        return;
                    }
                    if (int.TryParse(tb_elapsed.Text, out int d))
                    {
                        ushort num = 1;
                        ushort.TryParse(tb_num.Text.Trim(), out num);
                        string address = tb_address.Text.Trim();
                        t = Task.Run(() =>
                        {
                            while (Flag)
                            {
                                ReadData(address, num);
                                Task.Delay(d).Wait();
                            }
                        });
                    }
                    else
                    {
                        MessageBox.Show("时间间隔有误，请填写数字格式！");
                        tgbtn.IsChecked = false;
                    }
                }
            }
        }

        public string DataType { get; set; }
        private void ReadData(string address, ushort num)
        {
            try
            {
                Stopwatch _stopwatch = new Stopwatch();
                _stopwatch.Start();
                if (num > 1)
                {
                    switch (DataType)
                    {
                        case "Bool": showText(_plc?.Read<bool>(address, num)); break;
                        case "Int16": showText(_plc?.Read<short>(address, num)); break;
                        case "Int32": showText(_plc?.Read<int>(address, num)); break;
                        case "Int64": showText(_plc?.Read<long>(address, num)); break;
                        case "UInt16": showText(_plc?.Read<ushort>(address, num)); break;
                        case "UInt32": showText(_plc?.Read<uint>(address, num)); break;
                        case "UInt64": showText(_plc?.Read<ulong>(address, num)); break;
                        case "Byte": showText(_plc?.Read<byte>(address, num)); break;
                        case "Char": showText(_plc?.Read<char>(address, num)); break;
                        case "String": showText(_plc?.Read<string>(address, num)); break;
                        case "Float": showText(_plc?.Read<float>(address, num)); break;
                        case "Double": showText(_plc?.Read<double>(address, num)); break;
                        default: break;
                    }
                }
                else
                {
                    switch (DataType)
                    {
                        case "Bool": showText(_plc?.Read<bool>(address).ToString()); break;
                        case "Int16": showText(_plc?.Read<short>(address).ToString()); break;
                        case "Int32": showText(_plc?.Read<int>(address).ToString()); break;
                        case "Int64": showText(_plc?.Read<long>(address).ToString()); break;
                        case "UInt16": showText(_plc?.Read<ushort>(address).ToString()); break;
                        case "UInt32": showText(_plc?.Read<uint>(address).ToString()); break;
                        case "UInt64": showText(_plc?.Read<ulong>(address).ToString()); break;
                        case "Byte": showText(_plc?.Read<byte>(address).ToString()); break;
                        case "Char": showText(_plc?.Read<char>(address).ToString()); break;
                        case "Float": showText(_plc?.Read<float>(address).ToString()); break;
                        case "Double": showText(_plc?.Read<double>(address).ToString()); break;
                        default: break;
                    }
                }
                showTotalMs(_stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        private void showText(Array data)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var item in data)
            {
                sb.Append(item.ToString()).Append(",");
            }
            showText(sb.ToString());
        }
        private void showText(string data)
        {
            this.Dispatcher.BeginInvoke(new Action(() => { tb_ret.Text = data; }));
        }

        public bool MonitorFlag { get; set; }
        private void tbtn_monitor_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_plc?.IsConnected != true)
                {
                    MessageBox.Show("PLC未连接");
                    tbtn_monitor.IsChecked = false;
                    return;
                }
                int i = 0;
                if (MonitorFlag)
                {
                    if (cb_monitorType.SelectedIndex == 0)
                    {
                        _plc.WaitRisingEdge(tb_monitorAddress.Text.Trim(), 200, new Action<string>(str =>
                        {
                            this.Dispatcher.Invoke((ThreadStart)delegate
                            {
                                elp_signal.Fill = Brushes.Red;

                            });
                            Thread.Sleep(500);
                            this.Dispatcher.Invoke((ThreadStart)delegate
                            {
                                elp_signal.Fill = Brushes.Gray;
                            });
                        }
                        ), true);
                    }
                    else
                    {
                        _plc.WaitFallingEdge(tb_monitorAddress.Text.Trim(), 1000, new Action<string>(str =>
                        {
                            this.Dispatcher.Invoke((ThreadStart)delegate
                            {
                                elp_signal.Fill = Brushes.Red;
                            });
                            Thread.Sleep(500);
                            this.Dispatcher.Invoke((ThreadStart)delegate
                            {
                                elp_signal.Fill = Brushes.Gray;
                            });
                        }));
                    }
                }
                else
                {
                    _plc.StopWaitAddress(tb_monitorAddress.Text.Trim());
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
        #region 写数据
        public string WriteDataType { get; set; }
        private void btn_write_Click(object sender, RoutedEventArgs e)
        {
            WriteData(tb_writeAddress.Text.Trim());
        }
        private void WriteData(string address)
        {
            try
            {
                Stopwatch _stopwatch = new Stopwatch();
                _stopwatch.Start();
                bool ret = false;
                switch (WriteDataType)
                {
                    case "Bool": ret = _plc?.Write(address, bool.Parse(tb_writeValue.Text.Trim())) == true; break;
                    case "Int16": ret = _plc?.Write(address, short.Parse(tb_writeValue.Text.Trim())) == true; break;
                    case "Int32": ret = _plc?.Write(address, int.Parse(tb_writeValue.Text.Trim())) == true; break;
                    case "Int64": ret = _plc?.Write(address, long.Parse(tb_writeValue.Text.Trim())) == true; break;
                    case "UInt16": ret = _plc?.Write(address, ushort.Parse(tb_writeValue.Text.Trim())) == true; break;
                    case "UInt32": ret = _plc?.Write(address, uint.Parse(tb_writeValue.Text.Trim())) == true; break;
                    case "UInt64": ret = _plc?.Write(address, ulong.Parse(tb_writeValue.Text.Trim())) == true; break;
                    case "Byte": ret = _plc?.Write(address, byte.Parse(tb_writeValue.Text.Trim())) == true; break;
                    case "Char": ret = _plc?.Write(address, char.Parse(tb_writeValue.Text.Trim())) == true; break;
                    case "String": ret = _plc?.Write(address, tb_writeValue.Text.Trim()) == true; break;
                    case "Float": ret=_plc?.Write(address, float.Parse(tb_writeValue.Text.Trim()))==true; break;
                    case "Double": ret=_plc?.Write(address, double.Parse(tb_writeValue.Text.Trim()))==true; break;
                    default:
                        break;
                }
                elp_writeStatus.Fill = ret ? Brushes.Green : Brushes.Red;
                showTotalMs(_stopwatch.Elapsed);
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }
        #endregion
        private void showTotalMs(TimeSpan timeSpan)
        {
            this.Dispatcher.BeginInvoke(new Action(() => { time.Content = timeSpan.TotalMilliseconds + "ms"; }));
        }
    }
}
