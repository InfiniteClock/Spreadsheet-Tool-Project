using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class CustomTableEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTree;
    private ObjectField objField;
    private Button saveButton;

    public TextAsset json;
    private string currentJsonString;

    private List<Crop> cropList;

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
            // Get the newly input Text Asset file and cache the string version of it
            json = evt.newValue as TextAsset;
            currentJsonString = json.text;

            // Get the table visual element from the root for editing
            VisualElement root = rootVisualElement;
            MultiColumnListView oldTable = root.Q<MultiColumnListView>();

            // Removes old instances of the table
            if (oldTable != null) 
                root.Remove(oldTable);

            if (json != null)
            {
                string jsonString = json.text;
                CropDataListWrapper wrapper = JsonUtility.FromJson<CropDataListWrapper>(jsonString);

                if (wrapper == null || wrapper.Crops == null)
                {
                    Debug.Log("Failed to deserialize json or crops list is null");
                    // Erases the temp list if there is no json file
                    cropList = new List<Crop>();
                    return;
                }
                // Create the MultiColumnListView that will be our table
                MultiColumnListView table = new MultiColumnListView 
                {   
                    // The binding path determines the list it will try to access from wrapper
                    bindingPath = "Crops", 
                    // Whether the list displays the number of items in it
                    showBoundCollectionSize = true, 
                    // Don't fully understand this value yet, but it allows for scrolling and resizing
                    virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight
                };
                // Each column denotes a single variable within the scriptableObject "Crop"
                // The cell type must be compatible with the data type of the variable it is bound to
                table.columns.Add(new Column
                {
                    name = "itemName",
                    title = "Item Name",
                    makeCell = () => new TextField(),
                    bindCell = (element, index) => ((TextField)element).value = ((Crop)table.itemsSource[index]).itemName,
                    minWidth = 50,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "id",
                    title = "Item ID",
                    makeCell = () => new IntegerField(),
                    bindCell = (element, index) => ((IntegerField)element).value = ((Crop)table.itemsSource[index]).id,
                    minWidth = 50,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "sprite",
                    title = "Sprites",
                    makeCell = () => new ObjectField(),
                    bindCell = (element, index) => ((ObjectField)element).value = ((Crop)table.itemsSource[index]).sprite,
                    minWidth = 75,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "growthStages",
                    title = "Growth Stages",
                    makeCell = () => new IntegerField(),
                    bindCell = (element, index) => ((IntegerField)element).value = ((Crop)table.itemsSource[index]).growthStages,
                    minWidth = 50,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "color",
                    title = "Color Adjustment",
                    makeCell = () => new ColorField(),
                    bindCell = (element, index) => ((ColorField)element).value = ((Crop)table.itemsSource[index]).color,
                    minWidth = 75,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "growthDays",
                    title = "Days to Grow",
                    makeCell = () => new IntegerField(),
                    bindCell = (element, index) => ((IntegerField)element).value = ((Crop)table.itemsSource[index]).growthDays,
                    minWidth = 50,
                    stretchable = true
                });

                // Create a temporary list to hold the SO's we are going to make from the JSON
                cropList = new List<Crop>();

                // Assign json data values to each SO in the list, then assign it to cropList
                foreach(CropDataJSON jsonData in wrapper.Crops)
                {
                    Crop cropData = ScriptableObject.CreateInstance<Crop>();
                    cropData.itemName = jsonData.itemName;
                    cropData.id = jsonData.id;
                    cropData.growthStages = jsonData.growthStages;
                    cropData.growthDays = jsonData.growthDays;

                    cropList.Add(cropData);
                }

                // Pass through the list to the table to populate it with data
                table.itemsSource = cropList;

                // Refresh the UI 
                table.RefreshItems();

                // Add the table to the editor window
                root.Add(table);
            }
            
            
        }
        Debug.Log($"ObjectField value changed to: {evt.newValue?.name}");
        
    }

    // Saves the scriptable objects from the json file
    private void OnSaveButtonPressed()
    {
        if (json != null)
        {
            CropDataListWrapper wrapper = JsonUtility.FromJson<CropDataListWrapper>(currentJsonString);

            if (wrapper == null || wrapper.Crops == null)
            {
                Debug.Log("Json filed or crops list is null");
                return;
            }

            string cropString = "{\t \"Crops\" : [";
            for (int i = 0; i < cropList.Count; i++)
            {
                Crop crop = cropList[i];
                string assetPath = $"Assets/CreatedScriptableObjects/{crop.itemName}.asset";

                AssetDatabase.CreateAsset(crop, assetPath);

                cropString += JsonUtility.ToJson(crop, true);
                if (i < cropList.Count - 1) cropString += ",\t";
            }
            cropString += " \t] \t}";

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("Scriptable Objects created successfully!");
            
            string path = AssetDatabase.GetAssetPath(json);
            Debug.Log($"{path}");
            File.WriteAllText(path, cropString);

            
        }
    }
    void OnDisable()
    {
        // Unregister callbacks to prevent future errors when opening the window again
        if (objField != null) objField.UnregisterValueChangedCallback(OnObjectFieldValueChanged);
        if (saveButton != null) saveButton.clicked -= OnSaveButtonPressed;
        
    }
}
