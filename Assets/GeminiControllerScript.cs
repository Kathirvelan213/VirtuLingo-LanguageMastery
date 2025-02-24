using System;
using System.Collections;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Assets;





public class GeminiControllerScript : MonoBehaviour
{
    //public string message;
    //public TextToSpeechController textToSpeechController;
    //public SpeechToTextController speechToTextController;

    private string geminiEndPoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?alt=sse";
    private string apiKey = "AIzaSyBu8JgA0G9zv5gXcXNHUrrAIy11WKWEGfY";
    private HttpClient httpClient = new HttpClient();
    void Start()
    {
        httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
        //await GetResponse("hi", (string x) => Debug.Log("kat"));
        Debug.Log("hi");
    }


    public async Task GetResponse(string message, string promptInstructions, DialogueList RecentDialogue, Func<string, Task> callback)
    {
        try
        {

            string strRecentDialogue = string.Join(";", RecentDialogue);
            Debug.Log($"{promptInstructions + strRecentDialogue}");
            string requestBody = $"{{\"contents\": [{{ \"parts\": [ {{  \"text\":\"{promptInstructions + strRecentDialogue}\" }}]}} ]}}";
            var content = new StringContent(requestBody, System.Text.Encoding.UTF8, "application/json");

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, geminiEndPoint) { Content = content };

            using (var response = await httpClient.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead))
            using (var stream = await response.Content.ReadAsStreamAsync())
            using (var reader = new StreamReader(stream))
            {


                while (!reader.EndOfStream)
                {
                    string line = await reader.ReadLineAsync();
                    if (!string.IsNullOrWhiteSpace(line) && line.StartsWith("data:"))
                    {
                        string strReply = line.Substring(5).Trim();
                        var jsonResponse = JObject.Parse(strReply);
                        string replyMessage = jsonResponse["candidates"][0]["content"]["parts"][0]["text"].ToString();
                        RecentDialogue.Push("Cashier:" + replyMessage);
                        await callback(replyMessage);
                        Debug.Log($"Reply: {replyMessage}");
                    }
                }
            }

        }
        catch (Exception ex)
        {
            Debug.Log(ex.ToString());
        }
    }
}
