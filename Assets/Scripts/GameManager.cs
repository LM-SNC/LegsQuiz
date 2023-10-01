using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TimerBar _timerBar;

    // Start is called before the first frame update
    private void Start()
    {
        _timerBar = gameObject.GetComponent<TimerBar>();

        _timerBar.StartTimer();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}