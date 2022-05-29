using PlayerComponents.Weapons;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerShooter : MonoBehaviour
    {
        [SerializeField] private Weapon _weapon;
        
        public void Shoot()
        {
            _weapon.Shoot();
        }

        public void ShowTrajectory(float horizontal, float vertical)
        {
            _weapon.ShowTrajectory(horizontal, vertical);
        }
    }
}