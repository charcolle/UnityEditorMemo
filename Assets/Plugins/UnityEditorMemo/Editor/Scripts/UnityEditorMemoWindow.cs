using UnityEngine;
using UnityEditor;

namespace Charcolle.UnityEditorMemo {
    public class UnityEditorMemoWindow: EditorWindow {
        //======================================================================
        // Varies
        //======================================================================
        private static UnityEditorMemoWindow masterWin;

        private static int SelectMode;
        private static int SelectCategoryId;
        private static int DisplayMemoId; // use for display category which has more than 100 memos

        private static Vector2 MemoScrollView;

        //======================================================================
        // Window Process
        //======================================================================
        [MenuItem( "Window/Unity Editor Memo" )]
        private static void OpenWindow() {
            masterWin = GetWindow<UnityEditorMemoWindow>();
            masterWin.minSize = Helper.WINDOW_SIZE;
            masterWin.titleContent.text = Helper.WINDOW_TITLE;
            Initialize();

            Undo.undoRedoPerformed -= Initialize;
            Undo.undoRedoPerformed += Initialize; //umm...?  :(
        }

        static void Initialize() {
            Helper.Initialize( masterWin.position );
            SelectMode          = ScriptableSingleton<UnityEditorMemoWindowSave>.instance.selectMenu;
            SelectCategoryId    = ScriptableSingleton<UnityEditorMemoWindowSave>.instance.selectCategoryId;
            DisplayMemoId       = ScriptableSingleton<UnityEditorMemoWindowSave>.instance.displayMemoId;
            memoText            = ScriptableSingleton<UnityEditorMemoWindowSave>.instance.postMemoText;
            if ( masterWin != null )
                masterWin.Repaint();
        }

        private void OnDestroy() {
            Undo.undoRedoPerformed -= Initialize;
        }

        void OnGUI() {
            if ( masterWin == null ) OpenWindow();

            EditorGUI.BeginChangeCheck();
            Helper.OnGUIFirst( position.width );
            
            EditorGUILayout.BeginVertical();
            {
                Header();
                if ( SelectMode == 0 ) {
                    UnityEditorMemoSplitterGUI.BeginVerticalSplit( Helper.VerticalState );
                    {
                        DisplayProjectMemos();
                        DisplayPostProcess();
                    }
                    UnityEditorMemoSplitterGUI.EndVerticalSplit();
                } else if ( SelectMode == 1 ) {
                    CategorySetting();
                }
            }
            EditorGUILayout.EndVertical();

            Helper.OnGUIEnd();

            if ( Helper.DisplayedMemo != null && EditorGUI.EndChangeCheck() )
                EditorUtility.SetDirty( Helper.DisplayedMemo );
        }

        //======================================================================
        // OnGUI Contents
        //======================================================================

        #region header
        void Header() {
            //GUILayout.Space( 5 );
            //GUILayout.Label( Helper.TEXT_TITLE, Helper.WINDOW_MAX_SIZE );
            //GUILayout.Space( 5 );
            //GUILayout.Label( Helper.TEXT_DESC );
            //GUILayout.Space( 5 );

            // Display UnityEditorMemo Menu
            if ( Helper.WINDOW_MENU == null ) return;
            EditorGUILayout.BeginHorizontal( Helper.NO_SPACE_BOX_STYLE );
            {
                SelectMode = GUILayout.Toolbar( SelectMode, Helper.WINDOW_MENU, EditorStyles.toolbarButton );
            }
            EditorGUILayout.EndHorizontal();
            ScriptableSingleton<UnityEditorMemoWindowSave>.instance.selectMenu = SelectMode;
        }
        #endregion

        #region draw Memo
        void DisplayProjectMemos() {
            if ( Helper.SaveMemoList == null || Helper.DisplayedMemo == null )
                return;

            EditorGUILayout.BeginVertical( new GUILayoutOption[] { GUILayout.ExpandHeight( true ), GUILayout.ExpandWidth( true ) } );
            {
                // Display Category Menu
                if ( Helper.CategoryNameArray == null || Helper.CategoryNameArray.Length == 0 ) {
                    GUILayout.Space( 10 );
                    EditorGUILayout.HelpBox( Helper.TEXT_CATEGORY_NOTFOUND, MessageType.Warning );
                } else {
                    // Avoid error when user deleted category file in project view...:(
                    if ( SelectCategoryId >= Helper.CategoryNameArray.Length ) {
                        SelectCategoryId = 0;
                        DisplayMemoId = 0;
                        ScriptableSingleton<UnityEditorMemoWindowSave>.instance.displayMemoId = 0;
                        Helper.LoadUnityEditorMemoFromCategory( 0 );
                    }

                    // Show category popup menu
                    EditorGUILayout.BeginHorizontal( Helper.NO_SPACE_BOX_STYLE, GUILayout.ExpandWidth( true ) );
                    {
                        GUILayout.Label( "Category".ToBold() );
                        GUI.backgroundColor = Color.yellow;
                        SelectCategoryId = EditorGUILayout.Popup( SelectCategoryId, Helper.CategoryNameArray, EditorStyles.toolbarPopup );
                        GUI.backgroundColor = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();

                    if ( SelectCategoryId != ScriptableSingleton<UnityEditorMemoWindowSave>.instance.selectCategoryId ) {
                        Helper.LoadUnityEditorMemoFromCategory( SelectCategoryId );
                        DisplayMemoId = 0;
                        ScriptableSingleton<UnityEditorMemoWindowSave>.instance.displayMemoId = 0;
                        Undo.RecordObject( ScriptableSingleton<UnityEditorMemoWindowSave>.instance, Helper.UNDO_CATEGORYCHANGE );
                    }
                    ScriptableSingleton<UnityEditorMemoWindowSave>.instance.selectCategoryId = SelectCategoryId;
                }

                // Display UnityEditorMemo at Category
                var memoList = Helper.DisplayedMemo.UnityMemoList;
                if ( memoList == null || memoList.Count == 0 ) {
                    EditorGUILayout.HelpBox( Helper.TEXT_MEMO_NOTFOUND, MessageType.Warning );
                } else {
                    // Devide Memo When memo num is over 100. ...to avoid get slow:( 
                    if ( Helper.isDevideDisplay ) {
                        DisplayMemoId = GUILayout.Toolbar( DisplayMemoId, Helper.MENU_DISPLAY_MEMO, new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.ExpandWidth( true ) } );
                        ScriptableSingleton<UnityEditorMemoWindowSave>.instance.displayMemoId = DisplayMemoId;
                        GUILayout.Space( 5 );
                    } else {
                        DisplayMemoId = 0;
                        ScriptableSingleton<UnityEditorMemoWindowSave>.instance.displayMemoId = 0;
                    }

                    //GUILayout.Box( "", new GUILayoutOption[] { GUILayout.Height( 2 ), GUILayout.ExpandWidth( true ) } );

                    // Draw memo loop
                    EditorGUILayout.BeginHorizontal();
                    {
                        MemoScrollView = EditorGUILayout.BeginScrollView( MemoScrollView );
                        {
                            drawMemoPreProcess( DisplayMemoId );
                        }
                        EditorGUILayout.EndScrollView();
                        //EditorUtility.SetDirty( UnityEditorMemoWindowHelper.DisplayedMemo );
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// control memo view. 
        /// </summary>
        void drawMemoPreProcess( int displayIdx ) {
            var memoList    = Helper.DisplayedMemo.UnityMemoList;
            var to          = Helper.DisplayMemoTo( displayIdx );
            var from        = Helper.DisplayMemoFrom( displayIdx );

            for ( int i = from; i >= to; i-- )
                drawMemoContent( memoList[i], i + 1 );
        }

        /// <summary>
        /// drawing a memo
        /// </summary>
        void drawMemoContent( UnityMemoClass memo, int memoIdx ) {
            EditorGUILayout.BeginVertical( Helper.GUISKIN_BOX_STYLE );
            {
                // Display Memo Date
                GUI.backgroundColor = Helper.PostMemoTypeColor( ( int )memo.Type );
                var preState = memo.isFold;
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label( memoIdx.ToString(), Helper.GUISKIN_BOX_STYLE, GUILayout.Height( 20 ) );
                    GUILayout.Label( memo.Date.ToBold(), Helper.GUISKIN_BOX_STYLE, new GUILayoutOption[] { GUILayout.ExpandWidth( true ), GUILayout.Height( 20 ) } );
                    GUI.backgroundColor = Color.white;
                    memo.isFold = GUILayout.Toggle( memo.isFold, "●", "button", new GUILayoutOption[] { GUILayout.Width( 20 ), GUILayout.Height( 20 ) } );
                    if ( memo.isFold != preState )
                        GUIUtility.keyboardControl = 0;
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;

                GUILayout.Space( 1 );

                // Display Memo and Emotion
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Space( 5 );
                    if ( memo.Tex != 0 && Helper.POSTMEMO_TEX != null ) {
                        EditorGUILayout.BeginVertical( GUILayout.Width( 32 ) );
                        GUILayout.Box( Helper.POSTMEMO_TEX[( int )memo.Tex], GUIStyle.none, new GUILayoutOption[] { GUILayout.Width( 32 ), GUILayout.Height( 32 ) } );
                        EditorGUILayout.EndVertical();
                    } else {
                        GUILayout.Space( 34 );
                    }

                    // Display Or Edit Memo
                    if ( memo.isFold ) {
                        Undo.IncrementCurrentGroup();
                        Undo.RecordObject( Helper.DisplayedMemo, Helper.UNDO_EDITPOST );
                        memo.Memo = EditorGUILayout.TextArea( memo.Memo, Helper.TEXTAREA_WORDWRAP_STYLE );
                    } else {
                        GUILayout.Label( memo.Memo, Helper.GUISKIN_BOX_STYLE );
                    }

                }
                EditorGUILayout.EndHorizontal();

                GUILayout.Space( 5 );

                // Display Memo Edit Buttons
                //memo.isFold = EditorGUILayout.Foldout( memo.isFold, "" );
                if ( memo.isFold ) {
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.FlexibleSpace();
                        memo.Tex    = ( UnityMemoTexture )GUILayout.Toolbar( ( int )memo.Tex, Helper.POSTMEMO_TEX, new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 100 ) } );
                        memo.Type   = ( UnityMemoType )EditorGUILayout.Popup( ( int )memo.Type, Helper.POSTMEMO_TYPE, GUILayout.Width( 100 ) );

                        //memo.isEditable = GUILayout.Toggle( memo.isEditable, Helper.EDIT_TEX, "button", new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 30 ) } );
                        GUILayout.Space( 5 );
                        GUI.color = Color.red;
                        if ( GUILayout.Button( "×", new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 30 ) } ) ) {
                            Undo.RecordObject( Helper.DisplayedMemo, Helper.UNDO_DELETEPOST );
                            Helper.RemovePost( memo );
                        }
                        GUI.color = Color.white;
                        GUILayout.Space( 5 );
                    }
                    EditorGUILayout.EndHorizontal();
                    GUILayout.Space( 5 );
                }

            }
            EditorGUILayout.EndVertical();
        }
        #endregion

        #region post Memo
        static string   memoText = "";
        static int      postMemoType = 0;
        static int      postMemoTex = 0;

        /// <summary>
        /// display posting area
        /// </summary>
        void DisplayPostProcess() {
            if ( Helper.SaveMemoList == null || Helper.DisplayedMemo == null )
                return;

            EditorGUILayout.BeginVertical( new GUILayoutOption[] { GUILayout.ExpandHeight( true ), GUILayout.ExpandWidth( true ) } );
            {
                GUILayout.Box( "", new GUILayoutOption[] { GUILayout.Height( 2 ), GUILayout.ExpandWidth( true ) } );
                GUILayout.Space( 5 );
                GUILayout.Label( (Helper.TEXT_CREATEMEMO_TITLE + Helper.DisplayedMemo.CategoryName ).ToMiddleBold() );
                EditorGUILayout.BeginVertical();
                {
                    // Display Date
                    var memoDate = Helper.MemoDate;
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Label( memoDate, GUI.skin.box, new GUILayoutOption[] { GUILayout.Width( 150 ), GUILayout.Height( 25 ) } );
                        postMemoType = EditorGUILayout.Popup( postMemoType, Helper.POSTMEMO_TYPE, GUILayout.Width( 100 ) );
                        ScriptableSingleton<UnityEditorMemoWindowSave>.instance.postMemoType = postMemoType;
                    }
                    EditorGUILayout.EndHorizontal();

                    GUILayout.Space( 5 );

                    // Display Draft
                    Undo.IncrementCurrentGroup();
                    Undo.RecordObject( ScriptableSingleton<UnityEditorMemoWindowSave>.instance, Helper.UNDO_DRAFT );
                    memoText = EditorGUILayout.TextArea( memoText, Helper.TEXTAREA_WORDWRAP_STYLE, new GUILayoutOption[] { GUILayout.MaxHeight( 300 ) } );
                    ScriptableSingleton<UnityEditorMemoWindowSave>.instance.postMemoText = memoText;
                    EditorGUILayout.BeginHorizontal();
                    {
                        postMemoTex = GUILayout.Toolbar( postMemoTex, Helper.POSTMEMO_TEX, new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 150 ) } );
                        ScriptableSingleton<UnityEditorMemoWindowSave>.instance.postMemoTex = postMemoTex;
                        GUILayout.FlexibleSpace();
                        
                        // Display Post Button
                        GUI.backgroundColor = Color.cyan;
                        if ( GUILayout.Button( "Post", new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 100 ) } ) ) {
                            Undo.RecordObject( Helper.DisplayedMemo, Helper.UNDO_POST );
                            if ( !string.IsNullOrEmpty( memoText ) ) {
                                var memoClass = new UnityMemoClass( memoDate, memoText, postMemoType, postMemoTex );
                                Helper.PostMemo( memoClass );
                                memoText = ""; ScriptableSingleton<UnityEditorMemoWindowSave>.instance.postMemoText = "";
                                postMemoType = 0; ScriptableSingleton<UnityEditorMemoWindowSave>.instance.postMemoType = 0;
                                postMemoTex = 0; ScriptableSingleton<UnityEditorMemoWindowSave>.instance.postMemoTex = 0;

                                //Scroll Up
                                MemoScrollView = Vector2.zero;
                                GUIUtility.keyboardControl = 0;
                            } else {
                                Debug.LogWarning( Helper.WARNING_MEMO_EMPTY );
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

        #region category Setting
        static string tmpCategoryName = "";
        void CategorySetting() {
            // Draw Name Input Field
            GUILayout.Label( Helper.TEXT_CATEGORY_DESC );
            EditorGUILayout.BeginHorizontal();
            {
                GUI.skin.textField.alignment = TextAnchor.MiddleLeft;
                tmpCategoryName = GUILayout.TextField( tmpCategoryName, new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.MaxWidth( position.width - 130 ) } );
                GUI.skin.textField.alignment = TextAnchor.UpperLeft;

                GUI.backgroundColor = Color.green;
                if ( GUILayout.Button( "Register", new GUILayoutOption[] { GUILayout.Height( 30 ), GUILayout.Width( 100 ) } ) ) {
                    Helper.CreateNewCategory( tmpCategoryName );
                    tmpCategoryName = "";
                    SelectCategoryId = 0;
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();

            GUILayout.Space( 5 );

            if ( Helper.SaveMemoList == null || Helper.SaveMemoList.Count == 0 ) return;

            GUILayout.Box( "", new GUILayoutOption[] { GUILayout.Height( 2 ), GUILayout.ExpandWidth( true ) } );

            GUILayout.Space( 5 );

            EditorGUILayout.BeginVertical();
            {
                GUILayout.Label( Helper.TEXT_CATEGORY_LIST );
                GUILayout.Space( 5 );

                GUI.backgroundColor = Color.grey;
                EditorGUILayout.BeginHorizontal( GUI.skin.box, new GUILayoutOption[] { GUILayout.Height( 20 ), GUILayout.ExpandWidth( true ) } );
                {
                    GUILayout.Label( "CategoryName", GUILayout.Width( 100 ) );
                    GUILayout.Label( "Memo Num", GUILayout.Width( 70 ) );
                    GUILayout.Label( "Last Posted", GUILayout.ExpandWidth( true ) );
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;

                for ( int i = 0; i < Helper.SaveMemoList.Count; i++ ) {
                    var unityMemo = Helper.SaveMemoList[i];
                    displayCategories( unityMemo );
                }
            }
            EditorGUILayout.EndVertical();
        }

        void displayCategories( UnityMemoSaveClass unityMemo ) {
            EditorGUILayout.BeginHorizontal( GUI.skin.box, new GUILayoutOption[] { GUILayout.Height( 20 ), GUILayout.ExpandWidth( true ) } );
            {
                var memolist = unityMemo.UnityMemoList;
                GUILayout.Label( unityMemo.CategoryName, Helper.LABEL_WORDWRAP_STYLE, GUILayout.Width( 100 ) );
                GUILayout.Label( memolist.Count.ToString(), GUILayout.Width( 70 ) );
                GUILayout.Label( memolist.Count == 0 ? "none" : memolist[memolist.Count - 1].Date, GUILayout.ExpandWidth( true ) );
                GUI.backgroundColor = Color.red;
                if ( !unityMemo.CategoryName.Equals( "default" ) ) {
                    if ( GUILayout.Button( "×", GUILayout.Width( 40 ) ) )
                        Helper.DeleteUnityMemo( unityMemo );
                }
                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
    }
}