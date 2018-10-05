using Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UniRx;
using UnityEngine;
using Retroman;
using Common.Utils;
using System.Linq;

public class BasicDataGather : IDataController
{
    const string FileLocation = "/GatheredData/";
    string ActualFileLocation;

    string CachedFileName;

    MessageBroker _Broker;
    
    BasicDataClass _BasicDataClass;
    RunSession _CurrentRunSession;
    public void InitializeData(GameObject obj,MessageBroker broker)
    {
        _Broker = broker;
        _Broker.Receive<RegisterStats>().Subscribe(_ =>
        {
            Debug.LogError(D.AUTOMATION + _.TypeOfStatisticData);
            switch (_.TypeOfStatisticData)
            {
                case TypeOfStatisticData.InitRun:
                    ProcessInit();
                    break;
                case TypeOfStatisticData.ResultsData:
                    ProcessResults(_.RawData);
                    break;
                }
            }).AddTo(obj);

        InitializeFile();
    }

    void ProcessInit()
    {
        _CurrentRunSession = new RunSession();
        _CurrentRunSession.StartTime = DateTime.UtcNow;
        Debug.LogError(D.AUTOMATION + _CurrentRunSession.StartTime);
    }
    void ProcessResults(object rawData)
    {
        ProcessResults cachedResult = (ProcessResults)rawData;
        //RUN DATA CACHE
        _CurrentRunSession.TotalScore = (int)cachedResult.TotalScore;
        _CurrentRunSession.KilledBy = cachedResult.PlatformDeath;
        _CurrentRunSession.EndTime = DateTime.UtcNow;

        //RUN SESSION UPDATE
        string runSecondGap = (_CurrentRunSession.EndTime - _CurrentRunSession.StartTime).Seconds.ToString();
        string runMilliSecGap = (_CurrentRunSession.EndTime - _CurrentRunSession.StartTime).Milliseconds.ToString();
        _CurrentRunSession.RunTime = runSecondGap + "." + runMilliSecGap.Substring(0, 2);

        //UPDATE SCORE AND RUNNING
        _BasicDataClass.TotalScore += _CurrentRunSession.TotalScore;
        _BasicDataClass.TotalRuns += 1;
        _BasicDataClass.RunSessionList.Add(_CurrentRunSession);
        _BasicDataClass.EndTime = DateTime.UtcNow;

        //UPDATE KILL LIST
        string cachedKillBy = _CurrentRunSession.KilledBy;
        if (_BasicDataClass.KillerList.Exists(q => q.Name == cachedKillBy))
        {
            _BasicDataClass.KillerList.Find(q => q.Name == cachedKillBy).IterateCounter();
        }
        else
        {
            _BasicDataClass.KillerList.Add(new DeathData { Name = cachedKillBy, Counter = 1 });
        }

        //UPDATE TOTAL TIME
        string sessionSecondsGap = (_BasicDataClass.EndTime - _BasicDataClass.StartTime).Seconds.ToString();
        string sessionMilliSecGap = (_BasicDataClass.EndTime - _BasicDataClass.StartTime).Milliseconds.ToString();
        _BasicDataClass.TotalRunTime = sessionSecondsGap + "." + sessionMilliSecGap.Substring(0, 2);

        //WRITE TO DATA
        WriteToFile();
    }

    void InitializeFile()
    {
        _BasicDataClass = new BasicDataClass();
        _BasicDataClass.StartTime = DateTime.UtcNow;

        ActualFileLocation = Application.persistentDataPath + FileLocation + DateTime.UtcNow.Date.Month + "_" + DateTime.UtcNow.Date.Day + "_" + DateTime.UtcNow.Date.Year;
        CachedFileName = 
            DateTime.UtcNow.Date.Month +"_"
            +DateTime.UtcNow.Date.Day + "_"
            +DateTime.UtcNow.Date.Year +"_("
            +DateTime.UtcNow.Hour +"-"
            +DateTime.UtcNow.Minute+"-"
            +DateTime.UtcNow.Second.ToString()+")";

        if (Directory.Exists(ActualFileLocation))
        {
            Debug.LogError(D.AUTOMATION + "Yes it Exists");
        }
        else
        {
            Debug.LogError(D.AUTOMATION + "I will be creating the File Directory");
            Directory.CreateDirectory(ActualFileLocation);
        }
    }

    public void WriteToFile()
    {
        string JSONData = JsonUtility.ToJson(_BasicDataClass);

        string fileNameToCreate = ActualFileLocation + "/" +CachedFileName+ ".txt";
        File.WriteAllText(fileNameToCreate, JSONData);
    }
}
