using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;

public class Greeter : IStartable // IStartable will force it to be constructed on container build
{
    public Greeter(IEnumerable<string> strings)
    {
        Debug.Log(string.Join(" ", strings));
    }

    public void Start()
    {
    }
}