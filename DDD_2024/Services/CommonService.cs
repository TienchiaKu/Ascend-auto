using DDD_2024.Data;
using DDD_2024.Interfaces;
using DDD_2024.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.IO;

namespace DDD_2024.Services
{
    public class CommonService: ICommonService
    {
        public CommonService() 
        {

        }

        public int ExcelColNameToNumber(string columnName)
        {
            int columnNumber = 0;
            for (int i = 0; i < columnName.Length; i++)
            {
                columnNumber = columnNumber * 26 + (columnName[i] - 'A' + 1);
            }
            return columnNumber - 1;
        }
    }
}