using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using charcolle.UnityEditorMemo;

public class UnityEditorMemoSaveData: ScriptableObject {

    [SerializeField]
    public List<UnityEditorMemoCategory> Category  = new List<UnityEditorMemoCategory>();
    public string[] LabelTag = new string[5];

    public void AddCategory( UnityEditorMemoCategory category ) {
        Category.Add( category );
        SortCategory();
    }

    public string[] CategoryList {
        get {
            return Category.Select( c => c.Name ).ToArray();
        }
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
