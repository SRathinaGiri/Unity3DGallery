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

### 2. Scene Setup (Hierarchy)
1.  **Remove Default Camera**: Delete the `Main Camera`.
2.  **Add OVRCameraRig**:
    - Use the `OVRCameraRig` prefab (from `Oculus/VR/Prefabs`).
    - Note: Even with OpenXR, `OVRCameraRig` provides a convenient hierarchy for Passthrough setup.
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

### 4. Setup Scripts & Input
1.  **Create GameManager**:
    - Add `FractalManager` script.
    - Assign `FractalScreen` to **Target Renderer**.
2.  **Configure Input**:
    - The script uses Unity's **Input System**.
    - Create an **Input Action Asset** (Right Click > Create > Input Actions) named `VRControls`.
    - Edit `VRControls`:
        - Create Action Map `FractalControl`.
        - Add Action `NextImage` -> Bind to `<XRController>{RightHand}/primaryButton` (A Button).
        - Add Action `PreviousImage` -> Bind to `<XRController>{RightHand}/secondaryButton` (B Button).
        - Save Asset.
    - Select the `VRControls` asset in Project view and check **Generate C# Class** (optional) or just drag the references.
    - **Easier Method**: Add a `PlayerInput` component to `GameManager`, assign `VRControls`, and drag the specific Actions (Next/Previous) into the `FractalManager` slots in the Inspector.
    - *Alternatively*, you can assign default OpenXR bindings directly if you use the Input System's default asset.

### 5. Deployment
- Build for Android (Quest 3).
- Copy SBS 3D images to `/sdcard/Pictures/Fractals/`.
- Ensure you accept the "Allow access to files" permission dialog on first launch.

## Troubleshooting
- **No Input?**: Ensure the Input System package is active and the correct Action References are assigned in the Inspector.
- **Black Screen?**: Check Passthrough settings and alpha channel.
