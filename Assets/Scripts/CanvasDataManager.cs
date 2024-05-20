using System.Collections.Generic;
using System.Linq;
using JsonModels;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class CanvasDataManager : MonoBehaviour
{
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private TranslationsManager _translationsManager;

    [SerializeField] private CustomDropDown _customDropDown;
    [SerializeField] private GameObject _table;
    [SerializeField] private TMP_Text _score;

    private string _playerId = "1";
    private string _playerName = "SomeName";
    public int PlayerMaxScore { get; private set; }

    public List<Game> GamesList { get; private set; }


    // Start is called before the first frame update
    private void Start()
    {
        var gamesJson = Resources.Load<TextAsset>(@"Data/games");
        Debug.Log(gamesJson.text);
        GamesList = JsonUtility.FromJson<Games>(gamesJson.text).Value;

        var options = GamesList.Select(value => value.Name).ToList();
        _customDropDown.AddOptions(options);


        _buttonsHandler.AddHandler("LeaderBoardButton",
            (async (button, canvas) => { YandexGame.GetLeaderboard("top", 15, 15, 6, "nonePhoto"); }));

        YandexGame.onGetLeaderboard += data =>
        {
            for (int i = 2; i < _table.transform.childCount; i++)
            {
                Destroy(_table.transform.GetChild(i).gameObject);
            }

            var template = _table.transform.Find("Template");
            foreach (var lbPlayerData in data.players)
            {
                
                var tableElement = Instantiate(template, _table.transform);
                tableElement.transform.GetChild(0).GetComponent<Text>().text = lbPlayerData.name;
                tableElement.transform.GetChild(1).GetComponent<TMP_Text>().SetText(lbPlayerData.score.ToString());
                tableElement.gameObject.SetActive(true);
            }
        };

        PlayerMaxScore = YandexGame.savesData.MaxScore;
        

        _score.SetText($"{_translationsManager.GetPhrase("rec")}{PlayerMaxScore}");

        Debug.Log("Max Score: " + PlayerMaxScore);
    }

    public void UpdateScoreText()
    {
        _score.SetText($"{_translationsManager.GetPhrase("rec")}{PlayerMaxScore}");
    }

    public void UpdatePlayerMaxScore(int score)
    {
        if (score < PlayerMaxScore)
            return;
        
        PlayerMaxScore = score;
        YandexGame.savesData.MaxScore = score;
        YandexGame.SaveProgress();

        UpdateScoreText();
    }
}