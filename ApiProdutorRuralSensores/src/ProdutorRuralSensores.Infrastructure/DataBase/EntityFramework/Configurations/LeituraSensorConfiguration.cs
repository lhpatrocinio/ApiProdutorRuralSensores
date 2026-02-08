using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProdutorRuralSensores.Domain.Entities;

namespace ProdutorRuralSensores.Infrastructure.DataBase.EntityFramework.Configurations;

/// <summary>
/// Configuração do Entity Framework para a entidade LeituraSensor
/// </summary>
public class LeituraSensorConfiguration : IEntityTypeConfiguration<LeituraSensor>
{
    public void Configure(EntityTypeBuilder<LeituraSensor> builder)
    {
        builder.ToTable("LeiturasSensores");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Id)
            .ValueGeneratedNever();

        builder.Property(l => l.TalhaoId)
            .IsRequired();

        builder.Property(l => l.UmidadeSolo)
            .HasPrecision(5, 2);

        builder.Property(l => l.Temperatura)
            .HasPrecision(5, 2);

        builder.Property(l => l.Precipitacao)
            .HasPrecision(5, 2);

        builder.Property(l => l.UmidadeAr)
            .HasPrecision(5, 2);

        builder.Property(l => l.VelocidadeVento)
            .HasPrecision(5, 2);

        builder.Property(l => l.DirecaoVento)
            .HasMaxLength(10);

        builder.Property(l => l.RadiacaoSolar)
            .HasPrecision(7, 2);

        builder.Property(l => l.PressaoAtmosferica)
            .HasPrecision(6, 2);

        builder.Property(l => l.DataHoraLeitura)
            .IsRequired();

        builder.Property(l => l.CreatedAt)
            .IsRequired();

        // Índices otimizados para consultas temporais
        builder.HasIndex(l => new { l.TalhaoId, l.DataHoraLeitura })
            .IsDescending(false, true);

        builder.HasIndex(l => l.DataHoraLeitura)
            .IsDescending(true);

        builder.HasIndex(l => l.SensorId);

        // Relacionamento com Sensor
        builder.HasOne(l => l.Sensor)
            .WithMany(s => s.Leituras)
            .HasForeignKey(l => l.SensorId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
