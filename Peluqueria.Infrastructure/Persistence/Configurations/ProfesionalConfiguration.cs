using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peluqueria.Domain.Entities;

namespace Peluqueria.Infrastructure.Persistence.Configurations;

public class ProfesionalConfiguration : IEntityTypeConfiguration<Profesional>
{
    public void Configure(EntityTypeBuilder<Profesional> builder)
    {
        builder.ToTable("Profesionales");

        builder.HasKey(p => p.ProfesionalId);

        builder.Property(p => p.ProfesionalId)
            .ValueGeneratedOnAdd();

        builder.Property(p => p.NombreCompleto)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(p => p.Telefono)
            .HasMaxLength(30);

        builder.Property(p => p.Especialidad)
            .HasMaxLength(150);

        builder.Property(p => p.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(p => p.Observaciones)
            .HasMaxLength(1000);

        builder.HasIndex(p => p.NombreCompleto)
            .HasDatabaseName("IX_Profesional_NombreCompleto");

        builder.HasMany(p => p.Turnos)
            .WithOne(t => t.Profesional)
            .HasForeignKey(t => t.ProfesionalId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}