using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

namespace JumpRace
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        [SerializeField] private GameObject[] levelPrefabs;
        [SerializeField] private GameObject losePanel;
        [SerializeField] private GameObject winPanel;
        [SerializeField] private GameObject endParticles;
        [SerializeField] private Text levelTextInGame;
        [SerializeField] private Text levelTextInGame2;
        [SerializeField] private GameObject perfectText;
        [SerializeField] private GameObject longJumpText;
        [SerializeField] private Image levelBar;
        [SerializeField] private GameObject[] levelSplines;
        [SerializeField] private GameObject AI1, AI2;

        public bool gameOver;
        public int currentLevel;
        public GameObject player;
        public float maxTrampolineCount = 0;
        private void Awake()
        {
            Instance = this;
            GameObject.Find("HCLevelManager").GetComponent<HCLevelManager>().Init();
            GameObject.Find("HCLevelManager").GetComponent<HCLevelManager>().GenerateCurrentLevel();
            levelSplines[GameObject.Find("HCLevelManager").GetComponent<HCLevelManager>()._levelIndex].SetActive(true);
        }
        private void Start()
        {
            //Application.targetFrameRate = 60;
            levelBar.fillAmount = 0;
            currentLevel = GetLevel();
            SetLevel();
            StopGame();

            Physics.IgnoreCollision(AI1.GetComponent<BoxCollider>(), player.GetComponent<BoxCollider>());
            Physics.IgnoreCollision(AI2.GetComponent<BoxCollider>(), player.GetComponent<BoxCollider>());
            Physics.IgnoreCollision(AI1.GetComponent<BoxCollider>(), AI2.GetComponent<BoxCollider>());
        }
        private void SetLevel()
        {
            levelTextInGame.text = (GameObject.Find("HCLevelManager").GetComponent<HCLevelManager>().GetGlobalLevelIndex() + 1).ToString();
            levelTextInGame2.text = (GameObject.Find("HCLevelManager").GetComponent<HCLevelManager>().GetGlobalLevelIndex() + 2).ToString();
        }
        private int GetLevel()
        {
            return GameObject.Find("HCLevelManager").GetComponent<HCLevelManager>().GetGlobalLevelIndex() + 1;
        }
        public void NextLevel()
        {
            currentLevel++;
            PlayerPrefs.SetInt("Level", currentLevel);
            GameObject.Find("HCLevelManager").GetComponent<HCLevelManager>().LevelUp();
            //Application.LoadLevel(Application.loadedLevel);
            SceneManager.LoadScene("Main");
        }
        public void RestartLevel()
        {
            SceneManager.LoadScene("Main");
        }
        public void StartGame()
        {
            gameOver = false;
            player.GetComponent<Player>().StartFalling();
            AI1.GetComponent<JumperAI>().StartFalling();
            AI2.GetComponent<JumperAI>().StartFalling();
        }
        public void StopGame()
        {
            gameOver = true;
            player.GetComponent<Player>().StopFalling();
            AI1.GetComponent<JumperAI>().StopFalling();
            AI2.GetComponent<JumperAI>().StopFalling();
        }
        public void OpenLosePanel()
        {
            StopGame();
            losePanel.SetActive(true);
        }
        public void OpenWinPanel()
        {
            winPanel.SetActive(true);
        }

        public void OpenPerfectText()
        {
            perfectText.transform.localScale = perfectText.transform.localScale / 5;
            perfectText.SetActive(true);

            perfectText.transform.DOScale(perfectText.transform.localScale * 5, 0.5f)
                .OnComplete(() =>
                perfectText.transform.DOShakeRotation(0.5f)
                .OnComplete(() =>
                perfectText.SetActive(false)
                )
                ); ;
        }

        public void SetLevelBar(float lastJumpNode)
        {
            levelBar.fillAmount = ((maxTrampolineCount - lastJumpNode) / maxTrampolineCount);
        }

        public void OpenLongJumpText()
        {
            longJumpText.transform.localScale = longJumpText.transform.localScale / 5;
            longJumpText.SetActive(true);

            longJumpText.transform.DOScale(longJumpText.transform.localScale * 5, 0.5f)
                .OnComplete(() =>
                longJumpText.transform.DOShakeRotation(0.5f)
                .OnComplete(() =>
                longJumpText.SetActive(false)
                )
                ); ;
        }
    }
}
