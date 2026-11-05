
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PASGame
{
    public class GameFlowManager : MonoBehaviour
    {
        public static GameFlowManager Instance { get; private set; }

        [Header("Syarat menang")]
        public ItemDetails coinData;
        public ItemDetails keyData;
        public bool autoCountCoins = true;
        public int requiredCoins = 5;

        [Header("UI")]
        public TMP_Text objectiveText;
        public TMP_Text messageText;
        public GameObject winPanel;

        public bool HasWon { get; private set; }

        void Awake()
        {
            Instance = this;
            Time.timeScale = 1f;
        }

        void Start()
        {
            if (winPanel != null)
                winPanel.SetActive(false);

            if (messageText != null)
                messageText.text = "";

           requiredCoins = 5;

            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnInventoryChanged += UpdateObjective;

            UpdateObjective();
        }

        void OnDestroy()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnInventoryChanged -= UpdateObjective;
        }

        int Coins
        {
            get
            {
                if (InventoryManager.Instance == null || coinData == null)
                    return 0;

                return InventoryManager.Instance.GetAmount(coinData);
            }
        }

        bool HasKey
        {
            get
            {
                if (keyData == null)
                    return true;

                if (InventoryManager.Instance == null)
                    return false;

                return InventoryManager.Instance.GetAmount(keyData) > 0;
            }
        }

        void UpdateObjective()
        {
            if (objectiveText == null)
                return;

            string text = "";

            if (coinData != null)
                text += "Koin " + Coins + "/" + requiredCoins;

            if (keyData != null)
                text += "   Kunci: " + (HasKey ? "sudah" : "belum");

            objectiveText.text = text;
        }

        public void TryExit()
        {
            if (HasWon)
                return;

            int missing = coinData != null ? requiredCoins - Coins : 0;

            if (missing > 0)
                ShowMessage("Kumpulkan " + missing + " koin lagi!");
            else if (!HasKey)
                ShowMessage("Pintu terkunci! Cari kuncinya dulu.");
            else
                Win();
        }

        void Win()
        {
            HasWon = true;

            if (winPanel != null)
                winPanel.SetActive(true);

            Time.timeScale = 0f;
        }

        public void Restart()
        {
            Time.timeScale = 1f;

            int index = SceneManager.GetActiveScene().buildIndex;

            if (index < 0)
            {
                Debug.LogWarning("Scene belum ada di Build Profiles.");
                return;
            }

            SceneManager.LoadScene(index);
        }

        void ShowMessage(string text)
        {
            if (messageText == null)
                return;

            StopAllCoroutines();
            StartCoroutine(MessageRoutine(text));
        }

        IEnumerator MessageRoutine(string text)
        {
            messageText.text = text;

            yield return new WaitForSecondsRealtime(2f);

            messageText.text = "";
        }
    }
}
