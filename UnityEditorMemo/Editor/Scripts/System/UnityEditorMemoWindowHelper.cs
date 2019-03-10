using System.Linq;
using System.Collections.Generic;
using UnityEngine;

using FileUtility = charcolle.UnityEditorMemo.FileUtility;

namespace charcolle.UnityEditorMemo {

    internal static class UnityEditorMemoWindowHelper {

        //======================================================================
        // window varies
        //======================================================================

        public static UnityEditorMemoSaveData Data { get; private set; }

        public static readonly Vector2  WINDOW_SIZE          = new Vector2( 350f, 400f );

        public const string WINDOW_TITLE                     = "UnityMemo";

        public const string TEXT_NO_MEMO                     = "no memo.";
        public const string TEXT_CREATEMEMO_TITLE            = "Post Memo To ";
        public const string TEXT_CATEGORY_DESC               = "Create New Category" ;
        public const string TEXT_LABEL_LIST                  = "Label Config";

        public const string WARNING_MEMO_EMPTY               = "UnityEditorMemoWindow: Memo cannot be empty.";
        public const string WARNING_CATEGORY_EMPTY           = "UnityEditorMemoWindow: Category name is empty.";

        public const string UNDO_POST                        = "UnityEditorMemo Post";
        public const string UNDO_DELETEPOST                  = "UnityEditorMemo Delete";
        public const string UNDO_CATEGORYCHANGE              = "UnityEditorMemo Category Change";
        public const string UNDO_EDITPOST                    = "UnityEditorMemo Edit Post";
        public const string UNDO_DRAFT                       = "UnityEditorMemo Edit Draft";

        public static readonly string[] MENU_DISPLAY_MEMO    = { "lastest 100", "older" };
        public static readonly string[] POSTMEMO_TYPE        = { "Normal", "Important", "Question" };

        public static readonly bool[] FOOTER_TOGGLE          = { true, false, false, false, false, false };

        //======================================================================
        // initialize
        //======================================================================

        public static void LoadData() {
            Data = FileUtility.LoadUnityEditorMemoData();
        }

        public static void OnGUIFirst( float windowWidth ) {
            GUI.skin.label.richText = true;
            GUI.skin.box.richText = true;
        }

        public static void OnGUIEnd() {
            GUI.skin.label.richText = false;
            GUI.skin.box.richText = false;
        }

        //======================================================================
        // public
        //======================================================================

        public static UnityEditorMemoCategory GetCategory( int idx ) {
            if ( Data == null )
                return null;

            if ( idx >= Data.Category.Count )
                idx = 0;
            return Data.Category[idx];
        }

        public static void CheckMemoHasRootElement( int categoryIdx ) {
            var category = GetCategory( categoryIdx );
            if( category != null ) {
                var memo = category.Memo;
                if( !memo.Any( m => m.depth == -1 ) ) {
                    for( int i = 0; i < memo.Count; i++ ) {
                        memo[ i ].id = i;
                        memo[ i ].depth = 0;
                    }
                    memo.Insert( 0, UnityEditorMemo.Root );
                }
            }
        }

        public static List<UnityEditorMemo> DisplayMemoList( UnityEditorMemoCategory currentCategory, int label ) {
            if ( Data == null )
                return null;

            return currentCategory.Memo.Where( m => label == 0 || m.Label == ( UnityEditorMemoLabel )label ).Reverse().ToList();
        }

        //======================================================================
        // footer toggle area Utility
        //======================================================================

        /// <summary>
        /// custom toggle system
        /// </summary>
        public static int ChangeFooterStatus( int label, ref bool[] footer ) {
            var selectedNum = checkSelectedToggle( ref footer );
            var selected = -1;
            for ( int i = 0; i < footer.Length; i++ ) {
                if( footer[i] ) {
                    if( label == i ) {
                        footer[i] = selectedNum == 2 ? false : true;
                    } else {
                        footer[i] = true;
                        selected = i;
                    }
                } else {
                    if ( label == i && selectedNum == 0 )
                        footer[i] = true;
                }
            }
            if ( selected == -1 )
                selected = label;

            return selected;
        }

        private static int checkSelectedToggle( ref bool[] footer ) {
            var selectedNum = 0;
            for( int i = 0; i < footer.Length; i++ ) {
                if ( footer[i] )
                    selectedNum++;
            }
            if( selectedNum > 2 ) // avoid error
                footer = new bool[] { true, false, false, false, false, false };
            return selectedNum;
        }


    }

}