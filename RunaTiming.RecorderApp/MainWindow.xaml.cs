using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using RunaTiming.Csv;
using RunaTiming.Shared.Upload;

namespace RunaTiming.RecorderApp
{
    public partial class MainWindow : Window
    {
        private bool isRunning = false;
        private readonly string _serviceUrl;
        private readonly string _csvFolderPath;

        protected override void OnClosing(CancelEventArgs e)
        {
            isRunning = false;
            base.OnClosing(e);
        }

        public MainWindow()
        {
            InitializeComponent();
            _serviceUrl = ConfigurationManager.AppSettings["RunaTiming_ServiceUrl"] ?? string.Empty;
            _csvFolderPath = ConfigurationManager.AppSettings["RunaTiming_CsvFolderPath"] ?? string.Empty;
            ValidateAppSettings();
        }

        private void ValidateAppSettings()
        {
            if (string.IsNullOrEmpty(_serviceUrl))
            {
                MessageBox.Show("RunaTiming_ServiceUrl is missing or empty. Closing application");
                Close();
                return;
            }

            if (string.IsNullOrEmpty(_csvFolderPath))
            {
                MessageBox.Show("RunaTiming_CsvFolderPath is missing or empty. Closing application");
                Close();
                return;
            }

            if (!Directory.Exists(_csvFolderPath))
            {
                MessageBox.Show($"\"{_csvFolderPath}\" is not a valid directory. Closing application");
                Close();
                return;
            }
        }

        private async void StartCaptureButton_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
            {
                isRunning = false;
                StartCaptureButton.Content = "Start";
                WriteStatus("Capture service stopped");
                return;
            }

            isRunning = true;
            StartCaptureButton.Content = "Stop";
            StatusTextBox.Text = string.Empty;
            WriteStatus($"Service URL:      {_serviceUrl}");
            WriteStatus($"CSV directory:    {_csvFolderPath}");
            WriteStatus("---");
            await CheckCsvFiles();
        }

        private async Task CheckCsvFiles()
        {
            WriteStatus("Starting capture service");
            var checkedFiles = new HashSet<string>();
            var newestRecords = new Dictionary<int, CsvTimingFile>();

            while (isRunning)
            {
                var csvFiles = Directory.GetFiles(_csvFolderPath, "*.csv", SearchOption.TopDirectoryOnly);
                var recordsToUpload = new Dictionary<int, CsvTimingFile>();

                foreach (var csvFile in csvFiles)
                {
                    if (checkedFiles.Contains(csvFile))
                    {
                        continue;
                    }

                    checkedFiles.Add(csvFile);

                    try
                    {
                        var fileContent = CsvTimingHelper.ParseFile(csvFile);

                        WriteStatus($"Checking file: {Path.GetFileName(csvFile)}");

                        foreach (var item in fileContent)
                        {
                            if (!newestRecords.ContainsKey(item.Bib) || item.IsNewerThan(newestRecords[item.Bib]))
                            {
                                newestRecords[item.Bib] = item;
                                recordsToUpload[item.Bib] = item;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteException(ex);
                    }
                }

                if (recordsToUpload.Count > 0)
                {
                    await UploadResults(recordsToUpload.Values);
                }
                else
                {
                    await Task.Delay(1000);
                }
            }
        }

        private async Task UploadResults(IEnumerable<CsvTimingFile> values)
        {
            WriteStatus("Uploading results");
            var uploadStatus = false;

            try
            {
                var valuesToUpload = values
                    .Select(CsvTimingHelper.ConvertToResultItem)
                    .ToList();

                uploadStatus = await UploadService.UploadResults(_serviceUrl, valuesToUpload);

                foreach (var item in valuesToUpload)
                {
                    WriteStatus(
                        $"#{item.Bib} {item.FirstName} {item.LastName} {item.Sex} -  {item.StartTime} | {item.ChipStartTime} | {string.Join(", ", item.Splits)} | {item.FinishingTime}");
                }
            }
            catch (Exception ex)
            {
                WriteException(ex);
            }

            if (uploadStatus == false)
            {
                WriteStatus("ERROR: Failed to upload results");
            }

            WriteStatus("---");
        }

        private void WriteStatus(string text)
        {
            Application.Current.Dispatcher.BeginInvoke(
                () => { StatusTextBox.Text += $"{text}\r\n"; });
        }

        private void WriteException(Exception exception)
        {
            WriteStatus("---");
            WriteStatus($"ERROR: {exception.Message}");
            WriteStatus(exception.ToString());
            WriteStatus("---");
        }
    }
}