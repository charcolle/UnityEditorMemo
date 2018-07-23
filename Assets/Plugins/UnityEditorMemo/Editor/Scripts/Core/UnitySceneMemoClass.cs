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
        public UnitySceneMemoTextColor TextCol;
        public bool ShowAtScene = false;

        public int LocalIdentifierInFile;
        public string SceneGuid;
        public string Name;

        public float SceneMemoWidth  = 270f;
        public float SceneMemoHeight = 150f;

        [NonSerialized]
        public int InstanceId;
        [NonSerialized]
        public bool IsEdit;
        [NonSerialized]
        public bool InVisible;
        [NonSerialized]
        public bool CheckExist;

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
                EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
                {
                    var edit = GUILayout.Toggle( IsEdit, "≡", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 18 ) } );
                    if( edit != IsEdit ) {
                        GUIUtility.keyboardControl = 0;
                    }
                    IsEdit = edit;
                    EditorGUILayout.BeginHorizontal();
                    {
                        DrawTexture();
                        GUILayout.Label( Name );
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;

                // memo
                scrollView = EditorGUILayout.BeginScrollView( scrollView );
                if ( IsEdit ) {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.SceneMemoUndo( UndoHelper.UNDO_SCENEMEMO_EDIT );
                    Memo = EditorGUILayout.TextArea( Memo, GUIHelper.Styles.TextAreaWordWrap, new GUILayoutOption[] { GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) } );
                } else {
                    GUILayout.Label( Memo, GUIHelper.Styles.LabelWordWrap );
                }
                EditorGUILayout.EndScrollView();

                // footer
                if( IsEdit ) {
                    EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
                    {
                        GUILayout.FlexibleSpace();
                        ShowAtScene = GUILayout.Toggle( ShowAtScene, "ShowAtScene", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 80 ) } );
                        GUI.backgroundColor = GUIHelper.Colors.LabelColor( Label );
                        Label       = ( UnityEditorMemoLabel )EditorGUILayout.Popup( ( int )Label, GUIHelper.LabelMenu, EditorStyles.toolbarDropDown, GUILayout.Width( 70 ) );
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                    if( ShowAtScene ) {
                        GUILayout.Space( 3 );

                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label( "Width" );
                            SceneMemoWidth = EditorGUILayout.Slider( SceneMemoWidth, 200, 500 );
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label( "Height" );
                            SceneMemoHeight = EditorGUILayout.Slider( SceneMemoHeight, 100, 500 );
                        }
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Label( "TextColor" );
                            TextCol = ( UnitySceneMemoTextColor )EditorGUILayout.Popup( ( int )TextCol, GUIHelper.TextColorMenu, GUILayout.Width( 60 ) );
                            GUILayout.FlexibleSpace();
                        }
                        EditorGUILayout.EndVertical();
                    }
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
                EditorGUILayout.BeginHorizontal( GUIHelper.Styles.MemoHeader );
                {
                    if( GUILayout.Button( InVisible ? "●" : "x", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 18 ) } ) ) {
                        Undo.IncrementCurrentGroup();
                        UndoHelper.SceneMemoUndo( UndoHelper.UNDO_SCENEMEMO_EDIT );
                        InVisible = !InVisible;
                    }
                    EditorGUILayout.BeginHorizontal();
                    {
                        DrawTexture();
                        GUILayout.Label( Name );
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
                        GUIHelper.Styles.LabelWordWrap.normal.textColor = GUIHelper.Colors.SceneMemoTextColor( TextCol );
                        GUILayout.Label( Memo, GUIHelper.Styles.LabelWordWrap );
                        GUIHelper.Styles.LabelWordWrap.normal.textColor = GUIHelper.Colors.DefaultTextColor;
                    }
                    EditorGUILayout.EndScrollView();
                    GUI.backgroundColor = Color.white;
                }
            }
            EditorGUILayout.EndVertical();
        }

        public void DrawTexture() {
            if ( Components != null ) {
                GUILayout.Space( 3 );
                for ( int i = 0; i < Components.Length; i++ )
                    GUILayout.Box( Components[i], GUIStyle.none, new GUILayoutOption[] { GUILayout.MaxWidth( 16 ), GUILayout.MaxHeight( 16 ) } );
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