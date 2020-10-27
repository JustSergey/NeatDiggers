using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NeatDiggersPrototype
{
    public partial class MainForm : Form
    {
        Server server;
        UserInfo user1, user2;
        RoomPrepareInfo room;

        public MainForm()
        {
            InitializeComponent();

            server = new Server();
        }

        private void createRoomButton_Click(object sender, EventArgs e)
        {
            user1 = server.ConnectToServer("Oleg");
            room = server.CreateRoom(user1.Id);
            codeLabel.Text = room.Code;
            server.SetReady(room.Code, user1.Id);
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            user2 = server.ConnectToServer("Debil");
            RoomPrepareInfo roomInfo = server.ConnectToRoom(codeLabel.Text, user2.Id);
            if (roomInfo == null)
            {
                MessageBox.Show("Can not connect");
                return;
            }
            if (server.ChangeCharacter(roomInfo.Code, user2.Id, CharacterName.Pandora))
                ChangeCharacter();
            server.SetReady(roomInfo.Code, user2.Id);
        }

        private void ChangeCharacter() { }

        private void startGameButton_Click(object sender, EventArgs e)
        {
            server.StartTheGame(room.Code, user1.Id);
        }

        private void serverTimer_Tick(object sender, EventArgs e)
        {
            server.Update();
        }
    }
}
