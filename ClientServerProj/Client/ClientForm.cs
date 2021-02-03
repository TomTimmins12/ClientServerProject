using System;
using System.Windows.Forms;
using Packets;

namespace Client
{

    public partial class ClientForm : Form
    {
        private delegate void UpdateChatWindowDelegate(string message); // Creates the delegate
        private UpdateChatWindowDelegate updateChatWindowDelegate;

        private readonly Client m_client;

        internal ClientForm(Client client)
        {
            InitializeComponent();
            m_client = client;
        }


        public void UpdateChatWindow(string message)
        {
            if (UsernameInput.Enabled == false)
            {
                if (MessageWindow.InvokeRequired)
                {
                    Invoke(new Action(() =>
                    {
                        UpdateChatWindow(message);
                    }));
                }
                else
                {
                    MessageWindow.AppendText(message + Environment.NewLine);
                    MessageWindow.SelectionStart = MessageWindow.Text.Length;
                    MessageWindow.ScrollToCaret();
                    Console.WriteLine("UpdateChatWindow() called");

                }
            }
        }

        //public void UpdateClientList(string clientName)
        //{
        //    ClientList.Items.Add(clientName);
        //}


        //
        //Form event handler
        //
        private void ClientForm_Load(object sender, EventArgs e)
        {

            Console.WriteLine("ClientForm_Load() called" );
        }

        //
        // Text box update functions
        //
        private void InputField_TextChanged(object sender, EventArgs e)
        {

        }

        private void MessageWindow_TextChanged(object sender, EventArgs e)
        {

        }

        private void UsernameInput_TextChanged(object sender, EventArgs e)
        {

        }


        private void ClientList_SelectedIndexChanged(object sender, EventArgs e)
        {

            Console.WriteLine("User: " + m_client  + " added to list");
        }


        //
        //Button control
        //
        private void SubmitButton_Click(object sender, EventArgs e)
        {
            ChatMessagePacket chatPacket = new ChatMessagePacket(m_client.Name + ": " + InputField.Text + "\n", m_client.ID);
            m_client.SendMessage(chatPacket);
            //MessageWindow.AppendText(chatPacket.m_message+"\n");
            InputField.Clear();
            Console.WriteLine("Message " + chatPacket.m_message + " Submitted");
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (UsernameInput.Text.Length > 2)
            {
                if (ConnectButton.Text == "Connect")
                {
                    ConnectButton.Text = "Disconnect";
                    UsernameInput.Enabled = false;
                    ClientList.Enabled = true;
                    SubmitButton.Enabled = true;
                    InputField.Enabled = true;

                    ChatMessagePacket connectBroadcast = new ChatMessagePacket("User: " + UsernameInput.Text + " has connected", m_client.ID);
                    m_client.SendMessage(connectBroadcast);
                    m_client.Name = UsernameInput.Text;
                    UsernameInput.Clear();
                    //MessageWindow.AppendText("You have successfully connected with the username " + m_client.Name + ".\n");

                    ClientListUpdate listUpdate = new ClientListUpdate(m_client.Name, m_client.ID); ;
                    m_client.SendMessage(listUpdate);
                    //ClientList.Items.Add(m_client.Name);


                    Console.WriteLine(connectBroadcast.m_message);
                }
            }
            else
            {
                if (ConnectButton.Text == "Disconnect")
                {
                    ConnectButton.Text = "Connect";
                    UsernameInput.Enabled = true;
                    ClientList.Enabled = false;
                    SubmitButton.Enabled = false;
                    InputField.Enabled = false;

                    ChatMessagePacket disconnectBroadcast = new ChatMessagePacket("User: " + m_client.Name + " has disconnected.", m_client.ID);
                    m_client.SendMessage(disconnectBroadcast);
                    MessageWindow.AppendText("You have successfully Disconnected.\n");

                    Console.WriteLine(disconnectBroadcast.m_message);
                }
            }

        }

        private void ClientForm_Enter(object sender, EventArgs e)
        {
            SubmitButton_Click(sender, e);
        }

        private void ClientForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ExitPacket exitPacket = new ExitPacket("bitch");
            m_client.SendMessage(exitPacket);
            Console.WriteLine("Yeet");
        }

    }
}
