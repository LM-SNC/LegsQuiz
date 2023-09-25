using System;
using System.Collections.Generic;

namespace DefaultNamespace
{
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [Serializable]
    public class Players
    {
        public List<Value> value;
        public List<object> formatters;
        public List<object> contentTypes;
        public object declaredType;
        public int statusCode;
    }

    [Serializable]
    public class Value
    {
        public string id;
        public string name;
        public int answersCount;
    }
}