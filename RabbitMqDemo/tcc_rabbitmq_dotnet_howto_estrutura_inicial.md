# How To — Comunicação Assíncrona com RabbitMQ e .NET

## Objetivo

Este documento apresenta o processo realizado para criação de duas aplicações .NET integradas ao RabbitMQ utilizando o padrão Producer e Consumer.

O objetivo do projeto é demonstrar:

* Comunicação assíncrona
* Mensageria
* Desacoplamento entre aplicações
* Fluxo de mensagens
* Integração entre aplicações distribuídas

O projeto foi dividido em:

* Publisher (Produtor de mensagens)
* Consumer (Consumidor de mensagens)
* RabbitMQ (Broker de mensageria)

---

# Tecnologias Utilizadas

* .NET
* C#
* Erlang
* RabbitMQ
* Console Application

---

# Instalação do Erlang

O RabbitMQ é construído sobre a plataforma Erlang.

Por esse motivo, a instalação do Erlang é obrigatória antes da instalação do RabbitMQ.

Download oficial:

[https://www.erlang.org/downloads](https://www.erlang.org/downloads)

---

# Instalação do RabbitMQ

Após instalar o Erlang, foi realizada a instalação do RabbitMQ localmente no Windows.

Download oficial:

[https://www.rabbitmq.com/download.html](https://www.rabbitmq.com/download.html)

Versão utilizada:

* Série 4.x

---

# Habilitando o Painel Administrativo

Após a instalação do RabbitMQ, foi necessário habilitar o plugin de gerenciamento.

Abrir o PowerShell como administrador:

```powershell
cd "C:\Program Files\RabbitMQ Server\rabbitmq_server-4.x.x\sbin"
```

Executar:

```powershell
.\rabbitmq-plugins.bat enable rabbitmq_management
```

---

# Acesso ao Painel Web

URL:

```text
http://localhost:15672
```

Credenciais padrão:

```text
Usuário: guest
Senha: guest
```

---

# Estrutura da Solução

A solução foi criada da seguinte forma:

```text
RabbitMqDemo/
│
├── RabbitMqDemo.sln
│
├── Publisher/
│   ├── Program.cs
│   └── Publisher.csproj
│
└── Consumer/
    ├── Program.cs
    └── Consumer.csproj
```

---

# Criação da Solution

## Criando a pasta do projeto

```powershell
mkdir RabbitMqDemo
cd RabbitMqDemo
```

---

## Criando a Solution

```powershell
dotnet new sln -n RabbitMqDemo
```

---

## Criando o Publisher

```powershell
dotnet new console -n Publisher
```

---

## Criando o Consumer

```powershell
dotnet new console -n Consumer
```

---

## Adicionando os projetos na solution

```powershell
dotnet sln add Publisher
dotnet sln add Consumer
```

---

# Instalação do Pacote RabbitMQ.Client

No projeto Publisher:

```powershell
cd Publisher
dotnet add package RabbitMQ.Client
```

No projeto Consumer:

```powershell
cd ../Consumer
dotnet add package RabbitMQ.Client
```

---

# Explicação do Publisher

## ConnectionFactory

Responsável pela criação da conexão com o RabbitMQ.

---

## QueueDeclareAsync

Responsável pela criação da fila.

Fila utilizada:

```text
sales-queue
```

Foi utilizada uma fila persistente:

```csharp
durable: true
```

Essa abordagem foi necessária devido às mudanças da versão 4.x do RabbitMQ.

---

## BasicPublishAsync

Responsável pelo envio da mensagem para a fila.

---

# Execução do Publisher

Dentro da pasta Publisher:

```powershell
dotnet run
```

Saída esperada:

```text
Mensagem enviada: Nova venda criada
```

---

# Explicação do Consumer

## AsyncEventingBasicConsumer

Responsável por escutar mensagens recebidas da fila.

---

## ReceivedAsync

Evento executado quando uma nova mensagem é recebida.

---

## BasicConsumeAsync

Responsável por iniciar o consumo contínuo da fila.

---

# Execução do Consumer

Dentro da pasta Consumer:

```powershell
dotnet run
```

Saída esperada:

```text
Aguardando mensagens...
```

Após executar o Publisher:

```text
Mensagem recebida: Nova venda criada
```

---

# Fluxo da Comunicação

```text
Publisher (.NET)
        ↓
RabbitMQ
        ↓
Fila sales-queue
        ↓
Consumer (.NET)
```

---

# Visualização no RabbitMQ

No painel administrativo do RabbitMQ foi possível visualizar:

* Filas criadas
* Mensagens pendentes
* Consumers conectados
* Conexões ativas

Principal aba utilizada:

```text
Queues and Streams
```

---

# Conceitos Demonstrados

A implementação demonstrou:

* Comunicação assíncrona
* Desacoplamento entre aplicações
* Utilização de filas
* Integração entre aplicações distribuídas
* Persistência de mensagens
* Modelo Producer/Consumer

---

# Comparação Conceitual

## Comunicação síncrona

Características:

* Fluxo bloqueante
* Dependência direta
* Forte acoplamento
* Maior propagação de falhas

---

## Comunicação assíncrona

Características:

* Processamento independente
* Utilização de filas
* Isolamento de falhas
* Maior disponibilidade
* Comunicação desacoplada

---

# Considerações Finais

A integração entre aplicações .NET e RabbitMQ permitiu demonstrar de forma prática o funcionamento da comunicação assíncrona utilizando mensageria.

Mesmo utilizando um cenário simplificado, foi possível demonstrar os principais conceitos relacionados a desacoplamento, filas e comunicação entre serviços.

A estrutura construída servirá como base para futuras evoluções do projeto, incluindo:

* Simulação de falhas
* Retry
* Dead Letter Queue
* Múltiplos consumidores
* Processamento distribuído
