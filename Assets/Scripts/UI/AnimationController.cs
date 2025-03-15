using UnityEngine;

public abstract class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    protected string currentAnimation = "normal";

    protected void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        currentAnimation = animation;
        animator.CrossFade(animation, crossfade);
    }
}
