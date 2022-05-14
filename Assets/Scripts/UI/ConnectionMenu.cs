using Multiplayer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ConnectionMenu : MonoBehaviour
    {
      [Header("Connect")] [SerializeField]
      private GameObject _connectionMenu;

      [SerializeField] private TMP_InputField _inputField;
      [SerializeField] private Button _button;
      private Networking _networking;

      private void Awake()
      {
         _networking = FindObjectOfType<Networking>();
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
          _inputField.interactable = false;
          _connectionMenu.SetActive(false);
          
          _networking.Connect();
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
          _inputField.interactable = true;
          _connectionMenu.SetActive(true);
      }
    }
}
