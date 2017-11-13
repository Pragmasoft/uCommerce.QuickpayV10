namespace Pragmasoft.QuickpayV10.Extensions.Services.AppCenter
{
    public interface IAppEnvironmentAnalyser
    {
        Cms Cms();
        string UCommerceVersion();
        string AppVersion();
    }
}