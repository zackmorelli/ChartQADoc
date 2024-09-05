using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace ChartQADoc
{
    internal class MainContent
    {
        public void Add(Section section, List<ChartQA> chartQAList, List<string> PatientInfo)
        {
            AddMainContent(section, chartQAList, PatientInfo);
        }

        private void AddMainContent(Section section, List<ChartQA> chartQAList, List<string> PatientInfo)
        {
            Paragraph Heading = section.AddParagraph("Physics Chart QA Report", StyleNames.Heading3);
            Heading.Format.Font.Underline = Underline.Single;
            Heading.Format.Font.Size = 20;
            Heading.Format.Font.Bold = true;
            Heading.Format.Alignment = ParagraphAlignment.Center;
            Heading.AddLineBreak();

            Paragraph patinfo = section.AddParagraph();
            patinfo.Format.Alignment = ParagraphAlignment.Left;
            patinfo.Format.SpaceBefore = 10;
            patinfo.Format.SpaceAfter = 15;
            patinfo.Format.Font.Size = 18;
            patinfo.AddFormattedText("Patient: " + PatientInfo[0] + ", " + PatientInfo[1], StyleNames.Heading4);
            patinfo.AddTab();
            patinfo.AddFormattedText("Course: " + PatientInfo[2], StyleNames.Heading4);
            patinfo.AddTab();
            patinfo.AddFormattedText("Plan: " + PatientInfo[3], StyleNames.Heading4);

            if (PatientInfo[4] == null)
            {
                // do nothing
            }
            else
            {
                patinfo.AddLineBreak();
                patinfo.AddFormattedText("Prescription: " + PatientInfo[4], StyleNames.Heading4);
            }

            //Paragraph TableTitle = section.AddParagraph("Chart QA", StyleNames.Heading4);
            //TableTitle.Format.Alignment = ParagraphAlignment.Center;
            //TableTitle.Format.Font.Size = 24;
            //TableTitle.Format.Font.Bold = true;
            //TableTitle.Format.KeepWithNext = true;

            AddChartQATable(section, chartQAList);

            Paragraph QAinfo = section.AddParagraph();
            QAinfo.Format.Alignment = ParagraphAlignment.Left;
            QAinfo.Format.SpaceBefore = 10;
            QAinfo.Format.Font.Size = 14;
            QAinfo.AddFormattedText("This is a summary of physics Chart QA records for a specific RT treatment plan or course." +
                " Our standard practice is to review patient charts weekly or every 5 fractions for all patients on treatment." +
                " The review includes the following: Physician's clinical intent (clinical planning note and radiation prescription)," +
                " treatment plan conforming to the physician's intent, treatment technique and all technical delivery parameters," +
                " treatment plan documentation, IMRT / VMAT QA records, and treatment delivery history and parameters." +
                " An additional final chart review is performed once all prescribed treatment beams have been delivered." +
                " The plan is also marked as completed in the Aria oncology EMR after the final chart review is performed.", StyleNames.Normal);

        }

        private void AddChartQATable(Section section, List<ChartQA> chartQAList)
        {
            Table table = section.AddTable();
            FormatTable(table);
            AddColumnsAndHeaders(table);
            AddRows(table, chartQAList);
            AddLastRowBorder(table);
        }

        private static void FormatTable(Table table)
        {
            table.LeftPadding = 0;
            table.TopPadding = Size.TableCellPadding;
            table.RightPadding = 0;
            table.BottomPadding = Size.TableCellPadding;
            table.Format.LeftIndent = Size.TableCellPadding;
            table.Format.RightIndent = Size.TableCellPadding;

        }

        private void AddLastRowBorder(Table table)
        {
            var lastRow = table.Rows[table.Rows.Count - 1];
            lastRow.Borders.Bottom.Width = 2;
        }

        private void AddColumnsAndHeaders(Table table)
        {
            Unit width = Size.GetWidth(table.Section);
            table.AddColumn(width * 0.33);
            table.AddColumn(width * 0.33);
            table.AddColumn(width * 0.33);

            Row headerRow = table.AddRow();
            headerRow.Borders.Bottom.Width = 1;

            AddHeader(headerRow.Cells[0], "Completed on");
            AddHeader(headerRow.Cells[1], "Performed by");
            AddHeader(headerRow.Cells[2], "Comment");
        }

        private void AddHeader(Cell cell, string header)
        {
            Paragraph p = cell.AddParagraph();
            p.Style = CustomStyles.ColumnHeader;
            p.Format.Font.Size = 18;
            FormattedText formattedText = new FormattedText();
            formattedText = p.AddFormattedText(header);
        }

        private void AddRows(Table table, List<ChartQA> chartQAList)
        {
            foreach (ChartQA cq in chartQAList)
            {
                Row row = table.AddRow();
                row.VerticalAlignment = VerticalAlignment.Center;
                row.Format.Font.Size = 16;

                row.Cells[0].AddParagraph(cq.dateTime.ToShortDateString());
                row.Cells[1].AddParagraph(cq.user);
                row.Cells[2].AddParagraph(cq.comment);
            }
            
        }



    }
}
