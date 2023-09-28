using System;
using System.Collections.Generic;

namespace JsonModels
{
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    [Serializable]
    public class Players
    {
        public List<Player> value;
        public List<object> formatters;
        public List<object> contentTypes;
        public object declaredType;
        public int statusCode;
    }

    [Serializable]
    public class Player
    {
        public string id;
        public string name;
        public int answersCount;
    }
}