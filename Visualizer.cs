using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Course_21
{
    public class Visualizer
    {

        /// <summary>
        /// Изображения
        /// </summary>
        private MTImage[,] Images;

        /// <summary>
        /// Кнопки
        /// </summary>
        private Button[,] Buttons;
        /// <summary>
        /// Графическое игровое поле
        /// </summary>
        private Grid FieldControl;
        /// <summary>
        /// Логической егровое поле
        /// </summary>
        public Field Field;
        /// <summary>
        /// Графический элемент, отображающий кол-во очков
        /// </summary>
        private Label Points;

        private Label PointsCounter;

        /// <summary>
        /// Графический элемент, отображающий кол-во ходов
        /// </summary>
        private Label MoveCountLabel;

        /// <summary>
        /// Графический элемент, отображающий таблицу лидеорв
        /// </summary>
        private ListBox ScoreList;

        /// <summary>
        /// Происходит ли анимация в данный момент
        /// </summary>
        private MTImage AnimatedImage;

        /// <summary>
        /// Происходит ли анимация в данный момент
        /// </summary>
        private bool IsAnimationGoing = false;




        public Visualizer(Grid FieldGrid, int size, Label pointsLabel, Label pointsCounterLabel, Label moveLabel, ListBox scoreListBox)
        {
            FieldControl = FieldGrid;
            Field = new Field(size);
            Points = pointsLabel;
            PointsCounter = pointsCounterLabel;
            MoveCountLabel = moveLabel;
            ScoreList = scoreListBox;
            Images = new MTImage[size, size];
            Buttons = new Button[size, size];

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    Images[x, y] = new MTImage()
                    {
                        X = x,
                        Y = y,
                        figure = Field.cells[x, y].figure
                    };
                    Buttons[x, y] = new Button()
                    {
                        Background = Brushes.Transparent

                    };
                    Buttons[x, y].Content = Images[x, y];
                    Buttons[x, y].Click += CellClick;
                }
            }

            DefineAllImages();
            PrepareGrid();
            UpdateMoveCount();
        }

        /// <summary>
        /// Обновление счётчика очков
        /// </summary>
        private void UpdatePointsLabel()
        {
            Points.Content = Field.Points;
            PointsCounter.Content = $"(+{Field.PointAdded})";
        }

        /// <summary>
        /// Обновление счётчика ходов
        /// </summary>
        private void UpdateMoveCount()
        {
            MoveCountLabel.Content = Field.MoveCount;
        }
        /// <summary>
        /// Присвоение ячейки соответствующего изображения
        /// </summary>
        /// <param name="cell">Логическая ячейка</param>
        /// <param name="image">Графическая ячейка</param>
        private void DefineImage(MTImage image)
        {
            int X = image.X;
            int Y = image.Y;

            if (Field.cells[X, Y].figure == Figure.Empty)
            {
                image.Source = null;            
            }
            else
            {
                BitmapImage BMI = new BitmapImage();
                BMI.BeginInit();
                BMI.UriSource = new Uri(GetFigurePath(Field.cells[X, Y].figure), UriKind.Relative);
                BMI.EndInit();
                image.Source = BMI;
            }

            Field.cells[X, Y].IsChanged = false;
        }

        /// <summary>
        /// Возвращает относительный путь к изображению соответствующей фигурки
        /// </summary>      
        private string GetFigurePath(Figure figure)
        {
            switch (figure)
            {
                case Figure.Shell1:
                    return "/Assets/Shell1.png";
                case Figure.Shell2:
                    return "/Assets/Shell2.png";
                case Figure.Shell3:
                    return "/Assets/Shell3.png";
                case Figure.Shell4:
                    return "/Assets/Shell4.png";
                case Figure.Shell5:
                    return "/Assets/Shell5.png";
                case Figure.Shell6:
                    return "/Assets/Shell6.png";
                default:
                    throw new Exception("Ссылка на объект не может быть получена");
            }
        }

        /// <summary>
        /// Подготовка графического игрового поля для дальнейшей работы с ним
        /// </summary>
        private void PrepareGrid()
        {
            //Разбиение сетки на столбцы и строки
            for (int i = 0; i < Field.fieldSize; i++)
            {
                FieldControl.ColumnDefinitions.Add(new ColumnDefinition());
                FieldControl.RowDefinitions.Add(new RowDefinition());
            }

            //Присвоение кнопок гриду
            for (int x = 0; x < Field.fieldSize; x++)
            {
                for (int y = 0; y < Field.fieldSize; y++)
                {
                    Grid.SetColumn(Buttons[x, y], x);
                    Grid.SetRow(Buttons[x, y], y);
                    Images[x, y].Stretch = Stretch.Uniform;
                    FieldControl.Children.Add(Buttons[x, y]);
                }
            }
        }

        /// <summary>
        /// Первый клик по ячейке
        /// </summary>
        private void FirstClick(int X, int Y)
        {
            //Логика
            Field.SelectCell(X, Y);
            Field.IsSomeSelected = true;

            //Анимация
            StartImageAnimation(Images[X, Y], Animations.Selection);
            AnimatedImage = Images[X, Y];

        }

        /// <summary>
        /// Начало анимации выбора в указанной ячейке
        /// </summary>
        /// <param name="image">Ячейка</param>
        private void StartImageAnimation(Image image, DoubleAnimation doubleAnimation)
        {
            image.BeginAnimation(UIElement.OpacityProperty, doubleAnimation);
        }

        /// <summary>
        /// Завершает анимацию выбранной ячейки
        /// </summary>      
        private void StopImageAnimation(Image image)
        {
            image.BeginAnimation(UIElement.OpacityProperty, null);
        }

        /// <summary>
        /// Плавно меняет два изображение местами и вызывает функцию обмена содержимого ячеек
        /// </summary>
        /// <param name="firstImage">Первое изображение</param>
        /// <param name="secondImage">Второе изображение</param>
        private async Task SwapImages(MTImage firstImage, MTImage secondImage)
        {
            StartImageAnimation(firstImage, Animations.Disappearance);
            StartImageAnimation(secondImage, Animations.Disappearance);
            await Task.Delay(500);

            int x1 = firstImage.X;
            int y1 = firstImage.Y;
            int x2 = secondImage.X;
            int y2 = secondImage.Y;

            Field.SwapCells(Field.cells[x1, y1], Field.cells[x2, y2]);
            Field.MoveCount = Field.MoveCount - 1;

            DefineImage(firstImage);
            DefineImage(secondImage);

            StartImageAnimation(firstImage, Animations.Appearance);
            StartImageAnimation(secondImage, Animations.Appearance);
            await Task.Delay(500);
        }

        private async Task SecondClick(int X, int Y)
        {

            UpdatePointsLabel();
            Field.ResetCounter();
            //Снятие выделения           
            Field.IsSomeSelected = false;

            await SwapImages(Images[X, Y], AnimatedImage);

            //Проверка на ложное нажатие
            if (Field.IsFalseMove())
            {
                await SwapImages(Images[X, Y], AnimatedImage);

                StopImageAnimation(AnimatedImage);
                StopImageAnimation(Images[X, Y]);

                Field.IsSomeSelected = false;
                AnimatedImage = null;

                return;
            }

            StopImageAnimation(AnimatedImage);
            StopImageAnimation(Images[X, Y]);

            Field.CheckAllCells();
            Field.DeleteMarkedCells();
            UpdatePointsLabel();
            UpdateMoveCount();
            await PutDownFigures();

            do
            {
                Field.MakeNewFigures();
                await DefineAllImagesAnim();

                Field.CheckAllCells();
                Field.DeleteMarkedCells();
                UpdatePointsLabel();
                await PutDownFigures();

            }
            while (Field.HaveEmptyFigeres());

            await DefineAllImagesAnim();

            UpdatePointsLabel();
            Field.ResetCounter();
            if (Field.MoveCount <= 0)
            {
                ScoreList.Items.Add($"Attempt {ScoreList.Items.Count + 1} - {Field.Points}");
                NewGame();
            }
            AnimatedImage = null;

        }

        /// <summary>
        /// Анимированно меняет все изменённые изображения на актуальные
        /// </summary>
        private async Task DefineAllImagesAnim()
        {
            for (int x = 0; x < Field.fieldSize; x++)
            {
                for (int y = 0; y < Field.fieldSize; y++)
                {
                    AnimatedDefineImage(Images[x, y]);
                }
            }
            await Task.Delay(500);
        }
        /// <summary>
        /// Анимированно меняет язображение на актуальное
        /// </summary>
        /// <param name="image">Изображение</param>
        private async void AnimatedDefineImage(MTImage image)
        {
            int X = image.X;
            int Y = image.Y;

            if (Field.cells[X, Y].figure == Figure.Empty)
            {
                StartImageAnimation(image, Animations.QuickDisappearance);
                await Task.Delay(200);

                image.Source = null;
                //ImageBehavior.SetAnimatedSource(image, null);

                StopImageAnimation(image);



                Field.cells[X, Y].IsChanged = false;
            }
            else if (Field.cells[X, Y].IsChanged)
            {
                StartImageAnimation(image, Animations.QuickDisappearance);
                await Task.Delay(200);

                BitmapImage BMI = new BitmapImage();
                BMI.BeginInit();
                BMI.UriSource = new Uri(GetFigurePath(Field.cells[X, Y].figure), UriKind.Relative);
                BMI.EndInit();
                image.Source = BMI;
               // ImageBehavior.SetAnimatedSource(image, BMI);
               // ImageBehavior.SetAnimationSpeedRatio(image, 0.2);

                StartImageAnimation(image, Animations.QuickAppearance);
                await Task.Delay(200);

                Field.cells[X, Y].IsChanged = false;
            }
            else
            {

            }
        }

        /// <summary>
        /// Проверка фигур на удаление
        /// </summary>
        /// <returns></returns>
        private async Task DeleteFigures()
        {
            Field.CheckAllCells();
            Field.DeleteMarkedCells();
            UpdatePointsLabel();
            await PutDownFigures();
        }

        /// <summary>
        /// Анимированно опустить фигуры вниз
        /// </summary>
        private async Task PutDownFigures()
        {
            for (int i = 1; i < Field.fieldSize; i++)
            {
                if (Field.HasChangedCells())
                {
                    await DefineAllImagesAnim();
                    Field.PutDownFiguresOnes();
                }
            }
        }

        /// <summary>
        /// Нажатие на графическую ячейку
        /// </summary>
        public async void CellClick(object sender, EventArgs e)
        {

            //Извлечение координат
            int x = Grid.GetColumn(sender as Button);
            int y = Grid.GetRow(sender as Button);

            IsAnimationGoing = true;

            //Первое нажатие
            if (Field.IsSomeSelected == false)
            {
                FirstClick(x, y);
            }
            //Второе нажатие
            else
            {
                //Нажатие на ту же ячейку
                if (Field.IsSamePlase(x, y))
                {
                    SameClick();
                }
                //Нажатие на соседнюю клетку
                else if (Field.IsNeighbors(x, y))
                {
                    await SecondClick(x, y);
                }
            }
            IsAnimationGoing = false;


        }

        /// <summary>
        /// Второе нажатие на ту же кнопку
        /// </summary>
        private void SameClick()
        {
            Field.UnselectCell();
            Field.IsSomeSelected = false;

            StopImageAnimation(AnimatedImage);
            AnimatedImage = null;
        }

        public void NewGame()
        {
            Field.StartNewGame();
            DefineAllImages();
            UpdatePointsLabel();
            UpdateMoveCount();
        }

        /// <summary>
        /// Присвоение всем ячейкам соответствующего изображения
        /// </summary>
        private void DefineAllImages()
        {
            for (int x = 0; x < Field.fieldSize; x++)
            {
                for (int y = 0; y < Field.fieldSize; y++)
                {
                    DefineImage(Images[x, y]);
                }
            }
        }
    }
}
