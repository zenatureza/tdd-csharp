using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NerdStore.Vendas.Domain
{
    public class Pedido
    {
        public int MAX_UNIDADES_ITEM => 15;

        public decimal ValorTotal { get; private set; }

        public PedidoStatus PedidoStatus { get; private set; }

        private readonly List<PedidoItem> _pedidoItens;
        public IReadOnlyCollection<PedidoItem> PedidoItens => _pedidoItens;

        public Guid ClienteId { get; private set; }

        protected Pedido()
        {
            _pedidoItens = new List<PedidoItem>();
        }

        private void CalcularValorPedido()
        {
            ValorTotal = PedidoItens.Sum(item => item.CalcularValor());
        }

        public void AdicionarItem(PedidoItem pedidoItem)
        {
            if (pedidoItem.Quantidade > MAX_UNIDADES_ITEM)
            {
                throw new DomainException();
            }

            if (_pedidoItens.Any(p=>p.ProdutoId == pedidoItem.ProdutoId))
            {
                var itemExistente = _pedidoItens.FirstOrDefault(p => p.ProdutoId == pedidoItem.ProdutoId);
                itemExistente.AdicionarUnidades(pedidoItem.Quantidade);
                pedidoItem = itemExistente;

                _pedidoItens.Remove(itemExistente);                
            }

            _pedidoItens.Add(pedidoItem);
            CalcularValorPedido();   
        }

        public void TornarRascunho()
        {
            PedidoStatus = PedidoStatus.Rascunho;
        }

        public static class PedidoFactory
        {
            public static Pedido NovoPedidoRascunho(Guid clienteId)
            {
                var pedido = new Pedido()
                {
                    ClienteId = clienteId
                };

                pedido.TornarRascunho();
                return pedido;
            }
        }
    }

    public enum PedidoStatus
    {
        Rascunho = 0,
        Iniciado = 1,
        Pago = 4,
        Entregue = 5,
        Cancelado = 6
    }

    public class PedidoItem
    {
        public PedidoItem()
        {
        }

        public PedidoItem(Guid produtoId, string produtoNome, int quantidade, decimal valorUnitario)
        {
            ProdutoId = produtoId;
            ProdutoNome = produtoNome;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
        }

        public Guid ProdutoId { get; private set; }
        public string ProdutoNome { get; private set; }
        public int Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }

        internal void AdicionarUnidades(int unidades)
        {
            Quantidade += unidades;
        }

        internal decimal CalcularValor()
        {
            return Quantidade * ValorUnitario;
        }
    }

    public class DomainException : Exception
    {
        public DomainException()
        {
        }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
