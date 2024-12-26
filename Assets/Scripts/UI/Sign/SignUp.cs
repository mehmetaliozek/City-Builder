using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignUp : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField userName;

    [SerializeField]
    private TMP_InputField email;

    [SerializeField]
    private TMP_InputField password;

    [SerializeField]
    private Button signUpButton;

    [SerializeField]
    private Button signInLink;

    public void Start()
    {
        signUpButton.onClick.AddListener(() =>
        {
            if (userName.text != "" || email.text != "" || password.text != "")
            {
                bool isCreated = DatabaseManager.Instance.Signup(userName.text, email.text, password.text);
                if (isCreated)
                {
                    GameManager.Instance.signInPanel.SetActive(true);
                    gameObject.SetActive(false);
                }
            }
        });

        signInLink.onClick.AddListener(() =>
        {
            GameManager.Instance.signInPanel.SetActive(true);
            gameObject.SetActive(false);
        });
    }
}
