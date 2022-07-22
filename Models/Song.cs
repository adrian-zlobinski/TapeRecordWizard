using System;
using System.IO;
using System.Text.Json.Serialization;
using CsvHelper.Configuration;
using NAudio;
using NAudio.Wave;

namespace TapeRecordWizard.Models
{
    public class Song
    {
        #region Private properties
        private TimeSpan? _duration = null;
        private readonly bool _isVirtual = false;
        #endregion

        #region Constructor
        public Song() { }
        public Song(string name, TimeSpan duration)
        {
            this.FullFilePath = name;
            this._duration = duration;
            this._isVirtual = true;
        }
        #endregion


        #region Public properties
        public string FullFilePath { get; set; }
        public string Side { get; set; }
        public int OrderNo { get; set; }

        public bool IsVirtual
        {
            get
            {
                return this._isVirtual;
            }
        }
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
