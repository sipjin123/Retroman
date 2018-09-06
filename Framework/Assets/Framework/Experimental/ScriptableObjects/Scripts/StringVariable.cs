using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [CreateAssetMenu(
        menuName = MENUITEM_BASE + "string",
        order = ORDER_BASE + 4)]
    public class StringVariable : Variable<string>
    {
    }
}