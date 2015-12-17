using UnityEngine;
using System.Collections;

public class FacebookManager : MonoBehaviour {

    private static string FACEBOOK_URL = "http://www.facebook.com/dialog/feed";
    private static string FACEBOOK_APP_ID = "472366919613458";

    // Use this for initialization
    void Start () {
        DontDestroyOnLoad(this);
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    static void ShareToFacebook(string linkParameter, string nameParameter, string captionParameter, string descriptionParameter, string pictureParameter, string redirectParameter)
    {
        /* PARAMETERS:
        * link: Link behind post (when someone clicks on the post). Tip: https://drproject.twi.tudelft.nl/ewi3620tu1/Index.html
        * name: Name of the post
        * caption: text below the name, should be descriptive
        * description: footer text, should be short
        * picture: link to picture
        * redirect: link to send user to after posting. Just use https://www.facebook.com/ in general
        */
        Application.OpenURL(FACEBOOK_URL + "?app_id=" + FACEBOOK_APP_ID +
        "&link=" + WWW.EscapeURL(linkParameter) +
        "&name=" + WWW.EscapeURL(nameParameter) +
        "&caption=" + WWW.EscapeURL(captionParameter) +
        "&description=" + WWW.EscapeURL(descriptionParameter) +
        "&picture=" + WWW.EscapeURL(pictureParameter) +
        "&redirect_uri=" + WWW.EscapeURL(redirectParameter));
    }



}
