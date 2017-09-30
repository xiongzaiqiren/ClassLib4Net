using System;
using System.Management;

namespace ClassLib4Net
{
    /// <summary>
    ///Computer 的摘要说明
    /// </summary>
    public class Computer
    {
        /// <summary>
        /// CPU序列号代码
        /// </summary>
        public string CpuID;
        /// <summary>
        /// 网卡硬件地址（MacAddress）
        /// </summary>
        public string MacAddress;
        /// <summary>
        /// IP地址
        /// </summary>
        public string IpAddress;
        /// <summary>
        /// 硬盘ID
        /// </summary>
        public string DiskID;
        /// <summary>
        /// 操作系统的登录用户名
        /// </summary>
        public string LoginUserName;
        /// <summary>
        /// 电脑系统类型
        /// </summary>
        public string SystemType;
        /// <summary>
        /// 计算机名
        /// </summary>
        public string ComputerName;
        /// <summary>
        /// 物理内存总量（单位：Byte）
        /// </summary>
        public long TotalPhysicalMemory; //单位：Byte 

        private static Computer _instance;
        /// <summary>
        /// 单例实例
        /// </summary>
        /// <returns></returns>
        public static Computer Instance()
        {
            if (_instance == null)
                _instance = new Computer();
            return _instance;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        public Computer()
        {
            CpuID = GetCpuID();
            MacAddress = GetMacAddress();
            DiskID = GetDiskID();
            IpAddress = GetIPAddress();
            LoginUserName = GetUserName();
            SystemType = GetSystemType();
            TotalPhysicalMemory = GetTotalPhysicalMemory();
            ComputerName = GetComputerName();
        }

        /// <summary>
        /// 获取CPU序列号代码
        /// </summary>
        /// <returns></returns>
        public string GetCpuID()
        {
            try
            {
                //获取CPU序列号代码 
                string cpuInfo = "";//cpu序列号 
                ManagementClass mc = new ManagementClass("Win32_Processor");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                }
                moc = null;
                mc = null;
                return cpuInfo;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        /// <summary>
        /// 获取网卡硬件地址
        /// </summary>
        /// <returns></returns>
        public string GetMacAddress()
        {
            try
            {
                //获取网卡硬件地址 
                string mac = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        mac = mo["MacAddress"].ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return mac;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        /// <summary>
        /// 获取IP地址
        /// </summary>
        /// <returns></returns>
        public string GetIPAddress()
        {
            try
            {
                //获取IP地址 
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"] == true)
                    {
                        //st=mo["IpAddress"].ToString(); 
                        System.Array ar;
                        ar = (System.Array)(mo.Properties["IpAddress"].Value);
                        st = ar.GetValue(0).ToString();
                        break;
                    }
                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        /// <summary>
        /// 获取硬盘ID
        /// </summary>
        /// <returns></returns>
        public string GetDiskID()
        {
            try
            {
                //获取硬盘ID 
                String HDid = "";
                ManagementClass mc = new ManagementClass("Win32_DiskDrive");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    HDid = (string)mo.Properties["Model"].Value;
                }
                moc = null;
                mc = null;
                return HDid;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        /// <summary> 
        /// 操作系统的登录用户名 
        /// </summary> 
        /// <returns></returns> 
        public string GetUserName()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["UserName"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        /// <summary> 
        /// 电脑系统类型 
        /// </summary> 
        /// <returns></returns> 
        public string GetSystemType()
        {
            try
            {
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {

                    st = mo["SystemType"].ToString();

                }
                moc = null;
                mc = null;
                return st;
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }

        }
        /// <summary> 
        /// ComputerName
        /// </summary> 
        /// <returns></returns> 
        public string GetComputerName()
        {
            try
            {
                return System.Environment.GetEnvironmentVariable("ComputerName");
            }
            catch
            {
                return "unknow";
            }
            finally
            {
            }
        }

        /// <summary> 
        /// 物理内存总量（单位：Byte） 
        /// </summary> 
        /// <returns></returns> 
        public long GetTotalPhysicalMemory()
        {
            try
            {
                //物理内存
                string st = "";
                ManagementClass mc = new ManagementClass("Win32_ComputerSystem");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    st = mo["TotalPhysicalMemory"].ToString(); //单位：Byte
                }
                moc = null;
                mc = null;
                return string.IsNullOrEmpty(st) ? 0 : Convert.ToInt64(st);
            }
            catch
            {
                return 0;
            }
            finally
            {
            }
        }

    }
}
