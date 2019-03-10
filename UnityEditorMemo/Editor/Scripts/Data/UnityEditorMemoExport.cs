using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace charcolle.UnityEditorMemo {

    [Serializable]
    internal class UnityEditorMemoExport {

        public UnityEditorMemoCategory[] category;

        public UnityEditorMemoExport( List<UnityEditorMemoCategory> c ) {
            category = c.ToArray();
        }

    }

}