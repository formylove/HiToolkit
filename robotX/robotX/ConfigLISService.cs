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
using System.Data;

namespace robotX
{
    class ConfigLISService
    {
        private Window w;
        private DBAcesser acc = new DBAcesser();
        const string relUrl = @"./";
        public string[,] args4XML = {{"HisWebServices_Clound/WEB-INF/classes/generatorConfig.xml", "//jdbcConnection", "/@connectionURL", "/@userId", "/@password" },
                              { "SupLab_Clound/WEB-INF/classes/generatorConfig.xml", "//jdbcConnection","/@connectionURL", "/@userId", "/@password"  },
                              { "SupLab_Clound/WEB-INF/classes/applicationContext-dataSource.xml", "//*","[@name='jdbcUrl']/@value", "[@name='user']/@value", "[@name='password']/@value" } };
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
        public ConfigLISService(Window w)
        {
            this.w = w;
        }
        public void Init()
        {
            ipHis = ((TextBox)w.FindName("HisIP")).Text;
            ipLis = ((TextBox)w.FindName("LisIP")).Text;
            tomcatPort = ((TextBox)w.FindName("TomcatPort")).Text;
            ipMatrix = ((TextBox)w.FindName("MatrixIP")).Text;
            portMatrix = ((TextBox)w.FindName("MatrixPort")).Text;
            unitsCode = ((TextBox)w.FindName("UnitsCode")).Text;
            hisORCLInstance = ((TextBox)w.FindName("HisORCLInstance")).Text;
            lisORCLInstance = ((TextBox)w.FindName("LisORCLInstance")).Text;
            hisDBUser = ((TextBox)w.FindName("HisDBUser")).Text;
            lisDBUser = ((TextBox)w.FindName("LisDBUser")).Text;
            hisDBPSW = ((TextBox)w.FindName("HisDBPSW")).Text;
            lisDBPSW = ((TextBox)w.FindName("LisDBPSW")).Text;
            isNewHIS = (bool)(((CheckBox)w.FindName("IsNewHIS")).IsChecked);

        }
        Dictionary<string, string> defaultConfig = new Dictionary<string, string>(){
            { "HisDBUser", "suphiv3"}, { "HisDBPSW", "suphiv3" }, { "HisORCLInstance", "orcl" },
            { "LisIP", "" }, { "LisDBUser", "suplab" }, { "LisDBPSW", "suplab" }, { "LisORCLInstance", "orcl" },
            { "MatrixIP", "" }, { "MatrixPort", "2004" }, { "TomcatPort", "8080" },
            { "UnitsCode", "" }, { "HisIP", "" }, { "IsNewHIS", "True" }};
        public void FullFill()
        {
        CheckBox box = ((CheckBox)w.FindName("IsNewHIS"));
            DataTable dt;
            int ID = (int)((ComboBox)w.FindName("UnitLis")).SelectedValue;
                dt = new DataTable();
                DataRow dr = dt.NewRow();

            if (ID == 0)
            {
                defaultConfig["LisIP"] = CommonTool.LocalIP;
                defaultConfig["MatrixIP"] = CommonTool.LocalIP;
                foreach (string key in defaultConfig.Keys)
                {
                    dt.Columns.Add(key, Type.GetType("System.String"));
                }
                foreach(string key in defaultConfig.Keys)
                {
                dr[key] = defaultConfig[key];
                }
                dt.Rows.Add(dr);
            }
            else
            {
                string sql = String.Format( "select * from lisconfig where id = {0}",ID);
                dt = acc.FetchDataSet(sql);
            }
            dr = dt.Rows[0];
            foreach (string key in defaultConfig.Keys)
            {
                if (!key.Equals("IsNewHIS"))
                {
                ((TextBox)w.FindName(key)).Text = dr[key] != null? dr[key].ToString():"";
                }
            }
            box.IsChecked = Boolean.Parse(dr["IsNewHIS"].ToString());
        }

        public void SaveConfig()
        {
            CheckBox box = ((CheckBox)w.FindName("IsNewHIS"));
            string sql = "insert into lisconfig(stamp,UNITNAME{0}) values(now(),'????'{1})";
            string columms = "";
            string values = "";
            foreach (string key in defaultConfig.Keys)
            {
                if (!key.Equals("IsNewHIS"))
                {
                    string temp = ((TextBox)w.FindName(key)).Text;

                    if (!String.IsNullOrWhiteSpace(temp))
                    {

                    columms += "," + key;
                    values += ",'" + temp.Trim().Replace(" ","")+"'";
                    }
                }
            }
            columms += "," + "IsNewHIS";
            if ((bool)box.IsChecked)//由于 bool? 可以为 null 值，所以 if(null) 是无法作为 true / false 判断的，当然报错
            {
                values += "," + "'True'";
            }
            else
            {
                values += "," + "'False'";
            }
           sql = String.Format(sql, columms, values);
            ConfigSaver cs = new ConfigSaver();
            cs.Sql = sql;
            cs.Show();
            FetchBoxList();


        }
        public void FetchBoxList()
        {
            ComboBox box = (ComboBox)w.FindName("UnitLis");
            string sql = "SELECT t.UNITNAME,t.ID from lisconfig t order by stamp desc";
            System.Data.DataTable dt = acc.FetchDataSet(sql);
            System.Data.DataRow dr = dt.NewRow();
            dr["ID"] = 0;
            dr["UNITNAME"] = "自动填表";
            dt.Rows.Add(dr);
            System.Data.DataView dv = dt.DefaultView;
            box.ItemsSource = dv;
            box.DisplayMemberPath = "UNITNAME";
            box.SelectedValuePath = "ID";
            box.SelectedIndex = box.Items.Count - 1;
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
            bool rightIP = Regex.IsMatch(ipLis, CommonTool.IPReg);


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
        public void ConfigAll()
        {
            Init();
            ConfigTheXML();
            ConfigTheIni();
            ConfigTheProperties();
        }

    }
}
