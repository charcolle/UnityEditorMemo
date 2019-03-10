using System;
using UnityEngine;
using UnityEditor;

using Object     = UnityEngine.Object;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    internal class UnityEditorMemoObject {

        public Object Obj;
        public string ScenePath;
        public int LocalIdentifierInFile;

        public UnitySceneMemo SceneMemo { get; set; }

        public UnityEditorMemoObject( Object obj ) {
            if( obj == null )
                return;

            var path = AssetDatabase.GetAssetPath( obj );
            if( string.IsNullOrEmpty( path ) )
                sceneObjectProcess( obj );
            else
                projectAssetProcess( obj );
        }

        public void Initialize() {
            if( isSceneMemoValid ) {
                SceneMemo = UnitySceneMemoHelper.GetMemo( ScenePath, LocalIdentifierInFile );
                if( SceneMemo == null ) {
                    ScenePath = "";
                    LocalIdentifierInFile = 0;
                }
            }
        }

        public bool HasReferenceObject() {
            return Obj != null || isSceneMemoValid;
        }

        //======================================================================
        // process
        //======================================================================

        private void projectAssetProcess( Object obj ) {
            Obj = obj;
            ScenePath = "";
            LocalIdentifierInFile = 0;
        }

        private void sceneObjectProcess( Object obj ) {
            var go = obj as GameObject;
            SceneMemo = UnitySceneMemoHelper.GetMemo( go );
            if( SceneMemo != null ) {
                ScenePath = go.scene.path;
                LocalIdentifierInFile = SceneMemo.LocalIdentifierInFile;
            }
            Obj = null;
        }

        private bool isSceneMemoValid {
            get {
                return !string.IsNullOrEmpty( ScenePath ) && LocalIdentifierInFile != 0;
            }
        }

    }

}