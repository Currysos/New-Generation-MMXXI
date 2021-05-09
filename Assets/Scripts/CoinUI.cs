using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class CoinUI : MonoBehaviour
{
    public TextMeshProUGUI coinText;

    public void SetText(int coins) => coinText.SetText(coins.ToString());
}
