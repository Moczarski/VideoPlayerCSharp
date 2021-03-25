using Microsoft.Win32;
using PlaylistElement;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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
using System.Windows.Threading;

namespace Zadanie6_Mateusz_Moczarski
{
    public partial class MainWindow : Window
    {
        private bool playerOrPlaylist = true;
        private DispatcherTimer dispatcherTimer;
        private List<VideoListItem> playlist;
        private TimeSpan timeVar;
        private TimeSpan timeNow;
        private bool random = false;
        private bool loop = false;
        private bool loopPlaylist = false;

        public MainWindow()
        {
            InitializeComponent();

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(TimerEverySecond);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();

            playlist = new List<VideoListItem>();
        }

        private void TimerEverySecond(object sender, EventArgs e)
        {
            if (GetMediaState(windowPlayer) == MediaState.Play)
            {
                timeSlider.Value = windowPlayer.Position.TotalMilliseconds;
                timeVar = windowPlayer.Position;
                playingTime.Text = string.Format("{0:D2}:{1:D2}", (int)timeVar.TotalMinutes, timeVar.Seconds);
                timeVar = timeNow - windowPlayer.Position;
                leftTime.Text = string.Format("{0:D2}:{1:D2}", (int)timeVar.TotalMinutes, timeVar.Seconds);
            }
        }

        private MediaState GetMediaState(MediaElement myMedia)
        {
            FieldInfo helper = typeof(MediaElement).GetField("_helper", BindingFlags.NonPublic | BindingFlags.Instance);
            object helperObject = helper.GetValue(myMedia);
            FieldInfo stateField = helperObject.GetType().GetField("_currentState", BindingFlags.NonPublic | BindingFlags.Instance);
            MediaState state = (MediaState)stateField.GetValue(helperObject);
            return state;
        }

        private void OpenFile(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFile = new OpenFileDialog();
            if (openFile.ShowDialog() == true)
            {
                windowPlayer.Source = new Uri(openFile.FileName);

                playlist.Add(new VideoListItem(openFile.FileName));
                refreshThePlaylist();
                windowPlaylist.SelectedIndex = windowPlaylist.Items.Count - 1;
            }
        }

        private void OpenPlaylist(object sender, RoutedEventArgs e)
        {
            if (playerOrPlaylist)
            {
                windowPlayer.Visibility = Visibility.Collapsed;
                windowPlaylist.Visibility = Visibility.Visible;
                menuUp.Visibility = Visibility.Visible;
                menuDown.Visibility = Visibility.Visible;
                menuDelete.Visibility = Visibility.Visible;
                goBackPlaylist.Header = "Wróc do playera";
                playerOrPlaylist = false;
            }
            else
            {
                windowPlayer.Visibility = Visibility.Visible;
                windowPlaylist.Visibility = Visibility.Collapsed;
                menuUp.Visibility = Visibility.Collapsed;
                menuDown.Visibility = Visibility.Collapsed;
                menuDelete.Visibility = Visibility.Collapsed;
                goBackPlaylist.Header = "Przejdź do playlisty";
                playerOrPlaylist = true;
            }
        }

        private void refreshThePlaylist()
        {
            var temp = windowPlaylist.SelectedItem;
            windowPlaylist.ItemsSource = null;
            windowPlaylist.ItemsSource = playlist;
            windowPlaylist.SelectedItem = temp;

            totalTimePlaylist.Text = "Playlist duration time: " + VideoListItem.TotalDuration(playlist);
        }

        private void Start(object sender, RoutedEventArgs e)
        {
            if (windowPlaylist.Items.Count > 0)
            {
                if (windowPlayer.Source == null)
                {
                    windowPlayer.Source = new Uri(windowPlaylist.Items.GetItemAt(0).ToString());
                }

                if (GetMediaState(windowPlayer) == MediaState.Play)
                {
                    windowPlayer.Pause();
                }
                else
                {
                    windowPlayer.Play();
                }
            }
        }

        private void Stop(object sender, RoutedEventArgs e)
        {
            windowPlayer.Stop();
        }

        private void Previous(object sender, RoutedEventArgs e)
        {
            if (windowPlaylist.SelectedIndex == 0)
            {
                windowPlaylist.SelectedIndex = windowPlaylist.Items.Count - 1;
            }
            else
            {
                windowPlaylist.SelectedIndex--;
            }
            if (random)
            {
                Random rnd = new Random();
                int rndNum;
                rndNum = rnd.Next(0, windowPlaylist.Items.Count);
                windowPlaylist.SelectedIndex = rndNum;
            }

            windowPlayer.Stop();
            windowPlayer.Source = new Uri(windowPlaylist.SelectedItem.ToString());
            windowPlayer.Play();
        }

        private void Next(object sender, RoutedEventArgs e)
        {
            if (windowPlaylist.SelectedIndex == windowPlaylist.Items.Count - 1)
            {
                windowPlaylist.SelectedIndex = 0;
            }
            else
            {
                windowPlaylist.SelectedIndex++;
            }
            if (random)
            {
                Random rnd = new Random();
                int rndNum;
                rndNum = rnd.Next(0, windowPlaylist.Items.Count);
                windowPlaylist.SelectedIndex = rndNum;
            }

            windowPlayer.Stop();
            windowPlayer.Source = new Uri(windowPlaylist.SelectedItem.ToString());
            windowPlayer.Play();
        }

        private void Loop(object sender, RoutedEventArgs e)
        {
            if (loop)
            {
                loop = false;
                buttonLoop.Background = Brushes.Transparent;
            }
            else
            {
                loopPlaylist = false;
                random = false;
                loop = true;
                buttonLoop.Background = Brushes.Aqua;
                buttonLoopPlaylist.Background = Brushes.Transparent;
                buttonRadomly.Background = Brushes.Transparent;
            }
        }

        private void Random(object sender, RoutedEventArgs e)
        {
            if (random)
            {
                random = false;
                buttonRadomly.Background = Brushes.Transparent;
            }
            else
            {
                random = true;
                loopPlaylist = false;
                loop = false;
                buttonRadomly.Background = Brushes.Aqua;
                buttonLoop.Background = Brushes.Transparent;
                buttonLoopPlaylist.Background = Brushes.Transparent;
            }
        }

        private void LoopPlaylist(object sender, RoutedEventArgs e)
        {
            if (loopPlaylist)
            {
                loopPlaylist = false;
                buttonLoopPlaylist.Background = Brushes.Transparent;
            }
            else
            {
                loopPlaylist = true;
                random = false;
                loop = false;
                buttonLoop.Background = Brushes.Transparent;
                buttonLoopPlaylist.Background = Brushes.Aqua;
                buttonRadomly.Background = Brushes.Transparent;
            }
        }

        private void SetMediaTime(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                int sliderValue = (int)timeSlider.Value;
                TimeSpan ts = new TimeSpan(0, 0, 0, 0, sliderValue);
                windowPlayer.Position = ts;
            }
        }

        private void MediaOpen(object sender, RoutedEventArgs e)
        {
            timeNow = windowPlayer.NaturalDuration.TimeSpan;
            timeSlider.Maximum = windowPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            timeSlider.Value = 0;
        }

        private void deleteItem(object sender, RoutedEventArgs e)
        {
            if (windowPlaylist.SelectedItem != null)
            {
                playlist.RemoveAt(windowPlaylist.SelectedIndex);
                refreshThePlaylist();
            }
        }

        private void takeDown(object sender, RoutedEventArgs e)
        {
            if (windowPlaylist.SelectedIndex < windowPlaylist.Items.Count - 1)
            {
                var element = playlist[windowPlaylist.SelectedIndex];
                playlist.RemoveAt(windowPlaylist.SelectedIndex);
                playlist.Insert(windowPlaylist.SelectedIndex + 1, element);
                refreshThePlaylist();
            }
        }

        private void takeUp(object sender, RoutedEventArgs e)
        {
            if (windowPlaylist.SelectedIndex > 0)
            {
                var element = playlist[windowPlaylist.SelectedIndex];
                playlist.RemoveAt(windowPlaylist.SelectedIndex);
                playlist.Insert(windowPlaylist.SelectedIndex - 1, element);
                refreshThePlaylist();
            }
        }

        private void IfFinish(object sender, RoutedEventArgs e)
        {
            windowPlayer.Stop();
            if (loop)
            {
                Start(sender, e);
            }
            else if (loopPlaylist || random)
            {
                Next(sender, e);
            }
        }

        private void savePlaylist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ".txt";
                sfd.Filter = "Text documents (.txt)|*.txt";
                string result = String.Join(Environment.NewLine, playlist);
                if (sfd.ShowDialog() == true)
                {
                    File.WriteAllText(sfd.FileName, result);
                }
                MessageBox.Show("Zapisano!");
            }
            catch
            {

            }
        }

        private void openPlaylist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                playlist.Clear();
                OpenFileDialog sfd = new OpenFileDialog();
                sfd.DefaultExt = ".txt";
                sfd.Filter = "Text documents (.txt)|*.txt";
                if (sfd.ShowDialog() == true)
                {
                    foreach (var myFile in File.ReadAllLines(sfd.FileName))
                    {
                        playlist.Add(new VideoListItem(myFile));
                    }
                }
                refreshThePlaylist();
                MessageBox.Show("Otwarto!");
            }
            catch
            {

            }
        }
    }
}
