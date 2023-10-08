#nullable enable
using UnityEngine;
using UnityEngine.Networking;

public class WebUtils
{
    public static async Awaitable<Texture2D?> DownloadImage(string url)
    {
        using var www = UnityWebRequestTexture.GetTexture(url);

        await www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(www.error + " :: " + www.url);
            return null;
        }

        return ((DownloadHandlerTexture)www.downloadHandler).texture;
    }
}