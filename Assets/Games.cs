using System;
using System.Collections.Generic;

[Serializable]
public class Games
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
    public int id;
    public string name;
}