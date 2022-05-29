using PlayerComponents;
using UnityEngine;

namespace Projectiles
{
    [RequireComponent(typeof(Interpolator))]
    public class Projectile : MonoBehaviour
    {
        private Interpolator _interpolator;
        public ushort Id { get; private set; }

        private void Start()
        {
            _interpolator = GetComponent<Interpolator>();
        }

        public void Init(ushort id, Vector3 position)
        {
            Id = id;
            transform.position = position;
        }
        
        private void OnDestroy()
        {
            ProjectileSpawner.Projectiles.Remove(Id);
        }

        public void Move(ushort tick,Vector3 direction)
        {
            _interpolator.NewUpdate(tick ,direction, false);
        }
    }
}