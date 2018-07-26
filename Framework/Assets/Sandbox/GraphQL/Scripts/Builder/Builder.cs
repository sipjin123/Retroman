using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

using UniRx;
using UniRx.Triggers;

using Sirenix.OdinInspector;

using Common;

using Framework;

namespace Sandbox.GraphQL
{
    public enum EBuilder
    {
        query,
        mutation,
    }

    public class Function
    {
        public string Smiley = ",)";
        public string Frown = " )";
        public string Name = string.Empty;
        public Dictionary<string, object> Params = new Dictionary<string, object>();

        public Function(string name)
        {
            Name = name;
        }

        private Function AddQuoted(string key, object val)
        {
            Params.Add(key, val.ToString().ToSQ());
            return this;
        }

        public Function Add(string key, object val)
        {
            Params.Add(key, val);
            return this;
        }
        
        public Function AddString(string key, string val)
        {
            return AddQuoted(key, val);
        }

        public Function AddNumber<T>(string key, T val) where T : struct, IComparable
        {
            return Add(key, val);
        }

        public Function AddBoolean(string key, bool val)
        {
            return Add(key, val.ToString().ToLower());
        }
        
        public override string ToString()
        {
            string format = string.Format("{0}(", Name);

            if (Params.Count >= 1)
            {
                foreach (KeyValuePair<string, object> pair in Params)
                {
                    format += string.Format("{0}:{1},", pair.Key, pair.Value);
                }
            }
            
            format += ")";
            format = format.Replace(Smiley, Frown);
            return format;
        }
    }

    public class Payload
    {
        public readonly string SPACE = " ";
        public Dictionary<string, object> Values = new Dictionary<string, object>();

        public Payload()
        {
        }

        public Payload(Dictionary<string, object> values)
        {
            Values = values;
        }

        public void Add(string key, object value)
        {
            Values.Add(key, value);
        }

        public void AddString(string key, string value)
        {
            Values.Add(key, value.ToSQ());
        }

        public void AddJsonString(string key, string json)
        {
            string QOUTE = "\\\"";
            string jsonUnescape = Regex.Replace(json, "\"", "\\\\\\\"");
            jsonUnescape = string.Format("{0}", QOUTE + jsonUnescape + QOUTE);
            Values.Add(key, jsonUnescape);
        }

        public override string ToString()
        {
            string space = " ";
            string format = "{" + space;

            foreach(KeyValuePair<string, object> item in Values)
            {
                format += string.Format("{0} : {1},", item.Key, item.Value.ToString());
            };

            format += "}";
            return format;
        }
    }

    public class Return
    {
        public readonly string SPACE = " ";
        public string Name = string.Empty;
        public List<object> Values = new List<object>();

        public Return()
        {
        }

        public Return(params string[] values)
        {
            Values.AddRange(values);
        }

        public Return(params object[] values)
        {
            Values.AddRange(values);
        }

        public void Add(string value)
        {
            Values.Add(value);
        }

        public void Add(Return value)
        {
            Values.Add((object)value);
        }

        public void Add(string name, Return value)
        {
            Values.Add((object)(name + SPACE + value.ToString()));
        }

        public override string ToString()
        {
            string space = Name + SPACE;
            string format = "{" + space;

            Values.ForEach(s => format += s.ToString() + space);

            format += "}";
            return format;
        }
    }

    public class Builder
    {
        public static readonly string QOUTE = @"";
        public static readonly string DOUBLE_QOUTE = @"""";
        public EBuilder Type;
        public string Start = "{0}:" + DOUBLE_QOUTE + "{1}";
        public string End = "}" + DOUBLE_QOUTE + "}";
        public Function Function;
        public Return Return;

        public static string Q(string str)
        {
            return DOUBLE_QOUTE + str + DOUBLE_QOUTE;
        }
        
        public static Builder Query()
        {
            return new Builder(EBuilder.query);
        }

        public static Builder Mutation()
        {
            return new Builder(EBuilder.mutation);
        }

        public Builder(EBuilder type)
        {
            Type = type;
        }

        public Function CreateFunction(string name)
        {
            Function = new Function(name);
            return Function;
        }

        public Return CreateReturn(params string[] values)
        {
            Return = new Return(values);
            return Return;
        }

        public override string ToString()
        {
            string start = "{" + string.Format(Start, "query".ToDQ(), QOUTE + Type.ToString()) + "{";
            string method = string.Empty;
            string returnValues = string.Empty;

            if (Function != null)
            {
                method = Function.ToString();
            }

            if (Return != null)
            {
                returnValues = Return.ToString();
            }

            return start + method + returnValues + End;
        }
    }
}
