using System;
using System.Collections.Generic;
using OpenQA.Selenium;

namespace CorreiosAutomation.Drivers
{
    /// <summary>
    /// Abstrai a forma como o texto do captcha é obtido, permitindo trocar a
    /// implementação (ex: leitura manual, serviço de OCR, ou dado de teste)
    /// sem alterar a lógica de retentativa nas Page Objects.
    /// </summary>
    public interface ICaptchaSolver
    {
        string Resolver(IWebElement imagemCaptcha);
    }

    /// <summary>
    /// Implementação prática para execução real: pausa a execução e pede que a
    /// pessoa rodando o teste digite o texto do captcha, olhando a janela do
    /// Chrome que o Selenium abriu. Não depende de nenhuma biblioteca de OCR
    /// (o desafio não cita nenhuma), apenas entrada via console - por isso é o
    /// solver padrão usado em CorreiosSteps.
    /// </summary>
    public class ManualConsoleCaptchaSolver : ICaptchaSolver
    {
        public string Resolver(IWebElement imagemCaptcha)
        {
            Console.WriteLine();
            Console.WriteLine("=== CAPTCHA ===");
            Console.WriteLine("Veja a imagem do captcha na janela do Chrome aberta pelo teste e digite o texto abaixo:");
            Console.Write("Texto do captcha: ");
            return Console.ReadLine()?.Trim() ?? string.Empty;
        }
    }

    /// <summary>
    /// Implementação de referência para testes automatizados do próprio
    /// framework (ex: validar a lógica de retentativa/falha em isolamento),
    /// obtendo o texto do captcha a partir de uma massa de dados de teste
    /// (TestData/CorreiosData.cs) em vez de interação humana.
    /// </summary>
    public class TestDataCaptchaSolver : ICaptchaSolver
    {
        private readonly Queue<string> _tentativas;

        public TestDataCaptchaSolver(IEnumerable<string> tentativasEmOrdem)
        {
            _tentativas = new Queue<string>(tentativasEmOrdem);
        }

        public string Resolver(IWebElement imagemCaptcha)
        {
            // Cada chamada consome uma tentativa da fila (simula novas leituras
            // a cada nova imagem de captcha gerada pelo site).
            return _tentativas.Count > 0 ? _tentativas.Dequeue() : string.Empty;
        }
    }
}
