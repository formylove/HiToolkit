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


namespace robotX
{
    class ConfigLISService
    {
        private Window w;
        const string ipReg = "^((2[0-4]\\d|25[0-5]|[01]?\\d\\d?)\\.){3}(2[0-4]\\d|25[0-5]|[01]?\\d\\d?)$";
        const string relUrl = @"./";
        public string[,] args4XML = {{"HisWebServices_Clound/WEB-INF/classes/generatorConfig.xml", "//jdbcConnection", "/@connectionURL", "/@userId", "/@password" },
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
       public  ConfigLISService(Window w)
        {
            this.w = w;
        }
        public void Init()
        {
            ipHis = ((TextBox )w.FindName("HisIP")).Text;
            ipLis = ((TextBox )w.FindName("LisIP")).Text;
            tomcatPort = ((TextBox )w.FindName("TomcatPort")).Text;
            ipMatrix = ((TextBox )w.FindName("MatrixIP")).Text;
            portMatrix = ((TextBox )w.FindName("MatrixPort")).Text;
            unitsCode = ((TextBox )w.FindName("UnitsCode")).Text;
            hisORCLInstance = ((TextBox )w.FindName("HisORCLInstance")).Text;
            lisORCLInstance = ((TextBox )w.FindName("LisORCLInstance")).Text;
            hisDBUser = ((TextBox )w.FindName("HisDBUser")).Text;
            lisDBUser = ((TextBox )w.FindName("LisDBUser")).Text;
            hisDBPSW = ((TextBox )w.FindName("HisDBPSW")).Text;
            lisDBPSW = ((TextBox )w.FindName("LisDBPSW")).Text;
            isNewHIS = (bool)(((CheckBox)w.FindName("IsNewHIS")).IsChecked);

        }
        public void ConfigTheIni()
        {
            string[] fileList = { "SvMatrixServer/Config/Matrix.ini", "LIS_Cloud/Config/Matrix.ini" };
            string[] contents = { ipMatrix, portMatrix };
            foreach (string f in fileList)
            {
                File.WriteAllLines(relUrl + f, contents);
            }
        }
        public void ConfigTheXML()
        {
            // bool rightIP = Regex.IsMatch(ipLis, ipReg);
            bool rightIP = true;

            string connectionURL = "jdbc:oracle:thin:@" + ipLis + ":1521:" + lisORCLInstance;
            string[] values = { connectionURL, lisDBUser, lisDBPSW };



            if (rightIP)
            {
                XmlDocument xml;
                for (int i = 0; i < 3; i++)
                {
                    xml = new XmlDocument();
                    xml.Load(relUrl + args4XML[i, 0]);
                    for (int j = 2; j < 5; j++)
                    {
                        //XPath by itself cannot be used to modify XML documents.But the xmlstarlet command line utility can do it.
                        xml.SelectSingleNode(args4XML[i, 1] + args4XML[i, j]).Value = values[j - 2];
                        xml.Save(relUrl + args4XML[i, 0]);
                    }
                }

            }
            else
            {
            }
        }

        public void ConfigTheProperties()
        {
            // bool rightIP = Regex.IsMatch(ipLis, ipReg);
            string[] fileList = { "HisWebServices_Clound/WEB-INF/classes/application.properties", "HisWebServices_Clound/WEB-INF/classes/systemenv.properties",
                "SupLab_Clound/WEB-INF/classes/systemenv.properties", "LIS_Cloud/Config/WSConfig.ini" };

            foreach (string p in fileList)
            {
                ReplaceItem(p);
            }



        }

        public void ReplaceItem(string path)
        {
            bool rightIP = Regex.IsMatch(ipLis, ipReg);


            StreamReader r = new StreamReader(relUrl + path, Encoding.UTF8);
            FileStream fs = new FileStream(relUrl + path + "Copy", FileMode.Create);
            StreamWriter w = new StreamWriter(fs);
            Dictionary<string, string> prop = new Dictionary<string, string>()
            { { "UNITS_CODE", unitsCode }, { "UnitsCode", unitsCode }, { "MESSAGE_SERVER_HOST", ipMatrix },
              { "MESSAGE_SERVER_PORT", portMatrix }, { "IS_NEW_HIS_INTERFACE", isNewHIS?"true":"false"}, { "ISNewHis",isNewHIS? "1" :"0"} };

            prop.Add("EndPoint", "http://" + ipLis + ":" + tomcatPort + "/SupLab_Clound/services/supLabDict?wsdl");
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
                foreach (string key in prop.Keys)
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
            FileInfo fCopy = new FileInfo(relUrl + path + "Copy");
            fCopy.MoveTo(relUrl + path);

        }
        public void SetMetaInfo()
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
            ((TextBlock)w.FindName("LocalIP")).Text = localIP;

        }
        public void ConfigAll()
        {
            Init();
            ConfigTheXML();
            ConfigTheIni();
            ConfigTheProperties();
        }
    }
}
