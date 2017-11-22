using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
namespace robotX
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        const string ipReg = "^((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$";
        const string relUrl = @"./";
        string[,] args4XML = {{"HisWebServices_Clound/WEB-INF/classes/generatorConfig.xml", "//jdbcConnection", "/@connectionURL", "/@userId", "/@password" },
                              { "SupLab_Clound/WEB-INF/classes/generatorConfig.xml", "//jdbcConnection","/@connectionURL", "/@userId", "/@password"  },
                              { "SupLab_Clound/WEB-INF/classes/applicationContext-dataSource.xml", "//*","[@name='jdbcUrl']/@value", "[@name='user']/@value", "[@name='password']/@value" } };
        string localIP = "?";
        string ipHis;
        string ipLis;
        string tomcatPort;
        string ipMatrix;
        string portMatrix;

        string unitsCode;

        string hisORCLInstance;
        string lisORCLInstance;

        string hisDBUser;
        string lisDBUser;

        string hisDBPSW;
        string lisDBPSW;
        bool isNewHIS = true;
        System.Diagnostics.Process p;
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            Console.WriteLine("xxxx");
            InitializeComponent();
            Console.WriteLine("xxxx");
            Console.WriteLine(this.IsNewHIS.IsChecked);

            p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            //p.StandardInput.AutoFlush = true;
            p.Start();//启动程序



            //p.Close();
            SetSysMeta();
            
        }
        private void GetMetaInfo()
        {

            //获取并设置本地IP
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily.ToString() == "InterNetwork")
                {
                    if (ip.ToString().StartsWith("192.168"))
                    {
                        localIP = ip.ToString();
                    }

                    break;
                }
            }
            this.LocalIP.Text = localIP;

        }
        void SetSysMeta()
        {
            
            String IPAddress = (new System.Net.IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address)).ToString();//获取本机的IP地址
            this.SMeta.Text += "IP地址   ：" + IPAddress + Environment.NewLine;
            if (Environment.Is64BitOperatingSystem)
            {
                this.SMeta.Text += "系统位数：:64位" + Environment.NewLine;
            }
            else
            {
                this.SMeta.Text += "系统位数：32位" + Environment.NewLine;
            }
            this.SMeta.Text += "计算机名：" + Environment.MachineName + Environment.NewLine;
            this.SMeta.Text += "用户名   ：" + Environment.UserName + Environment.NewLine;
            this.SMeta.Text += "操作系统：" + Environment.OSVersion.Platform + Environment.NewLine;
            this.SMeta.Text += "版本号  ：" + Environment.OSVersion.VersionString + Environment.NewLine;
           
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
            GetMetaInfo();

        }

        private void CmdProcess(object sender, RoutedEventArgs e)
        {
            Button objButton = (Button)sender;
            p.StandardInput.WriteLine(objButton.ToolTip);
            //p.WaitForExit();//等待程序执行完退出进程
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine("xxxx");
            Console.WriteLine(this.IsNewHIS.IsChecked);
            //表单数据
            ipHis = this.HisIP.Text;
             ipLis = this.LisIP.Text;
             tomcatPort = this.TomcatPort.Text;
             ipMatrix = this.MatrixIP.Text;
             portMatrix = this.MatrixPort.Text;

             unitsCode = this.UnitsCode.Text;

             hisORCLInstance = this.HisORCLInstance.Text;
             lisORCLInstance = this.LisORCLInstance.Text;

             hisDBUser = this.HisDBUser.Text;
             lisDBUser = this.LisDBUser.Text;
            
             hisDBPSW = this.HisDBPSW.Text;
             lisDBPSW = this.LisDBPSW.Text;

            isNewHIS = (bool)this.IsNewHIS.IsChecked;
            try
            {
                //执行替换
                ConfigTheXML();
                ConfigTheIni();
                ConfigTheProperties();
                MessageBox.Show("替换成功!");
            }
            catch
            {
               
            }
          


           
      }





        private void ConfigTheIni()
        {
            string[] fileList = { "SvMatrixServer/Config/Matrix.ini", "LIS_Cloud/Config/Matrix.ini" };
            string[] contents = { ipMatrix,portMatrix };
            foreach (string f in fileList)
            {
                File.WriteAllLines(relUrl+f,contents);
            }
        }
        private void ConfigTheXML()
        {
            // bool rightIP = Regex.IsMatch(ipLis, ipReg);
            bool rightIP = true;
            
            string connectionURL = "jdbc:oracle:thin:@" + ipLis + ":1521:" + lisORCLInstance;
            string[] values = { connectionURL, lisDBUser, lisDBPSW };

            
            
            if (rightIP)
            {
                XmlDocument xml;
                for (int i=0; i<3; i++)
                {
                    xml = new XmlDocument();
                    xml.Load(relUrl + args4XML[i,0]);
                    for(int j = 2; j < 5; j++)
                    {
                        //XPath by itself cannot be used to modify XML documents.But the xmlstarlet command line utility can do it.
                          xml.SelectSingleNode(args4XML[i, 1] + args4XML[i, j]).Value = values[j - 2];
                          xml.Save(relUrl + args4XML[i, 0]);
                    }
                }
                
            }
            else
            {
                MessageBox.Show("LIS IP 格式错误");
            }
        }

        private void ConfigTheProperties()
        {
            // bool rightIP = Regex.IsMatch(ipLis, ipReg);
            string[] fileList = { "HisWebServices_Clound/WEB-INF/classes/application.properties", "HisWebServices_Clound/WEB-INF/classes/systemenv.properties",
                "SupLab_Clound/WEB-INF/classes/systemenv.properties", "LIS_Cloud/Config/WSConfig.ini" };

            foreach(string p in fileList)
            {
                ReplaceItem(p);
            }



        }

        private void ReplaceItem(string  path)
        {
            bool rightIP = Regex.IsMatch(ipLis, ipReg);


            StreamReader r = new StreamReader(relUrl + path, Encoding.UTF8);
            FileStream fs = new FileStream(relUrl + path +"Copy", FileMode.Create);
            StreamWriter w = new StreamWriter(fs);
            Dictionary<string, string> prop = new Dictionary<string, string>()
            { { "UNITS_CODE", unitsCode }, { "UnitsCode", unitsCode }, { "MESSAGE_SERVER_HOST", ipMatrix },
              { "MESSAGE_SERVER_PORT", portMatrix }, { "IS_NEW_HIS_INTERFACE", isNewHIS?"true":"false"}, { "ISNewHis",isNewHIS? "1" :"0"} };

            prop.Add("EndPoint", "http://" + ipLis  + ":" + tomcatPort + "/SupLab_Clound/services/supLabDict?wsdl");
            prop.Add("HisEndPoint", "http://" + ipHis + ":" + tomcatPort + "/HisWebServices_Clound/services/supHisWebServices?wsdl");
            prop.Add("dataSource.url", "jdbc:oracle:thin:@(description=(address=(protocol=tcp)(port=1521)(host=" + ipHis + "))(connect_data=(service_name=" + hisORCLInstance + ")))");
            prop.Add("dataSource2.url", "jdbc:oracle:thin:@" + ipLis + ":" + lisORCLInstance);
            prop.Add("dataSource3.url", "jdbc:oracle:thin:@(description=(address=(protocol=tcp)(port=1521)(host=" + ipHis + "))(connect_data=(service_name=" + hisORCLInstance + ")))");
            prop.Add("dataSource.username", hisDBUser);
            prop.Add("dataSource2.username", lisDBUser);
            prop.Add("dataSource3.username", hisDBUser);
            prop.Add("dataSource.password", hisDBPSW);
            prop.Add("dataSource2.password", lisDBPSW);
            prop.Add("dataSource3.password", hisDBPSW);

            string line;
            while ((line = r.ReadLine()) != null)
            {
                Console.WriteLine(line);
                line = line.Trim();
                foreach(string key in prop.Keys)
                {
                    if (line.StartsWith(key))
                    {
                        line = line.Substring(0, line.IndexOf("=") + 1) + prop[key];
                        break;
                    }
                }
                Console.WriteLine(line);
                
               
                w.WriteLine(line);

            }
            w.Flush();
            w.Close();
            fs.Close();
            r.Close();



            File.Delete(relUrl + path);
            FileInfo fCopy = new FileInfo(relUrl + path+"Copy");
            fCopy.MoveTo(relUrl + path);
            
        }










        //调用批处理
        //Process proc = new Process();
        //proc.StartInfo.WorkingDirectory = Application.StartupPath;
        //        proc.StartInfo.FileName = "service install.bat";
        //        proc.StartInfo.Arguments = String.Format("10");
        //        proc.StartInfo.CreateNoWindow = true;
        //        proc.Start();
        //        proc.WaitForExit();
        //        MessageBox.Show("Create Success!");





















        private void HisIP_TextChanged(object sender, TextChangedEventArgs e)
        {
         
        }



        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.MatrixIP.Text = this.LisIP.Text;
            Console.WriteLine(this.MatrixIP.Text);
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (this.CmdBox.SelectedIndex)
            {
                case 0:

                    break;
                case 1:

                    break;
                case 2:

                    break;
                case 3:

                    break;
                case 4:

                    break;
                case 5:

                    break;






            }
            MessageBox.Show(Convert.ToString(CmdBox.SelectedIndex));
            
        }

        private void SetPath(object sender, RoutedEventArgs e)
        {
            try
            {

            string parameter = this.Paramenter.Text;
            if(Regex.IsMatch(parameter, @"^[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\]+)+\\?)|(\\)?)\s*$"))
            {

            Environment.SetEnvironmentVariable("JAVA_HOME ", this.Paramenter.Text, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable("CLASSPATH", @".;%JAVA_HOME%\lib\dt.jar;%JAVA_HOME%\lib\tools.jar;", EnvironmentVariableTarget.Machine);
            Add2Path();

            }
            else
            {
                MessageBox.Show("非有效的Java程序的安装路径");

            }
            }
            catch(System.Security.SecurityException e2)
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
                if (item == pathValue)
                    isPathExist = true;
            }
            if (!isPathExist)
            {
                Environment.SetEnvironmentVariable("PATH", pathValue + ";" + pathlist, EnvironmentVariableTarget.Machine);
            }
        }
    }
}
