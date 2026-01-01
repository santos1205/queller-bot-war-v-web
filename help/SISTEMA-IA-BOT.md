# 🤖 Sistema de IA do Bot - War Vikings Bot

**Data de Criação:** 30/12/2025  
**Status:** ✅ Implementado e Integrado

---

## 📋 Visão Geral

O sistema de IA do bot foi criado para tomar decisões estratégicas automaticamente baseadas em:
1. **Estado atual do jogo** (territórios, exércitos, cartas, etc.)
2. **Objetivo do bot** (conquistar territórios, eliminar jogador, etc.)
3. **Análise estratégica** (avaliação de vantagens, riscos, oportunidades)

---

## 🏗️ Arquitetura

### Componentes Principais

#### 1. `BotObjective` (Enum)
Define os possíveis objetivos do bot:
- `ConquerTerritories` - Conquistar um número específico de territórios
- `ConquerRegion` - Conquistar uma região específica
- `EliminatePlayer` - Eliminar um jogador específico
- `ConquerPorts` - Conquistar portos
- `ExpandAndFortify` - Expandir e fortalecer posição (objetivo genérico)

#### 2. `DecisionContext`
Contexto de decisão que contém:
- Estado atual do jogo (`WarVikingsState`)
- ID do jogador (bot)
- Objetivo do bot
- Parâmetros do objetivo (ex: número de territórios a conquistar)
- Informações calculadas (territórios, exércitos, etc.)

#### 3. `BotStrategy`
Classe principal que implementa a lógica de decisão:
- `ShouldAttack()` - Decide se deve atacar
- `SelectAttackSourceTerritory()` - Escolhe território de origem
- `SelectAttackTargetTerritory()` - Escolhe território alvo
- `DecideArmiesToMoveAfterConquest()` - Decide quantos exércitos mover após conquista

---

## 🎯 Decisões Implementadas

### 1. Decidir se Deve Atacar (`ShouldAttack()`)

**Lógica:**
- Verifica se há territórios que podem atacar
- Verifica se há alvos disponíveis
- Analisa o objetivo do bot e decide estrategicamente

**Estratégias por Objetivo:**
- **ConquerTerritories**: Ataca agressivamente se está longe do objetivo (menos de 70% do alvo)
- **ConquerRegion**: Prioriza ataques na região objetivo
- **EliminatePlayer**: Ataca agressivamente territórios do jogador alvo
- **ConquerPorts**: Prioriza territórios com porto
- **ExpandAndFortify**: Ataca se tiver vantagem clara ou se precisa expandir

### 2. Selecionar Território de Origem (`SelectAttackSourceTerritory()`)

**Critérios de Avaliação:**
- **Mais exércitos** = melhor (pode usar mais dados)
- **Comandante presente** = bônus (+20 pontos)
- **Território com porto** = bônus (+5 pontos)
- **Menos adjacentes inimigos** = mais seguro (+5 pontos por adjacente inimigo a menos)

### 3. Selecionar Território Alvo (`SelectAttackTargetTerritory()`)

**Critérios de Avaliação:**
- **Alvo mais fraco** = melhor (+10 pontos por exército a menos)
- **Vantagem numérica** = melhor (+15 pontos por dado a mais)
- **Território com porto** = bônus (+50 pontos se objetivo for conquistar portos)
- **Região objetivo** = bônus (+30 pontos se objetivo for conquistar região)
- **Jogador alvo** = bônus (+40 pontos se objetivo for eliminar jogador)
- **Território isolado** = mais fácil de defender (+10 pontos)

### 4. Decidir Quantos Exércitos Mover Após Conquista (`DecideArmiesToMoveAfterConquest()`)

**Lógica:**
- **Mínimo:** 1 exército (obrigatório)
- **Máximo:** 3 exércitos (ou número que participou do ataque)

**Fatores Considerados:**
- **Adjacentes inimigos:** Se o território conquistado tem muitos adjacentes inimigos (≥2), move mais exércitos (até 3)
- **Segurança do território de origem:** Se o território de origem ficaria muito fraco (<2 exércitos), move menos

---

## 🔗 Integração com o Sistema

### Modo Bot vs Modo Manual

O sistema suporta dois modos:

1. **Modo Bot** (`IsBotMode = true`):
   - Bot toma todas as decisões automaticamente
   - Usa `BotStrategy` para escolhas estratégicas
   - Não requer interação do usuário

2. **Modo Manual** (`IsBotMode = false`):
   - Usuário toma todas as decisões
   - Bot apenas executa ações (por enquanto usa primeiro disponível)
   - Requer interação do usuário

### Integração com GraphCrawler

O `GraphCrawler` foi modificado para:

1. **Inicializar BotStrategy** quando `IsBotMode = true`
2. **Avaliar condições automaticamente** usando `BotStrategy.ShouldAttack()`
3. **Executar ações com decisões do bot**:
   - `set_combat_source` → usa `BotStrategy.SelectAttackSourceTerritory()`
   - `set_combat_target` → usa `BotStrategy.SelectAttackTargetTerritory()`
   - `move_armies_after_conquest` → usa `BotStrategy.DecideArmiesToMoveAfterConquest()`

### Propriedades no WarVikingsState

```csharp
public bool IsBotMode { get; set; } = true;  // Modo bot ativado por padrão
public BotObjective BotObjective { get; set; } = BotObjective.ExpandAndFortify;
public Dictionary<string, object> BotObjectiveParameters { get; set; }
```

---

## 📊 Exemplo de Uso

### Configurar Objetivo do Bot

```csharp
var state = new WarVikingsState();
state.IsBotMode = true;
state.BotObjective = BotObjective.ConquerTerritories;
state.BotObjectiveParameters["targetCount"] = 18; // Objetivo: conquistar 18 territórios
```

### Configurar Objetivo de Região

```csharp
state.BotObjective = BotObjective.ConquerRegion;
state.BotObjectiveParameters["targetRegion"] = "RegiaoNorte";
```

### Configurar Objetivo de Eliminação

```csharp
state.BotObjective = BotObjective.EliminatePlayer;
state.BotObjectiveParameters["targetPlayer"] = 2; // Eliminar jogador 2
```

---

## 🎮 Como Funciona na Prática

### Fluxo de Decisão do Bot

1. **Fase 2 - Ataques:**
   - Sistema pergunta: "Você quer realizar um ataque?"
   - Bot avalia: `ShouldAttack()` → retorna `true` ou `false`
   - Se `true`, continua para seleção de territórios

2. **Seleção de Território de Origem:**
   - Sistema executa: `set_combat_source`
   - Bot escolhe: `SelectAttackSourceTerritory()` → retorna melhor território
   - Sistema define: `CurrentCombatSourceTerritory`

3. **Seleção de Território Alvo:**
   - Sistema executa: `set_combat_target`
   - Bot escolhe: `SelectAttackTargetTerritory(source)` → retorna melhor alvo
   - Sistema define: `CurrentCombatTargetTerritory`

4. **Após Conquista:**
   - Sistema executa: `move_armies_after_conquest`
   - Bot decide: `DecideArmiesToMoveAfterConquest()` → retorna número de exércitos
   - Sistema move os exércitos

---

## 🔮 Melhorias Futuras

### Curto Prazo
- [ ] Adicionar estratégia para troca de cartas
- [ ] Adicionar estratégia para alocação de exércitos na Fase 1
- [ ] Adicionar estratégia para deslocamento de exércitos na Fase 3
- [ ] Melhorar avaliação de risco (não atacar se muito arriscado)

### Médio Prazo
- [ ] Adicionar estratégia para uso de poderes dos deuses
- [ ] Adicionar estratégia para combate naval
- [ ] Adicionar estratégia para construção de navios
- [ ] Implementar aprendizado adaptativo (bot aprende com erros)

### Longo Prazo
- [ ] Implementar múltiplos níveis de dificuldade
- [ ] Adicionar personalidade ao bot (agressivo, defensivo, equilibrado)
- [ ] Implementar análise de longo prazo (planejamento de múltiplos turnos)

---

## 📝 Notas Técnicas

### Pontuação de Territórios

O sistema usa um sistema de pontuação para avaliar territórios:
- Pontos positivos = melhor escolha
- Pontos negativos = pior escolha
- Escolhe sempre o território com maior pontuação

### Vantagem Numérica

O bot considera vantagem numérica ao decidir ataques:
- **Ataque com mais dados** = maior chance de vitória
- **Ataque com igualdade** = avalia exércitos totais
- **Ataque com desvantagem** = geralmente evita (exceto se necessário para objetivo)

### Segurança

O bot considera segurança ao mover exércitos:
- **Território conquistado exposto** = move mais exércitos
- **Território de origem vulnerável** = move menos exércitos
- **Equilíbrio** = mantém força em ambos os territórios

---

## ✅ Status de Implementação

| Funcionalidade | Status | Observações |
|----------------|--------|-------------|
| Decisão de Atacar | ✅ Completo | Baseado em objetivo e vantagem |
| Seleção de Origem | ✅ Completo | Avalia força, comandante, segurança |
| Seleção de Alvo | ✅ Completo | Avalia fraqueza, vantagem, objetivo |
| Movimento de Exércitos | ✅ Completo | Avalia segurança e exposição |
| Integração com GraphCrawler | ✅ Completo | Decisões automáticas funcionando |
| Múltiplos Objetivos | ✅ Completo | 5 objetivos diferentes implementados |

---

**Última atualização:** 30/12/2025

