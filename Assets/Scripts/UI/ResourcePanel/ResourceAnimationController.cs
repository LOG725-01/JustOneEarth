public class ResourceAnimationController : AnimationController
{
    public void Open()
    {
        ChangeAnimation("Opened");
    }

    public void Close()
    {
        ChangeAnimation("Closed");
    }
}
