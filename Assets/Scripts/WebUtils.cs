#nullable enable
using UnityEngine;
using UnityEngine.Networking;

public class WebUtils
{
    public static async Awaitable<Texture2D?> DownloadImage(string url, int tryCount = 3)
    {
        for (int i = 0; i < tryCount; i++)
        {
            using var www = UnityWebRequestTexture.GetTexture(url);

            await www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
                return ((DownloadHandlerTexture)www.downloadHandler).texture;
            Debug.LogError(www.error + " :: " + www.url);
        }
        
        return null;
    }
}