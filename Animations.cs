using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Animation;

namespace Course_21
{
    static public class Animations
    {
        /// <summary>
        /// Анимация выбора ячейки
        /// </summary>
        public static readonly DoubleAnimation Selection = new DoubleAnimation()
        {
            From = 1.0,
            To = 0.0,
            Duration = TimeSpan.FromSeconds(0.2),
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };
        /// <summary>
        /// Анимация пропадания фигуры
        /// </summary>
        public static readonly DoubleAnimation Disappearance = new DoubleAnimation()
        {
            From = 1.0,
            To = 0.0,
            Duration = TimeSpan.FromSeconds(0.5),
            AutoReverse = false
        };
        /// <summary>
        /// Анимация появления фигуры
        /// </summary>
        public static readonly DoubleAnimation Appearance = new DoubleAnimation()
        {
            From = 0.0,
            To = 1.0,
            Duration = TimeSpan.FromSeconds(0.5),
            AutoReverse = false
        };
        /// <summary>
        /// Анимация пропадания фигуры
        /// </summary>
        public static readonly DoubleAnimation QuickDisappearance = new DoubleAnimation()
        {
            From = 1.0,
            To = 0.0,
            Duration = TimeSpan.FromSeconds(0.2),
            AutoReverse = false
        };
        /// <summary>
        /// Анимация появления фигуры
        /// </summary>
        public static readonly DoubleAnimation QuickAppearance = new DoubleAnimation()
        {
            From = 0.0,
            To = 1.0,
            Duration = TimeSpan.FromSeconds(0.2),
            AutoReverse = false
        };
    }
}
