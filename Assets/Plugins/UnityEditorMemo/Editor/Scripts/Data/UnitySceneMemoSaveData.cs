using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using charcolle.UnityEditorMemo;

public class UnitySceneMemoSaveData : ScriptableObject {

    [SerializeField]
    public List<UnitySceneMemoScene> Scene  = new List<UnitySceneMemoScene>();

    //======================================================================
    // public
    //======================================================================

    public void AddSceneMemo( GameObject obj, int localIdentifierInFile ) {
        var guid = AssetDatabase.AssetPathToGUID( obj.scene.path );
        if ( string.IsNullOrEmpty( guid ) ) {
            Debug.LogWarning( "Please save current scene before add memo." );
            return;
        }
        var scene = getSceneFromGuid( guid );
        scene.AddMemo( obj, localIdentifierInFile );
    }

    public UnitySceneMemo GetSceneMemo( GameObject obj, int localIdentifierInFile ) {
        var guid = AssetDatabase.AssetPathToGUID( obj.scene.path );
        if ( string.IsNullOrEmpty( guid ) )
            return null;

        var scene = GetScene( guid );
        if ( scene == null )
            return null;
        return scene.GetMemo( obj );
    }

    public UnitySceneMemo GetSceneMemo( string scenePath, int localIdentifierInFile ) {
        var guid = AssetDatabase.AssetPathToGUID( scenePath );
        if ( string.IsNullOrEmpty( guid ) )
            return null;

        var scene = GetScene( guid );
        if ( scene == null )
            return null;
        return scene.GetMemo( localIdentifierInFile );
    }

    public UnitySceneMemoScene GetScene( string guid ) {
        return Scene.FirstOrDefault( m => m == guid );
    }

    //======================================================================
    // private
    //======================================================================

    private UnitySceneMemoScene getSceneFromGuid( string guid ) {
        var scene = GetScene( guid );
        if ( scene != null )
            return scene;

        return addSceneData( guid );
    }

    private UnitySceneMemoScene addSceneData( string guid ) {
        var sceneData = new UnitySceneMemoScene( guid );
        Scene.Add( sceneData );
        return sceneData;
    }

}
