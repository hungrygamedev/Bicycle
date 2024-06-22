using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSFX : MonoBehaviour
{
    public static PlayerSFX instance;
    
    public AudioSource boardRun1, boardRun2, boardStop;
    void Awake()
    {
        instance = this;
    }

    public void PlayBoard(int id)
    {
        if (id == 1)
        {
            if(boardRun1.isPlaying==false)
                StartCoroutine(Board1());
        }
        else
        {
            if(boardRun2.isPlaying==false)
                StartCoroutine(Board2());
        }
    }

    public void PauseRunBoard()
    {
        if (boardRun2.isPlaying || boardRun1.isPlaying)
        {
            boardRun1.Stop();
            boardRun2.Stop();
        }
    }

    IEnumerator Board1()
    {
        boardRun1.Play();
        while (boardRun1.isPlaying)
        {
            if (boardRun1.time >= 4f)
            {
                PlayBoard(2);
            }
            
            yield return null;
        }
    }
    IEnumerator Board2()
    {
        boardRun2.Play();
        while (boardRun2.isPlaying)
        {
            if (boardRun2.time >= 4f)
            {
                PlayBoard(1);
            }
            
            yield return null;
        }
    }
}
