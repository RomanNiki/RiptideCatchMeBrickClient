using System.Collections.Generic;
using Multiplayer;
using PlayerNameSpace;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;

[RequireComponent(typeof(Networking))]
[RequireComponent(typeof(PrefabContainer))]
public class PlayerSpawner : MonoBehaviour
{
    private static PrefabContainer _prefabContainer;
    private static Networking _networking;
    public static readonly Dictionary<ushort, Player> Players = new Dictionary<ushort, Player>();
    private static readonly Dictionary<ushort, PlayerMover> PlayerMovers = new Dictionary<ushort, PlayerMover>();
    private static ushort _clientId;

    private void Start()
    {
        _prefabContainer = GetComponent<PrefabContainer>();
        _networking = GetComponent<Networking>();
        _clientId = _networking.Client.Id;
    }

    public static void RemovePlayer(ushort id)
    {
        Players.Remove(id);
        PlayerMovers.Remove(id);
    }

    private static void Spawn(ushort id, string username, Vector3 position)
    {
        var player = Instantiate(_prefabContainer.LocalPlayerPrefab, position, Quaternion.identity);
        var isLocal = id == _clientId;
        var correctUserName = $"Player {id} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.name = correctUserName;
        player.Init(id, username, isLocal, _networking);
        var mover = player.GetComponent<PlayerMover>();
        Players.Add(id, player);
        PlayerMovers.Add(id, mover);
    }

    [MessageHandler((ushort) ServerToClientId.PlayerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }
    
    [MessageHandler((ushort)ServerToClientId.PlayerMovement)]
    private static void PlayerMovement(Message message)
    {
        if (PlayerMovers.TryGetValue(message.GetUShort(), out var mover))
        {
            mover.Move(message.GetUShort(),message.GetVector3(), message.GetVector3());
        }
    }
}