# Lógica de Decisão do Bot - Explicação Detalhada

**Data de Criação:** 30/12/2025  
**Versão:** 1.0

---

## Índice

1. [Conceito Geral](#conceito-geral)
2. [Decisão 1: Devo Atacar?](#decisão-1-devo-atacar)
3. [Decisão 2: Qual Território Usar Como Origem?](#decisão-2-qual-território-usar-como-origem)
4. [Decisão 3: Qual Território Atacar?](#decisão-3-qual-território-atacar)
5. [Decisão 4: Quantos Exércitos Mover Após Conquista?](#decisão-4-quantos-exércitos-mover-após-conquista)
6. [Resumo Visual do Fluxo](#resumo-visual-do-fluxo)
7. [Exemplo Completo](#exemplo-completo)
8. [Pontos Importantes](#pontos-importantes)

---

## Conceito Geral

O bot toma decisões baseadas em:

1. **Estado atual do jogo** (territórios, exércitos, cartas, etc.)
2. **Objetivo do bot** (conquistar territórios, eliminar jogador, etc.)
3. **Análise estratégica** (vantagem numérica, riscos, oportunidades)

**O bot não é aleatório!** Cada decisão segue regras claras e lógicas.

---

## Decisão 1: Devo Atacar?

### Passo 1: Verificações Básicas

Antes de qualquer coisa, o bot verifica se é possível atacar:

```
❓ "Tenho territórios que podem atacar?" (preciso de pelo menos 2 exércitos)
   → Se NÃO → Não ataco ❌

❓ "Há alvos disponíveis para atacar?" (territórios inimigos adjacentes)
   → Se NÃO → Não ataco ❌
```

### Passo 2: Estratégia Baseada no Objetivo

O bot tem **5 objetivos possíveis**. Cada um muda a forma como ele decide:

#### 🎯 Objetivo 1: Expandir e Fortalecer (Padrão)

```
📊 "Quantos territórios eu tenho?"
   → Se tenho MENOS de 10 territórios → ATAQUE AGRESSIVO ✅
   → Se tenho 10 ou mais → Só ataco se tiver VANTAGEM CLARA ✅
```

**Lógica:** No início do jogo, o bot precisa expandir rapidamente. Depois, só ataca quando tem vantagem.

#### 🎯 Objetivo 2: Conquistar X Territórios

```
📊 "Estou longe do meu objetivo?" (menos de 70% do alvo)
   → Se SIM → ATAQUE AGRESSIVO ✅
   → Se NÃO → Só ataco se tiver VANTAGEM CLARA ✅
```

**Lógica:** Se está longe do objetivo, precisa ser agressivo. Se está perto, só ataca quando tem certeza de vitória.

#### 🎯 Objetivo 3: Conquistar uma Região Específica

```
📊 "Há alvos na região que quero conquistar?"
   → Se SIM → Ataco se tiver vantagem ✅
   → Se NÃO → Uso estratégia genérica
```

**Lógica:** Foca em territórios da região objetivo, ignorando outros.

#### 🎯 Objetivo 4: Eliminar um Jogador Específico

```
📊 "Há territórios do jogador que quero eliminar?"
   → Se SIM → ATAQUE AGRESSIVO ✅
   → Se NÃO → Só ataco se tiver vantagem
```

**Lógica:** Prioriza atacar o jogador alvo, mesmo que não seja a melhor jogada tática.

#### 🎯 Objetivo 5: Conquistar Portos

```
📊 "Há territórios com porto para atacar?"
   → Se SIM → Ataco se tiver vantagem ✅
   → Se NÃO → Uso estratégia genérica
```

**Lógica:** Foca em conquistar portos, que são valiosos para estratégia naval.

### Passo 3: Avaliar Vantagem

Se a estratégia não for "atacar sempre", o bot avalia se há vantagem:

```
Para cada território meu que pode atacar:
   1. Encontra o melhor alvo
   2. Calcula:
      - Minha força de ataque (quantos dados posso rolar)
      - Força de defesa do alvo (quantos dados ele pode rolar)
   
   3. Decisão:
      ✅ Se eu tiver MAIS dados → ATAQUE (vantagem clara)
      ✅ Se tiver IGUAL número de dados E mais exércitos totais → ATAQUE
      ❌ Caso contrário → NÃO ATAQUE (muito arriscado)
```

**Exemplo Prático:**

```
Meu território: 4 exércitos (posso rolar 3 dados)
Alvo inimigo: 2 exércitos (pode rolar 2 dados)

✅ VANTAGEM! Eu tenho 3 dados vs 2 dados dele → ATAQUE!
```

---

## Decisão 2: Qual Território Usar Como Origem?

O bot **pontua cada território** e escolhe o de **maior pontuação**.

### Sistema de Pontuação:

```
PONTOS BASE:
+10 pontos por cada exército no território
   (Mais exércitos = mais dados = melhor)

BÔNUS:
+20 pontos se o COMANDANTE estiver presente
   (Comandante dá vantagem especial)

+5 pontos se o território tem PORTO
   (Útil para estratégia naval futura)

PENALIDADE:
-5 pontos por cada território INIMIGO adjacente
   (Território exposto = mais arriscado)
```

### Exemplo:

```
Território A:
- 5 exércitos = 50 pontos
- Comandante presente = +20 pontos
- 1 inimigo adjacente = -5 pontos
TOTAL: 65 pontos

Território B:
- 3 exércitos = 30 pontos
- Sem comandante = 0 pontos
- 2 inimigos adjacentes = -10 pontos
TOTAL: 20 pontos

✅ Bot escolhe Território A (65 > 20)
```

---

## Decisão 3: Qual Território Atacar?

O bot **pontua cada alvo** e escolhe o de **maior pontuação**.

### Sistema de Pontuação:

```
PONTOS BASE:
+10 pontos por cada exército A MENOS que o alvo tem
   (Alvo mais fraco = mais fácil de conquistar)

VANTAGEM NUMÉRICA:
+15 pontos por cada dado A MAIS que eu tenho
   (Mais dados = maior chance de vitória)

BÔNUS ESPECIAIS (dependem do objetivo):
+50 pontos se o alvo tem PORTO (se objetivo for conquistar portos)
+30 pontos se o alvo está na REGIÃO objetivo
+40 pontos se o alvo pertence ao JOGADOR que quero eliminar

BÔNUS ESTRATÉGICO:
+10 pontos se o alvo tem POUCOS territórios adjacentes
   (Território isolado = mais fácil de defender depois)
```

### Exemplo:

```
Alvo X:
- 2 exércitos (fraco) = +80 pontos (10 - 2 = 8, × 10)
- Eu tenho 3 dados, ele tem 2 = +15 pontos (vantagem)
- Sem bônus especial = 0 pontos
TOTAL: 95 pontos

Alvo Y:
- 3 exércitos = +70 pontos
- Eu tenho 3 dados, ele tem 3 = 0 pontos (igual)
- É da região objetivo = +30 pontos
TOTAL: 100 pontos

✅ Bot escolhe Alvo Y (100 > 95) - mesmo sendo mais forte, 
   está na região objetivo!
```

---

## Decisão 4: Quantos Exércitos Mover Após Conquista?

### Regras Básicas:

```
MÍNIMO: 1 exército (obrigatório)
MÁXIMO: 3 exércitos (ou número que participou do ataque)
```

### Lógica de Decisão:

```
1. Calcula quantos exércitos participaram do ataque
   (máximo 3, porque máximo 3 dados)

2. Verifica EXPOSIÇÃO do território conquistado:
   - Se tem 2+ territórios INIMIGOS adjacentes → MOVE MAIS (até 3)
   - Se tem 1 território inimigo adjacente → MOVE MÉDIO (2)
   - Se não tem inimigos adjacentes → MOVE MÍNIMO (1)

3. Verifica SEGURANÇA do território de origem:
   - Se ficaria com MENOS de 2 exércitos → MOVE MENOS
   - Se ficaria com 2+ exércitos → Pode mover mais
```

### Exemplo:

```
Situação:
- Território de origem: 4 exércitos
- Território conquistado: tem 2 inimigos adjacentes (EXPOSTO!)
- Exércitos que atacaram: 3

Decisão:
1. Território conquistado está EXPOSTO → precisa de mais defesa
2. Território de origem ficaria com 1 exército (4 - 3 = 1) → MUITO FRACO!
3. Compromisso: Move 2 exércitos
   - Conquistado fica com 2 exércitos (razoável)
   - Origem fica com 2 exércitos (seguro)
```

---

## Resumo Visual do Fluxo

```
┌─────────────────────────────────────┐
│  PERGUNTA: "Devo atacar?"           │
└─────────────────────────────────────┘
           │
           ├─→ ❌ Não tenho territórios → NÃO ATAQUE
           ├─→ ❌ Não há alvos → NÃO ATAQUE
           │
           └─→ ✅ Tenho tudo → Verifica OBJETIVO
                      │
                      ├─→ Objetivo: Expandir
                      │   └─→ Menos de 10 territórios? → ATAQUE
                      │   └─→ 10+ territórios? → Verifica VANTAGEM
                      │
                      ├─→ Objetivo: Conquistar Região
                      │   └─→ Há alvos na região? → ATAQUE se vantagem
                      │
                      └─→ Objetivo: Eliminar Jogador
                          └─→ Há alvos do jogador? → ATAQUE AGRESSIVO

┌─────────────────────────────────────┐
│  Se decidiu ATACAR:                 │
└─────────────────────────────────────┘
           │
           ├─→ Escolhe ORIGEM (maior pontuação)
           │   └─→ Mais exércitos = melhor
           │   └─→ Comandante = bônus
           │   └─→ Menos inimigos adjacentes = melhor
           │
           ├─→ Escolhe ALVO (maior pontuação)
           │   └─→ Mais fraco = melhor
           │   └─→ Vantagem numérica = melhor
           │   └─→ Alinhado com objetivo = bônus
           │
           └─→ Decide QUANTOS MOVER
               └─→ Território exposto? → Move mais
               └─→ Origem ficaria fraca? → Move menos
               └─→ Compromisso entre segurança e defesa
```

---

## Exemplo Completo

### Cenário:

- Bot tem **8 territórios** (objetivo: Expandir)
- **Território A:** 5 exércitos, comandante presente
- **Território B:** 3 exércitos, sem comandante
- **Alvo X:** 2 exércitos, adjacente a A
- **Alvo Y:** 3 exércitos, adjacente a B

### Decisão do Bot:

```
1. "Devo atacar?"
   → Tenho 8 territórios (< 10) → ATAQUE AGRESSIVO ✅

2. "Qual origem?"
   → Território A: 50 + 20 (comandante) = 70 pontos
   → Território B: 30 pontos
   → Escolhe A ✅

3. "Qual alvo?"
   → Alvo X: 80 (fraco) + 15 (vantagem) = 95 pontos
   → Alvo Y: 70 (menos fraco) = 70 pontos
   → Escolhe X ✅

4. "Quantos mover?"
   → Alvo X tem 1 inimigo adjacente → Move 2 exércitos
   → Origem A fica com 3 exércitos (seguro) ✅
```

**Resultado:** Bot ataca Alvo X a partir de Território A, movendo 2 exércitos após conquista.

---

## Pontos Importantes

### 1. O Bot Não É Aleatório
Todas as decisões seguem regras claras e lógicas. Não há sorte ou aleatoriedade nas escolhas estratégicas.

### 2. O Bot É Adaptativo
Muda a estratégia conforme o objetivo. Um bot que quer eliminar um jogador age diferente de um que quer conquistar territórios.

### 3. O Bot Avalia Riscos
Não ataca se não tiver vantagem clara. Prefere esperar uma oportunidade melhor do que arriscar uma derrota.

### 4. O Bot Prioriza Objetivos
Foca no que precisa para vencer. Se o objetivo é conquistar portos, prioriza ataques a territórios com porto.

### 5. O Bot Equilibra Segurança e Agressividade
Move exércitos considerando tanto a segurança do território conquistado quanto a do território de origem.

---

## Detalhes Técnicos

### Arquivos Relacionados:

- **`WarVikingsBot/src/AI/BotStrategy.cs`** - Implementação da lógica de decisão
- **`WarVikingsBot/src/AI/DecisionContext.cs`** - Contexto de decisão (estado do jogo)
- **`WarVikingsBot/src/AI/BotObjective.cs`** - Enum de objetivos do bot
- **`WarVikingsBot/src/Crawler/GraphCrawler.cs`** - Integração com o sistema de grafos

### Métodos Principais:

- **`ShouldAttack()`** - Decide se deve atacar
- **`SelectAttackSourceTerritory()`** - Escolhe território de origem
- **`SelectAttackTargetTerritory()`** - Escolhe território alvo
- **`DecideArmiesToMoveAfterConquest()`** - Decide quantos exércitos mover

---

## Melhorias Futuras

### Curto Prazo:
- [ ] Adicionar estratégia para troca de cartas
- [ ] Adicionar estratégia para alocação de exércitos na Fase 1
- [ ] Adicionar estratégia para deslocamento de exércitos na Fase 3
- [ ] Melhorar avaliação de risco (não atacar se muito arriscado)

### Médio Prazo:
- [ ] Adicionar estratégia para uso de poderes dos deuses
- [ ] Adicionar estratégia para combate naval
- [ ] Adicionar estratégia para construção de navios
- [ ] Implementar aprendizado adaptativo (bot aprende com erros)

### Longo Prazo:
- [ ] Implementar múltiplos níveis de dificuldade
- [ ] Adicionar personalidade ao bot (agressivo, defensivo, equilibrado)
- [ ] Implementar análise de longo prazo (planejamento de múltiplos turnos)

---

**Última atualização:** 30/12/2025

