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
    public class FileMessageMap : IEntityTypeConfiguration<FileMessage>
    {
        public void Configure(EntityTypeBuilder<FileMessage> builder)
        {
            builder.ToTable("FileMessages");
            builder.HasKey(f => f.Id);

            builder.Property(f => f.Path)
                .IsRequired();
            builder.Property(f => f.SenderId)
                .IsRequired();
            builder.Property(f => f.ReceiverId)
                .IsRequired();

        }
    }
}
