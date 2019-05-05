using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PlayerChat : MonoBehaviour
{
    public Text chatTextPrefab;

    InputField enteredText;
    GameObject content;

    public void Start()
    {
        //if (isLocalPlayer)
        {
            enteredText = GameObject.FindObjectOfType<InputField>();
            content = GameObject.FindGameObjectWithTag("Content");
        }

    }

    public void AddMessage()
    {
        string message = enteredText.text;
        Debug.Log(message);

        Text textModel = Instantiate(chatTextPrefab);
        Debug.Log(textModel == null);
        Debug.Log(chatTextPrefab == null);
        Debug.Log(GameObject.FindGameObjectsWithTag("Message").Length);
        Debug.Log(content==null);
        textModel.transform.SetParent(content.transform);

        textModel.text = $"{LocalizationManager.instance.GetLocalizedValue("You: ")} {message}";
        textModel.transform.localScale = new Vector3(1, 1, 1);

        enteredText.text = "";

        //CmdSendMessage(gameObject.GetComponent<Player>().name, message);
    }

    //[Command]
    //void CmdSendMessage(string name,string message)
    //{
    //    RpcSendMessage(name,message);
    //}

    //[ClientRpc]
    //void RpcSendMessage(string name, string message)
    //{
    //    if (isLocalPlayer) return;
    //    Text textModel = Instantiate(chatTextPrefab);

    //    textModel.transform.SetParent(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerChat>().content.transform);
    //    textModel.transform.localScale = new Vector3(1, 1, 1);

    //    textModel.text = $"{name}:  {message}";
    //}

    public void ClearChat()
    {
        Debug.Log("clear");
        GameObject[] messages = GameObject.FindGameObjectsWithTag("Message");
        for (int i = 0; i < messages.Length; i++)
        {
            Destroy(messages[i]);
        }
    }

}
