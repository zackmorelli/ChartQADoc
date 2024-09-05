# ChartQADoc

This program is for use with ARIA/Eclipse, which is a commerical radiation treatment planning software suite made by Varian Medical Systems which is used in Radiation Oncology. This is one of several programs which I have made while working in the Radiation Oncology department at Lahey Hospital and Medical Center in Burlington, MA. I have licensed it under GPL V3 so it is open-source and publicly available.

There is also a .docx README file in the repo that describes what the program does and how it is organized.

ChartQADoc is an ESAPI script that creates a PDF document of Chart QA activities (chart checks) performed by medical physicists in ARIA and puts it into the hospital's central EPIC EHR for record keeping and billing purposes.

This was something I made when the hospital's billing people decided that EPIC needed a document that spelled out all the chart check work done by the physicists.
This would have been an onerous documentation requirement for the physicists, but this script was able to automate it.

To understand what ChartQADoc does, it is necessary to understand the clinical workflow for chart checks.

When a patient is undergoing radiation treatement, medical physicists conduct chart checks to monitor their progress. Chart checks are done according to a particular schedule that depends on the kind of treatment plan the patient is recieving.
Chart checks are not done after every single treatment session, but there is always an inital chart check done shortly befor a patient starts treatment, and a final chart check that is performed when a patient has completed treatment.
Depending on the treatment plan, there are one or more chart checks that occur over the course of treatement, between the start and end.

The physicists performs chart checks in the Chart QA module of ARIA. 

When a physicist has completed the final chart check for a patient who has recently completed treatment in the Chart QA module of ARIA, they then open the patient in Eclipse to mark the plan/course as completed.
When they do that, they can also run the ChartQADoc ESAPI script.

ChartQADoc will perform database queries against the ARIA database to collect information about all chart checks done for the selected plan/course. This information is retrieved from the DB because it is not available via ESAPI, since Chart QA is a completley separate module of ARIA.
Once that chart check information is retrieved from the DB, ChartQADoc creates a PDF that outlines all the chart checks done for the selected plan/course.

ChartQADoc then uses the ARIA Web API (a REST API) to insert the PDF into Aria's document module (which is another separate module in ARIA for documents). It does this in such a way so that when the physicist approves this Chart QA overview document in the document module, it automatically gets copied to the hospital's EPIC via an existing HL7 MDM interface.