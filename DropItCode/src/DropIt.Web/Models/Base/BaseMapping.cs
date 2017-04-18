using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DropIt.Web.Models.Base
{
    public abstract class BaseMapping<T> where T : class, new()
    {
        public abstract void Map(EntityTypeBuilder<T> b);
    }
}
