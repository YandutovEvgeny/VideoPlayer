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

namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        bool isPlaying = false; //Видео запущено
        public MainWindow()
        {
            InitializeComponent();
            Player.Volume = 0.5;    //0 - отсутствие звука, 1 - 100% звука
        }

        private void PlayBTN_Click(object sender, RoutedEventArgs e)
        {
            if(isPlaying)
            {
                Player.Pause();
                PlayBTN.Content = "Play";
            }
            else
            {
                //PlayBTN.Background = Brushes.Bisque; //new SolidColorBrush(Color.FromRgb(123,45,12));
                Player.Play();
                PlayBTN.Content = "Pause";
            }
            isPlaying = !isPlaying;
        }

        private void StopBTN_Click(object sender, RoutedEventArgs e)
        {
            Player.Stop();
            isPlaying = false;
            PlayBTN.Content = "Play";
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

        }

        private void Player_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            //Delta показывает куда мы крутим колёсико мышки
            Volume.Value += e.Delta / 30;
        }
    }
}
