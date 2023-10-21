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
        /*{ typeof(Games), "https://legsquiz.hentach.ru/games" },
        { typeof(Backgrounds), "https://legsquiz.hentach.ru/backgrounds" },
        { typeof(Questions), "https://legsquiz.hentach.ru/questions" },*/
        { typeof(Player), "https://legsquiz.hentach.ru/player" }
    };

    public async Awaitable<T?> GetData<T>(string condition = "", int tryCount = 1)
    {
        for (int i = 0; i < tryCount; i++)
        {
            using var www = UnityWebRequest.Get(_apiUrls[typeof(T)] + condition);
            Debug.Log(_apiUrls[typeof(T)] + condition);
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
        }

        return default;
    }

    public async Awaitable SendData<T>(T data)
    {
        using var www = UnityWebRequest.Post(_apiUrls[typeof(T)], JsonUtility.ToJson(data), "application/json");

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
        }
    }

    public void Start()
    {
    }
}