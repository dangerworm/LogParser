using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Schema;

namespace IISLogViewer
{
    public partial class Main : Form
    {
        private const string ControlNameFormatString = "ctlConstraint{0}";

        private Dictionary<int, Constraint> _constraints;
        private List<string> _fields;
        private List<string[]> _rows;

        private bool _hasDateField;
        private bool _hasTimeField;
        private int _dateFieldIndex;
        private int _timeFieldIndex;

        private int comboBoxLeft = 80;
        private int textBoxLeft = 207;
        private int spacing = 27;

        public Main()
        {
            InitializeComponent();

            _constraints = new Dictionary<int, Constraint>();
            _hasDateField = false;
            _hasTimeField = false;
            _dateFieldIndex = -1;
            _timeFieldIndex = -1;

            dtpFrom.Value = DateTime.Today;
            dtpTo.Value = DateTime.Now;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            if (ofdFilename.ShowDialog(this) != DialogResult.Cancel && ofdFilename.FileNames.Length > 0)
            {
                tslStatus.Text = ofdFilename.FileNames.Length == 1 
                    ? $"Loaded file {ofdFilename.FileName}" 
                    : $"Loaded {ofdFilename.FileNames.Length} files";
            }

            try
            {
                var minDate = DateTime.MaxValue;
                var maxDate = DateTime.MinValue;
                foreach (var fileName in ofdFilename.FileNames)
                {
                    var yymmdd = Path.GetFileName(fileName ?? "").Substring(4, 6);
                    var logDate = DateTime.ParseExact(yymmdd, "yyMMdd", CultureInfo.InvariantCulture);

                    if (logDate < minDate)
                    {
                        minDate = logDate.Date;
                        continue;
                    }

                    if (logDate > maxDate)
                    {
                        maxDate = logDate.Date;
                    }
                }

                dtpFrom.Value = minDate;
                dtpTo.Value = maxDate + new TimeSpan(23, 59, 59);
            }
            catch (Exception)
            {
            }

            LoadFields();
        }

        private void btnProcessFile_Click(object sender, EventArgs e)
        {
            ProcessFile();
            WriteRows();
        }

        private void LoadFields()
        {
            if (_fields?.Count > 0)
            {
                UnloadFields();
            }

            using (var input = new StreamReader(new FileStream(ofdFilename.FileName, FileMode.Open)))
            {
                // Useless
                input.ReadLine(); // Software
                input.ReadLine(); // Version
                input.ReadLine(); // Date

                _fields = input.ReadLine()?.Split(' ').ToList(); // Fields
                if (_fields == null)
                {
                    return;
                }
                _fields.RemoveAt(0); // Remove "#Fields" string from list

                if (_fields.Contains("date"))
                {
                    _hasDateField = true;
                    _dateFieldIndex = _fields.IndexOf("date");
                }

                if (_fields.Contains("time"))
                {
                    _hasTimeField = true;
                    _timeFieldIndex = _fields.IndexOf("time");
                }

                AddConstraintInput();
            }
        }

        private void AddConstraintInput()
        {
            var index = _constraints.Count;

            var controlName = string.Format(ControlNameFormatString, index);
            var nonDateTimeFields = _fields.Where(f => _fields.IndexOf(f) != _dateFieldIndex &&
                                                       _fields.IndexOf(f) != _timeFieldIndex)
                .Select(f => (object)f)
                .ToArray();

            var comboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(comboBoxLeft, index * spacing + 71),
                Name = controlName,
                Size = new Size(121, 21),
                Tag = index
            };
            comboBox.Items.Add("No Filter");
            comboBox.Items.AddRange(nonDateTimeFields);
            comboBox.SelectedIndex = 0;
            comboBox.SelectedIndexChanged += UpdateConstraintField;
            Controls.Add(comboBox);

            var textBox = new TextBox
            {
                Size = new Size(277, 21),
                Location = new Point(textBoxLeft, index * spacing + 71),
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
            var fieldIndex = _fields.IndexOf(comboBox.SelectedItem.ToString());
            if (!_constraints.ContainsKey(index))
            {
                _constraints.Add(index, new Constraint { FieldIndex = fieldIndex });
            }
            else
            {
                _constraints[index].FieldIndex = fieldIndex;
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

            tslStatus.Text = $"Found {tspProgress.Maximum.ToString("N0")} log entries. Processing...";
            Application.DoEvents();

            _rows = new List<string[]>();
            foreach (var filename in ofdFilename.FileNames)
            {
                using (var input = new StreamReader(new FileStream(filename, FileMode.Open)))
                {
                    // Useless
                    input.ReadLine(); // Software
                    input.ReadLine(); // Version
                    input.ReadLine(); // Date
                    input.ReadLine(); // Fields

                    string line;
                    while (!string.IsNullOrEmpty(line = input.ReadLine()))
                    {
                        var row = line.Split(' ');

                        var rowDate = DateTime.MinValue;
                        if (_hasDateField)
                        {
                            rowDate = DateTime.Parse(row[_dateFieldIndex]);

                            if (_hasTimeField)
                            {
                                rowDate += TimeSpan.Parse(row[_timeFieldIndex]);
                            }
                        }

                        var inDateBand = true;
                        if (_hasDateField && rowDate > DateTime.MinValue)
                        {
                            inDateBand = rowDate >= dtpFrom.Value && rowDate <= dtpTo.Value;
                        }

                        var isMatch = true;
                        foreach (var constraint in _constraints.Values)
                        {
                            isMatch = row[constraint.FieldIndex].ToLower().Contains(constraint.Value.ToLower());
                            if (!isMatch)
                            {
                                break;
                            }
                        }

                        if (inDateBand && isMatch)
                        {
                            _rows.Add(row.Select(field => field.Contains(",") ? $"\"{field}\"" : $"{field}").ToArray());
                        }

                        tspProgress.Value++;

                        Application.DoEvents();
                    }
                }
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
                output.WriteLine(string.Join(",", _fields));

                tspProgress.Value = 0;
                tspProgress.Maximum = _rows.Count;
                tspProgress.Visible = true;

                foreach (var row in _rows)
                {
                    output.WriteLine(string.Join(",", row));

                    tspProgress.Value++;

                    Application.DoEvents();
                }
                tspProgress.Visible = false;
            }

            tslStatus.Text = $"Wrote new file {newPath}";
        }
    }
}

