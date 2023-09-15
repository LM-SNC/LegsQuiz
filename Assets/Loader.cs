using Reflex.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loader : MonoBehaviour
{
    private void Start()
    {
        ReflexSceneManager.LoadScene("Greet", LoadSceneMode.Single, builder =>
        {
            // This deferred descriptor will run just before Greet.unity SceneScope installers
           // builder.AddInstance("beautiful");
        });
    }
}