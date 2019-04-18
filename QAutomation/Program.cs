using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtmnServer;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using Microsoft.Win32;

namespace QAutomation
{
    class Program
    {
        public const int max_com_port = 20;
//        public System.Windows.Window NotificationWindow;
        public int NotificationWindowCount = 0;
//      public DispatcherTimer ReEnumTimer = new DispatcherTimer(DispatcherPriority.Send);
        public string[] CurrentPortList = new string[max_com_port];
        public string[] OldPortList = new string[max_com_port];
//        public ILog logger;
        public List<SerialPortInfo> PortList = new List<SerialPortInfo>();
//        public NotifyIcon ni;


        public sealed class SerialPortInfo
        {
            private readonly string _name;
            public string Name
            {
                get { return _name; }
            }

            private readonly string _friendlyname;
            public string FriendlyName
            {
                get { return _friendlyname; }
            }

            private readonly string _device;
            public string Device
            {
                get { return _device; }
            }

            private readonly string _porttype;
            public string PortType
            {
                get { return _porttype; }
            }

            public SerialPortInfo(string name, string device, string friendlyname)
            {
                _name = name;
                _device = device;
                _friendlyname = friendlyname;
                _porttype = "";
            }

            public SerialPortInfo(string name, string device, string friendlyname, string porttype)
            {
                _name = name;
                _device = device;
                _friendlyname = friendlyname;
                _porttype = porttype;
            }
        }

        private string FindPortFriendlyName(string portname)
        {
            string ret_string = "";

            try
            {
                RegistryKey localmachine = Registry.LocalMachine;
                RegistryKey reg_system = localmachine.OpenSubKey("SYSTEM");
                RegistryKey reg_CurrentControlSet = reg_system.OpenSubKey("CurrentControlSet");
                RegistryKey reg_Control = reg_CurrentControlSet.OpenSubKey("Control");
                RegistryKey reg_Class = reg_Control.OpenSubKey("Class");
                RegistryKey reg_GUID = reg_Class.OpenSubKey("{4D36E96D-E325-11CE-BFC1-08002BE10318}");

                if (reg_GUID != null)
                {
                    foreach (string SubKeyName in reg_GUID.GetSubKeyNames())
                    {
                        if ("Properties" != SubKeyName.ToString())
                        {
                            RegistryKey reg_sub = reg_GUID.OpenSubKey(SubKeyName.ToString());

                            string temp = reg_sub.GetValue("AttachedTo").ToString();

                            if (portname.ToString() == temp)
                            {
                                ret_string = reg_sub.GetValue("FriendlyName").ToString();
                                return ret_string;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[ERROR] " + ex.Message);
//                if (logger.IsErrorEnabled) logger.Error("[ERROR] " + ex.Message);
            }

            return "-";
        }

        static void Main(string[] args)
        {
            #region read_com_port
            var pr = new Program();
            
            List<SerialPortInfo> _portlist = new List<SerialPortInfo>();
            
            RegistryKey localmachine = Registry.LocalMachine;
            RegistryKey hardware = localmachine.OpenSubKey("HARDWARE");
            RegistryKey devicemap = hardware.OpenSubKey("DEVICEMAP");
            RegistryKey serialcomm = devicemap.OpenSubKey("SERIALCOMM");

            pr.PortList.Clear();
            
            if(serialcomm != null)
            {
                int device_index = 0;
                string temp = "";

                for (int k = 0 ; k < max_com_port; k++)
                {
                    pr.CurrentPortList[k] = "";
                }

                foreach (string devices in serialcomm.GetValueNames())
                {
                    string portName = serialcomm.GetValue(devices).ToString().Replace("\0", "");
                    temp = portName + "_" + devices + "_" + pr.FindPortFriendlyName(portName);

                    _portlist.Add(new SerialPortInfo(portName, devices, pr.FindPortFriendlyName(portName)));
                    pr.CurrentPortList[device_index] = temp;

                    if(pr.CurrentPortList[device_index] != pr.OldPortList[device_index])
                    {

                    }
                    device_index++;
                }

            }

            #endregion


            
            var ats = new AtmnServer.AtmnServer();
            var dateNow = System.DateTime.Now.ToString("yyyyMMdd");
            string nv_backup_path = "";

            var spc = "000000";

            var list = ats.GetPortList();
            var phCnt = list.PhoneCount;
            var port_Name = "";
            if (phCnt != 0)
            {
                for (int i = 0; i < phCnt; i++)
                {
                    port_Name = list.portName(i);                    
                    ats.RemovePort(port_Name);
                }
            }

            foreach(SerialPortInfo sm8150_port in _portlist)
            {
                if (sm8150_port.Device.Contains("LGSIDIAG1") || sm8150_port.Device.Contains("LGANDNETDIAG1"))
                {
                    ats.AddPort(sm8150_port.Name, sm8150_port.Name);
                    Console.WriteLine("{0} Add!!", sm8150_port.Name);
                }
                    
            }

            list = ats.GetPortList();
            phCnt = list.PhoneCount;
            port_Name = "";
            if (phCnt != 0)
            {                
                for (int i = 0; i < phCnt; i++)
                {
                    port_Name = list.portName(i);
                    var port = ats.GetPort(port_Name);
                    var sw = port.SoftwareDownload;
                    nv_backup_path = string.Format(@"d:\{0}_{1}.xqcn", port_Name, dateNow);            
                    sw.BackupNV(nv_backup_path, spc);
                    ats.RemovePort(port_Name);
                }
            }
                    
            Console.WriteLine("done");
        }
    }
}
