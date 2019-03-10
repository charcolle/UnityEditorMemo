using UnityEditor;

namespace charcolle.UnityEditorMemo {

    [CustomEditor( typeof( UnitySceneMemoSaveData ) )]
    public class UnitySceneMemoSaveDataEditor : Editor {

        public override void OnInspectorGUI() {
            EditorGUI.BeginDisabledGroup( true );
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();
        }

    }

}