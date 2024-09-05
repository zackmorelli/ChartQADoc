using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MigraDoc.DocumentObjectModel;

namespace ChartQADoc
{
    internal class  Header
    {
        public void Add(Section section)
        {
            AddHeader(section);
        }

        private void AddHeader(Section section)
        {
            Paragraph header = section.Headers.Primary.AddParagraph();
            header.Format.AddTabStop(Size.GetWidth(section), TabAlignment.Right);

            header.AddText($"Generated {DateTime.Now:g}");
            header.AddTab();
            header.AddText($"Lahey Radiation Oncology");
            header.Format.Borders.Bottom = new Border() { Width = "1pt", Color = Colors.DarkGray };
        }
    }
}
