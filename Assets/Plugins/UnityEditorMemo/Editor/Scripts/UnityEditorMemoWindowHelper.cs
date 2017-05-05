using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace Charcolle.UnityEditorMemo {
    public static class Helper {

        //======================================================================
        // Window Varies
        //======================================================================
        public static Vector2  WINDOW_SIZE  = new Vector2( 350f, 400f );
        public const string WINDOW_TITLE    = "Unity Memo";

        public static GUILayoutOption WINDOW_MAX_SIZE;
        public static UnityEditorMemoSplitterState VerticalState;
        public static GUIStyle LABEL_WORDWRAP_STYLE;
        public static GUIStyle TEXTAREA_WORDWRAP_STYLE;
        public static GUIStyle NO_SPACE_BOX_STYLE;
        public static GUIStyle GUISKIN_BOX_STYLE;
        
        public static Texture2D[] WINDOW_MENU;
        public static string[] POSTMEMO_TYPE    = { "Normal", "Important", "Question" };
        public static GUILayoutOption[] OPTION_WINDOWMENU = new GUILayoutOption[] { GUILayout.Height( 60 ), GUILayout.ExpandWidth( true ) };
        
        public const string TEXT_TITLE              = "<b><size=30>Unity Editor Memo</size></b>";
        public const string TEXT_CREATEMEMO_TITLE   = "Post Memo To ";
        public const string TEXT_DESC               = "Put your memo on Unity Editor";
        public const string TEXT_CATEGORY_NOTFOUND  = "Category not found.";
        public const string TEXT_MEMO_NOTFOUND      = "Unity Editor Memo is not found.";
        public const string TEXT_CATEGORY_DESC      = "Create New Category" ;
        public const string TEXT_CATEGORY_LIST      = "UnityEditorMemo Category List";
        public const string WARNING_MEMO_EMPTY      = "UnityEditorMemoWindow: Memo cannot be empty. Write Something.";

        public const string UNDO_POST               = "UnityEditorMemo Post";
        public const string UNDO_DELETEPOST         = "UnityEditorMemo Delete";
        public const string UNDO_CATEGORYCHANGE     = "UnityEditorMemo Category Change";
        public const string UNDO_EDITPOST           = "UnityEditorMemo Edit Post";
        public const string UNDO_DRAFT              = "UnityEditorMemo Edit Draft";

        public static string[] MENU_DISPLAY_MEMO    = { "lastest 100", "older" };

        public static bool isDebug = true;

        //======================================================================
        // Process Varies
        //======================================================================
        public static List<UnityMemoSaveClass>  SaveMemoList;
        public static UnityMemoSaveClass        DisplayedMemo;
        public static string[]                  CategoryNameArray;
        public static Texture2D[] POSTMEMO_TEX;
        public static Texture2D EDIT_TEX;
        public static GUISkin UnityEditorMemoGUISkin;

        private static Texture2D HAPPY_TEX;
        private static Texture2D ANGRY_TEX;
        private static Texture2D SAD_TEX;
        private static Texture2D HOME_TEX;
        private static Texture2D ADD_TEX;

        private const string SEARCH_MEMO_KEYWORD        = "t:UnityMemoSaveClass";
        private const string SEARCH_DIR_KEYWORD         = "UnityEditorMemoWindow";


        private const string ERROR_SAVEDATA_INVALID     = "UnityEditorMemoWindow: SaveDatas is broken.";
        private const string ERROR_SEARCH_NOTFOUND      = "UnityEditorMemoWindow: Memos are not found. ";
        private const string ERROR_SEARCH_ROOTNOTFOUND  = "UnityEditorMemoWindow: Root Directory not Found.";
        private const string ERROR_CREATE_MEMOSAVE      = "UnityEditorMemoWindow: This category already exist. :";
        private const string ERROR_CATEGORY_EMPTY       = "UnityEditorMemoWindow: Category cannot be empty.";
        private const string ERROR_SERIOUS              = "UnityEditorMemoWindow: Serious error occured. ";
        private const string INFO_CREATE_DIRECTORY      = "UnityEditorMemoWindow: Save directory is created.  ";

        private const string DIR_SAVE_NAME              = "/SaveData/";
        private const string DIR_GUI_NAME               = "/GUI/";
        private const string NAME_DEFAULT_MEMO          = "default";

        //======================================================================
        // Public Helper Method ・ Initialize
        //======================================================================
        public static void Initialize( Rect window ) {
            VerticalState = new UnityEditorMemoSplitterState( new float[] { window.height * 0.9f, window.height * 0.1f },
                                                              new int[] { 200, 200 }, new int[] { 1500, 300 } );

            LoadGUIData();
            LoadUnityEditorMemoSaveDatas();
            var selectCategoryId = ScriptableSingleton<UnityEditorMemoWindowSave>.instance.selectCategoryId;
            LoadUnityEditorMemoFromCategory( selectCategoryId );
        }

        public static void OnGUIFirst( float windowWidth ) {
            WINDOW_MAX_SIZE = GUILayout.MaxWidth( windowWidth );
            GUI.skin.label.richText = true;
            GUI.skin.box.richText = true;
            //GUI.skin.box.normal.textColor = ( EditorGUIUtility.isProSkin ? Color.white : Color.black );

            LABEL_WORDWRAP_STYLE = new GUIStyle( GUI.skin.label );
            LABEL_WORDWRAP_STYLE.wordWrap = true;
            LABEL_WORDWRAP_STYLE.richText = true;

            TEXTAREA_WORDWRAP_STYLE = new GUIStyle( GUI.skin.textArea );
            TEXTAREA_WORDWRAP_STYLE.wordWrap = true;

            NO_SPACE_BOX_STYLE = new GUIStyle( EditorStyles.toolbar );
            NO_SPACE_BOX_STYLE.margin = new RectOffset( 0, 0, 0, 0 );
            NO_SPACE_BOX_STYLE.padding = new RectOffset( 0, 0, 0, 0 );

            if ( UnityEditorMemoGUISkin != null ) {
                GUISKIN_BOX_STYLE = UnityEditorMemoGUISkin.box;
                GUISKIN_BOX_STYLE.normal.textColor = Color.black;
            } else {
                GUISKIN_BOX_STYLE = new GUIStyle( GUI.skin.box );
            }
            //GUI.skin = UnityEditorMemoGUISkin;
        }

        public static void OnGUIEnd() {
            GUI.skin.label.richText = false;
            GUI.skin.box.richText = false;
        }
        //======================================================================
        // Public Helper Method ・ Process
        //======================================================================
        public static void LoadUnityEditorMemoFromCategory( int selectId ) {
            LoadUnityEditorMemoSaveDatas();

            if ( selectId >= CategoryNameArray.Length ) {
                if( isDebug ) Debug.LogError( ERROR_SERIOUS );
                ScriptableSingleton<UnityEditorMemoWindowSave>.instance.selectCategoryId = 0;
                LoadUnityEditorMemoCategory( CategoryNameArray[0] );
                return;
            }

            LoadUnityEditorMemoCategory( CategoryNameArray[selectId] );
        }
       
        public static void DeleteUnityMemo( UnityMemoSaveClass memo ) {
            SaveMemoList.Remove( memo );

            var memoGuids = AssetDatabase.FindAssets( SEARCH_MEMO_KEYWORD );
            if ( memoGuids.Length != 0 ) {
                for ( int i = 0; i < memoGuids.Length; i++ ) {
                    var path = AssetDatabase.GUIDToAssetPath( memoGuids[i] );
                    var asset = AssetDatabase.LoadAssetAtPath<UnityMemoSaveClass>( path );

                    if ( asset.CategoryName.Equals( memo.CategoryName ) ) {
                        AssetDatabase.DeleteAsset( path );
                        break;
                    }
                }
            }
            AssetDatabase.Refresh();

            LoadUnityEditorMemoSaveDatas();
            if ( CategoryNameArray != null || CategoryNameArray.Length != 0 )
                LoadUnityEditorMemoCategory( CategoryNameArray[0] );
            else
                Debug.LogError( ERROR_SERIOUS );
        }

        public static void PostMemo( UnityMemoClass postMemo ) {
            if ( postMemo == null ) return;
            if ( DisplayedMemo == null ) {
                Debug.LogError( ERROR_SERIOUS );
                return;
            }

            // save memo at category
            AssetDatabase.StartAssetEditing();
            DisplayedMemo.UnityMemoList.Add( postMemo );
            AssetDatabase.StopAssetEditing();
            EditorUtility.SetDirty( DisplayedMemo );
            AssetDatabase.Refresh();
        }

        public static void RemovePost( UnityMemoClass memo ) {
            if ( memo == null ) return;
            if ( DisplayedMemo == null ) {
                Debug.LogError( ERROR_SERIOUS );
                return;
            }

            DisplayedMemo.UnityMemoList.Remove( memo );
        }

        public static void CreateNewCategory( string categoryName ) {
            if ( string.IsNullOrEmpty( categoryName ) ) {
                Debug.LogError( ERROR_CATEGORY_EMPTY );
                return;
            }
            CreateUnityEditorMemoCategory( categoryName );
        }

        public static string MemoDate {
            get {
                return System.DateTime.Now.Year + " " + System.DateTime.Now.Month + "/" + System.DateTime.Now.Day + " " + System.DateTime.Now.Hour + ":" + System.DateTime.Now.Minute;
            }
        }

        public static Color PostMemoTypeColor( int type ) {
            switch ( type ) {
                case 0:
                    return new Color( 0.7f, 0.7f, 0.7f );
                case 1:
                    return new Color( 1f, 0.35f, 0.35f );
                case 2:
                    return new Color( 0.4f, 1f, 0.4f );
                default:
                    return Color.gray;
            }
        }
        //======================================================================
        // Public Helper Method ・ PostDisplay
        //======================================================================
        /// <summary>
        /// Devide Posts if memos are over 100
        /// </summary>
        public static bool isDevideDisplay {
            get {
                if ( DisplayedMemo == null ) {
                    Debug.LogError( ERROR_SERIOUS );
                    return false;
                }

                if ( DisplayedMemo.UnityMemoList.Count > 100 )
                    return true;
                else
                    return false;
            }
        }

        public static int DisplayMemoFrom( int displayIdx ) {
            if ( isDevideDisplay ) {
                if ( displayIdx == 0 ) //1-100
                    return DisplayedMemo.UnityMemoList.Count - 1;
                else
                    return DisplayedMemo.UnityMemoList.Count - 101; //101-
            } else {
                return DisplayedMemo.UnityMemoList.Count - 1;
            }
        }

        public static int DisplayMemoTo( int displayIdx ) {
            if ( isDevideDisplay ) {
                if ( displayIdx == 0 ) //1-100
                    return DisplayedMemo.UnityMemoList.Count - 100;
                else
                    return 0; //101-
            } else {
                return 0;
            }
        }
        //======================================================================
        // private Method
        //======================================================================

        /// <summary>
        ///  Load all UnityEditorMemo Data and Cache category name.
        /// </summary>
        static void LoadUnityEditorMemoSaveDatas() {
            SaveMemoList = new List<UnityMemoSaveClass>();

            //load unityeditormemosaveclass
            var memoGuids = AssetDatabase.FindAssets( SEARCH_MEMO_KEYWORD );
            if ( memoGuids.Length != 0 ) {
                var fullPathList = new List<string>();
                for ( int i = 0; i < memoGuids.Length; i++ ) {
                    var path = AssetDatabase.GUIDToAssetPath( memoGuids[i] );
                    var asset = AssetDatabase.LoadAssetAtPath<UnityMemoSaveClass>( path );
                    if ( asset == null )
                        return;

                    SaveMemoList.Add( asset );
                    var fullpath =  Application.dataPath + "/" + AssetDatabase.GetAssetPath( SaveMemoList[i] ).Replace( "Assets/", "" );
                    fullPathList.Add( fullpath );
                }
                
                if ( fullPathList != null && fullPathList.Count > 0 ) {
                    var categoryNameList = new List<string>();

                    //sort by created time
                    var sortedCategory = fullPathList.OrderBy( path => File.GetCreationTime( path ) )
                        .Select( path => AssetDatabase.LoadAssetAtPath<UnityMemoSaveClass>( path.Replace( Application.dataPath + "/", "Assets/" ) ) );

                    //construct category name list
                    foreach ( var asset in sortedCategory )
                        categoryNameList.Add( asset.CategoryName );

                    CategoryNameArray = categoryNameList.ToArray();
                } else {
                    Debug.LogError( ERROR_SAVEDATA_INVALID );
                    CategoryNameArray = new string[] { "none" };
                }

            } else {
                Debug.LogWarning( ERROR_SEARCH_NOTFOUND );
                CreateUnityEditorMemoCategory( NAME_DEFAULT_MEMO );
            }
        }
        /// <summary>
        /// Load the category user want to display.
        /// </summary>
        static void LoadUnityEditorMemoCategory( string categoryName ) {
            if ( string.IsNullOrEmpty( categoryName ) )
                return;
            if ( SaveMemoList == null || SaveMemoList.Count == 0 )
                return;

            for ( int i = 0; i < SaveMemoList.Count; i++ ) {
                var memo = SaveMemoList[i];
                if ( memo.CategoryName.Equals( categoryName ) ) {
                    DisplayedMemo = memo;
                    return;
                }
            }
            Debug.LogWarning( ERROR_SEARCH_NOTFOUND + categoryName );
        }
        /// <summary>
        /// Create new category.
        /// </summary>
        static void CreateUnityEditorMemoCategory( string categoryName ) {
            var saveDir = SaveMemoDirPath;
            var savePath = saveDir + MemoFileName( categoryName );

            if ( File.Exists( savePath ) ) {
                Debug.LogError( ERROR_CREATE_MEMOSAVE + categoryName );
                return;
            }

            // create scriptableObject
            var memo = ScriptableObject.CreateInstance<UnityMemoSaveClass>();
            AssetDatabase.CreateAsset( memo, savePath );

            AssetDatabase.StartAssetEditing();
            memo.Initialize( categoryName );
            AssetDatabase.StopAssetEditing();
            EditorUtility.SetDirty( memo );

            LoadUnityEditorMemoSaveDatas();
        }

        /// <summary>
        /// Load texture, GuiSkin and Cache them.
        /// </summary>
        static void LoadGUIData() {
            //Load GUISKin
            string guiSkinFileName = EditorGUIUtility.isProSkin ? "UnityEditorMemoSkin_Pro.guiskin" : "UnityEditorMemoSkin.guiskin";
            UnityEditorMemoGUISkin = AssetDatabase.LoadAssetAtPath<GUISkin>( GUIDirPath + guiSkinFileName );

            //Load Texture
            HAPPY_TEX = AssetDatabase.LoadAssetAtPath<Texture2D>( GUIDirPath + "Texture/UnityEditorMemo_Happy.png" );
            ANGRY_TEX = AssetDatabase.LoadAssetAtPath<Texture2D>( GUIDirPath + "Texture/UnityEditorMemo_Angry.png" );
            SAD_TEX = AssetDatabase.LoadAssetAtPath<Texture2D>( GUIDirPath + "Texture/UnityEditorMemo_Sad.png" );
            HOME_TEX = AssetDatabase.LoadAssetAtPath<Texture2D>( GUIDirPath + "Texture/UnityEditorMemo_Home.png" );
            ADD_TEX = AssetDatabase.LoadAssetAtPath<Texture2D>( GUIDirPath + "Texture/UnityEditorMemo_Add.png" );

            POSTMEMO_TEX = new Texture2D[] { null, HAPPY_TEX, ANGRY_TEX, SAD_TEX };
            WINDOW_MENU = new Texture2D[] { HOME_TEX, ADD_TEX };
        }

        //======================================================================
        // Utility
        //======================================================================

        static string MemoFileName( string category ) {
            return category + "_memo.asset";
        }

        static string SaveMemoDirPath {
            get {
                var rootDir = EditorDirPath;
                var path = rootDir + DIR_SAVE_NAME;
                if ( !string.IsNullOrEmpty( rootDir ) ) {
                    if ( !Directory.Exists( path ) ) {
                        Directory.CreateDirectory( path );
                        Debug.Log( INFO_CREATE_DIRECTORY );
                    }
                    return path;
                } else {
                    return null;
                }
            }
        }

        static string GUIDirPath {
            get {
                var rootDir = EditorDirPath;
                if ( !string.IsNullOrEmpty( rootDir ) )
                    return rootDir + DIR_GUI_NAME;
                else
                    return null;
            }
        }

        static string EditorDirPath {
            get {
                var guids = AssetDatabase.FindAssets( SEARCH_DIR_KEYWORD );
                if ( guids.Length == 0 ) {
                    Debug.LogError( ERROR_SEARCH_ROOTNOTFOUND );
                    return null;
                }
                var scriptDirPath   = Path.GetDirectoryName( AssetDatabase.GUIDToAssetPath( guids[0] ) );
                var editorDirPath   = Path.GetDirectoryName( scriptDirPath );
       
                return editorDirPath;
            }
        }

        public static string ToSuccess( this string str ) {
            return string.Format( "<color=green>{0}</color>", str );
        }

        public static string ToBold( this string str ) {
            return string.Format( "<b>{0}</b>", str );
        }

        public static string ToMiddleBold( this string str ) {
            return string.Format( "<size=15><b>{0}</b></size>", str );
        }

    }

    //======================================================================
    // Editor Save Class
    //======================================================================
    public class UnityEditorMemoWindowSave: ScriptableSingleton<UnityEditorMemoWindowSave> {
        public int selectMenu           = 0;
        public int selectCategoryId     = 0;
        public int displayMemoId        = 0;
        public int filteredType         = -1;

        public string postMemoText      = "";
        public int postMemoType         = 0;
        public int postMemoTex          = 0;
    }
}