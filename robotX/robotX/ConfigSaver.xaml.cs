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
using System.Windows.Shapes;

namespace robotX
{
    /// <summary>
    /// ConfigSaver.xaml 的交互逻辑
    /// </summary>
    public partial class ConfigSaver : Window
    {
        string sql;
        DBAcesser dba = new DBAcesser();
        public string Sql { get; set; }
        public ConfigSaver()
        {
            InitializeComponent();
            this.UNITNAME.Focus();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string name = this.UNITNAME.Text;
            if (String.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("医院名称不能为空");
            }
            else
            {
                name = name.Trim().Replace(" ","");
                Sql = Sql.Replace("????",name);
                int resultCount = dba.Update(Sql);
                if (resultCount == 1)
                {
                MessageBox.Show("保存成功！");
                    this.Close();
                }
            }
        }
    }
}
