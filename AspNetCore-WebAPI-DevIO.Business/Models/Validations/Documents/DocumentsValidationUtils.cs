using System;
using System.Collections.Generic;
using System.Text;

namespace AspNetCore_WebAPI_DevIO.Business.Models.Validations.Documents
{
	public class DocumentsValidationUtils
	{
        public static string OnlyNumbers(string value)
        {
            var onlyNumber = "";
            foreach (var s in value)
            {
                if (char.IsDigit(s))
                {
                    onlyNumber += s;
                }
            }
            return onlyNumber.Trim();
        }
    }
}
