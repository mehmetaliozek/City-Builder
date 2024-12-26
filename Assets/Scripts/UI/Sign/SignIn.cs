using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignIn : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField userName;

    [SerializeField]
    private TMP_InputField password;

    [SerializeField]
    private Button signInButton;

    [SerializeField]
    private Button signUpLink;

    public void Start()
    {
        signInButton.onClick.AddListener(() =>
        {
            if (userName.text != "" || password.text != "")
            {
                UserModel userModel = DatabaseManager.Instance.Login(userName.text, password.text);
                if (userModel.UserId != -1)
                {
                    if (userModel.IsBanned)
                    {
                        Debug.Log("Banlýsýnýz");
                        return;
                    }
                    PlayerPrefs.SetString("currentUser", userModel.ToJson());
                    GameManager.Instance.userModel = userModel;
                    GameManager.Instance.homePanel.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        });

        signUpLink.onClick.AddListener(() =>
        {
            GameManager.Instance.signUpPanel.SetActive(true);
            gameObject.SetActive(false);
        });
    }
}
