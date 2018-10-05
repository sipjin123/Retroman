using System;
using System.Collections.Generic;

public class BasicDataClasses
{
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