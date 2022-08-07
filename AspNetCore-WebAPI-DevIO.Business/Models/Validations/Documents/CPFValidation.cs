using System;
using System.Linq;

namespace AspNetCore_WebAPI_DevIO.Business.Models.Validations.Documents
{
	public class CPFValidation
	{
        public const int CPFSize = 11;

        public static bool Validate(string cpf)
        {
            var cpfNumbers = DocumentsValidationUtils.OnlyNumbers(cpf);

            if (!ValidSize(cpfNumbers))
            {
                return false;
            }

            return !HasRepeatedDigits(cpfNumbers) && HasValidDigits(cpfNumbers);
        }

        private static bool ValidSize(string value) => value.Length == CPFSize;

        private static bool HasRepeatedDigits(string value)
        {
            string[] invalidNumbers =
            {
                "00000000000",
                "11111111111",
                "22222222222",
                "33333333333",
                "44444444444",
                "55555555555",
                "66666666666",
                "77777777777",
                "88888888888",
                "99999999999"
            };

            return invalidNumbers.Contains(value);
        }

        private static bool HasValidDigits(string value)
        {
            var number = value.Substring(0, CPFSize - 2);
            var verifyingDigit = new VerifyingDigit(number)
                .WithMultipliersUpTo(2, 11)
                .Replace("0", 10, 11);
            var firstDigit = verifyingDigit.CalculateDigit();
            verifyingDigit.AddDigit(firstDigit);
            var secondDigit = verifyingDigit.CalculateDigit();

            return string.Concat(firstDigit, secondDigit) == value.Substring(CPFSize - 2, 2);
        }
    }
}
