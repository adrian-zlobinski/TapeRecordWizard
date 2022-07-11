using System;

namespace TapeRecordWizard.Models
{
    public class CassetteType
    {
        public string Name { get; set; }
        public TimeSpan TotalLength { get; set; }
        public TimeSpan SideLength 
        { 
            get
            {
                return new TimeSpan(TotalLength.Ticks / 2);
            }
        }
    }
}
