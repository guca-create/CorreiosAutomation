using System;
using FluentAssertions;
using OpenQA.Selenium;
using Reqnroll;
using CorreiosAutomation.Drivers;
using CorreiosAutomation.Pages;

namespace CorreiosAutomation.StepDefinitions
{
    [Binding]
    public class CorreiosSteps
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly HomePage _homePage;
        private readonly ResultadoPage _resultadoPage;
        private readonly ICaptchaSolver _captchaSolver;

        public CorreiosSteps(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            var driver = (IWebDriver)_scenarioContext["driver"];

            _homePage = new HomePage(driver);
            _resultadoPage = new ResultadoPage(driver);
            // Solver manual: pausa a execução e pede pra você digitar o texto
            // do captcha olhando a janela do Chrome (não usa OCR/serviço externo,
            // já que o desafio não cita nenhum). Para testes de unidade do
            // próprio framework, troque por TestDataCaptchaSolver.
            _captchaSolver = new ManualConsoleCaptchaSolver();
        }

        [Given(@"que acesso a tela de busca de CEP dos Correios")]
        public void DadoQueAcessoATelaDeBuscaDeCepDosCorreios()
        {
            _homePage.AcessarBuscaCep();
        }

        [When(@"pesquiso pelo CEP ""(.*)""")]
        public void QuandoPesquisoPeloCep(string cep)
        {
            _homePage.PesquisarCep(cep, _captchaSolver);
        }

        [Then(@"o sistema deve confirmar que o CEP não existe")]
        public void EntaoOSistemaDeveConfirmarQueOCepNaoExiste()
        {
            _resultadoPage.CepNaoExiste().Should().BeTrue("o CEP pesquisado não deveria existir na base dos Correios");
        }

        [When(@"volto para a tela inicial de busca de CEP")]
        public void QuandoVoltoParaATelaInicialDeBuscaDeCep()
        {
            _resultadoPage.VoltarParaBuscaCep();
        }

        [Then(@"o resultado deve conter o endereço ""(.*)""")]
        public void EntaoOResultadoDeveConterOEndereco(string enderecoEsperado)
        {
            var enderecoObtido = _resultadoPage.ObterEnderecoResultado();

            // Comparamos por partes (logradouro / localidade-UF) em vez de string exata,
            // pois o site pode retornar complementos no logradouro
            // (ex: "Rua Quinze de Novembro - lado ímpar" em vez de só "Rua Quinze de Novembro").
            foreach (var parte in enderecoEsperado.Split(',', StringSplitOptions.TrimEntries))
            {
                enderecoObtido.Should().Contain(parte, $"o endereço obtido deveria conter '{parte}'");
            }
        }

        [When(@"acesso o rastreamento pelo código ""(.*)""")]
        public void QuandoAcessoORastreamentoPeloCodigo(string codigo)
        {
            _homePage.AcessarRastreamento();
            _homePage.PesquisarRastreio(codigo);
        }

        [Then(@"o sistema deve confirmar que o código de rastreio não está correto")]
        public void EntaoOSistemaDeveConfirmarQueOCodigoDeRastreioNaoEstaCorreto()
        {
            _resultadoPage.CodigoRastreioInvalido().Should().BeTrue("o código de rastreio informado é inválido");
        }

        [Then(@"fecho o navegador")]
        public void EntaoFechoONavegador()
        {
            var driver = (IWebDriver)_scenarioContext["driver"];
            driver.Quit();
        }
    }
}
