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

namespace Pylot
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private double X = 0;
        private double Y = 0;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Canvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                Canvas.SetTop(pointer, Canvas.GetTop(pointer) + 10);
            }
            else if (e.Key == Key.Up)
            {
                Canvas.SetTop(pointer, Canvas.GetTop(pointer) - 10);
            }
            else if (e.Key == Key.Left)
            {
                Canvas.SetLeft(pointer, Canvas.GetLeft(pointer) - 10);
            }
            else if (e.Key == Key.Right)
            {
                Canvas.SetLeft(pointer, Canvas.GetLeft(pointer) + 10);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double left = (WorkCanvas.ActualWidth - target.ActualWidth) / 2;
            Canvas.SetLeft(target, left);

            double top = (WorkCanvas.ActualHeight - target.ActualHeight) / 2;
            Canvas.SetTop(target, top);
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += timer_Tick;
            timer.Start();
            ButtonStart.IsEnabled = false;
        }
        void timer_Tick(object sender, EventArgs e)
        {
            this.Y = ((WorkCanvas.ActualHeight - target.ActualHeight) / 2) * Math.Sin( Math.PI * this.X);
            this.X += 0.1;
            Move(target, Y);
        }
        private void Move(FrameworkElement fe, double y)
        {
            double topInit = Canvas.GetTop(fe);

            var top = new DoubleAnimation
            {
                From = topInit,
                To = ((WorkCanvas.ActualHeight - target.ActualHeight) / 2)+ y,
                Duration = new Duration(TimeSpan.FromMilliseconds(100))
            };

            fe.BeginAnimation(Canvas.TopProperty, top);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double left = (WorkCanvas.ActualWidth - target.ActualWidth) / 2;
            Canvas.SetLeft(target, left);

            if (ButtonStart.IsEnabled)
            {
                double top = (WorkCanvas.ActualHeight - target.ActualHeight) / 2;
                Canvas.SetTop(target, top);
            }
        }
    }
}
