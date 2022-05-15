using System.IO;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "Login/Login Data", fileName = "LoginData")]
    public class LoginTokenData : ScriptableObject
    {
        [SerializeField] private string _bearerToken;
        public string BearerToken => _bearerToken;

        private string FilePath => Application.persistentDataPath + $"{GetInstanceID()}.so";

        private void SaveState()
        {
            var json = JsonUtility.ToJson(this);
            File.WriteAllText(FilePath, json);
        }

        public void LoadState()
        {
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                JsonUtility.FromJsonOverwrite(json, this);
            }
        }

        public void SetToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            _bearerToken = RequestData.Bearer + " " + token;
            SaveState();
        }
    }
}