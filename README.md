# Stereo Fractal-a-Day Showcase

This project provides the necessary scripts and shaders to create a VR showcase app for Side-by-Side (SBS) 3D fractal images on the Meta Quest 3.

## Requirements

- Unity 2022.3 LTS or later.
- Meta XR Core SDK (available via Package Manager or Asset Store).
- Project configured for Android (Quest).

## Step-by-Step Implementation Guide

### 1. Initial Setup
1.  **Open Unity** and create a new 3D URP or Built-in Render Pipeline project.
2.  **Import Meta XR Core SDK**:
    - Go to **Window > Package Manager**.
    - Add the **Meta XR Core SDK** package.
    - Ensure **Oculus XR Plugin** is enabled in **Project Settings > XR Plug-in Management**.

### 2. Scene Setup (Hierarchy)
1.  **Remove Default Camera**: Delete the `Main Camera` from the scene.
2.  **Add OVRCameraRig**:
    - Search for `OVRCameraRig` in the Project view (under `Oculus/VR/Prefabs`).
    - Drag it into the Hierarchy.
    - This prefab handles head tracking and controllers.
3.  **Enable Passthrough**:
    - Select the `OVRCameraRig` in the Hierarchy.
    - In the Inspector, find the `OVRManager` component.
    - Under **Quest Features**, set **Passthrough Support** to **Supported** (or **Required**).
    - Enable **Insight Passthrough**.
    - Expand `OVRCameraRig` -> `TrackingSpace` -> `CenterEyeAnchor`.
    - Change the **Clear Flags** to **Solid Color**.
    - Set the **Background** color alpha to 0 (RGBA: `0, 0, 0, 0`).
    - Add an `OVRPassthroughLayer` component to the `OVRCameraRig` object.
    - Set **Placement** to **Underlay**.

### 3. Create the Curved Screen
1.  **Create a Mesh**:
    - Right-click in Hierarchy > **3D Object > Cylinder**.
    - Name it `FractalScreen`.
    - Reset its Transform.
    - **Position**: Set to `(0, 1.5, 3)` (3 meters in front, slightly elevated).
    - **Rotation**: Set to `(0, 180, 0)` (Face the camera? No, standard cylinder faces camera via its curved side properly. We might need `90` on X if using a Quad. For Cylinder: `(0, 0, 90)` creates a horizontal cylinder. `(0, 0, 0)` is vertical. We want a vertical cylinder segment.
    - **Scale**: `(8, 5, 8)`. This creates a large curved surface.
    - Since we are inside the cylinder, we need to ensure the shader renders backfaces (Cull Off), which we included in `StereoSplit.shader`.
    - Delete the `Capsule Collider` component.
2.  **Create Material**:
    - Create a new Material in `Assets/Materials` named `FractalMaterial`.
    - Set its Shader to `Custom/StereoSplit`.
    - Assign this material to the `FractalScreen` object.

### 4. Setup Scripts
1.  **Create GameManager**:
    - Create an empty GameObject named `GameManager`.
    - Add the `FractalManager` script component to it.
    - Drag the `FractalScreen` object (the Cylinder with the Renderer) into the `Target Renderer` field of the `FractalManager` component.

### 5. Input Configuration
- The script uses `OVRInput` from the Meta XR Core SDK.
- **Next Image**: Press **A** on the Right Controller.
- **Previous Image**: Press **B** on the Right Controller.

### 6. Preparation & Testing
1.  **Prepare Images**:
    - Create Side-by-Side (SBS) 3D fractal images (JPEG or PNG).
    - Ensure they are named correctly (e.g., `fractal1.jpg`, `fractal2.png`).
2.  **Deploy to Quest**:
    - Build and Run the project on your Meta Quest 3.
    - Connect the Quest to your PC via USB.
    - Use `adb` or SideQuest to copy your fractal images to the folder:
      `/sdcard/Pictures/Fractals/`
      (You may need to create the `Fractals` folder inside `Pictures`).
    - *Example Command*: `adb push my_fractal.jpg /sdcard/Pictures/Fractals/`
3.  **Run the App**:
    - Launch the app on the Quest.
    - You should see the Passthrough environment with the fractal screen floating in front of you.
    - Use A/B buttons to cycle through images.

## Troubleshooting
- **Black Screen**: Ensure Passthrough is enabled and `CenterEyeAnchor` background alpha is 0.
- **No Images**: Check that images are in `/sdcard/Pictures/Fractals` and permissions are granted (you may need to add `READ_EXTERNAL_STORAGE` permission in Player Settings > Publishing Settings > Android Manifest).
- **Stereo Inverted**: If the 3D effect looks wrong (inverted depth), swap the left/right images in your source file or modify the shader logic.
