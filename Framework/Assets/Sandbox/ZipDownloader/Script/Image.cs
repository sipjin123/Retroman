using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace Sandbox.ZipDownloader
{
    /// <summary>
    /// A simple image loader class.
    /// </summary>
    public class Image
    {
        /// <summary>
        /// Load png file with the given path.
        /// </summary>
        public static Texture2D LoadPNG(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (File.Exists(filePath))
            {
                fileData = File.ReadAllBytes(filePath);
                tex = new Texture2D(1, 1);

                // automatically resize the texture by its dimensions.
                tex.LoadImage(fileData);
            }
            return tex;
        }
    }
}