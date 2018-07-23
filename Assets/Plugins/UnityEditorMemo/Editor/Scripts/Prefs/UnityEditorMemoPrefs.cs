using UnityEngine;
using UnityEditor;

namespace charcolle.UnityEditorMemo {

    internal static class UnityEditorMemoPrefs {

        //======================================================================
        // Preference
        //======================================================================

        private static readonly string[] SCENEMEMO_POSITION = new string[] { "TopLeft", "BottomLeft", "BottomRight" };

        [PreferenceItem( "UnityMemo" )]
        public static void PreferenceView() {
            EditorGUILayout.BeginVertical();
            {
                EditorGUILayout.BeginHorizontal();
                {
                    GUILayout.Label( "Memo Font Size" );
                    UnityEditorMemoFontSize = EditorGUILayout.IntSlider( UnityEditorMemoFontSize, 9, 20 );
                }
                EditorGUILayout.EndHorizontal();
                GUILayout.Space( 10 );

                EditorGUILayout.HelpBox( "Setting will changed after unity editor compiled or played.", MessageType.Warning );
                UnitySceneMemoActive   = EditorGUILayout.Toggle( "Enable UnitySceneMemo", UnitySceneMemoActive );
                GUILayout.Space( 10 );
                GUILayout.Label( "Memo position at SceneView" );
                UnitySceneMemoPosition = GUILayout.Toolbar( UnitySceneMemoPosition, SCENEMEMO_POSITION );

                GUILayout.Space( 10 );
                GUILayout.Label( "UnityEditorMemo with Slack" );
                UnityEditorMemoUseSlack = EditorGUILayout.Toggle( "Use Slack integration", UnityEditorMemoUseSlack );
                if( UnityEditorMemoUseSlack ) {
                    EditorGUILayout.BeginVertical( EditorStyles.helpBox );
                    {
                        GUILayout.Label( "AccessToken" );
                        UnityEditorMemoSlackAccessToken = EditorGUILayout.TextField( UnityEditorMemoSlackAccessToken );
                        GUILayout.Space( 3 );
                        GUILayout.Label( "Channel" );
                        UnityEditorMemoSlackChannel = EditorGUILayout.TextField( UnityEditorMemoSlackChannel );
                        GUILayout.Space( 3 );
                    }
                    EditorGUILayout.EndVertical();
                }


                GUILayout.FlexibleSpace();
                GUILayout.Label( "Version " + UnityEditorMemoInfo.Version, EditorStyles.miniBoldLabel );
            }
            EditorGUILayout.EndVertical();
        }

        //======================================================================
        // Prefs
        //======================================================================

        private const string KEY_SCENEMEMO_AVAILABLE     = "charcolle.UnityEditorMemo.SceneMemoAvailable";
        private const string KEY_SCENEMEMO_POSITION      = "charcolle.UnityEditorMemo.SceneMemoPosition";
        private const string KEY_EDITORMEMO_FONTSIZE     = "charcolle.UnityEditorMemo.FontSize";
        private const string KEY_EDITORMEMO_USESLACK     = "charcolle.UnityEditorMemo.UseSlack";
        private const string KEY_EDITORMEMO_SLACKTOKEN   = "charcolle.UnityEditorMemo.SlackToken";
        private const string KEY_EDITORMEMO_SLACKCHANNEL = "charcolle.UnityEditorMemo.SlackChannel";

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

        public static int UnityEditorMemoFontSize {
            get {
                return EditorPrefs.GetInt( KEY_EDITORMEMO_FONTSIZE, 11 );
            }
            set {
                EditorPrefs.SetInt( KEY_EDITORMEMO_FONTSIZE, value );
            }
        }

        public static bool UnityEditorMemoUseSlack {
            get {
                return EditorPrefs.GetBool( KEY_EDITORMEMO_USESLACK, false );
            }
            set {
                EditorPrefs.SetBool( KEY_EDITORMEMO_USESLACK, value );
            }
        }

        public static string UnityEditorMemoSlackAccessToken {
            get {
                return EditorPrefs.GetString( KEY_EDITORMEMO_SLACKTOKEN, "" );
            }
            set {
                EditorPrefs.SetString( KEY_EDITORMEMO_SLACKTOKEN, value );
            }
        }

        public static string UnityEditorMemoSlackChannel {
            get {
                return EditorPrefs.GetString( KEY_EDITORMEMO_SLACKCHANNEL, "general" );
            }
            set {
                EditorPrefs.SetString( KEY_EDITORMEMO_SLACKCHANNEL, value );
            }
        }


    }

}