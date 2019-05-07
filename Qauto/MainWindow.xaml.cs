using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using AtmnServer;

namespace Qauto
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public sealed class SerialPortInfo
        {
            private readonly string _name;
            public string Name
            {
                get { return _name; }
            }

            private readonly string _device;
            public string Device
            {
                get { return _device; }
            } 

            private readonly string _porttype;
            public string Porttype
            {
                get { return _porttype; }
            }

            public SerialPortInfo(string name, string device)
            {
                _name = name;
                _device = device;
                _porttype = "";
            }

            public SerialPortInfo(string name, string device, string porttype)
            {
                _name = name;
                _device = device;
                _porttype = porttype;
            }
        }

        List<SerialPortInfo> _portList = new List<SerialPortInfo>();

        readonly int max_com_port = 20;
        string ConnectedPortName = "";


        private void getPortInfo()
        {
            _portList.Clear();
            RegistryKey localmachine = Registry.LocalMachine;
            RegistryKey hardware = localmachine.OpenSubKey("HARDWARE");
            RegistryKey devicemap = hardware.OpenSubKey("DEVICEMAP");
            RegistryKey serialcomm = devicemap.OpenSubKey("SERIALCOMM");

            if (serialcomm != null)
            {


                foreach (string devices in serialcomm.GetValueNames())
                {
                    string portName = serialcomm.GetValue(devices).ToString().Replace("\0", "");                    
                    _portList.Add(new SerialPortInfo(portName, devices));                                      
                }
            }
            foreach (SerialPortInfo sm8150_port in _portList)
            {
                if (sm8150_port.Device.Contains("LGSIDIAG1") || sm8150_port.Device.Contains("LGANDNETDIAG1"))
                {
                    ConnectedPortName = sm8150_port.Name;
                }
            }
        }

        readonly string[] port_mode = {"no_phone", "", "download", "online", "offline", "", "stream_download", "sahara" };


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            getPortInfo();
 
            if (ConnectedPortName != "")
            {                
                var ats = new AtmnServer.AtmnServer();
                var port_list = ats.GetPortList();
                var port = ats.GetPort(ConnectedPortName);
                Ldevice.Content = port.DeviceName();
                Lname.Content = port.PortName();
                Lmode.Content = port_mode[port.PhoneMode()];
                LbuildID.Content = port.BuildId();
            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (ConnectedPortName != "")
            {
                var spc = "000000";
                var at = new AtmnServer.AtmnServer();
                var port = at.GetPort(ConnectedPortName);
                var sw = port.SoftwareDownload;
                if(TBackupQCN.Text != "")
                    sw.BackupNV(TBackupQCN.Text, spc);
            }
            
        }
    }
}
