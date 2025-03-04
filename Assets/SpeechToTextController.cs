﻿using UnityEngine;
using System;
using System.Collections.Generic;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Threading.Tasks;
using Assets;
using UnityEngine.UIElements;
using System.IO;
using System.Linq;
using System.Collections;




public class SpeechToTextController : MonoBehaviour
{
    private string subscriptionKey = "AmnfYu09JxX8C50Tl0YRX5jSE06PLMeDydjIt53CDkGlC8ICiehTJQQJ99BBACGhslBXJ3w3AAAYACOGr4Wj";
    private string region = "centralindia";

    //public float silenceThreshold = 0.02f; // Adjust this value based on your microphone
    //public float silenceDuration = 2.0f;   // Stop recording after 2 seconds of silence
    //public int sampleRate = 16000;

    //private SpeechRecognizer recognizer;
    //private AudioClip micClip;
    //private string microphoneDevice;
    //private bool isRecording = false;
    //private bool isProcessing = false;
    //private float lastSpokenTime;
    //private int lastSamplePosition = 0;
    //private List<float> audioBuffer = new List<float>(); // Buffer for audio data

    //public void StartRecord(DialogueList RecentDialogue, Func<string, Task> callback1, Func<string, Task> callback2)
    //{
    //    StartMicrophone();
    //}

    //private void StartMicrophone()
    //{
    //    if (Microphone.devices.Length > 0)
    //    {
    //        microphoneDevice = Microphone.devices[0];
    //        micClip = Microphone.Start(microphoneDevice, true, 10, sampleRate);
    //    }
    //    else
    //    {
    //        Debug.LogError("No microphone detected!");
    //    }
    //}

    //void Update()
    //{
    //    if (Microphone.IsRecording(microphoneDevice) && !isProcessing)
    //    {
    //        int micPosition = Microphone.GetPosition(microphoneDevice);
    //        if (micPosition < lastSamplePosition) lastSamplePosition = 0;
    //        int sampleCount = micPosition - lastSamplePosition;

    //        if (sampleCount > 0)
    //        {
    //            float[] samples = new float[sampleCount];
    //            micClip.GetData(samples, lastSamplePosition);

    //            float volume = CalculateVolume(samples);

    //            if (volume > silenceThreshold)
    //            {
    //                lastSpokenTime = Time.time;
    //                isRecording = true;
    //                audioBuffer.AddRange(samples);
    //            }
    //            else if (isRecording && (Time.time - lastSpokenTime > silenceDuration))
    //            {
    //                isRecording = false;
    //                StartCoroutine(ProcessAudio());
    //            }

    //            lastSamplePosition = micPosition;
    //        }
    //    }
    //}

    //private float CalculateVolume(float[] samples)
    //{
    //    float sum = samples.Sum(sample => sample * sample);
    //    return Mathf.Sqrt(sum / samples.Length);
    //}

    //private IEnumerator ProcessAudio()
    //{
    //    if (audioBuffer.Count == 0) yield break;

    //    isProcessing = true;

    //    // Convert buffered audio to byte array
    //    byte[] audioData = ConvertAudioToByteArray(audioBuffer.ToArray());

    //    // Send to Azure Speech
    //    yield return StartCoroutine(SendToAzureSpeech(audioData));

    //    // Clear buffer
    //    audioBuffer.Clear();
    //    isProcessing = false;
    //}

    //private byte[] ConvertAudioToByteArray(float[] samples)
    //{
    //    byte[] byteArray = new byte[samples.Length * 2];
    //    for (int i = 0; i < samples.Length; i++)
    //    {
    //        short sample = (short)(samples[i] * short.MaxValue);
    //        byteArray[i * 2] = (byte)(sample & 0xFF);
    //        byteArray[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
    //    }
    //    return byteArray;
    //}

    //private IEnumerator SendToAzureSpeech(byte[] audioData)
    //{
    //    var config = SpeechConfig.FromSubscription(subscriptionKey, region);
    //    using var pushStream = AudioInputStream.CreatePushStream();
    //    using var audioConfig = AudioConfig.FromStreamInput(pushStream);
    //    using var speechRecognizer = new SpeechRecognizer(config, audioConfig);

    //    // Write full audio data at once
    //    pushStream.Write(audioData);
    //    pushStream.Close();

    //    Task<SpeechRecognitionResult> recognitionTask = speechRecognizer.RecognizeOnceAsync();
    //    while (!recognitionTask.IsCompleted) yield return null;

    //    if (recognitionTask.Result != null && recognitionTask.Result.Reason == ResultReason.RecognizedSpeech)
    //    {
    //        Debug.Log($"Recognized Speech: {recognitionTask.Result.Text}");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("No speech recognized.");
    //    }
    //}

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
        InitializeSpeechRecognizer(RecentDialogue, callback1, callback2);
        StartMicrophone();
    }

    private void InitializeSpeechRecognizer(DialogueList RecentDialogue, Func<string, Task> callback1, Func<string, Task> callback2)
    {
        try
        {
            var config = SpeechConfig.FromSubscription(subscriptionKey, region);
            config.SpeechRecognitionLanguage = "en-uk";
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



}
