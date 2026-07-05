namespace CorreiosAutomation.Config
{
    /// <summary>
    /// Configurações centrais da automação.
    /// Mantidas em um único ponto para facilitar manutenção e execução
    /// em diferentes ambientes (local, servidor de automação, pipeline CI).
    /// </summary>
    public static class Settings
    {
        // URLs oficiais dos Correios utilizadas no desafio
        public static string BaseUrlBuscaCep => "https://buscacepinter.correios.com.br/app/endereco/index.php";
        public static string BaseUrlRastreamento => "https://rastreamento.correios.com.br/app/index.php";

        // Timeout padrão para esperas explícitas (evita Thread.Sleep e reduz tempo de execução)
        public static int TimeoutSegundosPadrao => 10;
        public static int TimeoutSegundosCurto => 3;

        // Regra de negócio do desafio: falhar o teste após N tentativas erradas de captcha
        public static int MaxTentativasCaptcha => 3;

        // Permite rodar headless em servidor de automação sem alterar código
        public static bool Headless =>
            bool.TryParse(System.Environment.GetEnvironmentVariable("HEADLESS"), out var headless) && headless;
    }
}
