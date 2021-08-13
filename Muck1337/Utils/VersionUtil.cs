using UnityEngine.Networking;

namespace Muck1337.Utils
{
    public static class VersionUtil
    {
        public static string GetVersion()
        {
            UnityWebRequest uwr = UnityWebRequest.Get("https://raw.githubusercontent.com/Supercolbat/Muck1337/master/_version");

            if (uwr.isNetworkError)
            {
                Muck1337Plugin.Instance.log.LogError("Error retrieving version: " + uwr.error);
                return "";
            }

            Muck1337Plugin.Instance.log.LogInfo("Received version: " + uwr.downloadHandler.text);
            return uwr.downloadHandler.text;
        }
    }
}