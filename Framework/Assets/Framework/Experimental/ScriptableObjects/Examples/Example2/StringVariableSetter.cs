using Framework.Experimental.ScriptableObjects;
using UnityEngine;

public class StringVariableSetter : MonoBehaviour
{
    public StringVariable Variable;

    public void SetValue(string str)
    {
        Assertion.AssertNotNull(Variable);

        Variable.CurrentValue = str;
    }
}