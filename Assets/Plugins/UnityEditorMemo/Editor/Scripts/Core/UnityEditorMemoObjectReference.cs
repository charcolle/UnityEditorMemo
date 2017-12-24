using System;
using UnityEngine;
using UnityEditor;

using Object     = UnityEngine.Object;
using UndoHelper = charcolle.UnityEditorMemo.UndoHelper;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    public class UnityEditorMemoObject {

        public Object Obj;

        public string ScenePath;
        public int LocalIdentifierInFile;

        public UnitySceneMemo SceneMemo;
        private bool tryFlag = false;

        public void Initialize() {
            tryFlag = false;
        }

        public void SetReference( Object obj ) {
            tryFlag = false;

            var path = AssetDatabase.GetAssetPath( obj );
            if ( string.IsNullOrEmpty( path ) ) {
                var go = obj as GameObject;
                SceneMemo = UnitySceneMemoHelper.GetMemo( go );
                if( SceneMemo != null ) {
                    ScenePath             = go.scene.path;
                    LocalIdentifierInFile = SceneMemo.LocalIdentifierInFile;
                }
                Obj = null;
            } else {
                Obj = obj;
                ScenePath = "";
                LocalIdentifierInFile = 0;
            }
        }

        public void Draw( bool isFold ) {
            if ( Obj != null ) {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space( 41 );
                    UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                    Obj = EditorGUILayout.ObjectField( "", Obj, typeof( Object ), true );
                    if( isFold ) {
                        if( GUILayout.Button( "clear", GUILayout.Width( 70 ) ) ) {
                            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                            clearReference();
                            GUIUtility.keyboardControl = 0;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            } else if( isSceneMemoValid ) {
                var rect = EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space( 41 );
                    UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                    SceneMemo.DrawTexture();
                    if( GUILayout.Button( SceneMemo.Name, GUI.skin.label ) ) {
                        SceneMemo.SelectObject();
                        UnitySceneMemoHelper.PopupWindowContent.Initialize( SceneMemo );
                        PopupWindow.Show( rect, UnitySceneMemoHelper.PopupWindowContent );
                    }
                    if ( isFold ) {
                        if ( GUILayout.Button( "clear", GUILayout.Width( 70 ) ) ) {
                            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                            clearReference();
                            GUIUtility.keyboardControl = 0;
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        //public void OnClick() {
        //    if( isSceneMemoValid ) {
        //        SceneMemo.SelectObject();
        //    } else {
        //        Selection.activeObject = Obj;
        //        EditorGUIUtility.PingObject( Obj );
        //    }
        //}

        private void clearReference() {
            Obj = null;
            ScenePath = null;
            SceneMemo = null;
            LocalIdentifierInFile = 0;
        }

        private bool isSceneMemoValid {
            get {
                if ( !tryFlag ) {
                    SceneMemo = UnitySceneMemoHelper.GetMemo( ScenePath, LocalIdentifierInFile );
                    if( SceneMemo == null ) {
                        ScenePath = "";
                        LocalIdentifierInFile = 0;
                    }
                    tryFlag = true;
                }
                return SceneMemo != null && SceneMemo.LocalIdentifierInFile != 0;
            }
        }


    }

}