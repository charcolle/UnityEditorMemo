using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

using UndoHelper = charcolle.UnityEditorMemo.UndoHelper;

namespace charcolle.UnityEditorMemo {

    [InitializeOnLoad]
    internal static class UnitySceneMemoHierarchyView {

        static UnitySceneMemoHierarchyView() {
            if ( !UnityEditorMemoPrefs.UnitySceneMemoActive )
                return;

            UnitySceneMemoHelper.Initialize();
            if ( UnitySceneMemoHelper.Data == null )
                return;

            EditorApplication.hierarchyWindowItemOnGUI += OnGUI;
            Undo.undoRedoPerformed += () => {
                EditorApplication.RepaintHierarchyWindow();
                for ( int i = 0; i < EditorSceneManager.sceneCount; i++ )
                    UnitySceneMemoHelper.InitializeSceneMemo( EditorSceneManager.GetSceneAt( i ) );
            };
            SceneView.onSceneGUIDelegate += ( view ) => {
                UnitySceneMemoSceneView.DrawMemo( currentMemo );
            };
        }

        private static UnitySceneMemo currentMemo;
        public static void OnGUI( int instanceID, Rect selectionRect ) {
            if ( Application.isPlaying )
                return;
            var obj = EditorUtility.InstanceIDToObject( instanceID );
            if ( obj == null )
                return;

            var localIdentifier = UnitySceneMemoHelper.GetLocalIdentifierInFile( obj );
            if ( localIdentifier == 0 )
                return;
            if ( IsNoSelection )
                currentMemo = null;
            var gameObject = obj as GameObject;
            var buttonRect = ButtonRect( selectionRect, gameObject.transform.childCount > 0 );
            var isSelected = CheckSelected( instanceID );

            var memo = UnitySceneMemoHelper.GetMemo( gameObject, localIdentifier );
            if ( memo == null ) {
                if ( isSelected ) {
                    if ( GUI.Button( buttonRect, "" ) ) {
                        UndoHelper.SceneMemoUndo( UndoHelper.UNDO_SCENEMEMO_ADD );
                        UnitySceneMemoHelper.AddMemo( obj as GameObject, localIdentifier );
                    }
                    GUI.Label( buttonRect, "+" );
                }
            } else {
                if ( isSelected )
                    currentMemo = memo;

                GUI.color = GUIHelper.Colors.LabelColor( memo.Label );
                GUI.DrawTexture( buttonRect, GUIHelper.Textures.Balloon );
                if ( GUI.Button( buttonRect, "", GUIStyle.none ) ) {
                    UnitySceneMemoHelper.PopupWindowContent.Initialize( memo );
                    PopupWindow.Show( selectionRect, UnitySceneMemoHelper.PopupWindowContent );
                }
                GUI.color = Color.white;
                SceneView.RepaintAll();
            }

        }

        //======================================================================
        // private
        //======================================================================

        private static Rect ButtonRect( Rect rect, bool hasChild ) {
            var buttonRect    = rect;
            buttonRect.width  = 15;
            buttonRect.height = 15;
            buttonRect.x -= hasChild ? 29 : 17;
            return buttonRect;
        }

        private static bool CheckSelected( int instanceID ) {
            var selection = Selection.gameObjects;
            for ( int i = 0; i < selection.Length; i++ ) {
                if ( selection[i].GetInstanceID() == instanceID )
                    return true;
            }
            return false;
        }

        private static bool IsNoSelection {
            get {
                return Selection.gameObjects.Length == 0;
            }
        }

    }

}