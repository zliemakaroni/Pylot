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
using System.Configuration;
using System.Text.RegularExpressions;

namespace Pylot
{
    /// <summary>
    /// Логика взаимодействия для RegWindow.xaml
    /// </summary>
    public partial class RegWindow : Window
    {
        public RegWindow()
        {
            InitializeComponent();
            int valid;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (int.TryParse(config.AppSettings.Settings["PrevNumber"].Value, out valid))
                Results.Number = valid + 1;
            NumberBox.Text = Results.Number.ToString();
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            if(NumberBox.Text.Length != 0)
                Results.Number = int.Parse(NumberBox.Text);
            Results.Name = NameBox.Text;
            int age = DateTime.Today.Year - date.SelectedDate.Value.Year;       //Вычисляем кол-во полных лет
            if (date.SelectedDate.Value > DateTime.Today.AddYears(-age)) age--; //Отнимаем лишний год если есть
            Results.Age = age.ToString();

            HintWindow hintWindow = new HintWindow();
            hintWindow.Owner = this.Owner;
            if (hintWindow.ShowDialog() ?? true)
                this.DialogResult = true;
            else
                this.DialogResult = false;
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
