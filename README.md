# Stereo Fractal-a-Day Showcase

This project provides the necessary scripts and shaders to create a VR showcase app for Side-by-Side (SBS) 3D fractal images on the Meta Quest 3, using the modern **OpenXR Plugin** and **Unity Input System**.

## Requirements

- Unity 2022.3 LTS or later.
- Meta XR Core SDK (for Passthrough and OVRCameraRig helper features).
- **OpenXR Plugin** (via XR Plug-in Management).
- **Input System** package (com.unity.inputsystem).
- Project configured for Android (Quest).

## Step-by-Step Implementation Guide

### 1. Initial Setup
1.  **Open Unity** and create a new 3D URP or Built-in Render Pipeline project.
2.  **Install Packages**:
    - Go to **Window > Package Manager**.
    - Install **OpenXR Plugin** (and ensure it's enabled in Project Settings > XR Plug-in Management for Android).
    - Install **Input System** (agree to restart the editor if prompted).
    - Import **Meta XR Core SDK** (for OVRCameraRig and Passthrough features).
3.  **Project Settings**:
    - **Player > Other Settings**: Set **Write Permission** to **External (SDCard)**. This is crucial for accessing files outside the app bundle.
    - **Resolution and Presentation**: Ensure orientation is Landscape Left.

### 2. Scene Setup (Hierarchy)
1.  **Remove Default Camera**: Delete the `Main Camera`.
2.  **Add OVRCameraRig**:
    - Use the `OVRCameraRig` prefab (from `Oculus/VR/Prefabs`).
3.  **Enable Passthrough**:
    - Select `OVRCameraRig` > `OVRManager` component.
    - Set **Passthrough Support** to **Supported**.
    - Enable **Insight Passthrough**.
    - Set `CenterEyeAnchor` **Clear Flags** to **Solid Color** (Alpha 0).
    - Add `OVRPassthroughLayer` component (Placement: Underlay).

### 3. Create the Curved Screen
1.  **Create Mesh**:
    - Create a **Cylinder** (`FractalScreen`) at Position `(0, 1.5, 3)`, Scale `(8, 5, 8)`, Rotation `(0, 180, 0)`.
    - Remove Collider.
2.  **Material**:
    - Create material `FractalMaterial` using shader `Custom/StereoSplit`.
    - Assign to Cylinder.

### 4. Setup Scripts & Debugging
1.  **Create GameManager**:
    - Add `FractalManager` script.
    - Assign `FractalScreen` to **Target Renderer**.
2.  **Add Debug Text**:
    - Create a **3D Object > Text - TextMeshPro** in the scene.
    - Name it `DebugText`.
    - Position it at `(0, 1.5, 2.5)` so it floats in front of the screen.
    - Add the `DebugText` script to it.
    - Assign the `TextMeshPro` component to the `DebugText` script slot.

### 5. Adding Images (Gallery Mode)
You have two options for adding images:

**Option A: Embedded (Built-in)**
- Place your `.jpg` or `.png` textures into the folder: `Assets/Resources/Fractals/`.
- Unity will build these into the app. They will work immediately without permissions.

**Option B: External (User Content)**
- Build and run the app.
- Copy SBS 3D images to `/sdcard/Pictures/Fractals/` on the Quest.
- On first launch, accept the "Allow access to files" permission dialog.

### 6. Configure Input
- Create an **Input Action Asset** (Right Click > Create > Input Actions) named `VRControls`.
- Add Action `NextImage` -> Bind to `<XRController>{RightHand}/primaryButton` (A Button).
- Add Action `PreviousImage` -> Bind to `<XRController>{RightHand}/secondaryButton` (B Button).
- Assign these Actions to the `FractalManager` component in the Inspector.

## Troubleshooting
- **White Screen**: Means no images are loaded. Check the **DebugText** in the scene for errors.
- **Permission Denied**: Ensure Player Settings > Write Permission is set to **External (SDCard)**.
- **No Input**: Ensure Input System package is installed and Actions are assigned.
