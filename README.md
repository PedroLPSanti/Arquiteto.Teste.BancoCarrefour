# Desafio Arquiteto de Software - Carrefour
## Descritivo da Solução
Um comerciante precisa controlar o seu fluxo de caixa diário com os lançamentos
(débitos e créditos), também precisa de um relatório que disponibilize o saldo
diário consolidado.
### Requisitos de negócio
• Serviço que faça o controle de lançamentos<br>
• Serviço do consolidado diário<br>
### Requisitos técnicos obrigatórios
• Desenho da solução<br>
• Deve ser feito usando C#<br>
• Testes<br>
• Boas praticas são bem vindas (Design Patterns, Padrões de Arquitetura,
SOLID e etc)<br>
• Readme com instruções claras de como a aplicação funciona, e como rodar
localmente<br>
• Hospedar em repositório publico (GitHub)<br>
• Todas as documentações de projeto devem estar no repositório<br><br>
_Caso os requisitos técnicos obrigatórios não sejam minimamente atendidos, o
teste será descartado._
### Requisitos não funcionais
O serviço de controle de lançamento não deve ficar indisponível se o sistema de
consolidado diário cair. Em dias de picos, o serviço de consolidado diário recebe
50 requisições por segundo, com no máximo 5% de perda de requisições.
