using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Home : MonoBehaviour
{
    [SerializeField]
    private Button adminButton;

    [SerializeField]
    private Button playButton;

    [SerializeField]
    private Button signOutButton;

    [SerializeField]
    private Button exitButton;

    public void Start()
    {
        adminButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(2);
        });

        playButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("LoadedSceneUserId", GameManager.Instance.userModel.UserId);
            SceneManager.LoadScene(1);
        });

        signOutButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetString("currentUser", new UserModel().ToJson());
            GameManager.Instance.homePanel.SetActive(false);
            GameManager.Instance.signInPanel.SetActive(true);
        });

        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    public void OnEnable()
    {
        if (GameManager.Instance.userModel.UserRoleID != 3)
        {
            adminButton.gameObject.SetActive(true);
        }
    }

    public void OnDisable()
    {
        adminButton.gameObject.SetActive(false);
    }
}
