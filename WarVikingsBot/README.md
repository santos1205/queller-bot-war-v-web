# War Vikings Bot

Sistema CLI (Command Line Interface) em .NET (C#) que implementa um bot de IA para jogar **War Vikings** solo, seguindo a mesma arquitetura do projeto Queller Bot original.

## 🎯 Objetivo

Este projeto permite jogar War Vikings sozinho, controlando um jogador enquanto o bot controla os adversários seguindo regras de decisão baseadas em grafos.

## 🛠️ Tecnologias

- **.NET 8.0** (LTS - versão estável)
- **C#** (linguagem principal)
- **CLI** (Command Line Interface)

## 📁 Estrutura do Projeto

```
WarVikingsBot/
├── src/
│   ├── Models/          # Modelos de dados (enums, classes)
│   ├── Graphs/          # Classes de grafos de decisão
│   ├── State/           # Gerenciamento de estado do jogo
│   ├── Cli/             # Interface de linha de comando
│   └── Crawler/         # Navegador de grafos
├── Graphs/              # Definições de grafos (JSON ou C#)
├── Program.cs           # Ponto de entrada
└── WarVikingsBot.csproj # Arquivo de projeto
```

## 🚀 Como Executar

```bash
dotnet run
```

## 📚 Documentação

- Regras do jogo: `../help/regras.md`
- Progresso do projeto: `../help/PROGRESSO-PROJETO.md`

## 📝 Status

🟡 Em desenvolvimento inicial - Estrutura base criada

---

**Baseado em:** Queller Bot (War of the Ring)  
**Adaptado para:** War Vikings

