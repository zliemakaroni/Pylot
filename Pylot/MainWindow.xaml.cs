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
using System.Windows.Media.Animation;
using System.Windows.Threading;
using SharpDX.DirectInput;
using System.Configuration;

namespace Pylot
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private double Xt = 0;   // X мишени
        private double Yt = 0;   // Y мишени
        private double Yp = 0;   // Y указателя
        private double targetpath = 0;   // расстояние, пройденное мишенью
        private double pointerpath = 0;   // расстояние, пройденное указателем
        DispatcherTimer timer = new DispatcherTimer();  // таймер для движения
        DispatcherTimer joystickTimer = new DispatcherTimer();  //таймер для подключения джойстика
        private double pointerSpeed = 0; // Скорость для указателя
        private MyJoystick myJoystick = new MyJoystick();

        #region Window actions
        //Точка входа
        public MainWindow()
        {
            InitializeComponent();
        }
        //Действия при загрузке окна
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            double valid;
            if(double.TryParse(config.AppSettings.Settings["Sensivity"].Value, out valid))
                Results.sensivity = valid;

            Results.datas.Add(new data { x = 0, yt = 0, yp = 0, deltapath = 0 });
            timer.Interval = TimeSpan.FromSeconds(Results.timing);  //Устанавливаем интервал таймера
            progressBar.Width = this.ActualWidth;   //Выставляем полоску прогресса по ширине окна
            Results.Height = WorkCanvas.ActualHeight - target.ActualHeight; //Вычислияем значения высоты поля с меткой и визиром
            Results.halfHeight = Results.Height/2;                          //Чтобы не вычислять их каждый раз потом
            Reset();

            RegWindow regWindow = new RegWindow();  //Окно регистрации
            regWindow.Owner = this;  
            if (!regWindow.ShowDialog() ?? true)    // ?? нужен т.к. ShowDialog() возвращает bool?
            {
                ToMainTest();
            }

            joystickTimer.Interval = TimeSpan.FromSeconds(0.1);
            joystickTimer.Tick += joystickTimer_Tick_findJoystick;  //начинаем искать джойстик, пока его не найдём, ничего нельзя нажать
            joystickTimer.Start();
        }
        private void StartProbeButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Tick += timer_Tick_Move_target;
            timer.Tick += timer_Tick_Move_pointer;
            timer.Tick += timer_Tick_Check_60_Seconds;
            timer.Start();
            StartProbeButton.IsEnabled = false;
            StopProbeButton.IsEnabled = true;
        }   //Старт пробного эксперимента
        private void StopProbeButton_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            timer.Tick -= timer_Tick_Move_target;
            timer.Tick -= timer_Tick_Move_pointer;
            timer.Tick -= timer_Tick_Check_60_Seconds;
            StartProbeButton.IsEnabled = true;
            StopProbeButton.IsEnabled = false;
        }   //Остановка Пробного эксперимента
        private void ToTestButton_Click(object sender, RoutedEventArgs e)
        {
            ToMainTest();
        }   //Переход к основному тесту
        private void StartTest_Click(object sender, RoutedEventArgs e)
        {
            timer.Tick += timer_Tick_Move_target;
            timer.Tick += timer_Tick_Move_pointer;
            timer.Tick += timer_Tick_Save_data;
            timer.Tick += timer_Tick_Check_40_Seconds;
            timer.Start();
            StartTestButton.IsEnabled = false;
            SettingsButton.IsEnabled = false;
            Results.Date = DateTime.Now;

        }               //Старт основного теста
        private void ResultButton_Click(object sender, RoutedEventArgs e)
        {
            ResultsWindow resultsWindow = new ResultsWindow();
            resultsWindow.Owner = this;
            bool resres = resultsWindow.ShowDialog() ?? false;
            if (resres)                                                          //Если пользователь захотел ещё раз пройти тест, 
            {                                                                   //то подготавливаем всё для нового испытания.
                StartProbeButton.IsEnabled = true;
                StartProbeButton.Visibility = Visibility.Visible;
                StopProbeButton.IsEnabled = false;
                StopProbeButton.Visibility = Visibility.Visible;
                StartTestButton.Visibility = Visibility.Hidden;
                StartTestButton.IsEnabled = false;
                ToTestButton.IsEnabled = true;
                ToTestButton.Visibility = Visibility.Visible;
                progressBar.Maximum = 60;
                progressBar.Value = 0;
                timer.Tick -= timer_Tick_Move_pointer;
                timer.Tick -= timer_Tick_Move_target;
                timer.Tick -= timer_Tick_Check_60_Seconds;
                ResultButton.IsEnabled = false;
                SettingsButton.IsEnabled = true;
                target.Visibility = Visibility.Visible;
                pointer.Visibility = Visibility.Visible;
                Results.datas.Clear();
                Results.datas.Add(new data { x = 0, yt = 0, yp = 0, deltapath = 0 });
                Reset();
                RegWindow regWindow = new RegWindow();  //Окно регистрации
                regWindow.Owner = this;
                if (!regWindow.ShowDialog() ?? true)    // ?? нужен т.к. ShowDialog() возвращает bool?
                {
                    ToMainTest();
                }
            }
            else
            {
                System.Windows.Application.Current.Shutdown();
            }
        }        //Переход к результатам теста
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }           
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.Owner = this;
            settingsWindow.ShowDialog();
        }   
        #endregion

        #region Handle timer events
        //Обработчик для поиска джойстика
        void joystickTimer_Tick_findJoystick(object sender, EventArgs e)
        {
            if (!myJoystick.FindJoystick())
            {
                StatusLabel.Content = "Джойстик не обнаружен. Подключите джойстик";
            }
            else
            {
                StatusLabel.Content = "Обнаружен джойстик " + myJoystick.GetInformation() + ".";
                joystickTimer.Tick -= joystickTimer_Tick_findJoystick;
                joystickTimer.Stop();
                StartProbeButton.IsEnabled = true;
                ToTestButton.IsEnabled = true;
            }
        }
        //Обработчик для сохранения X и Y точки-визира и точки-метки
        void timer_Tick_Save_data(object sender, EventArgs e)
        {
            Results.datas.Add(new data { x = Xt, yt = Canvas.GetTop(target), yp = Canvas.GetTop(pointer), deltapath = targetpath - pointerpath });
        }   //Сохраняет x и y
        //Обработчик, который ждёт 40 секунд
        void timer_Tick_Check_40_Seconds(object sender, EventArgs e)
        {
            progressBar.Value += Results.timing;
            if (this.Xt > Results.TimeToDissapeare)
            {
                target.Visibility = Visibility.Hidden;
                if (Results.Name != "testtesttest")
                    pointer.Visibility = Visibility.Hidden;
                timer.Tick -= timer_Tick_Check_40_Seconds;
                timer.Tick += timer_Tick_Check_Phase;
            }
        }
        //Обработчик, который ждёт 60 секунд
        void timer_Tick_Check_60_Seconds(object sender, EventArgs e)
        {
            progressBar.Value += Results.timing;
            if (this.Xt > 60)
            {
                timer.Tick -= timer_Tick_Check_60_Seconds;
                ToMainTest();
            }
        }
        //Обработчкик, который сравнивает положение точки-метки и точки-визира. Включается после 40 секунд
        void timer_Tick_Check_Phase(object sender, EventArgs e)
        {
            if (Math.Abs(targetpath - pointerpath) > Results.Height * 2) //Если метка и визир оказались в противофазе
            {
                timer.Tick -= timer_Tick_Move_target;
                timer.Tick -= timer_Tick_Move_pointer;
                timer.Tick -= timer_Tick_Save_data;
                timer.Tick -= timer_Tick_Check_Phase;
                timer.Stop();
                ResultButton.IsEnabled = true;
                MessageBox.Show("Нажмите результат для получения результатов тестирования", "Тестирование завершёно", MessageBoxButton.OK);
            }

        }
        //Обработчик для прередвижения точки-метки
        void timer_Tick_Move_target(object sender, EventArgs e)
        {
            this.Yt = Results.halfHeight * Math.Sin(Results.targetspeed * Math.PI * this.Xt - Math.PI / 2) + Results.halfHeight; //Y метки меняется по синусоиде
            this.Xt += Results.timing;
            MoveTo(target, WorkCanvas, Yt);
            targetpath += Math.Abs(this.Yt - Results.datas.Last().yt); 
        }
        //Обработчки для передвижения точки-визира
        void timer_Tick_Move_pointer(object sender, EventArgs e)
        {
            this.pointerSpeed = myJoystick.NewPointerSpeed(this.pointerSpeed);
            this.Yp = Canvas.GetTop(pointer) + pointerSpeed * Results.timing;  //Вычислем новый Y визира
            if (this.Yp < 0)                               //Обработка столкновения с верхней границей
                this.Yp = 0;
            else if (this.Yp > Results.Height)             //Обработка столкновения с нижней границей
                this.Yp = Results.Height;
            MoveTo(pointer, WorkCanvas, Yp);
            pointerpath += Math.Abs(this.Yp - Results.datas.Last().yp);

        }
        #endregion

        //Центрирует элемент сверху по центру в канвасе
        private void Center(FrameworkElement fe, Canvas canvas)
        {

            double left = (canvas.ActualWidth - fe.ActualWidth) / 2;
            Canvas.SetLeft(fe, left);
            MoveTo(fe, canvas, 0);   //После начала анимаций по другому дигать не получалось
        }
        
        //Передвинуть что, где, куда по y
        private void MoveTo(FrameworkElement fe, Canvas canvas, double y)   
        {
            double topInit = Canvas.GetTop(fe);
            var top = new DoubleAnimation
            {
                From = topInit,
                To = y,
                Duration = new Duration(TimeSpan.FromSeconds(Results.timing))
            };
            fe.BeginAnimation(Canvas.TopProperty, top);
        }
        //Сброс переменных и положения точки-метки и точки-визира
        private void Reset()
        {
            this.Xt = 0;   // X мишени
            this.Yt = -Results.halfHeight;   // Y мишени
            this.Yp = -Results.halfHeight;   // Y указателя
            this.pointerpath = 0;
            this.targetpath = 0;
            progressBar.Value = 0;
            pointerSpeed = 0;
            Center(pointer, WorkCanvas);
            Center(target, WorkCanvas);
        }
        private void ToMainTest()
        {
            StartProbeButton.IsEnabled = false;
            StartProbeButton.Visibility = Visibility.Hidden;
            StopProbeButton.IsEnabled = false;
            StopProbeButton.Visibility = Visibility.Hidden;
            StartTestButton.Visibility = Visibility.Visible;
            StartTestButton.IsEnabled = true;
            ToTestButton.IsEnabled = false;
            ToTestButton.Visibility = Visibility.Hidden;
            progressBar.Maximum = 40;
            progressBar.Value = 0;
            timer.Tick -= timer_Tick_Move_pointer;
            timer.Tick -= timer_Tick_Move_target;
            timer.Tick -= timer_Tick_Check_60_Seconds;
            Reset();
        }
    }
}
