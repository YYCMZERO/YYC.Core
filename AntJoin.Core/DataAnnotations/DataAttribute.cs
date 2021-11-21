using System;

namespace AntJoin.Core.DataAnnotations
{
    public abstract class DataAttribute : Attribute
    {
        public string Name { get; set; }

        public string Storage { get; set; }

        protected DataAttribute()
        {
        }
    }
}
