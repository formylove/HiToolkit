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
using System.Threading.Tasks;


namespace robotX
{
    /// <summary>
    /// Saver.xaml 的交互逻辑
    /// </summary>
    public partial class Saver : Window
    {
        string parameter;
        public string Parameter { get { return parameter; }  set { parameter = value; this.Para.Text = value; } }
        private DBAcesser accesser = new DBAcesser();
        public Saver()
        {
            InitializeComponent();
            this.Name.Focus();
            
        }
        private void Save(object sender, RoutedEventArgs e)
        {
            string name = (this.Name.Text != null) ? this.Name.Text.Trim().Replace(" ", "") : this.Name.Text.Trim();
            if (name == null || name.Length == 0)
            {
                MessageBox.Show("请输入正确的变量名称以便保存");
            }
            else
            {
                string sql = String.Format("insert into Parameter(tag,para,capital) values('{0}','{1}','{2}')", name, Parameter, CommonTool.CaptureUpperCase(name));
                int result = accesser.Update(sql);
                if (result == 1)
                {
                    MessageBox.Show("保存成功");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("保存失败！");
                    this.Close();
                }
            }
        }
}
}
