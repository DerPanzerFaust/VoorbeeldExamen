using LocalMultiplayer.Player;
using StateMachines.GlobalStateMachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace LocalMultiplayer.Lobby
{
    public class LobbyJoinManager : SingletonBehaviour<LobbyJoinManager>
    {
        //--------------------Private--------------------//
        [Header("UpperLeft")]
        [SerializeField]
        private GameObject _upperLeft;
        [SerializeField]
        private Material _upperLeftMaterial;
        [SerializeField]
        private Transform _spawnPositionOne;
        
        [Header("UpperRight")]
        [SerializeField]
        private GameObject _upperRight;
        [SerializeField] 
        private Material _upperRightMaterial;
        [SerializeField]
        private Transform _spawnPositionTwo;

        [Header("LowerLeft")]
        [SerializeField]
        private GameObject _lowerLeft;
        [SerializeField]
        private Material _lowerLeftMaterial;
        [SerializeField]
        private Transform _spawnPositionThree;

        [Header("LowerRight")]
        [SerializeField] 
        private GameObject _lowerRight;
        [SerializeField]
        private Material _lowerRightMaterial;
        [SerializeField]
        private Transform _spawnPositionFour;

        private StateMachine _stateMachine;

        [SerializeField]
        private List<LobbySpot> _lobbySpots = new();

        private PlayerSpawner _playerSpawner;

        private bool _lobbyHasStarted;

        private Material _lowestMaterial;

        private Transform _playerSpawnPosition;

        //--------------------Functions--------------------//
        private void Start()
        {
            _playerSpawner = PlayerSpawner.Instance;
            _stateMachine = StateMachine.Instance;
        }

        /// <summary>
        /// A function to join the localmultiplayer lobby with your playermaster
        /// </summary>
        /// <param name="master">The master to join into the lobby</param>
        public void JoinLobby(PlayerMaster master)
        {
            if (!CheckSpotFree())
                return;

            JoinLowestSpot(master);
        }

        private bool CheckSpotFree()
        {
            bool _spotIsFree;

            if(_lobbySpots.Count < 4) 
                _spotIsFree = true;
            else
                _spotIsFree = false;

            return _spotIsFree;
        }

        private void JoinLowestSpot(PlayerMaster playerMaster)
        {
            int lowestAvailableIndex = 5;

            if(_lobbySpots.Count == 0)
            {
                lowestAvailableIndex = 1;
            }
            else
            {
                int[] possibleIndexes = new int[4] {1, 2, 3, 4};

                foreach (LobbySpot spot in _lobbySpots)
                {
                    for (int i = 0; i < possibleIndexes.Length; i++)
                    {
                        if (possibleIndexes[i] == spot.SpotIndex)
                            possibleIndexes[i] = 0;
                    }
                }

                foreach (int i in possibleIndexes)
                {
                    if (i == 0)
                        continue;
                    else
                        lowestAvailableIndex = i;
                    break;
                }
            }
            
            if(lowestAvailableIndex == 1)
                playerMaster.PlayerInputComponent.OnInteractInputAction.performed += StartGame;
            
            switch (lowestAvailableIndex)
            {
                case 1:
                    _lowestMaterial = _upperLeftMaterial;
                    _playerSpawnPosition = _spawnPositionOne;
                    _upperLeft.SetActive(true);
                    break;
                case 2:
                    _lowestMaterial = _upperRightMaterial;
                    _playerSpawnPosition = _spawnPositionTwo;

                    _upperRight.SetActive(true);
                    break;
                case 3:
                    _lowestMaterial = _lowerLeftMaterial;
                    _playerSpawnPosition = _spawnPositionThree;

                    _lowerLeft.SetActive(true);
                    break;
                case 4:
                    _lowestMaterial = _lowerRightMaterial;
                    _playerSpawnPosition = _spawnPositionFour;
                    _lowerRight.SetActive(true);
                    break;
            }

            playerMaster.PlayerMaterial = _lowestMaterial;
            LobbySpot _playerLobbySpot = new LobbySpot(playerMaster, _lowestMaterial, lowestAvailableIndex, _playerSpawnPosition);

            playerMaster.CurrentLobbySpot = _playerLobbySpot;
            _lobbySpots.Add(_playerLobbySpot);
        }

        /// <summary>
        /// A function to leave the lobby with the given master
        /// </summary>
        /// <param name="master">The master to leave the lobby with</param>
        public void LeaveLobby(PlayerMaster master)
        {
            LobbySpot masterSpot = new LobbySpot();
            int masterSpotIndex = 0;

            foreach (LobbySpot spot in _lobbySpots)
            {
                if (spot.CurrentPlayerMaster == master)
                {
                    masterSpot = spot;
                    masterSpotIndex = masterSpot.SpotIndex;
                }
            }

            master.HasJoinedLobby = true;

            if (masterSpotIndex == 1)
                master.PlayerInputComponent.OnInteractInputAction.performed -= StartGame;

            switch (masterSpotIndex)
            {
                case 1:
                    _upperLeft.SetActive(false);
                    break;
                case 2:
                    _upperRight.SetActive(false);
                    break;
                case 3:
                    _lowerLeft.SetActive(false);
                    break;
                case 4:
                    _lowerRight.SetActive(false);
                    break;
            }

            _lobbySpots.Remove(masterSpot);
        }

        private void StartGame(InputAction.CallbackContext context)
        {
            if (_lobbyHasStarted)
                return;

            _playerSpawner.SpawnPlayers(_lobbySpots);

            _stateMachine.SetState(_stateMachine.GameStateInstance);
            _lobbyHasStarted = true;
        }
    }

    [Serializable]
    public struct LobbySpot
    {
        //--------------------Private--------------------//
        private PlayerMaster _currentPlayerMaster;
        [SerializeField]
        private Material _spotMaterial;
        [SerializeField]
        private int _spotIndex;
        [SerializeField]
        private Transform _spawnPosition;

        //--------------------Public--------------------//
        public PlayerMaster CurrentPlayerMaster => _currentPlayerMaster;
        
        public Material SpotMaterial => _spotMaterial;
        
        public int SpotIndex => _spotIndex;

        public Transform SpawnPosition => _spawnPosition;

        //--------------------Functions--------------------//
        public LobbySpot(PlayerMaster playerMaster, Material spotMaterial, int spotIndex, Transform spawnPosition)
        {
            _currentPlayerMaster = playerMaster;
            _spotMaterial = spotMaterial;
            _spotIndex = spotIndex;
            _spawnPosition = spawnPosition;
        }
    }
}