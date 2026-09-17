using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peluqueria.Domain.Entities;

namespace Peluqueria.Infrastructure.Persistence.Configurations;

public class ServicioConfiguration : IEntityTypeConfiguration<Servicio>
{
    public void Configure(EntityTypeBuilder<Servicio> builder)
    {
        builder.ToTable("Servicios");

        builder.HasKey(s => s.ServicioId);

        builder.Property(s => s.ServicioId)
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Nombre)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(s => s.Descripcion)
            .HasMaxLength(500);

        builder.Property(s => s.Precio)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(s => s.DuracionMinutos)
            .IsRequired();

        builder.Property(s => s.ImagenUrl)
            .HasMaxLength(500);

        builder.Property(s => s.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(s => s.Nombre)
            .HasDatabaseName("IX_Servicio_Nombre");

        builder.HasCheckConstraint(
            "CK_Servicio_Precio_NoNegativo",
            "[Precio] >= 0");

        builder.HasCheckConstraint(
            "CK_Servicio_Duracion_Positiva",
            "[DuracionMinutos] > 0");

        builder.HasMany(s => s.Turnos)
            .WithOne(t => t.Servicio)
            .HasForeignKey(t => t.ServicioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}