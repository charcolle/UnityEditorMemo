using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;

namespace charcolle.UnityEditorMemo {

    internal class UnityEditorMemoCategoryWindowTreeView : TreeView {

        public event Action<UnityEditorMemoCategory> OnContextClick;
        public event Action<List<UnityEditorMemoCategory>> OnCategoryOrderChanged;

        private List<UnityEditorMemoCategory> category;

        public UnityEditorMemoCategoryWindowTreeView( TreeViewState treeViewState, List<UnityEditorMemoCategory> category ) : base( treeViewState ) {
            this.category = category;
            showBorder = true;
            baseIndent = -7f;
            Reload();
        }

        public void BeginRename( UnityEditorMemoCategory rename ) {
            var id = category.IndexOf( rename );
            BeginRename( GetRows()[ id ] );
        }

        protected override TreeViewItem BuildRoot() {
            var root = new TreeViewItem { id = -1, depth = -1, displayName = "Root" };

            var items = category.Select( ( category, index ) => new TreeViewItem { id = index, depth = category.MenuDepth, displayName = category.Name } ).ToList();

            SetupParentsAndChildrenFromDepths( root, items );

            return root;
        }

        protected override bool CanMultiSelect( TreeViewItem item ) {
            return false;
        }

        protected override bool CanRename( TreeViewItem item ) {
            if( item.displayName == "default" )
                return false;
            return true;
        }

        protected override void RenameEnded( RenameEndedArgs args ) {
            if( category.Any( c => c.Name == args.newName ) )
                return;

            category[ args.itemID ].Name = args.newName;
            GetRows()[ args.itemID ].displayName = args.newName;
        }

        protected override void ContextClickedItem( int id ) {
            OnContextClick( category[ id ] );
        }

        protected override void RowGUI( RowGUIArgs args ) {
            var rect = args.rowRect;
            rect.x += 5f;
            var _category = category[ args.item.id ];
            var _categoryCount = _category.Memo.Count == 0 ? 0 : _category.Memo.Count - 1;
            GUI.Label( rect, new GUIContent( args.item.displayName, args.item.displayName + ": " + _categoryCount ) );
        }

        #region drag and drop

        private const string k_GenericDragID = "UnityEditorMemoCategoryDragging";

        protected override bool CanStartDrag( CanStartDragArgs args ) {
            return true;
        }

        protected override bool CanBeParent( TreeViewItem item ) {
            return false;
        }

        protected override void SetupDragAndDrop( SetupDragAndDropArgs args ) {
            if( hasSearch )
                return;

            DragAndDrop.PrepareStartDrag();
            var draggedRows = GetRows().Where( item => args.draggedItemIDs.Contains( item.id ) ).ToList();
            DragAndDrop.SetGenericData( k_GenericDragID, draggedRows );
            DragAndDrop.objectReferences = new UnityEngine.Object[] { };
            string title = draggedRows.Count == 1 ? draggedRows[ 0 ].displayName : "< Multiple >";
            DragAndDrop.StartDrag( title );
        }

        protected override DragAndDropVisualMode HandleDragAndDrop( DragAndDropArgs args ) {
            var draggedRows = DragAndDrop.GetGenericData( k_GenericDragID ) as List<TreeViewItem>;
            if( draggedRows == null )
                return DragAndDropVisualMode.None;

            // Parent item is null when dragging outside any tree view items.
            switch( args.dragAndDropPosition ) {
                case DragAndDropPosition.UponItem:
                case DragAndDropPosition.BetweenItems: {
                        var validDrag = ValidDrag( args.parentItem, draggedRows );
                        if( args.performDrop && validDrag )
                            OnDropDraggedElementsAtIndex( draggedRows, ( TreeViewItem )args.parentItem, args.insertAtIndex == -1 ? 0 : args.insertAtIndex );

                        return validDrag ? DragAndDropVisualMode.Move : DragAndDropVisualMode.None;
                    }

                case DragAndDropPosition.OutsideItems: {
                        if( args.performDrop )
                            OnDropDraggedElementsAtIndex( draggedRows, rootItem, rootItem.children.Count );

                        return DragAndDropVisualMode.Move;
                    }
                default:
                    Debug.LogError( "Unhandled enum " + args.dragAndDropPosition );
                    return DragAndDropVisualMode.None;
            }
        }

        public void OnDropDraggedElementsAtIndex( List<TreeViewItem> draggedRows, TreeViewItem parent, int insertIndex ) {
            if( parent == null || insertIndex < 0 )
                return;

            // latter index must be adjusted.
            if( insertIndex > draggedRows[ 0 ].id )
                --insertIndex;

            // change category order
            var categoryIds = category.Select( ( category, index ) => index ).ToList();
            categoryIds.Remove( draggedRows[0].id );
            categoryIds.Insert( insertIndex, draggedRows[ 0 ].id );

            category[ draggedRows[ 0 ].id ].MenuDepth = ++parent.depth;
            var newCategory = new List<UnityEditorMemoCategory>();
            foreach( var id in categoryIds )
                newCategory.Add( category[ id ] );

            category = newCategory;
            OnCategoryOrderChanged( newCategory );

            // restore selection
            var selectedIDs = new List<int>() { insertIndex };
            SetSelection( selectedIDs, TreeViewSelectionOptions.RevealAndFrame );

            Reload();
        }

        private bool ValidDrag( TreeViewItem parent, List<TreeViewItem> draggedItems ) {
            TreeViewItem currentParent = parent;
            while( currentParent != null ) {
                if( draggedItems.Contains( currentParent ) )
                    return false;
                currentParent = currentParent.parent;
            }
            return true;
        }

        #endregion

    }

}