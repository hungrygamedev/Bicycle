using System;
using System.Collections;
using System.Collections.Generic;
using RandomNameAndCountry.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<Transform> enemys = new List<Transform>();
    public Transform player;

    public int moneyToAdd;

    public List<string> namesFinished = new List<string>();
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        player = Player.instance.transform;
        Player.instance.SetName(PlayerPrefs.GetString("Name",""));
    }

    public void ActiveRandomPlayer()
    {
        int id = GetDisablePlayer();
        RandomPlayerInfo info = RandomNameAndCountryPicker.Instance.GetRandomPlayerInfo();
        enemys[id].gameObject.SetActive(true);
        enemys[id].GetComponent<Enemy>().SetName(info.playerName);
    }

    int GetDisablePlayer()
    {
        int r = Random.Range(0, enemys.Count);

        if (enemys[r].gameObject.activeSelf)
        {
            return GetDisablePlayer();
        }
        else
        {
            return r;
        }
    }
    public void StartRace()
    {
        Player.instance.canMove = true;
        StartCoroutine(RandomStart());
    }

    IEnumerator RandomStart()
    {
        foreach (Transform enemy in enemys)
        {
            float rT = Random.Range(0f, 0.5f);
            yield return new WaitForSeconds(rT);
            enemy.GetComponent<Enemy>().canMove = true;
        }
    }

    public void AddPlayerMoney()
    {
        UIController.instance.AddMoney();
    }

    public void AddEnemyFinish(string name)
    {
        namesFinished.Add(name);
    }

    public void PlayerFinished()
    {
        namesFinished.Add("YOU");
        for (int i = 0; i < enemys.Count; i++)
        {
            if (enemys[i].GetComponent<Enemy>().canMove)
            {
                enemys[i].GetComponent<Enemy>().canMove = false;
            }

            if (namesFinished.Contains(enemys[i].GetComponent<Enemy>().nameText.text) == false)
            {
                namesFinished.Add(enemys[i].GetComponent<Enemy>().nameText.text);
            }
        }
        UIController.instance.ActiveWinPanel(namesFinished);
    }
}
