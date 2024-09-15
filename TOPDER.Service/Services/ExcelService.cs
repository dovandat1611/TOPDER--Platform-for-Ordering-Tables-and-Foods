using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using TOPDER.Service.Dtos.Excel;
using TOPDER.Service.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Services
{
    public class ExcelService : IExcelService
    {
        public async Task<List<Dictionary<string, string>>> ReadFromExcelAsync(IFormFile file, List<ExcelColumnConfiguration> columnConfigurations)
        {
            if (file == null || file.Length <= 0)
                throw new ArgumentException("File not found.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", System.StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Invalid file format.");

            var result = new List<Dictionary<string, string>>();

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                using (var package = new ExcelPackage(stream))
                {
                    var worksheet = package.Workbook.Worksheets[0];
                    int rowCount = worksheet.Dimension.Rows;
                    int colCount = worksheet.Dimension.Columns;

                    // Kiểm tra các cột bắt buộc có trong file Excel
                    foreach (var columnConfig in columnConfigurations)
                    {
                        var columnExists = Enumerable.Range(1, colCount)
                            .Any(i => worksheet.Cells[1, i].Text == columnConfig.ColumnName);

                        if (columnConfig.IsRequired && !columnExists)
                        {
                            throw new Exception($"The required column '{columnConfig.ColumnName}' is missing.");
                        }
                    }

                    // Đọc dữ liệu từ Excel vào danh sách các Dictionary
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var rowData = new Dictionary<string, string>();
                        foreach (var columnConfig in columnConfigurations)
                        {
                            var columnIndex = Enumerable.Range(1, colCount)
                                .FirstOrDefault(i => worksheet.Cells[1, i].Text == columnConfig.ColumnName);

                            if (columnIndex > 0)
                            {
                                rowData[columnConfig.ColumnName] = worksheet.Cells[row, columnIndex].Text;
                            }
                        }

                        result.Add(rowData);
                    }
                }
            }

            return result;
        }
    }
}
