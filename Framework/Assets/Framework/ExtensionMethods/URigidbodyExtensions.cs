using UnityEngine;
using URigidbody = UnityEngine.Rigidbody;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// Extension methods to UnityEngine.Rigidbody.
    /// </summary>
    public static class URigidbodyExtensions
    {
        #region Static Methods

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(rigidbody.velocity.x + /x/, rigidbody.velocity.y, rigidbody.velocity.z)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="x"></param>
        public static void AddVelocityX(this URigidbody rigidbody, float x)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x + x, rigidbody.velocity.y, rigidbody.velocity.z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(rigidbody.velocity.x + /x/, rigidbody.velocity.y + /y/, rigidbody.velocity.z)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AddVelocityXY(this URigidbody rigidbody, float x, float y)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x + x, rigidbody.velocity.y + y, rigidbody.velocity.z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(rigidbody.velocity.x + /x/, rigidbody.velocity.y, rigidbody.velocity.z + /z/)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void AddVelocityXZ(this URigidbody rigidbody, float x, float z)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x + x, rigidbody.velocity.y, rigidbody.velocity.z + z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y + /y/, rigidbody.velocity.z)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="y"></param>
        public static void AddVelocityY(this URigidbody rigidbody, float y)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y + y, rigidbody.velocity.z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y + /y/, rigidbody.velocity.z + /z/)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void AddVelocityYZ(this URigidbody rigidbody, float y, float z)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y + y, rigidbody.velocity.z + z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, rigidbody.velocity.z + /z/)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="z"></param>
        public static void AddVelocityZ(this URigidbody rigidbody, float z)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, rigidbody.velocity.z + z);
        }

        /// <summary>
        /// Shorthand method for 'if (rigidbody.velocity.magnitude > limit) rigidbody.velocity = rigidbody.velocity.normalized * /limit/'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="limit"></param>
        public static void LimitVelocity(this URigidbody rigidbody, float limit)
        {
            if (rigidbody.velocity.magnitude > limit)
                rigidbody.velocity = rigidbody.velocity.normalized * limit;
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(/x/, rigidbody.velocity.y, rigidbody.velocity.z)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="x"></param>
        public static void SetVelocityX(this URigidbody rigidbody, float x)
        {
            rigidbody.velocity = new Vector3(x, rigidbody.velocity.y, rigidbody.velocity.z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(/x/, /y/, rigidbody.velocity.z)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void SetVelocityXY(this URigidbody rigidbody, float x, float y)
        {
            rigidbody.velocity = new Vector3(x, y, rigidbody.velocity.z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(/x/, rigidbody.velocity.y, /z/)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="x"></param>
        /// <param name="z"></param>
        public static void SetVelocityXZ(this URigidbody rigidbody, float x, float z)
        {
            rigidbody.velocity = new Vector3(x, rigidbody.velocity.y, z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(rigidbody.velocity.x, /y/, rigidbody.velocity.z)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="y"></param>
        public static void SetVelocityY(this URigidbody rigidbody, float y)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, y, rigidbody.velocity.z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(rigidbody.velocity.x, /y/, /z/)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void SetVelocityYZ(this URigidbody rigidbody, float y, float z)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, y, z);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, /z/)'.
        /// </summary>
        /// <param name="rigidbody"></param>
        /// <param name="z"></param>
        public static void SetVelocityZ(this URigidbody rigidbody, float z)
        {
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y, z);
        }

        #endregion Static Methods
    }
}