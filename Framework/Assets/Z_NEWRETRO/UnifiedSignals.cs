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
    public class TogglePause
    {

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
    public class SpawnAPlatform
    {

    }
    public class ChangeCamAngle
    {
        public int Angle;
    }
    public class ToggleCoins
    {
        public bool IfActive;
    }
    public class RefreshCoins
    {

    }
    public class InsufficientCoins
    {

    }
    public class RefereshAllItems
    {

    }
    public class ShowVersion
    {
        public bool IfActive;
    }
    public class UpdateScore
    {

    }
    public class AddScore
    {
        public float ScoreToAdd;
    }
    public class EnableRagdoll
    {
    }
    public class DisablePlayableCharacter
    {
    }
    public class GameOver
    {

    }
    public class UpdatePlayerAction
    {
        public PlayerControls.PlayerAction PlayerAction;
    }
    public class EnablePlayerControls
    {
        public bool IfACtive;
    }
    public class EnablePlayerShadows
    {
        public bool IfActive;
    }
    public class ActivePlayerObject
    {
        public bool IfActive;
    }
    public class SetupPlayerSplash
    {
        public bool IfActive;
        public Vector3 Position;
    }

    public class PressBackButtonINIT
    {
    }
    public class PressBackButton
    {
        public BackButtonType BackButtonType;
    }
}
public enum BackButtonType
{
    SceneIsTitle,
    SceneIsShop,
    SceneIsGame,
    ExitGame,
    SceneIsSettings
}