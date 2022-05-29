using System.Collections.Generic;
using Multiplayer;
using RiptideNetworking;
using SharedLibrary;
using UI;
using UnityEngine;

namespace PlayerComponents
{
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerShooter))]
    public class PlayerControl : MonoBehaviour
    {
        [SerializeField] private float _angleModifier = 20f;
        private TrajectoryRenderer _trajectoryRenderer;
        private VariableJoystick _trajectoryJoystick;
        private bool[] _inputs;
        private Vector2 _moveInput;
        private Player _player;
        private PlayerShooter _playerShooter;
        public bool JoystickPressed { get; private set; }
        public Vector2 TrajectoryJoystickInput { get; private set; }
        private bool _died;
        private VariableJoystick _movementJoystick;

        private void OnDied(bool value)
        {
            _died = value;
        }
        
        private void Start()
        {
            _player = GetComponent<Player>();
            _playerShooter = GetComponent<PlayerShooter>();
            _trajectoryJoystick = FindObjectOfType<TrajectoryJoystick>().GetComponent<VariableJoystick>();
            _movementJoystick = FindObjectOfType<MovementJoystick>().GetComponent<VariableJoystick>();
            _trajectoryRenderer = FindObjectOfType<TrajectoryRenderer>();
            _trajectoryJoystick.PointerDown += OnPointerDown;
            _trajectoryJoystick.PointerUp += OnPointerUp;
            _player.Died += OnDied;
            _inputs = new bool[2];
        }

        private void OnDisable()
        {
            _player.Died -= OnDied;
            _trajectoryJoystick.PointerDown -= OnPointerDown;
            _trajectoryJoystick.PointerUp -= OnPointerUp;
        }

        private void OnPointerDown()
        {
            JoystickPressed = true;
            _trajectoryRenderer.gameObject.SetActive(true);
        }
        
        private void OnPointerUp()
        {
            if (Mathf.Abs(_trajectoryJoystick.Horizontal) > 0.2f || Mathf.Abs(_trajectoryJoystick.Vertical) > 0.2f)
            {
                var angle = Vector3.Angle(_trajectoryRenderer.BulletPoints[_trajectoryRenderer.BulletPoints.Length / 2] - transform.position, transform.forward);
                angle += _angleModifier;
                SendPrimaryUse(_trajectoryRenderer.BulletPoints, angle);
            }
            _trajectoryRenderer.gameObject.SetActive(false);
            JoystickPressed = false;
        }

        private void Update()
        {
            HandleInput();
            if (JoystickPressed)
            {
                var horizontal = TrajectoryJoystickInput.x;
                var vertical = TrajectoryJoystickInput.y;
                _playerShooter.ShowTrajectory(horizontal, vertical);
            }
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
            _moveInput = Vector2.zero;
            if (Mathf.Abs(_movementJoystick.Horizontal) > 0.01f && Mathf.Abs(_movementJoystick.Vertical) > 0.01f)
            {
                _moveInput.y = _movementJoystick.Vertical;
                _moveInput.x = _movementJoystick.Horizontal;
            }
            
            
            if (Input.GetAxis("Jump") > 0)
            {
                _inputs[0] = true;
            }

            if (Input.GetAxis("Run") > 0)
            {
                _inputs[1] = true;
            }

            if (_died == false)
            {
                TrajectoryJoystickInput = new Vector2(_trajectoryJoystick.Horizontal, _trajectoryJoystick.Vertical);
            }
        }

        #region Messages

        private void SendInput()
        {
            var message = Message.Create(MessageSendMode.unreliable, ClientToServerId.Input);
            message.AddVector2(_moveInput);
            message.AddBools(_inputs, false);
            message.AddVector2(TrajectoryJoystickInput);
            _player.Networking.Client.Send(message);
        }

        private  void SendPrimaryUse(IReadOnlyList<Vector3> points, float angle)
        {
            var message = Message.Create(MessageSendMode.reliable, ClientToServerId.PrimaryUse);
            message.AddVector3(points[points.Count -1]);
            message.AddFloat(angle);
            _player.Networking.Client.Send(message);
        }

        #endregion
    }
}