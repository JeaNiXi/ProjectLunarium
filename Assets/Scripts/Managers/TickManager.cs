using UnityEngine;
namespace Managers
{
    // 1 час        = 0.0416 секунд     = 2.496 тика
    // 1 день       = 1 секунда         = 60 тиков
    // 1 неделя     = 7 секунд          = 420 тиков
    // 1 месяц      = 31 секунда        = 1860 тиков
    // 1 год        = 6.2 минуты        = 22320 тиков
    // 10 лет       = 1 час 2 минуты    = 223200 тиков
    // 100 лет      = 10 часов, 20 мин  = 2232000 тиков

    public class TickManager : MonoBehaviour
    {
        public static TickManager Instance { get; private set; }

        private const float SECONDS_PER_DAY = 1f;
        private float timer = 0f;

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }
        private void FixedUpdate()
        {
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING && GameManager.Instance.IsTickPossible)
                UpdateTick();
        }
        private void UpdateTick()
        {
            timer += Time.deltaTime;
            if(timer > SECONDS_PER_DAY)
            {
                GameManager.Instance.DisableTickPossibility();
                TimeManager.Instance.OnTickUpdate();
                timer = 0f;
            }
        }
    }
}