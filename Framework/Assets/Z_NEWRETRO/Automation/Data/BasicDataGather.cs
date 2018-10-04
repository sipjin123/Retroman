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
                        {
                            _CurrentRunSession = new RunSession();
                            _CurrentRunSession.StartTime = DateTime.UtcNow;
                            Debug.LogError(D.AUTOMATION + _CurrentRunSession.StartTime);
                        }
                        break;
                case TypeOfStatisticData.ResultsData:
                        {

                            ProcessResults cachedResult = (ProcessResults)_.RawData;

                            _CurrentRunSession.TotalScore = (int)cachedResult.TotalScore;
                            _CurrentRunSession.KilledBy = cachedResult.PlatformDeath;
                            _CurrentRunSession.EndTime = DateTime.UtcNow;
                            

                            string runSecondGap = (_CurrentRunSession.EndTime - _CurrentRunSession.StartTime).Seconds.ToString();
                            string runMilliSecGap = (_CurrentRunSession.EndTime - _CurrentRunSession.StartTime).Milliseconds.ToString();
                            _CurrentRunSession.RunTime = runSecondGap + "." + runMilliSecGap.Substring(0, 2);


                            _BasicDataClass.TotalScore += _CurrentRunSession.TotalScore;
                            _BasicDataClass.TotalRuns += 1;
                            _BasicDataClass.RunSessionList.Add(_CurrentRunSession);
                            _BasicDataClass.EndTime = DateTime.UtcNow;
                            string cachedKillBy = _CurrentRunSession.KilledBy;
                        Debug.LogError(D.AUTOMATION+"The Killer Is : " + cachedKillBy);

                            if (_BasicDataClass.KillerList.Exists(q => q.Name == cachedKillBy))
                            {
                                _BasicDataClass.KillerList.Find(q => q.Name == cachedKillBy).IterateCounter();
                            }
                            else
                            {
                                _BasicDataClass.KillerList.Add(new DeathData { Name = cachedKillBy, Counter = 1 });
                            }


                            string sessionSecondsGap = (_BasicDataClass.EndTime - _BasicDataClass.StartTime).Seconds.ToString();
                            string sessionMilliSecGap = (_BasicDataClass.EndTime - _BasicDataClass.StartTime).Milliseconds.ToString();
                            _BasicDataClass.TotalRunTime = sessionSecondsGap + "." + sessionMilliSecGap.Substring(0, 2);
                            Debug.LogError(D.AUTOMATION + "Total Run List :: " + _BasicDataClass.RunSessionList.Count);
                            WriteToFile();
                        }
                        break;
                }
            }).AddTo(obj);

        InitializeFile();
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

    [Serializable]
    public class BasicDataClass
    {
        public DateTime StartTime;
        public DateTime EndTime;
        public string TotalRunTime;
        public int TotalScore;
        public int TotalRuns;
        public List<RunSession> RunSessionList = new List<RunSession>();
        public List<DeathData> KillerList = new List<DeathData>();
    }
    [Serializable]
    public class RunSession
    {
        public DateTime StartTime;
        public DateTime EndTime;
        public string KilledBy;
        public string RunTime;
        public int TotalScore;
    }
    [Serializable]
    public class DeathData
    {
        public string Name;
        public int Counter;
        public void IterateCounter()
        {
            Counter++;
        }
    }
}
