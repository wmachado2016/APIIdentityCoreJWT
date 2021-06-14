namespace ApiCatalogo.Repository
{
    public interface IUnitOfWork
    {
        IProdutoRepository ProdutoRepository { get; }
        ICategoriaRepository CategoriaRepository { get; }
        IUsuarioRepository UsuarioRepository { get; }
        void Commit();
    }
}
