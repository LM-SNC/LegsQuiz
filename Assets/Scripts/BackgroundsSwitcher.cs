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
        for (int i = 0; i < 4; i++)
        {
            _backgroundsUrl.Add(i, new List<string>());
            foreach (var background in (await _legsQuizApi.GetData<JsonModels.Backgrounds>($"?gameid={i}")).value)
            {
                _backgroundsUrl[i].Add(background.image);
            }
        }

        DownloadBackgrounds();


        _buttonsHandler.AddHandler("BackButton", async (button, canvas) => { await ChangeBackground(); });
        _buttonsHandler.AddHandler("MainMenuButton", async (button, canvas) => { await ChangeBackground(); });
        _buttonsHandler.AddHandler("LeaderBoardButton",
            async (button, canvas) => { await ChangeBackground(); });

        await ChangeBackground();
    }

    public async void OnGameChanged(int gameId)
    {
        _selectedGame = gameId;

        await ChangeBackground();
    }

    private async Awaitable ChangeBackground()
    {
        _canvasSwitcher.ActiveCanvas.transform.Find("Background").GetComponent<RawImage>().texture =
            GetRandomBackground(_selectedGame);
    }

    private Texture2D GetRandomBackground(int gameId)
    {
        var backgrounds = _backgroundsUrl[gameId];

        var background = backgrounds[Random.Range(0, backgrounds.Count)];

        if (_downloadedBackgrounds.TryGetValue(background, out var texture)) return texture;

        return null;
    }

    private async Awaitable DownloadBackgrounds()
    {
        foreach (var backgroundsUrl in _backgroundsUrl.Values)
        {
            foreach (var backgroundUrl in backgroundsUrl)
            {
                var texture = await WebUtils.DownloadImage(backgroundUrl);
                _downloadedBackgrounds[backgroundUrl] = texture;
            }
        }
    }
}