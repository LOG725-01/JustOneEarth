using UnityEngine;

public class AnimalAnimatorController : AnimationController
{

    [SerializeField] private const string idle = "IdleBlend";
    [SerializeField] private const string idleValue = "Idle";
    [SerializeField] private const string forward = "ForwardBlend";
    [SerializeField] private const string forwardValue = "Forward";
    [SerializeField] private const string turn = "TurnBlend";
    [SerializeField] private const string turnValue = "Turn";
    [SerializeField] private const string shuffle = "ShuffleBlend";
    [SerializeField] private const string shuffleValue = "Shuffle";
    [SerializeField] private const string sit = "Sit";
    [SerializeField] private const string stand = "Stand";

    [SerializeField] private const string eat = "Eat";
    [SerializeField] private const string backward = "Backward";
    [SerializeField] private const string attack = "Attack";
    [SerializeField] private const string gotHit = "GotHit";


    private float sitStandDuration = 2.5f;
    private float turnDuration = 1.5f;
    private float gotHitDuration = 1f;
    private float attackDuration = 1f;
    private float shuffleDuration;
    private float eatDuration;

    private void Start()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "GoatSheep_turn_90_L":
                    turnDuration = clip.length;
                    break;
                case "GoatSheep_stand_to_sit":
                    sitStandDuration = clip.length;
                    break;
                case "GoatSheep_hit_reaction":
                    gotHitDuration = clip.length;
                    break;
                case "GoatSheep_attack01":
                    attackDuration = clip.length;
                    break;
                case "GoatSheep_shuffle_R":
                    shuffleDuration = clip.length;
                    break;
                case "GoatSheep_eating":
                    eatDuration = clip.length;
                    break;
            }
        }
    }
    private void SetValue(string name, float value)
    {
        animator.SetFloat(name, value);
    }
    private void SetRandomValue(string name)
    {
        SetValue(name, Random.Range(0f, 1f));
    }

    public void IdleAnimation()
    {
        SetRandomValue(idleValue);
        ChangeAnimation(idle);
    }

    public void WalkRandomAnimation()
    {
        if (currentAnimation == forward) return;
        SetRandomValue(forwardValue);
        ChangeAnimation(forward);
    }
    public void RunAnimation()
    {
        SetValue(forwardValue, 1f);
        ChangeAnimation(forward);
    }
    public void WalkBackwardAnimation()
    {
        ChangeAnimation(backward);
    }

    public float TurnAnimation(bool turnLeft)
    {
        if (turnLeft) SetValue(turnValue, 0f);
        else SetValue(turnValue, 1f);
        ChangeAnimation(turn);
        return turnDuration;
    }

    public float ShuffleAnimation()
    {
        SetRandomValue(shuffleValue);
        ChangeAnimation(shuffle);
        return shuffleDuration;
    }

    public float SitStandAnimation(bool sitDown)
    {
        if (sitDown) ChangeAnimation(sit);
        else ChangeAnimation(stand);
        return sitStandDuration;
    }
    public float EatAnimation()
    {
        ChangeAnimation(eat);
        return eatDuration;
    }
    public float AttackAnimation()
    {
        ChangeAnimation(attack);
        return attackDuration;
    }
    public float GotHitAnimation()
    {
        ChangeAnimation(gotHit);
        return gotHitDuration;
    }
}
