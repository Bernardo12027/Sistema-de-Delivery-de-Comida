# Roteiro de apresentação — Rota do Sabor (≈ 12 minutos, 3 integrantes)

## Antes de começar (preparação, 5 min antes)
1. Abrir `RotaDoSabor.slnx` no VS 2026, compilar (Ctrl+Shift+B).
2. (Opcional) Banco limpo: `Drop-Database -Project RotaDoSabor.Infrastructure -StartupProject RotaDoSabor.Wpf`.
3. Deixar abertos: `docs/diagramas/classes.png`, `docs/diagramas/casos-de-uso.png`, `HistoricoPedidoSqlRepository.cs`, `PedidoConfiguration.cs`, `Application/Pagamentos`.
4. Pesquisador de Objetos do SQL Server aberto em `(localdb)\MSSQLLocalDB`.

## Parte 1 — Integrante 1: visão geral e arquitetura (3 min)
1. Problema: delivery de comida, perfil Cliente, desktop WPF.
2. Mostrar a solução com 4 projetos e explicar dependências (`Wpf → Application → Domain`, `Infrastructure` implementa interfaces da Application).
3. MVC: abrir `CarrinhoView.xaml` (View) → `CarrinhoController` (Controller) → `CarrinhoService`/`Carrinho` (Model).
4. Diagrama de classes: herança Produto→Prato/Bebida, interface IPreparacao, composição Pedido◆ItemPedido, 1:1 Pedido–Pagamento.

## Parte 2 — Integrante 2: demonstração do fluxo principal (5 min)
1. **F5** → o banco é criado sozinho (mostrar tabelas no Pesquisador de Objetos depois).
2. **Cadastro** com e-mail inválido (mostrar validação) → cadastro correto → tentar o mesmo e-mail de novo (bloqueado).
3. **Login com o nome de usuário** (depois mostrar que com o e-mail também funciona).
4. Home: categorias → **Hambúrguer** → Smash Burguer → **Ver cardápio** (pratos e bebidas, item indisponível com botão desabilitado, média de avaliação).
5. Adicionar 2x Smash Clássico + Coca-Cola. Ir em Pizza Prime e tentar adicionar uma pizza → **pede confirmação** para trocar de restaurante → *Não*.
6. **Carrinho**: aumentar/diminuir, tentar diminuir abaixo de 1 (bloqueado), total atualizado.
7. **Confirmar pedido** → sem endereço → *Gerenciar endereços* → cadastrar (NOVO: marcar "Tornar padrão") → voltar.
8. Cupom `EXPIRADO15` (recusado) → `BEMVINDO10` (desconto de 10%). Pagamento **Pix** → Confirmar.
9. **Acompanhamento**: barra de progresso, tempo estimado e previsão; esperar ~20 s ou clicar em **Avançar status (demo)** até ENTREGUE.

## Parte 3 — Integrante 3: novas funcionalidades, SQL manual e banco (4 min)
1. **Histórico** (SQL manual): abrir `HistoricoPedidoSqlRepository.cs` e mostrar `@ClienteId`, JOIN e `ORDER BY DataHora DESC`.
2. **Avaliar** o pedido entregue (5 estrelas + comentário) → voltar ao restaurante e mostrar a **média atualizada**. Tentar avaliar de novo → bloqueado.
3. **Repetir pedido** (NOVO) pelo histórico → carrinho preenchido com preço atual.
4. **Cartões** (NOVO): cadastrar cartão de crédito `4111 1111 1111 1234` e um de débito terminado em `0000`. Mostrar no banco a tabela `CartoesSimulados` **sem número completo nem CVV**.
5. Fazer um pedido com o cartão final `0000` → **pagamento RECUSADO** (pedido registrado como CANCELADO, carrinho mantido).
6. Fazer um pedido em **dinheiro** e **cancelar** enquanto PENDENTE (NOVO). Tentar cancelar um pedido EM_PREPARO → bloqueado.
7. **Configurações** (NOVO): editar nome/telefone → saudação da Home muda. **Sair**.
8. Fechar com SOLID: Strategy de pagamento (OCP), IPreparacao (ISP), interfaces + DI (DIP).

## Perguntas prováveis e onde mostrar
- *Onde está o SQL manual?* `Infrastructure/Repositorios/Sql/HistoricoPedidoSqlRepository.cs`.
- *Como evitam SQL Injection?* `SqlParameter` (`@ClienteId`); EF Core também parametriza.
- *Por que Restrict nos pedidos?* preservar histórico e evitar múltiplos caminhos de cascata no SQL Server (`PedidoConfiguration`).
- *Como a senha é guardada?* PBKDF2 com salt (`Pbkdf2PasswordHasher`).
- *Como testar a simulação sem esperar?* botão "Avançar status (demo)" e `IRelogio`.
- *O que acontece se o banco estiver fora?* mensagem amigável (`MensagensErro`) e o app não quebra.
