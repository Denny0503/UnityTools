using HandyControl.Controls;
using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UnityMethods.Extend;
using TopUIControl.Controls;

namespace AudioVideoControlLibrary.Views
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class VlcVideoSample : RegionViewControl
    {
        LibVLC _libVLC;
        LibVLCSharp.Shared.MediaPlayer VlcMediaPlayer;

        public VlcVideoSample()
        {
            InitializeComponent();

            this.ItemHeader = "VLC播放器";
            this.ViewDataContext = this;

            _libVLC = new LibVLC("-I", "--no-dummy-quiet", "--ignore-config", "--no-video-title",
                "--plugins-cache", "--file-logging", "--logfile=Logs\\vlc_log.log",
                "--verbose=2", "--no-sub-autodetect-file", "--network-caching=200",
                //"--no-osd", /*不显示osd*/
                "--no-snapshot-preview"/*禁止截图预览*/,
                "--sout-all"/*处理所有的流*/,
                "--high-priority"   /*提高程序优先权*/
                );

            VlcMediaPlayer = new MediaPlayer(_libVLC);
            VlcMediaPlayer.PositionChanged += VlcMediaPlayer_PositionChanged;
            VlcMediaPlayer.EndReached += VlcMediaPlayer_EndReached;
            VlcMediaPlayer.Playing += VlcMediaPlayer_Playing;
            VlcVideoView.Loaded += VlcVideoView_Loaded;

            //PlayProgressInfo.AddHandler(PreviewSlider.MouseLeftButtonDownEvent, new MouseButtonEventHandler(PlayProgressInfo_MouseLeftButtonDown), true);
        }


        private void VlcMediaPlayer_Playing(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                PlayOrPause.IsChecked = true;
            }));
        }

        private void VlcMediaPlayer_EndReached(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new Action(() =>
            {
                PlayOrPause.IsChecked = false;
                PlayProgressInfo.Value = 0;
                PlayTimeText.Text = $@"{VlcMediaPlayer.Time.FormatMillisecond()} / {VlcMediaPlayer.Length.FormatMillisecond()}";
            }));
        }

        private void VlcMediaPlayer_PositionChanged(object sender, MediaPlayerPositionChangedEventArgs e)
        {
            if (!IsMouseDragStarted)
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    PlayProgressInfo.Value = e.Position * 100;
                    PlayTimeText.Text = $@"{VlcMediaPlayer.Time.FormatMillisecond()} / {VlcMediaPlayer.Length.FormatMillisecond()}";
                }));
            }
        }

        private void VlcVideoView_Loaded(object sender, RoutedEventArgs e)
        {
            VlcVideoView.MediaPlayer = VlcMediaPlayer;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PlayLocalFile(@"D:\教学视频\OpenCV4-图像处理与视频分析教程\2-第一个OpenCV程序.mp4");
            //PlayMediaUrl(VideoUrl.Text);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            StopPlay();
        }

        /// <summary>
        /// 播放本地文件
        /// </summary>
        /// <param name="filePath"></param>
        public void PlayLocalFile(string filePath)
        {
            if (!VlcVideoView.MediaPlayer.IsPlaying && File.Exists(filePath))
            {
                VlcVideoView.MediaPlayer.Play(new Media(_libVLC, filePath, FromType.FromPath));
                VlcVideoView.MediaPlayer.Volume = 150;
            }
        }

        public void PlayMediaUrl(string url)
        {
            if (!VlcVideoView.MediaPlayer.IsPlaying && !url.IsEmpty())
            {
                VlcVideoView.MediaPlayer.Play(new Media(_libVLC, url, FromType.FromLocation));
            }
        }

        /// <summary>
        /// 停止播放
        /// </summary>
        public void StopPlay()
        {
            if (VlcVideoView.MediaPlayer.IsPlaying)
            {
                VlcVideoView.MediaPlayer.Stop();
            }
        }

        private void ShowVideoMask_Click(object sender, RoutedEventArgs e)
        {
            CheckBox checkBox = sender as CheckBox;

            VideoMaskCanvas.Visibility = (bool)checkBox.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// 鼠标左键是否按下
        /// </summary>
        private bool IsMouseDragStarted = false;

        private void PlayProgressInfo_DragStarted(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {
            IsMouseDragStarted = true;
        }

        private void PlayProgressInfo_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            VlcMediaPlayer.Position = (float)PlayProgressInfo.Value / 100;
            IsMouseDragStarted = false;
        }

        private void PlayProgressInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            VlcMediaPlayer.Position = (float)PlayProgressInfo.Value / 100;
        }

        private void PlayOrPause_Click(object sender, RoutedEventArgs e)
        {
            if ((bool)PlayOrPause.IsChecked)
            {
                VlcMediaPlayer.Play();
            }
            else
            {
                VlcMediaPlayer.Pause();
            }
        }

        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton toggle = sender as ToggleButton;

            if ((bool)toggle.IsChecked)
            {
                VlcMediaPlayer.Volume = 0;
            }
            else
            {
                VlcMediaPlayer.Volume = 150;
            }
        }

        private void RegionViewControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (null != VlcMediaPlayer && VlcMediaPlayer.IsPlaying)
            {
                VlcMediaPlayer.Stop();
            }
        }
    }
}
