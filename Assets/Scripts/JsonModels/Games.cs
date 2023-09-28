using System;
using System.Collections.Generic;

namespace JsonModels
{
    [Serializable]
    public class Games
    {
        public List<Game> value;
        public List<object> formatters;
        public List<object> contentTypes;
        public object declaredType;
        public int statusCode;
    }

    [Serializable]
    public class Game
    {
        public int id;
        public string name;
    }
}