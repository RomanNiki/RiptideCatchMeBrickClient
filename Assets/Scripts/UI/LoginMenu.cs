using System.Threading.Tasks;
using Certificates;
using UnityEngine;
using UnityEngine.Networking;

namespace UI
{
    public class LoginMenu : AuthenticationMenu
    {
        private async void Start()
        {
            await CheckCurrentToken();
        }

        private async Task CheckCurrentToken()
        {
            _token.LoadState();
            var request = UnityWebRequest.Get(RequestData.AuthenticationUrl);
            request.SetRequestHeader(RequestData.Authorization, _token.BearerToken);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.certificateHandler = new CertificateWhore();
            var send = request.SendWebRequest();
            while (send.isDone == false)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                LeaveToHub();
            }
            else
            {
                Debug.Log(request.error);
            }
        }

        protected override void SendData()
        {
            SendPostData(RequestData.LoginUrl);
        }
    }
}