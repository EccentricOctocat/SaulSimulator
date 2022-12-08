using System.Media;
using System.Resources;

namespace FinalExam
{
    public partial class Form1 : Form
    {
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Form1() => InitializeComponent();
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // ========
        // Declare Assets:
        // This will also prevent memory leaking if ran in a loop!
        Bitmap picBestGame = SaulSimulator.Properties.Resources.BestGame;
        Bitmap picClipboard = SaulSimulator.Properties.Resources.cipboard;
        Bitmap picCourt1 = SaulSimulator.Properties.Resources.court1;
        Bitmap picCourtFinal = SaulSimulator.Properties.Resources.courtfinal;
        Bitmap picJail = SaulSimulator.Properties.Resources.jail;
        Bitmap picLawyerAd = SaulSimulator.Properties.Resources.lawyerad;
        Bitmap picSaul = SaulSimulator.Properties.Resources.saul;
        Bitmap picSaulArgument = SaulSimulator.Properties.Resources.saulargument;
        Bitmap picSaulFinal = SaulSimulator.Properties.Resources.saulfinal;
        Bitmap picSaulGif = SaulSimulator.Properties.Resources.saulgif;
        // Try getting sound in here too!
        // ========

        bool AutoScene = false; //used to automatically change scenes
        int intSaulScene = 0; //keeps track of which saul scene we are on
        private void Form1_Load(object sender, EventArgs e)
        {
            //hide certain controls since they appear only in later scenes
            lblDialogue.Visible = false;
            foreach (CheckBox? check in Controls.OfType<CheckBox>())
            {
                check.Visible = false;
            }
            //scale the form
            Form1_Resize(sender, e);
        }
        public SoundPlayer Player { get; set; }
        //  Play a new sound.
        public void PlaySound(UnmanagedMemoryStream location)
        {
            Player = new SoundPlayer(location);
            Player.Play();
        }
        private void btnNextScene_Click(object sender, EventArgs e)
        {
            //in case the user clicks the button with autoscene on, we reset the interval
            if (intSaulScene == (int)SaulScene.LawyerIntro)
            {
                tmrAutoScene.Interval = 250;
            }
            else tmrAutoScene.Interval = 150;
            if (intSaulScene == (int)SaulScene.CrimeRead && !AreAnyCheckboxesTicked())
            {//prevent the player from continuing if they didn't say their crime
                return;
            }
            intSaulScene++;
            NextSaulScene();
        }
        // Changes all controls on screen to their next iteration, based on the saul scene coming up.
        public void NextSaulScene()
        {
            switch (intSaulScene)
            {
                case (int)SaulScene.JailTime:
                    lblStoryText.Text = "You were hauled off to jail!";
                    picBigImage.Image = picJail;
                    PlaySound(SaulSimulator.Properties.Resources.JailCloseSound);
                    btnNextScene.Text = "Next Scene";
                    break;
                case (int)SaulScene.LawyerIntro:
                    lblStoryText.Text = "I should get a lawyer... But who?";
                    lblDialogue.Visible = true;
                    lblDialogue.Text = "Hey, inmate.\nNow's the time to talk to your lawyer. \nThat is, if you have one.";
                    PlaySound(SaulSimulator.Properties.Resources.saul_trap_remix);
                    break;
                case (int)SaulScene.LawyerAd:
                    lblStoryText.Text = "You recall a friendly voice in your head...";
                    lblDialogue.Visible = false;
                    picBigImage.Image = picLawyerAd;
                    Player.Stop();
                    PlaySound(SaulSimulator.Properties.Resources.saulcommercial);
                    break;
                case (int)SaulScene.CrimeRead:
                    lblStoryText.Text = "Select all crimes that apply.";
                    picBigImage.Image = picClipboard;
                    foreach (CheckBox? check in Controls.OfType<CheckBox>())
                    {
                        check.Visible = true;
                    }
                    Player.Stop();
                    break;
                case (int)SaulScene.Intermission:
                    lblStoryText.Visible = false;
                    lblDialogue.Visible = true;
                    lblDialogue.Text = "";
                    foreach (CheckBox? check in Controls.OfType<CheckBox>())
                    {
                        check.Enabled = false;
                        if (check.Checked)
                        {
                            lblDialogue.Text += $"{check.Text}, \n";
                        }
                    }
                    lblDialogue.Text += "That everything? I'll see if I can \nget you off those charges.";
                    break;
                case (int)SaulScene.TrialPart1:
                    PlaySound(SaulSimulator.Properties.Resources.JailOpenSound);
                    foreach (CheckBox? check in Controls.OfType<CheckBox>())
                    {
                        check.Visible = false;
                    }
                    lblDialogue.Text = "May it please the court...";
                    picBigImage.Image = picCourt1;
                    break;
                case (int)SaulScene.TrialPart2:
                    lblDialogue.Text = "As you can see from all these facts,\nmy client is innocent!";
                    picBigImage.Image = picSaulArgument;
                    break;
                case (int)SaulScene.TrialEnd:
                    lblDialogue.Text = "The court finds this case dismissed.\nDefendant is not guilty.";
                    picBigImage.Image = picCourtFinal;
                    break;
                case (int)SaulScene.ThankingSaul:
                    lblStoryText.Visible = true;
                    lblStoryText.Text = "Your charges were dropped\nAll thanks to Saul!";
                    lblDialogue.Visible = false;
                    picBigImage.Image = picSaulFinal;
                    break;
                case (int)SaulScene.Ending:
                    lblStoryText.Text = "Thanks for playing!";
                    picBigImage.Image = picSaulGif;
                    btnNextScene.Text = "Close Game";
                    break;
                default:
                    //out of bounds or just finished all scenes, close
                    Close();
                    break;
            }
        }
        private void Control_Keypress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Space))
            {
                AutoScene = !AutoScene;
                lblAuto.Text = $"Auto {AutoScene}\r\nPress Spacebar to auto";
                tmrAutoScene.Enabled = AutoScene;
            }
        }
        /// <summary>
        /// Represents each Saul Scene's name and their ordered number.
        /// </summary>
        public enum SaulScene
        {
            Start,
            JailTime,
            LawyerIntro,
            LawyerAd,
            CrimeRead,
            Intermission,
            TrialPart1,
            TrialPart2,
            TrialEnd,
            ThankingSaul,
            Ending
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            //adjust controls to fit the new dimensions
            picBigImage.Size = new Size(this.Width - 35, this.Height - 55);
            btnNextScene.Top = this.Height - 70;
            lblAuto.Location = new Point(this.Width - 150, this.Height - 80);
            lblDialogue.Location = new Point((int)((double)this.Width * 5.75 / 8.0), (int)((double)this.Height * 1.5 / 8.0));
            foreach (CheckBox? check in Controls.OfType<CheckBox>())
            {
                //can't use a switch case cause it's not a constant ffs
                //WITDH RESIZING
                if (check == chkAssault || check == chkTheft || check == chkInfluencedDriving || check == chkRecklessEndanger)
                {
                    //move the checkbox to the 3/8 length
                    check.Left = (int)((double)this.Width * 2.0 / 8.0);
                }
                else
                {
                    //move the checkbox to the 5/8 length
                    check.Left = (int)((double)this.Width * 4.0 / 8.0);
                }
                //LENGTH RESIZING
                if (check == chkAssault || check == chkSubstancePoss)
                {
                    check.Top = (int)((double)this.Height * 2.0 / 8.0);
                }
                else if (check == chkTheft || check == chkFirearmPoss)
                {
                    check.Top = (int)((double)this.Height * 3.0 / 8.0);
                }
                else if (check == chkInfluencedDriving|| check == chkCriminalMischief)
                {
                    check.Top = (int)((double)this.Height * 4.0 / 8.0);
                }
                else if (check == chkRecklessEndanger|| check == chkManslaughter)
                {
                    check.Top = (int)((double)this.Height * 5.0 / 8.0);
                }//this is so fucking ugly
            }
        }
        /// <summary>
        /// Gets a list of all checkboxes on the form and returns true or false based on if any of them are checked.
        /// </summary>
        /// <returns></returns>
        public bool AreAnyCheckboxesTicked()
        {
            foreach (CheckBox? check in Controls.OfType<CheckBox>())
            {
                if (check.Checked) return true;
            }
            return false;
        }

        private void tmrAutoScene_Tick(object sender, EventArgs e)
        {
            //count down until it reaches 0
            tmrAutoScene.Interval--;
            if (tmrAutoScene.Interval == 1)
            {
                //spend more time on the lawyer ad scene in order to fully appreciate saul
                if (intSaulScene == (int)SaulScene.LawyerIntro)
                {
                    tmrAutoScene.Interval = 250;
                }
                else tmrAutoScene.Interval = 150;

                if (intSaulScene == (int)SaulScene.CrimeRead && !AreAnyCheckboxesTicked())
                {
                    //wouldn't make sense to wait for user input when auto is on, so we tick some relevant checkboxes
                    chkCriminalMischief.Checked = true;
                    chkInfluencedDriving.Checked = true;
                    chkRecklessEndanger.Checked = true;
                    chkSubstancePoss.Checked = true;
                    chkManslaughter.Checked = true;
                }
                intSaulScene++;
                NextSaulScene();
            }
        }

        private void checkboxClick(object sender, EventArgs e)
        {
            //give more time to the user if they manually select a checkbox
            tmrAutoScene.Interval += 20;
        }
    }
}