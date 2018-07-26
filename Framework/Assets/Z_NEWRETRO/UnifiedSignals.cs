using Framework;
using Synergy88;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnifiedSignals : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}


namespace Retroman
{
    public class ChangeScene
    {
        public EScene Scene;

    }
    public class PauseGame
    {
        public bool IfPause;
    }
    public class RestartGame
    {

    }
    public class EndGame
    {

    }
    public class SelectItem
    {
        public ShopItemData ShopItem;
    }
    public class RefreshShopItems
    {

    }
    public class LaunchGamePlay
    {

    }
    public class AddCoin
    {
        public float CoinsToAdd;
    }
}