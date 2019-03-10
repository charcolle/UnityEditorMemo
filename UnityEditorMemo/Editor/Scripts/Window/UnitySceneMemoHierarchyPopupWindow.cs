using UnityEngine;
using UnityEditor;

using SceneMemoHelper = charcolle.UnityEditorMemo.UnitySceneMemoHelper;

namespace charcolle.UnityEditorMemo {

    internal class UnitySceneMemoHierarchyWindow : PopupWindowContent {

        private UnitySceneMemo memo;
        private UnitySceneMemoHierarchyEditorItem memoEditorItem;

        public void Initialize( UnitySceneMemo memo ) {
            this.memo = memo;
            memoEditorItem = new UnitySceneMemoHierarchyEditorItem( memo );
        }

        public override void OnOpen() {
            base.OnOpen();

            if( memo.SceneMemoWidth == 0 ) {
                memo.SceneMemoWidth = 200f;
                memo.SceneMemoWidth = 100f;
            }

            editorWindow.minSize = new Vector2( 250, 150 );
            editorWindow.maxSize = new Vector2( 350, 200 );
            Undo.undoRedoPerformed += editorWindow.Repaint;
        }

        public override void OnClose() {
            Undo.undoRedoPerformed -= editorWindow.Repaint;
        }

        public override void OnGUI( Rect rect ) {
            if( memoEditorItem == null ) {
                editorWindow.Close();
                return;
            }

            EditorGUI.BeginChangeCheck();

            memoEditorItem.OnGUI();
            if( memoEditorItem.IsContextClick ) {
                var menu = new GenericMenu();
                menu.AddItem( new GUIContent( "Edit" ), false, () => {
                    memoEditorItem.IsEdit = true;
                } );
                menu.AddItem( new GUIContent( "Delete" ), false, () => {
                    UndoHelper.SceneMemoUndo( UndoHelper.UNDO_SCENEMEMO_DELETE );
                    SceneMemoHelper.RemoveMemo( memo );
                    memo = null;
                    editorWindow.Close();
                } );
                menu.ShowAsContext();
            }

            if ( EditorGUI.EndChangeCheck() )
                SceneMemoHelper.SetDirty();
        }

        public override Vector2 GetWindowSize() {
            if( memo.ShowAtScene && memoEditorItem.IsEdit ) {
                return new Vector2( 270, 200 );
            } else {
                return new Vector2( 270, 150 );
            }
        }

    }

}