using System.Collections;

using UnityEngine;

using Sirenix.OdinInspector;

/**
 * A component that holds a comment so we could see it in the inspector
 */
public class Comment : MonoBehaviour
{
    [SerializeField, ShowInInspector]
    private string text;

    void Awake()
    {
        DestroyImmediate(this); // auto destroy to save memory
    }

    public string Text
    {
        get
        {
            return text;
        }

        set
        {
            this.text = value;
        }
    }

}
