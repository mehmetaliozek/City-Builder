using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField]
    private Button pauseButton;

    [SerializeField]
    private Button contiuneButton;

    [SerializeField]
    private Button backToHomeButton;

    [SerializeField]
    private GameObject pausePanel;

    public void Start()
    {
        pauseButton.onClick.AddListener(() =>
        {
            Time.timeScale = 0;
            pausePanel.SetActive(true);
        });

        contiuneButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false);
        });

        backToHomeButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        });
    }
}
