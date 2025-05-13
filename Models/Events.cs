using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EventeaseP7.Models
{
  
    

    [Table("Event")]
    public partial class Events
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        [StringLength(350)]
        public string? EventName { get; set; }

        [Column(TypeName = "date")]
        public DateTime EventDate { get; set; }

        public string? Description { get; set; }

        public int? Venue_ID { get; set; }

        public virtual Venue? Venue { get; set; }
    }
}
