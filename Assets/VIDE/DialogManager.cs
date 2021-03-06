﻿using UnityEngine;
using System.Collections;
using VIDE_Data; //Access VD class to retrieve node data
using UnityEngine.UI;
using System;

public class DialogManager : Singleton<DialogManager>
{
    public Action CurentDialogBegin;
    public Action CurentDialogEnds;

    public GameObject container_NPC;
    public GameObject container_PLAYER;
    public Text text_NPC;
    public Image characterImage;
    public Text characterName;
    public Button pushDialogNext;
    public Text[] text_Choices;
    public AudioSource audioSource;
    public Image stopClicking;
    public GameObject nameBack;


    // Use this for initialization
    void Start()
    {
        VD.LoadDialogues();
        container_NPC.SetActive(false);
        container_PLAYER.SetActive(false);
        pushDialogNext.gameObject.SetActive(false);
    }
   
   
    public void Begin(string dialog, Character character, bool isCharAlive, AudioClip audioClip)
    {
        characterImage.sprite = character.DialogSprite;
        if(character.Name == "")
        {
            nameBack.SetActive(false);
        }
        else
        {
            nameBack.SetActive(true);
        }
        CurentDialogBegin?.Invoke();
        characterImage.color = isCharAlive ? Color.white : Color.black;
        characterName.text = character.Name;
        pushDialogNext.gameObject.SetActive(true);
        stopClicking.gameObject.SetActive(true);
        VD.OnNodeChange += UpdateUI;
        VD.OnEnd += End;
        VD.BeginDialogue(dialog);

        if (audioClip != null && isCharAlive)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }

    

    void UpdateUI(VD.NodeData data)
    {
        container_PLAYER.SetActive(false);
        if (data.isPlayer)
        {
            container_PLAYER.SetActive(true);
            pushDialogNext.gameObject.SetActive(false);
            for (int i = 0; i < text_Choices.Length; i++)
            {
                if (i < data.comments.Length)
                {
                    text_Choices[i].transform.parent.gameObject.SetActive(true);
                    text_Choices[i].text = data.comments[i];
                }
                else
                {
                    text_Choices[i].transform.parent.gameObject.SetActive(false);
                }
            }
            text_Choices[0].transform.parent.GetComponent<Button>().Select();
        }
        else
        {
            pushDialogNext.gameObject.SetActive(true);
            container_NPC.SetActive(true);
            text_NPC.text = data.comments[data.commentIndex];
            if (VD.GetNext(true, false).isPlayer)
            {
                VD.Next();
            }
        }
    }

    void End(VD.NodeData data)
    {
        container_NPC.SetActive(false);
        CurentDialogEnds?.Invoke();
        container_PLAYER.SetActive(false);
        pushDialogNext.gameObject.SetActive(false);
        stopClicking.gameObject.SetActive(false);
        GameTimeHolder.Instance.IncreaseTime(GameTimeHolder.Instance.DialogTimeCost);
        VD.OnNodeChange -= UpdateUI;
        VD.OnEnd -= End;
        VD.EndDialogue();
    }

    void OnDisable()
    {
        if (container_NPC != null)
            End(null);
    }

    public void SetPlayerChoice(int choice)
    {
       VD.nodeData.commentIndex = choice;
       if (Input.GetMouseButtonUp(0))
            VD.Next();
    }
    public void pushDialog()
    {
        VD.Next();
    }
}
