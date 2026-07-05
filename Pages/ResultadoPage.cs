using System;
using OpenQA.Selenium;

namespace CorreiosAutomation.Pages
{
    /// <summary>
    /// Representa a tela de resultado da busca de CEP e do rastreamento.
    ///
    /// Seletores confirmados via inspeção real (DevTools) em 05/07/2026:
    ///   - table#resultado-DNEC: tabela de resultado, com <td data-th="..."> por coluna
    ///   - #mensagem-resultado-alerta: mensagem de alerta (populada dinamicamente)
    ///   - button#btn_nbusca: botão "Nova Busca"
    /// </summary>
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

        // Rastreamento: estrutura ainda não inspecionada.
        // Check por XPath (baseado em texto, mais resiliente a mudança de layout)
        // TODO CONFIRMAR: mensagem real de código de rastreio inválido.
        private readonly By _mensagemCodigoInvalido =
            By.XPath("//*[contains(text(),'não foi encontrado') or contains(text(),'inválido') or contains(text(),'Verifique se o código')]");

        public ResultadoPage(IWebDriver driver) : base(driver) { }

        /// <summary>
        /// Lê o texto de #mensagem-resultado-alerta via JavaScript (textContent),
        /// já que o elemento pode estar com o atributo "hidden" quando vazio.
        /// </summary>
        public string ObterMensagemAlertaResultado()
        {
            var elemento = Driver.FindElement(_mensagemAlertaResultado);
            return (elemento.GetAttribute("textContent") ?? string.Empty).Trim();
        }

        /// <summary>
        /// Um CEP inexistente não retorna linhas na tabela #resultado-DNEC e/ou
        /// exibe uma mensagem de alerta informando que não foi encontrado.
        /// </summary>
        public bool CepNaoExiste()
        {
            var mensagem = ObterMensagemAlertaResultado();
            if (!string.IsNullOrWhiteSpace(mensagem) &&
                mensagem.Contains("não", StringComparison.OrdinalIgnoreCase))
                return true;

            return !ElementoVisivelSemEsperarErro(_linhasResultado, WaitCurto);
        }

        public bool CodigoRastreioInvalido() => ElementoVisivelSemEsperarErro(_mensagemCodigoInvalido);

        /// <summary>
        /// Retorna "Logradouro, Localidade/UF" da primeira linha da tabela de resultado.
        /// Ex: "Rua Quinze de Novembro - lado ímpar, São Paulo/SP".
        /// </summary>
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
