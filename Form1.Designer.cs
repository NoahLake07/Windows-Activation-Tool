using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Security.Principal;
using System.Windows.Forms;

namespace WindowsActivationService
{
    public class MainForm : Form
    {
        private Button activateButton;

        // Dictionary to store version names and corresponding codes
        private Dictionary<string, string> versionCodes = new Dictionary<string, string>
        {
            {"HOME", "TX9XD-98N7V-6WMQ6-BX7FG-H8Q99"},
            {"HOME_N", "3KHY7-WNT83-DGQKR-F7HPR-844BM"},
            {"HOME_SINGLE_LANGUAGE", "7HNRX-D7KGG-3K4RQ-4WPJ4-YTDFH"},
            {"HOME_COUNTRY_SPECIFIC", "PVMJN-6DFY6-9CCP6-7BKTT-D3WVR"},
            {"PROFESSIONAL", "W269N-WFGWX-YVC9B-4J6C9-T83GX"},
            {"PROFESSIONAL_N", "MH37W-N47XK-V7XM9-C7227-GCQG9"},
            {"EDUCATION", "NW6C2-QMPVW-D7KKK-3GKT6-VCFB2"},
            {"EDUCATION_N", "2WH4N-8QGBV-H22JP-CT43Q-MDWWJ"},
            {"ENTERPRISE", "NPPR9-FWDCX-D2C8J-H872K-2YT43"},
            {"ENTERPRISE_N", "DPH2V-TTNVB-4X9Q3-TJR4H-KHJW4"},
        };

        public MainForm()
        {
            Text = "Windows Activation Tool";
            Size = new System.Drawing.Size(400, 200);
            FormBorderStyle = FormBorderStyle.FixedSingle; // Fixed single border

            // Create and configure the header label
            Label headerLabel = new Label
            {
                Text = "Windows Activation Tool",
                Font = new System.Drawing.Font("Arial", 16, System.Drawing.FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 40
            };

            // Create and configure the activateButton
            activateButton = new Button
            {
                Text = "Activate Windows",
                Size = new System.Drawing.Size(150, 40),
                Location = new System.Drawing.Point((Width - 150) / 2, 80)
            };
            activateButton.Click += ActivateButton_Click;

            // Add controls to the form
            Controls.Add(headerLabel);
            Controls.Add(activateButton);
        }

        private void ActivateButton_Click(object sender, EventArgs e)
        {
            // Display a combo box to select the Windows version
            string selectedVersion = SelectVersion();

            // Validate if version is selected
            if (!string.IsNullOrEmpty(selectedVersion))
            {
                // Get the version code from the dictionary
                if (versionCodes.TryGetValue(selectedVersion, out string versionCode))
                {
                    // Check if the application is running as an administrator
                    if (IsAdministrator())
                    {
                        // Execute commands in the command prompt
                        ExecuteCommand($"slmgr /ipk \"{versionCode}\"");
                        ExecuteCommand("slmgr /skms kms8.msguides.com");
                        ExecuteCommand("slmgr /ato");
                    }
                    else
                    {
                        MessageBox.Show("Please run the application as administrator.");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid version selection.");
                }
            }
        }

        private string SelectVersion()
        {
            // Display a combo box with version options
            Form versionForm = new Form()
            {
                Width = 300,
                Height = 150,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                Text = "Select Windows Version",
                StartPosition = FormStartPosition.CenterScreen
            };

            ComboBox versionComboBox = new ComboBox() { Left = 50, Top = 20, Width = 200 };
            versionComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            // Add versions to the combo box
            foreach (var version in versionCodes.Keys)
            {
                versionComboBox.Items.Add(version);
            }

            Button confirmationButton = new Button() { Text = "Go", Left = 120, Width = 60, Top = 70, DialogResult = DialogResult.OK };
            confirmationButton.Click += (sender, e) => { versionForm.Close(); };
            versionForm.Controls.Add(versionComboBox);
            versionForm.Controls.Add(confirmationButton);
            versionForm.AcceptButton = confirmationButton;

            return versionForm.ShowDialog() == DialogResult.OK ? versionComboBox.SelectedItem.ToString() : "";
        }

        private bool IsAdministrator()
        {
            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private void ExecuteCommand(string command)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe")
            {
                UseShellExecute = true,
                Verb = "runas", // Run as administrator
                Arguments = $"/c {command}" // Pass the command to the command prompt
            };

            try
            {
                // Start the process
                Process.Start(processInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error executing command: {ex.Message}");
            }
        }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
