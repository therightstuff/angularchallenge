using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Angular_Coding_Challenge
{
    public class JsonObjects
    {
        public static bool HasProperty(dynamic obj, string name)
        {
            if (obj is ExpandoObject) {
                return ((IDictionary<string, object>)obj).ContainsKey(name);
            }

            return obj.GetType().GetProperty(name) != null;
        }

        /// <summary>
        /// returns true if property doesn't exist or if string is null or empty
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsStringNullOrEmpty(dynamic obj, string name) {
            if (HasProperty(obj, name)) {
                if (obj is JObject) {
                    if (obj is JObject) {
                        return (obj as JObject)[name] == null;
                    }
                }
                if (obj is ExpandoObject) {
                    return string.IsNullOrEmpty(
                        ((IDictionary<string, object>)obj)[name].ToString()
                    );
                }
                return obj.GetType().GetProperty(name);
            }
            return true;
        }

        public static dynamic Jsonify(Models.User user)
        {
            dynamic result = new ExpandoObject();
            result.UserId = user.UserId;
            result.Username = user.Username;
            result.IsAdministrator = user.IsAdministrator;
            return result;
        }

        public static dynamic Jsonify(List<Models.User> users)
        {
            List<dynamic> results = new List<dynamic>();
            foreach (Models.User user in users) {
                results.Add(Jsonify(user));
            }
            return results;
        }

        public static dynamic Jsonify(Models.Index index)
        {
            dynamic result = new ExpandoObject();
            result.IndexId = index.IndexId;
            result.Name = index.Name;
            result.Ticker = index.Ticker;
            result.Price = index.Price;
            return result;
        }

        public static List<dynamic> Jsonify(List<Models.Index> indices)
        {
            List<dynamic> results = new List<dynamic>();
            foreach (Models.Index index in indices) {
                results.Add(Jsonify(index));
            }
            return results;
        }

    }
}