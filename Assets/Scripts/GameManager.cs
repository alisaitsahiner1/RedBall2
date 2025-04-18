using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Level Ayarları")]
    public Button levelButton;               // Bu sahnedeki buton
    public int levelToLoad = 1;              // Hangi level sahnesi yüklenecek

    [Header("Görsel Ayarlar")]
    public TextMeshProUGUI levelText;        // Butonun üzerindeki sayı yazısı

    void Start()
    {
        if (levelText != null)
        {
            levelText.text = levelToLoad.ToString(); // Level numarasını yaz
        }

        if (levelButton != null)
        {
            levelButton.onClick.AddListener(() =>
            {
                LoadLevel();
            });
        }
        
    }

    public void LoadLevel()
    {
        string levelName = "Level" + levelToLoad;
        SceneManager.LoadScene(levelName);
    }

    public void OpenLevelMenu()
    {
        SceneManager.LoadScene("Levels");
    }
}
