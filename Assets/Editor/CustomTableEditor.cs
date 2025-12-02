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

    private MultiColumnListView table;
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
    }

    // Triggers an event whenever the object field value is changed
    private void OnObjectFieldValueChanged(ChangeEvent<Object> evt)
    {
        // Get the table visual element from the root for editing
        VisualElement root = rootVisualElement;
        // Search for existing tables within the root element
        MultiColumnListView oldTable = root.Q<MultiColumnListView>();
        // Removes old instances of the table
        if (oldTable != null) root.Remove(oldTable);
        

        // Access the assigned object: evt.newValue
        // Example: If you expect a TextAsset, cast it: TextAsset assignedGameObject = evt.newValue as TextAsset;
        if (evt.newValue is TextAsset)
        {
            // Get the newly input Text Asset file and cache the string version of it
            json = evt.newValue as TextAsset;
            currentJsonString = json.text;

            // Create a temporary list to hold the SO's we are going to make from the JSON
            cropList = new List<Crop>();

            if (json != null)
            {
                string jsonString = json.text;
                CropDataListWrapper wrapper = JsonUtility.FromJson<CropDataListWrapper>(jsonString);

                if (wrapper == null || wrapper.Crops == null)
                {
                    Debug.Log("Failed to deserialize json or crops list is null");
                    return;
                }

                // Create the MultiColumnListView that will be our table
                table = new MultiColumnListView 
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
                    bindCell = (element, index) => 
                    {
                        // Bind the data field type to the elements within the column
                        TextField field = element as TextField;
                        field.value = ((Crop)table.itemsSource[index]).itemName;

                        // Unregister any existing callbacks just in case, then register the event callback for this data field
                        field.UnregisterCallback<ChangeEvent<string>, int>(OnNameChangedWithIndex);
                        field.RegisterCallback<ChangeEvent<string>, int>(OnNameChangedWithIndex, index);
                    },
                    minWidth = 100,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "id",
                    title = "Item ID",
                    makeCell = () => new IntegerField(),
                    bindCell = (element, index) => 
                    {
                        // Bind the data field type to the elements within the column
                        IntegerField field = element as IntegerField;
                        field.value = ((Crop)table.itemsSource[index]).id;

                        // Unregister any existing callbacks just in case, then register the event callback for this data field
                        field.UnregisterCallback<ChangeEvent<int>, int>(OnIDChangedWithIndex);
                        field.RegisterCallback<ChangeEvent<int>, int>(OnIDChangedWithIndex, index);
                    },
                    minWidth = 100,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "sprite",
                    title = "Sprites",
                    makeCell = () => new ObjectField(),
                    bindCell = (element, index) =>
                    {
                        // Bind the data field type to the elements within the column
                        ObjectField field = element as ObjectField;
                        field.value = ((Crop)table.itemsSource[index]).sprite;

                        // Unregister any existing callbacks just in case, then register the event callback for this data field
                        field.UnregisterCallback<ChangeEvent<Sprite>, int>(OnSpriteChangedWithIndex);
                        field.RegisterCallback<ChangeEvent<Sprite>, int>(OnSpriteChangedWithIndex, index);
                    },
                    minWidth = 100,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "growthStages",
                    title = "Growth Stages",
                    makeCell = () => new IntegerField(),
                    bindCell = (element, index) =>
                    {
                        // Bind the data field type to the elements within the column
                        IntegerField field = element as IntegerField;
                        field.value = ((Crop)table.itemsSource[index]).growthStages;

                        // Unregister any existing callbacks just in case, then register the event callback for this data field
                        field.UnregisterCallback<ChangeEvent<int>, int>(OnGrowthStagesChangedWithIndex);
                        field.RegisterCallback<ChangeEvent<int>, int>(OnGrowthStagesChangedWithIndex, index);
                    },
                    minWidth = 100,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "color",
                    title = "Color Adjustment",
                    makeCell = () => new ColorField(),
                    bindCell = (element, index) =>
                    {
                        // Bind the data field type to the elements within the column
                        ColorField field = element as ColorField;
                        field.value = ((Crop)table.itemsSource[index]).color;

                        // Unregister any existing callbacks just in case, then register the event callback for this data field
                        field.UnregisterCallback<ChangeEvent<Color>, int>(OnColorChangedWithIndex);
                        field.RegisterCallback<ChangeEvent<Color>, int>(OnColorChangedWithIndex, index);
                    },
                    minWidth = 100,
                    stretchable = true
                });
                table.columns.Add(new Column
                {
                    name = "growthDays",
                    title = "Days to Grow",
                    makeCell = () => new IntegerField(),
                    bindCell = (element, index) =>
                    {
                        // Bind the data field type to the elements within the column
                        IntegerField field = element as IntegerField;
                        field.value = ((Crop)table.itemsSource[index]).growthDays;

                        // Unregister any existing callbacks just in case, then register the event callback for this data field
                        field.UnregisterCallback<ChangeEvent<int>, int>(OnGrowthDaysChangedWithIndex);
                        field.RegisterCallback<ChangeEvent<int>, int>(OnGrowthDaysChangedWithIndex, index);
                    },
                    minWidth = 100,
                    stretchable = true
                });

                // Assign json data values to each SO in the list, then assign the SOs to cropList for future adjustment
                foreach(CropDataJSON jsonData in wrapper.Crops)
                {
                    Crop cropData = ScriptableObject.CreateInstance<Crop>();
                    cropData.itemName = jsonData.itemName;
                    cropData.id = jsonData.id;
                    cropData.sprite = jsonData.sprite;
                    cropData.growthStages = jsonData.growthStages;
                    cropData.color = jsonData.color;
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
        // Don't do anything if there is no selected JSON file
        if (json != null)
        {
            CropDataListWrapper wrapper = JsonUtility.FromJson<CropDataListWrapper>(currentJsonString);

            // Do not continue if there is no access to the local SO list
            if (cropList == null)
            {
                Debug.Log("Error with Scriptable Object list, can't save!");
                return;
            }
            
            // Save file must being with   { "name" : [
            string cropString = "{\t \"Crops\" : [";
            // Create and add each SO to Assets. Covert SO data to string for JSON saving
            // This structure aligns the JSON to be read as an array of objects so it can be read again later
            for (int i = 0; i < cropList.Count; i++)
            {
                Crop crop = cropList[i];

                // Create each SO in the specified asset folder
                string assetPath = $"Assets/CreatedScriptableObjects/{crop.itemName}.asset";
                AssetDatabase.DeleteAsset(assetPath);
                AssetDatabase.CreateAsset(crop, assetPath);

                // Save each SO's data to a string, in readable format
                cropString += JsonUtility.ToJson(crop, true);
                // Add a comma after each data entry, except the last, so it can be read as an object array later
                if (i < cropList.Count - 1) cropString += ",\t";
            }
            // Close out the JSON string with appropriate brackets
            cropString += " \t] \t}";

            // Save asset changes and refresh to update Unity
            AssetDatabase.SaveAssets();
            Debug.Log("Scriptable Objects created successfully!");
            
            // Rewrite the JSON file with the new string derived from the loop above
            string path = AssetDatabase.GetAssetPath(json);
            Debug.Log($"JSON rewritten at: {path}");
            File.WriteAllText(path, cropString);

            AssetDatabase.Refresh();
            objField.value = null;
        }
    }

    // Following events assign a new value from event at the established index in the SO list
    private void OnNameChangedWithIndex(ChangeEvent<string> evt, int index)
    {
        cropList[index].itemName = evt.newValue;
    }
    private void OnIDChangedWithIndex(ChangeEvent<int> evt, int index)
    {
        cropList[index].id = evt.newValue;
    }
    private void OnSpriteChangedWithIndex(ChangeEvent<Sprite> evt, int index)
    {
        cropList[index].sprite = evt.newValue;
    }
    private void OnGrowthStagesChangedWithIndex(ChangeEvent<int> evt, int index)
    {
        cropList[index].growthStages = evt.newValue;
    }
    private void OnColorChangedWithIndex(ChangeEvent<Color> evt, int index)
    {
        cropList[index].color = evt.newValue;
    }
    private void OnGrowthDaysChangedWithIndex(ChangeEvent<int> evt, int index)
    {
        cropList[index].growthDays = evt.newValue;
    }
    void OnDisable()
    {
        // Unregister callbacks to prevent future errors when opening the window again
        if (objField != null) objField.UnregisterValueChangedCallback(OnObjectFieldValueChanged);
        if (saveButton != null) saveButton.clicked -= OnSaveButtonPressed;
    }
}
