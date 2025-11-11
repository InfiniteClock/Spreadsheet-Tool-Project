using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomTableEditor : EditorWindow
{
    [SerializeField] private VisualTreeAsset visualTree;

    [MenuItem("Window/Custom Table Editor")]
    public static void ShowExample()
    {
        CustomTableEditor wnd = GetWindow<CustomTableEditor>();
        wnd.titleContent = new GUIContent("Custom Table Editor");
    }

    public void CreateGUI()
    {
        // Set the root visual element to the UXML file created with UI Toolkit
        visualTree.CloneTree(rootVisualElement);

        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        // This creates a label and adds it to the root
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

    }
}
