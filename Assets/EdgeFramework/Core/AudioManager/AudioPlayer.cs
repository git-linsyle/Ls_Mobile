﻿
/****************************************************
	文件：AudioPlayer.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 16:54   	
	Features：
*****************************************************/

using UnityEngine;
using EdgeFramework.Audio;
using EdgeFramework.Sheet;

namespace EdgeFramework.Audio
{
    public class AudioPlayer :Singleton<AudioPlayer>
    {
        AudioPlayer() { }

        public void PlaySound(SoundEnum key)
        {
            SoundSheet sound = SheetManager.Instance.GetSoundSheet(key);
            string audioPath = sound.Path;
            AudioClip clip = AudioManager.Instance.LoadClip(audioPath, true);
            if (sound.Repeat == 0)
                AudioManager.Instance.PlayOneShot(clip);
            else
                AudioManager.Instance.RepeatSFX(clip, sound.Repeat, sound.Single);
        }

        public void PlayBGM(MusicEnum key)
        {
            MusicSheet sound = SheetManager.Instance.GetMusicSheet(key);
            AudioManager.Instance.PlayBGM(sound.Path, (MusicTransition)sound.MusTransition, sound.Duration, sound.Volume);
        }
        public void StopAllSFX()
        {
            AudioManager.Instance.StopAllSFX();
        }
        public void PauseBGM()
        {
            AudioManager.Instance.PauseBGM();
        }
        public void ResumeBGM()
        {
            AudioManager.Instance.ResumeBGM();
        }
    }
}
