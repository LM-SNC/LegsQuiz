using System;
using System.Collections.Generic;
using System.Linq;
using JsonModels;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CanvasDataManager : MonoBehaviour
{
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private LegsQuizApi _legsQuizApi;

    [SerializeField] private CustomDropDown _customDropDown;
    [SerializeField] private GameObject _table;

    private List<Backgrounds.Background> _backgroundsList;
    [SerializeField] private RawImage[] _canvasBackgrounds;
    private int _selectedGameId = 0;

    private DateTime _nextUpdate = DateTime.Now;

    // Start is called before the first frame update
    private async void Start()
    {
        _backgroundsList = (await _legsQuizApi.GetData<Backgrounds>()).value;
        SwitchBackgrounds();

        var games = await _legsQuizApi.GetData<Games>();

        var options = games.value.Select(value => value.name).ToList();


        _customDropDown.AddOptions(options);


        _buttonsHandler.AddHandler("LeaderBoardButton", (async (button, canvas) =>
        {
            if (DateTime.Now < _nextUpdate) return;

            _nextUpdate = DateTime.Now.AddMinutes(5);

            for (int i = 2; i < _table.transform.childCount; i++)
            {
                Destroy(_table.transform.GetChild(i).gameObject);
            }

            var template = _table.transform.Find("Template");

            var players = await _legsQuizApi.GetData<Players>();

            foreach (var player in players.value)
            {
                var tableElement = Instantiate(template, _table.transform);
                tableElement.transform.GetChild(0).GetComponent<TMP_Text>().SetText(player.name);
                tableElement.transform.GetChild(1).GetComponent<TMP_Text>().SetText(player.answersCount.ToString());

                tableElement.gameObject.SetActive(true);
            }
        }));

        // _buttonsHandler.AddHandler("BackButton", (async (button, canvas) => { OnGameChanged(0); }));
        //
        // OnGameChanged(0);
    }

    private async Awaitable SwitchBackgrounds()
    {
        while (true)
        {
            var gameBackgrounds = _backgroundsList.Where(background => background.gameId == _selectedGameId).ToList();

            var www = UnityWebRequestTexture.GetTexture(gameBackgrounds[Random.Range(0, gameBackgrounds.Count)]
                .image);
            await www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);

                await Awaitable.WaitForSecondsAsync(1);
                continue;
            }

            foreach (var canvasBackground in _canvasBackgrounds)
            {
                Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                canvasBackground.canvasRenderer.SetTexture(myTexture);
            }

            await Awaitable.WaitForSecondsAsync(1);
        }
    }

    public void OnGameChanged(int id)
    {
        _selectedGameId = id;
        // Debug.Log(id);
        // var backgroundsUrl = await _legsQuizApi.GetData<Backgrounds>($"?gameid={id}");
        //
        // Debug.Log(backgroundsUrl.statusCode);
        // if (backgroundsUrl.value.Count < 1)
        //     return;
        //
        // var www = UnityWebRequestTexture.GetTexture(backgroundsUrl.value[Random.Range(0, backgroundsUrl.value.Count)]
        //     .image);
        // await www.SendWebRequest();
        //
        // if (www.result != UnityWebRequest.Result.Success)
        // {
        //     Debug.Log(www.error);
        // }
        // else
        // {
        //     Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        //     _background.canvasRenderer.SetTexture(myTexture);
        // }
    }

    // Update is called once per frame
    private void Update()
    {
    }
}