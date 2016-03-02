using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Demo_RealTime.Common
{
    public class ClientInfo
    {
        /// <summary>
        /// 获取IP
        /// </summary>
        /// <returns></returns>
        public static string GetIP()
        {
            string HostName = Dns.GetHostName(); //get Hostname
            IPHostEntry IpEntry = Dns.GetHostEntry(HostName); //get IP
            string strIPAddr = IpEntry.AddressList[0].ToString();
            return (strIPAddr);
        }
        /// <summary>
        /// 获取Mac地址
        /// </summary>
        /// <returns></returns>
        public static string GetMac()
        {
            string MacAddress = String.Empty;
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if ((bool)mo["IPEnabled"] == true)
                {
                    MacAddress = mo["MacAddress"].ToString();
                    break;
                }
            }
            moc = null;
            mc = null;
            MacAddress = MacAddress.Replace(":", "");
            return MacAddress;
        }
    }
}