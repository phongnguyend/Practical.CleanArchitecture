using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClassifiedAds.Persistence.MiniProfiler
{
    public class MiniProfilerClientTimings
    {
        [Key]
        public int RowId { get; set; }

        public Guid Id { get; set; }

        public Guid MiniProfilerId { get; set; }

        [StringLength(200)]
        public string Name { get; set; }

        [Column(TypeName = "decimal(9,3)")]
        public decimal Start { get; set; }

        [Column(TypeName = "decimal(9,3)")]
        public decimal Duration { get; set; }
    }
}
