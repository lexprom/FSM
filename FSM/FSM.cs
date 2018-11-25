using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace FSM
{
        public class FiniteState
        {
            public static List<Tuple<FiniteState, string>> numbers = new List<Tuple<FiniteState, string>>();
            private List<string> states;
            private List<string> startState;
            private List<string> finishState;
            private List<string> alphabet;
            private List<string> currentState;
            private List<string[]> files;
            private List<string[]> tempTransitions;
            private Dictionary<string, string[]> transitions;
            private HashSet<string> tempStates;
            private readonly string stopSymbol;
 
        public FiniteState(string filePath, bool flag)
        {
            if (flag)
            {
                stopSymbol = " ";
            }
            else
            {
                stopSymbol = "*";
            }

            states = new List<string>();
            startState = new List<string>();
            finishState = new List<string>();
            currentState = new List<string>();
            files = new List<string[]>();
            tempTransitions = new List<string[]>();
            transitions = new Dictionary<string, string[]>();
            tempStates = new HashSet<string>();
            alphabet = new List<string>();

            //Информация из файла заносится в DataFile, который состоит из массивов
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    files.Add(line.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
                }
            }


            //Первый элемент списка file содержит множество состояний
            files[0].ToList().ForEach(item => states.Add(item));

            //Второй элемент списка file содержит алфавит
            files[1].ToList().ForEach(item => alphabet.Add(item));

            //Третий элемент списка file содержит начальные состояния
            files[2].ToList().ForEach(item => startState.Add(item));

            //Четвертый элемент списка file содержит конечные состояния
            files[3].ToList().ForEach(item => finishState.Add(item));

            //После 4 строки идут переходы автомата
            for (int i = 4; i < files.Count; i++)
            {
                tempTransitions.Add(files[i]);
            }

            //С помощью цикла проходимся по алфавиту и записываем в словарь Transitions Key - символ алфавита и Value - переходы 
            //для символа из алфавита
            for (int i = 0; i < alphabet.Count; i++)
            {
                transitions.Add(alphabet[i].ToString(), tempTransitions[i]);
            }

            //Проверка на то, что указанные начальные или конечные состояния входят в множество состояний
            if (!ContainsList(startState, states) || !ContainsList(finishState, states))
            {
                throw new Exception("Указанные начальные или конечные состояния не входят в множество состояний.");
            }

            //Присваивание списку, который содержит текущие состояния, начальных состояний
            currentState = startState;
        }


        private void Transition(List<string> currentState, string symbol)
        {
            tempStates = new HashSet<string>();
            foreach (var item in currentState)
            {
                var valueIndex = states.IndexOf(item);
                foreach (var item2 in transitions[symbol][valueIndex].Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    tempStates.Add(item2);
                }
            }
            this.currentState = tempStates.ToList();
        }

            // Метод для подсчёта максимальной подстроки
            // k - позиция, с которой начнётся перебор входной строки
            public Tuple<bool, int> MaxString(string input, int k)
            {
                bool result = false;
                int m = 0;


                if (startState.Intersect(finishState).Count() > 0)
                {
                    result = true;
                }


                for (int i = k; i < input.Length; i++)
                {
                    // Проверка символов на вхождение в алфавит
                    if (alphabet.Contains(input[i].ToString()))
                    {
                        Console.WriteLine(input[i].ToString());
                        Transition(currentState, input[i].ToString());
                        //m присваиваем длину найденной подстроки. 
                        if (currentState.Intersect(finishState).Count() > 0)
                        {
                            result = true;
                            m = i - k + 1;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                return new Tuple<bool, int>(result, m);
            }


        // Метод для подсчёта максимальной подстроки
        //input - Входная строка
        //k - Позиция, с которой начнётся перебор входной строки
        //Возвращает Tuple, который содержит result - True или False в зависимости от того, 
        //найдена подстрока во входной строке или нет, а также m - длину этой строки
        public void MaxStringForNumber(string input, int k)
        {
            string output = "";
            string tempString = "";

            //Проход по входной строке
            for (int i = k; i < input.Length; i++)
            {

                //Проверка символов на вхождение в алфавит
                if (alphabet.Contains(input[i].ToString()))
                {
                    //Вызов функции перехода. Передаём текущие состояния автомата и символ
                    Transition(currentState, input[i].ToString());

                    //в промежуточную строку добавляем символ из входной строки
                    tempString += input[i];

                    //Если текущие состояния "достигли" конечных, то result присваиваем True, в строку output записываем найденную построку,
                    //а m присваиваем длину найденной подстроки. 
                    //И продолжаем цикл, пока не пройдём всю входную строку. 
                    if (ContainsList(currentState, finishState))
                    {
                        if (output.Length < tempString.Length)
                        {
                            output = tempString;
                        }
                    }
                    //
                    if (currentState.Contains(stopSymbol))
                    {
                        if (output.Length > 0)
                        {
                            numbers.Add(new Tuple<FiniteState, string>(this, output));
                        }

                        if (tempString.Length > 1)
                        {
                            i--;
                        }
                        tempString = "";
                        output = "";

                        currentState = startState;
                    }
                }
            }

            if (output.Length > 0)
            {
                numbers.Add(new Tuple<FiniteState, string>(this, output));
            }
        }

        // Метод для подсчёта вещественных чисел в строке
        //input - Входная строка
        //k - Позиция, с которой начнётся перебор входной строки
        //Возвращает Tuple, который содержит result - True или False в зависимости от того,
        //найдено хотя бы одно вещественное число или нет,
        // а также m - количество вещественных чисел
        public void CountNumbers(string input, int k)
        {
            string number = "";

            for (int i = k; i < input.Length; i++)
            {
                if (alphabet.Contains(input[i].ToString()))
                {
                    number += input[i];
                }
                else
                {
                    if (!String.IsNullOrEmpty(number) || !String.IsNullOrWhiteSpace(number))
                    {
                        MaxStringForNumber(number, 0);
                    }
                    currentState = startState;
                    number = "";
                }
            }

            MaxStringForNumber(number, 0);

            foreach (var item in numbers)
            {
                Console.WriteLine(item);
            }

            currentState = startState;
        }

        // если пересечение двух множеств не пустое, тогда true иначе false.
        public bool ContainsList(IEnumerable<string> first, IEnumerable<string> second)
        {
            if (first.Intersect(second).Count() > 0)
            {
                return true;
            }
            return false;
        }
    }
}

