using UnityEngine;

public class AnimationTriggerController : MonoBehaviour
{
    public Animator animator;

    public void TriggerPlaySound()
    {
        animator.SetTrigger("PlaySoundTrigger");
    }
}