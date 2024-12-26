using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserRow : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI Id;
    [SerializeField]
    private TextMeshProUGUI UserName;
    [SerializeField]
    private TextMeshProUGUI Email;
    [SerializeField]
    private TextMeshProUGUI Energy;
    [SerializeField]
    private TextMeshProUGUI Money;
    [SerializeField]
    private TextMeshProUGUI Food;
    [SerializeField]
    private TMP_Dropdown banDropDown;
    [SerializeField]
    private TMP_Dropdown roleDropDown;
    [SerializeField]
    private Button viewButton;

    public void SetRow(UserModel userModel)
    {
        Id.text = userModel.UserId.ToString();
        UserName.text = userModel.Username;
        Email.text = userModel.Email;
        Energy.text = userModel.Energy.ToString();
        Money.text = userModel.Money.ToString();
        Food.text = userModel.Food.ToString();

        banDropDown.value = (userModel.IsBanned ? 1 : 0);
        banDropDown.onValueChanged.AddListener((call) =>
        {
            DatabaseManager.Instance.BanUser(userModel.UserId, banDropDown.options[call].text.ToUpper());
        });

        roleDropDown.value = userModel.UserRoleID - 1;
        roleDropDown.onValueChanged.AddListener(call =>
        {
            DatabaseManager.Instance.UpdateUserRole(userModel.UserId, call + 1);
        });

        viewButton.onClick.AddListener(() =>
        {
            PlayerPrefs.SetInt("LoadedSceneUserId", userModel.UserId);
            SceneManager.LoadScene(1);
        });
    }
}
