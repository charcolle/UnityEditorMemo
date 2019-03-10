using UnityEditor;
using UnityEngine;

namespace charcolle.UnityEditorMemo {

    internal abstract class EditorWindowItem<T> {

        [SerializeField]
        protected T data;

        public EditorWindowItem( T data ) {
            this.data = data;
        }

        public T Data {
            get {
                return data;
            }
        }

        public void OnGUI() {
            if( data == null ) {
                DrawIfDataIsNull();
                return;
            }
            EditorGUI.BeginChangeCheck();
            Draw();
            if( EditorGUI.EndChangeCheck() )
                GUI.changed = true;
        }

        public void OnGUI( Rect rect ) {
            if( data == null ) {
                DrawIfDataIsNull();
                return;
            }

            EditorGUI.BeginChangeCheck();
            Draw( rect );
            if( EditorGUI.EndChangeCheck() )
                GUI.changed = true;
        }

        protected virtual void Draw() { }
        protected virtual void Draw( Rect rect ) { }
        protected virtual void DrawIfDataIsNull() { }

    }

}