using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace WebJob
{
    public class IotDatabaseContext : DbContext
    {
        public IotDatabaseContext()
        {
            //Blank constructor for tests
        }
        public IotDatabaseContext(DbContextOptions<IotDatabaseContext> options) : base(options)
        {
            //Pass the options into the base constructor
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SensorData>().ToTable("SensorData");
            
            modelBuilder.Entity<SensorData>()
                .HasKey(u => u.Id)
                .HasName("PK_SensorData");
        }

        public virtual DbSet<SensorData> SensorData { get; set; }
    }
}