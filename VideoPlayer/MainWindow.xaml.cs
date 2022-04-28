using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        bool isPlaying = false; //Видео запущено
        Thread thread;
        bool posChange = true;  //Позиция ползунка видео
        List<string> path;  //Пути к видео
        List<string> names; //Имена видео
        bool isFullScreen = false;  //На весь экран

        public MainWindow()
        {
            InitializeComponent();
            Player.Volume = 0.5;    //0 - отсутствие звука, 1 - 100% звука
        }

        void PositionSliderMove()
        {
            while(isPlaying)
            {
                if (posChange)
                {
                    Dispatcher.Invoke(new Action(() =>
                    //Player.Position.TotalSeconds - сколько секунд прошло с момента запуска видео
                    PositionSlider.Value = Player.Position.TotalSeconds));
                    Thread.Sleep(1000);
                }
            }
        }

        private void PlayBTN_Click(object sender, RoutedEventArgs e)
        {
            if(isPlaying)
            {
                Player.Pause();
                //PlayBTN.Content = "Play";
                //UriKind.Relative - говорим, что путь к картинке относительный и находится в папке Resources
                PlayIMG.Source = new BitmapImage(new Uri("/Resources/play-icon.png", UriKind.Relative));
            }
            else
            {
                //PlayBTN.Background = Brushes.Bisque; //new SolidColorBrush(Color.FromRgb(123,45,12));
                Player.Play();
                //PlayBTN.Content = "Pause";
                PlayIMG.Source = new BitmapImage(new Uri("/Resources/pause-icon.png", UriKind.Relative));
            }
            isPlaying = !isPlaying;
            thread = new Thread(PositionSliderMove);
            thread.Start();
        }

        private void StopBTN_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            isPlaying = false;
            PlayIMG.Source = new BitmapImage(new Uri("/Resources/play-icon.png", UriKind.Relative));
        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Player.Volume = Volume.Value / 100.0;
        }

        private void Player_Loaded(object sender, RoutedEventArgs e)
        {
            //Длину времени изменяем на промежуток времени в секундах и присваиваем в переменную
            double duration = Player.NaturalDuration.TimeSpan.TotalSeconds;
            //Максимальное значение ползунка = длина видео в секундах
            PositionSlider.Maximum = duration;
            Player.Play();
            isPlaying = true;
            PlayIMG.Source = new BitmapImage(new Uri("/Resources/pause-icon.png", UriKind.Relative));
        }

        private void Player_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Delta показывает куда мы крутим колёсико мышки
            Volume.Value += e.Delta / 30;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            isPlaying = false;
        }

        private void PositionSlider_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            //PositionSlider_DragStarted не даёт перемещать ползунок самостоятельно, когда мы двигаем ползунок
            posChange = false;
        }

        private void PositionSlider_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            //PositionSlider_DragCompleted - перематывает видео, при отпускании мыши с ползунка
            Player.Position = TimeSpan.FromSeconds(PositionSlider.Value);
            posChange = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(Player.Position.TotalSeconds > 5)
            {
                Player.Position = TimeSpan.FromSeconds(Player.Position.TotalSeconds - 5);
            }
            else
            {
                Player.Position = TimeSpan.FromSeconds(0);
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //если когда мы перематываем на 5 секунд вперёд, видео не кончается
            if (Player.Position.TotalSeconds + 5 < Player.NaturalDuration.TimeSpan.TotalSeconds)
            {
                //перематываем на 5 сек
                Player.Position = TimeSpan.FromSeconds(Player.Position.TotalSeconds + 5);
            }
            else
            {
                //иначе на конец видео
                Player.Position = TimeSpan.FromSeconds(Player.NaturalDuration.TimeSpan.TotalSeconds);
            }
        }

        private void Player_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
                PlayBTN.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            if(e.ClickCount == 2)
            {
                //TODO:Увеличить видео на весь экран
                if(isFullScreen)    
                {
                    /*WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.None;*/
                    WindowState = WindowState.Normal;
                    WindowStyle = WindowStyle.None;

                }
                else
                {
                    /*WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.SingleBorderWindow;*/
                    WindowState = WindowState.Maximized;
                    WindowStyle = WindowStyle.SingleBorderWindow;
                }
                isFullScreen = !isFullScreen;
            }
        }

        private void Player_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Space)
                PlayBTN.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }

        List<string> GetFileNames(List<string> path)
        {
            List<string> temp = new List<string>();
            for (int i = 0; i < path.Count; i++)
            {
                int j = path[i].Length - 1;
                for (; path[i][j] != '\\'; j--);
                
                temp.Add(path[i].Remove(0, j + 1));
            }
            return temp;
        }

        private void OpenBTN_Click(object sender, RoutedEventArgs e)
        {
            //Нужно подключить библиотеку Microsoft.Win32
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "AVI | *.avi|MP4|*.mp4|ALL|*.*";
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                path = openFileDialog.FileNames.ToList<string>();
            }
            FilmsLB.Items.Clear();
            names = GetFileNames(path);
            for (int i = 0; i < path.Count; i++)
            {
                FilmsLB.Items.Add(names[i]);
            }
        }

        private void FilmsLB_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if(FilmsLB.SelectedIndex > -1)
            {
                Player.Source = new Uri(path[FilmsLB.SelectedIndex]);
                Player.Stop();
            }
        }

        private void First_trackBTN_Click(object sender, RoutedEventArgs e)
        {
            if (FilmsLB.SelectedIndex > 0)
            {
                FilmsLB.SelectedIndex -= 1;
            }
            else
            {
                FilmsLB.SelectedIndex = 0;
            }
            Player.Source = new Uri(path[FilmsLB.SelectedIndex]);
        }

        private void Last_trackBTN_Click(object sender, RoutedEventArgs e)
        {
            if (FilmsLB.SelectedIndex < path.Count - 1)
            {
                FilmsLB.SelectedIndex += 1;
            }
            else
            {
                FilmsLB.SelectedIndex = 0;
            }
            Player.Source = new Uri(path[FilmsLB.SelectedIndex]);
        }
    }
}
