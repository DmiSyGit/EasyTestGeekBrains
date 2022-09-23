namespace EasyTestGeekBrains
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.connectionBtn = new System.Windows.Forms.Button();
            this.testUrlTb = new System.Windows.Forms.TextBox();
            this.checkTestBtn = new System.Windows.Forms.Button();
            this.testLbl = new System.Windows.Forms.Label();
            this.solveTest = new System.Windows.Forms.Button();
            this.passwordTb = new System.Windows.Forms.TextBox();
            this.passLbl = new System.Windows.Forms.Label();
            this.loginLbl = new System.Windows.Forms.Label();
            this.loginTb = new System.Windows.Forms.TextBox();
            this.nextAccount = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.button1 = new System.Windows.Forms.Button();
            this.counterAccountLbl = new System.Windows.Forms.Label();
            this.modeSolutionBtn = new System.Windows.Forms.Button();
            this.modeSolutionLbl = new System.Windows.Forms.Label();
            this.rememberAnswersBtn = new System.Windows.Forms.Button();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // connectionBtn
            // 
            this.connectionBtn.BackColor = System.Drawing.Color.Bisque;
            this.connectionBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.connectionBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.connectionBtn.Location = new System.Drawing.Point(511, 12);
            this.connectionBtn.Name = "connectionBtn";
            this.connectionBtn.Size = new System.Drawing.Size(277, 62);
            this.connectionBtn.TabIndex = 0;
            this.connectionBtn.Text = "Установить соединение с базой ответов";
            this.connectionBtn.UseVisualStyleBackColor = false;
            this.connectionBtn.Click += new System.EventHandler(this.connectionBtn_Click);
            // 
            // testUrlTb
            // 
            this.testUrlTb.Location = new System.Drawing.Point(511, 96);
            this.testUrlTb.Name = "testUrlTb";
            this.testUrlTb.Size = new System.Drawing.Size(277, 20);
            this.testUrlTb.TabIndex = 2;
            this.testUrlTb.Text = "https://gb.ru/tests/166";
            // 
            // checkTestBtn
            // 
            this.checkTestBtn.BackColor = System.Drawing.Color.Bisque;
            this.checkTestBtn.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.checkTestBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.checkTestBtn.Location = new System.Drawing.Point(511, 122);
            this.checkTestBtn.Name = "checkTestBtn";
            this.checkTestBtn.Size = new System.Drawing.Size(277, 45);
            this.checkTestBtn.TabIndex = 3;
            this.checkTestBtn.Text = "Проверить тест";
            this.checkTestBtn.UseVisualStyleBackColor = false;
            this.checkTestBtn.Click += new System.EventHandler(this.checkTestBtn_Click);
            // 
            // testLbl
            // 
            this.testLbl.AutoSize = true;
            this.testLbl.Location = new System.Drawing.Point(511, 81);
            this.testLbl.Name = "testLbl";
            this.testLbl.Size = new System.Drawing.Size(89, 13);
            this.testLbl.TabIndex = 4;
            this.testLbl.Text = "Ссылка на тест:";
            // 
            // solveTest
            // 
            this.solveTest.BackColor = System.Drawing.Color.Bisque;
            this.solveTest.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.solveTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.solveTest.Location = new System.Drawing.Point(514, 315);
            this.solveTest.Name = "solveTest";
            this.solveTest.Size = new System.Drawing.Size(277, 45);
            this.solveTest.TabIndex = 5;
            this.solveTest.Text = "Решить тест";
            this.solveTest.UseVisualStyleBackColor = false;
            this.solveTest.Click += new System.EventHandler(this.solveTest_Click);
            // 
            // passwordTb
            // 
            this.passwordTb.Location = new System.Drawing.Point(514, 260);
            this.passwordTb.Name = "passwordTb";
            this.passwordTb.Size = new System.Drawing.Size(277, 20);
            this.passwordTb.TabIndex = 6;
            // 
            // passLbl
            // 
            this.passLbl.AutoSize = true;
            this.passLbl.Location = new System.Drawing.Point(511, 244);
            this.passLbl.Name = "passLbl";
            this.passLbl.Size = new System.Drawing.Size(166, 13);
            this.passLbl.TabIndex = 7;
            this.passLbl.Text = "Пароль от аккаунта GeekBrains";
            // 
            // loginLbl
            // 
            this.loginLbl.AutoSize = true;
            this.loginLbl.Location = new System.Drawing.Point(511, 205);
            this.loginLbl.Name = "loginLbl";
            this.loginLbl.Size = new System.Drawing.Size(96, 13);
            this.loginLbl.TabIndex = 9;
            this.loginLbl.Text = "Логин GeekBrains";
            // 
            // loginTb
            // 
            this.loginTb.Location = new System.Drawing.Point(514, 221);
            this.loginTb.Name = "loginTb";
            this.loginTb.Size = new System.Drawing.Size(277, 20);
            this.loginTb.TabIndex = 8;
            // 
            // nextAccount
            // 
            this.nextAccount.BackColor = System.Drawing.Color.Bisque;
            this.nextAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.nextAccount.Location = new System.Drawing.Point(640, 286);
            this.nextAccount.Name = "nextAccount";
            this.nextAccount.Size = new System.Drawing.Size(148, 23);
            this.nextAccount.TabIndex = 10;
            this.nextAccount.Text = "Следующий аккаунт";
            this.nextAccount.UseVisualStyleBackColor = false;
            this.nextAccount.Visible = false;
            this.nextAccount.Click += new System.EventHandler(this.nextAccount_Click);
            // 
            // label1
            // 
            this.label1.AutoEllipsis = true;
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.MaximumSize = new System.Drawing.Size(481, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "ЛОГ:";
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.flowLayoutPanel1.AutoScroll = true;
            this.flowLayoutPanel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.flowLayoutPanel1.Controls.Add(this.label1);
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 3);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(502, 529);
            this.flowLayoutPanel1.TabIndex = 12;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(511, 509);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 13;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // counterAccountLbl
            // 
            this.counterAccountLbl.AutoSize = true;
            this.counterAccountLbl.Location = new System.Drawing.Point(538, 292);
            this.counterAccountLbl.Name = "counterAccountLbl";
            this.counterAccountLbl.Size = new System.Drawing.Size(71, 13);
            this.counterAccountLbl.TabIndex = 14;
            this.counterAccountLbl.Text = "Аккаунт № 0";
            // 
            // modeSolutionBtn
            // 
            this.modeSolutionBtn.BackColor = System.Drawing.Color.Bisque;
            this.modeSolutionBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.modeSolutionBtn.Location = new System.Drawing.Point(514, 460);
            this.modeSolutionBtn.Name = "modeSolutionBtn";
            this.modeSolutionBtn.Size = new System.Drawing.Size(277, 28);
            this.modeSolutionBtn.TabIndex = 15;
            this.modeSolutionBtn.Text = "Режим решения теста";
            this.modeSolutionBtn.UseVisualStyleBackColor = false;
            this.modeSolutionBtn.Click += new System.EventHandler(this.modeSolutionBtn_Click);
            // 
            // modeSolutionLbl
            // 
            this.modeSolutionLbl.AutoEllipsis = true;
            this.modeSolutionLbl.AutoSize = true;
            this.modeSolutionLbl.Location = new System.Drawing.Point(514, 367);
            this.modeSolutionLbl.MaximumSize = new System.Drawing.Size(280, 0);
            this.modeSolutionLbl.Name = "modeSolutionLbl";
            this.modeSolutionLbl.Size = new System.Drawing.Size(241, 26);
            this.modeSolutionLbl.TabIndex = 16;
            this.modeSolutionLbl.Text = "Режим решения: 1 Автоматическое решение, вмешиваться не рекомендуется";
            // 
            // rememberAnswersBtn
            // 
            this.rememberAnswersBtn.Enabled = false;
            this.rememberAnswersBtn.Location = new System.Drawing.Point(592, 494);
            this.rememberAnswersBtn.Name = "rememberAnswersBtn";
            this.rememberAnswersBtn.Size = new System.Drawing.Size(196, 23);
            this.rememberAnswersBtn.TabIndex = 17;
            this.rememberAnswersBtn.Text = "Запомнить ответы";
            this.rememberAnswersBtn.UseVisualStyleBackColor = true;
            this.rememberAnswersBtn.Click += new System.EventHandler(this.rememberAnswersBtn_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 535);
            this.Controls.Add(this.rememberAnswersBtn);
            this.Controls.Add(this.modeSolutionLbl);
            this.Controls.Add(this.modeSolutionBtn);
            this.Controls.Add(this.counterAccountLbl);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.nextAccount);
            this.Controls.Add(this.loginLbl);
            this.Controls.Add(this.loginTb);
            this.Controls.Add(this.passLbl);
            this.Controls.Add(this.passwordTb);
            this.Controls.Add(this.solveTest);
            this.Controls.Add(this.testLbl);
            this.Controls.Add(this.checkTestBtn);
            this.Controls.Add(this.testUrlTb);
            this.Controls.Add(this.connectionBtn);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button connectionBtn;
        private System.Windows.Forms.TextBox testUrlTb;
        private System.Windows.Forms.Button checkTestBtn;
        private System.Windows.Forms.Label testLbl;
        private System.Windows.Forms.Button solveTest;
        private System.Windows.Forms.TextBox passwordTb;
        private System.Windows.Forms.Label passLbl;
        private System.Windows.Forms.Label loginLbl;
        private System.Windows.Forms.TextBox loginTb;
        private System.Windows.Forms.Button nextAccount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label counterAccountLbl;
        private System.Windows.Forms.Button modeSolutionBtn;
        private System.Windows.Forms.Label modeSolutionLbl;
        private System.Windows.Forms.Button rememberAnswersBtn;
    }
}

