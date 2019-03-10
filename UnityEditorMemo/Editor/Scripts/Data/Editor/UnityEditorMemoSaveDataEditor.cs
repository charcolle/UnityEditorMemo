using UnityEditor;

namespace charcolle.UnityEditorMemo {

    [CustomEditor( typeof( UnityEditorMemoSaveData ) )]
    public class UnityEditorMemoSaveDataEditor : Editor {

        public override void OnInspectorGUI() {
            EditorGUI.BeginDisabledGroup( true );
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();
        }

    }

}