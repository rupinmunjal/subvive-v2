using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

namespace SubVive.Minigame
{
    public class MinigameManager : MonoBehaviour
    {
        public static MinigameManager Instance { get; private set; }

        [Header("Game Settings")]
        [SerializeField] private float gameDuration = 15f;
        [SerializeField] private int maxLives = 2;

        [Header("UI References")]
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private TMP_Text livesText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TMP_Text gameOverTitleText;
        [SerializeField] private TMP_Text gameOverReasonText;

        public bool IsGameOver { get; private set; }
        public int Lives { get; private set; }

        private float _timeRemaining;

        void Awake()
        {
            Instance = this;
        }

        void Start()
        {
            Lives = maxLives;
            _timeRemaining = gameDuration;
            IsGameOver = false;

            if (gameOverPanel != null) gameOverPanel.SetActive(false);

            UpdateUI();
        }

        void Update()
        {
            if (IsGameOver) return;

            // Update timer
            _timeRemaining -= Time.deltaTime;
            if (_timeRemaining <= 0f)
            {
                _timeRemaining = 0f;
                WinGame();
            }

            UpdateUI();
        }

        public void TakeDamage()
        {
            if (IsGameOver) return;

            Lives--;
            UpdateUI();

            if (Lives <= 0)
            {
                LoseGame();
            }
        }

        private void UpdateUI()
        {
            if (timerText != null) timerText.text = $"Time: {Mathf.CeilToInt(_timeRemaining)}s";
            if (livesText != null) livesText.text = $"Lives: {Lives}";
        }

        private void WinGame()
        {
            IsGameOver = true;
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                if (gameOverTitleText != null) gameOverTitleText.text = "VICTORY!";
                if (gameOverReasonText != null)
                {
                    gameOverReasonText.text = "You survived the debris field!";
                }
            }
            StopGameplay();
        }

        private void LoseGame()
        {
            IsGameOver = true;
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true);
                if (gameOverTitleText != null) gameOverTitleText.text = "GAME OVER";
                if (gameOverReasonText != null)
                {
                    gameOverReasonText.text = "Your submarine was destroyed!";
                }
            }
            StopGameplay();
        }

        private void StopGameplay()
        {
            // Stop spawning
            if (RockSpawner.Instance != null)
            {
                RockSpawner.Instance.StopAllCoroutines();
            }

            // Destroy all active projectiles and rocks
            var rocks = GameObject.FindGameObjectsWithTag("Rock");
            foreach (var r in rocks) Destroy(r);

            var projs = Object.FindObjectsByType<Projectile>(FindObjectsSortMode.None);
            foreach (var p in projs) Destroy(p.gameObject);
        }

        // Restart helper
        public void RestartMinigame()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}