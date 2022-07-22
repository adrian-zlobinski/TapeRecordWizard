using System;
using System.Collections.Generic;
using System.Linq;

namespace TapeRecordWizard.Models
{
    public sealed class PlayList : BaseModel
    {
        #region Constructor
        public PlayList()
        {
            this.Songs = new List<Song>();
        }
        #endregion

        #region Public properties
        public string Name { get; set; }
        public CassetteType CassetteType { get; set; }
        public decimal GapBetweenSongs { get; set; }
        public List<Song> Songs { get; set; }
        public decimal FadeIn { get; set; }
        public decimal FadeOut { get; set; }
        #endregion

        #region Readonly properties
        
        private int GapBetweenSongsMiliseconds
        {
            get
            {
                return (int)(GapBetweenSongs * 1000);
            }
        }
        
        public List<Song> SideASongs
        {
            get
            {
                return Songs.Where(x => x.Side == "A").ToList();
            }
        }
        
        public List<Song> SideBSongs
        {
            get
            {
                return Songs.Where(x => x.Side == "B").ToList();
            }
        }
        
        public TimeSpan TotalDuration
        {
            get
            {
                var result = new TimeSpan(Songs.Sum(s => s.Duration.Ticks));
                if (GapBetweenSongs > 0)
                {
                    int totalGaps = this.Songs.Count - 1;
                    result += TimeSpan.FromMilliseconds(totalGaps * GapBetweenSongsMiliseconds);
                }

                return result;
            }
        }
        
        public TimeSpan SideADuration
        {
            get
            {
                var result = new TimeSpan(SideASongs.Sum(x => x.Duration.Ticks));
                if (GapBetweenSongs > 0 && SideASongs.Count > 1)
                {
                    int totalGaps = this.SideASongs.Count - 1;
                    result += TimeSpan.FromMilliseconds(totalGaps * GapBetweenSongsMiliseconds);
                }
                return result;
            }
        }
        
        public TimeSpan SideBDuration
        {
            get
            {
                var result = new TimeSpan(SideBSongs.Sum(x => x.Duration.Ticks));
                if (GapBetweenSongs > 0 && SideBSongs.Count > 1)
                {
                    int totalGaps = this.SideBSongs.Count - 1;
                    result += TimeSpan.FromMilliseconds(totalGaps * GapBetweenSongsMiliseconds);
                }
                return result;
            }
        }
        
        public bool SongsFitOnTape
        {
            get
            {
                if (CassetteType != null)
                {
                    return CassetteType.TotalLength >= TotalDuration;
                }
                else
                    return false;
            }
        }

        
        public bool SongsFitOnSideA
        {
            get
            {
                if (CassetteType != null)
                {
                    return CassetteType.SideLength >= SideADuration;
                }
                else
                    return false;
            }
        }

        
        public bool SongsFitOnSideB
        {
            get
            {
                if (CassetteType != null)
                {
                    return CassetteType.SideLength >= SideBDuration;
                }
                else
                    return false;
            }
        }

        
        public double FadeInMiliseconds
        {
            get
            {
                return (double)(FadeIn * 1000);
            }
        }

        
        public double FadeOutMiliseconds
        {
            get
            {
                return (double)(FadeOut * 1000);
            }
        }
        #endregion

        #region Internal methods
        internal void AutoArrangeSongs()
        {
            var s = Songs.OrderBy(x => x.OrderNo).ToArray();
            List<AutoArrangeRecord[]> aaRecords = new List<AutoArrangeRecord[]>();
            for (int a = 0; a < s.Length; a++)
            {
                var aar_SideA = new AutoArrangeRecord();
                if (aar_SideA.TotalLength + s[a].Duration <= CassetteType.SideLength)
                    aar_SideA.Songs.Add(s[a]);
                for (int b = 0; b < Songs.Count; b++)
                {
                    if (b == a) continue;
                    if (aar_SideA.TotalLength + Songs[b].Duration <= CassetteType.SideLength)
                        aar_SideA.Songs.Add(Songs[b]);
                    else
                        continue;
                }

                var aar_SideB = new AutoArrangeRecord();
                var songs2 = Songs.Except(aar_SideA.Songs).OrderBy(x => x.OrderNo).ToArray();
                if (new TimeSpan(songs2.Sum(x => x.Duration.Ticks)) <= CassetteType.SideLength)
                {
                    aar_SideB.Songs.AddRange(songs2);
                }
                else
                {
                    //utwory nie mieszczą się na drugiej stronie, ta kombinacja odpada
                    continue;
                }
                aaRecords.Add(new AutoArrangeRecord[] { aar_SideA, aar_SideB });
            }

            long minTimeWaste = long.MaxValue;
            AutoArrangeRecord[] minTimeWasteRecord = null;
            foreach (var r in aaRecords)
            {
                var wasteTime = CassetteType.TotalLength - (r[0].TotalLength + r[1].TotalLength);
                if (wasteTime.Ticks < minTimeWaste)
                {
                    minTimeWaste = wasteTime.Ticks;
                    minTimeWasteRecord = r;
                }
            }

            foreach (var sA in minTimeWasteRecord[0].Songs)
            {
                sA.Side = "A";
            }
            foreach (var sB in minTimeWasteRecord[1].Songs)
            {
                sB.Side = "B";
            }
        }
        #endregion

        #region Events
        public void SongsChanged()
        {
            OnPropertyChanged(nameof(Songs));
            OnPropertyChanged(nameof(SideASongs));
            OnPropertyChanged(nameof(SideBSongs));
            OnPropertyChanged(nameof(TotalDuration));
            OnPropertyChanged(nameof(SideADuration));
            OnPropertyChanged(nameof(SideBDuration));
        }
        #endregion
    }
}
