using System;
using System.Linq;
using UnityEngine;
using UnityEditor;

using GUIHelper  = charcolle.UnityEditorMemo.GUIHelper;
using UndoHelper = charcolle.UnityEditorMemo.UndoHelper;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    public class UnitySceneMemo {

        public string Date;
        public string Memo;
        public UnityEditorMemoLabel Label;
        public UnityEditorMemoTexture Tex;
        public bool ShowAtScene = false;

        public int LocalIdentifierInFile;
        public string SceneGuid;
        public string Name;

        [NonSerialized]
        public int InstanceId;
        [NonSerialized]
        public bool isEdit;
        [NonSerialized]
        public bool checkExist;

        private Texture2D[] Components;

        public UnitySceneMemo( int localIdentifierInFile, string sceneGuid ) {
            Date                  = DateTime.Now.RenderDate();
            SceneGuid             = sceneGuid;
            LocalIdentifierInFile = localIdentifierInFile;
            ShowAtScene           = false;
        }

        public void Initialize( int instanceId ) {
            var obj = EditorUtility.InstanceIDToObject( instanceId ) as GameObject;
            if ( obj == null )
                return;
            Name           = obj.name;
            InstanceId     = instanceId;
            var components = obj.GetComponents<Component>().Where( c => c != null ).Where( c => !( c is Transform ) ).Reverse();
            Components     = components.Select( c => AssetPreview.GetMiniThumbnail( c ) ).Where( t => t != null ).ToArray();
        }

        public void SelectObject() {
            var obj = EditorUtility.InstanceIDToObject( InstanceId );
            if( obj != null ) {
                Selection.activeObject = obj;
                EditorGUIUtility.PingObject( obj );
            }
        }

        public static implicit operator int( UnitySceneMemo memo ) {
            return memo.LocalIdentifierInFile;
        }

        //=======================================================
        // Drawer
        //=======================================================
        [NonSerialized]
        public bool IsContextClick  = false;
        private Rect rect           = Rect.zero;
        private Vector2 scrollView  = Vector2.zero;
        public void OnGUI() {
            rect = EditorGUILayout.BeginVertical();
            {
                // header
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( Label );
                EditorGUILayout.BeginHorizontal( GUIHelper.Styles.MemoHeader, new GUILayoutOption[] { GUILayout.ExpandWidth( true ) } );
                {
                    DrawTexture();
                    GUILayout.Label( Name, GUIHelper.Styles.LabelWordWrap );
                    GUILayout.FlexibleSpace();
                    var edit = GUILayout.Toggle( isEdit, "≡", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 18 ) } );
                    if( edit != isEdit ) {
                        GUIUtility.keyboardControl = 0;
                    }
                    isEdit = edit;
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;

                // memo
                scrollView = EditorGUILayout.BeginScrollView( scrollView );
                if ( isEdit ) {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.SceneMemoUndo( UndoHelper.UNDO_SCENEMEMO_EDIT );
                    Memo = EditorGUILayout.TextArea( Memo, GUIHelper.Styles.TextAreaWordWrap, new GUILayoutOption[] { GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) } );
                } else {
                    GUILayout.Label( Memo, GUIHelper.Styles.LabelWordWrap );
                }
                EditorGUILayout.EndScrollView();

                // footer
                if( isEdit ) {
                    GUILayout.Space( 5 );
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        ShowAtScene = GUILayout.Toggle( ShowAtScene, "ShowAtScene", GUI.skin.button, GUILayout.Width( 100f ) );
                        Label       = ( UnityEditorMemoLabel )EditorGUILayout.Popup( ( int )Label, GUIHelper.Label, GUILayout.Width( 70 ) );
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space( 5 );
                }
            }
            EditorGUILayout.EndVertical();

            IsContextClick = eventProcess( Event.current );
        }

        public void OnSceneGUI() {
            EditorGUILayout.BeginVertical();
            {
                // header
                GUI.backgroundColor = GUIHelper.Colors.SceneMemoLabelColor( Label );
                EditorGUILayout.BeginHorizontal( GUIHelper.Styles.MemoHeader, new GUILayoutOption[] { GUILayout.ExpandWidth( true ) } );
                {
                    DrawTexture();
                    GUILayout.Label( Name, GUIHelper.Styles.LabelWordWrap );
                    GUILayout.FlexibleSpace();
                    if( GUILayout.Button( "x", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 18 ) } ) ) {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.SceneMemoUndo( UndoHelper.UNDO_SCENEMEMO_EDIT );
                        ShowAtScene = false;
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;

                // memo
                GUI.backgroundColor = GUIHelper.Colors.TransparentColor;
                scrollView = EditorGUILayout.BeginScrollView( scrollView, GUIHelper.Styles.NoSpaceBox );
                {
                    GUILayout.Label( Memo, GUIHelper.Styles.LabelWordWrap );
                }
                EditorGUILayout.EndScrollView();
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndVertical();
        }

        public void DrawTexture() {
            if ( Components != null ) {
                GUILayout.Space( 3 );
                for ( int i = 0; i < Components.Length; i++ )
                    GUILayout.Box( Components[i], GUIStyle.none, new GUILayoutOption[] { GUILayout.Width( 16 ), GUILayout.Height( 16 ) } );
            }
        }

        private bool eventProcess( Event e ) {
            switch ( e.type ) {
                case EventType.ContextClick:
                    if ( rect.Contains( e.mousePosition ) )
                        return true;
                    else
                        return false;
            }
            return false;
        }

    }

}