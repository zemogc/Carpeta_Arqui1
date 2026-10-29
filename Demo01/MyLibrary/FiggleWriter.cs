namespace MyLibrary
{
    public class FiggleWriter
    {
        public void Write(string message)
        {
            Console.WriteLine(Render(message));
        }

        // Separar el formateo de la escritura permite reutilizar la biblioteca fuera de la consola.
        public string Render(string message, Figgle.FiggleFont? font = null)
        {
            return (font ?? Figgle.FiggleFonts.Standard).Render(message);
        }
    }
}
