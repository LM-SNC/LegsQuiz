#nullable enable
using UnityEngine;
using UnityEngine.Networking;

public class WebUtils
{
    public static async Awaitable<byte[]?> DownloadImage(string url, int tryCount = 3)
    {
        for (int i = 0; i < tryCount; i++)
        {
            using var www = UnityWebRequestTexture.GetTexture(url);

            www.timeout = 3;
            await www.SendWebRequest();
            
            if (www.result == UnityWebRequest.Result.Success)
            {
                return www.downloadHandler.data;
            }
            Debug.LogError(www.error + " :: " + www.url);
        }
    
        return null;
    }
}