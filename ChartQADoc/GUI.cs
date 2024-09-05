using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Diagnostics;


namespace ChartQADoc
{
    public partial class GUI : Form
    {
        public GUI(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, IEnumerable<VMS.TPS.Common.Model.API.PlanSetup> plans, VMS.TPS.Common.Model.API.User user)
        {
            List<VMS.TPS.Common.Model.API.PlanSetup> planlist = (List<VMS.TPS.Common.Model.API.PlanSetup>)plans;
            InitializeComponent();

            YesBut.Visible = true;
            NoBut.Visible = true;

            //yes to run on entire course, no to run on one plan.
            YesBut.Click += (sender, EventArgs) => { CourseExecute(patient, course, user); };
            NoBut.Click += (sender, EventArgs) => { PlanExecute(patient, course, planlist, user); };
        }

        private void CourseExecute(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.User user)
        {
            ExecuteCourse(patient, course, user);
        }

        private void PlanExecute(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, List<VMS.TPS.Common.Model.API.PlanSetup> planlist, VMS.TPS.Common.Model.API.User user)
        {
            Prompt2Label.Text = "Please choose one of the plans from the open course to run ChartQADoc on.";
            YesBut.Visible = false;
            NoBut.Visible = false;
            foreach (VMS.TPS.Common.Model.API.PlanSetup pl in planlist)
            {
                PlanListBox.Items.Add(pl.Id);
            }
            PlanListBox.Visible = true;
            ExecBut.Visible = true;
            ExecBut.Click += (sender, EventArgs) => { ExecButPreExecute(patient, course, planlist, user); };
        }

        private void ExecButPreExecute(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, List<VMS.TPS.Common.Model.API.PlanSetup> planlist, VMS.TPS.Common.Model.API.User user)
        {
            if (PlanListBox.SelectedItem.ToString() == null || PlanListBox.SelectedItem.ToString() == "")
            {
                MessageBox.Show("You must select a plan from the ListBox to run the program on.");
            }
            else
            {
                List<VMS.TPS.Common.Model.API.PlanSetup> selectedplans = planlist.Where(pl => pl.Id.Equals(PlanListBox.SelectedItem.ToString())).ToList();  //should be a list of one
                VMS.TPS.Common.Model.API.PlanSetup plan = selectedplans.First();
                Execute(patient, course, plan, user);
            }
        }

        private void ExecuteCourse(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.User user)
        {
            try
            {
                progressBar1.Visible = true;

                string patientID = patient.Id;
                string courseID = course.Id;
                string userID = user.Id; 
                List<long> planSetupSerList = new List<long>();
                long temp = 0;

                List<long> Beams = new List<long>(); //Called "Radiations" in the database
                List<ChartQA> chartQAList = new List<ChartQA>();

                string ConnectionString = @"Data Source=WVARIASDBP01SS;Initial Catalog=VARIAN;Integrated Security=true;";
                //@"Data Source=WVARIASDBP01SS;Initial Catalog=VARIAN;Integrated Security=true;";

                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand command;
                SqlDataReader datareader;
                string sql;

                //MessageBox.Show("PatientId: " + patientID);

                conn.Open();
                sql = "USE VARIAN SELECT PatientSer FROM dbo.Patient WHERE PatientId = '" + patientID + "'"; //This just queries the patient table to get the patient serial number;
                command = new SqlCommand(sql, conn);
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    temp = Convert.ToInt64(datareader["PatientSer"]);
                }
                conn.Close();

                long patientSer = temp;
                //MessageBox.Show("PatientSer: " + patientSer);

                conn.Open();
                sql = "USE VARIAN SELECT CourseSer FROM dbo.Course WHERE CourseId = '" + courseID + "' AND PatientSer = " + patientSer; //This just queries the course table to get the course serial number;
                command = new SqlCommand(sql, conn);
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    temp = (long)datareader["CourseSer"];
                }
                conn.Close();

                //MessageBox.Show("Trigger 3");
                long courseSer = temp;

                conn.Open();
                sql = "USE VARIAN SELECT PlanSetupSer FROM dbo.PlanSetup WHERE CourseSer = " + courseSer;  //This just queries the PlanSetup table to get the PlanSetup serial number;
                command = new SqlCommand(sql, conn);
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    planSetupSerList.Add((long)datareader["PlanSetupSer"]);
                }
                conn.Close();

                //MessageBox.Show("Trigger 4");

                //Then we query the "Radiation" table (i.e. Beams) to get all the RadiationSer for all the beams in the plan.
                //We need the RadiationSer of the beams in the plan to identify which ChartQA records are relevant to this plan.
                foreach (long pss in planSetupSerList)
                {
                    conn.Open();
                    sql = "USE VARIAN SELECT RadiationSer FROM dbo.Radiation WHERE PlanSetupSer = " + pss;
                    command = new SqlCommand(sql, conn);
                    datareader = command.ExecuteReader();

                    while (datareader.Read())
                    {
                        Beams.Add((long)datareader["RadiationSer"]);
                    }
                    conn.Close();
                }

                //MessageBox.Show("Trigger 5");
                //Now to query ChartQA
                conn.Open();
                sql = "USE VARIAN SELECT ChartQASer, ChartQADateTime, ChartQABy, Comment  FROM dbo.ChartQA WHERE PatientSer = " + patientSer;
                command = new SqlCommand(sql, conn);
                datareader = command.ExecuteReader();

                //The list made below is for ALL the ChartQA entries associated with this patient, for ALL their plans in ALL their course.
                //We need to do the leg work to determine which ones apply to the plan we are interested in.
                while (datareader.Read())
                {
                    chartQAList.Add(new ChartQA { ChartQASer = (long)datareader["ChartQASer"], user = (datareader["ChartQABy"] as string), dateTime = Convert.ToDateTime(datareader["ChartQADateTime"]), comment = (datareader["Comment"] as string) });
                }
                conn.Close();

                //MessageBox.Show("Trigger 6");

                foreach (ChartQA CQ in chartQAList)
                {
                    //MessageBox.Show("User: " + CQ.user + "\nDateTime: " + CQ.dateTime.ToString() + "\nComment: " + CQ.comment + "\nChartQASer: " + CQ.ChartQASer);

                    conn.Open();
                    sql = "USE VARIAN SELECT RadiationHstrySer FROM dbo.ChartQATreatment WHERE ChartQASer = " + CQ.ChartQASer;
                    command = new SqlCommand(sql, conn);
                    datareader = command.ExecuteReader();

                    while (datareader.Read())
                    {
                        CQ.radiationHstrySerList.Add((long)datareader["RadiationHstrySer"]);
                    }
                    conn.Close();
                }

                //MessageBox.Show("Trigger 7");

                foreach (ChartQA CQ in chartQAList)
                {
                    foreach (long RH in CQ.radiationHstrySerList)
                    {
                        conn.Open();
                        sql = "USE VARIAN SELECT RadiationSer FROM dbo.RadiationHstry WHERE RadiationHstrySer = " + RH;
                        command = new SqlCommand(sql, conn);
                        datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            CQ.radiationSerList.Add((long)datareader["RadiationSer"]);
                        }
                        conn.Close();
                    }

                    //removes duplicates
                    CQ.radiationSerList = CQ.radiationSerList.Distinct().ToList();
                }

                //MessageBox.Show("Trigger 8");

                //So when it comes to matching the radiations (or beams) in the Beam list, from the plan that the program is running on, to the radiations in the ChartQA objects,
                //there is a problem regarding the imaging beams. The Beam list includes all the treatment and imaging beams that are in the plan, while the radiations in the 
                //ChartQA objects are based off actual treatment records, and therefore most likley will not include all the imaging beams, as it is standard treatment planning
                //practice to include the full set of imaging beams, however most likley the therapists are not using all of them during treatment.
                //Therefore, to successfully match these lists, we need to be generous and do so only using treatment Beams.

                //But, we don't have a surfire way of doing that, since there is nothing in the database tables to distinguish treatment beams from imaging beams, other than their names
                //which is not 100% reliable.

                //So, we have to be even more generous and simply include all ChartQA objects in our final list that do not have a radiation that does not appear in the beam list, which should be good enough.
                //Remember, we don't care about the radiations here, we are just trying to get rid of ChartQA elements that don't apply to the given plan, due to the way the database is organized.
                //Thus the below code just removes chartQAList elements as soon as a non-matching radiation is found, and breaks out of the loop to test the next chartQA object.

                List<long> chartQARemoveList = new List<long>();
                //string str = null;
                //MessageBox.Show("chartQAList size: " + chartQAList.Count);

                for (int i = 0; i < chartQAList.Count; i++)
                {
                    //str = null;
                    //foreach (long li in chartQAList[i].radiationSerList)
                    //{
                    //    str = str + ", " + li;
                    //}
                    //MessageBox.Show("chartQA " + i + " radiationSer list:  " + str);

                    for (int k = 0; k < chartQAList[i].radiationSerList.Count; k++)
                    {
                        if (Beams.Any(ra => ra.Equals(chartQAList[i].radiationSerList[k])))
                        {
                            //then that radiation is good
                        }
                        else
                        {
                            //MessageBox.Show("chartQA " + i + " removed!");
                            chartQARemoveList.Add(chartQAList[i].ChartQASer);
                            //chartQAList.RemoveAt(i);
                            //Note: using the RemoveAt above doesn't work because the List dynamically reizes itself.
                            //To work with indices like this we should have used a simple array.
                            break;
                        }
                    }
                }

                foreach (long RS in chartQARemoveList)
                {
                    chartQAList.RemoveAll(cq => cq.ChartQASer.Equals(RS));
                }

                //MessageBox.Show("ChartQAList Size: " + chartQAList.Count);
                //So then all the chartQA objects left are what I want to put in a table in a PDF.

                //foreach (ChartQA cq in chartQAList)
                //{
                //    MessageBox.Show("User: " + cq.user + "\nDateTime: " + cq.dateTime.ToString() + "\nComment: " + cq.comment);
                //}

                List<string> PatientInfo = new List<string>();
                PatientInfo.Add(patient.LastName);
                PatientInfo.Add(patient.FirstName);
                PatientInfo.Add(courseID);
                PatientInfo.Add("ALL PLANS IN COURSE");
                PatientInfo.Add(null);
                PatientInfo.Add(patientID);

                PDFMaker pdfmaker = new PDFMaker();
                string path = @"\\ntfs16\TherapyPhysics\LCN Scans\Script_Reports\ChartQA_Reports\ChartQA Report " + patient.LastName + ", " + patient.FirstName + " " + courseID + ".pdf";
                pdfmaker.PDFInit(path, chartQAList, PatientInfo);

                InsertDoc.InsertDocExecute(OutBox, path, PatientInfo, userID);

                Process.Start(path);

            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred in the Execute method!\n\n\n" + e.ToString() + "\n\n\n" + e.StackTrace + "\n\n\n" + e.Source + "\n\n\n" + e.InnerException);
            }

            OutBox.AppendText(Environment.NewLine + Environment.NewLine + "This window will close in ten seconds.");
            Thread.Sleep(10000);
            this.Close();
        }

        private void Execute(VMS.TPS.Common.Model.API.Patient patient, VMS.TPS.Common.Model.API.Course course, VMS.TPS.Common.Model.API.PlanSetup plan, VMS.TPS.Common.Model.API.User user)
        {
            try
            {
                //MessageBox.Show("Trigger 1");
                progressBar1.Visible = true;

                string patientID = patient.Id;
                string courseID = course.Id;
                string planID = plan.Id;
                string userID = user.Id;
                long temp = 0;

                List<long> Beams = new List<long>(); //Called "Radiations" in the database
                List<ChartQA> chartQAList = new List<ChartQA>();

                string ConnectionString = @"Data Source=WVARIASDBP01SS;Initial Catalog=VARIAN;Integrated Security=true;";
                //@"Data Source=WVARIASDBP01SS;Initial Catalog=VARIAN;Integrated Security=true;";

                SqlConnection conn = new SqlConnection(ConnectionString);
                SqlCommand command;
                SqlDataReader datareader;
                string sql;

                //MessageBox.Show("PatientId: " + patientID);

                conn.Open();
                sql = "USE VARIAN SELECT PatientSer FROM dbo.Patient WHERE PatientId = '" + patientID + "'"; //This just queries the patient table to get the patient serial number;
                command = new SqlCommand(sql, conn);
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    temp = Convert.ToInt64(datareader["PatientSer"]);
                }
                conn.Close();

                long patientSer = temp;
                //MessageBox.Show("PatientSer: " + patientSer);

                conn.Open();
                sql = "USE VARIAN SELECT CourseSer FROM dbo.Course WHERE CourseId = '" + courseID + "' AND PatientSer = " + patientSer; //This just queries the course table to get the course serial number;
                command = new SqlCommand(sql, conn);
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    temp = (long)datareader["CourseSer"];
                }
                conn.Close();

                //MessageBox.Show("Trigger 3");
                long courseSer = temp;

                conn.Open();
                sql = "USE VARIAN SELECT PlanSetupSer FROM dbo.PlanSetup WHERE CourseSer = " + courseSer + " AND PlanSetupId = '" + planID + "'"; //This just queries the PlanSetup table to get the PlanSetup serial number;
                command = new SqlCommand(sql, conn);
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    temp = (long)datareader["PlanSetupSer"];
                }
                conn.Close();

                //MessageBox.Show("Trigger 4");
                long planSer = temp;

                //Then we query the "Radiation" table (i.e. Beams) to get all the RadiationSer for all the beams in the plan.
                //We need the RadiationSer of the beams in the plan to identify which ChartQA records are relevant to this plan.
                conn.Open();
                sql = "USE VARIAN SELECT RadiationSer FROM dbo.Radiation WHERE PlanSetupSer = " + planSer;
                command = new SqlCommand(sql, conn);
                datareader = command.ExecuteReader();

                while (datareader.Read())
                {
                    Beams.Add((long)datareader["RadiationSer"]);
                }
                conn.Close();

                //MessageBox.Show("Trigger 5");
                //Now to query ChartQA
                conn.Open();
                sql = "USE VARIAN SELECT ChartQASer, ChartQADateTime, ChartQABy, Comment  FROM dbo.ChartQA WHERE PatientSer = " + patientSer;
                command = new SqlCommand(sql, conn);
                datareader = command.ExecuteReader();

                //The list made below is for ALL the ChartQA entries associated with this patient, for ALL their plans in ALL their course.
                //We need to do the leg work to determine which ones apply to the plan we are interested in.
                while (datareader.Read())
                {
                    chartQAList.Add(new ChartQA { ChartQASer = (long)datareader["ChartQASer"], user = (datareader["ChartQABy"] as string), dateTime = Convert.ToDateTime(datareader["ChartQADateTime"]), comment = (datareader["Comment"] as string) });
                }
                conn.Close();

                //MessageBox.Show("Trigger 6");

                foreach (ChartQA CQ in chartQAList)
                {
                    //MessageBox.Show("User: " + CQ.user + "\nDateTime: " + CQ.dateTime.ToString() + "\nComment: " + CQ.comment + "\nChartQASer: " + CQ.ChartQASer);

                    conn.Open();
                    sql = "USE VARIAN SELECT RadiationHstrySer FROM dbo.ChartQATreatment WHERE ChartQASer = " + CQ.ChartQASer;
                    command = new SqlCommand(sql, conn);
                    datareader = command.ExecuteReader();

                    while (datareader.Read())
                    {
                        CQ.radiationHstrySerList.Add((long)datareader["RadiationHstrySer"]);
                    }
                    conn.Close();
                }

                //MessageBox.Show("Trigger 7");

                foreach (ChartQA CQ in chartQAList)
                {
                    foreach (long RH in CQ.radiationHstrySerList)
                    {
                        conn.Open();
                        sql = "USE VARIAN SELECT RadiationSer FROM dbo.RadiationHstry WHERE RadiationHstrySer = " + RH;
                        command = new SqlCommand(sql, conn);
                        datareader = command.ExecuteReader();

                        while (datareader.Read())
                        {
                            CQ.radiationSerList.Add((long)datareader["RadiationSer"]);
                        }
                        conn.Close();
                    }

                    //removes duplicates
                    CQ.radiationSerList = CQ.radiationSerList.Distinct().ToList();
                }

                //MessageBox.Show("Trigger 8");

                //So when it comes to matching the radiations (or beams) in the Beam list, from the plan that the program is running on, to the radiations in the ChartQA objects,
                //there is a problem regarding the imaging beams. The Beam list includes all the treatment and imaging beams that are in the plan, while the radiations in the 
                //ChartQA objects are based off actual treatment records, and therefore most likley will not include all the imaging beams, as it is standard treatment planning
                //practice to include the full set of imaging beams, however most likley the therapists are not using all of them during treatment.
                //Therefore, to successfully match these lists, we need to be generous and do so only using treatment Beams.

                //But, we don't have a surfire way of doing that, since there is nothing in the database tables to distinguish treatment beams from imaging beams, other than their names
                //which is not 100% reliable.

                //So, we have to be even more generous and simply include all ChartQA objects in our final list that do not have a radiation that does not appear in the beam list, which should be good enough.
                //Remember, we don't care about the radiations here, we are just trying to get rid of ChartQA elements that don't apply to the given plan, due to the way the database is organized.
                //Thus the below code just removes chartQAList elements as soon as a non-matching radiation is found, and breaks out of the loop to test the next chartQA object.

                List<long> chartQARemoveList = new List<long>();
                //string str = null;
                //MessageBox.Show("chartQAList size: " + chartQAList.Count);

                for(int i = 0; i < chartQAList.Count; i++)
                {
                    //str = null;
                    //foreach (long li in chartQAList[i].radiationSerList)
                    //{
                    //    str = str + ", " + li;
                    //}
                    //MessageBox.Show("chartQA " + i + " radiationSer list:  " + str);

                    for(int k = 0; k < chartQAList[i].radiationSerList.Count; k++)
                    {
                        if(Beams.Any(ra => ra.Equals(chartQAList[i].radiationSerList[k])))
                        {
                            //then that radiation is good
                        }
                        else
                        {
                            //MessageBox.Show("chartQA " + i + " removed!");
                            chartQARemoveList.Add(chartQAList[i].ChartQASer);
                            //chartQAList.RemoveAt(i);
                            //Note: using the RemoveAt above doesn't work because the List dynamically reizes itself.
                            //To work with indices like this we should have used a simple array.
                            break;
                        }
                    }
                }

                foreach(long RS in chartQARemoveList)
                {
                    chartQAList.RemoveAll(cq => cq.ChartQASer.Equals(RS));
                }

                //MessageBox.Show("ChartQAList Size: " + chartQAList.Count);
                //So then all the chartQA objects left are what I want to put in a table in a PDF.

                //foreach (ChartQA cq in chartQAList)
                //{
                //    MessageBox.Show("User: " + cq.user + "\nDateTime: " + cq.dateTime.ToString() + "\nComment: " + cq.comment);
                //}

                List<string> PatientInfo = new List<string>();
                PatientInfo.Add(patient.LastName);
                PatientInfo.Add(patient.FirstName);
                PatientInfo.Add(courseID);
                PatientInfo.Add(planID);
                PatientInfo.Add(plan.RTPrescription.Name);
                PatientInfo.Add(patientID);

                string path = @"\\ntfs16\TherapyPhysics\LCN Scans\Script_Reports\ChartQA_Reports\ChartQA Report " + patient.LastName + ", " + patient.FirstName + " " + courseID + " " + planID + ".pdf";
                PDFMaker pdfmaker = new PDFMaker();
                pdfmaker.PDFInit(path, chartQAList, PatientInfo);

                InsertDoc.InsertDocExecute(OutBox, path, PatientInfo, userID);

                Process.Start(@"\\ntfs16\TherapyPhysics\LCN Scans\Script_Reports\ChartQA_Reports\ChartQA Report " + patient.LastName + ", " + patient.FirstName + " " + courseID + " " + planID + ".pdf");
            }
            catch(Exception e)
            {
                MessageBox.Show("An error occurred in the Execute method!\n\n\n" + e.ToString() + "\n\n\n" + e.StackTrace + "\n\n\n" + e.Source + "\n\n\n" + e.InnerException);
            }

            OutBox.AppendText(Environment.NewLine + Environment.NewLine + "This window will close in ten seconds.");
            Thread.Sleep(10000);
            this.Close();           
        }





    }
}
