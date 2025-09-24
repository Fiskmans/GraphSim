using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GraphSim.Extensions
{
    public static class ReflectionExtensions
    {
        public static AttributeType GetAttribute<AttributeType>(this Enum value) 
            where AttributeType : Attribute
        {
            Type enumType = value.GetType();

            IEnumerable<AttributeType> attributes =
                enumType.GetMember(value.ToString()).First()
                .GetCustomAttributes(typeof(AttributeType), false)
                .Select(o => o as AttributeType);

            if (attributes.Count() == 0)
                return null;

            return attributes.First();
        }
    }
}
