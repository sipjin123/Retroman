using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Framework;

using Common.Query;

namespace Sandbox.GraphQL
{
    [Serializable]
    public class GraphConfigs
    {
        public static readonly string ENVIRONMENT = "__env";
        public static readonly string BUILD = "__build";
        public static readonly string PUBSUB_ENDPOINT = "pubsub_endpoint";
        public static readonly string PUBLIC_CHANNEL = "pubsub_public_channel";
        public static readonly string PRIVATE_CHANNEL = "pubsub_private_channel";
        public static readonly string SU = "su";
        public static readonly string HOT_ENABLED = "hot_enabled";
        public static readonly string HOT_START = "hot_start";
        public static readonly string HOT_END = "hot_end";
        public static readonly string HOT_MULTIPLIER = "terms_url";
        public static readonly string TERMS_AND_CONDITION = "terms_url";
        public static readonly string QL_IMAGE_URL = "qr_image_url";
        
        private List<Config> Configs;

        public string Environment;
        public string Build;
        public string EndPoint;
        public string PubChannel;
        public string PrivateChannel;
        public string Su;
        public ReactiveProperty<bool> HEnabled = new ReactiveProperty<bool>(false);
        public DateTime HStart;
        public DateTime HEnd;
        public string HMultiplier;
        public string Terms;
        public string QRUrl;

        public void UpdateConfigs(List<Config> configs)
        {
            Configs = configs;

            Func<string, string> GetConfig = delegate (string key)
            {
                if (Configs.Exists(c => c.Key.Equals(key)))
                {
                    return Configs.Find(c => c.Key.Equals(key)).Value;
                }

                return string.Empty;
            };

            Environment = GetConfig(ENVIRONMENT);
            Build = GetConfig(BUILD);
            EndPoint = GetConfig(PUBSUB_ENDPOINT);
            PubChannel = GetConfig(PUBLIC_CHANNEL);
            PrivateChannel = GetConfig(PRIVATE_CHANNEL);
            HEnabled.Value = GetConfig(HOT_ENABLED).ToBool();
            Su = GetConfig(SU);
            HStart = ParseTime(GetConfig(HOT_START));
            HEnd = ParseTime(GetConfig(HOT_END));
            HMultiplier = GetConfig(HOT_MULTIPLIER);
            Terms = GetConfig(TERMS_AND_CONDITION);
            QRUrl = GetConfig(QL_IMAGE_URL);
        }

        public DateTime ParseTime(string time)
        {
            try
            {
                return DateTime.Parse(time);
            }
            catch
            {
                return DateTime.Now;
            }
        }

        public double GetSpan()
        {
            return (HEnd - HStart).TotalMilliseconds;
        }
    }
}
