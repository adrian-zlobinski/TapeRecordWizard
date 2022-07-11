using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Text;

namespace TapeRecordWizard.NAudio
{
    public class DelayFadeOutSampleProvider : ISampleProvider
    {
        enum FadeState
        {
            Silence,
            FadingIn,
            FullVolume,
            FadingOut
        }

        private readonly object lockObject = new object();
        private readonly ISampleProvider source;
        private int fadeInSamplePosition;
        private int fadeInSampleCount;
        private int fadeOutSampleCount;
        private int fadeOutSampleStart;
        private int fadeOutSamplePosition;
        private FadeState fadeState;

        /// <summary>
        /// Creates a new FadeInOutSampleProvider
        /// </summary>
        /// <param name="source">The source stream with the audio to be faded in or out</param>
        /// <param name="initiallySilent">If true, we start faded out</param>
        public DelayFadeOutSampleProvider(ISampleProvider source, bool initiallySilent = false)
        {
            this.source = source;
            this.fadeState = initiallySilent ? FadeState.Silence : FadeState.FullVolume;
        }

        /// <summary>
        /// Requests that a fade-in begins(will start on the next call to Read)
        /// </summary>
        /// <param name="fadeDurationInMiliseconds">Duration of fade in in miliseconds</param>
        public void BeginFadeIn(double fadeDurationInMiliseconds)
        {
            lock(lockObject)
            {
                fadeInSamplePosition = 0;
                fadeInSampleCount = (int)((fadeDurationInMiliseconds * WaveFormat.SampleRate) / 1000);
                fadeState = FadeState.FadingIn;
            }
        }

        /// <summary>
        /// Request that fade-out begins (will start on the next call to Read)
        /// </summary>
        /// <param name="fadeAfterMiliseconds">Fade delay in miliseconds</param>
        /// <param name="fadeDurationInMiliseconds">Duration of fade in miliseconds</param>
        public void BeginFadeOut(double fadeAfterMiliseconds, double fadeDurationInMiliseconds)
        {
            lock (lockObject)
            {
                fadeOutSampleCount = (int)((fadeDurationInMiliseconds * WaveFormat.SampleRate) / 1000);
                fadeOutSampleStart = (int)((fadeAfterMiliseconds * WaveFormat.SampleRate) / 1000);
                fadeOutSamplePosition = 0;
            }
        }

        /// <summary>
        /// Read samples from this sample provider
        /// </summary>
        /// <param name="buffer">Buffer to read into</param>
        /// <param name="offset">Offset within buffer to write to</param>
        /// <param name="count">Number of sampler desired</param>
        /// <returns></returns>
        public int Read(float[] buffer, int offset, int count)
        {
            int sourceSamplesRead = source.Read(buffer, offset, count);

            lock(lockObject)
            {
                if(fadeOutSampleStart > 0)
                {
                    if(fadeState != FadeState.FadingOut && fadeState != FadeState.Silence && fadeOutSamplePosition + (sourceSamplesRead / WaveFormat.Channels) >= fadeOutSampleStart)
                    {
                        fadeState = FadeState.FadingOut;
                    }
                    else if(fadeState != FadeState.FadingOut)
                    {
                        fadeOutSamplePosition += (sourceSamplesRead / WaveFormat.Channels);
                    }
                }

                switch(fadeState)
                {
                    case FadeState.FadingIn:
                        FadeIn(buffer, offset, sourceSamplesRead);
                        break;
                    case FadeState.FadingOut:
                        FadeOut(buffer, offset, sourceSamplesRead);
                        break;
                    case FadeState.Silence:
                        ClearBuffer(buffer, offset, sourceSamplesRead);
                        break;

                }
            }
            return sourceSamplesRead;
        }

        private void ClearBuffer(float[] buffer, int offset, int count)
        {
            for(int n = 0; n < count; n++)
            {
                buffer[n + offset] = 0;
            }
        }

        private void FadeOut(float[] buffer, int offset, int sourceSamplesRead)
        {
            int sample = 0;
            while(sample < sourceSamplesRead)
            {
                if (fadeOutSamplePosition >= fadeOutSampleStart)
                {
                    float multiplier = 1 - ((float)fadeOutSamplePosition - fadeOutSampleStart) / fadeOutSampleCount;
                    for (int ch = 0; ch < WaveFormat.Channels; ch++)
                    {
                        buffer[offset + sample++] *= multiplier;
                    }
                }
                else
                {
                    sample += WaveFormat.Channels;
                }
                fadeOutSamplePosition++;
                if(fadeOutSamplePosition > fadeOutSampleStart + fadeOutSampleCount)
                {
                    fadeState = FadeState.Silence;
                    ClearBuffer(buffer, sample + offset, sourceSamplesRead - sample);
                    break;
                }
            }
        }

        private void FadeIn(float[] buffer, int offset, int sourceSamplesRead)
        {
            int sample = 0;
            while(sample < sourceSamplesRead)
            {
                float multiplier = (fadeInSamplePosition / (float)fadeInSampleCount);
                for(int ch = 0; ch < source.WaveFormat.Channels; ch++)
                {
                    buffer[offset + sample++] *= multiplier;
                }
                fadeInSamplePosition++;
                if(fadeInSamplePosition > fadeInSampleCount)
                {
                    fadeState = FadeState.FullVolume;
                    break;
                }
            }
        }
        /// <summary>
        /// WaveFormat of this SampleProvider
        /// </summary>
        public WaveFormat WaveFormat
        {
            get { return source.WaveFormat; }
        }
    }
}
