using Managers;
using UnityEngine;
namespace Initialization
{
    public class OnGameInit : MonoBehaviour
    {
        public bool PlayGIF;
        public void Start()
        {
            Debug.Log("Started Game Initialization!");
            GameManager.Instance.SetGameState(GameManager.GameState.RUNNING);
        }
    }
}