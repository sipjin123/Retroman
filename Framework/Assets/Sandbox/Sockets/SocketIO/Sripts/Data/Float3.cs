using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using Quobject.SocketIoClientDotNet.Client;

using MiniJSON;

using Sirenix.OdinInspector;

namespace Sandbox.SocketIo
{
    public struct Float3
    {
        public float X;
        public float Y;
        public float Z;

        public Float3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public Float3(Vector3 v)
        {
            X = v.x;
            Y = v.y;
            Z = v.z;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", X, Y, Z);
        }

        public string ToString(string format)
        {
            return string.Format(format, X, Y, Z);
        }

        #region operators, overrides, etc.
        
        public static Float3 operator +(Float3 a, Float3 b)
        {
            Float3 c;
            c.X = a.X + b.X;
            c.Y = a.Y + b.Y;
            c.Z = a.Z + b.Z;
            return c;
        }
        
        public static Float3 operator -(Float3 a, Float3 b)
        {
            Float3 c;
            c.X = a.X - b.X;
            c.Y = a.Y - b.Y;
            c.Z = a.Z - b.Z;
            return c;
        }

        public static Float3 operator -(Float3 a)
        {
            return -a;
        }

        public static Float3 operator *(Float3 a, float d)
        {
            Float3 c;
            c.X = a.X * d;
            c.Y = a.Y * d;
            c.Z = a.Z * d;
            return c;
        }

        public static Float3 operator *(float d, Float3 a)
        {
            Float3 c;
            c.X = a.X * d;
            c.Y = a.Y * d;
            c.Z = a.Z * d;
            return c;
        }
        
        public static Float3 operator /(Float3 a, float d)
        {
            Float3 c;
            c.X = a.X / d;
            c.Y = a.Y / d;
            c.Z = a.Z / d;
            return c;
        }

        public static Float3 operator /(float d, Float3 a)
        {
            Float3 c;
            c.X = a.X / d;
            c.Y = a.Y / d;
            c.Z = a.Z / d;
            return c;
        }

        public static bool operator ==(Float3 a, Float3 b)
        {
            bool bX = a.X == b.X;
            bool bY = a.Y == b.Y;
            bool bZ = a.Z == b.Z;
            return bX && bY && bZ;
        }
        
        public static bool operator !=(Float3 a, Float3 b)
        {
            bool bX = a.X != b.X;
            bool bY = a.Y != b.Y;
            bool bZ = a.Z != b.Z;
            return bX && bY && bZ;
        }
        
        public override bool Equals(object other)
        {
            return Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = -307843816;
            hashCode = hashCode * -1521134295 + base.GetHashCode();
            hashCode = hashCode * -1521134295 + X.GetHashCode();
            hashCode = hashCode * -1521134295 + Y.GetHashCode();
            hashCode = hashCode * -1521134295 + Z.GetHashCode();
            return hashCode;
        }

        #endregion
    }
}