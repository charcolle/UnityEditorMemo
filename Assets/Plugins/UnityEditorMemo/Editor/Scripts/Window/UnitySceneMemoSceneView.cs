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
            GUILayout.BeginArea( memoRect( memo ) );
            {
                memo.OnSceneGUI();
            }
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private static Rect memoRect( UnitySceneMemo memo ) {
            var width = memo.SceneMemoWidth;
            var height = memo.SceneMemoHeight;
            var sceneWidth = SceneView.lastActiveSceneView.position.width;
            var sceneHeight = SceneView.lastActiveSceneView.position.height;

            // clamp
            if( sceneWidth - 15f < width )
                width = sceneWidth - 15f;
            if( sceneHeight - 15f < height )
                height = sceneHeight - 15f;

            var pos = ( SceneViewPos )UnityEditorMemoPrefs.UnitySceneMemoPosition;
            switch( pos ) {
                case SceneViewPos.TOPLEFT:
                    return new Rect( 5f, 5f, width, height );
                case SceneViewPos.BOTTOMLEFT:
                    return new Rect( 5f, ( sceneHeight - height ) - 25f, width, height );
                case SceneViewPos.BOTTOMRIGHT:
                    return new Rect( ( sceneWidth - width ) - 5f, ( sceneHeight - height ) - 25f, width, height );
            }
            return new Rect( ( sceneWidth - width ) - 5f, ( sceneHeight - height ) - 25f, width, height );
        }

    }
}