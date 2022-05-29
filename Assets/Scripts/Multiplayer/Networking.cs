using System;
using System.Collections;
using PlayerComponents;
using RiptideNetworking;
using RiptideNetworking.Utils;
using SharedLibrary;
using UnityEngine;
using UnityEngine.Events;
using Client = RiptideNetworking.Client;

namespace Multiplayer
{
    public class Networking : MonoBehaviour
    {
        [SerializeField] private string _ip;
        [SerializeField] private ushort _port;
        [SerializeField] private ushort _tickDivergenceTolerance = 1;
        [SerializeField] private float _secondsToConnect = 3;
        private static ushort _serverTick;
        private static ushort _ticksBetweenPositionUpdate = 2;
        private static ushort _staticTickDivergenceToTolerance;
        private bool _connected;
        public UnityAction<bool> Connected;
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
        private static ushort TicksBetweenPositionUpdate
        {
            get => _ticksBetweenPositionUpdate;
            set
            {
                _ticksBetweenPositionUpdate = value;
                InterpolationTick = (ushort) (ServerTick - value);
            }
        }
        public Client Client { get; private set; }

        private void OnValidate()
        {
            _staticTickDivergenceToTolerance = _tickDivergenceTolerance;
        }

        private void Start()
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            InitClient();
        }

        private void InitClient()
        {
            Client = new Client();

            ServerTick = TicksBetweenPositionUpdate;
            Client.Connected += OnConnected;
            Client.ConnectionFailed += OnFailedToConnect;
            Client.ClientDisconnected += OnPlayerLeft;
            Client.Disconnected += OnDisconnected;
        }

        private void Unsubscribe()
        {
            Client.Connected -= OnConnected;
            Client.ConnectionFailed -= OnFailedToConnect;
            Client.ClientDisconnected -= OnPlayerLeft;
            Client.Disconnected -= OnDisconnected;
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void FixedUpdate()
        {
            if (Client.IsConnected)
            {
                Client.Tick();
                ServerTick++;
            }
        }

        private IEnumerator TryToConnect()
        {
            yield return new WaitForSeconds(_secondsToConnect);
            if (_connected == false)
            {
                Client.Disconnect();
                Unsubscribe();
                InitClient();
                Connected?.Invoke(false);
            }
        }

        private void OnApplicationQuit()
        {
            Client.Disconnect();
        }

        public void Connect()
        {
            StartCoroutine(TryToConnect());
            Client.Connect($"{_ip}:{_port}");
        }

        public void SetClientName(string newName)
        {
            var message = Message.Create(MessageSendMode.reliable, ClientToServerId.Name);
            message.AddString(newName);
            Client.Send(message);
        }

        private void OnConnected(object sender, EventArgs e)
        {
            _connected = true;
            Connected?.Invoke(true);
        }

        private void OnFailedToConnect(object sender, EventArgs e)
        {
            Connected?.Invoke(false);
        }

        private void OnDisconnected(object sender, DisconnectedEventArgs e)
        {
            _connected = false;
            Connected?.Invoke(false);
            foreach (var player in PlayerSpawner.Players.Values)
            {
                Destroy(player.Player.gameObject);
            }
        }

        private static void OnPlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            if (PlayerSpawner.Players.TryGetValue(e.Id, out var player))
            {
                Destroy(player.Player.gameObject);
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