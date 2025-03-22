using System.Collections;
using System.IO;
using HuggingFace.API;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;

public class AIResponseManager : MonoBehaviour {
    [SerializeField] private Button startButton;
    [SerializeField] private Button stopButton;
    [SerializeField] private TextMeshProUGUI text;

    private AudioClip clip;
    private byte[] bytes;
    private bool recording;
    private string recognizedText;

    private void Start() {
        startButton.onClick.AddListener(StartRecording);
        stopButton.onClick.AddListener(StopRecording);
        stopButton.interactable = false;
    }

    private void Update() {
        if (recording && Microphone.GetPosition(null) >= clip.samples) {
            StopRecording();
        }
    }

    private void StartRecording() {
        text.color = Color.white;
        text.text = "Recording...";
        startButton.interactable = false;
        stopButton.interactable = true;
        clip = Microphone.Start(null, false, 10, 44100);
        recording = true;
    }

    private void StopRecording() {
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        SendRecording();
    }

    private void SendRecording() {
        text.color = Color.yellow;
        text.text = "Sending...";
        stopButton.interactable = false;
        HuggingFaceAPI.AutomaticSpeechRecognition(bytes, response => {
            text.color = Color.white;
            text.text = response;
            recognizedText = response; // Store recognized text
            SendRequestToAPI(recognizedText); // Send to Hugging Face API
            startButton.interactable = true;
        }, error => {
            text.color = Color.red;
            text.text = error;
            startButton.interactable = true;
        });
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels) {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2)) {
            using (var writer = new BinaryWriter(memoryStream)) {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples) {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }

    private string apiUrl = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";

    public void SendRequestToAPI(string inputText) {
        StartCoroutine(SendRequestCoroutine(inputText));
    }

    private IEnumerator SendRequestCoroutine(string inputText) {
        string jsonPayload = JsonConvert.SerializeObject(new { inputs = inputText });

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST")) {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer(); 
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success) {
                string responseText = request.downloadHandler.text;
                Debug.Log("Response: " + responseText);

                try {
                    var responseObject = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(responseText);
                    if (responseObject != null && responseObject.Count > 0 && responseObject[0].ContainsKey("generated_text")) {
                        string generatedText = responseObject[0]["generated_text"];
                        text.text = generatedText;
                    } else {
                        text.text = "No generated text found.";
                    }
                } catch (System.Exception e) {
                    text.text = "Error parsing response.";
                    Debug.LogError("Error parsing API response: " + e.Message);
                }
            } else {
                text.text = "Error: " + request.error;
                Debug.LogError("API Request Error: " + request.error);
            }
        }
    }
}
