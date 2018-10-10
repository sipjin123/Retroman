using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Framework
{
    public static class ByteExtensions
    {
        public static string ToBaseString(this byte[] val)
        {
            return Convert.ToString(val);
        }

        public static string ToBase64(this byte[] val)
        {
            return Convert.ToBase64String(val);
        }

        public static Texture2D ToTexture(this byte[] bytes)
        {
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(bytes);
            //texture.alphaIsTransparency = true;
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;

            return texture;
        }

        public static Sprite ToSprite(this byte[] bytes)
        {
            return bytes.ToTexture().ToSprite();
        }
    }
}
