using System;
using System.Windows;
using System.Windows.Media;

namespace HY.Devices.RobotDemo
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Closed += MainWindow_Closed;
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            try
            {
                server.DisConnect();
            }
            catch
            {
            }
        }
        Robot.RobotServer server = new Robot.RobotServer();
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                server.IP_Server = System.Net.IPAddress.Parse(tb_ip.Text.Trim());
                server.Port_Server = int.Parse(tb_port.Text.Trim());
                bool b = server.Connect();
                if (b)
                {
                    server.PushResultEvent -= RobotServer_HasNewMsgEvent;
                    server.PushResultEvent += RobotServer_HasNewMsgEvent;
                    MessageBox.Show("启动成功");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                tgbtn.Background = Brushes.Red;
            }
        }

        private void RobotServer_HasNewMsgEvent(object sender, string e)
        {
            try
            {
                this.Dispatcher.Invoke(new Action(() =>
                {
                    tb_recData.AppendText(DateTime.Now + "=====>>>>" + e + Environment.NewLine);
                }));
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.ToString());
            }
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                server.DisConnect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void Btn_newDataCls_Click(object sender, RoutedEventArgs e)
        {
            tb_recData.Clear();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (server.WriteData(tb_msg.Text.Trim()))
                {
                    tb_sendData.AppendText(DateTime.Now + "=====>>>>" + tb_msg.Text.Trim() + Environment.NewLine);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
        }
    }
}
