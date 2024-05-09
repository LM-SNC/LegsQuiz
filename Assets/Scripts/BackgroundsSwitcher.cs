using System.Collections.Generic;
using JsonModels;
using Reflex.Attributes;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundsSwitcher : MonoBehaviour
{
    public int SelectedGame { get; private set; }
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private LoadingProgressBar _loadingProgressBar;

    [SerializeField] private CanvasSwitcher _canvasSwitcher;
    
    private Dictionary<int, Texture2D> _backgrounds = new();

    private async void Start()
    {
        var backgroundsJson = Resources.Load<TextAsset>(@"Data/backgrounds");
        Debug.Log(backgroundsJson.text);
        var backgrounds = JsonUtility.FromJson<Backgrounds>(backgroundsJson.text);
        
        foreach (var background in backgrounds.Value)
        {
            var texture = Resources.Load<Texture2D>(background.Image);
            _backgrounds[background.GameId] = texture;
        }

        _buttonsHandler.AddHandler("BackButton", async (button, canvas) => { ChangeBackground(); });
        _buttonsHandler.AddHandler("MainMenuButton", async (button, canvas) => { ChangeBackground(); });
        _buttonsHandler.AddHandler("LeaderBoardButton",
            async (button, canvas) => { ChangeBackground(); });

        ChangeBackground();
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
}