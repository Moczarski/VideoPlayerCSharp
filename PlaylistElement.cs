using Microsoft.WindowsAPICodePack.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PlaylistElement
{
    class VideoListItem : ListViewItem
    {
        string file;
        TimeSpan duration;
        public VideoListItem(string filePath)
        {
            file = filePath;

            StackPanel panel = new StackPanel();
            panel.Orientation = Orientation.Horizontal;
            Content = panel;

            Image image = new Image();
            image.Source = ShellFile.FromFilePath(filePath).Thumbnail.BitmapSource;
            image.Height = 100;
            image.Width = 100;
            image.Margin = new Thickness(0, 0, 20, 0);
            panel.Children.Add(image);

            TextBlock source = new TextBlock();
            source.Text = Path.GetFileName(filePath);
            source.Width = 150;
            source.Margin = new Thickness(0, 0, 20, 0);
            source.TextWrapping = TextWrapping.Wrap;
            source.VerticalAlignment = VerticalAlignment.Center;
            panel.Children.Add(source);

            double.TryParse(ShellFile.FromFilePath(filePath).Properties.System.Media.Duration.Value.ToString(), out double nanoseconds);
            duration = TimeSpan.FromSeconds((int)Math.Round((nanoseconds / 10000000), 0));

            TextBlock durationTime = new TextBlock();
            durationTime.Text = "Duration time: " + FormatTimeSpan(duration);
            durationTime.Width = 200;
            durationTime.TextWrapping = TextWrapping.Wrap;
            durationTime.VerticalAlignment = VerticalAlignment.Center;
            panel.Children.Add(durationTime);
        }

        public static string TotalDuration(List<VideoListItem> playlist)
        {
            TimeSpan totalDuration = TimeSpan.Zero;
            foreach (VideoListItem file in playlist)
            {
                totalDuration += file.duration;
            }
            return FormatTimeSpan(totalDuration);
        }

        public static string FormatTimeSpan(TimeSpan ts)
        {
            return string.Format("{0:D2}:{1:D2}", (int)ts.TotalMinutes, ts.Seconds);
        }

        public override string ToString()
        {
            return file;
        }
    }
}