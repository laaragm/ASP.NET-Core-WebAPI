using System.Collections.Generic;

namespace AspNetCore_WebAPI_DevIO.Business.Models.Validations.Documents
{
    public class VerifyingDigit
    {
        private string Number;
        private const int Modulo = 11;
        private readonly List<int> Multipliers = new List<int> { 2, 3, 4, 5, 6, 7, 8, 9 };
        private readonly IDictionary<int, string> Substitutions = new Dictionary<int, string>();
        private bool ModuleComplement = true;

        public VerifyingDigit(string number)
        {
            Number = number;
        }

        public VerifyingDigit WithMultipliersUpTo(int firstMultiplier, int lastMultiplier)
        {
            Multipliers.Clear();
            for (var i = firstMultiplier; i <= lastMultiplier; i++)
                Multipliers.Add(i);

            return this;
        }

        public VerifyingDigit Replace(string substitute, params int[] digits)
        {
            foreach (var i in digits)
            {
                Substitutions[i] = substitute;
            }

            return this;
        }

        public void AddDigit(string digit)
        {
            Number = string.Concat(Number, digit);
        }

        public string CalculateDigit() => !(Number.Length > 0) ? "" : GetDigitSum();

        private string GetDigitSum()
        {
            var sum = 0;
            for (int i = Number.Length - 1, m = 0; i >= 0; i--)
            {
                var product = (int)char.GetNumericValue(Number[i]) * Multipliers[m];
                sum += product;
                if (++m >= Multipliers.Count)
                {
                    m = 0;
                }
            }

            var mod = (sum % Modulo);
            var result = ModuleComplement ? Modulo - mod : mod;

            return Substitutions.ContainsKey(result) ? Substitutions[result] : result.ToString();
        }
    }
}
