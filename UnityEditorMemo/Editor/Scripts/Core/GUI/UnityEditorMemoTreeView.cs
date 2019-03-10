using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UndoHelper = charcolle.UnityEditorMemo.UndoHelper;

// this code from unity technologies tree view sample
// http://files.unity3d.com/mads/TreeViewExamples.zip
namespace charcolle.UnityEditorMemo {

    internal class UnityEditorMemoTreeView : TreeViewWithTreeModel<UnityEditorMemo> {

        public UnityEditorMemoLabel SelectLabel = UnityEditorMemoLabel.NORMAL;
        public event Action<UnityEditorMemo> OnContextClicked;

        public UnityEditorMemoTreeView( TreeViewState state, TreeModel<UnityEditorMemo> model, float rowRectWidth )
            : base( state, model ) {
            this.rowRectWidth = rowRectWidth;
            showBorder = false;
            Reload();
        }

        public void UpdateRowHeight() {
            RefreshCustomRowHeights();
        }

        public void UpdateRowHeight( float rowRectWidth ) {
            this.rowRectWidth = rowRectWidth;
            RefreshCustomRowHeights();
        }

        protected override IList<TreeViewItem> BuildRows( TreeViewItem root ) {
            var rows = base.BuildRows( root );
            rows = rows.Select( r => ( TreeViewItem<UnityEditorMemo> )r )
                       .Where( m => SelectLabel == UnityEditorMemoLabel.NORMAL || m.data.Label == SelectLabel ).Select( s => ( TreeViewItem )s ).Reverse().ToList();
            for( int i = 0; i < rows.Count; i++ ) {
                var item = ( TreeViewItem<UnityEditorMemo> )rows[ i ];
                item.editorItem = new UnityEditorMemoEditorItem( item.data, RefreshCustomRowHeights );
            }
            return rows.ToList();
        }

        protected override bool DoesItemMatchSearch( TreeViewItem item, string search ) {
            var target = ( TreeViewItem<UnityEditorMemo> )item;
            return target.data.Memo.Contains( search );
        }

        protected override void DoubleClickedItem( int id ) {
            Undo.IncrementCurrentGroup();
            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
            var item = FindItem( id, rootItem );
            var target = ( TreeViewItem<UnityEditorMemo> )item;
            target.data.IsEdit = !target.data.IsEdit;
            RefreshCustomRowHeights();
        }

        protected override void ContextClickedItem( int id ) {
            var item = FindItem( id, rootItem );
            var target = ( TreeViewItem<UnityEditorMemo> )item;
            OnContextClicked( target.data );
        }

        protected override bool CanMultiSelect( TreeViewItem item ) {
            return false;
        }

        protected override bool CanBeParent( TreeViewItem item ) {
            return true;
        }

        protected override bool CanStartDrag( CanStartDragArgs args ) {
            return false;
        }

        //=======================================================
        // gui
        //=======================================================

        private const float SIZE_EMOJI = 32f;
        private const float SIZE_TEX = 23f;

        private float rowRectWidth = 0f;

        public override void OnGUI( Rect rect ) {
            base.OnGUI( rect );
        }

        protected override float GetCustomRowHeight( int row, TreeViewItem item ) {
            var data = ( TreeViewItem<UnityEditorMemo> )item;

            var memo = data.data;

            var itemHeight = GUIHelper.Styles.MemoHeader.fixedHeight;

            var memoHeight = memo.IsEdit ? GUIHelper.Styles.TextAreaWordWrap.CalcHeight( new GUIContent( memo.Memo ), rowRectWidth - SIZE_EMOJI - 26f )
                                         : GUIHelper.Styles.MemoLabel.CalcHeight( new GUIContent( memo.Memo ), rowRectWidth - SIZE_EMOJI - 26f );

            if( SIZE_EMOJI > memoHeight ) {
                itemHeight += SIZE_EMOJI;
                if( memo.IsEdit )
                    itemHeight -= ( SIZE_EMOJI - memoHeight );
            } else {
                itemHeight += memoHeight;
            }

            if( memo.IsEdit )
                itemHeight += EditorGUIUtility.singleLineHeight + SIZE_TEX + 15f + 20f;
            if( memo.ObjectRef.HasReferenceObject() )
                itemHeight += EditorGUIUtility.singleLineHeight;
            itemHeight += 10f;
            return itemHeight;
        }

        protected override void RowGUI( RowGUIArgs args ) {
            var item = ( TreeViewItem<UnityEditorMemo> )args.item;
            item.editorItem.OnGUI( args.rowRect );
        }

        //=======================================================
        // drag and drop
        //=======================================================

        protected override DragAndDropVisualMode HandleDragAndDrop( DragAndDropArgs args ) {
            // remove item which came from category.
            var draggedCategory = DragAndDrop.GetGenericData( "UnityEditorMemoCategoryDragging" );
            if( draggedCategory != null )
                return DragAndDropVisualMode.None;

            switch( args.dragAndDropPosition ) {
                case DragAndDropPosition.UponItem: {
                        if( args.performDrop ) {
                            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                            var target = ( TreeViewItem<UnityEditorMemo> )args.parentItem;
                            target.data.ObjectRef = new UnityEditorMemoObject( DragAndDrop.objectReferences[0] );
                            RefreshCustomRowHeights();
                        }
                        return DragAndDropVisualMode.Move;
                    }
            }
            return DragAndDropVisualMode.None;
        }

    }

}