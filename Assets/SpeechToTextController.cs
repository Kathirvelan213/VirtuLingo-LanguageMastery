using UnityEngine;
using System;
using System.Collections.Generic;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using Assets;
using UnityEngine.UIElements;




public class SpeechToTextController : MonoBehaviour
{
    private string subscriptionKey = "AmnfYu09JxX8C50Tl0YRX5jSE06PLMeDydjIt53CDkGlC8ICiehTJQQJ99BBACGhslBXJ3w3AAAYACOGr4Wj";
    private string region = "centralindia";


    private SpeechRecognizer recognizer;
    private PushAudioInputStream pushStream;
    private bool isRecognizing = false;

    private AudioClip micClip;
    private string microphoneDevice;
    private const int sampleRate = 16000;
    private float silenceThreshold = 0.02f;
    private float silenceDuration = 2.0f;
    private float lastSpokenTime;

    private int lastSamplePosition = 0;
    private bool isTranslating = false;


    private void Start()
    {
        
    }
    public void StartRecord(DialogueList RecentDialogue, Func<string, Task> callback1, Func<string, Task> callback2)
    {
        InitializeSpeechRecognizer(RecentDialogue, callback1,callback2);
        StartMicrophone();
    }

    private void InitializeSpeechRecognizer(DialogueList RecentDialogue, Func<string, Task> callback1, Func<string, Task> callback2)
    {
        try
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            config.SpeechRecognitionLanguage = "en-IN";
            pushStream = AudioInputStream.CreatePushStream();

            var audioConfig = AudioConfig.FromStreamInput(pushStream);

            recognizer = new SpeechRecognizer(config, audioConfig);

            recognizer.Recognized += async (s, e) =>
            {
                if (e.Result.Reason == ResultReason.RecognizedSpeech)
                {
                    
                    isTranslating = true;
                    RecentDialogue.Push("Customer: " + e.Result.Text);
                    await callback1(e.Result.Text);
                    await callback2(e.Result.Text);
                    isTranslating = false;
                    Debug.Log($"Recognized: {e.Result.Text}");
                }
            };


            recognizer.Canceled += (s, e) =>
            {
                Debug.LogError($"Recognition Cancelled: {e.Reason}");
            };

            recognizer.SessionStopped += (s, e) =>
            {
                Debug.Log("Session Stopped.");
                isRecognizing = false;
            };
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    private void StartMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            microphoneDevice = Microphone.devices[0];
            micClip = Microphone.Start(microphoneDevice, true, 10, sampleRate);
        }
        else
        {
            Debug.LogError("No microphone detected!");
        }
    }

    void Update()
    {
        try
        {

            if (!isTranslating && Microphone.IsRecording(microphoneDevice))
            {
                int micPosition = Microphone.GetPosition(microphoneDevice);

                if (micPosition < lastSamplePosition)
                {
                    lastSamplePosition = 0;
                }

                int sampleCount = micPosition - lastSamplePosition;

                if (sampleCount > 0)
                {
                    float[] samples = new float[sampleCount];
                    micClip.GetData(samples, lastSamplePosition);

                    float volume = CalculateVolume(samples);

                    if (volume > silenceThreshold)
                    {
                        lastSpokenTime = Time.time;
                        if (!isRecognizing)
                        {
                            StartRecognition();
                        }
                        byte[] audioData = ConvertAudioToByteArray(samples);
                        pushStream.Write(audioData);
                    }
                    else if (isRecognizing && (Time.time - lastSpokenTime > silenceDuration))
                    {
                        StopRecognition();
                    }
                    lastSamplePosition = micPosition;
                }

            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private float CalculateVolume(float[] samples)
    {
        float sum = 0f;
        foreach (var sample in samples)
        {
            sum += sample * sample;
        }
        return Mathf.Sqrt(sum / samples.Length);
    }

    private byte[] ConvertAudioToByteArray(float[] samples)
    {
        byte[] byteArray = new byte[samples.Length * 2];
        for (int i = 0; i < samples.Length; i++)
        {
            short sample = (short)(samples[i] * short.MaxValue);
            byteArray[i * 2] = (byte)(sample & 0xFF);
            byteArray[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
        }
        return byteArray;
    }

    private async void StartRecognition()
    {
        isRecognizing = true;
        Debug.Log("Speech detected! Starting Azure recognition...");
        await recognizer.StartContinuousRecognitionAsync();
    }

    private async void StopRecognition()
    {
        isRecognizing = false;
        Debug.Log("Silence detected. Stopping Azure recognition...");
        await recognizer.StopContinuousRecognitionAsync();
    }



    //private string subscriptionKey = "AmnfYu09JxX8C50Tl0YRX5jSE06PLMeDydjIt53CDkGlC8ICiehTJQQJ99BBACGhslBXJ3w3AAAYACOGr4Wj";
    //private string region = "centralindia";
    //float silenceTimeThreshold = 2.5f;
    //float silenceLevelThreshold = 0.0005f;


    //public void StartRecord(System.Action<string> callback)
    //{
    //    StartCoroutine(RecordAndSendAudio(callback));
    //}

    //IEnumerator RecordAndSendAudio(System.Action<string> callback)
    //{
    //    // Start recording audio from microphone
    //    int maxTime = 30;
    //    bool isRecording = false;
    //    float silenceTimer = 0f;
    //    AudioClip audioClip = Microphone.Start(null, false, maxTime, 16000);
    //    //yield return new WaitForSeconds(maxTime);
    //    while (!isRecording)
    //    {
    //        if (!isSilent(audioClip))
    //        {
    //            isRecording = true;
    //        }
    //    }
    //    while (isRecording)
    //    {
    //        yield return null;
    //        if (isSilent(audioClip))
    //        {
    //            silenceTimer += Time.deltaTime;
    //            if (silenceTimer >= silenceTimeThreshold) 
    //            {
    //                isRecording = false;
    //                Microphone.End(null);
    //                Debug.Log("stopped recording");
    //            }
    //        }
    //        else
    //        {
    //            silenceTimer = 0f;
    //        }
    //    }

    //    byte[] wavData = WavUtility.FromAudioClip(audioClip);
    //    yield return StartCoroutine(SendToAzureSpeechAPI(wavData, callback));
    //}
    //bool isSilent(AudioClip clip)
    //{
    //    int sampleWindow = 1024; // Check a small sample window
    //    float[] samples = new float[sampleWindow];
    //    int micPosition = Microphone.GetPosition(null) - sampleWindow;

    //    if (micPosition < 0) return false;

    //    clip.GetData(samples, micPosition); // Get the latest audio chunk

    //    float sum = 0f;
    //    for (int i = 0; i < samples.Length; i++)
    //    {
    //        sum += Mathf.Abs(samples[i]);
    //    }

    //    float averageVolume = sum / samples.Length;
    //    return averageVolume < silenceLevelThreshold; // Silence if below threshold
    //}

    //IEnumerator SendToAzureSpeechAPI(byte[] audioData, System.Action<string> callback)
    //{
    //    string url = $"https://{region}.stt.speech.microsoft.com/speech/recognition/conversation/cognitiveservices/v1?language=en-US";

    //    UnityWebRequest request = new UnityWebRequest(url, "POST");
    //    request.uploadHandler = new UploadHandlerRaw(audioData);
    //    request.downloadHandler = new DownloadHandlerBuffer();

    //    request.SetRequestHeader("Content-Type", "audio/wav");
    //    request.SetRequestHeader("Ocp-Apim-Subscription-Key", subscriptionKey);

    //    yield return request.SendWebRequest();

    //    if (request.result == UnityWebRequest.Result.Success)
    //    {
    //        var jsonReply = JObject.Parse(request.downloadHandler.text);
    //        string message = jsonReply["DisplayText"].ToString();
    //        Debug.Log("Response: " + message);
    //        callback(message);
    //    }
    //    else
    //    {
    //        Debug.LogError("Error: " + request.error);
    //    }
    //}   
}
