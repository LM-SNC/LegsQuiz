using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JsonModels;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private BackgroundsSwitcher _backgroundsSwitcher;
    [Inject] private CanvasDataManager _canvasDataManager;
    [Inject] private TranslationsManager _translationsManager;


    private TimerBar _timerBar;
    private GameImageController _gameImageController;

    private Dictionary<int, List<Question>> _allQuestions;
    private List<Question> _gameQuestions;


    //private Dictionary<string, string> _gameImages;

    [SerializeField] private GameObject _buttonsContainer;
    private List<TMP_Text> _answerButtonTextFields;
    private List<Button> _answerButtons;

    [SerializeField] private int _currentQuestion = -1;
    private int _score = 0;

    private int _imagesInProcess;
    private int _trueAnswerButton;
    private bool _effectTime = false;

    private int _lastGameId;


    [SerializeField] private Color _trueAnswerColor;
    [SerializeField] private Color _wrongAnswerColor;


    private CancellationTokenSource _questionCancellationTokenSource;

    private int _hp = 1;
    [SerializeField] private RawImage _heart;
    [SerializeField] private Sprite[] _heartState;

    private bool _isLoose;

    [SerializeField] private GameObject _defeatMenu;
    [SerializeField] private GameObject _winMenu;
    private TMP_Text _defeatMenuScore;
    private TMP_Text _winMenuScore;

    private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;

    private void Rewarded(int id)
    {
        _currentQuestion -= 1;
        StartGame(_lastGameId, true);
    }

    private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;


    private async void Start()
    {
        _timerBar = gameObject.GetComponent<TimerBar>();
        _gameImageController = gameObject.GetComponent<GameImageController>();

        _gameImageController.OnImageLoaded += () =>
        {
            _timerBar.StartTimer();

            foreach (var answerButton in _answerButtons)
            {
                answerButton.enabled = true;
            }
        };


        _allQuestions = new();
        _gameQuestions = new();
        //_gameImages = new(150);
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

        var questionsJson = Resources.Load<TextAsset>(@"Data/questions");
        Debug.Log(questionsJson.text);
        var questions = JsonUtility.FromJson<Questions>(questionsJson.text);

        for (int i = 0; i < 4; i++)
        {
            _allQuestions[i] = new List<Question>();
        }

        foreach (var question in questions.Value)
        {
            _allQuestions[question.GameId].Add(question);
        }

        _buttonsHandler.AddHandler("StartButton",
            async (button, canvas) => { StartGame(_backgroundsSwitcher.SelectedGame); });

        _buttonsHandler.AddHandler("BackButton", async (button, canvas) =>
        {
            _canvasDataManager.UpdateScoreText();
            StopGame();
        });

        _buttonsHandler.AddHandler("RestartButton", async (button, canvas) => { StartGame(_lastGameId); });

        _buttonsHandler.AddHandler("ResumeButton", async (button, canvas) => { YandexGame.RewVideoShow(0); });

        _timerBar.OnTimerEnd += async () =>
        {
            if (_hp <= 0)
                return;

            _hp--;
            UpdateHealPoints();

            _effectTime = true;

            await EndQuestion();
            ChangeQuestion();
        };


        _defeatMenuScore = _defeatMenu.transform.Find("Score").GetComponent<TMP_Text>();
        _winMenuScore = _winMenu.transform.Find("Score").GetComponent<TMP_Text>();


        _translationsManager.Register("legs", "ru", "Отгадано ножек: ");
        _translationsManager.Register("legs", "en", "LEGS GUESSED: ");
        _translationsManager.Register("legs", "tr", "TAHMİN EDİLEN BACAK SAYISI: ");
    }

    private async void ProcessAnswer(Button button, string answer)
    {
        if (_effectTime)
            return;

        _effectTime = true;

        if (answer == _gameQuestions[_currentQuestion].Answer)
        {
            YandexGame.NewLeaderboardScores("top", ++YandexGame.savesData.AllTimeScore);
            YandexGame.SaveProgress();

            if (++_score > _canvasDataManager.PlayerMaxScore)
            {
                _canvasDataManager.UpdatePlayerMaxScore(_score);
            }

            UpdateButtonColor(button, _trueAnswerColor);
        }
        else
        {
            UpdateButtonColor(button, _wrongAnswerColor);

            _hp--;
            UpdateHealPoints();
        }

        await EndQuestion();

        if (_hp > 0 && !_questionCancellationTokenSource.Token.IsCancellationRequested)
            ChangeQuestion();
    }


    private void StartGame(int gameId, bool saveData = false)
    {
        _lastGameId = gameId;
        _defeatMenu.SetActive(false);
        _winMenu.SetActive(false);
        _isLoose = false;

        _hp = 3;
        UpdateHealPoints();

        _questionCancellationTokenSource?.Dispose();
        _questionCancellationTokenSource = new CancellationTokenSource();

        if (!saveData)
        {
            _currentQuestion = 0;
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
        foreach (var answerButton in _answerButtons)
        {
            answerButton.enabled = false;
        }

        Debug.Log($"Question {_currentQuestion}");

        if (++_currentQuestion >= _gameQuestions.Count)
        {
            StopGame();
            _winMenuScore.SetText($"{_translationsManager.GetPhrase("legs")}: {_score}");
            _winMenu.SetActive(true);
            return;
        }

        _timerBar.ResetTimer();


        _gameImageController.LegsFocus();

        var newQuestion = _gameQuestions[_currentQuestion];
        _gameImageController.SetImage(newQuestion.Image);
        if (_gameQuestions.Count > _currentQuestion + 1)
        {
            _gameImageController.DownloadImage(false, _gameQuestions[_currentQuestion + 1].Image);
        }

        _trueAnswerButton = Random.Range(0, 3);

        var randomNames = new List<string>();

        while (randomNames.Count < 4)
        {
            var name = _gameQuestions[Random.Range(0, _gameQuestions.Count)].Answer;

            if (!randomNames.Contains(name) && name != newQuestion.Answer)
                randomNames.Add(name);
        }

        for (int i = 0; i < _answerButtonTextFields.Count; i++)
        {
            UpdateButtonColor(_answerButtons[i], Color.white);

            if (i == _trueAnswerButton)
            {
                _answerButtonTextFields[i].text = newQuestion.Answer;
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
            _defeatMenuScore.SetText($"{_translationsManager.GetPhrase("legs")}{_score}");
            _defeatMenu.SetActive(true);
            return;
        }

        _heart.texture = _heartState[Math.Abs(_hp - 1)].texture;
    }

    private async Awaitable EndQuestion()
    {
        _timerBar.StopTimer();
        StartCoroutine(_gameImageController.FaceFocus(_questionCancellationTokenSource.Token));

        while (_gameImageController.IsMoving)
        {
            await Awaitable.WaitForSecondsAsync(0.5f);
        }

        try
        {
            await Awaitable.WaitForSecondsAsync(1, _questionCancellationTokenSource.Token);
        }
        catch (Exception e)
        {
            // ignored
        }
    }
}