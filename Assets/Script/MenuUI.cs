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
    [SerializeField] private TextMeshProUGUI _starText;

    private int _levelIndex = 1;

    private void Start() 
    {
        _playButton.onClick.AddListener(PlayButtonClickHandler);

        LevelManager.Instance.LoadCurrentLevel();
        _levelIndex = LevelManager.Instance.currentLevel.LevelIndex;

        _levelindexText.SetText(_levelIndex.ToString());

        if (PlayerPrefs.HasKey(Constanst.NUMBER_STAR_KEY))
        {
            int starCount = PlayerPrefs.GetInt(Constanst.NUMBER_STAR_KEY);
            _starText.SetText(starCount.ToString());
        }
    }

    private void PlayButtonClickHandler()
    {
        SceneManager.LoadScene("SampleScene");
    }
}
