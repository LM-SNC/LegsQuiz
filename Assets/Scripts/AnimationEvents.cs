using Reflex.Attributes;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
   [Inject] private GameManager _gameManager;
   
   
   public void OnAnimationEnd(string data)
   {
      _gameManager.OnAnimationEnd(data);
   }
}
