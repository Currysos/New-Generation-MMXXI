using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using UnityEditor;

public class OnCollisionDialog : MonoBehaviour
{
    public NPCConversation conversation;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PixelArtPlayerController>() != null) OnPlayerEnter();
    } 
    private void OnPlayerEnter()
    {
        print("Player entered dialog");
        ConversationManager.Instance.StartConversation(conversation);
    }

    public void DeleteWithFade(float fadeSpeed)
    {
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(FadeDeath(fadeSpeed));
    }

    private IEnumerator FadeDeath(float fadeSpeed)
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        while (renderer.color.a > 0)
        {
            Color objectColor = renderer.color;
            float fadeAmount = objectColor.a - (fadeSpeed * Time.deltaTime);

            objectColor = new Color(objectColor.r, objectColor.g, objectColor.b, fadeAmount);
            renderer.color = objectColor;
            yield return null;
        }
        Destroy(gameObject);
    }
}