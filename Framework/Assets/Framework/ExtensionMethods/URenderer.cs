using System;
using UnityEngine;
using URenderer = UnityEngine.Renderer;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// Extension methods related to UnityEngine.Renderer.
    /// </summary>
    public static class EMRendering
    {
        #region Static Methods

        /// <summary>
        /// Is renderer visible from /camera/.
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="camera"></param>
        /// <returns></returns>
        public static bool IsVisibleFrom(this URenderer renderer, Camera camera)
        {
            if (camera == null)
                throw new ArgumentNullException("Cannot be null.", "camera");

            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);

            return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
        }

        #endregion Static Methods
    }
}