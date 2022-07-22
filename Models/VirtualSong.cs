using CsvHelper.Configuration;
using System;

namespace TapeRecordWizard.Models
{
    public class VirtualSong
    {
        public string SongName { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class VirtualSongMap : ClassMap<VirtualSong>
    {
        public VirtualSongMap()
        {
            Map(s => s.SongName).Name("SongName");
            Map(s => s.Duration).Name("Duration");
        }
    }

}
