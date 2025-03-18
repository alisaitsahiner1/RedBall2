using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public GameObject controllerUI; // UI Objeni Inspector'dan atayacağız

    public static LevelLoader instance; // Singleton için
    public Animator transition; // UI animasyonu için Animator
    public float transitionTime = 1f; // Geçiş süresi

    void Awake()
    {
        // Eğer zaten bir LevelLoader varsa, yok et (Duplicate oluşmasını önlüyoruz)
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // LevelLoader sahne değişse bile yok olmasın

            if (controllerUI != null)
            {
                DontDestroyOnLoad(controllerUI); // UI Objeni de koru
            }
        }
        else
        {
            Destroy(gameObject); // Zaten bir tane varsa, yenisini yok et
        }
    }

    void Start()
    {
        if (transition != null)
        {
            transition.SetTrigger("Start"); // Yeni level başladığında "Start" animasyonunu oynat
        }
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(SceneManager.GetActiveScene().buildIndex + 1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        // 1️⃣ End Animasyonunu Oynat
        if (transition != null)
        {
            transition.SetTrigger("End");
        }

        // 2️⃣ End animasyonu oynayana kadar bekle
        yield return new WaitForSeconds(transitionTime);

        // 3️⃣ Yeni Level'i Yükle
        SceneManager.LoadScene(levelIndex);

        // 4️⃣ Yeni sahne yüklendiğinde Start Animasyonunu HEMEN tetikle
      
        if (transition != null)
        {
            transition.SetTrigger("Start");
        }
    }
}
