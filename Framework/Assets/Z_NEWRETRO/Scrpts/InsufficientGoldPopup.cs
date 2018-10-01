using Framework;
using Sandbox.Popup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsufficientGoldPopup : MonoBehaviour {

    public void CloseNotif()
    {

        SoundControls.Instance._buttonClick.Play();
        Scene.GetScene<PopupCollectionRoot>(EScene.PopupCollection).Hide();
    }
}
