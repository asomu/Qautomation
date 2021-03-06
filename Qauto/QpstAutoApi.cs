﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AtmnServer;

namespace Qauto
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
            if (this.atm.GetPort(PortNum) != null)
            {
                this.DeviceName = Port.DeviceName();
                this.Mode = Port.PhoneMode();
                this.BuildId = this.port_mode[Port.BuildId()];
            }
        }

    }
}
