using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum GameType
{
    Archer,
    Basketball,
    Frisbee,
    Throwcan,
    None
}

[Serializable]
public class GameComponent
{
    public int initTime;
    public int gameScore;
    public Transform gameObjectSpawnPos;
    public GameObject gameObjectGroup;
    public GameObject gameGroup;
    public GameObject gameCanvas;
    public GameObject timerPanel;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI timerText;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public int globalTime;
    public GameObject gameObjectGroupCurr;
    public GameType gameType = GameType.None;
    [SerializeField] public List<GameComponent> gameComponents;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private void Update()
    {
        if (globalTime <= 0)
        {
            if (gameType != GameType.None)
            {
                StopCoroutine(GameTimer());
                Destroy(gameObjectGroupCurr);
                gameComponents[(int)gameType].gameGroup.SetActive(false);
                gameComponents[(int)gameType].timerPanel.SetActive(false);
                gameComponents[(int)gameType].gameScore = 0;

                foreach (GameComponent component in gameComponents)
                {
                    component.gameCanvas.SetActive(true);
                }

                gameType = GameType.None;
                globalTime = 0;
            }
        }
        else
        {
            if (gameType != GameType.None)
            {
                gameComponents[(int)gameType].scoreText.text = gameComponents[(int)gameType].gameScore.ToString();
            }
        }
    }

    public void PlayGame(int gameIndex)
    {
        gameType = (GameType)gameIndex;
        globalTime = gameComponents[(int)gameType].initTime;

        gameComponents[(int)gameType].gameGroup.SetActive(true);
        gameComponents[(int)gameType].timerPanel.SetActive(true);
        foreach (GameComponent component in gameComponents)
        {
            component.gameCanvas.SetActive(false);
        }

        gameObjectGroupCurr = Instantiate(gameComponents[(int)gameType].gameObjectGroup);
        
        gameObjectGroupCurr.SetActive(true);
        gameObjectGroupCurr.transform.parent = gameComponents[(int)gameType].gameGroup.transform;
        gameObjectGroupCurr.transform.position = gameComponents[(int)gameType].gameObjectSpawnPos.position;

        StartCoroutine(GameTimer());
    }

    IEnumerator GameTimer()
    {
        globalTime--;
        gameComponents[(int)gameType].timerText.text = globalTime.ToString();

        yield return new WaitForSeconds(1);

        if (gameType != GameType.None)
        {
            StartCoroutine(GameTimer());
        }
    }
}
