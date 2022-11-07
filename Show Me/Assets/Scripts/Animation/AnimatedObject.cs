using UnityEngine;

public class AnimatedObject : MonoBehaviour
{
    [SerializeField] protected AnimationClip clip;
    [SerializeField] protected Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
        }
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void PlayAnimationCrossFade()
    {
        animator.CrossFade(clip.name, 0f);
    }

    public void PlayAnimationCrossFade(AnimationClip _clip)
    {
        animator.CrossFade(_clip.name, 0f);
    }
}
