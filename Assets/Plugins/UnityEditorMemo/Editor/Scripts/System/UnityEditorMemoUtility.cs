using System;

namespace charcolle.UnityEditorMemo {

    internal static class UnityEditorMemoUtility {

        public static string RenderDate( this DateTime date ) {
            return string.Format( "{0}/{1}/{2} {3}:{4}", date.Year, date.Month, date.Day, date.Hour, date.Minute );
        }

        public static string ToSuccess( this string str ) {
            return string.Format( "<color=green>{0}</color>", str );
        }

        public static string ToBold( this string str ) {
            return string.Format( "<b>{0}</b>", str );
        }

        public static string ToMiddleBold( this string str ) {
            return string.Format( "<size=15><b>{0}</b></size>", str );
        }

    }
}