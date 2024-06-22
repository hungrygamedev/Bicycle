using System.Collections;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    public AnimancerComponent _character;
    public AnimationClip _idle, _stopIdle, _jumpIdle, _jumpDown;
    public ClipTransition _stop,_jumpUp;
    public string lastAnimState;
    public void PlayAnimation(string state)
    {
        if (lastAnimState != state)
        {
            if (state == "idle")
            {
                _character.Play(_idle, 0.25f);
            }

            if (state == "stop")
            {
                _character.Play(_stop, 0.25f);
            }
            
            if (state == "stopIdle")
            {
                _character.Play(_stopIdle, 0.25f);
            }
            
            if (state == "jumpUp")
            {
                _character.Play(_jumpUp, 0.05f);
            }
            
            if (state == "jumpIdle")
            {
                _character.Play(_jumpIdle, 0.25f);
            }
            
            if (state == "jumpDown")
            {
                _character.Play(_jumpDown, 0.25f);
            }

            lastAnimState = state;
        }
    }
}
