using System.Threading.Tasks;
#if !UNITY_WEBGL
using UnityEngine;
#endif
namespace Virtuademy.SDK.Core.Utilities
{
    public static class NetworkUtilities
    {

        private static int PING_MAX_SECONDS_DELAY = 2;

        public static async Task<bool> CheckUserInternetConnection()
        {
#if !UNITY_WEBGL
            var googlePing = new Ping("8.8.8.8");

            var cloudFlarePing = new Ping("1.1.1.1"); // CloudFlare DNS

            float timePassed = 0;
            while (!googlePing.isDone && !cloudFlarePing.isDone && timePassed < PING_MAX_SECONDS_DELAY)
            {
                await Task.Yield();
                timePassed = timePassed + Time.deltaTime;
            }
            return googlePing.isDone || cloudFlarePing.isDone;
#else
            return true;
#endif
        }

    }
}
