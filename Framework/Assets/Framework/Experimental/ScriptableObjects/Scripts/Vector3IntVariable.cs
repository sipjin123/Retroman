using UnityEngine;

namespace Framework.Experimental.ScriptableObjects
{
    [CreateAssetMenu(
        menuName = MENUITEM_BASE + "Vector3Int",
        order = ORDER_BASE + 8)]
    public class Vector3IntVariable : Variable<Vector3Int>
    {
    }
}