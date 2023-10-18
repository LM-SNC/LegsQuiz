using System;
using System.Collections.Generic;
using System.Linq;
using JsonModels;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

public class CanvasDataManager : MonoBehaviour
{
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private LegsQuizApi _legsQuizApi;

    [SerializeField] private CustomDropDown _customDropDown;
    [SerializeField] private GameObject _table;
    [SerializeField] private TMP_Text _score;

    private string _playerId = "1";
    private string _playerName = "SomeName";
    public int PlayerMaxScore { get; private set; }

    public Games Games { get; private set; }

    private DateTime _nextUpdate = DateTime.Now;

    // Start is called before the first frame update
    private async void Start()
    {
        Games = await _legsQuizApi.GetData<Games>();

        var options = Games.value.Select(value => value.name).ToList();


        _customDropDown.AddOptions(options);


        _buttonsHandler.AddHandler("LeaderBoardButton", (async (button, canvas) =>
        {
            if (DateTime.Now < _nextUpdate) return;

            var players = await _legsQuizApi.GetData<Players>();

            if (players == null)
                return;
            
            _nextUpdate = DateTime.Now.AddMinutes(5);

            for (int i = 2; i < _table.transform.childCount; i++)
            {
                Destroy(_table.transform.GetChild(i).gameObject);
            }

            var template = _table.transform.Find("Template");


            foreach (var player in players.value.Take(15).OrderByDescending(player => player.answersCount))
            {
                var tableElement = Instantiate(template, _table.transform);
                tableElement.transform.GetChild(0).GetComponent<TMP_Text>().SetText(player.name);
                tableElement.transform.GetChild(1).GetComponent<TMP_Text>().SetText(player.answersCount.ToString());

                tableElement.gameObject.SetActive(true);
            }
        }));

        var playerData = await _legsQuizApi.GetData<Player>($"?id=1");

        _playerId = playerData.id;
        _playerName = playerData.name;

        PlayerMaxScore = playerData.answersCount;
        _score.SetText($"Рекорд: {PlayerMaxScore}");
    }

    public async Awaitable UpdatePlayerMaxScore(int score)
    {
        PlayerMaxScore = score;
        _score.SetText($"Рекорд: {PlayerMaxScore}");

        await _legsQuizApi.SendData(new Player
        {
            id = _playerId,
            name = _playerName,
            answersCount = score
        });
    }
}