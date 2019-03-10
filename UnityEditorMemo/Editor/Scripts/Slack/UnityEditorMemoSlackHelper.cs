using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;

using GUIHelper = charcolle.UnityEditorMemo.GUIHelper;

namespace charcolle.UnityEditorMemo {

    internal static class SlackHelper {

        private const string APIURL = @"https://slack.com/api/chat.postMessage?token={0}&channel={1}&text={2}&attachments=[{3},{4}]";
        private readonly static string[] FaceEmoji = new string[] { "", ":slightly_smiling_face:", ":rage:", ":sweat:" };

        public static bool Post( UnityEditorMemo memo, string categoryName ) {
            var token = UnityEditorMemoPrefs.UnityEditorMemoSlackAccessToken;
            var channel = UnityEditorMemoPrefs.UnityEditorMemoSlackChannel;
            if( string.IsNullOrEmpty( token ) ) {
                Debug.LogWarning( "UnityEditorMemo: You must set up your access token." );
                return false;
            }
            if( string.IsNullOrEmpty( channel ) ) {
                Debug.LogWarning( "UnityEditorMemo: You must set up your access token." );
                return false;
            }

            var text = memo.Memo;
            if( !string.IsNullOrEmpty( memo.URL ) )
                text += string.Format( "\n<{0}|URL>", memo.URL );

            var titleAttachment = new Attachment {
                title = string.Format( "【{0}】 - {1} {2}\n", PlayerSettings.productName, categoryName, FaceEmoji[ ( int )memo.Tex ] ),
                color = "FFFFFF",
            };
            var memoAttachment = new Attachment {
                color  = ColorUtility.ToHtmlStringRGB( GUIHelper.Colors.LabelColor( memo.Label ) ),
                text   = text,
                footer = memo.Date,
            };

            var url = string.Format( APIURL, token, channel, "", JsonUtility.ToJson( titleAttachment ), JsonUtility.ToJson( memoAttachment ) );
            var post = postCo( url );
            while( post.MoveNext() ) { }
            return ( bool )post.Current;
        }

        private static IEnumerator postCo( string url ) {
            var req = UnityWebRequest.Get( url );
#if UNITY_2017_2_OR_NEWER
            yield return req.SendWebRequest();
#else
            yield return req.Send();
#endif

            //Debug.Log( url );
            if( !string.IsNullOrEmpty( req.error ) ) {
                EditorUtility.DisplayDialog( "UnityEditorMemo", "fail to post memo to Slack.\n" + req.error, "ok" );
                yield return false;
            } else {
                yield return true;
            }

        }

        [Serializable]
        public class Attachment {
            public string color;
            public string title;
            public string text;
            public string footer;
        }

    }

}