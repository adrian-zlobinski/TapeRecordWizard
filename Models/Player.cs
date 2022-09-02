using NAudio.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TapeRecordWizard.Models
{
    public class Player : BaseModel
    {
        private WaveOutEvent outputDevice;
        private List<Song> songsToPlay;
        private char playingSide = '-';
        private System.Timers.Timer playbackTimer;
        public TimeSpan PlayBackTime { get; set; }

        public Player()
        {
            playbackTimer = new System.Timers.Timer(100) { AutoReset = true };
            playbackTimer.Elapsed += PlaybackTimer_Elapsed;
            PlayBackTime = TimeSpan.FromSeconds(0);
        }

        public void PlaySideA()
        {
            songsToPlay = Model.ModelInstance.CurrentPlaylist.SideASongs.OrderBy(x => x.OrderNo).ToList();
            playingSide = 'A';
            this.Play();
        }

        public void PlaySideB()
        {
            songsToPlay = Model.ModelInstance.CurrentPlaylist.SideBSongs.OrderBy(x => x.OrderNo).ToList();
            playingSide = 'B';
            this.Play();
        }

        private void Play()
        {
            if(PlayingSideA || PlayingSideB)
            {
                return;
            }
            if (outputDevice is null)
            {
                outputDevice = new WaveOutEvent();
                outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;
            }
            AudioFileReader firstSong = new AudioFileReader(songsToPlay[0].FullFilePath);

            ISampleProvider sampleProvider = ApplyFadeInOut(songsToPlay[0].Duration.TotalMilliseconds, firstSong);
            if (songsToPlay.Count > 1)
            {
                for (int i = 1; i < songsToPlay.Count; i++)
                {
                    var nextSong = ApplyFadeInOut(songsToPlay[1].Duration.TotalMilliseconds, new AudioFileReader(songsToPlay[i].FullFilePath));
                    if (Model.ModelInstance.CurrentPlaylist.GapBetweenSongs > 0)
                    {
                        sampleProvider = sampleProvider.FollowedBy(TimeSpan.FromMilliseconds((int)Model.ModelInstance.CurrentPlaylist.GapBetweenSongs*1000), nextSong);
                    }
                    else
                    {
                        sampleProvider = sampleProvider.FollowedBy(nextSong);
                    }
                }
            }
            //WaveFileWriter.CreateWaveFile16(System.IO.Path.Combine(System.IO.Path.GetTempPath(), "record.wav"), sampleProvider);
            outputDevice.Init(sampleProvider);
            outputDevice.Play();
            playbackTimer.Start();
            outputDevice.GetPositionTimeSpan();
            OnPropertyChanged(nameof(PlayingSideA));
            OnPropertyChanged(nameof(PlayingSideB));
            OnPropertyChanged(nameof(PlayedSideDuration));
            OnPropertyChanged(nameof(Stopped));
            Model.ModelInstance.CanPlaySideChanged();
            firstSong.Dispose();
            firstSong = null;
        }

        public void Stop()
        {
            outputDevice?.Stop();
            OnPropertyChanged(nameof(PlayingSideA));
            OnPropertyChanged(nameof(PlayingSideB));
            playbackTimer.Stop();
            PlayBackTime = TimeSpan.FromSeconds(0);
            OnPropertyChanged(nameof(PlayBackTime));
            OnPropertyChanged(nameof(PlayedSideDuration));
            Model.ModelInstance.CanPlaySideChanged();
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

        public bool Stopped
        {
            get
            {
                return outputDevice?.PlaybackState == PlaybackState.Stopped;
            }
        }

        private void OutputDevice_PlaybackStopped(object sender, StoppedEventArgs e)
        {
            OnPropertyChanged(nameof(PlayingSideA));
            OnPropertyChanged(nameof(PlayingSideB));
            OnPropertyChanged(nameof(Stopped));
            outputDevice.Dispose();
            outputDevice = null;
            //btnPlaySideA.Background = (Brush)new BrushConverter().ConvertFrom("#FFDDDDDD");
            //btnPlaySideB.Background = (Brush)new BrushConverter().ConvertFrom("#FFDDDDDD");
            playbackTimer.Stop();
            PlayBackTime = TimeSpan.FromSeconds(0);
            OnPropertyChanged(nameof(PlayBackTime));
            OnPropertyChanged(nameof(PlayedSideDuration));
            Model.ModelInstance.CanPlaySideChanged();
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
