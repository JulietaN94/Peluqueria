using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Peluqueria.Domain.Entities;

namespace Peluqueria.Infrastructure.Persistence.Configurations;

public class ClienteConfiguration : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Clientes");

        builder.HasKey(c => c.ClienteId);

        builder.Property(c => c.ClienteId)
            .ValueGeneratedOnAdd();

        builder.Property(c => c.NombreCompleto)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(c => c.Telefono)
            .IsRequired()
            .HasMaxLength(30);

        builder.Property(c => c.FechaAlta)
            .HasColumnType("datetime2(0)")
            .IsRequired();

        builder.Property(c => c.Activo)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(c => c.Observaciones)
            .HasMaxLength(1000);

        builder.HasIndex(c => c.Telefono)
            .HasDatabaseName("IX_Cliente_Telefono");

        builder.HasIndex(c => c.NombreCompleto)
            .HasDatabaseName("IX_Cliente_NombreCompleto");

        builder.HasMany(c => c.Turnos)
            .WithOne(t => t.Cliente)
            .HasForeignKey(t => t.ClienteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}