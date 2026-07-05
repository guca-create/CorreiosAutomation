# CorreiosAutomation

Automação do desafio técnico "Avaliação – Busca CEP", construída em **C# + Reqnroll (sucessor do SpecFlow) + NUnit + Selenium WebDriver**.

## Estrutura

```
CorreiosAutomation/
├── Config/Settings.cs          # URLs, timeouts e regra de tentativas de captcha
├── Drivers/DriverFactory.cs    # Criação e configuração do ChromeDriver
├── Drivers/CaptchaSolver.cs    # Abstração de resolução de captcha (ICaptchaSolver)
├── Drivers/CaptchaInvalidoException.cs
├── Features/Correios.feature   # Cenário Gherkin com o fluxo completo do desafio
├── Hooks/Hooks.cs              # Setup/teardown do browser e screenshot em falha
├── Pages/BasePage.cs           # Esperas explícitas e ações reutilizáveis
├── Pages/HomePage.cs           # Telas de busca de CEP e rastreamento
├── Pages/ResultadoPage.cs      # Telas de resultado (CEP e rastreamento)
├── StepDefinitions/CorreiosSteps.cs
└── TestData/CorreiosData.cs    # Massa de dados do desafio
```

## Regra de negócio: captcha

Após 3 tentativas incorretas (`Settings.MaxTentativasCaptcha`), `HomePage.PesquisarCep`
lança `CaptchaInvalidoException`, fazendo o Reqnroll/NUnit marcar o cenário como falho
automaticamente.

A leitura do captcha é isolada atrás da interface `ICaptchaSolver`, permitindo trocar
a implementação (ex.: integração de leitura de imagem) sem alterar a lógica de retentativa.

## Seletores utilizados

| Tipo   | Elemento |
|--------|----------|
| **Id**    | `#endereco`, `#captcha`, `#btn_pesquisar`, `#captcha_image`, `#resultado-DNEC`, `#btn_nbusca` |
| **XPath** | Mensagem "Captcha Inválido!" (`div.msg`) |
| **CSS**   | Colunas de resultado (`td[data-th='...']`) |

Elementos confirmados via inspeção no DevTools:
- `#endereco` — campo de busca de CEP/endereço
- `#captcha` — campo de resposta do captcha
- `#captcha_image` — imagem do captcha (biblioteca Securimage)
- `#btn_pesquisar` — botão de busca
- `div.msg` com texto "Captcha Inválido!" — mensagem de erro do captcha
- `table#resultado-DNEC` com colunas `Logradouro/Nome`, `Bairro/Distrito`, `Localidade/UF`, `CEP`
- `#mensagem-resultado-alerta` — mensagem de alerta do resultado
- `#btn_nbusca` — botão "Nova Busca"

## Resolução de captcha

O site utiliza a biblioteca Securimage, sem alternativa de bypass ético para automação
e sem ferramenta de OCR prevista no escopo do desafio. O solver padrão
(`ManualConsoleCaptchaSolver`) pausa a execução e aguarda entrada via console,
permitindo validar o fluxo completo com o navegador visível.

Para execução sem interação manual (ex.: pipeline de CI), substituir por
`TestDataCaptchaSolver` em `CorreiosSteps.cs` — válido para testar a lógica de
retentativa/exceção isoladamente, mas não resolve o captcha real.

## Pontos em aberto

1. Mensagem exata de "CEP não encontrado" — `ResultadoPage.CepNaoExiste()` usa fallback
   duplo (mensagem de alerta OU ausência de linhas na tabela), pendente de confirmação do texto real.
2. Tela de rastreamento (`rastreamento.correios.com.br`) — seletores assumem reaproveitamento
   do padrão rótulo/controle da busca de CEP, sem inspeção direta ainda. Maior risco do projeto no momento.

## Otimizações de execução

- `PageLoadStrategy.Eager`: não aguarda recursos secundários (analytics, ads).
- Apenas esperas explícitas (`WebDriverWait`), sem `Thread.Sleep`.
- `ImplicitWait = Zero` para não mascarar tempos reais de carregamento.
- Suporte a execução headless via variável de ambiente `HEADLESS=true`.

## Execução

```bash
dotnet restore
dotnet test
```

## a) Comandos Git para versionamento

Fluxo padrão para submissão de código em um projeto executado diariamente por
servidor de automação:

```bash
git status
git add .
git commit -m "feat: automação de busca de CEP e rastreamento dos Correios"
git pull origin main --rebase
git push origin main
```

Para novas funcionalidades ou correções, isolar em branch e integrar via Pull Request,
mantendo `main` estável para o servidor de automação:

```bash
git checkout -b feature/nome-da-alteracao
# alterações e commits
git push origin feature/nome-da-alteracao
# abrir Pull Request para revisão antes do merge em main
```
