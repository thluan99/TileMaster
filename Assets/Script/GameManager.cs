using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static readonly float TIME_TO_ANIMATE = 0.4F;
    private const string CUBE_LAYER = "Cube";
    private const int GAIN_POINT_NUMBER = 3;
    [SerializeField] private List<Transform> _rectPositions;

    private Stack<IUndo> _history;
    private int _collectCount = 1;

    private void Awake() 
    {
        _history = new Stack<IUndo>();
        _rectPositions = new List<Transform>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 20))
            {
                GameObject hitObject = hit.collider.gameObject;
                if (hitObject.layer == LayerMask.NameToLayer(CUBE_LAYER))
                {
                    SelectCube(hitObject);
                    ChangeGainPoint();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Z) && _history.Count > 0)
        {
            IUndo command = _history.Pop();
            command.Undo();

            ReloadCubePositions();
        }
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

            if ( cubeItem != null && cubeItem.Name == hitObject.GetComponent<IEntity>().Name)
            {
                SameNameHandler(hitObject, i, cubeItem);
                break;
            }

            if (_rectPositions[i].transform.childCount == 0)
            {
                IUndo undoCommand = hitObject.GetComponent<IUndo>();
                _history.Push(undoCommand);

                MoveCubeToOtherPosition(hitObject, i);
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

    private void ChangeGainPoint()
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
                StartCoroutine(DestroyAndShipCube(i));
                _history.Clear();
                break;
            }
        }
    }

    private IEnumerator DestroyAndShipCube(int index)
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
    }

    private void Reset() 
    {
        var storeCubePanel = GameObject.Find("StoreCubePanel");
        _rectPositions.Clear();

        foreach (Transform item in storeCubePanel.transform)
        {
            _rectPositions.Add(item);
        }
    }
}

