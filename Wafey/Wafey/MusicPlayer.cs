using NAudio.Wave;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using NAudio.Wave.SampleProviders;

namespace Wafey
{
    public class MusicPlayer
    {
        public string url;
        public static bool playing = true;
        public static bool paused = false;
        private static bool resume = false;
        private static bool seeking = false;
        private static long seekTime;
        public long currentTime = 0;
        public long length = 0;
        public static bool volumebool = false;
        public static double volumeamount;
        public string userName;
        public int totalSongs = 0;

        /// <summary>
        /// Plays the music when is song is selected
        /// </summary>
        public void play()
        {
            playing = true;
            resume = true;

        }

        /// <summary>
        /// Stops the music if there is music playing
        /// </summary>
        public void stop()
        {
            playing = false;
        }

        /// <summary>
        /// Pauses the music if music is playing
        /// </summary>
        public void pause()
        {
            paused = true;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startTime"></param>
        public void jumpTime(int startTime)
        {
            seekTime = startTime;
            seeking = true;
        }

        /// <summary>
        /// Stream music from a webserver over port 80.
        /// The function requires a URL to be able to stream thhe audio from a URL
        /// </summary>
        public void streamAudio()
        {
            using (var mf = new MediaFoundationReader(url))
            using (var wo = new WaveOut())
            {
                wo.Init(mf);
                wo.Play();
                var ABPS = mf.WaveFormat.AverageBytesPerSecond;
                length = mf.Length / ABPS;
                while (wo.PlaybackState == PlaybackState.Playing)
                {
                    if (paused)
                    {
                        wo.Pause();
                        paused = false;
                        while (wo.PlaybackState == PlaybackState.Paused)
                        {
                            if (resume)
                            {
                                wo.Play();
                                resume = false;
                            }
                        }
                        Thread.Sleep(50);
                    }
                    else if (seeking)
                    {
                        long seekTimeInBytes = seekTime * ABPS;
                        mf.Position = seekTimeInBytes;
                        seeking = false;
                    }
                    else if (volumebool)
                    {
                        wo.Volume = (float)volumeamount;
                        volumebool = false;
                    }
                    currentTime = mf.Position / ABPS;
                }
            }
        }

        /// <summary>Set volume 0 is low 1 is high</summary>
        public void SetVolume(double volume)
        {
            volumeamount = volume;
            volumebool = true;
        }
    }
}
