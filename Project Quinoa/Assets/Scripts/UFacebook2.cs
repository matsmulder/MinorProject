//using Facebook;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Threading;
//using UnityEngine;

//public class UFacebook2 : MonoBehaviour
//{
//    private static readonly string _facebookAppId = "346314538774437";
//    private static readonly string _redirectEndpoint = "http://localhost:21165/Facebook";
//    private static readonly short _checkInterval = 1000;

//    public void Start()
//    {
//        this.Login();
//    }

//    public void Login()
//    {
//        var token = Guid.NewGuid().ToString();
//        var fbName = GameObject.Find("Facebook Name").GetComponent<TextMesh>();
//        var fbPicture = GameObject.Find("Facebook Picture").GetComponent<MeshRenderer>();

//        Application.OpenURL(String.Format("https://graph.facebook.com/oauth/authorize?client_id={0}&redirect_uri={1}&state={2}", _facebookAppId, _redirectEndpoint, token));

//        base.StartCoroutine(this.CheckForToken(token, access_token =>
//        {
//            var fbClient = new FacebookClient(access_token);
//            var me = (JsonObject)fbClient.Get("me");

//            fbName.text = "Hello\n" + me["first_name"].ToString() + "\n" + me["last_name"].ToString();

//            var profilePicture = new WWW(String.Format("http://graph.facebook.com/{0}/picture?width=100&height=100", me["id"].ToString()));
//            var tex = new Texture2D(100, 100, TextureFormat.DXT1, false);

//            while (!profilePicture.isDone) { };

//            profilePicture.LoadImageIntoTexture(tex);
//            fbPicture.material.mainTexture = tex;
//        }));
//    }

//    private IEnumerator CheckForToken(string token, Action<string> result)
//    {
//        while (true)
//        {
//            var downloader = new WWW(_redirectEndpoint + "?token=" + token);

//            yield return downloader;

//            var json = SimpleJson.DeserializeObject<Dictionary<string, object>>(downloader.text);
            
//            if (json.ContainsKey("access_token") && !String.IsNullOrEmpty(json["access_token"].ToString()))
//            {
//                result(json["access_token"].ToString());

//                yield break;
//            }
//            else
//            {
//                yield return new WaitForSeconds(_checkInterval / 1000);

//                this.CheckForToken(token, result);
//            }
//        }
//    }
//}