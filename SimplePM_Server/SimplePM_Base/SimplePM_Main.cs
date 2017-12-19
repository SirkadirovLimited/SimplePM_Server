using System;

namespace SimplePM_Server
{

    /*!
     * \brief
     * Основной класс, метод Main которого инициализирует объект
     * базового класса сервера проверки пользовательских решений
     * и вызывает метод инициализации сервера.
     */
    
    class SimplePM_Main
    {

        public static void Main(string[] args)
        {
            
            //Инициализируем объект типа SimplePM_Worker
            //и вызываем метод запуска сервера
            new SimplePM_Worker().Run(args);

        }

    }

}
