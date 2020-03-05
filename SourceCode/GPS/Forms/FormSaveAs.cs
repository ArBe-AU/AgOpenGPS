﻿using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AgOpenGPS
{
    public partial class FormSaveAs : Form
    {
        //class variables
        private readonly FormGPS mf;

        public FormSaveAs(Form _callingForm)
        {
            mf = _callingForm as FormGPS;

            InitializeComponent();

            label1.Text = gStr.gsEnterFieldName;
            label2.Text = gStr.gsDateWillBeAdded;
            label3.Text = gStr.gsBasedOnField;
            label4.Text = gStr.gsEnterTask;
            label5.Text = gStr.gsEnterVehicleUsed;

            this.Text = gStr.gsSaveAs;
            lblTemplateChosen.Text = gStr.gsNoneUsed;
        }

        private void FormSaveAs_Load(object sender, EventArgs e)
        {
            btnSave.Enabled = false;
            lblTemplateChosen.Text = Properties.Settings.Default.setF_CurrentDir;
            //tboxVehicle.Text = mf.vehicleFileName + " " + mf.toolFileName;
            lblFilename.Text = "";
            //isTemplateSet = true;
        }

        private void TboxFieldName_TextChanged(object sender, EventArgs e)
        {
            var textboxSender = (TextBox)sender;
            var cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, "[^0-9a-zA-Z {Ll}{Lt}]", "");
            textboxSender.SelectionStart = cursorPosition;

            if (String.IsNullOrEmpty(tboxFieldName.Text.Trim()))
            {
                btnSave.Enabled = false;
            }
            else
            {
                btnSave.Enabled = true;
            }

            lblFilename.Text = tboxFieldName.Text.Trim() + "_" + tboxTask.Text.Trim()
                + "_" + tboxVehicle.Text.Trim() + "_" + DateTime.Now.ToString("yyyy.MMM.dd HH_mm", CultureInfo.InvariantCulture);
        }

        private void TboxTask_TextChanged(object sender, EventArgs e)
        {
            var textboxSender = (TextBox)sender;
            var cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, "[^0-9a-zA-Z {Ll}{Lt}]", "");
            textboxSender.SelectionStart = cursorPosition;

            lblFilename.Text = tboxFieldName.Text.Trim() + "_" + tboxTask.Text.Trim() 
                + "_" + tboxVehicle.Text.Trim() + "_" + DateTime.Now.ToString("yyyy.MMM.dd HH_mm", CultureInfo.InvariantCulture);
        }

        private void TboxVehicle_TextChanged(object sender, EventArgs e)
        {
            var textboxSender = (TextBox)sender;
            var cursorPosition = textboxSender.SelectionStart;
            textboxSender.Text = Regex.Replace(textboxSender.Text, "[^0-9a-zA-Z {Ll}{Lt}]", "");
            textboxSender.SelectionStart = cursorPosition;

            lblFilename.Text = tboxFieldName.Text.Trim() + "_" + tboxTask.Text.Trim()
                + "_" + tboxVehicle.Text.Trim() + "_" + DateTime.Now.ToString("yyyy.MMM.dd HH_mm", CultureInfo.InvariantCulture);
        }

        private void BtnSerialCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            //fill something in
            if (String.IsNullOrEmpty(tboxFieldName.Text.Trim()))
            {
                Close();
                return;
            }

            //append date time to name

            mf.currentFieldDirectory = tboxFieldName.Text.Trim() + "_";

            //task
            if (!String.IsNullOrEmpty(tboxTask.Text.Trim())) mf.currentFieldDirectory += tboxTask.Text.Trim() + "_";

            //vehicle
            if (!String.IsNullOrEmpty(tboxVehicle.Text.Trim())) mf.currentFieldDirectory += tboxVehicle.Text.Trim() + "_";

            //date
            mf.currentFieldDirectory += String.Format("{0}", DateTime.Now.ToString("yyyy.MMM.dd HH_mm", CultureInfo.InvariantCulture));

            //get the directory and make sure it exists, create if not
            string dirNewField = mf.fieldsDirectory + mf.currentFieldDirectory + "\\";

            mf.menustripLanguage.Enabled = false;
            //if no template set just make a new file.
            //if (!isTemplateSet)
            //{
            //    try
            //    {
            //        //start a new job
            //        mf.JobNew();

            //        //create it for first save
            //        string directoryName = Path.GetDirectoryName(dirNewField);

            //        if ((!string.IsNullOrEmpty(directoryName)) && (Directory.Exists(directoryName)))
            //        {
            //            MessageBox.Show(gStr.gsChooseADifferentName, gStr.gsDirectoryExists, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            //            return;
            //        }
            //        else
            //        {
            //            //reset the offsets
            //            mf.pn.utmEast = (int)mf.pn.actualEasting;
            //            mf.pn.utmNorth = (int)mf.pn.actualNorthing;

            //            mf.worldGrid.CreateWorldGrid(0, 0);

            //            //calculate the central meridian of current zone
            //            mf.pn.centralMeridian = -177 + ((mf.pn.zone - 1) * 6);

            //            //Azimuth Error - utm declination
            //            mf.pn.convergenceAngle = Math.Atan(Math.Sin(glm.toRadians(mf.pn.latitude))
            //                                        * Math.Tan(glm.toRadians(mf.pn.longitude - mf.pn.centralMeridian)));
            //            mf.lblConvergenceAngle.Text = Math.Round(glm.toDegrees(mf.pn.convergenceAngle), 3).ToString();

            //            //make sure directory exists, or create it
            //            if ((!string.IsNullOrEmpty(directoryName)) && (!Directory.Exists(directoryName)))
            //            { Directory.CreateDirectory(directoryName); }

            //            //create the field file header info
            //            mf.FileCreateField();
            //            mf.FileCreateSections();
            //            mf.FileCreateRecPath();
            //            mf.FileCreateContour();
            //            mf.FileCreateElevation();
            //            mf.FileSaveFlags();
            //            //mf.FileSaveABLine();
            //            //mf.FileSaveCurveLine();
            //            //mf.FileSaveHeadland();
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        mf.WriteErrorLog("Creating new field " + ex);

            //        MessageBox.Show(gStr.gsError, ex.ToString());
            //        mf.currentFieldDirectory = "";
            //    }
            //}
            //else
            {
                // create from template
                string directoryName = Path.GetDirectoryName(dirNewField);

                if ((!string.IsNullOrEmpty(directoryName)) && (Directory.Exists(directoryName)))
                {
                    MessageBox.Show(gStr.gsChooseADifferentName, gStr.gsDirectoryExists, MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }
                else
                {
                    //create the new directory
                    if ((!string.IsNullOrEmpty(directoryName)) && (!Directory.Exists(directoryName)))
                    { Directory.CreateDirectory(directoryName); }
                }

                string line;
                string offsets, convergence, startFix;

                using (StreamReader reader = new StreamReader(mf.fieldsDirectory + lblTemplateChosen.Text + "\\Field.txt"))
                {
                    try
                    {
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();
                        line = reader.ReadLine();

                        //read the Offsets  - all we really need from template field file
                        offsets = reader.ReadLine();

                        line = reader.ReadLine();
                        convergence = reader.ReadLine();

                        line = reader.ReadLine();
                        startFix = reader.ReadLine();
                    }
                    catch (Exception ex)
                    {
                        mf.WriteErrorLog("While Opening Field" + ex);

                        mf.TimedMessageBox(2000, gStr.gsFieldFileIsCorrupt, gStr.gsChooseADifferentField);
                        mf.JobClose();
                        return;
                    }

                    const string myFileName = "Field.txt";

                    using (StreamWriter writer = new StreamWriter(dirNewField + myFileName))
                    {
                        //Write out the date
                        writer.WriteLine(DateTime.Now.ToString("yyyy-MMMM-dd hh:mm:ss tt", CultureInfo.InvariantCulture));

                        writer.WriteLine("$FieldDir");
                        writer.WriteLine(mf.currentFieldDirectory.ToString(CultureInfo.InvariantCulture));

                        //write out the easting and northing Offsets
                        writer.WriteLine("$Offsets");
                        writer.WriteLine(offsets);

                        writer.WriteLine("$Convergence");
                        writer.WriteLine(convergence);

                        writer.WriteLine("StartFix");
                        writer.WriteLine(startFix);
                    }

                    //create blank Contour and Section files
                    mf.FileCreateSections();
                    mf.FileCreateContour();
                    //mf.FileCreateElevation();

                    //copy over the files from template
                    string templateDirectoryName = (mf.fieldsDirectory + lblTemplateChosen.Text);

                    string fileToCopy = templateDirectoryName + "\\Boundary.txt";
                    string destinationDirectory = directoryName + "\\Boundary.txt";
                    if (File.Exists(fileToCopy))
                        File.Copy(fileToCopy, destinationDirectory);

                    fileToCopy = templateDirectoryName + "\\Elevation.txt";
                    destinationDirectory = directoryName + "\\Elevation.txt";
                    if (File.Exists(fileToCopy))
                        File.Copy(fileToCopy, destinationDirectory);

                    fileToCopy = templateDirectoryName + "\\Flags.txt";
                    destinationDirectory = directoryName + "\\Flags.txt";
                    if (File.Exists(fileToCopy))
                        File.Copy(fileToCopy, destinationDirectory);

                    fileToCopy = templateDirectoryName + "\\ABLines.txt";
                    destinationDirectory = directoryName + "\\ABLines.txt";
                    if (File.Exists(fileToCopy))
                        File.Copy(fileToCopy, destinationDirectory);

                    fileToCopy = templateDirectoryName + "\\RecPath.txt";
                    destinationDirectory = directoryName + "\\RecPath.txt";
                    if (File.Exists(fileToCopy))
                        File.Copy(fileToCopy, destinationDirectory);

                    fileToCopy = templateDirectoryName + "\\CurveLines.txt";
                    destinationDirectory = directoryName + "\\CurveLines.txt";
                    if (File.Exists(fileToCopy))
                        File.Copy(fileToCopy, destinationDirectory);

                    //now open the newly cloned field
                    mf.FileOpenField(dirNewField + myFileName);
                    mf.Text = "AgOpenGPS - " + mf.currentFieldDirectory;

                }
            }

            DialogResult = DialogResult.OK;
            Close();
        }

    }
}