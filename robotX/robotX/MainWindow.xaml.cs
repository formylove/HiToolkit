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

            //p.Close();
            lisService.SetMetaInfo();
            cmdService.SetSysMeta();
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

        private void Button_Click_2(object sender, RoutedEventArgs e)
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
            this.Popup.StaysOpen = true;
            this.Popup.IsOpen = true;
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
           
            this.Popup.StaysOpen = true;
            this.Popup.IsOpen = true;
        }
        void HidePopUp(object sender, object e)
        {
            this.Result.Text += this.Popup.IsOpen;
            this.Popup.IsOpen = false;

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

        }
    }
}
