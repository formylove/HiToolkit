using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.XPath;
using System.Xml;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;

namespace robotX
{
    class CmdSetService
    {
        private System.Diagnostics.Process p;
        private Window w;
        private DBAcesser accesser = new DBAcesser();

        public void Start(Button objButton) {
            string para = objButton.ToolTip.ToString();
            DisplayResult(para);
        }

        public void ShuffleList()
        {
            string query = "select top 4  Para from  LatestUsed l order by l.stamp desc";
            ListBox recList = (ListBox)w.FindName("RecList");
            System.Data.DataView dv = accesser.FetchDataSet(query).DefaultView;
            recList.ItemsSource = dv;
            recList.DisplayMemberPath = "Para";
            recList.SelectedValuePath = "Para";
        }
        public void SavePara()
        {
          
        }
        public void ShuffleList(string acronym)
        {
            string query = "select top 6 Para,Para & '      ' & Tag as fullName from  Parameter i  where capital like '{0}%' or capital like '%{0}%' or  Para like '{0}%' or Para like '%{0}%' ";
            query = String.Format(query,acronym);
            ListBox recList = (ListBox)w.FindName("RecList");
            System.Data.DataView dv = accesser.FetchDataSet(query).DefaultView;
            recList.ItemsSource = dv;
            recList.DisplayMemberPath = "fullName";
            recList.SelectedValuePath = "Para";
        }
        public  CmdSetService(Window w) {
            this.w = w;
            p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;              //是否指定操作系统外壳进程启动程序，这里需为false  
            p.StartInfo.RedirectStandardOutput = true;// 设置为 true
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.UseShellExecute = false;              //是否指定操作系统外壳进程启动程序，这里需为false  
            p.StartInfo.RedirectStandardOutput = true;// 设置为 true
            



        }
        public void Excute()
        {
            string parameter = ((TextBox)w.FindName("Parameter")).Text;
            parameter = (parameter != null) ? parameter.Trim().Replace(" ", "") : parameter;
            string tag = ((ComboBoxItem)((ComboBox)w.FindName("CmdBox")).SelectedItem).Tag.ToString();
            if (parameter != null && parameter.Length>0)
            {
                if (tag.Equals("java"))
                {
                    SetEnvironmentVariable();
                }
                else
                {

                    string para = tag + parameter.Trim();
                    DisplayResult(para);
                    //p.Close();
                    Thread t = new Thread(new ParameterizedThreadStart(UpdateLUsed));
                    t.Start(parameter);
                }
            }
            else
            {
                MessageBox.Show("参数为空");
            }
        }
        private void UpdateLUsed(object data )
        {
            string parameter = data as string;
            string sql;
            if (Regex.IsMatch(parameter, CommonTool.IPReg) || Regex.IsMatch(parameter, " ^.+\\.exe$") || Regex.IsMatch(parameter, CommonTool.UrlReg) || Regex.IsMatch(parameter, @"[0-9]+") || (((ComboBoxItem)w.FindName("ftp")).IsSelected && Regex.IsMatch(parameter, CommonTool.FTPReg)))
            {
                bool isDuplicated = accesser.GetCount( String.Format("select count(*) from latestused where para = '{0}'",parameter)) > 0 ;
                if (isDuplicated)
                {
                sql = String.Format("update latestused set para = '{0}',stamp=now() where para = '{0}'", parameter); ;
                }
                else
                {
                sql = String.Format("insert into latestused(para) values('{0}')", parameter);
                }
                accesser.Update(sql);
                sql = "delete * from latestused where para not in (select top 6 para from latestused order by stamp desc)";
                accesser.Update(sql);
            }
        }
        private void DisplayResult(string para)
        {
            p.Start();
            p.StandardInput.WriteLine(para + "&exit");
            p.StandardInput.Flush();
            p.StandardInput.AutoFlush = true;
            //string result = p.StandardOutput.ReadToEnd() + p.StandardError.ReadToEnd();
            StreamReader reader = p.StandardOutput;
            string result = reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string temp = reader.ReadLine();
                bool s = temp.Contains("DNS 后缀");
                bool isShown = !(temp.Contains("保留所有权利") || temp.Contains("DNS 后缀") || temp.Contains("IPv6 地址") || temp.Contains("媒体状态") || temp.Contains("Windows IP 配置") || temp.Contains("版权所有") || temp.Contains("Windows IP Configuration") || temp.Contains("DNS Suffix"));
                if (isShown)
                {
                    if (temp.Contains("IPv4"))
                    {
                    result += "*************" + temp  + "****************"  + "\r\n";

                    }
                    else
                    {

                    result += temp + "\r\n";
                    }
                }

            }
            result += p.StandardError.ReadToEnd();
            ((TextBlock)w.FindName("Result")).Text = result.Replace("\r\n\r\n", "\r\n").Replace("\r\n\r\n", "\r\n").Replace("&exit", ""); ;
            p.WaitForExit();//等待程序执行完退出进程


        }

        private void SetEnvironmentVariable()
        {
            try
            {

                string parameter = ((TextBox)w.FindName("Parameter")).Text;
                parameter = (parameter != null) ? parameter.Trim() : parameter;
                if (parameter != null && Regex.IsMatch(parameter, CommonTool.UrlReg))
                {
                    Environment.SetEnvironmentVariable("JAVA_HOME", parameter, EnvironmentVariableTarget.Machine);
                    Environment.SetEnvironmentVariable("CLASSPATH", @".;%JAVA_HOME%\lib\dt.jar;%JAVA_HOME%\lib\tools.jar;", EnvironmentVariableTarget.Machine);
                    Add2Path();
                    DisplayResult("javac");
                }
                else
                {
                    MessageBox.Show("非有效的Java程序的安装路径(例如 C:\\Program Files\\Java\\jdk1.8.0_144)");

                }
            }
            catch (System.Security.SecurityException )
            {
                MessageBox.Show("使用此功能需要用管理员权限打开此应用。");
            }
        }
        public void Add2Path()
        {
            string pathlist;
            string pathValue = @"%JAVA_HOME%\bin;%JAVA_HOME%\jre\bin";
            pathlist = Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            string[] list = pathlist.Split(';');
            bool isPathExist = false;

            foreach (string item in list)
            {
                if (item.Equals(pathValue))
                    isPathExist = true;
            }
            if (!isPathExist)
            {
                Environment.SetEnvironmentVariable("PATH", pathValue + ";" + pathlist, EnvironmentVariableTarget.Machine);
           }

        }
        public void SetSysMeta()
        {
            TextBlock sMeta = (TextBlock)w.FindName("SMeta");
            String IPAddress = (new System.Net.IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address)).ToString();//获取本机的IP地址
            sMeta.Text += "内网IP   ：" + CommonTool.LocalIP + Environment.NewLine;
            sMeta.Text += "外网IP   ：" + IPAddress + Environment.NewLine;
            if (Environment.Is64BitOperatingSystem)//Environment.Is64BitOperatingSystem .NET 4.0使用
            {
                sMeta.Text += "系统位数：64位" + Environment.NewLine;
            }
            else
            {
                sMeta.Text += "系统位数：32位" + Environment.NewLine;
            }
            sMeta.Text += "计算机名：" + Environment.MachineName + Environment.NewLine;
            sMeta.Text += "用户名   ：" + Environment.UserName + Environment.NewLine;

            //String hostInfo = Dns.GetHostName();//获取本机的计算机名  
            //richTextBox1.AppendText("计算机名:" + SystemInformation.ComputerName);
            //richTextBox1.AppendText(Environment.NewLine);//换行  
            //richTextBox1.AppendText("计算机名:" + Environment.MachineName);
            //richTextBox1.AppendText(Environment.NewLine);
            //richTextBox1.AppendText("操作系统:" + Environment.OSVersion.Platform);
            //richTextBox1.AppendText(Environment.NewLine);
            //richTextBox1.AppendText("版本号:" + Environment.OSVersion.VersionString);
            //richTextBox1.AppendText(Environment.NewLine);
            //richTextBox1.AppendText("处理器个数:" + Environment.ProcessorCount);

        }









    }
}

