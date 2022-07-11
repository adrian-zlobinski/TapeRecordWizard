using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace TapeRecordWizard.Models
{
    public class Player : BaseModel
    {
        private readonly PlayList _playList;
        private WaveOutEvent outputDevice;
        private char playingSide = '-';
        private System.Timers.Timer playbackTimer;
        public TimeSpan PlayBackTime { get; set; }

        public Player(PlayList playList)
        {
            this._playList = playList;
            playbackTimer = new System.Timers.Timer(100) { AutoReset = true };
            playbackTimer.Elapsed += PlaybackTimer_Elapsed;
            PlayBackTime = TimeSpan.FromSeconds(0);
        }

        public void PlaySideA()
        {

        }

        public void PlaySideB()
        { 
        }

        public void Stop()
        {

        }

        public TimeSpan PlayedSideDuration
        {
            get
            {
                if (PlayingSideA)
                    return Model.ModelInstance.CurrentPlaylist.SideADuration;
                if (PlayingSideB)
                    return Model.ModelInstance.CurrentPlaylist.SideBDuration;
                return TimeSpan.FromSeconds(0);
            }
        }


        public bool PlayingSideA
        {
            get
            {
                return outputDevice?.PlaybackState == PlaybackState.Playing && playingSide == 'A';
            }
        }

        public bool PlayingSideB
        {
            get
            {
                return outputDevice?.PlaybackState == PlaybackState.Playing && playingSide == 'B';
            }
        }

        private void OutputDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            outputDevice.Dispose();
            outputDevice = null;
            btnStop.Background = Brushes.Red;
            btnPlaySideA.Background = (Brush)new BrushConverter().ConvertFrom("#FFDDDDDD");
            btnPlaySideB.Background = (Brush)new BrushConverter().ConvertFrom("#FFDDDDDD");
            OnPropertyChanged(nameof(PlayingSideA));
            OnPropertyChanged(nameof(PlayingSideB));
            playbackTimer.Stop();
            PlayBackTime = TimeSpan.FromSeconds(0);
            OnPropertyChanged(nameof(PlayBackTime));
            OnPropertyChanged(nameof(PlayedSideDuration));
        }



        private void PlaybackTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            PlayBackTime = outputDevice.GetPositionTimeSpan();
            OnPropertyChanged(nameof(PlayBackTime));
        }


        private ISampleProvider ApplyFadeInOut(double totalLength, ISampleProvider source)
        {
            NAudio.DelayFadeOutSampleProvider result = null;
            if (Model.ModelInstance.CurrentPlaylist.FadeInMiliseconds > 0)
            {
                result = new NAudio.DelayFadeOutSampleProvider(source, true);
                result.BeginFadeIn(Model.ModelInstance.CurrentPlaylist.FadeInMiliseconds);
            }
            if (Model.ModelInstance.CurrentPlaylist.FadeOutMiliseconds > 0)
            {
                if (result is null)
                {
                    result = new NAudio.DelayFadeOutSampleProvider(source, false);
                }
                result.BeginFadeOut(totalLength - Model.ModelInstance.CurrentPlaylist.FadeOutMiliseconds, Model.ModelInstance.CurrentPlaylist.FadeOutMiliseconds);
            }
            if (result is null)
                return source;
            return result;
        }

    }
}
