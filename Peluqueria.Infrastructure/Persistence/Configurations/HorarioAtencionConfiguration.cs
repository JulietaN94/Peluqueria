using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peluqueria.Domain.Entities;

namespace Peluqueria.Infrastructure.Persistence.Configurations;

public class HorarioAtencionConfiguration : IEntityTypeConfiguration<HorarioAtencion>
{
    public void Configure(EntityTypeBuilder<HorarioAtencion> builder)
    {
        builder.ToTable("HorariosAtencion");

        builder.HasKey(h => h.HorarioAtencionId);

        builder.Property(h => h.HorarioAtencionId)
            .ValueGeneratedOnAdd();

        builder.Property(h => h.DiaSemana)
            .IsRequired();

        builder.Property(h => h.HoraApertura)
            .HasColumnType("time(0)")
            .IsRequired();

        builder.Property(h => h.HoraCierre)
            .HasColumnType("time(0)")
            .IsRequired();

        builder.Property(h => h.HoraInicioDescanso)
            .HasColumnType("time(0)");

        builder.Property(h => h.HoraFinDescanso)
            .HasColumnType("time(0)");

        builder.Property(h => h.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.HasIndex(h => h.DiaSemana)
            .IsUnique()
            .HasDatabaseName("UX_HorarioAtencion_DiaSemana");

        builder.HasCheckConstraint(
            "CK_HorarioAtencion_AperturaAntesCierre",
            "[HoraApertura] < [HoraCierre]");

        builder.HasCheckConstraint(
            "CK_HorarioAtencion_DescansoCompleto",
            "([HoraInicioDescanso] IS NULL AND [HoraFinDescanso] IS NULL) " +
            "OR ([HoraInicioDescanso] IS NOT NULL AND [HoraFinDescanso] IS NOT NULL)");

        builder.HasCheckConstraint(
            "CK_HorarioAtencion_DescansoDentroHorario",
            "([HoraInicioDescanso] IS NULL AND [HoraFinDescanso] IS NULL) " +
            "OR ([HoraApertura] < [HoraInicioDescanso] " +
            "AND [HoraInicioDescanso] < [HoraFinDescanso] " +
            "AND [HoraFinDescanso] < [HoraCierre])");
    }
}