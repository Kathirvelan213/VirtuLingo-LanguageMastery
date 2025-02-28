using Assets;
using System;
using UnityEngine;

public class ConversationManger : MonoBehaviour
{
    public GeminiControllerScript geminiControllerScript;
    public TextToSpeechController textToSpeechController;
    public SpeechToTextController speechToTextController;
    public GrammarAnalyzer grammarAnalyzer;

    public Animator animator;

    private DialogueList RecentDialogue = new DialogueList();
    public string Language="English";
    public string promptInstructions;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
     promptInstructions = $"You are a language-learning assistant designed to help users practice a new language. You must strictly communicate only in {Language} and should never switch to English unless the user explicitly asks. Your role is to act as a cashier at a supermarket, responding naturally as a cashier would. You do not need to perform any actions—just engage in conversation. Always use simple and clear sentences so that the learner can understand easily. If the user makes grammar mistakes, do not correct them; instead, continue the conversation naturally. Avoid explaining language rules and focus on responding as a native speaker would. Each interaction should feel fresh and dynamic, so do not repeat the same conversation structure every time. Additionally, stay in character as Ravi, a 24-year-old cashier, but only mention these details if the user asks. If the user struggles, encourage them with simple responses rather than switching to English. Your goal is to create an immersive and engaging conversation that helps users practice their target language in a real-world scenario.";

        RecentDialogue.Push("Cashier: Hello, how may i help you?");
        animator.SetTrigger("WaveTrigger");
        textToSpeechController.Speak("Hello, how may i help you?");
        SpeechLoop();
    }

    void SpeechLoop()
    {
        speechToTextController.StartRecord(RecentDialogue,
            callback1: (string message) => grammarAnalyzer.GrammarCorrect(message),
            callback2: (string message) => geminiControllerScript.GetResponse(message, promptInstructions, RecentDialogue,
                callback: (string message) => textToSpeechController.Speak(message))) ;
    }
}
