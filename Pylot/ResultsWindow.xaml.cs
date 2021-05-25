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
using OxyPlot;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Pylot
{
    /// <summary>
    /// Логика взаимодействия для ResultsWindow.xaml
    /// </summary>
    public partial class ResultsWindow : Window
    {
        bool isSaved = false;
        ResultsWorker rw = new ResultsWorker();
        public ResultsWindow()
        {
                InitializeComponent();
            this.DataContext = new MainViewModel(); //Подрубаем инфу для графиков
            Before40LessStrategy.Content = rw.Before40LessStrategyPercent;
            Before40GreaterStrategy.Content = rw.Before40GreaterStrategyPercent;
            After40LessStrategy.Content = rw.After40LessStrategyPercent;
            After40GreaterStrategy.Content = rw.After40GreaterStrategyPercent;
            VPhase.Content = rw.VPhaseTimeA40; //вывод времени фаз восстановления
            SPhase.Content = rw.SPhaseTimeA40; //вывод времени фаз стабилизации
            RPhase.Content = rw.RPhaseTimeA40; //вывод времени фаз распада
            RamLabel.Content = rw.RamRes();


            FinalStrategy.Content = rw.StratAfter40Res();
            Strategy3540.Content = rw.Strat3540Res();
            VSRLabel.Content = rw.FinPhaseRes();
            MarkLabel.Content = rw.FinalMark();

            NumberLabel.Content = Results.Number.ToString();
            NameLabel.Content = Results.Name.ToString();
            DateLabel.Content = Results.Date.ToString();
        }

        private void ExitResultsButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.isSaved)
            {
                this.DialogResult = false;
            }
            else
            {
                NotSavedWindow notSavedWindow = new NotSavedWindow();
                notSavedWindow.Owner = this;
                if (notSavedWindow.ShowDialog() ?? false)
                    this.DialogResult = false;
            }

        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.isSaved)
            {
                this.DialogResult = true;
            }
            else
            {
                NotSavedWindow notSavedWindow = new NotSavedWindow();
                notSavedWindow.Owner = this;
                if (notSavedWindow.ShowDialog() ?? false)
                    this.DialogResult = true;
            }
        }

        private void SaveResultsButton_Click(object sender, RoutedEventArgs e)
        {
            var ew = new ExcelWorker();
            MakeScreens();
            try
            {
                ew.SaveResults(Environment.CurrentDirectory + "\\Протоколы", rw);    //создаём новый файл с отчетом по указанному пути
                ew.SaveInMarks(Environment.CurrentDirectory + "\\Сводный протокол.xlsx", rw);  //дозаписываем результаты в общий файл
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["PrevNumber"].Value = Results.Number.ToString();
                config.Save(ConfigurationSaveMode.Modified);
                SaveResultsButton.IsEnabled = false;
                this.isSaved = true;
            }
            catch(IOException)
            {
                MessageBox.Show("Ошибка записи, закройте сводный протокол","Ошибка");
            }
                        
        }

        private void MakeScreens()
        {
            if (!Directory.Exists(Environment.CurrentDirectory + "\\Temp"))
            {
                Directory.CreateDirectory(Environment.CurrentDirectory + "\\Temp");
            }

            RenderTargetBitmap bmp = new RenderTargetBitmap((int)this.ActualWidth, (int)this.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            bmp.Render(this);

            //</ get Screenshot of Element >
            //< create Encoder >
            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            //< save >
            FileStream fs = new FileStream(Environment.CurrentDirectory + "\\Temp\\ScreenShot.png", FileMode.Create);
            encoder.Save(fs);
            fs.Close();
        }
    }
}

