using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using VRC.SDK3.Components;
using VRC.Core;

using UdonVR.Tools.ApiUpdate;
using UdonVR.EditorUtility;

// created by Hamster9090901

public class UdonVR_WorldSettings : EditorWindow
{
    private static GUIContent windowTitleContent = new GUIContent("Udon VR - World Settings");
    public static UdonVR_WorldSettings window;

    #region GUI Images
    private Texture2D gui_icon_plus;
    private Texture2D gui_icon_minus;

    private Texture2D gui_icon_unlocked;
    private Texture2D gui_icon_locked;
    private Texture2D gui_icon_communityLabs;
    #endregion

    #region Save Load Variables
    private UVRImageUpload uVRImageUpload;

    private bool showBlueprintIDFoldoutMenu = true;
    private bool lastShowBlueprintIDFoldoutMenu = false;
    private string userEnteredBlueprintID;
    private string blueprintID;
    private ApiWorld apiWorld = null;
    private APIUser apiUser = null;
    private bool isInitalized = false;
    private bool isUploading = false;

    private const int MAX_USER_TAGS_FOR_WORLD = 5; // maximum user tags
    private const int MAX_CHARACTERS_ALLOWED_IN_USER_TAG = 20; // maximum characters allowed in user tags
    #endregion

    #region Image Variables
    private Texture2D udonVRLogo = null;
    private Texture2D worldTexture = null; // loaded world texture
    private Texture2D displayedTexture = null; // displayed texture
    private string displayedTexturePath = ""; // path gotten from picking a file from disc or taking a screenshot using the camera
    private int currentPickerWindow = 0;
    #endregion

    #region World Variables
    private float worldFileSize = -1;
    private string worldName = "Sample Name";
    private string worldDescription = "Sample Description";
    private int worldCapacity = 16;
    private string[] worldTags = { };
    #endregion

    #region Debug
    private bool showContainers = false; // used for debug
    #endregion

    #region Scroll View
    private Rect preScrollViewArea = Rect.zero; // contains the size of the area before the scroll view (used for always showing logos at the bottom)
    private Vector2 scrollPosition = Vector2.zero;
    private Rect scrollContentRect = Rect.zero; // contains the size of the scroll view content
    #endregion

    [MenuItem("UdonVR/World Settings")]
    private static void Init()
    {
        window = EditorWindow.GetWindow<UdonVR_WorldSettings>();
        window.maxSize = new Vector2(700, 850);
        window.minSize = new Vector2(350, 300);
        window.titleContent = windowTitleContent;
        window.Show();
    }

    private void OnGUI()
    {
        if (uVRImageUpload == null)
        {
            uVRImageUpload = UVRImageUpload.Instance;
            Debug.Log("Got Instance of UVRImageUpload");
        }

        if (window == null) window = EditorWindow.GetWindow<UdonVR_WorldSettings>(); // get windopw if it was null

        int inspectorWidth = (int)window.position.width; // get window width
        int inspectorHeight = (int)window.position.height; // get window height

        PipelineManager pipelineManager = UnityEngine.Object.FindObjectOfType<PipelineManager>();
        if (pipelineManager != null)
        {
            //blueprintID = pipelineManager.blueprintId;
            //if (!isInitalized) Initalize();
        }

        #region Show Options to allow user to add a world ID with out requiring a VRC World Scene Descriptor
        EditorGUILayout.BeginVertical(showContainers ? UdonVR_Style.Get(new Vector4(0.5f, 0.25f, 0.25f, 1f)) : new GUIStyle());

        if (lastShowBlueprintIDFoldoutMenu != showBlueprintIDFoldoutMenu)
        {
            lastShowBlueprintIDFoldoutMenu = showBlueprintIDFoldoutMenu;
            Repaint();
        }

        if (UdonVR_GUI.BeginButtonFoldout(new GUIContent("Show World ID Options"), ref showBlueprintIDFoldoutMenu, "showWorldIDOptions", EditorStyles.helpBox, null, 0, 0, false, showContainers))
        {
            #region Show Input field and buttons for letting users input a World ID or use the Scene Descriptor to get the ID
            EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
            EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
            UdonVR_GUI.Label(new GUIContent("Enter World ID: ", "Enter a World ID to View / Edit."), UdonVR_Predefined.Color.Style_DefaultTextColor);
            userEnteredBlueprintID = EditorGUILayout.TextField(userEnteredBlueprintID, UdonVR_Style.SetWidthHeight(-1, 18 * 1, GUI.skin.textField));
            EditorGUILayout.EndHorizontal();

            bool isPipelineNull = pipelineManager == null;
            bool isPipelineComplete = pipelineManager?.blueprintId != null;
            string vrcWorldToolTip = isPipelineNull ? "Could not find Scene Descriptor." : "Use the Scene Descriptor to get the world information to edit.";
            if (!isPipelineNull) vrcWorldToolTip = !isPipelineComplete ? "Pipeline Manager does not have a World ID." : vrcWorldToolTip;

            UdonVR_GUI.FieldArea(
                new GUIContent[] {
                    new GUIContent("Use Entered ID", "Use the entered World ID to get the world information to edit."),
                    new GUIContent("Use Scene Descriptor", vrcWorldToolTip),
                },
                new UdonVR_GUI.FieldAreaValues[]
                {
                    UdonVR_GUI.FieldAreaValues.SetValueMomentary(!string.IsNullOrWhiteSpace(userEnteredBlueprintID)),
                    UdonVR_GUI.FieldAreaValues.SetValueMomentary(isPipelineComplete, UdonVR_GUIOption.TintColorDefault(isPipelineComplete ? Color.white : Color.red))
                },
                new Action<UdonVR_GUI.FieldAreaValues>[]
                {
                    (areaValues) =>
                    {
                        // use entered id
                        if(!areaValues.boolValue.Value) return;
                        blueprintID = userEnteredBlueprintID;
                        Initalize();
                    },
                    (areaValues) =>
                    {
                        // use scene descriptor id
                        if(!areaValues.boolValue.Value) return;
                        blueprintID = pipelineManager.blueprintId;
                        Initalize();
                    }
                },
                EditorStyles.helpBox,
                null,
                showContainers
            );

            if (isPipelineNull || !isPipelineComplete)
            {
                EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
                GUIStyle warningStyle = new GUIStyle(GUI.skin.textField);
                warningStyle.normal.textColor = Color.red;
                warningStyle.wordWrap = true;
                warningStyle.alignment = TextAnchor.MiddleCenter;
                EditorGUILayout.LabelField(vrcWorldToolTip, UdonVR_Style.SetWidthHeight(-1, -1, warningStyle));
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
            #endregion

            #region Display the current ID of the world were editing.
            EditorGUILayout.BeginVertical(UdonVR_Style.SetWidthHeight(-1, -1, showContainers ? UdonVR_Style.Get(new Vector4(0.25f, 0.5f, 0.25f, 1f)) : new GUIStyle()));
            EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
            UdonVR_GUI.Label(new GUIContent("Current World ID: "), UdonVR_Predefined.Color.Style_DefaultTextColor);
            GUI.enabled = false;
            EditorGUILayout.TextField(blueprintID, new GUIStyle(GUI.skin.textField));
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            #endregion
        }
        UdonVR_GUI.EndButtonFoldout("showWorldIDOptions");

        EditorGUILayout.EndVertical();
        if (Event.current.type == EventType.Repaint) preScrollViewArea = GUILayoutUtility.GetLastRect(); // get the size of the rectangle
        #endregion



        if (apiWorld == null && blueprintID != null)
        {
            if (!isInitalized) Initalize();
        }

        if (apiUser == null || apiWorld == null)
        {
            showBlueprintIDFoldoutMenu = true; // show the ID foldout menu for users to enter or select an ID

            EditorGUILayout.BeginVertical(showContainers ? UdonVR_Style.Get(new Vector4(0.5f, 0.25f, 0.25f, 1)) : new GUIStyle(EditorStyles.helpBox));
            GUIStyle warningStyle = new GUIStyle(GUI.skin.textField);
            warningStyle.normal.textColor = Color.red;
            warningStyle.wordWrap = true;
            warningStyle.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(
                "Potentially not logged in please log in to your SDK if you have not." + "\n" +
                "No world information found for this ID, Please select a World ID to start.",
                UdonVR_Style.SetWidthHeight(-1, -1, warningStyle));
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();

            UdonVR_GUI.ShowUdonVRLinks(32, 32, true, null, showContainers);
            return;
        }

        // check if the logged in user is the author of the world
        bool isUserWorldAuthor = (apiUser.id == apiWorld.authorId) ? true : false;

        #region Scroll Area
        // add a line to show where the end of the scroll view ends and where the area for the links starts
        bool isDividerActive = (inspectorHeight - (96 + preScrollViewArea.height) <= scrollContentRect.height) ? true : false;
        GUIStyle dividerStyle = isDividerActive ? UdonVR_Style.Get(UdonVR_Predefined.Color.Background_Light) : UdonVR_Style.Get(UdonVR_Predefined.Color.General_Clear);
        dividerStyle = showContainers ? UdonVR_Style.Get(Color.white) : dividerStyle;
        GUILayout.Box(GUIContent.none, UdonVR_Style.SetWidthHeight(-1, 1, dividerStyle, new RectOffset(0, 0, 0, 0)));

        UdonVR_GUI.BeginDynamicScrollViewHeight(new Vector2(inspectorWidth, inspectorHeight - (96 + preScrollViewArea.height)), ref scrollPosition, new Rect(0, 0, inspectorWidth, scrollContentRect.height), showContainers);
        /*
        // should be used but update needs to be pushed so disabled until bugs can be fixed and it resizes proporly
        string guid = "testing";
        var output = UdonVR_VariableStorage.Instance.GetVariableGroup().GetVariable(guid); // save the rect
        Rect areaRect = output != null ? (Rect)Convert.ChangeType(output, typeof(Rect)) : Rect.zero;
        areaRect.width = inspectorWidth;
        */

        #region Begin inspector width modes
        float areaWidth = -1; // width of the area to center areas 1 & 2 in
        if (inspectorWidth >= 350 && !(inspectorWidth <= 550))
        {
            EditorGUILayout.BeginHorizontal();

            areaWidth = inspectorWidth; // areaRect.width
            areaWidth -= isDividerActive ? 13 : 0; // subtract the scrollbar width if scrollbar is active
        }
        else
        {
            EditorGUILayout.BeginHorizontal(showContainers ? UdonVR_Style.Get(new Vector4(0.25f, 0.25f, 0.5f, 1)) : new GUIStyle());
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginVertical(showContainers ? UdonVR_Style.Get(new Vector4(0.25f, 0.5f, 0.5f, 1)) : new GUIStyle());
        }
        #endregion

        //----------------

        #region Area 1
        EditorGUILayout.BeginVertical(); // group one start
        // width was 350
        EditorGUILayout.BeginVertical(UdonVR_Style.SetWidthHeight(-1, -1, showContainers ? UdonVR_Style.Get(new Vector4(0.5f, 0.25f, 0.25f, 1)) : new GUIStyle()));

        EditorGUILayout.BeginHorizontal(UdonVR_Style.SetWidthHeight(areaWidth != -1 ? areaWidth / 2 : -1, -1));
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical(UdonVR_Style.SetWidthHeight(256 + 16, -1, EditorStyles.helpBox));

        #region Image Display
        EditorGUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical(UdonVR_Style.SetWidthHeight(256 + 8, -1, EditorStyles.helpBox));
        UdonVR_GUI.DrawImage(displayedTexture, 256, 196);
        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        #endregion

        #region Image Options
        EditorGUILayout.BeginHorizontal(showContainers ? UdonVR_Style.Get(new Vector4(0.5f, 0.5f, 0.25f, 1)) : new GUIStyle());
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical(UdonVR_Style.SetWidthHeight(256, -1, EditorStyles.helpBox));

        UdonVR_GUI.FieldArea(
            new GUIContent[] {
                new GUIContent("Image From File", ".png ONLY"),
                new GUIContent("Image From Assets", ".png ONLY"),
            },
            new UdonVR_GUI.FieldAreaValues[]
            {
                UdonVR_GUI.FieldAreaValues.SetValueMomentary(isUserWorldAuthor),
                UdonVR_GUI.FieldAreaValues.SetValueMomentary(isUserWorldAuthor)
            },
            new Action<UdonVR_GUI.FieldAreaValues>[]
            {
                (areaValues) =>
                {
                    // image from file
                    if(!areaValues.boolValue.Value) return;
                    string path = EditorUtility.OpenFilePanel("Select a .png", "", "png");
                    if (path.Length != 0)
                    {
                        var fileContent = File.ReadAllBytes(path);
                        displayedTexture = new Texture2D(1, 1);
                        displayedTexture.LoadImage(fileContent);
                    }
                    displayedTexturePath = path;
                },
                (areaValues) =>
                {
                    // image from assets
                    if(!areaValues.boolValue.Value) return;
                    currentPickerWindow = EditorGUIUtility.GetControlID(FocusType.Passive) + 100;
                    EditorGUIUtility.ShowObjectPicker<Texture2D>(displayedTexture, false, "", currentPickerWindow);
                }
            },
            EditorStyles.helpBox,
            null,
            showContainers
        );

        // used for getting the image from the object picker
        if (Event.current.commandName == "ObjectSelectorUpdated" && EditorGUIUtility.GetObjectPickerControlID() == currentPickerWindow)
        {
            Texture2D pickedTexture = (Texture2D)EditorGUIUtility.GetObjectPickerObject();

            string path = AssetDatabase.GetAssetPath(pickedTexture);
            string extension = Path.GetExtension(path);
            if (extension.ToLower() == ".png")
            {
                displayedTexture = pickedTexture;
                displayedTexturePath = path;
            }
            else
            {
                EditorUtility.DisplayDialog(
                    windowTitleContent.text,
                    "You Selected a file with the " + extension + " extension and that extension cannot be used.",
                    "Ok");
                currentPickerWindow = -1;
            }

            Repaint();
        }
        // object picker cleanup
        if (Event.current.commandName == "ObjectSelectorClosed")
        {
            currentPickerWindow = -1;
        }

        GUI.enabled = isUserWorldAuthor; // enable / disable editing if user is not world owner
        if (GUILayout.Button(new GUIContent("From Camera")))
        {
            UdonVR_CameraImage capture = Camera.main.gameObject.GetComponent<UdonVR_CameraImage>();
            if (capture == null) capture = Camera.main.gameObject.AddComponent<UdonVR_CameraImage>();
            string path = "Assets\\_UdonVR\\Tools\\Settings\\World\\SavedImages\\";
            capture.TakeImage(worldName, ref path, -1, -1, true);
            var fileContent = File.ReadAllBytes(path);
            displayedTexture = new Texture2D(1, 1);
            displayedTexture.LoadImage(fileContent);
            displayedTexturePath = path;
        }
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();
        #endregion

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical(); // group one end
        #endregion

        //----------------

        if ((inspectorWidth >= 350) && (inspectorWidth <= 550)) EditorGUILayout.Space();

        //----------------

        #region Area 2
        EditorGUILayout.BeginVertical(); // group two start
        // width was 350
        EditorGUILayout.BeginVertical(UdonVR_Style.SetWidthHeight(-1, -1, showContainers ? UdonVR_Style.Get(new Vector4(0.25f, 0.5f, 0.25f, 1)) : new GUIStyle()));

        EditorGUILayout.BeginHorizontal(UdonVR_Style.SetWidthHeight(areaWidth != -1 ? areaWidth / 2 : -1, -1));
        GUILayout.FlexibleSpace();
        EditorGUILayout.BeginVertical(UdonVR_Style.SetWidthHeight(256 + 16, -1, EditorStyles.helpBox));

        #region World Creater, Version & File Size
        EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
        UdonVR_GUI.Label(new GUIContent("Author: "), UdonVR_Predefined.Color.Style_DefaultTextColor);
        UdonVR_GUI.Label(new GUIContent(apiWorld.authorName), (apiUser.displayName == apiWorld.authorName) ? Color.green : Color.red);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
        UdonVR_GUI.Label(new GUIContent("Version:"), UdonVR_Predefined.Color.Style_DefaultTextColor);
        UdonVR_GUI.Label(new GUIContent(apiWorld.version.ToString()), UdonVR_Predefined.Color.Style_DefaultTextColor);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal(new GUIStyle(EditorStyles.helpBox));
        UdonVR_GUI.Label(new GUIContent("File Size:"), UdonVR_Predefined.Color.Style_DefaultTextColor);
        UdonVR_GUI.Label(new GUIContent(UdonVR_Functions.FormatFileSize(worldFileSize)), UdonVR_Predefined.Color.Style_DefaultTextColor);
        EditorGUILayout.EndHorizontal();
        #endregion

        #region World Name
        EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
        UdonVR_GUI.Header(new GUIContent("Name:"));
        GUI.enabled = isUserWorldAuthor; // enable / disable editing if user is not world owner
        worldName = EditorGUILayout.TextArea(worldName, UdonVR_Style.SetWidthHeight(-1, 18 * 1, GUI.skin.textField));
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        #endregion

        #region World Description
        EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
        UdonVR_GUI.Header(new GUIContent("Description:"));
        GUIStyle worldDescriptionStyle = UdonVR_Style.SetWidthHeight(-1, -1, GUI.skin.textField);
        worldDescriptionStyle.wordWrap = true;
        GUI.enabled = isUserWorldAuthor; // enable / disable editing if user is not world owner
        worldDescription = EditorGUILayout.TextArea(worldDescription, worldDescriptionStyle);
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        #endregion

        #region World Capacity
        EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
        UdonVR_GUI.Header(new GUIContent("Capacity:"));
        EditorGUILayout.BeginHorizontal();
        GUI.enabled = isUserWorldAuthor; // enable / disable editing if user is not world owner
        if (UdonVR_GUI.TintedButton(new GUIContent(gui_icon_plus), UdonVR_GUIOption.WidthHeight(18, 18), UdonVR_GUIOption.Padding(new RectOffset()), UdonVR_GUIOption.TintColorDefault(Color.green))) worldCapacity++;
        worldCapacity = EditorGUILayout.IntField(worldCapacity, UdonVR_Style.SetWidthHeight(-1, 18 * 1, GUI.skin.textField));
        if (UdonVR_GUI.TintedButton(new GUIContent(gui_icon_minus), UdonVR_GUIOption.WidthHeight(18, 18), UdonVR_GUIOption.Padding(new RectOffset()), UdonVR_GUIOption.TintColorDefault(Color.red))) worldCapacity--;
        GUI.enabled = true;
        EditorGUILayout.EndHorizontal();
        worldCapacity = Mathf.Clamp(worldCapacity, 1, 40); // clamp world capacity to be within 1 - 40
        EditorGUILayout.EndVertical();
        #endregion

        #region World Visibility
        EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
        EditorGUILayout.BeginHorizontal();
        UdonVR_GUI.Label(new GUIContent("Visibility:"), UdonVR_Predefined.Color.Style_DefaultTextColor, new RectOffset(0, 0, 1, 0));
        GUIContent content = new GUIContent(apiWorld.releaseStatus.ToUpperFirst());
        if (apiWorld.releaseStatus == "public") content.image = gui_icon_unlocked; // show unlocked ucon if world is public
        if (apiWorld.releaseStatus == "private") content.image = gui_icon_locked; // show locked icon if world is private
        UdonVR_GUI.Label(content, UdonVR_Predefined.Color.Style_DefaultTextColor);
        GUILayout.FlexibleSpace();
        if (apiWorld.IsCommunityLabsWorld) UdonVR_GUI.DrawImage(gui_icon_communityLabs, 16, 16); // draw community labs logo if world is community labs
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        #endregion

        #region World Tags
        EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox));
        UdonVR_GUI.Header(new GUIContent("Tags: "));

        // display tags in array
        for (int i = 0; i < worldTags.Length; i++)
        {
            EditorGUILayout.BeginHorizontal();
            GUI.enabled = isUserWorldAuthor; // enable / disable editing if user is not world owner
            worldTags[i] = EditorGUILayout.TextArea(worldTags[i], UdonVR_Style.SetWidthHeight(-1, 18 * 1, GUI.skin.textField)); // show input field for this index in the array
            if (UdonVR_GUI.TintedButton(new GUIContent("X"), UdonVR_GUIOption.WidthHeight(18, 18), UdonVR_GUIOption.TintColorDefault(Color.red)))
            {
                string removeKeyword = "__REMOVE_ME_THX_U__";
                worldTags[i] = removeKeyword;
                worldTags = worldTags.Remove(removeKeyword);
                //worldTags = UdonVR_Functions.ArrayRemoveItem(worldTags, removeKeyword);
            }
            GUI.enabled = true;
            EditorGUILayout.EndHorizontal();
        }

        GUI.enabled = (worldTags.Length < MAX_USER_TAGS_FOR_WORLD && isUserWorldAuthor) ? true : false;
        if (GUILayout.Button(new GUIContent("Add"), GUI.skin.button))
        {
            worldTags = worldTags.Add("New Tag");
            //worldTags = (string[])UdonVR_Functions.ArrayAddItem(worldTags, "New Tag");
        }
        GUI.enabled = true;
        EditorGUILayout.EndVertical();
        #endregion

        #region Upload Changes
        GUI.enabled = (isUserWorldAuthor && !isUploading) ? true : false; // enable / disable editing if user is not world owner
        GUIContent saveChangesButtonContent = GUIContent.none;
        saveChangesButtonContent = new GUIContent("Save Changes", isUserWorldAuthor ? "Upload changes to VRChat Servers." : "Not World Author cannot Upload changes.");
        saveChangesButtonContent = isUploading ? new GUIContent("Uploading Changes...", "Uploading changes to VRChat Servers.") : saveChangesButtonContent;
        if (UdonVR_GUI.TintedButton(saveChangesButtonContent, UdonVR_GUIOption.Height(18 * 2), UdonVR_GUIOption.TintColorDefault(isUserWorldAuthor ? Color.green : Color.red)))
        {
            switch (EditorUtility.DisplayDialogComplex(
                windowTitleContent.text,
                "Are you sure you want to save your changes?",
                "Im Sure.",
                "Not Yet.",
                "Don't Save Image."))
            {
                case 0:
                    Debug.Log("Uploading with image.");
                    SaveWorldChanges(true);
                    break;
                case 1:
                    //Debug.Log("Not Uploading Yet.");
                    break;
                case 2:
                    Debug.Log("Uploading with out Image.");
                    SaveWorldChanges(false);
                    break;
            }
        }
        GUI.enabled = true;
        #endregion

        EditorGUILayout.EndVertical();
        GUILayout.FlexibleSpace();
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical(); // group two end
        #endregion

        //----------------

        #region End inspector width modes
        if (inspectorWidth >= 350 && !(inspectorWidth <= 550))
        {
            EditorGUILayout.EndHorizontal();
            /*
            // should be used but update needs to be pushed so disabled until bugs can be fixed and it resizes proporly
            if (Event.current.type == EventType.Repaint)
            {
                areaRect = GUILayoutUtility.GetLastRect();
                UdonVR_VariableStorage.Instance.GetVariableGroup().SetVariable(guid, areaRect);
            }
            */
        }
        else
        {
            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        UdonVR_GUI.EndDynamicScrollViewHeight(ref scrollContentRect); // end scroll view

        // add a line to show where the end of the scroll view ends and where the area for the links starts
        GUILayout.Box(GUIContent.none, UdonVR_Style.SetWidthHeight(-1, 1, dividerStyle, new RectOffset(0, 0, 0, 0)));
        #endregion

        #region UdonVR Links
        bool forceBottom = false;
        if (!forceBottom) EditorGUILayout.BeginVertical(showContainers ? UdonVR_Style.Get(new Vector4(0.5f, 0.25f, 0.125f, 1)) : new GUIStyle());
        if (!forceBottom) GUILayout.FlexibleSpace();
        UdonVR_GUI.ShowUdonVRLinks(32, 32, forceBottom, null, showContainers);
        if (!forceBottom) GUILayout.FlexibleSpace();
        if (!forceBottom) EditorGUILayout.EndVertical();
        #endregion
    }

    #region Functions
    private void Initalize()
    {
        udonVRLogo = (Texture2D)Resources.Load("_UdonVR/Logos/udonVR_256_196");
        displayedTexture = udonVRLogo; // set the default displayed texture to the udonVR logo

        gui_icon_plus = (Texture2D)Resources.Load("_UdonVR/Icons/gui_icon_plus");
        gui_icon_minus = (Texture2D)Resources.Load("_UdonVR/Icons/gui_icon_minus");

        gui_icon_unlocked = (Texture2D)Resources.Load("_UdonVR/Icons/gui_icon_unlocked");
        gui_icon_locked = (Texture2D)Resources.Load("_UdonVR/Icons/gui_icon_locked");
        gui_icon_communityLabs = (Texture2D)Resources.Load("CL_Icons/CL_Lab_Icon_256");

        API.SetOnlineMode(true); // force online mode
        if (!APIUser.IsLoggedIn)
        {
            if (ApiCredentials.Load())
                APIUser.InitialFetchCurrentUser((c) => UserLoggedInCallback(c.Model as APIUser), (c) => ThrowError(c.Error));
            else
                ThrowError("No Credentials.");
        }
        else
        {
            UserLoggedInCallback(APIUser.CurrentUser);
        }

        isInitalized = true;
    }

    private void ThrowError(string error)
    {
        Debug.LogError("Error: " + error);
    }

    private void UserLoggedInCallback(APIUser user)
    {
        apiUser = user;
        Debug.Log("UserLoggedInCallback");
        ApiWorld model = new ApiWorld
        {
            id = blueprintID
        };
        model.Fetch(null,
            (c) =>
            {
                //VRC.Core.Logger.Log("<color=magenta>Updating an existing world.</color>", DebugLevel.All);
                apiWorld = c.Model as ApiWorld;
                ImageDownloader.DownloadImage(apiWorld.imageUrl, 0,
                    obj =>
                    {
                        worldTexture = obj;
                        displayedTexture = worldTexture;
                        displayedTexturePath = AssetDatabase.GetAssetPath(displayedTexture);
                        Repaint();
                    },
                    () =>
                    {
                        worldTexture = null;
                        displayedTexture = udonVRLogo;
                        Repaint();
                    });

                GetWorldFileSize(); // get the worlds filesize in bytes

                worldName = apiWorld.name;
                worldDescription = apiWorld.description;
                worldCapacity = apiWorld.capacity;
                worldTags = apiWorld.tags.Where(s => s.StartsWith("author_tag_")).ToArray();

                for (int i = 0; i < worldTags.Length; i++)
                {
                    apiWorld.tags.Remove(worldTags[i]); // required to remove the tags so if we remove them on our side they actually get removed
                    worldTags[i] = worldTags[i].Replace("author_tag_", "");
                }
                showBlueprintIDFoldoutMenu = false; // hide the ID foldout menu because we have a world to edit
                                                    //Debug.Log("<color=magenta>World record fetched.</color>");
            },
            (c) =>
            {
                showBlueprintIDFoldoutMenu = true; // show the ID foldout menu because the ID we tried failed
                Debug.Log("<color=magenta>World record not found.</color>");
            });
    }

    private void SaveWorldChanges(bool saveImage)
    {
        isUploading = true;

        string[] delimiterStrings = { " ", ",", ".", ":", "\t", "\n", "\"", "#" };

        apiWorld.name = worldName;
        if (string.IsNullOrEmpty(worldDescription.Trim())) worldDescription = worldName;
        apiWorld.description = worldDescription;
        apiWorld.capacity = worldCapacity;
        for (int i = 0; i < worldTags.Length; i++)
        {
            string tag = worldTags[i];

            // stop if tag contains more characters than the max number
            if (tag.Length > MAX_CHARACTERS_ALLOWED_IN_USER_TAG)
            {
                EditorUtility.DisplayDialog(
                    windowTitleContent.text,
                    "One or more of your tags exceeds the maximum " + MAX_CHARACTERS_ALLOWED_IN_USER_TAG + " character limit.\n\n" + "Please shorten tags before uploading.",
                    "OK");
                isUploading = false;
                return;
            }

            // stop if tag contains a character that is not a letter or digit
            if (!tag.All(char.IsLetterOrDigit))
            {
                EditorUtility.DisplayDialog(windowTitleContent.text,
                    "Please remove any non-alphanumeric characters from tags before uploading.",
                    "OK");
                isUploading = false;
                return;
            }

            // replace seperator characters in tags
            foreach (string s in delimiterStrings)
            {
                tag = tag.Replace(s, "");
            }

            Debug.Log("Tag: " + tag);

            // add tag
            apiWorld.tags.Add("author_tag_" + tag);
        }

        if (!saveImage)
        {
            SaveAPIWorld(); // save world information
        }
        else
        {
            string filePath = AssetDatabase.GetAssetPath(displayedTexture);
            if (string.IsNullOrEmpty(filePath.Trim())) filePath = displayedTexturePath;
            if (!string.IsNullOrEmpty(filePath.Trim()))
            {
                if (!Path.IsPathRooted(filePath)) filePath = Path.Combine(Directory.GetCurrentDirectory(), filePath);
                filePath = filePath.Replace("/", "\\");
            }

            // check to see if the filepath is null and if it is abort
            if (string.IsNullOrEmpty(filePath.Trim()))
            {
                isUploading = false;
                Debug.LogWarning("Image file path was null. Skipping Image Upload. \n" +
                    "This error can be caused if we tried to upload the displayed image but the displayed image was never changed from the downloaded world image.");
                SaveAPIWorld();
                return;
            }

            EditorCoroutine.Start(uVRImageUpload.DoUpdateImage(apiWorld.imageUrl, GetFriendlyWorldFileName(), filePath, delegate (string fileUrl)
            {
                if (!string.IsNullOrEmpty(fileUrl.Trim()))
                {
                    apiWorld.imageUrl = fileUrl;
                    Debug.Log("Image Uploaded");
                }
                else
                {
                    Debug.Log("Image URL is NULL");
                }

                SaveAPIWorld(); // save world information
            }));
        }
    }

    private void SaveAPIWorld()
    {
        ApiWorld saveWorld = new ApiWorld
        {
            id = apiWorld.id,
            authorId = apiWorld.authorId,
            name = apiWorld.name,
            description = apiWorld.description,
            capacity = apiWorld.capacity,
            tags = apiWorld.tags,
            imageUrl = apiWorld.imageUrl,
        };
        saveWorld.Save(
            (c) =>
            {
                ApiWorld savedBP = (ApiWorld)c.Model;
                if (EditorUtility.DisplayDialog(
                    windowTitleContent.text,
                    "Changes have been saved.",
                    "OK"))
                {
                    Initalize(); // reinitalize to refresh and load the changes
                }
            },
            (c) =>
            {
                Debug.LogError(c.Error);
                Initalize(); // reinitalize to refresh and load the changes
            });
        isUploading = false;
    }

    private string GetFriendlyWorldFileName(string type = "Image")
    {
        return "World - " + apiWorld.name + " - " + type + " - " + Application.unityVersion + "_" + ApiWorld.VERSION.ApiVersion +
            "_" + VRC.Tools.Platform + "_" + API.GetServerEnvironmentForApiUrl();
    }

    private void GetWorldFileSize()
    {
        ApiFile _file = new ApiFile(ApiFile.ParseFileIdFromFileAPIUrl(apiWorld.assetUrl));
        worldFileSize = -1; // reset filesize before grabbing filesize
        _file.Fetch(
            (c) =>
            {
                ApiFile fetchedFile = c.Model as ApiFile;
                worldFileSize = fetchedFile.GetLatestVersion().file.sizeInBytes;
            });
    }
    #endregion
}
