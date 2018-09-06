using Common.Utils;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Retroman
{
    public class GodMode : MonoBehaviour
    {
        MessageBroker broker;
        // Use this for initialization
        void Start()
        {
            broker = Factory.Get<DataManagerService>().MessageBroker;
        }

        // Update is called once per frame
        void Update()
        {

            if(Input.GetKeyDown(KeyCode.Alpha1))
            {
                Debug.LogError("Go To Game");
                broker.Publish(new ChangeScene { Scene = Framework.EScene.GameRoot });
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Debug.LogError("Go To Title");
                broker.Publish(new ChangeScene { Scene = Framework.EScene.TitleRoot });
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                Debug.LogError("Go To Shop");
                broker.Publish(new ChangeScene { Scene = Framework.EScene.ShopRoot });
            }
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {

            }
            if (Input.GetKeyDown(KeyCode.Alpha5))
            {
                broker.Publish(new LaunchGamePlay());
            }
        }
    }

}