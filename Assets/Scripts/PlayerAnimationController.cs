using System.Collections;
using System.Collections.Generic;
using Animancer;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{

    public AnimancerComponent _board;

    public AnimationClip _boardIdle, _boardStop;
    public ClipTransition _jump360,_jump360_2;
    public CharacterAnimation _Character;

    private string lastAnimState;
    
    public void PlayAnimationBoard(string state)
    {
        if (lastAnimState != state)
        {
            if (state == "idle")
            {
                _board.Play(_boardIdle, 0.25f);
                //_Character.PlayAnimation(state);
            }

            if (state == "stop")
            {
               // _board.Play(_boardStop, 0.25f);
                //_Character.PlayAnimation(state);
            }

            if (state == "jumpUp")
            {
                //_Character.PlayAnimation(state);
            }
            
            if (state == "jumpDown")
            {
                //_Character.PlayAnimation(state);
            }

            lastAnimState = state;
        }
    }
    void Start()
    {
        PlayAnimationBoard("idle");
    }

    public void Jump360(int id)
    {
        if (id == 1)
        {
            _board.Play(_jump360, 0.25f);
        }
        if (id == 2)
        {
            _board.Play(_jump360_2, 0.25f);
        }
    }
}
