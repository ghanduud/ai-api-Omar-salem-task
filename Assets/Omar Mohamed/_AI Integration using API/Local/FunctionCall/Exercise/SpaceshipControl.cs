using System.Collections.Generic;
using System.Reflection;
using LLMUnity;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Whisper;
using Whisper.Utils;

public class SpaceshipControl : MonoBehaviour
{
    public LLMCharacter llmCharacter;
    public Spaceship spaceship;
    public MicrophoneRecord microphoneRecord;
    public WhisperManager whisper;
    public Button recordButton;
    public TMP_Text recordButtonText;
    public TMP_Text playerText;
    public TMP_Text responseText; // NEW: LLM response text

    void Start()
    {
        // Force Whisper to process only English
        whisper.language = "en"; 
        whisper.translateToEnglish = false; // No need to translate since it's already English

        // Assign button function
        recordButton.onClick.AddListener(OnRecordButtonPressed);

        // Subscribe to Whisper transcription event
        microphoneRecord.OnRecordStop += OnRecordStop;
    }

    void OnRecordButtonPressed()
    {
        if (!microphoneRecord.IsRecording)
        {
            microphoneRecord.StartRecord();
            recordButtonText.text = "Stop Recording";
        }
        else
        {
            microphoneRecord.StopRecord();
            recordButtonText.text = "Start Recording";
        }
    }

    async void OnRecordStop(AudioChunk recordedAudio)
    {
        Debug.Log("Start processing...");
        playerText.text = "Processing voice...";

        // Send recorded audio to Whisper (English only)
        var result = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
        if (result == null || result.Language != "en") 
        {
            playerText.text = "Only English is supported.";
            return;
        }

        string transcribedText = result.Result;
        playerText.text = transcribedText;

        // Send transcription to LLM
        string actionFunctionName = await llmCharacter.Chat(ConstructPrompt<ActionFunctions>(transcribedText));
        string action = (string)typeof(ActionFunctions).GetMethod(actionFunctionName)?.Invoke(null, null);

        // Execute action
        PerformAction(action);

        // Get LLM's response (character interaction)
        string llmResponse = await llmCharacter.Chat(transcribedText);
        responseText.text = llmResponse; // NEW: Display LLM response
    }

    string[] GetFunctionNames<T>()
    {
        List<string> functionNames = new List<string>();
        foreach (var function in typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)) 
            functionNames.Add(function.Name);
        return functionNames.ToArray();
    }

    string ConstructPrompt<T>(string message)
    {
        string prompt = "Which of the following choices best matches the input?\n\n";
        prompt += "Input: " + message + "\n\n";
        prompt += "Choices:\n";
        foreach (string functionName in GetFunctionNames<T>()) 
            prompt += $"- {functionName}\n";
        prompt += "\nAnswer directly with the choice";
        return prompt;
    }

    void PerformAction(string action)
    {
        if (action == "MoveRight")
        {
            spaceship.MoveRight();
        }
        else if (action == "MoveLeft")
        {
            spaceship.MoveLeft();
        }
        else if (action == "Shoot")
        {
            spaceship.Shoot();
        }
        else if (action == "ShieldOn")
        {
            spaceship.ActivateShield();
        }
        else if (action == "ShieldOff")
        {
            spaceship.DeactivateShield();
        }
        else if (action == "Heal")
        {
            spaceship.Heal(40);
        }
    }
}
