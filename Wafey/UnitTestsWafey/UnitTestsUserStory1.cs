using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wafey;

namespace UnitTestsWafey
{
    [TestClass]
    public class UnitTestsUserStory1
    {
        MusicPlayer musicplayer = new MusicPlayer();
        private string userName;

        [TestMethod]
        public void TestMethod1()
        {
            //Detects if music is playing.
            musicplayer.DownloadMp3FromUrl("http://145.44.233.180/Music/Thomas_The_Tank_Engine_Theme_Song.mp3");
            this.userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string[] echteUserName = userName.Split('\\');
            this.userName = echteUserName[echteUserName.Length - 1];
            musicplayer.open($@"C:\Users\{this.userName}\Downloads\Song.mp3");
            musicplayer.play(); 

            Thread.Sleep(5000);

            Assert.AreNotEqual(0, musicplayer.getTime());
        }

        [TestMethod]
        public void TestMethod2()
        {
            //Tests if the pause function works
            musicplayer.DownloadMp3FromUrl("http://145.44.233.180/Music/Thomas_The_Tank_Engine_Theme_Song.mp3");
            this.userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            string[] echteUserName = userName.Split('\\');
            this.userName = echteUserName[echteUserName.Length - 1];
            musicplayer.open($@"C:\Users\{this.userName}\Downloads\Song.mp3");
            musicplayer.play();

            Thread.Sleep(5000);

            musicplayer.pause();

            int pauzed_time = musicplayer.getTime();

            musicplayer.play();

            Thread.Sleep(5000);

            Assert.AreNotEqual(pauzed_time, musicplayer.getTime()); 
        }


        //[TestMethod]
        //public void Testmethode3()
        //{

        //}
    }
}
