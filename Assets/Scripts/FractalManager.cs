using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // Requires Input System package
using System.IO;

public class FractalManager : MonoBehaviour
{
    [Header("Rendering")]
    [Tooltip("Assign the GameObject with the Renderer using the StereoSplit shader.")]
    public Renderer targetRenderer;

    [Header("Input Actions")]
    [Tooltip("Action for 'Next Image' (e.g. Right Controller 'A' or Primary Button)")]
    public InputActionReference nextAction;
    [Tooltip("Action for 'Previous Image' (e.g. Right Controller 'B' or Secondary Button)")]
    public InputActionReference previousAction;

    private List<string> fractalFiles = new List<string>();
    private int currentIndex = 0;
    private Texture2D currentTexture;

    // Path on Quest local storage
    private string fractalPath = "/sdcard/Pictures/Fractals";

    void OnEnable()
    {
        if (nextAction != null) nextAction.action.Enable();
        if (previousAction != null) previousAction.action.Enable();
    }

    void OnDisable()
    {
        if (nextAction != null) nextAction.action.Disable();
        if (previousAction != null) previousAction.action.Disable();
    }

    void Start()
    {
        // Request permissions on Android (Quest)
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageRead))
        {
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageRead);
        }
        #endif

        StartCoroutine(InitSequence());
    }

    IEnumerator InitSequence()
    {
        yield return new WaitForSeconds(0.5f);

        ScanForFractals();

        if (fractalFiles.Count > 0)
        {
            LoadFractal(currentIndex);
        }
        else
        {
            Debug.LogWarning("No fractal images found in " + fractalPath);
        }
    }

    void Update()
    {
        // Check Input Actions
        if (nextAction != null && nextAction.action.WasPerformedThisFrame())
        {
            NextImage();
        }

        if (previousAction != null && previousAction.action.WasPerformedThisFrame())
        {
            PreviousImage();
        }

        // Keyboard Fallback for Editor testing (using Input System directly if configured, or legacy if both enabled)
        #if UNITY_EDITOR
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rightArrowKey.wasPressedThisFrame || Keyboard.current.aKey.wasPressedThisFrame)
            {
                NextImage();
            }
            if (Keyboard.current.leftArrowKey.wasPressedThisFrame || Keyboard.current.bKey.wasPressedThisFrame)
            {
                PreviousImage();
            }
        }
        #endif
    }

    void ScanForFractals()
    {
        if (Directory.Exists(fractalPath))
        {
            try
            {
                string[] jpgs = Directory.GetFiles(fractalPath, "*.jpg");
                string[] pngs = Directory.GetFiles(fractalPath, "*.png");

                fractalFiles.AddRange(jpgs);
                fractalFiles.AddRange(pngs);

                Debug.Log($"Found {fractalFiles.Count} fractals in {fractalPath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error scanning for fractals: {e.Message}");
            }
        }
        else
        {
            Debug.LogWarning("Fractal directory not found: " + fractalPath);
        }
    }

    public void NextImage()
    {
        if (fractalFiles.Count == 0) return;

        currentIndex = (currentIndex + 1) % fractalFiles.Count;
        LoadFractal(currentIndex);
    }

    public void PreviousImage()
    {
        if (fractalFiles.Count == 0) return;

        currentIndex--;
        if (currentIndex < 0) currentIndex = fractalFiles.Count - 1;
        LoadFractal(currentIndex);
    }

    void LoadFractal(int index)
    {
        if (index < 0 || index >= fractalFiles.Count) return;

        string filePath = fractalFiles[index];

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);

            if (currentTexture != null)
            {
                Destroy(currentTexture);
            }

            // Create a new Texture2D with mipmaps enabled
            currentTexture = new Texture2D(2, 2, TextureFormat.RGBA32, true);
            currentTexture.wrapMode = TextureWrapMode.Clamp;

            if (currentTexture.LoadImage(fileData))
            {
                currentTexture.Apply(true, true);

                if (targetRenderer != null)
                {
                    targetRenderer.material.mainTexture = currentTexture;
                }
            }
        }
    }
}
