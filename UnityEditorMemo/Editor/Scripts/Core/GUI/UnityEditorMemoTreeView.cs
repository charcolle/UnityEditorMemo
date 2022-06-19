using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

using UndoHelper = charcolle.UnityEditorMemo.UndoHelper;

// this code from unity technologies tree view sample
// http://files.unity3d.com/mads/TreeViewExamples.zip
namespace charcolle.UnityEditorMemo
{

    internal class UnityEditorMemoTreeView : TreeViewWithTreeModel<UnityEditorMemo>
    {
        public UnityEditorMemoLabel SelectLabel = UnityEditorMemoLabel.NORMAL;
        public event Action<UnityEditorMemo> OnContextClicked;
        public event Action<List<UnityEditorMemo>> OnMemoOrderChanged;

        private List<UnityEditorMemo> memo;
        private UnityEditorMemoSort sortType = UnityEditorMemoSort.DESCENDING;

        public UnityEditorMemoTreeView( TreeViewState state, TreeModel<UnityEditorMemo> model, List<UnityEditorMemo> memo, float rowRectWidth )
            : base( state, model )
        {
            this.memo = memo;
            this.rowRectWidth = rowRectWidth;
            showBorder = false;
            Reload();
        }

        public void SortMemo(UnityEditorMemoSort sortType)
        {
            this.sortType = sortType;
        }

        public UnityEditorMemoSort GetSortType()
        {
            return sortType;
        }

        public void UpdateRowHeight()
        {
            RefreshCustomRowHeights();
        }

        public void UpdateRowHeight( float rowRectWidth )
        {
            this.rowRectWidth = rowRectWidth;
            RefreshCustomRowHeights();
        }

        protected override IList<TreeViewItem> BuildRows( TreeViewItem root )
        {
            var rows = base.BuildRows( root );
            if (sortType == UnityEditorMemoSort.ASCENDING)
                rows = rows.Select( r => (TreeViewItem<UnityEditorMemo>)r )
                           .Where( m => SelectLabel == UnityEditorMemoLabel.NORMAL || m.data.Label == SelectLabel ).Select( s => (TreeViewItem)s ).ToList();
            else
                rows = rows.Select( r => (TreeViewItem<UnityEditorMemo>)r )
                           .Where( m => SelectLabel == UnityEditorMemoLabel.NORMAL || m.data.Label == SelectLabel ).Select( s => (TreeViewItem)s ).Reverse().ToList();

            for ( int i = 0; i < rows.Count; i++ )
            {
                var item = ( TreeViewItem<UnityEditorMemo> )rows[ i ];
                item.editorItem = new UnityEditorMemoEditorItem( item.data, RefreshCustomRowHeights );
            }
            return rows.ToList();
        }

        protected override bool DoesItemMatchSearch( TreeViewItem item, string search )
        {
            var target = ( TreeViewItem<UnityEditorMemo> )item;
            return target.data.Memo.Contains( search );
        }

        protected override void DoubleClickedItem( int id )
        {
            Undo.IncrementCurrentGroup();
            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
            var item = FindItem( id, rootItem );
            var target = ( TreeViewItem<UnityEditorMemo> )item;
            target.data.IsEdit = !target.data.IsEdit;
            RefreshCustomRowHeights();
        }

        protected override void ContextClickedItem( int id )
        {
            var item = FindItem( id, rootItem );
            var target = (TreeViewItem<UnityEditorMemo>)item;
            OnContextClicked( target.data );
            Event.current.Use();
        }

        protected override void ContextClicked()
        {
            OnContextClicked( null ); 
        }

        protected override bool CanMultiSelect( TreeViewItem item )
        {
            return false;
        }

        protected override bool CanBeParent( TreeViewItem item )
        {
            return true;
        }

        protected override bool CanStartDrag( CanStartDragArgs args )
        {
            return true;
        }

        //=======================================================
        // gui
        //=======================================================

        private const float SIZE_EMOJI = 32f;
        private const float SIZE_TEX = 23f;

        private float rowRectWidth = 0f;

        public override void OnGUI( Rect rect )
        {
            base.OnGUI( rect );
        }

        protected override float GetCustomRowHeight( int row, TreeViewItem item )
        {
            var data = ( TreeViewItem<UnityEditorMemo> )item;

            var memo = data.data;

            var itemHeight = GUIHelper.Styles.MemoHeader.fixedHeight;

            var memoHeight = memo.IsEdit ? GUIHelper.Styles.TextAreaWordWrap.CalcHeight( new GUIContent( memo.Memo ), rowRectWidth - SIZE_EMOJI - 26f )
                                         : GUIHelper.Styles.MemoLabel.CalcHeight( new GUIContent( memo.Memo ), rowRectWidth - SIZE_EMOJI - 26f );

            if ( SIZE_EMOJI > memoHeight )
            {
                itemHeight += SIZE_EMOJI;
                if ( memo.IsEdit )
                    itemHeight -= ( SIZE_EMOJI - memoHeight );
            } else
            {
                itemHeight += memoHeight;
            }

            if ( memo.IsEdit )
                itemHeight += EditorGUIUtility.singleLineHeight + SIZE_TEX + 15f + 30f;
            if ( memo.ObjectRef.HasReferenceObject() )
                itemHeight += EditorGUIUtility.singleLineHeight;
            itemHeight += 10f;
            return itemHeight;
        }

        protected override void RowGUI( RowGUIArgs args )
        {
            var item = ( TreeViewItem<UnityEditorMemo> )args.item;
            DrawBackgroundGUI( args.rowRect, args.selected );
            if ( item.editorItem != null )
                item.editorItem.OnGUI( args.rowRect );
        }

        private void DrawBackgroundGUI( Rect rect, bool isSelected )
        {
            if ( Event.current.rawType != EventType.Repaint )
                return;
            if ( isSelected )
                GUI.backgroundColor = Color.cyan;
            DefaultStyles.backgroundEven.Draw( rect, false, false, false, false );
            GUI.backgroundColor = Color.white;
        }

        #region drag and drop

        private const string k_GenericDragID = "UnityEditorMemoDragging";

        protected override void SetupDragAndDrop( SetupDragAndDropArgs args )
        {
            if ( hasSearch )
                return;

            DragAndDrop.PrepareStartDrag();
            var draggedRows = GetRows().Where( item => args.draggedItemIDs.Contains( item.id ) ).ToList();
            DragAndDrop.SetGenericData( k_GenericDragID, draggedRows );
            DragAndDrop.objectReferences = new UnityEngine.Object[] { };
            string title = draggedRows[ 0 ].displayName;
            DragAndDrop.StartDrag( title );
        }

        protected override DragAndDropVisualMode HandleDragAndDrop( DragAndDropArgs args )
        {
            // remove item which came from category.
            var draggedCategory = DragAndDrop.GetGenericData( "UnityEditorMemoCategoryDragging" );
            if ( draggedCategory != null )
                return DragAndDropVisualMode.None;

            var draggedMemo = DragAndDrop.GetGenericData( "UnityEditorMemoDragging" );
            if ( draggedMemo != null )
            {
                //var draggedItem = draggedMemo as List<TreeViewItem>;
                switch ( args.dragAndDropPosition )
                {
                    case DragAndDropPosition.BetweenItems:
                        {
                            //var validDrag = isValidDrag( args.parentItem, draggedItem );
                            //if ( args.performDrop && validDrag )
                            //    OnDropDraggedElementsAtIndex( draggedItem, ( TreeViewItem )args.parentItem, args.insertAtIndex == -1 ? 0 : args.insertAtIndex );

                            return DragAndDropVisualMode.None;
                        }

                    //case DragAndDropPosition.OutsideItems:
                    //    {
                    //        if ( args.performDrop )
                    //            OnDropDraggedElementsAtIndex( draggedItem, rootItem, 1 );

                    //        return DragAndDropVisualMode.Move;
                    //    }
                }
            } else
            {
                // asset dragging
                switch ( args.dragAndDropPosition )
                {
                    case DragAndDropPosition.UponItem:
                        {
                            if ( args.performDrop )
                            {
                                UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                                var target = ( TreeViewItem<UnityEditorMemo> )args.parentItem;
                                target.data.ObjectRef = new UnityEditorMemoObject( DragAndDrop.objectReferences[ 0 ] );
                                RefreshCustomRowHeights();
                            }
                            return DragAndDropVisualMode.Move;
                        }
                }
            }

            return DragAndDropVisualMode.None;
        }

        public void OnDropDraggedElementsAtIndex( List<TreeViewItem> draggedRows, TreeViewItem parent, int insertIndex )
        {
            if ( parent == null || insertIndex < 0 )
                return;

            Debug.Log( insertIndex );
            if ( insertIndex > draggedRows[ 0 ].id )
                ++insertIndex;
            Debug.Log( insertIndex );

            // change memo order
            var memoIds = memo.Select( ( memo, index ) => memo.id ).ToList();
            memoIds.Remove( draggedRows[ 0 ].id );
            memoIds.Insert( insertIndex, draggedRows[ 0 ].id );

            var newMemos = new List<UnityEditorMemo>();
            foreach ( var id in memoIds )
                newMemos.Add( memo[ id ] );

            memo = newMemos;

            // restore selection
            var selectedIDs = new List<int>() { insertIndex };
            SetSelection( selectedIDs, TreeViewSelectionOptions.RevealAndFrame );

            if ( OnMemoOrderChanged != null )
                OnMemoOrderChanged.Invoke( newMemos );
        }

        private bool isValidDrag( TreeViewItem parent, List<TreeViewItem> draggedItems )
        {
            TreeViewItem currentParent = parent;
            while ( currentParent != null )
            {
                if ( draggedItems.Contains( currentParent ) )
                    return false;
                currentParent = currentParent.parent;
            }
            return true;
        }

        #endregion

    }

}