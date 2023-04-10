using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Rsesrvations.Models
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string firstName { get; set; }
        [Required, MaxLength(50)]
        public string lastName { get; set; }
        [Required, MaxLength(10)]
        public string phoneNumber { get; set; }
        [Required]
        public bool isAdult { get; set; }
        public virtual HashSet<ReservationClient> ReservationClients { get; set; }
    }
}
