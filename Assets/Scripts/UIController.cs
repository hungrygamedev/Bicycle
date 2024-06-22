using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    public VariableJoystick _Joystick;
    public FixedTouchField touchField;

    public TMP_Text countDownText;

    public PlayersPositionUI _PlayersPositionUI;

    public TMP_Text moneyText;
    public GameObject moneyUI;
    public Transform moneyEndPoint;

    public CanvasGroup pauseMenu,winPanel, loadingImage;

    public List<FinishedTitle> FinishedTitles = new List<FinishedTitle>();

    private void Awake()
    {
        instance = this;
    }
    
    
    void Start()
    {
        ActiveLoadingImage(false);
        StartCoroutine(WaitPlayers());
        ResetText();
    }

    public void ActiveWinPanel(List<string> _names)
    {
        for (int i = 0; i < 5; i++)
        {
            FinishedTitles[i].SetName(_names[i]);
        }

        winPanel.interactable = winPanel.blocksRaycasts = true;
        winPanel.DOFade(1f, 1f);
    }

    public void Leave()
    {
        ActiveLoadingImage(true,true);
    }

    public void ActiveMenu(bool value)
    {
        pauseMenu.interactable = pauseMenu.blocksRaycasts = value;
        if (value)
        {
            pauseMenu.DOFade(1f, 1f);
        }
        else
        {
            pauseMenu.DOFade(0f, 1f);
        }
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
                    SceneManager.LoadScene(1);
                }
            });
            
        }
        else
        {
            loadingImage.DOFade(0f, 1f);
        }
    }

    IEnumerator LoadMainMenu()
    {
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(1);
    }
    public void ResetText()
    {
        moneyText.text = PlayerPrefs.GetInt("Money", 0).ToString();
    }
    IEnumerator WaitPlayers()
    {
        int enemyConnected = 0;
        while (enemyConnected<9)
        {
            yield return new WaitForSeconds(0.25f);
            GameManager.instance.ActiveRandomPlayer();
            enemyConnected++;
        }
        yield return new WaitForSeconds(1f);
        countDownText.transform.localScale = Vector3.zero;
        countDownText.text = "3";
        countDownText.transform.DOScale(1f, 1f);
        yield return new WaitForSeconds(1f);
        countDownText.transform.localScale = Vector3.zero;
        countDownText.text = "2";
        countDownText.transform.DOScale(1f, 1f);
        yield return new WaitForSeconds(1f);
        countDownText.transform.localScale = Vector3.zero;
        countDownText.text = "1";
        countDownText.transform.DOScale(1f, 1f);
        yield return new WaitForSeconds(1f);
        countDownText.transform.localScale = Vector3.zero;
        countDownText.text = "GO!";
        countDownText.transform.DOScale(1f, 1f).OnComplete(() =>
        {
            countDownText.DOColor(new Color(0, 0, 100, 0), 0.5f);
        });
        GameManager.instance.StartRace();
    }
    public void JumpButton()
    {
        Player.instance.Jump();
    }

    public void AddMoney()
    {
        GameObject m = Instantiate(moneyUI, transform);
        m.transform.DOMove(moneyEndPoint.position, 0.5f).SetEase(Ease.Linear).OnComplete(() =>
        {
            PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) +GameManager.instance.moneyToAdd);
            ResetText();
            Destroy(m);
        });
    }
}
