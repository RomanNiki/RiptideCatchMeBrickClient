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
        private string _clientName;
        public Client Client { get; set; } = new Client();
        public UnityAction<bool> Connected;

        private void Start()
        {
            RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
            
            Client.Connected += OnConnected;
            Client.ConnectionFailed += OnFailedToConnect;
            Client.Disconnected += OnDisconnected;
            Client.ClientDisconnected += OnPlayerLeft;
        }

        private void OnDisable()
        {
            Client.Connected -= OnConnected;
            Client.ConnectionFailed -= OnFailedToConnect;
            Client.Disconnected -= OnDisconnected;
        }

        private void FixedUpdate()
        {
            if (Client.IsConnected)
            {
                Client.Tick();
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
            if (newName != string.Empty)
            {
                _clientName = newName;
            }
            else
            {
                _clientName = string.Empty;
            }
            
            var message = Message.Create(MessageSendMode.reliable, (ushort) ClientToServerId.Name);
            message.AddString(_clientName);
            Client.Send(message);
        }

        private void OnConnected(object sender, EventArgs e)
        {
            Connected.Invoke(true);
        }

        private void OnFailedToConnect(object sender, EventArgs e)
        {
            Connected.Invoke(false);
        }
        
        private void OnDisconnected(object sender, EventArgs e)
        {
            Connected.Invoke(false);
        }
        
        private static void OnPlayerLeft(object sender, ClientDisconnectedEventArgs e)
        {
            Destroy(PlayerSpawner.Players[e.Id].gameObject);
        }
    }
}
