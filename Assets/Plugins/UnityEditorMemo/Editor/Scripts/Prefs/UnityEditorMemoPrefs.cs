using UnityEngine;
using UnityEditor;

namespace charcolle.UnityEditorMemo {

    internal static class UnityEditorMemoPrefs {

        //======================================================================
        // Preference
        //======================================================================

        private static readonly string[] SCENEMEMO_POSITION = new string[] { "TopLeft", "BottomLeft", "BottomRight" };

        [PreferenceItem( "UnityEditorMemo" )]
        public static void PreferenceView() {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.HelpBox( "Setting will changed after unity editor compiled or played.", MessageType.Warning );
                UnitySceneMemoActive   = EditorGUILayout.Toggle( "Enable UnitySceneMemo", UnitySceneMemoActive );
                GUILayout.Space( 10 );
                GUILayout.Label( "Memo position at SceneView" );
                UnitySceneMemoPosition = GUILayout.Toolbar( UnitySceneMemoPosition, SCENEMEMO_POSITION );

                GUILayout.FlexibleSpace();
                GUILayout.Label( "Version " + UnityEditorMemoInfo.Version, EditorStyles.miniBoldLabel );
            }
            EditorGUILayout.EndVertical();
        }

        //======================================================================
        // Prefs
        //======================================================================

        private const string KEY_SCENEMEMO_AVAILABLE = "charcolle.UnityEditorMemo.SceneMemoAvailable";
        private const string KEY_SCENEMEMO_POSITION  = "charcolle.UnityEditorMemo.SceneMemoPosition";

        public static bool UnitySceneMemoActive {
            get {
                return EditorPrefs.GetBool( KEY_SCENEMEMO_AVAILABLE, false );
            }
            set {
                EditorPrefs.SetBool( KEY_SCENEMEMO_AVAILABLE, value );
            }
        }

        public static int UnitySceneMemoPosition {
            get {
                return EditorPrefs.GetInt( KEY_SCENEMEMO_POSITION, 0 );
            }
            set {
                EditorPrefs.SetInt( KEY_SCENEMEMO_POSITION, value );
            }
        }


    }

}