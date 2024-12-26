using System.Collections.Generic;
using UnityEngine;

public class Admin : MonoBehaviour
{
    [SerializeField]
    private GameObject userRow;

    [SerializeField]
    private GameObject content;

    public void Start()
    {
        List<UserModel> users = DatabaseManager.Instance.GetAllUser();

        foreach (UserModel user in users)
        {
            var row = Instantiate(userRow, content.transform);
            row.GetComponent<UserRow>().SetRow(user);
        }
    }

    public void OrderBy(string column, string type)
    {
        List<UserModel> users = DatabaseManager.Instance.OrderBy(column, type);

        foreach (UserRow item in gameObject.GetComponentsInChildren<UserRow>())
        {
            Destroy(item.gameObject);
        }

        foreach (UserModel user in users)
        {
            var row = Instantiate(userRow, content.transform);
            row.GetComponent<UserRow>().SetRow(user);
        }
    }
}
