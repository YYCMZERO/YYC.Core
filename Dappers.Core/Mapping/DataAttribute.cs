using System;
using System.Collections.Generic;
using System.Text;

namespace Dappers.Mapping
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
