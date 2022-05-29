using Multiplayer;
using SharedLibrary;
using UnityEngine;
using UnityEngine.Events;

namespace PlayerComponents
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private GameObject _model;
        [SerializeField] private ParticleSystem _damageParticleSystem;
        public event UnityAction<float> HealthChanged; 
        public event UnityAction<bool> Died; 
        public event UnityAction<Player> PlayerLeave; 
        public ushort Id { get; private set; }
        public string UserName { get; private set; }
        public bool IsLocal { get; private set; }
        public Networking Networking { get; private set; }
        public Team Team { get; private set; }
        public float MaxHealth { get; private set; }
        private float _health;

        public void Init(ushort id, string userName, bool isLocal, Networking networking, Team team, float health)
        {
            Id = id;
            UserName = userName;
            IsLocal = isLocal;
            Networking = networking;
            Team = team;
            MaxHealth = health;
            _health = health;
            HealthChanged?.Invoke(MaxHealth);
        }

        private void OnDestroy()
        {
            PlayerLeave?.Invoke(this);
            PlayerSpawner.RemovePlayer(Id);
        }

        public void SetHealth(float health)
        {
            _health = health;
            HealthChanged?.Invoke(health);
        }

        public void GetDamageEffect()
        {
            _damageParticleSystem.Play();
        }

        public void OnDied(Vector3 position)
        {
            transform.position = position;
            _health = 0f;
            _model.SetActive(false);
            Died?.Invoke(true);
            if (IsLocal)
                HealthChanged?.Invoke(_health);
        }
        
        public void OnRespawned(Vector3 position)
        {
            Died?.Invoke(false);
            transform.position = position;
            _health = MaxHealth;
            _model.SetActive(true);

            if (IsLocal)
                HealthChanged?.Invoke(_health);
        }
    }
}