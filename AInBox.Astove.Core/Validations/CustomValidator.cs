using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AInBox.Astove.Core.Validations
{
    public class CustomValidator
    {
        public const int ComprimentoCNPJ = 18;
        public const int ComprimentoCPF = 14;

        public static bool ValidarCPF(string cpf)
        {
            try
            {
                int[] multiplicador1 = new int[9] { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = new int[10] { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };
                string tempCpf;
                string digito;
                int soma;
                int resto;

                cpf = cpf.Trim();
                cpf = cpf.Replace(".", "").Replace("-", "");

                if (cpf.Length != 11)
                    return false;

                for (int i = 0; i < 9; i++)
                    if (cpf == i.ToString().PadLeft(11, i.ToString().ToCharArray()[0]))
                        return false;

                tempCpf = cpf.Substring(0, 9);
                soma = 0;
                for (int i = 0; i < 9; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador1[i];

                resto = soma % 11;
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = resto.ToString();

                tempCpf = tempCpf + digito;

                soma = 0;
                for (int i = 0; i < 10; i++)
                    soma += int.Parse(tempCpf[i].ToString()) * multiplicador2[i];

                resto = soma % 11;
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = digito + resto.ToString();

                return cpf.EndsWith(digito);
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidarCNPJ(string cnpj)
        {
            try
            {
                int[] multiplicador1 = new int[12] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                int[] multiplicador2 = new int[13] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
                int soma;
                int resto;
                string digito;
                string tempCnpj;

                cnpj = cnpj.Trim();
                cnpj = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");

                if (cnpj.Length != 14)
                    return false;


                for (int i = 0; i < 9; i++)
                    if (cnpj == i.ToString().PadLeft(14, i.ToString().ToCharArray()[0]))
                        return false;

                tempCnpj = cnpj.Substring(0, 12);

                soma = 0;
                for (int i = 0; i < 12; i++)
                    soma += int.Parse(tempCnpj[i].ToString()) * multiplicador1[i];

                resto = (soma % 11);
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = resto.ToString();

                tempCnpj = tempCnpj + digito;
                soma = 0;
                for (int i = 0; i < 13; i++)
                    soma += int.Parse(tempCnpj[i].ToString()) * multiplicador2[i];

                resto = (soma % 11);
                if (resto < 2)
                    resto = 0;
                else
                    resto = 11 - resto;

                digito = digito + resto.ToString();

                return cnpj.EndsWith(digito);
            }
            catch
            {
                return false;
            }
        }

        protected const string validChars = " -_0123456789abcdefghijklmnopqrstuvxzywABCDEFGHIJKLMNOPQRSTUVXZ";

        public static bool ValidarNomeDiretorio(string nome, bool validateEmptySpace)
        {
            if (validateEmptySpace)
            {
                if (nome.Contains(' '))
                {
                    return false;
                }
            }

            var chars = validChars.ToCharArray().Union(nome.ToCharArray());
            return (chars.Distinct().Count() == validChars.Length);
        }

        /// <summary>
        /// Regra para validar nomes de arquivos [A-z] || [0-9] || -_
        /// </summary>
        /// <param name="nome">Nome do arquivo sem extensão</param>
        /// <param name="validateEmptySpace">True para não permitir espaços vazios, False para permitir</param>
        /// <returns></returns>
        public static bool ValidarNomeArquivo(string nome, bool validateEmptySpace)
        {
            if (validateEmptySpace)
            {
                if (nome.Contains(' '))
                {
                    return false;
                }
            }

            var chars = validChars.ToCharArray().Union(nome.ToCharArray());
            return (chars.Distinct().Count() == validChars.Length);
        }


        protected const string validColumnDataBaseChars = "0123456789abcdefghijklmnopqrstuvxzywABCDEFGHIJKLMNOPQRSTUVXZ";

        /// <summary>
        /// Regra para validar nomes de colunas de bancos de [A-z] || [0-9]
        /// </summary>
        /// <param name="nome"></param>
        /// <returns></returns>
        public static bool ValidarNomeColunaBancoDados(string nome)
        {
            var chars = validColumnDataBaseChars.ToCharArray().Union(nome.ToCharArray());
            return (chars.Distinct().Count() == validColumnDataBaseChars.Length);
        }

    }
}
