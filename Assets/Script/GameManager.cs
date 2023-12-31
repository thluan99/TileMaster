using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UniRx;
using System;

public enum GamePlayState
{
    Playing,
    Won,
    Lost
}

public class GameManager : MonoBehaviour
{
    public static readonly float TIME_TO_ANIMATE = 0.4F;
    public static GameManager Instance; // Singleton
    private const string TILE_LAYER = "Tile";

    [SerializeField] private GamePlayUI _gamePlayUI;
    [SerializeField] private Transform _tilePool;
    [SerializeField] private List<Transform> _rectPositions;

    private Stack<IUndo> _history;
    private int _collectCount = 1;
    private bool _isGainingStar = false;
    private int _gainPointNumber = 3;
    private Level _currentLevel;

    public GamePlayObservable GamePlayObservable { get; private set; }
    public ReactiveProperty<GamePlayState> GamePlayCurrentState { get; private set; }

    private void Awake()
    {
        Instance = this;
        GamePlayObservable = GetComponent<GamePlayObservable>();
        _history = new Stack<IUndo>();
        GamePlayCurrentState = new ReactiveProperty<GamePlayState>(GamePlayState.Playing);
    }

    private void Start()
    {
        LoadLevelSetting();

        GamePlayObservable.LoseHandleRequest.Subscribe(_ =>
        {
            GamePlayCurrentState.Value = GamePlayState.Lost;
            SoundManager.Instance.PlaySFX(SoundConstants.LOSE);
        }).AddTo(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GamePlayCurrentState.Value == GamePlayState.Playing && !_isGainingStar)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 20))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.layer == LayerMask.NameToLayer(TILE_LAYER) &&
                    !hitObject.GetComponentInChildren<IStored>().IsStored)
                {
                    SelectTile(hitObject);
                    ChangeGainStar();
                }
            }
        }
    }

    private void LoadLevelSetting()
    {
        _currentLevel = LevelManager.Instance.currentLevel;
        _gainPointNumber = _currentLevel.MatchingCount;
        _gamePlayUI.SetTime(_currentLevel.PlayTime);
        _gamePlayUI.SetLelvel(_currentLevel.LevelIndex);
    }

    public void Undo()
    {
        if (_history.Count == 0) return;

        IUndo command = _history.Pop();
        command.Undo();

        ReloadTilePositions();
    }

    private void ReloadTilePositions()
    {
        for (int i = 0; i < _rectPositions.Count - 1; i++)
        {
            if (_rectPositions[i].childCount == 0 && _rectPositions[i + 1].childCount != 0)
            {
                ShiftTileAllLeft(i + 1);
                break;
            }
        }
    }

    private void SelectTile(GameObject hitObject)
    {
        for (int i = 0; i < _rectPositions.Count; i++)
        {
            var tileItem = _rectPositions[i].GetComponentInChildren<IEntity>();
            hitObject.GetComponent<Rigidbody>().isKinematic = true;


            if (tileItem != null && tileItem.Name == hitObject.GetComponent<IEntity>().Name)
            {
                SameNameHandler(hitObject, i, tileItem);
                break;
            }

            if (_rectPositions[i].transform.childCount == 0)
            {
                MoveTileSelected(hitObject, i);
                break;
            }
        }
    }

    private void MoveTileSelected(GameObject hitObject, int i)
    {
        SoundManager.Instance.PlaySFX(SoundConstants.SELECT_TILE);

        IUndo undoCommand = hitObject.GetComponent<IUndo>();
        _history.Push(undoCommand);

        MoveTileToOtherPosition(hitObject, i);
        hitObject.GetComponentInChildren<IStored>().IsStored = true;
    }

    private void SameNameHandler(GameObject hitObject, int i, IEntity tileItem)
    {
        for (int j = _rectPositions.Count - 1; j >= i; j--)
        {
            if (_rectPositions[j].childCount != 0 && _rectPositions[j].GetComponentInChildren<IEntity>().Name == tileItem.Name)
            {
                ShiftTileAllRight(j + 1);
                MoveTileSelected(hitObject, j + 1);

                break;
            }
        }
    }

    private void ShiftTileAllLeft(int start, int length = 1)
    {
        for (int i = start; i < _rectPositions.Count; i++)
        {
            Debug.Log("Log i: " + i + " length: " + length);

            if (_rectPositions[i].childCount == 0) continue;

            GameObject transformRect = _rectPositions[i].GetChild(0).gameObject;
            MoveTileToOtherPosition(transformRect, i - length);
        }
    }

    private void ShiftTileAllRight(int start)
    {
        for (int i = _rectPositions.Count - 2; i >= start; i--)
        {
            if (_rectPositions[i].childCount != 0 && (i != _rectPositions.Count - 1))
            {
                GameObject transformRect = _rectPositions[i].GetChild(0).gameObject;
                MoveTileToOtherPosition(transformRect, i + 1);
            }
        }
    }

    private void MoveTileToOtherPosition(GameObject hitObject, int position)
    {
        DOTween.Kill(hitObject.transform);

        hitObject.transform.SetParent(_rectPositions[position]);
        hitObject.transform.DORotate(Vector3.zero, TIME_TO_ANIMATE);
        hitObject.transform.DOLocalMove(Vector3.zero, TIME_TO_ANIMATE);
    }

    private void ChangeGainStar()
    {
        string name = "";

        for (int i = 0; i < _rectPositions.Count; i++)
        {
            var tileItem = _rectPositions[i].GetComponentInChildren<IEntity>();

            if (tileItem == null) return;

            if (name == tileItem.Name)
            {
                _collectCount++;
            }
            else
            {
                name = tileItem.Name;
                _collectCount = 1;
            }

            if (_collectCount == _gainPointNumber)
            {
                StartCoroutine(HandleGainStar(i));
                _history.Clear();
                return;
            }

        }

        GamePlayObservable.LoseHandleRequest.OnNext(Unit.Default);
    }

    private IEnumerator HandleGainStar(int index)
    {
        _isGainingStar = true;
        yield return new WaitForSeconds(TIME_TO_ANIMATE);

        for (int j = index - _gainPointNumber + 1; j <= index; j++)
        {
            Destroy(_rectPositions[j].GetChild(0).gameObject);
        }

        ShiftTileAllLeft(index + 1, _gainPointNumber);

        _gamePlayUI.IncreaseStar();
        SoundManager.Instance.PlaySFX(SoundConstants.COLLECT_TILE);

        if (_tilePool.childCount == 0)
            HandleWin();

        _isGainingStar = false;
    }

    private void HandleWin()
    {
        GamePlayCurrentState.Value = GamePlayState.Won;

        int starCount = PlayerPrefs.HasKey(Constanst.NUMBER_STAR_KEY) ? PlayerPrefs.GetInt(Constanst.NUMBER_STAR_KEY) : 0;
        PlayerPrefs.SetInt(Constanst.NUMBER_STAR_KEY, starCount + _currentLevel.NumberOfStar);
        
        SoundManager.Instance.PlaySFX(SoundConstants.WIN);
        LevelManager.Instance.NextLevel();
    }

    // ======================

    private void Reset()
    {
        _rectPositions.Clear();

        foreach (Transform item in _tilePool.transform)
        {
            _rectPositions.Add(item);
        }
    }
}

