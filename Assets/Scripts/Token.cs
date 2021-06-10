using System;
using UnityEngine;

public class Token : MonoBehaviour
{
    public GameObject _tokenCollectAudio;
    void OnTriggerEnter2D(Collider2D other)
    {
        //only exectue OnPlayerEnter if the player collides with this token.
        var player = other.gameObject.GetComponent<PixelArtPlayerController>();
        if (player != null) OnPlayerEnter(player);
    }

    void OnPlayerEnter(PixelArtPlayerController player)
    {
        player.CoinsCollected += 1;
        Instantiate(_tokenCollectAudio, transform.position, transform.rotation);
        Destroy(gameObject);
    }
}
