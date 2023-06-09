﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel_Rsesrvations.Models
{
    public class Reservation
    {
        [Key]
        public int Id { get; set; }
        //user which created the reservation...
        public int RoomId { get; set; }
        public Room Room { get; set; }

        public DateTime checkInDate { get; set; }
        public DateTime vacatingDate { get; set; }
        public bool includingBreakfast { get; set; }
        public bool isAllInclusive { get; set; }
        public double totalPrice { get; set; }
        public virtual HashSet<ReservationClient> ReservationClients { get; set; }
    }
}
