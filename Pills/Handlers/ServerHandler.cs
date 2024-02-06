using FunnyPills;

namespace FunnyPills
{
    public class ServerHandler
    {
        public void OnReloadingConfigs()
        {
            Plugin.Instance.Config.LoadConfigs();
        }
    }
}