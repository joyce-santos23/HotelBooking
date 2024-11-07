using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data.Room
{
    public class RoomConfiguration : IEntityTypeConfiguration<Domain.Entities.Room>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Room> builder)
        {
            builder.HasKey(e => e.Id);

            builder.OwnsOne(e => e.Price, price =>
            {
                price.Property(p => p.Value).HasColumnName("Price_Value"); 
                price.Property(p => p.Currency).HasColumnName("Price_Currency");
            });
        }


    }
}
