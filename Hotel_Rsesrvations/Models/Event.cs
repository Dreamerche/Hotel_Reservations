using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Rsesrvations.Models
{
    public class Event
    {
        [Key]
        public int ID { get; set; }
        [Required, MaxLength(64)]
        [Column("Name")]
        public string name { get; set; }
        [Required, MaxLength(64)]
        [Column("Description")]
        public string description { get; set; }
        [Required]
        [Column("PremiereDate")]
        public DateTime premiereDate { get; set; }
        [Required]
        [Column("PhotoLink")]
        public string photoLink { get; set; }
        public virtual HashSet<Ticket> Tickets { get; set; }

        public Event()
        {
            Tickets = new HashSet<Ticket>();
        }
    }
}
