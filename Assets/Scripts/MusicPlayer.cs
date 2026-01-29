using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;
    private AudioSource audioSource;
 
    public List<AudioClip> playlist; // lisää inspectorissa kappaleet järjestyksessä
    public bool loopPlaylist = true;  // haluatko soittaa uudelleen alusta lopussa

    private int currentTrackIndex = 0;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (playlist.Count > 0)
        {
            PlayTrack(currentTrackIndex);
        }
    }

    void Update()
    {
        if (!audioSource.isPlaying && playlist.Count > 0)
        {
            NextTrack();
        }
    }

    private void PlayTrack(int index)
    {
        audioSource.clip = playlist[index];
        audioSource.Play();
    }

    private void NextTrack()
    {
        currentTrackIndex++;

        if (currentTrackIndex >= playlist.Count)
        {
            if (loopPlaylist)
                currentTrackIndex = 0;
            else
                return; // lopeta soittaminen
        }

        PlayTrack(currentTrackIndex);
    }

    // Voit myös vaihtaa playlistin kesken kaiken
    public void SetPlaylist(List<AudioClip> newPlaylist, bool loop = true)
    {
        playlist = newPlaylist;
        loopPlaylist = loop;
        currentTrackIndex = 0;
        PlayTrack(currentTrackIndex);
    }
}
