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
        public string URL;
        public UnityEditorMemoLabel Label;
        public UnityEditorMemoTexture Tex;
        public UnityEditorMemoObject ObjectRef;
        public bool isFoldout;

        public UnityEditorMemo( string memo, int type, int tex, string url ) {
            Date        = DateTime.Now.RenderDate();
            Memo        = memo;
            Label       = ( UnityEditorMemoLabel )type;
            Tex         = ( UnityEditorMemoTexture )tex;
            ObjectRef   = new UnityEditorMemoObject();
            URL         = url;
        }

        [NonSerialized]
        public bool IsContextClick  = false;
        private Rect rect           = Rect.zero;
        public void OnGUI() {
            rect = EditorGUILayout.BeginVertical();
            {
                // header
                GUI.backgroundColor = GUIHelper.Colors.LabelColor( Label );
                var preState = isFoldout;
                EditorGUILayout.BeginHorizontal( GUIHelper.Styles.MemoHeader );
                {
                    var fold = GUILayout.Toggle( isFoldout, "≡", EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 20 ) } );
                    if( fold != preState ) {
                        UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                        GUIUtility.keyboardControl = 0;
                        isFoldout = fold;
                    }

                    GUILayout.Label( Date.ToBold(), new GUILayoutOption[] { GUILayout.ExpandWidth( true ), GUILayout.Height( 18 ) } );

                    GUILayout.FlexibleSpace();

                    if( !string.IsNullOrEmpty( URL ) ) {
                        if( GUILayout.Button( new GUIContent( "open", GUIHelper.Textures.OpenLink ), EditorStyles.toolbarButton, new GUILayoutOption[] { GUILayout.Width( 60 ) } ) ) {
                            Application.OpenURL( URL );
                        }
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

                        // display or edit memo
                        if ( isFoldout ) {
                            Undo.IncrementCurrentGroup();
                            UndoHelper.EditorMemoUndo( UndoHelper.UNDO_MEMO_EDIT );
                            Memo = EditorGUILayout.TextArea( Memo, GUIHelper.Styles.TextAreaWordWrap );
                        } else {
                            GUIHelper.Styles.LabelWordWrap.fontSize = UnityEditorMemoPrefs.UnityEditorMemoFontSize;
                            GUILayout.Label( Memo, GUIHelper.Styles.LabelWordWrap );
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    ObjectRef.Draw( isFoldout );

                    GUILayout.Space( 5 );

                    // edit memo menu
                    if ( isFoldout ) {
                        EditorGUILayout.BeginHorizontal();
                        {
                            GUILayout.Space( 37 );

                            GUILayout.Label( "URL", GUILayout.Width( 30 ) );
                            URL = EditorGUILayout.TextField( URL, GUIHelper.Styles.TextFieldWordWrap );

                            Tex = ( UnityEditorMemoTexture )GUILayout.Toolbar( ( int )Tex, GUIHelper.Textures.Emotions, new GUILayoutOption[] { GUILayout.Height( 23 ), GUILayout.Width( 110 ) } );
                            Label = ( UnityEditorMemoLabel )EditorGUILayout.Popup( ( int )Label, GUIHelper.LabelMenu, GUIHelper.Styles.LargeDropdown, GUILayout.Width( 90 ) );
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