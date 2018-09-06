using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Framework.Miscellaneous.DicePool
{
    /// <summary>
    /// A pool of dice that can store its results.
    /// </summary>
    public class DicePool
    {
        #region Enums
        
        /// <summary>
        /// Type of Dice.
        /// </summary>
        public enum DiceType
        {
            /// <summary>
            /// A die with an 'n' number of faces.
            /// </summary>
            Custom = 1,

            /// <summary>
            /// A coin.
            /// </summary>
            D2 = 2,

            /// <summary>
            /// A tetrahedon.
            /// </summary>
            D4 = 4,

            /// <summary>
            /// A cube.
            /// </summary>
            D6 = 6,

            /// <summary>
            /// An octahedron.
            /// </summary>
            D8 = 8,

            /// <summary>
            /// A deltohedron.
            /// </summary>
            D10 = 10,

            /// <summary>
            /// A dodecahedron.
            /// </summary>
            D12 = 12,

            /// <summary>
            /// An icosahedron.
            /// </summary>
            D20 = 20,

            /// <summary>
            /// A zocchihedron.
            /// </summary>
            D100 = 100
        }

        #endregion Enums

        #region Fields

        [SerializeField]
        private DiceType _type = DiceType.D20;

        [SerializeField, HideInInspector]
        private int _faces = 20;

        [SerializeField]
        private int _size = 0;

        [SerializeField, HideInInspector]
        private List<int> _rolls = new List<int>();

        #endregion Fields

        #region Properties

        /// <summary>
        /// The Dice Type used by the Dice Pool.
        /// </summary>
        public DiceType Type
        {
            get { return _type; }
            set { SetType(value); }
        }

        /// <summary>
        /// Number of Faces of the Dice Type used by the Dice Pool.
        /// </summary>
        public int Faces
        {
            get { return _faces; }
            set { SetFaces(value); }
        }

        /// <summary>
        /// Size of the Dice Pool.
        /// </summary>
        public int Size
        {
            get { return _size; }
            set { _size = value; }
        }

        /// <summary>
        /// Rolls of the Dice Pool.
        /// </summary>
        public int[] Rolls
        {
            get { return _rolls.ToArray(); }
        }

        /// <summary>
        /// Highest Roll of the Dice Pool.
        /// </summary>
        public int HighestRoll
        {
            get { return _rolls.Max(); }
        }

        /// <summary>
        /// Lowest Roll of the Dice Pool.
        /// </summary>
        public int LowestRoll
        {
            get { return _rolls.Min(); }
        }

        /// <summary>
        /// Total Roll of the Dice Pool.
        /// </summary>
        public int TotalRoll
        {
            get { return _rolls.Sum(); }
        }

        /// <summary>
        /// Average of the Rolls of the Dice Pool.
        /// </summary>
        public float AverageRoll
        {
            get { return (float)_rolls.Average(); }
        }

        #endregion Properties

        #region Constructor

        /// <summary>
        /// Creates a Dice Pool with /size/ and Dice Type of /type/.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="type"></param>
        public DicePool(int size, DiceType type = DiceType.D20)
        {
            Size = size;
            Type = type;

            Reroll();
        }

        /// <summary>
        /// Creates a Dice Pool with /size/ and D/faces/.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="faces"></param>
        public DicePool(int size, int faces)
        {
            Size = size;
            Faces = faces;

            Reroll();
        }

        #endregion Constructor

        #region Methods

        /// <summary>
        /// Sets the Dice Type and Rerolls the Dice Pool.
        /// </summary>
        /// <param name="value"></param>
        public void SetType(DiceType value)
        {
            _type = value;

            _faces = (int)value;

            Reroll();
        }

        /// <summary>
        /// Sets the Faces (Dice Type) and Rerolls the Dice Pool.
        /// </summary>
        /// <param name="value"></param>
        public void SetFaces(int value)
        {
            _faces = value;

            if (Enum.IsDefined(typeof(DiceType), value))
                _type = (DiceType)value;
            else
                _type = DiceType.Custom;

            Reroll();
        }

        /// <summary>
        /// Rerolls this instance.
        /// </summary>
        public void Reroll()
        {
            _rolls.Clear();

            for (int i = 0; i < _size; i++)
                _rolls.Add(UnityEngine.Random.Range(1, _faces + 1));
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}D{1}", _size, _faces);
        }

        #endregion Methods
    }
}