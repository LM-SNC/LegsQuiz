using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;

public class Network : MonoBehaviour
{
    [SerializeField] private CustomDropDown _customDropDown;
    [SerializeField] private GameObject _table;

    void Start()
    {
        StartCoroutine(GetGames());
        StartCoroutine(GetPlayers());
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
            var json = JsonUtility.FromJson<Games>(www.downloadHandler.text);


            var options = new List<string>();

            foreach (var value in json.value)
            {
                options.Add(value.name);
            }

            _customDropDown.AddOptions(options);
        }
    }

    IEnumerator GetPlayers()
    {
        using var www = UnityWebRequest.Get(
            "https://legsquiz.hentach.ru/players");
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log(www.downloadHandler.text);
            var json = JsonUtility.FromJson<Players>(www.downloadHandler.text);

            var template = _table.gameObject.transform.Find("Template");
            foreach (var player in json.value)
            {
                var newItem = Instantiate(template, _table.transform, true);
                newItem.GetChild(0).GetComponent<TMP_Text>().SetText(player.name);
                newItem.GetChild(1).GetComponent<TMP_Text>().SetText(player.answersCount.ToString());
                
                newItem.gameObject.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}