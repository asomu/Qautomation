using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtmnServer;
using Microsoft.Win32;

namespace QuickQPST
{
    class Program
    {
        private string _ConnectedPortName = "";
        public string ConnectedPortName
        {
            get { return _ConnectedPortName; }
            set { _ConnectedPortName = value; }
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

        public void getPortInfo()
        {
            if (_portList != null)
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
            else
                ConnectedPortName = null;

            foreach (SerialPortInfo sm8150_port in _portList)
            {
                if (sm8150_port.Device.Contains("LGSIDIAG1") || sm8150_port.Device.Contains("LGANDNETDIAG1"))
                {
                    ConnectedPortName = sm8150_port.Name;
                }
            }
        }

        public void runAuto(string[] args, string portName)
        {
            var ats = new AtmnServer.AtmnServer();
            QpstAutoApi Qapi = new QpstAutoApi(ats, portName);
            byte[] ret = null;
            if (args.Length >= 1)
            {
                if (args[0] == "backup")
                    Qapi.BackupQcn();
                else if (args[0] == "info")
                {
                    Console.WriteLine("Device : {0}", Qapi.DeviceName);
                    Console.WriteLine("Mode   : {0}", Qapi.Mode);
                    Console.WriteLine("Build  : {0}", Qapi.BuildId);
                }
                else if (args[0] == "nv")
                {
                    if (args[1] != null)
                    {
                        ret = Qapi.GetNvItem(Convert.ToInt32(args[1]));
                        if (ret == null)
                            Console.WriteLine("Noting or Can't read!");
                        else
                        {
                            Console.Write("Result : ");
                            foreach (int i in ret)
                                Console.Write(" {0}", i);
                            Console.WriteLine("");
                            if (args[1] == "569" && ret[7] == 99)
                                Console.WriteLine("Calibration OK!!!!");
                            else if (args[1] == "569")
                                Console.WriteLine("You need to calibrate DUT!!!");
                        }

                    }
                }
                else if (args[0] == "delete")
                {
                    if (args[1] != null)
                    {
                        Qapi.efs_unlink(args[1]);
                    }
                }
            }
            Console.WriteLine("Done....");
        }

        static void Main(string[] args)
        {
            var pr = new Program();
            pr.getPortInfo();
            pr.runAuto(args, pr.ConnectedPortName);
        }
    }
}
