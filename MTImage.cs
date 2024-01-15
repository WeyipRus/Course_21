using System.Windows.Controls;

namespace Course_21
{
    /// <summary>
    /// Изображение фигурки
    /// </summary>
    public class MTImage : Image
    {
        /// <summary>
        /// Координата X
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Координата Y
        /// </summary>
        public int Y { get; set; }
        /// <summary>
        /// Фигурка изображения
        /// </summary>
        public Figure figure { get; set; }
    }
}