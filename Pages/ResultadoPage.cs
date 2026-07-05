using System;
using OpenQA.Selenium;

namespace CorreiosAutomation.Pages
{

    public class ResultadoPage : BasePage
    {
        // Check por ID (confirmado via DevTools)
        private readonly By _tabelaResultado = By.Id("resultado-DNEC");
        private readonly By _mensagemAlertaResultado = By.Id("mensagem-resultado-alerta");
        private readonly By _botaoNovaBusca = By.Id("btn_nbusca");

        // Check por CSS (confirmado via DevTools - colunas identificadas por data-th)
        private readonly By _linhasResultado = By.CssSelector("#resultado-DNEC tbody tr");
        private readonly By _colunaLogradouro = By.CssSelector("#resultado-DNEC tbody tr td[data-th='Logradouro/Nome']");
        private readonly By _colunaLocalidadeUf = By.CssSelector("#resultado-DNEC tbody tr td[data-th='Localidade/UF']");

        private readonly By _mensagemCodigoInvalido =
            By.XPath("//*[contains(text(),'não foi encontrado') or contains(text(),'inválido') or contains(text(),'Verifique se o código')]");

        public ResultadoPage(IWebDriver driver) : base(driver) { }

        public string ObterMensagemAlertaResultado()
        {
            var elemento = Driver.FindElement(_mensagemAlertaResultado);
            return (elemento.GetAttribute("textContent") ?? string.Empty).Trim();
        }
        
        public bool CepNaoExiste()
        {
            var mensagem = ObterMensagemAlertaResultado();
            if (!string.IsNullOrWhiteSpace(mensagem) &&
                mensagem.Contains("não", StringComparison.OrdinalIgnoreCase))
                return true;

            return !ElementoVisivelSemEsperarErro(_linhasResultado, WaitCurto);
        }

        public bool CodigoRastreioInvalido() => ElementoVisivelSemEsperarErro(_mensagemCodigoInvalido);

        public string ObterEnderecoResultado()
        {
            EsperarVisivel(_tabelaResultado);
            var logradouro = EsperarVisivel(_colunaLogradouro).Text.Trim();
            var localidadeUf = EsperarVisivel(_colunaLocalidadeUf).Text.Trim();
            return $"{logradouro}, {localidadeUf}";
        }

        public void VoltarParaBuscaCep() => Clicar(_botaoNovaBusca);
    }
}
