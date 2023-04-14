using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wafey;




namespace UnitTestsWafey
{
    [TestClass]
    //Als gebruiker wil ik zonder in te loggen gebruik kunnen maken van wafey zodat ik zonder account ook de muziek kan beluisteren
    public class UnitTestUserStory5
    {

        
        DBHelper conncectie = new DBHelper();
        public User CurrentUser { get; private set; }
        

        [TestMethod]
        //knop die je inlogt met tijdelijk gastaccount en je doorverwijst naar het hoofdscherm
        public void TestMethod1()
        {
            Welcome welcome = new Welcome();
            List<User> db;
            welcome.Button_Click_1(this, new RoutedEventArgs(Button.ClickEvent));
            db = conncectie.getData<User>("http://145.44.233.180/selectuser.php?loginCheck&userLastName=" + welcome.Guestnr + "&userEmail=Guest" + welcome.Guestnr + "@wafey.com");
            Assert.AreNotEqual(db, 0);
        }

        [TestMethod]
        //gastaccount wordt verwijdert na de sessie
        public void TestMethod2()
        {
            //TODO 
            MainWindow mw = new MainWindow();
            Welcome welcome = new Welcome();
            List<User> db;
            welcome.Button_Click_1(this, new RoutedEventArgs(Button.ClickEvent));
            mw.Window_Closing(this, new System.ComponentModel.CancelEventArgs());
            db = conncectie.getData<User>("http://145.44.233.180/selectuser.php?loginCheck&userLastName=" + welcome.Guestnr + "&userEmail=Guest" + welcome.Guestnr + "@wafey.com");
            Assert.AreEqual(db, null);
        }
    }
}
