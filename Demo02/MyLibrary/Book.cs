namespace MyLibrary;

// Herencia: un libro es un producto con un autor.
internal class Book(int id, string name, decimal unitPrice, int unitsInStock, string author)
    : Product(id, name, unitPrice, unitsInStock)
{
    public string Author => author;

    public override string ToString() => $"{base.ToString()} | Autor: {Author}";
}
