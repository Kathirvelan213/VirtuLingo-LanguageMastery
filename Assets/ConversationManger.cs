using System;
using System.Collections;
using UnityEngine;

public class ConversationManger : MonoBehaviour
{
    public GeminiControllerScript geminiControllerScript;
    public TextToSpeechController textToSpeechController;
    public SpeechToTextController speechToTextController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SpeechLoop(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator SpeechLoop2()
    { int i=0;
        while (i < 10) 
        {
            yield return null;
            speechToTextController.StartRecord(
            callback:(string message)=>geminiControllerScript.GetResponse(message,
                callback:(string message)=>textToSpeechController.Speak(message)));
            i++;
        }
    }
    void SpeechLoop()
    {
        speechToTextController.StartRecord(
            callback: (string message) => geminiControllerScript.GetResponse(message,
                callback: (string message) => textToSpeechController.Speak(message)));
    }
}
