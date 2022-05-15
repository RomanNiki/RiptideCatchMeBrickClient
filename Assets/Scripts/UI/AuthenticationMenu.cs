using System;
using System.Text;
using Newtonsoft.Json;
using ScriptableObjects;
using SharedLibrary.Requests;
using SharedLibrary.Responses;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public abstract class AuthenticationMenu : MonoBehaviour
    {
        [SerializeField] protected TMP_InputField _loginInputField;
        [SerializeField] protected TMP_InputField _passwordInputField;
        [SerializeField] protected TMP_Text _responseText;
        [SerializeField] private Button _sendPostDataButton;
        [SerializeField] private Button _changePanelButton;
        [SerializeField] protected LoginTokenData _token;
        [SerializeField] private GameObject _hubPanel;
        [SerializeField] protected GameObject _anotherPanel;

        private void OnEnable()
        {
            _sendPostDataButton.onClick.AddListener(SendData);
            _changePanelButton.onClick.AddListener(ChangePanel);
        }

        private void OnDisable()
        {
            _sendPostDataButton.onClick.RemoveListener(SendData);
            _changePanelButton.onClick.RemoveListener(ChangePanel);
        }

        protected void LeaveToHub()
        {
            _hubPanel.SetActive(true);
            gameObject.SetActive(false);
        }

        protected abstract void SendData();

        protected async void SendPostData(Uri url)
        {
            var (success, content) = await RequestData.SendPostData(url, HandleAuthenticationData());

            if (success)
            {
                var token = JsonConvert.DeserializeObject<AuthenticationResponse>(content);
                if (token == null) return;
                _token.SetToken(token.Token);
                LeaveToHub();
            }
            else
            {
                _responseText.text = content;
            }
        }
        
        protected byte[] HandleAuthenticationData()
        {
            var loginData = new AuthenticationRequest()
            {
                Password = _passwordInputField.text,
                UserName = _loginInputField.text
            };

            var roomRequestJson = JsonConvert.SerializeObject(loginData);
            return Encoding.UTF8.GetBytes(roomRequestJson);
        }

        private void ChangePanel()
        {
            _anotherPanel.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}