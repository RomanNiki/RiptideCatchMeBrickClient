using PlayerComponents;
using UnityEngine;

namespace PlayerCamera
{
    public class FollowCamera : MonoBehaviour
    {
        private Transform _mainCharacter;

        [SerializeField] private float _returnSpeed;

        [SerializeField] private float _height;
        [SerializeField] private float _rearDistance;

        private Vector3 _currentVector;

        private void Update()
        {
            CameraMove();
        }

        private void CameraMove()
        {
            if (_mainCharacter == null) return;
            var _position = _mainCharacter.transform.position;
            _currentVector = new Vector3(_position.x, _position.y + _height, _position.z - _rearDistance);
            transform.position = Vector3.Lerp(transform.position, _currentVector, _returnSpeed * Time.deltaTime);
        }

        public void SetCharacter(Player player)
        {
            _mainCharacter = player.transform;
            var position = _mainCharacter.transform.position;
            transform.position = new Vector3(position.x, position.y + _height, position.z - _rearDistance);
            transform.rotation = Quaternion.LookRotation(position - transform.position);
        }
    }
}