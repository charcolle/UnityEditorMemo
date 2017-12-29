using System;
using UnityEngine;
using UnityEditor;

using UndoHelper   = charcolle.UnityEditorMemo.UndoHelper;
using GUIHelper    = charcolle.UnityEditorMemo.GUIHelper;
using WindowHelper = charcolle.UnityEditorMemo.UnityEditorMemoWindowHelper;

namespace charcolle.UnityEditorMemo {

    public class UnityEditorMemoWindow: EditorWindow {

        public static UnityEditorMemoWindow win;

        //======================================================================
        // Window Process
        //======================================================================

        [MenuItem( "Window/UnityEditorMemo" )]
        private static void OpenWindow() {
            win                   = GetWindow<UnityEditorMemoWindow>();
            win.minSize           = WindowHelper.WINDOW_SIZE;
            win.titleContent.text = WindowHelper.WINDOW_TITLE;
            Initialize();

            Undo.undoRedoPerformed -= Initialize;
            Undo.undoRedoPerformed += Initialize;
        }

        static void Initialize() {
            WindowHelper.Initialize( win.position );
            if ( win != null )
                win.Repaint();
        }

        private void OnDestroy() {
            Undo.undoRedoPerformed -= Initialize;
        }

        void OnFocus() {
            if( WindowHelper.Data != null )
                EditorUtility.SetDirty( WindowHelper.Data );
        }

        void OnGUI() {
            if ( win == null ) {
                OpenWindow();
                if ( WindowHelper.CurCategory( selectCategoryId ) != null )
                    WindowHelper.CurCategory( selectCategoryId ).Initialize();
            }

            EditorGUI.BeginChangeCheck();
            WindowHelper.OnGUIFirst( position.width );

            DrawContents();
            EventProcess( Event.current );

            WindowHelper.OnGUIEnd();

            if ( WindowHelper.Data != null && EditorGUI.EndChangeCheck() )
                EditorUtility.SetDirty( WindowHelper.Data );
        }

        //======================================================================
        // OnGUI Contents
        //======================================================================

        #region gui contents
        [SerializeField]
        private int selectCategoryId;
        [SerializeField]
        private int selectMode;
        [SerializeField]
        private int selectLabel;
        private Vector2 memoScrollView;
        void DrawContents() {
            if ( WindowHelper.Data == null || WindowHelper.CurCategory( selectCategoryId ) == null ) {
                EditorGUILayout.HelpBox( "fatal Error.", MessageType.Error );
                selectCategoryId = 0;
                return;
            }

            Header();
            if ( selectMode == 0 ) {
                UnityEditorMemoSplitterGUI.BeginVerticalSplit( WindowHelper.VerticalState );
                {
                    MemoContents();
                    PostContents();
                }
                UnityEditorMemoSplitterGUI.EndVerticalSplit();

            } else if ( selectMode == 1 ) {
                CategoryContents();
                LabelConfigContents();
            }
        }

        #region header
        void Header() {
            var mode = 0;
            EditorGUILayout.BeginHorizontal();
            {
                mode = GUILayout.Toolbar( selectMode, GUIHelper.Textures.Menu, EditorStyles.toolbarButton );
            }
            EditorGUILayout.EndHorizontal();
            if ( mode != selectMode ) {
                categoryEdit = false;
                GUIUtility.keyboardControl = 0;
            }
            selectMode = mode;
        }
        #endregion

        #region memo contents
        void CategoryMenu() {
            var selectedId = 0;
            EditorGUILayout.BeginHorizontal( GUIHelper.Styles.NoSpaceBox, GUILayout.ExpandWidth( true ) );
            {
                GUILayout.Label( "Category".ToBold() );
                GUI.backgroundColor = Color.yellow;
                selectedId          = EditorGUILayout.Popup( selectCategoryId, WindowHelper.Data.CategoryList, EditorStyles.toolbarPopup );
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();

            if ( selectCategoryId != selectedId ) {
                Undo.IncrementCurrentGroup();
                UndoHelper.WindowUndo( UndoHelper.UNDO_CHANGE_CATEGORY );
                GUIUtility.keyboardControl = 0;
                selectLabel                = 0;
                footerToggle               = new bool[]{ true, false, false, false, false, false };
                WindowHelper.CurCategory( selectCategoryId ).OnCategoryChange();
            }
            selectCategoryId = selectedId;
        }

        private int displayMemoMode = 0;
        private static readonly string[] displayMemoModeTexts = new string[] { "lastest 100", "101-" };
        void MemoContents() {
            var curCategory = WindowHelper.CurCategory( selectCategoryId );
            var memos = WindowHelper.DisplayMemoList( curCategory, selectLabel, displayMemoMode );
            EditorGUILayout.BeginVertical();
            {
                CategoryMenu();

                GUILayout.Space( 5 );

                if ( curCategory.IsDevideMemo( selectLabel ) ) {
                    displayMemoMode = GUILayout.Toolbar( displayMemoMode, displayMemoModeTexts, EditorStyles.toolbarButton );
                } else {
                    displayMemoMode = 0;
                }

                if ( curCategory.Memo.Count == 0 ) {
                    EditorGUILayout.HelpBox( WindowHelper.TEXT_NO_MEMO, MessageType.Warning );
                } else if( memos.Count > 0 ){
                    //try { // this cause erro!:(
                    memoScrollView = EditorGUILayout.BeginScrollView( memoScrollView );
                    {
                        for ( int i = 0; i < memos.Count; i++ )
                            memos[i].OnGUI();
                    }
                    EditorGUILayout.EndScrollView();
                    //} catch { }
                }

                Footer();
            }
            EditorGUILayout.EndVertical();
        }

        private bool[] footerToggle = { true, false, false, false, false, false };
        void Footer() {
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();
            {
                var curToggles = new bool[6];
                footerToggle.CopyTo( curToggles, 0 );

                curToggles[0]       = GUILayout.Toggle( curToggles[0], "all", EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 1 );
                curToggles[1]       = GUILayout.Toggle( curToggles[1], WindowHelper.Data.LabelTag[0], EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 2 );
                curToggles[2]       = GUILayout.Toggle( curToggles[2], WindowHelper.Data.LabelTag[1], EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 3 );
                curToggles[3]       = GUILayout.Toggle( curToggles[3], WindowHelper.Data.LabelTag[2], EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 4 );
                curToggles[4]       = GUILayout.Toggle( curToggles[4], WindowHelper.Data.LabelTag[3], EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 5 );
                curToggles[5]       = GUILayout.Toggle( curToggles[5], WindowHelper.Data.LabelTag[4], EditorStyles.toolbarButton );
                GUI.backgroundColor = Color.white;
                var label = WindowHelper.ChangeFooterStatus( selectLabel, ref curToggles );
                if ( label != selectLabel ) {
                    //Undo.IncrementCurrentGroup();
                    //UndoHelper.WindowUndo( UndoHelper.UNDO_CHANGE_LABEL ); // avoid error. why? :(
                    postMemoLabel = label;
                    GUIUtility.keyboardControl = 0;
                }
                selectLabel  = label;
                footerToggle = curToggles;
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion

        #region post contents
        [SerializeField]
        private string memoText     = "";
        private int postMemoLabel   = 0;
        private int postMemoTex     = 0;
        /// <summary>
        /// display posting area
        /// </summary>
        void PostContents() {
            var category = WindowHelper.CurCategory( selectCategoryId );
            EditorGUILayout.BeginVertical( new GUILayoutOption[] { GUILayout.ExpandHeight( true ), GUILayout.ExpandWidth( true ) } );
            {
                GUILayout.Box( "", GUIHelper.Styles.NoSpaceBox, new GUILayoutOption[] { GUILayout.Height( 2 ), GUILayout.ExpandWidth( true ) } );
                GUILayout.Space( 5 );
                GUILayout.Label( ( WindowHelper.TEXT_CREATEMEMO_TITLE + category.Name ).ToMiddleBold() );
                EditorGUILayout.BeginVertical();
                {
                    // date
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label( DateTime.Now.RenderDate(), GUIHelper.Styles.MemoBox, new GUILayoutOption[] { GUILayout.Width( 150 ), GUILayout.Height( 25 ) } );
                        postMemoLabel = EditorGUILayout.Popup( postMemoLabel, GUIHelper.Label, GUILayout.Width( 100 ) );
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space( 5 );

                    // draft
                    Undo.IncrementCurrentGroup();
                    UndoHelper.WindowUndo( UndoHelper.UNDO_DRAFT );
                    memoText = EditorGUILayout.TextArea( memoText, GUIHelper.Styles.TextAreaWordWrap, new GUILayoutOption[] { GUILayout.MaxHeight( 300 ) } );
                    EditorGUILayout.BeginHorizontal();
                    {
                        postMemoTex = GUILayout.Toolbar( postMemoTex, GUIHelper.Textures.Emotions, new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 150 ) } );
                        GUILayout.FlexibleSpace();

                        //if ( GUILayout.Button( "test", new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 50 ) } ) ) {
                        //    for( int i = 0; i < 110; i++ ) {
                        //        category.AddMemo( new UnityEditorMemo( i.ToString(), postMemoLabel, postMemoTex ) );
                        //    }
                        //}

                        // post button
                            GUI.backgroundColor = Color.cyan;
                        if ( GUILayout.Button( "Post", new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 100 ) } ) ) {
                            if ( !string.IsNullOrEmpty( memoText ) ) {
                                Undo.IncrementCurrentGroup();
                                UndoHelper.EditorMemoUndo( UndoHelper.UNDO_POST );

                                category.AddMemo( new UnityEditorMemo( memoText, postMemoLabel, postMemoTex ) );
                                memoText = "";
                                postMemoLabel = 0;
                                postMemoTex = 0;
                                memoScrollView = Vector2.zero;
                                GUIUtility.keyboardControl = 0;
                            } else {
                                Debug.LogWarning( WindowHelper.WARNING_MEMO_EMPTY );
                            }
                        }
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space( 5 );
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region category contents
        private string categoryName        = "";
        private bool categoryEdit          = false;
        private Vector2 categoryScrollView = Vector2.zero;
        void CategoryContents() {
            GUILayout.Label( WindowHelper.TEXT_CATEGORY_DESC );
            EditorGUILayout.BeginHorizontal();
            {
                GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
                categoryName = EditorGUILayout.TextField( categoryName, new GUILayoutOption[] { GUILayout.Height( 30 )  } );
                GUI.skin.textField.alignment = TextAnchor.UpperLeft;

                GUI.backgroundColor = Color.green;
                if ( GUILayout.Button( "Register", new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 100 ) } ) ) {
                    if( string.IsNullOrEmpty( categoryName ) ) {
                        Debug.LogWarning( WindowHelper.WARNING_CATEGORY_EMPTY );
                    } else {
                        UndoHelper.EditorMemoUndo( UndoHelper.UNDO_CREATE_CATEGORY );
                        WindowHelper.Data.AddCategory( new UnityEditorMemoCategory( categoryName ) );
                        categoryName = "";
                        GUIUtility.keyboardControl = 0;
                        EditorUtility.SetDirty( WindowHelper.Data );
                        AssetDatabase.SaveAssets();
                    }
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();


            GUILayout.Space( 5 );

            categoryScrollView = EditorGUILayout.BeginScrollView( categoryScrollView );
            {
                GUI.backgroundColor = Color.grey;
                EditorGUILayout.BeginHorizontal( GUI.skin.box, new GUILayoutOption[] { GUILayout.Height( 20 ), GUILayout.ExpandWidth( true ) } );
                {
                    GUILayout.Label( "CategoryName", GUILayout.ExpandWidth( true ) );
                    GUILayout.Label( "Num", GUILayout.Width( 50 ) );
                    GUILayout.Label( "Last Posted", GUILayout.Width( 120 ) );
                    var isEdit = GUILayout.Toggle( categoryEdit, "≡", EditorStyles.toolbarButton, GUILayout.Width( 20 ) );
                    if( isEdit != categoryEdit ) {
                        UndoHelper.EditorMemoUndo( UndoHelper.UNDO_EDIT_CATEGORY );
                        if ( !isEdit ) {
                            WindowHelper.Data.SortCategory();
                            EditorUtility.SetDirty( WindowHelper.Data );
                            AssetDatabase.SaveAssets();
                        }
                        GUIUtility.keyboardControl = 0;
                    }
                    categoryEdit = isEdit;
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;

                try {
                    for ( int i = 0; i < WindowHelper.Data.Category.Count; i++ ) {
                        var category = WindowHelper.Data.Category[i];
                        EditorGUILayout.BeginHorizontal();
                        {
                            category.OnGUI( categoryEdit );
                            if ( !category.Name.Equals( "default" ) ) {
                                GUI.backgroundColor = Color.red;
                                if ( GUILayout.Button( "x", new GUILayoutOption[] { GUILayout.Width( 20 ) } ) ) {
                                    UndoHelper.EditorMemoUndo( UndoHelper.UNDO_DELETE_CATEGORY );
                                    WindowHelper.Data.Category.Remove( category );
                                    EditorUtility.SetDirty( WindowHelper.Data );
                                    AssetDatabase.SaveAssets();
                                }
                                GUI.backgroundColor = Color.white;
                            } else {
                                GUILayout.Space( 23 );
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                } catch { }
            }
            EditorGUILayout.EndScrollView();

            GUILayout.Space( 7 );
        }
        #endregion

        #region label config
        void LabelConfigContents() {
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Box( "", GUIHelper.Styles.NoSpaceBox, new GUILayoutOption[] { GUILayout.Height( 2 ), GUILayout.ExpandWidth( true ) } );

                GUILayout.Label( WindowHelper.TEXT_LABEL_LIST );

                GUILayout.Space( 3 );

                var labels = WindowHelper.Data.LabelTag;
                for( int i = 0; i < labels.Length; i++ ) {
                    EditorGUILayout.BeginHorizontal( GUI.skin.box, new GUILayoutOption[] { GUILayout.Height( 20 ), GUILayout.ExpandWidth( true ) } );
                    {
                        GUI.backgroundColor = GUIHelper.Colors.LabelColor( i + 1 );
                        GUILayout.Toggle( false, "", EditorStyles.toolbarButton, GUILayout.Width( 50 ) );
                        labels[i] = EditorGUILayout.TextField( labels[i] );
                    }
                    EditorGUILayout.EndHorizontal();
                    GUI.backgroundColor = Color.white;
                }
            }
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
        }
        #endregion

        #endregion

        //======================================================================
        // Event Process
        //======================================================================

        #region event process
        void EventProcess( Event e ) {
            if ( WindowHelper.Data == null || WindowHelper.CurCategory( selectCategoryId ) == null )
                return;

            var memos = WindowHelper.CurCategory( selectCategoryId ).Memo;
            for ( int i = 0; i < memos.Count; i++ ) {
                var memo = memos[i];
                if ( memo.IsContextClick ) {
                    var menu = new GenericMenu();
                    menu.AddItem( new GUIContent( "Edit" ), false, () => {
                        memo.isFold = true;
                    } );
                    menu.AddItem( new GUIContent( "Delete" ), false, () => {
                        UndoHelper.EditorMemoUndo( UndoHelper.UNDO_DELETE_MEMO );
                        WindowHelper.CurCategory( selectCategoryId ).Memo.Remove( memo );
                    } );
                    menu.ShowAsContext();
                    break;
                }
            }

        }

        #endregion

    }
}