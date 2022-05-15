using Multiplayer;
using RiptideNetworking;
using SharedLibrary;
using UnityEngine;

namespace PlayerNameSpace
{
    [RequireComponent(typeof(Player))]
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        private Player _player;
        private bool[] _inputs;
        private Vector2 _moveInput;

        private void Start()
        {
            _player = GetComponent<Player>();
            _inputs = new bool[2];
        }

        private void Update()
        {
            HandleInput();
        }

        private void FixedUpdate()
        {
            SendInput();

            for (var i = 0; i < _inputs.Length; i++)
            {
                _inputs[i] = false;
            }
        }

        private void HandleInput()
        {
            _moveInput.y = Input.GetAxis("Vertical");
            _moveInput.x = Input.GetAxis("Horizontal");

            if (Input.GetAxis("Jump") > 0)
            {
                _inputs[0] = true;
            }

            if (Input.GetAxis("Run") > 0)
            {
                _inputs[1] = true;
            }
        }

        #region Messages

        private void SendInput()
        {
            var message = Message.Create(MessageSendMode.unreliable, ClientToServerId.Input);
            message.AddVector2(_moveInput);
            message.AddBools(_inputs, false);
            message.AddVector3(_cameraTransform.forward);
            _player.Networking.Client.Send(message);
        }
    
        #endregion
    }
}