# Explicação do Projeto: War Vikings Bot

[↑ Voltar ao topo](#explicação-do-projeto-war-vikings-bot)

Este documento contém explicações detalhadas de cada componente implementado no projeto, incluindo:
- O que foi implementado
- Como funciona (lógica do código)
- Qual regra do jogo foi implementada

---

## 📋 Índice

1. [Estrutura Base do Projeto](#1-estrutura-base-do-projeto)
2. [Tipos de Dados Básicos](#2-tipos-de-dados-básicos)
3. [Sistema de Estado (WarVikingsState)](#3-sistema-de-estado-warvikingsstate)
4. [Classes Base de Grafos de Decisão](#4-classes-base-de-grafos-de-decisão)
5. [GraphCrawler - Navegador de Grafos](#5-graphcrawler---navegador-de-grafos)
6. [Grafos de Fase do Turno](#6-grafos-de-fase-do-turno)

---

## 1. Estrutura Base do Projeto

### O que foi implementado

Criação da estrutura inicial do projeto .NET 8.0, incluindo:
- Projeto console application
- Estrutura de diretórios organizada
- Arquivos de configuração básicos

### Estrutura de Diretórios

```
WarVikingsBot/
├── src/
│   ├── Models/      # Modelos de dados (enums, classes)
│   ├── Graphs/      # Classes de grafos de decisão
│   ├── State/       # Gerenciamento de estado do jogo
│   ├── Cli/         # Interface de linha de comando
│   └── Crawler/     # Navegador de grafos
├── Graphs/          # Definições de grafos (JSON ou C#)
├── Program.cs       # Ponto de entrada
└── WarVikingsBot.csproj
```

### Lógica do código

O projeto foi criado usando `dotnet new console`, configurado para .NET 8.0 (versão LTS estável). A estrutura de diretórios foi organizada para separar responsabilidades:
- **Models**: Define os tipos de dados do jogo
- **Graphs**: Implementa o sistema de grafos de decisão
- **State**: Gerencia o estado do jogo durante a partida
- **Cli**: Interface com o usuário
- **Crawler**: Navega pelos grafos automaticamente

### Regras implementadas

Nenhuma regra específica do jogo foi implementada nesta etapa. Esta é a base estrutural que permite a implementação das regras do jogo nas etapas seguintes.

[↑ Voltar ao topo](#-índice)

---

## 2. Tipos de Dados Básicos

### O que foi implementado

Criação dos enums e classes fundamentais que representam os componentes do jogo War Vikings.

### Enums criados

#### `ArmyType`
```csharp
public enum ArmyType
{
    Guerreiro = 1,        // Representa 1 exército
    EmblemaDoCla = 5      // Representa 5 exércitos
}
```

#### `CommandEffectType`
```csharp
public enum CommandEffectType
{
    GritoDeBatalha,       // Rerrolar 1 dado de ataque
    AguasSangrentas,      // Rerrolar em combate naval
    ParedeDeEscudos,      // Rerrolar 1 dado de defesa
    PreceDaGuerra         // Ignorar carta, embaralhar, comprar nova
}
```

#### `DiceColor`
```csharp
public enum DiceColor
{
    Vermelho,    // Dados do atacante
    Amarelo      // Dados do defensor
}
```

#### `TerritoryType`
```csharp
public enum TerritoryType
{
    ComPorto,    // Território com porto (ícone de barco)
    SemPorto     // Território sem porto
}
```

#### `GodType`
```csharp
public enum GodType
{
    Odin,    // Pai dos deuses
    Thor,    // Deus do trovão
    Loki,    // Deus da trapaça
    Freyja   // Deusa do amor e da guerra
}
```

### Classes criadas

#### `Territory`
Representa um território no tabuleiro com:
- Nome, tipo (com/sem porto), ocupação, exércitos
- Região à qual pertence
- Territórios adjacentes
- Propriedades auxiliares (`IsOccupied`, `CanAttack`, `HasPort`)

#### `Army`
Representa um exército com:
- Tipo (Guerreiro ou Emblema do Clã)
- Valor (quantidade de exércitos que representa)
- Localização (território)
- Jogador dono

#### `Ship`
Representa um navio de guerra (Hersekskip) com:
- ID, localização (território com porto)
- Jogador dono
- Estado (em combate, destruído)

#### `CombatResult`
Representa o resultado de um combate com:
- Rolagens de dados (vermelhos e amarelos)
- Comparações realizadas
- Perdas de exércitos
- Indicação de conquista de território

#### `DiceComparison`
Representa uma comparação individual entre dois dados:
- Valores do atacante e defensor
- Indicação de quem venceu (empate = vitória do defensor)

### Lógica do código

Os enums usam valores numéricos quando faz sentido (`ArmyType` usa 1 e 5 para representar o valor do exército). As classes usam propriedades auto-implementadas e propriedades calculadas (`HasPort`, `IsOccupied`, `CanAttack`) para facilitar o acesso a informações derivadas.

### Regras implementadas

**Regra: Tipos de Exércitos** (regras.md, linha 22)
> "O **Guerreiro** representa 1 exército. O **Emblema do Clã** representa 5 exércitos."

**Regra: Efeitos de Comando** (regras.md, linha 27)
> "Quatro efeitos diferentes influenciam a partida; um é sorteado no início."

**Regra: Dados de Combate** (regras.md, linha 30)
> "**Dados vermelhos** são usados pelo atacante, e **dados amarelos** pelo defensor."

**Regra: Territórios com Portos** (regras.md, linha 25)
> "Alguns territórios possuem **portos** (ícone de barco)."

**Regra: Deuses** (regras.md, linha 28)
> "Cartas de Poder dos Deuses - 12 (3 por deus)"

**Regra: Navios de Guerra** (regras.md, linha 24)
> "Navios de Guerra (Hersekskip) - 5 por jogador"

**Regra: Empate no Combate** (regras.md, linha 53)
> "A vitória é definida por quem tiver mais pontos no dado, e em caso de empate, a vitória é da defesa"

[↑ Voltar ao topo](#-índice)

---

## 3. Sistema de Estado (WarVikingsState)

### O que foi implementado

A classe `WarVikingsState` gerencia todo o estado do jogo durante uma partida. Ela armazena e fornece acesso a todas as informações necessárias para o bot tomar decisões.

### Estrutura de Dados

#### Propriedades Principais

```csharp
public Dictionary<string, Territory> Territories { get; set; }
```
- Armazena todos os territórios do tabuleiro
- Chave: nome do território
- Valor: objeto `Territory` completo

```csharp
public Dictionary<int, List<Army>> PlayerArmies { get; set; }
```
- Armazena exércitos por jogador
- Chave: ID do jogador
- Valor: lista de exércitos do jogador

```csharp
public Dictionary<int, int> ValhallaArmies { get; set; }
```
- Armazena quantidade de exércitos no Valhalla por jogador
- Chave: ID do jogador
- Valor: quantidade (máximo 6)

```csharp
public Dictionary<int, List<Ship>> PlayerShips { get; set; }
```
- Armazena navios por jogador
- Chave: ID do jogador
- Valor: lista de navios (máximo 5)

```csharp
public Dictionary<int, string> CommanderLocation { get; set; }
```
- Armazena localização do comandante por jogador
- Chave: ID do jogador
- Valor: nome do território onde está o comandante

```csharp
public Dictionary<int, List<string>> TerritoryCards { get; set; }
```
- Armazena cartas de território por jogador
- Chave: ID do jogador
- Valor: lista de nomes de cartas

```csharp
public Dictionary<int, string> ObjectiveCards { get; set; }
```
- Armazena carta-objetivo de cada jogador (mantida em segredo)

```csharp
public CommandEffectType ActiveCommandEffect { get; set; }
```
- Armazena o efeito de comando sorteado no início
- Válido para todos os comandantes

### Métodos Auxiliares Implementados

#### `CalculateArmiesFromTerritories(int playerId)`

**Lógica:**
```csharp
var territoryCount = GetPlayerTerritoryCount(playerId);
var armies = territoryCount / 2;
return territoryCount < 6 ? Math.Max(armies, 3) : armies;
```

1. Conta quantos territórios o jogador possui
2. Divide por 2 (arredondado para baixo)
3. Se tem menos de 6 territórios, retorna no mínimo 3
4. Se tem 6 ou mais, retorna exatamente `territórios / 2`

**Regra implementada:** (regras.md, linha 46)
> "Soma-se o número de territórios possuídos e divide-se por 2 (o resultado é arredondado para baixo). O mínimo de exércitos a receber é 3, a não ser que o jogador possua menos de 6 territórios."

#### `MustTradeCards(int playerId)`

**Lógica:**
```csharp
return GetTerritoryCardCount(playerId) >= 5;
```

Retorna `true` se o jogador tem 5 ou mais cartas, forçando a troca obrigatória.

**Regra implementada:** (regras.md, linha 48)
> "É obrigatório trocar se o jogador acumular 5 cartas."

#### `GetAttackableTerritories(int playerId)`

**Lógica:**
1. Obtém todos os territórios do jogador
2. Para cada território, verifica se pode atacar (pelo menos 2 exércitos)
3. Para cada território adjacente, verifica se está ocupado por outro jogador
4. Adiciona à lista de territórios atacáveis

**Regra implementada:** (regras.md, linha 51)
> "O ataque é anunciado contra um território inimigo contíguo, desde que o atacante tenha no mínimo 2 exércitos no território de origem (sendo 1 o exército de ocupação, que não ataca)."

#### `CanAddToValhalla(int playerId)`

**Lógica:**
```csharp
return GetValhallaArmyCount(playerId) < 6;
```

Retorna `true` apenas se o jogador tem menos de 6 exércitos no Valhalla.

**Regra implementada:** (regras.md, linha 26)
> "O máximo de exércitos que um jogador pode ter no Valhalla é seis."

#### `CanBuildShip(int playerId)`

**Lógica:**
```csharp
return GetValhallaArmyCount(playerId) >= 1 && 
       GetPlayerShipCount(playerId) < 5;
```

Requer:
- Pelo menos 1 exército no Valhalla (para sacrificar)
- Menos de 5 navios (limite máximo)

**Regra implementada:** (regras.md, linha 115)
> "Uma embarcação é construída sacrificando 1 exército do Valhalla"
> "Cada jogador tem 5 navios em sua reserva"

#### `CanUseCommandEffect(int playerId, string territoryName)`

**Lógica:**
```csharp
return HasCommanderInTerritory(playerId, territoryName);
```

Verifica se o comandante do jogador está no território especificado.

**Regra implementada:** (regras.md, linha 91)
> "Os Efeitos de Comando são utilizados exclusivamente em combates que envolvam territórios onde o Comandante do jogador está presente."

#### `GetConqueredRegions(int playerId)`

**Lógica:**
```csharp
var allRegions = Territories.Values.Select(t => t.Region).Distinct().ToList();
foreach (var region in allRegions)
{
    var regionTerritories = Territories.Values.Where(t => t.Region == region).ToList();
    bool allConquered = regionTerritories.All(t => t.OccupiedByPlayer == playerId);
    if (allConquered)
        conqueredRegions.Add(region);
}
```

1. Obtém todas as regiões únicas do tabuleiro
2. Para cada região, verifica se todos os territórios pertencem ao jogador
3. Retorna lista de regiões completamente conquistadas

**Regra implementada:** (regras.md, linha 47)
> "Recebe exércitos extras se possuir uma Região inteira (valor conforme a tabela no tabuleiro)."

#### `CalculateArmiesFromRegions(int playerId)`

**Lógica:**
```csharp
var conqueredRegions = GetConqueredRegions(playerId);
int totalArmies = 0;
foreach (var region in conqueredRegions)
{
    totalArmies += 2; // Valor temporário - será substituído por valores da tabela
}
return totalArmies;
```

1. Obtém todas as regiões conquistadas
2. Para cada região, adiciona exércitos (valores temporários, aguardando tabela do tabuleiro)
3. Retorna total de exércitos por regiões

**Regra implementada:** (regras.md, linha 47)
> "Recebe exércitos extras se possuir uma Região inteira (valor conforme a tabela no tabuleiro). Estes exércitos devem ser distribuídos obrigatoriamente nesta Região."

**Nota:** Valores atuais são temporários (2 exércitos por região). Serão substituídos pelos valores reais da tabela do tabuleiro.

#### `CanTradeCards(int playerId)`

**Lógica:**
```csharp
return GetTerritoryCardCount(playerId) >= 3;
```

Verifica se o jogador tem pelo menos 3 cartas (mínimo necessário para trocar).

**Regra implementada:** (regras.md, linha 48)
> "A troca de cartas (3 cartas com figuras iguais ou 3 cartas com figuras diferentes) garante exércitos."

#### `HasThreeSameCards(int playerId)`

**Lógica:**
```csharp
var cards = TerritoryCards[playerId];
var grouped = cards.GroupBy(c => c);
return grouped.Any(g => g.Count() >= 3);
```

1. Obtém cartas do jogador
2. Agrupa cartas por nome
3. Verifica se algum grupo tem 3 ou mais cartas iguais

**Regra implementada:** (regras.md, linha 48)
> "A troca de cartas (3 cartas com figuras iguais ou 3 cartas com figuras diferentes) garante exércitos."

#### `HasThreeDifferentCards(int playerId)`

**Lógica:**
```csharp
var cards = TerritoryCards[playerId];
return cards.Distinct().Count() >= 3;
```

1. Obtém cartas do jogador
2. Conta quantas cartas diferentes existem
3. Retorna `true` se há pelo menos 3 cartas diferentes

**Regra implementada:** (regras.md, linha 48)
> "A troca de cartas (3 cartas com figuras iguais ou 3 cartas com figuras diferentes) garante exércitos."

#### `GetNextCardTradeArmies(int playerId)`

**Lógica:**
```csharp
var tradeCount = CardTradeCount.ContainsKey(playerId) ? CardTradeCount[playerId] : 0;
return 4 + (tradeCount * 2);
```

Calcula exércitos da próxima troca usando valores progressivos:
- 1ª troca: 4 exércitos
- 2ª troca: 6 exércitos
- 3ª troca: 8 exércitos
- 4ª troca: 10 exércitos
- E assim por diante...

**Regra implementada:** (regras.md, linha 48)
> "Os valores de exércitos por troca são progressivos (4, 6, 8, 10, etc.)."

#### `GetTradeableCards(int playerId)`

**Lógica:**
```csharp
// Verifica 3 cartas iguais
var grouped = cards.GroupBy(c => c);
foreach (var group in grouped)
{
    if (group.Count() >= 3)
        tradeable.AddRange(group.Take(3));
}

// Se não tem 3 iguais, verifica 3 diferentes
if (tradeable.Count == 0 && cards.Distinct().Count() >= 3)
{
    tradeable.AddRange(cards.Distinct().Take(3));
}
```

1. Primeiro verifica se há 3 cartas iguais
2. Se não encontrar, verifica se há 3 cartas diferentes
3. Retorna lista de cartas que podem ser trocadas

**Regra implementada:** (regras.md, linha 48)
> "A troca de cartas (3 cartas com figuras iguais ou 3 cartas com figuras diferentes) garante exércitos."

### Resumo das Regras Implementadas

| Regra | Método | Status |
|-------|--------|--------|
| Cálculo de exércitos por territórios (÷2, min 3) | `CalculateArmiesFromTerritories()` | ✅ |
| Troca obrigatória com 5+ cartas | `MustTradeCards()` | ✅ |
| Territórios atacáveis (contíguos, min 2 exércitos) | `GetAttackableTerritories()` | ✅ |
| Limite de Valhalla (máx 6) | `CanAddToValhalla()` | ✅ |
| Construção de navios (1 exército Valhalla, máx 5 navios) | `CanBuildShip()` | ✅ |
| Efeito de comando (comandante presente) | `CanUseCommandEffect()` | ✅ |
| Primeira rodada (sem ataques) | `IsFirstRound` | ✅ |
| Regiões conquistadas | `GetConqueredRegions()`, `CalculateArmiesFromRegions()` | ✅ |
| Verificação de troca de cartas | `CanTradeCards()`, `HasThreeSameCards()`, `HasThreeDifferentCards()` | ✅ |
| Cálculo de exércitos por troca (progressivo) | `GetNextCardTradeArmies()` | ✅ |
| Lista de cartas trocáveis | `GetTradeableCards()` | ✅ |

[↑ Voltar ao topo](#-índice)

---

## 4. Classes Base de Grafos de Decisão

### O que foi implementado

Sistema completo de grafos de decisão que permite representar árvores de decisão que o bot seguirá durante o jogo. Este sistema é o coração da arquitetura do bot, permitindo definir fluxos de decisão complexos de forma estruturada.

### Hierarquia de Classes

```
Node (abstrata)
├── NonInteractiveNode (abstrata)
│   ├── StartNode
│   ├── EndNode
│   ├── JumpToGraphNode
│   └── ReturnFromGraphNode
└── InteractiveNode (abstrata)
    ├── PerformActionNode
    ├── BinaryConditionNode
    └── MultipleChoiceNode
```

### Tipos de Nós Implementados

#### `Node` (Classe Base)
- Define propriedade `Id` para identificação única
- Método estático `IsValidId()` valida formato de IDs
- IDs devem começar com letra minúscula e conter apenas letras minúsculas, dígitos e underscore

#### `StartNode`
- Ponto de entrada de cada grafo
- Não requer interação do usuário
- Sempre avança automaticamente para o próximo nó

#### `EndNode`
- Ponto de saída do grafo
- Não tem próximo nó (`GetNext()` retorna `null`)
- Pode exibir mensagem final

#### `PerformActionNode`
- Exibe uma ação que o jogador deve executar
- Opção única: pressionar Enter (string vazia)
- Sempre avança para o próximo nó após confirmação

**Exemplo de uso:**
```
"Recupere seus dados de ação."
[Pressione Enter para continuar]
```

#### `BinaryConditionNode`
- Exibe uma pergunta sim/não
- Aceita "true"/"t" ou "false"/"f"
- Encaminha para `TrueNode` ou `FalseNode` conforme resposta

**Exemplo de uso:**
```
"Você tem mais de 6 cartas?"
[true/false] > true
→ Vai para nó de descarte de cartas
```

#### `MultipleChoiceNode`
- Exibe pergunta com múltiplas opções
- Gera opções numeradas (1, 2, 3, ...)
- Retorna nó correspondente ao índice escolhido

**Exemplo de uso:**
```
"Qual território atacar?"
1. Território A
2. Território B
3. Território C
[1/2/3] > 2
→ Vai para nó de ataque ao Território B
```

#### `JumpToGraphNode`
- Permite chamar outro grafo como sub-rotina
- `TargetGraphId` identifica o grafo destino
- `Next` é o nó para retornar após sub-grafo terminar
- Útil para modularizar lógica (ex: combate, troca de cartas)

**Exemplo de uso:**
```
Grafo principal: "phase_2"
  → JumpToGraphNode("combate")  // Chama grafo de combate
  → Após combate, retorna para Next
```

#### `ReturnFromGraphNode`
- Marca o retorno de um grafo chamado
- Usado em conjunto com `JumpToGraphNode`

### Classe `Graph`

Representa um grafo completo com:
- `Id`: Identificador único do grafo
- `RootNode`: Ponto de entrada (`StartNode`)
- `AllNodes`: Lista de todos os nós do grafo
- `GetNodeById()`: Busca nó por ID
- `GetJumpTargets()`: Lista grafos referenciados por saltos

### Lógica do código

O sistema funciona como um fluxograma:
1. Começa no `StartNode` (raiz)
2. Navega automaticamente por nós não-interativos
3. Para em nós interativos para aguardar resposta do usuário
4. Baseado na resposta, segue para próximo nó
5. Continua até chegar em um `EndNode`

**Fluxo de navegação:**
```
StartNode → PerformActionNode → BinaryConditionNode
                                    ├─ true → NodeA
                                    └─ false → NodeB
```

### Regras implementadas

Este sistema não implementa regras específicas do jogo diretamente. Ele fornece a **estrutura** para representar as decisões do bot.

**Conceito:** O sistema permite criar fluxogramas que representam a lógica de decisão do bot, seguindo o mesmo padrão do projeto Queller Bot original.

**Uso no jogo:**
- Cada fase do turno será um grafo separado
- Sub-grafos especializados para combate, troca de cartas, etc.
- Permite modularização e reutilização de lógica

### Resumo

| Componente | Função | Tipo |
|------------|--------|------|
| `Node` | Classe base | Abstrata |
| `StartNode` | Ponto de entrada | Não interativo |
| `EndNode` | Ponto de saída | Não interativo |
| `PerformActionNode` | Exibe ação | Interativo |
| `BinaryConditionNode` | Pergunta sim/não | Interativo |
| `MultipleChoiceNode` | Múltiplas opções | Interativo |
| `JumpToGraphNode` | Salto para outro grafo | Não interativo |
| `Graph` | Grafo completo | Container |

[↑ Voltar ao topo](#-índice)

---

## 5. GraphCrawler - Navegador de Grafos

### O que foi implementado

A classe `GraphCrawler` é o "motor" que navega automaticamente pelos grafos de decisão. Ela funciona como um leitor automático que percorre a árvore de decisão, acumulando mensagens e parando apenas quando precisa de interação do usuário.

### Estrutura da Classe

#### Propriedades Principais

```csharp
private Dictionary<string, Graph> _graphs;
```
- Armazena todos os grafos disponíveis
- Permite navegação entre grafos diferentes

```csharp
private WarVikingsState _state;
```
- Referência ao estado do jogo
- Permite que os nós acessem informações do jogo

```csharp
private Node? _currentNode;
```
- Nó atual sendo processado
- Muda conforme a navegação progride

```csharp
private StartNode? _rootNode;
```
- Nó raiz do grafo atual
- Usado para reiniciar navegação (undo)

```csharp
private Stack<Node> _jumpStack;
```
- Pilha de nós de salto (`JumpToGraphNode`)
- Permite rastrear chamadas aninhadas de grafos

```csharp
private List<string> _options;
```
- Histórico de opções escolhidas pelo usuário
- Usado para implementar funcionalidade de undo

```csharp
private string _messageBuffer;
```
- Buffer que acumula mensagens dos nós
- Exibido quando encontra nó interativo

### Métodos Principais

#### `AutoCrawl()`

**Lógica:**
```csharp
private void AutoCrawl()
{
    _messageBuffer = string.Empty;
    
    while (!IsAtEnd() && _currentNode != null)
    {
        AddToMessageBuffer(_currentNode);
        
        if (_currentNode is InteractiveNode)
            break;  // Para e espera interação
        
        _currentNode = GetNextNode(_currentNode);
    }
}
```

**Funcionamento:**
1. Limpa o buffer de mensagens
2. Enquanto não chegou ao fim e há nó atual:
   - Adiciona mensagem do nó ao buffer
   - Se o nó é interativo, para e aguarda resposta
   - Se não é interativo, avança automaticamente para o próximo
3. Repete até encontrar nó interativo ou fim do grafo

**Exemplo de fluxo:**
```
StartNode → PerformActionNode → PerformActionNode → BinaryConditionNode
                                                      ↑ PARA AQUI
Buffer: "Ação 1\nAção 2\nPergunta?"
```

#### `Proceed(string option)`

**Lógica:**
```csharp
public void Proceed(string option)
{
    if (_currentNode is InteractiveNode interactiveNode)
    {
        _options.Add(option);  // Salva escolha
        _currentNode = interactiveNode.GetNext(option);  // Vai para próximo
        AutoCrawl();  // Continua navegação automática
    }
}
```

**Funcionamento:**
1. Verifica se nó atual é interativo
2. Salva a opção escolhida no histórico
3. Obtém próximo nó baseado na opção
4. Continua navegação automática (`AutoCrawl()`)

**Exemplo:**
```
Usuário escolhe "true" em BinaryConditionNode
→ Salva "true" no histórico
→ Vai para TrueNode
→ AutoCrawl() continua até próximo nó interativo
```

#### `Undo()`

**Lógica:**
```csharp
public void Undo()
{
    if (!CanUndo())
        return;
    
    _options.RemoveAt(_options.Count - 1);  // Remove última escolha
    
    // Reinicia navegação do início
    _currentNode = _rootNode;
    _jumpStack.Clear();
    _messageBuffer = string.Empty;
    
    AutoCrawl();
    
    // Reaplica todas as escolhas anteriores
    foreach (var option in _options)
    {
        if (_currentNode is InteractiveNode interactiveNode)
        {
            _currentNode = interactiveNode.GetNext(option);
            AutoCrawl();
        }
    }
}
```

**Funcionamento:**
1. Remove última opção do histórico
2. Reinicia navegação do nó raiz
3. Limpa pilha de saltos e buffer
4. Reaplica todas as escolhas anteriores em ordem
5. Resultado: estado anterior à última escolha

**Exemplo:**
```
Histórico: ["true", "2", "false"]
Undo() → Remove "false"
→ Reinicia do StartNode
→ Reaplica "true" → "2"
→ Estado: após escolha "2", antes de "false"
```

#### `GetNextNode(Node node)`

**Lógica:**
```csharp
private Node? GetNextNode(Node node)
{
    if (node is NonInteractiveNode nonInteractiveNode)
    {
        var next = nonInteractiveNode.GetNext();
        
        if (node is JumpToGraphNode jumpNode)
        {
            HandleJump(jumpNode);
            return GetNextNode(jumpNode);
        }
        
        if (node is ReturnFromGraphNode)
        {
            return HandleReturn();
        }
        
        return next;
    }
    
    return null;
}
```

**Funcionamento:**
1. Verifica se nó é não-interativo
2. Obtém próximo nó
3. Se é `JumpToGraphNode`, trata salto para outro grafo
4. Se é `ReturnFromGraphNode`, trata retorno de grafo chamado
5. Retorna próximo nó

#### `HandleJump(JumpToGraphNode jumpNode)`

**Lógica:**
```csharp
private void HandleJump(JumpToGraphNode jumpNode)
{
    _jumpStack.Push(jumpNode);  // Salva nó de salto na pilha
    
    var targetGraph = _graphs[jumpNode.TargetGraphId];
    _currentNode = targetGraph.RootNode;  // Vai para raiz do grafo destino
}
```

**Funcionamento:**
1. Empilha nó de salto (para retornar depois)
2. Busca grafo destino pelo ID
3. Muda nó atual para raiz do grafo destino
4. Navegação continua no novo grafo

**Exemplo:**
```
Grafo A: JumpToGraphNode("combate")
→ Empilha JumpToGraphNode
→ Vai para StartNode do grafo "combate"
→ Navega grafo de combate
→ ReturnFromGraphNode
→ Desempilha e retorna para Next do JumpToGraphNode
```

#### `HandleReturn()`

**Lógica:**
```csharp
private Node? HandleReturn()
{
    if (_jumpStack.Count == 0)
        return null;
    
    var jumpNode = _jumpStack.Pop();  // Recupera nó de salto
    if (jumpNode is JumpToGraphNode jump)
    {
        return jump.GetNext();  // Retorna para próximo nó após salto
    }
    return null;
}
```

**Funcionamento:**
1. Verifica se há nó de salto na pilha
2. Desempilha nó de salto
3. Retorna próximo nó do nó de salto (continuação após sub-grafo)

### Lógica do código

O `GraphCrawler` implementa o padrão **State Machine** (Máquina de Estados):
- **Estado**: Nó atual no grafo
- **Transição**: Resposta do usuário ou navegação automática
- **Ação**: Acumular mensagens, processar nós

**Fluxo completo:**
```
1. Inicialização
   → Cria crawler com grafo inicial
   → AutoCrawl() navega até primeiro nó interativo

2. Loop principal
   → Exibe mensagem acumulada
   → Aguarda resposta do usuário
   → Proceed() com resposta
   → AutoCrawl() continua navegação
   → Repete até EndNode

3. Undo
   → Remove última escolha
   → Reinicia do início
   → Reaplica escolhas anteriores
```

### Regras implementadas

Este componente não implementa regras específicas do jogo diretamente. Ele implementa a **mecânica de navegação** que permite executar as regras definidas nos grafos.

**Conceito:** O `GraphCrawler` é o "executor" dos grafos de decisão. Ele:
- Lê os grafos (árvores de decisão)
- Navega automaticamente por nós não-interativos
- Para em nós interativos para aguardar resposta
- Gerencia saltos entre grafos (modularização)
- Permite desfazer escolhas (undo)

**Uso no jogo:**
- Cada fase do turno será executada através de um grafo
- O crawler navega pelo grafo guiando o jogador
- Sub-grafos permitem reutilizar lógica (combate, troca de cartas, etc.)
- Undo permite corrigir erros ou testar diferentes escolhas

### Resumo

| Funcionalidade | Método | Descrição |
|----------------|--------|-----------|
| Navegação automática | `AutoCrawl()` | Percorre nós não-interativos automaticamente |
| Processamento de escolhas | `Proceed()` | Avança baseado na resposta do usuário |
| Desfazer escolhas | `Undo()` | Volta para estado anterior |
| Salto entre grafos | `HandleJump()` | Chama outro grafo como sub-rotina |
| Retorno de grafos | `HandleReturn()` | Retorna de grafo chamado |
| Acumulação de mensagens | `AddToMessageBuffer()` | Coleta mensagens para exibição |

[↑ Voltar ao topo](#-índice)

---

## 📝 Notas

Este documento será atualizado continuamente conforme novas funcionalidades são implementadas. Cada nova seção seguirá o mesmo formato:
- O que foi implementado
- Lógica do código
- Regras implementadas

---

---

## 6. Grafos de Fase do Turno

### O que foi implementado

Implementação dos grafos que representam as fases do turno do jogo War Vikings. Cada fase é um grafo separado que guia o jogador através das ações necessárias.

### Phase1Graph - Fase 1: Recebimento de Exércitos

#### Estrutura do Grafo

O grafo `Phase1Graph` implementa a primeira fase do turno, onde o jogador recebe novos exércitos de três fontes:
1. **Territórios possuídos** (÷2, mínimo 3)
2. **Regiões conquistadas** (valores da tabela)
3. **Troca de cartas** (progressivo: 4, 6, 8, 10...)

#### Fluxo do Grafo

```
StartNode
  ↓
PerformActionNode: "FASE 1: RECEBIMENTO DE EXÉRCITOS"
  ↓
BinaryConditionNode: "Você tem 5 ou mais cartas?"
  ├─ true → PerformActionNode: "Você DEVE trocar cartas"
  │         ↓
  │         JumpToGraphNode("card_trade")
  │         ↓
  └─ false → BinaryConditionNode: "Você quer trocar cartas? (opcional)"
              ├─ true → JumpToGraphNode("card_trade")
              └─ false → (pula troca)
                        ↓
PerformActionNode: "Calculando exércitos por territórios..."
  ↓
PerformActionNode: "Calculando exércitos por regiões..."
  ↓
PerformActionNode: "Total de exércitos recebidos calculado."
  ↓
PerformActionNode: "Aloque os exércitos recebidos nos seus territórios."
  ↓
EndNode: "Fase 1 concluída."
```

#### Lógica do código

O grafo verifica primeiro se o jogador tem 5 ou mais cartas (troca obrigatória). Se não tiver, oferece troca opcional. Após a troca (ou se não trocou), calcula e exibe os exércitos recebidos por territórios e regiões, permitindo que o jogador aloque os exércitos.

### Phase2Graph - Fase 2: Ataques

#### Estrutura do Grafo

O grafo `Phase2Graph` implementa a segunda fase do turno, onde o jogador pode realizar ataques contra territórios inimigos.

#### Fluxo do Grafo

```
StartNode
  ↓
PerformActionNode: "FASE 2: ATAQUES"
  ↓
BinaryConditionNode: "É a primeira rodada?"
  ├─ true → PerformActionNode: "Primeira rodada: não há ataques"
  │         ↓
  │         EndNode
  └─ false → BinaryConditionNode: "Você tem territórios que podem atacar?"
              ├─ false → PerformActionNode: "Você não tem territórios atacáveis"
              │         ↓
              │         EndNode
              └─ true → BinaryConditionNode: "Você quer realizar um ataque?"
                          ├─ false → PerformActionNode: "Você decidiu não atacar"
                          │         ↓
                          │         EndNode
                          └─ true → ExecuteActionNode: "set_combat_source"
                                    ↓
                                    ExecuteActionNode: "set_combat_target"
                                    ↓
                                    JumpToGraphNode("combat")
                                    ↓
                                    PerformActionNode: "Combate resolvido"
                                    ↓
                                    (loop de volta para perguntar se quer atacar novamente)
```

#### Lógica do código

O grafo verifica se é a primeira rodada (sem ataques). Se não for, verifica se há territórios que podem atacar (mínimo 2 exércitos). Se houver, pergunta se o jogador quer atacar. Se sim, define os territórios de origem e alvo (atualmente usa o primeiro disponível) e chama o `CombatGraph` para resolver o combate. Após o combate, pergunta novamente se quer atacar (loop).

**Regra implementada:** (regras.md, linhas 50-54)
> "**2. Ataques (Combate Terrestre):**
> - O ataque é anunciado contra um território inimigo contíguo (ou por linha pontilhada), desde que o atacante tenha no mínimo 2 exércitos no território de origem (sendo 1 o exército de ocupação, que não ataca).
> - O atacante pode usar no máximo 3 dados vermelhos, e o defensor, no máximo 3 dados amarelos, limitados pelo número de exércitos.
> - **Resolução de Combate (Rolagem de Dados):** Comparam-se os dados de maior ponto do atacante com os de maior ponto do defensor, e assim sucessivamente (segundo maior com segundo maior, etc.). **A vitória é definida por quem tiver mais pontos no dado, e em caso de empate, a vitória é da defesa**. O perdedor perde 1 exército. Os exércitos perdidos retornam à reserva do jogador.
> - **Conquista de Território:** Ocorre quando todos os exércitos defensores são destruídos. O atacante deve mover exércitos para o território conquistado (mínimo 1, máximo 3, e nunca mais do que os exércitos que participaram do ataque)."

### CombatGraph - Sub-grafo de Resolução de Combate

#### Estrutura do Grafo

O grafo `CombatGraph` implementa a resolução de um combate terrestre entre atacante e defensor.

#### Fluxo do Grafo

```
StartNode
  ↓
PerformActionNode: "RESOLUÇÃO DE COMBATE"
  ↓
BinaryConditionNode: "O comandante está presente?"
  ├─ true → PerformActionNode: "Efeito de Comando disponível"
  └─ false → (pula)
            ↓
BinaryConditionNode: "Você quer invocar poder dos deuses?"
  ├─ true → PerformActionNode: "Poder dos deuses será invocado"
  └─ false → (pula)
            ↓
ExecuteActionNode: "resolve_combat" (rola dados, compara, calcula perdas)
  ↓
PerformActionNode: "Resultados da rolagem calculados"
  ↓
PerformActionNode: "Comparando dados..."
  ↓
PerformActionNode: "Perdas calculadas"
  ↓
ExecuteActionNode: "apply_combat_losses" (aplica perdas ao estado)
  ↓
BinaryConditionNode: "O território foi conquistado?"
  ├─ true → PerformActionNode: "Território conquistado!"
            ↓
            ExecuteActionNode: "move_armies_after_conquest" (move exércitos)
            ↓
            EndNode
  └─ false → PerformActionNode: "Território não foi conquistado"
            ↓
            EndNode
```

#### Lógica do código

O grafo verifica se há comandante (para efeitos de comando) e se o jogador quer usar poder dos deuses. Depois, executa a resolução do combate (`resolve_combat`) que rola os dados, compara e calcula perdas. Em seguida, aplica as perdas ao estado (`apply_combat_losses`). Se o território foi conquistado, move exércitos para o território conquistado.

**Regra implementada:** (regras.md, linhas 50-54) - Mesmas regras da Fase 2 acima.

### ExecuteActionNode - Nó de Execução de Ações

#### O que foi implementado

O `ExecuteActionNode` é um novo tipo de nó que executa ações reais no estado do jogo, diferente do `PerformActionNode` que apenas exibe mensagens.

#### Estrutura

```csharp
public class ExecuteActionNode : InteractiveNode
{
    public string Message { get; set; }      // Mensagem exibida ao usuário
    public string ActionId { get; set; }      // ID da ação a executar
    public Node? Next { get; set; }          // Próximo nó
}
```

#### Ações Implementadas

O `GraphCrawler` processa as seguintes ações quando encontra um `ExecuteActionNode`:

1. **`resolve_combat`**: Chama `ResolveCombat()` no `WarVikingsState`
   - Rola dados do atacante e defensor
   - Compara dados (maior com maior, segundo com segundo)
   - Calcula perdas
   - Armazena resultado em `CurrentCombatResult`

2. **`apply_combat_losses`**: Chama `ApplyCombatLosses()` no `WarVikingsState`
   - Remove exércitos do atacante e defensor
   - Transfere território se conquistado

3. **`move_armies_after_conquest`**: Chama `MoveArmiesAfterConquest()` no `WarVikingsState`
   - Move exércitos do território de origem para o conquistado
   - Respeita limites (mínimo 1, máximo 3)

4. **`set_combat_source`**: Define o território de origem do combate
   - Atualmente usa o primeiro território atacável disponível
   - TODO: Implementar seleção real do usuário

5. **`set_combat_target`**: Define o território alvo do combate
   - Atualmente usa o primeiro território atacável a partir da origem
   - TODO: Implementar seleção real do usuário

#### Lógica do código

Quando o `GraphCrawler` encontra um `ExecuteActionNode`, ele chama o método `ExecuteAction()` que processa o `ActionId` e executa a ação correspondente no `WarVikingsState`. Isso permite que os grafos executem lógica real além de apenas exibir mensagens.

**Regra implementada:** Permite a execução de ações do jogo através dos grafos de decisão, conectando a estrutura de grafos com a lógica do estado do jogo.

### Phase3Graph - Fase 3: Deslocamento de Exércitos

#### Estrutura do Grafo

O grafo `Phase3Graph` implementa a terceira fase do turno, onde o jogador pode deslocar exércitos entre territórios contíguos do mesmo jogador.

#### Fluxo do Grafo

```
StartNode
  ↓
PerformActionNode: "FASE 3: DESLOCAMENTO DE EXÉRCITOS"
  ↓
BinaryConditionNode: "Você tem possibilidades de deslocamento?"
  ├─ false → PerformActionNode: "Você não tem possibilidades de deslocamento"
  │         ↓
  │         EndNode
  └─ true → BinaryConditionNode: "Você quer realizar um deslocamento?"
              ├─ false → PerformActionNode: "Você decidiu não deslocar"
              │         ↓
              │         EndNode
              └─ true → ExecuteActionNode: "set_movement_source"
                        ↓
                        ExecuteActionNode: "set_movement_target"
                        ↓
                        PerformActionNode: "Quantos exércitos mover?"
                        ↓
                        ExecuteActionNode: "execute_movement"
                        ↓
                        PerformActionNode: "Deslocamento realizado"
                        ↓
                        EndNode
```

#### Lógica do código

O grafo verifica se há possibilidades de deslocamento (territórios com mais de 1 exército e territórios contíguos do mesmo jogador). Se houver, pergunta se o jogador quer deslocar. Se sim, define os territórios de origem e destino (atualmente usa o primeiro disponível) e executa o deslocamento. A regra de apenas 1 deslocamento por turno é implementada pela estrutura do grafo (não há loop).

**Regra implementada:** (regras.md, linhas 56-60)
> "**3. Deslocamento de Exércitos:**
> - Realizado no final do turno.
> - O deslocamento é feito entre territórios contíguos do jogador.
> - Um exército deve permanecer no território de origem.
> - É permitido apenas um deslocamento (transferência de exércitos) por turno, exceto para mover exércitos para um território recém-conquistado, que é um deslocamento imediato."

**Regra implementada:** (regras.md, linhas 45-48)
> "**1. Recebimento de Exércitos:** O jogador recebe exércitos no início do turno de três maneiras:
> - **Territórios Possuídos:** Soma-se o número de territórios possuídos e divide-se por 2 (o resultado é arredondado para baixo). O mínimo de exércitos a receber é 3, a não ser que o jogador possua menos de 6 territórios.
> - **Regiões Conquistadas:** Recebe exércitos extras se possuir uma Região inteira (valor conforme a tabela no tabuleiro). Estes exércitos devem ser distribuídos obrigatoriamente nesta Região.
> - **Troca de Cartas:** A troca de cartas (3 cartas com figuras iguais ou 3 cartas com figuras diferentes) garante exércitos. Os valores de exércitos por troca são progressivos (4, 6, 8, 10, etc.). **É obrigatório** trocar se o jogador acumular 5 cartas."

### CardTradeGraph - Sub-grafo de Troca de Cartas

#### Estrutura do Grafo

O grafo `CardTradeGraph` é um sub-grafo chamado pelo `Phase1Graph` quando o jogador precisa ou quer trocar cartas.

#### Fluxo do Grafo

```
StartNode
  ↓
PerformActionNode: "TROCA DE CARTAS"
  ↓
PerformActionNode: "Mostrando suas cartas de território..."
  ↓
BinaryConditionNode: "Você tem 3 cartas com a mesma figura?"
  ├─ true → PerformActionNode: "Troque 3 cartas iguais e receba exércitos."
  │         ↓
  └─ false → BinaryConditionNode: "Você tem 3 cartas com figuras diferentes?"
              ├─ true → PerformActionNode: "Troque 3 cartas diferentes e receba exércitos."
              │         ↓
              └─ false → PerformActionNode: "Você não pode trocar cartas agora."
                          ↓
PerformActionNode: "Exércitos recebidos pela troca calculados."
  ↓
PerformActionNode: "Remova as 3 cartas trocadas do seu baralho."
  ↓
ReturnFromGraphNode
```

#### Lógica do código

O grafo verifica primeiro se o jogador tem 3 cartas iguais. Se não tiver, verifica se tem 3 cartas diferentes. Se nenhuma das condições for satisfeita, informa que não pode trocar. Após a troca, calcula os exércitos recebidos e remove as cartas trocadas.

**Regra implementada:** (regras.md, linha 48)
> "A troca de cartas (3 cartas com figuras iguais ou 3 cartas com figuras diferentes) garante exércitos. Os valores de exércitos por troca são progressivos (4, 6, 8, 10, etc.)."

### Resumo

| Grafo | Função | Status |
|-------|--------|--------|
| `Phase1Graph` | Fase 1 - Recebimento de Exércitos | ✅ |
| `CardTradeGraph` | Sub-grafo de Troca de Cartas | ✅ |

[↑ Voltar ao topo](#-índice)

---

**Última atualização:** 30/12/2025 - TODOS OS TESTES CONCLUÍDOS - 49/49 testes (100%) ✅

