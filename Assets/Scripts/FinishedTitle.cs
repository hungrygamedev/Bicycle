using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishedTitle : MonoBehaviour
{
    public TMP_Text name;

    public void SetName(string _name)
    {
        name.text = _name;
    }
}
