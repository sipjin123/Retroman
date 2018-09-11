using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using ULayerMask = UnityEngine.LayerMask;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// Extension methods to UnityEngine.LayerMask.
    /// </summary>
    public static class ULayerMaskExtensions
    {
        #region Static Methods

        /// <summary>
        /// Gets the collision mask of this /layer/.
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static int GetPhysicsCollisionMask(int layer)
        {
            int collisionMask = 0;

            for (int i = 0; i < 32; i++)
                collisionMask |= (Physics.GetIgnoreLayerCollision(layer, i) ? 0 : 1) << i;

            return collisionMask;
        }

        /// <summary>
        /// Creates a LayerMask with the provided /layers/.
        /// </summary>
        /// <param name="layers"></param>
        /// <returns></returns>
        public static ULayerMask Create(params int[] layers)
        {
            if (layers == null)
                throw new ArgumentNullException("layers");

            if (layers.Length == 0)
                throw new ArgumentException("layers" + " is empty.");

            ULayerMask layerMask = 0;

            for (int i = 0; i < layers.Length; i++)
                layerMask |= 1 << layers[i];

            return layerMask;
        }

        /// <summary>
        /// Creates a LayerMask with the provided /layers/.
        /// </summary>
        /// <param name="layers"></param>
        /// <returns></returns>
        public static ULayerMask Create(params string[] layers)
        {
            if (layers == null)
                throw new ArgumentNullException("layers");

            if (layers.Length == 0)
                throw new ArgumentException("layers" + " is empty.");

            ULayerMask layerMask = 0;
            int layer = 0;

            for (int i = 0; i < layers.Length; i++)
            {
                layer = ULayerMask.NameToLayer(layers[i]);

                if (layer >= 0 && layer <= 31)
                    layerMask |= 1 << layer;
            }

            return layerMask;
        }

        /// <summary>
        /// Creates a LayerMask with the provided string /layers/.
        /// </summary>
        /// <param name="layers"></param>
        /// <returns></returns>
        public static ULayerMask Create(string layers)
        {
            string[] layerNames = Regex.Split(layers, ", ");

            if (layerNames != null && layerNames.Length > 0)
                return Create(layerNames);
            else
                return 0;
        }

        /// <summary>
        /// Returns LayerMask as string.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string AsString(this ULayerMask layerMask, string separator = ", ")
        {
            return string.Join(separator, layerMask.AsStrings());
        }

        /// <summary>
        /// Returns LayerMask as an array of string.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static string[] AsStrings(this ULayerMask layerMask)
        {
            List<string> layerMaskStrings = new List<string>();

            int layerMaskInt = 0;
            string layerMaskString = "";

            for (int i = 0; i < 32; ++i)
            {
                layerMaskInt = 1 << i;

                if ((layerMask & layerMaskInt) == layerMaskInt)
                {
                    layerMaskString = ULayerMask.LayerToName(i);

                    if (!string.IsNullOrEmpty(layerMaskString))
                    {
                        layerMaskStrings.Add(layerMaskString);
                    }
                }
            }

            return layerMaskStrings.ToArray();
        }

        /// <summary>
        /// Adds /layers/ to LayerMask.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layers"></param>
        /// <returns></returns>
        public static ULayerMask Add(this ULayerMask layerMask, params int[] layers)
        {
            return layerMask | Create(layers);
        }

        /// <summary>
        /// Adds /layers/ to LayerMask.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layers"></param>
        /// <returns></returns>
        public static ULayerMask Add(this ULayerMask layerMask, params string[] layers)
        {
            return layerMask | Create(layers);
        }

        /// <summary>
        /// Returns the inverse of LayerMask.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <returns></returns>
        public static ULayerMask Inverse(this ULayerMask layerMask)
        {
            return ~layerMask;
        }

        /// <summary>
        /// Removes /layers/ from LayerMask.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layers"></param>
        /// <returns></returns>
        public static ULayerMask Remove(this ULayerMask layerMask, params int[] layers)
        {
            return ~(layerMask.Inverse() | Create(layers));
        }

        /// <summary>
        /// Removes /layers/ from LayerMask.
        /// </summary>
        /// <param name="layerMask"></param>
        /// <param name="layers"></param>
        /// <returns></returns>
        public static ULayerMask Remove(this ULayerMask layerMask, params string[] layers)
        {
            return ~(layerMask.Inverse() | Create(layers));
        }

        #endregion Static Methods
    }
}