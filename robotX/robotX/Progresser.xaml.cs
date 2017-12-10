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
    /// Progresser.xaml 的交互逻辑
    /// </summary>
    public partial class Progresser : Window
    {
        int progress;
        public int Progress
        {
            get { return progress; }
            set {
                this.progress = value;
                this.Bar.Value = value;
            }
        }
        string fileName;
        public string FileName
        {
            get { return fileName; }
            set
            {
                this.fileName = value;
                this.FileObj.Text = value;
            }
        }
        public Progresser()
        {
            InitializeComponent();
        }

        private void ProgressBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}
