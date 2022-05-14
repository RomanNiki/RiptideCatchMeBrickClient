using System.Collections.Generic;
using Multiplayer;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;

[RequireComponent(typeof(Networking))]
[RequireComponent(typeof(PrefabContainer))]
public class PlayerSpawner : MonoBehaviour
{
    private static PrefabContainer _prefabContainer;
    private Networking _networking;
    public static readonly Dictionary<ushort, Player> Players = new Dictionary<ushort, Player>();
    private static ushort _clientId;

    private void Start()
    {
        _prefabContainer = GetComponent<PrefabContainer>();
        _networking = GetComponent<Networking>();
        _clientId = _networking.Client.Id;
    }

    public static void Spawn(ushort id, string username, Vector3 postion)
    {
        Player player;
        player = Instantiate(_prefabContainer.LocalPlayerPrefab, postion, Quaternion.identity);
        var isLocal = id == _clientId;
        var correctUserName = $"Player {id} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
        player.name = correctUserName;
        player.Init(id, username, isLocal);
    }

    [MessageHandler((ushort) ServerToClientId.PlayerSpawned)]
    private static void SpawnPlayer(Message message)
    {
        Spawn(message.GetUShort(), message.GetString(), message.GetVector3());
    }
}