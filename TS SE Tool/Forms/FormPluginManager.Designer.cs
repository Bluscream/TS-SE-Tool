namespace TS_SE_Tool.Forms {
    partial class FormPluginManager {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPluginManager));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.enumerableExtBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.openPluginsDirectoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x64ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.x86ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.enumerableExtBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openPluginsDirectoryToolStripMenuItem});
            resources.ApplyResources(this.menuStrip1, "menuStrip1");
            this.menuStrip1.Name = "menuStrip1";
            // 
            // enumerableExtBindingSource
            // 
            this.enumerableExtBindingSource.DataSource = typeof(TS_SE_Tool.Global.EnumerableExt);
            // 
            // openPluginsDirectoryToolStripMenuItem
            // 
            this.openPluginsDirectoryToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.x64ToolStripMenuItem,
            this.x86ToolStripMenuItem});
            this.openPluginsDirectoryToolStripMenuItem.Name = "openPluginsDirectoryToolStripMenuItem";
            resources.ApplyResources(this.openPluginsDirectoryToolStripMenuItem, "openPluginsDirectoryToolStripMenuItem");
            // 
            // x64ToolStripMenuItem
            // 
            this.x64ToolStripMenuItem.Name = "x64ToolStripMenuItem";
            resources.ApplyResources(this.x64ToolStripMenuItem, "x64ToolStripMenuItem");
            this.x64ToolStripMenuItem.Click += new System.EventHandler(this.openPluginsDirToolStripMenuItem_Click);
            // 
            // x86ToolStripMenuItem
            // 
            this.x86ToolStripMenuItem.Name = "x86ToolStripMenuItem";
            resources.ApplyResources(this.x86ToolStripMenuItem, "x86ToolStripMenuItem");
            this.x86ToolStripMenuItem.Click += new System.EventHandler(this.openPluginsDirToolStripMenuItem_Click);
            // 
            // FormPluginManager
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormPluginManager";
            this.Load += new System.EventHandler(this.FormPluginManager_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.enumerableExtBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.BindingSource enumerableExtBindingSource;
        private System.Windows.Forms.ToolStripMenuItem openPluginsDirectoryToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x64ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem x86ToolStripMenuItem;
    }
}