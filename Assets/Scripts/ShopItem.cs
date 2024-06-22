using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItem : MonoBehaviour
{
    public enum CostType
    {
        money,
        daily
    }

    public CostType costType;
    public string itemType;
    public int itemID;
    public int cost;
    public TMP_Text costText;
    
    public GameObject btnBuy, btnChoose, textChoosed;
    public Image bg,btnBuyImage;
    public Sprite bgChoosed, bgIdle, btnOn, btnOff;
    void Start()
    {
        CheckState();
        if (GetComponent<Button>() != null)
        {
            GetComponent<Button>().onClick.AddListener(delegate { ActivePreview(); });
        }

        if (costType != CostType.daily)
        {
            btnBuy.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(); });
        }

        btnChoose.GetComponent<Button>().onClick.AddListener(delegate { SelectItem(); });
    }

    public void CheckState()
    {
        if (costType == CostType.money)
        {
            if (itemID > 0)
            {
                if (PlayerPrefs.GetInt(itemType + "-" + itemID, 0) == 0)
                {
                    costText.text = cost.ToString();
                    btnBuy.SetActive(true);
                    btnChoose.SetActive(false);
                    textChoosed.SetActive(false);
                    bg.sprite = bgIdle;
                    if (PlayerPrefs.GetInt("Money", 0) >= cost)
                    {
                        btnBuyImage.sprite = btnOn;
                    }
                    else
                    {
                        btnBuyImage.sprite = btnOff;
                    }
                }
                else
                {
                    btnBuy.SetActive(false);
                    if (PlayerPrefs.GetInt(itemType, 0) == itemID)
                    {
                        bg.sprite = bgChoosed;
                        btnChoose.SetActive(false);
                        textChoosed.SetActive(true);
                    }
                    else
                    {
                        bg.sprite = bgIdle;
                        btnChoose.SetActive(true);
                        textChoosed.SetActive(false);
                    }
                }
            }
            else
            {
                btnBuy.SetActive(false);
                if (PlayerPrefs.GetInt(itemType, 0) == itemID)
                {
                    bg.sprite = bgChoosed;
                    btnChoose.SetActive(false);
                    textChoosed.SetActive(true);
                }
                else
                {
                    bg.sprite = bgIdle;
                    btnChoose.SetActive(true);
                    textChoosed.SetActive(false);
                }
            }
        }
        else
        {
            
            if (PlayerPrefs.GetInt(itemType + "-" + itemID, 0) == 0)
            {
                btnBuy.SetActive(true);
                btnChoose.SetActive(false);
                textChoosed.SetActive(false);
                bg.sprite = bgIdle;
            }
            else
            {
                btnBuy.SetActive(false);
                if (PlayerPrefs.GetInt(itemType, 0) == itemID)
                {
                    bg.sprite = bgChoosed;
                    btnChoose.SetActive(false);
                    textChoosed.SetActive(true);
                }
                else
                {
                    bg.sprite = bgIdle;
                    btnChoose.SetActive(true);
                    textChoosed.SetActive(false);
                }
            }
        }
    }

    public void SelectItem()
    {
        if (itemID > 0)
        {
            if (PlayerPrefs.GetInt(itemType + "-" + itemID, 0) == 0)
            {
                if (PlayerPrefs.GetInt("Money", 0) >= cost)
                {
                    PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) - cost);
                    PlayerPrefs.SetInt(itemType + "-" + itemID, 1);
                    PlayerPrefs.SetInt(itemType, itemID);
                    CheckStateAll();
                }
            }
            else
            {
                PlayerPrefs.SetInt(itemType, itemID);
                CheckStateAll();
            }
        }
        else
        {
            PlayerPrefs.SetInt(itemType, itemID);
            CheckStateAll();
        }
    }

    public void ActivePreview()
    {
        UIMainMenu.instance._playerSkinController.ActivePreview(itemType,itemID);
    }
    void CheckStateAll()
    {
        UIMainMenu.instance._playerSkinController.ActiveSkin();
        UIMainMenu.instance.ResetText();
        foreach (Transform child in transform.parent)
        {
            child.GetComponent<ShopItem>().CheckState();
        }
    }
}
