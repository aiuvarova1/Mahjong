using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerChat : NetworkBehaviour
{
    public Text chatTextPrefab;
    ScrollRect scrollRect;

    public GameObject chatScroll;

    public InputField enteredText;
    public GameObject content;

    //sets references
    public void Start()
    {
        scrollRect = chatScroll.GetComponent<ScrollRect>();
    }

    //adds message on this client
    public void AddMessage()
    {
        string message = enteredText.text;

        Text textModel = Instantiate(chatTextPrefab);
        textModel.transform.SetParent(content.transform, false);

        textModel.text = $"{LocalizationManager.instance.GetLocalizedValue("You: ")} {message}";
        textModel.transform.localScale = new Vector3(1, 1, 1);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        enteredText.text = "";
        scrollRect.velocity = new Vector2(0f, 1000f);

        enteredText.ActivateInputField();
        enteredText.Select();

        CmdSendMessage(gameObject.GetComponent<Player>().name, message);
    }

    //commands to send message
    [Command]
    void CmdSendMessage(string name, string message)
    {
        RpcSendMessage(name, message);
    }

    //adds message on other clients
    [ClientRpc]
    void RpcSendMessage(string name, string message)
    {
        if (isLocalPlayer) return;
        Text textModel = Instantiate(chatTextPrefab);

        textModel.transform.SetParent(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerChat>().content.transform);
        textModel.transform.localScale = new Vector3(1, 1, 1);

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerChat>().content.GetComponent<RectTransform>());

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerChat>().scrollRect.velocity = new Vector2(0f, 1000f);
        textModel.text = $"{name}:  {message}";
    }

    //clears all messages
    public void ClearChat()
    {
        GameObject[] messages = GameObject.FindGameObjectsWithTag("Message");
        for (int i = 0; i < messages.Length; i++)
        {
            Destroy(messages[i]);
        }
    }

    //increases chat images' sizes
    public void IncreaseImage(GameObject image)
    {
        image.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f);
    }

    //decreases chat images' sizes
    public void DecreaseImage(GameObject image)
    {
        image.GetComponent<RectTransform>().localScale = new Vector3(1, 1f);
    }

    //sends message on pressing "enter"
    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKey(KeyCode.Return))
        {
            enteredText.ActivateInputField();

            if (enteredText.isFocused && enteredText.text != "")
                AddMessage();
        }
    }

}
