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
using System.Net.Http;
using UnityEditor.PackageManager;
using System.Threading.Tasks;
using Unity.Android.Gradle;
using System.IO;
using UnityEditor.PackageManager.Requests;




public class GeminiControllerScript : MonoBehaviour
{
    //public string message;
    //public TextToSpeechController textToSpeechController;
    //public SpeechToTextController speechToTextController;

    private string geminiEndPoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?alt=sse";
    private string apiKey = "AIzaSyBu8JgA0G9zv5gXcXNHUrrAIy11WKWEGfY";
    private HttpClient httpClient = new HttpClient();
    async void Start()
    {
        httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
        //await GetResponse("hi", (string x) => Debug.Log("kat"));
        Debug.Log("hi");
    }

    public async Task GetResponse(string message,Func<string,Task> callback)
    {
        await GetReply(message,callback);
    }

    private async Task GetReply(string message, Func<string,Task> callback)
    {
        try
        {

        string requestBody = $"{{\"contents\": [{{ \"parts\": [ {{  \"text\":\"{message}\" }}]}} ]}}";
        var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

        var requestMessage=new HttpRequestMessage(HttpMethod.Post, geminiEndPoint) { Content = content };

        var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead);
        var stream = await response.Content.ReadAsStreamAsync();
        var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            string line = await reader.ReadLineAsync();
            if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("data:"))
            {
                string strReply = line.Substring(5).Trim();
                var jsonResponse = JObject.Parse(strReply);
                string replyMessage = jsonResponse["candidates"][0]["content"]["parts"][0]["text"].ToString();
                await callback(replyMessage);
                Debug.Log($"Reply: {replyMessage}");
            }
        }

        }
        catch (Exception ex) {Debug.Log(ex.ToString());}    
    }    
}
