using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenController : MonoBehaviour
{
    public Slider x_start, y_start, x_end, y_end;
    public Text text_x_start, text_y_start, text_x_end, text_y_end;
    public Button go;
    public mainScript scripts;

    void Start()
    {
        x_start.onValueChanged.AddListener((x) => { text_x_start.text = x.ToString(); });
        x_end.onValueChanged.AddListener((x) => { text_x_end.text = x.ToString(); });
        y_start.onValueChanged.AddListener((x) => { text_y_start.text = x.ToString(); });
        y_end.onValueChanged.AddListener((x) => { text_y_end.text = x.ToString(); });
        go.onClick.AddListener(go_onclick);
    }

    private void go_onclick()
    {
        scripts.onGo((int)x_start.value, (int)y_start.value, (int)x_end.value, (int)y_end.value);
    }
}
