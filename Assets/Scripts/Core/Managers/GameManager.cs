using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector]
    public UserModel userModel = new UserModel();

    public GameObject signInPanel;
    public GameObject signUpPanel;
    public GameObject homePanel;
    public void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Instance.signInPanel = signInPanel;
            Instance.signUpPanel = signUpPanel;
            Instance.homePanel = homePanel;
            Instance.Start();
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void Start()
    {
        userModel = UserModel.FromJson(PlayerPrefs.GetString("currentUser", new UserModel().ToJson()));

        if (userModel.UserId != -1)
        {
            homePanel.SetActive(true);
        }
        else
        {
            signInPanel.SetActive(true);
        }
    }
}
