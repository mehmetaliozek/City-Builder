using UnityEngine;
using UnityEngine.UI;

public class UserColumn : MonoBehaviour
{
    [SerializeField]
    private string columnName;

    [SerializeField]
    private Admin admin;

    private string[] type = { "asc", "desc" };
    private int currentIndex = 0;

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            currentIndex = currentIndex == 0 ? 1 : 0;
            admin.OrderBy(columnName, type[currentIndex]);
        });
    }
}
