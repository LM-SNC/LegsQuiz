using System;
using System.Collections.Generic;
using System.Linq;
using JsonModels;
using Reflex.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using YG;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [Inject] private ButtonsHandler _buttonsHandler;
    [Inject] private BackgroundsSwitcher _backgroundsSwitcher;
    [Inject] private CanvasDataManager _canvasDataManager;
    [Inject] private TranslationsManager _translationsManager;

    [SerializeField] private Animator _timeBarAnimator;
    [SerializeField] private Animator _imageAnimator;

    [SerializeField] private Color _trueAnswerColor;
    [SerializeField] private Color _wrongAnswerColor;

    [SerializeField] private GameObject[] _hearts;

    [SerializeField] private GameObject _defeatMenu;
    [SerializeField] private GameObject _winMenu;

    [SerializeField] private RawImage _gameImage;

    private Dictionary<int, List<Question>> _allQuestions;
    private List<Question> _currentQuestions;

    [SerializeField] private Button[] _answerButtons;

    [SerializeField] private int _currentQuestion;
    private int _hp;

    private int _score = 0;


    private void OnEnable() => YandexGame.RewardVideoEvent += Rewarded;

    private void Rewarded(int id) => OnResumeButton();

    private void OnDisable() => YandexGame.RewardVideoEvent -= Rewarded;


    private void Start()
    {
        _allQuestions = new Dictionary<int, List<Question>>();

        var questionsJson = Resources.Load<TextAsset>(@"Data/questions");
        var questions = JsonUtility.FromJson<Questions>(questionsJson.text);

        for (int i = 0; i < 4; i++)
        {
            _allQuestions[i] = new List<Question>();
        }

        foreach (var question in questions.Value)
        {
            _allQuestions[question.GameId].Add(question);
        }

        Debug.Log("Questions count: " + _allQuestions[0]);

        foreach (var answerButton in _answerButtons)
        {
            answerButton.onClick.AddListener(() =>
            {
                OnAnswerButton(answerButton,
                    answerButton.transform.GetChild(0).GetComponent<TMP_Text>().text);
            });
        }

        _buttonsHandler.AddHandler("StartButton",
            (button, canvas) => { OnStartButton(_backgroundsSwitcher.SelectedGame); });

        _buttonsHandler.AddHandler("BackButton", (button, canvas) => { OnBackButton(); });

        _buttonsHandler.AddHandler("RestartButton", (button, canvas) => { OnRestartButton(); });

        _buttonsHandler.AddHandler("ResumeButton", (button, canvas) => { YandexGame.RewVideoShow(0); });
    }

    private void OnAnswerButton(Button button, string answer)
    {
        EndQuestion();
        if (answer == _currentQuestions[_currentQuestion].Answer)
        {
            SetButtonColor(button, _trueAnswerColor);
        }
        else
        {
            SetButtonColor(button, _wrongAnswerColor);

            _hp--;
            UpdateHeartImage();
        }
    }


    private void OnStartButton(int gameId)
    {
        ResetGameState(-1);

        _currentQuestions = _allQuestions[gameId].ToList();

        for (var i = 0; i < _currentQuestions.Count; i++)
        {
            var j = Random.Range(0, _currentQuestions.Count);
            (_currentQuestions[j], _currentQuestions[i]) = (_currentQuestions[i], _currentQuestions[j]);
        }

        ShowQuestion();
    }

    private void OnBackButton()
    {
    }

    private void OnRestartButton()
    {
        ResetGameState(-1);
        ShowQuestion();
    }

    private void OnResumeButton()
    {
        ResetGameState(_currentQuestion - 1);
        ShowQuestion();
    }

    private void ResetGameState(int currentQuestion)
    {
        _defeatMenu.SetActive(false);
        _winMenu.SetActive(false);

        _currentQuestion = currentQuestion;
        _hp = 3;
        _score = 0;
        UpdateHeartImage();
    }

    public void OnAnimationEnd(string data)
    {
        if (data == "timer")
        {
            _hp--;
            UpdateHeartImage();
            EndQuestion();
        }
        else if (data == "image")
        {
            if (_hp <= 0)
            {
                _defeatMenu.transform.Find("Score").GetComponent<TMP_Text>()
                    .SetText($"{_translationsManager.GetPhrase("legs")}{_score}");
                _defeatMenu.SetActive(true);
            }
            else
            {
                _score++;
                _canvasDataManager.UpdatePlayerMaxScore(_score);

                YandexGame.NewLeaderboardScores("top", ++YandexGame.savesData.AllTimeScore);
                YandexGame.SaveProgress();

                ShowQuestion();
            }
        }
    }

    private void ShowQuestion()
    {
        SetAnswerButtonsStatus(true);

        foreach (var answerButton in _answerButtons)
        {
            SetButtonColor(answerButton, Color.white);
        }

        if (++_currentQuestion >= _currentQuestions.Count)
        {
            _winMenu.transform.Find("Score").GetComponent<TMP_Text>()
                .SetText($"{_translationsManager.GetPhrase("legs")}{_score}");


            _winMenu.SetActive(true);
            return;
        }

        Debug.Log("Show Question: " + _currentQuestion);
        var question = _currentQuestions[_currentQuestion];

        DownloadImage(question.Image, texture2D =>
        {
            _gameImage.texture = texture2D;

            var answers = GetRandomAnswers(question.Answer);
            for (int i = 0; i < _answerButtons.Length; i++)
            {
                SetButtonText(_answerButtons[i], answers[i]);
            }

            StartTimer();
            SetAnswerButtonsStatus(true);
        });

        if (_currentQuestion + 1 < _currentQuestions.Count)
            DownloadImage(_currentQuestions[_currentQuestion + 1].Image);
    }


    private void DownloadImage(string image, Action<Texture2D> action = null)
    {
        var operation = Addressables.LoadAssetAsync<Texture2D>(image);

        if (action == null) return;
        operation.Completed += handle => { action.Invoke(handle.Result); };
    }

    private void UpdateHeartImage()
    {
        SetHeart(4 - (_hp + 1));
    }

    private void StartTimer()
    {
        _timeBarAnimator.speed = 1;
        _timeBarAnimator.SetTrigger("start");
    }

    private void StopTimer()
    {
        _timeBarAnimator.SetTrigger("stop");
        _timeBarAnimator.speed = 0;
    }

    private void SetAnswerButtonsStatus(bool status)
    {
        foreach (var answerButton in _answerButtons)
        {
            answerButton.enabled = status;
        }
    }

    private void StartImageAnimation()
    {
        _imageAnimator.SetTrigger("move");
    }

    private List<string> GetRandomAnswers(string currentAnswer)
    {
        var randomNames = new List<string>();

        var answerPosition = Random.Range(0, 4);
        for (int i = 0; i < 4; i++)
        {
            if (i == answerPosition)
            {
                randomNames.Add(currentAnswer);
                continue;
            }

            var characterName = _currentQuestions[Random.Range(0, _currentQuestions.Count)].Answer;
            if (randomNames.Contains(characterName) && characterName != currentAnswer)
            {
                i--;
                continue;
            }

            randomNames.Add(characterName);
        }

        return randomNames;
    }

    private void SetButtonColor(Button button, Color color)
    {
        button.colors = new ColorBlock
        {
            pressedColor = color,
            selectedColor = color,
            normalColor = color,
            disabledColor = color,
            highlightedColor = color,
            colorMultiplier = 1.0f
        };
    }

    private void EndQuestion()
    {
        SetAnswerButtonsStatus(false);
        StopTimer();
        StartImageAnimation();
    }

    private void SetButtonText(Button button, string text)
    {
        button.transform.GetChild(0).GetComponent<TMP_Text>().text = text;
    }

    private void SetHeart(int id)
    {
        Debug.Log("ID: " + id);
        foreach (var heart in _hearts)
            heart.SetActive(false);

        _hearts[id].SetActive(true);
    }
}