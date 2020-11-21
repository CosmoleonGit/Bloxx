using Microsoft.Xna.Framework;
using MonoExt;
using MonoNet;
using MonoUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bloxx.Source.Menus
{
    public class MainMenu : ControlGroup
    {
        public MainMenu()
        {
            Button serverBtn = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50, Main.screenHeight / 2 - 50),
                Size = new Point(100, 50),
                Text = "Host Game",
                Font = Main.MediumFont
            };

            serverBtn.ClickEvent = (object s) =>
            {
                Networking.SetupServer(1234);
                Main.main.screen = new GameScreen();
            };

            components.Add(serverBtn);


            Button clientBtn = new Button(this)
            {
                Position = new Point(Main.screenWidth / 2 - 50, Main.screenHeight / 2),
                Size = new Point(100, 50),
                Text = "Join Game",
                Font = Main.MediumFont
            };

            clientBtn.ClickEvent = (object s) =>
            {
                Networking.SetupClient("192.168.1.10", 1234);
                Main.main.screen = new GameScreen();
            };

            components.Add(clientBtn);
        }
    }
}
