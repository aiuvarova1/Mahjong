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

    public void Start()
    {

        scrollRect = chatScroll.GetComponent<ScrollRect>();
    }

    public void AddMessage()
    {

       
        string message = enteredText.text;
        Debug.Log(message);

        Text textModel = Instantiate(chatTextPrefab);

       
        textModel.transform.SetParent(content.transform,false);
        Debug.Log(textModel.transform.position);
        Debug.Log(textModel.transform.parent);

        textModel.text = $"{LocalizationManager.instance.GetLocalizedValue("You: ")} {message}";
        textModel.transform.localScale = new Vector3(1, 1, 1);
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(content.GetComponent<RectTransform>());

        enteredText.text = "";
        scrollRect.velocity = new Vector2(0f, 1000f);

        enteredText.ActivateInputField();
        enteredText.Select();

        CmdSendMessage(gameObject.GetComponent<Player>().name, message);
    }

    [Command]
    void CmdSendMessage(string name, string message)
    {
        RpcSendMessage(name, message);
    }

    [ClientRpc]
    void RpcSendMessage(string name, string message)
    {
        if (isLocalPlayer) return;
        Text textModel = Instantiate(chatTextPrefab);

        textModel.transform.SetParent(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerChat>().content.transform);
        textModel.transform.localScale = new Vector3(1, 1, 1);

        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerChat>().content.GetComponent<RectTransform>());

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerChat>().scrollRect.velocity= new Vector2(0f, 1000f);

        textModel.text = $"{name}:  {message}";
    }

    public void ClearChat()
    {
        Debug.Log("clear");
        GameObject[] messages = GameObject.FindGameObjectsWithTag("Message");
        for (int i = 0; i < messages.Length; i++)
        {
            Destroy(messages[i]);
        }
    }

    public void IncreaseImage(GameObject image)
    {
        image.GetComponent<RectTransform>().localScale = new Vector3(1.1f, 1.1f);
    }

    public void DecreaseImage(GameObject image)
    {
        image.GetComponent<RectTransform>().localScale = new Vector3(1, 1f);
    }

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
