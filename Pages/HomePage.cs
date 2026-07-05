using OpenQA.Selenium;
using CorreiosAutomation.Config;
using CorreiosAutomation.Drivers;

namespace CorreiosAutomation.Pages
{
    public class HomePage : BasePage
    {
        // ---------- Busca CEP ----------

        // Check por ID (confirmado via DevTools)
        private readonly By _campoEndereco = By.Id("endereco");
        private readonly By _campoCaptcha = By.Id("captcha");
        private readonly By _botaoBuscarCep = By.Id("btn_pesquisar");

        // Imagem do captcha (confirmada via DevTools - biblioteca Securimage)
        private readonly By _imagemCaptcha = By.Id("captcha_image");
        private readonly By _linkAtualizarCaptcha = By.CssSelector("a[title*='atualiz' i], a[title*='refresh' i], #captcha_image_audio_controls a");

        // Mensagem de erro do captcha (confirmada via DevTools: <div class="msg">Captcha Inválido!</div>)
        // Check por XPath
        private readonly By _mensagemCaptchaInvalido =
            By.XPath("//div[contains(@class,'msg') and contains(normalize-space(.),'Captcha Inválido')]");

        // ---------- Rastreamento ----------
        // TODO CONFIRMAR: estrutura da tela de rastreamento ainda não foi inspecionada.
        // Assumimos que reutiliza o mesmo padrão rotulo/controle do restante do site
        private const string RotuloCampoRastreio = "rastre";
        private readonly By _botaoRastrear = By.CssSelector("form button[type='submit'], form input[type='submit']");

        public HomePage(IWebDriver driver) : base(driver) { }

        public void AcessarBuscaCep() => Driver.Navigate().GoToUrl(Settings.BaseUrlBuscaCep);

        public void AcessarRastreamento() => Driver.Navigate().GoToUrl(Settings.BaseUrlRastreamento);

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
