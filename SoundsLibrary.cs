using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tivuco
{
    static class SoundsLibrary
    {
        public static SoundEffect menuSong;
        public static SoundEffect matchLittleYellowSong;
        public static SoundEffect victoryScreenSong;

        static SoundEffectInstance lastSong;

        public static SoundEffect moveThroughtButtonsSound;

        public static SoundEffect dwarfDrinkingSound;
        public static SoundEffect dwarfSpittingFireSound;
        public static SoundEffect dwarfSmokingSound;
        public static SoundEffect dwarfThrowingBoozeSound;

        public static SoundEffect doveEatingSound;
        public static SoundEffect dovePooingSound;
        public static SoundEffect doveThrowingForkSound;

        public static SoundEffect skinnySpaceshipSound;
        public static SoundEffect skinnyTeleportSound;
        public static SoundEffect skinnyThrowingCrazyRaySound;

        public static SoundEffect goatReleasingDisillusionRaysSound;
        public static SoundEffect goatConfiguringRouteSound;
        public static SoundEffect goatThrowingDisconnectionSound;

        public static void LoadSoundLibrary(ContentManager content)
        {
            menuSong = content.Load<SoundEffect>("Songs/TroopsComingWithTheirBugs");
            matchLittleYellowSong = content.Load<SoundEffect>("Songs/JohnnyChapaQuente");
            victoryScreenSong = content.Load<SoundEffect>("Songs/HollyCave");

            //moveThroughtButtonsSound = content.Load<SoundEffect>("Sounds/Blip");

            //dwarfDrinkingSound = content.Load<SoundEffect>("Sounds/Gulp");
            //dwarfSpittingFireSound = content.Load<SoundEffect>("Sounds/Spit");
            //dwarfSmokingSound = content.Load<SoundEffect>("Sounds/Smoking");
            //dwarfThrowingBoozeSound = content.Load<SoundEffect>("Sounds/OldMan");

            //doveEatingSound = content.Load<SoundEffect>("Sounds/Delicious");
            //dovePooingSound = content.Load<SoundEffect>("Sounds/Poo");
            //doveThrowingForkSound = content.Load<SoundEffect>("Sounds/BlaBlaBla");

            //skinnySpaceshipSound = content.Load<SoundEffect>("Sounds/XFile");
            //skinnyTeleportSound = content.Load<SoundEffect>("Sounds/Yah");
            //skinnyThrowingCrazyRaySound = content.Load<SoundEffect>("Sounds/Waan");

            //goatReleasingDisillusionRaysSound = content.Load<SoundEffect>("Sounds/Halp");
            //goatConfiguringRouteSound = content.Load<SoundEffect>("Sounds/OldModem");
            //goatThrowingDisconnectionSound = content.Load<SoundEffect>("Sounds/FuckYou");
        }

        //public static void PlaySong(Song song)
        //{
        //    if (!Game1.configurations.muteSounds)
        //    {
        //        try
        //        {
        //            MediaPlayer.Stop();
        //            MediaPlayer.Play(song);
        //            MediaPlayer.IsRepeating = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            string hm = ex.Message;
        //        }
        //    }
        //}

        public static void PlaySongs(SoundEffect song)
        {
            if (!Game1.configurations.muteSounds)
            {
                if (lastSong != null)
                    lastSong.Stop();

                var currentSong = song.CreateInstance();
                currentSong.IsLooped = true;
                currentSong.Play();
                currentSong.Volume = Game1.configurations.soundsVolume;
                lastSong = currentSong;
            }
        }

        public static void PlaySound(SoundEffect sound)
        {
            if (!Game1.configurations.muteSounds)
            {
                sound.Play(Game1.configurations.soundsVolume, 0f, 0f);
            }
        }
    }
}
