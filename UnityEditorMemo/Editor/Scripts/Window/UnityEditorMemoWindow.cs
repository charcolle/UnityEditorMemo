using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UndoHelper   = charcolle.UnityEditorMemo.UndoHelper;
using GUIHelper    = charcolle.UnityEditorMemo.GUIHelper;
using WindowHelper = charcolle.UnityEditorMemo.UnityEditorMemoWindowHelper;
using SlackHelper  = charcolle.UnityEditorMemo.SlackHelper;

namespace charcolle.UnityEditorMemo {

    internal class UnityEditorMemoWindow : EditorWindow {

        public static UnityEditorMemoWindow win;
        private bool IsInitialized = false;

        [SerializeField]
        private TreeViewState memoTreeViewState;
        private UnityEditorMemoTreeView memoTreeView;
        [SerializeField]
        private TreeViewState categoryTreeViewState;
        private UnityEditorMemoCategoryWindowTreeView categoryTreeView;

        private static SplitterState verticalState;
        private static SplitterState horizontalState;

        [ MenuItem( "Window/UnityEditorMemo" )]
        private static void OpenWindow() {
            win                   = GetWindow<UnityEditorMemoWindow>();
            win.minSize           = WindowHelper.WINDOW_SIZE;
            win.titleContent.text = WindowHelper.WINDOW_TITLE;
            win.Show();
            win.IsInitialized = false;
        }

        private void OnEnable() {
            Undo.undoRedoPerformed -= Initialize;
            Undo.undoRedoPerformed += Initialize;
        }

        private void Initialize() {
            WindowHelper.LoadData();
            var category = WindowHelper.GetCategory( selectCategoryId );
            if( category == null ) {
                selectCategoryId = 0;
                return;
            }
            category.Initialize();

            verticalState = new SplitterState( new float[] { position.height * 0.9f, position.height * 0.1f },
                                                          new int[] { 200, 180 }, new int[] { 1500, 300 } );
            SetHorizontalState();

            CategoryTreeViewInitialize();
            MemoTreeViewInitialize();
            IsInitialized = true;
            EditorGUIUtility.keyboardControl = 0;
            Repaint();
        }

        private void SetHorizontalState() {
            horizontalState = isCategoryVisible ? new SplitterState( new float[] { position.width * 0.3f, position.width * 0.7f },
                                                          new int[] { 50, 100 }, new int[] { 350, 1500 } )
                                                : new SplitterState( new float[] { position.width * 0.0f, position.width },
                                                          new int[] { 2, 300 }, new int[] { 300, 1500 } );
        }

        private void CategoryTreeViewInitialize() {
            if( categoryTreeViewState == null )
                categoryTreeViewState = new TreeViewState();

            if( selectCategoryId >= WindowHelper.Data.Category.Count )
                selectCategoryId = WindowHelper.Data.Category.Count - 1;

            categoryTreeViewState.lastClickedID = selectCategoryId;
            categoryTreeViewState.selectedIDs = new List<int>() { selectCategoryId };
            categoryTreeView = new UnityEditorMemoCategoryWindowTreeView( categoryTreeViewState, WindowHelper.Data.Category );
            categoryTreeView.OnContextClick += OnCategoryContextClicked;
            categoryTreeView.OnCategoryOrderChanged += OnCategoryOrderChanged;
            categoryTreeView.SetSelection( categoryTreeViewState.selectedIDs, TreeViewSelectionOptions.RevealAndFrame );
            categoryTreeView.Reload();
        }

        private void MemoTreeViewInitialize() {
            WindowHelper.CheckMemoHasRootElement( selectCategoryId );

            if( memoTreeViewState == null )
                memoTreeViewState = new TreeViewState();

            var rowRectSize = isCategoryVisible ? ( preMemoWidth == 0 ? position.width * 0.7f : preMemoWidth ) : position.width;
            var treeModel = new TreeModel<UnityEditorMemo>( WindowHelper.GetCategory( selectCategoryId ).Memo );
            memoTreeView = new UnityEditorMemoTreeView( memoTreeViewState, treeModel, rowRectSize );
            memoTreeView.OnContextClicked += OnMemoContextClicked;
            memoTreeView.SelectLabel = ( UnityEditorMemoLabel )selectLabel;
            memoTreeView.Reload();
        }

        void OnGUI() {
            if ( win == null )
                OpenWindow();
            if( !IsInitialized )
                Initialize();

            EditorGUI.BeginChangeCheck();
            WindowHelper.OnGUIFirst( position.width );

            DrawContents();

            WindowHelper.OnGUIEnd();

            if ( WindowHelper.Data != null && EditorGUI.EndChangeCheck() )
                EditorUtility.SetDirty( WindowHelper.Data );
        }

        #region gui contents

        //======================================================================
        // gui contents
        //======================================================================

        [SerializeField]
        private int selectCategoryId;
        [SerializeField]
        private int selectLabel;
        [SerializeField]
        private string searchText;
        [SerializeField]
        private bool isCategoryVisible = true;
        private readonly string[] categoryMenu = new string[] { "Menu", "", "Create New Category", "", "Open Preference", "", "Export Memo", "Import Memo/Override", "Import Memo/Additive" };
        void DrawContents() {
            if ( WindowHelper.Data == null || WindowHelper.GetCategory( selectCategoryId ) == null ) {
                EditorGUILayout.HelpBox( "fatal Error.", MessageType.Error );
                selectCategoryId = 0;
                return;
            }
            HeaderGUI();
            SplitterGUI.BeginVerticalSplit( verticalState );
            {
                MemoGUI();
                PostGUI();
            }
            SplitterGUI.EndVerticalSplit();
        }

        #region header

        void HeaderGUI() {
            EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
            {
                var visible = GUILayout.Toggle( isCategoryVisible, "≡", EditorStyles.toolbarButton, GUILayout.Width( 25 ) );
                if( visible != isCategoryVisible ) {
                    isCategoryVisible = visible;
                    SetHorizontalState();
                    memoTreeView.UpdateRowHeight( isCategoryVisible ? position.width * 0.7f : position.width );
                }
                var selectMenu = EditorGUILayout.Popup( 0, categoryMenu, EditorStyles.toolbarPopup, GUILayout.Width( 60 ) );
                switch( selectMenu ) {
                    case 2:
                        OnCategoryCreate();
                        break;
                    case 4:
#if UNITY_2018_3_OR_NEWER
                        SettingsService.OpenUserPreferences( "Preferences/UnityEditorMemo" );
#else
                        var assembly = Assembly.Load( "UnityEditor" );
                        var type = assembly.GetType( "UnityEditor.PreferencesWindow" );
                        var method = type.GetMethod( "ShowPreferencesWindow", BindingFlags.NonPublic | BindingFlags.Static );
                        method.Invoke( null, null );
#endif
                        break;
                    case 6:
                        OnUnityEditorMemoExport();
                        break;
                    case 7:
                        OnUnityEditorMemoImport( true );
                        break;
                    case 8:
                        OnUnityEditorMemoImport( false );
                        break;
                }

                searchText = EditorGUILayout.TextField( searchText, GUIHelper.Styles.SearchField );
                if( GUILayout.Button( "", GUIHelper.Styles.SearchFieldCancel ) ) {
                    searchText = "";
                    EditorGUIUtility.keyboardControl = 0;
                }
                memoTreeView.searchString = searchText;
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region memo contents

        private float preMemoWidth = 0f;
        void MemoGUI() {
            var memoCount = memoTreeView.GetRows().Count;
            EditorGUILayout.BeginVertical();
            {
                SplitterGUI.BeginHorizontalSplit( horizontalState );
                {
                    // category area
                    EditorGUILayout.BeginVertical();
                    GUILayout.Space( 1 );
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndVertical();

                    categoryTreeView.OnGUI( GUILayoutUtility.GetLastRect() );
                    var selected = categoryTreeView.state.lastClickedID;
                    if( selected != selectCategoryId ) {
                        selectCategoryId = selected;
                        OnCategoryChange();
                    }

                    // memo area
                    EditorGUILayout.BeginVertical();
                    {
                        if( memoCount == 0 ) {
                            EditorGUILayout.HelpBox( WindowHelper.TEXT_NO_MEMO, MessageType.Info );
                            GUILayout.FlexibleSpace();
                        } else {
                            EditorGUILayout.BeginVertical();
                            GUILayout.Space( 2 );
                            GUILayout.FlexibleSpace();
                            EditorGUILayout.EndVertical();

                            var memoRect = GUILayoutUtility.GetLastRect();
                            if( Event.current.type == EventType.Repaint ) {
                                if( preMemoWidth != memoRect.width ) {
                                    memoTreeView.UpdateRowHeight( memoRect.width );
                                    preMemoWidth = memoRect.width;
                                }
                            }
                            memoTreeView.OnGUI( memoRect );
                        }
                        LabelGUI();
                    }
                    EditorGUILayout.EndVertical();
                }
                SplitterGUI.EndHorizontalSplit();

            }
            EditorGUILayout.EndVertical();
        }

        private bool[] footerToggle = { true, false, false, false, false, false };
        void LabelGUI() {
            EditorGUILayout.BeginHorizontal();
            {
                var curToggles = new bool[6];
                footerToggle.CopyTo( curToggles, 0 );

                curToggles[0]       = GUILayout.Toggle( curToggles[0], "all", EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 1 );
                curToggles[1]       = GUILayout.Toggle( curToggles[1], UnityEditorMemoPrefs.Label1, EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 2 );
                curToggles[2]       = GUILayout.Toggle( curToggles[2], UnityEditorMemoPrefs.Label2, EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 3 );
                curToggles[3]       = GUILayout.Toggle( curToggles[3], UnityEditorMemoPrefs.Label3, EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 4 );
                curToggles[4]       = GUILayout.Toggle( curToggles[4], UnityEditorMemoPrefs.Label4, EditorStyles.toolbarButton );
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( 5 );
                curToggles[5]       = GUILayout.Toggle( curToggles[5], UnityEditorMemoPrefs.Label5, EditorStyles.toolbarButton );
                GUI.backgroundColor = Color.white;
                var label = WindowHelper.ChangeFooterStatus( selectLabel, ref curToggles );
                if ( label != selectLabel ) {
                    UndoHelper.WindowUndo( UndoHelper.UNDO_CHANGE_LABEL ); // avoid error. why? :(
                    postMemoLabel = label;
                    selectLabel = label;
                    memoTreeView.SelectLabel = ( UnityEditorMemoLabel )selectLabel;
                    memoTreeView.Reload();
                    GUIUtility.keyboardControl = 0;
                }
                footerToggle = curToggles;
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region post contents

        [SerializeField]
        private string memoText        = "";
        [SerializeField]
        private string postMemoUrl     = "";
        [SerializeField]
        private int postMemoLabel      = 0;
        [SerializeField]
        private int postMemoTex        = 0;
        [SerializeField]
        private bool postToSlack       = false;
        private Vector2 postScrollView = Vector2.zero;
        /// <summary>
        /// display posting area
        /// </summary>
        void PostGUI() {
            var category = WindowHelper.GetCategory( selectCategoryId );
            EditorGUILayout.BeginVertical( new GUILayoutOption[] { GUILayout.ExpandHeight( true ), GUILayout.ExpandWidth( true ) } );
            {
                GUILayout.Box( "", GUIHelper.Styles.NoSpaceBox, new GUILayoutOption[] { GUILayout.Height( 2 ), GUILayout.ExpandWidth( true ) } );

                EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
                {
                    GUILayout.Label( DateTime.Now.RenderDate() );
                    GUILayout.Space( 5 );
                    GUI.backgroundColor = GUIHelper.Colors.LabelColor( postMemoLabel );
                    postMemoLabel = EditorGUILayout.Popup( postMemoLabel, GUIHelper.LabelMenu, EditorStyles.toolbarPopup, GUILayout.Width( 80 ) );
                    GUI.backgroundColor = Color.white;

                    GUILayout.FlexibleSpace();

                    GUILayout.Label( "URL", GUILayout.Width( 30 ) );
                    postMemoUrl = EditorGUILayout.TextField( postMemoUrl, EditorStyles.toolbarTextField );
                }
                EditorGUILayout.EndHorizontal();

                if( UnityEditorMemoPrefs.UnityEditorMemoUseSlack ) {
                    EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
                    {
                        UnityEditorMemoPrefs.UnityEditorMemoSlackChannel = EditorGUILayout.TextField( UnityEditorMemoPrefs.UnityEditorMemoSlackChannel );
                        postToSlack = GUILayout.Toggle( postToSlack, "Post to Slack", EditorStyles.toolbarButton, GUILayout.Width( 100 ) );
                    }
                    EditorGUILayout.EndHorizontal();
                }

                GUILayout.Space( 5 );

                EditorGUILayout.BeginVertical();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label( ( WindowHelper.TEXT_CREATEMEMO_TITLE + category.Name ).ToMiddleBold() );
                        GUILayout.FlexibleSpace();
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space( 5 );

                    postScrollView = EditorGUILayout.BeginScrollView( postScrollView );
                    {
                        // draft
                        var tmp = EditorGUILayout.TextArea( memoText, GUIHelper.Styles.TextAreaWordWrap, new GUILayoutOption[] { GUILayout.MaxHeight( 300 ) } );
                        if( tmp != memoText ) {
                            Undo.IncrementCurrentGroup();
                            UndoHelper.WindowUndo( UndoHelper.UNDO_DRAFT );
                            memoText = tmp;
                        }
                    }
                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.BeginHorizontal();
                    {

                        GUILayout.FlexibleSpace();

                        postMemoTex = GUILayout.Toolbar( postMemoTex, GUIHelper.Textures.Emotions, new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.MaxWidth( 150 ) } );

                        //if ( GUILayout.Button( "test", new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 50 ) } ) ) {
                        //    for( int i = 0; i < 110; i++ ) {
                        //        category.AddMemo( new UnityEditorMemo( i.ToString(), postMemoLabel, postMemoTex ) );
                        //    }
                        //}

                        // post button
                        GUI.backgroundColor = Color.cyan;
                        if ( GUILayout.Button( "Post", new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.MaxWidth( 120 ) } ) ) {
                            Undo.IncrementCurrentGroup();
                            UndoHelper.WindowUndo( UndoHelper.UNDO_POST );
                            if ( !string.IsNullOrEmpty( memoText ) ) {
                                var memo = new UnityEditorMemo( memoText, postMemoLabel, postMemoTex, postMemoUrl );
                                memo.id = category.Memo.Count;
                                if( UnityEditorMemoPrefs.UnityEditorMemoUseSlack && postToSlack ) {
                                    if ( SlackHelper.Post( memo, category.Name ) )
                                        OnMemoPost( category, memo );
                                } else {
                                    OnMemoPost( category, memo );
                                }
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

        #endregion

        #region callback

        private void OnCategoryChange() {
            Undo.IncrementCurrentGroup();
            UndoHelper.WindowUndo( UndoHelper.UNDO_CHANGE_CATEGORY );

            selectLabel = 0;
            footerToggle = new bool[] { true, false, false, false, false, false };
            WindowHelper.GetCategory( selectCategoryId ).Initialize();
            MemoTreeViewInitialize();
            EditorGUIUtility.keyboardControl = 0;
        }

        private void OnCategoryContextClicked( UnityEditorMemoCategory caterogy ) {
            var menu = new GenericMenu();

            if( caterogy.Name != "default" ) {
                menu.AddItem( new GUIContent( "Rename" ), false, () => {
                    categoryTreeView.BeginRename( caterogy );
                } );
                menu.AddSeparator( "" );
                menu.AddItem( new GUIContent( "Delete" ), false, () => {
                    OnCategoryDelete( caterogy );
                } );
            }
            menu.ShowAsContext();
        }

        private void OnCategoryOrderChanged( List<UnityEditorMemoCategory> newCategory ) {
            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_EDIT_CATEGORY );

            WindowHelper.Data.Category = newCategory;
        }

        private void OnCategoryDelete( UnityEditorMemoCategory category ) {
            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_DELETE_CATEGORY );

            WindowHelper.Data.Category.Remove( category );
            CategoryTreeViewInitialize();
            MemoTreeViewInitialize();
        }

        private void OnCategoryCreate() {
            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_CREATE_CATEGORY );

            var newCategory = new UnityEditorMemoCategory( "new Category" );
            WindowHelper.Data.AddCategory( newCategory );
            CategoryTreeViewInitialize();
            categoryTreeView.BeginRename( newCategory );
        }

        private void OnMemoPost( UnityEditorMemoCategory category, UnityEditorMemo memo ) {
            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_POST );

            category.AddMemo( memo );
            category.Initialize();
            memoText = "";
            postMemoTex = 0;
            postMemoUrl = "";
            if( selectLabel == 0 )
                postMemoLabel = 0;
            GUIUtility.keyboardControl = 0;
            MemoTreeViewInitialize();
        }

        private void OnMemoDelete( UnityEditorMemo memo ) {
            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_DELETE_MEMO );

            WindowHelper.GetCategory( selectCategoryId ).Memo.Remove( memo );
            EditorUtility.SetDirty( WindowHelper.Data );
            MemoTreeViewInitialize();
        }

        private void OnMemoContextClicked( UnityEditorMemo memo ) {
            var menu = new GenericMenu();
            menu.AddItem( new GUIContent( !memo.IsEdit ? "Edit" : "Done" ), false, () => {
                UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                memo.IsEdit = !memo.IsEdit;
                memoTreeView.UpdateRowHeight();
            } );

            menu.AddItem( new GUIContent( "Repost" ), false, () => {
                OnMemoDelete( memo );
                memo.Date = DateTime.Now.RenderDate();
                OnMemoPost( WindowHelper.GetCategory( selectCategoryId ), memo );
            } );

            if( !string.IsNullOrEmpty( UnityEditorMemoPrefs.Label1 ) ) {
                menu.AddItem( new GUIContent( "Label/" + UnityEditorMemoPrefs.Label1 ), memo.Label == 0, () => {
                    UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                    memo.Label = 0;
                } );
            }

            if( !string.IsNullOrEmpty( UnityEditorMemoPrefs.Label2 ) ) {
                menu.AddItem( new GUIContent( "Label/" + UnityEditorMemoPrefs.Label2 ), ( int )memo.Label == 1, () => {
                    UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                    memo.Label = ( UnityEditorMemoLabel )1;
                } );
            }

            if( !string.IsNullOrEmpty( UnityEditorMemoPrefs.Label3 ) ) {
                menu.AddItem( new GUIContent( "Label/" + UnityEditorMemoPrefs.Label3 ), ( int )memo.Label == 2, () => {
                    UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                    memo.Label = ( UnityEditorMemoLabel )2;
                } );
            }

            if( !string.IsNullOrEmpty( UnityEditorMemoPrefs.Label4 ) ) {
                menu.AddItem( new GUIContent( "Label/" + UnityEditorMemoPrefs.Label4 ), ( int )memo.Label == 3, () => {
                    UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                    memo.Label = ( UnityEditorMemoLabel )3;
                } );
            }

            if( !string.IsNullOrEmpty( UnityEditorMemoPrefs.Label5 ) ) {
                menu.AddItem( new GUIContent( "Label/" + UnityEditorMemoPrefs.Label5 ), ( int )memo.Label == 5, () => {
                    UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                    memo.Label = ( UnityEditorMemoLabel )5;
                } );
            }

            menu.AddSeparator( "" );

            if( !string.IsNullOrEmpty( memo.URL ) ) {
                menu.AddItem( new GUIContent( "Open URL" ), false, () => {
                    Application.OpenURL( memo.URL );
                } );
                menu.AddSeparator( "" );
            }

            menu.AddItem( new GUIContent( "Delete" ), false, () => {
                OnMemoDelete( memo );
            } );

            menu.ShowAsContext();
        }

        private void OnUnityEditorMemoExport() {
            var exportData = new UnityEditorMemoExport( WindowHelper.Data.Category );
            var json = JsonUtility.ToJson( exportData );
            FileUtility.ExportUnityEditorMemoData( json );
        }

        private void OnUnityEditorMemoImport( bool isOverride ) {
            var text = FileUtility.ImportUnityEditorMemoData();
            var data = JsonUtility.FromJson<UnityEditorMemoExport>( text );
            if( data == null )
                return;
            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_IMPORT_MEMO );
            if( isOverride ) {
                WindowHelper.Data.Category = data.category.ToList();
            } else {
                data.category.ToList().RemoveAll( c => c.Name == "default" );
                WindowHelper.Data.Category.AddRange( data.category.ToList() );
            }
            Initialize();
        }

        #endregion

    }
}