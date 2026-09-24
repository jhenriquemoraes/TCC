## Pré-requisitos

Para executar o projeto localmente, é necessário ter instalado:

- .NET SDK 8.0
- RabbitMQ Server
- Erlang/OTP (necessário para execução do RabbitMQ)
- Git
- Navegador web para acesso ao Swagger e ao RabbitMQ Management

### RabbitMQ

O projeto utiliza RabbitMQ para comunicação assíncrona entre os serviços.

Configuração utilizada atualmente:

- Host: `localhost`
- Usuário: `guest`
- Senha: `guest`
- Porta AMQP padrão: `5672`
- Interface de gerenciamento: porta `15672`

É necessário que o RabbitMQ esteja em execução antes de iniciar os serviços.

Também é necessário habilitar o plugin de gerenciamento do RabbitMQ, caso ainda não esteja habilitado:

```bash
rabbitmq-plugins enable rabbitmq_management
```

### Estrutura da solução

A solução é composta atualmente pelos seguintes projetos:

- OrderApi - ponto de entrada para criação dos pedidos e publicação do evento OrderCreated.
- ProductService - recebe OrderCreated e publica ProductsReady.
- PaymentService - recebe os eventos OrderCreated e ProductsReady.
- DeliveryService - recebe o evento ProductsReady.
- MessageContracts - biblioteca compartilhada contendo os contratos das mensagens utilizadas entre os serviços.

## Dependências principais

Os projetos que realizam comunicação com o RabbitMQ utilizam o pacote:

- RabbitMQ.Client

A restauração dos pacotes NuGet pode ser realizada através do comando:
```bash
dotnet restore
```
## Como executar  

Clone o repositório:
```bash
git clone <URL_DO_REPOSITORIO>
```
Entre na pasta da solução:
```bash
cd TccAsyncArchitecture
```
Restaure as dependências:
```bash
dotnet restore
```
Certifique-se de que o RabbitMQ está em execução.  

Execute os serviços em terminais separados:  
```bash
dotnet run --project OrderApi
```
```bash
dotnet run --project ProductService
```
```bash
dotnet run --project PaymentService
```
```bash
dotnet run --project DeliveryService
```
Acesse o Swagger do OrderApi pelo endereço informado no terminal durante a inicialização.
Realize uma requisição POST no endpoint de criação de pedido.

Após a requisição, o mesmo OrderId poderá ser acompanhado nos terminais dos diferentes serviços conforme as mensagens forem processadas.