using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace LogParser
{
    public partial class Main : Form
    {
        private const string ControlNameFormatString = "ctlConstraint{0}";
        private const int ComboBoxLeft = 80;
        private const int TextBoxLeft = 207;
        private const int Spacing = 27;

        private readonly Dictionary<int, Constraint> _constraints;
        private readonly LogFileParser _fileParser;

        private List<LogFileField> _fields;
        private List<string[]> _outputLines;

        public Main()
        {
            InitializeComponent();

            _constraints = new Dictionary<int, Constraint>();
            _fields = new List<LogFileField>();
            _fileParser = new LogFileParser();

            _fileParser.FileLineProcessed += FileLineProcessed;

            dtpFrom.Value = DateTime.Today;
            dtpTo.Value = DateTime.Now;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            var openFileDialogResult = ofdFilename.ShowDialog(this);
            if (openFileDialogResult == DialogResult.Cancel ||
                string.IsNullOrWhiteSpace(ofdFilename.FileName) ||
                ofdFilename.FileNames.Length == 0)
            {
                return;
            }

            GetDateRange();
            LoadFields();

            tslStatus.Text = ofdFilename.FileNames.Length == 1
                ? $"Loaded file {ofdFilename.FileName}."
                : $"Loaded {ofdFilename.FileNames.Length} files.";
        }

        private void btnProcessFile_Click(object sender, EventArgs e)
        {
            ProcessFile();
            WriteRows();
        }

        private void GetDateRange()
        {
            var dateRange = LogFileParser.GetDateRange(ofdFilename.FileNames);
            dtpFrom.Value = dateRange.GetMinDateTime();
            dtpTo.Value = dateRange.GetMaxDateTime();
        }

        private void LoadFields()
        {
            if (string.IsNullOrWhiteSpace(ofdFilename.FileName))
            {
                return;
            }

            if (_fields?.Count > 0)
            {
                UnloadFields();
            }

            _fields = _fileParser.GetFields(ofdFilename.FileName);

            AddConstraintInput();
        }

        private void AddConstraintInput()
        {
            var index = _constraints.Count;

            var controlName = string.Format(ControlNameFormatString, index);
            var nonDateTimeFields = _fields
                .Where(f => !f.IsDateField && !f.IsTimeField)
                .ToArray();

            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(ComboBoxLeft, index * Spacing + 71),
                Name = controlName,
                Size = new Size(121, 21),
                Tag = index
            };

            comboBox.Items.Add("No Filter");
            comboBox.Items.AddRange(nonDateTimeFields.Select(f => (object)f.Name).ToArray());
            comboBox.SelectedIndex = 0;
            comboBox.SelectedIndexChanged += UpdateConstraintField;
            Controls.Add(comboBox);

            var textBox = new TextBox
            {
                Size = new Size(277, 21),
                Location = new Point(TextBoxLeft, index * Spacing + 71),
                Name = controlName,
                Tag = index
            };
            textBox.TextChanged += UpdateConstraintValue;
            Controls.Add(textBox);
        }

        private void RemoveConstraintInput(int index)
        {
            var controls = Controls.Find(string.Format(ControlNameFormatString, index), false);

            foreach (var control in controls)
            {
                Controls.Remove(control);
            }

            _constraints.Remove(index);
        }

        private void UpdateConstraintField(object sender, EventArgs eventArgs)
        {
            var comboBox = sender as ComboBox;
            var tag = comboBox?.Tag.ToString();
            if (string.IsNullOrWhiteSpace(tag) || string.IsNullOrWhiteSpace(comboBox.SelectedItem.ToString()))
            {
                return;
            }

            var index = int.Parse(tag);
            var field = _fields.FirstOrDefault(x => x.Name.Equals(comboBox.SelectedItem));
            if (field == null)
            {
                return;
            }

            if (!_constraints.ContainsKey(index))
            {
                _constraints.Add(index, new Constraint { FieldIndex = field.Index });
            }
            else
            {
                _constraints[index].FieldIndex = field.Index;
            }
        }

        private void UpdateConstraintValue(object sender, EventArgs eventArgs)
        {
            var textBox = sender as TextBox;
            var tag = textBox?.Tag.ToString();
            if (string.IsNullOrWhiteSpace(tag))
            {
                return;
            }
            var index = int.Parse(tag);
            if (string.IsNullOrWhiteSpace(textBox.Text) && index < _constraints.Count - 1)
            {
                RemoveConstraintInput(_constraints.Count - 1);
                return;
            }

            if (!_constraints.ContainsKey(index))
            {
                _constraints.Add(index, new Constraint { Value = textBox.Text });
            }
            else
            {
                _constraints[index].Value = textBox.Text;
            }

            if (Controls.Find(string.Format(ControlNameFormatString, index + 1), false).Length == 0)
            {
                AddConstraintInput();
            }
        }

        private void UnloadFields()
        {
            var controlsEnumerator = Controls.GetEnumerator();
            while (true)
            {
                Control control;
                try
                {
                    control = controlsEnumerator.Current as Control;
                }
                catch (ArgumentOutOfRangeException)
                {
                    break;
                }


                if (!string.IsNullOrWhiteSpace(control?.Tag?.ToString()))
                {
                    Controls.Remove(control);
                }
                else
                {
                    if (!controlsEnumerator.MoveNext())
                    {
                        break;
                    }
                }

                Application.DoEvents();
            }

            _constraints.Clear();
        }

        private void ProcessFile()
        {
            tslStatus.Text = @"Pre-processing...";
            Application.DoEvents();

            tspProgress.Value = 0;
            tspProgress.Maximum = ofdFilename.FileNames.Sum(filename => File.ReadLines(filename).Count());
            tspProgress.Visible = true;

            tslStatus.Text = $@"Found {tspProgress.Maximum:N0} log entries. Processing...";
            Application.DoEvents();

            _outputLines = new List<string[]>();
            var dateRange = new DateRange(dtpFrom.Value, dtpTo.Value);
            foreach (var filename in ofdFilename.FileNames)
            {
                _outputLines = _fileParser.ProcessFile(filename, dateRange, _constraints.Values);
            }

            tspProgress.Visible = false;
        }

        private void WriteRows()
        {
            if (string.IsNullOrWhiteSpace(ofdFilename.FileName))
            {
                MessageBox.Show(@"You must select a file to process before you can continue.", @"Error");
                return;
            }

            var newFilename = $"{Path.GetFileNameWithoutExtension(ofdFilename.FileName)}-processed.csv";
            var newPath = Path.Combine(Path.GetDirectoryName(ofdFilename.FileName) ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop), newFilename);

            if (File.Exists(newPath))
            {
                try
                {
                    File.Delete(newPath);
                }
                catch (IOException ioe)
                {
                    MessageBox.Show(ioe.Message, @"Cannot access file.");
                    return;
                }
            }

            using (var output = new StreamWriter(new FileStream(newPath, FileMode.CreateNew)))
            {
                output.WriteLine(string.Join(",", _fields.Select(f => f.Name)));

                tspProgress.Value = 0;
                tspProgress.Maximum = _outputLines.Count;
                tspProgress.Visible = true;

                foreach (var row in _outputLines)
                {
                    output.WriteLine(string.Join(",", row));

                    tspProgress.Value++;

                    Application.DoEvents();
                }
                tspProgress.Visible = false;
            }

            tslStatus.Text = $@"Wrote new file {newPath}";
        }

        private void FileLineProcessed(object sender, EventArgs e)
        {
            tspProgress.Value++;

            Application.DoEvents();
        }
    }
}

