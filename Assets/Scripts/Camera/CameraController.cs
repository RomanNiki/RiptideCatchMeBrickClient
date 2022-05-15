using UnityEngine;

namespace Camera
{
    [RequireComponent(typeof(PlayerNameSpace.Player))]
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _sensitivity = 100f;
        [SerializeField] private float _clampAngle = 85f;

        private PlayerNameSpace.Player _player;
        private float _verticalRotation;
        private float _horizontalRotation;

        private void Start()
        {
            _player = GetComponent<PlayerNameSpace.Player>();
            _verticalRotation = _cameraTransform.localEulerAngles.x;
            _horizontalRotation = _player.transform.eulerAngles.y;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursorMode();
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Look();
            }

            Debug.DrawRay(_cameraTransform.position, _cameraTransform.forward * 2f, Color.green);
        }

        private static void ToggleCursorMode()
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.lockState == CursorLockMode.None ? CursorLockMode.Locked : CursorLockMode.None;
        }

        private void Look()
        {
            var mouseVertical = -Input.GetAxis("Mouse Y");
            var mouseHorizontal = Input.GetAxis("Mouse X");

            _verticalRotation += mouseVertical * _sensitivity * Time.deltaTime;
            _horizontalRotation += mouseHorizontal * _sensitivity * Time.deltaTime;

            _verticalRotation = Mathf.Clamp(_verticalRotation, -_clampAngle, _clampAngle);

            _cameraTransform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
            _player.transform.rotation = Quaternion.Euler(0f, _horizontalRotation, 0f);
        }
    }
}