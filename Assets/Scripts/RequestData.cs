using System;
using System.Threading.Tasks;
using Certificates;
using UnityEngine.Networking;

public static class RequestData
{
    public static readonly Uri AuthenticationUrl = new Uri("https://localhost:7018/authentication/");
    public static readonly Uri LoginUrl = new Uri("https://localhost:7018/authentication/login");
    public static readonly Uri RegistrationUrl = new Uri("https://localhost:7018/authentication/register");
    public const string Bearer = nameof(Bearer);
    public const string Authorization = nameof(Authorization);

    public static async Task<(bool success, string content)> SendPostData(Uri url, byte[] data)
    {
        var request =new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
        request.uploadHandler = new UploadHandlerRaw(data);
        request.uploadHandler.contentType = "application/json";
        request.certificateHandler = new CertificateWhore();
        request.downloadHandler = new DownloadHandlerBuffer();
        var send = request.SendWebRequest();

        while (send.isDone == false)
        {
            await Task.Yield();
        }
        
        if (request.result == UnityWebRequest.Result.Success)
        {
            return (true, request.downloadHandler.text);
        }

        return (false, request.error);
    }
}