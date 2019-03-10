using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

using FileUtility = charcolle.UnityEditorMemo.FileUtility;

namespace charcolle.UnityEditorMemo {

    internal static class UnitySceneMemoHelper {

        public static UnitySceneMemoSaveData Data;
        public static UnitySceneMemoHierarchyWindow PopupWindowContent;

        public static void Initialize() {
            Data = FileUtility.LoadUnitySceneMemoData();

            if( Data != null ) {
                PopupWindowContent = new UnitySceneMemoHierarchyWindow();
                EditorSceneManager.sceneSaved += ( scene ) => {
                    InitializeSceneMemo( scene );
                    RefreshSceneMemo( scene );
                };
                EditorSceneManager.sceneOpened += ( scene, mode ) => {
                    InitializeSceneMemo( scene );
                };
            }
        }

        //======================================================================
        // public
        //======================================================================

        public static void InitializeSceneMemo( Scene scene ) {
            if ( EditorApplication.isPlaying )
                return;

            var sceneMemo = GetSceneMemo( scene.path );
            if ( sceneMemo == null )
                return;
            sceneMemo.Initialize();
        }

        public static UnitySceneMemo GetMemo( GameObject obj, int localIdentifier = 0 ) {
            if ( Data == null )
                return null;

            if( localIdentifier == 0 )
                localIdentifier = GetLocalIdentifierInFile( obj );

            return Data.GetSceneMemo( obj, localIdentifier );
        }

        public static UnitySceneMemo GetMemo( string scenePath, int localIdentifier ) {
            if ( Data == null )
                return null;

            return Data.GetSceneMemo( scenePath, localIdentifier );
        }

        public static void AddMemo( GameObject obj, int localIdentifierInFile ) {
            if ( Data == null )
                return;
            if ( localIdentifierInFile == 0 )
                return;

            Data.AddSceneMemo( obj, localIdentifierInFile );
        }

        public static void RemoveMemo( UnitySceneMemo memo ) {
            if ( Data == null )
                return;

            var scene = Data.GetScene( memo.SceneGuid );
            scene.DeleteMemo( memo );
        }

        public static void SetDirty() {
            if ( Data == null )
                return;

            EditorUtility.SetDirty( Data );
        }

        //======================================================================
        // private
        //======================================================================

        private static void RefreshSceneMemo( Scene scene ) {
            if ( EditorApplication.isPlaying )
                return;

            var sceneMemo = GetSceneMemo( scene.path );
            if ( sceneMemo == null )
                return;

            var objs = new List<Transform>();
            foreach ( GameObject obj in scene.GetRootGameObjects() )
                objs.AddRange( getObjectRecursive( obj ) );

            var existingMemos = objs.Select( obj => GetMemo( obj.gameObject ) ).Where( memo => memo != null );
            sceneMemo.Memo = existingMemos.ToList();

            if ( sceneMemo.Memo.Count == 0 )
                Data.Scene.Remove( sceneMemo );
        }

        private static Transform[] getObjectRecursive( GameObject obj ) {
            return obj.GetComponentsInChildren<Transform>(true);
        }

        private static bool isMemoObjectExist( GameObject obj ) {
            var memo = GetMemo( obj.gameObject );
            return memo != null;
        }

        //======================================================================
        //  Utility
        //======================================================================

        private static PropertyInfo inspectorModeInfo;
        /// <summary>
        /// get LocalIdentifierInFile
        /// https://forum.unity.com/threads/how-to-get-the-local-identifier-in-file-for-scene-objects.265686/
        /// </summary>
        public static int GetLocalIdentifierInFile( Object obj ) {
            int localId, localSubId = 0;
            if ( inspectorModeInfo == null )
                inspectorModeInfo = typeof( SerializedObject ).GetProperty( "inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance );
            SerializedObject serializedObject = new SerializedObject( obj );
            inspectorModeInfo.SetValue( serializedObject, InspectorMode.Debug, null );
            SerializedProperty localIdProp = serializedObject.FindProperty( "m_LocalIdentfierInFile" );
            localId = localIdProp.intValue;
            //if( localId != 0 )
            //    return localId;
#if UNITY_2018_3_OR_NEWER
            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType( obj );
            if( prefabType != PrefabAssetType.MissingAsset ) {
                var o = PrefabUtility.GetPrefabInstanceHandle( obj );
                if( o == null )
                    o = obj;
                serializedObject = new SerializedObject( o );
                inspectorModeInfo.SetValue( serializedObject, InspectorMode.Debug, null );
                localIdProp = serializedObject.FindProperty( "m_LocalIdentfierInFile" );
                localId = localIdProp.intValue;

                if( obj != PrefabUtility.GetOutermostPrefabInstanceRoot( obj as GameObject ) ) {
                    o = PrefabUtility.GetCorrespondingObjectFromSource( obj );
                    if( o != null ) {
                        serializedObject = new SerializedObject( o );
                        inspectorModeInfo.SetValue( serializedObject, InspectorMode.Debug, null );
                        localIdProp = serializedObject.FindProperty( "m_LocalIdentfierInFile" );
                        localSubId = localIdProp.intValue;
                    }
                }
            }
#else
            PrefabType prefabType = PrefabUtility.GetPrefabType( obj );
            if ( prefabType != PrefabType.None ) {
                var o = PrefabUtility.GetPrefabObject( obj );
                if ( o == null )
                    o = obj;
                serializedObject = new SerializedObject( o );
                inspectorModeInfo.SetValue( serializedObject, InspectorMode.Debug, null );
                localIdProp = serializedObject.FindProperty( "m_LocalIdentfierInFile" );
                localId = localIdProp.intValue;

                if ( obj != PrefabUtility.FindPrefabRoot( obj as GameObject ) ) {
                    o = PrefabUtility.GetPrefabParent( obj );
                    if ( o != null ) {
                        serializedObject = new SerializedObject( o );
                        inspectorModeInfo.SetValue( serializedObject, InspectorMode.Debug, null );
                        localIdProp = serializedObject.FindProperty( "m_LocalIdentfierInFile" );
                        localSubId = localIdProp.intValue;
                    }
                }
            }
#endif
            return localId + localSubId;
        }

        private static UnitySceneMemoScene GetSceneMemo( string path ) {
            if ( Data == null )
                return null;

            var guid = AssetDatabase.AssetPathToGUID( path );
            if ( string.IsNullOrEmpty( guid ) )
                return null;

            return Data.GetScene( guid );
        }

    }

}