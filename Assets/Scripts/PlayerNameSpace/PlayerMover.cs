using UnityEngine;

namespace PlayerNameSpace
{
    [RequireComponent(typeof(Interpolator))]
    [RequireComponent(typeof(Player))]
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Transform _camTransform;
        private Interpolator _interpolator;
        private Player _player;

        private void Start()
        {
            _player = GetComponent<Player>();
            _interpolator = GetComponent<Interpolator>();
        }

        public void Move(ushort tick, Vector3 newPosition, Vector3 forward)
        {
            _interpolator.NewUpdate(tick, newPosition);
            
            if (_player.IsLocal == false)
            {
                _camTransform.forward = forward;
            }
        }
    }
}