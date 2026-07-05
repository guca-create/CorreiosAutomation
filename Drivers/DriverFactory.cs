using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using CorreiosAutomation.Config;

namespace CorreiosAutomation.Drivers
{
    /// <summary>
    /// Responsável exclusivamente por criar e configurar a instância do WebDriver.
    /// Centralizar aqui evita duplicação de configuração de browser em cada teste
    /// e facilita trocar/otimizar opções (ex: rodar headless no servidor de automação).
    /// </summary>
    public static class DriverFactory
    {
        public static IWebDriver CriarDriver()
        {
            var options = new ChromeOptions();

            // Reduz ruído e melhora estabilidade/performance da execução
            options.AddArgument("--start-maximized");
            options.AddArgument("--disable-notifications");
            options.AddArgument("--disable-popup-blocking");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--lang=pt-BR");

            // Otimização: evita carregar imagens desnecessárias quando não afeta o teste
            // (mantemos imagens habilitadas pois o captcha depende de imagem)
            options.PageLoadStrategy = PageLoadStrategy.Eager; // não espera recursos secundários (analytics, ads etc.)

            if (Settings.Headless)
                options.AddArgument("--headless=new");

            var driver = new ChromeDriver(options);

            // Usamos apenas esperas explícitas (WebDriverWait) no BasePage,
            // por isso o implicit wait fica zerado para não mascarar tempos reais.
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(30);

            return driver;
        }
    }
}
