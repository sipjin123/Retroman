using System;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// Extension methods for System.Enum with the Flags attribute.
    /// </summary>
    public static class EnumFlagsExtensions
    {
        #region Static Methods

        /// <summary>
        /// Is enum equal to T?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool Is<T>(this Enum e, T v)
        {
            try
            {
                int currentValue = (int)((object)e);
                int newValue = (int)((object)v);

                return currentValue == newValue;
            }
            catch { return false; }
        }

        /// <summary>
        /// Does enum contain T?
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static bool Has<T>(this Enum e, T v)
        {
            try
            {
                int currentValue = (int)((object)e);
                int newValue = (int)((object)v);

                if (newValue == 0)
                {
                    return (currentValue == 0);
                }
                else
                    return ((currentValue & newValue) == newValue);
            }
            catch { return false; }
        }

        /// <summary>
        /// Add T to enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static T Add<T>(this Enum e, T v)
        {
            try
            {
                int newValue = (int)((object)v);

                if (newValue == 0)
                    return (T)((object)(0));
                else
                {
                    int currentValue = (int)((object)e);
                    return (T)((object)(currentValue | newValue));
                }
            }
            catch (Exception ex) { throw new ArgumentException(string.Format("Could not add value to enumerated type '{0}'.", typeof(T).Name), ex); }
        }

        /// <summary>
        /// Remove T from enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static T Remove<T>(this Enum e, T v)
        {
            try
            {
                int newValue = (int)((object)v);

                if (newValue == 0)
                    return (T)((object)e);
                else
                {
                    int currentValue = (int)((object)e);
                    return (T)((object)(currentValue & ~newValue));
                }
            }
            catch (Exception ex) { throw new ArgumentException(string.Format("Could not remove value to enumerated type '{0}'.", typeof(T).Name), ex); }
        }

        /// <summary>
        /// Toggle T from enum.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="e"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public static T Toggle<T>(this Enum e, T v)
        {
            try
            {
                int newValue = (int)((object)v);

                if (newValue == 0)
                    return (T)(object)e;
                else
                {
                    int currentValue = (int)((object)e);
                    return (T)((object)(currentValue ^ newValue));
                }
            }
            catch (Exception ex) { throw new ArgumentException(string.Format("Could not toggle value to enumerated type '{0}'.", typeof(T).Name), ex); }
        }

        #endregion Static Methods
    }
}