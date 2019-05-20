using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace OpenDnsLogs.Domain.Extensions
{
    public static class EnumExtensions
    {
        public static string ToName(this Enum value)
        {
            return value.GetType().GetField(value.ToString())
                .CustomAttributes.Where(x => x.AttributeType.Name == typeof(DisplayAttribute).Name)
                .FirstOrDefault().NamedArguments.FirstOrDefault().TypedValue.Value.ToString();
        }
    }
}