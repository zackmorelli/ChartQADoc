using MigraDoc.DocumentObjectModel;
using MigraDoc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChartQADoc
{
    public class PDFMaker
    {
        public void PDFInit(string path, List<ChartQA> chartQAList, List<string> PatientInfo)
        {
            RenderAndSavePDF(path, MakePDFDocument(chartQAList, PatientInfo));
        }

        private void RenderAndSavePDF(string path, Document doc)
        {
            PdfDocumentRenderer pdfRenderer = new PdfDocumentRenderer();
            pdfRenderer.Document = doc;
            pdfRenderer.RenderDocument();

            try
            {
                pdfRenderer.PdfDocument.Save(path);
            }
            catch (Exception e)
            {
                MessageBox.Show("Something has gone wrong while attempting to save the PDF. This happens when you have the file open while running the program.\n\nThis can also happen if there is a special character in the Plan, Course or Patient ID that the program is using to construct the file name that is not allowed by the Windows file system. Please remove these characters and try rerunning the script.");
            }

            MessageBox.Show("Pdf successfully rendered and saved");
        }

        private Document MakePDFDocument(List<ChartQA> chartQAList, List<string> PatientInfo)
        {
            Document doc = new Document();
            CustomStyles.Define(doc);
            doc.Add(CreateMainSection(chartQAList, PatientInfo));
            return doc;
        }

        private Section CreateMainSection(List<ChartQA> chartQAList, List<string> PatientInfo)
        {
            Section section = new Section();
            SetUpPage(section);
            AddHeader(section);
            AddMainContent(section, chartQAList, PatientInfo);
            return section;
        }

        private void SetUpPage(Section section)
        {
            section.PageSetup.PageFormat = PageFormat.Letter;

            section.PageSetup.LeftMargin = Size.LeftRightPageMargin;
            section.PageSetup.TopMargin = Size.TopBottomPageMargin;
            section.PageSetup.RightMargin = Size.LeftRightPageMargin;
            section.PageSetup.BottomMargin = Size.TopBottomPageMargin;

            section.PageSetup.HeaderDistance = Size.HeaderFooterMargin;
            section.PageSetup.FooterDistance = Size.HeaderFooterMargin;
        }

        private void AddHeader(Section section)
        {
            new Header().Add(section);
        }

        private void AddMainContent(Section section, List<ChartQA> chartQAList, List<string> PatientInfo)
        {
            new MainContent().Add(section, chartQAList, PatientInfo);
        }



    }
}
