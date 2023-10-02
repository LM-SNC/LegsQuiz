using System;
using System.Collections.Generic;

namespace JsonModels
{
    [Serializable]
    public class Questions
    {
        public List<Question> value;
        public List<object> formatters;
        public List<object> contentTypes;
        public object declaredType;
        public int statusCode;

        [Serializable]
        public class Question
        {
            public int id;
            public int gameId;
            public string text;
            public string image;
            public string answer;
        }
    }
}