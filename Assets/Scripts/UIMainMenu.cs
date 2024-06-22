using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMainMenu : MonoBehaviour
{
    public static UIMainMenu instance;

    public SkinController _playerSkinController;

    public TMP_Text moneyText;

    public Transform CameraTr, pointMenu, pointShop;
    public CanvasGroup shopPanel, menuPanel,loadingImage;
    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ResetText();
        ActiveLoadingImage(false);
    }

    public void ActiveLoadingImage(bool value, bool loadMenu = false)
    {
        loadingImage.interactable = loadingImage.blocksRaycasts = value;
        if (value)
        {
            loadingImage.DOFade(1f, 1f).OnComplete(() =>
            {
                if (loadMenu)
                {
                    StartCoroutine(LoadGame());
                }
            });
            
        }
        else
        {
            loadingImage.DOFade(0f, 1f);
        }
    }

    public void LoadGameBtn()
    {
        ActiveLoadingImage(true,true);
    }
    
    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Game");
    }
    
    public void SetActiveShop(bool value)
    {
        shopPanel.interactable = shopPanel.blocksRaycasts = value;
        menuPanel.interactable = menuPanel.blocksRaycasts = !value;
        if (value)
        {
            shopPanel.DOFade(1f, 1f);
            menuPanel.DOFade(0f, 1f);
            CameraTr.DOMove(pointShop.position, 1f);
            CameraTr.DORotateQuaternion(pointShop.rotation, 1f);
        }
        else
        {
            shopPanel.DOFade(0f, 1f);
            menuPanel.DOFade(1f, 1f);
            CameraTr.DOMove(pointMenu.position, 1f);
            CameraTr.DORotateQuaternion(pointMenu.rotation, 1f);
            _playerSkinController.ActiveSkin();
        }
    }

    public void ResetText()
    {
        moneyText.text = PlayerPrefs.GetInt("Money", 0).ToString();
    }
}
