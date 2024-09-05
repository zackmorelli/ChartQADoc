using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

using ChartQADoc;


/*
    ChartQADoc - (Script Execute)
    
    Description:
    This is the Script Execute start-up file for the ChartQADoc program.
    The code in this file uses ESAPI to create a "Script" class and then uses the ESAPI Execute method to start an Eclipse script and pull the scriptcontext object to get info about the current open patient in Eclipse.

    ChartQADoc produces a PDF document outlining of all of the Physics Chart QA performed on a patient's plan. This is intended to be run once a patient's treatment is complete and a Physicist has performed the final chart check.
    The PDF document is then automatically exported to Aria as an Aria document using Aria's Web API (so via a REST request). Once the Aria document has been approved, it should flow to Epic automatically via the MDM HL7 interface.
    This way the document can be accessed in Epic so that people outside the department have a record of the Chart QA tasks that the physicists are performing.
    The purpose of this workflow is to document Physics chart checks for billing purposes.
    Aria does this, but there is no document produced, so this program simply faciltates making a document that records the chart QA, mostly for the purposes of exporting this information to outside systems.
    The program simply does a lot of SQL queries to collect all of this information that is stored in Aria's database and makes a PDF with a table of all the physics Chart QA performed on a plan. 

    This program is expressely written as a plug-in script for use with Varian's Eclipse Treatment Planning System, and requires Varian's API files to run properly.
    This program runs on .NET Framework 4.6.1. 

    Copyright (C) 2021 Zackary Thomas Ricci Morelli
    
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.

    I can be contacted at: zackmorelli@gmail.com


    Release 1.0 - 1/14/2022

*/


namespace VMS.TPS
{
    public class Script  // creates a class called Script within the VMS.TPS Namespace
    {
        [MethodImpl(MethodImplOptions.NoInlining)]       // prevents compiler optimization from messing with the program's methods.
        public Script() { }  // instantiates a Script object

        public void Execute(ScriptContext context)     // PROGRAM START - sending a return to Execute will end the program
        {
            if (context.Patient == null)
            {
                MessageBox.Show("Please load a patient with a treatment plan before running this script!");
                return;
            }

            Patient patient = context.Patient;   // creates an object of the patient class called patient equal to the active patient open in Eclipse
            Course course = context.Course;
            User user = context.CurrentUser;
            IEnumerable<PlanSetup> plans = context.PlansInScope;

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.Run(new GUI(patient, course, plans, user));

            return;
        }  //ends Execute  END OF PROGRAM
    }   //ends Script class
}  //ends namespace























