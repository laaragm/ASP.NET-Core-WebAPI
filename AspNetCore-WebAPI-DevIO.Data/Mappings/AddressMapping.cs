using AspNetCore_WebAPI_DevIO.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNetCore_WebAPI_DevIO.Data.Mappings
{
	public class AddressMapping : IEntityTypeConfiguration<Address>
	{
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(c => c.Street)
                .IsRequired()
                .HasColumnType("varchar(200)");

            builder.Property(c => c.Number)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.Property(c => c.Cep)
                .IsRequired()
                .HasColumnType("varchar(8)");

            builder.Property(c => c.Adjunct)
                .HasColumnType("varchar(250)");

            builder.Property(c => c.Neighbourhood)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(c => c.City)
                .IsRequired()
                .HasColumnType("varchar(100)");

            builder.Property(c => c.Street)
                .IsRequired()
                .HasColumnType("varchar(50)");

            builder.ToTable("Address");
        }
    }
}
