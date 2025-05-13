using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace EventeaseP7.Models
{

 

    
    public partial class Booking
    {
        [Key]
        public int BookingID { get; set; }

        public int EventID { get; set; }
        [Required]
        [ForeignKey("Venue")]
        public int Venue_ID { get; set; }

        [Column(TypeName = "date")]
        public DateTime BookingDate { get; set; }

        public virtual Events? Event { get; set; }

        public virtual Venue? Venue { get; set; }
    }

}
