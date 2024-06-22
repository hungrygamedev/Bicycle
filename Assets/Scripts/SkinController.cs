using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SkinController : MonoBehaviour
{
    public bool isPlayer;
    public List<GameObject> skins = new List<GameObject>();
    private int currentSkin, currentBoard, currentTrails;

    public Renderer board;
    public List<Texture> boardTextures = new List<Texture>();

    public Transform trailPos;
    public List<GameObject> trails = new List<GameObject>();

    void Awake()
    {
        ActiveSkin();
    }

    public void ActiveSkin()
    {
        if (isPlayer)
        {
            currentSkin = PlayerPrefs.GetInt("PlayerSkin", 0);
            currentBoard = PlayerPrefs.GetInt("PlayerBoard", 0);
            currentTrails = PlayerPrefs.GetInt("PlayerTrail", 0);
        }
        else
        {
            currentSkin = Random.Range(0, skins.Count);
            currentBoard = Random.Range(0, boardTextures.Count);
            currentTrails = Random.Range(0, trails.Count);
        }

        foreach (var skin in skins)
        {
            skin.SetActive(false);
        }
        skins[currentSkin].SetActive(true);
        board.material.mainTexture = boardTextures[currentBoard];
        if(GetComponent<PlayerAnimationController>()!=null)
            GetComponent<PlayerAnimationController>()._Character = skins[currentSkin].GetComponent<CharacterAnimation>();

        if (trails.Count > 0)
        {
            if (currentTrails > 0)
            {
                GameObject trail = Instantiate(trails[currentTrails-1], trailPos);
                trail.transform.localPosition = Vector3.zero;
            }
        }
    }

    public void ActivePreview(string type, int id=0)
    {
        if (type == "PlayerSkin")
        {
            foreach (var skin in skins)
            {
                skin.SetActive(false);
            }
            skins[id].SetActive(true);
        }
        if (type == "PlayerBoard")
        {
            board.material.mainTexture = boardTextures[id];
        }
    }
   

}
