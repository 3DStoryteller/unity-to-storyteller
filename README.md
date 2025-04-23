# unity-to-storyteller
Custom Unity Package to export props and scenes for use in the 3DStoryteller App. Works with Unity 6.

# To use:
Ensure the following build profiles are active in your project:
MacOS, iOS, Android
You can check which build profiles you have active via File > Build Profiles

# For props:
1. Import your model into Unity
2. Open the "Assets/3DStoryteller/Scenes/Asset Exporter.unity" scene
3. Drag the model into the scene and zero it out
4. Add an empty parent object by right-clicking on the model and selecting "Create Empty Parent"
5. Name the parent how you want the asset to be displayed in Storyteller
6. Add the Prop component to the empty parent
7. Drag the parent object to the project window to create a prefab
8. Save the scene
9. Right-click on the prefab in the project window and select "3DStoryteller Studio/Export Asset to 3DStoryteller Studio"
10. That's it! You can find your assetbundle file in Assets/3DStoryteller/Export
11. Open the 3DStoryteller application, load your story, click "Import" from the "Chapter Assets" panel, and select your .assetbundle file

# For environments:
1. Import your scene into Unity or create a new scene with your environment models
2. Delete any existing cameras
3. You can keep one or two dynamic lights in the scene, the rest need to be baked or removed
4. Drag the Storyteller Studio prefab from "3DStoryteller/Prefabs" into your scene
5. Make sure the prefab is zeroed out
6. Inside the prefab, position and rotate the Views/Default game object to set the desired camera position
7. You can modify the post-processing volume on "Views/Default/Post Process Volume" if you wish
8. Save the scene
9. Right-click on the scene in the project window and select "3DStoryteller Studio/Export Asset to 3DStoryteller Studio"
10. That's it! You can find your assetbundle file in 3DStoryteller/Export
11. Open the 3DStoryteller application, load your story, click "Import" from the "Chapter Assets" panel, and select your .assetbundle file
