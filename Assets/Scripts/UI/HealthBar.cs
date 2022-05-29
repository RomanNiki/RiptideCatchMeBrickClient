using PlayerComponents;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;

        public void Init(Player player)
        {
            player.HealthChanged += OnHealthChanged;
            player.PlayerLeave += OnPlayerLeave;
            _slider.maxValue = player.MaxHealth;
        }

        private void OnPlayerLeave(Player player)
        {
            player.HealthChanged -= OnHealthChanged;
            player.PlayerLeave -= OnPlayerLeave;
        }

        private void OnHealthChanged(float health)
        {
            Debug.Log(health);
            _slider.value = health;
        }
    }
}