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
using System.Text.RegularExpressions;
using System.Configuration;

namespace Pylot
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SettingsWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SensivitySlider.Value = Results.sensivity;
            StratSlider.Value = Results.stratborder;
            TargetSpeedSlider.Value = Results.targetspeed;
            OptRamTime.Text = Results.optRamTime.ToString();
        }

        private void ConfimButton_Click(object sender, RoutedEventArgs e)
        {
            Results.sensivity = SensivitySlider.Value;
            Results.stratborder = StratSlider.Value;
            Results.targetspeed = TargetSpeedSlider.Value;
            if (OptRamTime.Text.Length != 0)
                Results.optRamTime = int.Parse(OptRamTime.Text);

            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["Sensivity"].Value = Results.sensivity.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            this.Close();
        }

        private void CanselButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
