using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    public class UnitySceneMemoScene {

        public string Guid;
        [SerializeField]
        public List<UnitySceneMemo> Memo = new List<UnitySceneMemo>();

        private Dictionary<int, UnitySceneMemo> CachedMemo = new Dictionary<int, UnitySceneMemo>();

        public UnitySceneMemoScene( string guid ) {
            Guid = guid;
            Memo = new List<UnitySceneMemo>();
        }

        public void Initialize() {
            CachedMemo = new Dictionary<int, UnitySceneMemo>();
        }

        public static implicit operator string( UnitySceneMemoScene scene ) {
            return scene.Guid;
        }

        //=======================================================
        // public
        //=======================================================

        public void AddMemo( GameObject obj, int localIdentifierInFile ) {
            if ( GetMemo( obj ) != null ) {
                Debug.LogError( "dame dame" );
                return;
            }

            Memo.Add( new UnitySceneMemo( localIdentifierInFile, Guid ) );
        }

        public UnitySceneMemo GetMemo( GameObject obj ) {
            var instanceId = obj.GetInstanceID();
            UnitySceneMemo memo = getMemoFromCache( instanceId );
            if( memo == null ) {
                var localIdentifierInFile = UnitySceneMemoHelper.GetLocalIdentifierInFile( obj );
                memo = GetMemo( localIdentifierInFile );
                if ( memo != null ) {
                    memo.Initialize( instanceId );
                    CachedMemo.Add( instanceId, memo );
                }
            }
            return memo;
        }

        public UnitySceneMemo GetMemo( int localIdentifierInFile ) {
            return Memo.FirstOrDefault( m => m == localIdentifierInFile );
        }

        public void DeleteMemo( UnitySceneMemo memo ) {
            CachedMemo.Remove( memo.InstanceId );
            Memo.Remove( memo );
        }

        //=======================================================
        // private
        //=======================================================

        public UnitySceneMemo getMemoFromCache( int instanceId ) {
            if ( CachedMemo == null ) {
                Initialize();
                return null;
            }

            UnitySceneMemo memo = null;
            CachedMemo.TryGetValue( instanceId, out memo );
            return memo;
        }

    }

}