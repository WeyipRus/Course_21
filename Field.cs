using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Course_21
{
    public class Field
    {
        /// <summary>
        /// Поле ячеек
        /// </summary>
        public Cell[,] cells;

        /// <summary>
        /// Размер поля
        /// </summary>
        public int fieldSize;

        /// <summary>
        /// Выбрана ли какая либо ячейка
        /// </summary>
        public bool IsSomeSelected = false;

        /// <summary>
        /// Ссылка на выбранную ячейку в массиве
        /// </summary>
        public Cell ChosenCell;

        /// <summary>
        /// Кол-во очков
        /// </summary>
        public int Points { get; set; }

        /// <summary>
        /// Кол-во очков
        /// </summary>
        public int MoveCount { get; set; }

        /// <summary>
        /// Количестко очков, полученных в результате последнего хода
        /// </summary>
        public int PointAdded { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <size - это размер поля>
        public Field(int size)
        {
            cells = new Cell[size, size];
            fieldSize = size;
            Points = 0;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    cells[x, y] = new Cell(x, y);
                }
            }
            StartNewGame();
        }
        public void StartNewGame() /// Начало новой игры
        {
            DefineFigures();
            CheckAllCells();
            DeleteMarkedCells();
            PutDownFigures();
            FillVoids();

            Points = 0;
            PointAdded = 0;
            MoveCount = 9;
        }

        /// <summary>
        /// Возвращает случайную фигурку
        /// </summary>
        /// <param name="random">Random который должен быть инициализирован вне цикла</param>
        private Figure GetRandomFigure(Random random)
        {
            Array values = Enum.GetValues(typeof(Figure));

            Figure randomFigure = (Figure)values.GetValue(random.Next(1, values.Length));
            return randomFigure;
        }

        /// <summary>
        /// Присвоение всем ячейкам случайной фигурки
        /// </summary>
        private void DefineFigures()
        {
            Random random = new Random();

            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    cells[x, y].figure = GetRandomFigure(random);
                }
            }
        }

        /// <summary>
        /// Выделение ячейки по ссылке
        /// </summary>     
        private void SelectCell(Cell cell)
        {
            cell.Select();
            IsSomeSelected = true;
            ChosenCell = cell;
        }

        /// <summary>
        /// Выделение ячейки по указанным координатам
        /// </summary>
        public void SelectCell(int x, int y)
        {
            SelectCell(cells[x, y]);
        }

        /// <summary>
        /// Снятие выделения с выбранной в данный момент ячейки
        /// </summary>
        public void UnselectCell()
        {
            ChosenCell.UnSelect();
            IsSomeSelected = false;
            ChosenCell = null;
        }

        /// <summary>
        /// Обмен фигурами двуг ячеек
        /// </summary>
        public void SwapCells(Cell firstCell, Cell secondCell)
        {
            Figure buffer = firstCell.figure;

            firstCell.figure = secondCell.figure;
            secondCell.figure = buffer;
        }

        /// <summary>
        /// Помечает подходящие ячейки на удаление
        /// </summary>
        public void CheckAllCells()
        {
            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    MatchCheckDown(cells[x, y]);
                    MatchCheckRight(cells[x, y]);
                }
            }
        }

        /// <summary>
        /// Проверка совпадений ячеек внизу
        /// </summary>
        private void MatchCheckDown(Cell cell)
        {
            int X = cell.X;
            int Y = cell.Y;

            Figure figure = cell.figure;

            int count = 0;
            int i = Y;

            while (true)
            {

                if (i <= (fieldSize - 1) && cells[X, i].figure == figure)
                {
                    count++;
                    i++;
                }
                else
                {
                    break;
                }
            }

            if (count >= 3)
            {
                for (int j = Y; j < i; j++)
                {
                    cells[X, j].IsMarkedForDeletion = true;
                    cells[X, j].IsChanged = true;
                }
            }

            Random random = new Random();
           
        }

        /// <summary>
        /// Проверка совпадений ячеек справа
        /// </summary>
        private void MatchCheckRight(Cell cell)
        {
            int X = cell.X;
            int Y = cell.Y;

            Figure figure = cell.figure;

            int count = 0;
            int i = X;

            while (true)
            {

                if (i <= (fieldSize - 1) && cells[i, Y].figure == figure)
                {
                    count++;
                    i++;
                }
                else
                {
                    break;
                }
            }

            if (count >= 3)
            {
                for (int j = X; j < i; j++)
                {
                    cells[j, Y].IsMarkedForDeletion = true;
                    cells[j, Y].IsChanged = true;
                }
            }
        }
        /// <summary>
        /// Удалить помеченные на удаление ячейки
        /// </summary>
        public void DeleteMarkedCells()
        {
            foreach (Cell cell in cells)
            {
                DeleteCell(cell);
            }
        }
        /// <summary>
        /// Очищает ячейку если она помечена на удаление
        /// </summary>
        private void DeleteCell(Cell cell)
        {
            if (cell.IsMarkedForDeletion)
            {
                cell.IsMarkedForDeletion = false;
                Points += DefineCellPoints(cell);
                PointAdded += DefineCellPoints(cell);
                cell.figure = Figure.Empty;
            }
        }
        /// <summary>
        /// Определить стоимость фигурки ячейки
        /// </summary>
        public int DefineCellPoints(Cell cell)
        {
            switch (cell.figure)
            {
                case Figure.Empty:
                    return 0;
                case Figure.Shell1:
                    return 5;
                case Figure.Shell2:
                    return 10;
                case Figure.Shell3:
                    return 15;
                case Figure.Shell4:
                    return 20;
                case Figure.Shell5:
                    return 25;
                case Figure.Shell6:
                    return 30;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Сброс счётчика полученных очков
        /// </summary>
        public void ResetCounter()
        {
            PointAdded = 0;
        }

        /// <summary>
        /// Поднимает пустые ячейки вверх
        /// </summary>
        private void PutDownFigures()
        {
            for (int i = 1; i < fieldSize; i++)
            {
                for (int x = 0; x < fieldSize; x++)
                {
                    for (int y = 1; y < fieldSize; y++)
                    {
                        if (cells[x, y].figure == Figure.Empty && cells[x, y - 1].figure != Figure.Empty)
                        {
                            SwapCells(cells[x, y], cells[x, y - 1]);

                            cells[x, y].IsChanged = true;
                            cells[x, y - 1].IsChanged = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Опустить фигуры вниз один раз
        /// </summary>
        public void PutDownFiguresOnes()
        {
            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 1; y < fieldSize; y++)
                {
                    if (cells[x, y].figure == Figure.Empty && cells[x, y - 1].figure != Figure.Empty)
                    {
                        SwapCells(cells[x, y], cells[x, y - 1]);

                        cells[x, y].IsChanged = true;
                    }
                }
            }

        }

        /// <summary>
        /// Создаёт новые фигурки на месте пустых
        /// </summary>
        public void MakeNewFigures()
        {
            Random random = new Random();

            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    if (cells[x, y].figure == Figure.Empty)
                    {
                        cells[x, y].figure = GetRandomFigure(random);
                        cells[x, y].IsChanged = true;
                    }
                }
            }
        }

        /// <summary>
        /// Есть ли пустые ячейки
        /// </summary>
        public bool HaveEmptyFigeres()
        {
            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    if (cells[x, y].figure == Figure.Empty)
                    {                       
                        return true;
                        
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Заполняет и опускает ячейки пока не останется совпадений
        /// </summary>
        private void FillVoids()
        {
            while (HaveEmptyFigeres() )
            {
                MakeNewFigures();
                CheckAllCells();
                DeleteMarkedCells();
                PutDownFigures();
            }
            //if (MoveCount <= 0) 
            //{
            //    ScoreList.add("Player " + Points);
            //    Visualizer.NewGame();
            //}
        }

        /// <summary>
        /// Проверяет является ли ячейка по заданным координатам соседом выбранной ячейки
        /// </summary>
        public bool IsNeighbors(int x, int y)
        {
            bool left = true;
            bool right = true;
            bool up = true;
            bool down = true;

            //Left
            if (x - 1 >= 0)
            {
                if (x - 1 != ChosenCell.X) left = false;
            }
            else left = false;
            //Right
            if (x + 1 <= 9)
            {
                if (x + 1 != ChosenCell.X) right = false;
            }
            else right = false;
            //Up
            if (y - 1 >= 0)
            {
                if (y - 1 != ChosenCell.Y) up = false;
            }
            else up = false;
            //Down
            if (y + 1 <= 9)
            {
                if (y + 1 != ChosenCell.Y) down = false;
            }
            else down = false;

            if ((up && x == ChosenCell.X) || (down && x == ChosenCell.X) || (left && y == ChosenCell.Y) || (right && y == ChosenCell.Y))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Проверка, совпают ли координаты с координатами выбранной ячейки
        /// </summary>
        public bool IsSamePlase(int x, int y)
        {
            if (ChosenCell.X == x && ChosenCell.Y == y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Есть ли комбинации фигур после хода
        /// </summary>
        public bool IsFalseMove()
        {
            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    MatchCheckDown(cells[x, y]);
                    MatchCheckRight(cells[x, y]);
                }
            }

            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    if (cells[x, y].IsMarkedForDeletion) return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Есть ли ячейки, помеченные как изменённые
        /// </summary>
        public bool HasChangedCells()
        {
            for (int x = 0; x < fieldSize; x++)
            {
                for (int y = 0; y < fieldSize; y++)
                {
                    if (cells[x, y].IsChanged == true)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
