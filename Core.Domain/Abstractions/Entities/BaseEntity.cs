using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Abstractions.Entities
{
    public abstract class BaseEntity<T>
    {
       public T Id { get; set; }
    }
}
