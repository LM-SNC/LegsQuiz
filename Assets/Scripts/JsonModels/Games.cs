using System;
using System.Collections.Generic;

namespace JsonModels
{
    [Serializable]
    public class Games
    {
        public List<Game> Value;
    }
    
    [Serializable]
    public class Game
    {
        public int Id;
        public string Name;
    }
}