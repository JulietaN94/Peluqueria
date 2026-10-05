using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peluqueria.Domain.Entities;
using Peluqueria.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Peluqueria.Infrastructure.Persistence.Configurations;

public class TurnoConfiguration : IEntityTypeConfiguration<Turno>
{
    public void Configure(EntityTypeBuilder<Turno> builder)
    {
        builder.ToTable("Turnos");

        builder.HasKey(t => t.TurnoId);

        builder.Property(t => t.TurnoId)
            .ValueGeneratedOnAdd();

        builder.Property(t => t.ClienteId)
            .IsRequired();

        builder.Property(t => t.ProfesionalId)
            .IsRequired();

        builder.Property(t => t.ServicioId)
            .IsRequired();

        builder.Property(t => t.FechaHoraInicio)
            .HasColumnType("datetime2(0)")
            .IsRequired();

        builder.Property(t => t.FechaHoraFin)
            .HasColumnType("datetime2(0)")
            .IsRequired();

        builder.Property(t => t.Precio)
            .IsRequired()
            .HasPrecision(18, 2);

        builder.Property(t => t.Estado)
            .IsRequired();

        builder.Property(t => t.CodigoReserva)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(t => t.Observaciones)
            .HasMaxLength(1000);

        builder.Property(t => t.FechaCreacion)
            .HasColumnType("datetime2(0)")
            .IsRequired();

        builder.HasIndex(t => t.CodigoReserva)
            .IsUnique()
            .HasDatabaseName("UX_Turno_CodigoReserva");

        builder.HasIndex(t => new
        {
            t.ProfesionalId,
            t.FechaHoraInicio,
            t.FechaHoraFin
        })
        .HasDatabaseName("IX_Turno_Profesional_Horario");

        builder.HasIndex(t => new
        {
            t.ClienteId,
            t.FechaHoraInicio
        })
        .HasDatabaseName("IX_Turno_Cliente_Fecha");

        builder.HasIndex(t => new
        {
            t.Estado,
            t.FechaHoraInicio
        })
        .HasDatabaseName("IX_Turno_Estado_Fecha");

        builder.HasCheckConstraint(
            "CK_Turno_Precio_NoNegativo",
            "[Precio] >= 0");

        builder.HasCheckConstraint(
            "CK_Turno_FechaHoraFin_MayorInicio",
            "[FechaHoraFin] > [FechaHoraInicio]");

        builder.HasOne(t => t.Cliente)
            .WithMany(c => c.Turnos)
            .HasForeignKey(t => t.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Profesional)
            .WithMany(p => p.Turnos)
            .HasForeignKey(t => t.ProfesionalId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Servicio)
            .WithMany(s => s.Turnos)
            .HasForeignKey(t => t.ServicioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}