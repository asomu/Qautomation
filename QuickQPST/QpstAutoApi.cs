using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtmnServer;
using System.Threading;

namespace QuickQPST
{
    class QpstAutoApi
    {
        private readonly string[] port_mode = { "no_phone", "", "download", "online", "offline", "", "stream_download", "", "", "", "", "", "", "sahara" };
        private AtmnServer.AtmnServer atm;
        private string _PortNum;
        public string PortNum
        {
            get { return _PortNum; }
            set { _PortNum = value; }
        }

        private dynamic _Port;
        private dynamic Port
        {
            get { return _Port; }
            set { _Port = value; }
        }

        private string _deviceName;
        public string DeviceName
        {
            get { return _deviceName; }
            set { _deviceName = value; }
        }
        private string _BuildId;
        public string BuildId
        {
            get { return _BuildId; }
            set { _BuildId = value; }
        }
        private string _Mode;
        public string Mode
        {
            get { return _Mode; }
            set { _Mode = value; }
        }

        public QpstAutoApi(AtmnServer.AtmnServer atm, string PortNum)
        {
            this.atm = atm;
            this.PortNum = PortNum;
            this.AddPort(PortNum);
            this.GetPort();

            this.GetDeviceInfo();
        }

        public void AddPort(string PortNum)
        {
            if (this.atm.GetPort(PortNum) == null)
            {
                this.RemovePort();
                this.atm.AddPort(PortNum, PortNum);
                Thread.Sleep(1500);
            }
            else
            {
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
            try
            {
                var prov = this.Port.Provisioning;
                return prov.GetNVItem(id);
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            
        }
        public void BackupQcn()
        {
            try
            {
                var sw = Port.SoftwareDownload;
                
                var FilneName = string.Format(@"C:\Users\User\Desktop\{0}_{1}.xqcn", DeviceName, DateTime.Now.Second);
                sw.BackupNV(FilneName, "000000");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
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
            if (this.Port != null)
            {
                this.DeviceName = this.Port.DeviceName();
                this.Mode = this.port_mode[Port.PhoneMode()];
                this.BuildId = this.Port.BuildId();
            }
        }

    }
}
