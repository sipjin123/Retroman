using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UniRx;
using Framework;
using System;
using System.Linq;
using Retroman;

public class AutomationUI : SerializedMonoBehaviour{

    [SerializeField]
    Dictionary<string, UnityEngine.UI.Text> _TextPair;
    [SerializeField]
    Dictionary<string, UnityEngine.UI.Text> _TextPairinList;

    [SerializeField]
    UnityEngine.UI.Text _IndexText;
    int runSessionIndex;
    int sessionListCount;

    [SerializeField]
    Canvas _StatsUI;

    BasicDataClass _CachedBasicDataClass;
    public void ToggleView()
    {
        _StatsUI.enabled = !_StatsUI.isActiveAndEnabled;
    }

    [SerializeField]
    AutomatedTestInstaller _AutomationInstaller;


    [SerializeField]
    UnityEngine.UI.Dropdown _ControllerDropDown, _UIDropDown;
    public void ChangeAutomation()
    {
        ControlType automationControllerType;
        Enum.TryParse(_ControllerDropDown.value.ToString(), out automationControllerType);
        _AutomationInstaller.SetControl(automationControllerType);
    }

    public void ChangeUIAutomation()
    {
        UIControlType automationControllerType;
        Enum.TryParse(_UIDropDown.value.ToString(), out automationControllerType);
        _AutomationInstaller.SetUI(automationControllerType);
    }


    public void LoadGenericData(BasicDataClass basicDataClass)
    {
        _CachedBasicDataClass = basicDataClass;
        _TextPair[nameof(basicDataClass.TotalRunTime)].text = basicDataClass.TotalRunTime;
        _TextPair[nameof(basicDataClass.TotalRuns)].text = basicDataClass.TotalRuns.ToString();
        _TextPair[nameof(basicDataClass.TotalScore)].text = basicDataClass.TotalScore.ToString();
        sessionListCount = basicDataClass.RunSessionList.Count;

        _IndexText.text = (runSessionIndex) + "/" + (sessionListCount - 1);
        if (sessionListCount > 0)
        LoadRunListData(basicDataClass.RunSessionList[runSessionIndex]);
    }
    void LoadRunListData(RunSession runSession)
    {
        _TextPairinList[nameof(runSession.RunTime)].text = (runSession.RunTime).ToString();
        _TextPairinList[nameof(runSession.KilledBy)].text = (runSession.KilledBy).ToString();
        _TextPairinList[nameof(runSession.TotalScore)].text = (runSession.TotalScore).ToString();
    }

    public void NextIndex()
    {
        if(runSessionIndex < sessionListCount-1)
        runSessionIndex++;
        Debug.LogError(D.AUTOMATION + "Run Session is now :: " + runSessionIndex);
        LoadRunListData(_CachedBasicDataClass.RunSessionList[runSessionIndex]);
        _IndexText.text = (runSessionIndex) + "/" + (sessionListCount-1);
    }
    public void PreviousIndex()
    {
        if (runSessionIndex >0)
            runSessionIndex--;
        Debug.LogError(D.AUTOMATION + "Run Session is now :: " + runSessionIndex);
        LoadRunListData(_CachedBasicDataClass.RunSessionList[runSessionIndex]);
        _IndexText.text = (runSessionIndex ) + "/" + (sessionListCount - 1);
    }



    public void InjectBroker(MessageBroker broker)
    {
        broker.Receive<UpdateAutomationData>().Subscribe(_ =>
        {
            LoadGenericData(_.BasicDataClass);
            //Debug.LogError(D.AUTOMATION + nameof(_.BasicDataClass.TotalCoins));
        }).AddTo(this);
    }
    public void InitUI(string controlType, string uiType)
    {
        Debug.LogError(D.AUTOMATION + "Logging saved Prefs");
        int indexControl = _ControllerDropDown.options.FindIndex(_ => _.text == controlType);
        _ControllerDropDown.value = indexControl;

        int indexUIControl = _UIDropDown.options.FindIndex(_ => _.text == uiType);
        _UIDropDown.value = indexUIControl;

    }

}
