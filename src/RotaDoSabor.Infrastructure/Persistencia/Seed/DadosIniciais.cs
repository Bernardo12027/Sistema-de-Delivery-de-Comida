namespace RotaDoSabor.Infrastructure.Persistencia.Seed;

using Microsoft.EntityFrameworkCore;
using RotaDoSabor.Domain.Entidades;

/// <summary>
/// Dados iniciais (seed) — idempotente: cada bloco só é inserido se a tabela estiver vazia,
/// então rodar o aplicativo várias vezes NÃO duplica registros.
/// Nenhum usuário é criado aqui (não existem senhas no código): o cliente se cadastra pela tela.
/// </summary>
public static class DadosIniciais
{
    public const string Hamburguer = "Hambúrguer";
    public const string Pizza = "Pizza";
    public const string Marmita = "Marmita";
    public const string Sushi = "Sushi";
    public const string Acai = "Açaí";
    public const string Bebidas = "Bebidas";

    public static async Task PopularAsync(RotaDoSaborDbContext db, CancellationToken cancellationToken = default)
    {
        await PopularCategoriasAsync(db, cancellationToken);
        await PopularRestaurantesEProdutosAsync(db, cancellationToken);
        await PopularCuponsAsync(db, cancellationToken);
    }

    private static async Task PopularCategoriasAsync(RotaDoSaborDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Categorias.AnyAsync(cancellationToken))
            return;

        db.Categorias.AddRange(
            new Categoria(Hamburguer),
            new Categoria(Pizza),
            new Categoria(Marmita),
            new Categoria(Sushi),
            new Categoria(Acai),
            new Categoria(Bebidas));
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task PopularRestaurantesEProdutosAsync(RotaDoSaborDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Restaurantes.AnyAsync(cancellationToken))
            return;

        var categorias = await db.Categorias.ToDictionaryAsync(categoria => categoria.Nome, categoria => categoria.Id, cancellationToken);
        int Cat(string nome) => categorias[nome];

        // Média/quantidade iniciais simulam avaliações anteriores; o Açaí da Praia começa sem avaliações
        // para demonstrar a média sendo calculada só com as avaliações feitas no sistema.
        var smash = new Restaurante("Smash Burguer", "Av. Hugo Musso, 1200 - Praia da Costa, Vila Velha - ES", "(27) 3000-1001", Cat(Hamburguer), 25, 4.80m, 152);
        var pizzaPrime = new Restaurante("Pizza Prime", "Rua Sete de Setembro, 455 - Centro, Vitória - ES", "(27) 3000-1002", Cat(Pizza), 35, 4.50m, 98);
        var reiDoTemaki = new Restaurante("Rei do Temaki", "Av. Champagnat, 890 - Centro, Vila Velha - ES", "(27) 3000-1003", Cat(Sushi), 30, 4.70m, 120);
        var marmitaria = new Restaurante("Marmitaria da Vó", "Rua Henrique Moscoso, 310 - Centro, Vila Velha - ES", "(27) 3000-1004", Cat(Marmita), 20, 4.60m, 75);
        var acaiDaPraia = new Restaurante("Açaí da Praia", "Av. Antônio Gil Veloso, 2100 - Praia da Costa, Vila Velha - ES", "(27) 3000-1005", Cat(Acai), 15);
        var burgerStation = new Restaurante("Burger Station", "Av. Jerônimo Monteiro, 77 - Centro, Vitória - ES", "(27) 3000-1006", Cat(Hamburguer), 30, 4.10m, 40);

        db.Restaurantes.AddRange(smash, pizzaPrime, reiDoTemaki, marmitaria, acaiDaPraia, burgerStation);
        await db.SaveChangesAsync(cancellationToken); // gera os Ids usados pelos produtos abaixo

        db.Produtos.AddRange(
            // Smash Burguer
            new Prato("Smash Clássico", "Pão brioche, dois smash de 90 g, cheddar e molho da casa.", 29.90m, Cat(Hamburguer), smash.Id, true, 15, "Duplo"),
            new Prato("Smash Bacon", "Três smash de 90 g, cheddar, bacon crocante e cebola caramelizada.", 34.90m, Cat(Hamburguer), smash.Id, true, 18, "Triplo"),
            new Prato("Smash Salada", "Dois smash de 90 g, queijo prato, alface, tomate e maionese verde.", 31.90m, Cat(Hamburguer), smash.Id, true, 15, "Duplo"),
            new Prato("Smash Veggie", "Burger de grão-de-bico, queijo, rúcula e tomate seco.", 32.90m, Cat(Hamburguer), smash.Id, false, 16, "Simples"),
            new Prato("Batata Rústica", "Porção de batata rústica com alecrim e páprica.", 18.90m, Cat(Hamburguer), smash.Id, true, 10, "Média"),
            new Bebida("Coca-Cola Lata", "Refrigerante Coca-Cola gelado.", 6.50m, Cat(Bebidas), smash.Id, true, 350, "Lata"),
            new Bebida("Milkshake de Ovomaltine", "Milkshake cremoso de Ovomaltine.", 19.90m, Cat(Bebidas), smash.Id, true, 400, "Copo"),

            // Pizza Prime
            new Prato("Pizza Margherita", "Molho de tomate, muçarela, tomate e manjericão fresco.", 54.90m, Cat(Pizza), pizzaPrime.Id, true, 25, "Grande (8 fatias)"),
            new Prato("Pizza Calabresa", "Muçarela, calabresa fatiada e cebola roxa.", 52.90m, Cat(Pizza), pizzaPrime.Id, true, 25, "Grande (8 fatias)"),
            new Prato("Pizza Quatro Queijos", "Muçarela, provolone, parmesão e gorgonzola.", 59.90m, Cat(Pizza), pizzaPrime.Id, true, 25, "Grande (8 fatias)"),
            new Prato("Pizza Portuguesa", "Presunto, ovo, cebola, ervilha e azeitona.", 46.90m, Cat(Pizza), pizzaPrime.Id, false, 22, "Média (6 fatias)"),
            new Bebida("Guaraná Antarctica 2 L", "Refrigerante Guaraná Antarctica.", 14.00m, Cat(Bebidas), pizzaPrime.Id, true, 2000, "Garrafa PET"),
            new Bebida("Suco de Laranja", "Suco natural de laranja, sem açúcar.", 9.90m, Cat(Bebidas), pizzaPrime.Id, true, 500, "Suco natural"),

            // Rei do Temaki
            new Prato("Temaki de Salmão", "Cone de alga com arroz, salmão fresco e cebolinha.", 32.90m, Cat(Sushi), reiDoTemaki.Id, true, 10, "Unidade"),
            new Prato("Combo Sushi 20 peças", "Sashimi, niguiri, uramaki e hossomaki variados.", 69.90m, Cat(Sushi), reiDoTemaki.Id, true, 20, "20 peças"),
            new Prato("Hot Roll", "Roll empanado de salmão com cream cheese e tarê.", 29.90m, Cat(Sushi), reiDoTemaki.Id, true, 15, "10 peças"),
            new Prato("Uramaki Filadélfia", "Salmão, cream cheese e gergelim.", 27.90m, Cat(Sushi), reiDoTemaki.Id, false, 12, "8 peças"),
            new Bebida("Chá Verde Gelado", "Chá verde gelado levemente adoçado.", 8.50m, Cat(Bebidas), reiDoTemaki.Id, true, 300, "Garrafa"),
            new Bebida("Água Mineral", "Água mineral sem gás.", 4.00m, Cat(Bebidas), reiDoTemaki.Id, true, 500, "Garrafa"),

            // Marmitaria da Vó
            new Prato("Marmita de Frango Grelhado", "Arroz, feijão, frango grelhado, farofa e salada.", 22.90m, Cat(Marmita), marmitaria.Id, true, 12, "Média"),
            new Prato("Marmita de Feijoada", "Feijoada completa com arroz, couve, farofa e laranja.", 29.90m, Cat(Marmita), marmitaria.Id, true, 15, "Grande"),
            new Prato("Marmita Fit de Tilápia", "Tilápia grelhada, arroz integral e legumes no vapor.", 27.90m, Cat(Marmita), marmitaria.Id, true, 12, "Média"),
            new Prato("Marmita de Carne de Panela", "Carne de panela, purê de batata e arroz.", 26.90m, Cat(Marmita), marmitaria.Id, false, 15, "Grande"),
            new Bebida("Suco de Maracujá", "Suco natural de maracujá.", 7.90m, Cat(Bebidas), marmitaria.Id, true, 400, "Suco natural"),

            // Açaí da Praia
            new Prato("Açaí 300 ml", "Açaí puro com banana e granola.", 16.90m, Cat(Acai), acaiDaPraia.Id, true, 5, "300 ml"),
            new Prato("Açaí 500 ml", "Açaí com morango, banana, granola e leite em pó.", 24.90m, Cat(Acai), acaiDaPraia.Id, true, 5, "500 ml"),
            new Prato("Açaí Tropical 700 ml", "Açaí com frutas tropicais, paçoca e mel.", 32.90m, Cat(Acai), acaiDaPraia.Id, true, 7, "700 ml"),
            new Prato("Açaí com Cupuaçu", "Creme de açaí e cupuaçu com castanhas.", 19.90m, Cat(Acai), acaiDaPraia.Id, false, 6, "400 ml"),
            new Bebida("Água de Coco", "Água de coco natural gelada.", 7.00m, Cat(Bebidas), acaiDaPraia.Id, true, 300, "Natural"),

            // Burger Station
            new Prato("X-Burguer", "Pão, hambúrguer de 120 g, queijo e maionese.", 21.90m, Cat(Hamburguer), burgerStation.Id, true, 12, "Simples"),
            new Prato("X-Tudo", "Hambúrguer, ovo, bacon, presunto, queijo, alface e tomate.", 32.90m, Cat(Hamburguer), burgerStation.Id, true, 18, "Grande"),
            new Bebida("Guaraná Lata", "Refrigerante de guaraná gelado.", 6.00m, Cat(Bebidas), burgerStation.Id, true, 350, "Lata"));

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task PopularCuponsAsync(RotaDoSaborDbContext db, CancellationToken cancellationToken)
    {
        if (await db.Cupons.AnyAsync(cancellationToken))
            return;

        var hoje = DateTime.Today;
        db.Cupons.AddRange(
            new Cupom("BEMVINDO10", "10% de desconto de boas-vindas", 10m, hoje.AddYears(1)),
            new Cupom("ROTA20", "20% de desconto em qualquer restaurante", 20m, hoje.AddMonths(6)),
            new Cupom("SABOR50", "50% de desconto (cupom de demonstração)", 50m, hoje.AddMonths(3)),
            new Cupom("GRATIS100", "100% — demonstra que o desconto nunca passa do total", 100m, hoje.AddMonths(1)),
            new Cupom("EXPIRADO15", "15% — cupom vencido (demonstração)", 15m, hoje.AddDays(-30)),
            new Cupom("INATIVO5", "5% — cupom desativado (demonstração)", 5m, hoje.AddYears(1), ativo: false));
        await db.SaveChangesAsync(cancellationToken);
    }
}
