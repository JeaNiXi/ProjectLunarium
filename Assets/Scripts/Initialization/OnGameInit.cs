using Managers;
using SO;
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
        public bool DevModeSkipIntro;
        public NewGameStateSO DefaultDevNewGameState;
        public void Start()
        {
            if (DevModeSkipIntro)
            {
                Debug.Log("OnGameInit Start Called!");
                Debug.Log("[DEV SKIP MODE ACTIVATED!]");
                UIManager.Instance.InitializeUI();
                GameManager.Instance.StartNewGame(new State.NewGameState(DefaultDevNewGameState));
                UIManager.Instance.HideCurrentPage();
                return;
            }
            Debug.Log("Started Game Initialization!");
            UIManager.Instance.InitializeUI();
        }
    }
}