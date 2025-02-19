using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class SpeechServiceControllerScript : MonoBehaviour
{
    private const string SUBSCRIPTION_KEY = "AmnfYu09JxX8C50Tl0YRX5jSE06PLMeDydjIt53CDkGlC8ICiehTJQQJ99BBACGhslBXJ3w3AAAYACOGr4Wj";  // Replace with your key
    private const string REGION = "centralindia";
    private string speechServiceEndPoint = $"https://{REGION}.tts.speech.microsoft.com/cognitiveservices/v1";

    public AudioSource audioSource;

    public void Speak(string text)
    {
        StartCoroutine(GetAzureTTS(text));
    }

    IEnumerator GetAzureTTS(string text)
    {
        string xml = $@"
        <speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='en-US'>
            <voice name='en-US-JennyNeural'>{text}</voice>
        </speak>";

        byte[] xmlBytes = Encoding.UTF8.GetBytes(xml);

        using (UnityWebRequest request = new UnityWebRequest(speechServiceEndPoint, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(xmlBytes);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Ocp-Apim-Subscription-Key", SUBSCRIPTION_KEY);
            request.SetRequestHeader("Content-Type", "application/ssml+xml");
            request.SetRequestHeader("X-Microsoft-OutputFormat", "riff-16khz-16bit-mono-pcm");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                byte[] audioBytes = request.downloadHandler.data;
                PlayAudio(audioBytes);
            }
            else
            {
                Debug.LogError("Azure TTS Error: " + request.error);
            }
        }
    }

    void PlayAudio(byte[] audioData)
    {
        Debug.Log(audioData.ToString());
        AudioClip audioClip = WavUtility.ToAudioClip(audioData);
        audioSource.PlayOneShot(audioClip);
    }
}
