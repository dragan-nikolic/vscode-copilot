using UnityEngine;
using System.Collections.Generic;

namespace CardGame.Game
{
    public class BiddingManager : MonoBehaviour
    {
        [Header("Bidding State")]
        [SerializeField] private int _currentHighBid = 4; // Start below the minimum bid of 5
        [SerializeField] private int _declarerIndex = -1;
        [SerializeField] private int _currentPlayerBidding = 0;
        
        private int _passCount = 0;
        private bool[] _playerHasPassed = new bool[3];

        public void StartBidding()
        {
            _currentHighBid = 4;
            _declarerIndex = -1;
            _currentPlayerBidding = 0;
            _passCount = 0;
            System.Array.Clear(_playerHasPassed, 0, _playerHasPassed.Length);
            
            GameManager.Instance.ChangeState(GameState.Bidding);
            Debug.Log("[Bidding] Bidding started. Player 0 starts.");
        }

        // Called by UI Buttons
        public void PlaceBid(int playerIndex, int bidAmount)
        {
            if (bidAmount <= _currentHighBid && bidAmount != 10) // 10 is "Meksiko"
            {
                Debug.LogWarning("Bid must be higher than current bid.");
                return;
            }

            _currentHighBid = bidAmount;
            _declarerIndex = playerIndex;
            
            Debug.Log($"[Bidding] Player {playerIndex} bid {bidAmount}");
            MoveNextBiddingPlayer();
        }

        public void Pass(int playerIndex)
        {
            _playerHasPassed[playerIndex] = true;
            _passCount++;
            
            Debug.Log($"[Bidding] Player {playerIndex} passed.");

            if (_passCount >= 2 && _declarerIndex != -1)
            {
                EndBidding();
            }
            else
            {
                MoveNextBiddingPlayer();
            }
        }

        private void MoveNextBiddingPlayer()
        {
            do {
                _currentPlayerBidding = (_currentPlayerBidding + 1) % 3;
            } while (_playerHasPassed[_currentPlayerBidding]);
            
            // Notify UI to update buttons for the next player
        }

        private void EndBidding()
        {
            Debug.Log($"[Bidding] Bidding over. Declarer: Player {_declarerIndex} with bid {_currentHighBid}");
            
            // Move to Talon Phase as per PROJECT_PLAN Phase 3
            GameManager.Instance.ChangeState(GameState.TalonPhase);
        }
    }
}