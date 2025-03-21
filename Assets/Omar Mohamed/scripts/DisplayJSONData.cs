using UnityEngine;
using TMPro;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;
using System.Collections;

public class DisplayJSONData : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI displayText;
    
    private Dictionary<string, object> jsonData;
    
    // Fixed path for the JSON file in the Assets folder
    private string jsonFilePath;

    void Start()
    {
        // Set the fixed path to the JSON file in Assets folder
        jsonFilePath = Application.dataPath + "/JSONData.json";
        LoadJsonData();
    }

    private void LoadJsonData()
    {
        if (File.Exists(jsonFilePath))
        {
            string jsonContent = File.ReadAllText(jsonFilePath);
            jsonData = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonContent);
            Debug.Log("JSON file loaded successfully from: " + jsonFilePath);
        }
        else
        {
            Debug.LogError("JSON file not found at path: " + jsonFilePath);
        }
    }

    // Function to display all JSON data
    public void DisplayAllJsonData()
    {
        if (jsonData == null)
        {
            displayText.text = "No JSON data loaded";
            return;
        }

        string displayContent = "";
        foreach (var item in jsonData)
        {
            displayContent += $"{item.Key}: {item.Value}\n";
        }
        
        displayText.text = displayContent;
    }

    // Function to display value by key
    public void DisplayValueByKey()
    {
        if (jsonData == null)
        {
            displayText.text = "No JSON data loaded";
            return;
        }

        try
        {
            // Get the user object and convert it to a dictionary
            var userObject = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonData["user"].ToString());
            string firstName = userObject["firstName"].ToString();
            displayText.text = $"First Name: {firstName}";
        }
        catch (Exception e)
        {
            displayText.text = "Error reading firstName from JSON";
            Debug.LogError($"Error reading firstName: {e.Message}");
        }
    }




private string apiUrl = "https://api-inference.huggingface.co/models/google/gemma-2-2b-it";
private string apiKey = "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"; // Replace with your actual Hugging Face API key

private string fixedMessage = "Hello AI, how are you?"; // Fixed message

public void SendRequestToAPI()
{
    StartCoroutine(SendRequestCoroutine());
}

private IEnumerator SendRequestCoroutine()
{
    // Create the JSON payload with the fixed message
    string jsonPayload = JsonConvert.SerializeObject(new { inputs = fixedMessage });

    using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
    {
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonPayload);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer(); 
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

        // Send the request
        yield return request.SendWebRequest();

        // Handle the response
        if (request.result == UnityWebRequest.Result.Success)
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("Response: " + responseText);

            // Parse JSON response and extract the generated text
            try
            {
                var responseObject = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(responseText);
                if (responseObject != null && responseObject.Count > 0 && responseObject[0].ContainsKey("generated_text"))
                {
                    string generatedText = responseObject[0]["generated_text"];
                    displayText.text = generatedText;
                }
                else
                {
                    displayText.text = "No generated text found.";
                }
            }
            catch (System.Exception e)
            {
                displayText.text = "Error parsing response.";
                Debug.LogError("Error parsing API response: " + e.Message);
            }
        }
        else
        {
            displayText.text = "Error: " + request.error;
            Debug.LogError("API Request Error: " + request.error);
        }
    }
}
}