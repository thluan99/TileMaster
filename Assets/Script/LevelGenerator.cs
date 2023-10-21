using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private Transform _tilePool;
    [SerializeField] private GameObject _tilePrefab;
    [SerializeField] private LevelSettings _levelSettings;

    public Level CurrentLevel => LevelManager.Instance.currentLevel;

    private void Start() 
    {
        for (int i = 0; i < CurrentLevel.TileType.Length; i++)
        {
            var tileType = CurrentLevel.TileType[i];

            for (int j = 0; j < tileType.NumberStar; j++)
            {
                for (int matchCount = 0; matchCount < CurrentLevel.MatchingCount; matchCount++)
                {
                    CreateTile(tileType);
                }
            }
        }
    }

    float randomX;
    float randomY;
    float randomZ;
    float randomYRotation;

    private void CreateTile(TileItems tileType)
    {
        randomX = Random.Range(-1.5f, 1.5f);
        randomZ = Random.Range(-2f, 4f);
        randomY = Random.Range(1f, 1.5f);
        randomYRotation = Random.Range(1, 180);

        var tileItem = Instantiate(_tilePrefab,
            new Vector3(randomX, randomY, randomZ),
            Quaternion.Euler(new Vector3(0, randomYRotation, 0))
            , _tilePool);

        var tileImages = tileItem.GetComponentsInChildren<Image>();

        for (int k = 0; k < tileImages.Length; k++)
        {
            tileImages[k].sprite = tileType.TileSprite;
        }

        tileItem.GetComponent<CubeController>().SetName(tileType.TileSprite.name);
    }
}