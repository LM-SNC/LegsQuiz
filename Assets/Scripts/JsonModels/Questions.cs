using System;
using System.Collections.Generic;

namespace JsonModels
{
    [Serializable]
    public class Questions
    {
        public List<Question> Value;
    }
    
    [Serializable]
    public class Question
    {
        public int Id;
        public int GameId;
        
        public string Text;
        public string Image;
        public string PhraseKey;
        public string AnswerRu;
    }
}