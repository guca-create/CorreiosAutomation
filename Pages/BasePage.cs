using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using CorreiosAutomation.Config;

namespace CorreiosAutomation.Pages
{
    public abstract class BasePage
    {
        protected readonly IWebDriver Driver;
        protected readonly WebDriverWait Wait;
        protected readonly WebDriverWait WaitCurto;

        protected BasePage(IWebDriver driver)
        {
            Driver = driver;
            Wait = new WebDriverWait(driver, TimeSpan.FromSeconds(Settings.TimeoutSegundosPadrao));
            WaitCurto = new WebDriverWait(driver, TimeSpan.FromSeconds(Settings.TimeoutSegundosCurto));
        }

        protected IWebElement EsperarVisivel(By locator, WebDriverWait? wait = null) =>
            (wait ?? Wait).Until(driver =>
            {
                var elemento = driver.FindElement(locator);
                return elemento.Displayed ? elemento : null;
            });

        protected IWebElement EsperarClicavel(By locator) =>
            Wait.Until(driver =>
            {
                var elemento = driver.FindElement(locator);
                return elemento.Displayed && elemento.Enabled ? elemento : null;
            });

        protected bool ElementoVisivelSemEsperarErro(By locator, WebDriverWait? wait = null)
        {
            try
            {
                EsperarVisivel(locator, wait);
                return true;
            }
            catch (WebDriverTimeoutException)
            {
                return false;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        protected void Preencher(By locator, string valor)
        {
            var elemento = EsperarVisivel(locator);
            elemento.Clear();
            elemento.SendKeys(valor);
        }

        protected void Clicar(By locator) => EsperarClicavel(locator).Click();

        /// <summary>
        /// Localiza o campo de input associado a um rótulo, usando o padrão
        /// de estrutura confirmado via DevTools no site dos Correios:

        protected By CampoPorRotulo(string textoRotulo)
        {
            var xpath = "//div[contains(concat(' ', normalize-space(@class), ' '), ' rotulo ') " +
                        $"and contains(normalize-space(.), '{textoRotulo}')]" +
                        "/following-sibling::div[contains(concat(' ', normalize-space(@class), ' '), ' controle ')][1]" +
                        "//input";
            return By.XPath(xpath);
        }
    }
}
