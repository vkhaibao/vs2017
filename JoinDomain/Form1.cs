using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.DirectoryServices;
using System.Management;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.IO;
using System.Threading;

namespace JoinDomain
{
    public partial class Form1 : Form
    {
        bool joinSuccess = false;
        string userName = "";
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string newcpname = "KEDA-" + GetBIOSNumber();
            textBox4.Text = newcpname;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string domian = "Administrator";
            //string domain = "kedacom.com";

            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            Debug.Assert(currentIdentity != null, "currentIdentity != null");
            WindowsPrincipal currentPrincipal = new WindowsPrincipal(currentIdentity);

            //string domain = Environment.UserDomainName.ToString();
            userName = Environment.MachineName.ToString();
            //EnumComputers();
            isInDomain("WORKGROUP","Schema");

            GetDomainName();

            //userName = "LiuJun@ecjtucs.com";
            //if (currentPrincipal.IsInRole(domain))
            //{

                //MessageBox.Show(Environment.UserDomainName.ToString());
               // MessageBox.Show("该用户" + userName + "在域" + domain + "中");
            //}
            //else

                //MessageBox.Show("该用户" + currentPrincipal.Identity.Name + "不在域" + domain + "中");
        }
        /// <summary>
        /// 判断客户机是否在域中，注意过滤Schema
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="hostName"></param>
        public void isInDomain(string domainName, string hostName)
        {
            bool isInTheDomain = false;
            using (DirectoryEntry root = new DirectoryEntry("WinNT:"))
            {
                foreach (DirectoryEntry domain in root.Children)
                {
                    if (domain.Name.Equals(domainName))
                    {
                        foreach (DirectoryEntry computer in domain.Children)
                        {
                            if (computer.Name.Equals(hostName))
                            {
                                isInTheDomain = true;
                                MessageBox.Show("计算在域中已经存在");
                            }
                        }
                        if (isInTheDomain == false)
                            MessageBox.Show("计算机不在域中");

                    }

                }
            }
        }
        /// <summary>
        /// 获取局域网中的域的名称和客户机的名称
        /// </summary>
        private void EnumComputers()
        {
            TreeNode treeNode = new TreeNode("域信息");
            this.treeView1.Nodes.Add(treeNode);
            using (DirectoryEntry root = new DirectoryEntry("WinNT:"))
            {
                foreach (DirectoryEntry domain in root.Children)
                {
                    TreeNode node = new TreeNode(domain.Name);
                    treeNode.Nodes.Add(node);
                    foreach (DirectoryEntry computer in domain.Children)
                    {
                        TreeNode node_ = new TreeNode(computer.Name);
                        node.Nodes.Add(node_);
                    }
                }
            }
        }
        //获取域名
        private static string GetDomainName()
        {
            // 注意：这段代码需要在Windows XP及较新版本的操作系统中才能正常运行。  
          
            SelectQuery query = new SelectQuery("Win32_ComputerSystem");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                foreach (ManagementObject mo in searcher.Get())
                {
                    if ((bool)mo["partofdomain"])
                        return mo["domain"].ToString();
                }
            }
            return null;
        }
        //加域
        public string SetDomainMembership(string DomainName, string UserName, string Password, out string err)
        {
            err = "successful";
            // Invoke WMI to join the domain
            using (ManagementObject wmiObject = new ManagementObject(new ManagementPath("Win32_ComputerSystem.Name='" + System.Environment.MachineName + "'")))
            {
                try
                {
                    // Obtain in-parameters for the method
                    ManagementBaseObject inParams = wmiObject.GetMethodParameters("JoinDomainOrWorkgroup");

                    inParams["Name"] = DomainName;
                    inParams["Password"] = Password;
                    inParams["UserName"] = DomainName + "\\" + UserName;
                    //inParams["AccountOU"] = null;
                    inParams["FJoinOptions"] = 3;
                    //Execute the method and obtain the return values.
                    ManagementBaseObject outParams = wmiObject.InvokeMethod("JoinDomainOrWorkgroup", inParams, null);
                    switch (outParams["ReturnValue"].ToString())
                    {
                        case "5":
                            err = "拒绝访问";
                            break;
                        case "87":
                            err = "参数不正确";
                            break;
                        case "110":
                            err = "系统无法打开指定对象";
                            break;
                        case "1323":
                            err = "无法更新密码";
                            break;
                        case "1326":
                            err = "登录失败：未知用户名或密码错误";
                            break;
                        case "1355":
                            err = "指定的域不存在或无法联系，请检查网络以及DNS设置";
                            break;
                        case "2224":
                            err = "帐户已经存在";
                            break;
                        case "2691":
                            err = "该电脑已经加入到域中";
                            break;
                        case "2692":
                            err = "该电脑当前没有连接到域";
                            break;
                        case "0":
                            err = "加域成功，请重启电脑使用OA账户进行登陆";
                            break;
                    }
                    if (err.Equals("successful"))
                    {
                        joinSuccess = true;
                        //MessageBox.Show(err);
                        //MessageBox.Show(logtxt.Text.Count().ToString());
                        logtxt.AppendText(err+"\n");
                    }
                    else
                    {
                        joinSuccess = false;
                        //MessageBox.Show(err);                    
                        logtxt.AppendText(err+"\n");
                    }
                    return err;
                }
                catch (ManagementException e)
                {

                    throw new Exception(e.Message);
                }
            }
        }
        ///退域
        ///
        public string SetDomainMembership3(string DomainName, string UserName, string Password, out string err)
        {
            err = "successful";
            // Invoke WMI to join the domain
            using (ManagementObject wmiObject = new ManagementObject(new ManagementPath("Win32_ComputerSystem.Name='" + System.Environment.MachineName + "'")))
            {
                try
                {
                    // Obtain in-parameters for the method
                    ManagementBaseObject inParams = wmiObject.GetMethodParameters("UnJoinDomainOrWorkgroup");

                    //inParams["Name"] = DomainName;
                    //inParams["Password"] = Password;
                    //inParams["UserName"] = DomainName + "\\" + UserName;
                    //inParams["AccountOU"] = null;
                    //inParams["FJoinOptions"] = 3;
                    //Execute the method and obtain the return values.
                    ManagementBaseObject outParams = wmiObject.InvokeMethod("UnJoinDomainOrWorkgroup", inParams, null);
                    switch (outParams["ReturnValue"].ToString())
                    {
                        case "5":
                            err = "拒绝访问";
                            break;
                        case "87":
                            err = "参数不正确";
                            break;
                        case "110":
                            err = "系统无法打开指定对象";
                            break;
                        case "1323":
                            err = "无法更新密码";
                            break;
                        case "1326":
                            err = "登录失败：未知用户名或密码错误";
                            break;
                        case "1355":
                            err = "指定的域不存在或无法联系，请检查网络以及DNS设置";
                            break;
                        case "2224":
                            err = "帐户已经存在";
                            break;
                        case "2691":
                            err = "该电脑已经加入到域中";
                            break;
                        case "2692":
                            err = "该电脑当前没有连接到域";
                            break;
                        case "0":
                            err = "退域成功，请重启电脑使用本地账户登陆使用";
                            break;
                    }
                    if (err.Equals("successful"))
                    {
                        joinSuccess = true;
                        //MessageBox.Show(err);
                        logtxt.AppendText(err+"\n");
                    }

                    else
                    { 
                        joinSuccess = false;
                        //MessageBox.Show(err);
                        logtxt.AppendText(err+"\n");
                    }
                    return err;
                }
                catch (ManagementException ef)
                {
                    throw new Exception(ef.Message);
                }
            }
        }
        /// <summary>  
        /// 设置客户机的DNS  
        /// </summary>  
        /// <param name="domainDNS"></param>  
        public static void SetDNS(string[] domainDNS)
        {

            //获取本机的IP地址和网关  
            ManagementObjectCollection moc = GetLocalIPAndGateway();
            ManagementBaseObject inPar = null;
            ManagementBaseObject outPar = null;

            foreach (ManagementObject mo in moc)
            {
                //如果没有启用IP设置的网络设备则跳过  
                if (!(bool)mo["IPEnabled"])
                {
                    continue;
                }
                if (domainDNS != null)
                {
                    //设置DNS    
                    inPar = mo.GetMethodParameters("SetDNSServerSearchOrder");
                    // 1.DNS 2.备用DNS   
                    inPar["DNSServerSearchOrder"] = domainDNS;

                    outPar = mo.InvokeMethod("SetDNSServerSearchOrder", inPar, null);
                }
            }
        }

        public static ManagementObjectCollection GetLocalIPAndGateway()
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            return moc;
        }
        public static int SetDomainMembership2(string DomainName, string UserName, string Password, out string err)
        {
            err = string.Empty;
            try
            {
                string DomainNameHost = DomainName;
                uint value1 = NetJoinDomain(null, DomainNameHost, null, UserName + "@" + DomainName, Password, (JoinOptions.NETSETUP_JOIN_DOMAIN | JoinOptions.NETSETUP_DOMAIN_JOIN_IF_JOINED | JoinOptions.NETSETUP_ACCT_CREATE));
                err = value1.ToString();
                return Convert.ToInt32(value1);
            }
            catch (Exception e)
            {
                err = e.ToString();
                return -1;
            }
        }

        [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
        static extern uint NetJoinDomain(
          string lpServer,
          string lpDomain,
          string lpAccountOU,
          string lpAccount,
          string lpPassword,
          JoinOptions NameType);

        [Flags]
        enum JoinOptions
        {
            NETSETUP_JOIN_DOMAIN = 0x00000001,
            NETSETUP_ACCT_CREATE = 0x00000002,
            NETSETUP_ACCT_DELETE = 0x00000004,
            NETSETUP_WIN9X_UPGRADE = 0x00000010,
            NETSETUP_DOMAIN_JOIN_IF_JOINED = 0x00000020,
            NETSETUP_JOIN_UNSECURE = 0x00000040,
            NETSETUP_MACHINE_PWD_PASSED = 0x00000080,
            NETSETUP_DEFER_SPN_SET = 0x10000000
        }
        /// <summary>  
        /// 启用DHCP服务器  
        /// </summary>  
        public static void EnableDHCP()
        {
            ManagementClass wmi = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = wmi.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                //如果没有启用IP设置的网络设备则跳过  
                if (!(bool)mo["IPEnabled"])
                    continue;
                //重置DNS为空  
                mo.InvokeMethod("SetDNSServerSearchOrder", null);
                //开启DHCP  
                mo.InvokeMethod("EnableDHCP", null);
            }
        }
        public void joinad()
        {
            userName = textBox1.Text.ToString();
            string domainName = textBox3.Text.ToString();
            string pwd = textBox2.Text.ToString();
            string dns = "10.0.0.2";
            string dns1 = "10.1.1.1";
            string message;
            //string newcpname = "KEDA-" + GetBIOSNumber();
            //textBox4.Text = newcpname;                    
            //修改DNS  
            SetDNS(new string[] { dns, dns1 });
            //EnableDHCP();  
            //int message1 = SetDomainMembership2(domainName, userName, pwd, out message);
            SetDomainMembership(domainName, userName, pwd, out message);
            //SetMachineName(newcpname, "123456", domainName+"\\"+userName);
            //SetDomainMembership2(domainName, "Lele", pwd, out message);  
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Thread joinadt = new Thread(joinad);
            joinadt.Start();
        }

        public void exitad()
        {
            userName = textBox1.Text.ToString();
            string domainName = textBox3.Text.ToString();
            string pwd = textBox2.Text.ToString();
            string dns = "10.0.0.2";
            string dns1 = "10.1.1.1";
            string message;
            //修改DNS  
            SetDNS(new string[] { dns, dns1 });
            //EnableDHCP();  
            //int message1 = SetDomainMembership2(domainName, userName, pwd, out message);
            SetDomainMembership3(domainName, userName, pwd, out message);
            //SetDomainMembership2(domainName, "Lele", pwd, out message); 
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Thread exitadt = new Thread(exitad);
            exitadt.Start();
        }
        private string GetBIOSNumber()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("Select SerialNumber From Win32_BIOS");
            string biosNumber = string.Empty;
            foreach (ManagementObject mgt in searcher.Get())
            {
                biosNumber += mgt["SerialNumber"].ToString();
            }
            return biosNumber;
        }
        public  bool SetMachineName(string newName,string Passwd, string Uname)
        {
            //Invoke WMI to populate the machine name
            using (ManagementObject wmiObject = new ManagementObject(new ManagementPath("Win32_ComputerSystem.Name='" + System.Environment.MachineName + "'")))
            {
                ManagementBaseObject inputArgs = wmiObject.GetMethodParameters("Rename");
                inputArgs["Name"] = newName;
                inputArgs["Password"] = Passwd;
                inputArgs["UserName"] = Uname;
                //Set the name
                ManagementBaseObject outParams = wmiObject.InvokeMethod("Rename", inputArgs, null);
                //Weird WMI shennanigans to get a return code 
                uint ret = (uint)(outParams.Properties["ReturnValue"].Value);
                if (ret == 0)
                {
                    //It worked
                    //MessageBox.Show("计算机名修改成功");                  
                    logtxt.AppendText("计算机名修改成功\n");
                    return true;
                }
                else
                {
                    //It didn't work
                    //MessageBox.Show("计算机名修改失败，请手动修改成新的计算机名");
                    logtxt.AppendText("计算机名修改失败，请手动修改成新的计算机名\n");
                    return false;
                }
            }
        }
        public void changname()
        {
            string domainName = textBox3.Text.ToString();
            string newcpname = "KEDA-" + GetBIOSNumber();
            textBox4.Text = newcpname;
            SetMachineName(newcpname, "123456", domainName + "\\" + userName);
            System.Diagnostics.Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = "cmd.exe";//启动cmd命令
            myProcess.StartInfo.UseShellExecute = false;//是否使用系统外壳程序启动进程
            myProcess.StartInfo.RedirectStandardInput = true;//是否从流中读取
            myProcess.StartInfo.RedirectStandardOutput = true;//是否写入流
            myProcess.StartInfo.RedirectStandardError = true;//是否将错误信息写入流
            myProcess.StartInfo.CreateNoWindow = true;//是否在新窗口中启动进程
            myProcess.Start();//启动进程
            myProcess.StandardInput.WriteLine("shutdown -r -t 0");
        }
        private void button3_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Thread changeu = new Thread(changname);
            changeu.Start();
        }
        public class ApiCopyFile
        {
            private const int FO_COPY = 0x0002;
            private const int FOF_ALLOWUNDO = 0x00044;
            //显示进度条  0x00044 // 不显示一个进度对话框 0x0100 显示进度对话框单不显示进度条  0x0002显示进度条和对话框  
            private const int FOF_SILENT = 0x0002;//0x0100;  

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 0)]
            public struct SHFILEOPSTRUCT
            {
                public IntPtr hwnd;
                [MarshalAs(UnmanagedType.U4)]
                public int wFunc;
                public string pFrom;
                public string pTo;
                public short fFlags;
                [MarshalAs(UnmanagedType.Bool)]
                public bool fAnyOperationsAborted;
                public IntPtr hNameMappings;
                public string lpszProgressTitle;

               
            }
            [DllImport("shell32.dll", CharSet = CharSet.Auto)]
            static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);
            public static bool DoCopy(string strSource, string strTarget)
            {
                SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
                fileop.wFunc = FO_COPY;
                fileop.pFrom = strSource;
                fileop.lpszProgressTitle = "复制文件";
                fileop.pTo = strTarget;
                //fileop.fFlags = FOF_ALLOWUNDO;  
                fileop.fFlags = FOF_SILENT;
                return SHFileOperation(ref fileop) == 0;
            }
        }     
        private void button4_Click(object sender, EventArgs e)
        {
            Thread usmtfm = new Thread(usmtfrom);
            usmtfm.Start();
            
        }
        public void usmtfrom()
        {
            MethodInvoker MethInvo = new MethodInvoker(showusmt);
            BeginInvoke(MethInvo);           
        }
        public void showusmt()
        {
            USMT FRM = new USMT();
            FRM.Show();
        }
    }
}   


   
