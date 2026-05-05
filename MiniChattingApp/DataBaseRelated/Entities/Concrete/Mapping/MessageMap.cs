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
    public class MessageMap : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Content)
                .IsRequired();
            builder.Property(m => m.ReceiverId)
                .IsRequired();
            builder.Property(m => m.SenderId)
                .IsRequired();
            builder.Property(m => m.Type)
                .HasDefaultValue("message");
           
        }
    }
}
