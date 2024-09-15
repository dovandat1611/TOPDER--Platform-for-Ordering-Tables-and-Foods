using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.Dtos.Excel
{
    public class ExcelColumnConfiguration
    {
        public string ColumnName { get; set; } = null!;
        public int Position { get; set; }
        public bool IsRequired { get; set; }
    }

}
