using System;
using System.Linq;
using Xunit;

namespace NerdStore.Vendas.Domain.Tests
{
    public class PedidoTests
    {
        [Fact(DisplayName = "Adicionar Item ao Novo Pedido")]
        [Trait("Categoria", "Pedido Tests")]
        public void AdicionarItemPedido_NovoPedido_DeveAtualizarValor()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());
            var pedidoItem = new PedidoItem(
                Guid.NewGuid(),
                "Produto Teste",
                2,
                100);

            // Act
            pedido.AdicionarItem(pedidoItem);

            // Assert
            Assert.Equal(200, pedido.ValorTotal);
        }

        [Fact(DisplayName = "Adicionar Item ao Pedido Existente")]
        [Trait("Categoria", "Pedido Tests")]
        public void AdicionarItemPedido_ItemExistente_DeveIncrementarQuantidadeESomarValores()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(
                produtoId,
                "Produto Teste",
                2,
                100);
            pedido.AdicionarItem(pedidoItem);

            var pedidoItem2 = new PedidoItem(
                produtoId,
                "Produto Teste",
                1,
                100);

            // Act
            pedido.AdicionarItem(pedidoItem2);

            // Assert
            Assert.Equal(300, pedido.ValorTotal);
            Assert.Equal(1, pedido.PedidoItens.Count);
            Assert.Equal(3, pedido.PedidoItens.FirstOrDefault(p => p.ProdutoId == produtoId).Quantidade);
        }

        [Fact(DisplayName = "Adicionar Item ao Pedido Acima de 15")]
        [Trait("Categoria", "Pedido Tests")]
        public void AdicionarItemPedido_ItemAcima15Unidades_DeveRetornarException()
        {
            // Arrange
            var pedido = Pedido.PedidoFactory.NovoPedidoRascunho(Guid.NewGuid());

            var produtoId = Guid.NewGuid();
            var pedidoItem = new PedidoItem(
                produtoId,
                "Produto Teste",
                16,
                100);

            // Act & Assert
            Assert.Throws<DomainException>(() => pedido.AdicionarItem(pedidoItem));
        }
    }
}
