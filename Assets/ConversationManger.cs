using Assets;
using System;
using UnityEngine;

public class ConversationManger : MonoBehaviour
{
    public GeminiControllerScript geminiControllerScript;
    public TextToSpeechController textToSpeechController;
    public SpeechToTextController speechToTextController;

    private DialogueList RecentDialogue=new DialogueList();
    
    private string promptInstructions= "you are a model being used to help people practice a new language. Pretend to be a cashier at a counter at a supermarket.You do not have to do any action, just speak. Here are some details you can use, and you dont have to say this unless asked . your name is Ravi, you are a 24 year old. give simple sentences, so that they can understand you, cause they are just learning the language. continue on with the following conversation";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RecentDialogue.Enqueue("Cashier: Hello, how may i help you?");
        textToSpeechController.Speak("Hello, how may i help you?");
        SpeechLoop(); 
    }

    void SpeechLoop()
    {
        speechToTextController.StartRecord(RecentDialogue,
            callback: (string message) => geminiControllerScript.GetResponse(message, promptInstructions, RecentDialogue,
                callback: (string message) => textToSpeechController.Speak(message)));
    }
}
