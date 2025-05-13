
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using EventeaseP7.Models;



   namespace EventeaseP7.Models
{ 
    public class Venue
    {

        [Key]
        public int Venue_ID { get; set; }

        [Required]
        [StringLength(350)]
        public string? Venue_Name { get; set; }

        [Required]
        [StringLength(350)]
        public string? Location { get; set; }

        public int Capacity { get; set; }

        [StringLength(500)]
        public string? Image_Url { get; set; }

        [NotMapped] 
        public IFormFile? ImageFile { get; set; }
     
        public List <Events> Events { get; set; }
    }
}
