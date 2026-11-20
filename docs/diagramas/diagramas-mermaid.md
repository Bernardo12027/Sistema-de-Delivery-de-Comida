# Diagramas em Mermaid

Mesmo conteúdo dos arquivos `.puml` (PlantUML). As imagens `classes.png` e `casos-de-uso.png` foram geradas a partir destes códigos. Para editar: cole o código em https://mermaid.live ou abra este arquivo no VS Code/GitHub.

## Diagrama de classes

```mermaid
classDiagram
    direction TB
    class Cliente {
        +int Id
        +string Nome
        +string Email
        +string Telefone
        +string NomeUsuario
        +string SenhaHash
        +DateTime DataCadastro
        +AtualizarPerfil(nome, telefone) NOVO
    }
    class Categoria {
        +int Id
        +string Nome
    }
    class Restaurante {
        +int Id
        +string Nome
        +string Endereco
        +string Telefone
        +decimal AvaliacaoMedia
        +int QuantidadeAvaliacoes
        +int TempoEntregaMinutos
        +RegistrarAvaliacao(nota)
    }
    class Produto {
        <<abstract>>
        +int Id
        +string Nome
        +string Descricao
        +decimal Preco
        +bool Disponivel
        +int CategoriaId
        +int RestauranteId
        +ObterTipo()* string
        +ObterDetalhes()* string
    }
    class Prato {
        +int TempoPreparo
        +string Tamanho
        +Preparar() string
    }
    class Bebida {
        +int Volume
        +string Tipo
    }
    class IPreparacao {
        <<interface>>
        +int TempoPreparo
        +Preparar() string
    }
    class Pedido {
        +int Id
        +DateTime DataHora
        +StatusPedido Status
        +decimal Subtotal
        +decimal Desconto
        +decimal TaxaEntrega
        +decimal ValorTotal
        +int TempoEstimadoMinutos
        +int ClienteId
        +int RestauranteId
        +int EnderecoId
        +AdicionarItem(produto, qtd)
        +AplicarCupom(cupom, agora)
        +Cancelar(agora) NOVO
        +AvancarStatus(agora)
        +PodeSerAvaliado() bool
    }
    class ItemPedido {
        +int Id
        +int PedidoId
        +int ProdutoId
        +string NomeProduto
        +int Quantidade
        +decimal PrecoUnitario
        +decimal Subtotal
    }
    class Endereco {
        +int Id
        +int ClienteId
        +string Logradouro
        +string Numero
        +string Complemento
        +string Bairro
        +string Cidade
        +string Cep
        +bool Padrao
        +bool Ativo
        +DefinirComoPadrao() NOVO
        +Inativar()
    }
    class Pagamento {
        +int Id
        +int PedidoId
        +decimal Valor
        +FormaPagamento Forma
        +StatusPagamento Status
        +DateTime DataPagamento
        +Aprovar(data, codigo)
        +Recusar(codigo)
        +Estornar()
    }
    class Cupom {
        +int Id
        +string Codigo
        +decimal PercentualDesconto
        +DateTime DataValidade
        +bool Ativo
        +EstaValido(agora) bool
        +CalcularDesconto(total) decimal
    }
    class Avaliacao {
        +int Id
        +int PedidoId
        +int Nota
        +string Comentario
        +DateTime DataAvaliacao
    }
    class CartaoSimulado {
        +int Id
        +string IdentificadorFicticio
        +string Bandeira
        +string UltimosDigitos
        +TipoCartao Tipo
        +string Apelido
        +Criar(...)$ CartaoSimulado
        +AceitaForma(forma) bool
    }
    class Carrinho {
        +int RestauranteId
        +decimal Total
        +Adicionar(produto, nome, qtd)
        +Aumentar(id)
        +Diminuir(id)
        +Remover(id)
        +Limpar()
    }
    class ItemCarrinho {
        +int ProdutoId
        +decimal PrecoUnitario
        +int Quantidade
        +decimal Subtotal
    }
    class StatusPedido {
        <<enumeration>>
        Pendente
        EmPreparo
        SaiuParaEntrega
        Entregue
        Cancelado
    }
    class StatusPagamento {
        <<enumeration>>
        Pendente
        Aprovado
        Recusado
        Estornado
    }
    class FormaPagamento {
        <<enumeration>>
        Pix
        CartaoCredito
        CartaoDebito
        Dinheiro
    }
    class TipoCartao {
        <<enumeration>>
        Credito
        Debito
    }
    Produto <|-- Prato
    Produto <|-- Bebida
    IPreparacao <|.. Prato
    Categoria "1" -- "0..*" Restaurante
    Categoria "1" -- "0..*" Produto
    Restaurante "1" o-- "0..*" Produto : cardápio
    Cliente "1" *-- "0..*" Endereco
    Cliente "1" *-- "0..*" CartaoSimulado
    Cliente "1" -- "0..*" Pedido
    Restaurante "1" -- "0..*" Pedido
    Endereco "1" -- "0..*" Pedido
    Cupom "0..1" -- "0..*" Pedido
    Pedido "1" *-- "1..*" ItemPedido : composição
    Produto "1" -- "0..*" ItemPedido
    Pedido "1" *-- "1" Pagamento
    CartaoSimulado "0..1" -- "0..*" Pagamento
    Pedido "1" -- "0..1" Avaliacao
    Cliente "1" -- "0..*" Avaliacao
    Restaurante "1" -- "0..*" Avaliacao
    Carrinho "1" *-- "0..*" ItemCarrinho
    Pedido ..> StatusPedido
    Pagamento ..> StatusPagamento
    Pagamento ..> FormaPagamento
    CartaoSimulado ..> TipoCartao
```

## Diagrama de casos de uso

```mermaid
flowchart LR
    Cliente((Cliente))
    PG((Sistema de Pagamento<br/>Simulado))
    subgraph Sistema[Rota do Sabor]
        UC01([Cadastrar-se])
        UC02([Fazer login - usuário ou e-mail])
        UC03([Fazer logout])
        UC04([Visualizar categorias])
        UC05([Buscar e filtrar restaurantes/produtos])
        UC06([Ver média de avaliação])
        UC07([Visualizar cardápio])
        UC08([Gerenciar carrinho])
        UC09([Finalizar pedido])
        UC10([Selecionar endereço de entrega])
        UC11([Realizar pagamento])
        UC12([Aplicar cupom de desconto])
        UC13([Acompanhar pedido])
        UC14([Ver tempo estimado de preparo/entrega])
        UC15([Gerenciar endereços])
        UC16([Consultar histórico de pedidos])
        UC17([Avaliar pedido])
        N1([NOVO: Editar perfil])
        N2([NOVO: Gerenciar cartões fictícios])
        N3([NOVO: Cancelar pedido pendente])
        N4([NOVO: Repetir pedido anterior])
        N5([NOVO: Definir endereço padrão])
    end
    Cliente --- UC01 & UC02 & UC03 & UC04 & UC05 & UC06 & UC07 & UC08 & UC09
    Cliente --- UC13 & UC14 & UC15 & UC16 & N1 & N2
    UC09 -. «include» .-> UC08
    UC09 -. «include» .-> UC10
    UC09 -. «include» .-> UC11
    UC12 -. «extend» .-> UC09
    UC13 -. «include» .-> UC14
    UC17 -. «extend» .-> UC16
    N3 -. «extend» .-> UC13
    N4 -. «extend» .-> UC16
    N5 -. «extend» .-> UC15
    UC11 --- PG
```
