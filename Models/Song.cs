using System;
using System.IO;
using NAudio;
using NAudio.Wave;

namespace TapeRecordWizard.Models
{
    public class Song
    {
        public string FullFilePath { get; set; }

        private TimeSpan? _duration = null;
        public TimeSpan Duration
        {
            get
            {
                if(!_duration.HasValue)
                {
                    _duration = new AudioFileReader(FullFilePath).TotalTime;
                }
                return _duration.Value;
            }
        }
        public string Side { get; set; }
        public int OrderNo { get; set; }

        public string FileName 
        { 
            get
            {
                return Path.GetFileName(FullFilePath);
            }
        }

        public string FileType
        {
            get
            {
                return Path.GetExtension(FileName);
            }
        }
    }
}
