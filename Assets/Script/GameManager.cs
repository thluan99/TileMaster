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
    private const string CUBE_LAYER = "Cube";
    private const int GAIN_POINT_NUMBER = 3;
    [SerializeField] private GamePlayUI _gamePlayUI;
    [SerializeField] private Transform _cubePool;
    [SerializeField] private List<Transform> _rectPositions;

    private Stack<IUndo> _history;
    private int _collectCount = 1;
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
        GamePlayObservable.LoseHandleRequest.Subscribe(_ =>
        {
            GamePlayCurrentState.Value = GamePlayState.Lost;
        }).AddTo(gameObject);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && GamePlayCurrentState.Value == GamePlayState.Playing)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 20))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.layer == LayerMask.NameToLayer(CUBE_LAYER) &&
                    !hitObject.GetComponentInChildren<IStored>().IsStored)
                {
                    SelectCube(hitObject);
                    ChangeGainStar();
                }
            }
        }
    }

    public void Undo()
    {
        if (_history.Count == 0) return;

        IUndo command = _history.Pop();
        command.Undo();

        ReloadCubePositions();
    }

    private void ReloadCubePositions()
    {
        for (int i = 0; i < _rectPositions.Count - 1; i++)
        {
            if (_rectPositions[i].childCount == 0 && _rectPositions[i + 1].childCount != 0)
            {
                ShiftCubeAllLeft(i + 1);
                break;
            }
        }
    }

    private void SelectCube(GameObject hitObject)
    {
        for (int i = 0; i < _rectPositions.Count; i++)
        {
            var cubeItem = _rectPositions[i].GetComponentInChildren<IEntity>();

            if (cubeItem != null && cubeItem.Name == hitObject.GetComponent<IEntity>().Name)
            {
                SameNameHandler(hitObject, i, cubeItem);
                break;
            }

            if (_rectPositions[i].transform.childCount == 0)
            {
                IUndo undoCommand = hitObject.GetComponent<IUndo>();
                _history.Push(undoCommand);

                MoveCubeToOtherPosition(hitObject, i);
                hitObject.GetComponentInChildren<IStored>().IsStored = true;
                break;
            }
        }
    }

    private void SameNameHandler(GameObject hitObject, int i, IEntity cubeItem)
    {
        for (int j = _rectPositions.Count - 1; j >= i; j--)
        {
            if (_rectPositions[j].childCount != 0 && _rectPositions[j].GetComponentInChildren<IEntity>().Name == cubeItem.Name)
            {
                ShiftCubeAllRight(j + 1);

                IUndo undoCommand = hitObject.GetComponent<IUndo>();
                _history.Push(undoCommand);

                MoveCubeToOtherPosition(hitObject, j + 1);

                break;
            }
        }
    }

    private void ShiftCubeAllLeft(int start, int length = 1)
    {
        for (int i = start; i < _rectPositions.Count; i++)
        {
            if (_rectPositions[i].childCount == 0) continue;

            GameObject transformRect = _rectPositions[i].GetChild(0).gameObject;
            MoveCubeToOtherPosition(transformRect, i - length);
        }
    }

    private void ShiftCubeAllRight(int start)
    {
        for (int i = _rectPositions.Count - 1; i >= start; i--)
        {
            if (_rectPositions[i].childCount != 0 && (i != _rectPositions.Count - 1))
            {
                GameObject transformRect = _rectPositions[i].GetChild(0).gameObject;
                MoveCubeToOtherPosition(transformRect, i + 1);
            }
        }
    }

    private void MoveCubeToOtherPosition(GameObject hitObject, int position)
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
            var cubeItem = _rectPositions[i].GetComponentInChildren<IEntity>();

            if (cubeItem == null) return;

            if (name == cubeItem.Name)
            {
                _collectCount++;
            }
            else
            {
                name = cubeItem.Name;
                _collectCount = 1;
            }

            if (_collectCount == GAIN_POINT_NUMBER)
            {
                StartCoroutine(HandleGainStar(i));
                _history.Clear();
                return;
            }

        }

        GamePlayCurrentState.Value = GamePlayState.Lost;
    }

    private IEnumerator HandleGainStar(int index)
    {
        yield return new WaitForSeconds(TIME_TO_ANIMATE);

        for (int j = index - GAIN_POINT_NUMBER + 1; j <= index; j++)
        {
            Destroy(_rectPositions[j].GetChild(0).gameObject);
        }

        for (int j = index + 1; j < _rectPositions.Count; j++)
        {
            ShiftCubeAllLeft(j, GAIN_POINT_NUMBER);
        }

        _gamePlayUI.IncreaseStar();

        if (_cubePool.childCount == 0)
        {
            GamePlayCurrentState.Value = GamePlayState.Won;
        }
    }

    private void Reset()
    {
        _rectPositions.Clear();

        foreach (Transform item in _cubePool.transform)
        {
            _rectPositions.Add(item);
        }
    }
}

