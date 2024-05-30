using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip animationEndClip;

    // Método para ser chamado no evento de animação
    public void PlayAnimationEndSound()
    {
        if (audioSource != null && animationEndClip != null)
        {
            audioSource.PlayOneShot(animationEndClip);
        }
    }
}
