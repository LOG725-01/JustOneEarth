using TMPro;
using UnityEngine;

public class Chat : MonoBehaviour
{
    public static Chat Instance;

    [SerializeField] private TextMeshProUGUI chatLog;
    [SerializeField] private TMP_InputField chatInput;
    private string chatText;
    
    private void Awake()
    {
        if (Instance != null) { if (Instance != this) Destroy(this); }
        else Instance = this;
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

    private void AddLine(string line)
    {
        chatText += line + "\n";
        chatLog.text = chatText;
    }

    public void AddChatMessage(string userName, string message)
    {
        AddLine(userName + " : " + message);
    }

    public void AddAnnoucement(string userName, string annoucemen)
    {
        AddLine(userName + " " + annoucemen);
    }

    // @brief Event for the input field
    public void AddChatMessage(string message)
    {
        //TODO : get player username
        AddChatMessage("player0", message);
        chatInput.text = "";
    }
}
