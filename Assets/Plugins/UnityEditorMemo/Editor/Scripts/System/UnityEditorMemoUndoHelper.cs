using UnityEditor;

using Window          = charcolle.UnityEditorMemo.UnityEditorMemoWindow;
using EditorMemo      = charcolle.UnityEditorMemo.UnityEditorMemoWindowHelper;
using SceneMemoHelper = charcolle.UnityEditorMemo.UnitySceneMemoHelper;

namespace charcolle.UnityEditorMemo {

    internal static class UndoHelper {

        public static string UNDO_DRAFT             = "edit memo";
        public static string UNDO_POST              = "post memo";
        public static string UNDO_MEMO_EDIT         = "edit memo";
        public static string UNDO_DELETE_MEMO       = "delete memo";
        public static string UNDO_EDIT_CATEGORY     = "edit category";
        public static string UNDO_DELETE_CATEGORY   = "delete category";
        public static string UNDO_CREATE_CATEGORY   = "create category";

        public static string UNDO_SCENEMEMO_ADD     = "add scene memo";
        public static string UNDO_SCENEMEMO_EDIT    = "edit scene memo";
        public static string UNDO_SCENEMEMO_DELETE  = "delete scene memo";

        public static string UNDO_CHANGE_CATEGORY   = "change category";
        public static string UNDO_CHANGE_LABEL      = "change label";

        public static void EditorMemoUndo( string text ) {
            if ( EditorMemo.Data != null )
                Undo.RecordObject( EditorMemo.Data, text );
        }

        public static void SceneMemoUndo( string text ) {
            if ( SceneMemoHelper.Data != null )
                Undo.RecordObject( SceneMemoHelper.Data, text );
        }

        public static void WindowUndo( string text ) {
            if ( Window.win != null ) {
                Undo.IncrementCurrentGroup();
                Undo.RecordObject( Window.win, text );
            }
        }

    }
}