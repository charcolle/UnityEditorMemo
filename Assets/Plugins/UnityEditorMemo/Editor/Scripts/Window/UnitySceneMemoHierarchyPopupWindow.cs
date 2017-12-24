using UnityEngine;
using UnityEditor;

using SceneMemoHelper = charcolle.UnityEditorMemo.UnitySceneMemoHelper;

namespace charcolle.UnityEditorMemo {

    public class UnitySceneMemoHierarchyWindow : PopupWindowContent {

        private UnitySceneMemo Memo;

        public void Initialize( UnitySceneMemo memo ) {
            Memo = memo;
            Memo.Initialize( Memo.InstanceId );
            Memo.isEdit = false;
        }

        public override void OnOpen() {
            base.OnOpen();

            editorWindow.minSize = new Vector2( 250, 150 );
            editorWindow.maxSize = new Vector2( 350, 200 );
            Undo.undoRedoPerformed += editorWindow.Repaint;
        }

        public override void OnClose() {
            Undo.undoRedoPerformed -= editorWindow.Repaint;
        }

        public override void OnGUI( Rect rect ) {
            //Debug.Log( editorWindow.position.x + " " + editorWindow.position.y + " " + editorWindow.position.width + " " + editorWindow.position.height );
            if( Memo == null ) {
                editorWindow.Close();
                return;
            }

            EditorGUI.BeginChangeCheck();

            DrawMemo();

            if ( EditorGUI.EndChangeCheck() )
                SceneMemoHelper.SetDirty();
        }

        private void DrawMemo() {
            Memo.OnGUI();
            if( Memo.IsContextClick ) {
                var menu = new GenericMenu();
                menu.AddItem( new GUIContent( "Edit" ), false, () => {
                    Memo.isEdit = true;
                } );
                menu.AddItem( new GUIContent( "Delete" ), false, () => {
                    UndoHelper.SceneMemoUndo( UndoHelper.UNDO_SCENEMEMO_DELETE );
                    SceneMemoHelper.RemoveMemo( Memo );
                    Memo = null;
                    editorWindow.Close();
                } );
                menu.ShowAsContext();
            }
        }

        public override Vector2 GetWindowSize() {
            return new Vector2( 250, 150 );
        }

    }

}