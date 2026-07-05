using OpenQA.Selenium;
using CorreiosAutomation.Config;
using CorreiosAutomation.Drivers;

namespace CorreiosAutomation.Pages
{
    /// <summary>
    /// Representa a tela inicial de busca de CEP e de rastreamento.
    ///
    /// Seletores confirmados via inspeção real (DevTools) em 05/07/2026 no site
    /// https://buscacepinter.correios.com.br/app/endereco/index.php:
    ///   - input#endereco: campo de busca (CEP ou endereço)
    ///   - select#tipoCEP: filtro de tipo de CEP (mantido no valor padrão "Todos")
    ///   - input#captcha: campo de resposta do captcha
    ///   - div.msg contendo o texto "Captcha Inválido!": mensagem de erro do captcha
    /// </summary>
    public class HomePage : BasePage
    {
        // ---------- Busca CEP ----------

        // Check por ID (confirmado via DevTools)
        private readonly By _campoEndereco = By.Id("endereco");
        private readonly By _campoCaptcha = By.Id("captcha");
        private readonly By _botaoBuscarCep = By.Id("btn_pesquisar");

        // Imagem do captcha (confirmada via DevTools - biblioteca Securimage)
        private readonly By _imagemCaptcha = By.Id("captcha_image");

        // Link/botão de atualizar captcha: ícone de refresh existe visualmente,
        // mas o id/atributo real não foi confirmado via DevTools ainda.
        // Não é bloqueante: a biblioteca Securimage normalmente já gera uma nova
        // imagem de captcha automaticamente a cada novo carregamento/submit com
        // código incorreto, então este clique é apenas uma tentativa extra —
        // se o elemento não existir, o código simplesmente ignora (ver
        // ElementoVisivelSemEsperarErro) e segue para a próxima tentativa.
        private readonly By _linkAtualizarCaptcha = By.CssSelector("a[title*='atualiz' i], a[title*='refresh' i], #captcha_image_audio_controls a");

        // Mensagem de erro do captcha (confirmada via DevTools: <div class="msg">Captcha Inválido!</div>)
        // Check por XPath
        private readonly By _mensagemCaptchaInvalido =
            By.XPath("//div[contains(@class,'msg') and contains(normalize-space(.),'Captcha Inválido')]");

        // ---------- Rastreamento ----------
        // TODO CONFIRMAR: estrutura da tela de rastreamento ainda não foi inspecionada.
        // Assumimos que reutiliza o mesmo padrão rotulo/controle do restante do site
        // (ambos são apps do mesmo ecossistema Correios), a confirmar com print do F12.
        private const string RotuloCampoRastreio = "rastre";
        private readonly By _botaoRastrear = By.CssSelector("form button[type='submit'], form input[type='submit']");

        public HomePage(IWebDriver driver) : base(driver) { }

        public void AcessarBuscaCep() => Driver.Navigate().GoToUrl(Settings.BaseUrlBuscaCep);

        public void AcessarRastreamento() => Driver.Navigate().GoToUrl(Settings.BaseUrlRastreamento);

        /// <summary>
        /// Preenche o CEP/endereço pesquisado e resolve o captcha, respeitando
        /// a regra de negócio: após MaxTentativasCaptcha erros seguidos, o
        /// teste deve falhar (CaptchaInvalidoException).
        /// </summary>
        public void PesquisarCep(string cepOuEndereco, ICaptchaSolver captchaSolver)
        {
            Preencher(_campoEndereco, cepOuEndereco);
            ResolverCaptchaComRetentativa(captchaSolver);
        }

        public void PesquisarRastreio(string codigo)
        {
            Preencher(CampoPorRotulo(RotuloCampoRastreio), codigo);
            Clicar(_botaoRastrear);
        }

        private void ResolverCaptchaComRetentativa(ICaptchaSolver solver)
        {
            for (var tentativa = 1; tentativa <= Settings.MaxTentativasCaptcha; tentativa++)
            {
                var imagemElemento = EsperarVisivel(_imagemCaptcha);
                var textoCaptcha = solver.Resolver(imagemElemento);

                Preencher(_campoCaptcha, textoCaptcha);
                Clicar(_botaoBuscarCep);

                if (!CaptchaFoiRejeitado())
                    return; // captcha aceito, segue o fluxo normalmente

                if (tentativa == Settings.MaxTentativasCaptcha)
                {
                    throw new CaptchaInvalidoException(
                        $"Captcha incorreto após {Settings.MaxTentativasCaptcha} tentativas. Teste falhou conforme regra de negócio.");
                }

                if (ElementoVisivelSemEsperarErro(_linkAtualizarCaptcha, WaitCurto))
                    Clicar(_linkAtualizarCaptcha);
            }
        }

        private bool CaptchaFoiRejeitado() => ElementoVisivelSemEsperarErro(_mensagemCaptchaInvalido, WaitCurto);
    }
}
