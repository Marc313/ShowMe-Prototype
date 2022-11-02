using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatedObject : MonoBehaviour
{
    [SerializeField] protected AnimationClip clip;
    protected Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            animator = GetComponentInParent<Animator>();
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
