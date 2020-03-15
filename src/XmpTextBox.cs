////////////////////////////////////////////////////////////////////////
//
// This file is part of merge-jpeg-extended-xmp, a utility that
// demonstrates merging the JPEG Extended XMP packet with the main
// XMP packet.
//
// Copyright (c) 2020 Nicholas Hayes
//
// This file is licensed under the MIT License.
// See LICENSE.txt for complete licensing and attribution information.
//
////////////////////////////////////////////////////////////////////////

using ScintillaNET;
using System.Drawing;

namespace MergeJPEGExtendedXMP
{
    internal sealed class XmpTextBox : Scintilla
    {
        public XmpTextBox()
        {
            InitializeComponent();
        }

        public void ClearAllReadOnly()
        {
            if (ReadOnly)
            {
                ReadOnly = false;
                ClearAll();
                ReadOnly = true;
            }
            else
            {
                ClearAll();
            }
        }

        public void SetTextReadOnly(string text)
        {
            if (ReadOnly)
            {
                ReadOnly = false;
                Text = text;
                ReadOnly = true;
            }
            else
            {
                Text = text;
            }
        }

        private void InitializeComponent()
        {
            SuspendLayout();

            Lexer = Lexer.Xml;
            ReadOnly = true;

            // The following code is adapted from the example ScintillaNET XML Configuration at:
            // https://gist.github.com/anonymous/63036aa8c1cefcfcb013

            // Enable folding
            SetProperty("fold", "1");
            SetProperty("fold.compact", "1");
            SetProperty("fold.html", "1");
            AutomaticFold = AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change;

            // Use Margin 2 for fold markers
            Margins[2].Type = MarginType.Symbol;
            Margins[2].Mask = Marker.MaskFolders;
            Margins[2].Sensitive = true;
            Margins[2].Width = 20;

            // Reset folder markers
            for (int i = Marker.FolderEnd; i <= Marker.FolderOpen; i++)
            {
                Markers[i].SetForeColor(SystemColors.ControlLightLight);
                Markers[i].SetBackColor(SystemColors.ControlDark);
            }

            // Style the folder markers
            Markers[Marker.Folder].Symbol = MarkerSymbol.BoxPlus;
            Markers[Marker.Folder].SetBackColor(SystemColors.ControlText);
            Markers[Marker.FolderOpen].Symbol = MarkerSymbol.BoxMinus;
            Markers[Marker.FolderEnd].Symbol = MarkerSymbol.BoxPlusConnected;
            Markers[Marker.FolderEnd].SetBackColor(SystemColors.ControlText);
            Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            Markers[Marker.FolderOpenMid].Symbol = MarkerSymbol.BoxMinusConnected;
            Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Set the Styles
            StyleResetDefault();
            Styles[Style.Default].Font = "Consolas";
            Styles[Style.Default].Size = 10;
            StyleClearAll();
            Styles[Style.Xml.Tag].ForeColor = XmlStyleColors.TagForeColor;
            Styles[Style.Xml.TagUnknown].ForeColor = XmlStyleColors.TagUnknownForeColor;
            Styles[Style.Xml.TagEnd].ForeColor = XmlStyleColors.TagEndForeColor;
            Styles[Style.Xml.Attribute].ForeColor = XmlStyleColors.AttributeForeColor;
            Styles[Style.Xml.AttributeUnknown].ForeColor = XmlStyleColors.AttributeUnknown;
            Styles[Style.Xml.Number].ForeColor = XmlStyleColors.NumberForeColor;
            Styles[Style.Xml.DoubleString].ForeColor = XmlStyleColors.DoubleStringForeColor;
            Styles[Style.Xml.SingleString].ForeColor = XmlStyleColors.SingleStringForeColor;
            Styles[Style.Xml.Entity].ForeColor = XmlStyleColors.EntityForeColor;
            Styles[Style.Xml.Other].ForeColor = XmlStyleColors.OtherForeColor;
            Styles[Style.Xml.Comment].ForeColor = XmlStyleColors.CommentForeColor;
            Styles[Style.Xml.XmlStart].ForeColor = XmlStyleColors.XmlStartForeColor;
            Styles[Style.Xml.XmlEnd].ForeColor = XmlStyleColors.XmlEndForeColor;

            ResumeLayout(true);
        }

        private static class XmlStyleColors
        {
            // These values are the native Scintilla defaults.

            public static readonly Color TagForeColor = ColorTranslator.FromHtml("#000080");
            public static readonly Color TagUnknownForeColor = TagForeColor;
            public static readonly Color TagEndForeColor = TagForeColor;

            public static readonly Color AttributeForeColor = ColorTranslator.FromHtml("#008080");
            public static readonly Color AttributeUnknown = AttributeForeColor;

            public static readonly Color NumberForeColor = ColorTranslator.FromHtml("#007F7F");

            public static readonly Color DoubleStringForeColor = ColorTranslator.FromHtml("#7F007F");
            public static readonly Color SingleStringForeColor = DoubleStringForeColor;

            public static readonly Color EntityForeColor = ColorTranslator.FromHtml("#800080");
            public static readonly Color OtherForeColor = EntityForeColor;

            public static readonly Color CommentForeColor = ColorTranslator.FromHtml("#808000");

            public static readonly Color XmlStartForeColor = AttributeForeColor;

            public static readonly Color XmlEndForeColor = XmlStartForeColor;
        }
    }
}
