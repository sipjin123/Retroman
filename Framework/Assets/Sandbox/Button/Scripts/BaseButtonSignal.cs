using System;

using UnityEngine;

namespace Framework
{
    using Sandbox.ButtonSandbox;

    using Sirenix.OdinInspector;
    using Sirenix.Serialization;

    /// <summary>
    /// Base signal for all button events.
    /// </summary>
    [Serializable]
    public class BaseButtonSignal
    {
        /// <summary>
        /// The type of button the signal is for.
        /// </summary>
        public ButtonType Button { get; set; }

        /// <summary>
        /// Optional button signal data.
        /// </summary>
        public object Data { get; set; }
    }
}
