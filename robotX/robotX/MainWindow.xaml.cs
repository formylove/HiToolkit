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
using System.Data;


namespace robotX
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private ConfigLISService lisService;
        private CmdSetService cmdService;
        
        /// <summary>
        /// 
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            lisService = new ConfigLISService(this);
            cmdService = new CmdSetService(this);
            lisService.FetchBoxList();
            cmdService.SetSysMeta();
        }

        private void Button_FullFill(object sender, RoutedEventArgs e)
        {
            lisService.FullFill();
        }

        private void Button_Save(object sender, RoutedEventArgs e)
        {
            lisService.SaveConfig();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //执行替换
                lisService.ConfigAll();
                MessageBox.Show("替换成功!");
            }
            catch
            {
               
            }
            





      }
        private void Excute(object sender, RoutedEventArgs e)
        {
            cmdService.Excute();
        }





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



        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
 
        }

        private void Paramenter_TextChanged(object sender, TextChangedEventArgs e)
        {
            string input = ((TextBox)sender).Text.Trim();
            string acronym;
            if (input != null && input.Length >0) {

                acronym = CommonTool.CaptureUpperCase(input);
                cmdService.ShuffleList(acronym);
            }
            int count = this.RecList.Items.Count;
            if (count > 0)
            {
            this.Popup.IsOpen = true;
            }
            else
            {
                this.Popup.IsOpen = false;
            }
        }

        private void TipSelected(object sender, SelectionChangedEventArgs e)
        {
            ListBox lb = (ListBox)sender;
            object o = lb.SelectedValue;
            if (o != null) {
            this.Parameter.Text = o.ToString();
            this.Popup.IsOpen = false;
            }
        }
        void ShowPopUp(object sender, object e)
        {
            cmdService.ShuffleList();
            int count = this.RecList.Items.Count;
            if (count > 0)
            {
                this.Popup.StaysOpen = true;
                this.Popup.IsOpen = true;
            }
            else
            {
                this.Popup.IsOpen = false;
            }
        }
        void HidePopUp(object sender, object e)
        {
            this.Popup.IsOpen = false;

        }
        void SavePara(object sender, object e)
        {
            string parameter = (this.Parameter.Text != null)?this.Parameter.Text.Trim().Replace(" ",""): this.Parameter.Text;
            if (parameter == null || parameter.Length == 0 )
            {
                MessageBox.Show("参数输入框为空，无法保存参数");
            }
            else if(!(Regex.IsMatch(parameter, CommonTool.IPReg) || Regex.IsMatch(parameter, "^.+\\.exe$") || Regex.IsMatch(parameter, CommonTool.UrlReg) || Regex.IsMatch(parameter, @"[0-9]+") || (this.ftp.IsSelected && Regex.IsMatch(parameter, CommonTool.FTPReg))))
            {
                MessageBox.Show("参数格式错误");
            }
            else
            {
            Saver saver = new Saver();
            saver.Parameter = parameter;
            saver.ShowDialog();
            }
        }
        void NotStaysOpen(object sender, object e)
        {
            this.Popup.StaysOpen = false;

        }

        private void CmdProcess(object sender, RoutedEventArgs e)
        {
            cmdService.Start((Button)sender);
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }



    }
}
