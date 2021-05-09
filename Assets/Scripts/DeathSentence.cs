using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathSentence : MonoBehaviour
{
    public float time = 1f;
    void Start() => Destroy(this.gameObject, time);
}
