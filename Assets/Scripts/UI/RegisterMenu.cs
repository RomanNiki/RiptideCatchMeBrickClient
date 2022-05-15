namespace UI
{
    public class RegisterMenu : AuthenticationMenu
    {
        protected override void SendData()
        {
            SendPostData(RequestData.RegistrationUrl);
        }
    }
}