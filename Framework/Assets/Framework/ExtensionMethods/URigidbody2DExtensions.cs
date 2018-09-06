using UnityEngine;
using URigidbody2D = UnityEngine.Rigidbody2D;

namespace Framework.ExtensionMethods
{
    /// <summary>
    /// Extension methods to UnityEngine.Rigidbody2D.
    /// </summary>
    public static class URigidbody2DExtensions
    {
        #region Static Methods

        /// <summary>
        /// Shorthand method for 'rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x + /x/, rigidbody2D.velocity.y)'.
        /// </summary>
        /// <param name="rigidbody2D"></param>
        /// <param name="x"></param>
        public static void AddVelocity2DX(this URigidbody2D rigidbody2D, float x)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x + x, rigidbody2D.velocity.y);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y + /y/)'.
        /// </summary>
        /// <param name="rigidbody2D"></param>
        /// <param name="y"></param>
        public static void AddVelocity2DY(this URigidbody2D rigidbody2D, float y)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, rigidbody2D.velocity.y + y);
        }

        /// <summary>
        /// Shorthand method for 'if (rigidbody2D.velocity.magnitude > limit) rigidbody2D.velocity = rigidbody2D.velocity.normalized * /limit/'.
        /// </summary>
        /// <param name="rigidbody2D"></param>
        /// <param name="limit"></param>
        public static void LimitVelocity2D(this URigidbody2D rigidbody2D, float limit)
        {
            if (rigidbody2D.velocity.magnitude > limit)
                rigidbody2D.velocity = rigidbody2D.velocity.normalized * limit;
        }

        /// <summary>
        /// Shorthand method for 'rigidbody2D.velocity = new Vector2(/x/, rigidbody2D.velocity.y)'.
        /// </summary>
        /// <param name="rigidbody2D"></param>
        /// <param name="x"></param>
        public static void SetVelocity2DX(this URigidbody2D rigidbody2D, float x)
        {
            rigidbody2D.velocity = new Vector2(x, rigidbody2D.velocity.y);
        }

        /// <summary>
        /// Shorthand method for 'rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, /y/)'.
        /// </summary>
        /// <param name="rigidbody2D"></param>
        /// <param name="y"></param>
        public static void SetVelocity2DY(this URigidbody2D rigidbody2D, float y)
        {
            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, y);
        }

        #endregion Static Methods
    }
}