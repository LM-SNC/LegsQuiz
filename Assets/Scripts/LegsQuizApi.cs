using System;
using System.Collections.Generic;
using Reflex.Core;
using UnityEngine;
using UnityEngine.Networking;

public class LegsQuizApi : IStartable
{
    public LegsQuizApi()
    {
        Debug.Log("HUI12345");
    }
    public enum Request
    {
        Players = 0,
        Games = 1
    }

    private Dictionary<Request, string> _apiUrls = new()
    {
        { Request.Players, "https://legsquiz.hentach.ru/players" },
        { Request.Games, "https://legsquiz.hentach.ru/games" }
    };

    public async Awaitable<T> GetData<T>(Request request, string condition = "")
    {
        using var www = UnityWebRequest.Get(_apiUrls[request] + condition);

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            return JsonUtility.FromJson<T>(www.downloadHandler.text);
        }

        return default;
    }

    public void Start()
    {
        
    }
}