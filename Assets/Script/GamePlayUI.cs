using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    private const int TIME_TO_PLAY = 400;
    [SerializeField] private Button _undoButton;
    [SerializeField] private TextMeshProUGUI _starText;
    [SerializeField] private TextMeshProUGUI _timeText;

    private int _time;
    private int _minutes;
    private int _seconds;
    private GamePlayObservable _gamePlayObservable;

    private void Start()
    {
        _gamePlayObservable = GameManager.Instance.GamePlayObservable;
        _time = TIME_TO_PLAY;
        _undoButton.onClick.AddListener(OnClickUndoButtonHandler);

        InvokeRepeating("DecreaseTime", 1, 1);

        GameManager.Instance.GamePlayCurrentState
            .Where(state => state == GamePlayState.Won)
            .Subscribe(_ => {
                CancelInvoke("DecreaseTime");
                Debug.Log("You win");
            }).AddTo(gameObject);

        GameManager.Instance.GamePlayCurrentState
            .Where(state => state == GamePlayState.Lost)
            .Subscribe(_ => {
                CancelInvoke("DecreaseTime");
                Debug.Log("You Lose!");
            }).AddTo(gameObject);
    }

    private void OnClickUndoButtonHandler()
    {
        GameManager.Instance.Undo();
    }

    private void DecreaseTime()
    {
        _time--;

        _minutes = _time / 60;
        _seconds = _time % 60;

        _timeText.SetText(_minutes.ToString().PadLeft(2, '0') + ":" + _seconds.ToString().PadLeft(2, '0'));

        if (_time == 0)
        {
            _gamePlayObservable.LoseHandleRequest.OnNext(Unit.Default);
        }
    }

    public void IncreaseStar(int inscreaseStarCount = 1)
    {
        int starNumber = int.Parse(_starText.text) + inscreaseStarCount;
        _starText.SetText(starNumber.ToString());
    }
}
