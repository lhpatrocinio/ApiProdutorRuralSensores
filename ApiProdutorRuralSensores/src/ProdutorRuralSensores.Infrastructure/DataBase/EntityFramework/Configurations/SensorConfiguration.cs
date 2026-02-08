using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdutorRuralSensores.Domain.Entities;

namespace ProdutorRuralSensores.Infrastructure.DataBase.EntityFramework.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade Sensor
/// </summary>
public class SensorConfiguration : IEntityTypeConfiguration<Sensor>
{
    public void Configure(EntityTypeBuilder<Sensor> builder)
    {
        builder.ToTable("Sensores");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .ValueGeneratedNever();

        builder.Property(s => s.TalhaoId)
            .IsRequired();

        builder.Property(s => s.Codigo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Tipo)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(s => s.Modelo)
            .HasMaxLength(100);

        builder.Property(s => s.Fabricante)
            .HasMaxLength(100);

        builder.Property(s => s.Latitude)
            .HasPrecision(10, 8);

        builder.Property(s => s.Longitude)
            .HasPrecision(11, 8);

        builder.Property(s => s.Ativo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Índices
        builder.HasIndex(s => s.Codigo)
            .IsUnique();

        builder.HasIndex(s => s.TalhaoId);

        builder.HasIndex(s => s.Ativo);

        // Relacionamento com Leituras
        builder.HasMany(s => s.Leituras)
            .WithOne(l => l.Sensor)
            .HasForeignKey(l => l.SensorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
