using Multiplayer;
using UnityEngine;

namespace PlayerNameSpace
{
    public class Player : MonoBehaviour
    {
        public ushort Id { get; private set; }
        public string UserName { get; private set; }
        public bool IsLocal { get; private set; }
        public Networking Networking { get; private set; }

        public void Init(ushort id, string userName, bool isLocal, Networking networking)
        {
            Id = id;
            UserName = userName;
            IsLocal = isLocal;
            Networking = networking;
        }

        private void OnDestroy()
        {
            PlayerSpawner.RemovePlayer(Id);
        }
    }
}