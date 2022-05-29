using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(Image))]
    public class ConnectionMenu : MonoBehaviour
    {
      [Header("Connect")] 
      [SerializeField] private GameObject _connectionMenu;
      [SerializeField] private GameObject _gameUI;
      [SerializeField] private TMP_InputField _inputField;
      [SerializeField] private Button _button;
      [SerializeField] private Networking _networking;
      private Image _backGround;

      private void Start()
      {
          _backGround = GetComponent<Image>();
      }

      private void OnEnable()
      {
          _button.onClick.AddListener(OnConnectionButtonClick);
          _networking.Connected += OnConnect;
      }

      private void OnDisable()
      {
          _button.onClick.RemoveListener(OnConnectionButtonClick);
          _networking.Connected -= OnConnect;
      }

      private void OnConnectionButtonClick()
      {
          _gameUI.SetActive(true);
          _networking.Connect();
          _connectionMenu.SetActive(false);
          var color = _backGround.color;
          color.a = 0f;
          _backGround.color = color;
      }

      private void OnConnect(bool success)
      {
          if (success)
          {
              _networking.SetClientName(_inputField.text);
          }
          else
          {
              BackToMain();
          }
      }

      private void BackToMain()
      {
          _gameUI.SetActive(false);
          _connectionMenu.SetActive(true);
          var color = _backGround.color;
          color.a = 1f;
          _backGround.color = color;
      }
    }
}
