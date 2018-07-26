using UnityEngine;

using System;
using System.Collections;

using FLogger = Common.Logger.Logger;

/**
 * A component that can be attached to a GameObject so it could run Logger updates.
 */
public class LoggerComponent : MonoBehaviour
{

    [SerializeField]
    private string name;

    private FLogger logger;

    void Awake()
    {
        logger = FLogger.GetInstance();
        logger.SetName(name);
    }

    // Update is called once per frame
    void Update()
    {
        logger.Update();
    }

}
