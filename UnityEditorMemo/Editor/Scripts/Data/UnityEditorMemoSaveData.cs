using System.Collections.Generic;
using UnityEngine;
using charcolle.UnityEditorMemo;

internal class UnityEditorMemoSaveData: ScriptableObject {

    [SerializeField]
    public List<UnityEditorMemoCategory> Category  = new List<UnityEditorMemoCategory>();
    public string[] LabelTag = new string[5];

    public void AddCategory( UnityEditorMemoCategory category ) {
        Category.Add( category );
    }

    public void SortCategory() {
        Category.Sort( compareCategory );
    }

    private int compareCategory( UnityEditorMemoCategory a, UnityEditorMemoCategory b ) {
        if ( a.Name.Equals( "default" ) )
            return 1;
        if ( b.Name.Equals( "default" ) )
            return -1;
        return string.Compare( a.Name, b.Name );
    }

}
