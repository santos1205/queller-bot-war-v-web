# PROGRESSO DO PROJETO: War Vikings Bot

## 📋 Visão Geral

Este documento rastreia o progresso da conversão do projeto **Queller Bot** (War of the Ring) para **War Vikings Bot**, um sistema de IA para jogar War Vikings solo.

**Data de Início:** 2024  
**Status Atual:** 🟡 Planejamento e Estruturação

---

## 🎯 Objetivo do Projeto

Criar um sistema CLI (Command Line Interface) em **.NET (C#)** que implementa um bot de IA para jogar **War Vikings** solo, seguindo a mesma arquitetura do projeto Queller Bot original (adaptado para .NET/C#).

---

## 📊 Status Geral

| Componente | Status | Progresso |
|------------|--------|-----------|
| Estrutura Base | 🟡 Em Progresso | 30% |
| Sistema de Estado | ⬜ Não Iniciado | 0% |
| Grafos de Decisão | ⬜ Não Iniciado | 0% |
| Sistema de Combate | ⬜ Não Iniciado | 0% |
| Interface CLI | ⬜ Não Iniciado | 0% |
| Mecânicas Especiais | ⬜ Não Iniciado | 0% |
| Documentação | 🟡 Em Progresso | 30% |

**Legenda:**
- 🟢 Completo
- 🟡 Em Progresso
- ⬜ Não Iniciado
- 🔴 Bloqueado

---

## 📝 Componentes a Implementar

### 1. Estrutura Base do Projeto

#### 1.1 Estrutura de Diretórios
- [x] Criar projeto .NET CLI (`dotnet new console`)
- [x] Criar estrutura `src/` com classes principais
- [x] Criar `Graphs/` para árvores de decisão (JSON ou C#)
- [x] Configurar `.csproj` e `Program.cs`
- [x] Configurar `.gitignore` apropriado para .NET
- [x] Criar `README.md` inicial

#### 1.2 Módulo Principal
- [x] Criar `Program.cs` (ponto de entrada)
- [x] Criar estrutura básica do namespace principal
- [ ] Implementar carregamento de grafos (JSON ou classes C#)
- [ ] Implementar loop principal do jogo
- [ ] Implementar gerenciamento de fases

---

### 2. Sistema de Estado (State Management)

#### 2.1 Estado do Jogo
- [ ] Criar classe `WarVikingsState` (equivalente a `QuellerState`)
- [ ] Implementar rastreamento de territórios
- [ ] Implementar rastreamento de exércitos por território
- [ ] Implementar rastreamento de regiões conquistadas
- [ ] Implementar rastreamento de cartas de território
- [ ] Implementar rastreamento de cartas-objetivo
- [ ] Implementar rastreamento de exércitos no Valhalla (máx 6)
- [ ] Implementar rastreamento de navios de guerra (máx 5)
- [ ] Implementar rastreamento de comandante

#### 2.2 Efeito de Comando
- [ ] Criar enum `CommandEffect` (Grito de Batalha, Águas Sangrentas, Parede de Escudos, Prece da Guerra)
- [ ] Implementar sorteio de efeito no início
- [ ] Implementar aplicação de efeitos em combate

#### 2.3 Poderes dos Deuses
- [ ] Criar estrutura para cartas de poder dos deuses
- [ ] Implementar rastreamento de cartas usadas/disponíveis
- [ ] Implementar sistema de sacrifício de exércitos do Valhalla
- [ ] Implementar invocação de poderes (antes da rolagem)

---

### 3. Tipos de Dados e Enums

#### 3.1 Componentes do Jogo
- [ ] Criar enum `ArmyType` (Guerreiro, EmblemaDoCla)
- [ ] Criar enum `TerritoryType` (ComPorto, SemPorto)
- [ ] Criar enum `RegionType` (regiões do tabuleiro)
- [ ] Criar enum `GodType` (Odin, Thor, Loki, etc.)
- [ ] Criar enum `CommandEffectType` (GritoDeBatalha, AguasSangrentas, ParedeDeEscudos, PreceDaGuerra)
- [ ] Criar classe `Territory` (nome, tipo, porto, ocupação)
- [ ] Criar classe `Army` (tipo, quantidade, localização)
- [ ] Criar classe `Ship` (localização, porto)

#### 3.2 Sistema de Combate
- [ ] Criar enum `DiceColor` (Vermelho, Amarelo)
- [ ] Criar classe `CombatResult` (rolagens, comparações, perdas)
- [ ] Implementar lógica de rolagem de dados (máx 3 dados por lado)

---

### 4. Grafos de Decisão (Árvores de Decisão)

#### 4.1 Estrutura Base dos Grafos
- [ ] Criar classes base de nós (`Node`, `StartNode`, `EndNode`, etc.)
- [ ] Criar classes de nós: `PerformActionNode`, `BinaryConditionNode`, `MultipleChoiceNode`, `JumpToGraphNode`
- [ ] Decidir formato de grafos (JSON ou classes C#)
- [ ] Criar sistema de carregamento de grafos
- [ ] Criar novos tipos de nós específicos se necessário

#### 4.2 Fases do Turno
- [ ] **Fase 1: Recebimento de Exércitos**
  - [ ] Calcular exércitos por territórios (÷2, min 3)
  - [ ] Calcular exércitos por regiões conquistadas
  - [ ] Verificar necessidade de troca de cartas (5+ cartas)
  - [ ] Implementar troca de cartas (3 iguais ou 3 diferentes)
  - [ ] Alocar exércitos recebidos

- [ ] **Fase 2: Ataques**
  - [ ] Identificar territórios atacáveis (contíguos, min 2 exércitos)
  - [ ] Selecionar alvos prioritários
  - [ ] Resolver combates (rolagem de dados)
  - [ ] Aplicar poderes dos deuses (se invocados)
  - [ ] Aplicar efeitos de comando (se comandante presente)
  - [ ] Conquistar territórios (se defensor eliminado)
  - [ ] Mover exércitos para território conquistado

- [ ] **Fase 3: Deslocamento de Exércitos**
  - [ ] Identificar possibilidades de deslocamento
  - [ ] Selecionar deslocamentos estratégicos
  - [ ] Executar deslocamento (1 por turno, exceto após conquista)

- [ ] **Fase 4: Recebimento de Carta de Território**
  - [ ] Verificar se conquistou território adversário
  - [ ] Receber carta de território
  - [ ] Verificar se acumulou 5+ cartas (forçar troca)

#### 4.3 Sub-grafos Especializados
- [ ] **CombatGraph.cs** ou **combat.json** - Resolução de combate terrestre
  - [ ] Rolagem de dados (vermelhos vs amarelos)
  - [ ] Comparação de resultados (maior com maior, etc.)
  - [ ] Aplicação de perdas
  - [ ] Decisão de enviar para Valhalla ou reserva

- [ ] **NavalCombatGraph.cs** ou **combate-naval.json** - Resolução de combate naval
  - [ ] Verificação de navios em portos
  - [ ] Combate entre navios (3 dados cada)
  - [ ] Destruição de navios (3 vitórias)
  - [ ] Ataque terrestre após vitória naval

- [ ] **ValhallaGraph.cs** ou **valhalla.json** - Gerenciamento do Valhalla
  - [ ] Envio de exércitos derrotados para Valhalla
  - [ ] Verificação de limite (máx 6 exércitos)
  - [ ] Sacrifício de exércitos para poderes dos deuses
  - [ ] Construção de navios (sacrificar 1 exército)

- [ ] **GodPowersGraph.cs** ou **poderes-deuses.json** - Invocação de poderes
  - [ ] Seleção de deus
  - [ ] Anúncio de sacrifício
  - [ ] Aplicação de efeito na rolagem
  - [ ] Segunda rolagem (se necessário)

- [ ] **CardTradeGraph.cs** ou **troca-cartas.json** - Sistema de troca de cartas
  - [ ] Verificação de 3 cartas iguais
  - [ ] Verificação de 3 cartas diferentes
  - [ ] Cálculo de exércitos progressivos (4, 6, 8, 10...)
  - [ ] Forçar troca se 5+ cartas

---

### 5. Sistema de Combate

#### 5.1 Combate Terrestre
- [ ] Implementar rolagem de dados vermelhos (atacante, máx 3)
- [ ] Implementar rolagem de dados amarelos (defensor, máx 3)
- [ ] Implementar comparação (maior com maior, segundo com segundo, etc.)
- [ ] Implementar regra de empate (vitória da defesa)
- [ ] Implementar perda de exércitos (1 por comparação perdida)
- [ ] Implementar decisão de enviar para Valhalla ou reserva
- [ ] Implementar conquista de território (quando defensor eliminado)
- [ ] Implementar movimento de exércitos após conquista (min 1, máx 3)

#### 5.2 Combate Naval
- [ ] Implementar verificação de portos
- [ ] Implementar combate entre navios (3 dados cada)
- [ ] Implementar sistema de vitórias (3 vitórias = destruição)
- [ ] Implementar retorno ao porto de origem (vencedor)
- [ ] Implementar remoção de navio destruído

#### 5.3 Aplicação de Efeitos
- [ ] Implementar Grito de Batalha (rerrolar 1 dado de ataque)
- [ ] Implementar Águas Sangrentas (rerrolar em combate naval)
- [ ] Implementar Parede de Escudos (rerrolar 1 dado de defesa)
- [ ] Implementar Prece da Guerra (ignorar carta, embaralhar, comprar nova)

---

### 6. Interface CLI (Command Line Interface)

#### 6.1 Adaptação do CLI
- [ ] Criar classe `CliInterface` ou usar `System.Console`
- [ ] Usar biblioteca CLI (ex: `System.CommandLine` ou `Spectre.Console`)
- [ ] Atualizar mensagens de boas-vindas
- [ ] Atualizar mensagens de ajuda
- [ ] Implementar comandos especiais (help, undo, exit, reset, phase)

#### 6.2 Inputs Específicos
- [ ] Criar input para seleção de territórios
- [ ] Criar input para quantidade de exércitos
- [ ] Criar input para rolagem de dados (simulação ou manual)
- [ ] Criar input para decisão Valhalla vs Reserva
- [ ] Criar input para seleção de deus
- [ ] Criar input para quantidade de sacrifício

#### 6.3 Display de Informações
- [ ] Mostrar estado atual do tabuleiro (territórios ocupados)
- [ ] Mostrar exércitos disponíveis
- [ ] Mostrar exércitos no Valhalla
- [ ] Mostrar navios disponíveis
- [ ] Mostrar cartas de território
- [ ] Mostrar objetivo (se revelado)

---

### 7. Mecânicas Especiais

#### 7.1 Sistema de Recebimento de Exércitos
- [ ] Calcular por territórios: `floor(territórios / 2)`, mínimo 3
- [ ] Calcular por regiões: valores da tabela do tabuleiro
- [ ] Distribuir obrigatoriamente na região (se região conquistada)
- [ ] Implementar sistema de troca de cartas progressivo

#### 7.2 Sistema de Cartas
- [ ] Implementar cartas de território
- [ ] Implementar cartas-objetivo (mantidas em segredo)
- [ ] Implementar sistema de embaralhamento
- [ ] Implementar distribuição inicial
- [ ] Implementar recebimento após conquista

#### 7.3 Sistema de Vitória
- [ ] Verificar condições de vitória (carta-objetivo)
- [ ] Implementar revelação de objetivo
- [ ] Implementar eliminação de jogador (receber cartas do eliminado)
- [ ] Implementar forçar troca após eliminação (se 5+ cartas)

#### 7.4 Primeira Rodada
- [ ] Implementar proibição de ataque na primeira rodada
- [ ] Implementar apenas posicionamento de exércitos

---

### 8. Sistema de Navegação (GraphCrawler)

#### 8.1 Adaptação do Crawler
- [ ] Criar classe `GraphCrawler` para War Vikings
- [ ] Manter sistema de auto-navegação
- [ ] Manter sistema de undo
- [ ] Adaptar para novos tipos de estado
- [ ] Implementar navegação assíncrona (se necessário)

#### 8.2 Nós Específicos
- [ ] Criar nós para verificação de territórios
- [ ] Criar nós para verificação de exércitos
- [ ] Criar nós para verificação de regiões
- [ ] Criar nós para verificação de Valhalla
- [ ] Criar nós para verificação de navios

---

### 9. Documentação

#### 9.1 Documentação Técnica
- [ ] Criar README.md principal
- [ ] Documentar estrutura do projeto
- [ ] Documentar como criar novos grafos
- [ ] Documentar tipos de nós disponíveis
- [ ] Documentar sistema de estado

#### 9.2 Manual do Usuário
- [ ] Criar manual de uso (baseado em `guia-uso-queller-bot.md`)
- [ ] Documentar comandos disponíveis
- [ ] Documentar fluxo do jogo
- [ ] Criar exemplos de uso
- [ ] Documentar regras específicas do bot

#### 9.3 Glossário
- [ ] Adaptar glossário para War Vikings
- [ ] Definir termos técnicos do jogo
- [ ] Definir termos do bot

---

### 10. Testes e Validação

#### 10.1 Testes Unitários
- [ ] Configurar framework de testes (xUnit, NUnit ou MSTest)
- [ ] Testar sistema de combate
- [ ] Testar cálculo de exércitos
- [ ] Testar sistema de Valhalla
- [ ] Testar sistema de cartas
- [ ] Testar grafos de decisão

#### 10.2 Testes de Integração
- [ ] Testar fluxo completo de turno
- [ ] Testar múltiplos turnos
- [ ] Testar condições de vitória
- [ ] Testar eliminação de jogador

#### 10.3 Validação de Regras
- [ ] Validar contra regras oficiais
- [ ] Validar lógica de decisão do bot
- [ ] Validar prioridades de ação

---

## 🚧 Bloqueadores e Dependências

### Bloqueadores Atuais
- Nenhum no momento

### Dependências
1. **Estrutura Base** → Necessária para todos os outros componentes
2. **Sistema de Estado** → Necessário para grafos de decisão
3. **Tipos de Dados** → Necessários para sistema de estado
4. **Grafos de Decisão** → Dependem de estado e tipos
5. **Sistema de Combate** → Pode ser desenvolvido em paralelo
6. **CLI** → Depende de grafos e estado

---

## 📅 Próximos Passos (Roadmap)

### Fase 1: Fundação (Atual)
- [x] Assimilar projeto legado
- [x] Assimilar regras de War Vikings
- [x] Criar documento de progresso
- [x] Definir arquitetura detalhada
- [x] Criar estrutura base do projeto

### Fase 2: Core System
- [ ] Implementar tipos de dados básicos
- [ ] Implementar sistema de estado
- [ ] Adaptar GraphCrawler
- [ ] Criar primeiro grafo de teste

### Fase 3: Mecânicas Principais
- [ ] Implementar sistema de combate
- [ ] Implementar recebimento de exércitos
- [ ] Implementar sistema de cartas
- [ ] Implementar Valhalla

### Fase 4: Grafos Completos
- [ ] Implementar todas as fases do turno
- [ ] Implementar sub-grafos especializados
- [ ] Implementar mecânicas especiais

### Fase 5: Interface e Polimento
- [ ] Adaptar CLI completamente
- [ ] Melhorar mensagens e feedback
- [ ] Implementar comandos especiais
- [ ] Criar documentação completa

### Fase 6: Testes e Refinamento
- [ ] Testes unitários
- [ ] Testes de integração
- [ ] Validação de regras
- [ ] Ajustes finais

---

## 📝 Notas de Desenvolvimento

### Decisões de Design
- Manter arquitetura similar ao Queller Bot para facilitar manutenção
- Usar **.NET (C#)** como plataforma (CLI application)
- Manter sistema de grafos de decisão (proven eficaz)
- Usar JSON ou classes C# para definir grafos (a decidir)

### Considerações Especiais
- War Vikings tem mecânicas diferentes de War of the Ring
- Sistema de combate é mais simples (dados vermelhos vs amarelos)
- Valhalla e poderes dos deuses são únicos
- Combate naval é nova mecânica

### Referências
- Projeto legado: `projeto-legado/` (Julia - referência arquitetural)
- Regras do jogo: `help/regras.md`
- Manual original: `projeto-legado/manual regras do jogo.txt`

### Tecnologias Utilizadas
- **Plataforma:** .NET (versão 8.0+ recomendada)
- **Linguagem:** C#
- **Dependências Potenciais:**
  - `System.CommandLine` - Interface CLI moderna (opcional)
  - `Spectre.Console` - Interface CLI rica com cores e tabelas (opcional)
  - `Newtonsoft.Json` ou `System.Text.Json` - Parsing de grafos JSON (se usar JSON)
- **Paradigma:** Programação baseada em grafos de decisão
- **Estrutura:** Classes C# e/ou arquivos JSON

---

## 🔄 Histórico de Atualizações

### 2024 - Início do Projeto
- **Data:** [Data atual]
- **Ação:** Criação do documento de progresso
- **Status:** Planejamento inicial completo

---

## ✅ Checklist Rápido

- [ ] Estrutura base criada (.NET project)
- [ ] `.csproj` configurado
- [ ] Tipos de dados definidos (enums e classes)
- [ ] Sistema de estado implementado (`WarVikingsState`)
- [ ] Motor de grafos implementado (`Graph`, `Node`, etc.)
- [ ] GraphCrawler implementado
- [ ] Primeiro grafo funcionando
- [ ] Sistema de combate implementado
- [ ] CLI adaptado (`CliInterface` ou `System.CommandLine`)
- [ ] Todas as fases do turno implementadas
- [ ] Mecânicas especiais implementadas
- [ ] Documentação completa
- [ ] Testes realizados (xUnit ou NUnit)
- [ ] Projeto pronto para uso (`dotnet run` ou executável)

---

**Última atualização:** [Data será atualizada automaticamente]  
**Mantido por:** Equipe de Desenvolvimento War Vikings Bot

