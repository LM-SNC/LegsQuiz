using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JsonModels;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Inject] private LegsQuizApi _legsQuizApi;
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private LoadingProgressBar _progressBar;
    [Inject] private BackgroundsSwitcher _backgroundsSwitcher;
    [Inject] private CanvasDataManager _canvasDataManager;

    [SerializeField] private Image _gameBorder;

    private TimerBar _timerBar;
    private GameImageController _gameImageController;

    private Dictionary<int, List<Questions.Question>> _allQuestions;
    private List<Questions.Question> _gameQuestions;


    private Dictionary<string, Texture2D> _gameImages;

    [SerializeField] private GameObject _buttonsContainer;
    private List<TMP_Text> _answerButtonTextFields;
    private List<Button> _answerButtons;

    private int _currentQuestion = -1;
    private int _score = 0;

    private int _imagesInProcess;
    private int _trueAnswerButton;
    private bool _effectTime = false;

    private int _lastGameId;


    [SerializeField] private Color _trueAnswerColor;
    [SerializeField] private Color _wrongAnswerColor;


    private CancellationTokenSource _questionCancellationTokenSource;

    private int _hp = 3;
    [SerializeField] private GameObject _heartContainer;
    private RectTransform _heartContainerRect;
    private float _heartPart;
    private float _defaultHeartHeight;
    private Vector3 _defaultHeartPosition;

    private bool _isLoose;

    [SerializeField] private GameObject _defeatMenu;
    private TMP_Text _defeatMenuScore;

    private async void Start()
    {
        _timerBar = gameObject.GetComponent<TimerBar>();
        _gameImageController = gameObject.GetComponent<GameImageController>();

        _allQuestions = new();
        _gameQuestions = new();
        _gameImages = new();
        _answerButtonTextFields = new();
        _answerButtons = new();

        for (int i = 0; i < _buttonsContainer.transform.childCount; i++)
        {
            var buttonGameObject = _buttonsContainer.transform.GetChild(i);
            var button = buttonGameObject.GetComponent<Button>();

            var textComponent = buttonGameObject.GetChild(0).GetComponent<TMP_Text>();

            buttonGameObject.GetComponent<Button>().onClick.AddListener(() =>
            {
                ProcessAnswer(button, textComponent.text);
            });

            _answerButtons.Add(button);
            _answerButtonTextFields.Add(textComponent);
        }


        _heartContainerRect = _heartContainer.GetComponent<RectTransform>();
        _defaultHeartHeight = _heartContainerRect.rect.height;
        _defaultHeartPosition = _heartContainerRect.transform.position;
        _heartPart = _defaultHeartHeight / 3.0f;

        var games = await _legsQuizApi.GetData<Games>();
        foreach (var game in games.value)
        {
            _allQuestions[game.id] = new List<Questions.Question>();
            var questions = (await _legsQuizApi.GetData<Questions>($"?gameid={game.id}")).value;
            _progressBar.AddProgressItems(questions.Count);

            foreach (var question in questions)
            {
                Debug.Log("Question image: " + question.image);
                _allQuestions[game.id].Add(question);
                ProcessGameImage(question.image);
            }
        }

        _buttonsHandler.AddHandler("StartButton",
            async (button, canvas) => { StartGame(_backgroundsSwitcher.SelectedGame); });

        _buttonsHandler.AddHandler("BackButton", async (button, canvas) => { StopGame(); });

        _buttonsHandler.AddHandler("RestartButton", async (button, canvas) => { StartGame(_lastGameId); });

        _buttonsHandler.AddHandler("ResumeButton", async (button, canvas) =>
        {
            _currentQuestion--;
            StartGame(_lastGameId, true);
        });

        _timerBar.OnTimerEnd += async () =>
        {
            if (_hp <= 0)
                return;
            
            _hp--;
            UpdateHealPoints();

            _effectTime = true;

            _gameBorder.color = _wrongAnswerColor;
            await EndQuestion();
            ChangeQuestion();
        };


        _defeatMenuScore = _defeatMenu.transform.Find("Score").GetComponent<TMP_Text>();
    }

    private async void ProcessAnswer(Button button, string answer)
    {
        if (_effectTime)
            return;

        _effectTime = true;

        if (answer == _gameQuestions[_currentQuestion].answer)
        {
            if (++_score > _canvasDataManager.PlayerMaxScore)
            {
                _canvasDataManager.UpdatePlayerMaxScore(_score);
            }

            UpdateButtonColor(button, _trueAnswerColor);
            _gameBorder.color = _trueAnswerColor;
        }
        else
        {
            UpdateButtonColor(button, _wrongAnswerColor);
            _gameBorder.color = _wrongAnswerColor;

            _hp--;
            UpdateHealPoints();
        }

        await EndQuestion();
        ChangeQuestion();
    }


    private void StartGame(int gameId, bool saveData = false)
    {
        _lastGameId = gameId;
        _defeatMenu.SetActive(false);
        _isLoose = false;

        _hp = 3;
        UpdateHealPoints();

        _questionCancellationTokenSource = new CancellationTokenSource();

        if (!saveData)
        {
            _score = 0;

            _gameQuestions = _allQuestions[gameId].ToList();

            for (int i = 0; i < _gameQuestions.Count; i++)
            {
                int j = Random.Range(0, _gameQuestions.Count);
                (_gameQuestions[j], _gameQuestions[i]) = (_gameQuestions[i], _gameQuestions[j]);
            }
        }


        ChangeQuestion();

        Debug.Log("Game Start!");
    }

    private void StopGame()
    {
        _timerBar.StopTimer();
        _effectTime = false;

        _questionCancellationTokenSource?.Cancel();
        Debug.Log("Game Stop!");
    }

    private void ChangeQuestion()
    {
        Debug.Log($"Question {_currentQuestion}");
        _timerBar.ResetTimer();
        _timerBar.StartTimer();


        _gameImageController.LegsFocus();

        var newQuestion = _gameQuestions[++_currentQuestion];
        _gameImageController.SetImage(_gameImages[newQuestion.image]);

        _trueAnswerButton = Random.Range(0, 3);

        var randomNames = new List<string>();

        _gameBorder.color = Color.white;

        while (randomNames.Count < 4)
        {
            var name = _gameQuestions[Random.Range(0, _gameQuestions.Count)].answer;

            if (!randomNames.Contains(name) && name != newQuestion.answer)
                randomNames.Add(name);
        }

        for (int i = 0; i < _answerButtonTextFields.Count; i++)
        {
            UpdateButtonColor(_answerButtons[i], Color.white);

            if (i == _trueAnswerButton)
            {
                _answerButtonTextFields[i].text = newQuestion.answer;
                continue;
            }

            _answerButtonTextFields[i].text = randomNames[i];
        }

        _effectTime = false;
    }

    private void UpdateButtonColor(Button button, Color color)
    {
        button.colors = new ColorBlock()
        {
            pressedColor = color,
            selectedColor = color,
            normalColor = color,
            disabledColor = color,
            highlightedColor = color,
            colorMultiplier = 1.0f
        };
    }

    private void UpdateHealPoints()
    {
        if (_hp <= 0)
        {
            StopGame();
            _defeatMenuScore.SetText($"Отгадано ножек: {_score}");
            _defeatMenu.SetActive(true);
            return;
        }

        var position = _heartContainer.transform.position;
        var rect = _heartContainerRect.sizeDelta;


        position.y = _defaultHeartPosition.y - (3 - _hp) * _heartPart / 2;
        rect.y = _defaultHeartHeight - (3 - _hp) * _heartPart;

        _heartContainer.transform.position = position;
        _heartContainerRect.sizeDelta = rect;
    }

    private async Awaitable EndQuestion()
    {
        _timerBar.StopTimer();
        await _gameImageController.FaceFocus(_questionCancellationTokenSource.Token);
        try
        {
            await Awaitable.WaitForSecondsAsync(1, _questionCancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            // ignored
        }
    }

    private async Awaitable ProcessGameImage(string imageUrl)
    {
        while (_imagesInProcess >= 15)
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
        }

        _imagesInProcess++;
        _gameImages[imageUrl] = await WebUtils.DownloadImage(imageUrl);
        _progressBar.CompleteItem();

        _imagesInProcess--;
    }
}