using System;
using UnityEngine;
using UnityEditor;

using GUIHelper  = charcolle.UnityEditorMemo.GUIHelper;
using UndoHelper = charcolle.UnityEditorMemo.UndoHelper;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    internal class UnitySceneMemoHierarchyEditorItem : EditorWindowItem<UnitySceneMemo> {

        public bool IsEdit;
        public bool IsContextClick = false;

        internal UnitySceneMemoHierarchyEditorItem( UnitySceneMemo data ) : base( data ) {

        }

        protected override void Draw() {
            DrawProcess();
        }

        //======================================================================
        // drawer
        //======================================================================

        private Rect rect = Rect.zero;
        private Vector2 scrollView = Vector2.zero;
        private void DrawProcess() {
            rect = EditorGUILayout.BeginVertical();
            {
                // header
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( Label );
                EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
                {
                    var edit = GUILayout.Toggle( IsEdit, "≡", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 18 ) } );
                    if( edit != IsEdit ) {
                        GUIUtility.keyboardControl = 0;
                        IsEdit = edit;
                    }
                    EditorGUILayout.BeginHorizontal();
                    {
                        drawComponents( data.Components );
                        GUILayout.Label( Name );
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.FlexibleSpace();
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;

                // memo
                scrollView = EditorGUILayout.BeginScrollView( scrollView );
                if( IsEdit ) {
                    Undo.IncrementCurrentGroup();
                    UndoHelper.SceneMemoUndo( UndoHelper.UNDO_SCENEMEMO_EDIT );
                    Memo = EditorGUILayout.TextArea( Memo, GUIHelper.Styles.TextAreaWordWrap, new GUILayoutOption[] { GUILayout.ExpandWidth( true ), GUILayout.ExpandHeight( true ) } );
                } else {
                    GUILayout.Label( Memo, GUIHelper.Styles.MemoLabel );
                }
                EditorGUILayout.EndScrollView();

                // footer
                if( IsEdit ) {
                    EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
                    {
                        GUILayout.FlexibleSpace();
                        var showAtScene = GUILayout.Toggle( ShowAtScene, "ShowAtScene", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 80 ) } );
                        if( showAtScene != ShowAtScene ) {
                            SceneView.RepaintAll();
                            ShowAtScene = showAtScene;
                        }
                        GUI.backgroundColor = GUIHelper.Colors.LabelColor( Label );
                        Label = ( UnityEditorMemoLabel )EditorGUILayout.Popup( ( int )Label, GUIHelper.LabelMenu, EditorStyles.toolbarDropDown, GUILayout.Width( 70 ) );
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

        private static void drawComponents( Texture2D[] components ) {
            if( components != null ) {
                GUILayout.Space( 3 );
                for( int i = 0; i < components.Length; i++ )
                    GUILayout.Box( components[ i ], GUIStyle.none, new GUILayoutOption[] { GUILayout.MaxWidth( 16 ), GUILayout.MaxHeight( 16 ) } );
            }
        }

        private bool eventProcess( Event e ) {
            switch( e.type ) {
                case EventType.ContextClick:
                    if( rect.Contains( e.mousePosition ) )
                        return true;
                    else
                        return false;
            }
            return false;
        }

        #region property

        //======================================================================
        // property
        //======================================================================

        public string Date {
            get {
                return data.Date;
            }
            set {
                data.Date = value;
            }
        }

        public string Memo {
            get {
                return data.Memo;
            }
            set {
                data.Memo = value;
            }
        }

        public UnityEditorMemoLabel Label {
            get {
                return data.Label;
            }
            set {
                data.Label = value;
            }
        }

        public UnityEditorMemoTexture Tex {
            get {
                return data.Tex;
            }
            set {
                data.Tex = value;
            }
        }

        public UnitySceneMemoTextColor TextCol {
            get {
                return data.TextCol;
            }
            set {
                data.TextCol = value;
            }
        }

        public bool ShowAtScene {
            get {
                return data.ShowAtScene;
            }
            set {
                data.ShowAtScene = value;
            }
        }

        public int LocalIdentifierInFile {
            get {
                return data.LocalIdentifierInFile;
            }
            set {
                data.LocalIdentifierInFile = value;
            }
        }

        public string SceneGuid {
            get {
                return data.SceneGuid;
            }
            set {
                data.SceneGuid = value;
            }
        }

        public string Name {
            get {
                return data.ObjectName;
            }
            set {
                data.ObjectName = value;
            }
        }

        public float SceneMemoWidth {
            get {
                return data.SceneMemoWidth;
            }
            set {
                data.SceneMemoWidth = value;
            }
        }

        public float SceneMemoHeight {
            get {
                return data.SceneMemoHeight;
            }
            set {
                data.SceneMemoHeight = value;
            }
        }


        public int InstanceId {
            get {
                return data.InstanceId;
            }
            set {
                data.InstanceId = value;
            }
        }

        #endregion

    }

}