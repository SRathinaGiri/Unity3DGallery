using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class FractalManager : MonoBehaviour
{
    [Tooltip("Assign the GameObject with the Renderer using the StereoSplit shader.")]
    public Renderer targetRenderer;

    private List<string> fractalFiles = new List<string>();
    private int currentIndex = 0;
    private Texture2D currentTexture;

    // Path on Quest local storage
    private string fractalPath = "/sdcard/Pictures/Fractals";

    void Start()
    {
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
        // Handle Input
        // A Button (Next)
        if (OVRInput.GetDown(OVRInput.Button.One, OVRInput.Controller.RTouch))
        {
            NextImage();
        }

        // B Button (Previous)
        if (OVRInput.GetDown(OVRInput.Button.Two, OVRInput.Controller.RTouch))
        {
            PreviousImage();
        }

        // Keyboard Fallback for Editor testing
        #if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.A))
        {
            NextImage();
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.B))
        {
            PreviousImage();
        }
        #endif
    }

    void ScanForFractals()
    {
        // In Editor, we can't easily access /sdcard unless we simulate it or use a different path.
        // But for the build, we use the specified path.

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

            // Clean up previous texture to free memory
            if (currentTexture != null)
            {
                Destroy(currentTexture);
            }

            // Create a new Texture2D.
            // We enable mipmaps (last parameter true) for better visual quality at distance/angles.
            currentTexture = new Texture2D(2, 2, TextureFormat.RGBA32, true);
            currentTexture.wrapMode = TextureWrapMode.Clamp;

            // LoadImage resizes the texture to match the image data.
            if (currentTexture.LoadImage(fileData))
            {
                // Apply(true, true) generates mipmaps and makes the texture non-readable to save memory.
                currentTexture.Apply(true, true);

                if (targetRenderer != null)
                {
                    targetRenderer.material.mainTexture = currentTexture;
                }
            }
        }
    }
}
