using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Threading;

namespace EasyTestGeekBrains
{
    
    public partial class MainForm : Form
    {
        MySqlConnection connectionBD;
        bool waitingUser = false;
        string connString;
        string idTest;
        string logText;
        int countBDCorrectQuestion = 0;
        int countTest = 0;
        List<Question> questions;
        List<string> combinationMask;
        List<Account> accounts;
        Account currentAccount;
        IWebDriver webDriver;
        List<string> modesSolution = new List<string>() { "Режим решения: 1\nАвтоматическое решение, вмешиваться не рекомендуется.",
            "Режим решения: 2\nВопросы с известными правильными отвветами отвечаются автоматически, неизвестные выбирает пользователь. В случае правильного выбора ответ запоминается." };

        public string LogText { get => logText; set { logText = value; label1.Invoke(new Action (() => label1.Text = logText)); this.Invoke(new Action (() => this.Update())); } }

        public Account CurrentAccount 
        { 
            get => currentAccount; 
            set 
            { 
                currentAccount = value;
                counterAccountLbl.Text = "Аккаунт № "+ accounts.IndexOf(value).ToString();
                loginTb.Text = value.Login; 
                passwordTb.Text = value.Password; 
            } 
        }

        public bool WaitingUser 
        { 
            get => waitingUser;
            set
            { 
                waitingUser = value;
                rememberAnswersBtn.Invoke(new Action(() => rememberAnswersBtn.Enabled = value));
            }
        }

        public MainForm()
        {
            InitializeComponent();
            questions = new List<Question>();
            combinationMask = new List<string>();
            accounts = new List<Account>();
            modeSolutionLbl.Text = modesSolution[0];
            modeSolutionLbl.Tag = 0;
        }

        private void connectionBtn_Click(object sender, EventArgs e)
        {
            if (connectionBD == null || (connectionBD.State != ConnectionState.Open && connectionBD.State != ConnectionState.Connecting)) 
            {
                try
                {
                    connString = "В целях конфиденциальности строка скрыта";
                    connectionBD = new MySqlConnection(connString);
                    connectionBD.Open();
                    connectionBtn.Text = "Подключение установлено! Разорвать.";
                    connectionBtn.BackColor = Color.GreenYellow;
                    GetAccounts();
                }
                catch (Exception ex)
                {
                    connectionBtn.Text = "Подключение не установлено!";
                    connectionBtn.BackColor = Color.Red;
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                try
                {
                    connectionBD.Close();
                    connectionBtn.Text = "Подключение разорвано!";
                    connectionBtn.BackColor = Color.Bisque;
                    nextAccount.Visible = false;
                }
                catch (Exception ex)
                {
                    connectionBtn.Text = "Подключение не установлено!";
                    connectionBtn.BackColor = Color.Red;
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (webDriver != null)
            {
                webDriver.Quit();
            }
        }

        private void checkTestBtn_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(testUrlTb.Text))// Проверка введена ли ссылка на тест
            {
                // Проверка есть ли открытое подключение, если нет открыть
                if (connectionBD == null || (connectionBD.State != ConnectionState.Open && connectionBD.State != ConnectionState.Connecting))
                {
                    this.connectionBtn_Click(sender, e);
                    if (connectionBD.State != ConnectionState.Open)
                    {
                        return;
                    }
                }
                MessageBox.Show(GetInfoTest());// Получение информации о тесте
            }
            else
            {
                MessageBox.Show("Требуется ввести ссылку на тест!");
            }

            //string query = "INSERT INTO `questions` (`idQuestion`, `textQuestion`) VALUES (NULL, 'hello');";
            //MySqlCommand command = new MySqlCommand(query, connectionBD);
            //command.ExecuteNonQuery();
        }

        
        private string GetInfoTest()// Получение информации о тесте
        {
            int idTest = 0;
            string nameTest = "", resolved = "", resolvedCorrect = "";
            string query = $"SELECT * FROM `tests` WHERE `nameTest` = '{testUrlTb.Text}'";
            MySqlCommand command = new MySqlCommand(query, connectionBD);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        idTest = reader.GetInt32(0);
                        nameTest = reader["nameTest"].ToString();
                        resolved = reader["resolved"].ToString();
                        resolvedCorrect = reader["resolvedCorrect"].ToString();
                    }
                }
            }

            query = $"SELECT COUNT(*) FROM attempts AS A INNER JOIN questions AS Q ON A.idQuestion = Q.idQuestion " +
                $"WHERE Q.idTest = {idTest} AND A.correctly = 1";
            command = new MySqlCommand(query, connectionBD);
            string correctAttempts = command.ExecuteScalar().ToString();

                string result = $"Название теста: {nameTest}\n" +
                $"Тест пройден: {resolved} раз\n" +
                $"Тест пройден верно: {resolvedCorrect} раз\n" +
                $"Верных ответов для теста: {correctAttempts}";
            return result;
        }

        private void solveTest_Click(object sender, EventArgs e) // Решить тест
        {
            Task.Run(() => SolveTest());
           //SolveTest();
        }
        private void SolveTest ()
        {
            questions.Clear();
            countBDCorrectQuestion = 0;
            if (String.IsNullOrEmpty(loginTb.Text) || String.IsNullOrEmpty(passwordTb.Text) || String.IsNullOrEmpty(testUrlTb.Text))// Заполнены ли поля логина, пароля, ссылки
            {
                MessageBox.Show("Поля логина, пароля, ссылки на тест должны быть заполнены актуальными значениями!");
                return;
            }
            if (!TryAuthorization())
            {
                MessageBox.Show("Не удалось авторизоваться! \nВозможно указаны некорректные данные.");
                return;
            }
            idTest = FixingTest(testUrlTb.Text);// Получили ид решаемого теста
            if(idTest == "noIdTest")
            {
                return;
            }
            webDriver.Navigate().GoToUrl(testUrlTb.Text);// Перешли на тест
            if (!IsPageCanStartTest())// Если кнопка для старта/продолжения теста отсутствует
            {
                return;
            }
            LogText += "Тест начат решаться\n";
            Thread.Sleep(5000);
            while (true)
            {
                if (IsPageQuestion())
                {
                    ResolveQuestion();
                }
                else if (IsPageFailedQuestions())
                {
                    MemorizeQuestions();
                    break;
                }
                else if(IsPageSuccessfully())
                {
                    TestSolved(idTest, IsPageSuccessfully());
                }
            }

            LogText += $"Решение теста закончено\nПравильных ответов в Б.Д найдено: {countBDCorrectQuestion}";
        }
        private void MemorizeQuestions()// Запомнить новые вопросы в Б.Д
        {
            NoticeQuestions();
            int countQuestion = 0;
            int countCorrectQuestion = 0;
            LogText += "\n Запоминание вопросов:\n";
            foreach (Question question in questions)
            {
                try
                {
                    FixingQuestion(question.IdQuestion, question.TextQuestion);
                    int correct = 0;
                    if(question.Correctly)
                    {
                        countCorrectQuestion++;
                        correct = 1;
                    }
                    string query = $"INSERT INTO `attempts` (`idAttempt`, `idQuestion`, `correctly`) VALUES (NULL, '{question.IdQuestion}', {correct.ToString()}); SELECT @@IDENTITY;";
                    MySqlCommand command = new MySqlCommand(query, connectionBD);
                    string attemptId = command.ExecuteScalar().ToString();
                    LogText += $"\nПопытка: {attemptId}\nВопрос: {question.IdQuestion} | {question.TextQuestion} \nПравильно отвечен: {correct}\n";
                    for(int answerIndex = 0; answerIndex < question.IdAnswers.Count; answerIndex++)
                    {
                        LogText += $"Ответ: {question.IdAnswers[answerIndex]} | {question.TextAnswers[answerIndex]}\n";
                        FixingAnswer(question.IdAnswers[answerIndex], question.TextAnswers[answerIndex]);
                        query = $"INSERT INTO `AnswerAttempts` (`idAnswer`, `idAttempts`, `idAnswerAttempts`) VALUES ('{question.IdAnswers[answerIndex]}', '{attemptId}', NULL);";
                        command = new MySqlCommand(query, connectionBD);
                        command.ExecuteNonQuery();
                    }
                    countQuestion++;
                    LogText += "Запоминание ответа выполнено успешно!\n";
                }
                catch (Exception ex)
                {
                    LogText += $"Произошла ошибка при попытке запоминания вопроса:{question.TextQuestion} в БД: {ex.Message}\n";
                }
            }
            TestSolved(idTest, IsPageSuccessfully());
            LogText += $"\nЗапоминание ответов выполнено: {countQuestion} раз\n" +
                $"Правильных ответов без учета ранее известных было совершено: {countCorrectQuestion}\n";
        }
        private void NoticeQuestions()// Отметить неверные вопросы
        {
            List<IWebElement> failedQuestionsElements = webDriver.FindElement(By.ClassName("failed-questions__wrapper")).FindElements(By.ClassName("failed-questions__item")).ToList();
            foreach (IWebElement failedQuestion in failedQuestionsElements)
            {
                foreach (Question question in questions)
                {
                    try
                    {
                        if (question.TextQuestion == failedQuestion.FindElement(By.ClassName("failed-questions__item-title")).FindElement(By.TagName("p")).Text)
                        {
                            question.Correctly = false;
                            break;
                        }
                    }
                    catch
                    {
                        if (question.TextQuestion == failedQuestion.FindElement(By.ClassName("failed-questions__item-title")).Text)
                        {
                            question.Correctly = false;
                            break;
                        }
                    }
                }
            }
        }
        private bool IsPageFailedQuestions()
        {
            try
            {
                webDriver.FindElement(By.ClassName("failed-questions"));
                return true;
            }
            catch
            {
                LogText += "Страница не является failed-questions или произошла другая ошибка!\n";
                return false;
            }
        }
        private bool IsPageSuccessfully()
        {
            try
            {
                webDriver.FindElement(By.ClassName("content__status_success"));
                return true;
            }
            catch
            {
                LogText += "Страница не является content__status_success или произошла другая ошибка!\n";
                return false;
            }
        }
        private bool IsPageQuestion ()
        {
            try
            {
                webDriver.FindElement(By.Id("questions-wrapper"));
                return true;
            }
            catch
            {
                LogText += "Страница не является вопросом(возможно это страница неправильных ответов) или произошла другая ошибка!\n";
                return false;
            }
        }
        private void ResolveQuestion()
        {
            string idQuestion = webDriver.FindElement(By.Id("questions-wrapper")).FindElement(By.ClassName("text-center")).GetAttribute("data-question-id");// Получили ид вопроса
            string textQuestion = webDriver.FindElement(By.Id("questions-wrapper")).FindElement(By.TagName("h3")).Text;
            string idAttempt = HasRightAnswers(idQuestion);// Ищем правильные ответы для вопроса в базе
            if (idAttempt != "no")// Если в базе есть правильные ответы
            {
                countBDCorrectQuestion++;
                LogText += $"\nДля вопроса: {textQuestion} найдена правильная попытка с id: {idAttempt}";
                string query = $"SELECT AA.idAnswer , A.textAnswer FROM `AnswerAttempts` AS AA JOIN `answer` AS A ON AA.idAnswer = A.idAnswer WHERE `idAttempts` = '{idAttempt}'";
                MySqlCommand command = new MySqlCommand(query, connectionBD);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                webDriver.FindElement(By.XPath($".//*[@data-id='{reader.GetString(0)}']")).Click();
                                LogText += $" правильный ответ: {reader.GetString(1)}\n";
                            }
                            catch
                            {
                                LogText += $"(Правильный ответ)Произошла ошибка при поиске на странице ответа: {reader.GetString(1)}, на вопрос: {textQuestion}\n";
                            }
                        }
                    }
                }
            }
            else//Если в базе нету правильных ответов
            {
                if (Convert.ToInt32(modeSolutionLbl.Tag) == 0)// Если режим решения автоматический
                {
                    List<Attempt> theoreticalAttempts = GetTheoreticalAttempts();// Получение теоретических попыток 
                    List<Attempt> incorrectAttempts = GetIncorrectAttempts(idQuestion);// Получение неверных попыток
                    GetDifferenceAttempts(theoreticalAttempts, incorrectAttempts);// Удалили неправильные попытки из теоретических
                    Random randomAttempt = new Random();
                    List<string> idAnswers = theoreticalAttempts[randomAttempt.Next(theoreticalAttempts.Count - 1)].IdAnswers;// Получили ответы из случайной теоретической попытки
                    List<string> textAnswers = new List<string>();
                    foreach (string idAnswer in idAnswers)
                    {
                        webDriver.FindElement(By.XPath($".//*[@data-id='{idAnswer}']")).Click();
                        textAnswers.Add(webDriver.FindElement(By.XPath($".//*[@data-id='{idAnswer}']/..")).FindElement(By.TagName("code")).Text);
                    }
                    questions.Add(new Question(textQuestion, idQuestion, idAnswers, textAnswers, true));
                }
                else
                {
                    List<Attempt> incorrectAttempts = GetIncorrectAttempts(idQuestion);// Получение неверных попыток
                    ShowIncorrectAttempts(incorrectAttempts);
                    WaitingUser = true;
                    while(WaitingUser)
                    {

                    }
                }
            }
            webDriver.FindElement(By.XPath($".//*[@id='answer-button']")).Click();
            Thread.Sleep(5000);
        }
        private bool IsPageCanStartTest()
        {
            try
            {
                webDriver.FindElement(By.ClassName("test-control__button")).Click();
                return true;
            }
            catch
            {
                LogText += "На странице отсутствует кнопка для решения теста или произошла другая ошибка!\n";
                return false;
            }
        }
        private void GetDifferenceAttempts(List<Attempt> theoreticalAttempts, List<Attempt> incorrectAttempts)
        {
            List<Attempt> theoreticalAttemptsCopy = new List<Attempt>(theoreticalAttempts);
            foreach (Attempt theoretAttempt in theoreticalAttempts )
            {
                foreach (Attempt incorAttempt in incorrectAttempts)
                {
                    if (new HashSet<string>(theoretAttempt.IdAnswers).SetEquals(incorAttempt.IdAnswers))
                    {
                        theoreticalAttemptsCopy.Remove(theoretAttempt);
                        break;
                    }
                }
            }
            theoreticalAttempts = theoreticalAttemptsCopy;
        }
        private List<Attempt> GetTheoreticalAttempts()// Получение всех возможных вариантов ответа на текущий вопрос
        {
            List<Attempt> theoreticalAttempts = new List<Attempt>();// Список теоретически возможных попыток
            string typeList = webDriver.FindElement(By.Id("questions-wrapper")).FindElement(By.TagName("input")).GetAttribute("type");
            List<IWebElement> answersElements = webDriver.FindElement(By.Id("questions-wrapper")).FindElements(By.TagName("input")).ToList();
            if (typeList == "checkbox")// Если можно выбрать более одного ответа
            {
                List<string> answers = new List<string>();// Список ответов со страницы
                foreach (IWebElement webElement in answersElements)
                {
                    answers.Add(webElement.GetAttribute("data-id"));
                }
                combinationMask.Clear();
                GenerateCombinationsMask(answers.Count, "");// Генерируем маски все возможных ответов
                int stringMaskIndex = 0;
                foreach (string stringMask in combinationMask)// На основе маски создаем ответ
                {
                    theoreticalAttempts.Add(new Attempt());
                    for (int charMaskIndex = 0; charMaskIndex < stringMask.Length; charMaskIndex++)
                    {
                        if (stringMask[charMaskIndex] == '1')
                        {
                            theoreticalAttempts[stringMaskIndex].IdAnswers.Add(answers[charMaskIndex]);
                        }
                    }
                    stringMaskIndex++;
                }
                return theoreticalAttempts;
            }
            else if (typeList == "radio")// Если можно выбрать только один ответ
            {
                foreach (IWebElement webElement in answersElements)
                {
                    theoreticalAttempts.Add(new Attempt(webElement.GetAttribute("data-id")));
                }
                return theoreticalAttempts;
            }
            return null;
        }
        private void GenerateCombinationsMask(int count, string combination)// Получение все возможных комбинаций в виде маски 01110
        {
            if(count == 0)
            {
                combinationMask.Add(combination);
            }
            else
            {
                GenerateCombinationsMask(count-1, combination + '1');
                GenerateCombinationsMask(count - 1, combination + '0');
            }
        }
        private List<Attempt> GetIncorrectAttempts(string idQuestion)// Получение всех неудачных попыток и ответов которые были использованы
        {
            List<Attempt> result = new List<Attempt>(); 
            List<string> incorrectAttemps = new List<string>();
            string query = $"SELECT idAttempt FROM `attempts` WHERE `idQuestion` = '{idQuestion}' AND `correctly` = 0";
            MySqlCommand command = new MySqlCommand(query, connectionBD);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        incorrectAttemps.Add(reader.GetString(0));// Все id неудачных попыток для текущего вопроса
                    }
                }
            }
            foreach(string incorrAttempt in incorrectAttemps)
            {
                result.Add(new Attempt());
                result[result.Count - 1].IdAttempt = incorrAttempt;
                query = $"SELECT `idAnswer` FROM `AnswerAttempts` WHERE `idAttempts` = '{incorrAttempt}'";
                command = new MySqlCommand(query, connectionBD);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            result[result.Count-1].IdAnswers.Add(reader.GetString(0));// Все id неправильных ответов для текущего вопроса в рамках одной попытки
                        }
                    }
                }
                foreach (string idAnswer in result[result.Count - 1].IdAnswers)
                {
                    query = $"SELECT `textAnswer` FROM `answer` WHERE `idAnswer` = '{idAnswer}'";
                    command = new MySqlCommand(query, connectionBD);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                result[result.Count - 1].TextAnswers.Add(reader.GetString(0));// Все text неправильных ответов для текущего вопроса в рамках одной попытки
                            }
                        }
                    }
                }
            }
            return result;
        }
        private string HasRightAnswers(string idQuestion)
        {
            string query = $"SELECT idAttempt FROM `attempts` WHERE `idQuestion` = '{idQuestion}' AND `correctly` = 1";
            MySqlCommand command = new MySqlCommand(query, connectionBD);
            var idAttempt = command.ExecuteScalar();
            if (idAttempt != null)
            {
                return idAttempt.ToString();
            }
            else
            {
                return "no";
            }
        }
        private string FixingQuestion(string idQuestion, string textQuestion)// Фиксируем вопрос, если есть уже такой вопрос с таким ид, то отправляем ид
        {
            string query = $"SELECT `idQuestion` FROM `questions` WHERE `idQuestion` = '{idQuestion}';";
            MySqlCommand command = new MySqlCommand(query, connectionBD);
            var idQuest = command.ExecuteScalar();
            if (idQuest != null)
            {
                return idQuest.ToString();
            }
            else
            {
                query = $"INSERT INTO `questions` (`idQuestion`, `textQuestion`, `idTest`) " +
                    $"VALUES ('{idQuestion}', '{textQuestion}', '{this.idTest}');";
                command = new MySqlCommand(query, connectionBD);
                command.ExecuteNonQuery();
                return FixingQuestion(idQuestion, textQuestion);
            }
        }
        private string FixingAnswer(string idAnswer, string textAnswer)// Фиксируем вопрос, если есть уже такой вопрос с таким ид, то отправляем ид
        {
            string query = $"SELECT `idAnswer` FROM `answer` WHERE `idAnswer` = '{idAnswer}'";
            MySqlCommand command = new MySqlCommand(query, connectionBD);
            var idAnsw = command.ExecuteScalar();
            if (idAnsw != null)
            {
                return idAnsw.ToString();
            }
            else
            {
                query = $"INSERT INTO `answer` (`idAnswer`, `textAnswer`) VALUES ('{idAnswer}', @textAnswer);";
                command = new MySqlCommand(query, connectionBD);
                command.Parameters.AddWithValue("@textAnswer", textAnswer);
                command.ExecuteNonQuery();
                return FixingAnswer(idAnswer, textAnswer);
            }
        }
        private string FixingTest(string nameTest)// Фиксируем тест, если есть в бд, то получем id, если нет в бд, то записываем и получаем id
        {
            try
            {
                string query = $"SELECT `idTest` FROM `tests` WHERE `nameTest` = '{nameTest}'";
                MySqlCommand command = new MySqlCommand(query, connectionBD);
                var idTest = command.ExecuteScalar();
                if (idTest != null)
                {
                    return idTest.ToString();
                }
                else
                {
                    query = $"INSERT INTO `tests` (`idTest`, `nameTest`, `resolved`, `resolvedCorrect`) VALUES (NULL, '{nameTest}', '0', '0');";
                    command = new MySqlCommand(query, connectionBD);
                    command.ExecuteNonQuery();
                    return FixingTest(nameTest);
                }
            }
            catch
            {
                MessageBox.Show("Ошибка при попытке фиксирования теста в Б.Д");
                return "noIdTest";
            }
        }
        private void TestSolved(string idTest, bool correct)
        {
            string query = $"SELECT `resolved` FROM `tests` WHERE `idTest` = {idTest}";
            MySqlCommand command = new MySqlCommand(query, connectionBD);
            var resolved = command.ExecuteScalar();
            if (resolved != null)
            {
                int resolvedCount = Convert.ToInt32(resolved);
                resolvedCount++;
                query = $"UPDATE `tests` SET `resolved`= {resolvedCount} WHERE `idTest` = {idTest}";
                command = new MySqlCommand(query, connectionBD);
                command.ExecuteNonQuery();
            }
            if (correct)
            {
                query = $"SELECT `resolvedCorrect` FROM `tests` WHERE `idTest` = {idTest}";
                command = new MySqlCommand(query, connectionBD);
                var resolvedCorrect = command.ExecuteScalar();
                if (resolvedCorrect != null)
                {
                    int resolvedCount = Convert.ToInt32(resolvedCorrect);
                    resolvedCount++;
                    query = $"UPDATE `tests` SET `resolvedCorrect`= {resolvedCount} WHERE `idTest` = {idTest}";
                    command = new MySqlCommand(query, connectionBD);
                    command.ExecuteNonQuery();
                }
            }
        }
        private bool TryAuthorization()
        {
            webDriver = new ChromeDriver();
            webDriver.Navigate().GoToUrl(@"https://gb.ru/login");
            IWebElement webElement = webDriver.FindElement(By.Id("user_email"));
            webElement.SendKeys(loginTb.Text);
            webElement = webDriver.FindElement(By.Id("user_password"));
            webElement.SendKeys(passwordTb.Text);
            webElement = webDriver.FindElement(By.ClassName("btn-success"));
            webElement.Click();
            try
            {
                webElement = webDriver.FindElement(By.ClassName("mn-avatar-icon__wrapper")).FindElement(By.ClassName("gb-icon"));
                return true;
            }
            catch
            {
                return false;
            }
        }
        private void GetAccounts()
        {
            try
            {
                int countAccounts = 0;
                accounts.Clear();
                string query = "SELECT * FROM `accounts`;";
                MySqlCommand command = new MySqlCommand(query, connectionBD);
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            accounts.Add(new Account(reader.GetString(1), reader.GetString(2)));
                            countAccounts++;
                        }
                    }
                }
                nextAccount.Visible = true;
                LogText += $"Получено аккаунтов: {countAccounts}";
                SelectNextAccount();
            }
            catch
            {
                LogText += "Неудалось получить аккаунты!\n";
            }
        }
        private void SelectNextAccount()
        {
            if (accounts.Count > 0)
            {
                if (accounts.IndexOf(CurrentAccount) == -1)
                {
                    CurrentAccount = accounts[0];
                }
                else
                {
                    if (accounts.IndexOf(CurrentAccount) < accounts.Count - 1)
                    {
                        CurrentAccount = accounts[accounts.IndexOf(CurrentAccount) + 1];
                    }
                    else
                    {
                        CurrentAccount = accounts[0];
                    }
                }
            }
        }
        private void nextAccount_Click(object sender, EventArgs e)
        {
            SelectNextAccount();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            TestSolved("1", false);
        }

        private void modeSolutionBtn_Click(object sender, EventArgs e)
        {
            if (modesSolution.Count() - 1 > Convert.ToInt32(modeSolutionLbl.Tag))
            {
                modeSolutionLbl.Text = modesSolution[Convert.ToInt32(modeSolutionLbl.Tag) + 1];
                modeSolutionLbl.Tag = Convert.ToInt32(modeSolutionLbl.Tag) + 1;
            }
            else
            {
                modeSolutionLbl.Text = modesSolution[0];
                modeSolutionLbl.Tag = 0;
            }
        }

        private void rememberAnswersBtn_Click(object sender, EventArgs e)// Запомнить ответы вручную выбранные
        {
            FindSelectedAnswers();
        }
        private void FindSelectedAnswers()
        {
            try
            {
                List<string> textAnswers = new List<string>();
                List<string> idAnswers = new List<string>();
                string idQuestion = webDriver.FindElement(By.Id("questions-wrapper")).FindElement(By.ClassName("text-center")).GetAttribute("data-question-id");// Получили ид вопроса
                string textQuestion = webDriver.FindElement(By.Id("questions-wrapper")).FindElement(By.TagName("h3")).Text;
                List<IWebElement> answersElements = webDriver.FindElement(By.ClassName("questions-list")).FindElements(By.ClassName("active")).ToList();
                if (answersElements.Count != 0)
                {
                    foreach (IWebElement answerElement in answersElements)
                    {
                        string idAnswer = answerElement.FindElement(By.TagName("input")).GetAttribute("data-id");
                        string textAnswer = webDriver.FindElement(By.XPath($".//*[@data-id='{idAnswer}']/..")).FindElement(By.TagName("code")).Text;
                        idAnswers.Add(idAnswer);
                        textAnswers.Add(textAnswer);
                        LogText += "\n" + idAnswer + "\n" + textAnswer;
                    }
                    DeleteDuplicateQuestion(idQuestion);// Удалили из локального списка вопросов, дубликат
                    questions.Add(new Question(textQuestion, idQuestion, idAnswers, textAnswers, true));
                    WaitingUser = false;
                }
            }
            catch
            {
                LogText += "На странице отсутствует выбранный ответ или произошла другая ошибка!\n";
            }
        }
        private void ShowIncorrectAttempts(List<Attempt> incorrectAttempts)
        {
            LogText += "\nНеверные попытки:\n";
            foreach (Attempt incorrAttempt in incorrectAttempts)
            {
                LogText += "Попытка:" + incorrAttempt.IdAttempt + "\nОтветы:\n";
                foreach(string incorrText in incorrAttempt.TextAnswers)
                {
                    LogText += incorrText + "\n";
                }
            }
        }
        private void DeleteDuplicateQuestion(string idQuestion)
        {
            Question duplicateQuestion = questions.Find((item) => item.IdQuestion == idQuestion);
            if (duplicateQuestion != null)
            {
                questions.Remove(duplicateQuestion);
            }
        }
    }

    public class Attempt
    {
        string idAttempt;
        List<string> idAnswers;
        List<string> textAnswers;

        public Attempt()
        {
            this.idAnswers = new List<string>();
            this.TextAnswers = new List<string>();
        }
        public Attempt(string answer)
        {
            this.idAnswers = new List<string>() { answer };
        }

        public List<string> IdAnswers { get => idAnswers; set => idAnswers = value; }
        public List<string> TextAnswers { get => textAnswers; set => textAnswers = value; }
        public string IdAttempt { get => idAttempt; set => idAttempt = value; }
    }
    public class Question
    {
        string textQuestion;
        string idQuestion;
        List<string> idAnswers;
        List<string> textAnswers;
        bool correctly;

        public Question(string textQuestion, string idQuestion, List<string> idAnswers, List<string> textAnswers, bool correctly)
        {
            TextQuestion = textQuestion;
            IdQuestion = idQuestion;
            IdAnswers = idAnswers;
            TextAnswers = textAnswers;
            Correctly = correctly;
        }
        public Question(string textQuestion, bool correctly)
        {
            TextQuestion = textQuestion;
            Correctly = correctly;
        }

        public string TextQuestion { get => textQuestion; set => textQuestion = value; }
        public string IdQuestion { get => idQuestion; set => idQuestion = value; }
        public List<string> IdAnswers { get => idAnswers; set => idAnswers = value; }
        public List<string> TextAnswers { get => textAnswers; set => textAnswers = value; }
        public bool Correctly { get => correctly; set => correctly = value; }
    }
    public class Account
    {
        string login;
        string password;

        public Account(string login, string password)
        {
            Login = login;
            Password = password;
        }

        public string Login { get => login; set => login = value; }
        public string Password { get => password; set => password = value; }
    }
}
