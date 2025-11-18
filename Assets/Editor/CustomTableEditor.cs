using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomTableEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTree;
    private ObjectField objField;

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

        VisualElement toolbar = root.Q<VisualElement>("Toolbar");
        Toolbar bar = toolbar.Q<Toolbar>("Bar");
        objField = bar.Q<ObjectField>("JSONSelection");

        if (objField != null)
        {
            objField.RegisterValueChangedCallback(OnObjectFieldValueChanged);
            Debug.Log("Callback Registered: objField Value Change");
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
        // Example: If you expect a GameObject, cast it: GameObject assignedGameObject = evt.newValue as GameObject;
        if (evt.newValue is TextAsset)
        {
            json = evt.newValue as TextAsset;

            VisualElement root = rootVisualElement;
            VisualElement table = root.Q<VisualElement>("Table");
            if (table != null)
            {
                if (json != null)
                {
                    VisualElement label = new Label(json.text);
                    if (table.Q<Label>() != null)
                    {
                        table.Remove(table.Q<Label>());
                    }
                    table.Add(label);
                }
            }
        }
        Debug.Log($"ObjectField value changed to: {evt.newValue?.name}");
        
    }

    void OnDisable()
    {
        if (objField != null)
        {
            // Unregister callback to prevent future errors when opening the window again
            objField.UnregisterValueChangedCallback(OnObjectFieldValueChanged);
        }
    }
}
