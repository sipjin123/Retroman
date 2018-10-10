using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Framework;
using Sandbox.FGCAutomation.Data;
using Sandbox.FGCAutomation.Interfaces;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;

namespace Sandbox.FGCAutomation
{
    public class CurrencyProcessor : IProcessor
    {
        [ShowInInspector]
        public SessionData SessionData { get; private set; }

        public CurrencyProcessor()
        {
            SessionData = new SessionData();
        }

        public void Reset()
        {
            SessionData = new SessionData();
        }

        public void Process(TaskData data)
        {
            Debug.Log($"{D.WARNING} Processing track currency");
            TrackCurrency track = data.GetData<TrackCurrency>();

            GameMatch first = SessionData.GameMatches.FirstOrDefault(_ => _.MatchNumber == track.MatchNumber);
            if (first != null)
                first.CurrencyValueMap.Add(track.Currency.ToString(), track.Value);
            else
            {
                GameMatch match = new GameMatch {MatchNumber = track.MatchNumber};
                match.CurrencyValueMap.Add(track.Currency.ToString(), track.Value);
                SessionData.GameMatches.Add(match);
            }
        }

        [Button(ButtonSizes.Medium)]
        public SessionData AggregateAndFinalize()
        {
            foreach (GameMatch match in SessionData.GameMatches)
            {
                foreach (KeyValuePair<string, TrackedData> trackedData in match.CurrencyValueMap)
                {
                    if (SessionData.CurrencyAverageMap.ContainsKey(trackedData.Key))
                    {
                        SessionData.CurrencyAverageMap[trackedData.Key].Reset();
                        SessionData.CurrencyAverageMap[trackedData.Key] += trackedData.Value;
                    }
                    else
                    {
                        TrackedData data = new TrackedData
                        {
                            Score = 0,
                            StampsFloat = 0.0f,
                            StampsInt = 0,
                            AverageScore = 0.0f,
                            AverageStamps = 0.0f,
                            IsAverage = true
                        };
                        data += trackedData.Value;
                        SessionData.CurrencyAverageMap.Add(trackedData.Key, data);
                    }
                }
            }

            SessionData sessionData = new SessionData(SessionData);
            SessionData.CurrencyAverageMap.ForEach(_ =>
            {
                TrackedData data = sessionData.CurrencyAverageMap[_.Key];
                data.AverageScore = sessionData.CurrencyAverageMap[_.Key].Score;
                data.AverageStamps = sessionData.CurrencyAverageMap[_.Key].StampsFloat;
                sessionData.CurrencyAverageMap[_.Key] = data;
                sessionData.CurrencyAverageMap[_.Key] /= SessionData.GameMatches.Count;
            });

            SessionData = sessionData;
            return SessionData;
        }
    }
}
