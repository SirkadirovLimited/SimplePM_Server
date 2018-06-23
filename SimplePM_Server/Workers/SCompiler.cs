/*
 * ███████╗██╗███╗   ███╗██████╗ ██╗     ███████╗██████╗ ███╗   ███╗
 * ██╔════╝██║████╗ ████║██╔══██╗██║     ██╔════╝██╔══██╗████╗ ████║
 * ███████╗██║██╔████╔██║██████╔╝██║     █████╗  ██████╔╝██╔████╔██║
 * ╚════██║██║██║╚██╔╝██║██╔═══╝ ██║     ██╔══╝  ██╔═══╝ ██║╚██╔╝██║
 * ███████║██║██║ ╚═╝ ██║██║     ███████╗███████╗██║     ██║ ╚═╝ ██║
 * ╚══════╝╚═╝╚═╝     ╚═╝╚═╝     ╚══════╝╚══════╝╚═╝     ╚═╝     ╚═╝
 *
 * SimplePM Server
 * A part of SimplePM programming contests management system.
 *
 * Copyright 2017 Yurij Kadirov
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 * Visit website for more details: https://spm.sirkadirov.com/
 */

using System.IO;
using CompilerBase;

namespace SimplePM_Server
{

    /*
     * Класс компиляции  пользовательских  решений
     * задач по программированию. Функции в классе
     * вызывается функциями класса-официанта.
     */

    internal class SCompiler
    {

        private readonly string _submissionId; // Идентификатор запроса
        private readonly string _fileLocation; // Полный путь к файлу и его расширение
        private dynamic _compilerConfig;
        private readonly ICompilerPlugin _compilerPlugin; // Список загруженных модулей компиляторв
        
        public SCompiler(
            ref dynamic _compilerConfig,
            ref ICompilerPlugin _compilerPlugin,
            string submissionId,
            string fileLocation
        )
        {
            
            // Проверяем путь к исходному коду на ошибки
            if (string.IsNullOrEmpty(fileLocation) || string.IsNullOrWhiteSpace(fileLocation) || !File.Exists(fileLocation))
                throw new FileNotFoundException("File not found!", fileLocation);

            /*
             * Присваиваем глобальным для класса переменным
             * значения локальных  переменных  конструктора
             * класса.
             */
            this._compilerConfig = _compilerConfig;
            this._compilerPlugin = _compilerPlugin;
            _submissionId = submissionId;
            _fileLocation = fileLocation;

        }
        
        public static ICompilerPlugin FindCompilerPlugin(
            string pluginName
        )
        {

            /*
             * Производим поиск искомого плагина
             * в списке доступных модулей компиляции.
             */
            foreach (var plugin in SWorker._compilerPlugins)
                if (plugin.PluginName == pluginName)
                    return plugin;

            /*
             * Если по запросу  ничего не
             * найдено - возвращаем null.
             */
            return null;

        }

        /*
         * Метод производит поиск искомой
         * конфигурации модуля компиляции
         * пользовательских решений задач
         * по программированию и передаёт
         * ссылку на эту конфигурацию.
         *
         * Поиск специфической конфигурации
         * происходит   по   названию языка
         * программирования.
         */
        
        public static dynamic GetCompilerConfig(
            ref dynamic _compilerConfigs,
            string languageName
        )
        {

            /*
             * Производим перебор всех конфигураций компиляторов
             */
            foreach (var compilerConfig in _compilerConfigs)
            {

                /*
                 * Если текущая конфигурация является
                 * искомой, возвращаем ссылку на неё.
                 */
                if ((string)compilerConfig.language_name == languageName)
                    return compilerConfig;
                
                /*
                 * Иначе продолжаем поиск.
                 */

            }

            /*
             * Если соответствующий модуль компиляции
             * не найден, возвращаем значение null.
             */
            return null;

        }

        /*
         * Функция, которая по enum-у выбирает и
         * запускает определённый компилятор, а также
         * возвращает результат компиляции.
         */
        public CompilerResult ChooseCompilerAndRun()
        {
            
            /*
             * Выполняем   необходимые   действия  в
             * случае обнаружения нулевого возврата.
             */
            if (_compilerPlugin == null)
            {

                /*
                 * Возвращаем результат выполнения
                 * компиляции    пользовательского
                 * решения  поставленной   задачи,
                 * который сигнализирует о наличии
                 * ошибки,  которая  возникла  при
                 * попытке   поиска   необходимого
                 * модуля компиляции  для  данного
                 * скриптового  языка   или  языка
                 * программирования.
                 */
                return new CompilerResult
                {
                    HasErrors = true, // Хьюстон, у нас проблема!
                    CompilerMessage = "Language not supported by SimplePM!"
                };

            }

            /*
             * Возвращаем   ссылку  на  объект,
             * содержащий результаты компиляции
             * пользовательского        решения
             * поставленной задачи.
             */
            return _compilerPlugin.StartCompiler(
                ref _compilerConfig,
                _submissionId,
                _fileLocation
            );

        }

    }

}
