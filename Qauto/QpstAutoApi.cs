using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtmnServer;

namespace Qauto
{
    class QpstAutoApi
    {
        private AtmnServer.AtmnServer atm;
        private string PortNum;
        private dynamic Port;
        private SerialPortInfo[] SP;
        //Contain SeralPortInfo...
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

        public QpstAutoApi(AtmnServer.AtmnServer atm, string PortNum)
        {
            this.atm = atm;
            this.PortNum = PortNum;
            this.AddPort(PortNum);
            this.GetPort();
        }

        public void AddPort(string PortNum)
        {
            if (this.atm.GetPort(PortNum) == null)
                this.atm.AddPort(PortNum, PortNum);
            else
            {
                this.RemovePort();
                this.atm.AddPort(PortNum, PortNum);
            }

        }

        public void RemovePort()
        {
            if(this.atm.GetPortList() != null)
            {
                this.atm.RemovePort(this.PortNum);
            }
        }
        public void GetPort()
        {
            this.Port = this.atm.GetPort(this.PortNum);
        }
        public dynamic GetPortList()
        {
            return this.atm.GetPortList();
        }

        public byte[] GetNvItem(int id)
        {
            var prov = this.Port.Provisioning;
            return prov.GetNVItem(id);
        }
        public void BackupQcn()
        {
            var sw = Port.SoftwareDownload;
            sw.BackupNV(@"c:\test.xqcn", 000000);
        }
        public void RestoreQcn()
        {

        }
        public void SendDiagCmd()
        {

        }
        public void PhoneReset()
        {
            if (this.Port != null)
                this.Port.Reset();

        }
        public void GetDeviceInfo()
        {


        }

    }
}
