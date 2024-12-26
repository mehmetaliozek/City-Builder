using Assets.Scripts.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketManager : MonoBehaviour
{

    public GameObject marketContent;
    public GameObject[] categories;
    public GameObject marketItem;
    private List<GameObject> marketItems = new();
    public Sprite[] marketItemSprites;

    private bool isAnimateMarketPanel;


    public void Start()
    {
        for (int i = 0; i < categories.Length; i++)
        {
            int index = i;
            categories[i].GetComponent<Button>().onClick.AddListener(() => CategoryButtonOnClick(index));
        }
        InitiliazeMarketItem();
    }

    public void Update()
    {
        if (GameManager.Instance.userModel.UserId != PlayerPrefs.GetInt("LoadedSceneUserId")) return;
        if (Input.GetKeyDown(KeyCode.P) && !isAnimateMarketPanel)
        {
            isAnimateMarketPanel = true;
            StartCoroutine(MarketOpenClose());
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Debug.Log("aaaaaa");
        }
    }

    private IEnumerator MarketOpenClose()
    {
        Vector2 startPos = transform.position;
        Vector2 targetPos = new(startPos.x, startPos.y < 0 ? 20 : -291);

        float duration = 0.5f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            transform.position = Vector2.Lerp(startPos, targetPos, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPos;

        isAnimateMarketPanel = false;
    }

    private void CategoryButtonOnClick(int index)
    {
        for (int i = 0; i < categories.Length; i++)
        {
            categories[i].GetComponent<Image>().color = new Color(1, 1, 1, i == index ? 1 : 0.5f);
            categories[i].GetComponentInChildren<TextMeshProUGUI>().color = new Color(0, 0, 0, i == index ? 1 : 0.5f);
        }
        SetMarketItems(index);
    }

    private void InitiliazeMarketItem()
    {
        for (int i = 0; i < marketItemSprites.Length; i++)
        {
            GameObject mi = Instantiate(marketItem, marketContent.transform.position, marketContent.transform.rotation);
            mi.GetComponent<MarketItem>().SetImage(marketItemSprites[i]);
            mi.GetComponent<MarketItem>().SetPrice(BuildingSystem.Instance.GetItemPrice(i));
            mi.GetComponent<MarketItem>().SetButtonOnClick(i);
            mi.transform.SetParent(marketContent.transform, false);
            marketItems.Add(mi);
        }

        SetMarketItems(0);
    }

    private void SetMarketItems(int index)
    {
        (int startIndex, int count) = BuildingSystem.Instance.GetSelectedCategoryItemCount(index);

        for (int i = 0; i < marketItemSprites.Length; i++)
        {
            marketItems[i].SetActive(false);
            if (i >= startIndex && i < startIndex + count)
            {
                marketItems[i].SetActive(true);
            }
        }
    }
}
