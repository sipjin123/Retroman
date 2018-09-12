using Common.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Retroman;
using Framework;
using UniRx;
using UnityEngine.UI;

using Sirenix.OdinInspector;
using Sandbox.GraphQL;

public class ResultsPopup : MonoBehaviour {


    public CanvasGroup InteractiveCanvas;

    public Text HiScore1, HiScore2, CScore1, CScore2;
    public Image CharImage;
    GameRoot newGameRoot;
    private void Awake()
    {
        newGameRoot = Scene.GetScene<GameRoot>(EScene.GameRoot);

        Factory.Get<DataManagerService>().MessageBroker.Receive<EndGame>().Subscribe(_ =>
        {
            ShowResults();
        }).AddTo(this);
        Factory.Get<DataManagerService>().MessageBroker.Receive<TriggerCanvasInteraction>().Subscribe(_ =>
        {
            InteractiveCanvas.interactable = false;
        }).AddTo(this);
    }
    private void OnEnable()
    {
        ShowResults();
    }
    void ShowResults()
    {

        CScore1.text = "" + Factory.Get<DataManagerService>().GetScore();
        CScore2.text = "" + Factory.Get<DataManagerService>().GetScore();
        HiScore1.text = "Best Score " + Factory.Get<DataManagerService>().GetHighScore();
        HiScore2.text = "Best Score " + Factory.Get<DataManagerService>().GetHighScore();

        SystemRoot sysroot = Scene.GetScene<SystemRoot>(EScene.System);
        sysroot.Publish(new SendToFGCWalletSignal { Value = (int)Factory.Get<DataManagerService>().GetScore() });
        sysroot.Publish(new FetchConversionSignal());

        int currChar = Factory.Get<DataManagerService>().CurrentCharacterSelected - 1;
        CharImage.sprite = Factory.Get<DataManagerService>().ShopItems[currChar].ItemImage.sprite;
    }

}
