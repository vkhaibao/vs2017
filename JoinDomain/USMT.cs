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
using System.Threading;


namespace JoinDomain
{
    public partial class USMT : Form
    {
        public USMT()
        {
            InitializeComponent();
        }
        public void backupuser()
        {
            textBox1.Text = "";
            string bakuser;
            string bakcmd;
            string bakdir;
            bool type;
            string file;
            bakuser = UserName.Text.ToString();
            type = Environment.Is64BitOperatingSystem;
            bakdir = textBox4.Text.ToString();
            string runpath = System.IO.Directory.GetCurrentDirectory();
            file = runpath + "\\prog.log";
            if (File.Exists(file))
            {
                File.Delete(file);
            }
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
                flistlog = "/listfiles:filelist.log ";
                scanlog = "/l:scan.log ";
                log = processlog + flistlog + scanlog;

                //"/i:" + bakurl + "\\MigApp.xml " +
                xml = " /i:" + bakurl + "\\MigApp.xml " + "/i:" + bakurl + "\\MigDocs.xml " + "/i:" + bakurl + "\\MigUser.xml ";
                bakcmd = bakurl + "\\scanstate.exe " + bakdir + xml + log + "/vsc /o /localonly /c /ue:*\\* /ui:" + bakuser;
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
                processlog = "/progress:prog.log ";
                flistlog = "/listfiles:filelist.log ";
                scanlog = "/l:scan.log ";
                log = processlog + flistlog + scanlog;
                xml = " /i:" + bakurl + "\\MigApp.xml " + "/i:" + bakurl + "\\MigDocs.xml " + "/i:" + bakurl + "\\MigUser.xml ";
                bakcmd = bakurl + "\\scanstate.exe " + bakdir + xml + log + "/vsc /o /localonly /c /ue:*\\* /ui:" + bakuser;
            }
            textBox1.AppendText(bakcmd + "\n");
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
          
            while (IsFileInUse(file) is true)
            {
                for (int i = 0; i <= 100; i++)
                {
                    toolStripProgressBar1.Value = i;
                    System.Threading.Thread.Sleep(500);
                }
            }
            toolStripProgressBar1.Value = 100;
            toolStripStatusLabel1.Text = "备份完成";
            StreamReader sr = new StreamReader(file, Encoding.Default);
            string[] allLines = System.IO.File.ReadAllLines(file);
            string lastestLine = allLines[allLines.Length - 1];
            textBox2.AppendText(lastestLine + "\n");

        }
        private void button1_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Thread back = new Thread(backupuser);           
            back.Start();
        }
    
        private void USMT_Load(object sender, EventArgs e)
        {
            string path = Environment.UserDomainName + "\\" + Environment.UserName;
            UserName.Text = path;          
        }

        private void timer1_Tick(object sender, EventArgs e)
        { 
            //string runpath = System.IO.Directory.GetCurrentDirectory();
            //string file;
            //file =runpath + "\\prog.log";
            //if (IsFileInUse(file) is true)
            //{
            //    for (int i = 0; i <= 100; i++)
            //    {
            //        toolStripProgressBar1.Value = i;
            //        System.Threading.Thread.Sleep(500);
            //    }
            //}
            //else
            //{
            //    toolStripProgressBar1.Value = 100;
            //    toolStripStatusLabel1.Text = "备份完成";
            //    StreamReader sr = new StreamReader(file, Encoding.Default);                             
            //    string[] allLines = System.IO.File.ReadAllLines(file);
            //    string lastestLine = allLines[allLines.Length - 1];
            //    textBox2.AppendText(lastestLine+"\n");
            //    timer1.Enabled = false;                   
            //}         
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
        public void restoreuser()
        {
            textBox1.Text = "";
            string olduser;
            string newuser;
            string rsdir;
            string bakcmd;
            bool type;
            string file;
            olduser = UserName.Text.ToString();
            newuser = textBox3.Text.ToString();
            type = Environment.Is64BitOperatingSystem;
            rsdir = textBox4.Text.ToString();
            string runpath = System.IO.Directory.GetCurrentDirectory();
            file = runpath + "\\sprog.log";
            if (File.Exists(file))
            {
                File.Delete(file);
            }
            if (type is true)
            {
                string bakurl;
                string xml;
                string processlog;
                string scanlog;
                string log;
                bakurl = ".\\USMT\\amd64";
                processlog = "/progress:sprog.log ";
                scanlog = "/l:load.log ";
                log = processlog + scanlog;
                //"/i:" + bakurl + "\\MigApp.xml " +
                xml = " /i:" + bakurl + "\\MigApp.xml " + "/i:" + bakurl + "\\MigDocs.xml " + "/i:" + bakurl + "\\MigUser.xml ";
                bakcmd = bakurl + "\\loadstate.exe " + rsdir + xml + "/lac /lae " + log + "/c /MU:" + olduser + ":" + newuser + " /ue:*\\* /ui:" + olduser;
            }
            else
            {
                string bakurl;
                string xml;
                string processlog;
                string scanlog;
                string log;
                bakurl = ".\\USMT\\x86";
                processlog = "/progress:sprog.log ";
                scanlog = "/l:load.log ";
                log = processlog + scanlog;
                xml = " /i:" + bakurl + "\\MigApp.xml " + "/i:" + bakurl + "\\MigDocs.xml " + "/i:" + bakurl + "\\MigUser.xml ";
                bakcmd = bakurl + "\\loadstate.exe " + rsdir + xml + "/lac /lae " + log + "/c /MU:" + olduser + ":" + newuser + " /ue:*\\* /ui:" + olduser;
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

            while (IsFileInUse(file) is true)
            {
                for (int i = 0; i <= 100; i++)
                {
                    toolStripProgressBar1.Value = i;
                    System.Threading.Thread.Sleep(500);
                }
            }
            toolStripProgressBar1.Value = 100;
            toolStripStatusLabel1.Text = "还原完成";
            StreamReader sr = new StreamReader(file, Encoding.Default);
            string[] allLines = System.IO.File.ReadAllLines(file);
            string lastestLine = allLines[allLines.Length - 1];
            textBox2.AppendText(lastestLine + "\n");
            timer1.Enabled = false;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            CheckForIllegalCrossThreadCalls = false;
            Thread rstore = new Thread(restoreuser);         
            rstore.Start();       
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

        private void button3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string savePath = dialog.SelectedPath;
                textBox4.Text = savePath;
            }
        }
    }
}
