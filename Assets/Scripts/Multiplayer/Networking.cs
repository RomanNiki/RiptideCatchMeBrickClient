using System;
using RiptideNetworking;
using RiptideNetworking.Utils;
using SharedLibrary;
using UnityEngine;
using UnityEngine.Events;

namespace Multiplayer
{
    public class Networking : MonoBehaviour
    {
        [SerializeField] private string _ip;
        [SerializeField] private ushort _port;
        [SerializeField] private ushort _tickDivergenceTolerance = 1;
        private string _clientName;
        private static ushort _serverTick;
        private static ushort _ticksBetweenPositionUpdate = 2;
        private static ushort _staticTickDivergenceToTolerance;
        public static ushort InterpolationTick { get; private set; }

        public static ushort ServerTick
        {
            get => _serverTick;
            private set
            {
                _serverTick = value;
                InterpolationTick = (ushort) (value - TicksBetweenPositionUpdate);
            }
        }

        public static ushort TicksBetweenPositionUpdate
        {
            get => _ticksBetweenPositionUpdate;
            private set
            {
                _ticksBetweenPositionUpdate = value;
                InterpolationTick = (ushort) (ServerTick - value);
            }
        }

        public Client Client { get; } = new Client();
        public UnityAction<bool> Connected;

        private void OnValidate()
        {
            _staticTickDivergenceToTolerance = _tickDivergenceTolerance;
        }

        private void Start()
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            Client.Connected += OnConnected;
            Client.ConnectionFailed += OnFailedToConnect;
            Client.Disconnected += OnDisconnected;
            Client.ClientDisconnected += OnPlayerLeft;

            ServerTick = TicksBetweenPositionUpdate;
        }

        private void OnDisable()
        {
            Client.Connected -= OnConnected;
            Client.ConnectionFailed -= OnFailedToConnect;
            Client.Disconnected -= OnDisconnected;
            Client.ClientDisconnected -= OnPlayerLeft;
        }

        private void FixedUpdate()
        {
            if (Client.IsConnected)
            {
                Client.Tick();
                ServerTick++;
            }
        }

        private void OnApplicationQuit()
        {
            Client.Disconnect();
        }

        public void Connect()
        {
            Client.Connect($"{_ip}:{_port}");
        }

        public void SetClientName(string newName)
        {
            _clientName = newName;

            var message = Message.Create(MessageSendMode.reliable, ClientToServerId.Name);
            message.AddString(_clientName);
            Client.Send(message);
        }

        private void OnConnected(object sender, EventArgs e)
        {
            Connected?.Invoke(true);
        }

        private void OnFailedToConnect(object sender, EventArgs e)
        {
            Connected?.Invoke(false);
        }

        private void OnDisconnected(object sender, EventArgs e)
        {
            Connected?.Invoke(false);
            foreach (var player in PlayerSpawner.Players.Values)
            {
                Destroy(player.gameObject);
            }
        }

        private static void OnPlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            if (PlayerSpawner.Players.TryGetValue(e.Id, out var player))
            {
                Destroy(player.gameObject);
            }
        }

        private static void SetTick(ushort serverTick)
        {
            if (Mathf.Abs(ServerTick - serverTick) > _staticTickDivergenceToTolerance)
            {
                Debug.Log($"Client tick: {ServerTick} -> {serverTick}");
                ServerTick = serverTick;
            }
        }

        [MessageHandler((ushort) ServerToClientId.Sync)]
        public static void Sync(Message message)
        {
            SetTick(message.GetUShort());
        }
    }
}