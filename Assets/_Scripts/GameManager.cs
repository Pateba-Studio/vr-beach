using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
    public GameObject ballObjectSpawned;
    public GameType gameType = GameType.None;
    [SerializeField] public List<GameComponent> gameComponents;

    GameObject temp;

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
                if (temp != null)
                {
                    Destroy(temp);
                    temp = null;
                }

                Destroy(gameObjectGroupCurr);
                gameObjectGroupCurr = null;

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

        var obj = Instantiate(gameComponents[(int)gameType].gameObjectGroup);
        obj.SetActive(true);

        if (obj.GetComponent<XRGrabInteractable>() == null)
        {
            gameObjectGroupCurr = obj.GetComponentInChildren<XRGrabInteractable>().gameObject;
            temp = obj;
        }
        else
        {
            gameObjectGroupCurr = obj;
        }

        gameObjectGroupCurr.SetActive(false);
        gameObjectGroupCurr.transform.parent = gameComponents[(int)gameType].gameGroup.transform;
        gameObjectGroupCurr.transform.position = gameComponents[(int)gameType].gameObjectSpawnPos.position;

        StartCoroutine(GameTimer());
    }

    public void StopGame()
    {
        if (gameType != GameType.None)
        {
            StopAllCoroutines();
            if (temp != null)
            {
                Destroy(temp);
                temp = null;
            }

            Destroy(gameObjectGroupCurr);
            gameObjectGroupCurr = null;

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

    public void GrabCurrentBall(Transform attachPoint)
    {
        if (gameObjectGroupCurr == null) return;
        if (ballObjectSpawned == null) 
        {
            ballObjectSpawned = Instantiate(gameObjectGroupCurr);
        }

        ballObjectSpawned.SetActive(true);
        ballObjectSpawned.transform.position = attachPoint.position;
        ballObjectSpawned.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ballObjectSpawned.GetComponent<BallDestroyer>().isWork = true;
        Debug.Log("Trying Grab: " + ballObjectSpawned.name);
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
