using System;
using System.Collections.Generic;
using System.Linq;
using JsonModels;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using YG;

public class CanvasDataManager : MonoBehaviour
{
    [Inject] private ButtonsHandler _buttonsHandler;
    //[Inject] private LegsQuizApi _legsQuizApi;

    [SerializeField] private CustomDropDown _customDropDown;
    [SerializeField] private GameObject _table;
    [SerializeField] private TMP_Text _score;

    private string _playerId = "1";
    private string _playerName = "SomeName";
    public int PlayerMaxScore { get; private set; }

    public List<Game> GamesList { get; private set; }
    


    // Start is called before the first frame update
    private async void Start()
    {
        var gamesJson = Resources.Load<TextAsset>(@"Data/games");
        Debug.Log(gamesJson.text);
        GamesList = JsonUtility.FromJson<Games>(gamesJson.text).Value;

        var options = GamesList.Select(value => value.Name).ToList();
        _customDropDown.AddOptions(options);

        var template = _table.transform.Find("Template");
        
        YandexGame.onGetLeaderboard += data =>
        {
            for (int i = 2; i < _table.transform.childCount; i++)
            {
                Destroy(_table.transform.GetChild(i).gameObject);
            }

            foreach (var lbPlayerData in data.players)
            {
                var tableElement = Instantiate(template, _table.transform);
                tableElement.transform.GetChild(0).GetComponent<TMP_Text>().SetText(lbPlayerData.name);
                tableElement.transform.GetChild(1).GetComponent<TMP_Text>().SetText(lbPlayerData.score.ToString());
                tableElement.gameObject.SetActive(true);
            }
        };

        _buttonsHandler.AddHandler("LeaderBoardButton", (async (button, canvas) =>
        {
            YandexGame.GetLeaderboard("top", 15, 15, 6, "nonePhoto");

            // _nextUpdate = DateTime.Now.AddMinutes(5);
            //
            // for (int i = 2; i < _table.transform.childCount; i++)
            // {
            //     Destroy(_table.transform.GetChild(i).gameObject);
            // }
            //
            // var template = _table.transform.Find("Template");
            //
            //
            // foreach (var player in players.value.Take(15).OrderByDescending(player => player.answersCount))
            // {
            //     var tableElement = Instantiate(template, _table.transform);
            //     tableElement.transform.GetChild(0).GetComponent<TMP_Text>().SetText(player.name);
            //     tableElement.transform.GetChild(1).GetComponent<TMP_Text>().SetText(player.answersCount.ToString());
            //
            //     tableElement.gameObject.SetActive(true);
            // }
        }));

        // var playerData = await _legsQuizApi.GetData<Player>($"?id=1");
        //
        // _playerId = playerData.id;
        // _playerName = playerData.name;
        //
        // PlayerMaxScore = playerData.answersCount;

        PlayerMaxScore = YandexGame.savesData.MaxScore;
        _score.SetText($"Рекорд: {PlayerMaxScore}");
    }

    public async Awaitable UpdatePlayerMaxScore(int score)
    {
        PlayerMaxScore = score;
        _score.SetText($"Рекорд: {PlayerMaxScore}");

        YandexGame.savesData.MaxScore = score;
        YandexGame.SaveProgress();

        // await _legsQuizApi.SendData(new Player
        // {
        //     id = _playerId,
        //     name = _playerName,
        //     answersCount = score
        // });
    }
}