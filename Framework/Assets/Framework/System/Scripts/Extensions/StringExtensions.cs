using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using Common.Utils;

using UColor = UnityEngine.Color;

namespace Framework
{
    using Sandbox.Security;

    public static class StringExtension
    {
        public static readonly string DOUBLE_QOUTE = @"""";
        public static readonly string QOUTE = @"\""";
        public static readonly AESEncyrption Encryption = AESEncryptionFactory.CreateDefaultEncryption();
        public static readonly TextInfo TextInfo = new CultureInfo("en-US", false).TextInfo; 

        public static string ToTitleCase(this string str)
        {
            return TextInfo.ToTitleCase(str.ToLower());
        }
        
        public static byte[] ToASCIIBytes(this string str)
        {
            return Encoding.ASCII.GetBytes(str);
        }

        public static byte[] ToUTF8Bytes(this string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }
        
        public static byte[] FromBase64(this string str)
        {
            return Convert.FromBase64String(str);
        }

        public static string Encrypt(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            return Encryption.Encrypt(str);
        }

        public static string Decrypt(this string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            return Encryption.Decrypt(str);
        }

        public static int ToInt(this string str)
        {
            int result = -1;

            int.TryParse(str, out result);

            return result;
        }

        public static float ToFloat(this string str)
        {
            float result = -1f;

            float.TryParse(str, out result);

            return result;
        }

        public static double ToDouble(this string str)
        {
            double result = -1.0f;

            double.TryParse(str, out result);

            return result;
        }

        public static long ToLong(this string str)
        {
            long result = -1L;

            long.TryParse(str, out result);

            return result;
        }

        public static string ToDQ(this string str)
        {
            return DOUBLE_QOUTE + str + DOUBLE_QOUTE;
        }

        public static string ToSQ(this string str)
        {
            return QOUTE + str + QOUTE;
        }
        
        public static Texture2D ToTexture(this string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(1, 1);

                // automatically resize the texture by its dimensions.
                tex.LoadImage(fileData);
                tex.filterMode = FilterMode.Bilinear;
                tex.wrapMode = TextureWrapMode.Clamp;
            }

            return tex;
        }

        public static Sprite ToSprite(this string filePath)
        {
            Texture2D tex = filePath.ToTexture();
            return Sprite.Create(tex, new Rect(Vector2.zero, new Vector2(tex.width, tex.height)), Vector2.zero);
        }

        public static bool IsEnum<T>(this string strValue) where T : struct, IConvertible
        {
            T t = default(T);
            return Enum.TryParse<T>(strValue, out t);
        }

        public static T ToEnum<T>(this string strValue) where T : struct, IConvertible
        {
            T t = default(T);
            Assertion.Assert(typeof(T).IsEnum, D.ERROR + " T:{0} is not an Enum! strValue:{1}\n", typeof(T), strValue);
            Assertion.Assert(Enum.TryParse<T>(strValue, out t), D.ERROR + " Enum:{0} is not an Enum!\n", strValue);
            return t;
        }

        public static string ToCurrencyFormat(this int val)
        {
            //string.Format("{0:n}", 1234);     //Output: 1,234.00
            //string.Format("{0:n0}", 9876);    //no digits after the decimal point. Output: 9,876
            return string.Format("{0:n0}", val);
        }

        public static UColor HexToColor(this string hex)
        {
            UColor color = UColor.black;
            ColorUtility.TryParseHtmlString(hex, out color);

            return color;
        }

        public static T ToJson<T>(this string str) where T : IJson
        {
            return JsonUtility.FromJson<T>(str);
        }
    }
}
