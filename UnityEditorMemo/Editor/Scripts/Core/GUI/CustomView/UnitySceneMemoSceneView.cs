using UnityEngine;
using UnityEditor;

namespace charcolle.UnityEditorMemo {

    internal enum SceneViewPos {
        TOPLEFT,
        BOTTOMLEFT,
        BOTTOMRIGHT,
    };

    internal static class UnitySceneMemoSceneView {

        public static void OnGUI( UnitySceneMemo memo ) {
            if ( memo == null || !memo.ShowAtScene )
                return;

            Handles.BeginGUI();
            GUILayout.BeginArea( memoRect( memo ) );
            {
                Draw( memo );
            }
            GUILayout.EndArea();
            Handles.EndGUI();
        }

        private static Vector2 scrollView = Vector2.zero;
        private static bool InVisible;
        private static void Draw( UnitySceneMemo memo ) {
            EditorGUILayout.BeginVertical();
            {
                // header
                GUI.backgroundColor = GUIHelper.Colors.SceneMemoLabelColor( memo.Label );
                EditorGUILayout.BeginHorizontal( GUIHelper.Styles.MemoHeader );
                {
                    if( GUILayout.Button( InVisible ? "●" : "x", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 18 ) } ) ) {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.SceneMemoUndo( UndoHelper.UNDO_SCENEMEMO_EDIT );
                        InVisible = !InVisible;
                    }
                    EditorGUILayout.BeginHorizontal();
                    {
                        drawComponents( memo.Components );
                        GUILayout.Label( memo.ObjectName );
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;

                if( !InVisible ) {
                    // memo
                    GUI.backgroundColor = GUIHelper.Colors.TransparentColor;
                    scrollView = EditorGUILayout.BeginScrollView( scrollView, GUIHelper.Styles.NoSpaceBox );
                    {
                        GUIHelper.Styles.MemoLabel.normal.textColor = GUIHelper.Colors.SceneMemoTextColor( memo.TextCol );
                        GUILayout.Label( memo.Memo, GUIHelper.Styles.MemoLabel );
                        GUIHelper.Styles.MemoLabel.normal.textColor = GUIHelper.Colors.DefaultTextColor;
                    }
                    EditorGUILayout.EndScrollView();
                    GUI.backgroundColor = Color.white;
                }
            }
            EditorGUILayout.EndVertical();
        }

        private static void drawComponents( Texture2D[] components ) {
            if( components != null ) {
                GUILayout.Space( 3 );
                for( int i = 0; i < components.Length; i++ )
                    GUILayout.Box( components[ i ], GUIStyle.none, new GUILayoutOption[] { GUILayout.MaxWidth( 16 ), GUILayout.MaxHeight( 16 ) } );
            }
        }

        private static Rect memoRect( UnitySceneMemo memo ) {
            var width       = memo.SceneMemoWidth;
            var height      = memo.SceneMemoHeight;
            var sceneWidth  = SceneView.lastActiveSceneView.position.width;
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