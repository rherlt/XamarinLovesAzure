using System;
using DropIt.Web.Models.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DropIt.Web.Models
{
    public class Drop : BaseModel
    {
        public virtual Guid? CreatorId { get; set; }

        public virtual double? Lon { get; set; }

        public virtual double? Lat { get; set; }

        public virtual double? Alt { get; set; }

        public virtual DateTime? Date { get; set; }

        public virtual DateTime? ValidTo { get; set; }

        public virtual bool? IsValidForever { get; set; }

        public virtual string Title { get; set; }

        public virtual string Message { get; set; }

        internal class Mapping : BaseMapping<Drop>
        {
            public override void Map(EntityTypeBuilder<Drop> b)
            {
                b.ToTable("Drop");
                b.HasKey(x => x.Id);
                b.Property(x => x.CreatorId).IsRequired();
                b.Property(x => x.Lon).IsRequired();
                b.Property(x => x.Lat).IsRequired();
                b.Property(x => x.Alt);
                b.Property(x => x.Date).IsRequired();
                b.Property(x => x.ValidTo).IsRequired(false);
                b.Property(x => x.IsValidForever).IsRequired();
                b.Property(x => x.Title).IsRequired();
                b.Property(x => x.Message).IsRequired();
            }
        }
    }
}