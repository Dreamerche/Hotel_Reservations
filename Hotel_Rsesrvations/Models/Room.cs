using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Rsesrvations.Models
{
    public class Room
    {
        [Key]
        public int ID { get; set; }
        [Required]
        [Range(1, 20)]
        public int capacity { get; set; }
        [Required]
        [Column("RoomType")]
        public string roomType { get; set; }
        /* public void SetType(RoomType roomType1)
         {
             roomType = roomType1.ToString();
         }*/

        public bool isOccupied { get; set; }
        [Required]
        [Range(0, 10000)]
        public double priceForAdult { get; set; }
        [Required]
        [Range(0, 10000)]
        public double priceForChild { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public int number { get; set; }
        public virtual HashSet<Reservation> Reservations { get; set; }

        public Room()
        {
            Reservations = new HashSet<Reservation>();
        }
    }
}