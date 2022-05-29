using UI;
using UnityEngine;

namespace PlayerComponents.Weapons
{
    public abstract class Weapon : MonoBehaviour
    {
        [SerializeField] protected AudioSource _source;
        [SerializeField] protected AudioClip _throwAudio;
        protected TrajectoryRenderer _trajectoryRenderer;

        private void Start()
        {
            _trajectoryRenderer = FindObjectOfType<TrajectoryRenderer>();
        }
        
        public abstract void Shoot();
        public abstract void ShowTrajectory(float horizontal, float vertical);
    }
}