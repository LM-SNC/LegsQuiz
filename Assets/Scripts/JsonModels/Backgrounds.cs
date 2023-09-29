// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

using System;
using System.Collections.Generic;

namespace JsonModels
{
    [Serializable]
    public class Backgrounds
    {
        public List<Background> value;
        public List<object> formatters;
        public List<object> contentTypes;
        public object declaredType;
        public int statusCode;

        [Serializable]
        public class Background
        {
            public int gameId;
            public string image;
        }
    }
}