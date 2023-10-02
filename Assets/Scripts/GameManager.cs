using System.Collections.Generic;
using JsonModels;
using Reflex.Attributes;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Inject] private LegsQuizApi _legsQuizApi;
    private TimerBar _timerBar;
    private GameImageController _gameImageController;

    private Dictionary<int, List<Questions.Question>> _allQuestions;
    private Queue<Questions.Question> _gameQuestions;
    private Dictionary<string, Texture2D> _gameImages;

    // Start is called before the first frame update
    private async void Start()
    {
        _timerBar = gameObject.GetComponent<TimerBar>();
        _gameImageController = gameObject.GetComponent<GameImageController>();

        _allQuestions = new();
        _gameQuestions = new();
        _gameImages = new();

        for (int i = 0; i < 4; i++)
        {
            _allQuestions[i] = new List<Questions.Question>();

            foreach (var question in (await _legsQuizApi.GetData<Questions>($"?gameid={i}")).value)
            {
                Debug.Log("Question image: " + question.image);
                _allQuestions[i].Add(question);
            }
        }
    }

    public void StartGame(int gameId)
    {
        ProcessGameImages(gameId);
    }

    public void StopGame()
    {
        
    }

    private async Awaitable ProcessGameImages(int gameId)
    {
        foreach (var question in _allQuestions[gameId])
        {
            var texture = await WebUtils.DownloadImage(question.image);
            _gameImages[question.image] = texture;
        }
    }
}