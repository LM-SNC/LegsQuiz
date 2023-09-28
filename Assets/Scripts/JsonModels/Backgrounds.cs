// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

using System.Collections.Generic;

namespace JsonModels
{
    public class Backgrounds
    {
        public List<Background> value { get; set; }
        public List<object> formatters { get; set; }
        public List<object> contentTypes { get; set; }
        public object declaredType { get; set; }
        public int statusCode { get; set; }

        public class Background
        {
            public int gameId { get; set; }
            public string image { get; set; }
        }
    }
}