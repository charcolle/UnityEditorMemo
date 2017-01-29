using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class UnityMemoSaveClass: ScriptableObject {

    public string CategoryName;
    [SerializeField]public List<Charcolle.UnityEditorMemo.UnityMemoClass> UnityMemoList  = new List<Charcolle.UnityEditorMemo.UnityMemoClass>();

    public void Initialize( string category ) {
        CategoryName = category;
    }

    public void AddMemo( Charcolle.UnityEditorMemo.UnityMemoClass memo ) {
        UnityMemoList.Add( memo );
    }

}

namespace Charcolle.UnityEditorMemo {
    [System.Serializable]
    public class UnityMemoClass {

        public string Date;
        public string Memo;
        public UnityMemoType Type;
        public UnityMemoTexture Tex;
        [System.NonSerialized]public bool isFold;

        public UnityMemoClass( string date, string memo, int type, int tex ) {
            Date = date;
            Memo = memo;
            Type = ( UnityMemoType )type;
            Tex = ( UnityMemoTexture )tex;
        }

    }

    public enum UnityMemoTexture {
        NONE = 0,
        HAPPY = 1,
        ANGRY = 2,
        SAD = 3,
    }

    public enum UnityMemoType {
        NORMAL = 0,
        IMPORTANT = 1,
        QUESTION = 2,
    }
}