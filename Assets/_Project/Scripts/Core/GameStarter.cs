using UnityEngine;

namespace TechTest.Core
{
    public class GameStarter : MonoBehaviour
    {
        [Header("Scene Transition Cover")]
        public GameObject loadingPanel;

        private System.Collections.IEnumerator Start()
        {
            // Pastikan panel loading menyala menutupi layar saat scene baru saja terbuka
            if (loadingPanel != null) loadingPanel.SetActive(true);

            // Tunggu 1 frame agar semua script lain selesai memanggil Awake()
            yield return null;

            // Jalankan proses spawn musuh, pembagian kartu, dll di belakang layar
            if (RunManager.Instance != null)
            {
                RunManager.Instance.StartNewRun();
            }

            // Biarkan Loading Panel menutupi proses tersebut selama 1 detik (seperti yang kamu minta)
            yield return new WaitForSeconds(1f);

            // Matikan cover loading-nya, ta-da! Game sudah siap dimainkan
            if (loadingPanel != null) loadingPanel.SetActive(false);
        }
    }
}
