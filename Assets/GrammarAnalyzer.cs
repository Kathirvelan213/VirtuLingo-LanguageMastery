using Assets;
using Newtonsoft.Json.Linq;
using System.IO;
using System;
using System.Net.Http;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;
using TMPro;
using PimDeWitte.UnityMainThreadDispatcher;


public class GrammarAnalyzer : MonoBehaviour
{
    
    private string geminiEndPoint = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.0-flash:generateContent?alt=sse";
    private string apiKey = "AIzaSyBu8JgA0G9zv5gXcXNHUrrAIy11WKWEGfY";
    private HttpClient httpClient = new HttpClient();
    public TextMeshProUGUI grammarCorrectedText;
    public TextMeshProUGUI dialogueSpoken;
    private string promptInstructions = "Give only the grammar corrected sentence of the following sentence:";
    void Start()
    {
        httpClient.DefaultRequestHeaders.Add("X-Goog-Api-Key", apiKey);
    }


    public async Task GrammarCorrect(string message)
    {
        try
        {
            print("here");

            string requestBody = $"{{\"contents\": [{{ \"parts\": [ {{  \"text\":\"{promptInstructions+message }\" }}]}} ]}}";
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
                        UnityMainThreadDispatcher.Instance().Enqueue(() =>
                        {
                            dialogueSpoken.text ="You: "+ message;
                            grammarCorrectedText.text ="Corrected: " +replyMessage;
                        });

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
