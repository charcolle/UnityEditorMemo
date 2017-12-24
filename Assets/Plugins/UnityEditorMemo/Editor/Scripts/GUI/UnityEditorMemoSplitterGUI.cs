using System;
using UnityEditor;
using UnityEngine;

//
// from UNIBOOK5 by ando http://www.unity-bu.com/2016/06/unibook-5-info.html
//
namespace charcolle.UnityEditorMemo {
    public class UnityEditorMemoSplitterGUI {

        static Type splitterGUILayoutType;
        object splitterGUILayout;

        [InitializeOnLoadMethod]
        static void Init() {
            splitterGUILayoutType = Type.GetType( "UnityEditor.SplitterGUILayout,UnityEditor" );
        }

        public static void BeginVerticalSplit( UnityEditorMemoSplitterState state, GUIStyle style, params GUILayoutOption[] options ) {
            DoMethod( "BeginVerticalSplit", new Type[] {
            UnityEditorMemoSplitterState.splitterStateType,
            typeof(GUIStyle),
            typeof(GUILayoutOption[])
        }, null, new object[] { state.GetState(), style, options } );
        }

        public static void BeginVerticalSplit( UnityEditorMemoSplitterState state, params GUILayoutOption[] options ) {
            DoMethod( "BeginVerticalSplit", new Type[] {
            UnityEditorMemoSplitterState.splitterStateType,
            typeof(GUILayoutOption[])
        }, null, new object[] { state.GetState(), options } );
        }
        
        public static void EndVerticalSplit() {
            DoMethod( "EndVerticalSplit", null, new object[0] );
        }
        
        private static object DoMethod( string name, object obj, object[] args ) {
            return DoMethod( name, new Type[0], obj, args );
        }

        private static object DoMethod( string name, Type[] types, object obj, object[] args ) {
            if ( types.Length != 0 )
                return splitterGUILayoutType.GetMethod( name, types ).Invoke( obj, args );
            else
                return splitterGUILayoutType.GetMethod( name ).Invoke( obj, args );
        }
    }

    public class UnityEditorMemoSplitterState {
        public static Type splitterStateType {
            get {
                return Type.GetType( "UnityEditor.SplitterState,UnityEditor" );
            }
        }

        object splitterState;

        [InitializeOnLoadMethod]
        static void Init() {
        }

        public object GetState() {
            return splitterState;
        }

        public UnityEditorMemoSplitterState( float[] relativeSizes, int[] minSizes, int[] maxSizes, int splitSize ) {
            splitterState = Activator.CreateInstance( splitterStateType, new object[] {
            relativeSizes,
            minSizes,
            maxSizes,
            splitSize
        } );
        }

        public UnityEditorMemoSplitterState( float[] relativeSizes, int[] minSizes, int[] maxSizes ) {
            splitterState = Activator.CreateInstance( splitterStateType, new object[] {
            relativeSizes,
            minSizes,
            maxSizes
        } );
        }

        public UnityEditorMemoSplitterState( int[] realSizes, int[] minSizes, int[] maxSizes ) {
            splitterState = Activator.CreateInstance( splitterStateType, new object[] {
            minSizes,
            maxSizes
        } );
        }

        public UnityEditorMemoSplitterState( params float[] relativeSizes ) {
            splitterState = Activator.CreateInstance( splitterStateType, new object[] {
            relativeSizes
        } );

        }
    }
}