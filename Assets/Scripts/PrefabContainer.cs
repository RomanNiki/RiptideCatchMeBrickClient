using PlayerNameSpace;
using UnityEngine;

public class PrefabContainer : MonoBehaviour
{
    [SerializeField] private Player _localPlayerPrefab;
    public Player LocalPlayerPrefab => _localPlayerPrefab; 
    
    [SerializeField] private Player _playerPrefab;
    public Player PlayerPrefab => _playerPrefab;
}