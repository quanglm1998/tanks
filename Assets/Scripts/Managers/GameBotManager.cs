﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameBotManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 3;
    public float m_StartDelay = 1f;
    public float m_EndDelay = 2f;
    public CameraControl m_CameraControl;
    public Text m_MessageText;
    public GameObject m_TankPrefab;
    public GameObject m_TankBotPrefab;
    public TankManager[] m_Tanks;


    private int m_RoundNumber;
    private WaitForSeconds m_StartWait;
    private WaitForSeconds m_EndWait;
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;
    private GameObject panel;


    private void Start()
    {
        panel = GameObject.FindGameObjectWithTag("Panel"); 
        panel.SetActive(false);
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        SpawnAllTanks();
        SetCameraTargets();

        StartCoroutine(GameLoop());
    }


    private void BotSetup(TankManager tankBot) {
        if (DifficultyManager.difficulty == 0) {
            Debug.Log("Easy level");
            tankBot.GetTankMovement().SetUp(3f, 0.6f, 0.5f);
            tankBot.GetTankShooting().SetUp(4f, 0.2f);
            return;
        }
        if (DifficultyManager.difficulty == 1) {
            Debug.Log("Medium level");
            tankBot.GetTankMovement().SetUp(2f, 1f, 0.8f);
            tankBot.GetTankShooting().SetUp(2f, 0.3f);
            return;
        }
        if (DifficultyManager.difficulty == 2) {
            Debug.Log("Hard level");
            tankBot.GetTankMovement().SetUp(1.4f, 1.5f, 1f);
            tankBot.GetTankShooting().SetUp(1f, 0.5f);
            return;
        }
        Debug.Log("Insane level");
    }
    private void SpawnAllTanks()
    {
        // human controlled
        m_Tanks[0].m_Instance = Instantiate(m_TankPrefab, m_Tanks[0].m_SpawnPoint.position,
                m_Tanks[0].m_SpawnPoint.rotation) as GameObject;
        m_Tanks[0].m_PlayerNumber = 1;
        m_Tanks[0].Setup();

        // bot controlled
        m_Tanks[1].m_Instance = Instantiate(m_TankBotPrefab, m_Tanks[1].m_SpawnPoint.position,
                m_Tanks[1].m_SpawnPoint.rotation) as GameObject;
        m_Tanks[1].m_PlayerNumber = 2;
        m_Tanks[1].Setup();
        BotSetup(m_Tanks[1]);
    }


    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }


    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            m_MessageText.text = string.Empty;
            m_MessageText.enabled = false;
            panel.GetComponent<Image>().color = new Color(118f / 255f, 99f / 255f, 74f/255f, 91f/255f);
            panel.GetComponent<Image>().sprite = null;
            panel.SetActive(true);
            while (true) yield return null;
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        ResetAllTanks();
        DisableTankControl();

        m_CameraControl.SetStartPositionAndSize();

        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        EnableTankControl();

        m_MessageText.text = string.Empty;

        while (!OneTankLeft())
        {
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        DisableTankControl();

        m_RoundWinner = GetRoundWinner();

        if (m_RoundWinner != null)
        {
            m_RoundWinner.m_Wins++;
        }

        m_GameWinner = GetGameWinner();

        m_MessageText.text = EndMessage();

        yield return m_EndWait;
    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}