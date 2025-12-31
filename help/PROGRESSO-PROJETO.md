# PROGRESSO DO PROJETO: War Vikings Bot

[↑ Voltar ao topo](#progresso-do-projeto-war-vikings-bot)

## 📋 Visão Geral

Este documento rastreia o progresso da conversão do projeto **Queller Bot** (War of the Ring) para **War Vikings Bot**, um sistema de IA para jogar War Vikings solo.

**Data de Início:** 20/12/2025  
**Status Atual:** 🟢 Core System Validado - Pronto para Mecânicas do Jogo

---

## 🎯 Objetivo do Projeto

Criar um sistema CLI (Command Line Interface) em **.NET (C#)** que implementa um bot de IA para jogar **War Vikings** solo, seguindo a mesma arquitetura do projeto Queller Bot original (adaptado para .NET/C#).

---

## 📊 Status Geral

| Componente | Status | Progresso |
|------------|--------|-----------|
| Estrutura Base | 🟢 Completo | 100% |
| Sistema de Estado | 🟢 Completo | 100% |
| Grafos de Decisão | 🟢 Completo | 100% |
| GraphCrawler | 🟢 Completo | 100% |
| Interface CLI | 🟢 Completo | 100% |
| Grafo de Teste | 🟢 Completo | 100% |
| Fase 1 - Recebimento de Exércitos | 🟢 Completo | 100% |
| Fase 2 - Ataques | 🟡 Em Progresso | 80% |
| Fase 3 - Deslocamento de Exércitos | 🟢 Completo | 100% |
| Sistema de Combate | 🟡 Em Progresso | 70% |
| Mecânicas Especiais | 🟡 Em Progresso | 20% |
| Documentação | 🟡 Em Progresso | 70% |

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
- [x] Implementar carregamento de grafos (classes C#)
- [x] Implementar loop principal do jogo (via CliInterface)
- [ ] Implementar gerenciamento de fases

[↑ Voltar ao topo](#-visão-geral)

---

### 2. Sistema de Estado (State Management)

#### 2.1 Estado do Jogo
- [x] Criar classe `WarVikingsState` (equivalente a `QuellerState`)
- [x] Implementar rastreamento de territórios
- [x] Implementar rastreamento de exércitos por território
- [x] Implementar rastreamento de regiões conquistadas
- [x] Implementar rastreamento de cartas de território
- [x] Implementar rastreamento de cartas-objetivo
- [x] Implementar rastreamento de exércitos no Valhalla (máx 6)
- [x] Implementar rastreamento de navios de guerra (máx 5)
- [x] Implementar rastreamento de comandante
- [x] Implementar rastreamento de trocas de cartas (progressivo)

#### 2.2 Efeito de Comando
- [x] Criar enum `CommandEffectType` (Grito de Batalha, Águas Sangrentas, Parede de Escudos, Prece da Guerra)
- [ ] Implementar sorteio de efeito no início
- [ ] Implementar aplicação de efeitos em combate

#### 2.3 Poderes dos Deuses
- [ ] Criar estrutura para cartas de poder dos deuses
- [ ] Implementar rastreamento de cartas usadas/disponíveis
- [ ] Implementar sistema de sacrifício de exércitos do Valhalla
- [ ] Implementar invocação de poderes (antes da rolagem)

[↑ Voltar ao topo](#-visão-geral)

---

### 3. Tipos de Dados e Enums

#### 3.1 Componentes do Jogo
- [x] Criar enum `ArmyType` (Guerreiro, EmblemaDoCla)
- [x] Criar enum `TerritoryType` (ComPorto, SemPorto)
- [ ] Criar enum `RegionType` (regiões do tabuleiro)
- [x] Criar enum `GodType` (Odin, Thor, Loki, etc.)
- [x] Criar enum `CommandEffectType` (GritoDeBatalha, AguasSangrentas, ParedeDeEscudos, PreceDaGuerra)
- [x] Criar classe `Territory` (nome, tipo, porto, ocupação)
- [x] Criar classe `Army` (tipo, quantidade, localização)
- [x] Criar classe `Ship` (localização, porto)

#### 3.2 Sistema de Combate
- [x] Criar enum `DiceColor` (Vermelho, Amarelo)
- [x] Criar classe `CombatResult` (rolagens, comparações, perdas)
- [x] Implementar lógica de rolagem de dados (máx 3 dados por lado)
- [x] Implementar resolução de combate (`ResolveCombat()`)
- [x] Implementar aplicação de perdas (`ApplyCombatLosses()`)
- [x] Implementar movimento de exércitos após conquista (`MoveArmiesAfterConquest()`)

[↑ Voltar ao topo](#-visão-geral)

---

### 4. Grafos de Decisão (Árvores de Decisão)

#### 4.1 Estrutura Base dos Grafos
- [x] Criar classes base de nós (`Node`, `StartNode`, `EndNode`, etc.)
- [x] Criar classes de nós: `PerformActionNode`, `BinaryConditionNode`, `MultipleChoiceNode`, `JumpToGraphNode`
- [x] Decidir formato de grafos (JSON ou classes C#) - Classes C#
- [x] Criar sistema de carregamento de grafos (via classes estáticas)
- [x] Criar primeiro grafo de teste (`TestGraph.cs`)
- [ ] Criar novos tipos de nós específicos se necessário

#### 4.2 Fases do Turno
- [x] **Fase 1: Recebimento de Exércitos**
  - [x] Calcular exércitos por territórios (÷2, min 3)
  - [x] Calcular exércitos por regiões conquistadas
  - [x] Verificar necessidade de troca de cartas (5+ cartas)
  - [x] Implementar troca de cartas (3 iguais ou 3 diferentes)
  - [ ] Alocar exércitos recebidos (interface de alocação pendente)

- [x] **Fase 2: Ataques**
  - [x] Identificar territórios atacáveis (contíguos, min 2 exércitos)
  - [x] Selecionar alvos prioritários (estrutura pronta, seleção automática temporária)
  - [x] Resolver combates (rolagem de dados)
  - [ ] Aplicar poderes dos deuses (se invocados) - estrutura pronta
  - [ ] Aplicar efeitos de comando (se comandante presente) - estrutura pronta
  - [x] Conquistar territórios (se defensor eliminado)
  - [x] Mover exércitos para território conquistado

- [x] **Fase 3: Deslocamento de Exércitos**
  - [x] Identificar possibilidades de deslocamento
  - [x] Selecionar deslocamentos estratégicos (estrutura pronta, seleção automática temporária)
  - [x] Executar deslocamento (1 por turno, exceto após conquista)

- [ ] **Fase 4: Recebimento de Carta de Território**
  - [ ] Verificar se conquistou território adversário
  - [ ] Receber carta de território
  - [ ] Verificar se acumulou 5+ cartas (forçar troca)

#### 4.3 Sub-grafos Especializados
- [x] **CombatGraph.cs** - Resolução de combate terrestre
  - [x] Rolagem de dados (vermelhos vs amarelos)
  - [x] Comparação de resultados (maior com maior, etc.)
  - [x] Aplicação de perdas
  - [ ] Decisão de enviar para Valhalla ou reserva (estrutura pronta)

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

- [x] **CardTradeGraph.cs** - Sistema de troca de cartas
  - [x] Verificação de 3 cartas iguais
  - [x] Verificação de 3 cartas diferentes
  - [x] Cálculo de exércitos progressivos (4, 6, 8, 10...)
  - [x] Forçar troca se 5+ cartas (integrado no Phase1Graph)

[↑ Voltar ao topo](#-visão-geral)

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

[↑ Voltar ao topo](#-visão-geral)

---

### 6. Interface CLI (Command Line Interface)

#### 6.1 Adaptação do CLI
- [x] Criar classe `CliInterface` ou usar `System.Console`
- [x] Usar `System.Console` (biblioteca CLI opcional para futuro)
- [x] Atualizar mensagens de boas-vindas
- [x] Atualizar mensagens de ajuda
- [x] Implementar comandos especiais (help, undo, exit)
- [ ] Implementar comandos adicionais (reset, phase)

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

[↑ Voltar ao topo](#-visão-geral)

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

[↑ Voltar ao topo](#-visão-geral)

---

### 8. Sistema de Navegação (GraphCrawler)

#### 8.1 Adaptação do Crawler
- [x] Criar classe `GraphCrawler` para War Vikings
- [x] Manter sistema de auto-navegação
- [x] Manter sistema de undo
- [x] Adaptar para novos tipos de estado
- [ ] Implementar navegação assíncrona (se necessário)

#### 8.2 Nós Específicos
- [ ] Criar nós para verificação de territórios
- [ ] Criar nós para verificação de exércitos
- [ ] Criar nós para verificação de regiões
- [ ] Criar nós para verificação de Valhalla
- [ ] Criar nós para verificação de navios

[↑ Voltar ao topo](#-visão-geral)

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

[↑ Voltar ao topo](#-visão-geral)

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

[↑ Voltar ao topo](#-visão-geral)

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

[↑ Voltar ao topo](#-visão-geral)

---

## 📅 Próximos Passos (Roadmap)

### Fase 1: Fundação (Atual)
- [x] Assimilar projeto legado
- [x] Assimilar regras de War Vikings
- [x] Criar documento de progresso
- [x] Definir arquitetura detalhada
- [x] Criar estrutura base do projeto

### Fase 2: Core System ✅ COMPLETO
- [x] Implementar tipos de dados básicos
- [x] Implementar sistema de estado
- [x] Adaptar GraphCrawler
- [x] Criar primeiro grafo de teste
- [x] Criar interface CLI básica
- [x] Integrar tudo no Program.cs
- [x] Validar sistema completo

### Fase 3: Mecânicas Principais
- [ ] Implementar sistema de combate
- [x] Implementar recebimento de exércitos (Fase 1 completa)
- [x] Implementar sistema de cartas (troca de cartas implementada)
- [ ] Implementar Valhalla (estrutura base pronta, falta lógica de invocação)

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

[↑ Voltar ao topo](#-visão-geral)

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

### Documentação de Implementação
**IMPORTANTE:** Toda implementação de código será acompanhada de uma explicação detalhada que inclui:

1. **O que foi implementado:**
   - Descrição clara do componente, classe ou funcionalidade criada
   - Estrutura de dados utilizada
   - Propósito e responsabilidades

2. **Como funciona (Lógica do código):**
   - Explicação passo a passo do funcionamento interno
   - Fluxo de execução
   - Relações entre componentes
   - Algoritmos e decisões de design

3. **Qual regra do jogo foi implementada:**
   - Referência específica às regras do jogo (com citações de `help/regras.md`)
   - Mapeamento entre código e regras
   - Validações e restrições implementadas
   - Exceções e casos especiais tratados

**Formato e Localização:**
- As explicações serão fornecidas **neste chat** após cada etapa de implementação
- Formato padrão:
```
## Explicação: [Nome do Componente]

### O que foi implementado
[Descrição do componente]

### Lógica do código
[Explicação detalhada do funcionamento]

### Regras implementadas
[Referências às regras do jogo com citações]
```

**Objetivo:**
- Garantir rastreabilidade e compreensão do código
- Facilitar manutenção futura
- Documentar decisões de design
- Validar implementação contra as regras do jogo

[↑ Voltar ao topo](#-visão-geral)

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

[↑ Voltar ao topo](#-visão-geral)

---

## 🔄 Histórico de Atualizações

### 20/12/2025 - Início do Projeto
- **Data:** 20/12/2025
- **Ação:** Criação do documento de progresso
- **Status:** Planejamento inicial completo

### 20/12/2025 - Core System Implementado
- **Data:** 20/12/2025
- **Ação:** Implementação completa do Core System
- **Status:** 
  - ✅ Estrutura base do projeto (.NET 8.0)
  - ✅ Tipos de dados básicos (enums e classes)
  - ✅ Sistema de Estado (`WarVikingsState`)
  - ✅ Classes base de grafos de decisão (Node, Graph, etc.)
  - ✅ GraphCrawler implementado
  - 📝 Documentação de explicação criada (`explicacao-projeto.md`)

### 20/12/2025 - Sistema Validado com Grafo de Teste
- **Data:** 20/12/2025
- **Ação:** Criação do primeiro grafo funcional e interface CLI
- **Status:**
  - ✅ Grafo de teste criado (`TestGraph.cs`)
  - ✅ Interface CLI implementada (`CliInterface.cs`)
  - ✅ Integração completa no `Program.cs`
  - ✅ Sistema validado e funcionando
  - ✅ Comandos especiais implementados (help, undo, exit)
  - 📝 Roteiro de testes criado (`TESTES.md`)

### 30/12/2025 - Testes Completos - Sistema 100% Validado
- **Data:** 30/12/2025
- **Ação:** Execução completa de todos os testes
- **Status:**
  - ✅ **19/19 testes concluídos (100%)**
  - ✅ Testes automatizados (compilação e execução)
  - ✅ Testes manuais do grafo (todos os nós)
  - ✅ Testes da Interface CLI (todos os comandos)
  - ✅ Testes do GraphCrawler (validação via código)
  - ✅ Testes do Sistema de Estado (validação via código)
  - ✅ Testes de Integração (validação via código + testes manuais)
  - ✅ Sistema completamente validado e pronto para implementação das mecânicas do jogo

### 30/12/2025 - Fase 1 - Recebimento de Exércitos Implementada
- **Data:** 30/12/2025
- **Ação:** Implementação completa da Fase 1 do turno
- **Status:**
  - ✅ Métodos adicionados no `WarVikingsState` para cálculo de exércitos por regiões
  - ✅ Métodos adicionados para verificação e processamento de troca de cartas
  - ✅ `Phase1Graph.cs` criado e implementado
  - ✅ `CardTradeGraph.cs` criado como sub-grafo de troca de cartas
  - ✅ Grafos integrados no `Program.cs`
  - ✅ Sistema compilando sem erros
  - 📝 Documentação atualizada (`EXPLICACAO-PROJETO.md` e `PROGRESSO-PROJETO.md`)
  - ⚠️ Alocação de exércitos ainda requer interface de seleção de territórios

### 30/12/2025 - Fase 2 - Ataques e Lógica de Combate Implementada
- **Data:** 30/12/2025
- **Ação:** Implementação da Fase 2 e lógica de combate
- **Status:**
  - ✅ `Phase2Graph.cs` criado e implementado
  - ✅ `CombatGraph.cs` criado com lógica de combate
  - ✅ `ExecuteActionNode` criado para executar ações no estado
  - ✅ Métodos de combate implementados no `WarVikingsState`:
    - `ResolveCombat()` - rolagem e resolução de combate
    - `ApplyCombatLosses()` - aplicação de perdas
    - `MoveArmiesAfterConquest()` - movimento após conquista
  - ✅ Propriedades temporárias adicionadas para armazenar estado do combate atual
  - ✅ `GraphCrawler` atualizado para executar ações via `ExecuteActionNode`
  - ✅ Sistema compilando sem erros
  - ⚠️ Seleção de territórios ainda automática (primeiro disponível)
  - ⚠️ Verificação de conquista ainda requer input manual (pode ser automatizada)
  - ⚠️ Efeitos de comando e poderes dos deuses têm estrutura pronta mas não aplicados na rolagem

[↑ Voltar ao topo](#-visão-geral)

---

## ✅ Checklist Rápido

- [x] Estrutura base criada (.NET project)
- [x] `.csproj` configurado
- [x] Tipos de dados definidos (enums e classes)
- [x] Sistema de estado implementado (`WarVikingsState`)
- [x] Motor de grafos implementado (`Graph`, `Node`, etc.)
- [x] GraphCrawler implementado
- [x] Primeiro grafo funcionando (`TestGraph.cs`)
- [x] CLI adaptado (`CliInterface`)
- [ ] Sistema de combate implementado
- [ ] Todas as fases do turno implementadas
- [ ] Mecânicas especiais implementadas
- [ ] Documentação completa
- [ ] Testes realizados (xUnit ou NUnit)
- [ ] Projeto pronto para uso (`dotnet run` ou executável)

[↑ Voltar ao topo](#-visão-geral)

---

**Última atualização:** 30/12/2025 - Fase 3 - Deslocamento de Exércitos Implementada  
**Mantido por:** Equipe de Desenvolvimento War Vikings Bot

