using TMPro;
using UnityEngine;

public class ResourceItem : MonoBehaviour
{
    public ResourceType type;
    public int amount;
    public TextMeshProUGUI amountText;

    public void SetAmount(int newAmount)
    {
        amountText.text = newAmount.ToString();
    }
}
