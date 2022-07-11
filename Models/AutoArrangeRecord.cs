using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TapeRecordWizard.Models
{
    public class AutoArrangeRecord
    {
        public AutoArrangeRecord()
        {
            this.Songs = new List<Song>();
        }
        public List<Song> Songs { get; set; }
        public TimeSpan TotalLength
        {
            get
            {
                return new TimeSpan(Songs.Sum(x => x.Duration.Ticks));
            }
        }
    }
}
