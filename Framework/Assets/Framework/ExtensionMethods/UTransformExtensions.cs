using System;
using System.Linq;
using UnityEngine;
using UTransform = UnityEngine.Transform;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// Extension methods to UnityEngine.Transform.
    /// </summary>
    public static class UTransformExtensions
    {
        #region Static Methods

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(transform.position.x + /x/, transform.position.y, transform.position.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void AddPositionX(this UTransform transform, float x)
        {
            transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(transform.position.x + /x/, transform.position.y + /y/, transform.position.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AddPositionXY(this UTransform transform, float x, float y)
        {
            transform.position = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(transform.position.x + /x/, transform.position.y, transform.position.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void AddPositionXZ(this UTransform transform, float x, float z)
        {
            transform.position = new Vector3(transform.position.x + x, transform.position.y, transform.position.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(transform.position.x, transform.position.y + /y/, transform.position.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void AddPositionY(this UTransform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + y, transform.position.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(transform.position.x, transform.position.y + /y/, transform.position.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void AddPositionYZ(this UTransform transform, float y, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + y, transform.position.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void AddPositionZ(this UTransform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(transform.localPosition.x + /x/, transform.localPosition.y, transform.localPosition.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void AddPositionLocalX(this UTransform transform, float x)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(transform.localPosition.x + /x/, transform.localPosition.y + /y/, transform.localPosition.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AddPositionLocalXY(this UTransform transform, float x, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y + y, transform.localPosition.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(transform.localPosition.x + /x/, transform.localPosition.y, transform.localPosition.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void AddPositionLocalXZ(this UTransform transform, float x, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x + x, transform.localPosition.y, transform.localPosition.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + /y/, transform.localPosition.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void AddPositionLocalY(this UTransform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + y, transform.localPosition.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + /y/, transform.localPosition.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void AddPositionLocalYZ(this UTransform transform, float y, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y + y, transform.localPosition.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void AddPositionLocalZ(this UTransform transform, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, transform.localPosition.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(transform.eulerAngles.x + /x/, transform.eulerAngles.y, transform.eulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void AddRotationX(this UTransform transform, float x)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + x, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(transform.eulerAngles.x + /x/, transform.eulerAngles.y + /y/, transform.eulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AddRotationXY(this UTransform transform, float x, float y)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + x, transform.eulerAngles.y + y, transform.eulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(transform.eulerAngles.x + /x/, transform.eulerAngles.y, transform.eulerAngles.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void AddRotationXZ(this UTransform transform, float x, float z)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x + x, transform.eulerAngles.y, transform.eulerAngles.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + /y/, transform.eulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void AddRotationY(this UTransform transform, float y)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + y, transform.eulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + /y/, transform.eulerAngles.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void AddRotationYZ(this UTransform transform, float y, float z)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + y, transform.eulerAngles.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void AddRotationZ(this UTransform transform, float z)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x + /x/, transform.localEulerAngles.y, transform.localEulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void AddRotationLocalX(this UTransform transform, float x)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x + x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x + /x/, transform.localEulerAngles.y + /y/, transform.localEulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AddRotationLocalXY(this UTransform transform, float x, float y)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x + x, transform.localEulerAngles.y + y, transform.localEulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x + /x/, transform.localEulerAngles.y, transform.localEulerAngles.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void AddRotationLocalXZ(this UTransform transform, float x, float z)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x + x, transform.localEulerAngles.y, transform.localEulerAngles.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + /y/, transform.localEulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void AddRotationLocalY(this UTransform transform, float y)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + y, transform.localEulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + /y/, transform.localEulerAngles.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void AddRotationLocalYZ(this UTransform transform, float y, float z)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y + y, transform.localEulerAngles.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void AddRotationLocalZ(this UTransform transform, float z)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, transform.localEulerAngles.z + z);
        }

        /// <summary>
        ///  Shorthand method for 'transform.localScale = new Vector3(transform.localScale.x + /x/, transform.localScale.y, transform.localScale.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void AddScaleLocalX(this UTransform transform, float x)
        {
            transform.localScale = new Vector3(transform.localScale.x + x, transform.localScale.y, transform.localScale.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(transform.localScale.x + /x/, transform.localScale.y + /y/, transform.localScale.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AddScaleLocalXY(this UTransform transform, float x, float y)
        {
            transform.localScale = new Vector3(transform.localScale.x + x, transform.localScale.y + y, transform.localScale.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(transform.localScale.x + /x/, transform.localScale.y, transform.localScale.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void AddScaleLocalXZ(this UTransform transform, float x, float z)
        {
            transform.localScale = new Vector3(transform.localScale.x + x, transform.localScale.y, transform.localScale.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + /y/, transform.localScale.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void AddScaleLocalY(this UTransform transform, float y)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + y, transform.localScale.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + /y/, transform.localScale.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void AddScaleLocalYZ(this UTransform transform, float y, float z)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + y, transform.localScale.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void AddScaleLocalZ(this UTransform transform, float z)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z + z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = Vector3.zero'.
        /// </summary>
        /// <param name="transform"></param>
        public static void ResetPosition(this UTransform transform)
        {
            transform.position = Vector3.zero;
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.identity'.
        /// </summary>
        /// <param name="transform"></param>
        public static void ResetRotation(this UTransform transform)
        {
            transform.rotation = Quaternion.identity;
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = Vector3.zero'.
        /// </summary>
        /// <param name="transform"></param>
        public static void ResetPositionLocal(this UTransform transform)
        {
            transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.identity'.
        /// </summary>
        /// <param name="transform"></param>
        public static void ResetRotationLocal(this UTransform transform)
        {
            transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = Vector3.one'.
        /// </summary>
        /// <param name="transform"></param>
        public static void ResetScaleLocal(this UTransform transform)
        {
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Shorthand method for 'transform.position = Vector3.zero; transform.rotation = Quaternion.identity; transform.localScale = Vector3.one'.
        /// </summary>
        /// <param name="transform"></param>
        public static void Reset(this UTransform transform)
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = Vector3.zero; transform.localRotation = Quaternion.identity; transform.localScale = Vector3.one'.
        /// </summary>
        /// <param name="transform"></param>
        public static void ResetLocal(this UTransform transform)
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
            transform.localScale = Vector3.one;
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(/x/, transform.position.y, transform.position.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void SetPositionX(this UTransform transform, float x)
        {
            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(/x/, /y/, transform.position.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetPositionXY(this UTransform transform, float x, float y)
        {
            transform.position = new Vector3(x, y, transform.position.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(/x/, transform.position.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void SetPositionXZ(this UTransform transform, float x, float z)
        {
            transform.position = new Vector3(x, transform.position.y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(transform.position.x, /y/, transform.position.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void SetPositionY(this UTransform transform, float y)
        {
            transform.position = new Vector3(transform.position.x, y, transform.position.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(transform.position.x, /y/, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void SetPositionYZ(this UTransform transform, float y, float z)
        {
            transform.position = new Vector3(transform.position.x, y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.position = new Vector3(transform.position.x, transform.position.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void SetPositionZ(this UTransform transform, float z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(/x/, transform.localPosition.y, transform.localPosition.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void SetPositionLocalX(this UTransform transform, float x)
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, transform.localPosition.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(/x/, /y/, transform.localPosition.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetPositionLocalXY(this UTransform transform, float x, float y)
        {
            transform.localPosition = new Vector3(x, y, transform.localPosition.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(/x/, transform.localPosition.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void SetPositionLocalXZ(this UTransform transform, float x, float z)
        {
            transform.localPosition = new Vector3(x, transform.localPosition.y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(transform.localPosition.x, /y/, transform.localPosition.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void SetPositionLocalY(this UTransform transform, float y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, transform.localPosition.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(transform.localPosition.x, /y/, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void SetPositionLocalYZ(this UTransform transform, float y, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void SetPositionLocalZ(this UTransform transform, float z)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(/x/, transform.eulerAngles.y, transform.eulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void SetRotationX(this UTransform transform, float x)
        {
            transform.rotation = Quaternion.Euler(x, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(/x/, /y/, transform.eulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetRotationXY(this UTransform transform, float x, float y)
        {
            transform.rotation = Quaternion.Euler(x, y, transform.eulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(/x/, transform.eulerAngles.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void SetRotationXZ(this UTransform transform, float x, float z)
        {
            transform.rotation = Quaternion.Euler(x, transform.eulerAngles.y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(transform.eulerAngles.x, /y/, transform.eulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void SetRotationY(this UTransform transform, float y)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, y, transform.eulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(transform.eulerAngles.x, /y/, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void SetRotationYZ(this UTransform transform, float y, float z)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void SetRotationZ(this UTransform transform, float z)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(/x/, transform.localEulerAngles.y, transform.localEulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void SetRotationLocalX(this UTransform transform, float x)
        {
            transform.localRotation = Quaternion.Euler(x, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(/x/, /y/, transform.localEulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetRotationLocalXY(this UTransform transform, float x, float y)
        {
            transform.localRotation = Quaternion.Euler(x, y, transform.localEulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(/x/, transform.localEulerAngles.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void SetRotationLocalXZ(this UTransform transform, float x, float z)
        {
            transform.localRotation = Quaternion.Euler(x, transform.localEulerAngles.y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, /y/, transform.localEulerAngles.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void SetRotationLocalY(this UTransform transform, float y)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, y, transform.localEulerAngles.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, /y/, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void SetRotationLocalYZ(this UTransform transform, float y, float z)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void SetRotationLocalZ(this UTransform transform, float z)
        {
            transform.localRotation = Quaternion.Euler(transform.localEulerAngles.x, transform.localEulerAngles.y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(/x/, transform.localScale.y, transform.localScale.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        public static void SetScaleLocalX(this UTransform transform, float x)
        {
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(/x/, /y/, transform.localScale.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetScaleLocalXY(this UTransform transform, float x, float y)
        {
            transform.localScale = new Vector3(x, y, transform.localScale.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(/x/, transform.localScale.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void SetScaleLocalXZ(this UTransform transform, float x, float z)
        {
            transform.localScale = new Vector3(x, transform.localScale.y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(transform.localScale.x, /y/, transform.localScale.z)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        public static void SetScaleLocalY(this UTransform transform, float y)
        {
            transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(transform.localScale.x, /y/, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void SetScaleLocalYZ(this UTransform transform, float y, float z)
        {
            transform.localScale = new Vector3(transform.localScale.x, y, z);
        }

        /// <summary>
        /// Shorthand method for 'transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, /z/)'.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="z"></param>
        public static void SetScaleLocalZ(this UTransform transform, float z)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, z);
        }

        /// <summary>
        /// Returns the farthest transform from origin.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="transforms"></param>
        /// <returns></returns>
        public static UTransform GetFarthest(this UTransform transform, params UTransform[] transforms)
        {
            if (transforms == null)
                throw new ArgumentNullException("Cannot be null.", "transforms");

            if (transforms.Length == 0)
                throw new ArgumentException("Cannot be empty.", "transforms");

            return transforms.OrderByDescending(x => Vector3.Distance(transform.position, x.position)).FirstOrDefault();
        }

        /// <summary>
        /// Returns the nearest transform from origin.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="transforms"></param>
        /// <returns></returns>
        public static UTransform GetNearest(this UTransform transform, params UTransform[] transforms)
        {
            if (transforms == null)
                throw new ArgumentNullException("Cannot be null.", "transforms");

            if (transforms.Length == 0)
                throw new ArgumentException("Cannot be empty.", "transforms");

            return transforms.OrderBy(x => Vector3.Distance(transform.position, x.position)).FirstOrDefault();
        }

        #endregion Static Methods
    }
}