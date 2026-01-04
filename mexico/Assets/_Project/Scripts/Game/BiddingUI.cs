using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardGame.Network;
using Unity.Netcode;

namespace CardGame.Game
{
    public class BiddingUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _biddingPanel;
        [SerializeField] private Transform _buttonContainer;
        [SerializeField] private GameObject _buttonPrefab;
        [SerializeField] private TextMeshProUGUI _currentBidText;

        private void Start()
        {
            // Hide panel initially
            _biddingPanel.SetActive(false);
            
            // Subscribe to state changes from GameManager
            GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        }

        private void OnDestroy()
        {
            if (GameManager.Instance != null)
                GameManager.Instance.OnGameStateChanged -= HandleGameStateChanged;
        }

        private void HandleGameStateChanged(GameState newState)
        {
            if (newState == GameState.Bidding)
            {
                _biddingPanel.SetActive(true);
                RefreshBiddingButtons();
            }
            else
            {
                _biddingPanel.SetActive(false);
            }
        }

        public void RefreshBiddingButtons()
        {
            // Clear old buttons
            foreach (Transform child in _buttonContainer)
            {
                Destroy(child.gameObject);
            }

            // Check if we are in a networked session to avoid NullReferenceException
            bool isNetworkActive = NetworkManager.Singleton != null && 
                                  NetworkManager.Singleton.IsClient && 
                                  NetworkManager.Singleton.LocalClient != null;

            // Create buttons 5-10
            for (int i = 5; i <= 10; i++)
            {
                int bidValue = i;
                GameObject btnObj = Instantiate(_buttonPrefab, _buttonContainer);
                Button btn = btnObj.GetComponent<Button>();
                
                string label = bidValue == 10 ? "MEKSIKO" : bidValue.ToString();
                btnObj.GetComponentInChildren<TextMeshProUGUI>().text = label;

                // The listener must be inside the loop so 'btn' and 'bidValue' are in scope
                btn.onClick.AddListener(() => {
                    if (isNetworkActive)
                    {
                        var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<NetworkPlayer>();
                        localPlayer.SubmitBidServerRpc(bidValue);
                    }
                    else
                    {
                        // Local testing fallback - Using HandleIncomingBid to match BiddingManager
                        Debug.Log($"[Offline Test] Player clicked bid: {bidValue}");
                        FindFirstObjectByType<BiddingManager>().HandleIncomingBid(0, bidValue);
                    }
                });
            }

            // Add Pass Button
            GameObject passBtnObj = Instantiate(_buttonPrefab, _buttonContainer);
            passBtnObj.GetComponentInChildren<TextMeshProUGUI>().text = "PASS";
            Button passBtn = passBtnObj.GetComponent<Button>();

            passBtn.onClick.AddListener(() => {
                if (isNetworkActive)
                {
                    var localPlayer = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<NetworkPlayer>();
                    localPlayer.PassServerRpc();
                }
                else
                {
                    // Local testing fallback - Using HandlePass to match BiddingManager
                    Debug.Log("[Offline Test] Player clicked PASS");
                    FindFirstObjectByType<BiddingManager>().HandlePass(0);
                }
            });
        }

        public void UpdateCurrentBidDisplay(int amount, string bidderName)
        {
            _currentBidText.text = $"Highest Bid: {amount} by {bidderName}";
        }
    }
}