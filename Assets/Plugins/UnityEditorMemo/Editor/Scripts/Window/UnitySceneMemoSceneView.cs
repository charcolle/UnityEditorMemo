using UnityEngine;
using UnityEditor;

namespace charcolle.UnityEditorMemo {

    internal enum SceneViewPos {
        TOPLEFT,
        BOTTOMLEFT,
        BOTTOMRIGHT,
    };

    internal static class UnitySceneMemoSceneView {

        public static void DrawMemo( UnitySceneMemo memo ) {
            if ( memo == null || !memo.ShowAtScene )
                return;

            Handles.BeginGUI();
            GUILayout.BeginArea( memoRect );
            {
                memo.OnSceneGUI();
            }
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private static Rect memoRect {
            get {
                var width  = 180f;
                var height = 100f;
                var sceneWidth  = SceneView.lastActiveSceneView.position.width;
                var sceneHeight = SceneView.lastActiveSceneView.position.height;
                //if ( sceneWidth * 0.25f < width )
                //    width = sceneWidth * 0.25f;
                //if ( sceneHeight * 0.2f < height )
                //    height = sceneHeight * 0.2f;
                var pos = ( SceneViewPos )UnityEditorMemoPrefs.UnitySceneMemoPosition;
                switch ( pos ) {
                    case SceneViewPos.TOPLEFT:
                        return new Rect( 5f, 5f, width, height );
                    case SceneViewPos.BOTTOMLEFT:
                        return new Rect( 5f, ( sceneHeight - height ) - 25f, width, height );
                    case SceneViewPos.BOTTOMRIGHT:
                        return new Rect( ( sceneWidth - width ) - 5f, ( sceneHeight - height ) - 25f, width, height );
                }
                return new Rect( ( sceneWidth - width ) -5f, ( sceneHeight - height ) - 25f, width, height );
            }
        }

    }
}