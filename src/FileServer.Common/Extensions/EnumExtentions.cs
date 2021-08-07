using System;
using System.Collections.Concurrent;
using System.ComponentModel;

namespace FileServer.Common.Extensions
{
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static partial class EnumExtentions
    {
        private static readonly ConcurrentDictionary<string, string> DescriptionCache = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// 获取枚举类型的Description说明
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToDescription(this Enum value)
        {
            var type = value.GetType();
            var info = type.GetField(value.ToString());
            var key = type.FullName + info.Name;
            if (!DescriptionCache.TryGetValue(key, out string desc))
            {
                var attrs = info.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs.Length < 1)
                    desc = string.Empty;
                else
                    desc = attrs[0] is DescriptionAttribute
                        descriptionAttribute
                        ? descriptionAttribute.Description
                        : value.ToString();

                DescriptionCache.TryAdd(key, desc);
            }

            return desc;
        }
    }
}
