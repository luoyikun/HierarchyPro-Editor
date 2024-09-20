#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

//
//
//Enable 'Use Custom Buttons' toggles in the top bar settings
//
//

public class ToolBar_Example_Buttons
{
    [InitializeOnLoadMethod]
    static void AddButtonOoToolBar()
    {
        //you can subscribe to the left or right area
        EMX.CustomizationHierarchy.TopGUI.onLeftLayoutGUI += (full_rect) => {

            var rect = full_rect;


            if (EMX.CustomizationHierarchy.TopGUI.ButtonFitContent(ref rect, "Hello Unity!")) Debug.Log("Hello Unity!"); rect.x += rect.width;
            if (EMX.CustomizationHierarchy.TopGUI.ButtonFitContent(ref rect, "Hello World!")) Debug.Log("Hello World!"); rect.x += rect.width;

            rect.x += 10;

            rect.width = rect.height;
            if (GUI.Button(Shrink(rect, 2), "Go", button_style_0)) Debug.Log("Go!"); rect.x += rect.width;

            rect.x += 10;

            // we allocate space for GUILayout for case when custom and layout buttons areas are using in one place both
            GUILayout.Space(rect.x - full_rect.x);
        };
    }
    static Rect Shrink(Rect r, int v) { r.x += v; r.y += v; r.width -= v * 2; r.height -= v * 2; return r; }
    static GUIStyle _button_style; static GUIStyle button_style { get { return _button_style ?? (_button_style = new GUIStyle(EditorStyles.toolbarButton) { fixedHeight = 0 }); } }
    static GUIStyle _button_style_0; static GUIStyle button_style_0 { get { return _button_style_0 ?? (_button_style_0 = new GUIStyle(EditorStyles.toolbarButton) { fixedHeight = 0, padding = new RectOffset() }); } }
}

#endif