using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUI : MonoBehaviour
{
    [SerializeField] private Button _undoButton;

    private void Start() 
    {
        _undoButton.onClick.AddListener(OnClickUndoButtonHandler);
    }

    private void OnClickUndoButtonHandler()
    {
        GameManager.Instance.Undo();
    }
}
