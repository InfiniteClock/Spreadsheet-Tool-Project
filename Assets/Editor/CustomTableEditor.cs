using System.IO;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomTableEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTree;
    private ObjectField objField;
    private Button saveButton;

    public TextAsset json;


    [MenuItem("Window/Table Editor")]
    public static void ShowExample()
    {
        CustomTableEditor wnd = GetWindow<CustomTableEditor>();
        wnd.titleContent = new GUIContent("Table Editor");
    }

    public void CreateGUI()
    {
        // Set the root visual element to the UXML file created with UI Toolkit
        visualTree.CloneTree(rootVisualElement);

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Get access to each important element in the tool
        VisualElement toolbar = root.Q<VisualElement>("Toolbar");
        Toolbar bar = toolbar.Q<Toolbar>("Bar");
        objField = bar.Q<ObjectField>("JSONSelection");
        saveButton = bar.Q<Button>("SaveButton");


        if (objField != null)
        {
            objField.RegisterValueChangedCallback(OnObjectFieldValueChanged);
        }
        if (saveButton != null)
        {
            saveButton.clicked += OnSaveButtonPressed;
        }

        // Access the Table element and begin filling it if not null
        VisualElement table = root.Q<VisualElement>("Table");
        if (table != null)
        {
            if (json != null)
            {
                VisualElement label = new Label(json.text);
            }
        }


        /*
        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        // This creates a label and adds it to the root
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);*/

    }

    // Triggers an event whenever the object field value is changed
    private void OnObjectFieldValueChanged(ChangeEvent<Object> evt)
    {
        // Access the assigned object: evt.newValue
        // Example: If you expect a TextAsset, cast it: TextAsset assignedGameObject = evt.newValue as TextAsset;
        if (evt.newValue is TextAsset)
        {
            json = evt.newValue as TextAsset;

            // Get the table visual element from the root for editing
            VisualElement root = rootVisualElement;
            VisualElement table = root.Q<VisualElement>("Table");
            if (table != null)
            {
                if (json != null)
                {
                    string jsonString = json.text;
                    CropDataListWrapper wrapper = JsonUtility.FromJson<CropDataListWrapper>(jsonString);

                    Debug.Log(wrapper.Crops.Count);

                    if (wrapper == null || wrapper.Crops == null)
                    {
                        Debug.Log("Failed to deserialize json or crops list is null");
                        return;
                    }

                    /*
                    // Filler that creates a label with all of the json text in it
                    VisualElement label = new Label(json.text);
                    
                    // Remove any existing labels from Table
                    if (table.Q<Label>() != null)
                    {
                        table.Remove(table.Q<Label>());
                    }

                    // Adds the new element to the Table visual element
                    table.Add(label);*/
                }
            }
        }
        Debug.Log($"ObjectField value changed to: {evt.newValue?.name}");
        
    }

    // Saves the scriptable objects from the json file
    private void OnSaveButtonPressed()
    {
        if (json != null)
        {
            string jsonString = json.text;
            CropDataListWrapper wrapper = JsonUtility.FromJson<CropDataListWrapper>(jsonString);

            Debug.Log(wrapper.Crops.Count);

            if (wrapper == null || wrapper.Crops == null)
            {
                Debug.Log("Failed to deserialize json or crops list is null");
                return;
            }

            foreach (CropDataJSON jsonData in wrapper.Crops)
            {

                Crop cropData = ScriptableObject.CreateInstance<Crop>();
                cropData.itemName = jsonData.itemName;
                cropData.id = jsonData.id;
                cropData.growthStages = jsonData.growthStages;
                cropData.growthDays = jsonData.growthDays;
                string assetPath = $"Assets/CreatedScriptableObjects/{jsonData.itemName}.asset";

                AssetDatabase.CreateAsset(cropData, assetPath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Scriptable Objects created successfully!");
        }
    }
    void OnDisable()
    {
        // Unregister callbacks to prevent future errors when opening the window again
        if (objField != null) objField.UnregisterValueChangedCallback(OnObjectFieldValueChanged);
        if (saveButton != null) saveButton.clicked -= OnSaveButtonPressed;
        
    }
}
