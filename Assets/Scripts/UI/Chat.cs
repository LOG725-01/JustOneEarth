using TMPro;
using UnityEngine;

public class Chat : AnimationController
{
    public static Chat Instance;

    [SerializeField] private TextMeshProUGUI chatLog;
    [SerializeField] private TextMeshProUGUI lastLine;
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private TMP_InputField chatInput;
    private string chatText;
    private bool isOpened = false;
    private bool wasOpened = false;
    
    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
        Close();
    }

    private void Start()
    {
        Clean();
    }

    public void Clean()
    {
        chatText = "";
        chatLog.text = chatText;
        chatInput.text = "";
    }

    public void OpenClose()
    {
        if (isOpened) Close();
        else Open();
    }

    public void InputSelect()
    {
        wasOpened = isOpened;
        Open();
    }

    public void InputDeselect()
    {
        if (!wasOpened) Close();
    }

    private void Open()
    {
        isOpened = true;
        buttonText.text = "Close";
        ChangeAnimation("Opened");
    }


    private void Close()
    {
        isOpened = false;
        buttonText.text = "Open";
        ChangeAnimation("Closed");
    }

    private void AddLine(string line)
    {
        chatText += line + "\n";
        lastLine.text = line;
        chatLog.text = chatText;
    }

    public void AddChatMessage(string userName, string message)
    {
        AddLine(userName + " : " + message);
    }

    public void AddAnnouncement(string userName, string announcement)
    {
        AddLine(userName + " " + announcement);
    }

    // @brief Event for the input field
    public void AddChatMessage(string message)
    {
        //TODO : get player username
        AddChatMessage("player0", message);
        chatInput.text = "";
    }
}
