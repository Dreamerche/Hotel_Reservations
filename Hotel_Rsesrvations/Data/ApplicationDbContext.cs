﻿using Hotel_Rsesrvations.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hotel_Rsesrvations.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ReservationClient> ReservationClients { get; set; }

        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<Event> Events { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserLogin<string>>().HasKey(x => x.UserId);
            modelBuilder.Entity<Room>()
                .HasMany(r => r.Reservations)
                .WithOne(res => res.Room)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ReservationClient>()
                .HasKey(rc => new { rc.ReservationId, rc.ClientId });

            modelBuilder.Entity<ReservationClient>()
                .HasOne(rc => rc.Reservation)
                .WithMany(r => r.ReservationClients)
                .HasForeignKey(rc => rc.ReservationId);

            modelBuilder.Entity<ReservationClient>()
                .HasOne(rc => rc.Client)
                .WithMany(c => c.ReservationClients)
                .HasForeignKey(rc => rc.ClientId);



            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .IsRequired();
        }


        public async Task<Reservation> UpdateReservationClients(Reservation reservation, List<ReservationClient>reservationClients,List<Client> clients)
        {
            var originalReservation = await this.FindAsync<Reservation>(reservation.Id);
            
            if (originalReservation != null && reservationClients!=null && clients!=null)
            {
 
                for (int i = 0; i < reservationClients.Count; i++)
                    {
                    reservationClients[i].Client = clients[i];
                    }
                originalReservation.ReservationClients = reservationClients.ToHashSet();
                await this.SaveChangesAsync();
            }
            return originalReservation;
        }
        public async Task<int> UpdateRoomsAsync(List<Room> rooms)
        {
            foreach (var room in rooms)
            {
                this.Entry(room).State = EntityState.Modified;
            }

            return await this.SaveChangesAsync();
        }


    }
}
