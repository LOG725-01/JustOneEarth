using UnityEngine;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private ButtonListController listController = null;
    private int listControllerIndex = -1;
    private string currentAnimation = "normal";

    private void ChangeAnimation(string animation, float crossfade = 0.2f)
    {
        currentAnimation = animation;
        animator.CrossFade(animation, crossfade);
    }

    public void SetListController(ButtonListController _listController, int index)
    {
        listController = _listController;
        listControllerIndex = index;
    }

    public void HighlightAnimation()
    {
        if (currentAnimation == "Selected") return;
        ChangeAnimation("Highlighted");
    }

    public void NormalAnimation(bool force)
    {
        if (currentAnimation == "Selected" & !force) return;
        ChangeAnimation("Normal");
    }

    public void PressAnimation()
    {
        ChangeAnimation("Selected");
    }

    public void Select()
    {
        if (listController == null) { PressAnimation(); }
        else { listController.PressButton(listControllerIndex); }
    }

}
