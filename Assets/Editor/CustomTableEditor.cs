using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class CustomTableEditor : EditorWindow
{
    [MenuItem("Window/UI Toolkit/CustomTableEditor")]
    public static void ShowExample()
    {
        CustomTableEditor wnd = GetWindow<CustomTableEditor>();
        wnd.titleContent = new GUIContent("CustomTableEditor");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy.
        VisualElement label = new Label("Hello World! From C#");
        root.Add(label);

    }
}
