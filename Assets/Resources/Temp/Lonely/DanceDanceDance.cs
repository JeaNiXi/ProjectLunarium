using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class DanceDanceDance : MonoBehaviour
{
    public bool isDanceTime;
    public AudioSource audioSource;
    public AudioClip[] clips;
    public SpriteRenderer sRenderer;
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
    }
    private IEnumerator startSprite()
    {
        var color = new Color(1, 1, 1, 1);
        yield return new WaitForSeconds(7f);
        sRenderer.color = color;
    }
}
