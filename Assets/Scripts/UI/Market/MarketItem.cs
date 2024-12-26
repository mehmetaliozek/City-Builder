using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class MarketItem : MonoBehaviour
    {
        [SerializeField]
        private Image image;

        [SerializeField]
        private TextMeshProUGUI priceText;

        [SerializeField]
        private Button button;

        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }

        public void SetPrice(int price)
        {
            priceText.text = $"{price}$";
        }

        public void SetButtonOnClick(int index)
        {
            button.onClick.AddListener(() =>
            {
                if (!BuildingSystem.Instance.IsDestroyMode)
                {
                    Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
                    Ray ray = Camera.main.ScreenPointToRay(screenCenter);
                    if (Physics.Raycast(ray, out RaycastHit hit))
                    {
                        BuildingSystem.Instance.InitializeWithObject(BuildingSystem.Instance.GetSelectedItemCategory(index), hit.point);
                    }
                }
            });
        }
    }
}