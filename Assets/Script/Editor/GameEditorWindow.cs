using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

public class GameEditorWindow : EditorWindow
{
    [MenuItem("Window/UI Toolkit/GameEditorWindow")]
    public static void ShowExample()
    {
        GameEditorWindow wnd = GetWindow<GameEditorWindow>();
        wnd.titleContent = new GUIContent("GameEditorWindow");
    }

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // VisualElements objects can contain other VisualElement following a tree hierarchy
        Label label = new Label("Hello World!");
        root.Add(label);

        // Create button
        Button button = new Button();
        button.name = "button";
        button.text = "Button";
        root.Add(button);

        // Create toggle
        Toggle toggle = new Toggle();
        toggle.name = "toggle";
        toggle.label = "Toggle";
        root.Add(toggle);

    }
}
