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
            foreach (var background in (await _legsQuizApi.GetData<JsonModels.Backgrounds>($"?gameid={i}")).value)
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
        }


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