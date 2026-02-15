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

    // Store loaded textures for embedded resources, file paths for external
    private List<Texture2D> embeddedFractals = new List<Texture2D>();
    private List<string> externalFractalFiles = new List<string>();

    private int totalCount => embeddedFractals.Count + externalFractalFiles.Count;
    private int currentIndex = 0;

    private Texture2D currentActiveTexture; // The one currently displayed

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
        DebugText.Log("Initializing...");

        // Load Embedded Resources first
        LoadEmbeddedResources();

        // Request permissions on Android (Quest)
        #if UNITY_ANDROID && !UNITY_EDITOR
        if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.ExternalStorageRead))
        {
            DebugText.Log("Requesting Storage Permission...");
            UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.ExternalStorageRead);
        }
        #endif

        StartCoroutine(InitSequence());
    }

    void LoadEmbeddedResources()
    {
        // Load all Texture2D objects from Resources/Fractals folder
        Texture2D[] textures = Resources.LoadAll<Texture2D>("Fractals");
        if (textures != null && textures.Length > 0)
        {
            embeddedFractals.AddRange(textures);
            DebugText.Log($"Loaded {embeddedFractals.Count} embedded fractals.");
        }
        else
        {
            DebugText.Log("No embedded fractals found in Resources/Fractals.");
        }
    }

    IEnumerator InitSequence()
    {
        yield return new WaitForSeconds(1.0f); // Increased delay for permission dialog interaction

        ScanForExternalFractals();

        if (totalCount > 0)
        {
            LoadFractal(currentIndex);
        }
        else
        {
            DebugText.Log("No images found (Embedded or External).");
            Debug.LogWarning("No fractal images found.");
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

    void ScanForExternalFractals()
    {
        if (Directory.Exists(fractalPath))
        {
            try
            {
                string[] jpgs = Directory.GetFiles(fractalPath, "*.jpg");
                string[] pngs = Directory.GetFiles(fractalPath, "*.png");

                externalFractalFiles.AddRange(jpgs);
                externalFractalFiles.AddRange(pngs);

                DebugText.Log($"Found {externalFractalFiles.Count} external files.");
            }
            catch (System.Exception e)
            {
                DebugText.Log($"Error scanning external: {e.Message}");
            }
        }
        else
        {
            DebugText.Log($"Dir not found: {fractalPath}");
        }
    }

    public void NextImage()
    {
        if (totalCount == 0) return;

        currentIndex = (currentIndex + 1) % totalCount;
        LoadFractal(currentIndex);
    }

    public void PreviousImage()
    {
        if (totalCount == 0) return;

        currentIndex--;
        if (currentIndex < 0) currentIndex = totalCount - 1;
        LoadFractal(currentIndex);
    }

    void LoadFractal(int index)
    {
        if (index < 0 || index >= totalCount) return;

        // Cleanup previous dynamic texture if it was loaded from file
        // Embedded textures (Resources) are managed by Unity, don't destroy them manually unless unloading assets
        if (currentActiveTexture != null && !embeddedFractals.Contains(currentActiveTexture))
        {
            Destroy(currentActiveTexture);
        }

        // Case 1: Embedded
        if (index < embeddedFractals.Count)
        {
            currentActiveTexture = embeddedFractals[index];
            DebugText.Log($"Displaying Embedded: {currentActiveTexture.name}");
            ApplyTexture(currentActiveTexture);
        }
        // Case 2: External
        else
        {
            int externalIndex = index - embeddedFractals.Count;
            LoadExternalTexture(externalIndex);
        }
    }

    void LoadExternalTexture(int index)
    {
        string filePath = externalFractalFiles[index];

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);

            // Create a new Texture2D with mipmaps enabled
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, true);
            tex.wrapMode = TextureWrapMode.Clamp;

            if (tex.LoadImage(fileData))
            {
                tex.Apply(true, true); // Upload to GPU, release CPU memory
                currentActiveTexture = tex;
                DebugText.Log($"Displaying External: {Path.GetFileName(filePath)}");
                ApplyTexture(currentActiveTexture);
            }
            else
            {
                DebugText.Log($"Failed to load image: {Path.GetFileName(filePath)}");
            }
        }
    }

    void ApplyTexture(Texture2D tex)
    {
        if (targetRenderer != null)
        {
            targetRenderer.material.mainTexture = tex;
        }
    }
}
