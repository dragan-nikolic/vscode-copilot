using UnityEngine;
using System.Collections.Generic;
using Unity.Netcode; // Required for NetworkBehaviour

namespace CardGame.Game
{
    public class BiddingManager : NetworkBehaviour
    {
        [Header("Bidding State")]
        // Use NetworkVariables to sync the bid state to all clients automatically
        private NetworkVariable<int> _currentHighBid = new NetworkVariable<int>(4);
        private NetworkVariable<int> _currentPlayerBiddingIndex = new NetworkVariable<int>(0);
        private NetworkVariable<int> _declarerIndex = new NetworkVariable<int>(-1);
        
        private int _passCount = 0;
        private bool[] _playerHasPassed = new bool[3];

        // Properties to allow UI to check state
        public int CurrentPlayerBiddingIndex => _currentPlayerBiddingIndex.Value;

        public void StartBidding()
        {
            if (!IsServer) return; // Only the server should initialize game states

            _currentHighBid.Value = 4;
            _declarerIndex.Value = -1;
            _currentPlayerBiddingIndex.Value = 0;
            _passCount = 0;
            System.Array.Clear(_playerHasPassed, 0, _playerHasPassed.Length);
            
            GameManager.Instance.ChangeState(GameState.Bidding);
            Debug.Log("[Bidding] Bidding started on server. Player 0 starts.");
        }

        // Called by NetworkPlayer.SubmitBidServerRpc
        public void HandleIncomingBid(int playerIndex, int bidAmount)
        {
            if (!IsServer) return;

            if (bidAmount <= _currentHighBid.Value && bidAmount != 10) 
            {
                Debug.LogWarning("Bid must be higher than current bid.");
                return;
            }

            _currentHighBid.Value = bidAmount;
            _declarerIndex.Value = playerIndex;
            
            Debug.Log($"[Bidding] Player {playerIndex} bid {bidAmount}");

            if (bidAmount == 10) // "Meksiko" ends bidding immediately
            {
                EndBidding();
            }
            else
            {
                MoveNextBiddingPlayer();
            }
        }

        public void HandlePass(int playerIndex)
        {
            if (!IsServer) return;

            _playerHasPassed[playerIndex] = true;
            _passCount++;
            
            Debug.Log($"[Bidding] Player {playerIndex} passed.");

            if (_passCount >= 2 && _declarerIndex.Value != -1)
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
            int nextIndex = _currentPlayerBiddingIndex.Value;
            do {
                nextIndex = (nextIndex + 1) % 3;
            } while (_playerHasPassed[nextIndex]);
            
            _currentPlayerBiddingIndex.Value = nextIndex;
        }

        private void EndBidding()
        {
            Debug.Log($"[Bidding] Bidding over. Declarer: Player {_declarerIndex.Value} with bid {_currentHighBid.Value}");
            
            // Transition to Talon Phase as per PROJECT_PLAN Phase 3
            GameManager.Instance.ChangeState(GameState.TalonPhase);
        }
    }
}