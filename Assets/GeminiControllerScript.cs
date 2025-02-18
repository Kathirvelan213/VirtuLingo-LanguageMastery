using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GeminiControllerScript : MonoBehaviour
{
    private string baseEndPoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?key=AIzaSyBu8JgA0G9zv5gXcXNHUrrAIy11WKWEGfY";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Talk("hi"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Talk(string message)
    {
        string jsonMessage = $"{{\"contents\": [{{ \"parts\": [ {{  \"text\":\"{message}\" }}]}} ]}}";
        Debug.Log(jsonMessage);
        byte[] jsonBytes = Encoding.UTF8.GetBytes(jsonMessage);
        UnityWebRequest request = new UnityWebRequest(baseEndPoint,"POST");
        request.uploadHandler=new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(request.downloadHandler.text);
        }
        else
        {
            Debug.Log(request.error);
        }

    }
}
