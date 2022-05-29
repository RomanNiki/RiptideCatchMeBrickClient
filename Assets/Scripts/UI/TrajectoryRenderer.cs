using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(LineRenderer))]
    public class TrajectoryRenderer : MonoBehaviour
    {
        [SerializeField] private int _pointCount = 3;
        [SerializeField] private float _heightModifier = 1.5f;
        private LineRenderer _lineRendererComponent;
        private Vector3[] _bulletPoints;
        public Vector3[] BulletPoints => _bulletPoints;

        private void Start()
        {
            _bulletPoints = new Vector3[_pointCount];
            _lineRendererComponent = GetComponent<LineRenderer>();
            _lineRendererComponent.positionCount = _bulletPoints.Length + 1;
        }

        public Vector3[] ShowTrajectory(Vector3 origin, float horizontal, float vertical)
        {
            origin.y = Mathf.Cos(Mathf.PI / 2) + origin.y;
            _lineRendererComponent.SetPosition(0, origin);
            for (var i = 1; i < _bulletPoints.Length + 1; i++)
            {
                var previousPosition = _lineRendererComponent.GetPosition(i - 1);
                previousPosition.x += horizontal;
                previousPosition.y = Mathf.Cos(Mathf.PI / 2 * (i * 0.1f)) * (i * _heightModifier) + origin.y;
                previousPosition.z += vertical;
                _lineRendererComponent.SetPosition(i, previousPosition);
                _bulletPoints[i - 1] = _lineRendererComponent.GetPosition(i);
            }

            return _bulletPoints;
        }
    }
}