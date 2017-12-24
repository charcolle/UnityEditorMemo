using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    public class UnityEditorMemoCategory {

        public string Name;
        public DateTime Date;

        [SerializeField]
        public List<UnityEditorMemo> Memo = new List<UnityEditorMemo>();

        public UnityEditorMemoCategory( string name ) {
            Name = name;
            Date = DateTime.Now;
            Memo = new List<UnityEditorMemo>();
        }

        public void Initialize() {
            for ( int i = 0; i < Memo.Count; i++ )
                Memo[i].ObjectRef.Initialize();
        }

        public void AddMemo( UnityEditorMemo memo ) {
            Memo.Add( memo );
        }

        public void OnCategoryChange() {
            for ( int i = 0; i < Memo.Count; i++ ) {
                Memo[i].isFold = false;
                Memo[i].ObjectRef.Initialize();
            }
        }

        private const int MaxMemoDisplay = 100;
        public bool IsDevideMemo( int label ) {
            if ( Memo.Count <= MaxMemoDisplay )
                return false;

            var memo = Memo.Where( m => label == 0 || m.Label == ( UnityEditorMemoLabel )label ).ToList();
            if ( memo.Count > MaxMemoDisplay )
                return true;
            else
                return false;
        }

        public void OnGUI( bool isEdit ) {
            EditorGUILayout.BeginHorizontal( GUI.skin.box, new GUILayoutOption[] { GUILayout.Height( 20 ), GUILayout.ExpandWidth( true ) } );
            {
                if( Name != "default" && isEdit ) {
                    Name = GUILayout.TextField( Name, GUIHelper.Styles.TextAreaWordWrap );
                } else {
                    GUILayout.Label( Name, GUIHelper.Styles.LabelWordWrap, GUILayout.ExpandWidth( true ) );
                }
                GUILayout.Label( Memo.Count.ToString(), GUILayout.Width( 50 ) );
                GUILayout.Label( Memo.Count == 0 ? "none" : Memo[Memo.Count - 1].Date, GUILayout.Width( 120 ) );
            }
            EditorGUILayout.EndHorizontal();
        }

    }

}