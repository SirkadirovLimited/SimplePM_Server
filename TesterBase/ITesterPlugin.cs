using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TesterBase
{

    /*!
     * \brief
     * Интерфейс, который предоставляет возможность
     * создания собственных модулей для подсистемы
     * тестирования пользовательских решений задач
     * по программированию.
     */

    public interface ITesterPlugin
    {

        ///////////////////////////////////////////////////
        /// Раздел описания важных констант
        ///////////////////////////////////////////////////

        string TesterPluginDisplayName { get; } //<! Отображаемое имя плагина подсистемы тестирования
        string TesterPluginAuthor { get; } //<! Имя автора плагина подсистемы тестирования
        string TesterPluginSupportUrl { get; } //<! Адрес сайта либо e-mail адрес технической поддержки
        string TesterPluginModeName { get; } //<! Наименование режима, поддержка которого реализуется плагином



    }

}
