using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;


namespace JoinDomain
{
    public partial class USMT : Form
    {
        public USMT()
        {
            InitializeComponent();
        }        
        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            string bakuser;
            string bakcmd;
            bool type;
            bakuser = UserName.Text.ToString();
            type = Environment.Is64BitOperatingSystem;
            string runpath = System.IO.Directory.GetCurrentDirectory();
            if (type is true)
            {
                string bakurl;
                string xml;
                string processlog;
                string flistlog;
                string scanlog;
                string log;
                bakurl = ".\\USMT\\amd64";
                processlog = "/progress:prog.log ";
                flistlog =  "/listfiles:filelist.log ";
                scanlog ="/l:scan.log ";
                log = processlog + flistlog + scanlog;
                //"/i:" + bakurl + "\\MigApp.xml " +
                xml = "/i:" + bakurl + "\\MigApp.xml "+"/i:" + bakurl + "\\MigDocs.xml " + "/i:" + bakurl + "\\MigUser.xml ";
                bakcmd = bakurl + "\\scanstate.exe C:\\ " + xml + log + "/vsc /o /localonly /c /ue:*\\* /ui:" + bakuser;
            }
            else
            {
                string bakurl;
                string xml;
                string processlog;
                string flistlog;
                string scanlog;
                string log;
                bakurl = ".\\USMT\\x86";
                processlog ="/progress:prog.log ";
                flistlog ="/listfiles:filelist.log ";
                scanlog ="/l:scan.log ";
                log = processlog + flistlog + scanlog;
                xml = "/i:" + bakurl + "\\MigApp.xml "+"/i:" + bakurl + "\\MigDocs.xml " + "/i:" + bakurl + "\\MigUser.xml ";
                bakcmd = bakurl + "\\scanstate.exe C:\\ " + xml + log + "/vsc /o /localonly /c /ue:*\\* /ui:" + bakuser;               
            }            
            textBox1.AppendText(bakcmd+"\n");
            textBox2.AppendText("正在备份，请等待备份完成\n");
            toolStripStatusLabel1.Text = "开始备份";
            Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = "cmd.exe";//启动cmd命令
            myProcess.StartInfo.UseShellExecute = false;//是否使用系统外壳程序启动进程
            myProcess.StartInfo.RedirectStandardInput = true;//是否从流中读取
            myProcess.StartInfo.RedirectStandardOutput = true;//是否写入流
            myProcess.StartInfo.RedirectStandardError = true;//是否将错误信息写入流
            myProcess.StartInfo.CreateNoWindow = true;//是否在新窗口中启动进程
            myProcess.Start();//启动进程
            myProcess.StandardInput.WriteLine(bakcmd);
            timer1.Enabled = true;
            timer1.Interval = 10000;
            timer1.Start();
        }
    
        private void USMT_Load(object sender, EventArgs e)
        {
            string path = Environment.UserDomainName + "\\" + Environment.UserName;
            UserName.Text = path;
        }

        private void timer1_Tick(object sender, EventArgs e)
        { 
            string runpath = System.IO.Directory.GetCurrentDirectory();
            string file;
            file =runpath + "\\prog.log";
            if (IsFileInUse(file) is true)
            {
                for (int i = 0; i <= 100; i++)
                {
                    toolStripProgressBar1.Value = i;
                    System.Threading.Thread.Sleep(500);
                }
            }
            else
            {
                toolStripProgressBar1.Value = 100;
                toolStripStatusLabel1.Text = "备份完成";
                StreamReader sr = new StreamReader(file, Encoding.Default);                             
                string[] allLines = System.IO.File.ReadAllLines(file);
                string lastestLine = allLines[allLines.Length - 1];
                textBox2.AppendText(lastestLine+"\n");
                timer1.Enabled = false;                   
            }         
        }
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = true;

            FileStream fs = null;
            try
            {

                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,

                FileShare.None);

                inUse = false;
            }
            catch
            {
            }
            finally
            {
                if (fs != null)

                    fs.Close();
            }
            return inUse;//true表示正在使用,false没有使用  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            //string runpath = System.IO.Directory.GetCurrentDirectory();

            //string fn = runpath + "\\prog.log";
            //string[] allLines = System.IO.File.ReadAllLines(fn);
            //string lastestLine = allLines[allLines.Length - 1];
            //MessageBox.Show(lastestLine);
            //for (int i = 0; i <= 100; i++)
            //{
            //    toolStripProgressBar1.Value = i;
            //    System.Threading.Thread.Sleep(500);
            //}
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            string bakuser;
            string bakcmd;
            bool type;
            bakuser = UserName.Text.ToString();
            type = Environment.Is64BitOperatingSystem;
            string runpath = System.IO.Directory.GetCurrentDirectory();
            if (type is true)
            {
                string bakurl;
                string xml;
                string processlog;
                string flistlog;
                string scanlog;
                string log;
                bakurl = ".\\USMT\\amd64";
                processlog = "/progress:sprog.log ";
                flistlog = "/listfiles:filelist.log ";
                scanlog = "/l:load.log ";
                log = processlog + flistlog + scanlog;
                //"/i:" + bakurl + "\\MigApp.xml " +
                xml = "/i:" + bakurl + "\\MigApp.xml " + "/i:" + bakurl + "\\MigDocs.xml " + "/i:" + bakurl + "\\MigUser.xml ";
                bakcmd = bakurl + "\\loadstate.exe C:\\ " + xml + "/lac /lae " + log + "/c /MU:*\\*:" + bakuser + " /ue:*\\* /ui:*\\*" ;
            }
            else
            {
                string bakurl;
                string xml;
                string processlog;
                string flistlog;
                string scanlog;
                string log;
                bakurl = ".\\USMT\\x86";
                processlog = "/progress:sprog.log ";
                flistlog = "/listfiles:filelist.log ";
                scanlog = "/l:load.log ";
                log = processlog + flistlog + scanlog;
                xml = "/i:" + bakurl + "\\MigApp.xml " + "/i:" + bakurl + "\\MigDocs.xml " + "/i:" + bakurl + "\\MigUser.xml ";
                bakcmd = bakurl + "\\loadstate.exe C:\\ " + xml + "/lac /lae " + log + "/c /MU:*\\*:"+ bakuser + " /ue:*\\* /ui:*\\*";
            }
            textBox1.AppendText(bakcmd + "\n");
            textBox2.AppendText("正在还原，请等待还原完成\n");
            toolStripStatusLabel1.Text = "开始还原";
            Process myProcess = new System.Diagnostics.Process();
            myProcess.StartInfo.FileName = "cmd.exe";//启动cmd命令
            myProcess.StartInfo.UseShellExecute = false;//是否使用系统外壳程序启动进程
            myProcess.StartInfo.RedirectStandardInput = true;//是否从流中读取
            myProcess.StartInfo.RedirectStandardOutput = true;//是否写入流
            myProcess.StartInfo.RedirectStandardError = true;//是否将错误信息写入流
            myProcess.StartInfo.CreateNoWindow = true;//是否在新窗口中启动进程
            myProcess.Start();//启动进程
            myProcess.StandardInput.WriteLine(bakcmd);
            timer2.Enabled = true;
            timer2.Interval = 10000;
            timer2.Start();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            string runpath = System.IO.Directory.GetCurrentDirectory();
            string file;
            file = runpath + "\\sprog.log";
            if (IsFileInUse(file) is true)
            {
                for (int i = 0; i <= 100; i++)
                {
                    toolStripProgressBar1.Value = i;
                    System.Threading.Thread.Sleep(500);
                }
            }
            else
            {
                toolStripProgressBar1.Value = 100;
                toolStripStatusLabel1.Text = "还原完成";
                StreamReader sr = new StreamReader(file, Encoding.Default);
                string[] allLines = System.IO.File.ReadAllLines(file);
                string lastestLine = allLines[allLines.Length - 1];
                textBox2.AppendText(lastestLine + "\n");
                timer1.Enabled = false;
            }
        }
    }
}
