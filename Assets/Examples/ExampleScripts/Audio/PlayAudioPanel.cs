﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cosmos;
using Cosmos.Audio;
using UnityEngine.UI;
public class PlayAudioPanel : MonoBehaviour
{
    Button btnPlay;
    Button btnPause;
    Button btnUnPause;
    Button btnStop;
    void Awake()
    {
        btnPlay = transform.Find("Play").GetComponent<Button>();
        btnPause= transform.Find("Pause").GetComponent<Button>();
        btnUnPause= transform.Find("Unpause").GetComponent<Button>();
        btnStop= transform.Find("Stop").GetComponent<Button>();

        btnPlay.onClick.AddListener(PlayAudio);
        btnPause.onClick.AddListener(PauseAudio);
        btnStop.onClick.AddListener(StopAudio);
        btnUnPause.onClick.AddListener(UnpauseAudio);
    }
    void Start()
    {
        CosmosEntry.ResourceManager.AddOrUpdateBuildInLoadHelper(Cosmos.Resource.ResourceLoadMode.Resource, new QuarkLoader());
        CosmosEntry.AudioManager.AudioRegisterSuccess += AudioRegisterSuccess;
        CosmosEntry.AudioManager.AudioRegistFailure += AudioRegistFailure; ;
        var audioAssetInfo = new AudioAssetInfo("AudioTechHouse", "AudioTechHouse");
        CosmosEntry.AudioManager.RegistAudio(audioAssetInfo);
    }
    void AudioRegisterSuccess(AudioRegistSuccessEventArgs eventArgs)
    {
        Utility.Debug.LogInfo($" {eventArgs.AudioName} Register success", MessageColor.GREEN);
    }
    void AudioRegistFailure(AudioRegistFailureEventArgs eventArgs)
    {
        Utility.Debug.LogError($" {eventArgs.AudioName} Register Failure");
    }
    void PlayAudio()
    {
        var ap = AudioParams.Default;
        ap.Loop = true;
        CosmosEntry.AudioManager.PalyAudio("AudioTechHouse", ap, AudioPlayInfo.Default);
        Utility.Debug.LogInfo("PlayAudio");
    }
    void PauseAudio()
    {
        CosmosEntry.AudioManager.PauseAudio("AudioTechHouse");
        Utility.Debug.LogInfo("PuaseAudio");
    }
    void UnpauseAudio()
    {
        CosmosEntry.AudioManager.UnPauseAudio("AudioTechHouse");
        Utility.Debug.LogInfo("UnpauseAudio");
    }
    void StopAudio()
    {
        CosmosEntry.AudioManager.StopAudio("AudioTechHouse");
        Utility.Debug.LogInfo("StopAudio");
    }
}
