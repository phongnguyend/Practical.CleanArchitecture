using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassifiedAds.Persistence.MiniProfiler
{
    public class MiniProfilerTimings
    {
        [Key]
        public int RowId { get; set; }

        public Guid Id { get; set; }

        public Guid MiniProfilerId { get; set; }

        public Guid? ParentTimingId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(15,3)")]
        public decimal DurationMilliseconds { get; set; }

        [Column(TypeName = "decimal(15,3)")]
        public decimal StartMilliseconds { get; set; }

        public bool IsRoot { get; set; }

        public short Depth { get; set; }

        public string CustomTimingsJson { get; set; }
    }
}
