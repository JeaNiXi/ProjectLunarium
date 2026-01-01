using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DanceDanceDance : MonoBehaviour
{
    public bool isDanceTime;
    public AudioSource audioSource;
    public AudioClip[] clips;
    public SpriteRenderer sRenderer;
    public SpriteRenderer[] lonelySprites;
    public float lonelyTime;
    public void Start()
    {
        var color = new Color(1, 1, 1, 0);
        sRenderer.color= color;
        if(isDanceTime)
        {
            var danceInt = Random.Range(0,clips.Length);
            audioSource.PlayOneShot(clips[danceInt]);
        }
        StartCoroutine(startSprite());
        StartCoroutine(startLanelySprites(lonelyTime));
    }
    private IEnumerator startSprite()
    {
        var color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(7f);
        sRenderer.color = color;
    }
    private IEnumerator startLanelySprites(float time)
    {
        var color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(time);
        foreach(var spriteRenderes in lonelySprites)
            spriteRenderes.color = color;
    }
}
