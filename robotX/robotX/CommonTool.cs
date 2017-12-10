using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
namespace robotX
{
    class CommonTool
    {
        public const string UrlReg = @"^[a-zA-Z]:(((\\(?! )[^/:*?<>\""|\\]+)+\\?)|(\\)?)\s*$";
        public const string IPReg = @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        public const string FTPReg = "^ ((.+):(.+)@)?" + IPReg + "(:[0-9]+)?$";
        public const string PortReg = "^[1-9]{1}[0-9]*$";
        public const string AllNumReg = "^\\d+$";
        static private string localIP = "未接入局域网";
        static public string LocalIP {
            get {
                IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (IPAddress ip in host.AddressList)
                {
                    if (ip.AddressFamily.ToString() == "InterNetwork")
                    {
                        if (ip.ToString().StartsWith("192.168") ||  ip.ToString().StartsWith("172.") || ip.ToString().StartsWith("10."))//内网IP有3种：第一种10.0.0.0～10.255.255.255，第二种172.16.0.0～172.31.255.255，第三种192.168.0.0～192.168.255.255
                        {
                            localIP = ip.ToString();
                        break;
                        }

                    }
                }

                return localIP; }
            set { localIP = value; }
        }
        static public string CaptureUpperCase(string CnStr)
        {

            string strTemp = "";

            int iLen = CnStr.Length;

            int i = 0;

            for (i = 0; i <= iLen - 1; i++)
            {
                //遍历每个汉字将处理获得的首字符拼接
                strTemp += GetCharSpellCode(CnStr.Substring(i, 1)).ToLower();

            }

            return strTemp;

        }

        private static string[] folderList = { "HisWebServices_Clound", "SupLab_Clound", "SvMatrixServer" };

        public static void CopyDir(string fromDir, string toDir)
        {
            string[] files = Directory.GetFiles(fromDir);


            if (!Directory.Exists(fromDir))
            {
                return;
            }



            if (!Directory.Exists(toDir))
            {
                Directory.CreateDirectory(toDir);
            }
            //复制文件
            foreach (string file in files)
            {
                string fileName = Path.GetFileName(file);
                string toFileName = Path.Combine(toDir, fileName);
                if (File.Exists(toFileName))
                {
                    File.Delete(toFileName);
                File.Copy(file, toFileName);
                }
                else
                {
                    try
                    {
                File.Copy(file, toFileName);
                    }
                    catch(System.IO.IOException e)
                    {
                        
                    }
                }
                //改变进度条状态
            }
            //处理文件夹
            string[] fromDirs = Directory.GetDirectories(fromDir);
            foreach (string dir in fromDirs)
            {
                string dirName = Path.GetFileName(dir);
                string toDirName = Path.Combine(toDir, dirName);
                CopyDir(dir, toDirName);//递归
            }
           
        }

        //处理单个字符
        private static string GetCharSpellCode(string CnChar)
        {

            long iCnChar;

            byte[] ZW = System.Text.Encoding.Default.GetBytes(CnChar);

            //如果是字母，则直接返回

            if (ZW.Length == 1)
            {

                return CnChar.ToLower();

            }

            else
            {

                // get the array of byte from the single char

                int i1 = (short)(ZW[0]);

                int i2 = (short)(ZW[1]);

                iCnChar = i1 * 256 + i2;

            }

            // iCnChar match the constant

            if ((iCnChar >= 45217) && (iCnChar <= 45252))
            {

                return "A";

            }

            else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            {

                return "B";

            }
            else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            {

                return "C";

            }
            else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            {

                return "D";

            }
            else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            {

                return "E";

            }
            else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            {

                return "F";

            }
            else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            {

                return "G";

            }
            else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            {

                return "H";

            }
            else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            {

                return "J";

            }
            else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            {

                return "K";

            }
            else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            {

                return "L";

            }
            else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            {

                return "M";

            }
            else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            {

                return "N";

            }
            else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            {

                return "O";

            }
            else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            {

                return "P";

            }
            else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            {

                return "Q";

            }
            else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            {

                return "R";

            }
            else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            {

                return "S";

            }
            else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            {

                return "T";

            }
            else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            {

                return "W";

            }
            else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            {

                return "X";

            }
            else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            {

                return "Y";

            }
            else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            {

                return "Z";

            }
            else

                return ("?");

        }
    }
}
