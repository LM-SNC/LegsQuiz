using Reflex.Attributes;
using UnityEngine;

public class GameImage : MonoBehaviour
{
   [Inject] private GameManager _gameManager;
   
   
   public void OnAnimationEnd()
   {
      _gameManager.OnAnimationEnd();
   }
}
