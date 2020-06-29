using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassifiedAds.Persistence.MiniProfiler
{
    public class MiniProfilers
    {
        [Key]
        public int RowId { get; set; }

        public Guid Id { get; set; }

        public Guid? RootTimingId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        public DateTime Started { get; set; }

        [Column(TypeName = "decimal(15,1)")]
        public decimal DurationMilliseconds { get; set; }

        [StringLength(100)]
        public string User { get; set; }

        public bool HasUserViewed { get; set; }

        [StringLength(100)]
        public string MachineName { get; set; }

        public string CustomLinksJson { get; set; }

        public int? ClientTimingsRedirectCount { get; set; }
    }
}
