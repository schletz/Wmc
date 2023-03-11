using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WmcApi.Model
{
    public class Period : Entity
    {
        protected Period()
        { }

        public Period(int nr, TimeSpan start, TimeSpan end, bool isEvening)
        {
            Nr = nr;
            Start = start;
            End = end;
            IsEvening = isEvening;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Nr { get; set; }

        public TimeSpan Start { get; set; }
        public TimeSpan End { get; set; }
        public bool IsEvening { get; set; }
    }
}