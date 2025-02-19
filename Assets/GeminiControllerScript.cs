using System;
using System.Collections;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
//using System.Text.Json;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


public class GeminiControllerScript : MonoBehaviour
{
    public string message;
    public SpeechServiceControllerScript speechServiceController;

    private string geminiEndPoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=AIzaSyBu8JgA0G9zv5gXcXNHUrrAIy11WKWEGfY";

    void Start()
    {
        StartCoroutine(GetReply(message));
    }

    IEnumerator GetReply(string message)
    {
        string jsonMessage = $"{{\"contents\": [{{ \"parts\": [ {{  \"text\":\"{message}\" }}]}} ]}}";

        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonMessage);
        UnityWebRequest request = new UnityWebRequest(geminiEndPoint,"POST");
        request.uploadHandler=new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
            string strReply = request.downloadHandler.text;
            var jsonReply = JObject.Parse(strReply);
            string replyMessage = jsonReply["candidates"][0]["content"]["parts"][0]["text"].ToString();
            speechServiceController.Speak(replyMessage);

        }
        else
        {
            Debug.Log(request.error);
        }
    }    
}
