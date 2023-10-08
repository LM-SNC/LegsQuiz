using System.Collections.Generic;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundsSwitcher : MonoBehaviour
{
    public int SelectedGame { get; private set; }
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private LoadingProgressBar _progressBar;

    [SerializeField] private CanvasSwitcher _canvasSwitcher;

    [Inject] private LegsQuizApi _legsQuizApi;
    private Dictionary<int, Texture2D> _backgrounds = new();

    private async void Start()
    {
        for (int i = 0; i < 4; i++)
        {
            var gameBackgrounds = await _legsQuizApi.GetData<JsonModels.Backgrounds>($"?gameid={i}");
            if (gameBackgrounds.value == null || gameBackgrounds.value.Count < 1)
                continue;

            _progressBar.AddProgressItems(1);
            DownloadBackground(gameBackgrounds.value[0].image, i);
        }


        _buttonsHandler.AddHandler("BackButton", async (button, canvas) => { ChangeBackground(); });
        _buttonsHandler.AddHandler("MainMenuButton", async (button, canvas) => { ChangeBackground(); });
        _buttonsHandler.AddHandler("LeaderBoardButton",
            async (button, canvas) => { ChangeBackground(); });

        _progressBar.OnResourcesLoaded += ChangeBackground;
    }

    public async void OnGameChanged(int gameId)
    {
        SelectedGame = gameId;

        ChangeBackground();
    }

    private void ChangeBackground()
    {
        Debug.Log("Change background");
        _canvasSwitcher.ActiveCanvas.transform.Find("Background").GetComponent<RawImage>().texture =
            _backgrounds[SelectedGame];
    }

    private async Awaitable DownloadBackground(string url, int gameId)
    {
        var texture = await WebUtils.DownloadImage(url);
        _backgrounds[gameId] = texture;
        _progressBar.CompleteItem();
    }
}