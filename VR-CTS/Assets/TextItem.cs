using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextItem : MonoBehaviour
{
    public Text title;
    public Text date;

    public void SetTitle(string textTitle)
    {
        title.text = textTitle;
    }

    public void Selectlevel()
    {
        GlobalData.LevelName = title.text;
    }
}
