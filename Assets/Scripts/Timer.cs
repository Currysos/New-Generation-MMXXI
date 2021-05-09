using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private TextMeshProUGUI ClockTextObject;
    private string[] ClockText;
    private float ElapsedTime = 0f;
    private bool Ticking = false;
    public float LevelTime = 0;

    private void Start()
    {
        ClockTextObject = GetComponent<TextMeshProUGUI>();
        StartTimer();
    }

    public void StartTimer() => Ticking = true;

    public void StopTimer()
    {
        Ticking = false;
        LevelTime = ElapsedTime;
    }

    private void Update()
    {
        if (Ticking)
        {
            ElapsedTime += Time.deltaTime;
            string text = ElapsedTime.ToString("F1");
            ClockText = text.Split('.');

            if (ClockText[0].Length == 1) { ClockText[0] = "0" + ClockText[0]; }

            ClockTextObject.SetText(ClockText[0] + ":" + ClockText[1]);
        }
    }
}
