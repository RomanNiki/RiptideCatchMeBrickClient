using System.Collections.Generic;
using Multiplayer;
using PlayerCamera;
using RiptideNetworking;
using SharedLibrary;
using UI;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerSpawner : MonoBehaviour
    {
        [SerializeField] private Networking _networking;
        [SerializeField] private FollowCamera _camera;
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private PrefabContainer _prefabContainer;
        private static PrefabContainer _staticPrefabContainer;

        public static readonly Dictionary<ushort, PlayerComponents> Players =
            new Dictionary<ushort, PlayerComponents>();

        private static FollowCamera _cameraStatic;
        private static Networking _staticNetworking;
        private static HealthBar _staticHealthBar;

        private void Start()
        {
            _staticHealthBar = _healthBar;
            _staticNetworking = _networking;
            _staticPrefabContainer = _prefabContainer;
            _cameraStatic = _camera;
        }

        public static void RemovePlayer(ushort id)
        {
            Players.Remove(id);
        }

        private static void Spawn(ushort id, string username, Vector3 position, Team team, float health)
        {
            var isLocal = id == _staticNetworking.Client.Id;
            var player = Instantiate(GetPrefab(isLocal, team), position, Quaternion.identity);
            var correctUserName = $"Player {id} {(string.IsNullOrEmpty(username) ? "Guest" : username)}";
            player.name = correctUserName;
            player.Init(id, (string.IsNullOrEmpty(username) ? $"Guest {id}" : username), isLocal, _staticNetworking,
                team, health);
            Players.Add(id,
                new PlayerComponents(player, player.GetComponent<PlayerMover>(), player.GetComponent<PlayerShooter>()));
            if (isLocal)
            {
                _staticHealthBar.Init(player);
                _cameraStatic.SetCharacter(player);
            }
        }

        private static Player GetPrefab(bool isLocal, Team team)
        {
            if (isLocal)
            {
                return _staticPrefabContainer.LocalPlayerPrefab;
            }

            switch (team)
            {
                case Team.None:
                    break;
                case Team.Red:
                    return _staticPrefabContainer.RedPlayerPrefab;
                case Team.Green:
                    return _staticPrefabContainer.BluePlayerPrefab;
            }

            return _staticPrefabContainer.RedPlayerPrefab;
        }

        [MessageHandler((ushort) ServerToClientId.PlayerSpawned)]
        private static void SpawnPlayer(Message message)
        {
            var id = message.GetUShort();
            var name = message.GetString();
            var position = message.GetVector3();
            var team =(Team) message.GetByte();
            var health = message.GetFloat();
            Spawn(id, name, position, team, health);
        }

        [MessageHandler((ushort) ServerToClientId.PlayerMovement)]
        private static void PlayerMovement(Message message)
        {
            if (Players.TryGetValue(message.GetUShort(), out var player))
            {
                var tick = message.GetUShort();
                var position = message.GetVector3();
                var rotation = message.GetQuaternion();
                var isTeleport = message.GetBool();

                player.Mover.Move(tick, position, rotation, isTeleport);
            }
        }
        
        [MessageHandler((ushort)ServerToClientId.PlayerDied)]
        private static void PlayerDied(Message message)
        {
            if (Players.TryGetValue(message.GetUShort(), out var player))
                player.Player.OnDied(message.GetVector3());
        }

        [MessageHandler((ushort) ServerToClientId.PlayerHealthChanged)]
        private static void PlayerHealthChanged(Message message)
        {
            if (Players.TryGetValue(_staticNetworking.Client.Id, out var player))
            {
                player.Player.SetHealth(message.GetFloat());
            }
        }
        
        [MessageHandler((ushort)ServerToClientId.PlayerRespawned)]
        private static void PlayerRespawned(Message message)
        {
            if (Players.TryGetValue(message.GetUShort(), out var player))
                player.Player.OnRespawned(message.GetVector3());
        }
        
        [MessageHandler((ushort)ServerToClientId.ProjectileHitMarker)]
        private static void HitMarker(Message message)
        {
            if (Players.TryGetValue(message.GetUShort(), out var player))
                player.Player.GetDamageEffect();
        }
    }
}