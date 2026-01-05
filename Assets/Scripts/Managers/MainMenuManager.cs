using UnityEngine;
namespace Managers
{
    public class MainMenuManager : MonoBehaviour
    {
        public static MainMenuManager Instance;
        private void Awake()
        {
            if (Instance != this && Instance != null)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
    }
}