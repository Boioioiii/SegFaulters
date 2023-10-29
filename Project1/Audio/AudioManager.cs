﻿using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using static Project1.Constants;

namespace Project1.Audio
{
    public class AudioManager
    {
        //BackGround Music
        //private static Song BGM;

        // Sound Effects
        //private static SoundEffect addUI, boomerang, cling, death, zeldaSound, dragon, dragon2, dragon3, enemyDie, fire, flame;


        public static void LoadContent(ContentManager content)
        {
            BGM = content.Load<Song>("Audio\\04 Underworld BGM");

            addUI = content.Load<SoundEffect>("Audio\\add ui");
            boomerang = content.Load<SoundEffect>("Audio\\boomerang");
            death = content.Load<SoundEffect>("Audio\\death");
            dragon = content.Load<SoundEffect>("Audio\\dragon");
            dragon2 = content.Load<SoundEffect>("Audio\\dragon2");
            dragon3 = content.Load<SoundEffect>("Audio\\dragon3");
            enemyDie = content.Load<SoundEffect>("Audio\\enemy die");
            fire = content.Load<SoundEffect>("Audio\\fire");
            flame = content.Load<SoundEffect>("Audio\\flame");
            item = content.Load<SoundEffect>("Audio\\item");
            lowHealth = content.Load<SoundEffect>("Audio\\low health");
            plus = content.Load<SoundEffect>("Audio\\plus");
            plusPlus = content.Load<SoundEffect>("Audio\\plus plus");
            secret = content.Load<SoundEffect>("Audio\\secret revealed");
            stairs = content.Load<SoundEffect>("Audio\\stairs");
            subtractUI = content.Load<SoundEffect>("Audio\\subtract ui");
            sword = content.Load<SoundEffect>("Audio\\sword");

            zeldaSound = content.Load<SoundEffect>("Audio\\zelda sound");
            //  Uncomment the following line will also loop the song
            //  MediaPlayer.IsRepeating = true;
            //MediaPlayer.MediaStateChanged += MediaPlayer_MediaStateChanged;
        }

        public static void PlayMusic(Song music)
        {
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(music);
        }
        public static void StopAllAudio()
        {
            MediaPlayer.Stop();
        }

        public static void ResumeMusic() 
        {         
            MediaPlayer.Resume();
        }

        public static void PlaySoundEffect(SoundEffect SFX)
        {
            SFX.Play();
        }      
    }
}
