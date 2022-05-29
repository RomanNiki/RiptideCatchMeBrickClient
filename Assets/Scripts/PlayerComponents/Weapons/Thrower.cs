namespace PlayerComponents.Weapons
{
    public class Thrower : Weapon
    {
        public override void Shoot()
        {
            _source.PlayOneShot(_throwAudio);
        }

        public override void ShowTrajectory(float horizontal, float vertical)
        {
            _trajectoryRenderer.ShowTrajectory(transform.position, horizontal, vertical);
        }
    }
}