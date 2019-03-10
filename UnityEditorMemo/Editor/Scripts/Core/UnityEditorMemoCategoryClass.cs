using System;
using System.Collections.Generic;
using UnityEngine;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    internal class UnityEditorMemoCategory {

        public string Name;
        public DateTime Date;
        public int MenuDepth;

        [SerializeField]
        public List<UnityEditorMemo> Memo = new List<UnityEditorMemo>();

        public UnityEditorMemoCategory( string name ) {
            Name = name;
            Date = DateTime.Now;
            Memo = new List<UnityEditorMemo>();
            MenuDepth = 0;
        }

        public void Initialize() {
            for( int i = 0; i < Memo.Count; i++ )
                Memo[ i ].Initialize( i );
        }

        public void AddMemo( UnityEditorMemo memo ) {
            Memo.Add( memo );
        }

    }

}