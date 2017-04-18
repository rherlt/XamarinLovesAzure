using System;

namespace DropIt.Web.Models.Base
{
    public abstract class BaseModel : IBaseModel
    {
        public virtual Guid Id { get; set; }
    } 
}