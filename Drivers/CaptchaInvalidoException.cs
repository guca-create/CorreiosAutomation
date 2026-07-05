using System;

namespace CorreiosAutomation.Drivers
{
    /// <summary>
    /// Lançada quando o usuário (automação) erra o captcha o número máximo
    /// de vezes permitido (Settings.MaxTentativasCaptcha).
    /// Ao propagar essa exceção, o Reqnroll/NUnit marca o cenário como
    /// FALHO automaticamente, cumprindo a regra de negócio do desafio.
    /// </summary>
    public class CaptchaInvalidoException : Exception
    {
        public CaptchaInvalidoException(string mensagem) : base(mensagem) { }
    }
}
