using System;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace CashLoanTool.Helper
{
    public static class Utility
    {
        public static byte[] DataReaderToCsv(DbDataReader reader)
        {
            var sb = new StringBuilder();
            //Get All column 
            var columnNames = Enumerable.Range(0, reader.FieldCount)
                                    .Select(reader.GetName)
                                    .ToList();
            //Create headers
            sb.Append(string.Join(",", columnNames));
            //Append Line
            sb.AppendLine();
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string value = reader[i].ToString();
                    sb.Append(value.Replace(Environment.NewLine, " ") + ",");
                }
                sb.Length--; // Remove the last comma
                sb.AppendLine();
            }
            return sb.ToString().ConvertToBytes();
        }
    }
}
