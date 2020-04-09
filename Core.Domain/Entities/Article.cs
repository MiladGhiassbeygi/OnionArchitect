using Core.Domain.Abstractions.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Entities
{
    public class Article:BaseEntity<long>,IEntity
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool Published { get; set; }
        public int UserId { get; set; }
        
    }
}
