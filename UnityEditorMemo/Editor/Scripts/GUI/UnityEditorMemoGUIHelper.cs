using UnityEngine;
using UnityEditor;

using FileUtility  = charcolle.UnityEditorMemo.FileUtility;
using WindowHelper = charcolle.UnityEditorMemo.UnityEditorMemoWindowHelper;

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

    public enum UnitySceneMemoTextColor {
        BLACK = 0,
        WHITE = 1,
    }

    internal static class GUIHelper {

        public static string[] LabelMenu     = { "Normal", "Red", "Green", "Cyan", "Yellow", "Magenta" };
        public static string[] TextColorMenu = { "Black", "White" };

        //======================================================================
        // GUIStyle
        //======================================================================

        public static class Styles {

            static Styles() {
                NoSpaceBox = new GUIStyle( GUI.skin.box ) {
                    margin = new RectOffset( 0, 0, 0, 0 ),
                    padding = new RectOffset( 1, 1, 1, 1 )
                };

                MemoHeader = new GUIStyle( "RL Header" ) {
                    margin = new RectOffset( 0, 0, 0, 0 ),
                    padding = new RectOffset( 0, 0, 0, 0 ),
                };

                MemoBack = new GUIStyle( "RL Background" ) {
                    margin = new RectOffset( 0, 0, 0, 0 ),
                    padding = new RectOffset( 1, 1, 1, 1 ),
                    alignment = TextAnchor.MiddleCenter,
                    stretchHeight = false,
                    stretchWidth = false
                };

                MemoLabel = new GUIStyle( GUI.skin.label ) {
                    wordWrap = true,
                    richText = true,
                    fontSize = UnityEditorMemoPrefs.UnityEditorMemoFontSize,
                };

                LabelWordWrap = new GUIStyle( GUI.skin.label ) {
                    wordWrap = true,
                    richText = true,
                };

                TextAreaWordWrap = new GUIStyle( GUI.skin.textArea ) {
                    wordWrap = true
                };

                TextFieldWordWrap = new GUIStyle( GUI.skin.textField ) {
                    wordWrap = true
                };

                GUIStyleState state       = new GUIStyleState();
                state                     = GUI.skin.box.normal;
                state.textColor           = EditorGUIUtility.isProSkin ? new Color( 1f, 1f, 1f, 0.7f ) : Color.black;
                MemoBox = new GUIStyle( GUI.skin.box ) {
                    alignment = TextAnchor.MiddleCenter,
                    normal = state
                };

                SearchField       = new GUIStyle( "ToolbarSeachTextField" );

                SearchFieldCancel = new GUIStyle( "ToolbarSeachCancelButton" );

                LargeButtonLeft   = new GUIStyle( "LargeButtonLeft" );

                LargeButtonMid    = new GUIStyle( "LargeButtonMid" );

                LargeButtonRight  = new GUIStyle( "LargeButtonRight" );
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

            public static GUIStyle MemoLabel {
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
            public static GUIStyle TextFieldWordWrap {
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

            public static GUIStyle SearchField {
                get;
                private set;
            }

            public static GUIStyle SearchFieldCancel {
                get;
                private set;
            }

            public static GUIStyle LargeButtonLeft {
                get;
                private set;
            }

            public static GUIStyle LargeButtonMid {
                get;
                private set;
            }

            public static GUIStyle LargeButtonRight {
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
                OpenLink = EditorGUIUtility.FindTexture( "winbtn_win_rest" );
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

            public static Texture2D OpenLink {
                get;
                private set;
            }

        }

        public static class Colors {

            static Colors() {
                TransparentColor  = EditorGUIUtility.isProSkin ? Color.white : new Color( 0.6f, 0.6f, 0.6f, 0.5f );
                DefaultWhiteColor = new Color( 0.76f, 0.76f, 0.76f, 1f );
                DefaultBlackColor = Color.black;
                DefaultTextColor  = GUI.skin.label.normal.textColor;
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

            public static Color SceneMemoTextColor( UnitySceneMemoTextColor color ) {
                return color.Equals( UnitySceneMemoTextColor.BLACK ) ? DefaultBlackColor : DefaultWhiteColor;
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

            public static Color TransparentColor {
                get;
                private set;
            }

            public static Color DefaultTextColor {
                get;
                private set;
            }

            public static Color DefaultWhiteColor {
                get;
                private set;
            }

            public static Color DefaultBlackColor {
                get;
                private set;
            }

        }

    }

}