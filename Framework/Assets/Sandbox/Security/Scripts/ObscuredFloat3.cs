namespace CodeStage.AntiCheat.ObscuredTypes
{
    using System;
    using UnityEngine;
    using Random = UnityEngine.Random;

    using Sandbox.SocketIo;

    /// <summary>
    /// Use it instead of regular <c>Vector3</c> for any cheating-sensitive variables.
    /// </summary>
    /// <strong>\htmlonly<font color="FF4040">WARNING:</font>\endhtmlonly Doesn't mimic regular type API, thus should be used with extra caution.</strong> Cast it to regular, not obscured type to work with regular APIs.<br/>
    /// <strong><em>Regular type is faster and memory wiser comparing to the obscured one!</em></strong>
    [Serializable]
    public struct ObscuredFloat3
    {
        private static int cryptoKey = 120207;
        private static readonly Float3 zero = new Float3(0f, 0f, 0f);

#if UNITY_EDITOR
        // For internal Editor usage only (may be useful for drawers).
        public static int cryptoKeyEditor = cryptoKey;
#endif

        [SerializeField]
        private int currentCryptoKey;

        [SerializeField]
        private Float3 hiddenValue;

        [SerializeField]
        private bool inited;

        [SerializeField]
        private Float3 fakeValue;

        [SerializeField]
        private bool fakeValueActive;

        private ObscuredFloat3(Float3 value)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(value);

            var detectorRunning = Detectors.ObscuredCheatingDetector.ExistsAndIsRunning;
            fakeValue = detectorRunning ? value : zero;
            fakeValueActive = detectorRunning;

            inited = true;
        }

        /// <summary>
        /// Mimics constructor of regular Float3.
        /// </summary>
        /// <param name="x">X component of the vector</param>
        /// <param name="y">Y component of the vector</param>
        /// <param name="z">Z component of the vector</param>
        public ObscuredFloat3(float x, float y, float z)
        {
            currentCryptoKey = cryptoKey;
            hiddenValue = Encrypt(x, y, z, currentCryptoKey);

            if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
            {
                fakeValue.X = x;
                fakeValue.Y = y;
                fakeValue.Z = z;
                fakeValueActive = true;
            }
            else
            {
                fakeValue = zero;
                fakeValueActive = false;
            }

            inited = true;
        }

        public float x
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.X);
                if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue.Y) > Detectors.ObscuredCheatingDetector.Instance.vector3Epsilon)
                {
                    Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
                }
                return decrypted;
            }

            set
            {
                hiddenValue.X = InternalEncryptField(value);
                if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
                {
                    fakeValue.X = value;
                    fakeValue.Y = InternalDecryptField(hiddenValue.Y);
                    fakeValue.Z = InternalDecryptField(hiddenValue.Z);
                    fakeValueActive = true;
                }
                else
                {
                    fakeValueActive = false;
                }
            }
        }

        public float y
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.Y);
                if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue.Y) > Detectors.ObscuredCheatingDetector.Instance.vector3Epsilon)
                {
                    Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
                }
                return decrypted;
            }

            set
            {
                hiddenValue.Y = InternalEncryptField(value);
                if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
                {
                    fakeValue.X = InternalDecryptField(hiddenValue.X);
                    fakeValue.Y = value;
                    fakeValue.Z = InternalDecryptField(hiddenValue.Z);
                    fakeValueActive = true;
                }
                else
                {
                    fakeValueActive = false;
                }
            }
        }

        public float z
        {
            get
            {
                var decrypted = InternalDecryptField(hiddenValue.Z);
                if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && Math.Abs(decrypted - fakeValue.Z) > Detectors.ObscuredCheatingDetector.Instance.vector3Epsilon)
                {
                    Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
                }
                return decrypted;
            }

            set
            {
                hiddenValue.Z = InternalEncryptField(value);
                if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
                {
                    fakeValue.X = InternalDecryptField(hiddenValue.X);
                    fakeValue.Y = InternalDecryptField(hiddenValue.Y);
                    fakeValue.Z = value;
                    fakeValueActive = true;
                }
                else
                {
                    fakeValueActive = false;
                }
            }
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return x;
                    case 1:
                        return y;
                    case 2:
                        return z;
                    default:
                        throw new IndexOutOfRangeException("Invalid ObscuredFloat3 index!");
                }
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    case 2:
                        z = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid ObscuredFloat3 index!");
                }
            }
        }

        /// <summary>
        /// Allows to change default crypto key of this type instances. All new instances will use specified key.<br/>
        /// All current instances will use previous key unless you call ApplyNewCryptoKey() on them explicitly.
        /// </summary>
        public static void SetNewCryptoKey(int newKey)
        {
            cryptoKey = newKey;
        }

        /// <summary>
        /// Use this simple encryption method to encrypt any Float3 value, uses default crypto key.
        /// </summary>
        public static Float3 Encrypt(Float3 value)
        {
            return Encrypt(value, 0);
        }

        /// <summary>
        /// Use this simple encryption method to encrypt any Float3 value, uses passed crypto key.
        /// </summary>
        public static Float3 Encrypt(Float3 value, int key)
        {
            return Encrypt(value.X, value.Y, value.Z, key);
        }

        /// <summary>
        /// Use this simple encryption method to encrypt Float3 components, uses passed crypto key.
        /// </summary>
        public static Float3 Encrypt(float x, float y, float z, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            Float3 result;
            result.X = ObscuredFloat.Encrypt(x, key);
            result.Y = ObscuredFloat.Encrypt(y, key);
            result.Z = ObscuredFloat.Encrypt(z, key);

            return result;
        }

        /// <summary>
        /// Use it to decrypt Float3 you got from Encrypt(), uses default crypto key.
        /// </summary>
        public static Float3 Decrypt(Float3 value)
        {
            return Decrypt(value, 0);
        }

        /// <summary>
        /// Use it to decrypt Float3 you got from Encrypt(), uses passed crypto key.
        /// </summary>
        public static Float3 Decrypt(Float3 value, int key)
        {
            if (key == 0)
            {
                key = cryptoKey;
            }

            Float3 result;
            result.X = ObscuredFloat.Decrypt(value.X, key);
            result.Y = ObscuredFloat.Decrypt(value.Y, key);
            result.Z = ObscuredFloat.Decrypt(value.Z, key);

            return result;
        }

        /// <summary>
        /// Use it after SetNewCryptoKey() to re-encrypt current instance using new crypto key.
        /// </summary>
        public void ApplyNewCryptoKey()
        {
            if (currentCryptoKey != cryptoKey)
            {
                hiddenValue = Encrypt(InternalDecrypt(), cryptoKey);
                currentCryptoKey = cryptoKey;
            }
        }

        /// <summary>
        /// Allows to change current crypto key to the new random value and re-encrypt variable using it.
        /// Use it for extra protection against 'unknown value' search.
        /// Just call it sometimes when your variable doesn't change to fool the cheater.
        /// </summary>
        public void RandomizeCryptoKey()
        {
            var decrypted = InternalDecrypt();

            do
            {
                currentCryptoKey = Random.Range(int.MinValue, int.MaxValue);
            } while (currentCryptoKey == 0);
            hiddenValue = Encrypt(decrypted, currentCryptoKey);
        }

        /// <summary>
        /// Allows to pick current obscured value as is.
        /// </summary>
        /// Use it in conjunction with SetEncrypted().<br/>
        /// Useful for saving data in obscured state.
        public Float3 GetEncrypted()
        {
            ApplyNewCryptoKey();
            return hiddenValue;
        }

        /// <summary>
        /// Allows to explicitly set current obscured value.
        /// </summary>
        /// Use it in conjunction with GetEncrypted().<br/>
        /// Useful for loading data stored in obscured state.
        public void SetEncrypted(Float3 encrypted)
        {
            inited = true;
            hiddenValue = encrypted;

            if (currentCryptoKey == 0)
            {
                currentCryptoKey = cryptoKey;
            }

            if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning)
            {
                fakeValueActive = false;
                fakeValue = InternalDecrypt();
                fakeValueActive = true;
            }
            else
            {
                fakeValueActive = false;
            }
        }

        /// <summary>
        /// Alternative to the type cast, use if you wish to get decrypted value 
        /// but can't or don't want to use cast to the regular type.
        /// </summary>
        /// <returns>Decrypted value.</returns>
        public Float3 GetDecrypted()
        {
            return InternalDecrypt();
        }

        private Float3 InternalDecrypt()
        {
            if (!inited)
            {
                currentCryptoKey = cryptoKey;
                hiddenValue = Encrypt(zero, cryptoKey);
                fakeValue = zero;
                fakeValueActive = false;
                inited = true;

                return zero;
            }

            Float3 value;

            value.X = ObscuredFloat.Decrypt(hiddenValue.X, currentCryptoKey);
            value.Y = ObscuredFloat.Decrypt(hiddenValue.Y, currentCryptoKey);
            value.Z = ObscuredFloat.Decrypt(hiddenValue.Z, currentCryptoKey);

            if (Detectors.ObscuredCheatingDetector.ExistsAndIsRunning && fakeValueActive && !CompareVectorsWithTolerance(value, fakeValue))
            {
                Detectors.ObscuredCheatingDetector.Instance.OnCheatingDetected();
            }

            return value;
        }

        private bool CompareVectorsWithTolerance(Float3 vector1, Float3 vector2)
        {
            var epsilon = Detectors.ObscuredCheatingDetector.Instance.vector3Epsilon;
            return Math.Abs(vector1.X - vector2.X) < epsilon &&
                   Math.Abs(vector1.Y - vector2.Y) < epsilon &&
                   Math.Abs(vector1.Z - vector2.Z) < epsilon;
        }

        private float InternalDecryptField(float encrypted)
        {
            return InternalDecryptField((int)encrypted);
        }

        private float InternalDecryptField(int encrypted)
        {
            var key = cryptoKey;

            if (currentCryptoKey != cryptoKey)
            {
                key = currentCryptoKey;
            }

            var result = ObscuredFloat.Decrypt(encrypted, key);
            return result;
        }

        private int InternalEncryptField(float encrypted)
        {
            var result = ObscuredFloat.Encrypt(encrypted, cryptoKey);
            return result;
        }

        #region operators, overrides, etc.
        //! @cond
        public static implicit operator ObscuredFloat3(Float3 value)
        {
            return new ObscuredFloat3(value);
        }

        public static implicit operator Float3(ObscuredFloat3 value)
        {
            return value.InternalDecrypt();
        }

        public static ObscuredFloat3 operator +(ObscuredFloat3 a, ObscuredFloat3 b)
        {
            return a.InternalDecrypt() + b.InternalDecrypt();
        }

        public static ObscuredFloat3 operator +(Float3 a, ObscuredFloat3 b)
        {
            return a + b.InternalDecrypt();
        }

        public static ObscuredFloat3 operator +(ObscuredFloat3 a, Float3 b)
        {
            return a.InternalDecrypt() + b;
        }

        public static ObscuredFloat3 operator -(ObscuredFloat3 a, ObscuredFloat3 b)
        {
            return a.InternalDecrypt() - b.InternalDecrypt();
        }

        public static ObscuredFloat3 operator -(Float3 a, ObscuredFloat3 b)
        {
            return a - b.InternalDecrypt();
        }

        public static ObscuredFloat3 operator -(ObscuredFloat3 a, Float3 b)
        {
            return a.InternalDecrypt() - b;
        }

        public static ObscuredFloat3 operator -(ObscuredFloat3 a)
        {
            return -a.InternalDecrypt();
        }

        public static ObscuredFloat3 operator *(ObscuredFloat3 a, float d)
        {
            return a.InternalDecrypt() * d;
        }

        public static ObscuredFloat3 operator *(float d, ObscuredFloat3 a)
        {
            return d * a.InternalDecrypt();
        }

        public static ObscuredFloat3 operator /(ObscuredFloat3 a, float d)
        {
            return a.InternalDecrypt() / d;
        }

        public static bool operator ==(ObscuredFloat3 lhs, ObscuredFloat3 rhs)
        {
            return lhs.InternalDecrypt() == rhs.InternalDecrypt();
        }

        public static bool operator ==(Float3 lhs, ObscuredFloat3 rhs)
        {
            return lhs == rhs.InternalDecrypt();
        }

        public static bool operator ==(ObscuredFloat3 lhs, Float3 rhs)
        {
            return lhs.InternalDecrypt() == rhs;
        }

        public static bool operator !=(ObscuredFloat3 lhs, ObscuredFloat3 rhs)
        {
            return lhs.InternalDecrypt() != rhs.InternalDecrypt();
        }

        public static bool operator !=(Float3 lhs, ObscuredFloat3 rhs)
        {
            return lhs != rhs.InternalDecrypt();
        }

        public static bool operator !=(ObscuredFloat3 lhs, Float3 rhs)
        {
            return lhs.InternalDecrypt() != rhs;
        }

        public override bool Equals(object other)
        {
            return InternalDecrypt().Equals(other);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// 
        /// <returns>
        /// A 32-bit signed integer hash code.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return InternalDecrypt().GetHashCode();
        }

        /// <summary>
        /// Returns a nicely formatted string for this vector.
        /// </summary>
        public override string ToString()
        {
            return InternalDecrypt().ToString();
        }

        /// <summary>
        /// Returns a nicely formatted string for this vector.
        /// </summary>
        public string ToString(string format)
        {
            return InternalDecrypt().ToString(format);
        }

        //! @endcond
        #endregion
    }
}