using UnityEngine;

public class DanceDanceDance : MonoBehaviour
{
    public bool isDanceTime;
    public AudioSource audioSource;
    public AudioClip audioClip;
    public void Start()
    {
        if(isDanceTime)
        {
            audioSource.PlayOneShot(audioClip);
        }
    }
}
