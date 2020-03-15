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
using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace MergeJPEGExtendedXMP
{
    public partial class Form1 : Form
    {
        private readonly string[] commandLineArgs;

        public Form1(string[] args)
        {
            InitializeComponent();
            commandLineArgs = args;
            fileNameLabel.Text = string.Empty;
            xmpSizeLabel.Text = string.Empty;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (commandLineArgs.Length == 1)
            {
                OpenFile(commandLineArgs[0]);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                OpenFile(openFileDialog1.FileName);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void OpenFile(string path)
        {
            mergedXmpTextBox.ClearAllReadOnly();
            mainXmpTextBox.ClearAllReadOnly();
            extendedXmpTextBox.ClearAllReadOnly();
            fileNameLabel.Text = string.Empty;
            xmpSizeLabel.Text = string.Empty;

            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync(path);
            }
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string path = (string)e.Argument;

            JpegXmpPackets xmpPackets = new JpegXmpReader(path).GetXmpPackets();

            e.Result = new WorkerResult(path, xmpPackets);
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(this, e.Error.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
            }
            else
            {
                WorkerResult result = (WorkerResult)e.Result;
                JpegXmpPackets xmpPackets = result.XmpPackets;

                fileNameLabel.Text = result.Path;

                if (xmpPackets != null)
                {
                    long extendedXmpLength = 0;

                    mainXmpTextBox.SetTextReadOnly(xmpPackets.Main.ToString());
                    long mainXmpLength = xmpPackets.Main.LengthInBytes;

                    if (xmpPackets.Extended != null)
                    {
                        extendedXmpTextBox.SetTextReadOnly(xmpPackets.Extended.ToString());
                        extendedXmpLength = xmpPackets.Extended.LengthInBytes;
                    }

                    mergedXmpTextBox.SetTextReadOnly(xmpPackets.Merged.ToString());
                    long mergedXmpLength = xmpPackets.Merged.LengthInBytes;

                    xmpSizeLabel.Text = string.Format(CultureInfo.CurrentCulture,
                                                      "Main XMP size: {0}, Extended XMP size: {1}, Merged XMP size: {2}",
                                                      GetFileSizeString(mainXmpLength),
                                                      GetFileSizeString(extendedXmpLength),
                                                      GetFileSizeString(mergedXmpLength));
                }
            }
        }

        private void wrapXMPTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem menuItem = (ToolStripMenuItem)sender;

            if (menuItem.Checked)
            {
                mergedXmpTextBox.WrapMode = WrapMode.Char;
                mainXmpTextBox.WrapMode = WrapMode.Char;
                extendedXmpTextBox.WrapMode = WrapMode.Char;
            }
            else
            {
                mergedXmpTextBox.WrapMode = WrapMode.None;
                mainXmpTextBox.WrapMode = WrapMode.None;
                extendedXmpTextBox.WrapMode = WrapMode.None;
            }
        }

        private static string GetFileSizeString(long fileSize)
        {
            const long OneKilobyte = 1024;
            const long OneMegabyte = 1024 * 1024;
            const long OneGigabyte = 1024 * 1024 * 1024;

            double bytesDouble;
            string suffix;

            if (fileSize >= OneGigabyte)
            {
                bytesDouble = (double)fileSize / OneGigabyte;
                suffix = "GB";
            }
            else if (fileSize >= OneMegabyte)
            {
                bytesDouble = (double)fileSize / OneMegabyte;
                suffix = "MB";
            }
            else if (fileSize >= OneKilobyte)
            {
                bytesDouble = (double)fileSize / OneKilobyte;
                suffix = "KB";
            }
            else
            {
                bytesDouble = fileSize;
                suffix = "B";
            }

            return string.Format(CultureInfo.CurrentCulture, "{0:0.###} {1}", bytesDouble, suffix);
        }

        private sealed class WorkerResult
        {
            public WorkerResult(string path, JpegXmpPackets xmpPackets)
            {
                Path = path;
                XmpPackets = xmpPackets;
            }

            public string Path { get; }

            public JpegXmpPackets XmpPackets { get; }
        }
    }
}
