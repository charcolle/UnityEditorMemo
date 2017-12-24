using System.IO;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

namespace charcolle.UnityEditorMemo {

    internal static class FileUtility {

        private const string SEARCH_KEYWORD         = "UnityEditorMemoFileUtility";
        private const string SEARCH_UNITYEDITORMEMO = "UnityEditorMemoSaveData";
        private const string SEARCH_UNITYSCENEMEMO  = "UnitySceneMemoSaveData";

        private const string RELATIVEPATH_GUI       = "Editor/GUI/";
        private const string RELATIVEPATH_SAVEDATA  = "Editor/SaveData/";
        private const string RELATIVEPATH_DEFAULT   = "Assets/Plugins/UnityEditorMemo/";

        private const string NAME_UNITYEDITORMEMO   = "UnityEditorMemoData.asset";
        private const string NAME_UNITYSCENEMEMO    = "UnitySceneMemoData.asset";

        //=======================================================
        // path
        //=======================================================

        public static string GUIDirectoryPath {
            get {
                return pathSlashFix( Path.Combine( UnityEditorMemoRootPath, RELATIVEPATH_GUI ) );
            }
        }

        public static string SaveDataDirectoryPath {
            get {
                return pathSlashFix( Path.Combine( UnityEditorMemoRootPath, RELATIVEPATH_SAVEDATA ) );
            }
        }

        public static string UnityEditorMemoRootPath {
            get {
                var guid = getAssetGUID( SEARCH_KEYWORD );

                if ( string.IsNullOrEmpty( guid ) ) {
                    Debug.LogError( "fatal error." );
                    return RELATIVEPATH_DEFAULT;
                }
                var filePath   = Path.GetDirectoryName( AssetDatabase.GUIDToAssetPath( guid ) );
                var scriptPath = Path.GetDirectoryName( filePath );
                var editorPath = Path.GetDirectoryName( scriptPath );
                var rootPath   = Path.GetDirectoryName( editorPath );

                return pathSlashFix( rootPath );
            }
        }

        //=======================================================
        // file
        //=======================================================

        public static UnityEditorMemoSaveData LoadUnityEditorMemoData() {
            var saveData = FindAssetByType<UnityEditorMemoSaveData>( SEARCH_UNITYEDITORMEMO );
            if( saveData == null ) {
                if ( !Directory.Exists( SaveDataDirectoryPath ) )
                    Directory.CreateDirectory( SaveDataDirectoryPath );
                var savePath = Path.Combine( SaveDataDirectoryPath, NAME_UNITYEDITORMEMO );
                saveData = ScriptableObject.CreateInstance<UnityEditorMemoSaveData>();
                saveData.AddCategory( new UnityEditorMemoCategory( "default" ) );
                AssetDatabase.CreateAsset( saveData, savePath );
                AssetDatabase.Refresh();
                Debug.Log( "UnityEditorMemo: EditorMemo-SaveData is created successfully" );
            }
            return saveData;
        }

        public static UnitySceneMemoSaveData LoadUnitySceneMemoData() {
            var saveData = FindAssetByType<UnitySceneMemoSaveData>( SEARCH_UNITYSCENEMEMO );
            if ( saveData == null ) {
                if ( !Directory.Exists( SaveDataDirectoryPath ) )
                    Directory.CreateDirectory( SaveDataDirectoryPath );
                var savePath = Path.Combine( SaveDataDirectoryPath, NAME_UNITYSCENEMEMO );
                saveData = ScriptableObject.CreateInstance<UnitySceneMemoSaveData>();
                AssetDatabase.CreateAsset( saveData, savePath );
                AssetDatabase.Refresh();
                Debug.Log( "UnityEditorMemo: SceneMemo-SaveData is created successfully" );
            }
            return saveData;
        }

        //=======================================================
        // utility
        //=======================================================

        /// <summary>
        /// get path of dropped file
        /// </summary>
        public static Object GetDraggedObject( Event curEvent, Rect dropArea ) {
            int ctrlID = GUIUtility.GetControlID( FocusType.Passive );
            switch ( curEvent.type ) {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                    if ( !dropArea.Contains( curEvent.mousePosition ) )
                        break;

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                    DragAndDrop.activeControlID = ctrlID;

                    if ( curEvent.type == EventType.DragPerform ) {
                        DragAndDrop.AcceptDrag();
                        foreach ( var draggedObj in DragAndDrop.objectReferences ) {
                            return draggedObj;
                        }
                    }
                    curEvent.Use();
                    break;
            }
            return null;
        }

        //=======================================================
        // process
        //=======================================================

        private static T FindAssetByType<T>( string type ) where T : Object {
            var searchFilter = "t:" + type;
            var guid = getAssetGUID( searchFilter );
            if ( string.IsNullOrEmpty( guid ) )
                return null;
            var assetPath = AssetDatabase.GUIDToAssetPath( guid );
            return AssetDatabase.LoadAssetAtPath<T>( assetPath );
        }

        private static string getAssetGUID( string searchFilter ) {
            var guids = AssetDatabase.FindAssets( searchFilter );
            if ( guids == null || guids.Length == 0 ) {
                return null;
            }

            if ( guids.Length > 1 ) {
                Debug.LogWarning( "more than one file was found." );
            }
            return guids[0];
        }

        private const string forwardSlash   = "/";
        private const string backSlash      = "\\";
        private static string pathSlashFix( string path ) {
            return path.Replace( backSlash, forwardSlash );
        }

    }
}
