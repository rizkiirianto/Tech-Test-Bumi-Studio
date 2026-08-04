using UnityEngine;
using UnityEngine.SceneManagement;

namespace TechTest.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        [Header("UI Elements")]
        public GameObject loadingText; // Assign a "Loading..." text here if you want

        public void StartGame()
        {
            if (loadingText != null) loadingText.SetActive(true);
            StartCoroutine(LoadSceneCoroutine("BattleScene"));
        }

        private System.Collections.IEnumerator LoadSceneCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            // Cegah Unity langsung pindah scene yang membuat freeze
            asyncLoad.allowSceneActivation = false;

            // Tunggu sampai Unity selesai memuat data scene di background (mentok di 0.9)
            while (asyncLoad.progress < 0.9f)
            {
                yield return null;
            }

            // Beri waktu sejenak (opsional) agar teks "Loading..." terbaca 
            // dan frame sempat me-render UI MainMenu dengan benar.
            yield return new WaitForSeconds(0.5f);

            // Izinkan pindah scene
            asyncLoad.allowSceneActivation = true;
        }

        public void ExitGame()
        {
            Debug.Log("Exiting Game...");
            Application.Quit();
        }
    }
}
