using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class AudioMenu : MonoBehaviour
{
    [SerializeField] private bool isMainMenu = true;

    private EventInstance hoverSound;
    private EventInstance clickSound;
    private EventInstance menuMusic;

    private const string HOVERSOUND_PATH = "event:/ui/ui_hover";
    private const string CLICK_PATH = "event:/ui/ui_click";
    private const string MUSIC_PATH = "event:/music/menu/music_menu";
    private const string MASTER_SFX_PATH = "bus:/Master/SFX";

    private void Start()
    {
        hoverSound = RuntimeManager.CreateInstance(HOVERSOUND_PATH);
        clickSound = RuntimeManager.CreateInstance(CLICK_PATH);
        menuMusic = RuntimeManager.CreateInstance(MUSIC_PATH);

        if (isMainMenu)
        {
            stopAllEvents();
            menuMusic.start();
        }
    }

    public void playHoverSound()
    {
        hoverSound.start();
    }

    public void playClickSound()
    {
        clickSound.start();
    }

    public void stopMusic()
    {
        menuMusic.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void stopAllEvents()
    {
        Bus SFX = RuntimeManager.GetBus(MASTER_SFX_PATH);
        SFX.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
