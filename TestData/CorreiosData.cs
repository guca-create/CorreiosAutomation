namespace CorreiosAutomation.TestData
{
    /// Massa de dados do desafio, centralizada para facilitar manutenção
    /// (ex: se os Correios mudarem o endereço de referência do CEP de teste).
    /// </summary>
    public static class CorreiosData
    {
        public const string CepInexistente = "80700000";
        public const string CepExistente = "01013-001";
        public const string EnderecoEsperado = "Rua Quinze de Novembro, São Paulo/SP";
        public const string CodigoRastreioInvalido = "SS987654321BR";

        /// Texto de captcha usado nas tentativas de resolução durante o teste.
        /// Ver comentário em Drivers/CaptchaSolver.cs sobre a estratégia adotada.
        /// </summary>
        public static readonly string[] TentativasCaptcha = { "TESTE1", "TESTE2", "TESTE3" };
    }
}
