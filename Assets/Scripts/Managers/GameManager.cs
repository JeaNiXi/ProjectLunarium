using UnityEngine;
namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public enum GameState
        {
            INIT,
            PAUSE,
            RUNNING
        }
        public GameState CurrentGameState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            CurrentGameState = GameState.INIT;
        }
        public void SetGameState(GameState state) => CurrentGameState = state;
    }
}