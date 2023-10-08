using System.Collections.Generic;
using System.Threading;
using JsonModels;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Inject] private LegsQuizApi _legsQuizApi;
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private LoadingProgressBar _progressBar;
    

    private TimerBar _timerBar;
    private GameImageController _gameImageController;

    private Dictionary<int, List<Questions.Question>> _allQuestions;
    private Queue<Questions.Question> _gameQuestions;
    private Dictionary<string, Texture2D> _gameImages;

    [SerializeField] private GameObject _buttonsContainer;
    private List<TMP_Text> _answerButtonTextFields;

    public GameManager(LegsQuizApi legsQuizApi)
    {
        _timerBar = gameObject.GetComponent<TimerBar>();
        _gameImageController = gameObject.GetComponent<GameImageController>();
    }

    private async void Start()
    {
        _allQuestions = new();
        _gameQuestions = new();
        _gameImages = new();
        _answerButtonTextFields = new();

        for (int i = 0; i < _buttonsContainer.transform.childCount; i++)
        {
            var button = _buttonsContainer.transform.GetChild(i);
            var textComponent = button.GetChild(0).GetComponent<TMP_Text>();

            button.GetComponent<Button>().onClick.AddListener(() => { ProcessAnswer(textComponent.text); });

            _answerButtonTextFields.Add(textComponent);
        }


        for (int i = 0; i < 4; i++)
        {
            _allQuestions[i] = new List<Questions.Question>();

            foreach (var question in (await _legsQuizApi.GetData<Questions>($"?gameid={i}")).value)
            {
                Debug.Log("Question image: " + question.image);
                _allQuestions[i].Add(question);
                _progressBar.AddProgressItems(1);
                ProcessGameImage(question.image);
            }
        }
    }

    private void ProcessAnswer(string answer)
    {
        Debug.Log("Answer: " + answer);
    }

    public void StartGame(int gameId)
    {
    }

    public void StopGame()
    {
    }

    private async Awaitable ProcessGameImage(string imageUrl)
    {
        _gameImages[imageUrl] = await WebUtils.DownloadImage(imageUrl);
        _progressBar.CompleteItem();

    }
}