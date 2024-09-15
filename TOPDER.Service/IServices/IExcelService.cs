using Microsoft.AspNetCore.Http;
using TOPDER.Service.Dtos.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOPDER.Service.IServices
{
    public interface IExcelService
    {
        Task<List<Dictionary<string, string>>> ReadFromExcelAsync(IFormFile file, List<ExcelColumnConfiguration> columnConfigurations);
    }
}
