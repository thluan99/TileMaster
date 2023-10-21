using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button _playButton;
    [SerializeField] private TextMeshProUGUI _levelindexText;

    private int _levelIndex = 1;

    private void Start() 
    {
        _playButton.onClick.AddListener(PlayButtonClickHandler);

        if (PlayerPrefs.HasKey(Level.ActivatedLevelKey))
        {
            _levelIndex = PlayerPrefs.GetInt(Level.ActivatedLevelKey);
        }
        _levelindexText.SetText(_levelIndex.ToString());
    }

    private void PlayButtonClickHandler()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
