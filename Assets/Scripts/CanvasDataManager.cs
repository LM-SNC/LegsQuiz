using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DefaultNamespace;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

public class CanvasDataManager : MonoBehaviour
{
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private LegsQuizApi _legsQuizApi;

    [SerializeField] private CustomDropDown _customDropDown;
    [SerializeField] private GameObject _table;

    // Start is called before the first frame update
    private async void Start()
    {
        var games = await _legsQuizApi.GetData<Games>();

        Debug.Log(games == null);

        var options = games.value.Select(value => value.name).ToList();
        Debug.Log(options[0]);
        _customDropDown.AddOptions(options);


        _buttonsHandler.AddHandler("LeaderBoardButton", (async (button, canvas) =>
        {
            Debug.Log(Thread.CurrentThread.ManagedThreadId);
            for (int i = 2; i < _table.transform.childCount; i++)
            {
                Destroy(_table.transform.GetChild(i).gameObject);
            }

            var template = _table.transform.Find("Template");
            
            var players = await _legsQuizApi.GetData<Players>();

            Debug.Log(Thread.CurrentThread.ManagedThreadId);
            
            foreach (var player in players.value)
            {
                var tableElement = Instantiate(template, _table.transform);
                tableElement.transform.GetChild(0).GetComponent<TMP_Text>().SetText(player.name);
                tableElement.transform.GetChild(1).GetComponent<TMP_Text>().SetText(player.answersCount.ToString());

                tableElement.gameObject.SetActive(true);
            }
        }));
    }

    // Update is called once per frame
    private void Update()
    {
    }
}