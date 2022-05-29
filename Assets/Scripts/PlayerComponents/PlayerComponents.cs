namespace PlayerComponents
{
    public class PlayerComponents
    {
        public PlayerComponents(Player player, PlayerMover mover, PlayerShooter shooter)
        {
            Player = player;
            Mover = mover;
            Shooter = shooter;
        }

        public Player Player { get; private set; }
        public PlayerMover Mover { get; private set; }
        public PlayerShooter Shooter { get; private set; }
    }
}