using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework
{
    public static class EnumExtensions
    {
        public static int ToInt(this Enum type)
        {
            return (int)(object)type;
        }

        public static bool Has<T>(this Enum type, T value)
        {
            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this Enum type, T value)
        {
            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }

        public static T Add<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Could not append value from enumerated type '{0}'.", typeof(T).Name), ex);
            }
        }

        public static T Remove<T>(this Enum type, T value)
        {
            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(string.Format("Could not remove value from enumerated type '{0}'.", typeof(T).Name), ex);
            }
        }

        public static T Random<T>(this Enum types)
        {
            var matches = Enum.GetValues(typeof(T))
                .Cast<T>()
                .Where(i => types.Has(i))
                .ToList();

            return matches.Random();
        }

        public static IEnumerable<Enum> GetFlags(this Enum p_value)
        {
            return GetFlags(p_value, Enum.GetValues(p_value.GetType()).Cast<Enum>().ToArray());
        }

        public static IEnumerable<Enum> GetIndividualFlags(this Enum p_value)
        {
            return GetFlags(p_value, GetFlagValues(p_value.GetType()).ToArray());
        }

        private static IEnumerable<Enum> GetFlags(Enum p_value, Enum[] p_values)
        {
            ulong bits = Convert.ToUInt64(p_value);
            List<Enum> results = new List<Enum>();

            for (int i = p_values.Length - 1; i >= 0; i--)
            {
                ulong mask = Convert.ToUInt64(p_values[i]);
                if (i == 0 && mask == 0L)
                {
                    break;
                }
                if ((bits & mask) == mask)
                {
                    results.Add(p_values[i]);
                    bits -= mask;
                }
            }

            if (bits != 0L)
            {
                return Enumerable.Empty<Enum>();
            }

            if (Convert.ToUInt64(p_value) != 0L)
            {
                return results.Reverse<Enum>();
            }

            if (bits == Convert.ToUInt64(p_value)
            && p_values.Length > 0
            && Convert.ToUInt64(p_values[0]) == 0L
            )
            {
                return p_values.Take(1);
            }

            return Enumerable.Empty<Enum>();
        }

        private static IEnumerable<Enum> GetFlagValues(Type p_enumType)
        {
            ulong flag = 0x1;
            foreach (var value in Enum.GetValues(p_enumType).Cast<Enum>())
            {
                ulong bits = Convert.ToUInt64(value);

                if (bits == 0L)
                {
                    continue;
                }// skip the zero value
                while (flag < bits)
                {
                    flag <<= 1;
                }

                if (flag == bits)
                {
                    yield return value;
                }
            }
        }
    }
}
