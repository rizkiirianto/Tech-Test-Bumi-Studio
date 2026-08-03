using UnityEngine;

namespace TechTest.Core
{
    public class GameStarter : MonoBehaviour
    {
        // Add a slight delay so all managers have time to run Awake/Start
        private void Start()
        {
            Invoke("StartGame", 0.5f);
        }

        private void StartGame()
        {
            if (RunManager.Instance != null)
            {
                RunManager.Instance.StartNewRun();
            }
        }
    }
}
