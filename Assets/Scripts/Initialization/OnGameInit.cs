using Managers;
using UI;
using UnityEngine;
namespace Initialization
{
    // HAPPY NEW YEAR 2026
    /*
     *      Основная точка входа в игру. Используется для инициализации основного меню при включении игры.
     */
    public class OnGameInit : MonoBehaviour
    {
        public void Start()
        {
            Debug.Log("Started Game Initialization!");
            UIManager.Instance.InitializeMainMenu();
        }
        //public void Start()
        //{
        //    GameManager.Instance.SetGameState(GameManager.GameState.RUNNING);
        //    GameManager.Instance.EnableTickPossibility();
        //}
    }
}