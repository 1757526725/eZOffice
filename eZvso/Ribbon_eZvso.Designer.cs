﻿using System.Collections.Generic;
using System;
using Office = Microsoft.Office.Core;
using Microsoft.VisualBasic;
using System.Xml.Linq;
using Visio = Microsoft.Office.Interop.Visio;
using System.Text;
using System.Linq;

namespace eZvso
{
    partial class Ribbon_eZvso : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public Ribbon_eZvso()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

        /// <summary> 
        /// Clean up any resources being used.
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Tab1 = this.Factory.CreateRibbonTab();
            this.Group_Drawing = this.Factory.CreateRibbonGroup();
            this.btnPaste = this.Factory.CreateRibbonButton();
            this.btnArrayCircle = this.Factory.CreateRibbonButton();
            this.btnArray = this.Factory.CreateRibbonButton();
            this.btnMove = this.Factory.CreateRibbonButton();
            this.btnArea = this.Factory.CreateRibbonButton();
            this.Group_Master = this.Factory.CreateRibbonGroup();
            this.btnMasterBase = this.Factory.CreateRibbonSplitButton();
            this.btnLocPin = this.Factory.CreateRibbonButton();
            this.Tab1.SuspendLayout();
            this.Group_Drawing.SuspendLayout();
            this.Group_Master.SuspendLayout();
            this.SuspendLayout();
            // 
            // Tab1
            // 
            this.Tab1.Groups.Add(this.Group_Drawing);
            this.Tab1.Groups.Add(this.Group_Master);
            this.Tab1.Label = "eZvso";
            this.Tab1.Name = "Tab1";
            // 
            // Group_Drawing
            // 
            this.Group_Drawing.Items.Add(this.btnPaste);
            this.Group_Drawing.Items.Add(this.btnArrayCircle);
            this.Group_Drawing.Items.Add(this.btnArray);
            this.Group_Drawing.Items.Add(this.btnMove);
            this.Group_Drawing.Items.Add(this.btnArea);
            this.Group_Drawing.Label = "绘图";
            this.Group_Drawing.Name = "Group_Drawing";
            // 
            // btnPaste
            // 
            this.btnPaste.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnPaste.Label = "原位粘贴";
            this.btnPaste.Name = "btnPaste";
            this.btnPaste.OfficeImageId = "Paste";
            this.btnPaste.ShowImage = true;
            this.btnPaste.SuperTip = "    即是通过\"开发工具>组合>添加到组\"命令实现。 \n\r   当前选择必须包含要添加的形状和要在其中添加这些形状的组合。组合必须为首要选择或选择中的唯一一个组" +
    "合。\"";
            this.btnPaste.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.AddToGroup);
            // 
            // btnArrayCircle
            // 
            this.btnArrayCircle.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnArrayCircle.Label = "阵列";
            this.btnArrayCircle.Name = "btnArrayCircle";
            this.btnArrayCircle.OfficeImageId = "PictureBrightnessGallery";
            this.btnArrayCircle.ShowImage = true;
            this.btnArrayCircle.Tag = "7";
            this.btnArrayCircle.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.CircleArray);
            // 
            // btnArray
            // 
            this.btnArray.Label = "阵列";
            this.btnArray.Name = "btnArray";
            this.btnArray.OfficeImageId = "NavPaneThumbnailView";
            this.btnArray.ShowImage = true;
            this.btnArray.Tag = "7";
            this.btnArray.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMove_Click);
            // 
            // btnMove
            // 
            this.btnMove.Label = "移动";
            this.btnMove.Name = "btnMove";
            this.btnMove.OfficeImageId = "PageRightPreview";
            this.btnMove.ShowImage = true;
            this.btnMove.Tag = "5";
            this.btnMove.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMove_Click);
            // 
            // btnArea
            // 
            this.btnArea.Label = "面积/周长";
            this.btnArea.Name = "btnArea";
            this.btnArea.OfficeImageId = "BlackAndWhiteWhite";
            this.btnArea.ShowImage = true;
            this.btnArea.Tag = "6";
            this.btnArea.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMove_Click);
            // 
            // Group_Master
            // 
            this.Group_Master.Items.Add(this.btnMasterBase);
            this.Group_Master.Label = "主控形状编辑";
            this.Group_Master.Name = "Group_Master";
            // 
            // btnMasterBase
            // 
            this.btnMasterBase.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnMasterBase.Items.Add(this.btnLocPin);
            this.btnMasterBase.Label = "固定基点";
            this.btnMasterBase.Name = "btnMasterBase";
            this.btnMasterBase.OfficeImageId = "BorderInside";
            this.btnMasterBase.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnMasterBase_Click);
            // 
            // btnLocPin
            // 
            this.btnLocPin.Label = "局部坐标";
            this.btnLocPin.Name = "btnLocPin";
            this.btnLocPin.ShowImage = true;
            this.btnLocPin.SuperTip = "此主控形状的实例对象的旋转中心点相对于实例对象的左下角点的位置";
            this.btnLocPin.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnLocPin_Click);
            // 
            // Ribbon_eZvso
            // 
            this.Name = "Ribbon_eZvso";
            this.RibbonType = "Microsoft.Visio.Drawing";
            this.Tabs.Add(this.Tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.Ribbon_eZvso_Load);
            this.Tab1.ResumeLayout(false);
            this.Tab1.PerformLayout();
            this.Group_Drawing.ResumeLayout(false);
            this.Group_Drawing.PerformLayout();
            this.Group_Master.ResumeLayout(false);
            this.Group_Master.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab Tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Group_Master;
        internal Microsoft.Office.Tools.Ribbon.RibbonSplitButton btnMasterBase;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup Group_Drawing;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnPaste;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnMove;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnArray;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnArea;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnArrayCircle;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnLocPin;
    }

    partial class ThisRibbonCollection
    {
        internal Ribbon_eZvso Ribbon_eZvso
        {
            get { return this.GetRibbon<Ribbon_eZvso>(); }
        }
    }
}
