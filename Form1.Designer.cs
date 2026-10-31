namespace sztywniutko;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
        StartStopButton = new System.Windows.Forms.Button();
        FlashDuration = new System.Windows.Forms.NumericUpDown();
        PauseDuration = new System.Windows.Forms.NumericUpDown();
        label1 = new System.Windows.Forms.Label();
        label2 = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)FlashDuration).BeginInit();
        ((System.ComponentModel.ISupportInitialize)PauseDuration).BeginInit();
        SuspendLayout();
        // 
        // StartStopButton
        // 
        StartStopButton.Location = new System.Drawing.Point(23, 42);
        StartStopButton.Name = "StartStopButton";
        StartStopButton.Size = new System.Drawing.Size(83, 36);
        StartStopButton.TabIndex = 0;
        StartStopButton.Text = "Start";
        StartStopButton.UseVisualStyleBackColor = true;
        StartStopButton.Click += button1_Click;
        // 
        // FlashDuration
        // 
        FlashDuration.Increment = new decimal(new int[] { 100, 0, 0, 0 });
        FlashDuration.Location = new System.Drawing.Point(134, 51);
        FlashDuration.Maximum = new decimal(new int[] { 2000, 0, 0, 0 });
        FlashDuration.Minimum = new decimal(new int[] { 100, 0, 0, 0 });
        FlashDuration.Name = "FlashDuration";
        FlashDuration.Size = new System.Drawing.Size(120, 23);
        FlashDuration.TabIndex = 1;
        FlashDuration.Value = new decimal(new int[] { 100, 0, 0, 0 });
        FlashDuration.ValueChanged += FlashDuration_ValueChanged;
        // 
        // PauseDuration
        // 
        PauseDuration.Location = new System.Drawing.Point(278, 51);
        PauseDuration.Name = "PauseDuration";
        PauseDuration.Size = new System.Drawing.Size(120, 23);
        PauseDuration.TabIndex = 2;
        PauseDuration.ValueChanged += PauseDuration_ValueChanged;
        // 
        // label1
        // 
        label1.CausesValidation = false;
        label1.Location = new System.Drawing.Point(134, 29);
        label1.Name = "label1";
        label1.Size = new System.Drawing.Size(100, 19);
        label1.TabIndex = 3;
        label1.Text = "Flash duration";
        // 
        // label2
        // 
        label2.Location = new System.Drawing.Point(278, 29);
        label2.Name = "label2";
        label2.Size = new System.Drawing.Size(100, 19);
        label2.TabIndex = 4;
        label2.Text = "Pause duration";
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(656, 135);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(PauseDuration);
        Controls.Add(FlashDuration);
        Controls.Add(StartStopButton);
        Icon = ((System.Drawing.Icon)resources.GetObject("$this.Icon"));
        Text = "Szponcik";
        ((System.ComponentModel.ISupportInitialize)FlashDuration).EndInit();
        ((System.ComponentModel.ISupportInitialize)PauseDuration).EndInit();
        ResumeLayout(false);
    }

    private System.Windows.Forms.NumericUpDown FlashDuration;
    private System.Windows.Forms.NumericUpDown PauseDuration;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;

    private System.Windows.Forms.Button StartStopButton;

    #endregion
}