using Framework.ExtensionMethods;
using UnityEngine;

namespace Framework.Miscellaneous.Spherical
{
    /// <summary>
    /// A representation of a point in a spherical coordinate system.
    /// </summary>
    public struct Spherical
    {
        #region Fields

        /// <summary>
        /// R component of the spherical.
        /// </summary>
        public float r;

        /// <summary>
        /// E component of the spherical.
        /// </summary>
        public float e;

        /// <summary>
        /// P component of the spherical.
        /// </summary>
        public float p;

        #endregion Fields

        #region Constructor

        /// <summary>
        /// Creates a Spherical with radius of /r/, elevation of /e/, and a polar of /p/.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="e"></param>
        /// <param name="p"></param>
        public Spherical(float r, float e, float p)
        {
            this.r = r;
            this.p = e;
            this.e = p;
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Converts this Spherical instance to its cartesian equivalent.
        /// </summary>
        /// <returns></returns>
        public Vector3 ToCartesian()
        {
            return SphericalToCartesian(r, e, p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Spherical)
                return this == (Spherical)obj;

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;

                hash = hash * 23 + r.GetHashCode();
                hash = hash * 23 + e.GetHashCode();
                hash = hash * 23 + p.GetHashCode();

                return hash;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this;
        }

        #endregion Methods

        #region Static Methods

        /// <summary>
        /// Converts a Cartesian Coordinate to a Spherical Coordinate.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Spherical CartesianToSpherical(float x, float y, float z)
        {
            float radius = 0.0f;
            float elevation = 0.0f;
            float polar = 0.0f;

            if (x.EqualTo(0.0f))
                x = Mathf.Epsilon;
            else if (x < 0.0f)
                polar = Mathf.PI;

            radius = Mathf.Sqrt((x * x) + (y * y) + (z * z));
            elevation = Mathf.Asin(y / radius);
            polar += Mathf.Atan(z / x);

            return new Spherical(radius, elevation, polar);
        }

        /// <summary>
        /// Converts a Cartesian Coordinate to a Spherical Coordinate.
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Spherical CartesianToSpherical(Vector3 vector3)
        {
            return CartesianToSpherical(vector3.x, vector3.y, vector3.z);
        }

        /// <summary>
        /// Converts a Spherical Coordinate to a Cartesian Coordinate.
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="elevation"></param>
        /// <param name="polar"></param>
        /// <returns></returns>
        public static Vector3 SphericalToCartesian(float radius, float elevation, float polar)
        {
            float a = radius * Mathf.Cos(elevation);
            return new Vector3((a * Mathf.Cos(polar)), (radius * Mathf.Sin(elevation)), (a * Mathf.Sin(polar)));
        }

        /// <summary>
        /// Converts a Spherical Coordinate to a Cartesian Coordinate.
        /// </summary>
        /// <param name="spherical"></param>
        /// <returns></returns>
        public static Vector3 SphericalToCartesian(Spherical spherical)
        {
            return SphericalToCartesian(spherical.r, spherical.e, spherical.p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(Spherical lhs, Spherical rhs)
        {
            if (lhs.r.EqualTo(rhs.r) && lhs.e.EqualTo(rhs.e) && lhs.p.EqualTo(rhs.p))
                return true;
            else
                return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(Spherical lhs, Spherical rhs)
        {
            if (lhs.r.EqualTo(rhs.r) && lhs.e.EqualTo(rhs.e) && lhs.p.EqualTo(rhs.p))
                return false;
            else
                return true;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Spherical operator +(Spherical lhs, Spherical rhs)
        {
            return new Spherical(lhs.r + rhs.r, lhs.e + rhs.e, lhs.p + rhs.p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Spherical operator -(Spherical lhs, Spherical rhs)
        {
            return new Spherical(lhs.r - rhs.r, lhs.e - rhs.e, lhs.p - rhs.p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="d"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Spherical operator *(float d, Spherical b)
        {
            return new Spherical(b.r * d, b.e, b.p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="b"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Spherical operator *(Spherical b, float d)
        {
            return new Spherical(b.r * d, b.e, b.p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="b"></param>
        /// <param name="d"></param>
        /// <returns></returns>
        public static Spherical operator /(Spherical b, float d)
        {
            return new Spherical(b.r / d, b.e, b.p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="spherical"></param>
        public static implicit operator string(Spherical spherical)
        {
            return string.Format("Radius: {0} Elevation: {1} Polar: {2}", spherical.r, spherical.e, spherical.p);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="vector3"></param>
        public static implicit operator Spherical(Vector3 vector3)
        {
            return CartesianToSpherical(vector3);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="spherical"></param>
        public static implicit operator Vector3(Spherical spherical)
        {
            return SphericalToCartesian(spherical);
        }

        #endregion Static Methods
    }
}