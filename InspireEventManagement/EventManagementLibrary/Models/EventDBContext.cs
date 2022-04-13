using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EventManagementLibrary.Models
{
    public partial class EventDBContext : DbContext
    {
        public EventDBContext()
        {
        }

        public EventDBContext(DbContextOptions<EventDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Event> Events { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=C:\\InspireEventManagement\\InspireEventManagement\\EventManagementUI\\Data\\DB\\EventManagementDB;Trusted_Connection=True;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Event>(entity =>
            {
                entity.Property(e => e.Description)
                    .HasMaxLength(400)
                    .IsFixedLength();

                entity.Property(e => e.EventEnd).HasColumnType("datetime");

                entity.Property(e => e.EventStart).HasColumnType("datetime");

                entity.Property(e => e.Images)
                    .HasMaxLength(50)
                    .IsFixedLength();

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
