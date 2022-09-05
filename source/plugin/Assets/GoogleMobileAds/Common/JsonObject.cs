using System;
using System.Collections.Generic;
using System.Linq;
using Google.MiniJSON;

namespace GoogleMobileAds.Common
{
    /// <summary>
    /// Internal utility for deserializing MiniJson objects.
    /// </summary>
    internal class JsonObject
    {
        Dictionary<string, object> _json;

        internal JsonObject(string json)
        {
            var jsonObject = Json.Deserialize(json);
            Init((Dictionary<string, object>) jsonObject);
        }

        internal JsonObject(Dictionary<string, object> json)
        {
            Init(json);
        }

        private void Init(Dictionary<string, object> json)
        {
            _json = (json != null ? json : new Dictionary<string, object>());
        }

        /// <summary>
        /// Returns a value from the json object.
        /// </summary>
        /// <param name="fieldName">name of the field to get.</param>
        /// <returns>The value from the json field.</returns>
        public object GetValue(string fieldName)
        {
            if (_json.ContainsKey(fieldName))
            {
                return _json[fieldName];
            }
            return null;
        }


        /// <summary>
        /// Returns a value from the json object.
        /// Only supports convertible values.
        /// Does not support objects, collections, enums, or other complex types.
        /// </summary>
        /// <typeparam name="T">type of the field to get.</typeparam>
        /// <param name="fieldName">name of the field to get.</param>
        /// <returns>The value from the json field.</returns>
        public T GetValue<T>(string fieldName) where T : IConvertible
        {
            if (_json.ContainsKey(fieldName))
            {
                return (T)Convert.ChangeType(_json[fieldName], typeof(T));
            }
            return default(T);
        }

        /// <summary>
        /// Returns a list of values from the json object.
        /// </summary>
        /// <typeparam name="T">type of the field to get.</typeparam>
        /// <param name="fieldName">name of the field to get.</param>
        /// <returns>The value from the json field.</returns>
        public List<object> GetList(string fieldName)
        {
            return (List<object>)GetValue(fieldName);
        }

        /// <summary>
        /// Returns a list of values from the json object.
        /// Only supports convertible values.
        /// Does not support objects, collections, enums, or other complex types.
        /// </summary>
        /// <typeparam name="T">type of the field to get.</typeparam>
        /// <param name="fieldName">name of the field to get.</param>
        /// <returns>The value from the json field.</returns>
        public List<T> GetList<T>(string fieldName) where T : IConvertible
        {
            var rawList = GetList(fieldName);
            if (rawList != null)
            {
                return rawList
                        .Select(item => (T)Convert.ChangeType(item, typeof(T)))
                        .ToList();
            }
            return null;
        }

        /// <summary>
        /// Returns a dictionary from the json object.
        /// </summary>
        /// <typeparam name="T">type of the field to get.</typeparam>
        /// <param name="fieldName">name of the field to get.</param>
        /// <returns>The value from the json field.</returns>
        public Dictionary<string, object> GetDictionary(string fieldName)
        {
            return (Dictionary<string, object>)GetValue(fieldName);
        }

        /// <summary>
        /// Returns a dictionary from the json object.
        /// Only supports convertible values.
        /// Does not support objects, collections, enums, or other complex types.
        /// </summary>
        /// <typeparam name="T">type of the field to get.</typeparam>
        /// <param name="fieldName">name of the field to get.</param>
        /// <returns>The value from the json field.</returns>
        public Dictionary<string, T> GetDictionary<T>(string fieldName) where T : IConvertible
        {
            var dictionary = GetDictionary(fieldName);
            if (dictionary != null)
            {
                return dictionary
                        .ToDictionary(item => item.Key,
                                      item => (T)Convert.ChangeType(item.Value, typeof(T)));
            }
            return null;
        }

        /// <summary>
        /// Returns json object from the json object.
        /// </summary>
        /// <typeparam name="T">type of the field to get.</typeparam>
        /// <param name="fieldName">name of the field to get.</param>
        /// <returns>The value from the json field.</returns>
        public JsonObject GetJsonObject(string fieldName)
        {
            if (_json.ContainsKey(fieldName))
            {
                object value = _json[fieldName];
                return ConvertJsonValue(value);
            }
            return null;
        }

        /// <summary>
        /// Returns a list of json objects from the json object.
        /// Only supports convertible values.
        /// Does not support objects, collections, enums, or other complex types.
        /// </summary>
        /// <typeparam name="T">type of the field to get.</typeparam>
        /// <param name="fieldName">name of the field to get.</param>
        /// <returns>The value from the json field.</returns>
        public List<JsonObject> GetJsonObjectList(string fieldName)
        {
            var rawList = GetList(fieldName);
            if (rawList != null)
            {
                return rawList
                        .Select(item => ConvertJsonValue(item))
                        .ToList();
            }
            return null;
        }

        /// <summary>
        /// Returns the json string representation.
        /// </summary>
        public override string ToString()
        {
            return Json.Serialize(_json);
        }

        private JsonObject ConvertJsonValue(object value)
        {
            var dictionary = value as Dictionary<string, object>;
            if (dictionary == null)
            {
                return null;
            }
            return new JsonObject(dictionary);
        }
    }
}
