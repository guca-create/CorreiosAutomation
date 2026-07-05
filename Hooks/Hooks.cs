using System;
using System.IO;
using NUnit.Framework;
using OpenQA.Selenium;
using Reqnroll;
using CorreiosAutomation.Drivers;

namespace CorreiosAutomation.Hooks
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private IWebDriver? _driver;

        public Hooks(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public void IniciarNavegador()
        {
            _driver = DriverFactory.CriarDriver();
            _scenarioContext["driver"] = _driver;
        }

        [AfterScenario]
        public void EncerrarNavegador()
        {
            if (_scenarioContext.TestError != null)
                CapturarScreenshot();

            _driver?.Quit();
            _driver?.Dispose();
        }

        /// <summary>
        /// Captura evidência visual sempre que o cenário falha - inclusive
        /// quando a falha é a CaptchaInvalidoException após as 3 tentativas.
        /// Facilita a análise do bug pelo QA/dev (conforme boas práticas de abertura de bug).
        /// </summary>
        private void CapturarScreenshot()
        {
            if (_driver is not ITakesScreenshot camera) return;

            try
            {
                var screenshot = camera.GetScreenshot();
                var pasta = Path.Combine(TestContext.CurrentContext.WorkDirectory, "Screenshots");
                Directory.CreateDirectory(pasta);

                var nomeArquivo = $"{_scenarioContext.ScenarioInfo.Title}_{DateTime.Now:yyyyMMdd_HHmmss}.png"
                    .Replace(' ', '_');
                var caminhoCompleto = Path.Combine(pasta, nomeArquivo);

                screenshot.SaveAsFile(caminhoCompleto);
                TestContext.AddTestAttachment(caminhoCompleto, "Screenshot da falha");
            }
            catch
            {
                // Não deixamos uma falha ao capturar o screenshot mascarar o erro original do teste.
            }
        }
    }
}
