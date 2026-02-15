using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // TextMeshPro is standard in Unity 2021+

public class DebugText : MonoBehaviour
{
    public static DebugText Instance;
    public TextMeshPro textMesh;
    private List<string> logs = new List<string>();
    private const int maxLines = 10;

    void Awake()
    {
        Instance = this;
        if (textMesh == null)
            textMesh = GetComponent<TextMeshPro>();
    }

    public static void Log(string message)
    {
        if (Instance != null)
        {
            Instance.AddLog(message);
        }
        Debug.Log(message);
    }

    private void AddLog(string message)
    {
        logs.Add(message);
        if (logs.Count > maxLines)
        {
            logs.RemoveAt(0);
        }
        UpdateText();
    }

    private void UpdateText()
    {
        if (textMesh != null)
        {
            textMesh.text = string.Join("\n", logs);
        }
    }
}
