# language: pt
Funcionalidade: Avaliação - Busca CEP e Rastreamento nos Correios
    Como QA responsável pela automação
    Quero validar a busca de CEP e o rastreamento de encomendas no site dos Correios
    Para garantir que os resultados exibidos estão corretos

@correios @busca-cep @rastreamento
Cenario: Validar CEP inexistente, CEP existente e código de rastreio inválido
    Dado que acesso a tela de busca de CEP dos Correios
    Quando pesquiso pelo CEP "80700000"
    Entao o sistema deve confirmar que o CEP não existe
    Quando volto para a tela inicial de busca de CEP
    E pesquiso pelo CEP "01013-001"
    Entao o resultado deve conter o endereço "Rua Quinze de Novembro, São Paulo/SP"
    Quando volto para a tela inicial de busca de CEP
    E acesso o rastreamento pelo código "SS987654321BR"
    Entao o sistema deve confirmar que o código de rastreio não está correto
    E fecho o navegador
