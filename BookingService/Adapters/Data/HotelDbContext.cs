﻿using Data.Booking;
using Data.Guest;
using Data.Room;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class HotelDbContext : DbContext
    {
        public HotelDbContext(DbContextOptions<HotelDbContext> options) : base(options) { }

        public virtual DbSet<Domain.Entities.Guest> Guests { get; set; }
        public virtual DbSet<Domain.Entities.Booking> Bokings { get; set; }
        public virtual DbSet<Domain.Entities.Room> Rooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GuestConfiguration());
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfiguration());
        }

    }
}
