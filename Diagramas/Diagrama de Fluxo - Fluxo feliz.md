sequenceDiagram
    participant C as Cliente
    participant API as API (.NET)
    participant V as Serviço de Venda
    participant E as Serviço de Estoque
    participant F as Serviço de Faturamento
    participant S as Serviço de Separação
    participant DB as Banco de Dados

    C->>API: Realizar venda
    API->>V: Criar venda

    V->>E: Verificar estoque
    E-->>V: Retorno (OK / Falha)

    alt Estoque OK
        V->>F: Solicitar faturamento
        F-->>V: Nota emitida

        V->>S: Solicitar separação
        S-->>V: Separação concluída

        V->>DB: Salvar venda (Sucesso)
        V-->>API: Venda concluída
        API-->>C: Sucesso

    else Estoque insuficiente
        V-->>API: Falha na venda
        API-->>C: Erro
    end