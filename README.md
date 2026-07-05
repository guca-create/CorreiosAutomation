# CorreiosAutomation

Automação do desafio técnico "Avaliação – Busca CEP", usando **C# + Reqnroll (sucessor do SpecFlow) + NUnit + Selenium WebDriver**.

## Estrutura

```
CorreiosAutomation/
├── Config/Settings.cs          # URLs, timeouts e regra de 3 tentativas de captcha
├── Drivers/DriverFactory.cs    # Criação/configuração do ChromeDriver
├── Drivers/CaptchaSolver.cs    # Abstração de resolução de captcha (ICaptchaSolver)
├── Drivers/CaptchaInvalidoException.cs
├── Features/Correios.feature   # Cenário Gherkin com o fluxo completo do desafio
├── Hooks/Hooks.cs              # Sobe/derruba o browser, screenshot em falha
├── Pages/BasePage.cs           # Esperas explícitas e ações reutilizáveis
├── Pages/HomePage.cs           # Telas de busca de CEP e rastreamento
├── Pages/ResultadoPage.cs      # Telas de resultado (CEP e rastreamento)
├── StepDefinitions/CorreiosSteps.cs
└── TestData/CorreiosData.cs    # Massa de dados do desafio
```

## Regra de negócio: captcha

Se o captcha for informado incorretamente **3 vezes seguidas**
(`Settings.MaxTentativasCaptcha`), o método `HomePage.PesquisarCep` lança
`CaptchaInvalidoException`, o que faz o Reqnroll/NUnit marcar o cenário como
**falho** automaticamente — sem necessidade de tratamento extra no step.

A leitura do captcha em si fica isolada atrás da interface `ICaptchaSolver`,
já que o desafio não cita nenhuma ferramenta de OCR/quebra de captcha. Isso
permite trocar a implementação (ex: um serviço interno de leitura de imagem)
sem alterar a lógica de retentativa.

## Checks utilizados (conforme exigido no desafio)

| Tipo   | Onde | Status |
|--------|------|--------|
| **Id**    | `HomePage._campoEndereco` (`#endereco`), `_campoCaptcha` (`#captcha`), `_botaoBuscarCep` (`#btn_pesquisar`), `_imagemCaptcha` (`#captcha_image`), `ResultadoPage._tabelaResultado` (`#resultado-DNEC`), `_botaoNovaBusca` (`#btn_nbusca`) |  Confirmado via DevTools |
| **XPath** | `HomePage._mensagemCaptchaInvalido` (texto "Captcha Inválido!" dentro de `div.msg`) | Confirmado via DevTools |
| **CSS**   | `ResultadoPage._colunaLogradouro` / `_colunaLocalidadeUf` (`td[data-th='...']`) | Confirmado via DevTools |

### 100% confirmado (inspeção real no DevTools)
- `#endereco` — campo de busca de CEP/endereço
- `#captcha` — campo de resposta do captcha
- `#captcha_image` — imagem do captcha (biblioteca **Securimage**)
- `#btn_pesquisar` — botão "Buscar"
- `div.msg` com texto **"Captcha Inválido!"** — mensagem de erro do captcha
- `table#resultado-DNEC` com `<td data-th="Logradouro/Nome">`, `data-th="Bairro/Distrito"`, `data-th="Localidade/UF"`, `data-th="CEP"`
- `#mensagem-resultado-alerta` — mensagem de alerta do resultado
- `#btn_nbusca` — botão "Nova Busca"

### Como o captcha é resolvido na execução real
O desafio não cita nenhuma ferramenta de OCR/quebra de captcha, e o **Securimage**
usado pelo site não tem bypass conhecido/ético para automação. Por isso o
solver padrão (`ManualConsoleCaptchaSolver`) **pausa a execução no console** e
pede pra você digitar o texto olhando a janela do Chrome que o teste abriu -
sem headless, sem OCR, sem dependência externa. É a forma honesta de rodar o
teste de ponta a ponta hoje. A regra de "falha após 3 tentativas erradas"
continua funcionando normalmente nesse fluxo.

Para rodar sem interação humana (ex: pipeline de CI), troque em
`CorreiosSteps.cs` por `new TestDataCaptchaSolver(CorreiosData.TentativasCaptcha)`
— mas isso vai sempre falhar no captcha real, servindo apenas para validar a
lógica de retentativa/exceção isoladamente (ex: em um teste de unidade do
próprio framework, sem depender do site).

### Ainda pendente de confirmação (marcado com `// TODO CONFIRMAR` no código)
1. **Mensagem exata de CEP não encontrado** — `ResultadoPage.CepNaoExiste()` usa uma lógica dupla (mensagem de alerta OU ausência de linhas na tabela) como fallback seguro, mas o texto real da mensagem ainda não foi visto
2. **Toda a tela de rastreamento** (`rastreamento.correios.com.br`) — nenhuma captura foi feita dela ainda; o código assume, sem confirmação, que reaproveita o padrão rótulo/controle usado na busca de CEP. **Este é o ponto de maior risco do projeto hoje** - se o layout for diferente, o step de rastreamento pode não encontrar o campo/botão.

> Assim que tiver os prints do F12 da tela de rastreamento, é só mandar que eu fecho esse último ponto.

## Otimizações para reduzir o tempo de execução

- `PageLoadStrategy.Eager` no `DriverFactory`: não espera recursos secundários (analytics, ads, etc.).
- Apenas esperas explícitas (`WebDriverWait`), sem `Thread.Sleep`.
- `ImplicitWait = Zero` para não mascarar tempos reais de carregamento.
- Suporte a execução `headless` via variável de ambiente `HEADLESS=true`, útil no servidor de automação.

## Como rodar

```bash
dotnet restore
dotnet test
```

Screenshots de cenários com falha são salvos em `bin/.../Screenshots`.

## a) Comandos Git para versionar o código

Como o código roda diariamente em um servidor de automação, o fluxo abaixo garante rastreabilidade e permite reverter facilmente caso uma mudança quebre a execução:

```bash
# 1. Verifica o que foi alterado
git status

# 2. Adiciona os arquivos ao stage
git add .

# 3. Cria o commit com mensagem descritiva
git commit -m "feat: automação de busca de CEP e rastreamento dos Correios"

# 4. Garante que está com a versão mais recente antes de enviar (evita conflitos)
git pull origin main --rebase

# 5. Envia para o repositório remoto
git push origin main
```

Para uma nova funcionalidade/correção, o recomendado é isolar em uma branch e integrar via Pull Request, preservando a `main` sempre estável para o servidor de automação:

```bash
git checkout -b feature/nome-da-alteracao
# ... alterações e commits ...
git push origin feature/nome-da-alteracao
# abrir Pull Request para revisão antes do merge em main
```
