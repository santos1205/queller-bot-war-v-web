# 🧪 Guia de Teste - Sistema de IA do Bot

**Data:** 30/12/2025  
**Status:** ⚠️ Teste Parcial (IA funciona apenas na Fase 2 - Ataques)

---

## ⚠️ Limitação Atual

O sistema de IA atualmente responde automaticamente apenas à pergunta:
- **"Você quer realizar um ataque?"** (Fase 2)

Outras perguntas ainda requerem resposta manual do usuário.

---

## ✅ Como Testar a IA na Fase 2

### Passo 1: Iniciar o Programa

```bash
cd WarVikingsBot
USE_TEST_DATA=true dotnet run
```

### Passo 2: Navegar até a Fase 2

1. **Fase 1 - Recebimento de Exércitos:**
   - Responda manualmente às perguntas sobre cartas:
     - "Você tem 5 ou mais cartas de território?" → `false`
     - "Você quer trocar cartas agora?" → `false`
   - Pressione Enter para passar pelas mensagens

2. **Fase 2 - Ataques:**
   - A pergunta "É a primeira rodada do jogo?" será respondida automaticamente (false)
   - A pergunta "Você tem territórios que podem atacar?" será respondida automaticamente (true)
   - **A pergunta "Você quer realizar um ataque?" será respondida automaticamente pelo bot!** ✅

### Passo 3: Observar as Decisões do Bot

Quando o bot decidir atacar, ele irá:
1. **Escolher automaticamente o território de origem** (usando `SelectAttackSourceTerritory()`)
2. **Escolher automaticamente o território alvo** (usando `SelectAttackTargetTerritory()`)
3. **Decidir quantos exércitos mover após conquista** (usando `DecideArmiesToMoveAfterConquest()`)

---

## 🔍 O que Observar

### Decisão de Atacar
- O bot avalia se deve atacar baseado em:
  - Objetivo do bot
  - Vantagem numérica
  - Disponibilidade de alvos

### Seleção de Territórios
- O bot escolhe o melhor território de origem considerando:
  - Número de exércitos (mais = melhor)
  - Presença de comandante (bônus)
  - Segurança (menos adjacentes inimigos = melhor)

- O bot escolhe o melhor alvo considerando:
  - Fraqueza do alvo (menos exércitos = melhor)
  - Vantagem numérica (mais dados = melhor)
  - Objetivo do bot (portos, regiões, jogadores específicos)

### Movimento de Exércitos
- O bot decide quantos exércitos mover baseado em:
  - Exposição do território conquistado
  - Segurança do território de origem
  - Número de exércitos que participaram do ataque

---

## 📝 Próximos Passos

Para completar o sistema de IA, precisamos adicionar respostas automáticas para:
- [ ] Pergunta sobre cartas de território (Fase 1)
- [ ] Pergunta sobre troca de cartas (Fase 1)
- [ ] Pergunta sobre comandante presente (CombatGraph)
- [ ] Pergunta sobre poderes dos deuses (CombatGraph)
- [ ] Pergunta sobre conquista (CombatGraph)
- [ ] Pergunta sobre movimento de exércitos (Fase 3)

---

## 🎯 Teste Rápido

Execute e observe:

```bash
cd WarVikingsBot
USE_TEST_DATA=true dotnet run
```

**Navegue até a Fase 2 e observe:**
- O bot responde automaticamente "Você quer realizar um ataque?"
- O bot escolhe territórios automaticamente
- O bot decide quantos exércitos mover

**Isso confirma que o sistema de IA está funcionando!** ✅

