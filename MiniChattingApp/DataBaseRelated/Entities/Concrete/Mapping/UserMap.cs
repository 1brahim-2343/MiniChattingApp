using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniChattingApp.DataBaseRelated.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniChattingApp.DataBaseRelated.Entities.Concrete.Mapping
{
    public class UserMap : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.IsVerified).HasDefaultValue(false);

            builder.Property(u => u.Email)
                .IsRequired();

            builder.Property(u => u.Username)
                .IsRequired();

            builder.HasMany(u => u.SentMessages)
                .WithOne(m => m.SenderUser)
                .HasForeignKey(m=>m.SenderId);

            builder.HasMany(u => u.ReceivedMessages)
                .WithOne(m => m.ReceiverUser)
                .HasForeignKey(m => m.ReceiverUser);


            builder.HasMany(u => u.SentFiles)
                .WithOne(f => f.SenderUser)
                .HasForeignKey(f => f.SenderId);

            builder.HasMany(u => u.ReceivedFiles)
                .WithOne(f => f.ReceiverUser)
                .HasForeignKey(f => f.ReceiverId);

        }
    }
}
