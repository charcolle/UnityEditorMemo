using UnityEngine;
using UnityEditor;

using FileUtility = charcolle.UnityEditorMemo.FileUtility;

namespace charcolle.UnityEditorMemo {

    public enum UnityEditorMemoTexture {
        NONE = 0,
        HAPPY = 1,
        ANGRY = 2,
        SAD = 3,
    }

    public enum UnityEditorMemoLabel {
        NORMAL = 0,
        RED = 1,
        GREEN = 2,
        CYAN = 3,
        YELLOW = 4,
        MAGENTA = 5,
    }

    internal static class GUIHelper {

        public static string[] Label    = { "Normal", "Red", "Green", "Cyan", "Yellow", "Magenta" };

        //======================================================================
        // GUIStyle
        //======================================================================

        public static class Styles {

            static Styles() {
                NoSpaceBox                = new GUIStyle( GUI.skin.box );
                NoSpaceBox.margin         = new RectOffset( 0, 0, 0, 0 );
                NoSpaceBox.padding        = new RectOffset( 1, 1, 1, 1 );

                MemoHeader                = new GUIStyle( "RL Header" );
                MemoHeader.margin         = new RectOffset( 0, 0, 0, 0 );
                MemoHeader.padding        = new RectOffset( 0, 0, 0, 0 );
                MemoHeader.fixedHeight    = 0f;

                MemoBack                  = new GUIStyle( "RL Background" );
                MemoBack.margin           = new RectOffset( 0, 0, 0, 0 );
                MemoBack.padding          = new RectOffset( 1, 1, 1, 1 );
                MemoBack.alignment        = TextAnchor.MiddleCenter;
                MemoBack.stretchHeight    = false;
                MemoBack.stretchWidth     = false;

                LabelWordWrap             = new GUIStyle( GUI.skin.label );
                LabelWordWrap.wordWrap    = true;
                LabelWordWrap.richText    = true;

                TextAreaWordWrap          = new GUIStyle( GUI.skin.textArea );
                TextAreaWordWrap.wordWrap = true;

                GUIStyleState state       = new GUIStyleState();
                state                     = GUI.skin.box.normal;
                state.textColor           = EditorGUIUtility.isProSkin ? new Color( 1f, 1f, 1f, 0.7f ) : Color.black;
                MemoBox                   = new GUIStyle( GUI.skin.box );
                MemoBox.alignment         = TextAnchor.MiddleCenter;
                MemoBox.normal            = state;
            }

            public static GUIStyle NoSpaceBox {
                get;
                private set;
            }

            public static GUIStyle MemoHeader {
                get;
                private set;
            }

            public static GUIStyle MemoBack {
                get;
                private set;
            }

            public static GUIStyle LabelWordWrap {
                get;
                private set;
            }

            public static GUIStyle TextAreaWordWrap {
                get;
                private set;
            }

            public static GUIStyle MemoBox {
                get;
                private set;
            }

            public static GUIStyle NoOverflow {
                get;
                private set;
            }


        }

        //======================================================================
        // Texture
        //======================================================================

        public static class Textures {

            public static Texture2D[] Emotions;
            public static Texture2D[] Menu;
            public static Texture2D[] Footer;

            static Textures() {
                var guiDir = FileUtility.GUIDirectoryPath;

                Happy    = AssetDatabase.LoadAssetAtPath<Texture2D>( guiDir + "Texture/unityeditormemo_smile.png" );
                Sad      = AssetDatabase.LoadAssetAtPath<Texture2D>( guiDir + "Texture/unityeditormemo_sad.png" );
                Angry    = AssetDatabase.LoadAssetAtPath<Texture2D>( guiDir + "Texture/unityeditormemo_angry.png" );
                Home     = AssetDatabase.LoadAssetAtPath<Texture2D>( guiDir + "Texture/unityeditormemo_home.png" );
                Plus     = AssetDatabase.LoadAssetAtPath<Texture2D>( guiDir + "Texture/unityeditormemo_add.png" );
                Balloon  = AssetDatabase.LoadAssetAtPath<Texture2D>( guiDir + "Texture/unityeditormemo_balloon.png" );
                Emotions = new Texture2D[] { null, Happy, Angry, Sad };
                Menu     = new Texture2D[] { Home, Plus };

                Footer   = new Texture2D[] { null, null, null, null, null, null };
            }

            public static Texture2D Happy {
                get;
                private set;
            }

            public static Texture2D Sad {
                get;
                private set;
            }

            public static Texture2D Angry {
                get;
                private set;
            }

            public static Texture2D Home {
                get;
                private set;
            }

            public static Texture2D Plus {
                get;
                private set;
            }

            public static Texture2D Balloon {
                get;
                private set;
            }

        }

        public static class Colors {

            public static Color TransparentColor;

            static Colors() {
                TransparentColor = EditorGUIUtility.isProSkin ? Color.white : new Color( 0.6f, 0.6f, 0.6f, 0.5f );
            }

            public static Color LabelColor( int type ) {
                return getColor( type );
            }
            public static Color LabelColor( UnityEditorMemoLabel label ) {
                return getColor( ( int )label );
            }

            public static Color SceneMemoLabelColor( UnityEditorMemoLabel label ) {
                return getColor( ( int )label ) - new Color( 0f, 0f, 0f, 0.5f );
            }

            private static Color getColor( int type ) {
                Color col;
                switch ( type ) {
                    case 0:
                        return new Color( 1f, 1f, 1f );
                    case 1:
                        col = new Color( 1f, 0.35f, 0.35f );
                        break;
                    case 2:
                        col = new Color( 0.4f, 1f, 0.4f );
                        break;
                    case 3:
                        col = Color.cyan;
                        break;
                    case 4:
                        col = Color.yellow;
                        break;
                    case 5:
                        col = Color.magenta;
                        break;
                    default:
                        return Color.gray;
                }
                return col + ( EditorGUIUtility.isProSkin ? Color.clear : new Color( 0.1f, 0.1f, 0.1f, 0f ) );
            }

        }

    }

}