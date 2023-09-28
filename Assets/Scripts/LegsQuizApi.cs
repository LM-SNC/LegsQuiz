#nullable enable
using System;
using System.Collections.Generic;
using JsonModels;
using Reflex.Core;
using UnityEngine;
using UnityEngine.Networking;

public class LegsQuizApi : IStartable
{
    private Dictionary<Type, string> _apiUrls = new()
    {
        { typeof(Players), "https://legsquiz.hentach.ru/players" },
        { typeof(Games), "https://legsquiz.hentach.ru/games" },
        { typeof(Backgrounds), "https://legsquiz.hentach.ru/backgrounds" },
    };

    public async Awaitable<T?> GetData<T>(string condition = "")
    {
        using var www = UnityWebRequest.Get(_apiUrls[typeof(T)] + condition);

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