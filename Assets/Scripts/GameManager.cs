using System.Collections.Generic;
using System.Linq;
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
    [Inject] private BackgroundsSwitcher _backgroundsSwitcher;

    [SerializeField] private TMP_Text _scoreField;
    
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


    [SerializeField] private Color _trueAnswerColor;
    [SerializeField] private Color _wrongAnswerColor;
    
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


        for (int i = 0; i < 4; i++)
        {
            _allQuestions[i] = new List<Questions.Question>();
            var questions = (await _legsQuizApi.GetData<Questions>($"?gameid={i}")).value;
            _progressBar.AddProgressItems(questions.Count);
            
            foreach (var question in questions)
            {
                Debug.Log("Question image: " + question.image);
                _allQuestions[i].Add(question);
                ProcessGameImage(question.image);
            }
        }

        _buttonsHandler.AddHandler("StartButton",
            async (button, canvas) => { StartGame(_backgroundsSwitcher.SelectedGame); });

        _buttonsHandler.AddHandler("BackButton", async (button, canvas) => { StopGame(); });
    }

    private async void ProcessAnswer(Button button, string answer)
    {
        if (_effectTime)
            return;

        if (answer == _gameQuestions[_currentQuestion].answer)
        {
            _score++;
            UpdateButtonColor(button, _trueAnswerColor);
            _gameBorder.color = _trueAnswerColor;
            _scoreField.SetText(_score.ToString());
        }
        else
        {
            UpdateButtonColor(button, _wrongAnswerColor);
            _gameBorder.color = _wrongAnswerColor;
        }

        _effectTime = true;

        _timerBar.StopTimer();
        await _gameImageController.FaceFocus();
        await Awaitable.WaitForSecondsAsync(1);
        ChangeQuestion();
    }

    private void StartGame(int gameId)
    {
        _score = 0;
        _gameQuestions = _allQuestions[gameId].ToList();

        for (int i = 0; i < _gameQuestions.Count; i++)
        {
            int j = Random.Range(0, _gameQuestions.Count);
            (_gameQuestions[j], _gameQuestions[i]) = (_gameQuestions[i], _gameQuestions[j]);
        }


        ChangeQuestion();

        Debug.Log("Game Start!");
    }

    private void StopGame()
    {
        _timerBar.StopTimer();
        _effectTime = false;

        Debug.Log("Game Stop!");
    }

    private void ChangeQuestion()
    {
        _timerBar.ResetTimer();
        _timerBar.StartTimer();


        var newQuestion = _gameQuestions[++_currentQuestion];
        _gameImageController.SetImage(_gameImages[newQuestion.image]);
        _gameImageController.LegsFocus();

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