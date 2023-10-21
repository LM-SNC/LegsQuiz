using System;
using System.Collections.Generic;

namespace JsonModels
{
    [Serializable]
    public class Backgrounds
    {
        public List<Background> Value;
    }
    
    [Serializable]
    public class Background
    {
        public int GameId;
        public string Image;
    }
}