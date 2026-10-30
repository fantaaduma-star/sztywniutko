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
        StartStopButton = new System.Windows.Forms.Button();
        SuspendLayout();
        // 
        // StartStopButton
        // 
        StartStopButton.Location = new System.Drawing.Point(57, 42);
        StartStopButton.Name = "StartStopButton";
        StartStopButton.Size = new System.Drawing.Size(83, 36);
        StartStopButton.TabIndex = 0;
        StartStopButton.Text = "Start";
        StartStopButton.UseVisualStyleBackColor = true;
        StartStopButton.Click += button1_Click;
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(StartStopButton);
        Text = "Szponcik";
        ResumeLayout(false);
    }

    private System.Windows.Forms.Button StartStopButton;

    #endregion
}