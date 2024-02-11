using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace ColorGameApp.WindowsForm
{
    public partial class Form1 : Form
    {
        private const int ColorPanelsNumber = 5;

        private const int ColorsNumber = 8;

        private const int TimePenaltyPoints = 5;

        private const int ColorPenaltyPoints = 2;

        private float _gameTime;

        private int _gamePoints;

        private float _roundTime;

        private Color _roundChosenColor;

        private string _roundChosenColorName;

        private SoundPlayer _soundPlayer = new SoundPlayer();

        private Color[] _tablicaKolorow = new Color[ColorsNumber] {
             Color.White,
             Color.Red,
             Color.Blue,
             Color.Green,
             Color.Yellow,
             Color.Purple,
             Color.Pink,
             Color.Orange
        };

        private string[] _tablicaTekstow = new string[ColorsNumber]
        {
             "biały",
             "czerwony",
             "niebieski",
             "zielony",
             "żółty",
             "fioletowy",
             "różowy",
             "pomarańczowy"
        };

        public Form1()
        {
            InitializeComponent();
        }

        //INITIALIZATION
        private void Form1_Load(object sender, EventArgs e)
        {
  
            Translate();
            ResetGame();
        }

        private void SetLevelRoundTime()
        {
            if (this.level1ToolStripMenuItem.Checked)
            {
                this._roundTime = 2f;
            }
            else if (this.level2ToolStripMenuItem.Checked)
            {
                this._roundTime = 1.5f;
            }
            else if (this.level3ToolStripMenuItem.Checked)
            {
                this._roundTime = 1f;
            }
            this.toolStripStatusLabel1.Text = this._roundTime.ToString("F1");
        }

        private void ActivateMenuOptions(bool value)
        {
            this.startToolStripMenuItem.Enabled = value;
            this.levelToolStripMenuItem.Enabled = value;
            this.informationToolStripMenuItem.Enabled = value;
        }
        
        private void SetPrivateFields()
        {
            this._gameTime = 15;
            this._gamePoints = 0;
            SetLevelRoundTime();
            this.chooseColorValue.Text = "";
            this.pointsValue.Text = this._gamePoints.ToString();
            this.timeValue.Text = $"{this._gameTime.ToString("F1")} {Properties.Settings.Default.secondsString}";
        }

        private void SetGame()
        {
            SetPrivateFields();
            RandomizeData();
            ActivateColorPanels(true);
            ResetAllTimers();
            ActivateMenuOptions(false);
            this.toolStripStatusLabel1.Text = this._roundTime.ToString("F1");
            PlayMusic("gas.wav");
        }

        private void ResetGame()
        {
            StopAllTimers();
            ResetColorPanels();
            SetPrivateFields();
            ActivateMenuOptions(true);
            this.toolStripStatusLabel1.Text = Properties.Settings.Default.startInfoString;
            StopMusic();   
        }

        private void NextRound()
        {
            RandomizeData();
            ResetAnswerTimer();
            SetLevelRoundTime();
        }

        private void GameOver()
        {
            StopAllTimers();
            this.toolStripStatusLabel1.Text = Properties.Settings.Default.menuStripGameOverString;
            string title = Properties.Settings.Default.alertGameOverTitleString;
            string content = $"{Properties.Settings.Default.alertGameOverContent1String} { this._gamePoints} {Properties.Settings.Default.alertGameOverContent2String}.";
            MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetGame();
        }

        private void RandomizeData()
        {
            int[] colors = Dumpster.losujNumeryKolorow(ColorPanelsNumber, ColorsNumber - 1);
            if (colors.Length == ColorPanelsNumber)
            {
                Panel [] colorPanels = { colorPanel1, colorPanel2, colorPanel3, colorPanel4, colorPanel5 };

                for (int i = 0; i < ColorPanelsNumber; i++)
                {
                    colorPanels[i].BackColor = _tablicaKolorow[colors[i]];
                    colorPanels[i].Name = _tablicaTekstow[colors[i]];
                }

                this.chooseColorValue.Text = _tablicaTekstow[colors[Dumpster.rand.Next(ColorPanelsNumber)]];
                this.chooseColorValue.ForeColor = _tablicaKolorow[Dumpster.rand.Next(ColorPanelsNumber)];
            }
        }

        private void Translate()
        {
            this.Text = Properties.Settings.Default.titleString;
            //Menu1
            this.actionToolStripMenuItem.Text = Properties.Settings.Default.actionString;
            this.startToolStripMenuItem.Text = Properties.Settings.Default.startString; 
            this.resetToolStripMenuItem.Text = Properties.Settings.Default.resetAppString; 
            this.closeToolStripMenuItem.Text = Properties.Settings.Default.closeString; 
            //Menu2
            this.levelToolStripMenuItem.Text = Properties.Settings.Default.levelString;
            this.level1ToolStripMenuItem.Text = Properties.Settings.Default.level1String;
            this.level2ToolStripMenuItem.Text = Properties.Settings.Default.level2String;
            this.level3ToolStripMenuItem.Text = Properties.Settings.Default.level3String;
            //Menu3
            this.informationToolStripMenuItem.Text = Properties.Settings.Default.infoString;
            //Labels
            this.chooseColorLabel.Text = Properties.Settings.Default.chooseColorString;
            this.timeLabel.Text = Properties.Settings.Default.remainingTimeString;
            this.pointsLabel.Text = Properties.Settings.Default.pointsString;
        }


        //TIMERS
        private void ResetGameTimer()
        {
            this.gameTimer.Stop();
            this.gameTimer.Interval = 500;
            this.gameTimer.Start();
        }

        private void ResetAnswerTimer()
        {
            this.answerTimer.Stop();
            this.answerTimer.Interval = 500;
            this.answerTimer.Start();
        }

        private void ResetAllTimers()
        {
            ResetGameTimer();
            ResetAnswerTimer();
        }

        private void StopAllTimers()
        {
            this.answerTimer.Stop();
            this.gameTimer.Stop();
        }

        private void gameTimer_Tick(object sender, EventArgs e)
        {
            float zero = 0f;
            if (this._gameTime > zero)
            {
                this._gameTime-=0.5f;
                this.timeValue.Text = $"{this._gameTime.ToString("F1")} {Properties.Settings.Default.secondsString}";
            }
            else
            {
                GameOver();
            }
        }

        private void answerTimer_Tick(object sender, EventArgs e)
        {
            this._roundTime-=0.5f;
            this.toolStripStatusLabel1.Text = this._roundTime.ToString("F1");
            float zero = 0f;
            if (this._roundTime == zero && this._gameTime > zero)
            {
                this._gamePoints -= TimePenaltyPoints;
                this.pointsValue.Text = this._gamePoints.ToString();
                NextRound();
            }
            else if (this._roundTime == zero && this._gameTime == zero)
            {
                this._gamePoints -= TimePenaltyPoints;
                this.pointsValue.Text = this._gamePoints.ToString();
                GameOver();
            }

        }

  
        //COLOR PANELS
        private void ActivateColorPanels(bool value)
        {
            this.colorPanel1.Enabled = value;
            this.colorPanel2.Enabled = value;
            this.colorPanel3.Enabled = value;
            this.colorPanel4.Enabled = value;
            this.colorPanel5.Enabled = value;
        }

        private void ResetColorPanels()
        {
            ActivateColorPanels(false);
            Panel[] colorPanels = { colorPanel1, colorPanel2, colorPanel3, colorPanel4, colorPanel5 };

            for (int i = 0; i < ColorPanelsNumber; i++)
            {
                colorPanels[i].BackColor = Color.DarkGray;
                colorPanels[i].Name = $"colorPanel{i + 1}";
            } 
        }

        private void VerifyClickedColor()
        {
            if (this.chooseColorValue.Text == this._roundChosenColorName)
            {
                this._gamePoints++;
            }
            else
            {
                this._gamePoints -= ColorPenaltyPoints;
            }
            NextRound();
            this.pointsValue.Text = this._gamePoints.ToString();
        }

        private void colorPanel1_Click(object sender, EventArgs e)
        {
            if(this._gameTime > 0f)
            {
                this._roundChosenColor = this.colorPanel1.BackColor;
                this._roundChosenColorName = this.colorPanel1.Name;
                VerifyClickedColor();
            }
            else GameOver();
        }

        private void colorPanel2_Click(object sender, EventArgs e)
        {
            if (this._gameTime > 0f)
            {
                this._roundChosenColor = this.colorPanel2.BackColor;
                this._roundChosenColorName = this.colorPanel2.Name;
                VerifyClickedColor();
            }
            else GameOver();
        }

        private void colorPanel3_Click(object sender, EventArgs e)
        {
            if (this._gameTime > 0f)
            {
                this._roundChosenColor = this.colorPanel3.BackColor;
                this._roundChosenColorName = this.colorPanel3.Name;
                VerifyClickedColor();
            }
            else GameOver();
        }

        private void colorPanel4_Click(object sender, EventArgs e)
        {
            if (this._gameTime > 0f)
            {
                this._roundChosenColor = this.colorPanel4.BackColor;
                this._roundChosenColorName = this.colorPanel4.Name;
                VerifyClickedColor();
            }
            else GameOver();
        }

        private void colorPanel5_Click(object sender, EventArgs e)
        {
            if (this._gameTime > 0f)
            {
                this._roundChosenColor = this.colorPanel5.BackColor;
                this._roundChosenColorName = this.colorPanel5.Name;
                VerifyClickedColor();
            }
            else GameOver();
        }


        //MENU
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGame();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetGame();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit(); 
        }

        private void level1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.level1ToolStripMenuItem.Checked = true;
            this.level2ToolStripMenuItem.Checked = false;
            this.level3ToolStripMenuItem.Checked = false;
        }

        private void level2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.level1ToolStripMenuItem.Checked = false;
            this.level2ToolStripMenuItem.Checked = true;
            this.level3ToolStripMenuItem.Checked = false;
        }

        private void level3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.level1ToolStripMenuItem.Checked = false;
            this.level2ToolStripMenuItem.Checked = false;
            this.level3ToolStripMenuItem.Checked = true;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string title = Properties.Settings.Default.alertExitTitleString;
            string content = Properties.Settings.Default.alertExitContentString;
            var clickedButton = MessageBox.Show(
                content,
                title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question,
                MessageBoxDefaultButton.Button2
            );
            switch (clickedButton)
            {
                case DialogResult.Yes: break;
                case DialogResult.No: e.Cancel = true; break;
                default: e.Cancel = true; break;
            }

        }

        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StopAllTimers();
            string title = Properties.Settings.Default.alertInfoTitleString;
            string content = Properties.Settings.Default.alertInfoContentString;
            MessageBox.Show(content, title, MessageBoxButtons.OK, MessageBoxIcon.Information);
            ResetGame();
        }

        //MUSIC
        private void PlayMusic(string fileName)
        {
            try
            {
                _soundPlayer = new SoundPlayer(fileName);
                _soundPlayer.Play();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private void StopMusic()
        {
            try
            {
                _soundPlayer.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }

}
