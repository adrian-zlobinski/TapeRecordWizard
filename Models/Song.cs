using System;
using System.IO;
using NAudio;
using NAudio.Wave;

namespace TapeRecordWizard.Models
{
    public class Song
    {
        #region Private properties
        private TimeSpan? _duration = null;
        #endregion

        #region Public properties
        public string FullFilePath { get; set; }
        public string Side { get; set; }
        public int OrderNo { get; set; }
        #endregion

        #region Readonly properties
        public TimeSpan Duration
        {
            get
            {
                if (!_duration.HasValue)
                {
                    _duration = new AudioFileReader(FullFilePath).TotalTime;
                }
                return _duration.Value;
            }
        }
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
        #endregion
    }
}
