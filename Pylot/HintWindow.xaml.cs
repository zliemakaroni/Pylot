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
using System.Windows.Shapes;

namespace Pylot
{
    /// <summary>
    /// Логика взаимодействия для HintWindow.xaml
    /// </summary>
    public partial class HintWindow : Window
    {
        public HintWindow()
        {
            InitializeComponent();
        }

        private void StartProbeButton_Click(object sender, RoutedEventArgs e)   //Старт пробы
        {
            this.DialogResult = true;
            this.Close();
        }

        private void StartTestButton_Click(object sender, RoutedEventArgs e)    //Старт теста
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
