using System;
using System.Linq;

namespace AspNetCore_WebAPI_DevIO.Business.Models.Validations.Documents
{
    public class CNPJValidation
    {
        public const int CNPJSize = 14;

        public static bool Validate(string cpnj)
        {
            var numbers = DocumentsValidationUtils.OnlyNumbers(cpnj);

            if (!HasValidSize(numbers))
            {
                return false;
            }

            return !HasRepeatedDigits(numbers) && HasValidDigits(numbers);
        }

        private static bool HasValidSize(string value) => value.Length == CNPJSize;

        private static bool HasRepeatedDigits(string value)
        {
            string[] invalidNumbers =
            {
                "00000000000000",
                "11111111111111",
                "22222222222222",
                "33333333333333",
                "44444444444444",
                "55555555555555",
                "66666666666666",
                "77777777777777",
                "88888888888888",
                "99999999999999"
            };

            return invalidNumbers.Contains(value);
        }

        private static bool HasValidDigits(string value)
        {
            var number = value.Substring(0, CNPJSize - 2);

            var digitoVerificador = new VerifyingDigit(number)
                .WithMultipliersUpTo(2, 9)
                .Replace("0", 10, 11);
            var firstDigit = digitoVerificador.CalculateDigit();
            digitoVerificador.AddDigit(firstDigit);
            var secondDigit = digitoVerificador.CalculateDigit();

            return string.Concat(firstDigit, secondDigit) == value.Substring(CNPJSize - 2, 2);
        }
    }
}
