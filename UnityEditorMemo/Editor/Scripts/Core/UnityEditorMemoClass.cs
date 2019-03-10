using System;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    internal class UnityEditorMemo : TreeElement {

        public string Date;
        public string Memo;
        public string URL;
        public UnityEditorMemoLabel Label;
        public UnityEditorMemoTexture Tex;
        public UnityEditorMemoObject ObjectRef;
        public bool IsEdit;

        public UnityEditorMemo( string memo, int type, int tex, string url ) {
            Date        = DateTime.Now.RenderDate();
            Memo        = memo;
            Label       = ( UnityEditorMemoLabel )type;
            Tex         = ( UnityEditorMemoTexture )tex;
            ObjectRef   = new UnityEditorMemoObject(null);
            URL         = url;
        }

        public void Initialize( int id ) {
            this.id = id;
            this.name = Memo;
            IsEdit = false;
            ObjectRef.Initialize();
        }

        public static UnityEditorMemo Root {
            get {
                var root = new UnityEditorMemo( "", 0, 0, "" ) {
                    id    = -1,
                    depth = -1,
                    name  = "root"
                };
                return root;
            }
        }

    }

}