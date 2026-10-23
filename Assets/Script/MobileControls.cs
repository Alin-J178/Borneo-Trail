using UnityEngine;

public class MobileControls : MonoBehaviour
{
    private bool leftHeld;
    private bool rightHeld;
    private bool jumpPressed;
    private bool emotePressed;

    public float MoveInput => (rightHeld ? 1f : 0f) - (leftHeld ? 1f : 0f);

    public void LeftDown()
    {
        leftHeld = true;
    }

    public void LeftUp()
    {
        leftHeld = false;
    }

    public void RightDown()
    {
        rightHeld = true;
    }

    public void RightUp()
    {
        rightHeld = false;
    }

    public void JumpButton()
    {
        jumpPressed = true;
    }

    public void EmoteButton()
    {
        emotePressed = true;
    }

    public bool ConsumeJumpPressed()
    {
        bool pressed = jumpPressed;
        jumpPressed = false;
        return pressed;
    }

    public bool ConsumeEmotePressed()
    {
        bool pressed = emotePressed;
        emotePressed = false;
        return pressed;
    }
}
