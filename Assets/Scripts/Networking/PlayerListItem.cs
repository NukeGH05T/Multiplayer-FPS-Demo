using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;
using System;

//Completed PlayerListItem's code
//Watch from here https://youtu.be/kld9sINMLGw?list=PLfFBezYu5hogMS3QeJkM1FQfl3s1sCzwV&t=661
public class PlayerListItem : MonoBehaviour
{
    public string playerName;
    public int connectionID;
    public ulong playerSteamID;
    private bool avatarReceived;

    public TMP_Text playerNameText;
    public TMP_Text playerReadyText;
    public RawImage playerIcon;
    public bool isReady;

    protected Callback<AvatarImageLoaded_t> ImageLoaded;

    private void Start() {
        ImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    public void ChangeReadyStatus() {
        if (isReady) {
            playerReadyText.text = "Ready";
            playerReadyText.color = Color.cyan;
        }
        else {
            playerReadyText.text = "Not Ready";
            playerReadyText.color = Color.red;
        }
    }

    public void SetPlayerValues() {
        playerNameText.text = playerName;
        ChangeReadyStatus();
        if (!avatarReceived) {
            GetPlayerIcon();
        }
    }

    private void GetPlayerIcon() {
        int imageID = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamID);
        if (imageID == -1) return;
        Debug.Log("Got ImgID");
        playerIcon.texture = GetSteamImageAsTexture(imageID);
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback) {
        if (callback.m_steamID.m_SteamID == playerSteamID) {
            playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        } else { //If it's another player
            return;
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        avatarReceived = true;
        return texture;
    }

}
