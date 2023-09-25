using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Network : MonoBehaviour
{
    [SerializeField] private CustomDropDown _customDropDown;

    // Start is called before the first frame update
    void Start()
    {
        // var options = new List<string>();
        //
        //
        //
        // _customDropDown.AddOptions(options);
        StartCoroutine(GetGames());
    }

    IEnumerator GetGames()
    {
        using var www = UnityWebRequest.Get(
            "https://legsquiz.hentach.ru/games");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            var json = JsonUtility.FromJson<Players>(www.downloadHandler.text);


            var options = new List<string>();

            foreach (var value in json.value)
            {
                options.Add(value.name);
            }
            
            _customDropDown.AddOptions(options);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}