using UnityEngine;

public class ButtonController : AnimationController
{
    private ButtonListController listController = null;
    private int listControllerIndex = -1;

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
        if (listController == null) {
            AudioManager.Instance.UiOpen();
            PressAnimation(); }
        else { listController.PressButton(listControllerIndex); }
    }

}
