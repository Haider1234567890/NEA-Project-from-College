using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using System.Data.SqlClient;
using System.Collections;
using System.Linq;

namespace Pandemic
{
    public partial class Form1 : Form
    {
        bool goUp, goDown, goRight, goLeft, gameOver, healthSpawned, speedSpawned;

        string facing = "up";

        int playerHealth = 100;
        int speed = 10;
        int ammo = 10;
        int enemySpeed = 1;
        int score;
        int waveNumber = 1;
        int maxZombiePerWave = 5;
        int enemyDamage = 1;

        Random randNum = new Random();
        //List of enemies, this is used for the spawning mechanic I have in the game.
        List<PictureBox> enemyList = new List<PictureBox>();
        public Form1()
        {
            InitializeComponent();
            RestartGame();
        }

        private void MainTimerEvent(object sender, EventArgs e)
        {
            Stack gameStack = new Stack();
            gameStack.Push("GAME START");
            gameStack.Push("GAME IS RUNNING"); 
            gameStack.Push("!");

            Console.WriteLine("gameStack");
            Console.WriteLine("\tCount:   {0}", gameStack.Count);
            Console.Write("\tValues:");
            PrintValues(gameStack);

            if (playerHealth > 0 && playerHealth <= 100)
            {
                HealthBar.Value = playerHealth;
            }

            else
            {
                gameOver = true;
                player.Image = Properties.Resources.dead;
                gameTimer.Stop();

                System.Windows.Forms.MessageBox.Show("Game Over! Please close this window and press Enter to proceed.");

                GameOver gameWindow = new GameOver();
                gameWindow.Show();

                Stack gameStack2 = new Stack();
                gameStack2.Push("GAME END");
                gameStack2.Push("GAME IS NO LONGER RUNNING");
                gameStack2.Push("!");

                Console.WriteLine("gameStack2");
                Console.WriteLine("\tCount:   {0}", gameStack2.Count);
                Console.Write("\tValues:");
                PrintValues(gameStack2);

                string filePath = @"C:\Users\user\OneDrive\Documents\A level Computer Science\Pandemic NEA (3) (1)\Pandemic NEA (2)\Pandemic NEA\Pandemic v2 (2)\Pandemic v2\Pandemic\Pandemic\LeaderBoard.txt";
                List<string> lines = new List<string>();
                lines = File.ReadAllLines(filePath).ToList();
                lines.Add(Convert.ToString(score));
                File.WriteAllLines(filePath, lines);

            }

            txtAmmo.Text = "Ammunition: " + ammo;
            txtScore.Text = "Eliminations: " + score;
            txtWave.Text = "Wave: " + waveNumber;

            if (goLeft == true && player.Left > 0)
            {
                player.Left -= speed;
            }

            if (goRight == true && player.Left + player.Width < this.ClientSize.Width)
            {
                player.Left += speed;
            }

            if (goUp == true && player.Top > 40)
            {
                player.Top -= speed;
            }

            if (goDown == true && player.Top + player.Height < this.ClientSize.Height)
            {
                player.Top += speed;
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        this.Controls.Remove(x);
                        ((PictureBox)x).Dispose();
                        ammo += 5;
                    }
                }

                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "health")
                    {
                        if (player.Bounds.IntersectsWith(j.Bounds))
                        {
                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            playerHealth += 50;
                            healthSpawned = false;
                        }

                        if (gameOver == true)
                        {
                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                        }
                    }
                }

                foreach (Control z in this.Controls)
                {
                    if (z is PictureBox && (string)z.Tag == "2xSpeed")
                    {
                        if (player.Bounds.IntersectsWith(z.Bounds))
                        {
                            this.Controls.Remove(z);
                            ((PictureBox)z).Dispose();
                            speedSpawned = false;
                            var timer = new System.Timers.Timer(10000);
                            timer.Elapsed += Timer_Elapsed;
                            speed += 5;
                        }

                        if (gameOver == true)
                        {
                            this.Controls.Remove(z);
                            ((PictureBox)z).Dispose();
                        }

                        if (speed >= 20)
                        {
                            speed = 20;
                            speedSpawned = true;
                        }
                    }
                }

                //Coding the AI for the zombies.
                if (x is PictureBox && (string)x.Tag == "zombie")
                {
                    if (player.Bounds.IntersectsWith(x.Bounds))
                    {
                        playerHealth -= enemyDamage;
                    }

                    //When the player moves in a certain direction the zombie will follow (pathfinding)
                    if (x.Left > player.Left)
                    {
                        x.Left -= enemySpeed;
                        ((PictureBox)x).Image = Properties.Resources.zleft;
                    }

                    if (x.Left < player.Left)
                    {
                        x.Left += enemySpeed;
                        ((PictureBox)x).Image = Properties.Resources.zright;
                    }

                    if (x.Top > player.Top)
                    {
                        x.Top -= enemySpeed;
                        ((PictureBox)x).Image = Properties.Resources.zup;
                    }

                    if (x.Top < player.Top)
                    {
                        x.Top += enemySpeed;
                        ((PictureBox)x).Image = Properties.Resources.zdown;
                    }

                }

                foreach (Control j in this.Controls)
                {
                    if (j is PictureBox && (string)j.Tag == "bullet" && x is PictureBox && (string)x.Tag == "zombie")
                    {
                        //Checks if bullet has collided with the zombie
                        if (x.Bounds.IntersectsWith(j.Bounds))
                        {
                            score++;
                            maxZombiePerWave--;
                            //Removes bullet
                            this.Controls.Remove(j);
                            ((PictureBox)j).Dispose();
                            //Removes zombie
                            this.Controls.Remove(x);
                            ((PictureBox)x).Dispose();
                            enemyList.Remove(((PictureBox)x));
                            SpawnEnemy();

                            if (maxZombiePerWave == 0)
                            {
                                waveNumber++;
                                enemySpeed++;
                                maxZombiePerWave = 5;
                                DropSpeed();
                                speedSpawned = true;

                                if (enemySpeed >= 5)
                                {
                                    enemySpeed = 5;
                                    enemyDamage = 2;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {

            if (gameOver == true)
            {
                return;
            }

            if (healthSpawned == false && playerHealth <= 25)
            {
                DropHealth();
                healthSpawned = true;
            }

            if (e.KeyCode == Keys.Right)
            {
                goRight = true;
                facing = "right";
                player.Image = Properties.Resources.right;
            }

            if (e.KeyCode == Keys.Left)
            {
                goLeft = true;
                facing = "left";
                player.Image = Properties.Resources.left;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = true;
                facing = "up";
                player.Image = Properties.Resources.up;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = true;
                facing = "down";
                player.Image = Properties.Resources.down;
            }
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            //Basic controls for player, move left and right or up and down using the arrow keys
            if (e.KeyCode == Keys.Right)
            {
                goRight = false;
            }

            if (e.KeyCode == Keys.Left)
            {
                goLeft = false;
            }

            if (e.KeyCode == Keys.Up)
            {
                goUp = false;
            }

            if (e.KeyCode == Keys.Down)
            {
                goDown = false;
            }

            if (e.KeyCode == Keys.Space && ammo > 0 && gameOver == false)
            {
                //uses ammo, takes 1 amay from current count of ammo
                ammo--;
                ShootBullet(facing);

                if (ammo < 1)
                {
                    DropAmmo();
                }
            }

            if (e.KeyCode == Keys.Enter && gameOver == true)
            {
                RestartGame();
            }

        }

        public static void mergesort(int[] array)
        {
            array = new int[] {9, 7, 8, 1, 3, 4 ,5 , 11, 14, 6, 10, 56, 78};
            for (int i = 0; i < array.Length; i++)
            {
                try
                {
                    array.DoMergeSort();
                }
                catch
                {
                    Console.WriteLine("No array found");
                }
            }
        }

        private void ShootBullet(string direction)
        {
            Bullet shootBullet = new Bullet();

            shootBullet.direction = direction;
            shootBullet.bulletLeft = player.Left + (player.Width / 2);
            shootBullet.bulletTop = player.Top + (player.Height / 2);
            shootBullet.MakeBullet(this);
        }

        private void SpawnEnemy()
        {
            PictureBox enemy = new PictureBox();
            enemy.Tag = "zombie";
            enemy.Image = Properties.Resources.zdown;
            enemy.Left = randNum.Next(0, 900);
            enemy.Top = randNum.Next(0, 900);
            enemy.SizeMode = PictureBoxSizeMode.AutoSize;
            enemyList.Add(enemy);
            this.Controls.Add(enemy);
            player.BringToFront();
        }

        private void DropAmmo()
        {
            PictureBox ammo = new PictureBox();
            ammo.Image = Properties.Resources.ammo_Image;
            ammo.SizeMode = PictureBoxSizeMode.AutoSize;
            ammo.Left = randNum.Next(10, this.ClientSize.Width - ammo.Width);
            ammo.Top = randNum.Next(60, this.ClientSize.Height - ammo.Height);
            ammo.Tag = "ammo";
            ammo.BringToFront();
            player.BringToFront();

            this.Controls.Add(ammo);

        }

        private void DropHealth()
        {
            PictureBox health = new PictureBox();
            health.Image = Properties.Resources.health2;
            health.SizeMode = PictureBoxSizeMode.AutoSize;
            health.Left = randNum.Next(20, this.ClientSize.Width - health.Width);
            health.Top = randNum.Next(60, this.ClientSize.Height - health.Height);
            health.Tag = "health";
            health.BringToFront();
            player.BringToFront();
            this.Controls.Add(health);
        }

        private void DropSpeed()
        {
            PictureBox speed = new PictureBox();
            speed.Image = Properties.Resources._2xspeed;
            speed.SizeMode = PictureBoxSizeMode.AutoSize;
            speed.Left = randNum.Next(20, this.ClientSize.Width - speed.Width);
            speed.Top = randNum.Next(60, this.ClientSize.Height - speed.Height);
            speed.Tag = "2xSpeed";
            speed.BringToFront();
            player.BringToFront();
            this.Controls.Add(speed);
        }

        private void RestartGame()
        {
            gameOver = false;
            goUp = false;
            goDown = false;
            goRight = false;
            goLeft = false;

            player.Image = Properties.Resources.up;

            foreach (PictureBox i in enemyList)
            {
                this.Controls.Remove(i);
            }

            foreach (Control x in this.Controls)
            {
                if (x is PictureBox && (string)x.Tag == "ammo")
                {
                    this.Controls.Remove(x);
                }
            }
            enemyList.Clear();

            for (int i = 0; i < maxZombiePerWave; i++)
            {
                SpawnEnemy();
            }

            playerHealth = 100;
            score = 0;
            ammo = 10;
            waveNumber = 1;
            enemySpeed = 1;
            enemyDamage = 1;
            speed = 10;

            gameTimer.Start();


        }
        public static void PrintValues(IEnumerable myCollection)
        {
            foreach (Object obj in myCollection)
                Console.Write("{0}", obj);
            Console.WriteLine();
        }
    }
}
