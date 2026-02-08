using Microsoft.EntityFrameworkCore;
using ProdutorRuralSensores.Domain.Entities;
using ProdutorRuralSensores.Infrastructure.DataBase.EntityFramework.Configurations;

namespace ProdutorRuralSensores.Infrastructure.DataBase.EntityFramework.Context
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Sensor> Sensores { get; set; }
        public DbSet<LeituraSensor> LeiturasSensores { get; set; }

        /// <summary>  
        /// Initializes a new instance of the <see cref="ApplicationDbContext"/> class with the specified options.  
        /// </summary>  
        /// <param name="options">The options to configure the database context.</param>  
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            builder.ApplyConfiguration(new SensorConfiguration());
            builder.ApplyConfiguration(new LeituraSensorConfiguration());
        }
    }
}
