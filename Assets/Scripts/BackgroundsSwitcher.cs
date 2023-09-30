using System;
using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BackgroundsSwitcher : MonoBehaviour
{
    private int _selectedGame;
    [Inject] private ButtonsHandler _buttonsHandler;

    [SerializeField] private CanvasSwitcher _canvasSwitcher;

    [Inject] private LegsQuizApi _legsQuizApi;
    private Dictionary<string, Texture2D> _downloadedBackgrounds = new();
    private Dictionary<int, List<string>> _backgroundsUrl = new();

    private async void Start()
    {
        foreach (var background in (await _legsQuizApi.GetData<JsonModels.Backgrounds>()).value)
        {
            if (_backgroundsUrl.TryGetValue(background.gameId, out var gameBackgrounds))
            {
                gameBackgrounds.Add(background.image);
            }
            else
            {
                _backgroundsUrl[background.gameId] = new List<string> { background.image };
            }
        }


        _buttonsHandler.AddHandler("BackButton", async (button, canvas) => { await ChangeBackground(); });
        _buttonsHandler.AddHandler("MainMenuButton", async (button, canvas) => { await ChangeBackground(); });
        _buttonsHandler.AddHandler("LeaderBoardButton",
            async (button, canvas) => { await ChangeBackground(); });

        await ChangeBackground();
    }

    public void OnGameChanged(int gameId)
    {
        Debug.Log("CHANGEEE");
        _selectedGame = gameId;
    }

    private async Awaitable ChangeBackground()
    {
        Debug.Log("Set");
        Debug.Log(_canvasSwitcher.ActiveCanvas == null);
        _canvasSwitcher.ActiveCanvas.transform.Find("Background").GetComponent<RawImage>().texture =
            await GetRandomBackground(_selectedGame);
    }

    private async Awaitable<Texture2D> GetRandomBackground(int gameId)
    {
        var backgrounds = _backgroundsUrl[gameId];
        var background = backgrounds[Random.Range(0, backgrounds.Count)];

        if (_downloadedBackgrounds.TryGetValue(background, out var texture)) return texture;

        using var www = UnityWebRequestTexture.GetTexture(background);

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            _downloadedBackgrounds[background] = texture;
        }

        return texture;
    }
}