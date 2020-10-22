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

        public MainForm()
        {
            InitializeComponent();

            server = new Server();
        }

        private void createRoomButton_Click(object sender, EventArgs e)
        {
            UserInfo userInfo = server.ConnectToServer("Oleg");
            RoomInfo roomInfo = server.CreateRoom(userInfo.Id);
            codeLabel.Text = roomInfo.Code;
            //roomInfo
        }

        private void connectButton_Click(object sender, EventArgs e)
        {
            UserInfo userInfo = server.ConnectToServer("Debil");
            RoomInfo roomInfo = server.ConnectToRoom(codeLabel.Text, userInfo.Id);
            if (roomInfo == null)
            {
                MessageBox.Show("Can not connect");
                return;
            }
            if (server.ChangeCharacter(roomInfo.Code, userInfo.Id, CharacterName.Pandora))
            {
                ChangeCharacter();
            }
            //roomInfo
        }

        private void ChangeCharacter() { }

        private void serverTimer_Tick(object sender, EventArgs e)
        {
            server.Update();
        }
    }
}
