using System;
using UnityEngine;
using UnityEditor;

using GUIHelper   = charcolle.UnityEditorMemo.GUIHelper;
using UndoHelper  = charcolle.UnityEditorMemo.UndoHelper;
using FileUtility = charcolle.UnityEditorMemo.FileUtility;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    public class UnityEditorMemo {

        public string Date;
        public string Memo;
        public UnityEditorMemoLabel Label;
        public UnityEditorMemoTexture Tex;
        public UnityEditorMemoObject ObjectRef;
        public bool isFold;

        public UnityEditorMemo( string memo, int type, int tex ) {
            Date        = DateTime.Now.RenderDate();
            Memo        = memo;
            Label       = ( UnityEditorMemoLabel )type;
            Tex         = ( UnityEditorMemoTexture )tex;
            ObjectRef   = new UnityEditorMemoObject();
        }

        [NonSerialized]
        public bool IsContextClick  = false;
        private Rect rect           = Rect.zero;
        public void OnGUI() {
            rect = EditorGUILayout.BeginVertical();
            {
                // header
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( Label );
                var preState = isFold;
                EditorGUILayout.BeginHorizontal( GUIHelper.Styles.MemoHeader );
                {
                    GUILayout.Space( 5f );
                    GUILayout.Label( Date.ToBold(), new GUILayoutOption[] { GUILayout.ExpandWidth( true ), GUILayout.Height( 18 ) } );
                    GUILayout.FlexibleSpace();
                    var fold = GUILayout.Toggle( isFold, "≡", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 20 ) } );
                    if ( fold != preState ) {
                        UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                        GUIUtility.keyboardControl = 0;
                        isFold = fold;
                    }
                }
                EditorGUILayout.EndHorizontal();
                GUI.backgroundColor = Color.white;

                EditorGUILayout.BeginVertical( GUIHelper.Styles.MemoBack );
                {
                    GUILayout.Space( 2 );

                    // memo
                    EditorGUILayout.BeginHorizontal();
                    {
                        GUILayout.Space( 5 );
                        EditorGUILayout.BeginVertical( GUILayout.Width( 32 ) );
                        GUILayout.Box( GUIHelper.Textures.Emotions[( int )Tex], GUIStyle.none, new GUILayoutOption[] { GUILayout.Width( 32 ), GUILayout.Height( 32 ) } );
                        EditorGUILayout.EndVertical();

                        // Display Or Edit Memo
                        if ( isFold ) {
                            Undo.IncrementCurrentGroup();
                            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                            Memo = EditorGUILayout.TextArea( Memo, GUIHelper.Styles.TextAreaWordWrap );
                        } else {
                            GUILayout.Label( Memo, GUIHelper.Styles.LabelWordWrap );
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    ObjectRef.Draw( isFold );

                    GUILayout.Space( 5f );

                    // Display Memo Edit Buttons
                    if ( isFold ) {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.FlexibleSpace();
                            Tex = ( UnityEditorMemoTexture )GUILayout.Toolbar( ( int )Tex, GUIHelper.Textures.Emotions, new GUILayoutOption[] { GUILayout.Height( 25 ), GUILayout.Width( 120 ) } );
                            Label = ( UnityEditorMemoLabel )EditorGUILayout.Popup( ( int )Label, GUIHelper.Label, GUILayout.Width( 100 ) );
                        }
                        EditorGUILayout.EndHorizontal();
                        GUILayout.Space( 5 );
                    }
                }
                EditorGUILayout.EndVertical();

            }
            EditorGUILayout.EndVertical();

            eventProcess( Event.current );
        }

        private void eventProcess( Event e ) {
            switch ( e.type ) {
                case EventType.ContextClick:
                    if ( rect.Contains( e.mousePosition ) ) {
                        IsContextClick = true;
                        e.Use();
                    } else {
                        IsContextClick = false;
                    }
                    break;
                default:
                    var obj = FileUtility.GetDraggedObject( e, rect );
                    if ( obj != null ) {
                        UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                        ObjectRef = new UnityEditorMemoObject();
                        ObjectRef.SetReference( obj );
                        e.Use();
                    }
                    IsContextClick = false;
                    break;
            }

        }

    }

}