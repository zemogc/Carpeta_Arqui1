namespace MyLibrary
{
    public class FiggleWriter
    {
        public void Write(string message)
        {
            Console.WriteLine(Figgle.FiggleFonts.Standard.Render(message));
        }
    }
}
