using System;   
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


    private DateTime _nextUpdate = DateTime.Now;

    // Start is called before the first frame update
    private async void Start()
    {
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

            foreach (var player in players.value.Take(15))
            {
                var tableElement = Instantiate(template, _table.transform);
                tableElement.transform.GetChild(0).GetComponent<TMP_Text>().SetText(player.name);
                tableElement.transform.GetChild(1).GetComponent<TMP_Text>().SetText(player.answersCount.ToString());

                tableElement.gameObject.SetActive(true);
            }
        }));
    }
}